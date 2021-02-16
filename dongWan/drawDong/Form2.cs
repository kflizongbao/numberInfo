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
    public partial class Form2 : Form
    {
        private string parm,rootPath;
        private int columns = 25;
        private ArrayList dLines = new ArrayList();
        public Form2(String parm1)
        {
            InitializeComponent();
            parm = parm1;
            rootPath = GlobalVariables.firPath + @"\" + parm;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            fileInit();
        }

        private void fileInit()
        {
            String path = GlobalVariables.firPath + @"\" + parm + @"\";
            DirectoryInfo rootDir = new DirectoryInfo(path);
            DirectoryInfo[] items = rootDir.GetDirectories();
            if (items.Length  > 0)
            {
                   foreach (var item in items)
                    {
                        setSaveBoxName(item.Name);
                    }
            }
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            int rows = this.dataGridView1.Rows.Count;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    String text = this.dataGridView1[j, i].Value.ToString();
                    if (text.Length == 0)
                    {
                        //DataGridViewTextBoxCell xx = new DataGridViewTextBoxCell();
                        //this.dataGridView1[j, i] = xx;
                        //this.dataGridView1[j, i].Value = "";
                    }
                }
            }
          
        }

        private void setSaveBoxName(String fileName )
        {
            Boolean newRow = true;
            int rows = this.dataGridView1.Rows.Count;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    String text = this.dataGridView1[j, i].Value.ToString();
                    if (text.Length == 0)
                    {
                        int index = (rows - 1) * columns + j + 1;
                        DataGridViewButtonCell xx = new DataGridViewButtonCell();
                        this.dataGridView1[j, i] = xx;
                        this.dataGridView1[j, i].Value = fileName;
                        newRow = false;
                        goto writeResult;
                    }
                }
            }

        writeResult:
            {
                if (newRow)
                {
                    this.dataGridView1.Rows.Add("", "", "", "", "", "", "");
                    int rowsNew = this.dataGridView1.Rows.Count;
                    for (int i = 0; i < columns; i++)
                    {
                        if (i == 0)
                        {
                            int text = (rowsNew - 1) * columns + i + 1;
                            DataGridViewButtonCell xx = new DataGridViewButtonCell();
                            this.dataGridView1[i, rowsNew - 1] = xx;
                            this.dataGridView1[i, rowsNew - 1].Value = fileName;
                        }
                        else
                        {
                            DataGridViewTextBoxCell xx = new DataGridViewTextBoxCell();
                            this.dataGridView1[i, rowsNew - 1] = xx;
                            this.dataGridView1[i, rowsNew - 1].Value = "";
                        }
                    }
                }

            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //创建文件夹
            //DirectoryInfo rootDir = new DirectoryInfo(rootPath);
           // int fileCount = rootDir.GetDirectories().Length + 1;
            //String fileName = "文件夹" + fileCount.ToString();
            //createSaveBox(fileName);
            //setSaveBoxName(fileName);

            DirectoryInfo rootDir = new DirectoryInfo(rootPath);
            int fileCount = rootDir.GetDirectories().Length + 1;
            string fileName = "A" + getFileName(fileCount);
            createSaveBox(fileName);
            setSaveBoxName(fileName);
        }
       
        private void createSaveBox(String name )
        {
            //创建文件夹
            DirectoryInfo rootDir = new DirectoryInfo(rootPath);
            int fileCount = rootDir.GetDirectories().Length + 1;
            String path = rootPath + @"\" + name;
            if (!Directory.Exists(path))//判断目录是否存在
            {
                DirectoryInfo newDir = new DirectoryInfo(path);
                newDir.Create();

                //创建文本文件
                for (int i = 0; i< GlobalVariables.fileNumber ; i++ )
                {
                    createTxt(rootPath + @"\"+ name + @"\" + "B" + (i+1).ToString());
                }
            }
        }

        private void createTxt(String fileName) 
        {
            if (!File.Exists(fileName+".txt"))
            {
                FileStream fs = new FileStream(fileName + ".txt", FileMode.Create, FileAccess.ReadWrite);
                fs.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (null != this.dataGridView1.CurrentCell)
            {
                string txt = this.dataGridView1.CurrentCell.Value.ToString();
                Setting frm5 = new Setting(rootPath);
                frm5.Show();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //判断文件是多少个
            if (null != this.dataGridView1.CurrentCell)
            {
                 string txt = this.dataGridView1.CurrentCell.Value.ToString();
                //FileForm3 frm3 = new FileForm3(parm, txt);
                //frm3.Show();

                Form7 frm7 = new Form7(parm, txt);
                frm7.Show();
            }

        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string txt = this.dataGridView1.CurrentCell.Value.ToString();
            InsertNums frm10 = new InsertNums(rootPath + @"\" + txt);
            frm10.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (null != this.dataGridView1.CurrentCell)
            {
            
            string fileNames = this.dataGridView1.CurrentCell.Value.ToString();
            if (MessageBox.Show("确定复制--" + fileNames + "--吗?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                //创建文件夹
                DirectoryInfo rootDir = new DirectoryInfo(rootPath);
                int fileCount = rootDir.GetDirectories().Length + 1;
                string fileName = "A" + getFileName(fileCount);

                //判断是不是存在同名的文件夹
                while (fileIsExists(fileName))
                {
                    fileCount += 1;
                    fileName = "A" + getFileName(fileCount);
                }

                createSaveBoxNoFile(fileName);
                setSaveBoxName(fileName);
                string[] files = Directory.GetFiles(rootPath + @"\" + fileNames);//取文件夹下所有文件名，放入数组；
                 if(files.Length   >   0)
                {
                    foreach(string   s   in   files)
                    {
                        File.Copy(s, rootPath + @"\" + fileName + s.Substring(s.LastIndexOf("\\")));                
                    }
                }
                this.dataGridView1.Rows.Clear();
                fileInit();
            }
                }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (null != this.dataGridView1.CurrentCell)
            {
                string fileName = this.dataGridView1.CurrentCell.Value.ToString();
                
                if (MessageBox.Show("确定删除文件夹--" + fileName + "--吗?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    string[] keys ;
                    string[] values;
                    int c = INIHelper.GetAllKeyValues("BBB",out keys,out values, GlobalVariables.infoPath);
                    foreach (string s in keys)
                    {
                        if (s.StartsWith(rootPath + @"\" + fileName))
                        {
                            INIHelper.Write("BBB", s, "", GlobalVariables.infoPath);
                        }
                    }
                    Directory.Delete(rootPath + @"\" + fileName, true);
                    this.dataGridView1.Rows.Clear();
                    fileInit();
                }
            }
        }

        private string getFileName(int fileCount) 
        {
            string fileName = "";
            int ys = fileCount / 10;
            if(ys >= 1 && ys < 10)
            {
                fileName = "0" + fileCount;
            }
            else if(ys >= 10)
            {
                fileName = "" + fileCount;
            }
            else
            {
                fileName = "00" + fileCount;
            }
            return fileName;
        }

        private void createSaveBoxNoFile(string name)
        {
            //创建文件夹
            DirectoryInfo rootDir = new DirectoryInfo(rootPath);
            int fileCount = rootDir.GetDirectories().Length + 1;
            String path = rootPath + @"\" + name;
            if (!Directory.Exists(path))//判断目录是否存在
            {
                DirectoryInfo newDir = new DirectoryInfo(path);
                newDir.Create();
            }
        }

        private Boolean fileIsExists(string filePath)
        {
            Boolean isExists = false;
            DirectoryInfo rootDir = new DirectoryInfo(rootPath);
            int fileCount = rootDir.GetDirectories().Length + 1;
            String path = rootPath + @"\" + filePath;
            if (Directory.Exists(path))//判断目录是否存在
            {
                isExists = true;
            }
            return isExists;
        }
        
        private void button8_Click(object sender, EventArgs e)
        {
            if (null != this.dataGridView1.CurrentCell)
            {
                dLines.Clear();
                int sizes = this.dataGridView1.SelectedCells.Count;
                for (int i = 0; i < sizes; i++)
                {
                    string text = this.dataGridView1.SelectedCells[i].Value.ToString();
                    dLines.Add(text);
                }
                Replace frm1 = new Replace(rootPath, dLines);
                frm1.Show();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }






    }
}
