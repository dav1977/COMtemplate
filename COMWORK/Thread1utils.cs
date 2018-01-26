using System;
using System.Windows.Forms;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

using System.Runtime.InteropServices;
using System.Globalization;
using System.Reflection;


namespace Program
{
    //public partial class Thread1
    public partial class Form1 : Form
    {
         
    //    public int ctviv;//счетчик выведенных сообщений с редактор
          
            public void StartThread()//main
        {
            try { while (!data.t1_closing) main(); }
            catch (Exception ex)
            { MessageBox.Show(String.Format("ОШИБКА ПОТОКА1 {0};", ex.Message), "Фатальная Ошибка"); }
            data.t1closeOK = true;
        }

        //============== ГЛАВНЫЙ ЦИКЛ ПОТОКА 1     формы1
        void main()
        {

            Thread.Sleep(1000);

        }

        //**********************************************************************************************
        //                      УТИЛИТЫ ПРОИЗВОДНЫЙ КЛАСС ОТ FORM
        //**********************************************************************************************

        public byte BCD(int a, int b)//упаковано 2 числа по 4 бита каждое
        {

            if (a > 9 || b > 9)
            {
                MessageBox.Show("ОШИБКА BCD функции число более 9", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                AddText(String.Format(" a={0}  b={1} ", Convert.ToString(a, 2), Convert.ToString(b, 2)));

                return 1;
            }

            return (byte)((a << 4) | b);

        }
        
        
         public  void BCDtoBYTE(ref byte BCD,ref byte ML, ref byte HL)//распаковка  BCD по HL ML
        {
           byte qw = BCD;

           int rr = BCD;

          

           rr = (byte)((qw & B._1111._0000));
           ML = (byte)(rr >> 4);   
              

               HL = (byte)(qw & B._0000._1111);




           //   AddText( Convert.ToString(BCD & B._0000._1111, 2));

            return ;

        }
        
        
        public string HX(UInt32 x) //int -- в string в 16виде
        {
            string l = Convert.ToString(x, 16).ToUpperInvariant();
            if (l.Length == 1) l = "  0" + l;
            else if (l.Length == 2) l = "  " + l;
            else if (l.Length == 3) l = "0" + l;
            return (l);
        }


        public UInt32 HB(string x) //HEX(string) --  в uint32
        {
            if (x == null || x == "") return 1;
            if (x.Length > 4)
            {
                MessageBox.Show("слишком болшое HEX число >" + x + "<", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation); return 1;
            }
            UInt32 dd = 1;

            try
            {
                dd = System.Convert.ToUInt32(x, 16);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ошибка HEX  >" + ex.Message + "<", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation); return 1;
            };
            return dd;
        }
        public string HS(string HexValue)// HEX в виде string - в String СИМВОЛЫ
        {
          
            string result = "";
            while (HexValue.Length > 0)
            {
                AddText("конверт " + HexValue);
                string aa = "";

                string ee=HexValue.Substring(0, 2);
                try
                {
                  //  aa = System.Convert.ToChar(System.Convert.ToUInt32( ee,16 ).ToString();

                }
                catch { AddText("err HS() convert to string"); }



                if ( Convert.ToByte(aa)>192  ) result += aa;
                else result += '.';
                
                HexValue = HexValue.Substring(2, HexValue.Length - 3);
                
            }
            return result;
        }

        public UInt32 SU(string x)// стринг в uint
        {
            if (x == "") return 1;
            UInt32 dd = 1;
            try
            {
                dd = System.Convert.ToUInt32(x, 10);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ошибка ввода чисел  >" + ex.Message + "<", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation); return 1;
            };
            return dd;

        }

         public string toSYMBOL(byte a)// ASCII  в UNICODE для вывода на экран
        {
             
             if ((a < 192 && a > 122) || a < 31) return (".");

            byte []t = new byte[1];
            t[0] = a;

          
           string rez = Encoding.GetEncoding(1251).GetString(t,0,t.Length) ;

             return (rez);

        }

  

    } //end class ПОТОКА
}//namespace


