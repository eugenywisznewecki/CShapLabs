using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace l2_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            x = Double.Parse(textBox1.Text);
            y = Double.Parse(textBox2.Text);

            double fnX = 0, fny= 0;
            int indexLine = 0;

            if (radioButton1.Checked) {
                fnX = Math.Sin(x);
                            
            }
            if (radioButton2.Checked) {
                fnX = Math.Cos(x);
               

            }
            if (radioButton3.Checked) {
                fnX = Math.Atan(x);
               
            }



            //funcs
            if (x / y > 0)
            {
                double b3 = Math.Sin(x);
                double b1 = Math.Log10(b3);
                double b2 = Math.Pow(Math.Pow(fnX, 3) + y, 3);
                b = b1 + b2;
                indexLine = 0;
            }

            if (x / y < 0)
            {
                b = Math.Log10(Math.Abs(fnX / y)) + Math.Pow(fnX + y, 3);
                indexLine = 1;

            }
            if (x == 0)
            {
                b = Math.Pow((fnX * fnX + y), 3);
                indexLine = 2;
            }
            if (y == 0)
            {
                b = 0;
                indexLine = 3;
            }




            if (min[indexLine] == null && max[indexLine] == null)
            {
                min[indexLine] = b;
                max[indexLine] = b;
            }
            else {
                if (min[indexLine] > b) {
                    min[indexLine] = b;
                }

                if (max[indexLine] < b)
                {
                    max[indexLine] = b;
                }

            }
            textBox3.Text = b.ToString();
            textBox4.Text = b.ToString();
            textBox5.Text = "branch " + (indexLine+1).ToString();
                        
            if (checkBox1.Checked) {
                textBox6.Text = "min:" + min[indexLine];
            }
            if (checkBox2.Checked)
            {
                textBox6.Text ="max: " + max[indexLine];
            }












        }

    }
}
