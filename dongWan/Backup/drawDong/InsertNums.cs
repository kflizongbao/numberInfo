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
    public partial class InsertNums : Form
    {
        private string rootPath;

        private ArrayList dLines = new ArrayList();//文件下面所有的数据
        private ArrayList dLinesCheck = new ArrayList();//文件下面所有符合要求的数据的列

        private ArrayList dLinesX = new ArrayList();
        private ArrayList dLinesCheckX = new ArrayList();

        private int rowCount = 0;

        public InsertNums(string path)
        {
            InitializeComponent();
            this.rootPath = path;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string content = this.textBox1.Text.ToString().Trim();
            if (content.Length == 0)
            {
                MessageBox.Show("请输入一个数字", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            else
            {
                 bianLi(content);
            }

        }

        private void bianLi(string content)
        {
            
            //创建文件夹
            DirectoryInfo rootDir = new DirectoryInfo(rootPath);
            Boolean  exists = Directory.Exists(rootPath);
            if (exists)//判断目录是否存在
            {
                FileInfo[] files = rootDir.GetFiles();
                for (int i = 0; i< files.Length ;i++ )
                {
                    FileInfo tit = files[i];
                    string filePath = tit.FullName;

                    if (filePath.EndsWith("x.txt"))
                    {
                        loadData(filePath.Replace("x.txt",".txt"));
                        handleContent(filePath, content);
                        replaceContent(filePath, content);
                    }
                }
            }
            this.Close();
        }


        private void  handleContent(string path,string content)
        {
            dLinesCheck.Clear();
            //查询符合要求的字符所在的列
            foreach (dLine item in dLines)
            {
                string itemValue = item.getValue();

                if (null != itemValue && itemValue.Equals(content))
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
            string[] contents = new string[GlobalVariables.columnsSize];
            int count = dLinesCheck.Count;
            for (int i = 0; i < count; i++)
            {
                int cIndex = (int)dLinesCheck[i];
                contents[cIndex] = content;
            }
            StringBuilder strBuilder = new StringBuilder();
            for (int j = 0; j< contents.Length ;j++ )
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

        private int getMaxRowIndex()
        {
            int rowIndex = 0;
            foreach (dLine item in dLinesX)
            {
                if (null != item.getValue()  &&  item.getValue().Length > 0)
                {
                    rowIndex = item.getRowIndex();
                }
            }
            return rowIndex;
        }


        private void Form10_Load(object sender, EventArgs e)
        {
            this.label1.Text = this.rootPath;
        }

        private void loadData(string fileName)
        {
            rowCount = 0;
            dLines.Clear();
            System.IO.StreamReader sr = new System.IO.StreamReader(fileName);
            while (!sr.EndOfStream)
            {
                string[] items = sr.ReadLine().Split(',');
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

        //记载显示数据
        private void loadDataX(string fileName)
        {
            rowCount = 0;
            dLines.Clear();
            System.IO.StreamReader sr = new System.IO.StreamReader(fileName);
            while (!sr.EndOfStream)
            {
                string[] items = sr.ReadLine().Split(',');
                for (int i = 0; i < items.Length; i++)
                {
                    dLine dline = new dLine();
                    dline.setValue(items[i].Equals("-1") ? "" : items[i]);
                    dline.setCloumnIndex(i);
                    dline.setRowIndex(rowCount);
                    dLinesX.Add(dline);
                }
                rowCount += 1;

            }
            sr.Close();
        }






    }
}
