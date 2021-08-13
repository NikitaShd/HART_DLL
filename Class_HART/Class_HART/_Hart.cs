using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using Class_HART;
namespace Class_HART
{
    public class Conect
    {
        private static SerialPort port;      ///< клас USB порта 
        // =========== стандартные настройки usb ========================================
        private string     Port_id = "COM0"; ///< ID порта 
        private int Spide = 9600;     ///< Скортость обмена 
        private Parity P = Parity.Odd;  ///< Паритет 
        private int Data_bits = 8;    ///< Количетсво бит
        private StopBits S = StopBits.One; ///< Стоп бит  
        private int ReadTimeout = 2000;
     	private int WriteTimeout = 2000;
        private string EROR;         ///< код ошибки
        //   port = new SerialPort("COM5", 9600, Parity.None, 8, StopBits.One);
        //============ состояние класа ====================================================  
        public bool scan = false; ///< true - если идет сканирывание сети 
        protected Read_Fraim[] Read = { }; ///<содержит в себе список длиных адрисов после выполнения функцыи skan
        private bool lock_ = true; ///< предотврощяет создание второго потоков
        public int scan_step = 0;  ///< текущий этап сканирывания сети (0..15) 
        private bool stop_skan = false; ///< сигнал для завершения потока сканирывания 
        public Read_Fraim last_fraim = new Read_Fraim(); ///<последний принятый кадр
        //============ настройки обмена ===================================================
        public int write_taim = 700; ///< время ожидания 1 байта  
        public int write_taimout = 1;    ///<Добовлять времени за каждый принатый байт
        public int preambula_leng = 7;   ///< Длина преамбулы  
        public int Master = 1;           ///< индитификатор мастера 
        public struct Read_Fraim ///<структура принемаемого кадра 
        {
            public byte SD; ///<признак старта (0x06-короткий фрейм) (0х86-длиный фрейм) (0х01-короткий фреим в пакетном режиме) (0х81-длиный фреим в пакетном режиме)
            public byte AD_master; ///<адрес мастера 1 или 0 
            public byte[] AD_slaiv;  ///<адрес (7 бит в коротком фрейме)(38 бит в длином фрейме)
            public byte AD_Short;  ///<адрес rjhjnrbq (7 бит в коротком фрейме)
            public byte CD; ///<Команда 
            public byte BC; ///<Количество байт ST+DT
            public byte[] ST; ///<статус (2 байта)
            public byte[] DT; ///<даные  (0..25 байт)
            public byte CHK;///<контролиная сумма
            public string Statys_1; ///<раскодирываный статус хранящийся в 1 байте 
            public string Statys_2; ///<раскодирываный статус хранящийся в 2 байте 
            public string temp; ///<заметка 
        }
      
        public Conect(string ID,int Spid) // 1 конструктор 
        {
            Port_id = ID;
            Spide   = Spid;
        }
        public Conect(string ID, int Spid, Parity p, int Databits, StopBits Stop_Bits) // 2 конструктор 
        {
            Port_id   = ID;
            Spide     = Spid;
            P         = p;
            Data_bits = Databits;
            S         = Stop_Bits;
        }
        public Conect(string ID, int Spid, Parity p, int Databits, StopBits Stop_Bits,int ReadTim,int WriteTim) // 3 конструктор 
        {
            Port_id      = ID;
            Spide        = Spid;
            P            = p;
            Data_bits    = Databits;
            S            = Stop_Bits;
            write_taim = ReadTim;
            WriteTimeout = WriteTim;
        }
      
        public string init() 
        {
            try
            {
                port = new SerialPort(Port_id, Spide, P, Data_bits, S);
                port.ReadTimeout = write_taim;
                port.WriteTimeout = 200;
                port.Open();
                init_Encod_unit();
                return "True";
               
            }
            catch (Exception ex)
            {
                if (ex is System.IO.IOException)
                {
                    return "Port Exception: " + ex.ToString();
                }
                else
                {
                    return "General Exception: " + ex.ToString();
                }
            }
        }

        
        
        
        public string close()
        {
            try
            {
                port.Close();
                return "True";
            }
            catch (Exception ex)
            {
                if (ex is System.IO.IOException)
                {
                    return "Port Exception: " + ex.ToString();
                }
                else
                {
                    return "General Exception: " + ex.ToString();
                }
            }
        }
        /*! \defgroup main_module > [(Функции для работы с типами даных)] */

        /*! \defgroup second_module Функцыи для работы с байтами 
             \ingroup main_module
         *  @{
        */

        //========================== Функцыи для работы с байтами ===========================================================================================================================================================================================================================
      /// <summary>
      /// Конветрирует строку в байты 
      /// </summary>
      /// <remarks>
      /// Конверктирует строку байт типа ХХХХХХХХ в масив типа байт [XX,XX,XX,XX];
      /// </remarks>
      /// <param name="str">- входная строка</param>
      /// <returns>выходной масив байт</returns>
        public byte[] GetBytes(string str)
        {
            int hexLength = str.Length;
            byte[] bytes = new byte[hexLength / 2];
            for (int i = 0; i < hexLength; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(str.Substring(i, 2), 16);
            }
            return bytes;
        }
       
       

        public string Eror_cod()
        {
            return EROR;
        }
        /// <summary>
        /// формировка хеш суммы 
        /// </summary>
        /// <param name="a">- входной масивв байт </param>
        /// <returns>байт хеш суммы</returns>
        public Byte XOR_calc(Byte[] a) // формировка хеш суммы 
        {
            Byte result = 0x00;
            bool calc = false ;
            for (int i = 0; i < a.Length-1; i++)
            {
                if (a[i] != 0xFF) calc = true;
                if (calc == true)
                {
                    result = (byte)(result ^ a[i]); // XOR всех байт 
                }
            }
            return result;
        }
        /// <summary>
        /// проверка хеш суммы
        /// </summary>
        /// <remarks>
        /// принемает масив байт полученого кадра и проверяет хеш сумму
        /// </remarks>
        /// <param name="a">- входной масив байт</param>
        /// <returns>true - если хеш сумма впорядке иначе вернет false </returns>
        private bool XOR_Chec(Byte[] a) // проверка хеш суммы
        {
            try
            {
                if (a[a.Length - 1] == XOR_calc(a))
                {
                    EROR = "True";
                    return true;
                }
                else 
                {
                    EROR = "CRH Eror";
                    return false;  
                }
            }
            catch
            {
              EROR = "CRH Eror 2";
              return false;
            }
        }

        Stopwatch sw = new Stopwatch();
        /*! \addtogroup <Write> [(Отправка даных)]
             * \brief Данный модуль содержит список функцый отправки.
          @{
            */
        /// <summary>
        /// отправка кадра 
        /// </summary>
        /// <remarks>
        /// отправляет масив байт кадра и ожидает ответа отсеивая ответы не по этому запросу. !(Стирает преамбулу из кадра)
        /// </remarks>
        /// <param name="a">- даные на отправку</param>
        /// <returns>масив даных ответивших приборов </returns>
        private Byte[][] Write(Byte[] a)
        {
                   
                port.DiscardInBuffer();
                port.ReadTimeout = 0;
                port.Write(a, 0, a.Length);
                int i = 0;
                Byte[] temp = { };
                Byte[][] read = {};
                int T = (int)(write_taim);
                int FF_Start = 0;
                bool F = false;
                while (i < T)
                {
                    
                    int intBuffer = port.BytesToRead;
                    if (intBuffer > 0)
                    {
                        Byte b = (byte)port.ReadByte();
                        Array.Resize(ref temp, temp.Length + 1);
                        temp[temp.Length - 1] = b;
                        T += (int)(write_taimout);
                    }
                    i++;
                    sw.Reset();
                    sw.Start();
                    while (sw.ElapsedMilliseconds < 1) { }
                    sw.Stop();
                 // Thread.Sleep(1);
                }
                EROR = "True";

                int tmp = 0;
                int size = 0;
                for (int j = 1; j < temp.Length ; j++) // я уже и сам не нимаю что тут происходит 
                {
                   // try { if ((temp[j + 1] == 0xFF) && (temp[j] == 0xFF) && (temp[j - 1] != 0xFF)) { F = false; } }
                   // catch { }
                    try
                    {
                        if (((temp[j] == 0x06) || (temp[j] == 0x86)) && (temp[j - 1] == 0xFF) && (temp[j - 2] == 0xFF))
                        {
                            FF_Start++;
                            F = true;
                            tmp = 0;
                            Array.Resize(ref read, read.Length + 1);
                            read[read.Length - 1] = new Byte[] { };

                        }
                    }
                    catch { }
                     if (F == true){
                         Array.Resize(ref   read[read.Length - 1], read[read.Length - 1].Length + 1);
                         read[read.Length - 1][read[read.Length - 1].Length - 1] = temp[j];
                        
                         if ((temp[j] == 0x06)&&(size == 0))
                         {
                             size = 4;
                         }
                         else if ((temp[j] == 0x86) && (size == 0))
                         {
                             size = 8;
                         }
                        
                             if (((tmp == 3) && (size == 4)) || ((tmp == 7) && (size == 8)))
                             {
                                 size += temp[j];
                             }
                         
                         if (size == tmp)
                         {
                             F = false;
                             tmp = 1;//// не трогать ! 
                             size = 0;

                         }
                          tmp++;
                    
                         
                     }
                }

              
                return read;

            
         
        }
        /*! @} */
        // =======================<  Спец команды   >========================================================================== 
           public string[] dev = { "MTM701" };    
           public void MTM701_Comand_130(int id_master, Byte[] id_slaiv, ref string[] res)
           {
               try
               {
                   Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 130, new Byte[] { });
                   Array.Resize(ref res, 5);
                   res[0] = BitConverter.ToString(temp[0].DT, 0, 2);
                   res[1] = BitConverter.ToString(temp[0].DT, 2, 2);
                   res[2] = BitConverter.ToString(temp[0].DT, 4, 2);
                   res[3] = BitConverter.ToString(temp[0].DT, 6, 2);
                   res[4] = BitConverter.ToString(temp[0].DT, 8, 2);
                 
               }
               catch
               {
                   res[0] = "READ_EROR";
                   res[1] = "READ_EROR";
                   res[2] = "READ_EROR";
                   res[3] = "READ_EROR";
                   res[4] = "READ_EROR";
                   EROR = "READ_EROR";
               }

           }
           public void MTM701_Comand_131(int id_master, Byte[] id_slaiv,int kod_op,ref int res)
           {
               try
               {
                   Read_Fraim[] temp = { };
                   if (kod_op == 1)
                   {
                       temp = Write_long(preambula_leng, id_master, id_slaiv, 131, new Byte[] { (byte)kod_op,(byte)res});
                   }
                   else if (kod_op == 0)
                   {
                       temp = Write_long(preambula_leng, id_master, id_slaiv, 131, new Byte[] { (byte)kod_op });
                       res = Convert.ToInt32(temp[0].DT[1]);
                   }else
                   {

                   }

                        

                        
                     
                    
               }
               catch
               {
                   res =-1;

                   EROR = "READ_EROR";
               }

           }

