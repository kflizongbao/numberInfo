using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Web;

namespace drawDong
{
    public partial class GongYue : Form
    {
        private int t;
        public GongYue()
        {
            InitializeComponent();
        }

        public GongYue(string p1,int arg0)
        {
            InitializeComponent();

            t = arg0;
            string[] sArray = p1.Split(',');

            for (int i = 0; i < sArray.Length / 5 + ((sArray.Length % 5 > 0) ? 1 : 0); i++)
            {
                this.dataGridView1.Rows.Add();
            }

            for (int i = 0; i<sArray.Length;i++ )
            {
                this.dataGridView1.Rows[i/5].Cells[i%5].Value = sArray[i];
            }
        }

        private void GongYue_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DirectoryInfo dirFile = new DirectoryInfo(GlobalVariables.xuanxiangPath);
            FileInfo[] files = dirFile.GetFiles();
            string s = INIHelper.Read("AAA", "name", GlobalVariables.confPath);
            string name = (Convert.ToInt32(s) + 1).ToString();
            for (int i = name.Length; i<6 ; i++ )
            {
                name = "0" + name;
            }

            string fileName = GlobalVariables.xuanxiangPath + @"\" + name + ".txt";

            if (!File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
                fs.Close();
            }


            FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate);
            StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
            StringBuilder strBuilder = new StringBuilder();
            try
            {
                for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                {
                    strBuilder = new StringBuilder();
                    for (int j = 0; j < this.dataGridView1.Columns.Count; j++)
                    {
                        if (null == this.dataGridView1.Rows[i].Cells[j].Value)
                        {
                            strBuilder.Append("-1" + ",");
                        }
                        else
                        {
                            String cellContent = this.dataGridView1.Rows[i].Cells[j].Value.ToString();
                            if (cellContent.Length == 0)
                            {
                                cellContent = "-1";
                            }
                            strBuilder.Append(this.dataGridView1.Rows[i].Cells[j].Value.ToString() + ",");
                        }
                    }
                    strBuilder.Remove(strBuilder.Length - 1, 1);
                    String content = strBuilder.ToString();
                    streamWriter.WriteLine(content);
                }
               INIHelper.Write("AAA", "name",name , GlobalVariables.confPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                streamWriter.Close();
                fileStream.Close();
            }

            if (MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK) == DialogResult.OK)
            {
                this.Close();
            }
        }






    }
}
