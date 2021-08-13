using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Class_HART
{
   public class _Convert
    {
       
        //===================================================================================================================================================================================================================================================================================
        /*! @} */
        /*! \defgroup 2_module конверктаця байт в различные типы даных 
             \ingroup main_module
         *  @{
        */
        //========================== конверктаця байт в различные типы даных ================================================================================================================================================================================================================

        /// <summary>
        /// конверктирует все байты в строку ( PACKED-ASCII (6-BIT ASCII) DATA FORMAT)
        /// </summary>
        /// <remarks>
        /// Использует табличные значения "PACKED_ASCII" для конверктацыи 
        /// </remarks>
        /// <param name="temp"> -входной масив байт в формате PACKED-ASCII</param>
        /// <returns> строку типа (string) </returns>
       static public string Byte_tu_A(Byte[] temp) // конверктирывать все байты в строку ( PACKED-ASCII (6-BIT ASCII) DATA FORMAT)
        {
            if (temp.Length != 0)
            {
                int i = 0;
                int j = 0;
                string str = "";
                Byte[] a = { };
                while (i < temp.Length)
                {
                    Array.Resize(ref a, a.Length + 1);
                    a[a.Length - 1] = temp[i];
                    if (a.Length == 3)
                    {

                        int tem = a[0];
                        tem <<= 8;
                        tem |= a[1];
                        tem <<= 8;
                        tem |= a[2];
                        int buet1 = (tem >> 18) & 0x3F;
                        int buet2 = (tem >> 12) & 0x3F;
                        int buet3 = (tem >> 6) & 0x3F;
                        int buet4 = tem & 0x3F;
                        str += Convert.ToString(_Tables.PACKED_ASCII[buet1]) + Convert.ToString(_Tables.PACKED_ASCII[buet2]) + Convert.ToString(_Tables.PACKED_ASCII[buet3]) + Convert.ToString(_Tables.PACKED_ASCII[buet4]);
                        j = 0;
                        Array.Resize(ref a, 0);
                    }

                    j++;
                    i++;
                }

                return str;
            }
            else return "";
        }
        /// <<summary>
        /// конверктирует все байты в строку ( PACKED-ASCII (6-BIT ASCII) DATA FORMAT)
        /// </summary>
        /// <remarks>
        /// Использует табличные значения "PACKED_ASCII" для конверктацыи 
        /// </remarks>
        /// <param name="Byte">- входной масив</param>
        /// <param name="offset">- начальный индекс</param>
        /// <param name="count">- длина</param>
        /// <returns>строка типа (string) </returns>
       static public string Byte_tu_A(Byte[] Byte, int offset, int count)// конверктирывать диапозон байт в строку 
        {
            Byte[] temp = new Byte[count];
            Array.Copy(Byte, offset, temp, 0, count);
            return Byte_tu_A(temp);
        }
        /// <summary>
        ///  конверктирыват 1 байт в масив типа ({T,F,F,T,F})
        /// </summary>
        /// <remarks>
        /// конверктирует входной байт в буливый масив где состояние индекса масива отоброжает состояния бита в байте ;
        /// </remarks>
        /// <param name="Byte">- входной байт</param>
        /// <returns>буливый масив</returns>
       static public bool[] Byte_tu_B(Byte Byte)  // конверктирыват 1 байт в масив типа ({T,F,F,T,F})
        {
            bool[] temp = { };
            for (int i = 0; i < 8; i++)
            {
                Array.Resize(ref temp, temp.Length + 1);
                if ((Byte & (1 << i)) != 0)
                {
                    temp[temp.Length - 1] = true;
                }
                else
                {
                    temp[temp.Length - 1] = false;
                }
            }
            return temp;
        }
        ///  <summary>
        ///  конверктирыват масив байт в масив типа ({T,F,F,T,F})
        /// </summary>
        /// <remarks>
        /// конверктирует входной масив байт в буливый масив где состояние индекса масива отоброжает состояния бита в байте ;
        /// </remarks>
        /// <param name="Byte">- входной масив байт </param>
        /// <returns>булевый масив</returns>
       static public bool[] Byte_tu_B(Byte[] Byte) // конверктирыват масив байт в масив типа ({T,F,F,T,F})
        {
            bool[] temp = { };
            for (int j = 0; j < Byte.Length; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    Array.Resize(ref temp, temp.Length + 1);
                    if ((Byte[j] & (1 << i)) != 0)
                    {
                        temp[temp.Length - 1] = true;
                    }
                    else
                    {
                        temp[temp.Length - 1] = false;
                    }
                }
            }
            return temp;
        }
        ///  <summary>
        ///  конверктирывать диапозон масива байт в масив типа ({T,F,F,T,F})
        /// </summary>
        /// <remarks>
        /// конверктирует входной масив байт в буливый масив где состояние индекса масива отоброжает состояния бита в байте ;
        /// </remarks>
        /// <param name="Byte">- входной масив</param>
        /// <param name="offset">- начальный индекс</param>
        /// <param name="count">- длина</param>
        /// <returns>выходной масив</returns>
       static public bool[] Byte_tu_B(Byte[] Byte, int offset, int count) // конверктирыват диапозон байт в масив типа ({T,F,F,T,F})
        {
            Byte[] temp = new Byte[count];
            Array.Copy(Byte, offset, temp, 0, count);
            return Byte_tu_B(temp);
        }
        /// <summary>
        ///  конверктирывать 3 первых байта в строку типа (dd.mm.yyyy)  
        /// </summary>
        /// <param name="Byte">- входной масив байт </param>
        /// <returns>строку содеожащюю дату в формате (dd.mm.yyyy) </returns>
       static public string Bute_tu_data(Byte[] Byte)// конверктирывать 3 первых байта в строку типа (dd.mm.yy)  
        {
            string day = "";
            string myn = "";
            string year = "";
            day = Convert.ToString(Byte[0]).PadLeft(2, '0');
            myn = Convert.ToString(Byte[1]).PadLeft(2, '0');
            year = Convert.ToString(Byte[2] + 1900).PadLeft(4, '0');
            return day + "." + myn + "." + year;
        }
        /// <summary>
        ///   конверктирывать диапозон байт в строку типа (dd.mm.yyyy)  
        /// </summary>
        /// <param name="Byte">- входной масив байт</param>
        /// <param name="offset">- начальный индекс</param>
        /// <param name="count">- длина </param>
        /// <returns>строку содеожащюю дату в формате (dd.mm.yyyy) </returns>
       static public string Bute_tu_data(Byte[] Byte, int offset, int count)// конверктирывать 3 байта из диапозона байт в строку типа (dd.mm.yy)  
        {
            Byte[] temp = new Byte[count];
            Array.Copy(Byte, offset, temp, 0, count);
            return Bute_tu_data(temp);
        }
        /// <summary>
        /// конвертирует масив байт в тип float 
        /// </summary>
        /// <remarks>
        /// конвертирует масив байт напрямую согласно IEEE.754
        /// </remarks>
        /// <param name="Byte">- входной масив байт</param>
        /// <returns>число типа float</returns>
       static unsafe public float Bute_tu_F(Byte[] Byte) // конверктирывать байты в float 
        {
            UInt32* pFlt;

            UInt32 temp = Byte[0];
            temp <<= 8;
            for (int i = 0; i < Byte.Length - 1; i++)
            {
                temp |= Byte[i];
                temp <<= 8;
            }
            temp |= Byte[Byte.Length - 1];

            float d = 0;
            pFlt = (UInt32*)&d;

            *pFlt = temp;

            return d;

        }
        /// <summary>
        /// конвертирует диапозон байт в тип float 
        /// </summary>
        /// <remarks>
        /// конвертирует диапозон байт напрямую согласно IEEE.754
        /// </remarks>
        /// <param name="Byte">- входной масив байт </param>
        /// <param name="offset">- начальный индекс</param>
        /// <param name="count">- длина (в основном это 4 байта) </param>
        /// <returns>число типа float</returns>
       static public float Buye_tu_F(Byte[] Byte, int offset, int count)// конверктирывать диапозон байт в float 
        {
            try
            {
                Byte[] temp = new Byte[count];
                Array.Copy(Byte, offset, temp, 0, count);
                return Bute_tu_F(temp);
            }
            catch
            {
                return -1;
            }
        }
        //===========================================================================================================================================================================================================================================================================
        /*! @} */

        /*! \defgroup 4_modul конверктаця типов даных в байты
             \ingroup main_module
         *  @{
        */
        //========================== конверктаця типов даных в байты  ===============================================================================================================================================================================================================
        /// <summary>
        /// число float в масив байт 
        /// </summary>
        /// <param name="a">- число </param>
        /// <returns> масив байт</returns>
       static unsafe public Byte[] Float_tu_byte(float a)
        {
            float* pFlt;
            UInt32 d = 0;
            pFlt = (float*)&d;
            *pFlt = a;
            Byte[] temp = { };
            for (int i = 0; i < 4; i++)
            {
                Array.Resize(ref temp, temp.Length + 1);
                temp[temp.Length - 1] = Convert.ToByte(d & 0xFF);
                d >>= 8;
            }
            Array.Reverse(temp);
            return temp;

        }
        /// <summary>
        /// строку (PACKED-ASCII) в масив байт опредиленой длины 
        /// </summary>
        /// <remarks>
        /// если строка будет содержать меньше байт чем задано то остальные будут равны 0 
        /// </remarks>
        /// <param name="s">- строка (PACKED-ASCII DATA FORMAT)</param>
        /// <param name="leng">- количество байт которое будет получено на выходе </param>
        /// <returns>масив байт длины leng </returns>
       static public Byte[] Str_tu_byte(string s, int leng)//строку в масив байт опредиленой длины если строка будет содержать меньше байт чем задано то остальные будут равны 0 
        {

            Byte[] T_buf = Str_tu_byte(s);
            Array.Resize(ref T_buf, leng);
            return T_buf;
        }
        /// <summary>
        /// строку (PACKED-ASCII) в масив байт 
        /// </summary>
        /// <remarks>
        /// упаковывает по 4 символа в 3 байта согласно формату (PACKED-ASCII) если длина входной строки не кратно 4 то последний 1,2,3 символа не будут упакованы 
        /// </remarks>
        /// <param name="s">- входная строка (должна быть кратна 4 )</param>
        /// <returns>масив байт</returns>
       static public Byte[] Str_tu_byte(string s)//строку в байт 4 сивола = 3 байта 
        {
            int buf = 0;
            Byte[] T_buf = { };
            int j = 0;
            string tem = s.PadRight(40, ' ');
            for (int i = 0; i < tem.Length; i++)
            {
                int temp = Array.IndexOf(_Tables.PACKED_ASCII, tem[i]);
                if (temp != -1)
                {
                    buf |= temp;
                    if (j != 3) buf <<= 6;
                    j++;
                }
                if (j == 4)
                {
                    j = 0;
                    Array.Resize(ref T_buf, T_buf.Length + 3);
                    Byte[] t = BitConverter.GetBytes(buf);
                    T_buf[T_buf.Length - 1] = Convert.ToByte(buf & 0xFF);
                    T_buf[T_buf.Length - 2] = Convert.ToByte((buf >> 8) & 0xFF);
                    T_buf[T_buf.Length - 3] = Convert.ToByte((buf >> 16) & 0xFF);

                    buf = 0;
                }
            }

            return T_buf;
        }
        /// <summary>
        /// конверктирует входную строку типа (dd.mm.yууy) в масив из 3 байт 
        /// </summary>
        /// <remarks>
        /// принемает строки форматов (dd.mm.yууy) (dd-mm-yууy) (dd,mm,yууy) (dd:mm:yууy) (dd.mm-yууy) (dd:mm.yууy) (dd.mm:yууy) (dd-mm:yууy)
        /// </remarks>
        /// <param name="date">- входная строка </param>
        /// <returns>масив из 3 байт</returns>
       static public Byte[] Data_tu_bute(string date) // конверктирует входную строку типа (dd.mm.yy) в масив из 3 байт 
        {
            string[] words = date.Split(new char[] { ':', '.', ',', '-' });
            if (words.Length >= 3)
            {
                Byte[] temp = { Convert.ToByte(words[0]), Convert.ToByte(words[1]), (Convert.ToByte(Convert.ToInt32(words[2]) - 1900)) };
                return temp;
            }
            else return new Byte[] { 0, 0, 0 };

        }
        /// <summary>
        /// конвертирует входные даные в масив байт
        /// </summary>
        /// <param name="dd">- день</param>
        /// <param name="mm">- месяц</param>
        /// <param name="yy">- год</param>
        /// <returns>масив из 3 байт</returns>
       static public Byte[] Data_tu_bute(string dd, string mm, string yy)
        {
            Byte[] temp = { Convert.ToByte(Convert.ToInt32(dd)), Convert.ToByte(Convert.ToInt32(mm)), Convert.ToByte(Convert.ToInt32(yy)) };
            return temp;
        }
        /// <summary>
        /// конвертирует входные даные в масив байт
        /// </summary>
        /// <param name="dd">- день</param>
        /// <param name="mm">- месяц</param>
        /// <param name="yy">- год</param>
        /// <returns>масив из 3 байт</returns>
       static public Byte[] Data_tu_bute(int dd, int mm, int yy)
        {
            Byte[] temp = { Convert.ToByte(dd), Convert.ToByte(mm), Convert.ToByte(yy) };
            return temp;
        }


        //===========================================================================================================================================================================================================================================================================
        /*! @} */
    }
}