           public void MTM701_Comand_132(int id_master, Byte[] id_slaiv,int kod,ref int nom_ust,ref int logic,ref float min,ref float max ,ref float gist)
           {
               try
               {
                  
                         
                           Read_Fraim[] temp = { };
                           if (kod == 1)
                           {
                           
                               byte[] mi = _Convert.Float_tu_byte(min);
                               byte[] ma = _Convert.Float_tu_byte(max);
                               byte[] g = _Convert.Float_tu_byte(gist);
                               Byte[] te = { (byte)kod, (byte)nom_ust, (byte)logic, mi[0], mi[1], mi[2], mi[3], ma[0], ma[1], ma[2], ma[3], g[0], g[1], g[2], g[3] };
                               temp = Write_long(preambula_leng, id_master, id_slaiv, 132, te);
                           }
                           else if (kod == 0)
                           {
                               temp = Write_long(preambula_leng, id_master, id_slaiv, 132, new Byte[] { (byte)kod,(byte)nom_ust});
                               nom_ust = temp[0].DT[1];
                               logic = temp[0].DT[2];
                               min = _Convert.Buye_tu_F(temp[0].DT, 3, 4);
                               max = _Convert.Buye_tu_F(temp[0].DT, 7, 4);
                               gist = _Convert.Buye_tu_F(temp[0].DT, 11, 4);
                           }
                           else
                           {

                           }
                           
               
               }
               catch
               {

                   EROR = "READ_EROR";
               }

           }
           public void MTM701_Comand_133(int id_master, Byte[] id_slaiv, int kod, ref int kod_d, ref int kod_ed, ref int allarm, ref byte[] kod_CAP, ref int t, ref int d, ref int t_m, ref int d_m)
           {
               try
               {


                   Read_Fraim[] temp = { };
                   if (kod == 1)
                   {
                       Byte[] te = { (byte)kod, (byte)kod_d, (byte)kod_ed, (byte)allarm, kod_CAP[0], kod_CAP[1], (byte)t, (byte)d };
                       temp = Write_long(preambula_leng, id_master, id_slaiv, 132, te);
                   }
                   else if (kod == 0)
                   {
                       temp = Write_long(preambula_leng, id_master, id_slaiv, 132, new Byte[] { (byte)kod });
                       kod_d = temp[0].DT[1];
                       kod_ed = temp[0].DT[2];
                       allarm = temp[0].DT[3];
                       kod_CAP[0] = temp[0].DT[4];
                       kod_CAP[1] = temp[0].DT[5];
                       t = temp[0].DT[8];
                       d = temp[0].DT[9];
                       t_m = temp[0].DT[6];
                       d_m = temp[0].DT[8];
                   }
                   else
                   {

                   }


               }
               catch
               {

                   EROR = "READ_EROR";
               }

           }
           /*! \addtogroup <COMAND> [(Базовые команды)]
            * \brief Данный модуль содержит список базовых команд и их описание.
         @{
           */
           // =======================< Базовые команды >========================================================================== 
       /// <summary>
       /// базовая команда для получения длиного адреса устройства
       /// </summary>
       /// <param name="id_master">- Адрес отправителя </param>
       /// <param name="id_slaiv">- Короткий адрес получателя</param>
       /// <returns> Двхмерный масив байт где каждая стока ответ от 1 устройства </returns>
        public Byte[][] Comand_0(int id_master, int id_slaiv)
        {
            Byte[] a = {};
            Read_Fraim[] temp = Write_short(preambula_leng,id_master,id_slaiv,0,a);
            Byte[][] Read = {};
            for (int i = 0; i < temp.Length; i++)
            {
                Array.Resize(ref  Read, Read.Length + 1);
                Read[Read.Length - 1] = new Byte[] { temp[i].DT[1], temp[i].DT[2], temp[i].DT[9], temp[i].DT[10], temp[i].DT[11]};
              
            }
            return Read;

        }
        /// <summary>
        /// базовая команда для получения длиного адреса устройства
        /// </summary>
        /// <param name="id_master">- Адрес отправителя </param>
        /// <param name="id_slaiv">- Короткий адрес получателя</param>
        /// <returns> масив структуры кадров в котором содержатца все ответы </returns>
        public Read_Fraim[] Comand_0_F(int id_master, int id_slaiv)
        {
            Read_Fraim[] res = { };
            Byte[] a = { };
            try
            {
                Read_Fraim[] temp = Write_short(preambula_leng, id_master, id_slaiv, 0, a);
               
                return temp;
            }
            catch
            {
                EROR = "READ_EROR";
                return res; 
                
            }
           

        }
        /// <summary>
        /// команда 0 для получения основной информации об устройстве  
        /// </summary>
        /// <remarks>
        /// расшифровует байты и формирует их в групы по параметрам конверктируя их в стоки 
        /// </remarks>
        /// <param name="id_master">- Адрес отправителя  </param>
        /// <param name="id_slaiv">- Длиный адрес получателя </param>
        /// <param name="data">- содержит 9 строк с параметрами устройсва см. HCF_SPEC-127_(Revision_6.0) </param>
        public void Comand_0(int id_master, Byte[] id_slaiv, ref string[] data)
        {
            
           try
            {
                string[] te = new string[10];
                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 0, new Byte[] { });

                try { te[0] = temp[0].DT[1].ToString("X2");} catch { te[0] = "eror"; }
                try { te[1] = temp[0].DT[2].ToString("X2");} catch { te[1] = "eror"; }
                try { te[2] = temp[0].DT[3].ToString();} catch { te[2] = "eror"; }
                try { te[3] = temp[0].DT[4].ToString();} catch { te[3] = "eror"; }
                try { te[4] = temp[0].DT[5].ToString();} catch { te[4] = "eror"; }
                try { te[5] = temp[0].DT[6].ToString();} catch { te[5] = "eror"; }
                try { te[6] = temp[0].DT[7].ToString();} catch { te[6] = "eror"; }
                try { te[7] = temp[0].DT[8].ToString("X2");} catch { te[7] = "eror"; }
                try { te[8] = Convert.ToString(temp[0].DT[8], 2).PadLeft(8, '0');} catch { te[8] = "eror"; }
                try { te[9] = temp[0].DT[9].ToString("X2") + " " + temp[0].DT[10].ToString("X2") + " " + temp[0].DT[11].ToString("X2"); }
                catch { te[9] = "eror"; }
                data = te;
            }
            catch
            {
                EROR = "READ_EROR";
            }
        }
        /// <summary>
        /// КОМАНДА #1 ЧТЕНИЕ ПЕРВИЧНОЙ ПЕРЕМЕННОЙ
        /// </summary>
        /// <remarks>
        ///  Чтение Первичной Переменной (ПП). ПП возвращается в формате с плавающей запятой. 
        /// </remarks>
        /// <param name="id_master"> - адрес отправителя</param>
        /// <param name="id_slaiv"> - длиный адрес получателя</param>
        /// <param name="cod"> - код единиц измерения см. HCF_SPEC-183_(Revision_13.0) </param>
        /// <param name="par"> - измеряемое значение</param>
        public void Comand_1(int id_master, Byte[] id_slaiv, ref int cod, ref float par)
        {
            try
            {
                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 1, new Byte[] { });

                cod = Convert.ToInt32(temp[0].DT[0]);
                par = _Convert.Buye_tu_F(temp[0].DT, 1, 4);
            }
            catch
            {
                cod = -1;
                cod = -1;
                EROR = "READ_EROR";
            }

        }
        /// <summary>
        /// КОМАНДА #1 ЧТЕНИЕ ПЕРВИЧНОЙ ПЕРЕМЕННОЙ
        /// </summary>
        /// <remarks>
        ///  Чтение Первичной Переменной (ПП). ПП возвращается в формате с плавающей запятой. 
        ///  Единицы измерения переводятца функцией Encod_unit(); согласно HCF_SPEC-183_(Revision_13.0)
        /// </remarks>
        /// <param name="id_master"> - адрес отправителя</param>
        /// <param name="id_slaiv"> - длиный адрес получателя</param>
        /// <param name="cod"> - переведеные единиц измерения cм. HCF_SPEC-183_(Revision_13.0) </param>
        /// <param name="par"> - измеряемое значение</param>
        public void Comand_1(int id_master, Byte[] id_slaiv, ref string cod, ref float par)
        {
            try
            {
                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 1, new Byte[] { });

                cod = Encod_unit( Convert.ToInt32(temp[0].DT[0]));
                par = _Convert.Buye_tu_F(temp[0].DT, 1, 4);
            }
            catch
            {
                cod = "eror";
                par = -1;
                EROR = "READ_EROR";
            }

        }
        /// <summary>
        /// КОМАНДА #2 ЧТЕНИЕ ПП КАК ВЕЛИЧИНЫ ТОКА И В ПРОЦЕНТАХ ОТ ДИАПАЗОНА
        /// </summary>
        /// <remarks>
        /// Чтение Первичной Переменной как величины тока в миллиамперах и в процентах от диапазона. 
        /// Величина ПП в миллиамперах всегда равна текущему значению Аналогового Выхода устройства включая состояние алармов и другие настройки.
        /// Величина ПП в процентах от диапазона всегда идет следом, даже если токовое значение ПП находится в состоянии аларма или зафиксировано на определенном значении.
        /// Кроме того, величина в процентах от диапазона не ограничена пределами между 0% и 100%, а отслеживает значение ПП за пределами заданной шкалы но в пределах границ измерения сенсора, если они заданы. 
        /// </remarks>
        /// <param name="id_master"> - адрес отправителя</param>
        /// <param name="id_slaiv"> - длиный адрес получателя</param>
        /// <param name="tok"> - значение тока в милиамперах</param>
        /// <param name="procent"> - % диапазона</param>
        public void Comand_2(int id_master, Byte[] id_slaiv, ref float tok, ref float procent)
        {
            try
            {
                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 2, new Byte[] { });

                tok = _Convert.Buye_tu_F(temp[0].DT, 0, 4);
                procent = _Convert.Buye_tu_F(temp[0].DT, 4, 4);
            }
            catch
            {
                tok = -1;
                procent = -1;
                EROR = "READ_EROR";
            }
        }
        /// <summary>
        /// КОМАНДА #3 ЧТЕНИЕ ДИНАМИЧЕСКИХ ПЕРЕМЕННЫХ И ТОКОВОГО ЗНАЧЕНИЯ ПП
        /// </summary>
        /// <remarks>
        /// Чтение величины тока, отражающего значение ПП, и до четырех предопределенных Динамических Переменных. 
        /// Токовое значение ПП всегда отражает величину Аналогового Выхода устройства включая состояние алармов и другие настройки.
        /// Содержание Вторичной, Третьей и Четвертой Переменных зависит от типа устройства (например, Вторичная Переменная для датчика давления 3051 показывает температуру измерительной ячейки датчика). 
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- адрес получателя</param>
        /// <param name="tok">- значение тока в милиамперах</param>
        /// <param name="kod_1">- переведеные единиц измерения первой переменой cогласно HCF_SPEC-183_(Revision_13.0)</param>
        /// <param name="par_1">- изменяемое значение первой переменой</param>
        /// <param name="kod_2">- переведеные единиц измерения второй переменой cогласно HCF_SPEC-183_(Revision_13.0)</param>
        /// <param name="par_2">- изменяемое значение второй переменой</param>
        /// <param name="kod_3">- переведеные единиц измерения третей переменой cогласно HCF_SPEC-183_(Revision_13.0)</param>
        /// <param name="par_3">- изменяемое значение третей переменой</param>
        /// <param name="kod_4">- переведеные единиц измерения четвертой переменой cогласно HCF_SPEC-183_(Revision_13.0)</param>
        /// <param name="par_4">- изменяемое значение четвертой переменой</param>
        public void Comand_3(int id_master, Byte[] id_slaiv, ref float tok, ref string kod_1, ref float par_1, ref string kod_2, ref float par_2, ref string kod_3, ref float par_3, ref string kod_4, ref float par_4)
        {
            try
            {
                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 3, new Byte[] { });

                tok = _Convert.Buye_tu_F(temp[0].DT, 0, 4);
                try
                {
                    kod_1 = Encod_unit(Convert.ToInt32(temp[0].DT[4]));
                    par_1 = _Convert.Buye_tu_F(temp[0].DT, 5, 4);
                }
                catch { kod_1 = "eror"; par_1 = -1; }

                try
                {
                kod_2 = Encod_unit(Convert.ToInt32(temp[0].DT[9]));
                par_2 = _Convert.Buye_tu_F(temp[0].DT, 10, 4);
                }
                catch { kod_2 = "eror"; par_2 = -1; }

                try
                {
                kod_3 = Encod_unit(Convert.ToInt32(temp[0].DT[14]));
                par_3 = _Convert.Buye_tu_F(temp[0].DT, 15, 4);
                }
                catch { kod_3 = "eror"; par_3 = -1; }
                try
                {
                    kod_4 = Encod_unit(Convert.ToInt32(temp[0].DT[19]));
                    par_4 = _Convert.Buye_tu_F(temp[0].DT, 20, 4);
                }
                catch { kod_4 = "eror"; par_4 = -1; }

            }
            catch
            {
                tok = -1;
                kod_1 = "eror";
                par_1 = -1;

                kod_2 = "eror";
                par_2 = -1;

                kod_3 = "eror";
                par_3 = -1;

                kod_4 = "eror";
                par_4 = -1;

                EROR = "READ_EROR";
            }
        }
        /// <summary>
        /// КОМАНДА #3 ЧТЕНИЕ ДИНАМИЧЕСКИХ ПЕРЕМЕННЫХ И ТОКОВОГО ЗНАЧЕНИЯ ПП
        /// </summary>
        /// <remarks>
        /// Чтение величины тока, отражающего значение ПП, и до четырех предопределенных Динамических Переменных. 
        /// Токовое значение ПП всегда отражает величину Аналогового Выхода устройства включая состояние алармов и другие настройки.
        /// Содержание Вторичной, Третьей и Четвертой Переменных зависит от типа устройства (например, Вторичная Переменная для датчика давления 3051 показывает температуру измерительной ячейки датчика). 
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- адрес получателя</param>
        /// <param name="tok">- значение тока в милиамперах</param>
        /// <param name="par_1">- значение 1 переменой + переведеные единиц измерения cогласно HCF_SPEC-183_(Revision_13.0)</param>
        /// <param name="par_2">- значение 2 переменой + переведеные единиц измерения cогласно HCF_SPEC-183_(Revision_13.0)</param>
        /// <param name="par_3">- значение 3 переменой + переведеные единиц измерения cогласно HCF_SPEC-183_(Revision_13.0)</param>
        /// <param name="par_4">- значение 4 переменой + переведеные единиц измерения cогласно HCF_SPEC-183_(Revision_13.0)</param>
        public void Comand_3(int id_master, Byte[] id_slaiv,ref string tok ,ref string par_1,ref string par_2,ref string par_3, ref string par_4)
        {
            try
            {
                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 3, new Byte[] { });

                tok =Convert.ToString( _Convert.Buye_tu_F(temp[0].DT, 0, 4)) + " mA";
                try { par_1 = Convert.ToString(_Convert.Buye_tu_F(temp[0].DT, 5, 4)) + " " + Encod_unit(Convert.ToInt32(temp[0].DT[4])); }catch { par_1 = "eror"; }
                try { par_2 = Convert.ToString(_Convert.Buye_tu_F(temp[0].DT, 10, 4)) +" "+ Encod_unit(Convert.ToInt32(temp[0].DT[9]));  }catch { par_2 = "eror"; }
                try { par_3 = Convert.ToString(_Convert.Buye_tu_F(temp[0].DT, 15, 4)) +" "+ Encod_unit(Convert.ToInt32(temp[0].DT[14])); }catch { par_3 = "eror"; }
                try { par_4 = Convert.ToString(_Convert.Buye_tu_F(temp[0].DT, 20, 4)) +" "+ Encod_unit(Convert.ToInt32(temp[0].DT[19])); }catch { par_4 = "eror"; }
               

            }
           catch
            {
                tok = "eror";            
                par_1 = "eror"; 
                par_2 = "eror";
                par_3 = "eror";
                par_4 = "eror";

                EROR = "READ_EROR";
            }
        }
        /// <summary>
        /// КОМАНДА #6 ЗАПИСЬ АДРЕСА УСТРОЙСТВА
        /// </summary>
        /// <remarks> 
        ///  Это команда из категории управления канального уровня. 
        ///  Записывает адрес в полевое устройство. Адрес используется для управления Аналоговым Выходом Первичной Переменной и является средством идентификации при работе нескольких устройств на одной шине. 
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- длиный адрес получетеля</param>
        /// <param name="adres">- короткий адрес от 0 до 15 на которое будет отзыватца устройство при коротком фрейме</param>
        /// <returns>true - если команда выполнилась успешно и было получено подтверждение от устройства</returns>
        /// <returns>false - если случилась ошибка или небыло получено подтверждение от устройства</returns>
        public bool Comand_6(int id_master, Byte[] id_slaiv, int adres)
        {
            try
            {
              Byte[] te = BitConverter.GetBytes(adres);
              Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 6, new Byte[] {te[0]});

              if (temp[0].DT[0] == te[0])
              {
              return true;
              }
              else 
              return false;
              
            }
            catch
            {
                EROR = "READ_EROR";
                return false;
               
            }
        }
        /// <summary>
        /// КОМАНДА #11 ЧТЕНИЕ УНИКАЛЬНОГО ИДЕНТИФИКАТОРА АССОЦИИРОВАННОГО С ТЭГОМ
        /// </summary>
        /// <remarks>
        /// Это команда из категории управления канального уровня. Возвращает Расширенный код типа устройства, номера версий, и идентификационный номер устройства содержащий заданный тэг. Она будет выполнена, когда будут получены Расширенный адрес или Широковещательный адрес. Расширенный адрес в ответном сообщении это тоже самое что и запрос. 
        /// </remarks>
        /// <param name="id_master">- адрес отправителя </param>
        /// <param name="teg">- короткий тег в формате PACKED-ASCII(</param>
        /// <param name="data">- масив строк с ответами от устройств каждая строка содержит ответ от 1 устройства и заполнена даными как в команде 0</param>
        /// <returns> количество устройств откликнувшихся на даный тег</returns>
        public int Comand_11(int id_master, string teg, ref string[][] data)
        {
            string[][] te = {};
            try
            {
                Byte[] id_slaiv = {00,00,00,00,00};
                Byte[] tte = _Convert.Str_tu_byte(teg);
                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 11,tte);
                Array.Resize(ref te, temp.Length);
                for (int i = 0; i < temp.Length; i++)
                {
                    Array.Resize(ref te[i], 10);

                    try { te[i][0] = temp[i].DT[0].ToString("X2"); }
                    catch { te[i][0] = "eror"; }
                    try { te[i][1] = temp[i].DT[1].ToString("X2"); }
                    catch { te[i][1] = "eror"; }
                    try { te[i][2] = temp[i].DT[2].ToString("X2"); }
                    catch { te[i][2] = "eror"; }
                    try { te[i][3] = temp[i].DT[3].ToString("X2"); }
                    catch { te[i][3] = "eror"; }
                    try { te[i][4] = temp[i].DT[4].ToString("X2"); }
                    catch { te[i][4] = "eror"; }
                    try { te[i][5] = temp[i].DT[5].ToString("X2"); }
                    catch { te[i][5] = "eror"; }
                    try { te[i][6] = temp[i].DT[6].ToString("X2"); }
                    catch { te[i][6] = "eror"; }
                    try { te[i][7] = temp[i].DT[7].ToString("X2"); }
                    catch { te[i][7] = "eror"; }
                    try { te[i][8] = Convert.ToString(temp[0].DT[8], 2).PadLeft(8, '0'); }
                    catch { te[i][8] = "eror"; }
                    try { te[i][9] = temp[i].DT[9].ToString("X2") + " " + temp[0].DT[10].ToString("X2") + " " + temp[0].DT[11].ToString("X2"); }
                    catch { te[i][9] = "eror"; }
                }
                data = te;
                return temp.Length;
            }
            catch
            {
                EROR = "READ_EROR";
                data = te;
                return 0;
            }
        }
        /// <summary>
        /// КОМАНДА #11 ЧТЕНИЕ УНИКАЛЬНОГО ИДЕНТИФИКАТОРА АССОЦИИРОВАННОГО С ТЭГОМ
        /// </summary>
        /// <remarks>
        /// Это команда из категории управления канального уровня. Возвращает Расширенный код типа устройства, номера версий, и идентификационный номер устройства содержащий заданный тэг. Она будет выполнена, когда будут получены Расширенный адрес или Широковещательный адрес. Расширенный адрес в ответном сообщении это тоже самое что и запрос. 
        /// </remarks>
        /// <param name="id_master">- адрес отправителя </param>
        /// <param name="teg">- короткий тег в формате PACKED-ASCII(</param>
        /// <returns>возврашяет масив структур фрейма откликнувшихся устройств</returns>
        public Read_Fraim[] Comand_11(int id_master, string teg)
        {
            Read_Fraim[] res = { };
         
            try
            {
                Byte[] id_slaiv = { 00, 00, 00, 00, 00 };
                Byte[] tte = _Convert.Str_tu_byte(teg,6);
                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 11, tte);
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i].temp = teg;
                }
                    return temp;
               
            }
            catch
            {
                EROR = "READ_EROR";
                return res;
            }
        }
        /// <summary>
        /// КОМАНДА #12 ЧТЕНИЕ СООБЩЕНИЯ
        /// </summary>
        /// <remarks>
        /// Читает Сообщение, содержащееся в устройстве. 
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- длиный адрес получателя </param>
        /// <returns>сообщение от устройства в формате  PACKED-ASCII</returns>
        public string Comand_12(int id_master, Byte[] id_slaiv)
        {
            try
            {
                
                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 12, new Byte[] { });


                return _Convert.Byte_tu_A(temp[0].DT);
            }
            catch
            {
                EROR = "READ_EROR";
                return "";

            }
        }
        /// <summary>
        /// КОМАНДА #12 ЧТЕНИЕ СООБЩЕНИЯ
        /// </summary>
        /// <remarks>
        /// Читает Сообщение, содержащееся в устройстве. 
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- длиный адрес получателя </param>
        /// <param name="a">- сообщение от устройства в формате  PACKED-ASCII</param>
        public void Comand_12(int id_master, Byte[] id_slaiv,ref string a)
        {
            a = Comand_12(id_master, id_slaiv);
        }
        /// <summary>
        /// КОМАНДА #13 ЧТЕНИЕ ТЭГА, ОПИСАТЕЛЯ, ДАТЫ
        /// </summary>
        /// <remarks>
        /// Читает Тэг, Описатель и Дату, содержащиеся в устройстве. 
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- адрес получателя </param>
        /// <returns>возвращяет масив строк из 3 ячеек {Тег,Описатель,Дата}</returns>
        public string[] Comand_13(int id_master, Byte[] id_slaiv)
        {
            string[] s = new string[3];
           try
            {

                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 13, new Byte[] { });

                s[0] = _Convert.Byte_tu_A(temp[0].DT, 0, 6);
                s[1] = _Convert.Byte_tu_A(temp[0].DT, 6, 11);
                s[2] = _Convert.Bute_tu_data(temp[0].DT, 18, 3);
                return s;
            }
           catch
            {
                EROR = "READ_EROR";
                string[] a = new string[3]{"eror","eror","eror"};
               return a;
            }
        }
        /// <summary>
        /// КОМАНДА #13 ЧТЕНИЕ ТЭГА, ОПИСАТЕЛЯ, ДАТЫ
        /// </summary>
        /// <remarks>
        /// Читает Тэг, Описатель и Дату, содержащиеся в устройстве. 
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- адрес получателя </param>
        /// <param name="s">- масив строк из 3 ячеек {Тег,Описатель,Дата}</param>
        public void Comand_13(int id_master, Byte[] id_slaiv,ref string[] s)
        {
            s = Comand_13(id_master, id_slaiv);
        }
        /// <summary>
        /// КОМАНДА #14 ЧТЕНИЕ ИНФОРМАЦИИ СЕНСОРА ПЕРВИЧНОЙ ПЕРЕМЕННОЙ
        /// </summary>
        /// <remarks>
        /// Читает Серийный Номер Сенсора Первичной Переменной Процесса, минимальную и максимальную шкалу измерения сенсора, и код единиц измерения для этих величин. 
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- длиный адрес получателя</param>
        /// <param name="s_num">- серийный номер сенсора</param>
        /// <param name="kod">- код единиц измерения согласно HCF_SPEC-183_(Revision_13.0) </param>
        /// <param name="min">- минимальный диапозон шкалы </param>
        /// <param name="max">- максимальный диапозон шкалы </param>
        public void Comand_14(int id_master, Byte[] id_slaiv,ref string s_num ,ref  string kod ,ref float min , ref float max)
        {
          
            try
            {

                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 14, new Byte[] { });

                s_num = BitConverter.ToString(temp[0].DT,0,3);
                kod = Encod_unit(temp[0].DT[3]);
                max = _Convert.Buye_tu_F(temp[0].DT,4,4);
                min = _Convert.Buye_tu_F(temp[0].DT, 8, 4);
                
            }
            catch
            {
                EROR = "READ_EROR";
                s_num ="eror";
                kod = "eror";
                max = 0;
                min = 0;
            }
        }
        /// <summary>
        /// КОМАНДА #15 ЧТЕНИЕ ИНФОРМАЦИИ О ВЫХОДНОМ СИГНАЛЕ ПО ПЕРВИЧНОЙ ПЕРЕМЕННОЙ
        /// </summary>
        /// <remarks>
        /// Читает код аларма ПП, код функции преобразования ПП, код единиц диапазона ПП, верхнюю границу диапазона ПП, нижнюю границу диапазона ПП, величину демпфирования ПП, код защиты от записи, и код метки продавца, ассоциированный с устройством или Первичной Переменной. 
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- длиный адрес получателя</param>
        /// <param name="Alarm">- код аларма ПП</param>
        /// <param name="Transfer">- код функции преобразования ПП</param>
        /// <param name="Unit">- код единиц диапазона ПП</param>
        /// <param name="Upper_Range">- верхняя граница диапазона</param>
        /// <param name="Lower_Range">- нижняя граница диапозона</param>
        /// <param name="Damping_Value">- величина демпфирования ПП</param>
        /// <param name="Write_Protec">- код защиты от записи</param>
        /// <param name="Manufacturer_Identification">- код метки продавца</param>
        public void Comand_15(int id_master, Byte[] id_slaiv, ref int Alarm, ref int Transfer, ref int Unit, ref float Upper_Range, ref float Lower_Range, ref float Damping_Value, ref int Write_Protec, ref int  Manufacturer_Identification)
        {

            try
            {

                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 15, new Byte[] { });

                Alarm         = temp[0].DT[0];
                Transfer      = temp[0].DT[1];
                Unit          = temp[0].DT[2];
                Upper_Range   = _Convert.Buye_tu_F(temp[0].DT, 3, 4);
                Lower_Range   = _Convert.Buye_tu_F(temp[0].DT, 7, 4);
                Damping_Value = _Convert.Buye_tu_F(temp[0].DT, 11, 4);
                Write_Protec  = temp[0].DT[15];
                Manufacturer_Identification = temp[0].DT[16];
            }
            catch
            {
                EROR = "READ_EROR";

                Alarm    = -1;
                Transfer = -1;
                Unit     = -1;
                Upper_Range   = -1;
                Lower_Range   = -1;
                Damping_Value = -1;
                Write_Protec  = -1;
                Manufacturer_Identification = -1;
            }
        }
        /// <summary>
        /// КОМАНДА #16 ЧТЕНИЕ СБОРОЧНОГО НОМЕРА УСТРОЙСТВА
        /// </summary>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- длиный адрес получателя </param>
        /// <param name="Final_Assembly_Number">- сборочный номер в формате строки</param>
        public void Comand_16(int id_master, Byte[] id_slaiv, ref string Final_Assembly_Number)
        {

            try
            {

                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 16, new Byte[] { });

                Final_Assembly_Number = temp[0].DT[0].ToString("X2") + "-" + temp[0].DT[1].ToString("X2") + "-" + temp[0].DT[2].ToString("X2");
               

            }
            catch
            {
                EROR = "READ_EROR";
                Final_Assembly_Number = "eror";
               
            }
        }
        /// <summary>
        /// КОМАНДА #16 ЧТЕНИЕ СБОРОЧНОГО НОМЕРА УСТРОЙСТВА
        /// </summary>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- длиный адрес получателя </param>
        /// <param name="Final_Assembly_Number">- сборочный номер в формате числа</param>
        public void Comand_16(int id_master, Byte[] id_slaiv, ref int Final_Assembly_Number)
        {

            try
            {

                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 16, new Byte[] { });

                Final_Assembly_Number = (int)BitConverter.ToUInt32(temp[0].DT,0);

            }
            catch
            {
                EROR = "READ_EROR";
                Final_Assembly_Number = 0;

            }
        }
        /// <summary>
        /// КОМАНДА #17 ЗАПИСЬ СООБЩЕНИЯ
        /// </summary>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- адрес получателя</param>
        /// <param name="Meseige">- сообщение для записи</param>
        /// <returns> true - если запись прошла успешно </returns>
        public bool Comand_17(int id_master, Byte[] id_slaiv, string Meseige)
        {

            try
            {
                Byte[] a = _Convert.Str_tu_byte(Meseige, 24);
                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 17, a);

                if (BitConverter.ToString(a) == BitConverter.ToString(temp[0].DT))
                {
                    return true;
                }
                else
                {
                  EROR = "DATA_EROR";
                  return false;
                }


            }
            catch
            {
                EROR = "READ_EROR";
                return false;

            }
        }
        /// <summary>
        /// КОМАНДА #18 ЗАПИСЬ ТЭГА, ОПИСАТЕЛЯ, ДАТЫ
        /// </summary>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- адрес получателя</param>
        /// <param name="Teg">- тег </param>
        /// <param name="Descriptor">- описатель</param>
        /// <param name="Data">- дата</param>
        /// <returns></returns>
        public bool Comand_18(int id_master, Byte[] id_slaiv, string Teg, string Descriptor ,string Data)
        {

            try
            {
                Byte[] a = _Convert.Str_tu_byte(Teg, 6);
                Byte[] b = _Convert.Str_tu_byte(Descriptor, 12);
                Byte[] c = _Convert.Data_tu_bute(Data);
                Byte[] e = { a[0], a[1], a[2], a[3], a[4], a[5], b[0], b[1], b[2], b[3], b[4], b[5], b[6], b[7], b[8], b[9], b[10], b[11], c[0], c[1], c[2] };
                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 18,e);

                if (BitConverter.ToString(e) == BitConverter.ToString(temp[0].DT))
                {
                    return true;
                }
                else
                {
                    EROR = "DATA_EROR";
                    return false;
                }

              
            }
            catch
            {
                EROR = "READ_EROR";
                return false;

            }
        }
        /// <summary>
        /// КОМАНДА #19 ЗАПИСЬ СБОРОЧНОГО НОМЕРА УСТРОЙСТВА
        /// </summary>
        /// <param name="id_master">- адрес отправителя </param>
        /// <param name="id_slaiv">- адрес получателя </param>
        /// <param name="Final_Assembly_Number">- сборочный номер </param>
        /// <returns></returns>
        public bool Comand_19(int id_master, Byte[] id_slaiv, string Final_Assembly_Number)
        {

            try
            {
                string d = Final_Assembly_Number.Replace("-", "");
                Byte[] a = GetBytes(d);
                
                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 19, a);

                if (BitConverter.ToString(a) == BitConverter.ToString(temp[0].DT))
                {
                      return true;
                }
                else
                {
                    EROR = "DATA_EROR";
                      return false;
                }

             
            }
            catch
            {
                EROR = "READ_EROR";
                return false;

            }
        }
        /// <summary>
        /// КОМАНДА #20 ЧИТАЕТ ДЛИНЫЙ ТЕГ
        /// </summary>
        /// <remarks>
        /// Считывает 32 байтный длиный тег !(тег и длиный тег - это совершено разный данные) 
        /// </remarks>
        /// <param name="id_master">- адрес отправителя </param>
        /// <param name="id_slaiv">- адрес получателя </param>
        /// <param name="Long_teg">- длиный тег</param>
        public void Comand_20(int id_master, Byte[] id_slaiv, ref string Long_teg)
        {

            try
            {

                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 20, new Byte[] { });

                Long_teg = Encoding.ASCII.GetString(temp[0].DT);

            }
            catch
            {
                EROR = "READ_EROR";
                Long_teg = "";

            }
        }
        /// <summary>
        /// КОМАНДА #21 СЧИТЫВАНИЕ УНИКАЛЬНОГО ИНДИТИФИКАТОРА СВЯЗАНОГО С ДАНЫМ ТЕГОМ 
        /// </summary>
        /// <remarks>
        /// Это команда из категории управления канального уровня. Возвращает Расширенный код типа устройства, номера версий, и идентификационный номер устройства содержащий заданный тэг. Она будет выполнена, когда будут получены Расширенный адрес или Широковещательный адрес. Расширенный адрес в ответном сообщении это тоже самое что и запрос. 
        /// </remarks>
        /// <param name="id_master">- адрес отправителя </param>
        /// <param name="teg">- длиный тег</param>
        /// <param name="data">- список устройст ответивших на команду</param>
        /// <returns></returns>
        public int Comand_21(int id_master, string teg, ref string[][] data)
        {
            string[][] te = { };
            try
            {
                Byte[] id_slaiv = { 00, 00, 00, 00, 00 };
                
            
                Byte[] tte = Encoding.ASCII.GetBytes(teg.PadRight(80, ' '));
                Array.Resize(ref tte, 32);
                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 21, tte);
                Array.Resize(ref te, temp.Length);
                for (int i = 0; i < temp.Length; i++)
                {
                    Array.Resize(ref te[i], 10);

                    try { te[i][0] = temp[i].DT[0].ToString("X2"); }
                    catch { te[i][0] = "eror"; }
                    try { te[i][1] = temp[i].DT[1].ToString("X2"); }
                    catch { te[i][1] = "eror"; }
                    try { te[i][2] = temp[i].DT[2].ToString("X2"); }
                    catch { te[i][2] = "eror"; }
                    try { te[i][3] = temp[i].DT[3].ToString("X2"); }
                    catch { te[i][3] = "eror"; }
                    try { te[i][4] = temp[i].DT[4].ToString("X2"); }
                    catch { te[i][4] = "eror"; }
                    try { te[i][5] = temp[i].DT[5].ToString("X2"); }
                    catch { te[i][5] = "eror"; }
                    try { te[i][6] = temp[i].DT[6].ToString("X2"); }
                    catch { te[i][6] = "eror"; }
                    try { te[i][7] = temp[i].DT[7].ToString("X2"); }
                    catch { te[i][7] = "eror"; }
                    try { te[i][8] = Convert.ToString(temp[0].DT[8], 2).PadLeft(8, '0'); }
                    catch { te[i][8] = "eror"; }
                    try { te[i][9] = temp[i].DT[9].ToString("X2") + " " + temp[0].DT[10].ToString("X2") + " " + temp[0].DT[11].ToString("X2"); }
                    catch { te[i][9] = "eror"; }
                }
                data = te;
                return temp.Length;
            }
            catch
            {
                EROR = "READ_EROR";
                data = te;
                return 0;
            }
        }
        /// <summary>
        /// КОМАНДА #21 СЧИТЫВАНИЕ УНИКАЛЬНОГО ИНДИТИФИКАТОРА СВЯЗАНОГО С ДАНЫМ ТЕГОМ 
        /// </summary>
        /// <remarks>
        /// Это команда из категории управления канального уровня. Возвращает Расширенный код типа устройства, номера версий, и идентификационный номер устройства содержащий заданный тэг. Она будет выполнена, когда будут получены Расширенный адрес или Широковещательный адрес. Расширенный адрес в ответном сообщении это тоже самое что и запрос. 
        /// </remarks>
        /// <param name="id_master">- адрес отправителя </param>
        /// <param name="teg">- длиный тег</param>
        /// <returns> масив формата фрейма с данымы ответивших устройств </returns>
        public Read_Fraim[] Comand_21(int id_master, string teg)
        {
            Read_Fraim[] res = { };
            try
            {
                Byte[] id_slaiv = { 00, 00, 00, 00, 00 };


                Byte[] tte = Encoding.ASCII.GetBytes(teg.PadRight(80, ' '));
                Array.Resize(ref tte, 32);
                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 21, tte);
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i].temp = teg;
                }
                
                return temp;
            }
            catch
            {
                EROR = "READ_EROR";

                return res;
            }
        }
        /// <summary>
        /// КОМАНДА #22 ЗАПИСЬ ДЛИНОГ ТЕГА 
        /// </summary>
        /// <param name="id_master">- адрес отправителя </param>
        /// <param name="id_slaiv">- адрес получателя </param>
        /// <param name="Long_teg">- длиный тег для записи </param>
        /// <returns>true - если запись прошла успешно </returns>
        public bool Comand_22(int id_master, Byte[] id_slaiv,  string Long_teg)
        {

            try
            {
                Byte[] bytes = Encoding.ASCII.GetBytes(Long_teg.PadRight(80,' '));
                Array.Resize(ref bytes,32);
                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 22, bytes);

                if (BitConverter.ToString(temp[0].DT) == BitConverter.ToString(bytes))
                {
                    return true;
                }
                else
                {
                    return false;
                }
             

            }
            catch
            {
                EROR = "READ_EROR";
                
                return false;
            }
        }
        /// <summary>
        /// КОМАНДА № 34 ЗАПИСЬ ПЕРЕМЕННОЙ ДЕМПФИРОВАНИЯ
        ///</summary>
        ///<remarks>
