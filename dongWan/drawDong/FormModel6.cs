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
    public partial class FormModel6 : Form
    {
        string fileName;

        private ArrayList dLines = new ArrayList();

        private int rowCount = 0;



        private ArrayList dLinesOri = new ArrayList();

        private int rowCountOri = 0;

        private ArrayList rowsDraw = new ArrayList();

        private ArrayList columnsDraw = new ArrayList();

        private ArrayList gridColumnsDraw = new ArrayList();

        private ArrayList groupColumnsDraw = new ArrayList();//显示有值的所有的列存储的是arraylist

        private ArrayList gridDots = new ArrayList();//方形提示框

        private Hashtable hTable = new Hashtable();

        private int canMaxColumnIndex = 0;
        private ArrayList dLinesColumns = new ArrayList();
        private ArrayList dLinesDataOri = new ArrayList();
        private int loadDataCount = 0;
        private ArrayList dLinesDrawRow = new ArrayList();//画红线的行
        private Boolean isLoad = false;

        private static System.Timers.Timer CheckUpdatetimer = new System.Timers.Timer();

        private static int preRow;
        private static int[] preNumbers;

        private ArrayList hasColumns = new ArrayList();
        private ArrayList hasRows = new ArrayList();
        private int prePosition = 0;
        private int maxColum, columnSize;
        public FormModel6()
        {
            InitializeComponent();
            chkControl();
        }

        public FormModel6(string p1, string p2, string p3)
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();

            InitializeComponent();

            fileName = GlobalVariables.firPath + @"\" + p1 + @"\" + p2 + @"\" + p3;
        }

        private void FormModel6_Load(object sender, EventArgs e)
        {
            handleControl();

            loadData();
            loadDataOri();
            getCadata();

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
            gdv2LoadData(10); ;

            CheckUpdatetimer.Interval = 500;
            CheckUpdatetimer.Enabled = true;
            CheckUpdatetimer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            CheckUpdatetimer.Start();
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

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //放大
            for (int j = 0; j < this.dataGridView2.Columns.Count && this.dataGridView2.RowCount > 0; j++)
            {
                this.dataGridView2.Columns[j].Width += 5;
            }

            /*
            for (int i = 0; i < this.dataGridView2.Rows.Count; i++)
            {
                this.dataGridView2.Rows[i].Height += 5;
            }*/

            for (int i = 0; i < 1 && this.dataGridView2.RowCount > 0; i++)
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

            if (this.dataGridView2.RowCount > 0)
            {
                float cellFont = this.dataGridView2.DefaultCellStyle.Font.Size;
                this.dataGridView2.DefaultCellStyle.Font = new Font("宋体", cellFont + 2);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //缩小
            for (int j = 0; j < this.dataGridView2.Columns.Count && this.dataGridView2.RowCount > 0; j++)
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
            for (int i = 0; i < 1 && this.dataGridView2.RowCount > 0; i++)
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

            if (cellFont > 2.0f && this.dataGridView2.RowCount > 0)
            {
                this.dataGridView2.DefaultCellStyle.Font = new Font("宋体", cellFont - 2);
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

                    if (dline.getValue().Length > 0)
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

        private void getCadata()
        {

            /*foreach (dLine dline in dLinesDataOri)
            {
                if (dline.getCloumnIndex() <= canMaxColumnIndex)
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

        private void setIsDraw0()
        {
            int parm = canMaxColumnIndex + 1;

            foreach (dLine dlinei in dLines)
            {
                string value = dlinei.getValue();
                if (value.Equals("999"))
                {
                    int ss = 0;
                    int a = ss;
                    a++;
                }
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

                    int columnIndex4 = columnIndex + 4;
                    int rowIndex4 = rowIndex;

                    int columnIndex5 = columnIndex + 5;
                    int rowIndex5 = rowIndex + 1;

                    int columnIndex6 = columnIndex + 6;
                    int rowIndex6 = rowIndex;

                    int columnIndex7 = columnIndex + 7;
                    int rowIndex7 = rowIndex + 1;
                    //第一种模式计算

                    //第二种模式计算
                    int columnIndex11 = columnIndex + 1;
                    int rowIndex11 = rowIndex + 1;

                    int columnIndex12 = columnIndex + 2;
                    int rowIndex12 = rowIndex;

                    int columnIndex13 = columnIndex + 3;
                    int rowIndex13 = rowIndex + 1;

                    int columnIndex14 = columnIndex + 4;
                    int rowIndex14 = rowIndex + 1;

                    int columnIndex15 = columnIndex + 5;
                    int rowIndex15 = rowIndex;

                    int columnIndex16 = columnIndex + 6;
                    int rowIndex16 = rowIndex;

                    int columnIndex17 = columnIndex + 7;
                    int rowIndex17 = rowIndex + 1;
                    //第二种模式计算

                    //第三种模式
                    int columnIndex21 = columnIndex + 1;
                    int rowIndex21 = rowIndex + 1;

                    int columnIndex22 = columnIndex + 2;
                    int rowIndex22 = rowIndex + 1;

                    int columnIndex23 = columnIndex + 3;
                    int rowIndex23 = rowIndex;

                    int columnIndex24 = columnIndex + 4;
                    int rowIndex24 = rowIndex + 1;

                    int columnIndex25 = columnIndex + 5;
                    int rowIndex25 = rowIndex ;

                    int columnIndex26 = columnIndex + 6;
                    int rowIndex26 = rowIndex;

                    int columnIndex27 = columnIndex + 7;
                    int rowIndex27 = rowIndex + 1;
                    //第三种模式

                    //第四种模式
                    int columnIndex31 = columnIndex + 1;
                    int rowIndex31 = rowIndex + 1;

                    int columnIndex32 = columnIndex + 2;
                    int rowIndex32 = rowIndex + 1;

                    int columnIndex33 = columnIndex + 3;
                    int rowIndex33 = rowIndex;

                    int columnIndex34 = columnIndex + 4;
                    int rowIndex34 = rowIndex;

                    int columnIndex35 = columnIndex + 5;
                    int rowIndex35 = rowIndex + 1;

                    int columnIndex36 = columnIndex + 6;
                    int rowIndex36 = rowIndex;

                    int columnIndex37 = columnIndex + 7;
                    int rowIndex37 = rowIndex + 1;

                    //第五种
                   int columnIndex41 = columnIndex + 1;
                   int rowIndex41 = rowIndex - 1;

                   int columnIndex42 = columnIndex + 2;
                   int rowIndex42 = rowIndex;

                   int columnIndex43 = columnIndex + 3;
                   int rowIndex43 = rowIndex - 1;

                   int columnIndex44 = columnIndex + 4;
                   int rowIndex44 = rowIndex;

                   int columnIndex45 = columnIndex + 5;
                   int rowIndex45 = rowIndex - 1;

                   int columnIndex46 = columnIndex + 6;
                   int rowIndex46 = rowIndex - 1;

                   int columnIndex47 = columnIndex + 7;
                   int rowIndex47 = rowIndex;

                   //第六种
                   int columnIndex51 = columnIndex + 1;
                   int rowIndex51 = rowIndex - 1;

                   int columnIndex52 = columnIndex + 2;
                   int rowIndex52 = rowIndex;

                   int columnIndex53 = columnIndex + 3;
                   int rowIndex53 = rowIndex - 1;

                   int columnIndex54 = columnIndex + 4;
                   int rowIndex54 = rowIndex - 1;

                   int columnIndex55 = columnIndex + 5;
                   int rowIndex55 = rowIndex;

                   int columnIndex56 = columnIndex + 6;
                   int rowIndex56 = rowIndex - 1;

                   int columnIndex57 = columnIndex + 7;
                   int rowIndex57 = rowIndex;

                   //第七种
                   int columnIndex61 = columnIndex + 1;
                   int rowIndex61 = rowIndex - 1;

                   int columnIndex62 = columnIndex + 2;
                   int rowIndex62 = rowIndex - 1;

                   int columnIndex63 = columnIndex + 3;
                   int rowIndex63 = rowIndex;

                   int columnIndex64 = columnIndex + 4;
                   int rowIndex64 = rowIndex;

                   int columnIndex65 = columnIndex + 5;
                   int rowIndex65 = rowIndex - 1;

                   int columnIndex66 = columnIndex + 6;
                   int rowIndex66 = rowIndex - 1;

                   int columnIndex67 = columnIndex + 7;
                   int rowIndex67 = rowIndex;

                   //第八种
                   int columnIndex71 = columnIndex + 1;
                   int rowIndex71 = rowIndex - 1;

                   int columnIndex72 = columnIndex + 2;
                   int rowIndex72 = rowIndex - 1;

                   int columnIndex73 = columnIndex + 3;
                   int rowIndex73 = rowIndex;

                   int columnIndex74 = columnIndex + 4;
                   int rowIndex74 = rowIndex - 1;

                   int columnIndex75 = columnIndex + 5;
                   int rowIndex75 = rowIndex;

                   int columnIndex76 = columnIndex + 6;
                   int rowIndex76 = rowIndex - 1;

                   int columnIndex77 = columnIndex + 7;
                   int rowIndex77 = rowIndex;

                   //第九种模式计算
                   int columnIndex91 = columnIndex + 1;
                   int rowIndex91 = rowIndex + 1;

                   int columnIndex92 = columnIndex + 2;
                   int rowIndex92 = rowIndex;

                   int columnIndex93 = columnIndex + 3;
                   int rowIndex93 = rowIndex + 1;

                   int columnIndex94 = columnIndex + 4;
                   int rowIndex94 = rowIndex;

                   int columnIndex95 = columnIndex + 5;
                   int rowIndex95 = rowIndex + 1;

                   int columnIndex96 = columnIndex + 6;
                   int rowIndex96 = rowIndex + 1;

                   int columnIndex97 = columnIndex + 7;
                   int rowIndex97 = rowIndex;
                   //第九种模式计算

                   //第十种模式计算
                   int columnIndex101 = columnIndex + 1;
                   int rowIndex101 = rowIndex + 1;

                   int columnIndex102 = columnIndex + 2;
                   int rowIndex102 = rowIndex;

                   int columnIndex103 = columnIndex + 3;
                   int rowIndex103 = rowIndex + 1;

                   int columnIndex104 = columnIndex + 4;
                   int rowIndex104 = rowIndex + 1;

                   int columnIndex105 = columnIndex + 5;
                   int rowIndex105 = rowIndex;

                   int columnIndex106 = columnIndex + 6;
                   int rowIndex106 = rowIndex + 1;

                   int columnIndex107 = columnIndex + 7;
                   int rowIndex107 = rowIndex;
                   //第十种模式计算

                   //第十一种模式
                   int columnIndex201 = columnIndex + 1;
                   int rowIndex201 = rowIndex + 1;

                   int columnIndex202 = columnIndex + 2;
                   int rowIndex202 = rowIndex + 1;

                   int columnIndex203 = columnIndex + 3;
                   int rowIndex203 = rowIndex;

                   int columnIndex204 = columnIndex + 4;
                   int rowIndex204 = rowIndex + 1;

                   int columnIndex205 = columnIndex + 5;
                   int rowIndex205 = rowIndex;

                   int columnIndex206 = columnIndex + 6;
                   int rowIndex206 = rowIndex + 1;

                   int columnIndex207 = columnIndex + 7;
                   int rowIndex207 = rowIndex;
                   //第十一种模式

                   //第十二种模式
                   int columnIndex301 = columnIndex + 1;
                   int rowIndex301 = rowIndex + 1;

                   int columnIndex302 = columnIndex + 2;
                   int rowIndex302 = rowIndex + 1;

                   int columnIndex303 = columnIndex + 3;
                   int rowIndex303 = rowIndex;

                   int columnIndex304 = columnIndex + 4;
                   int rowIndex304 = rowIndex;

                   int columnIndex305 = columnIndex + 5;
                   int rowIndex305 = rowIndex + 1;

                   int columnIndex306 = columnIndex + 6;
                   int rowIndex306 = rowIndex + 1;

                   int columnIndex307 = columnIndex + 7;
                   int rowIndex307 = rowIndex;
                   //第十二种模式

                   //第十三种
                   int columnIndex401 = columnIndex + 1;
                   int rowIndex401 = rowIndex - 1;

                   int columnIndex402 = columnIndex + 2;
                   int rowIndex402 = rowIndex;

                   int columnIndex403 = columnIndex + 3;
                   int rowIndex403 = rowIndex - 1;

                   int columnIndex404 = columnIndex + 4;
                   int rowIndex404 = rowIndex;

                   int columnIndex405 = columnIndex + 5;
                   int rowIndex405 = rowIndex - 1;

                   int columnIndex406 = columnIndex + 6;
                   int rowIndex406 = rowIndex;

                   int columnIndex407 = columnIndex + 7;
                   int rowIndex407 = rowIndex - 1;
                   //第十三种

                   //第十四种
                   int columnIndex501 = columnIndex + 1;
                   int rowIndex501 = rowIndex - 1;

                   int columnIndex502 = columnIndex + 2;
                   int rowIndex502 = rowIndex;

                   int columnIndex503 = columnIndex + 3;
                   int rowIndex503 = rowIndex - 1;

                   int columnIndex504 = columnIndex + 4;
                   int rowIndex504 = rowIndex - 1;

                   int columnIndex505 = columnIndex + 5;
                   int rowIndex505 = rowIndex;

                   int columnIndex506 = columnIndex + 6;
                   int rowIndex506 = rowIndex;

                   int columnIndex507 = columnIndex + 7;
                   int rowIndex507 = rowIndex - 1;
                   //第十四种

                   //第十五种
                   int columnIndex601 = columnIndex + 1;
                   int rowIndex601 = rowIndex - 1;

                   int columnIndex602 = columnIndex + 2;
                   int rowIndex602 = rowIndex - 1;

                   int columnIndex603 = columnIndex + 3;
                   int rowIndex603 = rowIndex;

                   int columnIndex604 = columnIndex + 4;
                   int rowIndex604 = rowIndex;

                   int columnIndex605 = columnIndex + 5;
                   int rowIndex605 = rowIndex - 1;

                   int columnIndex606 = columnIndex + 6;
                   int rowIndex606 = rowIndex;

                   int columnIndex607 = columnIndex + 7;
                   int rowIndex607 = rowIndex - 1;
                   //第十五种

                   //第十六种
                   int columnIndex701 = columnIndex + 1;
                   int rowIndex701 = rowIndex - 1;

                   int columnIndex702 = columnIndex + 2;
                   int rowIndex702 = rowIndex - 1;

                   int columnIndex703 = columnIndex + 3;
                   int rowIndex703 = rowIndex;

                   int columnIndex704 = columnIndex + 4;
                   int rowIndex704 = rowIndex - 1;

                   int columnIndex705 = columnIndex + 5;
                   int rowIndex705 = rowIndex;

                   int columnIndex706 = columnIndex + 6;
                   int rowIndex706 = rowIndex;

                   int columnIndex707 = columnIndex + 7;
                   int rowIndex707 = rowIndex - 1;
                   //第十六种

                    if (value.Length > 0)
                    {
                        //第一种
                        if (rowCount > rowIndex1 && rowCount > rowIndex2 && rowCount > rowIndex3 && rowCount > rowIndex4 && rowCount > rowIndex5 && rowCount > rowIndex6 && rowCount > rowIndex7 &&
                            columnIndex1 < parm && columnIndex2 < parm && columnIndex3 < parm && columnIndex4 < parm && columnIndex5 < parm && columnIndex6 < parm && columnIndex7 < parm)
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

                            int p4 = rowIndex4 * parm + columnIndex4;
                            dLine dl4 = (dLine)dLines[p4];
                            string value4 = dl4.getValue();

                            int p5 = rowIndex5 * parm + columnIndex5;
                            dLine dl5 = (dLine)dLines[p5];
                            string value5 = dl5.getValue();

                            int p6 = rowIndex6 * parm + columnIndex6;
                            dLine dl6 = (dLine)dLines[p6];
                            string value6 = dl6.getValue();

                            int p7 = rowIndex7 * parm + columnIndex7;
                            dLine dl7 = (dLine)dLines[p7];
                            string value7 = dl7.getValue();

                            if (value1.Length > 0 && value2.Length > 0 && value3.Length > 0 && value4.Length > 0 && value5.Length > 0 && value6.Length > 0 && value7.Length > 0)
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

                                int pa4 = rowIndex4 * parm + columnIndex4;
                                dLine dla4 = (dLine)dLines[pa4];
                                dla4.setBottom(1);

                                int pa5 = (rowIndex5 - 1) * parm + columnIndex5;
                                dLine dla5 = (dLine)dLines[pa5];
                                dla5.setBottom(1);

                                int pa6 = rowIndex6 * parm + columnIndex6;
                                dLine dla6 = (dLine)dLines[pa6];
                                dla6.setBottom(1);

                                int pa7 = (rowIndex7 - 1) * parm + columnIndex7;
                                dLine dla7 = (dLine)dLines[pa7];
                                dla7.setBottom(1);
                            }
                        }
                         
                        //第二种判断
                        if (rowCount > rowIndex11 && rowCount > rowIndex12 && rowCount > rowIndex13 && rowCount > rowIndex14 && rowCount > rowIndex15 && rowCount > rowIndex16 && rowCount > rowIndex17 &&
                            columnIndex11 < parm && columnIndex12 < parm && columnIndex13 < parm && columnIndex14 < parm && columnIndex15 < parm && columnIndex16 < parm && columnIndex17 < parm)
                        {
                            int p = rowIndex * parm + columnIndex;
                            dLine dl = (dLine)dLines[p];

                            int p11 = rowIndex11 * parm + columnIndex11;
                            dLine dl11 = (dLine)dLines[p11];
                            string value11 = dl11.getValue();

                            int p12 = rowIndex12 * parm + columnIndex12;
                            dLine dl12 = (dLine)dLines[p12];
                            string value12 = dl12.getValue();


                            int p13 = rowIndex13 * parm + columnIndex13;
                            dLine dl13 = (dLine)dLines[p13];
                            string value13 = dl13.getValue();

                            int p14 = rowIndex14 * parm + columnIndex14;
                            dLine dl14 = (dLine)dLines[p14];
                            string value14 = dl14.getValue();

                            int p15 = rowIndex15 * parm + columnIndex15;
                            dLine dl15 = (dLine)dLines[p15];
                            string value15 = dl15.getValue();

                            int p16 = rowIndex16 * parm + columnIndex16;
                            dLine dl16 = (dLine)dLines[p16];
                            string value16 = dl16.getValue();

                            int p17 = rowIndex17 * parm + columnIndex17;
                            dLine dl17 = (dLine)dLines[p17];
                            string value17 = dl17.getValue();

                            if (value11.Length > 0 && value12.Length > 0 && value13.Length > 0 && value14.Length > 0 && value15.Length > 0 && value16.Length > 0 && value17.Length > 0)
                            {
                                int pa = rowIndex * parm + columnIndex;
                                dLine dla = (dLine)dLines[pa];
                                dla.setBottom(1);

                                int pa11 = (rowIndex11 - 1) * parm + columnIndex11;
                                dLine dla11 = (dLine)dLines[pa11];
                                dla11.setBottom(1);

                                int pa12 = rowIndex12 * parm + columnIndex12;
                                dLine dla12 = (dLine)dLines[pa12];
                                dla12.setBottom(1);

                                int pa13 = (rowIndex13 - 1) * parm + columnIndex13;
                                dLine dla13 = (dLine)dLines[pa13];
                                dla13.setBottom(1);

                                int pa14 = (rowIndex14 - 1) * parm + columnIndex14;
                                dLine dla14 = (dLine)dLines[pa14];
                                dla14.setBottom(1);

                                int pa15 = rowIndex15 * parm + columnIndex15;
                                dLine dla15 = (dLine)dLines[pa15];
                                dla15.setBottom(1);

                                int pa16 = rowIndex16 * parm + columnIndex16;
                                dLine dla16 = (dLine)dLines[pa16];
                                dla16.setBottom(1);

                                int pa17 = (rowIndex17 - 1) * parm + columnIndex17;
                                dLine dla17 = (dLine)dLines[pa17];
                                dla17.setBottom(1);
                            }
                        }
                        //第二种判断

                        //第三种判断
                        if (rowCount > rowIndex21 && rowCount > rowIndex22 && rowCount > rowIndex23 && rowCount > rowIndex24 && rowCount > rowIndex25 && rowCount > rowIndex26 && rowCount > rowIndex27 &&
                            columnIndex21 < parm && columnIndex22 < parm && columnIndex23 < parm && columnIndex24 < parm && columnIndex25 < parm && columnIndex26 < parm && columnIndex27 < parm)
                        {
                            int p = rowIndex * parm + columnIndex;
                            dLine dl = (dLine)dLines[p];

                            int p21 = rowIndex21 * parm + columnIndex21;
                            dLine dl21 = (dLine)dLines[p21];
                            string value21 = dl21.getValue();

                            int p22 = rowIndex22 * parm + columnIndex22;
                            dLine dl22 = (dLine)dLines[p22];
                            string value22 = dl22.getValue();


                            int p23 = rowIndex23 * parm + columnIndex23;
                            dLine dl23 = (dLine)dLines[p23];
                            string value23 = dl23.getValue();

                            int p24 = rowIndex24 * parm + columnIndex24;
                            dLine dl24 = (dLine)dLines[p24];
                            string value24 = dl24.getValue();

                            int p25 = rowIndex25 * parm + columnIndex25;
                            dLine dl25 = (dLine)dLines[p25];
                            string value25 = dl25.getValue();

                            int p26 = rowIndex26 * parm + columnIndex26;
                            dLine dl26 = (dLine)dLines[p26];
                            string value26 = dl26.getValue();

                            int p27 = rowIndex27 * parm + columnIndex27;
                            dLine dl27 = (dLine)dLines[p27];
                            string value27 = dl27.getValue();

                            if (value21.Length > 0 && value22.Length > 0 && value23.Length > 0 && value24.Length > 0 && value25.Length > 0 && value26.Length > 0 && value27.Length > 0)
                            {
                                int pa = rowIndex * parm + columnIndex;
                                dLine dla = (dLine)dLines[pa];
                                dla.setBottom(1);

                                int pa21 = (rowIndex21 - 1) * parm + columnIndex21;
                                dLine dla21 = (dLine)dLines[pa21];
                                dla21.setBottom(1);

                                int pa22 = (rowIndex22 - 1) * parm + columnIndex22;
                                dLine dla22 = (dLine)dLines[pa22];
                                dla22.setBottom(1);

                                int pa23 = rowIndex23 * parm + columnIndex23;
                                dLine dla23 = (dLine)dLines[pa23];
                                dla23.setBottom(1);

                                int pa24 = (rowIndex24 - 1) * parm + columnIndex24;
                                dLine dla24 = (dLine)dLines[pa24];
                                dla24.setBottom(1);

                                int pa25 = rowIndex25 * parm + columnIndex25;
                                dLine dla25 = (dLine)dLines[pa25];
                                dla25.setBottom(1);

                                int pa26 = rowIndex26 * parm + columnIndex26;
                                dLine dla26 = (dLine)dLines[pa26];
                                dla26.setBottom(1);

                                int pa27 = (rowIndex27 - 1) * parm + columnIndex27;
                                dLine dla27 = (dLine)dLines[pa27];
                                dla27.setBottom(1);
                            }
                        }
                        //第三种判断

                        //第四种判断
                        if (rowCount > rowIndex31 && rowCount > rowIndex32 && rowCount > rowIndex33 && rowCount > rowIndex34 && rowCount > rowIndex35 && rowCount > rowIndex36 && rowCount > rowIndex37 &&
                            columnIndex31 < parm && columnIndex32 < parm && columnIndex33 < parm && columnIndex34 < parm && columnIndex35 < parm && columnIndex36 < parm && columnIndex37 < parm)
                        {
                            int p = rowIndex * parm + columnIndex;
                            dLine dl = (dLine)dLines[p];

                            int p31 = rowIndex31 * parm + columnIndex31;
                            dLine dl31 = (dLine)dLines[p31];
                            string value31 = dl31.getValue();

                            int p32 = rowIndex32 * parm + columnIndex32;
                            dLine dl32 = (dLine)dLines[p32];
                            string value32 = dl32.getValue();


                            int p33 = rowIndex33 * parm + columnIndex33;
                            dLine dl33 = (dLine)dLines[p33];
                            string value33 = dl33.getValue();

                            int p34 = rowIndex34 * parm + columnIndex34;
                            dLine dl34 = (dLine)dLines[p34];
                            string value34 = dl34.getValue();

                            int p35 = rowIndex35 * parm + columnIndex35;
                            dLine dl35 = (dLine)dLines[p35];
                            string value35 = dl35.getValue();

                            int p36 = rowIndex36 * parm + columnIndex36;
                            dLine dl36 = (dLine)dLines[p36];
                            string value36 = dl36.getValue();

                            int p37 = rowIndex37 * parm + columnIndex37;
                            dLine dl37 = (dLine)dLines[p37];
                            string value37 = dl37.getValue();

                            if (value31.Length > 0 && value32.Length > 0 && value33.Length > 0 && value34.Length > 0 && value35.Length > 0 && value36.Length > 0 && value37.Length > 0)
                            {
                                int pa = rowIndex * parm + columnIndex;
                                dLine dla = (dLine)dLines[pa];
                                dla.setBottom(1);

                                int pa31 = (rowIndex31 - 1) * parm + columnIndex31;
                                dLine dla31 = (dLine)dLines[pa31];
                                dla31.setBottom(1);

                                int pa32 = (rowIndex32 - 1) * parm + columnIndex32;
                                dLine dla32 = (dLine)dLines[pa32];
                                dla32.setBottom(1);

                                int pa33 = rowIndex33 * parm + columnIndex33;
                                dLine dla33 = (dLine)dLines[pa33];
                                dla33.setBottom(1);

                                int pa34 = rowIndex34 * parm + columnIndex34;
                                dLine dla34 = (dLine)dLines[pa34];
                                dla34.setBottom(1);

                                int pa35 = (rowIndex35  - 1)* parm + columnIndex35;
                                dLine dla35 = (dLine)dLines[pa35];
                                dla35.setBottom(1);

                                int pa36 = rowIndex36 * parm + columnIndex36;
                                dLine dla36 = (dLine)dLines[pa36];
                                dla36.setBottom(1);

                                int pa37 = (rowIndex37 - 1) * parm + columnIndex37;
                                dLine dla37 = (dLine)dLines[pa37];
                                dla37.setBottom(1);
                            }
                        }
                        //第四种判断

                        //第五种判断
                        if (rowIndex41 >= 0 && rowIndex42 >= 0 && rowIndex43 >= 0 && rowIndex44 >= 0 && rowIndex45 >= 0 && rowIndex46 >= 0 && rowIndex47 >= 0 &&
                            rowCount > rowIndex41 && rowCount > rowIndex42 && rowCount > rowIndex43 && rowCount > rowIndex44 && rowCount > rowIndex45 && rowCount > rowIndex46 && rowCount > rowIndex47 &&
                            columnIndex41 < parm && columnIndex42 < parm && columnIndex43 < parm && columnIndex44 < parm && columnIndex45 < parm && columnIndex46 < parm && columnIndex47 < parm)
                        {
                            int p = rowIndex * parm + columnIndex;
                            dLine dl = (dLine)dLines[p];

                            int p41 = rowIndex41 * parm + columnIndex41;
                            dLine dl41 = (dLine)dLines[p41];
                            string value41 = dl41.getValue();

                            int p42 = rowIndex42 * parm + columnIndex42;
                            dLine dl42 = (dLine)dLines[p42];
                            string value42 = dl42.getValue();

                            int p43 = rowIndex43 * parm + columnIndex43;
                            dLine dl43 = (dLine)dLines[p43];
                            string value43 = dl43.getValue();

                            int p44 = rowIndex44 * parm + columnIndex44;
                            dLine dl44 = (dLine)dLines[p44];
                            string value44 = dl44.getValue();

                            int p45 = rowIndex45 * parm + columnIndex45;
                            dLine dl45 = (dLine)dLines[p45];
                            string value45 = dl45.getValue();

                            int p46 = rowIndex46 * parm + columnIndex46;
                            dLine dl46 = (dLine)dLines[p46];
                            string value46 = dl46.getValue();

                            int p47 = rowIndex47 * parm + columnIndex47;
                            dLine dl47 = (dLine)dLines[p47];
                            string value47 = dl47.getValue();

                            if (value41.Length > 0 && value42.Length > 0 && value43.Length > 0 && value44.Length > 0 && value45.Length > 0 && value46.Length > 0 && value47.Length > 0)
                            {
                                int pa = (rowIndex - 1) * parm + columnIndex;
                                dLine dla = (dLine)dLines[pa];
                                dla.setBottom(1);

                                int pa41 = rowIndex41 * parm + columnIndex41;
                                dLine dla41 = (dLine)dLines[pa41];
                                dla41.setBottom(1);

                                int pa42 = (rowIndex42 - 1) * parm + columnIndex42;
                                dLine dla42 = (dLine)dLines[pa42];
                                dla42.setBottom(1);

                                int pa43 = rowIndex43 * parm + columnIndex43;
                                dLine dla43 = (dLine)dLines[pa43];
                                dla43.setBottom(1);

                                int pa44 = (rowIndex44 - 1) * parm + columnIndex44;
                                dLine dla44 = (dLine)dLines[pa44];
                                dla44.setBottom(1);

                                int pa45 = rowIndex45 * parm + columnIndex45;
                                dLine dla45 = (dLine)dLines[pa45];
                                dla45.setBottom(1);

                                int pa46 = rowIndex46 * parm + columnIndex46;
                                dLine dla46 = (dLine)dLines[pa46];
                                dla46.setBottom(1);

                                int pa47 = (rowIndex47 - 1) * parm + columnIndex47;
                                dLine dla47 = (dLine)dLines[pa47];
                                dla47.setBottom(1);
                            }
                        }
                        //第五种判断

                        //第六种判断
                        if (rowIndex51 >= 0 && rowIndex52 >= 0 && rowIndex53 >= 0 && rowIndex54 >= 0 && rowIndex55 >= 0 && rowIndex56 >= 0 && rowIndex57 >= 0 &&
                            rowCount > rowIndex51 && rowCount > rowIndex52 && rowCount > rowIndex53 && rowCount > rowIndex54 && rowCount > rowIndex55 && rowCount > rowIndex56 && rowCount > rowIndex57 &&
                            columnIndex51 < parm && columnIndex52 < parm && columnIndex53 < parm && columnIndex54 < parm && columnIndex55 < parm && columnIndex56 < parm && columnIndex57 < parm)
                        {
                            int p = rowIndex * parm + columnIndex;
                            dLine dl = (dLine)dLines[p];

                            int p51 = rowIndex51 * parm + columnIndex51;
                            dLine dl51 = (dLine)dLines[p51];
                            string value51 = dl51.getValue();

                            int p52 = rowIndex52 * parm + columnIndex52;
                            dLine dl52 = (dLine)dLines[p52];
                            string value52 = dl52.getValue();

                            int p53 = rowIndex53 * parm + columnIndex53;
                            dLine dl53 = (dLine)dLines[p53];
                            string value53 = dl53.getValue();

                            int p54 = rowIndex54 * parm + columnIndex54;
                            dLine dl54 = (dLine)dLines[p54];
                            string value54 = dl54.getValue();

                            int p55 = rowIndex55 * parm + columnIndex55;
                            dLine dl55 = (dLine)dLines[p55];
                            string value55 = dl55.getValue();

                            int p56 = rowIndex56 * parm + columnIndex56;
                            dLine dl56 = (dLine)dLines[p56];
                            string value56 = dl56.getValue();

                            int p57 = rowIndex57 * parm + columnIndex57;
                            dLine dl57 = (dLine)dLines[p57];
                            string value57 = dl57.getValue();

                            if (value51.Length > 0 && value52.Length > 0 && value53.Length > 0 && value54.Length > 0 && value55.Length > 0 && value56.Length > 0 && value57.Length > 0)
                            {
                                int pa = (rowIndex - 1) * parm + columnIndex;
                                dLine dla = (dLine)dLines[pa];
                                dla.setBottom(1);

                                int pa51 = rowIndex51 * parm + columnIndex51;
                                dLine dla51 = (dLine)dLines[pa51];
                                dla51.setBottom(1);

                                int pa52 = (rowIndex52 - 1) * parm + columnIndex52;
                                dLine dla52 = (dLine)dLines[pa52];
                                dla52.setBottom(1);

                                int pa53 = rowIndex53 * parm + columnIndex53;
                                dLine dla53 = (dLine)dLines[pa53];
                                dla53.setBottom(1);

                                int pa54 = rowIndex54 * parm + columnIndex54;
                                dLine dla54 = (dLine)dLines[pa54];
                                dla54.setBottom(1);

                                int pa55 = (rowIndex55 - 1) * parm + columnIndex55;
                                dLine dla55 = (dLine)dLines[pa55];
                                dla55.setBottom(1);

                                int pa56 = rowIndex56 * parm + columnIndex56;
                                dLine dla56 = (dLine)dLines[pa56];
                                dla56.setBottom(1);

                                int pa57 = (rowIndex57 - 1) * parm + columnIndex57;
                                dLine dla57 = (dLine)dLines[pa57];
                                dla57.setBottom(1);
                            }
                        }
                        //第六种判断

                        //第七种判断
                        if (rowIndex61 >= 0 && rowIndex62 >= 0 && rowIndex63 >= 0 && rowIndex64 >= 0 && rowIndex65 >= 0 && rowIndex66 >= 0 && rowIndex67 >= 0 &&
                            rowCount > rowIndex61 && rowCount > rowIndex62 && rowCount > rowIndex63 && rowCount > rowIndex64 && rowCount > rowIndex65 && rowCount > rowIndex66 && rowCount > rowIndex67 &&
                            columnIndex61 < parm && columnIndex62 < parm && columnIndex63 < parm && columnIndex64 < parm && columnIndex65 < parm && columnIndex66 < parm && columnIndex67 < parm)
                        {
                            int p = rowIndex * parm + columnIndex;
                            dLine dl = (dLine)dLines[p];

                            int p61 = rowIndex61 * parm + columnIndex61;
                            dLine dl61 = (dLine)dLines[p61];
                            string value61 = dl61.getValue();

                            int p62 = rowIndex62 * parm + columnIndex62;
                            dLine dl62 = (dLine)dLines[p62];
                            string value62 = dl62.getValue();

                            int p63 = rowIndex63 * parm + columnIndex63;
                            dLine dl63 = (dLine)dLines[p63];
                            string value63 = dl63.getValue();

                            int p64 = rowIndex64 * parm + columnIndex64;
                            dLine dl64 = (dLine)dLines[p64];
                            string value64 = dl64.getValue();

                            int p65 = rowIndex65 * parm + columnIndex65;
                            dLine dl65 = (dLine)dLines[p65];
                            string value65 = dl65.getValue();

                            int p66 = rowIndex66 * parm + columnIndex66;
                            dLine dl66 = (dLine)dLines[p66];
                            string value66 = dl66.getValue();

                            int p67 = rowIndex67 * parm + columnIndex67;
                            dLine dl67 = (dLine)dLines[p67];
                            string value67 = dl67.getValue();

                            if (value61.Length > 0 && value62.Length > 0 && value63.Length > 0 && value64.Length > 0 && value65.Length > 0 && value66.Length > 0 && value67.Length > 0)
                            {
                                int pa = (rowIndex - 1) * parm + columnIndex;
                                dLine dla = (dLine)dLines[pa];
                                dla.setBottom(1);

                                int pa61 = rowIndex61 * parm + columnIndex61;
                                dLine dla61 = (dLine)dLines[pa61];
                                dla61.setBottom(1);

                                int pa62 = rowIndex62 * parm + columnIndex62;
                                dLine dla62 = (dLine)dLines[pa62];
                                dla62.setBottom(1);

                                int pa63 = (rowIndex63 - 1) * parm + columnIndex63;
                                dLine dla63 = (dLine)dLines[pa63];
                                dla63.setBottom(1);

                                int pa64 = (rowIndex64 - 1) * parm + columnIndex64;
                                dLine dla64 = (dLine)dLines[pa64];
                                dla64.setBottom(1);

                                int pa65 = rowIndex65 * parm + columnIndex65;
                                dLine dla65 = (dLine)dLines[pa65];
                                dla65.setBottom(1);

                                int pa66 = rowIndex66 * parm + columnIndex66;
                                dLine dla66 = (dLine)dLines[pa66];
                                dla66.setBottom(1);

                                int pa67 = (rowIndex67 - 1) * parm + columnIndex67;
                                dLine dla67 = (dLine)dLines[pa67];
                                dla67.setBottom(1);
                            }
                        }
                        //第七种判断

                        //第八种判断
                        if (rowIndex71 >= 0 && rowIndex72 >= 0 && rowIndex73 >= 0 && rowIndex74 >= 0 && rowIndex75 >= 0 && rowIndex76 >= 0 && rowIndex77 >= 0 &&
                            rowCount > rowIndex71 && rowCount > rowIndex72 && rowCount > rowIndex73 && rowCount > rowIndex74 && rowCount > rowIndex75 && rowCount > rowIndex76 && rowCount > rowIndex77 &&
                            columnIndex71 < parm && columnIndex72 < parm && columnIndex73 < parm && columnIndex74 < parm && columnIndex75 < parm && columnIndex76 < parm && columnIndex77 < parm)
                        {
                            int p = rowIndex * parm + columnIndex;
                            dLine dl = (dLine)dLines[p];

                            int p71 = rowIndex71 * parm + columnIndex71;
                            dLine dl71 = (dLine)dLines[p71];
                            string value71 = dl71.getValue();

                            int p72 = rowIndex72 * parm + columnIndex72;
                            dLine dl72 = (dLine)dLines[p72];
                            string value72 = dl72.getValue();

                            int p73 = rowIndex73 * parm + columnIndex73;
                            dLine dl73 = (dLine)dLines[p73];
                            string value73 = dl73.getValue();

                            int p74 = rowIndex74 * parm + columnIndex74;
                            dLine dl74 = (dLine)dLines[p74];
                            string value74 = dl74.getValue();

                            int p75 = rowIndex75 * parm + columnIndex75;
                            dLine dl75 = (dLine)dLines[p75];
                            string value75 = dl75.getValue();

                            int p76 = rowIndex76 * parm + columnIndex76;
                            dLine dl76 = (dLine)dLines[p76];
                            string value76 = dl76.getValue();

                            int p77 = rowIndex77 * parm + columnIndex77;
                            dLine dl77 = (dLine)dLines[p77];
                            string value77 = dl77.getValue();

                            if (value71.Length > 0 && value72.Length > 0 && value73.Length > 0 && value74.Length > 0 && value75.Length > 0 && value76.Length > 0 && value77.Length > 0)
                            {
                                int pa = (rowIndex - 1) * parm + columnIndex;
                                dLine dla = (dLine)dLines[pa];
                                dla.setBottom(1);

                                int pa71 = rowIndex71 * parm + columnIndex71;
                                dLine dla71 = (dLine)dLines[pa71];
                                dla71.setBottom(1);

                                int pa72 = rowIndex72 * parm + columnIndex72;
                                dLine dla72 = (dLine)dLines[pa72];
                                dla72.setBottom(1);

                                int pa73 = (rowIndex73 - 1) * parm + columnIndex73;
                                dLine dla73 = (dLine)dLines[pa73];
                                dla73.setBottom(1);

                                int pa74 = rowIndex74 * parm + columnIndex74;
                                dLine dla74 = (dLine)dLines[pa74];
                                dla74.setBottom(1);

                                int pa75 = (rowIndex75 - 1) * parm + columnIndex75;
                                dLine dla75 = (dLine)dLines[pa75];
                                dla75.setBottom(1);

                                int pa76 = rowIndex76 * parm + columnIndex76;
                                dLine dla76 = (dLine)dLines[pa76];
                                dla76.setBottom(1);

                                int pa77 = (rowIndex77 - 1) * parm + columnIndex77;
                                dLine dla77 = (dLine)dLines[pa77];
                                dla77.setBottom(1);
                            }
                        }
                        //第八种判断

                        //第九种判断
                        if (rowCount > rowIndex91 && rowCount > rowIndex92 && rowCount > rowIndex93 && rowCount > rowIndex94 && rowCount > rowIndex95 && rowCount > rowIndex96 && rowCount > rowIndex97 &&
                            columnIndex91 < parm && columnIndex92 < parm && columnIndex93 < parm && columnIndex94 < parm && columnIndex95 < parm && columnIndex96 < parm && columnIndex97 < parm)
                        {

                            int p = rowIndex * parm + columnIndex;
                            dLine dl = (dLine)dLines[p];

                            int p91 = rowIndex91 * parm + columnIndex91;
                            dLine dl91 = (dLine)dLines[p91];
                            string value91 = dl91.getValue();

                            int p92 = rowIndex92 * parm + columnIndex92;
                            dLine dl92 = (dLine)dLines[p92];
                            string value92 = dl92.getValue();


                            int p93 = rowIndex93 * parm + columnIndex93;
                            dLine dl93 = (dLine)dLines[p93];
                            string value93 = dl93.getValue();

                            int p94 = rowIndex94 * parm + columnIndex94;
                            dLine dl94 = (dLine)dLines[p94];
                            string value94 = dl94.getValue();

                            int p95 = rowIndex95 * parm + columnIndex95;
                            dLine dl95 = (dLine)dLines[p95];
                            string value95 = dl95.getValue();

                            int p96 = rowIndex96 * parm + columnIndex96;
                            dLine dl96 = (dLine)dLines[p96];
                            string value96 = dl96.getValue();

                            int p97 = rowIndex97 * parm + columnIndex97;
                            dLine dl97 = (dLine)dLines[p97];
                            string value97 = dl97.getValue();

                            if (value91.Length > 0 && value92.Length > 0 && value93.Length > 0 && value94.Length > 0 && value95.Length > 0 && value96.Length > 0 && value97.Length > 0)
                            {
                                int pa = rowIndex * parm + columnIndex;
                                dLine dla = (dLine)dLines[pa];
                                dla.setBottom(1);

                                int pa91 = (rowIndex91 - 1) * parm + columnIndex91;
                                dLine dla91 = (dLine)dLines[pa91];
                                dla91.setBottom(1);

                                int pa92 = rowIndex92 * parm + columnIndex92;
                                dLine dla92 = (dLine)dLines[pa92];
                                dla92.setBottom(1);

                                int pa93 = (rowIndex93 - 1) * parm + columnIndex93;
                                dLine dla93 = (dLine)dLines[pa93];
                                dla93.setBottom(1);

                                int pa94 = rowIndex94 * parm + columnIndex94;
                                dLine dla94 = (dLine)dLines[pa94];
                                dla94.setBottom(1);

                                int pa95 = (rowIndex95 - 1) * parm + columnIndex95;
                                dLine dla95 = (dLine)dLines[pa95];
                                dla95.setBottom(1);

                                int pa96 = (rowIndex96 - 1) * parm + columnIndex96;
                                dLine dla96 = (dLine)dLines[pa96];
                                dla96.setBottom(1);

                                int pa97 = rowIndex97 * parm + columnIndex97;
                                dLine dla97 = (dLine)dLines[pa97];
                                dla97.setBottom(1);
                            }
                        }
                        //第九种判断

                        //第十种判断
                        if (rowCount > rowIndex101 && rowCount > rowIndex102 && rowCount > rowIndex103 && rowCount > rowIndex104 && rowCount > rowIndex105 && rowCount > rowIndex106 && rowCount > rowIndex107 &&
                            columnIndex101 < parm && columnIndex102 < parm && columnIndex103 < parm && columnIndex104 < parm && columnIndex105 < parm && columnIndex106 < parm && columnIndex107 < parm)
                        {
                            int p = rowIndex * parm + columnIndex;
                            dLine dl = (dLine)dLines[p];

                            int p101 = rowIndex101 * parm + columnIndex101;
                            dLine dl101 = (dLine)dLines[p101];
                            string value101 = dl101.getValue();

                            int p102 = rowIndex102 * parm + columnIndex102;
                            dLine dl102 = (dLine)dLines[p102];
                            string value102 = dl102.getValue();

                            int p103 = rowIndex103 * parm + columnIndex103;
                            dLine dl103 = (dLine)dLines[p103];
                            string value103 = dl103.getValue();

                            int p104 = rowIndex104 * parm + columnIndex104;
                            dLine dl104 = (dLine)dLines[p104];
                            string value104 = dl104.getValue();

                            int p105 = rowIndex105 * parm + columnIndex105;
                            dLine dl105 = (dLine)dLines[p105];
                            string value105 = dl105.getValue();

                            int p106 = rowIndex106 * parm + columnIndex106;
                            dLine dl106 = (dLine)dLines[p106];
                            string value106 = dl106.getValue();

                            int p107 = rowIndex107 * parm + columnIndex107;
                            dLine dl107 = (dLine)dLines[p107];
                            string value107 = dl107.getValue();

                            if (value101.Length > 0 && value102.Length > 0 && value103.Length > 0 && value104.Length > 0 && value105.Length > 0 && value106.Length > 0 && value107.Length > 0)
                            {
                                int pa = rowIndex * parm + columnIndex;
                                dLine dla = (dLine)dLines[pa];
                                dla.setBottom(1);

                                int pa101 = (rowIndex101 - 1) * parm + columnIndex101;
                                dLine dla101 = (dLine)dLines[pa101];
                                dla101.setBottom(1);

                                int pa102 = rowIndex102 * parm + columnIndex102;
                                dLine dla102 = (dLine)dLines[pa102];
                                dla102.setBottom(1);

                                int pa103 = (rowIndex103 - 1) * parm + columnIndex103;
                                dLine dla103 = (dLine)dLines[pa103];
                                dla103.setBottom(1);

                                int pa104 = (rowIndex104 - 1) * parm + columnIndex104;
                                dLine dla104 = (dLine)dLines[pa104];
                                dla104.setBottom(1);

                                int pa105 = rowIndex105 * parm + columnIndex105;
                                dLine dla105 = (dLine)dLines[pa105];
                                dla105.setBottom(1);

                                int pa106 = (rowIndex106 - 1) * parm + columnIndex106;
                                dLine dla106 = (dLine)dLines[pa106];
                                dla106.setBottom(1);

                                int pa107 = rowIndex107 * parm + columnIndex107;
                                dLine dla107 = (dLine)dLines[pa107];
                                dla107.setBottom(1);
                            }
                        }
                        //第十种判断

                        //第十一种判断
                        if (rowCount > rowIndex201 && rowCount > rowIndex202 && rowCount > rowIndex203 && rowCount > rowIndex204 && rowCount > rowIndex205 && rowCount > rowIndex206 && rowCount > rowIndex207 &&
                            columnIndex201 < parm && columnIndex202 < parm && columnIndex203 < parm && columnIndex204 < parm && columnIndex205 < parm && columnIndex206 < parm && columnIndex207 < parm)
                        {
                            int p = rowIndex * parm + columnIndex;
                            dLine dl = (dLine)dLines[p];

                            int p201 = rowIndex201 * parm + columnIndex201;
                            dLine dl201 = (dLine)dLines[p201];
                            string value201 = dl201.getValue();

                            int p202 = rowIndex202 * parm + columnIndex202;
                            dLine dl202 = (dLine)dLines[p202];
                            string value202 = dl202.getValue();


                            int p203 = rowIndex203 * parm + columnIndex203;
                            dLine dl203 = (dLine)dLines[p203];
                            string value203 = dl203.getValue();

                            int p204 = rowIndex204 * parm + columnIndex204;
                            dLine dl204 = (dLine)dLines[p204];
                            string value204 = dl204.getValue();

                            int p205 = rowIndex205 * parm + columnIndex205;
                            dLine dl205 = (dLine)dLines[p205];
                            string value205 = dl205.getValue();

                            int p206 = rowIndex206 * parm + columnIndex206;
                            dLine dl206 = (dLine)dLines[p206];
                            string value206 = dl206.getValue();

                            int p207 = rowIndex207 * parm + columnIndex207;
                            dLine dl207 = (dLine)dLines[p207];
                            string value207 = dl207.getValue();

                            if (value201.Length > 0 && value202.Length > 0 && value203.Length > 0 && value204.Length > 0 && value205.Length > 0 && value206.Length > 0 && value207.Length > 0)
                            {
                                int pa = rowIndex * parm + columnIndex;
                                dLine dla = (dLine)dLines[pa];
                                dla.setBottom(1);

                                int pa201 = (rowIndex201 - 1) * parm + columnIndex201;
                                dLine dla201 = (dLine)dLines[pa201];
                                dla201.setBottom(1);

                                int pa202 = (rowIndex202 - 1) * parm + columnIndex202;
                                dLine dla202 = (dLine)dLines[pa202];
                                dla202.setBottom(1);

                                int pa203 = rowIndex203 * parm + columnIndex203;
                                dLine dla203 = (dLine)dLines[pa203];
                                dla203.setBottom(1);

                                int pa204 = (rowIndex204 - 1) * parm + columnIndex204;
                                dLine dla204 = (dLine)dLines[pa204];
                                dla204.setBottom(1);

                                int pa205 = rowIndex205 * parm + columnIndex205;
                                dLine dla205 = (dLine)dLines[pa205];
                                dla205.setBottom(1);

                                int pa206 = (rowIndex206 - 1) * parm + columnIndex206;
                                dLine dla206 = (dLine)dLines[pa206];
                                dla206.setBottom(1);

                                int pa207 = rowIndex207 * parm + columnIndex207;
                                dLine dla207 = (dLine)dLines[pa207];
                                dla207.setBottom(1);
                            }
                        }
                        //第十一种判断

                        //第十二种判断
                        if (rowCount > rowIndex301 && rowCount > rowIndex302 && rowCount > rowIndex303 && rowCount > rowIndex304 && rowCount > rowIndex305 && rowCount > rowIndex306 && rowCount > rowIndex307 &&
                            columnIndex301 < parm && columnIndex302 < parm && columnIndex303 < parm && columnIndex304 < parm && columnIndex305 < parm && columnIndex306 < parm && columnIndex307 < parm)
                        {
                            int p = rowIndex * parm + columnIndex;
                            dLine dl = (dLine)dLines[p];

                            int p301 = rowIndex301 * parm + columnIndex301;
                            dLine dl301 = (dLine)dLines[p301];
                            string value301 = dl301.getValue();

                            int p302 = rowIndex302 * parm + columnIndex302;
                            dLine dl302 = (dLine)dLines[p302];
                            string value302 = dl302.getValue();

                            int p303 = rowIndex303 * parm + columnIndex303;
                            dLine dl303 = (dLine)dLines[p303];
                            string value303 = dl303.getValue();

                            int p304 = rowIndex304 * parm + columnIndex304;
                            dLine dl304 = (dLine)dLines[p304];
                            string value304 = dl304.getValue();

                            int p305 = rowIndex305 * parm + columnIndex305;
                            dLine dl305 = (dLine)dLines[p305];
                            string value305 = dl305.getValue();

                            int p306 = rowIndex306 * parm + columnIndex306;
                            dLine dl306 = (dLine)dLines[p306];
                            string value306 = dl306.getValue();

                            int p307 = rowIndex307 * parm + columnIndex307;
                            dLine dl307 = (dLine)dLines[p307];
                            string value307 = dl307.getValue();

                            if (value301.Length > 0 && value302.Length > 0 && value303.Length > 0 && value304.Length > 0 && value305.Length > 0 && value306.Length > 0 && value307.Length > 0)
                            {
                                int pa = rowIndex * parm + columnIndex;
                                dLine dla = (dLine)dLines[pa];
                                dla.setBottom(1);

                                int pa301 = (rowIndex301 - 1) * parm + columnIndex301;
                                dLine dla301 = (dLine)dLines[pa301];
                                dla301.setBottom(1);

                                int pa302 = (rowIndex302 - 1) * parm + columnIndex302;
                                dLine dla302 = (dLine)dLines[pa302];
                                dla302.setBottom(1);

                                int pa303 = rowIndex303 * parm + columnIndex303;
                                dLine dla303 = (dLine)dLines[pa303];
                                dla303.setBottom(1);

                                int pa304 = rowIndex304 * parm + columnIndex304;
                                dLine dla304 = (dLine)dLines[pa304];
                                dla304.setBottom(1);

                                int pa305 = (rowIndex305 - 1) * parm + columnIndex305;
                                dLine dla305 = (dLine)dLines[pa305];
                                dla305.setBottom(1);

                                int pa306 = (rowIndex306 - 1) * parm + columnIndex306;
                                dLine dla306 = (dLine)dLines[pa306];
                                dla306.setBottom(1);

                                int pa307 = rowIndex307 * parm + columnIndex307;
                                dLine dla307 = (dLine)dLines[pa307];
                                dla307.setBottom(1);
                            }
                        }
                        //第十二种判断

                        //第十三种判断
                        if (rowIndex401 >= 0 && rowIndex402 >= 0 && rowIndex403 >= 0 && rowIndex404 >= 0 && rowIndex405 >= 0 && rowIndex406 >= 0 && rowIndex407 >= 0 &&
                            rowCount > rowIndex401 && rowCount > rowIndex402 && rowCount > rowIndex403 && rowCount > rowIndex404 && rowCount > rowIndex405 && rowCount > rowIndex406 && rowCount > rowIndex407 &&
                            columnIndex401 < parm && columnIndex402 < parm && columnIndex403 < parm && columnIndex404 < parm && columnIndex405 < parm && columnIndex406 < parm && columnIndex407 < parm)
                        {
                            int p = rowIndex * parm + columnIndex;
                            dLine dl = (dLine)dLines[p];

                            int p401 = rowIndex401 * parm + columnIndex401;
                            dLine dl401 = (dLine)dLines[p401];
                            string value401 = dl401.getValue();

                            int p402 = rowIndex402 * parm + columnIndex402;
                            dLine dl402 = (dLine)dLines[p402];
                            string value402 = dl402.getValue();

                            int p403 = rowIndex403 * parm + columnIndex403;
                            dLine dl403 = (dLine)dLines[p403];
                            string value403 = dl403.getValue();

                            int p404 = rowIndex404 * parm + columnIndex404;
                            dLine dl404 = (dLine)dLines[p404];
                            string value404 = dl404.getValue();

                            int p405 = rowIndex405 * parm + columnIndex405;
                            dLine dl405 = (dLine)dLines[p405];
                            string value405 = dl405.getValue();

                            int p406 = rowIndex406 * parm + columnIndex406;
                            dLine dl406 = (dLine)dLines[p406];
                            string value406 = dl406.getValue();

                            int p407 = rowIndex407 * parm + columnIndex407;
                            dLine dl407 = (dLine)dLines[p407];
                            string value407 = dl407.getValue();

                            if (value401.Length > 0 && value402.Length > 0 && value403.Length > 0 && value404.Length > 0 && value405.Length > 0 && value406.Length > 0 && value407.Length > 0)
                            {
                                int pa = (rowIndex - 1) * parm + columnIndex;
                                dLine dla = (dLine)dLines[pa];
                                dla.setBottom(1);

                                int pa401 = rowIndex401 * parm + columnIndex401;
                                dLine dla401 = (dLine)dLines[pa401];
                                dla401.setBottom(1);

                                int pa402 = (rowIndex402 - 1) * parm + columnIndex402;
                                dLine dla402 = (dLine)dLines[pa402];
                                dla402.setBottom(1);

                                int pa403 = rowIndex403 * parm + columnIndex403;
                                dLine dla403 = (dLine)dLines[pa403];
                                dla403.setBottom(1);

                                int pa404 = (rowIndex404 - 1) * parm + columnIndex404;
                                dLine dla404 = (dLine)dLines[pa404];
                                dla404.setBottom(1);

                                int pa405 = rowIndex405 * parm + columnIndex405;
                                dLine dla405 = (dLine)dLines[pa405];
                                dla405.setBottom(1);

                                int pa406 = (rowIndex406 - 1) * parm + columnIndex406;
                                dLine dla406 = (dLine)dLines[pa406];
                                dla406.setBottom(1);

                                int pa407 = rowIndex407 * parm + columnIndex407;
                                dLine dla407 = (dLine)dLines[pa407];
                                dla407.setBottom(1);
                            }
                        }
                        //第十三种判断

                        //第十四种判断
                        if (rowIndex501 >= 0 && rowIndex502 >= 0 && rowIndex503 >= 0 && rowIndex504 >= 0 && rowIndex505 >= 0 && rowIndex506 >= 0 && rowIndex507 >= 0 &&
                            rowCount > rowIndex501 && rowCount > rowIndex502 && rowCount > rowIndex503 && rowCount > rowIndex504 && rowCount > rowIndex505 && rowCount > rowIndex506 && rowCount > rowIndex507 &&
                            columnIndex501 < parm && columnIndex502 < parm && columnIndex503 < parm && columnIndex504 < parm && columnIndex505 < parm && columnIndex506 < parm && columnIndex507 < parm)
                        {
                            int p = rowIndex * parm + columnIndex;
                            dLine dl = (dLine)dLines[p];

                            int p501 = rowIndex501 * parm + columnIndex501;
                            dLine dl501 = (dLine)dLines[p501];
                            string value501 = dl501.getValue();

                            int p502 = rowIndex502 * parm + columnIndex502;
                            dLine dl502 = (dLine)dLines[p502];
                            string value502 = dl502.getValue();

                            int p503 = rowIndex503 * parm + columnIndex503;
                            dLine dl503 = (dLine)dLines[p503];
                            string value503 = dl503.getValue();

                            int p504 = rowIndex504 * parm + columnIndex504;
                            dLine dl504 = (dLine)dLines[p504];
                            string value504 = dl504.getValue();

                            int p505 = rowIndex505 * parm + columnIndex505;
                            dLine dl505 = (dLine)dLines[p505];
                            string value505 = dl505.getValue();

                            int p506 = rowIndex506 * parm + columnIndex506;
                            dLine dl506 = (dLine)dLines[p506];
                            string value506 = dl506.getValue();

                            int p507 = rowIndex507 * parm + columnIndex507;
                            dLine dl507 = (dLine)dLines[p507];
                            string value507 = dl507.getValue();

                            if (value501.Length > 0 && value502.Length > 0 && value503.Length > 0 && value504.Length > 0 && value505.Length > 0 && value506.Length > 0 && value507.Length > 0)
                            {
                                int pa = (rowIndex - 1) * parm + columnIndex;
                                dLine dla = (dLine)dLines[pa];
                                dla.setBottom(1);

                                int pa501 = rowIndex501 * parm + columnIndex501;
                                dLine dla501 = (dLine)dLines[pa501];
                                dla501.setBottom(1);

                                int pa502 = (rowIndex502 - 1) * parm + columnIndex502;
                                dLine dla502 = (dLine)dLines[pa502];
                                dla502.setBottom(1);

                                int pa503 = rowIndex503 * parm + columnIndex503;
                                dLine dla503 = (dLine)dLines[pa503];
                                dla503.setBottom(1);

                                int pa504 = rowIndex504 * parm + columnIndex504;
                                dLine dla504 = (dLine)dLines[pa504];
                                dla504.setBottom(1);

                                int pa505 = (rowIndex505 - 1) * parm + columnIndex505;
                                dLine dla505 = (dLine)dLines[pa505];
                                dla505.setBottom(1);

                                int pa506 = (rowIndex506 - 1) * parm + columnIndex506;
                                dLine dla506 = (dLine)dLines[pa506];
                                dla506.setBottom(1);

                                int pa507 = rowIndex507 * parm + columnIndex507;
                                dLine dla507 = (dLine)dLines[pa507];
                                dla507.setBottom(1);
                            }
                        }
                        //第十四种判断

                        //第十五种判断
                        if (rowIndex601 >= 0 && rowIndex602 >= 0 && rowIndex603 >= 0 && rowIndex604 >= 0 && rowIndex605 >= 0 && rowIndex606 >= 0 && rowIndex607 >= 0 &&
                            rowCount > rowIndex601 && rowCount > rowIndex602 && rowCount > rowIndex603 && rowCount > rowIndex604 && rowCount > rowIndex605 && rowCount > rowIndex606 && rowCount > rowIndex607 &&
                            columnIndex601 < parm && columnIndex602 < parm && columnIndex603 < parm && columnIndex604 < parm && columnIndex605 < parm && columnIndex606 < parm && columnIndex607 < parm)
                        {
                            int p = rowIndex * parm + columnIndex;
                            dLine dl = (dLine)dLines[p];

                            int p601 = rowIndex601 * parm + columnIndex601;
                            dLine dl601 = (dLine)dLines[p601];
                            string value601 = dl601.getValue();

                            int p602 = rowIndex602 * parm + columnIndex602;
                            dLine dl602 = (dLine)dLines[p602];
                            string value602 = dl602.getValue();

                            int p603 = rowIndex603 * parm + columnIndex603;
                            dLine dl603 = (dLine)dLines[p603];
                            string value603 = dl603.getValue();

                            int p604 = rowIndex604 * parm + columnIndex604;
                            dLine dl604 = (dLine)dLines[p604];
                            string value604 = dl604.getValue();

                            int p605 = rowIndex605 * parm + columnIndex605;
                            dLine dl605 = (dLine)dLines[p605];
                            string value605 = dl605.getValue();

                            int p606 = rowIndex606 * parm + columnIndex606;
                            dLine dl606 = (dLine)dLines[p606];
                            string value606 = dl606.getValue();

                            int p607 = rowIndex607 * parm + columnIndex607;
                            dLine dl607 = (dLine)dLines[p607];
                            string value607 = dl607.getValue();

                            if (value601.Length > 0 && value602.Length > 0 && value603.Length > 0 && value604.Length > 0 && value605.Length > 0 && value606.Length > 0 && value607.Length > 0)
                            {
                                int pa = (rowIndex - 1) * parm + columnIndex;
                                dLine dla = (dLine)dLines[pa];
                                dla.setBottom(1);

                                int pa601 = rowIndex601 * parm + columnIndex601;
                                dLine dla601 = (dLine)dLines[pa601];
                                dla601.setBottom(1);

                                int pa602 = rowIndex602 * parm + columnIndex602;
                                dLine dla602 = (dLine)dLines[pa602];
                                dla602.setBottom(1);

                                int pa603 = (rowIndex603 - 1) * parm + columnIndex603;
                                dLine dla603 = (dLine)dLines[pa603];
                                dla603.setBottom(1);

                                int pa604 = (rowIndex604 - 1) * parm + columnIndex604;
                                dLine dla604 = (dLine)dLines[pa604];
                                dla604.setBottom(1);

                                int pa605 = rowIndex605 * parm + columnIndex605;
                                dLine dla605 = (dLine)dLines[pa605];
                                dla605.setBottom(1);

                                int pa606 = (rowIndex606 - 1) * parm + columnIndex606;
                                dLine dla606 = (dLine)dLines[pa606];
                                dla606.setBottom(1);

                                int pa607 = rowIndex607 * parm + columnIndex607;
                                dLine dla607 = (dLine)dLines[pa607];
                                dla607.setBottom(1);
                            }
                        }
                        //第十五种判断

                        //第十六种判断
                        if (rowIndex701 >= 0 && rowIndex702 >= 0 && rowIndex703 >= 0 && rowIndex704 >= 0 && rowIndex705 >= 0 && rowIndex706 >= 0 && rowIndex707 >= 0 &&
                            rowCount > rowIndex701 && rowCount > rowIndex702 && rowCount > rowIndex703 && rowCount > rowIndex704 && rowCount > rowIndex705 && rowCount > rowIndex706 && rowCount > rowIndex707 &&
                            columnIndex701 < parm && columnIndex702 < parm && columnIndex703 < parm && columnIndex704 < parm && columnIndex705 < parm && columnIndex706 < parm && columnIndex707 < parm)
                        {
                            int p = rowIndex * parm + columnIndex;
                            dLine dl = (dLine)dLines[p];

                            int p701 = rowIndex701 * parm + columnIndex701;
                            dLine dl701 = (dLine)dLines[p701];
                            string value701 = dl701.getValue();

                            int p702 = rowIndex702 * parm + columnIndex702;
                            dLine dl702 = (dLine)dLines[p702];
                            string value702 = dl702.getValue();

                            int p703 = rowIndex703 * parm + columnIndex703;
                            dLine dl703 = (dLine)dLines[p703];
                            string value703 = dl703.getValue();

                            int p704 = rowIndex704 * parm + columnIndex704;
                            dLine dl704 = (dLine)dLines[p704];
                            string value704 = dl704.getValue();

                            int p705 = rowIndex705 * parm + columnIndex705;
                            dLine dl705 = (dLine)dLines[p705];
                            string value705 = dl705.getValue();

                            int p706 = rowIndex706 * parm + columnIndex706;
                            dLine dl706 = (dLine)dLines[p706];
                            string value706 = dl706.getValue();

                            int p707 = rowIndex707 * parm + columnIndex707;
                            dLine dl707 = (dLine)dLines[p707];
                            string value707 = dl707.getValue();

                            if (value701.Length > 0 && value702.Length > 0 && value703.Length > 0 && value704.Length > 0 && value705.Length > 0 && value706.Length > 0 && value707.Length > 0)
                            {
                                int pa = (rowIndex - 1) * parm + columnIndex;
                                dLine dla = (dLine)dLines[pa];
                                dla.setBottom(1);

                                int pa701 = rowIndex701 * parm + columnIndex701;
                                dLine dla701 = (dLine)dLines[pa701];
                                dla701.setBottom(1);

                                int pa702 = rowIndex702 * parm + columnIndex702;
                                dLine dla702 = (dLine)dLines[pa702];
                                dla702.setBottom(1);

                                int pa703 = (rowIndex703 - 1) * parm + columnIndex703;
                                dLine dla703 = (dLine)dLines[pa703];
                                dla703.setBottom(1);

                                int pa704 = rowIndex704 * parm + columnIndex704;
                                dLine dla704 = (dLine)dLines[pa704];
                                dla704.setBottom(1);

                                int pa705 = (rowIndex705 - 1) * parm + columnIndex705;
                                dLine dla705 = (dLine)dLines[pa705];
                                dla705.setBottom(1);

                                int pa706 = (rowIndex706 - 1) * parm + columnIndex706;
                                dLine dla706 = (dLine)dLines[pa706];
                                dla706.setBottom(1);

                                int pa707 = rowIndex707 * parm + columnIndex707;
                                dLine dla707 = (dLine)dLines[pa707];
                                dla707.setBottom(1);
                            }
                        }
                        //第十六种判断
                    }
                }
            }
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
                                //int[] number = getNumber(rIndex, key, al);

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
                                if (diff >= 10 && diff <= 16 && columnsDraw.Contains(cIndex))
                                {
                                    dline.setBackGround(1);//绿色
                                }

                                if (diff >= 5 && diff <= 9 && rIndex == bb && cIndex == maxDrawColumnNumber + 1)
                                {
                                    if (!dLinesColumns.Contains(cIndex))
                                    {
                                        dLinesColumns.Add(cIndex);
                                    }
                                    dline.setInterval(diff);
                                    gridDots.Add(dline);
                                }

                                if (diff >= 17 && columnsDraw.Contains(cIndex))
                                {
                                    dline.setBackGround(2);//蓝色
                                }

                                if (diff >= 5 && rIndex == bb && cIndex == maxDrawColumnNumber + 1)
                                {
                                    //dline.setInterval(diff);
                                    //gridDots.Add(dline);
                                }



                            }
                        }
                    }
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
                if (interval == 5)
                {
                    dline.setBackGround(2);
                    //break;
                }
                if (interval == 6)
                {
                    dline.setBackGround(1);
                    //break;
                }
                if (interval == 7)
                {
                    dline.setBackGround(3);
                    // break;
                }
                if (interval == 8)
                {
                    dline.setBackGround(4);
                    //break;
                }
                if (interval == 9)
                {
                    dline.setBackGround(5);
                    // break;
                }
                //}
                //}
            }
        }

        private void gdv1LoadData()
        {
            for (int i = 0; i < rowCountOri; i++)
            {
                this.dataGridView1.Rows.Add();
                //int row = this.dataGridView1.Rows.Count - 2;
                //for (int j = 0; j < maxColum + 1; j++)
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
                    string content = getDataOpt(i, j);
                    if (content.Length > 0)
                    {
                        this.dataGridView2.Rows[i].Cells[j].Value = content;
                    }

                }
            }
            loadDataCount += readyLoadCount;
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
            if (size > 0)
            {
                int step = 100 / size;
                step = (step == 0) ? 1 : step;
                for (int i = 0; i < size; i++)
                {
                    gdv2LoadData(20);
                    int value = this.progressBar1.Value;
                    if (value < 100 - step)
                    {
                        this.progressBar1.Value = value + step;
                    }

                }
                this.progressBar1.Visible = false;
                for (int i = 0; i < 5; i++)
                {
                    this.dataGridView2.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                }
            }
            this.progressBar1.Visible = false;
            if (this.dataGridView2.Rows.Count > 10)
            {
                this.dataGridView2.FirstDisplayedScrollingRowIndex = this.dataGridView2.Rows[this.dataGridView2.RowCount - 1].Index;
            }
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

        private int[] getNumber(int rIndex, string key, ArrayList al)
        {
            int[] s = new int[2];
            if (dLinesDrawRow.Count == 0)
            {
                s[0] = 0;
                s[1] = 0;
            }
            else
            {

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
            }
            return s;
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

        private String getDataOpt(int rowIndex, int columnIndex)
        {
            String value = "";
            for (int i = prePosition; i < dLines.Count; i++)
            {
                dLine dline = (dLine)dLines[i];
                if (dline.getCloumnIndex() == columnIndex && dline.getRowIndex() == rowIndex)
                {
                    value = dline.getValue();
                    prePosition = i + 1;
                    return value;
                }
            }
            return value;
        }


        private void dataGridView2_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            int columnIndex = e.ColumnIndex;
            int rowIndex = e.RowIndex;

            if (columnIndex <= columnSize && dLinesColumns.Contains(columnIndex) && columnIndex >= 0 && rowIndex >= 0 && rowCount > rowIndex)
            {
                dLine line = getLine(columnIndex, rowIndex);
                if (line.getBottom() == 1)
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
                            else if (line.getBackGround() == 5)
                            {
                                e.Graphics.FillRectangle(new SolidBrush(Color.Yellow), e.CellBounds);//黄色
                            }
                            else
                            {
                                e.Graphics.FillRectangle(backColorBrush, e.CellBounds);//默认
                            }

                            // Draw the grid lines (only the right and bottom lines;
                            // DataGridView takes care of the others).

                            e.Graphics.DrawLine(gridRedLinePen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);//bootom

                            // e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 1, e.CellBounds.Top, e.CellBounds.Right - 1, e.CellBounds.Bottom);//right

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
                            else if (line.getBackGround() == 5)
                            {
                                e.Graphics.FillRectangle(new SolidBrush(Color.Yellow), e.CellBounds);//黄色
                            }
                            else
                            {
                                e.Graphics.FillRectangle(backColorBrush, e.CellBounds);//默认
                            }

                            if (e.Value != null)
                            {
                                e.Graphics.DrawString((String)e.Value, e.CellStyle.Font, null == line || line.getColor() == Color.Empty ? Brushes.Black : new SolidBrush(line.getColor()), e.CellBounds.X + 2, e.CellBounds.Y + 2, StringFormat.GenericDefault);
                            }
                            e.Handled = true;

                        }
                    }
                    //结束重绘背景
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
    
    
    
    
    
    
    }
}
