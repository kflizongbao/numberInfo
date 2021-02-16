using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace drawDong
{
    public partial class Setting : Form
    {
        private string  fileName ;
        private string strFileName = GlobalVariables.configPath;
        public Setting(string parm)
        {
            InitializeComponent();
            this.fileName = parm;
        }

        private void Form5_Load(object sender, EventArgs e)
        {
           string mode =  INIHelper.Read("AAA", this.fileName, strFileName);
           if (mode.Equals("1"))
            {
                this.radioButton1.Checked = true;
            }
           else if (mode.Equals("2"))
           {
               this.radioButton2.Checked = true;
           }
           else if (mode.Equals("3"))
           {
               this.radioButton3.Checked = true;
           }
           else if (mode.Equals("4"))
           {
               this.radioButton4.Checked = true;
           }
           else if (mode.Equals("5"))
           {
               this.radioButton5.Checked = true;
           }
           else if (mode.Equals("6"))
           {
               this.radioButton6.Checked = true;
           }
           else
           {

               this.radioButton1.Checked = true;

           }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int mode = 1;
            if (this.radioButton1.Checked)
            {
                mode = 1;
            } 
            else if (this.radioButton2.Checked)
            {
                mode = 2;
            }
            else  if (this.radioButton3.Checked)
            {
                mode = 3;
            }
            else if (this.radioButton4.Checked)
            {
                mode = 4;
            }
            else if (this.radioButton5.Checked)
            {
                mode = 5;
            }
            else if (this.radioButton6.Checked)
            {
                mode = 6;
            }

            
            INIHelper.Write("AAA", this.fileName, mode.ToString(), strFileName);
            this.Close();
        }

        private void createTxt(String fileName)
        {
            if (!File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName + ".txt", FileMode.Create, FileAccess.ReadWrite);
                fs.Close();
            }
        }

        private void saveConfig(string fileName)
        { 
            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {

        }





    }
}