///ЗНАЧЕНИЕ Значение демпфирования первичной переменной представляет собой одну постоянную времени. (Выходной отклик на шаговый вход составляет 63% от окончательного установившегося значения по истечении этого времени.) И аналоговые, и цифровые выходы первичной переменной используют это значение. Затухание, применяемое к этим выходам, может также зависеть от других команд.
///Некоторые устройства реализуют только дискретные значения демпфирования (например, 1, 2, 4). Значение, полученное с помощью команды, может быть округлено или усечено устройством. Ответное сообщение вернет фактическое значение, используемое устройством. Предупреждение выдается, если значение усечено или округлено.      
        ///</remarks>
        /// <param name="id_master">- адрес отправителя </param>
        /// <param name="id_slaiv">- адрес получателя </param>
        /// <param name="DAMPING_VALUE">- значение для записи </param>
        /// <returns>true - если запись прошла успешно !(не учитывает округление значений)</returns>
        public bool Comand_34(int id_master, Byte[] id_slaiv, float DAMPING_VALUE )
        {

            try
            {
                Byte[] bytes = _Convert.Float_tu_byte(DAMPING_VALUE);
                
                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 34, bytes);

                if (BitConverter.ToString(temp[0].DT) == BitConverter.ToString(bytes))
                {
                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch
            {
                EROR = "READ_EROR";

                return false;
            }
        }
        
        /// <summary>
        ///  КОМАНДА № 35 ЗАПИСЬ ПЕРВИЧНЫХ ЗНАЧЕНИЙ ПЕРЕМЕННОГО ДИАПАЗОНА
        /// </summary>
        /// <remarks>
        /// Значение верхнего диапазона первичной переменной не зависит от значения нижнего диапазона первичной переменной.
        ///Первичные единицы измерения диапазона переменных, полученные с помощью этой команды, не влияют на Первичные единицы измерения переменной устройства. Первичные значения диапазона переменных будут возвращены в тех же единицах, что и receivei.
        ///Большинство устройств допускают, чтобы значение верхнего диапазона первичной переменной было ниже, чем значение нижнего диапазона первичной переменной, позволяя устройству работать с обращенным выходом. В документе для конкретного передатчика будет указано, не реализована ли эта возможность.
        /// </remarks>
        /// <param name="id_master">- адрес отправителя </param>
        /// <param name="id_slaiv">- адрес получателя </param>
        /// <param name="Primary_Variable">- единицы измерения</param>
        /// <param name="Upper_Range_Value">- верхний диапозон</param>
        /// <param name="Lower_Range_Value">- нижний диапозон</param>
        /// <returns>true - если запись прошла успешно </returns>
        public bool Comand_35(int id_master, Byte[] id_slaiv,int Primary_Variable, float  Upper_Range_Value,float  Lower_Range_Value)
        {

            try
            {
                Byte   bytes0 = (byte)(Primary_Variable);
                Byte[] bytes1 = _Convert.Float_tu_byte(Upper_Range_Value);
                Byte[] bytes2 = _Convert.Float_tu_byte(Lower_Range_Value);
                Byte[] te = { bytes0, bytes1[0], bytes1[1], bytes1[2], bytes1[3], bytes2[0], bytes2[1], bytes2[2], bytes2[3] };

                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 35, te);

                if (BitConverter.ToString(temp[0].DT) == BitConverter.ToString(te))
                {
                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch
            {
                EROR = "READ_EROR";

                return false;
            }
        }
        /// <summary>
        /// КОМАНДА # 36 УСТАНОВИТЬ ПЕРВИЧНОЕ ПЕРЕМЕННОЕ ЗНАЧЕНИЕ ВЕРХНЕГО ДИАПАЗОНА 
        /// </summary>
        /// <remarks>
        /// Величина процесса, примененного к Первичной переменной, становится значением Первичного переменного верхнего диапазона. Изменение значения верхнего диапазона основной переменной не повлияет на значение нижнего диапазона основной переменной. Эта команда выполняет ту же функцию, что и нажатие кнопки Span на устройстве.
        ///Большинство устройств допускают, чтобы значение верхнего диапазона первичной переменной было ниже, чем значение нижнего диапазона первичной переменной, позволяя устройству работать с обращенным выходом. В документе для конкретного передатчика будет указано, не реализована ли эта возможность.
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- адрес получателя</param>
        /// <returns>true - если команда была отправлена </returns>
        public bool Comand_36(int id_master, Byte[] id_slaiv)
        {

            try
            {
                
                Byte[] te = { };

                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 36, te);

                if (BitConverter.ToString(temp[0].DT) == BitConverter.ToString(te))
                {
                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch
            {
                EROR = "READ_EROR";

                return false;
            }
        }
        /// <summary>
        /// КОМАНДА # 37 УСТАНОВИТЬ ПЕРВИЧНОЕ ПЕРЕМЕННОЕ ЗНАЧЕНИЕ НИЖНЕГО ДИАПАЗОНА
        /// </summary>
        /// <remarks>
        /// Эта команда выполняет ту же функцию, что и нажатие кнопки Zero на устройстве. Величина процесса, примененного к Первичной переменной, становится значением Нижнего диапазона Первичной переменной. Изменение значения нижнего диапазона основной переменной пропорционально смещает значение верхнего диапазона основной переменной, так что диапазон остается постоянным. Когда изменение толкает значение верхнего диапазона первичной переменной за пределы либо предела датчика первичной переменной, насыщение значения верхнего диапазона первичной переменной насыщается, и код ответа № 14: Предупреждение: новое значение нижнего диапазона выдвинуто значение верхнего диапазона выше предела датчика. Когда значение нижнего диапазона первичной переменной толкает значение верхнего диапазона первичной переменной выше предела датчика первичной переменной, и результирующий диапазон меньше, чем минимальный диапазон первичной переменной, код ответа № 9, слишком высокий прикладной процесс или № 10, также примененный процесс Низкий, возвращается.
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- адрес получателя</param>
        /// <returns>true - если команда была отправлена </returns>
        public bool Comand_37(int id_master, Byte[] id_slaiv)
        {

            try
            {

                Byte[] te = { };

                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 37, te);

                if (BitConverter.ToString(temp[0].DT) == BitConverter.ToString(te))
                {
                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch
            {
                EROR = "READ_EROR";

                return false;
            }
        }
        /// <summary>
        /// КОМАНДА № 38 СБРОС КОНФИГУРАЦИИ ИЗМЕНЕННОГО ФЛАГА
        /// </summary>
        /// <remarks>
        /// Сбрасывает измененный конфигурацией код ответа, бит № 6 байта состояния преобразователя.
        ///Вторичные ведущие устройства, адрес «0», не должны выдавать эту команду. Первичные ведущие устройства, адрес «1», должны выдавать эту команду только после того, как Конфигурационный код ответа с измененной конфигурацией был обнаружен и применен.
        /// </remarks>
        /// <param name="id_master">- адрес отправителя </param>
        /// <param name="id_slaiv">- адрес получателя </param>
        /// <returns>true - если команда выполнилась успешно</returns>
        public bool Comand_38(int id_master, Byte[] id_slaiv)
        {

            try
            {

                Byte[] te = { };

                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 38, te);

                if (BitConverter.ToString(temp[0].DT) == BitConverter.ToString(te))
                {
                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch
            {
                EROR = "READ_EROR";

                return false;
            }
        }
        /// <summary>
        /// КОМАНДА № 39 УПРАВЛЕНИЕ ЭСППЗУ
        /// </summary>
        /// <remarks>
        /// Эта команда вызывает передачу данных из теневой памяти в энергонезависимую память (запись) или из энергонезависимой памяти в теневую память (восстановление).
        ///Код ответа о сбое полевого устройства, бит № 7 байта состояния передатчика, будет установлен, если обнаружена ошибка контрольной суммы BEPROM. Когда это происходит, для получения конкретной информации следует использовать команду № 48 «Чтение статуса дополнительного передатчика». Обратитесь к документу, относящемуся к передатчику, чтобы определить проверку ошибок, осуществляемую каждым типом устройства.
        ///Для запросов на запись запись может не начаться, пока не будет отправлен ответ, подтверждающий получение команды. Если в этих случаях возникают ошибки, в ответе последующих команд будет установлен код ответа о сбое полевого устройства.
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- адрес получателя </param>
        /// <param name="Control_Code">- код управления (0-Burn EEPROM ; 1-Restore Shadow RAM) </param>
        /// <returns>true - если команда выполнилась успешно</returns>
        public bool Comand_39(int id_master, Byte[] id_slaiv,int Control_Code)
        {

            try
            {

                Byte[] te = { (byte)Control_Code };

                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 39, te);

                if (BitConverter.ToString(temp[0].DT) == BitConverter.ToString(te))
                {
                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch
            {
                EROR = "READ_EROR";

                return false;
            }
        }
        /// <summary>
        /// КОМАНДА № 40 ВВОД / ВЫХОД ИЗ РЕЖИМА ФИКСИРОВОНОГО ТОКА
        /// </summary>
        /// <remarks>
        ///  Устройство переводится в режим фиксированного первичного переменного тока с первичным переменным током, установленным на полученное значение. Значение, возвращаемое в байтах ответа, отражает округленное значение усеченного значения dr, которое фактически было записано в цифроаналоговый преобразователь. Уровень «0» выходит из режима фиксированного первичного переменного тока. Режим фиксированного первичного переменного тока также отключается при отключении питания от устройства.
        /// </remarks>
        /// <param name="id_master">- адрес отправителя </param>
        /// <param name="id_slaiv">- адрес получателя </param>
        /// <param name="Control_Code">- управляющий код </param>
        /// <returns> фактическое значение тока </returns>
        public float Comand_40(int id_master, Byte[] id_slaiv, float Control_Code)
        {

            try
            {

                Byte[] te = _Convert.Float_tu_byte( Control_Code) ;

                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 40, te);


                return _Convert.Bute_tu_F(temp[0].DT);
               
                


            }
            catch
            {
                EROR = "READ_EROR";

                return -1;
            }
        }
        /// <summary>
        /// КОМАНДА № 42 PERFORM MASTER RESET
        /// </summary>
        /// <remarks>
        /// Немедленно отреагируйте, а затем перезагрузите микропроцессор.
        ///Выполнение этой команды может занять относительно длительный период времени для завершения. Возможно, устройство не сможет ответить на другую команду до завершения. Обратитесь к документу для конкретного передатчика для получения подробной информации о реализации.
        /// </remarks>
        /// <param name="id_master">- адрес отправителя </param>
        /// <param name="id_slaiv">- адрес получателя</param>
        /// <returns>true - если команда выполнилась успешно !(устройство может не успеть ответить) </returns>
        public bool Comand_42(int id_master, Byte[] id_slaiv)
        {

            try
            {

                Byte[] te = {};

                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 42, te);

                if (BitConverter.ToString(temp[0].DT) == BitConverter.ToString(te))
                {
                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch
            {
                EROR = "READ_EROR";

                return false;
            }
        }
        /// <summary>
        /// КОМАНДА № 43 УСТАНОВИТЬ ПЕРВИЧНУЮ ПЕРЕМЕННУЮ НОЛЬ 
        /// </summary>
        /// <remarks>
        /// Обрезать первичную переменную так, чтобы она читала Ноль с существующим процессом, примененным к устройству. Результирующее смещение должно быть в пределах, определенных каждым устройством.
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- адрес получателя</param>
        /// <returns>true - если команда выполнилась успешно </returns>
        public bool Comand_43(int id_master, Byte[] id_slaiv)
        {

            try
            {

                Byte[] te = { };

                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 43, te);

                if (BitConverter.ToString(temp[0].DT) == BitConverter.ToString(te))
                {
                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch
            {
                EROR = "READ_EROR";

                return false;
            }
        }
        /// <summary>
        /// КОМАНДА № 44 ЗАПИСАТЬ ПЕРВИЧНЫЙ ПЕРЕМЕННЫЕ ЕДИНИЦЫ
        /// </summary>
        /// <remarks>
        /// Выбирает единицы измерения, в которых будут возвращены первичная переменная и первичный диапазон переменных. Также будут выбраны единицы измерения для пределов первичного переменного датчика и минимального диапазона первичной переменной.
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- адрес получателя</param>
        /// <param name="Primary_Variable">- код единиц измирения</param>
        /// <returns> true - если команда выполнилась успешно</returns>
        public bool Comand_44(int id_master, Byte[] id_slaiv,int Primary_Variable)
        {

            try
            {

                Byte[] te = {Convert.ToByte((UInt32)Primary_Variable)};

                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 44, te);

                if (BitConverter.ToString(temp[0].DT) == BitConverter.ToString(te))
                {
                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch
            {
                EROR = "READ_EROR";

                return false;
            }
        }
     /// <summary>
     /// КОМАНДА №45 ПЕРВИЧНЫЙ ПЕРЕМЕННЫЙ ТОК ЦАП НОЛЬ
     /// </summary>
     /// <remarks>
     /// Обрежьте нулевую или нижнюю конечную точку первичного переменного аналогового выхода, чтобы ток на этом выходе был точно установлен на минимум. Эта подстройка обычно выполняется путем настройки соответствующего цифроаналогового преобразователя устройства от 4 до 20 миллиампер на 4,0 миллиампер. Значение, отправленное с помощью команды, может быть округлено или усечено устройством. Байты данных ответа содержат значение из запроса, которое используется устройством.
     ///Используйте команду № 40 «Вход / выход из режима фиксированного первичного переменного тока», чтобы установить ток на минимальное значение аналогового выхода первичной переменной, прежде чем использовать эту команду. Код ответа № 9 «Не в правильном режиме тока» будет возвращен, если режим фиксированного первичного переменного тока не был введен или ток не установлен точно на минимальное значение.
     /// </remarks>
     /// <param name="id_master">- адрес отправителя</param>
     /// <param name="id_slaiv">- адрес получателя </param>
     /// <param name="Externally_Measured">- значение в милиамперах</param>
     /// <returns>фактическое значение</returns>
        public float Comand_45(int id_master, Byte[] id_slaiv, float  Externally_Measured )
        {

            try
            {

                Byte[] te = _Convert.Float_tu_byte(Externally_Measured) ;

                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 45, te);


                return _Convert.Bute_tu_F(temp[0].DT);
               


            }
            catch
            {
                EROR = "READ_EROR";

                return -1;
            }
        }
        /// <summary>
        /// КОМАНДА № 46 ПЕРВИЧНАЯ ПЕРЕМЕННАЯ ТОКОВАЯ ЧАСТЬ ЦАП 
        /// </summary>
        /// <remarks>
        /// Обрезать усиление для верхней конечной точки первичного переменного аналогового выхода, чтобы ток на этом выходе был точно установлен на максимум. Эта подстройка обычно выполняется путем настройки соответствующего цифроаналогового преобразователя устройства от 4 до 20 миллиампер на 4,0 миллиампер. Значение, отправленное с командой, может быть округлено или усечено устройством. Байты данных ответа содержат значение из запроса, которое используется устройством.
        ///Используйте команду № 40 «Вход / выход из режима фиксированного первичного переменного тока», чтобы установить ток на максимальное значение аналогового выхода первичной переменной, прежде чем использовать эту команду. Код ответа № 9 «Не в правильном режиме тока» будет возвращен, если режим фиксированного первичного переменного тока не был введен или ток не установлен точно на максимальное значение.
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- адрес получателя </param>
        /// <param name="Externally_Measured">- значение в милиамперах</param>
        /// <returns>фактическое значение</returns>
        public float Comand_46(int id_master, Byte[] id_slaiv, float Externally_Measured)
        {

            try
            {

                Byte[] te = _Convert.Float_tu_byte(Externally_Measured);

                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 46, te);


                return _Convert.Bute_tu_F(temp[0].DT);



            }
            catch
            {
                EROR = "READ_EROR";

                return -1;
            }
        }
        /// <summary>
        /// КОМАНДА № 47 ЗАПИСЬ ПЕРВИЧНОЙ ПЕРЕМЕННОЙ ФУНКЦИИ ПЕРЕДАЧИ 
        /// </summary>
        /// <remarks>
        /// Выберите функцию передачи для основного переменного аналогового выхода устройства.
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- адрес получателя </param>
        /// <param name="TRANSFER">- код функции </param>
        /// <returns>true - если команда успешно выполнилась</returns>
        public bool Comand_47(int id_master, Byte[] id_slaiv, int TRANSFER)
        {

            try
            {

                Byte[] te = {Convert.ToByte(TRANSFER)};

                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 47, te);


                if (BitConverter.ToString(temp[0].DT) == BitConverter.ToString(te))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch
            {
                EROR = "READ_EROR";

                return false;
            }
        }
        /// <summary>
        /// КОМАНДА № 49 ЗАПИСАТЬ ПЕРВИЧНЫЙ ПЕРЕМЕННЫЙ ДАТЧИК СЕРИЙНОГО НОМЕРА
        /// </summary>
        /// <remarks>
        /// Записывает серийный номер датчика, связанный с первичной переменной.
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- адрес получателя</param>
        /// <param name="num">- 3 байта для записи !(записывать можно только 3 байта иначе выдаст ошибку)</param>
        /// <returns> true - если команда выполнилась успешно</returns>
        public bool Comand_49(int id_master, Byte[] id_slaiv, Byte[] num)
        {

            try
            {

                Byte[] te = { num[0], num[1], num[2] };

                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 49, te);


                if (BitConverter.ToString(temp[0].DT) == BitConverter.ToString(te))
                {
                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch
            {
                EROR = "READ_EROR";
                return false;
            }
        }
        /// <summary>
        /// КОМАНДА № 59 ЗАПИСАТЬ ЧИСЛО ПРЕАМБУЛ ОТВЕТА 
        /// </summary>
        /// <remarks>
        /// Это команда управления канального уровня.
        ///Эта команда выбирает минимальное количество преамбул, которые должны быть отправлены устройством до запуска ответного пакета. В это число входят две преамбулы, содержащиеся в начале сообщения.
        ///Обычно устройство позволяет выбрать от 2 до 20 ответных преамбул. Некоторые устройства не могут реализовать эти ограничения. Обратитесь к документу для конкретного передатчика, чтобы определить фактические пределы для каждого типа устройства.
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- адрес получателя</param>
        /// <param name="Preambul_leng">- количество приамбул </param>
        /// <returns> true - если команда выполнилась успешно</returns>       
        public bool Comand_59(int id_master, Byte[] id_slaiv, int Preambul_leng)
        {

            try
            {

                Byte[] te = { Convert.ToByte((UInt32)Preambul_leng) };

                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 59, te);

                if (BitConverter.ToString(temp[0].DT) == BitConverter.ToString(te))
                {
                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch
            {
                EROR = "READ_EROR";

                return false;
            }
        }
       //=======================================================================================================================
        /*! @} */
        //=================< Декодирывание (Таблицы) >=====================================================================================
       
        public string Alarm_Cods(int cod)
        {
            switch (cod)
            {
               
                case 0: return "High";
                case 1: return "Low";
                case 239: return "Hold Last";
                case 240:
                case 241:
                case 242:
                case 243:
                case 244:
                case 245:
                case 246:
                case 247:
                case 248:
                case 249:
                return cod.ToString();
                case 250: return "Not Used";
                case 251: return "None";
                case 252: return "Unclown";
                case 253: return "Special";
            }
            return "-1";
        }
        public int Alarm_Cods(string cod)
        {
            switch (cod)
            {
                
                case "High": return 0;
                case "Low": return 1;
                case "Hold Last": return 239;
                case "Not Used": return 250;
                case "None": return 251;
                case "Unclown": return 252;
                case "Special": return 253;
            }
            return Convert.ToInt32(cod);
        }
        public string Transfer_Cods(int cod)
        {
            switch (cod)
            {
               
                case 0: return "Линейная (y=mx+b)";
                case 1: return "Квадратный корень (y=sqrt(x))";
                case 2: return "Квадратный корень 3 порядка (y=sqrt(x^3))";
                case 3: return "Квадратный корень 5 порядка (y=sqrt(x^5))";
                case 4: return "Специальная кривая";
                case 5: return "Возведение в степинь (y=x^2)";
                case 230: return "Дискретная (on/off)";
                case 231: return "Квадратный корень + Специальная кривая";
                case 232: return "Квадратный корень 3 порядка + Специальная кривая";
                case 233: return "Квадратный корень 5 порядка + Специальная кривая"; 
                case 240:
                case 241:
                case 242:
                case 243:
                case 244:
                case 245:
                case 246:
                case 247:
                case 248:
                case 249:
                    return cod.ToString();
                case 250: return "Not Used";
                case 251: return "None";
                case 252: return "Unclown";
                case 253: return "Special";
            }
            return "-1";
        }
        public int Transfer_Cods(string cod)
        {
            switch (cod)
            {
            
                case "Линейная (y=mx+b)": return 0;
                case "Квадратный корень (y=sqrt(x))": return 1;
                case "Квадратный корень 3 порядка (y=sqrt(x^3))": return 2;
                case "Квадратный корень 5 порядка (y=sqrt(x^5))": return 4;
                case "Специальная кривая": return 4;
                case "Возведение в степинь (y=x^2)": return 5;
                case "Дискретная (on/off)": return 230;
                case "Квадратный корень + Специальная кривая": return 231;
                case "Квадратный корень 3 порядка + Специальная кривая": return 232;
                case "Квадратный корень 5 порядка + Специальная кривая": return 233;
                case "Not Used": return 250;
                case "None": return 251;
                case "Unclown": return 252;
                case "Special": return 253;
              
            }
            return Convert.ToInt32(cod);
        }
        public string Protect_Cods(int cod)
        {
            switch (cod)
            {
           
                case 0: return "No";
                case 1: return "Yes";
                case 250: return "Not Used";
                case 251: return "None";
                case 252: return "Unclown";
                case 253: return "Special";
            }
            return "-1";
        }
        public int Protect_Cods(string cod)
        {
            switch (cod)
            {
               
                case "No": return 0;
                case "Yes": return 1;
                case "Not Used": return 250;
                case "None": return 251;
                case "Unclown": return 252;
                case "Special": return 253;
            }
            return -1;
        }
        private string[] Encod_unit_array = new string[255];
        public string Encod_unit(int kod)
        {
           
            try
            {
                return Encod_unit_array[kod];
            }
            catch
            {
                return "";
            }
        }
        public int Encod_unit(string kod)
        {
         
            return Array.IndexOf( Encod_unit_array ,kod);
        }
        public string[] unit_array()
        {
            string[] temp = {};
            for (int i = 0; i < Encod_unit_array.Length; i++)
            {
                if ((Encod_unit_array[i] != "") && (Encod_unit_array[i] != null))
                {
                    Array.Resize(ref temp, temp.Length + 1);
                    temp[temp.Length - 1] = Encod_unit_array[i];
                }
            }
            return temp;
        }
        private void init_Encod_unit()
        {
           
                // ==== давление ======
                Encod_unit_array[1] = "Дюймы воды при 68°F";
                Encod_unit_array[2] = "Дюймы ртути при 0°С";
                Encod_unit_array[ 3] = "";
                Encod_unit_array[ 4] = "Милеметры воды при 68°F";
                Encod_unit_array[ 5] = "Милеметры ртути при 0°С";
                Encod_unit_array[ 6] = "psi";
                Encod_unit_array[ 7] = "бар";
                Encod_unit_array[ 8] = "миллибар";
                Encod_unit_array[ 9] = "gf/cm²";
                Encod_unit_array[ 10] = "kgf/cm²";
                Encod_unit_array[ 11] = "Па";
                Encod_unit_array[ 12] = "кПа";
                Encod_unit_array[ 13] = "торр";
                Encod_unit_array[ 14] = "атм";
                Encod_unit_array[ 145] = "";
                Encod_unit_array[ 237] = "МПа";
                Encod_unit_array[ 238] = "";
                Encod_unit_array[ 239] = "";
                // ==== температуры ===
                Encod_unit_array[ 32] = "°C";
                Encod_unit_array[ 33] = "°F";
                Encod_unit_array[ 34] = "°R";
                Encod_unit_array[ 35] = "°K";
                // ==== Количество ========== 
                Encod_unit_array[ 15] = "cubic feet per minute";   
                Encod_unit_array[ 16] = "gallons per minute";   
                Encod_unit_array[ 17] = "liters per minute";   
                Encod_unit_array[ 18] = "imperial gallons per minute";   
                Encod_unit_array[ 19] = "cubic meter per hour";   
                Encod_unit_array[ 22] = "gallons per second";  
                Encod_unit_array[ 23] = "million gallons per day";   
                Encod_unit_array[ 24] = "liters per second";   
                Encod_unit_array[ 25] = "million liters per day";   
                Encod_unit_array[ 26] = "cubic feet per second";   
                Encod_unit_array[ 27] = "cubic feet per day";   
                Encod_unit_array[ 28] = "cubic meters per second";   
                Encod_unit_array[ 29] = "cubic meters per day";   
                Encod_unit_array[ 30] = "imperial gallons per hour";   
                Encod_unit_array[ 31] = "imperial gallons per day";  
                Encod_unit_array[ 121] = "normal cubic meter per hour";
                Encod_unit_array[ 122] = "normal liter per hour";  
                Encod_unit_array[ 123] = "standard cubic feet per minute"; 
                Encod_unit_array[ 130] = "cubic feet per hour";  
                Encod_unit_array[ 131] = "cubic meters per minute";  
                Encod_unit_array[ 132] = "barrels per second"; 
                Encod_unit_array[ 133] = "barrels per minute";  
                Encod_unit_array[ 134] = "barrels per hour";  
                Encod_unit_array[ 135] = "barrels per day"; 
                Encod_unit_array[ 136] = "gallons per hour";   
                Encod_unit_array[ 137] = "imperial gallons per second";   
                Encod_unit_array[ 138] = "liters per hour";  
                Encod_unit_array[ 235] = "gallons per day"; 
                // ==== скорость =======================
                Encod_unit_array[ 20] = "feet per second";  
                Encod_unit_array[ 21] = "meters per second"; 
                Encod_unit_array[ 114] = "inches per second"; 
                Encod_unit_array[ 115] = "inches per minute";  
                Encod_unit_array[ 116] = "feet per minute";
                Encod_unit_array[ 120] = "meters per hour";    
                // ==== обем ===========================
                Encod_unit_array[ 40] = "gallons";  
                Encod_unit_array[ 41] = "liters";  
                Encod_unit_array[ 42] = "imperial gallons";
                Encod_unit_array[ 43] = "cubic meters";   
                Encod_unit_array[ 46] = "barrels";  
                Encod_unit_array[ 110] = "bushels";  
                Encod_unit_array[ 111] = "cubic yards"; 
                Encod_unit_array[ 112] = "cubic feet";   
                Encod_unit_array[ 113] = "cubic inches";  
                Encod_unit_array[ 124] = "bbl liq";  
                Encod_unit_array[ 166] = "normal cubic meter";  
                Encod_unit_array[ 167] = "normal liter";
                Encod_unit_array[ 168] = "standard cubic feet";
                Encod_unit_array[ 236] = "hectoliters"; 
                // ==== растояние ============================
                Encod_unit_array[ 44] = "feet";  
                Encod_unit_array[ 45] = "meters";  
                Encod_unit_array[ 47] = "inches";  
                Encod_unit_array[ 48] = "centimeters";
                Encod_unit_array[ 49] = "millimeters"; 
                // ==== Время ================================
                Encod_unit_array[ 50] = "minutes"; 
                Encod_unit_array[ 51] = "seconds";
                Encod_unit_array[ 52] = "hours";  
                Encod_unit_array[ 53] = "days";   
                // ==== Масса ================================
                Encod_unit_array[ 60] = "grams";   
                Encod_unit_array[ 61] = "kilograms";   
                Encod_unit_array[ 62] = "metric tons";   
                Encod_unit_array[ 63] = "pounds";    
                Encod_unit_array[ 64] = "short tons";   
                Encod_unit_array[ 65] = "long tons";
                Encod_unit_array[ 125] = "ounce"; 
                // ==== Масса за время ========================
                Encod_unit_array[ 70] = "grams per second";   
                Encod_unit_array[ 71] = "grams per minute";  
                Encod_unit_array[ 72] = "grams per hour";   
                Encod_unit_array[ 73] = "kilograms per second";   
                Encod_unit_array[ 74] = "kilograms per minute";   
                Encod_unit_array[ 75] = "kilograms per hour";   
                Encod_unit_array[ 76] = "kilograms per day";  
                Encod_unit_array[ 77] = "metric tons per minute"; 
                Encod_unit_array[ 78] = "metric tons per hour";  
                Encod_unit_array[ 79] = "metric tons per day";   
                Encod_unit_array[ 80] = "pounds per second";  
                Encod_unit_array[ 81] = "pounds per minute";
                Encod_unit_array[ 82] = "pounds per hour"; 
                Encod_unit_array[ 83] = "pounds per day";
                Encod_unit_array[ 84] = "short tons per minute";  
                Encod_unit_array[ 85] = "short tons per hour";   
                Encod_unit_array[ 86] = "short tons per day";   
                Encod_unit_array[ 87] = "long tons per hour";
                Encod_unit_array[ 88] = "long tons per day";   
                // ==== Масса на обем =========================
               Encod_unit_array[ 90] = "specific gravity units";    
               Encod_unit_array[ 91] = "grams per cubic centimeter";    
               Encod_unit_array[ 92] = "kilograms per cubic meter";    
               Encod_unit_array[ 93] = "pounds per gallon";     
               Encod_unit_array[ 94] = "pounds per cubic foot";  
               Encod_unit_array[ 95] = "grams per milliliter";    
               Encod_unit_array[ 96] = "kilograms per liter";    
               Encod_unit_array[ 97] = "grams per liter";    
               Encod_unit_array[ 98] = "pounds per cubic inch";   
               Encod_unit_array[ 99] = "short tons per cubic yard";    
               Encod_unit_array[ 100] = "degrees twaddell";    
               Encod_unit_array[ 102] = "degrees baume heavy";  
               Encod_unit_array[ 103] = "degrees baume light";    
               Encod_unit_array[ 104] = "degrees API";    
               Encod_unit_array[ 146] = "micrograms per liter";
               Encod_unit_array[ 147] = "micrograms per cubic meter";    
               // ==== Вязкость ================================
               Encod_unit_array[ 54] = "centistokes";
               Encod_unit_array[ 55] = "centipoise";
               // ==== Электроника ===============================
               Encod_unit_array[ 36] = "millivolts";   
               Encod_unit_array[ 58] = "volts";   
               Encod_unit_array[ 39] = "milliamperes";  
               Encod_unit_array[ 37] = "ohms";
               Encod_unit_array[ 163] = "kohms";
               // ==== Энергия ===================================
               Encod_unit_array[ 69] = "newton meter";
               Encod_unit_array[ 89] = "deka therm";
               Encod_unit_array[ 126] = "foot pound force";
               Encod_unit_array[ 128] = "kilo watt hour"; 
               Encod_unit_array[ 162] = "mega calorie";
               Encod_unit_array[ 165] = "british thermal unit";
               // ==== сила =======================================
               Encod_unit_array[ 127] = "kilo watt";  
               Encod_unit_array[ 129] = "horsepower";
               Encod_unit_array[ 140] = "mega calorie per hour";
               Encod_unit_array[ 142] = "british thermal unit per hour"; 
               // ==== Радиальная скорость =========================
               Encod_unit_array[ 117] = "degrees per second";  
               Encod_unit_array[ 118] = "revolutions per second";
               Encod_unit_array[ 119] = "revolutions per minute";
               // ==== Различное =================================
               Encod_unit_array[ 38] = "hertz";  
               Encod_unit_array[ 56] = "microsiemens";  
               Encod_unit_array[ 57] = "%"; 
               Encod_unit_array[ 59] = "pH";  
               Encod_unit_array[ 66] = "milli siemens per centimeter";  
               Encod_unit_array[ 67] = "micro siemens per centimeter"; 
               Encod_unit_array[ 68] = "newton  101 degrees brix"; 
               Encod_unit_array[ 105] = "percent solids per weight";
               Encod_unit_array[ 106] = "percent solids per volume"; 
               Encod_unit_array[ 107] = "degrees balling"; 
               Encod_unit_array[ 108] = "proof per volume"; 
               Encod_unit_array[ 109] = "proof per mass"; 
               Encod_unit_array[ 139] = "parts per million";
               Encod_unit_array[ 143] = "degrees"; 
               Encod_unit_array[ 144] = "radian"; 
               Encod_unit_array[ 148] = "percent consistency"; 
               Encod_unit_array[ 149] = "volume percent"; 
               Encod_unit_array[ 150] = "percent steam quality"; 
               Encod_unit_array[ 151] = "feet in sixteenths"; 
               Encod_unit_array[ 152] = "cubic feet per pound";
               Encod_unit_array[ 153] = "picofarads"; 
               Encod_unit_array[ 154] = "mililiters per liter"; 
               Encod_unit_array[ 155] = "microliters per liter";  
               Encod_unit_array[ 160] = "percent plato";  
               Encod_unit_array[ 161] = "percent lower explosion level";
               Encod_unit_array[ 169] = "parts per billion"; 
               // ==== общии ============================================
              Encod_unit_array[ 240] = "Enumeration may be used for manufacturer specific definitions";  
              Encod_unit_array[ 250] = "Not Used";  
              Encod_unit_array[ 251] = "None";   
              Encod_unit_array[ 252] = "Unknown";  
              Encod_unit_array[ 253] = "Special"; 
                  
           
            
        }
       //=======================================================================================================================
        /*! \addtogroup <Write> [(Отправка даных)]
             * \brief Данный модуль содержит список функцый отправки.
          @{
            */
        private void Scan(object upperCase)
        {
            int upper = (int)upperCase;
           lock_ = false;
           scan = true;
             Byte[] a = {};          
            for (int j = 0; j <= 15; j++)
            {
                scan_step = j;
                Read_Fraim[] temp = Write_short(8, upper, j, 0, a);

                for (int i = 0; i < temp.Length; i++)
                {
                    Array.Resize(ref  Read, Read.Length + 1);
                    Read[Read.Length - 1].AD_master = temp[i].AD_master;
                    Read[Read.Length - 1].BC = temp[i].BC;
                    Read[Read.Length - 1].CD = temp[i].CD;
                    Read[Read.Length - 1].CHK = temp[i].CHK;
                    Read[Read.Length - 1].DT = temp[i].DT;
                    Read[Read.Length - 1].SD = temp[i].SD;
                    Read[Read.Length - 1].ST = temp[i].ST;
                    Read[Read.Length - 1].AD_Short = (byte)j;
                    Read[Read.Length - 1].AD_slaiv = new Byte[] { temp[i].DT[0], temp[i].DT[1], temp[i].DT[8], temp[i].DT[9], temp[i].DT[10] };
                    Read[Read.Length - 1].temp = temp[i].temp;
                }
                if (stop_skan == true) {
                    stop_skan = false;
                    break;
                }
            }
            scan = false;
            lock_ = true;
        }
        public Read_Fraim[] Net_config()
        {
            return Read;
        }
        public void Scan_start(int id_master)
        {
            stop_skan = false;
            if (lock_)
            {
            Read_Fraim[] tmp = { };
            Read = tmp;
            Thread t = new Thread(Scan);
            t.Start(id_master);   
            } 
        }
        public void Scan_stop()
        {
            stop_skan = true;
        }

        /// <summary>
        /// отправить короткий фреим 
        /// </summary>
        /// <remarks>
        /// формирует из входных даных кадр короткого формата после чего отправляет кадр (функция  Write();)
        /// после получения даных отправки разбиваит их согласно структуре короткого кадра и упаковывает в 
        /// структуры Read_Fraim формируя из них масив ;
        /// расшифровует поля статуса .
        /// </remarks>
        /// <param name="preambula">- длина приамбулы в запросе</param>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- короткий адрес получателя</param>
        /// <param name="comand">- номер команды</param>
        /// <param name="data">- поле с данымми </param>
        /// <returns> масив типа Read_Fraim </returns>
        public Read_Fraim[] Write_short(int preambula, int id_master, int id_slaiv, int comand, Byte[] data)
        {
            //=========================- Формировка короткого кадра -===================================================
            Byte[] write = {0}; //переменая для кадра

            for (int i = 0; i < preambula; i++) // Формировка преамбулы 
            {
                write[i] = 0xFF;
                Array.Resize(ref write, write.Length + 1);
            }
            
            write[write.Length - 1] = 0x02; // Delimiter
            Array.Resize(ref write, write.Length + 1);

            id_master = (byte)(id_master * 0x80); // Adres
            write[write.Length - 1] = (byte)(id_master + id_slaiv);

            Array.Resize(ref write, write.Length + 1); // comand 
            write[write.Length - 1] = (byte)comand;

            Array.Resize(ref write, write.Length + 1); // byte count
            write[write.Length - 1] = (byte)data.Length;

            for (int i = 0; i < data.Length; i++)   // data
            {
                Array.Resize(ref write, write.Length + 1);
                write[write.Length - 1] = data[i];      
            }

            Array.Resize(ref write, write.Length + 1);
            write[write.Length - 1] = XOR_calc(write);   // Check Byte  
            //=====================- Конец формировки кадра -===============================================================================
          

            Byte[][] read;

                    read = Write(write);
             List<Byte[]> test = new List<Byte[]>(read);
              for (int j = 0; j < read.Length - 1; j++)
                {
                    XOR_Chec(read[j]);
                    if (EROR != "True")
                    {
                       
                        test.RemoveAt(j);
                    }
                  
                }

              read = test.ToArray();

              Read_Fraim[] Fraim = new Read_Fraim[read.Length];
              for (int i = 0; i < read.Length; i++)
              {
                  Fraim[i].SD = read[i][0];
                  if (Fraim[i].SD == 0x06)
                  {
                      byte temp = 0;
                      Fraim[i].AD_master = (byte)((read[i][1] >> 7) & 1);
                      for (int j = 0 ;j<7;j++) temp += (byte)((read[i][1] >> j) & 1);
                      Fraim[i].AD_slaiv = new Byte[] { temp }; ;
                  } 
                  Fraim[i].CD = read[i][2];
                  Fraim[i].BC = read[i][3];
                  Byte[] temp1 = {0,0};
                  Byte[] temp2 = {};
                  for (int j = 4; j <(int)Fraim[i].BC+4; j++)
                  {
                      if (j == 4) { temp1[0] = read[i][j]; }
                      if (j == 5) { temp1[1] = read[i][j]; }
                      if (j > 5)
                      {
                          Array.Resize(ref temp2, temp2.Length + 1);
                          temp2[temp2.Length - 1] = read[i][j]; ;   
                      }
                  }
                  Fraim[i].ST = temp1;
                  Fraim[i].DT = temp2;
                  Fraim[i].temp = id_slaiv.ToString();
                  Fraim[i].CHK = read[i][read[i].Length-1];
                    Fraim[i].Statys_1 = "";
                if (Fraim[i].ST[0] >> 7 == 1)
                {
                    Fraim[i].Statys_1 = "Ошибка Передачи Даных : \n";
                    bool[] st = _Convert.Byte_tu_B(Fraim[i].ST[0]);
                
                    for (int j = 0; j < st.Length; j++)
                    {
                        if (st[j] == true)
                        {
                            switch (j)
                            {
                                case 0: Fraim[i].Statys_1 += "Неопредилен"; break;
                                case 1: Fraim[i].Statys_1 += "Переполнин буфер приемника "; break;
                                case 2: Fraim[i].Statys_1 += ""; break;
                                case 3: Fraim[i].Statys_1 += "Ошибка конторльной суммы "; break;
                                case 4: Fraim[i].Statys_1 += "Ошибка формирования фрейма"; break;
                                case 5: Fraim[i].Statys_1 += "Ошибка пререполнения"; break;
                                case 6: Fraim[i].Statys_1 += "Оштбка по четности"; break;
                            }
                        }
                    }
                }
                if (Fraim[i].ST[0] >> 7 == 0)
                {
                    Fraim[i].Statys_1 = "Ошибка Команды: " + '\n';
                    if ((int)Fraim[i].ST[0] == 0) { Fraim[i].Statys_1 += "ошибка не характерная для команд"; }
                    if ((int)Fraim[i].ST[0] == 1) { Fraim[i].Statys_1 += "(не определино)"; }
                    if ((int)Fraim[i].ST[0] == 2) { Fraim[i].Statys_1 += "Неверный выбор"; }
                    if ((int)Fraim[i].ST[0] == 3) { Fraim[i].Statys_1 += "переданый параметр слишком велик"; }
                    if ((int)Fraim[i].ST[0] == 4) { Fraim[i].Statys_1 += "переданый параметр слишком мал"; }
                    if ((int)Fraim[i].ST[0] == 5) { Fraim[i].Statys_1 += "получено слишком мало байт даных"; }
                    if ((int)Fraim[i].ST[0] == 6) { Fraim[i].Statys_1 += "ошибка команды , спецефической для датчика"; }
                    if ((int)Fraim[i].ST[0] == 7) { Fraim[i].Statys_1 += "в режиме защищоном от записи"; }
                    if ((int)Fraim[i].ST[0] >= 8 && Fraim[i].ST[0] <= 15) { Fraim[i].Statys_1 += "ошибки характерные для команд"; }
                    if ((int)Fraim[i].ST[0] == 16) { Fraim[i].Statys_1 += "доступ ограничен"; }
                    if ((int)Fraim[i].ST[0] == 32) { Fraim[i].Statys_1 += "устройсво занято"; }
                    if ((int)Fraim[i].ST[0] == 64) { Fraim[i].Statys_1 += "команда не задействона"; }
                }
                bool[] st2 = _Convert.Byte_tu_B(Fraim[i].ST[1]);
              Fraim[i].Statys_2 = "Ошибка Прибора: " + '\n';
              for (int j = 0; j < st2.Length; j++)
              {
                  if (st2[j] == true)
                  {
                      switch (j) {
                          case 0: Fraim[i].Statys_2 += "Первичная пременая вышла за ограничения" + '\n'; break;
                          case 1: Fraim[i].Statys_2 += "Переменая (не первичная ) вышла за ограничения " + '\n'; break;
                          case 2: Fraim[i].Statys_2 += "насыщение аналогово выходного сигнала " + '\n'; break;
                          case 3: Fraim[i].Statys_2 += "Фиксирываный выходной ток " + '\n'; break;
                          case 4: Fraim[i].Statys_2 += ""; break;
                          case 5: Fraim[i].Statys_2 += "Холодный старт" + '\n'; break;
                          case 6: Fraim[i].Statys_2 += "Изменина конфигураця" + '\n'; break;
                          case 7: Fraim[i].Statys_2 += "Неисправность прибора " + '\n'; break; 
                      }
                  }
              }
            }
            if(Fraim.Length>0)
            last_fraim = Fraim[0];
              

            return Fraim;

        }
        /// <summary>
        /// отправить длиный фреим 
        /// </summary>
        /// <remarks>
        /// формирует из входных даных кадр длиного формата после чего отправляет кадр (функция  Write();)
        /// после получения даных отправки разбиваит их согласно структуре короткого кадра и упаковывает в 
        /// структуры Read_Fraim формируя из них масив ;
        /// расшифровует поля статуса .
        /// </remarks>
        /// <param name="preambula">- длина приамбулы в запросе</param>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- длиный адрес получателя</param>
        /// <param name="comand">- номер команды</param>
        /// <param name="data">- поле с данымми </param>
        /// <returns> масив типа Read_Fraim </returns>
        public Read_Fraim[] Write_long(int preambula, int id_master, Byte[] id_slaiv, int comand, Byte[] data)
        {
            //=========================- Формировка длиного кадра -===================================================
            Byte[] write = { 0 }; //переменая для кадра

            for (int i = 0; i < preambula; i++) // Формировка преамбулы 
            {
                write[i] = 0xFF;
                Array.Resize(ref write, write.Length + 1);
            }

            write[write.Length - 1] = 0x82; // Delimiter
            id_master = (byte)(id_master * 0x80); // Adres

            Array.Resize(ref write, write.Length + 1);
            write[write.Length - 1] = (Byte)(id_master + id_slaiv[0]);
            for (int i = 2; i < id_slaiv.Length+1; i++)
            {
                Array.Resize(ref write, write.Length + 1);
                write[write.Length - 1] = (Byte)(id_slaiv[i-1]);
            }
                Array.Resize(ref write, write.Length + 1); // comand 
            write[write.Length - 1] = (byte)comand;
            Array.Resize(ref write, write.Length + 1); // byte count
            write[write.Length - 1] = (byte)data.Length;

            for (int i = 0; i < data.Length; i++)   // data
            {
                Array.Resize(ref write, write.Length + 1);
                write[write.Length - 1] = data[i];
            }

            Array.Resize(ref write, write.Length + 1);
            write[write.Length - 1] = XOR_calc(write);   // Check Byte  
            //=====================- Конец формировки кадра -===============================================================================


            Byte[][] read;

            read = Write(write);
            List<Byte[]> test = new List<Byte[]>(read);
            for (int j = 0; j < read.Length; j++)
            {
                Byte[] tm = { };
                Byte[] tm2 = { };
                XOR_Chec(read[j]);
                if (EROR != "True")
                {

                    test.RemoveAt(j);
                }
                      for (int i = 0; i < read[j].Length; i++)
                {
                    if (read[j][0] == 0x86){
                        if ((i >= 1) && (i <= 6))
                        {
                            Array.Resize(ref tm , tm.Length+1);
                            tm[tm.Length - 1] = read[j][i];
                        }
                    }
                }
                for (int i = 0; i < write.Length; i++)
                {
                    
                        if ((i >= preambula + 1) && (i <= preambula+6))
                        {
                            Array.Resize(ref tm2 , tm2.Length+1);
                            tm2[tm2.Length - 1] = write[i];
                        }
                    
                }
                if (BitConverter.ToString(tm) != BitConverter.ToString(tm2))
                  {
                    test.RemoveAt(j);
                  }
            }

            read = test.ToArray();

            Read_Fraim[] Fraim = new Read_Fraim[read.Length];
            for (int i = 0; i < read.Length; i++)
            {
                Fraim[i].SD = read[i][0];
                if (Fraim[i].SD == 0x86)
                {
                    byte temp = 0;
                    Fraim[i].AD_master = (byte)((read[i][1] >> 7));
                     temp = (byte)((read[i][1]) & 0x7F);
                    Fraim[i].AD_slaiv = new Byte[] { temp }; 

                    for (int h = 2; h < 6; h++)
                    {
                        temp = 0;
                        temp = (byte)(read[i][h]);
                        Array.Resize(ref  Fraim[i].AD_slaiv, Fraim[i].AD_slaiv.Length + 1);
                        Fraim[i].AD_slaiv[Fraim[i].AD_slaiv.Length - 1] = temp; 
                    }
                }
                Fraim[i].CD = read[i][6];
                Fraim[i].BC = read[i][7];
                Byte[] temp1 = { 0, 0 };
                Byte[] temp2 = { };
                for (int j = 8; j < (int)Fraim[i].BC + 8; j++)
                {
                    if (j == 8) { temp1[0] = read[i][j]; }
                    if (j == 9) { temp1[1] = read[i][j]; }
                    if (j > 9)
                    {
                        Array.Resize(ref temp2, temp2.Length + 1);
                        temp2[temp2.Length - 1] = read[i][j]; ;
                    }
                }
                Fraim[i].ST = temp1;
                Fraim[i].DT = temp2;
                Fraim[i].CHK = read[i][read[i].Length - 1];
                Fraim[i].Statys_1 = "";
                if (Fraim[i].ST[0] >> 7 == 1)
                {
                    Fraim[i].Statys_1 = "Ошибка Передачи Даных : \n";
                    bool[] st = _Convert.Byte_tu_B(Fraim[i].ST[0]);
                
                    for (int j = 0; j < st.Length; j++)
                    {
                        if (st[j] == true)
                        {
                            switch (j)
                            {
                                case 0: Fraim[i].Statys_1 += "Неопредилен"; break;
                                case 1: Fraim[i].Statys_1 += "Переполнин буыер приемника "; break;
                                case 2: Fraim[i].Statys_1 += ""; break;
                                case 3: Fraim[i].Statys_1 += "Ошибка конторльной суммы "; break;
                                case 4: Fraim[i].Statys_1 += "Ошибка формирования фрейма"; break;
                                case 5: Fraim[i].Statys_1 += "Ошибка пререполнения"; break;
                                case 6: Fraim[i].Statys_1 += "Оштбка по четности"; break;
                            }
                        }
                    }
                }
                if (Fraim[i].ST[0] >> 7 == 0)
                {
                    Fraim[i].Statys_1 = "Ошибка Команды: " + '\n';
                    if ((int)Fraim[i].ST[0] == 0) { Fraim[i].Statys_1 += "ошибка не характерная для команд"; }
                    if ((int)Fraim[i].ST[0] == 1) { Fraim[i].Statys_1 += "(не определино)"; }
                    if ((int)Fraim[i].ST[0] == 2) { Fraim[i].Statys_1 += "Неверный выбор"; }
                    if ((int)Fraim[i].ST[0] == 3) { Fraim[i].Statys_1 += "переданый параметр слишком велик"; }
                    if ((int)Fraim[i].ST[0] == 4) { Fraim[i].Statys_1 += "переданый параметр слишком мал"; }
                    if ((int)Fraim[i].ST[0] == 5) { Fraim[i].Statys_1 += "получено слишком мало байт даных"; }
                    if ((int)Fraim[i].ST[0] == 6) { Fraim[i].Statys_1 += "ошибка команды , спецефической для датчика"; }
                    if ((int)Fraim[i].ST[0] == 7) { Fraim[i].Statys_1 += "в режиме защищоном от записи"; }
                    if ((int)Fraim[i].ST[0] >= 8 && Fraim[i].ST[0] <= 15) { Fraim[i].Statys_1 += "ошибки характерные для команд"; }
                    if ((int)Fraim[i].ST[0] == 16) { Fraim[i].Statys_1 += "доступ ограничен"; }
                    if ((int)Fraim[i].ST[0] == 32) { Fraim[i].Statys_1 += "устройсво занято"; }
                    if ((int)Fraim[i].ST[0] == 64) { Fraim[i].Statys_1 += "команда не задействона"; }
                }
                bool[] st2 = _Convert.Byte_tu_B(Fraim[i].ST[1]);
              Fraim[i].Statys_2 = "Ошибка Прибора: " + '\n';
              for (int j = 0; j < st2.Length; j++)
              {
                  if (st2[j] == true)
                  {
                      switch (j) {
                          case 0: Fraim[i].Statys_2 += "Первичная пременая вышла за ограничения"; break;
                          case 1: Fraim[i].Statys_2 += "Переменая (не первичная ) вышла за ограничения "; break;
                          case 2: Fraim[i].Statys_2 += "насыщение аналогово выходного сигнала "; break;
                          case 3: Fraim[i].Statys_2 += "Фиксирываный выходной ток "; break;
                          case 4: Fraim[i].Statys_2 += ""; break;
                          case 5: Fraim[i].Statys_2 += "Холодный старт"; break;
                          case 6: Fraim[i].Statys_2 += "Изменина конфигураця"; break; 
                          case 7: Fraim[i].Statys_2 += "Неисправность прибора "; break; 
                      }
                  }
              }
            }
            last_fraim = Fraim[0];

            return Fraim;

        }
        /*! @} */
    }
}
