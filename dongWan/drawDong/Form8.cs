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
    public partial class Form8 : Form
    {
        private string fileName;
        private ArrayList dLines = new ArrayList();
        private int rowCount;

        public Form8()
        {
            InitializeComponent();
        }

        public Form8(string name)
        {
            fileName = name;
            InitializeComponent();
        }

        private void Form8_Load(object sender, EventArgs e)
        {
            dLines.Clear();
            System.IO.StreamReader sr = new System.IO.StreamReader(GlobalVariables.xuanxiangPath + @"\" + fileName + ".txt");
            while (!sr.EndOfStream)
            {
                string[] items = sr.ReadLine().Split(',');
                for (int i = 0; i < items.Length; i++)
                {
                    string value = items[i].Equals("-1") ? "" : items[i];
                    if (value.Trim().Length > 0)
                    {
                        dLines.Add(value);
                    }
                }

            }
            sr.Close();

            rowCount = (dLines.Count % 6 > 0) ? dLines.Count / 6 + 1 : dLines.Count / 6;

            for (int i = 0; i< rowCount;i++ )
            {
                this.dataGridView1.Rows.Add("","","","","","");
            }

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    int index = i * 6 + j;
                    if (index < dLines.Count)
                    {
                        this.dataGridView1.Rows[i].Cells[j].Value = dLines[index].ToString();
                    }
                }
            }
        }


    }
}
