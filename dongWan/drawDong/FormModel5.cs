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


namespace drawDong
{
    public partial class FormModel5 : Form
    {
        private string fileName;

        private ArrayList dLines = new ArrayList();

        private int rowCount = 0;

        private ArrayList dLinesOri = new ArrayList();

        private ArrayList dLinesData = new ArrayList();

        private ArrayList dLinesColumns = new ArrayList();//列索引
        private ArrayList dLinesColumnsCheck = new ArrayList();//需要处理列索引


        private ArrayList dLinesGroups = new ArrayList();//分组信息

        private int rowCountOri = 0;

        private int loadDataCount = 0;//每次加载30个数据,已经加载的数据

        private int canMaxColumnIndex = 0;

        public FormModel5(string p1, string p2, string p3)
        {
            InitializeComponent();
            fileName = GlobalVariables.firPath + @"\" + p1 + @"\" + p2 + @"\" + p3;
        }


        private void handleControl()
        {
            int wids = this.dataGridView1.Width;
            int heig = this.dataGridView1.Height;

            this.button6.Location = new Point(wids - this.button6.Width - 30, this.button6.Location.Y);
            this.button5.Location = new Point(wids - this.button5.Width - 30, this.button5.Location.Y);
            this.button4.Location = new Point(wids - this.button4.Width - 30, this.button4.Location.Y);
            this.button3.Location = new Point(wids - this.button3.Width - 30, this.button3.Location.Y);

            this.button2.Location = new Point(wids - this.button2.Width - 30, this.button2.Location.Y);
            this.button1.Location = new Point(wids - this.button1.Width - 30, this.button1.Location.Y);

            this.splitContainer1.SplitterDistance = 270;
        }

        private void FormModel5_Load(object sender, EventArgs e)
        {
            handleControl();

            loadData();
            loadDataOri();


            gdv1LoadData();
            gdv2LoadData(20);
        }


        private void loadData()
        {
            dLines.Clear();
            System.IO.StreamReader sr = new System.IO.StreamReader(fileName + "x.txt");
            while (!sr.EndOfStream)
            {
                string[] items = sr.ReadLine().Split(',');
                for (int i = 0; i < items.Length; i++)
                {
                    string value = items[i].Equals("-1") ? "" : items[i];
                    //if (value.Trim().Length > 0 )
                    //{
                    dLine dline = new dLine();
                    dline.setValue(value);
                    dline.setCloumnIndex(i);
                    dline.setRowIndex(rowCount);
                    dLines.Add(dline);
                    //}

                    if (value.Trim().Length > 0)
                    {
                        if (!dLinesColumns.Contains(i))
                        {
                            dLinesColumns.Add(i);
                            dLinesColumnsCheck.Add(i);
                        }
                    }
                }
                rowCount += 1;

            }
            sr.Close();
            dLinesColumns.Sort();
            dLinesColumnsCheck.Sort();

            if(dLinesColumnsCheck.Count >0)
            {
                canMaxColumnIndex = (int)dLinesColumnsCheck[dLinesColumnsCheck.Count - 1];
            }
            
        }

        private void loadDataOri()
        {
            dLinesOri.Clear();
            System.IO.StreamReader sr = new System.IO.StreamReader(fileName + ".txt");
            while (!sr.EndOfStream)
            {
                string[] items = sr.ReadLine().Split(',');
                for (int i = 0; i < items.Length; i++)
                {
                    string value = items[i].Equals("-1") ? "" : items[i];
                    if (value.Trim().Length > 0)
                    {
                        dLine dline = new dLine();
                        dline.setValue(value);
                        dline.setCloumnIndex(i);
                        dline.setRowIndex(rowCountOri);
                        dLinesOri.Add(dline);

                    }
                }
                rowCountOri += 1;

            }
            sr.Close();

        }

        private void gdv1LoadData()
        {
            for (int i = 0; i < rowCountOri; i++)
            {
                this.dataGridView1.Rows.Add();
                int row = this.dataGridView1.Rows.Count - 2;
                for (int j = 0; j < GlobalVariables.columnsSize; j++)
                {
                    this.dataGridView1.Rows[i].Cells[j].Value = getDataOri(i, j);
                }
            }

        }

