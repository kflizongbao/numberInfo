using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;



namespace drawDong
{
    public partial class Form4 : Form
    {
        private string saveBoxName;
        private ArrayList dLines = new ArrayList();//文件下面所有的数据
        private ArrayList dLinesCheck = new ArrayList();
        private ArrayList dLinesRead = new ArrayList();
        private int rowCount;
        private int itemSize = 0, itemSizeDown = 0;

        public Form4()
        {
            InitializeComponent();
        }

        public Form4(string saveBoxName)
        {
            this.saveBoxName = saveBoxName;
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            int wids = this.dataGridView1.Width;
            int heig = this.dataGridView1.Height;

            this.button3.Location = new Point(wids - this.button3.Width - 20, heig - this.button3.Height - 20);

            loadData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dLinesRead.Clear();
            System.IO.StreamReader sr = new System.IO.StreamReader(GlobalVariables.xuanxiangPath + @"\" + this.dataGridView1.CurrentCell.Value.ToString().Trim() + ".txt");
            while (!sr.EndOfStream)
            {
                string[] items = sr.ReadLine().Split(',');
                for (int i = 0; i < items.Length; i++)
                {
                    string value = items[i].Equals("-1") ? "" : items[i];
                    if (value.Trim().Length > 0)
                    {
                        dLinesRead.Add(value);
                    }
                }

            }
            sr.Close();

            this.progressBar1.Visible = true;
            int n =0;
            foreach (String s in dLinesRead)
            {
                rep(s);
                n++;
                int v = Convert.ToInt32(Convert.ToDouble(n) / Convert.ToDouble(dLinesRead.Count) * 100);
                this.progressBar1.Value = v;
            }
            this.progressBar1.Visible = false;

            if (MessageBox.Show("替换成功", "提示", MessageBoxButtons.OK) == DialogResult.OK)
            {
                
            }
        }

        private void rep(string content)
        {
            if (content.Length == 0)
            {
                MessageBox.Show("请输入一个数字", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            else
            {
                string rePath = saveBoxName;
                bianLi(GlobalVariables.firPath + @"\" + rePath, content);
            }
        }


        private void bianLi(string path, string content)
        {

            //创建文件夹
            DirectoryInfo rootDir = new DirectoryInfo(path);
            Boolean exists = Directory.Exists(path);
            if (exists)//判断目录是否存在
            {
                DirectoryInfo[] files = rootDir.GetDirectories();
                for (int i = 0; i < files.Length; i++)
                {
                    DirectoryInfo tit = files[i];
                    string fullName = tit.FullName;
                    bianLiNext(fullName, content);
                }
            }
        }

        private void bianLiNext(string path, string content)
        {

            //创建文件夹
            DirectoryInfo rootDir = new DirectoryInfo(path);
            Boolean exists = Directory.Exists(path);
            if (exists)//判断目录是否存在
            {
                FileInfo[] files = rootDir.GetFiles();
                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo tit = files[i];
                    string filePath = tit.FullName;

                    if (filePath.EndsWith("x.txt"))
                    {
                        loadData(filePath.Replace("x.txt", ".txt"));
                        loadDataDown(filePath);
                        handleContent(filePath, content);
                        replaceContent(filePath, content);
                    }
                }
            }
        }

        private void loadDataDown(string fileName)
        {
            itemSizeDown = 0;
            System.IO.StreamReader sr = new System.IO.StreamReader(fileName);
            while (!sr.EndOfStream)
            {
                string[] items = sr.ReadLine().Split(',');
                if (itemSizeDown == 0)
                {
                    itemSizeDown = items.Length;
                }
                break;
            }
            sr.Close();
        }

        private void loadData(string fileName)
        {
            rowCount = 0;
            dLines.Clear();
            System.IO.StreamReader sr = new System.IO.StreamReader(fileName);
            while (!sr.EndOfStream)
            {
                string[] items = sr.ReadLine().Split(',');
                if (itemSize == 0)
                {
                    itemSize = items.Length;
                }
                for (int i = 0; i < items.Length; i++)
                {
                    dLine dline = new dLine();
                    dline.setValue(items[i].Equals("-1") ? "" : items[i]);
                    dline.setCloumnIndex(i);
                    dline.setRowIndex(rowCount);
                    dLines.Add(dline);
                }
                rowCount += 1;

            }
            sr.Close();
        }

        private void handleContent(string path, string content)
        {
            dLinesCheck.Clear();
            //查询符合要求的字符所在的列
            foreach (dLine item in dLines)
            {
                string itemValue = item.getValue();

                if (null != itemValue && itemValue.Equals(content) && item.getCloumnIndex() < itemSizeDown)
                {
                    int columnIndex = item.getCloumnIndex();
                    if (!dLinesCheck.Contains(columnIndex))
                    {
                        dLinesCheck.Add(columnIndex);
                    }
                }
            }
            dLinesCheck.Sort();
        }

        private void replaceContent(string path, string content)
        {
            //生成新的一行数据追加到文件最后
            string[] contents = new string[itemSize];
            int count = dLinesCheck.Count;
            for (int i = 0; i < count; i++)
            {
                int cIndex = (int)dLinesCheck[i];
                contents[cIndex] = content;
            }
            StringBuilder strBuilder = new StringBuilder();
            for (int j = 0; j < contents.Length; j++)
            {
                string con = contents[j];
                if (null == con)
                {
                    con = "";
                }
                strBuilder.Append(con + ",");
            }
            strBuilder.Remove(strBuilder.Length - 1, 1);
            string contentStr = strBuilder.ToString();


            //FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
            StreamWriter streamWriter = File.AppendText(path);//new StreamWriter(fileStream, System.Text.Encoding.Unicode);
            try
            {
                if (contentStr.Length > 0)
                {
                    streamWriter.WriteLine(contentStr);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                streamWriter.Close();
                // fileStream.Close();
            }

        }



        private void setSaveBoxName()
        {
            DirectoryInfo rootDir = new DirectoryInfo(GlobalVariables.firPath);
            FileInfo[]  allDirs = rootDir.GetFiles();
            int rows = this.dataGridView1.Rows.Count;
            int columns = 13;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    String text = this.dataGridView1[j, i].Value.ToString();
                    if (text.Length == 0)
                    {
                        DataGridViewTextBoxCell xx = new DataGridViewTextBoxCell();
                        this.dataGridView1[j, i] = xx;
                        this.dataGridView1[j, i].Value = "";
                    }
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }


        private void open() 
        {
            string name = this.dataGridView1.CurrentCell.Value.ToString();

            Form8 frm = new Form8(name);
            frm.Show();
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            open();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            open();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (null == this.dataGridView1.CurrentCell)
            {
                return;
            }
            string fileName = GlobalVariables.xuanxiangPath + @"\" + this.dataGridView1.CurrentCell.Value.ToString() + ".txt";
            if (null != fileName && File.Exists(fileName))
            {
                File.Delete(fileName);
                loadData();
            }
        }


        private void loadData()
        {
            string str = ""; ;
            int arg0 = 0;
            this.dataGridView1.Rows.Clear();
            DirectoryInfo dirFile = new DirectoryInfo(GlobalVariables.xuanxiangPath);
            FileInfo[] files = dirFile.GetFiles();
            int s = files.Length % 13 > 0 ? files.Length / 13 + 1 : files.Length / 13;

            for (int i = 0; i < s; i++)
            {
                this.dataGridView1.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "");
            }

            for (int i = 0; i < files.Length; i++)
            {
                FileInfo fi = files[i];
                string name = fi.Name.Substring(0, 6);
                int s1 = i / 13;
                int s2 = i % 13;
                this.dataGridView1[s2, s1].Value = name;

                int arg1 = Convert.ToInt32(name);

                if (arg1 > arg0)
                {
                    arg0 = arg1;
                }
            }
            string arg2 = "";
            if (arg0 > 0)
            {
                arg2 = (Convert.ToInt32(arg0)).ToString();
            
            }

            str = arg2;

            for (int i = arg2.Length; i < 6; i++)
            {
                str = "0" + str;
            }
            INIHelper.Write("AAA", "name", str, GlobalVariables.confPath);
            setSaveBoxName();
        }



    }
}
