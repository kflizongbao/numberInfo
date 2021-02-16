using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace drawDong
{
    public partial class Form3 : Form
    {
        public string test = string.Empty;
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            test = this.textBox1.Text.Trim();
            this.DialogResult = DialogResult.OK;
        }
    }
}
