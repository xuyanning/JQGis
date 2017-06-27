using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;

namespace RailwayGIS
{
    public partial class LabelRoll : Label
    {
        //public string msg="最新消息";
        Brush brush;
        PointF startPoint;
        SizeF s = new SizeF();
        //int times;
        public delegate void LableMsgRepeated();
        public event LableMsgRepeated MsgRepeated = null;

        public LabelRoll()
        {
            InitializeComponent();
            brush = new SolidBrush(this.ForeColor);
        }

        public LabelRoll(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            brush = new SolidBrush(this.ForeColor);
            startPoint = new PointF(this.ClientRectangle.Width/2, 0);
           
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            s = g.MeasureString(Text, Font);//测量文字长度 
            
             
            g.Clear(this.BackColor);//清除背景 cl            
            
            g.DrawString(this.Text,this.Font,brush, startPoint);
            startPoint.X -= 15;
            if (s.Width + startPoint.X < this.ClientRectangle.Width / 2) { 
                startPoint.X = this.ClientRectangle.Width /2;
                MsgRepeated?.Invoke();
            }
            //base.OnPaint(e);
        }
    }
}
