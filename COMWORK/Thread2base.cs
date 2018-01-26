using System;
using System.Windows.Forms;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.IO.Ports;

using System.Runtime.InteropServices;
using System.Globalization;
using System.Reflection;

//using System.Threading.Tasks; 

namespace Program
{
    //*****************************************************************************
    //                      ОСНОВНОЙ ПОТОК thread2  + TASK1 TASK2
    //*****************************************************************************
    public partial class ThreadCOM
  //  public partial class Form1 : Form
    {
        public Form1 pFORM;//УКАЗАТЕЛЬ НА ФОРМУ
    
       
        //static UInt16 ctwrite;//счетчик контроля посылок
        //static bool progVIPOLNENA, the_end, the_end2;

        //static uint TEK_ADRES=0;

        //UInt16 toPLC = 0x7FE;
        //UInt16 fromPLC =0x7FD;

        public ThreadCOM()//-------------- конструктор ---------------------
        {
            //задача уходит в бесконечный цикл
            Task task1 = Task.Factory.StartNew(() =>
            {
                try { while (!data.t2_closing) TASK_1(); }
                catch (Exception ex)
                { MessageBox.Show(String.Format("ОШИБКА TASK1 {0};", ex), "Фатальная Ошибка"); }
      
            });
            
            //задача уходит в бесконечный цикл
            Task task2 = Task.Factory.StartNew(() =>
            {
                try { while (!data.t2_closing) TASK_2(); }
                catch (Exception ex)
                { MessageBox.Show(String.Format("ОШИБКА TASK2 {0};", ex), "Фатальная Ошибка"); }

            });
        }
       
        public void StartThread()
        {
            try { while (!data.t2_closing)  main();
                }
            catch 
            { MessageBox.Show(String.Format("ОШИБКА главного потока "), "Фатальная Ошибка");
            }
            data.t2closeOK = true;
        }

       
        //*****************************************************************************************
        //                     ГЛАВНЫЙ ЦИКЛ ОСНОВНОГО ПОТОКА thread2
        //*****************************************************************************************
        void main()
        {
            pFORM = data.adres_FORM1;//берем адрес формы
            if (data.adres_FORM1 == null) { Thread.Sleep(100); return; }//значит поток формы еще не запустился

            Thread.Sleep(500);

            try
            {


                if (data.port == null)
                {
                   
                    // pFORM.AddText(" test");

                    if (data.StartOpenport )
                    {
                        pFORM.AddText("Подключение... " + data.nameport);
                        data.port = new SerialPort(data.nameport, 19200, Parity.None, 8, StopBits.One);

                        try
                        {
                            data.StartOpenport = false;
                           
                            data.port.Handshake = Handshake.None;
                            data.port.DtrEnable = true;
                            data.port.RtsEnable = false;
                            data.port.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
                            data.port.Open();

                        }
                        catch (Exception ex)
                        {
                            pFORM.AddText("Ошибка открытия порта: " + ex.Message.ToString());
                            data.StartOpenport = false;
                            data.port = null;
                        }
                        finally
                        {
                            pFORM.AddText("Подключение успешно ");
                            Thread.Sleep(1000);
                        }
                    }

                }
            }
            catch { }

        }//work main

        /// <summary>
        /// Обработчик события получения данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            data.port.Read(datard, 0, 8);

            byte d0 = datard[0];
            byte d1 = datard[1];
            byte d2 = datard[2];
            byte d3 = datard[3];
            byte d4 = datard[4];
            byte d5 = datard[5];
            byte d6 = datard[6];
            byte d7 = datard[7];

            //pFORM.AddText(String.Format(e.EventType.ToString()+" read     {0};{1};{2};{3};{4};{5};{6};{7}; ",
           //        d0, d1, d2, d3, d4, d5, d6, d7));

        }
        //*****************************************************************************************
        //                     ГЛАВНЫЙ ЦИКЛ   TASK1         
        //*****************************************************************************************

        bool iniTASK()
        {
            //sleep надо 1сек что бы иницилизировались form1.МЕТОДЫ() 
            //data.adres_FORM1 - иницил.быстрее, а методы позже
            if (data.adres_FORM1 == null) { Thread.Sleep(5000); return (true); }//значит поток формы еще не запустился
            else return (false);
        }



        

        void TASK_1()//==============  TASK1
        {

            if (iniTASK()) return;


            if (data.port!=null && data.Transmit_Start)
            {

                byte[] bytes = { 0, 0 };
                bytes[0] = data.TXdata.d1;
                bytes[1] = data.TXdata.d2;
                ushort crc = CRC16.ComputeChecksum(bytes);

                data.TXdata.crc1 = (byte)(crc >> 8);
                ushort tmp = (ushort)(crc << 8);
                data.TXdata.crc2 = (byte)(tmp>>8);

                byte[] dt = { 0, 0, 0, 0,     0, 0, 0, 0 };

                dt[0] =  data.TXdata.SlaveAddres;
                dt[1] =  data.TXdata.fun_code;
                dt[2] =  data.TXdata.adr2;
                dt[3] =  data.TXdata.adr1;
                dt[4] =  data.TXdata.d2;
                dt[5] =  data.TXdata.d1;
                dt[6] =  data.TXdata.crc1;
                dt[7] =  data.TXdata.crc2;

                data.port.Write(dt, 0,  8);
              //  pFORM.AddText(String.Format("send1     {0};{1};{2};{3};{4};{5};{6};{7}; ",
              //       dt[0], dt[1], dt[2], dt[3], dt[4], dt[5], dt[6], dt[7]));

                Thread.Sleep(100);

               

                bytes[0] = data.TXdata.d3;
                bytes[1] = data.TXdata.d4;
                crc = CRC16.ComputeChecksum(bytes);

                data.TXdata.crc1 = (byte)(crc >> 8);
                tmp = (ushort)(crc << 8);
                data.TXdata.crc2 = (byte)(tmp >> 8);


                dt[0] = data.TXdata.SlaveAddres;
                dt[1] = data.TXdata.fun_code;
                dt[2] = data.TXdata.adr1;
                dt[3] = data.TXdata.adr2;
                dt[4] = data.TXdata.d4;
                dt[5] = data.TXdata.d3;
                dt[6] = data.TXdata.crc1;
                dt[7] = data.TXdata.crc2;

                 data.port.Write(dt, 0,   8);

               // pFORM.AddText(String.Format("send2     {0};{1};{2};{3};{4};{5};{6};{7}; ",
               //        dt[0], dt[1], dt[2], dt[3], dt[4], dt[5], dt[6], dt[7]));

                pFORM.AddText("Отправка выполнена");

                data.Transmit_Start = false;
            }


             Thread.Sleep(200);

        }//task1




        //*****************************************************************************************
        //                     ГЛАВНЫЙ ЦИКЛ   TASK2      II
        //*****************************************************************************************

        byte[] datard= { 0,0,0,0,0,0,0,0,0,0,0};
        void TASK_2()//==============  TASK2
        {
            if (iniTASK()) return; 
            else Thread.Sleep(900);
        }//task2


       


     

     
    } //end class ПОТОКА
}//namespace


