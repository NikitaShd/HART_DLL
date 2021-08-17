using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_HART
{
    public partial class Conect
    {
       static public string[] MTM701_Cods_Pressure = new string[] // Коды единиц измерения давления
        {
        "Па","кПа","МПа","гс/см2","кгс/см2","мм.рт.ст.","мм.вод.ст","psi","бар","Мбар"
        };
       static public string[] MTM701_Cods_TempSignal = new string[] //Коды источников сигнала температуры
        {
        "Сенсор встроенный в МК","Сенсор давления питающийся напряжением","Сенсор давления питающийся током"
        };
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
        public void MTM701_Comand_134(int id_master, Byte[] id_slaiv, int kod, int i, ref float tem,ref string cod_te,ref string cod_d,ref string cod_to)
        {
            try
            {
                Read_Fraim[] temp;
                if (kod == 0)
                {
                    temp = Write_long(preambula_leng, id_master, id_slaiv, 134, new Byte[] { (byte)kod , (byte)i });

                  
                   // Array.Reverse(temp[0].DT);

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
    }
}