        private void gdv2LoadData(int loadCount)
        {
            int readyLoadCount = loadCount;
            if (loadDataCount + loadCount > rowCount)
            {
                readyLoadCount = rowCount - loadDataCount;
            }
            else
            {
                readyLoadCount = loadCount;
            }
            for (int i = 0; i < readyLoadCount; i++)
            {
                this.dataGridView2.Rows.Add();
            }

            for (int i = loadDataCount; i < loadDataCount + readyLoadCount; i++)
            {
                for (int j = 0; j < canMaxColumnIndex + 1; j++)
                {
                    string content = getData(i, j);
                    if (content.Length > 0)
                    {
                        this.dataGridView2.Rows[i].Cells[j].Value = content;
                    }

                }
            }
            loadDataCount += readyLoadCount;
        }

        private String getDataOri(int rowIndex, int columnIndex)
        {
            String value = "";
            foreach (dLine dline in dLinesOri)
            {
                if (dline.getValue().Trim().Length > 0)
                {
                    if (dline.getCloumnIndex() == columnIndex && dline.getRowIndex() == rowIndex)
                    {
                        value = dline.getValue();
                        break;
                    }
                }

            }
            return value;
        }


        private String getData(int rowIndex, int columnIndex)
        {
            string value = "";
            foreach (dLine dline in dLines)
            {
                if (dline.getValue().Trim().Length > 0 && dline.getCloumnIndex() <= canMaxColumnIndex)
                {
                    if (dline.getCloumnIndex() == columnIndex && dline.getRowIndex() == rowIndex)
                    {
                        value = dline.getValue();
                        break;
                    }
                }
            }
            return value;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.splitContainer1.SplitterDistance = 45;
            this.button1.Text = "打开";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int j = 0; j < GlobalVariables.columnsSize; j++)
            {
                int wid = this.dataGridView2.Columns[j].Width;
                if (wid > 10)
                {
                    this.dataGridView2.Columns[j].Width -= 10;
                }
            }
            for (int i = 0; i < this.dataGridView2.Rows.Count; i++)
            {
                int hei = this.dataGridView2.Rows[i].Height;
                if (hei > 10)
                {
                    this.dataGridView2.Rows[i].Height -= 10;
                }
            }
            float cellFont = this.dataGridView2.DefaultCellStyle.Font.Size;

            if (cellFont > 9.0f)
            {
                this.dataGridView2.DefaultCellStyle.Font = new Font("宋体", cellFont - 2);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.button1.Text.Equals("打开"))
            {
                this.button1.Text = "编辑";
                this.splitContainer1.SplitterDistance = 270;
            }
            else
            {
                this.dataGridView1.ReadOnly = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            clearData(fileName);
            saveTxt();
            MessageBox.Show("保存成功!");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int j = 0; j < GlobalVariables.columnsSize; j++)
            {
                this.dataGridView2.Columns[j].Width += 10;
            }
            for (int i = 0; i < this.dataGridView2.Rows.Count; i++)
            {
                this.dataGridView2.Rows[i].Height += 10;
            }

            float cellFont = this.dataGridView2.DefaultCellStyle.Font.Size;
            this.dataGridView2.DefaultCellStyle.Font = new Font("宋体", cellFont + 2);
        }
        private void clearData(String filePath)
        {
            string path = filePath + ".txt";
            FileStream fs = File.OpenWrite(path);
            fs.SetLength(0);
            fs.Close();
        }

        private void saveTxt()
        {
            FileStream fileStream = new FileStream(fileName + ".txt", FileMode.OpenOrCreate);
            StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
            StringBuilder strBuilder = new StringBuilder();
            try
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    strBuilder = new StringBuilder();
                    for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    {
                        if (null == dataGridView1.Rows[i].Cells[j].Value)
                        {
                            strBuilder.Append("-1" + ",");
                        }
                        else
                        {
                            String cellContent = dataGridView1.Rows[i].Cells[j].Value.ToString();
                            if (cellContent.Length == 0)
                            {
                                cellContent = "-1";
                            }
                            strBuilder.Append(dataGridView1.Rows[i].Cells[j].Value.ToString() + ",");
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


    }
}
