using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Reflection;
using System.IO.Ports;
using System.Collections.Generic;


namespace Program
{
    /// <summary>
    /// ОБЩИЙ КЛАСС ДАННЫХ
    /// </summary>
    class data
    {

        public static bool t1_closing = false;
        public static bool t2_closing = false;
        public static bool t1closeOK = false;
        public static bool t2closeOK = false;
        public static bool task1_closing = false;
        public static bool task2_closing = false;
        public static bool task1closeOK = false;
        public static bool task2closeOK = false;
        public static Form1 adres_FORM1 = null;
        public static Queue<ДАННЫЕочередь> очередь { get; set; }
        public static Queue<ДАННЫЕочередь> очередьКОПИЯ { get; set; }//НЕ ИСПОЛЬЗ
        public static Queue<ДАННЫЕочередь> очередьКОПИЯрабочая { get; set; }
        // public static Queue<ДАННЫЕочередь> PLAYQueue { get; set; }
        public static ДАННЫЕочередь buf_read;

        public static Form1 pFORM1;//УКАЗАТЕЛЬ НА ФОРМУ


        //ПРИКЛАДНЫЕ
        public static SerialPort port;
        public static string nameport = null;
        public static bool StartOpenport,  gotovprog=false, zaprosINI = false;
        public static UInt32 sizeFILE=0;
        public static int sleep;
        public static byte[] filedata = new byte[2000000];
        public static bool Transmit_Start = false;
        public  struct TXdata
        { public static byte SlaveAddres, fun_code, adr1=0, adr2=0, d1, d2, d3, d4, crc1, crc2;  }
            
        
        public data()
        {
            // <ДАННЫЕочередь> - тип данных с какими работает очередь
            очередь = new Queue<ДАННЫЕочередь>();
            buf_read = new ДАННЫЕочередь();
         
        }

        public static void col(SByte u)
        {

            if (u == 1) Console.ForegroundColor = ConsoleColor.Green;
            if (u == 2) Console.ForegroundColor = ConsoleColor.Red;
            if (u == 3) Console.ForegroundColor = ConsoleColor.Blue;

        }
        public static void print(string pr)
        {
            col(1);
            Console.WriteLine(pr);

        }
        public static void printRED(string pr)
        {
            col(2);
            Console.WriteLine(pr); col(1);

        }
        public static void avar(string avar)
        {
            col(2);
            Console.WriteLine(avar); col(1);
        }

        public static void printBLUE(string pr)
        {
            col(3);
            Console.WriteLine(pr); col(1);

        }
       

  



        public void CONV(string s, ref double pr, ref bool err)
        {
            err = false;
            //==========================================================
            if (s == "0") { pr = 0; return; }
            try
            {
                // Gets a NumberFormatInfo associated with the en-US culture.
                NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                nfi.NumberDecimalSeparator = ",";//--
                pr = double.Parse(s, nfi);
            }
            catch
            {
                //  if (diag) printRED("не считал цену сделки пробуем с разделитель точка");
                try
                {
                    // Gets a NumberFormatInfo associated with the en-US culture.
                    NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                    nfi.NumberDecimalSeparator = ".";//--
                    pr = double.Parse(s, nfi);
                }

                catch
                {
                    err = true;
                }
            }

        }



        public void CONV(string s, ref int pr, ref bool err)
        {
            err = false;
            if (s == "0") { pr = 0; return; }
            try
            {
                pr = int.Parse(s);
            }
            catch { err = true; }

        }
        public void CONV(string s, ref SByte pr, ref bool err)
        {
            err = false;
            if (s == "0") { pr = 0; return; }
            try
            {

                pr = SByte.Parse(s);
            }
            catch { err = true; }

        }
      
        public void CONV(string s, ref long pr, ref bool err)
        {
            err = false;
            if (s == "0") { pr = 0; return; }
            try
            {
                pr = long.Parse(s);
            }
            catch { err = true; }

        }
        public void CONV(string s, ref bool pr, ref bool err)
        {
            err = false;
            if (s == "True") pr = true;
            else
                if (s == "False") pr = false;
                else err = true;
        }

        public static string getTIME()
        {
            string ss = String.Format("{0}",DateTime.Now.Millisecond);
           int  pr = int.Parse(ss);
           if (pr < 100) ss = "0" + ss;
           if (pr < 10) ss = "0" + ss;

           string ss2 = String.Format("{0}", DateTime.Now.Second);
            pr = int.Parse(ss2);
           
           if (pr < 10) ss2 = "0" + ss2;

           string ss1 = String.Format("{0}", DateTime.Now.Minute);
           pr = int.Parse(ss1);
           
           if (pr < 10) ss1 = "0" + ss1;

           return (String.Format("{0,2}:{1,2}:{2,2}.{3,3}", DateTime.Now.Hour, ss1, ss2, ss));
        }

