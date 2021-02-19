using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace drawDong
{
    public static class GlobalVariables
    {
        public static string firPath = "F:" + @"\" + "vsWork" + @"\" + "drawDongv1" + @"\" + "drawDong" + @"\" + "files";

        public static int fileNumber = 6;

        public static int columnsSize = 65;
        public static int columnsSetSize = 450;

        public static string configPath = GlobalVariables.firPath + @"\" + "config.ini";
        public static string infoPath = GlobalVariables.firPath + @"\" + "info.ini";
        public static string confPath = GlobalVariables.firPath + @"\" + "conf.ini";

        public static string xuanxiangPath = "F:" + @"\" + "vsWork" + @"\" + "drawDongv1" + @"\" + "drawDong" + @"\" + "xuanxiang";

        public static Boolean disa()
        {
            string  d = DateTime.Now.ToString();
            string dt = "2020-11-02 00:00:00";
            string dt1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); 
            TimeSpan ts = DateTime.Parse(dt1) - DateTime.Parse(dt);
            if (ts.Days > 60)
            {
                return true;
            }
            return false;
        }
    }


    public class dLine 
    {
        private String value;
        private int cloumnIndex;
        private int rowIndex;
        private Boolean isDraw;
        private int top = 0;  //0 not Draw 1 Draw  
        private int left = 0; //0 not Draw 1 Draw
        private int bottom = 0; //0 not Draw 1 Draw
        private int right = 0; //0 not Draw 1 Draw
        private int back = 0;
        private int backGround = 0; //0 not draw 1 蓝色 2 red
        private int interval = 0;
        private int linesIndex;
        private int oriIndex;

        public void setLinesIndex(int linesIndex)
        {
            this.linesIndex = linesIndex;
        }

        public int getLinesIndex()
        {
            return this.linesIndex;
        }

        public void setOriIndex(int oriIndex)
        {
            this.oriIndex = oriIndex;
        }

        public int getOriIndex()
        {
            return this.oriIndex;
        }

        public void setValue(string value)
        {
            this.value = value;
        }

        public string getValue()
        {
            return this.value;
        }

        public void setCloumnIndex(int cloumnIndex)
        {
            this.cloumnIndex = cloumnIndex;
        }

        public int getCloumnIndex()
        {
            return this.cloumnIndex;
        }

        public void setRowIndex(int rowIndex)
        {
            this.rowIndex = rowIndex;
        }

        public int getRowIndex()
        {
            return this.rowIndex;
        }

        public void setIsDraw(Boolean isDraw)
        {
            this.isDraw = isDraw;
        }

        public Boolean getIsDraw()
        {
            return this.isDraw;
        }

        public void setTop(int top)
        {
            this.top = top;
        }

        public int getTop() 
        {
            return this.top;
        }

        public void setRight(int right)
        {
            this.right = right;
        }

        public int getRight()
        {
            return this.right;
        }

        public void setBottom(int bottom)
        {
            this.bottom = bottom;
        }

        public int getBottom()
        {
            return this.bottom;
        }

        public void setLeft(int left)
        {
            this.left = left;
        }

        public int getLeft()
        {
            return this.left;
        }

        public void setBack(int back)
        {
            this.back = back;
        }

        public int getBack()
        {
            return this.back;
        }

        public void setBackGround(int backGround)
        {
            this.backGround = backGround;
        }

        public int getBackGround()
        {
            return this.backGround;
        }

        public void setInterval(int interval)
        {
            this.interval = interval;
        }

        public int getInterval()
        {
            return this.interval;
        }

    }


}
