using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
namespace drawDong
{
    public partial class Form7 : Form
    {
        private string parm, parm1, name = "B",rootPath;
        private string mode = "1";
        private ArrayList fileList = new ArrayList();


        public Form7(string p, string p1)
        {
            InitializeComponent();
            this.parm = p;
            this.parm1 = p1;
            mode = INIHelper.Read("AAA", GlobalVariables.firPath + @"\" + this.parm, GlobalVariables.configPath);
            rootPath = GlobalVariables.firPath + @"\" + this.parm + @"\" + this.parm1;
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            loadData();
        }

        private void loadData()
        {
            fileList.Clear();
            //加载对应目录下的文件
            DirectoryInfo rootDir = new DirectoryInfo(rootPath);
            Boolean exists = Directory.Exists(rootPath);
            if (exists)//判断目录是否存在
            {
                FileInfo[] files = rootDir.GetFiles();
                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo tit = files[i];
                    string filePath = tit.FullName;
                    string fileName = tit.Name;
                    if (!fileName.Contains("x.txt"))
                    {
                        fileList.Add(fileName);
                    }
                }
            }
            handleData();        
        }

        private void handleData()
        {
            int size = fileList.Count;
            if (size > 0)
            {
                int p1 = size / 6;
                int p2 = size % 6;
                int p3;
                if (p2 == 0)
                {
                    p3 = p1;
                }
                else
                {
                    p3 = p1 + 1;
                }
                for (int i = 0; i < p3; i++)
                {
                    this.dataGridView1.Rows.Add("", "", "", "", "", "");
                }
            }

            int columns = this.dataGridView1.Columns.Count;
            int rows = this.dataGridView1.Rows.Count;

            for (int i = 0; i< rows;i++ )
            {
                for (int j = 0; j < columns; j++)
                {
                    string name = getData(i, j, columns);
                    string path = rootPath + @"\" + name;
                    string desc = INIHelper.Read("BBB", path, GlobalVariables.infoPath);
                    if (null == desc || desc.Trim().Length == 0)
                    {
                        this.dataGridView1.Rows[i].Cells[j].Value = name;
                    }
                    else
                    {
                        string val = name;
                        int leg = 13;
                        int maxIndex = ((desc.Length % leg) > 0 ? desc.Length / leg + 1 : desc.Length / leg);
                        for (int n = 0; n < maxIndex; n++)
                        {
                            val = val + Environment.NewLine + desc.Substring(n * leg, ((n == maxIndex - 1) ? desc.Length % leg : leg));
                        }
                        this.dataGridView1.Rows[i].Cells[j].Value = val;
                    }
                }
            }

            resetDataGrid();

        }

        private string getData(int i,int j,int columns )
        {
            string data = "";
            if (i * columns < fileList.Count - j)
            {
                data = fileList[i * columns + j].ToString();
                data = data.Replace(".txt", "");
            }
            return data;
        }

        private void resetDataGrid()
        {
            int columns = this.dataGridView1.Columns.Count;
            int rows = this.dataGridView1.Rows.Count;
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (null != this.dataGridView1.Rows[j].Cells[i].Value  && this.dataGridView1.Rows[j].Cells[i].Value.ToString().Length == 0)
                    {
                        DataGridViewTextBoxCell xx = new DataGridViewTextBoxCell();
                        this.dataGridView1[i, j] = xx;
                        this.dataGridView1[i, j].Value = "";
                    }
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string content = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

            content = content.Replace("\r\n", "\n");
            string[] sArray = content.Split(new char[1] { '\n' });

            if (sArray.Length > 1)
            {
                content = sArray[0];
            }


            string fileName = GlobalVariables.firPath + @"\" + parm + @"\" + parm1 + @"\" + content;




            if (File.Exists(fileName + ".txt"))
            {
                if (mode.Equals("2"))
                {
                    FormModel2 frm5 = new FormModel2(parm, parm1, content);
                    frm5.Show();
                }
                else if (mode.Equals("3"))
                {
                    FormModel3 frm3 = new FormModel3(parm, parm1, content);
                    frm3.Show();
                }
                else if (mode.Equals("4"))
                {
                    FormModel4 frm4 = new FormModel4(parm, parm1, content);
                    frm4.Show();
                }
                else if (mode.Equals("5"))
                {
                    FormModel7 frm5 = new FormModel7(parm, parm1, content);
                    frm5.Show();
                }
                else if (mode.Equals("6"))
                {
                    FormModel6 frm6 = new FormModel6(parm, parm1, content);
                    frm6.Show();
                }
                else
                {
                    FormModel1 frm4 = new FormModel1(parm, parm1, content);
                    frm4.Show();
                }
            }
            else
            {
                MessageBox.Show("此文件不存在！");
            }
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e) 
        {
            string fileName = dataGridView1.SelectedCells[0].Value.ToString();

            fileName = fileName.Replace("\r\n", "\n");
            string[] sArray = fileName.Split(new char[1] { '\n' });

            if (sArray.Length > 1)
            {
                fileName = sArray[0];
            }

            string filePath = rootPath + @"\" + fileName+".txt";
            string filePath1 = rootPath + @"\" + fileName + "x.txt";
            if (File.Exists(filePath))//判断文件是不是存在
            {
                File.Delete(filePath);//如果存在则删除
            }
            if (File.Exists(filePath1))//判断文件是不是存在
            {
                File.Delete(filePath1);//如果存在则删除
            }

            INIHelper.Write("BBB", rootPath + @"\" + fileName, "", GlobalVariables.infoPath);

            loadData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string fileName = dataGridView1.SelectedCells[0].Value.ToString();
            fileName=fileName.Replace("\r\n","\n");
            string[] sArray = fileName.Split(new char[1] { '\n' });

            if (sArray.Length > 1)
            {
                fileName = sArray[0];
            }

            string filePath = rootPath + @"\" + fileName;

            Form3 frm = new Form3();
            DialogResult ddr = frm.ShowDialog();
            if (ddr == DialogResult.OK)
            {
                INIHelper.Write("BBB", filePath, frm.test, GlobalVariables.infoPath);
            }
            this.dataGridView1.Rows.Clear();
            loadData();
        }



    }
}
