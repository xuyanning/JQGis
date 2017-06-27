using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RailwayGIS
{
    public partial class PanelInfo : UserControl
    {
        public int mtotalProj=0;
        public int mcurrentProj=0; 
        public string mdkCode="";
        public string mnextProj="";
        public double mdistance=0;
        public string mconsLog="";
        public PanelInfo()
        {
            InitializeComponent();
            updatePanel();
        }
        public void updatePanel()
        {
            labelNavTotal.Text = "巡航完成(" + mtotalProj + "):" + mcurrentProj;
            labelMileage.Text = "当前里程：" + mdkCode;
            labelNextProj.Text = "下一工点："+ mnextProj;
            labelDistance.Text = "距离："+mdistance;
            labelConsLog.Text = "附近5公里实名情况:"+mconsLog;
        }
    }
}
