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
    public partial class FileForm3 : Form
    {
        private String parm, parm1, name = "B";
        private string mode = "1";
        public FileForm3(String p,String p1)
        {
            InitializeComponent();
            this.parm = p;
            this.parm1 = p1;
            mode = INIHelper.Read("AAA", GlobalVariables.firPath + @"\" + this.parm, GlobalVariables.configPath);
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            this.button2.Text = name + "2";
            this.button3.Text = name + "3";
            this.button4.Text = name + "4";
            this.button5.Text = name + "5";
            this.button6.Text = name + "6";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string fileName = GlobalVariables.firPath + @"\" + parm + @"\" + parm1 + @"\" + this.button2.Text.ToString();
            if (File.Exists(fileName + ".txt"))
            {
                if (mode.Equals("2"))
                {
                    FormModel2 frm5 = new FormModel2(parm, parm1, this.button2.Text.ToString());
                    frm5.Show();
                }
                else if (mode.Equals("3"))
                {
                    FormModel3 frm3 = new FormModel3(parm, parm1, this.button2.Text.ToString());
                    frm3.Show();
                }
                else if (mode.Equals("4"))
                {

                }
                else
                {
                    FormModel1 frm4 = new FormModel1(parm, parm1, this.button2.Text.ToString());
                    frm4.Show();
                }
            }
            else
            {
                MessageBox.Show("此文件不存在！");
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string fileName = GlobalVariables.firPath + @"\" + parm + @"\" + parm1 + @"\" + this.button4.Text.ToString();
            if (File.Exists(fileName + ".txt"))
            {
                if (mode.Equals("2"))
                {
                    FormModel2 frm5 = new FormModel2(parm, parm1, this.button4.Text.ToString());
                    frm5.Show();
                }
                else if (mode.Equals("3"))
                {
                    FormModel3 frm3 = new FormModel3(parm, parm1, this.button4.Text.ToString());
                    frm3.Show();
                }
                else if (mode.Equals("4"))
                {

                }
                else
                {
                    FormModel1 frm4 = new FormModel1(parm, parm1, this.button4.Text.ToString());
                    frm4.Show();
                }
            }
            else
            {
                MessageBox.Show("此文件不存在！");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string fileName = GlobalVariables.firPath + @"\" + parm + @"\" + parm1 + @"\" + this.button3.Text.ToString();
            if (File.Exists(fileName + ".txt"))
            {
                if (mode.Equals("2"))
                {
                    FormModel2 frm5 = new FormModel2(parm, parm1, this.button3.Text.ToString());
                    frm5.Show();
                }
                else if (mode.Equals("3"))
                {
                    FormModel3 frm3 = new FormModel3(parm, parm1, this.button3.Text.ToString());
                    frm3.Show();
                }
                else if (mode.Equals("4"))
                {

                }
                else
                {
                    FormModel1 frm4 = new FormModel1(parm, parm1, this.button3.Text.ToString());
                    frm4.Show();
                }
            }
            else
            {
                MessageBox.Show("此文件不存在！");
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            string fileName = GlobalVariables.firPath + @"\" + parm + @"\" + parm1 + @"\" + this.button6.Text.ToString();
            if (File.Exists(fileName + ".txt"))
            {
                if (mode.Equals("2"))
                {
                    FormModel2 frm5 = new FormModel2(parm, parm1, this.button6.Text.ToString());
                    frm5.Show();
                }
                else if (mode.Equals("3"))
                {
                    FormModel3 frm3 = new FormModel3(parm, parm1, this.button6.Text.ToString());
                    frm3.Show();
                }
                else if (mode.Equals("4"))
                {

                }
                else
                {
                    FormModel1 frm4 = new FormModel1(parm, parm1, this.button6.Text.ToString());
                    frm4.Show();
                }  
            }
            else
            {
                MessageBox.Show("此文件不存在！");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {


 
        }


    }
}
