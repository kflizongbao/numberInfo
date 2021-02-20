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
using System.Threading;
using System.Reflection;

namespace drawDong
{
    public partial class Standard : Form
    {
        private string savePath ,rootPath;
        public Standard(String  path)
        {
            InitializeComponent();
            for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
            {
                this.dataGridView1.Columns[i].Width = 30;
            }

            this.savePath = path;
            rootPath = GlobalVariables.firPath + @"\" + savePath;

            if (GlobalVariables.disa())
            {
                this.button1.Visible = false;
            }

            var dgvType = this.dataGridView1.GetType();
            var pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(this.dataGridView1, true, null);
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            insertDatas();
        }

        private void insertDatas()
        {
            for (int i = 0; i< 95;i++ )
            {
                this.dataGridView1.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
            }
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //遍历目录下面的文件夹查看左后一个文件的个数
            int maxNum = getMaxNum();
            switch (maxNum)
            {
                case 9:
                    //6个文件(0-9)
                    files6();
                    break;
                case 33:
                    //13个文件(1-33)
                    files33();
                    break;
                case 49:
                    //（36 个文件（6条）或者是12个文件（4条）(1-49)
                    files49();
                    break;
                default:

                    break;
            }
            this.Close();
        }

        private void files49()
        {
            DirectoryInfo dirFile = new DirectoryInfo(rootPath);
            DirectoryInfo[] files = dirFile.GetDirectories();

            string lastFilesName = "";
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = files[i].Name;
                if (i == files.Length - 1)
                {
                    lastFilesName = fileName;
                }
            }


            if (lastFilesName.Trim().Length == 0)
            {
                lastFilesName = "A001";
                createSaveBox(lastFilesName);
            }



            int fileCount = 0;
            string fileSonPath = rootPath + @"\" + lastFilesName;
            DirectoryInfo dir = new DirectoryInfo(fileSonPath);
            fileCount = dir.GetFiles().Length / 2;

