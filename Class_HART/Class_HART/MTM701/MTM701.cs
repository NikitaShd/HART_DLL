using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_HART
{
    public partial class Conect
    {
        // описание команд для MTM701
        static public string[] MTM701_Cods_Pressure = new string[]{ // Коды единиц измерения давления
        "Па","кПа","МПа","гс/см2","кгс/см2","мм.рт.ст.","мм.вод.ст","psi","бар","Мбар"
        };

        static public string[] MTM701_Cods_TempSignal = new string[]{ //Коды источников сигнала температуры
        "Сенсор встроенный в МК","Сенсор давления питающийся напряжением","Сенсор давления питающийся током"
        };

        /// <summary>
        /// Считать измеренные коды АЦП 
        /// </summary>
        /// <remarks>
        /// Считывает измеренные коды АЦП для температуры, давления и выходного сигнала постоянного тока.
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- длиный адрес получателя</param>
        /// <param name="res">- Поле данных ответа
        /// [0]Код АЦП с датчика давления, сигнал давления,
        /// [1]Kод АЦП со встроенного в МК датчика температуры,
        /// [2]Kод АЦП со встроенного в МК датчика температуры,
        /// [3]Kод АЦП со встроенного в МК датчика температуры,
        /// [4]Kод АЦП со встроенного в МК датчика температуры
        /// </param>
        /// <returns> void </returns>
        public void MTM701_Comand_130(int id_master, Byte[] id_slaiv, ref string[] res)
        {
            Array.Resize(ref res, 5);
            try
            {
                Read_Fraim[] temp = Write_long(preambula_leng, id_master, id_slaiv, 130, new Byte[] { });
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
                EROR = "EROR";
            }
        }

        /// <summary>
        /// Считать/записать значение контрастности дисплея
        /// </summary>
        /// <remarks>
        /// Считывает или записывает уровень контрастности дисплея.
        ///Значение представлено в виде целого без знакового числа в диапазоне: от 0 до 63.
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- длиный адрес получателя</param>
        /// <param name="kod">- Коды операций
        /// 0-Прочитать данные
        /// 1-Записать данные
        /// </param>
        /// <param name="display">- Значение контрастности дисплея (от 0 до 63) </param>
        /// <returns> void </returns>
        public void MTM701_Comand_131(int id_master, Byte[] id_slaiv, int kod ,ref int display)
        {
            try
            {
                Read_Fraim[] temp ;
                if (kod == 0)
                {
                   temp = Write_long(preambula_leng, id_master, id_slaiv, 131, new Byte[] { (byte)kod });
                   display = temp[0].DT[1];
                }
                else if (kod == 1)
                {
                   temp = Write_long(preambula_leng, id_master, id_slaiv, 131, new Byte[] { (byte)kod, (byte)display });
                   display = temp[0].DT[1];
                }
            }
            catch
            {
                display = -1;
                EROR = "EROR";
            }
        }

        /// <summary>
        /// Считать/записать параметры сигнализации
        /// </summary>
        /// <remarks>
        /// Считывает или записывает параметры указанной уставки.
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- длиный адрес получателя</param>
        /// <param name="kod">- Коды операций
        /// 0-Прочитать данные
        /// 1-Записать данные
        /// </param>
        /// <param name="ust">-Номер уставки </param>
        /// <param name="type_lo">-Тип логики уставки </param>
        /// <param name="t_param">-Привязка к технологическому параметру </param>
        /// <param name="d_level">-Нижний порог срабатывания уставки </param>
        /// <param name="u_level">-Верхний порог срабатывания уставки </param>
        /// <param name="gist">-Гистерезис уставки </param>
        /// <returns> void </returns>
        public void MTM701_Comand_132(int id_master, Byte[] id_slaiv, int kod, int ust,ref int type_lo,ref int t_param,ref float d_level , ref float u_level,ref float gist )
        {
            try
            {
                Read_Fraim[] temp;
                if (kod == 0)
                {
                    temp = Write_long(preambula_leng, id_master, id_slaiv, 132, new Byte[] { (byte)kod, (byte)ust });
                    type_lo = temp[0].DT[2];
                    t_param = temp[0].DT[3];
                    d_level = _Convert.Buye_tu_F(temp[0].DT, 4, 4);
                    u_level = _Convert.Buye_tu_F(temp[0].DT, 8, 4);
                    gist = _Convert.Buye_tu_F(temp[0].DT, 12, 4);
                }
                else if (kod == 1)
                {
                    List<byte> list = new List<byte>();
                    list.AddRange(new byte[] { (byte)kod, (byte)ust, (byte)type_lo, (byte)t_param, });
                    list.AddRange(_Convert.Float_tu_byte(d_level));
                    list.AddRange(_Convert.Float_tu_byte(u_level));
                    list.AddRange(_Convert.Float_tu_byte(gist));
                    byte[] z = list.ToArray();
                    temp = Write_long(preambula_leng, id_master, id_slaiv, 132, z);
                }
            }
            catch
            {
                ust = -1;
                type_lo = -1;
                t_param = -1;
                d_level = -1;
                u_level = -1;
                gist = -1;
                EROR = "EROR";
            }
        }

        /// <summary>
        /// Считать/записать параметры сигнализации
        /// </summary>
        /// <remarks>
        /// Считывает или записывает параметры указанной уставки.
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- длиный адрес получателя</param>
        /// <param name="kod">- Коды операций
        /// 0-Прочитать данные
        /// 1-Записать данные
        /// </param>
        /// <param name="ust">-Номер уставки </param>
        /// <returns> string[] - 
        /// [0] Тип логики уставки
        /// [1] Привязка к технологическому параметру
        /// [2] Нижний порог срабатывания уставки
        /// [3] Верхний порог срабатывания уставки
        /// [4] Гистерезис уставки
        /// </returns>
        public string[] MTM701_Comand_132(int id_master, Byte[] id_slaiv, int ust)
        {
        
           int type_lo =0;
           int t_param = 0;
           float d_level = 0;
           float u_level = 0;
           float gist = 0; 
           MTM701_Comand_132(id_master,id_slaiv,0, ust,ref type_lo,ref t_param,ref d_level,ref u_level,ref gist);
           string[] temp = new string[]{
           type_lo.ToString(),
           t_param.ToString(),
           d_level.ToString(),
           u_level.ToString(),
           gist.ToString(),
           };
           return temp;
        }

        /// <summary>
        /// Считать/записать параметры калибровочной таблицы
        /// </summary>
        /// <remarks>
        /// Считывает или записывает параметры калибровочной таблицы.
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- длиный адрес получателя</param>
        /// <param name="kod">- Коды операций
        /// 0-Прочитать данные
        /// 1-Записать данные
        /// </param>
        /// <param name="ust">-Номер уставки </param>
        /// <param name="cod_dt">-Код типа датчика температуры </param>
        /// <param name="cod_iz">-Код единиц измерения давления в калибровочной таблице </param>
        /// <returns> void </returns>
        public void MTM701_Comand_133(int id_master, Byte[] id_slaiv, int kod,ref int cod_dt, ref int cod_iz)
        {
            try
            {
                Read_Fraim[] temp;
                if (kod == 0)
                {
                    temp = Write_long(preambula_leng, id_master, id_slaiv, 133, new Byte[] { (byte)kod });
                    cod_dt = temp[0].DT[1];
                    cod_iz = temp[0].DT[2];
                    
                }
                else if (kod == 1)
                {
                    temp = Write_long(preambula_leng, id_master, id_slaiv, 133, new byte[] { (byte)kod, (byte)cod_dt, (byte)cod_iz });
                }
            }
            catch
            {
                cod_dt = 0;
                cod_iz = 0;
                EROR = "EROR";
            }

        }

        /// <summary>
        /// Считать/записать параметры калибровочной таблицы
        /// </summary>
        /// <remarks>
        /// Считывает или записывает параметры указанной точки калибровки по температуре.
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- длиный адрес получателя</param>
        /// <param name="kod">- Коды операций
        /// 0-Прочитать данные
        /// 1-Записать данные
        /// </param>
        /// <param name="ust">-Номер уставки </param>
        /// <param name="i">-Номер ячейки калибровки по температуре (от 0 до 4)</param>
        /// <param name="tem">-Значение температуры в точке калибровки, °С.Точка не активна если значение равно: -273. </param>
        /// <param name="cod_te">-Код АЦП со встроенного в МК датчика температуры</param>
        /// <param name="cod_d">-Код АЦП с датчика давления, питающее напряжение</param>
        /// <param name="cod_to">-Код АЦП с датчика давления, питающий ток</param>
        /// <returns> void </returns>
        public void MTM701_Comand_134(int id_master, Byte[] id_slaiv, int kod, int i, ref float tem,ref string cod_te,ref string cod_d,ref string cod_to)
        {
            try
            {
                Read_Fraim[] temp;
                if (kod == 0)
                {
                    temp = Write_long(preambula_leng, id_master, id_slaiv, 134, new Byte[] { (byte)kod , (byte)i });
                    tem = _Convert.Buye_tu_F( temp[0].DT,2,4);
                    cod_te = "0x" + BitConverter.ToString(temp[0].DT, 6, 2).Replace("-", "");
                    cod_d  = "0x" + BitConverter.ToString(temp[0].DT, 8, 2).Replace("-", "");
                    cod_to = "0x" + BitConverter.ToString(temp[0].DT, 10, 2).Replace("-", "");
                }
                else if (kod == 1)
                {
                    List<byte> list = new List<byte>();
                    list.AddRange(new byte[] {  (byte)kod, (byte)i });
                    list.AddRange(_Convert.Float_tu_byte(tem));
                    list.AddRange(_Convert.GetBytes(cod_te));
                    list.AddRange(_Convert.GetBytes(cod_d));
                    list.AddRange(_Convert.GetBytes(cod_to));
                    byte[] z = list.ToArray();
                    temp = Write_long(preambula_leng, id_master, id_slaiv, 134, z);
                }
            }
            catch
            {
                EROR = "EROR";
            }
        }
        /// <summary>
        /// Считать/записать точку калибровки по давлению
        /// </summary>
        /// <remarks>
        /// Считывает или записывает параметры указанной точки калибровки по давлению.
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- длиный адрес получателя</param>
        /// <param name="kod">- Коды операций
        /// 0-Прочитать данные
        /// 1-Записать данные
        /// </param>
        /// <param name="ust">-Номер уставки </param>
        /// <param name="i">-Номер ячейки калибровки по температуре (от 0 до 4)</param>
        /// <param name="j">-Номер ячейки калибровки по давлению (от 0 до 9)</param>
        /// <param name="davlenie">-Значение давления в точке калибровки</param>
        /// <param name="cod_d">-Код АЦП с датчика давления, сигнал давления</param>
        /// <param name="cod_mk">-Код АЦП со встроенного в МК датчика температуры</param>
        /// <param name="cod_dn">-Код АЦП с датчика давления, питающее напряжение</param>
        /// <param name="cod_dt">-Код АЦП с датчика давления, питающий ток</param>
        /// <returns> void </returns>
        public void MTM701_Comand_135(int id_master, Byte[] id_slaiv, int kod, int i,int j, ref float davlenie, ref string cod_d, ref string cod_mk, ref string cod_dn,ref string cod_dt)
        {
            try
            {
                Read_Fraim[] temp;
                if (kod == 0)
                {
                    temp = Write_long(preambula_leng, id_master, id_slaiv, 135, new Byte[] { (byte)kod, (byte)i, (byte)j });
                    davlenie = _Convert.Buye_tu_F(temp[0].DT, 3, 4);
                    cod_d  = "0x" + BitConverter.ToString(temp[0].DT, 7, 2).Replace("-", "");
                    cod_mk = "0x" + BitConverter.ToString(temp[0].DT, 9, 2).Replace("-", "");
                    cod_dn = "0x" + BitConverter.ToString(temp[0].DT, 11, 2).Replace("-", "");
                    cod_dt = "0x" + BitConverter.ToString(temp[0].DT, 13, 2).Replace("-", "");
                }
                else if (kod == 1)
                {
                    List<byte> list = new List<byte>();
                    list.AddRange(new byte[] { (byte)kod, (byte)i, (byte)j });
                    list.AddRange(_Convert.Float_tu_byte(davlenie));
                    list.AddRange(_Convert.GetBytes(cod_d));
                    list.AddRange(_Convert.GetBytes(cod_mk));
                    list.AddRange(_Convert.GetBytes(cod_dn));
                    list.AddRange(_Convert.GetBytes(cod_dt));
                    byte[] z = list.ToArray();
                    temp = Write_long(preambula_leng, id_master, id_slaiv, 135, z);
                }
            }
            catch
            {
                EROR = "EROR";
            }

        }
        /// <summary>
        /// Считать/записать точки калибровки выходного сигнала постоянного тока
        /// </summary>
        /// <remarks>
        /// Считывает или записывает параметры точек калибровки выходного сигнала постоянного тока.
        /// </remarks>
        /// <param name="id_master">- адрес отправителя</param>
        /// <param name="id_slaiv">- длиный адрес получателя</param>
        /// <param name="kod">- Коды операций
        /// 0-Прочитать данные
        /// 1-Записать данные
        /// </param>
        /// <param name="min">-Значение минимума выходного тока, мА</param>
        /// <param name="max">-Значение максимума выходного тока, мА</param>
        /// <param name="min_c">-Код ЦАП минимума выходного токового сигнала</param>
        /// <param name="min_a">-Код АЦП минимума выходного токового сигнала</param>
        /// <param name="max_c">-Код ЦАП максимума выходного токового сигнала</param>
        /// <param name="max_a">-Код АЦП максимума выходного токового сигнала</param>
        /// <returns> void </returns>
        public void MTM701_Comand_136(int id_master, Byte[] id_slaiv, int kod, ref float min, ref float max, ref string min_c, ref string min_a, ref string max_c, ref string max_a)
        {
            try
            {
                Read_Fraim[] temp;
                if (kod == 0)
                {
                    temp = Write_long(preambula_leng, id_master, id_slaiv, 136, new Byte[] { (byte)kod });
                    min = _Convert.Buye_tu_F(temp[0].DT, 1, 4);
                    max = _Convert.Buye_tu_F(temp[0].DT, 9, 4);
                    min_c = "0x" + BitConverter.ToString(temp[0].DT, 5, 2).Replace("-", "");
                    min_a = "0x" + BitConverter.ToString(temp[0].DT, 7, 2).Replace("-", "");
                    max_c = "0x" + BitConverter.ToString(temp[0].DT, 13, 2).Replace("-", "");
                    max_a = "0x" + BitConverter.ToString(temp[0].DT, 15, 2).Replace("-", "");

                }
                else if (kod == 1)
                {
                    List<byte> list = new List<byte>();
                    list.AddRange(new byte[] { (byte)kod});
                    list.AddRange(_Convert.Float_tu_byte(min));
                    list.AddRange(_Convert.GetBytes(min_c));
                    list.AddRange(_Convert.GetBytes(min_a));
                    list.AddRange(_Convert.Float_tu_byte(max));
                    list.AddRange(_Convert.GetBytes(max_c));
                    list.AddRange(_Convert.GetBytes(max_a));
                    byte[] z = list.ToArray();
                    temp = Write_long(preambula_leng, id_master, id_slaiv, 136, z);
                }
            }
            catch
            {
                EROR = "EROR";
            }

        }
    }
}
