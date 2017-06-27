using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ModelInfo;
using ModelInfo.Helper;
using RailwayGIS.TEProject;

namespace RailwayGIS
{
    public partial class MessageForm : Form
    {
        CRailwayProject mProject;
        ImageList mls1 = new ImageList();   //桥墩扫描码
        public int mTimeInterval = 1000;
        string mImageUrl;
        public ShowFormStatus ws = ShowFormStatus.eReadyForNext;
        //定义事件
        //定义委托
        //public delegate void DetailStopping();
        //public event DetailStopping DetailIsStopping = null;

        //public delegate void NextDetailStarting();
        //public event NextDetailStarting NextPanelStarting = null;

        Font defaultFont = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));

        public MessageForm()
        {
            InitializeComponent();
            // 三倍速10秒，五速1.1秒，单速 30秒
            mTimeInterval =(int) (Math.Pow((6 - CGisDataSettings.AppSpeed), 2) / 9 * 10000);
#if DEBUG
            mTimeInterval = 1000;
#endif 
            timer1.Interval = mTimeInterval;
            timer1.Stop();

            mls1.ImageSize = new Size(120, 120);
            //mls2.ImageSize = new Size(144, 144);

            //((System.ComponentModel.ISupportInitialize)(this.advPropertyGrid1)).BeginInit();
            this.SuspendLayout();

            this.webBrowser1.Dock = DockStyle.Fill;
            webBrowser1.Hide();
            this.panel1.Dock = DockStyle.Fill;

            this.ResumeLayout(false);

           
        }

        public void setProject(CRailwayProject rp)
        {
            if (rp == null || rp == this.propertyGridEx1.Tag || ! (rp is CRailwayProject))
                return;
            mProject = rp;

            this.Text = rp.ProjectName;

            if (!string.IsNullOrEmpty(rp.mPhotoUrl))
            {
                mImageUrl = @"http://" + CGisDataSettings.gCurrentProject.projectUrl + rp.mPhotoUrl;
                webBrowser1.Navigate(mImageUrl);
            }
            else
            {
                mImageUrl = null;
            }
            
            propertyGridEx1.Tag = rp;
            propertyGridEx1.SelectedObject = rp;
            if (rp.FXProgress.Count >= 3)
            {
                showOneFX(propertyGridEx2, rp.FXProgress[rp.selectedFXid[0]], 0);
                showOneFX(propertyGridEx3, rp.FXProgress[rp.selectedFXid[1]], 1);
                showOneFX(propertyGridEx4, rp.FXProgress[rp.selectedFXid[2]], 2);
            }
            initPrjPier(rp);
        }

#region 工点信息填充的辅助方法
        private void showOneFX(PropertyGridEx pg, CFXProj fx, int fxID)
        {
            pg.SelectedObject = fx;
            chart1.Series[fxID].Points.Clear();
            for (int j = 0; j < fx.strDate.Count; j++)
                chart1.Series[fxID].Points.AddXY(DateTime.ParseExact(fx.strDate[j], "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture),
            Math.Round(fx.doneAmount[j] / fx.TotalAmount * 100, 1));

        }
        private void initPrjPier(CRailwayProject rp)
        {
            mls1.Images.Clear();
            listView1.Items.Clear();
            if (rp is CContBeam)
            {
                int i = 0;
                CContBeam cb = rp as CContBeam;

                foreach (CRailwayPier p in cb.mPierList)
                {
                    mls1.Images.Add(CGeneralHelpers.generate2DCode(@"http://jqmis.cn/S/" + p.mSerialNo));
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = p.DWName + "#墩";
                    lvi.Font = defaultFont;
                    lvi.ImageIndex = i++;
                    listView1.Items.Add(lvi);
                }
                listView1.View = View.LargeIcon;

                listView1.LargeImageList = mls1;
            }
            listView1.Size = new Size(175 * listView1.Items.Count, 200);
            listView1.Location = new Point((panel1.Width - listView1.Width) / 2, listView1.Location.Y);
        }
#endregion

        private void MessageForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible && mProject != null)
            {
                //开启显示面板，如果有显示内容，开启计时，否则，发送展示结束消息;
                //NextPanelStarting?.Invoke();
                panel1.Show();
                this.webBrowser1.Hide();
                timer1.Interval = mTimeInterval;
                timer1.Start();
                

            }
            else
            {
                timer1.Stop();
            }
        }

        public void preShow()
        {
            timer1.Interval = mTimeInterval;
            timer1.Start();
            ws = ShowFormStatus.eStarting;
        }

        public void interruptShow()
        {
            timer1.Stop();
            ws = ShowFormStatus.eReadyForNext;
            this.Hide();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                // 利用panel显示工程进度信息
                this.Show();
            }
            else if (string.IsNullOrEmpty(mImageUrl) || webBrowser1.Visible)
            {
                // 如果没有图像或者已经显示图像，隐藏当前窗口
                this.Hide();
                ws = ShowFormStatus.eClosing;
                //DetailIsStopping?.Invoke();
            }
            else
            {
                //显示图像
                //NextPanelStarting?.Invoke();
                
                webBrowser1.Show();
                panel1.Hide();
                timer1.Interval = mTimeInterval;
                timer1.Start();
            }
        }
    }
}