            int counts = getColumns();
            int fileCounts = 36;
            if (counts == 6)
            {
                fileCounts = 36;
            }
            else
            {
                fileCounts = 12;
            }
            if (fileCount < fileCounts)
            {
                string fileTxtName = (fileCount + 1).ToString();
                createTxtFile(fileSonPath + @"\" + "B" + fileTxtName);
                saveTxt(fileSonPath + @"\" + "B" + fileTxtName);


                createTxtFile(fileSonPath + @"\" + "B" + fileTxtName + "x");
                saveEmpTxt(fileSonPath + @"\" + "B" + fileTxtName + "x");
            }
            else
            {
                //创建文件夹
                int fileIndex = Convert.ToInt16(lastFilesName.Substring(1, 3));
                string fileName = "A" + getFileName(fileIndex + 1);
                createSaveBox(fileName);
                createTxtFile(rootPath + @"\" + fileName + @"\" + "B1");
                saveTxt(rootPath + @"\" + fileName + @"\" + "B1");


                createTxtFile(rootPath + @"\" + fileName + @"\" + "B1" + "x");
                saveEmpTxt(rootPath + @"\" + fileName + @"\" + "B1" + "x");
            }
        }

        //判断是几条(1-49 判断是不是是不是存在6条的不存在就是4条)
        private int getColumns()
        {
            int columnCount = this.dataGridView1.ColumnCount;
            int rowCount = this.dataGridView1.RowCount;
            int nums = 4; 
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (j < columnCount - 7)
                    {
                        string con1 = null, con2 = null, con3 = null, con4 = null, con5 = null, con6 = null;
                        if (null != this.dataGridView1.Rows[i].Cells[j].Value)
                        {
                            con1 = this.dataGridView1.Rows[i].Cells[j].Value.ToString();
                        }

                        if (null != this.dataGridView1.Rows[i].Cells[j + 1].Value)
                        {
                            con2 = this.dataGridView1.Rows[i].Cells[j + 1].Value.ToString();
                        }

                        if (null != this.dataGridView1.Rows[i].Cells[j + 2].Value)
                        {
                            con3 = this.dataGridView1.Rows[i].Cells[j + 2].Value.ToString();
                        }

                        if (null != this.dataGridView1.Rows[i].Cells[j + 3].Value)
                        {
                            con4 = this.dataGridView1.Rows[i].Cells[j + 3].Value.ToString();
                        }

                        if (null != this.dataGridView1.Rows[i].Cells[j + 4].Value)
                        {
                            con5 = this.dataGridView1.Rows[i].Cells[j + 4].Value.ToString();
                        }

 
                        if (null != this.dataGridView1.Rows[i].Cells[j + 5].Value)
                        {
                            con6 = this.dataGridView1.Rows[i].Cells[j + 5].Value.ToString();
                        }

                        if (null != con1 && null != con2 && null != con3 && null != con4 && null != con5 && null != con6)
                        {
                            if (con1.Length > 0 && con2.Length > 0 && con3.Length > 0 && con4.Length > 0 && con5.Length > 0 && con6.Length > 0 )
                            {
                                nums = 6;
                                goto Label1;   
                            }
                        }



                    }
                }
            }

            Label1: { 
            
            }
            return nums;
        }


        private void files33()
        {
            DirectoryInfo dirFile = new DirectoryInfo(rootPath);
            DirectoryInfo[] files = dirFile.GetDirectories();

            string lastFilesName = "";
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = files[i].Name;
                if (i == files.Length - 1)
                {
                    lastFilesName = fileName;
                }
            }

            if (lastFilesName.Trim().Length == 0)
            {
                lastFilesName = "A001";
                createSaveBox(lastFilesName);
            }


            int fileCount = 0;
            string fileSonPath = rootPath + @"\" + lastFilesName;
            DirectoryInfo dir = new DirectoryInfo(fileSonPath);
            fileCount = dir.GetFiles().Length / 2 ;
            if (fileCount < 13)
            {
                string fileTxtName = (fileCount + 1).ToString();
                createTxtFile(fileSonPath + @"\" + "B" + fileTxtName);
                saveTxt(fileSonPath + @"\" + "B" + fileTxtName);


                createTxtFile(fileSonPath + @"\" + "B" + fileTxtName + "x");
                saveEmpTxt(fileSonPath + @"\" + "B" + fileTxtName + "x");
            }
            else
            {
                //创建文件夹
                int fileIndex = Convert.ToInt16(lastFilesName.Substring(1, 3));
                string fileName = "A" + getFileName(fileIndex + 1);
                createSaveBox(fileName);
                createTxtFile(rootPath + @"\" + fileName + @"\" + "B1");
                saveTxt(rootPath + @"\" + fileName + @"\" + "B1");

                createTxtFile(rootPath + @"\" + fileName + @"\" + "B1" + "x");
                saveEmpTxt(rootPath + @"\" + fileName + @"\" + "B1" + "x");
            }       
        }
        private void files6()
        {
            DirectoryInfo dirFile = new DirectoryInfo(rootPath);
            DirectoryInfo[]  files = dirFile.GetDirectories();

            string lastFilesName = "";
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = files[i].Name;
                if (i == files.Length - 1)
                {
                    lastFilesName = fileName;
                }   
            }

            if (lastFilesName.Trim().Length == 0)
            {
                lastFilesName = "A001";
                createSaveBox(lastFilesName);
            }


           int fileCount = 0;
           string fileSonPath = rootPath + @"\" + lastFilesName;
           DirectoryInfo dir = new DirectoryInfo(fileSonPath);
           fileCount = dir.GetFiles().Length / 2;
           if (fileCount < 6)
           {
               string fileTxtName = (fileCount + 1).ToString();
               createTxtFile(fileSonPath + @"\" + "B" + fileTxtName);
               saveTxt(fileSonPath + @"\" + "B" + fileTxtName);


               createTxtFile(fileSonPath + @"\" + "B" + fileTxtName + "x");
               saveEmpTxt(fileSonPath + @"\" + "B" + fileTxtName + "x");
           }
           else
           {
                //创建文件夹
               int fileIndex = Convert.ToInt16(lastFilesName.Substring(1,3));
               string fileName = "A" + getFileName(fileIndex + 1);
               createSaveBox(fileName);
               createTxtFile(rootPath + @"\"+  fileName + @"\" + "B1");
               saveTxt(rootPath + @"\" + fileName + @"\" + "B1");


               createTxtFile(rootPath + @"\" + fileName + @"\" + "B1" + "x");
               saveEmpTxt(rootPath + @"\" + fileName + @"\" + "B1"+"x");
           }
        }

        private string getFileName(int  num)
        {
            string fileName = "";
            if(num / 10 == 0)
            {
                fileName = "00" + num.ToString();
            }

            if(num / 10 >= 1 && num/10 <= 9)
            {
                fileName = "0" + num.ToString();
            }

            if (num / 100 >= 1)
            {
                fileName = "0" + num.ToString();
            }

            return fileName;
        }

        private void saveTxt(string  fileName)
        {
            FileStream fileStream = new FileStream(fileName + ".txt", FileMode.OpenOrCreate);
            StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
            StringBuilder strBuilder = new StringBuilder();
            try
            {
                for (int i = 0; i < this.dataGridView1.Rows.Count - 1; i++)
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

        }

        private void saveEmpTxt(string fileName)
        {
            FileStream fileStream = new FileStream(fileName + ".txt", FileMode.OpenOrCreate);
            StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
            StringBuilder strBuilder = new StringBuilder();
            try
            {
                for (int i = 0; i < this.dataGridView1.Rows.Count - 1; i++)
                {   /*
                    strBuilder = new StringBuilder();
                    for (int j = 0; j < this.dataGridView1.Columns.Count; j++)
                    {
                
                         strBuilder.Append("" + ",");
                       
                    }
                    strBuilder.Remove(strBuilder.Length - 1, 1);
                    String content = strBuilder.ToString();
                    streamWriter.WriteLine(content);
                     * */
                }
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

        }
        private void createSaveBox(string name)
        {
            //创建文件夹
            string path = rootPath + @"\" + name;
            if (!Directory.Exists(path))//判断目录是否存在
            {
                DirectoryInfo newDir = new DirectoryInfo(path);
                newDir.Create();
            }
        }

        private void createTxtFile(String fileName)
        {
            if (!File.Exists(fileName + ".txt"))
            {
                FileStream fs = new FileStream(fileName + ".txt", FileMode.Create, FileAccess.ReadWrite);
                fs.Close();
            }
        }

        private int getMaxNum()
        { 
            int columnCount = this.dataGridView1.Columns.Count ;
            int rowCount = this.dataGridView1.Rows.Count;
            int maxNum = 0;
            for (int i = 0; i < rowCount ;i++ )
            {
                for (int j = 0; j < columnCount; j++)
                {
                    string cellContent = "";
                    if (null != this.dataGridView1.Rows[i].Cells[j].Value)
                    {
                        cellContent = this.dataGridView1.Rows[i].Cells[j].Value.ToString();
                    }
                    
                    if (cellContent.Length > 0)
                    {
                        int cellNum = int.Parse(cellContent);
                        maxNum = cellNum > maxNum ? cellNum : maxNum;
                    }
                }
            }
            return maxNum;
        }
    }
}
