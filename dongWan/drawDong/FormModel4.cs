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
    public partial class FormModel4 : Form
    {
        private string fileName;

        private ArrayList dLines = new ArrayList();

        private int rowCount = 0, columnSize = 41;

        private ArrayList dLinesOri = new ArrayList();

        private ArrayList dLinesData = new ArrayList();

        private ArrayList dLinesColumns = new ArrayList();//列索引
        
        private ArrayList dLinesColumnsCheck = new ArrayList();//需要处理列索引

        private ArrayList dLinesDataOri = new ArrayList();

        private ArrayList dLinesGroups = new ArrayList();//分组信息

        private int rowCountOri = 0;

        private int loadDataCount = 0;//每次加载20个数据,已经加载的数据

        private int canMaxColumnIndex = 0;

        private Boolean isLoad = false;
        private static System.Timers.Timer CheckUpdatetimer = new System.Timers.Timer();

        private static int preRow;
        private static int[] preNumbers;
        private int index = 0, index0 = 0;
        private Boolean ts = false;
        private int maxColum;
        public FormModel4(string p1, string p2, string p3)
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();

            InitializeComponent();
            chkControl();
            fileName = GlobalVariables.firPath + @"\" + p1 + @"\" + p2 + @"\" + p3;

        }

        private void FormModel4_Load(object sender, EventArgs e)
        {
            handleControl();

            loadData();
            loadDataOri();

            string d = DateTime.Now.ToString();
            handleGroup();
            handleGroupData1();
            string d1 = DateTime.Now.ToString();
            gdv1LoadData();
            gdv2LoadData(20);
            gdv2LoadData(20);
            gdv2LoadData(10);

            CheckUpdatetimer.Interval = 500;
            CheckUpdatetimer.Enabled = true;
            CheckUpdatetimer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            CheckUpdatetimer.Start();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DoWork();
            CheckUpdatetimer.Stop();
        }

        public delegate void MyInvoke();

        public void DoWork()
        {
            MyInvoke mi = new MyInvoke(loadDatas);
            if (this.IsHandleCreated && !isLoad)
            {
                this.BeginInvoke(mi);
                isLoad = true;
            }
        }

        private void loadDatas()
        {
            run();
        }

        private void run()
        {
            int yu = rowCount % 20;
            int size = rowCount / 20 + (yu > 0 ? 1 : 0);
            int step = 100 / size;
            step = (step == 0) ? 1 : step;

            String d = DateTime.Now.ToString();
            Console.WriteLine(d);

            for (int i = 0; i < size; i++)
            {
                String d2 = DateTime.Now.ToString();
                Console.WriteLine(d2);
                gdv2LoadData(20);
                String d3 = DateTime.Now.ToString();
                Console.WriteLine(d3);

                if (this.progressBar1.Value < 100 - step)
                {
                    this.progressBar1.Value += step;
                }
            }
            String d1 = DateTime.Now.ToString();
            Console.WriteLine(d1);

            this.progressBar1.Visible = false;

            for (int i = 0; i < 5; i++)
            {
                this.dataGridView2.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
            }
            if (this.dataGridView2.Rows.Count > 10)
            {
                this.dataGridView2.FirstDisplayedScrollingRowIndex = this.dataGridView2.Rows[this.dataGridView2.RowCount - 1].Index;
            }
        }

        private void chkControl()
        {
            for (int i = this.dataGridView1.Columns.Count; i < GlobalVariables.columnsSetSize; i++)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                this.dataGridView1.Columns.Add(column);
            }

            for (int i = this.dataGridView2.Columns.Count; i < GlobalVariables.columnsSetSize; i++)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                this.dataGridView2.Columns.Add(column);
            }


            for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
            {
                this.dataGridView1.Columns[i].Width = 20;
            }
            for (int i = 0; i < this.dataGridView2.Columns.Count; i++)
            {
                this.dataGridView2.Columns[i].Width = 20;
            }

            var dgvType = this.dataGridView2.GetType();
            var pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(this.dataGridView2, true, null);


            var dgvType1 = this.dataGridView1.GetType();
            var pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(this.dataGridView1, true, null);
        }

        //初始化分组信息
        private void handleGroup()
        {
            for (int i = 0; i < dLinesColumns.Count; i++)
            {
                if (i > 0 && i % 2 == 1)
                {
                    ArrayList groups = new ArrayList();
                    int max = (int)dLinesColumns[i];
                    int min = (int)dLinesColumns[i - 1];
                    groups.Add(min);
                    groups.Add(max);
                    groups.Sort();
                    dLinesGroups.Add(groups);

                    for (int j = min; j <= max; j++)
                    {
                        if (!dLinesColumnsCheck.Contains(j))
                        {
                            dLinesColumnsCheck.Add(j);//需要处理的列信息
                        }
                    }
                }
            }
        }

        //处理分组数据
        private void handleGroupData()
        {
            foreach (ArrayList ar in dLinesGroups)
            {
                int min = (int)ar[0];
                int max = (int)ar[1];
                foreach (dLine dline in dLines)
                {

                    int cloumnIndex = dline.getCloumnIndex();
                    if (cloumnIndex <= max && cloumnIndex >= min)
                    {
                        if (cloumnIndex == min & (null == dline.getValue() || dline.getValue().Length == 0) && (cloumnIndex < (int)dLinesColumnsCheck[dLinesColumnsCheck.Count - 1]))
                        {
                            //dline.setRight(1);
                            int pre = dline.getRowIndex();

                            //判断下一个
                            Boolean nextExists = false;
                            foreach (dLine dlin in dLines)
                            {
                                //查询下一个是不是空格
                                if (dlin.getRowIndex() == (pre + 1) && dlin.getCloumnIndex() == min && (null == dlin.getValue() || dlin.getValue().Length == 0))
                                {
                                    nextExists = true;
                                    break;
                                }
                            }
                            int rowindex = rowCount;
                            if (!nextExists && (pre + 1 != rowCount))
                            {
                                foreach (dLine dl in dLines)
                                {
                                    if (dl.getRowIndex() == pre && (dl.getCloumnIndex() > min && dl.getCloumnIndex() < max))
                                    {
                                        dl.setBottom(1);
                                    }
                                }
                            }

                            //判断上一个
                            Boolean preExists = false;
                            foreach (dLine dlin in dLines)
                            {
                                //查询下一个是不是空格
                                if (dlin.getRowIndex() == (pre - 1) && dlin.getCloumnIndex() == min && (null == dlin.getValue() || dlin.getValue().Length == 0))
                                {
                                    preExists = true;
                                    break;
                                }
                            }
                            if (!preExists)
                            {
                                foreach (dLine dl in dLines)
                                {
                                    if (dl.getRowIndex() == pre && (dl.getCloumnIndex() > min && dl.getCloumnIndex() < max))
                                    {
                                        dl.setTop(1);
                                    }
                                }
                            }
                        }

                    }

                }
            }


        }

        //处理分组数据
        private void handleGroupData1()
        {
            foreach (ArrayList ar in dLinesGroups)
            {
                int min = (int)ar[0];
                int max = (int)ar[1];
                int ss = max;
                foreach (dLine dline in dLines)
                {

                    int cloumnIndex = dline.getCloumnIndex();
                    if (cloumnIndex <= max && cloumnIndex >= min)
                    {
                        if (cloumnIndex == min & (null == dline.getValue() || dline.getValue().Length == 0) && (cloumnIndex < (int)dLinesColumnsCheck[dLinesColumnsCheck.Count - 1]))
                        {
                            //dline.setRight(1);
                            int pre = dline.getRowIndex();

                            //判断下一个
                            Boolean nextExists = false;
                            /*
                            foreach (dLine dlin in dLines)
                            {
                                //查询下一个是不是空格
                                if (dlin.getRowIndex() == (pre + 1) && dlin.getCloumnIndex() == min && (null == dlin.getValue() || dlin.getValue().Length == 0))
                                {
                                    nextExists = true;
                                    break;
                                }
                            }*/
                            int colIndex = min;
                            int rowIndex = pre + 1;
                            if (rowIndex < rowCount)
                            {
                                int p3 = rowIndex * columnSize + colIndex;
                                dLine dl3 = (dLine)dLines[p3];
                                string value3 = dl3.getValue();
                                if (null == value3 || value3.Length == 0)
                                {
                                    nextExists = true;
                                }
                            }

                            int rowindex = rowCount;
                            if (!nextExists && (pre + 1 != rowCount))
                            {
                                /*
                                foreach (dLine dl in dLines)
                                {
                                    if (dl.getRowIndex() == pre && (dl.getCloumnIndex() > min && dl.getCloumnIndex() < max))
                                    {
                                        dl.setBottom(1);
                                    }
                                }*/

                                int colIndex1 = min;
                                int rowIndex1 = pre;
                                for (int i = min + 1; i < max; i++)
                                {
                                    colIndex1 = i;
                                    int p = rowIndex1 * columnSize + colIndex1;
                                    dLine dl = (dLine)dLines[p];
                                    dl.setBottom(1);
                                }



                            }

                            //判断上一个
                            Boolean preExists = false;

                            int pColIndex = min;
                            int pRowIndex = pre - 1;
                            if (pRowIndex > 0)
                            {
                                int p2 = pRowIndex * columnSize + pColIndex;
                                dLine dl2 = (dLine)dLines[p2];
                                string value2 = dl2.getValue();
                                if (null == value2 || value2.Length == 0)
                                {
                                    preExists = true;
                                }
                            }

                            /*
                            foreach (dLine dlin in dLines)
                            {
                                //查询下一个是不是空格
                                if (dlin.getRowIndex() == (pre - 1) && dlin.getCloumnIndex() == min && (null == dlin.getValue() || dlin.getValue().Length == 0))
                                {
                                    preExists = true;
                                    break;
                                }
                            }*/



                            if (!preExists)
                            {
                                /*
                                foreach (dLine dl in dLines)
                                {
                                    if (dl.getRowIndex() == pre && (dl.getCloumnIndex() > min && dl.getCloumnIndex() < max))
                                    {
                                        dl.setTop(1);
                                    }
                                }*/

                                int nColIndex1 = min;
                                int nRowIndex1 = pre;
                                for (int i = min + 1; i < max; i++)
                                {
                                    nColIndex1 = i;
                                    int p6 = nRowIndex1 * columnSize + nColIndex1;
                                    dLine dl6 = (dLine)dLines[p6];
                                    dl6.setTop(1);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void loadData()
        {
            int s1 = 0;
            System.IO.StreamReader sr1 = new System.IO.StreamReader(fileName + "x.txt");
            while (!sr1.EndOfStream)
            {
                string[] items = sr1.ReadLine().Split(',');
                if (s1 <= items.Length)
                {
                    s1 = items.Length;
                }
            }

            dLines.Clear();
            System.IO.StreamReader sr = new System.IO.StreamReader(fileName + "x.txt");
            while (!sr.EndOfStream)
            {
                string[] items = sr.ReadLine().Split(',');
                if (items.Length > columnSize)
                {
                    columnSize = items.Length;
                }
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
                for (int j = items.Length; j < s1; j++)
                {
                    dLine dline = new dLine();
                    dline.setValue("");
                    dline.setCloumnIndex(j);
                    dline.setRowIndex(rowCount);
                    dLines.Add(dline);
                }
                rowCount += 1;

            }
            sr.Close();
            dLinesColumns.Sort();
            dLinesColumnsCheck.Sort();
            if (dLinesColumnsCheck.Count > 0)
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
                        if (i > maxColum)
                        {
                            maxColum = i;
                        }
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
                //int row = this.dataGridView1.Rows.Count - 2;
               // for (int j = 0; j < maxColum + 1; j++)
                //{
                //    this.dataGridView1.Rows[i].Cells[j].Value = getDataOri(i, j);
                //}
            }

            foreach (dLine dline in dLinesOri)
            {
                int columnIndex = dline.getCloumnIndex();
                int rowIndex = dline.getRowIndex();
                this.dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = dline.getValue();
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

            String d5 = DateTime.Now.ToString();
            Console.WriteLine(d5);
            for (int i = 0; i < readyLoadCount; i++)
            {
                this.dataGridView2.Rows.Add();
            }
            String d6 = DateTime.Now.ToString();
            Console.WriteLine(d6);

            for (int i = loadDataCount; i < loadDataCount + readyLoadCount; i++)
            {
                for (int j = 0; j < canMaxColumnIndex + 1; j++)
                {
                    if (dLinesColumns.Contains(j))
                    {
                        string content = getData(i, j);
                        if (content.Length > 0)
                        {
                            this.dataGridView2.Rows[i].Cells[j].Value = content;
                        }
                    }

                }
            }
            String d7 = DateTime.Now.ToString();
            Console.WriteLine(d7);

            loadDataCount += readyLoadCount;
        }

        private String getDataOri(int rowIndex, int columnIndex)
        {
            String value = "";

            /*foreach (dLine dline in dLinesOri)
            {
                if (dline.getValue().Trim().Length > 0)
                {
                    if (dline.getCloumnIndex() == columnIndex && dline.getRowIndex() == rowIndex)
                    {
                        value = dline.getValue();
                        break;
                    }
                }

            }*/

            for (int j = index0; j < dLinesOri.Count; j++)
            {
                dLine dline = (dLine)dLinesOri[j];
                if (dline.getValue().Trim().Length > 0)
                {
                    if (dline.getCloumnIndex() == columnIndex && dline.getRowIndex() == rowIndex)
                    {
                        value = dline.getValue();
                        index0 = j + 1;
                        break;
                    }
                }
            }
            return value;
        }

        private String getData(int rowIndex, int columnIndex)
        {
            string value = "";
            /*foreach (dLine dline in dLines)
            {
                if (dline.getValue().Trim().Length > 0 && dline.getCloumnIndex() <= canMaxColumnIndex)
                {
                    if (dline.getCloumnIndex() == columnIndex && dline.getRowIndex() == rowIndex)
                    {
                        value = dline.getValue();
                        break;
                    }
                }
            }*/


            for (int j = index; j < dLines.Count; j++)
            {
                dLine dline = (dLine)dLines[j];
                if (dline.getValue().Trim().Length > 0 && dline.getCloumnIndex() <= canMaxColumnIndex)
                {
                    if (dline.getCloumnIndex() == columnIndex && dline.getRowIndex() == rowIndex)
                    {
                        value = dline.getValue();
                        index = j + 1;
                        break;
                    }
                }
            }

            return value;
        }
        private void handleControl()
        {
            int wids = this.dataGridView1.Width;
            int heig = this.dataGridView1.Height;

            int wid = 40;
            this.button6.Location = new Point(wids - this.button6.Width - 30, this.button6.Location.Y);
            this.button5.Location = new Point(this.button6.Location.X - wid, this.button5.Location.Y);
            this.button4.Location = new Point(this.button5.Location.X - wid, this.button4.Location.Y);
            this.button3.Location = new Point(this.button4.Location.X - wid, this.button3.Location.Y);

            this.button2.Location = new Point(this.button3.Location.X - wid, this.button2.Location.Y);
            this.button1.Location = new Point(this.button2.Location.X - wid, this.button1.Location.Y);
            this.button7.Location = new Point(this.button1.Location.X - wid, this.button7.Location.Y);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.dataGridView1.ReadOnly = false;

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (this.button6.Text.Equals("打开"))
            {
                this.button6.Text = "隐藏";
                this.splitContainer1.SplitterDistance = 270;
            }
            else if (this.button6.Text.Equals("隐藏"))
            {
                this.button6.Text = "打开";
                this.splitContainer1.SplitterDistance = 15;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int j = 0; j < this.dataGridView2.Columns.Count; j++)
            {
                this.dataGridView2.Columns[j].Width += 5;
            }

            /*
            for (int i = 0; i < this.dataGridView2.Rows.Count; i++)
            {
                this.dataGridView2.Rows[i].Height += 5;
            }*/

            for (int i = 0; i < 1; i++)
            {
                int hei = this.dataGridView2.Rows[i].Height;
                if (hei > 10)
                {
                    if (i == 0)
                    {
                        this.dataGridView2.Rows[i].Height += 5;
                    }
                    else
                    {
                        this.dataGridView2.Rows[i].Height = this.dataGridView2.Rows[0].Height;
                    }
                }
            }

            float cellFont = this.dataGridView2.DefaultCellStyle.Font.Size;
            this.dataGridView2.DefaultCellStyle.Font = new Font("宋体", cellFont + 2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            less();
        }

        private void less()
        {
            for (int j = 0; j < this.dataGridView2.Columns.Count; j++)
            {
                int wid = this.dataGridView2.Columns[j].Width;
                if (wid > 10)
                {
                    if (j == 0)
                    {
                        this.dataGridView2.Columns[j].Width -= 5;
                    }
                    else
                    {
                        this.dataGridView2.Columns[j].Width = this.dataGridView2.Columns[0].Width;
                    }
                }
            }
            //this.dataGridView2.Rows.Count
            for (int i = 0; i < 1; i++)
            {
                int hei = this.dataGridView2.Rows[i].Height;
                if (hei > 10)
                {
                    if (i == 0)
                    {
                        this.dataGridView2.Rows[i].Height -= 5;
                    }
                    else
                    {
                        this.dataGridView2.Rows[i].Height = this.dataGridView2.Rows[0].Height;
                    }
                }
            }
            float cellFont = this.dataGridView2.DefaultCellStyle.Font.Size;

            if (cellFont > 9.0f)
            {
                this.dataGridView2.DefaultCellStyle.Font = new Font("宋体", cellFont - 2);
            }
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

        private void button2_Click(object sender, EventArgs e)
        {
            clearData(fileName);
            saveTxt();
            saveTxt2();
            MessageBox.Show("保存成功!");
        }
        private void saveTxt2()
        {
            FileStream fileStream = new FileStream(fileName + "x.txt", FileMode.OpenOrCreate);
            StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
            StringBuilder strBuilder = new StringBuilder();
            try
            {
                for (int i = 0; i < this.dataGridView2.Rows.Count; i++)
                {
                    strBuilder = new StringBuilder();
                    StringBuilder arg0 = new StringBuilder();
                    for (int j = 0; j < this.dataGridView2.Columns.Count; j++)
                    {
                        string cellContent = "";
                        if (null != this.dataGridView2.Rows[i].Cells[j].Value)
                        {
                            cellContent = this.dataGridView2.Rows[i].Cells[j].Value.ToString().Trim();
                        }
                        arg0.Append(cellContent);
                        strBuilder.Append(cellContent + ",");
                    }
                    strBuilder.Remove(strBuilder.Length - 1, 1);
                    string content = strBuilder.ToString();
                    if (arg0.ToString().Trim().Length > 0)
                    {
                        streamWriter.WriteLine(content);
                    }
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

        private dLine getLine(int columnIndex, int rowIndex)
        {
            /*dLine line = null;
            foreach (dLine dline in dLines)
            {
                if (dline.getCloumnIndex() == columnIndex && dline.getRowIndex() == rowIndex)
                {
                    line = dline;
                    return line;
                }
            }
            return line;*/

            return (dLine)dLines[rowIndex * columnSize + columnIndex];
        }

        private void dataGridView2_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            int columnIndex = e.ColumnIndex;
            int rowIndex = e.RowIndex;

        }

        private void dataGridView2_CellPainting_1(object sender, DataGridViewCellPaintingEventArgs e)
        {
            int columnIndex = e.ColumnIndex;
            int rowIndex = e.RowIndex;
            if (rowIndex < rowCount  && columnIndex <= columnSize && dLinesColumnsCheck.Contains(columnIndex))
            {
                dLine line = getLine(columnIndex, rowIndex);
                Rectangle newRect = new Rectangle(e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width, e.CellBounds.Height);

                using (Brush gridBrush = new SolidBrush(this.dataGridView2.GridColor), backColorBrush = new SolidBrush(e.CellStyle.BackColor), redBrush = new SolidBrush(Color.Red))
                {
                    using (Pen gridLinePen = new Pen(gridBrush))
                    {
                        Pen gridRedLinePen = new Pen(redBrush);
                        gridRedLinePen.Width = 1.0f;

                        e.Graphics.FillRectangle(backColorBrush, e.CellBounds);


                        // Draw the grid lines (only the right and bottom lines;
                        // DataGridView takes care of the others).
                        if (null != line && line.getBottom() == 1)
                        {
                            e.Graphics.DrawLine(gridRedLinePen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);//bootom
                        }


                        if (null != line && line.getRight() == 1)
                        {
                            e.Graphics.DrawLine(gridRedLinePen, e.CellBounds.Right - 1, e.CellBounds.Top, e.CellBounds.Right - 1, e.CellBounds.Bottom);
                        }

                        if (null != line && line.getLeft() == 1)
                        {
                            e.Graphics.DrawLine(gridRedLinePen, e.CellBounds.Left - 1, e.CellBounds.Top, e.CellBounds.Left - 1, e.CellBounds.Bottom - 1);
                        }

                        if (null != line && line.getTop() == 1)
                        {
                            e.Graphics.DrawLine(gridRedLinePen, e.CellBounds.Left - 1, e.CellBounds.Top, e.CellBounds.Right - 1, e.CellBounds.Top);
                        }

                        // Draw the inset highlight box.

                        //e.Graphics.DrawRectangle(Pens.Blue, newRect);

                        // Draw the text content of the cell, ignoring alignment.
                        if (e.Value != null)
                        {
                            e.Graphics.DrawString((String)e.Value, e.CellStyle.Font, Brushes.Black, e.CellBounds.X + 2, e.CellBounds.Y + 2, StringFormat.GenericDefault);
                        }
                        e.Handled = true;

                    }
                }
            }
            else
            {
                using (SolidBrush backColorBrush = new SolidBrush(this.dataGridView2.BackgroundColor))
                {
                    e.Graphics.FillRectangle(backColorBrush, e.CellBounds);
                    e.Handled = true;
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.ts = true;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.ts = false;
            if (this.ts)
            {
                int row = this.dataGridView1.CurrentCell.RowIndex;
                int column = this.dataGridView1.CurrentCell.ColumnIndex;

                string content = "";
                foreach (dLine dl in dLinesOri)
                {
                    if (dl.getCloumnIndex() == column && dl.getValue().ToString().Length > 0)
                    {
                        content += (","+dl.getValue());
                    }
                }

                DirectoryInfo dirFile = new DirectoryInfo(GlobalVariables.xuanxiangPath);
                FileInfo[] files = dirFile.GetFiles();
                string s = INIHelper.Read("AAA", "name", GlobalVariables.confPath);
                string name = (Convert.ToInt32(s) + 1).ToString();
                for (int i = name.Length; i < 6; i++)
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
                    streamWriter.WriteLine(content);
                    INIHelper.Write("AAA", "name", name, GlobalVariables.confPath);
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

                this.ts = false;

                if (MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK) == DialogResult.OK)
                {
                    
                }
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.ts)
            {
                int row = this.dataGridView1.CurrentCell.RowIndex;
                int column = this.dataGridView1.CurrentCell.ColumnIndex;

                string content = "";
                foreach (dLine dl in dLinesOri)
                {
                    if (dl.getCloumnIndex() == column && dl.getValue().ToString().Length > 0)
                    {
                        if (content.Length > 0)
                        {
                            content += ("," + dl.getValue());
                        }
                        else
                        {
                            content += (dl.getValue());
                        }
                    }
                }

                ts = false;

                QuShu gy = new QuShu(content, 6);
                gy.Show();


            }
        }




    }
}
