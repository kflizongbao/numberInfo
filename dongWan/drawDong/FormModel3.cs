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
    public partial class FormModel3 : Form
    {
        string fileName;

        private ArrayList dLines = new ArrayList();
        private ArrayList dLinesBack = new ArrayList();

        private int rowCount = 0;

        private ArrayList dLinesOri = new ArrayList();

        private int rowCountOri = 0;
        private int loadDataCount = 0;
        private int canMaxColumnIndex = 0;
        private ArrayList dLinesColumns = new ArrayList();
        private ArrayList dLinesDataOri = new ArrayList();
        private ArrayList dColumns = new ArrayList();

        private ArrayList drawLinesColumns = new ArrayList();
        private ArrayList drawLinesItems = new ArrayList();
        private ArrayList drawLinesColVals = new ArrayList();
        
        private Boolean isLoad = false;
        private static System.Timers.Timer CheckUpdatetimer = new System.Timers.Timer();

        private int index = 0, index0 = 0, columnSize;
        private Boolean ts = false, drawLine = false, drawLineData = false;
        private int maxColum;
        public FormModel3(string p1, string p2, string p3)
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

        private void FormModel3_Load(object sender, EventArgs e)
        {
            handleControl();
            loadData();
            loadDataOri();
            getCadata();

            int s1 = dLines.Count;

            setIsDrawBack1();
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
            int step = 0;

            if (size>0)
            {
                step = 100 / size;
            }
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
            this.button8.Location = new Point(this.button7.Location.X - wid, this.button8.Location.Y);
            this.button9.Location = new Point(this.button8.Location.X - wid, this.button9.Location.Y);

            if (GlobalVariables.disa())
            {
                this.button8.Visible = false;
                this.button9.Visible = false;
            }
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
                        content += ("," + dl.getValue());
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

        private void getCadata()
        {
            /*foreach (dLine dline in dLinesDataOri)
            {
                if (dline.getCloumnIndex() < canMaxColumnIndex)
                {
                    dLines.Add(dline);
                }
            }*/

           for (int i = 0; i < dLinesDataOri.Count; i++)
            {
                dLine dline = (dLine)dLinesDataOri[i];
                if (dline.getCloumnIndex() < canMaxColumnIndex)
                {
                    dline.setOriIndex(i);
                    dline.setLinesIndex(dLines.Count);
                    dLines.Add(dline);
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
                    if (valueArr.Length > 1)
                    {
                        dline.setColor(Color.FromName(valueArr[1]));
                    }

                    dLinesDataOri.Add(dline);
                    if (dline.getValue() == "1")
                    {
                        string arg4 = "";
                        string arg3 = arg4 + "";
                    }
                    if (dline.getValue().Length > 0)
                    {
                        if (!dLinesColumns.Contains(i))
                        {
                            dLinesColumns.Add(i);
                        }
                    }

                    if (!dColumns.Contains(i))
                    {
                        dColumns.Add(i);
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
                dLinesColumns.Add(canMaxColumnIndex);
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
                    dLine dline = new dLine();
                    dline.setValue(items[i].Equals("-1") ? "" : items[i]);
                    dline.setCloumnIndex(i);
                    dline.setRowIndex(rowCountOri);
                    dLinesOri.Add(dline);
                    if (i > maxColum)
                    {
                        maxColum = i;
                    }
                }
                rowCountOri += 1;

            }
            sr.Close();
        }

        private String getDataOri(int rowIndex, int columnIndex)
        {
            String value = "";
            /*foreach (dLine dline in dLinesOri)
            {
                if (dline.getCloumnIndex() == columnIndex && dline.getRowIndex() == rowIndex)
                {
                    value = dline.getValue();
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

        private void gdv1LoadData()
        {
            for (int i = 0; i < rowCountOri; i++)
            {
                this.dataGridView1.Rows.Add();
                //int row = this.dataGridView1.Rows.Count - 2;
               // for (int j = 0; j < maxColum + 1; j++)
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
            loadDataCount += readyLoadCount;
        }

        private string getData(int rowIndex, int columnIndex)
        {
            string value = "";
            /*foreach (dLine dline in dLines)
            {
                if (dline.getCloumnIndex() == columnIndex && dline.getRowIndex() == rowIndex)
                {
                    value = dline.getValue();
                }
            }*/
            for (int j = index; j < dLines.Count; j++)
            {
                dLine dline = (dLine)dLines[j];
                if (dline.getCloumnIndex() == columnIndex && dline.getRowIndex() == rowIndex)
                {
                      value = dline.getValue();
                      index = j + 1;
                      break;
                }
            }
            return value;
        }

        private void setIsDrawBack1()
        {
            int parm = canMaxColumnIndex;
            foreach (dLine dline in dLines)
            {
                if (dline.getBack() == 1 || dline.getBack() == 2)
                {
                    continue;
                }
                int columnIndex = dline.getCloumnIndex();
                int rowIndex = dline.getRowIndex();
                string value = dline.getValue();
                if (dline.getBack() == 0 && value.Trim().Length > 0)
                {
                    int columnIndex01 = columnIndex;
                    int rowIndex01 = rowIndex - 1;
                    int p01 = rowIndex01 * canMaxColumnIndex + columnIndex01;
                    if (rowIndex01 > 0)
                    {
                        dLine d = (dLine)dLines[p01];
                        if (d.getBack() ==3)
                        {
                            dline.setBack(3);
                            continue;
                        }
                    }
                }
                if (null != value && dColumns.Contains(columnIndex) && value.Trim().Length > 0)
                {

                    int columnIndex01 = columnIndex;
                    int rowIndex01 = rowIndex - 1;

                    int columnIndex1 = columnIndex;
                    int rowIndex1 = rowIndex + 1;

                    int columnIndex2 = columnIndex;
                    int rowIndex2 = rowIndex + 2;

                    int columnIndex3 = columnIndex;
                    int rowIndex3 = rowIndex + 3;

                    int columnIndex4 = columnIndex;
                    int rowIndex4 = rowIndex + 4;

                    int columnIndex5 = columnIndex;
                    int rowIndex5 = rowIndex + 5;

                    int columnIndex6 = columnIndex;
                    int rowIndex6 = rowIndex + 6;

                    //第一种  连续2个
                    if (columnIndex1 < canMaxColumnIndex && rowIndex1 < rowCount)
                    {
                        int p01 = rowIndex01 * canMaxColumnIndex + columnIndex01;

                        int p1 = rowIndex1 * canMaxColumnIndex + columnIndex1;
                        int p2 = rowIndex2 * canMaxColumnIndex + columnIndex2;
                        int p3 = rowIndex3 * canMaxColumnIndex + columnIndex3;
                        int p4 = rowIndex4 * canMaxColumnIndex + columnIndex4;

                        if (p1 < dLines.Count && (p2 < dLines.Count || rowIndex2 >= this.dataGridView1.Rows.Count))
                        {
                            dLine dl1 = (dLine)dLines[p1];
                            dLine dl2 = null;
                            if (rowIndex2 >= this.dataGridView1.Rows.Count)
                            {

                            }
                            else
                            {
                                dl2 = (dLine)dLines[p2];
                            } 
                            string value1 = dl1.getValue();
                            string value2 = "";
                            if (null != dl2)
                            {
                                value2 = dl2.getValue();
                            }
                            string value01 = "";
                            if (rowIndex01 > 0)
                            {
                                dLine dl01 = (dLine)dLines[p01];
                                value01 = dl01.getValue();
                            }
                            if (value1.Length > 0 && value2.Trim().Length == 0 && value01.Trim().Length == 0)
                            {
                                dline.setBack(1);
                                dl1.setBack(1);
                            }
                        }

                        if (p1 < dLines.Count && p2 < dLines.Count && (p3 < dLines.Count || rowIndex3 >= this.dataGridView1.Rows.Count))
                        {
                            dLine dl1 = (dLine)dLines[p1];
                            dLine dl2 = (dLine)dLines[p2];
                            dLine dl3 = null;
                            if (rowIndex3 >= this.dataGridView1.Rows.Count)
                            {

                            }
                            else
                            {
                                dl3 = (dLine)dLines[p3];
                            }
                            
                            string value1 = dl1.getValue();
                            string value2 = dl2.getValue();
                            string value3 = "";
                            if (null != dl3)
                            {
                               value3 = dl3.getValue();
                            }
                            string value01 = "";
                            if (rowIndex01 > 0)
                            {
                                dLine dl01 = (dLine)dLines[p01];
                                value01 = dl01.getValue();
                            }
                            if (value1.Trim().Length > 0 && value2.Trim().Length > 0 && value3.Trim().Length == 0 && value01.Trim().Length == 0)
                            {
                                dline.setBack(2);
                                dl1.setBack(2);
                                dl2.setBack(2);
                            }
                        }

                        if (p1 < dLines.Count && p2 < dLines.Count && p3 < dLines.Count && (p4 < dLines.Count || rowIndex4 >= this.dataGridView1.Rows.Count))
                        {
                            dLine dl1 = (dLine)dLines[p1];
                            dLine dl2 = (dLine)dLines[p2];
                            dLine dl3 = (dLine)dLines[p3];

                            string value1 = dl1.getValue();
                            string value2 = dl2.getValue();
                            string value3 = dl3.getValue();

                            string value01 = "";
                            if (rowIndex01 > 0)
                            {
                                dLine dl01 = (dLine)dLines[p01];
                                value01 = dl01.getValue();
                            }
                            if (value1.Trim().Length > 0 && value2.Trim().Length > 0 && value3.Trim().Length > 0 && value01.Trim().Length == 0)
                            {
                                dline.setBack(3);
                                dl1.setBack(3);
                                dl2.setBack(3);
                                dl3.setBack(3);
                            }
                        }

                    }

                }
            }


        }

        private dLine getLine(int columnIndex, int rowIndex)
        {
            /*foreach (dLine dline in dLines)
            {
               
                if (dline.getCloumnIndex() == columnIndex && dline.getRowIndex() == rowIndex)
                {
                    if (dline.getValue() == "95" || dline.getValue() == "96" || dline.getValue() == "97" || dline.getValue() == "98")
                    {
                        string arg2 = "";
                        string arg3 = arg2 + "";
                    }
                    return dline;
                }
            }
            return null;*/

            int arg0 = rowIndex * columnSize + columnIndex;
            if (dLinesDataOri.Count <= arg0) 
            {
                return null;
            }
            return (dLine)dLines[((dLine)dLinesDataOri[arg0]).getLinesIndex()];
        }

        private void dataGridView2_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            int columnIndex = e.ColumnIndex;
            int rowIndex = e.RowIndex;
            if (columnIndex == 48)
            {
                int s = 0;
                int a = s + 1;
            }
            if (columnIndex <= columnSize && columnIndex >= 0 && rowIndex >= 0 && dLinesColumns.Contains(columnIndex) && rowCount > rowIndex && canMaxColumnIndex > columnIndex)
            {
                dLine line = getLine(columnIndex, rowIndex);

                if (line.getBack() > 0)
                {

                    Rectangle newRect = new Rectangle(e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width, e.CellBounds.Height);
                    Color clr = Color.Transparent;
                    switch (line.getBack())
                    {
                        case 1:
                            clr = Color.Yellow;
                            break;
                        case 2:
                            clr = Color.Green;
                            break;
                        case 3:
                            clr = Color.Blue;
                            break;
                    }
                    using (Brush backColorBrush = new SolidBrush(clr), redBrush = new SolidBrush(Color.Red))
                    {
                        Pen gridRedLinePen = new Pen(redBrush);
                        gridRedLinePen.Width = 1.0f;
                        // Erase the cell.
                        e.Graphics.FillRectangle(backColorBrush, e.CellBounds);

                        if (e.Value != null)
                        {
                            Font f = new Font(e.CellStyle.Font.Name, e.CellStyle.Font.Size, FontStyle.Bold);
                            e.Graphics.DrawString((String)e.Value, f, Brushes.Red, e.CellBounds.X + 2, e.CellBounds.Y + 2, StringFormat.GenericDefault);
                        }
                        e.Handled = true;
                    }
                }
                else
                {
                    using (Brush backColorBrush = new SolidBrush(e.CellStyle.BackColor))
                    {
                        // using (Pen gridLinePen = new Pen(gridBrush))
                        {
                            e.Graphics.FillRectangle(backColorBrush, e.CellBounds);//默认

                            if (e.Value != null)
                            {
                                Font f = new Font(e.CellStyle.Font.Name, e.CellStyle.Font.Size, FontStyle.Bold);
                                e.Graphics.DrawString((String)e.Value, f, line.getColor() == Color.Empty ? Brushes.Black : new SolidBrush(line.getColor()), e.CellBounds.X + 2, e.CellBounds.Y + 2, StringFormat.GenericDefault);
                            }
                            e.Handled = true;
                        }
                    }
                }
            }
            else
            {
                //如果是防线的话
                if (drawLine && !drawLineData)
                {
                    drwLines(e,columnIndex,rowIndex); 
                }
                else if (drawLine && drawLineData)
                {
                    drwLinesData(e, columnIndex, rowIndex);              
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
        }

        private void drwLinesData(DataGridViewCellPaintingEventArgs e, int columnIndex, int rowIndex)
        {
            if (drawLine && drawLinesColVals.Contains(columnIndex) && rowIndex < rowCount)
            {
                dLine lin = getCloseUpLine(columnIndex, rowIndex);
                Color clr1 = this.dataGridView2.GridColor;
                if (lin.getBack() > 0)
                {
                    Rectangle newRect = new Rectangle(e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width, e.CellBounds.Height);
                    switch (lin.getBack())
                    {
                        case 1:
                            clr1 = Color.Yellow;
                            break;
                        case 2:
                            clr1 = Color.Green;
                            break;
                        case 3:
                            clr1 = Color.Blue;
                            break;
                    }
                }
                using (Brush  backColorBrush = new SolidBrush(clr1), redBrush = new SolidBrush(Color.Red))
                {
                    Pen gridRedLinePen = new Pen(redBrush);
                    gridRedLinePen.Width = 3.0f;
                    e.Graphics.FillRectangle(backColorBrush, e.CellBounds);//默认
                    if (lin.getRight() == 1 || drawLinesColumns.Contains(columnIndex))
                    {
                        e.Graphics.DrawLine(gridRedLinePen, e.CellBounds.Right, e.CellBounds.Top, e.CellBounds.Right, e.CellBounds.Bottom);//right
                    }

                    if (lin.getValue() != null)
                    {
                        if (lin.getBack() > 0)
                        {
                            e.Graphics.DrawString(lin.getValue(), e.CellStyle.Font, Brushes.Red, e.CellBounds.X + 2, e.CellBounds.Y + 2, StringFormat.GenericDefault);
                        }
                        else
                        {
                            e.Graphics.DrawString(lin.getValue(), e.CellStyle.Font, Brushes.Black, e.CellBounds.X + 2, e.CellBounds.Y + 2, StringFormat.GenericDefault);
                        }
                    }
                    e.Handled = true;
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

        private void drwLines(DataGridViewCellPaintingEventArgs e, int columnIndex, int rowIndex)
        {
            using (Brush backColorBrush = new SolidBrush(this.dataGridView2.GridColor), redBrush = new SolidBrush(Color.Red))
            {
                e.Graphics.FillRectangle(backColorBrush, e.CellBounds);//默认
                e.Handled = true;
                if (drawLinesColumns.Contains(columnIndex) && rowIndex < rowCount)
                {
                    Pen gridRedLinePen = new Pen(redBrush);
                    gridRedLinePen.Width = 3.0f;
                    e.Graphics.DrawLine(gridRedLinePen, e.CellBounds.Right, e.CellBounds.Top, e.CellBounds.Right, e.CellBounds.Bottom);//right
                }
            }
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            less();
        }

        private void less()
        {
            //列的宽度最小20缩小
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
       
        private void button1_Click(object sender, EventArgs e)
        {
            //列的宽度最小20放大

            for (int j = 0; j < this.dataGridView2.Columns.Count; j++)
            {
                this.dataGridView2.Columns[j].Width += 5;
            }

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

        private void clearData(string filePath)
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

        private void button7_Click(object sender, EventArgs e)
        {
            this.ts = true;
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

        private void button8_Click(object sender, EventArgs e)
        {
            if (!drawLine)
            {
                int step = 4;
                drawLinesColumns.Clear();
                int j = 0;
                foreach (int i in dLinesColumns)
                {
                    if (j > 0 && i - j >= step)
                   {
                       drawLinesColumns.Add(j + step);
                       drawLinesColVals.Add(j + step);
                       drawLinesColVals.Add(j + step - 1);
                   }
                   j = i;
                }
                drawLine = true;
                if (!drawLinesColumns.Contains(dLinesColumns[dLinesColumns.Count - 1]))
                {
                    int val = (int)dLinesColumns[dLinesColumns.Count - 1] + step;
                    drawLinesColumns.Add(val);
                    drawLinesColVals.Add(val);
                    drawLinesColVals.Add(val - 1);
                }
                this.dataGridView2.Refresh();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //drawLinesColVals
            if (drawLine && !drawLineData)
            {
                drawLineData = true;
                int preIndex = 0;
                for(int i =0;i< drawLinesColumns.Count; i++)
                {
                    int n = (int)drawLinesColumns[i];
                    int j = 0;
                    for (int m = 0; m < dLines.Count; m++)
                    //foreach (dLine dl in dLines)
                    {
                        dLine dl = (dLine)dLines[m];
                        string val = dl.getValue();
                        if (null != val && val.Length > 0 && dl.getCloumnIndex() < n && dl.getCloumnIndex() >= preIndex)
                        {
                            dLine dline = new dLine();
                            dline.setValue(val);
                            dline.setCloumnIndex(n - (j+1) % 2);
                            dline.setRowIndex(dl.getRowIndex());
                            dline.setBack(dl.getBack());
                            if (val=="93")
                            {
                                int a = 0;
                                int b = a + 1;
                            }
                            if (j % 2 == 1)
                            {
                                dline.setRight(1);
                            }
                            dline.setBackGround(dl.getBackGround());
                            drawLinesItems.Add(dline);
                            j++;
                        }
                    }
                    preIndex= n;
                }
                this.dataGridView2.Refresh();
            }
        }

        private dLine getCloseUpLine(int columnIndex, int rowIndex)
        {
            if (columnIndex == 47)
            {
                int s = 0;
                int a = s + 1;
            }
            foreach (dLine dline in drawLinesItems)
            {              
                if (dline.getCloumnIndex() == columnIndex && dline.getRowIndex() == rowIndex)
                {
                    return dline;
                }
            }
            dLine dl = new dLine();
            dl.setRowIndex(rowIndex);
            dl.setCloumnIndex(columnIndex);
            return dl;
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (ts)
            {
                int row = this.dataGridView2.CurrentCell.RowIndex;
                int column = this.dataGridView2.CurrentCell.ColumnIndex;

                string content = "";
                foreach (dLine dl in dLinesOri)
                {
                    if (dl.getCloumnIndex() == column && dl.getValue().ToString().Length > 0)
                    {
                        if (content.Length > 0 )
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
