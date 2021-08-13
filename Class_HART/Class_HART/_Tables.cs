using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Class_HART
{
   public class _Tables
    {
        //=================< Декодирывание (Таблицы) >=====================================================================================
        static public char[] PACKED_ASCII = { '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 
                               'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W',
                               'X', 'Y', 'Z', '[', '\\', ']', '^', '_', ' ', '!', '"', '#',
                               '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/',
                               '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ';',
                               '<', '=', '>', '?'};
       static public string Alarm_Cods(int cod)
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
       static public int Alarm_Cods(string cod)
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
       static public string Transfer_Cods(int cod)
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
       static public int Transfer_Cods(string cod)
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
       static public string Protect_Cods(int cod)
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
       static public int Protect_Cods(string cod)
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
       static private string[] Encod_unit_array = new string[255];
        static public string Encod_unit(int kod)
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
        static public int Encod_unit(string kod)
        {

            return Array.IndexOf(Encod_unit_array, kod);
        }
        static public string[] unit_array()
        {
            string[] temp = { };
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
        static public void init_Encod_unit()
        {

            // ==== давление ======
            Encod_unit_array[1] = "Дюймы воды при 68°F";
            Encod_unit_array[2] = "Дюймы ртути при 0°С";
            Encod_unit_array[3] = "";
            Encod_unit_array[4] = "Милеметры воды при 68°F";
            Encod_unit_array[5] = "Милеметры ртути при 0°С";
            Encod_unit_array[6] = "psi";
            Encod_unit_array[7] = "бар";
            Encod_unit_array[8] = "миллибар";
            Encod_unit_array[9] = "gf/cm²";
            Encod_unit_array[10] = "kgf/cm²";
            Encod_unit_array[11] = "Па";
            Encod_unit_array[12] = "кПа";
            Encod_unit_array[13] = "торр";
            Encod_unit_array[14] = "атм";
            Encod_unit_array[145] = "";
            Encod_unit_array[237] = "МПа";
            Encod_unit_array[238] = "";
            Encod_unit_array[239] = "";
            // ==== температуры ===
            Encod_unit_array[32] = "°C";
            Encod_unit_array[33] = "°F";
            Encod_unit_array[34] = "°R";
            Encod_unit_array[35] = "°K";
            // ==== Количество ========== 
            Encod_unit_array[15] = "cubic feet per minute";
            Encod_unit_array[16] = "gallons per minute";
            Encod_unit_array[17] = "liters per minute";
            Encod_unit_array[18] = "imperial gallons per minute";
            Encod_unit_array[19] = "cubic meter per hour";
            Encod_unit_array[22] = "gallons per second";
            Encod_unit_array[23] = "million gallons per day";
            Encod_unit_array[24] = "liters per second";
            Encod_unit_array[25] = "million liters per day";
            Encod_unit_array[26] = "cubic feet per second";
            Encod_unit_array[27] = "cubic feet per day";
            Encod_unit_array[28] = "cubic meters per second";
            Encod_unit_array[29] = "cubic meters per day";
            Encod_unit_array[30] = "imperial gallons per hour";
            Encod_unit_array[31] = "imperial gallons per day";
            Encod_unit_array[121] = "normal cubic meter per hour";
            Encod_unit_array[122] = "normal liter per hour";
            Encod_unit_array[123] = "standard cubic feet per minute";
            Encod_unit_array[130] = "cubic feet per hour";
            Encod_unit_array[131] = "cubic meters per minute";
            Encod_unit_array[132] = "barrels per second";
            Encod_unit_array[133] = "barrels per minute";
            Encod_unit_array[134] = "barrels per hour";
            Encod_unit_array[135] = "barrels per day";
            Encod_unit_array[136] = "gallons per hour";
            Encod_unit_array[137] = "imperial gallons per second";
            Encod_unit_array[138] = "liters per hour";
            Encod_unit_array[235] = "gallons per day";
            // ==== скорость =======================
            Encod_unit_array[20] = "feet per second";
            Encod_unit_array[21] = "meters per second";
            Encod_unit_array[114] = "inches per second";
            Encod_unit_array[115] = "inches per minute";
            Encod_unit_array[116] = "feet per minute";
            Encod_unit_array[120] = "meters per hour";
            // ==== обем ===========================
            Encod_unit_array[40] = "gallons";
            Encod_unit_array[41] = "liters";
            Encod_unit_array[42] = "imperial gallons";
            Encod_unit_array[43] = "cubic meters";
            Encod_unit_array[46] = "barrels";
            Encod_unit_array[110] = "bushels";
            Encod_unit_array[111] = "cubic yards";
            Encod_unit_array[112] = "cubic feet";
            Encod_unit_array[113] = "cubic inches";
            Encod_unit_array[124] = "bbl liq";
            Encod_unit_array[166] = "normal cubic meter";
            Encod_unit_array[167] = "normal liter";
            Encod_unit_array[168] = "standard cubic feet";
            Encod_unit_array[236] = "hectoliters";
            // ==== растояние ============================
            Encod_unit_array[44] = "feet";
            Encod_unit_array[45] = "meters";
            Encod_unit_array[47] = "inches";
            Encod_unit_array[48] = "centimeters";
            Encod_unit_array[49] = "millimeters";
            // ==== Время ================================
            Encod_unit_array[50] = "minutes";
            Encod_unit_array[51] = "seconds";
            Encod_unit_array[52] = "hours";
            Encod_unit_array[53] = "days";
            // ==== Масса ================================
            Encod_unit_array[60] = "grams";
            Encod_unit_array[61] = "kilograms";
            Encod_unit_array[62] = "metric tons";
            Encod_unit_array[63] = "pounds";
            Encod_unit_array[64] = "short tons";
            Encod_unit_array[65] = "long tons";
            Encod_unit_array[125] = "ounce";
            // ==== Масса за время ========================
            Encod_unit_array[70] = "grams per second";
            Encod_unit_array[71] = "grams per minute";
            Encod_unit_array[72] = "grams per hour";
            Encod_unit_array[73] = "kilograms per second";
            Encod_unit_array[74] = "kilograms per minute";
            Encod_unit_array[75] = "kilograms per hour";
            Encod_unit_array[76] = "kilograms per day";
            Encod_unit_array[77] = "metric tons per minute";
            Encod_unit_array[78] = "metric tons per hour";
            Encod_unit_array[79] = "metric tons per day";
            Encod_unit_array[80] = "pounds per second";
            Encod_unit_array[81] = "pounds per minute";
            Encod_unit_array[82] = "pounds per hour";
            Encod_unit_array[83] = "pounds per day";
            Encod_unit_array[84] = "short tons per minute";
            Encod_unit_array[85] = "short tons per hour";
            Encod_unit_array[86] = "short tons per day";
            Encod_unit_array[87] = "long tons per hour";
            Encod_unit_array[88] = "long tons per day";
            // ==== Масса на обем =========================
            Encod_unit_array[90] = "specific gravity units";
            Encod_unit_array[91] = "grams per cubic centimeter";
            Encod_unit_array[92] = "kilograms per cubic meter";
            Encod_unit_array[93] = "pounds per gallon";
            Encod_unit_array[94] = "pounds per cubic foot";
            Encod_unit_array[95] = "grams per milliliter";
            Encod_unit_array[96] = "kilograms per liter";
            Encod_unit_array[97] = "grams per liter";
            Encod_unit_array[98] = "pounds per cubic inch";
            Encod_unit_array[99] = "short tons per cubic yard";
            Encod_unit_array[100] = "degrees twaddell";
            Encod_unit_array[102] = "degrees baume heavy";
            Encod_unit_array[103] = "degrees baume light";
            Encod_unit_array[104] = "degrees API";
            Encod_unit_array[146] = "micrograms per liter";
            Encod_unit_array[147] = "micrograms per cubic meter";
            // ==== Вязкость ================================
            Encod_unit_array[54] = "centistokes";
            Encod_unit_array[55] = "centipoise";
            // ==== Электроника ===============================
            Encod_unit_array[36] = "millivolts";
            Encod_unit_array[58] = "volts";
            Encod_unit_array[39] = "milliamperes";
            Encod_unit_array[37] = "ohms";
            Encod_unit_array[163] = "kohms";
            // ==== Энергия ===================================
            Encod_unit_array[69] = "newton meter";
            Encod_unit_array[89] = "deka therm";
            Encod_unit_array[126] = "foot pound force";
            Encod_unit_array[128] = "kilo watt hour";
            Encod_unit_array[162] = "mega calorie";
            Encod_unit_array[165] = "british thermal unit";
            // ==== сила =======================================
            Encod_unit_array[127] = "kilo watt";
            Encod_unit_array[129] = "horsepower";
            Encod_unit_array[140] = "mega calorie per hour";
            Encod_unit_array[142] = "british thermal unit per hour";
            // ==== Радиальная скорость =========================
            Encod_unit_array[117] = "degrees per second";
            Encod_unit_array[118] = "revolutions per second";
            Encod_unit_array[119] = "revolutions per minute";
            // ==== Различное =================================
            Encod_unit_array[38] = "hertz";
            Encod_unit_array[56] = "microsiemens";
            Encod_unit_array[57] = "%";
            Encod_unit_array[59] = "pH";
            Encod_unit_array[66] = "milli siemens per centimeter";
            Encod_unit_array[67] = "micro siemens per centimeter";
            Encod_unit_array[68] = "newton  101 degrees brix";
            Encod_unit_array[105] = "percent solids per weight";
            Encod_unit_array[106] = "percent solids per volume";
            Encod_unit_array[107] = "degrees balling";
            Encod_unit_array[108] = "proof per volume";
            Encod_unit_array[109] = "proof per mass";
            Encod_unit_array[139] = "parts per million";
            Encod_unit_array[143] = "degrees";
            Encod_unit_array[144] = "radian";
            Encod_unit_array[148] = "percent consistency";
            Encod_unit_array[149] = "volume percent";
            Encod_unit_array[150] = "percent steam quality";
            Encod_unit_array[151] = "feet in sixteenths";
            Encod_unit_array[152] = "cubic feet per pound";
            Encod_unit_array[153] = "picofarads";
            Encod_unit_array[154] = "mililiters per liter";
            Encod_unit_array[155] = "microliters per liter";
            Encod_unit_array[160] = "percent plato";
            Encod_unit_array[161] = "percent lower explosion level";
            Encod_unit_array[169] = "parts per billion";
            // ==== общии ============================================
            Encod_unit_array[240] = "Enumeration may be used for manufacturer specific definitions";
            Encod_unit_array[250] = "Not Used";
            Encod_unit_array[251] = "None";
            Encod_unit_array[252] = "Unknown";
            Encod_unit_array[253] = "Special";



        }
        //=======================================================================================================================
    }
}
