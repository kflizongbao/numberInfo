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
    public partial class FormModel1 : Form
    {
        private string fileName;

        private ArrayList dLines = new ArrayList();

        private int rowCount = 0;

        private ArrayList dLinesOri = new ArrayList();//dgv1显示的数据

        private int rowCountOri = 0;

        private ArrayList rowsDraw = new ArrayList();

        private ArrayList columnsDraw = new ArrayList();

        private ArrayList gridColumnsDraw = new ArrayList();

        private ArrayList groupColumnsDraw = new ArrayList();//显示有值的所有的列存储的是arraylist

        private Hashtable hTable = new Hashtable();

        private ArrayList gridDots = new ArrayList();//方形提示框

        private int loadDataCount = 0;//每次加载20个数据,已经加载的数据

        private int canMaxColumnIndex = 0;

        private ArrayList dLinesColumns = new ArrayList();//列索引
        private ArrayList dLinesColumnsCheck = new ArrayList();//需要处理列索引

        private ArrayList dLinesDataOri = new ArrayList();//原始的数据

        private ArrayList dLinesDataDraw = new ArrayList();//计算的基准对象

        private ArrayList dLinesValue = new ArrayList();//计算的基准对象(取出value为空)

        private ArrayList dLinesDrawRow = new ArrayList();//画红线的行

        private static System.Timers.Timer CheckUpdatetimer = new System.Timers.Timer();
        private Boolean isLoad = false;

        private static int preRow ;
        private static int[] preNumbers ;

        private ArrayList hasColumns = new ArrayList();
        private ArrayList hasRows = new ArrayList();
        private int prePosition = 0;
        private Hashtable values = new Hashtable();

        private Boolean ts = false;
        private int col1 = -1, row1 = -1, dvIndex = -1, col2 = -1, row2 = -1;
        private int maxColum, columnSize;
        private ArrayList linesDraw = new ArrayList();

        public FormModel1(string p1, string p2, string p3)
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();

            InitializeComponent();
            chkControl();

            fileName = GlobalVariables.firPath + @"\" + p1 + @"\" + p2 + @"\" + p3;
            preRow = -1;
            preNumbers = new int[2];

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

        private void handleControl()
        {
            int wids = this.dataGridView1.Width;
            int heig = this.dataGridView1.Height;

            int wid = 40;
            this.button6.Location = new Point(wids - this.button6.Width - 30, this.button6.Location.Y);
            this.button5.Location = new Point(this.button6.Location.X - wid, this.button5.Location.Y);
            this.button4.Location = new Point(this.button5.Location.X - wid, this.button4.Location.Y);
            this.button3.Location = new Point(this.button4.Location.X - wid, this.button3.Location.Y);

            this.button1.Location = new Point(this.button3.Location.X - wid, this.button3.Location.Y);
            this.button2.Location = new Point(this.button1.Location.X - wid, this.button1.Location.Y);
            this.button7.Location = new Point(this.button2.Location.X - wid, this.button7.Location.Y);
            this.button8.Location = new Point(this.button7.Location.X - wid, this.button8.Location.Y);
            this.button9.Location = new Point(this.button8.Location.X - wid, this.button9.Location.Y);
        }

        private void FormModel1_Load(object sender, EventArgs e)
        {
            handleControl();

            loadData();
            loadDataOri();

            getCadata();//将参与运算的格子取出来
            getCaColumns();//计算基准列

            setIsDraw0();

            getColumnsDraw();
            getRowsDraw();
            getGridColumns();
            handlerGroup();

            setDrawBackGround1();

            setIntervalShow();

            gdv1LoadData();

            gdv2LoadData(20);
            gdv2LoadData(20);
            gdv2LoadData(10);

            this.progressBar1.Value = 0;

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
            sr1.Close();

            dLines.Clear();
            System.IO.StreamReader sr = new System.IO.StreamReader(fileName + "x.txt");
            while (!sr.EndOfStream)
            {
                string[] items = sr.ReadLine().Split(',');
                if (columnSize <= items.Length)
                {
                    columnSize = items.Length;
                }
                for (int i = 0; i < items.Length; i++)
                {
                    string value = items[i].Equals("-1") ? "" : items[i];
                    string[] valueArr = value.Split(';');

                    dLine dline = new dLine();
                    dline.setValue(valueArr[0]);
                    dline.setCloumnIndex(i);
                    dline.setRowIndex(rowCount);
                    if (valueArr.Length > 1) {
                        dline.setColor(Color.FromName(valueArr[1]));
                    }
                    dLinesDataOri.Add(dline);//原始的数据


                    if (value.Trim().Length > 0)
                    {
                        if (!dLinesColumns.Contains(i))
                        {
                            dLinesColumns.Add(i);
                        }
                    }
                }
                for (int j = items.Length; j < s1; j++)
                {
                    dLine dline = new dLine();
                    dline.setValue("");
                    dline.setCloumnIndex(j);
                    dline.setRowIndex(rowCount);
                    dLinesDataOri.Add(dline);
                }
                rowCount += 1;

            }
            sr.Close();

            dLinesColumns.Sort();

            if (dLinesColumns.Count > 0)
            {
                canMaxColumnIndex = (int)dLinesColumns[dLinesColumns.Count - 1];
                canMaxColumnIndex += 1;
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
                    if (value.Trim().Length >= 0)
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
                //for (int j = 0; j < maxColum  + 1; j++)
                //{
                //    this.dataGridView1.Rows[i].Cells[j].Value = getDataOri(i, j);
               // }
            }

            foreach (dLine dline in dLinesOri)
            {
                int columnIndex = dline.getCloumnIndex();
                int rowIndex = dline.getRowIndex();
                this.dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = dline.getValue();
            }

        }

        /*
        private void gdv2LoadData()
        {
           
            for (int i = 0; i < rowCount; i++)
            {
                this.dataGridView2.Rows.Add();
                int row = this.dataGridView2.Rows.Count - 1;
                for (int j = 0; j < GlobalVariables.columnsSize; j++)
                {
                    string content = getData(i, j);
                    this.dataGridView2.Rows[i].Cells[j].Value = content;
                }
            }
        }*/

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

            if (loadDataCount > rowCount)
            {
                return;
            }

            for (int i = 0; i < readyLoadCount; i++)
            {
                this.dataGridView2.Rows.Add();
            }
           
            for (int i = loadDataCount; i < loadDataCount + readyLoadCount; i++)
            {
                for (int j = 0; j < canMaxColumnIndex + 1; j++)
                {
                    dLine item = getDataOpt(i, j);
                    string v = item.getValue();
                    if (null == v || v.Length <= 0)
                    {
                        continue;
                    }
                    this.dataGridView2.Rows[i].Cells[j].Value = v;
                    Color c = item.getColor();
                    if (null == c || c == Color.Empty)
                    {
                        continue;
                    }
                    this.dataGridView2.Rows[i].Cells[j].Style.ForeColor = c;
                }
            }
            loadDataCount += readyLoadCount;
        }

        private String getDataOri(int rowIndex, int columnIndex)
        {
            String value = "";
            foreach (dLine dline in dLinesOri)
            {
                if (dline.getCloumnIndex() == columnIndex && dline.getRowIndex() == rowIndex)
                {
                    value = dline.getValue();
                }
            }
            return value;
        }

        private String getData(int rowIndex, int columnIndex)
        {
            String value = "";
            foreach (dLine dline in dLines)
            {
                if (dline.getCloumnIndex() == columnIndex && dline.getRowIndex() == rowIndex)
                {
                    value = dline.getValue();
                    return value;
                }
            }
            return value;
        }

        private dLine getDataOpt(int rowIndex, int columnIndex)
        {
            dLine value = null;
            for (int i = prePosition; i < dLines.Count; i++)
            {
                dLine dline = (dLine)dLines[i];
                if (dline.getCloumnIndex() == columnIndex && dline.getRowIndex() == rowIndex)
                {
                    value = dline;
                    prePosition = i + 1;
                    return value;
                }
            }
            return value;
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
            int size = rowCount / 20 + (yu > 0 ? 1 : 0 );

            if (size > 0)
            {
                int step = 100 / size;
                step = (step == 0) ? 1 : step;
                for (int i = 0; i < size; i++)
                {
                    gdv2LoadData(20);

                    if (this.progressBar1.Value < 100 - step)
                    {
                        this.progressBar1.Value += step;
                    }
                }
                this.progressBar1.Visible = false;

                for (int i = 0; i < 5; i++)
                {
                    this.dataGridView2.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                } 
            }
            this.progressBar1.Visible = false;
            if (this.dataGridView2.Rows.Count > 10)
            {
                this.dataGridView2.FirstDisplayedScrollingRowIndex = this.dataGridView2.Rows[this.dataGridView2.RowCount - 1].Index;
            }
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

        private void saveTxt2()
        {
            Console.WriteLine(fileName);
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
                        DataGridViewCell obj = this.dataGridView2.Rows[i].Cells[j];
                        if (null != obj.Value)
                        {
                            cellContent = obj.Value.ToString().Trim();
                        }
                        if (Color.Empty != obj.Style.ForeColor) 
                        {
                            cellContent += ";";
                            cellContent += obj.Style.ForeColor.Name;
                        }
                        Console.WriteLine(obj.Style.ForeColor.ToArgb());
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
        private void clearData(String filePath)
        {
            string path = filePath + ".txt";
            FileStream fs = File.OpenWrite(path);
            fs.SetLength(0);
            fs.Close();

            string path1 = filePath + "x.txt";
            FileStream fs1 = File.OpenWrite(path1);
            fs1.SetLength(0);
            fs1.Close();
        }

        private void clearGridData()
        {
            this.dataGridView2.Rows.Clear();
            this.dataGridView1.Rows.Clear();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (ts)
            {
                int columnIndex = e.ColumnIndex;
                int rowIndex = e.RowIndex;
                string val = getDataOri(rowIndex, columnIndex);
                if (null != val && val.Length > 0)
                {
                    if (dvIndex != 1)
                    {
                        initGy();
                    }
                    dvIndex = 1;

                    if (col1 >= 0)
                    {
                        col2 = columnIndex;
                        row2 = rowIndex;
                        showGongYue();
                        ts = false;
                    }
                    else
                    {
                        col1 = columnIndex;
                        row1 = rowIndex;
                    }
                }
            }
        }

        private void dataGridView2_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

            int columnIndex = e.ColumnIndex;
            int rowIndex = e.RowIndex;

            if (columnIndex <= columnSize && dLinesColumns.Contains(columnIndex) && rowIndex < rowCount)
            {
                if (columnIndex >= 0 && rowIndex >= 0 && rowIndex < rowCount && columnIndex <= canMaxColumnIndex)
                {
                    dLine line = getLine(columnIndex, rowIndex);
                    if (null != line && line.getBottom() == 1)
                    {

                        Rectangle newRect = new Rectangle(e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width, e.CellBounds.Height);

                        using (Brush gridBrush = new SolidBrush(this.dataGridView2.GridColor), backColorBrush = new SolidBrush(e.CellStyle.BackColor), redBrush = new SolidBrush(Color.Red))
                        {
                            using (Pen gridLinePen = new Pen(gridBrush))
                            {
                                Pen gridRedLinePen = new Pen(redBrush);
                                gridRedLinePen.Width = 1.0f;
                                // Erase the cell.
                                if (line.getBackGround() == 1)
                                {
                                    e.Graphics.FillRectangle(new SolidBrush(Color.Green), e.CellBounds);//绿色
                                }
                                else if (line.getBackGround() == 2)
                                {
                                    e.Graphics.FillRectangle(new SolidBrush(Color.BlueViolet), e.CellBounds);//蓝色
                                }
                                else if (line.getBackGround() == 3)
                                {
                                    e.Graphics.FillRectangle(new SolidBrush(Color.Red), e.CellBounds);//红色
                                }
                                else if (line.getBackGround() == 4)
                                {
                                    e.Graphics.FillRectangle(new SolidBrush(Color.Black), e.CellBounds);//粉红色
                                }
                                else
                                {
                                    e.Graphics.FillRectangle(backColorBrush, e.CellBounds);//默认
                                }

                                // Draw the grid lines (only the right and bottom lines;
                                // DataGridView takes care of the others).

                                e.Graphics.DrawLine(gridRedLinePen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);//bootom

                                //e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 1, e.CellBounds.Top, e.CellBounds.Right - 1, e.CellBounds.Bottom);//right

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
                        //开始重绘背景

                        Rectangle newRect = new Rectangle(e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width, e.CellBounds.Height);

                        using (Brush gridBrush = new SolidBrush(this.dataGridView2.GridColor), backColorBrush = new SolidBrush(e.CellStyle.BackColor), redBrush = new SolidBrush(Color.Red))
                        {
                            using (Pen gridLinePen = new Pen(gridBrush))
                            {
                                Pen gridRedLinePen = new Pen(redBrush);
                                gridRedLinePen.Width = 1.0f;
                                // Erase the cell.
                                if (line.getBackGround() == 1)
                                {
                                    e.Graphics.FillRectangle(new SolidBrush(Color.Green), e.CellBounds);//绿色
                                }
                                else if (line.getBackGround() == 2)
                                {
                                    e.Graphics.FillRectangle(new SolidBrush(Color.BlueViolet), e.CellBounds);//蓝色
                                }
                                else if (line.getBackGround() == 3)
                                {
                                    e.Graphics.FillRectangle(new SolidBrush(Color.Red), e.CellBounds);//红色
                                }
                                else if (line.getBackGround() == 4)
                                {
                                    e.Graphics.FillRectangle(new SolidBrush(Color.Black), e.CellBounds);//粉红色
                                }
                                else
                                {
                                    e.Graphics.FillRectangle(backColorBrush, e.CellBounds);//默认
                                }

                                if (e.Value != null)
                                {
                                    e.Graphics.DrawString((String)e.Value, e.CellStyle.Font, line.getColor() == Color.Empty ? Brushes.Black : new SolidBrush(line.getColor()), e.CellBounds.X + 2, e.CellBounds.Y + 2, StringFormat.GenericDefault);
                                }
                                e.Handled = true;

                            }
                        }

                        //结束重绘背景
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


        private void setIsDraw0()
        {
            int parm = canMaxColumnIndex + 1;
            //foreach (dLine dline in dLinesDataDraw)
            //{
            foreach (dLine dlinei in dLines)
            {
                string value = dlinei.getValue();
                int columnIndex = dlinei.getCloumnIndex();
                int rowIndex = dlinei.getRowIndex();
                if (null != value)
                {
                    //第一种模式计算
                    int columnIndex1 = columnIndex + 1;
                    int rowIndex1 = rowIndex + 1;

                    int columnIndex2 = columnIndex + 2;
                    int rowIndex2 = rowIndex;

                    int columnIndex3 = columnIndex + 3;
                    int rowIndex3 = rowIndex + 1;

                    //第一种模式计算


                    //第二种模式计算
                    int columnIndex11 = columnIndex + 1;
                    int rowIndex11 = rowIndex + 1;

                    int columnIndex21 = columnIndex + 3;
                    int rowIndex21 = rowIndex;


                    int columnIndex31 = columnIndex + 2;
                    int rowIndex31 = rowIndex + 1;

                    //第二种模式计算

                    //第三种模式
                    int columnIndex12 = columnIndex + 1;
                    int rowIndex12 = rowIndex;

                    int columnIndex22 = columnIndex + 3;
                    int rowIndex22 = rowIndex;


                    int columnIndex32 = columnIndex;
                    int rowIndex32 = rowIndex + 1;

                    int columnIndex42 = columnIndex + 2;
                    int rowIndex42 = rowIndex + 1;


                    //第四种模式
                    int columnIndex13 = columnIndex + 1;
                    int rowIndex13 = rowIndex;

                    int columnIndex23 = columnIndex + 2;
                    int rowIndex23 = rowIndex;


                    int columnIndex33 = columnIndex;
                    int rowIndex33 = rowIndex + 1;

                    int columnIndex43 = columnIndex + 3;
                    int rowIndex43 = rowIndex + 1;


                    if (value.Length > 0)
                    {

                        //第一种

                        if (rowCount > rowIndex1 && rowCount > rowIndex2 && rowCount > rowIndex3 && columnIndex1 < parm && columnIndex2 < parm && columnIndex3 < parm)
                        {

                            int p = rowIndex * parm + columnIndex;
                            dLine dl = (dLine)dLines[p];


                            int p1 = rowIndex1 * parm + columnIndex1;
                            dLine dl1 = (dLine)dLines[p1];
                            string value1 = dl1.getValue();


                            int p2 = rowIndex2 * parm + columnIndex2;
                            dLine dl2 = (dLine)dLines[p2];
                            string value2 = dl2.getValue();


                            int p3 = rowIndex3 * parm + columnIndex3;
                            dLine dl3 = (dLine)dLines[p3];
                            string value3 = dl3.getValue();


                            if (value1.Length > 0 && value2.Length > 0 && value3.Length > 0)
                            {
                                int pa = rowIndex * parm + columnIndex;
                                dLine dla = (dLine)dLines[pa];
                                dla.setBottom(1);

                                int pa1 = (rowIndex1 - 1) * parm + columnIndex1;
                                dLine dla1 = (dLine)dLines[pa1];
                                dla1.setBottom(1);

                                int pa2 = rowIndex2 * parm + columnIndex2;
                                dLine dla2 = (dLine)dLines[pa2];
                                dla2.setBottom(1);

                                int pa3 = (rowIndex3 - 1) * parm + columnIndex3;
                                dLine dla3 = (dLine)dLines[pa3];
                                dla3.setBottom(1);
                            }
                            //break; 
                        }


                        //第二种

                        if (rowCount > rowIndex11 && rowCount > rowIndex21 && rowCount > rowIndex31 && columnIndex11 < parm && columnIndex21 < parm && columnIndex31 < parm)
                        {

                            int p = rowIndex * parm + columnIndex;
                            dLine dl = (dLine)dLines[p];


                            int p1 = rowIndex11 * parm + columnIndex11;
                            dLine dl1 = (dLine)dLines[p1];
                            string value1 = dl1.getValue();


                            int p2 = rowIndex21 * parm + columnIndex21;
                            dLine dl2 = (dLine)dLines[p2];
                            string value2 = dl2.getValue();


                            int p3 = rowIndex31 * parm + columnIndex31;
                            dLine dl3 = (dLine)dLines[p3];
                            string value3 = dl3.getValue();

                            if (value1.Length > 0 && value2.Length > 0 && value3.Length > 0)
                            {
                                int pa = rowIndex * parm + columnIndex;
                                dLine dla = (dLine)dLines[pa];
                                dla.setBottom(1);

                                int pa1 = (rowIndex11 - 1) * parm + columnIndex11;
                                dLine dla1 = (dLine)dLines[pa1];
                                dla1.setBottom(1);

                                int pa2 = rowIndex21 * parm + columnIndex21;
                                dLine dla2 = (dLine)dLines[pa2];
                                dla2.setBottom(1);

                                int pa3 = (rowIndex31 - 1) * parm + columnIndex31;
                                dLine dla3 = (dLine)dLines[pa3];
                                dla3.setBottom(1);
                            }
                        }
                    }
                    else
                    {
                        //第三种

                        if (rowCount > rowIndex12 && rowCount > rowIndex22 && rowCount > rowIndex32 && rowCount > rowIndex42 && columnIndex12 < parm && columnIndex22 < parm && columnIndex32 < parm && columnIndex42 < parm)
                        {

                            int p = rowIndex * parm + columnIndex;
                            dLine dl = (dLine)dLines[p];


                            int p1 = rowIndex12 * parm + columnIndex12;
                            dLine dl1 = (dLine)dLines[p1];
                            string value1 = dl1.getValue();


                            int p2 = rowIndex22 * parm + columnIndex22;
                            dLine dl2 = (dLine)dLines[p2];
                            string value2 = dl2.getValue();


                            int p3 = rowIndex32 * parm + columnIndex32;
                            dLine dl3 = (dLine)dLines[p3];
                            string value3 = dl3.getValue();

                            int p4 = rowIndex42 * parm + columnIndex42;
                            dLine dl4 = (dLine)dLines[p4];
                            string value4 = dl4.getValue();

                            if (value1.Length > 0 && value2.Length > 0 && value3.Length > 0 && value4.Length > 0)
                            {
                                int pa = rowIndex * parm + columnIndex;
                                dLine dla = (dLine)dLines[pa];
                                dla.setBottom(1);

                                int pa1 = rowIndex12 * parm + columnIndex12;
                                dLine dla1 = (dLine)dLines[pa1];
                                dla1.setBottom(1);

                                int pa2 = rowIndex22 * parm + columnIndex22;
                                dLine dla2 = (dLine)dLines[pa2];
                                dla2.setBottom(1);

                                int pa3 = (rowIndex32 - 1) * parm + columnIndex32;
                                dLine dla3 = (dLine)dLines[pa3];
                                dla3.setBottom(1);

                                int pa4 = (rowIndex42 - 1) * parm + columnIndex42;
                                dLine dla4 = (dLine)dLines[pa4];

                                dla4.setBottom(1);

                            }
                        }

                        //第四种

                        if (rowCount > rowIndex13 && rowCount > rowIndex23 && rowCount > rowIndex33 && rowCount > rowIndex43 && columnIndex13 < parm && columnIndex23 < parm && columnIndex33 < parm && columnIndex43 < parm && 0 < 1)
                        {

                            int p = rowIndex * parm + columnIndex;
                            dLine dl = (dLine)dLines[p];


                            int p1 = rowIndex13 * parm + columnIndex13;
                            dLine dl1 = (dLine)dLines[p1];
                            string value1 = dl1.getValue();


                            int p2 = rowIndex23 * parm + columnIndex23;
                            dLine dl2 = (dLine)dLines[p2];
                            string value2 = dl2.getValue();


                            int p3 = rowIndex33 * parm + columnIndex33;
                            dLine dl3 = (dLine)dLines[p3];
                            string value3 = dl3.getValue();

                            int p4 = rowIndex43 * parm + columnIndex43;
                            dLine dl4 = (dLine)dLines[p4];
                            string value4 = dl4.getValue();

                            if (value1.Length > 0 && value2.Length > 0 && value3.Length > 0 && value4.Length > 0)
                            {
                                int pa = rowIndex * parm + columnIndex;
                                dLine dla = (dLine)dLines[pa];
                                dla.setBottom(1);

                                int pa1 = rowIndex13 * parm + columnIndex13;
                                dLine dla1 = (dLine)dLines[pa1];
                                dla1.setBottom(1);

                                int pa2 = rowIndex23 * parm + columnIndex23;
                                dLine dla2 = (dLine)dLines[pa2];
                                dla2.setBottom(1);

                                int pa3 = (rowIndex33 - 1) * parm + columnIndex33;
                                dLine dla3 = (dLine)dLines[pa3];

                                int pa4 = (rowIndex43 - 1) * parm + columnIndex43;
                                dLine dla4 = (dLine)dLines[pa4];

                                dla4.setBottom(1);
                            }
                        }

                    }

                }


            }

            // }

        }


        private void getCaColumns()
        {
            foreach (int dline in dLinesColumns)
            {
                if (!dLinesColumns.Contains(dline - 1))
                {
                    dLinesColumnsCheck.Add(dline);
                    foreach (dLine dlin in dLines)
                    {
                        string value = dlin.getValue();
                        if (dlin.getCloumnIndex() == dline && null != value)
                        {
                            dLinesDataDraw.Add(dlin);
                        }
                    }
                }
            }

        }

        private void getCadata()
        {
            for (int i = 0; i < dLinesDataOri.Count; i++)
            {
                dLine dline = (dLine)dLinesDataOri[i];
                if (dline.getCloumnIndex() <= canMaxColumnIndex)
                {
                    dline.setOriIndex(i);
                    dline.setLinesIndex(dLines.Count);
                    dLines.Add(dline);

                    if (null != dline.getValue() && dline.getValue().Length > 0)
                    {
                        dLinesValue.Add(dline);
                        string key = dline.getCloumnIndex().ToString() + "," + dline.getRowIndex().ToString();
                        string value = dline.getValue();
                        values.Add(key, value);
                    }
                }
            }
        }
        private void setIsDraw()
        {
            foreach (dLine dline in dLinesDataDraw)
            {
                string value = dline.getValue();
                int columnIndex = dline.getCloumnIndex();
                int rowIndex = dline.getRowIndex();
                if (null != value && value.Length > 0)
                {

                    int columnIndex1 = columnIndex + 1;
                    int rowIndex1 = rowIndex + 1;

                    int columnIndex2 = columnIndex + 2;
                    int rowIndex2 = rowIndex;

                    int columnIndex3 = columnIndex + 3;
                    int rowIndex3 = rowIndex + 1;

                    Boolean first = false, second = false, third = false;

                    foreach (dLine dlinei in dLines)
                    {

                        //string valuei = dlinei.getValue();
                        //int columnIndexi = dlinei.getCloumnIndex();

                        if (null != dlinei.getValue() && dlinei.getValue().Length > 0)
                        {
                            if (dlinei.getRowIndex() == rowIndex1 && dlinei.getCloumnIndex() == columnIndex1)
                            {
                                first = true;
                            }
                            if (dlinei.getRowIndex() == rowIndex2 && dlinei.getCloumnIndex() == columnIndex2)
                            {
                                second = true;
                            }
                            if (dlinei.getRowIndex() == rowIndex3 && dlinei.getCloumnIndex() == columnIndex3)
                            {
                                third = true;
                            }
                        }
                        if (first && second && third)
                        {
                            break;
                        }

                    }


                    if (first && second && third)
                    {
                        dline.setBottom(1);
                        foreach (dLine dlinej in dLines)
                        {

                            if (dlinej.getRowIndex() == rowIndex1 - 1 && dlinej.getCloumnIndex() == columnIndex1)
                            {
                                dlinej.setBottom(1);
                            }
                            if (dlinej.getRowIndex() == rowIndex2 && dlinej.getCloumnIndex() == columnIndex2)
                            {
                                dlinej.setBottom(1);
                            }
                            if (dlinej.getRowIndex() == rowIndex3 - 1 && dlinej.getCloumnIndex() == columnIndex3)
                            {
                                dlinej.setBottom(1);
                            }

                        }
                    }



                }

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

            int arg0 = rowIndex * columnSize + columnIndex;
            return (dLine)dLines[((dLine)dLinesDataOri[arg0]).getLinesIndex()];
        }

        private void setIsDraw2()
        {

            foreach (dLine dline in dLinesDataDraw)
            {
                string value = "1";
                if (null != value && value.Length > 0)
                {
                    int columnIndex = dline.getCloumnIndex();
                    int rowIndex = dline.getRowIndex();

                    int columnIndex1 = columnIndex + 1;
                    int rowIndex1 = rowIndex;

                    int columnIndex2 = columnIndex + 3;
                    int rowIndex2 = rowIndex;


                    int columnIndex3 = columnIndex;
                    int rowIndex3 = rowIndex + 1;

                    int columnIndex4 = columnIndex + 2;
                    int rowIndex4 = rowIndex + 1;


                    Boolean first = false, second = false, third = false, four = false;
                    foreach (dLine dlinei in dLines)
                    {
                        String valuei = dlinei.getValue();
                        if (null != valuei && valuei.Length > 0)
                        {
                            if (dlinei.getRowIndex() == rowIndex1 && dlinei.getCloumnIndex() == columnIndex1)
                            {
                                first = true;
                            }
                            if (dlinei.getRowIndex() == rowIndex2 && dlinei.getCloumnIndex() == columnIndex2)
                            {
                                second = true;
                            }
                            if (dlinei.getRowIndex() == rowIndex3 && dlinei.getCloumnIndex() == columnIndex3)
                            {
                                third = true;
                            }
                            if (dlinei.getRowIndex() == rowIndex4 && dlinei.getCloumnIndex() == columnIndex4)
                            {
                                four = true;
                            }
                        }

                    }
                    if (first && second && third && four)
                    {
                        dline.setBottom(1);
                        foreach (dLine dlinej in dLines)
                        {
                            if (dlinej.getRowIndex() == rowIndex1 && dlinej.getCloumnIndex() == columnIndex1)
                            {
                                dlinej.setBottom(1);
                            }
                            if (dlinej.getRowIndex() == rowIndex2 && dlinej.getCloumnIndex() == columnIndex2)
                            {
                                dlinej.setBottom(1);
                            }
                            if (dlinej.getRowIndex() == rowIndex3 - 1 && dlinej.getCloumnIndex() == columnIndex3)
                            {
                                dlinej.setBottom(1);
                            }
                            if (dlinej.getRowIndex() == rowIndex4 - 1 && dlinej.getCloumnIndex() == columnIndex4)
                            {
                                dlinej.setBottom(1);
                            }
                        }
                    }
                }

            }

        }


        private void setIsDraw3()
        {

            foreach (dLine dline in dLinesDataDraw)
            {
                string value = "1";
                if (null != value && value.Length > 0)
                {
                    int columnIndex = dline.getCloumnIndex();
                    int rowIndex = dline.getRowIndex();

                    int columnIndex1 = columnIndex + 1;
                    int rowIndex1 = rowIndex;

                    int columnIndex2 = columnIndex + 2;
                    int rowIndex2 = rowIndex;


                    int columnIndex3 = columnIndex;
                    int rowIndex3 = rowIndex + 1;

                    int columnIndex4 = columnIndex + 3;
                    int rowIndex4 = rowIndex + 1;


                    Boolean first = false, second = false, third = false, four = false;
                    foreach (dLine dlinei in dLines)
                    {
                        String valuei = dlinei.getValue();
                        if (null != valuei && valuei.Length > 0)
                        {
                            if (dlinei.getRowIndex() == rowIndex1 && dlinei.getCloumnIndex() == columnIndex1)
                            {
                                first = true;
                            }
                            if (dlinei.getRowIndex() == rowIndex2 && dlinei.getCloumnIndex() == columnIndex2)
                            {
                                second = true;
                            }
                            if (dlinei.getRowIndex() == rowIndex3 && dlinei.getCloumnIndex() == columnIndex3)
                            {
                                third = true;
                            }
                            if (dlinei.getRowIndex() == rowIndex4 && dlinei.getCloumnIndex() == columnIndex4)
                            {
                                four = true;
                            }
                        }

                    }
                    if (first && second && third && four)
                    {
                        dline.setBottom(1);
                        foreach (dLine dlinej in dLines)
                        {
                            if (dlinej.getRowIndex() == rowIndex1 && dlinej.getCloumnIndex() == columnIndex1)
                            {
                                dlinej.setBottom(1);
                            }
                            if (dlinej.getRowIndex() == rowIndex2 && dlinej.getCloumnIndex() == columnIndex2)
                            {
                                dlinej.setBottom(1);
                            }
                            if (dlinej.getRowIndex() == rowIndex3 - 1 && dlinej.getCloumnIndex() == columnIndex3)
                            {
                                dlinej.setBottom(1);
                            }
                            if (dlinej.getRowIndex() == rowIndex4 - 1 && dlinej.getCloumnIndex() == columnIndex4)
                            {
                                dlinej.setBottom(1);
                            }
                        }
                    }
                }

            }

        }


        private void setIsDraw1()
        {

            foreach (dLine dline in dLinesDataDraw)
            {
                String value = dline.getValue();
                if (null != value && value.Length > 0)
                {
                    int columnIndex = dline.getCloumnIndex();
                    int rowIndex = dline.getRowIndex();

                    int columnIndex1 = columnIndex + 1;
                    int rowIndex1 = rowIndex + 1;

                    int columnIndex2 = columnIndex + 3;
                    int rowIndex2 = rowIndex;


                    int columnIndex3 = columnIndex + 2;
                    int rowIndex3 = rowIndex + 1;

                    Boolean first = false, second = false, third = false;
                    foreach (dLine dlinei in dLines)
                    {
                        String valuei = dlinei.getValue();
                        if (null != valuei && valuei.Length > 0)
                        {
                            if (dlinei.getRowIndex() == rowIndex1 && dlinei.getCloumnIndex() == columnIndex1)
                            {
                                first = true;
                            }
                            if (dlinei.getRowIndex() == rowIndex2 && dlinei.getCloumnIndex() == columnIndex2)
                            {
                                second = true;
                            }
                            if (dlinei.getRowIndex() == rowIndex3 && dlinei.getCloumnIndex() == columnIndex3)
                            {
                                third = true;
                            }
                        }

                    }
                    if (first && second && third)
                    {
                        dline.setBottom(1);
                        foreach (dLine dlinej in dLines)
                        {
                            if (dlinej.getRowIndex() == rowIndex1 - 1 && dlinej.getCloumnIndex() == columnIndex1)
                            {
                                dlinej.setBottom(1);
                            }
                            if (dlinej.getRowIndex() == rowIndex2 && dlinej.getCloumnIndex() == columnIndex2)
                            {
                                dlinej.setBottom(1);
                            }
                            if (dlinej.getRowIndex() == rowIndex3 - 1 && dlinej.getCloumnIndex() == columnIndex3)
                            {
                                dlinej.setBottom(1);
                            }
                        }
                    }
                }

            }

        }


        private void setDrawBackGround()
        {
            int size = rowsDraw.Count;
            int maxDrawRowNumber = 0;
            int minDrawRowNumber = 0;
            if (size > 0)
            {
                minDrawRowNumber = (int)rowsDraw[0];
                maxDrawRowNumber = (int)rowsDraw[size - 1];
                foreach (dLine dline in dLines)
                {
                    int rIndex = dline.getRowIndex();//格子的行号
                    int cIndex = dline.getCloumnIndex();
                    if (rIndex - 1 >= minDrawRowNumber && rIndex <= maxDrawRowNumber)
                    {
                        int ss = getSNumber(rIndex);
                        int bb = getBNumber(rIndex);
                        int diff = bb - ss;
                        if (diff >= 6 && diff <= 9 && columnsDraw.Contains(cIndex))
                        {
                            dline.setBackGround(1);//绿色
                        }
                        if (diff >= 10 && columnsDraw.Contains(cIndex))
                        {
                            dline.setBackGround(2);//蓝色
                        }

                    }
                    else
                    {

                    }
                }
            }

        }

        private int getSNumber(int rIndex)
        {
            int s = 0;
            //获取和这个格子行号相邻的2个画线号码，并计算它们之间的距离
            ArrayList minRowsDraw = new ArrayList();//小于格子的画线的行号
            for (int i = 0; i < rowsDraw.Count; i++)
            {
                int idex = (int)rowsDraw[i];
                if (idex < rIndex)
                {
                    minRowsDraw.Add(idex);
                }
            }
            minRowsDraw.Sort();

            //获取行号
            if (minRowsDraw.Count > 0)
            {
                s = (int)minRowsDraw[minRowsDraw.Count - 1];
            }

            return s;
        }

        private int getBNumber(int rIndex)
        {
            int b = 0;
            //获取和这个格子行号相邻的2个画线号码，并计算它们之间的距离

            ArrayList maxRowsDraw = new ArrayList();//小于格子的画线的行号
            for (int i = 0; i < rowsDraw.Count; i++)
            {
                int idex = (int)rowsDraw[i];
                if (idex >= rIndex)
                {
                    maxRowsDraw.Add(idex);
                }
            }
            maxRowsDraw.Sort();

            //获取行号
            if (maxRowsDraw.Count > 0)
            {
                b = (int)maxRowsDraw[0];
            }

            return b;
        }

        //把画线的格子的行号拿出来
        private void getColumnsDraw()
        {
            rowsDraw.Clear();
            foreach (dLine dline in dLines)
            {
                int rowIndex = dline.getRowIndex();
                if (dline.getBottom() == 1)
                {
                    if (!rowsDraw.Contains(rowIndex))
                    {
                        rowsDraw.Add(rowIndex);
                    }
                }
            }
            rowsDraw.Sort();
        }

        //把画线格子的列号取出来
        private void getRowsDraw()
        {
            columnsDraw.Clear();
            foreach (dLine dline in dLines)
            {
                int columnIndex = dline.getCloumnIndex();
                if (dline.getBottom() == 1)
                {
                    if (!columnsDraw.Contains(columnIndex))
                    {
                        columnsDraw.Add(columnIndex);
                    }
                }
            }
        }

        private void getGridColumns()
        {
            gridColumnsDraw.Clear();
            foreach (dLine dline in dLines)
            {
                int columnIndex = dline.getCloumnIndex();
                if (dline.getBottom() == 1)
                {
                    if (!gridColumnsDraw.Contains(columnIndex))
                    {
                        gridColumnsDraw.Add(columnIndex);
                    }
                }
            }
            gridColumnsDraw.Sort();
        }
        private void handlerGroup()
        {
            //判断有几组
            ArrayList g = new ArrayList();
            int dex = 0;
            for (int i = 0; i < gridColumnsDraw.Count; i++)
            {
                int col = (int)gridColumnsDraw[i];

                if (i > 0)
                {
                    if (col - dex > 1)
                    {
                        int coun = hTable.Count + 1;

                        ArrayList ds = (ArrayList)g.Clone();
                        hTable.Add(coun.ToString(), ds);
                        g.Clear();
                    }

                    if (i == gridColumnsDraw.Count - 1)
                    {
                        int coun = hTable.Count + 1;
                        g.Add(col);
                        ArrayList ds = (ArrayList)g.Clone();
                        hTable.Add(coun.ToString(), ds);
                    }
                    g.Add(col);
                    dex = col;
                }
                else
                {
                    g.Add(col);
                    dex = col;
                }
            }

        }

        private void setIntervalShow()
        {

            foreach (dLine dline in gridDots)
            {
                /*
                foreach (dLine dlin in gridDots)
                {
                    int c1 = dline.getCloumnIndex();
                    int c2 = dlin.getCloumnIndex();
                    int i1 = dlin.getInterval();
                    int i2 = dline.getInterval();
                    if (c2 == c1 && i1 == i2 && dline != dlin)
                    {*/
                int interval = dline.getInterval();
                if (interval == 3)
                {
                    dline.setBackGround(2);//蓝色
                    //break;
                }
                if (interval == 4)
                {
                    dline.setBackGround(1);//绿色
                    //break;
                }
                if (interval == 2)
                {
                    dline.setBackGround(3);//红色
                    //break;
                }
                if (interval == 5)
                {
                    dline.setBackGround(4);//分红色
                    //break;
                }

                //}
                //}
            }
        }

        private void setDrawBackGround1()
        {
            int size = hTable.Count;

            if (size > 0)
            {
                foreach (DictionaryEntry de in hTable)
                {
                    //分组进行处理
                    string key = de.Key.ToString();
                    ArrayList al = (ArrayList)de.Value;
                    al.Sort();

                    int maxDrawRowNumber = 0;
                    int minDrawRowNumber = 0;

                    int maxDrawColumnNumber = 0;
                    int minDrawColumnNumber = 0;

                    minDrawColumnNumber = (int)al[0];
                    maxDrawColumnNumber = (int)al[al.Count - 1];

                    getDlinesDraw(minDrawColumnNumber, maxDrawColumnNumber);

                    if (al.Count > 0)
                    {
                        preRow = -1;

                        ArrayList mdr = getDrawRowNumbers(al);

                        minDrawRowNumber = (int)mdr[0];
                        maxDrawRowNumber = (int)mdr[mdr.Count - 1];

                        foreach (dLine dline in dLines)
                        {
                            int rIndex = dline.getRowIndex();
                            int cIndex = dline.getCloumnIndex();
                            if (rIndex - 1 >= minDrawRowNumber && rIndex <= maxDrawRowNumber && cIndex >= minDrawColumnNumber && cIndex <= (maxDrawColumnNumber + 1))
                            {
                                int[] number;
                                if (rIndex != preRow)
                                {
                                    number = getNumber(rIndex, key, al);
                                    preNumbers = number;
                                }
                                else
                                {
                                    number = preNumbers;
                                }
                                preRow = rIndex;

                                int ss = number[1];
                                int bb = number[0];
                                int diff = bb - ss;
                                if (diff >= 2 && diff <= 5 && cIndex == (maxDrawColumnNumber + 1) && rIndex == bb)
                                {
                                    if (!dLinesColumns.Contains(cIndex))
                                    {
                                        dLinesColumns.Add(cIndex);
                                    }
                                    dline.setInterval(diff);
                                    gridDots.Add(dline);
                                }
                                if (diff >= 6 && diff <= 9 && columnsDraw.Contains(cIndex))
                                {
                                    dline.setBackGround(1);//绿色
                                }

                                if (diff >= 6 && diff <= 9 && cIndex == (maxDrawColumnNumber + 1) && rIndex == bb)
                                {
                                    if (!dLinesColumns.Contains(maxDrawColumnNumber + 1))
                                    {
                                        dLinesColumns.Add(maxDrawColumnNumber + 1);
                                    }
                                }

                                if (diff >= 10 && columnsDraw.Contains(cIndex))
                                {
                                    dline.setBackGround(2);//蓝色
                                }

                                if (diff >= 10 && cIndex == (maxDrawColumnNumber + 1) && rIndex == bb)
                                {

                                    if (!dLinesColumns.Contains(maxDrawColumnNumber + 1))
                                    {
                                        dLinesColumns.Add(maxDrawColumnNumber + 1);
                                    }
                                }


                            }
                        }




                    }
                }
            }
        }

        private ArrayList getDrawRowNumbers(ArrayList mdrc)
        {
            ArrayList mdc = new ArrayList();
            foreach (dLine dline in dLines)
            {
                int idx = dline.getCloumnIndex();
                int irx = dline.getRowIndex();
                if (mdrc.Contains(idx))
                {
                    if (null != dline.getValue() && dline.getBottom() == 1 && !mdc.Contains(irx))
                    {
                        mdc.Add(irx);
                    }
                }
            }
            mdc.Sort();
            return mdc;
        }


        private int getSNumber1(int rIndex, string key, ArrayList al)
        {
            int s = 0;
            ArrayList groupRowsDraw = new ArrayList();//存储本组画线的行号

            ArrayList minRowsDraw = new ArrayList();//存储刷选出来的小鱼画线的行号

            //获取这一组画线的行号
            foreach (dLine dline in dLines)
            {
                int colIndex = dline.getCloumnIndex();
                int roIndex = dline.getRowIndex();
                if (al.Contains(colIndex) && !groupRowsDraw.Contains(roIndex) && dline.getBottom() == 1)
                {
                    groupRowsDraw.Add(roIndex);
                }
            }

            for (int i = 0; i < groupRowsDraw.Count; i++)
            {
                int idex = (int)groupRowsDraw[i];
                if (idex < rIndex)
                {
                    minRowsDraw.Add(idex);
                }
            }
            minRowsDraw.Sort();

            //获取行号
            if (minRowsDraw.Count > 0)
            {
                s = (int)minRowsDraw[minRowsDraw.Count - 1];
            }

            return s;
        }
        private int getBNumber1(int rIndex, string key, ArrayList al)
        {
            int b = 0;

            ArrayList groupRowsDraw = new ArrayList();//存储本组画线的行号

            ArrayList maxRowsDraw = new ArrayList();//存储刷选出来的大于画线的行号

            //获取这一组画线的行号
            foreach (dLine dline in dLines)
            {
                int colIndex = dline.getCloumnIndex();
                int roIndex = dline.getRowIndex();
                if (al.Contains(colIndex) && !groupRowsDraw.Contains(roIndex) && dline.getBottom() == 1)
                {
                    groupRowsDraw.Add(roIndex);
                }
            }

            for (int i = 0; i < groupRowsDraw.Count; i++)
            {
                int idex = (int)groupRowsDraw[i];
                if (idex >= rIndex)
                {
                    maxRowsDraw.Add(idex);
                }
            }
            maxRowsDraw.Sort();

            //获取行号
            if (maxRowsDraw.Count > 0)
            {
                b = (int)maxRowsDraw[0];
            }

            return b;
        }


        private int[] getNumber(int rIndex, string key, ArrayList al)
        {
            int[] s = new int[2];

            if (dLinesDrawRow.Contains(rIndex))
            {
                s[0] = rIndex;
                int indexer = dLinesDrawRow.IndexOf(rIndex);
                if (indexer > 0)
                {
                    s[1] = (int)dLinesDrawRow[indexer - 1];
                }
                else
                {
                    s[1] = rIndex;
                }
            }
            else
            {
                s[0] = rIndex;
                dLinesDrawRow.Add(rIndex);
                dLinesDrawRow.Sort();
                int indexer = dLinesDrawRow.IndexOf(rIndex);

                if (indexer >= dLinesDrawRow.Count - 1)
                {
                    s[0] = rIndex;
                }
                else
                {
                    s[0] = (int)dLinesDrawRow[indexer + 1];
                }
                s[1] = (int)dLinesDrawRow[indexer - 1];
                dLinesDrawRow.Remove(rIndex);
                
            }

            return s;
        }

        private int getSNumber2(int rIndex, string key, ArrayList al)
        {
            int s = 0;

            if (dLinesDrawRow.Contains(rIndex))
            {
                int indexer = dLinesDrawRow.IndexOf(rIndex);
                if (indexer > 0)
                {
                    s = (int)dLinesDrawRow[indexer - 1];
                }
                else
                {
                    s = rIndex;
                }

            }
            else
            {

                dLinesDrawRow.Add(rIndex);
                dLinesDrawRow.Sort();
                int indexer = dLinesDrawRow.IndexOf(rIndex);
                s = (int)dLinesDrawRow[indexer - 1];
                dLinesDrawRow.Remove(rIndex);
            }
            return s;
        }

        private void getDlinesDraw(int min, int max)
        {
            dLinesDrawRow.Clear();
            foreach (dLine dline in dLines)
            {
                int rIndex = dline.getRowIndex();
                int cIndex = dline.getCloumnIndex();
                int bottom = dline.getBottom();
                if (cIndex >= min && cIndex <= max && !dLinesDrawRow.Contains(rIndex) && bottom == 1)
                {
                    dLinesDrawRow.Add(rIndex);
                }
            }
            dLinesDrawRow.Sort();

        }

        private void button1_Click(object sender, EventArgs e)
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
                if (hei > 5)
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
            string fontFamily = this.dataGridView2.DefaultCellStyle.Font.FontFamily.Name;
            this.dataGridView2.DefaultCellStyle.Font = new Font(fontFamily, cellFont + 2);

            /*
            for (int j = 0; j < GlobalVariables.columnsSize; j++)
            {
                this.dataGridView1.Columns[j].Width += 10;
            }
            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                this.dataGridView1.Rows[i].Height += 10;
            }

            float cellFont1 = this.dataGridView1.DefaultCellStyle.Font.Size;
            this.dataGridView1.DefaultCellStyle.Font = new Font("宋体", cellFont1 + 2);
             * */
        }

        private void button2_Click(object sender, EventArgs e)
        {
            less();
            ///dgv1
            ///
            /*
            for (int j = 0; j < GlobalVariables.columnsSize; j++)
            {
                int wid = this.dataGridView1.Columns[j].Width;
                if (wid > 10)
                {
                    this.dataGridView1.Columns[j].Width -= 10;
                }
            }
            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                int hei = this.dataGridView1.Rows[i].Height;
                if (hei > 10)
                {
                    this.dataGridView1.Rows[i].Height -= 10;
                }
            }
            float cellFont1 = this.dataGridView1.DefaultCellStyle.Font.Size;

            if (cellFont1 > 9.0f)
            {
                this.dataGridView1.DefaultCellStyle.Font = new Font("宋体", cellFont1 - 2);
            }
             * */
        }

        private void less()
        {

            //列的宽度最小20
            for (int j = 0; j < this.dataGridView2.Columns.Count; j++)
            {
                int wid = this.dataGridView2.Columns[j].Width;
                if (wid > 5)
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
                if (hei > 5)
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
            string fontFamily = this.dataGridView2.DefaultCellStyle.Font.FontFamily.Name;
            if (cellFont > 2.0f)
            {
                this.dataGridView2.DefaultCellStyle.Font = new Font(fontFamily, cellFont - 2);
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            this.dataGridView1.ReadOnly = false;
            this.dataGridView2.ReadOnly = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            clearData(fileName);
            saveTxt();
            saveTxt2();
            MessageBox.Show("保存成功!");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           if (ts)
            {
                int columnIndex = e.ColumnIndex;
                int rowIndex = e.RowIndex;
                dLine line = getLine(columnIndex, rowIndex);
                string val = line.getValue();
                if (null != val && val.Length > 0)
                {
                    if (dvIndex != 2)
                    {
                        initGy();
                    }
                    dvIndex = 2;

                    if (col1 >= 0)
                    {
                        col2 = columnIndex;
                        row2 = rowIndex;
                        showGongYue();
                        ts = false;
                        dvIndex = -1;
                    }
                    else
                    {
                        col1 = columnIndex;
                        row1 = rowIndex;
                    }
                }
            }
        }

        private void showGongYue()
        {
            ArrayList gyList1 = new ArrayList();
            ArrayList gyList2 = new ArrayList();

            ArrayList temp = new ArrayList();
            if (row1 == row2)
            {
                if (dvIndex == 1)
                {
                    foreach (dLine d in dLinesOri)
                    {
                        int col = d.getCloumnIndex();
                        int row = d.getRowIndex();
                        string val = d.getValue();
                        if ((col == col1) && (row < row1) && (null != val && val.Trim().Length > 0))
                        {
                            gyList1.Add(val);
                        }
                        if ((col == col2) && (row < row2) && (null != val && val.Trim().Length > 0))
                        {
                            gyList2.Add(val);
                        }
                    }
                }
                else if (dvIndex == 2)
                {
                    foreach (dLine d in dLinesOri)
                    {
                        int col = d.getCloumnIndex();
                        int row = d.getRowIndex();
                        string val = d.getValue();
                        if ((col == col1) && (null != val && val.Trim().Length > 0))
                        {
                            gyList1.Add(val);
                        }
                        if ((col == col2)  && (null != val && val.Trim().Length > 0))
                        {
                            gyList2.Add(val);
                        }
                    }
                }
            }
            string vals = "";
            foreach (string d in gyList1)
            {
                if (gyList2.Contains(d) && !temp.Contains(d))
                {
                    vals = vals + "," + d;
                    temp.Add(d);
                }
            }
            if (vals.Length > 0)
            {
                vals = vals.Substring(1, vals.Length - 1);
            }
            GongYue gy = new GongYue(vals, 4);
            gy.Show();
            initGy();
        }

        private void initGy()
        {
            col1 = -1;
            row1 = -1;
            col2 = -1;
            row2 = -1;
            dvIndex = -1;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (!ts)
            {
                ts = true;
                System.Timers.Timer t = new System.Timers.Timer(10000);//实例化Timer类，设置间隔时间为1000毫秒 就是1秒；
                t.Elapsed += new System.Timers.ElapsedEventHandler(theout);//到达时间的时候执行事件；
                t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
                t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
            }
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGridView2.ReadOnly) 
            {
                return;
            }
            this.dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Red;
            foreach (dLine dline in dLines)
            {
                if (dline.getCloumnIndex() == e.ColumnIndex && dline.getRowIndex() == e.RowIndex)
                {
                    dline.setColor(Color.Red);
                    break;
                }
            }
        }

        public void theout(object source, System.Timers.ElapsedEventArgs e)
        {
            if (!(null == this) && !this.IsDisposed && !this.IsHandleCreated)
            {
                this.Invoke(new TextOption(f));//invok 委托实现跨线程的调用
            }
        }

        delegate void TextOption();//定义一个委托

        void f()
        {
            ts = false;
            initGy();
            dvIndex = -1;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            linesDraw.Clear();
            foreach (DictionaryEntry de in hTable)
            {
                //分组进行处理
                string key = de.Key.ToString();
                ArrayList al = (ArrayList)de.Value;
                al.Sort();

                int maxDrawColumnNumber = (int)al[al.Count - 1];
                int minDrawColumnNumber = (int)al[0];

                linesDraw.Add(maxDrawColumnNumber + 3);
            }
            this.dataGridView2.Refresh();
        }

        private void button9_Click(object sender, EventArgs e)
        {

        }






    }
}