        public static string getTIME(DateTime time)
        {
            string ss = String.Format("{0}", time.Millisecond);
            int pr = int.Parse(ss);
            if (pr < 100) ss = "0" + ss;
            if (pr < 10) ss = "0" + ss;

            string ss2 = String.Format("{0}", time.Second);
            pr = int.Parse(ss2);

            if (pr < 10) ss2 = "0" + ss2;

            string ss1 = String.Format("{0}", time.Minute);
            pr = int.Parse(ss1);

            if (pr < 10) ss1 = "0" + ss1;

            return (String.Format("{0,2}:{1,2}:{2,2}.{3,3}", time.Hour, ss1, ss2, ss));
        }

        public static string getTIMEwithDAY()
        {
            return (String.Format(" {0}.{1,3}",DateTime.Now, DateTime.Now.Millisecond));
        }

        public static void зависание()
        {
            while (true) Thread.Sleep(1000);
        }


       

    }


    public static class CRC16
    {
        const ushort polynomial = 0xA001;
        static ushort[] table = new ushort[256];


        static byte[] HexToBytes(string input)
        {
            byte[] result = new byte[input.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToByte(input.Substring(2 * i, 2), 16);
            }
            return result;
        }


        public static ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = 0;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(crc ^ bytes[i]);
                crc = (ushort)((crc >> 8) ^ table[index]);
            }
            return crc;
        }

        public static byte[] ComputeChecksumBytes(byte[] bytes)
        {
            ushort crc = ComputeChecksum(bytes);
            return BitConverter.GetBytes(crc);
        }

        public static void  init()
        {
            ushort value;
            ushort temp;
            for (ushort i = 0; i < table.Length; ++i)
            {
                value = 0;
                temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
            }
        }
    }



    /// <summary>
    /// класс бинарного задания константы
    /// </summary>
    public static class B
    {
        public static readonly V _0000 = 0x0;
        public static readonly V _0001 = 0x1;
        public static readonly V _0010 = 0x2;
        public static readonly V _0011 = 0x3;
        public static readonly V _0100 = 0x4;
        public static readonly V _0101 = 0x5;
        public static readonly V _0110 = 0x6;
        public static readonly V _0111 = 0x7;

        public static readonly V _1000 = 0x8;
        public static readonly V _1001 = 0x9;
        public static readonly V _1010 = 0xA;
        public static readonly V _1011 = 0xB;
        public static readonly V _1100 = 0xC;
        public static readonly V _1101 = 0xD;
        public static readonly V _1110 = 0xE;
        public static readonly V _1111 = 0xF;

        public struct V
        {
            ulong Value;

            public V(ulong value)
            {
                this.Value = value;
            }

            private V Shift(ulong value)
            {
                return new V((this.Value << 4) + value);
            }

            public V _0000 { get { return this.Shift(0x0); } }
            public V _0001 { get { return this.Shift(0x1); } }
            public V _0010 { get { return this.Shift(0x2); } }
            public V _0011 { get { return this.Shift(0x3); } }
            public V _0100 { get { return this.Shift(0x4); } }
            public V _0101 { get { return this.Shift(0x5); } }
            public V _0110 { get { return this.Shift(0x6); } }
            public V _0111 { get { return this.Shift(0x7); } }

            public V _1000 { get { return this.Shift(0x8); } }
            public V _1001 { get { return this.Shift(0x9); } }
            public V _1010 { get { return this.Shift(0xA); } }
            public V _1011 { get { return this.Shift(0xB); } }
            public V _1100 { get { return this.Shift(0xC); } }
            public V _1101 { get { return this.Shift(0xD); } }
            public V _1110 { get { return this.Shift(0xE); } }
            public V _1111 { get { return this.Shift(0xF); } }

            static public implicit operator V(ulong value)
            {
                return new V(value);
            }

            static public implicit operator ulong(V this_)
            {
                return this_.Value;
            }

            static public implicit operator uint(V this_)
            {
                return (uint)this_.Value;
            }

            static public implicit operator ushort(V this_)
            {
                return (ushort)this_.Value;
            }

            static public implicit operator byte(V this_)
            {
                return (byte)this_.Value;
            }
        }
    }

    [Serializable]
  public class   ДАННЫЕочередь
    {
        public DateTime time { get; set; }
        public string strERROR { get; set; }
        public uint adr { get; set; }
        public byte sz { get; set; }
        public byte b1 { get; set; }
        public byte b2 { get; set; }
        public byte b3 { get; set; }
        public byte b4 { get; set; }
        public byte b5 { get; set; }
        public byte b6 { get; set; }
        public byte b7 { get; set; }
        public byte b8 { get; set; }
    }

}
 


