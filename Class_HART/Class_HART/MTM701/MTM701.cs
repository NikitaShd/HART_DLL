using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_HART
{
    public partial class Conect
    {
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

            }

        }
    }
}
