using System;
using System.Collections.Generic;

namespace Andriod_Hart.Models
{
    public class Item
    {
        public string Text { get; set; }
        public string Description { get; set; }
        public bool IsConected { get; set; }
    }
    public class Param
    {
        public string name { get; set; }
        public string type { get; set; }
        public string value { get; set; }
        public Param(string p1, string p2, string p3)
        {
            name = p1;
            type = p2;
            value = p3;
        }
    }
    public class DEVISE
    {
        public string Short_Address { get; set; }
        public string Long_Address { get; set; }
        public string Device_Version { get; set; }
        public string Software_Version { get; set; }
        public string Assembly_Number { get; set; }
        public bool IsSelected { get; set; }
        public static DEVISE[] Converter(Class_HART.Conect.Read_Fraim[] Fraim)
        {
            List<DEVISE> arrau = new List<DEVISE>();
            foreach (Class_HART.Conect.Read_Fraim item in Fraim)
            {
                DEVISE temp = new DEVISE();
                temp.Long_Address =
                      item.DT[1].ToString("X2") + '-' +
                      item.DT[2].ToString("X2") + '-' +
                      item.DT[9].ToString("X2") + '-' +
                      item.DT[10].ToString("X2") + '-' +
                      item.DT[11].ToString("X2");

                temp.Assembly_Number = BitConverter.ToString(item.DT, 9);
                temp.Software_Version = item.DT[6].ToString("X2");
                temp.Device_Version = item.DT[7].ToString("X2");
                temp.Short_Address = item.AD_Short.ToString();
                arrau.Add(temp);
            }
            return arrau.ToArray();
        }
    }
}