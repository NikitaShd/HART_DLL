using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
///using System.Threading.Tasks;
using System.Windows.Forms;

namespace Class_HART
{
    public partial class Form_131 : Form
    {
        public Conect P;
        public Form_131()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string d = dev.Text.Replace("-", "");
            Byte[] ad = P.GetBytes(d);
            string[] temp = { };
            P.MTM701_Comand_130(P.Master, ad,ref temp);
            textBox1.Text = temp[0];
            textBox2.Text = temp[1];
            textBox3.Text = temp[2];
            textBox4.Text = temp[3];
            textBox5.Text = temp[4];
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string d = dev.Text.Replace("-", "");
            Byte[] ad = P.GetBytes(d);
            int temp = 0;
            P.MTM701_Comand_131(P.Master, ad,0,ref temp);
            textBox6.Text = temp.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string d = dev.Text.Replace("-", "");
            Byte[] ad = P.GetBytes(d);
            int temp = Convert.ToInt32(textBox6.Text);
            P.MTM701_Comand_131(P.Master, ad, 1, ref temp);
           
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string d = dev.Text.Replace("-", "");
            Byte[] ad = P.GetBytes(d);
            int temp1 = 0;
            try
            {
                 temp1 = Convert.ToInt32(textBox8.Text);
            }
            catch
            {
                temp1 = 0;
            }
            int temp2 = 0;
            float temp3 = 0;
            float temp4 = 0;
            float temp5 = 0;
            P.MTM701_Comand_132(P.Master, ad, 0,ref temp1, ref temp2, ref temp3, ref temp4, ref temp5);
            textBox8.Text  = temp1.ToString();
            textBox9.Text  = temp2.ToString();
            textBox10.Text = temp3.ToString();
            textBox11.Text = temp4.ToString();
            textBox12.Text = temp5.ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string d = dev.Text.Replace("-", "");
            Byte[] ad = P.GetBytes(d);
  
            int temp1 = Convert.ToInt32(textBox8.Text);
            int temp2 = Convert.ToInt32(textBox9.Text);
            float temp3 = Convert.ToSingle(textBox10.Text);
            float temp4 = Convert.ToSingle(textBox11.Text); 
            float temp5 = Convert.ToSingle(textBox12.Text);
            P.MTM701_Comand_132(P.Master, ad, 1, ref temp1, ref temp2, ref temp3, ref temp4, ref temp5);
       
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string[] temp = P.unit_array();
            comboBox1.Items.AddRange(temp);
            string d = dev.Text.Replace("-", "");
            Byte[] ad = P.GetBytes(d);
            int temp1 = 0;
            int temp2 = 0;
            int temp3 = 0;
            byte[] temp4 = new byte[2] {0, 0};
            int temp5 = 0;
            int temp6 = 0;
            int temp7 = 0;
            int temp8 = 0;
            P.MTM701_Comand_133(P.Master, ad, 0, ref temp1, ref temp2, ref temp3, ref temp4, ref temp5, ref temp6, ref temp7, ref temp8);
            textBox7.Text = temp1.ToString("X2");
            comboBox1.Text = P.Encod_unit(temp2);
            textBox14.Text = temp3.ToString();
            textBox15.Text = BitConverter.ToString(temp4);
            textBox16.Text = temp5.ToString();
            textBox17.Text = temp6.ToString();
            textBox13.Text = temp7.ToString();
            textBox18.Text = temp8.ToString();
        }

  

        private void button8_Click(object sender, EventArgs e)
        {
            string[] temp = P.unit_array();
            comboBox1.Items.AddRange(temp);
            string d = dev.Text.Replace("-", "");
            Byte[] ad = P.GetBytes(d);
            int temp1 = Convert.ToInt32(textBox7.Text);
            int temp2 = P.Encod_unit(comboBox1.Text);
            int temp3 = Convert.ToInt32(textBox14.Text);
            byte[] temp4 = P.GetBytes(textBox15.Text.Replace("-", "").Replace(" ", ""));
            int temp5 = Convert.ToInt32(textBox16.Text);
            int temp6 = Convert.ToInt32(textBox17.Text);
            P.MTM701_Comand_133(P.Master, ad, 1, ref temp1, ref temp2, ref temp3, ref temp4, ref temp5, ref temp6, ref temp5, ref temp6);
            textBox7.Text = temp1.ToString("X2");
            comboBox1.Text = P.Encod_unit(temp2);
            textBox14.Text = temp3.ToString();
            textBox15.Text = BitConverter.ToString(temp4);
            textBox16.Text = temp5.ToString();
            textBox17.Text = temp6.ToString();
        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            string[] temp = P.unit_array();
            comboBox1.Items.AddRange(temp);
            string d = dev.Text.Replace("-", "");
            Byte[] ad = P.GetBytes(d);
            int temp1 = 0;
            int temp2 = 0;
            int temp3 = 0;
            byte[] temp4 = new byte[2] { 0, 0 };
            int temp5 = 0;
            int temp6 = 0;
            int temp7 = 0;
            int temp8 = 0;
            P.MTM701_Comand_133(P.Master, ad, 0, ref temp1, ref temp2, ref temp3, ref temp4, ref temp5, ref temp6, ref temp7, ref temp8);          
            t.Text = temp6.ToString();
            d_t.Text = temp8.ToString();
        }
    }
}
