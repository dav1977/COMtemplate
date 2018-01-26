using System;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.IO.Ports;
using  System.Text;

namespace Program
{
  
    public partial class Form1 : Form
    {
        public Thread t1;//дополнительный поток в форме

        

        public Form1()
        {
            CRC16.init();

            InitializeComponent();
          
            data.adres_FORM1 = this;

            UPDATEports();
        }
       
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "COMtemplate";

           // data.nameport = "COM4";
          

            numericUpDown1.Minimum = 0;
            numericUpDown1.Maximum = 255;
            data.sleep = 0;
            numericUpDown1.Value = 0;

            timer1.Interval = 200;
            timer1.Enabled = true;

            Control.CheckForIllegalCrossThreadCalls = false;

            //запуск ПОТОКА  внутренней функции
            t1 = new Thread(new System.Threading.ThreadStart(StartThread)); // Start Thread Session
            t1.IsBackground = true;
            t1.Start();            
        }
      

        //**********************************************************************************************
        //======================= ТАЙМЕР 1 ==============================
        //**********************************************************************************************
        bool restart_prokrutka;
        private void timer1_Tick_1(object sender, EventArgs e)//------ ТАЙМЕР -------------
        {

            if ( restart_prokrutka)//----------- ПРОКРУТКА
            {
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                restart_prokrutka = false;
            }



            bool s = false;
            if (data.port != null)  s=data.port.IsOpen;

            

            if (data.port != null)
            {
                label7.Visible = true;
                button4.Text = "Отключить";
                label7.Text = "Подключено "+data.port.PortName;

               
            }
            else
            {
                label7.Visible = false;
                button4.Text = "ПОДКЛЮЧИТЬ";

            }
        }
        //***********************************************************************************************

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            restart_prokrutka = true;
        }


        //***********************************************************************************************
       // BinaryReader pr; byte r;

        //void button1_Click(Object sender, EventArgs e)
        //{
            

        //    AddText("Старт чтения файла");
        //    try {
        //        openFileDialog1.Filter =
        //                           "Файлы (*.*)|*.*";

        //        if (data.lastpath == null)
        //        {
        //            openFileDialog1.InitialDirectory = "";
        //            openFileDialog1.FileName = openFileDialog1.InitialDirectory + @"";
        //        }
        //        else openFileDialog1.InitialDirectory = data.lastpath;

        //        if (openFileDialog1.ShowDialog() == DialogResult.OK)
        //        {   data.lastpath = openFileDialog1.FileName;

        //            // richTextBox1.LoadFile(openFileDialog1.FileName);
        //            if (File.Exists(openFileDialog1.FileName) == false) return;


        //             pr = new BinaryReader(File.OpenRead(openFileDialog1.FileName));

                 

        //            uint i = 0;  bool flagerror;
        //            while (true)
        //            {

        //                try
        //                {
        //                    flagerror = true;
        //                    r = pr.ReadByte();
        //                }
        //                catch
        //                {
        //                    flagerror = false;

        //                }
        //                if (!flagerror) break;
        //                 else { data.filedata[i] = r;  } 
                        
        //                i++;
        //                if (i > 1500000) {
        //                    AddTextCol("Слишком большой(>1.5Мб) файл ОШИБКА", System.Drawing.Color.Red);
        //                    break;
        //                }
        //              }
                   
        //            pr.Close();

        //            data.sizeFILE = i;
        //            AddText("Чтение выполнено");
        //            label2.Text = String.Format("Размер {0} байт ",data.sizeFILE) ;
        //        }

          
                

        //        richTextBox1.Modified = false;

        //    }
        //    catch (System.IO.FileNotFoundException Ситуация)
        //    {
        //        MessageBox.Show(Ситуация.Message +
        //            "\nНет такого файла", "Ошибка",
        //            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //    }
        //    catch (Exception Ситуация)
        //    {   // Отчет о других ошибках
        //        MessageBox.Show(Ситуация.Message, "Ошибка",
        //        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //    }
        //}
       


        void closeT1() 
        {

            int a = 0;

            //data.task1_closing = true;
            //while (!data.t1closeOK) Thread.Sleep(50);
            //// t1.Join(9000);
            //if (!data.task1closeOK) MessageBox.Show(String.Format("не могу закрыть поток t1"), "Ошибка Закрытия");
            if (data.port != null) { data.port.Close(); data.port.Dispose(); data.port = null;  Thread.Sleep(100); }


            data.t2_closing = true;

            while (!data.t2closeOK)
            {
                Thread.Sleep(50); a++;
                if (a > 10) { MessageBox.Show(String.Format("не могу закрыть поток t2"), "Ошибка Закрытия");
                    break; }
            }

            data.t1_closing = true;
            while (!data.t1closeOK) Thread.Sleep(50);
           // t1.Join(9000);
            if (!data.t1closeOK) MessageBox.Show(String.Format("не могу закрыть поток t1"), "Ошибка Закрытия");

   
           

        }
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();//вызывает Form1_FormClosing(
          
        }
        public void EXIT()
        {
            this.Close();
        }
        private void Form1_FormClosing( object sender, FormClosingEventArgs e)
        {
            closeT1();
        }

      
       
        //****************************************************************************************

        //****************************************************************************************

    //    Color[] colors = {Color.Black, Color.Red, Color.Blue, Color.Green, Color.Yellow};
     public void AddTextCol(string message, System.Drawing.Color color)
        {
            try
            {
                this.BeginInvoke(new LineReceivedEvent(LineReceived), data.getTIME() + " " + message + "\n");

                int start = richTextBox1.Text.Length;
                int length = message.Length;

                richTextBox1.Select(start, length); //выделяем текст
                richTextBox1.SelectionColor = color; //для выделенного текста устанавливаем цвет

            }
            catch { }
        }

        public void AddText(string message)
        {

            try {
            this.BeginInvoke(new LineReceivedEvent(LineReceived), data.getTIME() + " " + message + "\n");
            }
            catch {}
        }
        public void AddTextNOtime(string message)
        {
            try {
            this.BeginInvoke(new LineReceivedEvent(LineReceived), message + "\n");
            }
            catch { }
        }
        public void AddTextCONTperv(string message)
        {
            try
            {
            this.BeginInvoke(new LineReceivedEvent(LineReceived), message);
            }
            catch {}
        }

        public void AddTextCONT(string message)
        {
             try {
            this.BeginInvoke(new LineReceivedEvent(LineReceived),  " " + message);
             }
             catch { }
        }

        private delegate void LineReceivedEvent(string command);
        private void LineReceived(string message2)
        {
            richTextBox1.AppendText(message2 ); //AddText(POT);
        }



        //****************************************************************************************

        //****************************************************************************************

        private void button4_Click(object sender, EventArgs e)
        {
            CLEAR();
        }
        public void CLEAR ()
        {
  
            richTextBox1.Clear();
        
        }

        bool UPDATEports()
        {
            listBox1.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            foreach (string dd in ports)
            { 
                listBox1.Items.Add(dd);
            }

            if (ports.Length == 0) return false;
            else return true;
        }

        private void button_updatePORTS(object sender, EventArgs e)
        {
           
           if (!UPDATEports()) AddText("нет портов для открытия ");

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            if (data.port == null)
            {
                if (data.nameport == null || textBox1.Text == "") { AddTextCol("Не выбран порт ошибка ", System.Drawing.Color.Aqua); return; }
                data.nameport = textBox1.Text;
                data.StartOpenport = true;
    
            }
            else
            {
  
                    data.port.Close();
                    data.port.Dispose();
                    data.port = null;
                    data.StartOpenport = false;
                AddText("Порт отключен ");
            }

        }

       

        private void button5_Click(object sender, EventArgs e)
        {
            CLEAR();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            data.nameport = listBox1.Text; textBox1.Text = listBox1.Text;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            data.sleep = Decimal.ToInt32(numericUpDown1.Value);
        }
  

        /// <summary>
        /// transmit
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="fValue"></param>
        void tr(byte a1, byte a2, float fValue)
        {
            if (data.Transmit_Start) return;

            data.TXdata.SlaveAddres = 1;
            data.TXdata.fun_code = 6;
            data.TXdata.adr1 = a1;
            data.TXdata.adr1 = a2;

           
            byte[] bValue = BitConverter.GetBytes(fValue);
            //byte[] result = new byte[3] { bValue[1], bValue[2], bValue[3] };
            data.TXdata.d1 = bValue[0];
            data.TXdata.d2 = bValue[1];
            data.TXdata.d3 = bValue[2];
            data.TXdata.d4 = bValue[3];

            data.Transmit_Start = true;
        }


     


        //end
    }

}
