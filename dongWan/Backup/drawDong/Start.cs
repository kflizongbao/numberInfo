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
    public partial class Start : Form
    {
        private DirectoryInfo[] allDirs;
        public Start()
        {
            InitializeComponent();
            createXuanXiangBox();
        }

        private ArrayList dLines = new ArrayList();

        private int mainColumns = 13;
        private void Form1_Load(object sender, EventArgs e)
        {
            string runPath = System.IO.Directory.GetCurrentDirectory();

            if (!Directory.Exists(GlobalVariables.firPath))//判断目录是否存在
            {
                DirectoryInfo dir = new DirectoryInfo(GlobalVariables.firPath);
                dir.Create();
            }
            fileInit();
           
            //HttpClass hc = new HttpClass();
            //hc.available();
            dis();
        }

        private void fileInit()
        {
            DirectoryInfo rootDir = new DirectoryInfo(GlobalVariables.firPath);
            allDirs = rootDir.GetDirectories();
            int fileCount = rootDir.GetDirectories().Length;
            if (fileCount > 0)
            {
                for (int i = 0; i < fileCount; i++)
                {
                    setSaveBoxName();
                }
            }
            this.button1.Focus();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            createSaveBox();
            setSaveBoxName();
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            int rows = this.dataGridView1.Rows.Count;
            int columns = mainColumns;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    string text = this.dataGridView1[j, i].Value.ToString();
                    if (text.Length == 0)
                    {
                        //DataGridViewTextBoxCell xx = new DataGridViewTextBoxCell();
                        //this.dataGridView1[j, i] = xx;
                        //this.dataGridView1[j, i].Value = "";
                    }
                }
            }
        }

        private string createSaveBox()
        {
            //创建文件夹
            DirectoryInfo rootDir = new DirectoryInfo(GlobalVariables.firPath);
            allDirs = rootDir.GetDirectories();
            int fileCount = 0;
            foreach (DirectoryInfo d in allDirs)
            {
                string saveBoxName = d.Name;
                string name = saveBoxName.Replace("保存箱", "");
                int nameCount = int.Parse(name);
                if (nameCount > fileCount)
                {
                    fileCount = nameCount;
                }
            }
            ++fileCount;
            string fileCountName = "01";
            if (fileCount < 10)
            {
                fileCountName = "00" + fileCount;
            }
            else if (fileCount < 100 && fileCount >= 10)
            {
                fileCountName = "0" + fileCount;
            }
            else if (fileCount < 1000 && fileCount >= 100)
            {
                fileCountName = fileCount.ToString();
            }
            string path = GlobalVariables.firPath + @"\" + "保存箱" + fileCountName;
            if (!Directory.Exists(path))//判断目录是否存在
            {
                DirectoryInfo newDir = new DirectoryInfo(path);
                newDir.Create();
            }
            return path;

        }

        private void setSaveBoxName()
        {
            DirectoryInfo rootDir = new DirectoryInfo(GlobalVariables.firPath);
            allDirs = rootDir.GetDirectories();
            Boolean newRow = true;
            int rows = this.dataGridView1.Rows.Count;
            int columns = mainColumns;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    String text = this.dataGridView1[j, i].Value.ToString();
                    if (text.Length == 0)
                    {
                        int index = (rows - 1) * mainColumns + j + 1;
                        DataGridViewButtonCell xx = new DataGridViewButtonCell();
                        this.dataGridView1[j, i] = xx;
                        this.dataGridView1[j, i].Value = allDirs[index - 1].Name;
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
                            int text = (rowsNew - 1) * mainColumns + i + 1;
                            DataGridViewButtonCell xx = new DataGridViewButtonCell();
                            this.dataGridView1[i, rowsNew - 1] = xx;
                            this.dataGridView1[i, rowsNew - 1].Value = allDirs[text - 1].Name;
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

        private void button1_Click(object sender, EventArgs e)
        {
            String path = "";
            int count = this.dataGridView1.SelectedCells.Count;

            if (count == 0)
            {
                MessageBox.Show("请选择保存箱");
            }
            else
            {
                path = this.dataGridView1.SelectedCells[0].Value.ToString();
                Standard frm6 = new Standard(path);
                frm6.Show();
            }

        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            String content = this.dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
            /* Form frm = Application.OpenForms["Form2"];
             if (null != frm)
             {
                 frm.Show();
             }
             else
             {*/
            Form2 frm2 = new Form2(content);
            frm2.Show();
            //}
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string fileName = this.dataGridView1.CurrentCell.Value.ToString();
            if (fileName.Length > 0)
            {
                if (MessageBox.Show("确定删除文件夹--" + fileName + "--吗?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    string path = GlobalVariables.firPath + @"\" + fileName;
                    if (Directory.Exists(path))//判断目录是否存在
                    {
                        Directory.Delete(path, true);
                        string[] keys;
                        string[] values;
                        int c = INIHelper.GetAllKeyValues("BBB", out keys, out values, GlobalVariables.infoPath);
                        foreach (string s in keys)
                        {
                            if (s.StartsWith(path))
                            {
                                INIHelper.Write("BBB", s, "", GlobalVariables.infoPath);
                            }
                        }
                    }
                    this.dataGridView1.Rows.Clear();
                    fileInit();
                }

            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string fileName = this.dataGridView1.CurrentCell.Value.ToString();
            if (fileName.Length > 0)
            {
                if (MessageBox.Show("确定复制保存箱--" + fileName + "--吗?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    string saveBoxName = createSaveBox();
                    setSaveBoxName();
                    /*
                    string[] files = Directory.GetDirectories(GlobalVariables.firPath + @"\" + fileName);//();//取文件夹下所有文件名，放入数组；
                    if (files.Length > 0)
                    {
                        foreach (string s in files)
                        {
                            File.Copy(s, saveBoxName + @"\" + s.Substring(s.LastIndexOf("\\")));
                            Directory.c
                        }
                    }*/
                    CopyFolder(GlobalVariables.firPath + @"\" + fileName, saveBoxName);
                    this.dataGridView1.Rows.Clear();
                    fileInit();
                }

            }
        }
        private void CopyFolder(string from, string to)
        {
            if (!Directory.Exists(to))
                Directory.CreateDirectory(to);

            // 子文件夹
            foreach (string sub in Directory.GetDirectories(from))
                CopyFolder(sub + "\\", to + @"\" + Path.GetFileName(sub) + "\\");

            // 文件
            foreach (string file in Directory.GetFiles(from))
                File.Copy(file, to + Path.GetFileName(file), true);
        }

        private void button5_Click(object sender, EventArgs e)
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
            }
            MainReplace frm5 = new MainReplace(dLines);
            frm5.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string boxName = this.dataGridView1.CurrentCell.Value.ToString();
            if (boxName.Length > 0)
            {
                Form4 frm4 = new Form4(boxName);
                frm4.Show();
            }
            else 
            {
                MessageBox.Show("请选择保存箱", "提示", MessageBoxButtons.OK);
            }
 
        }

        private void createXuanXiangBox()
        {
            //创建文件夹
            string path = GlobalVariables.xuanxiangPath;
            if (!Directory.Exists(path))//判断目录是否存在
            {
                DirectoryInfo newDir = new DirectoryInfo(path);
                newDir.Create();
            }
        }

        private void dis()
        { 
             if (GlobalVariables.disa())
             {
                 this.button6.Visible = false;
                 this.button2.Visible = false;
                 this.button4.Visible = false;
             }
        }

    }
}
