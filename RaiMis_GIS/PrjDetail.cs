using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

using ModelInfo;
using ModelInfo.Helper;
using DevComponents.DotNetBar;

namespace RailwayGIS
{
    public partial class PrjDetail : UserControl
    {
        CRailwayScene mRWScene;
        ImageList mls1 = new ImageList();   //桥墩扫描码
        string mImageUrl = null;
        public int timeSpan = 10000;
        //ImageList mls2 = new ImageList();  // 影像资料

        //定义事件
        //定义委托
        public delegate void DetailStopping();
        public event DetailStopping DetailIsStopping = null;

        public delegate void NextDetailStarting();
        public event NextDetailStarting NextPanelStarting = null;

        private int panelStatus = 0;
        //private bool isWebBrowserVisible = false;

        //private PropertyGridEx advPropertyGrid1; // 工程属性

        Font defaultFont= new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));

        public string ImageUrl
        {
            get
            {
                return mImageUrl;
            }

        }



        public PrjDetail(CRailwayScene s)
        {
            InitializeComponent();
            timer1.Interval = timeSpan;
            timer1.Stop();

            mRWScene = s;
            initCustomComponent();
            //this.Size.Width
            
        }

        public PrjDetail()
        {
            InitializeComponent();
            timer1.Interval = timeSpan;
            timer1.Stop();
        }

        public void initPrjDetail(CRailwayScene s)
        {
            mRWScene = s;
            initCustomComponent();
            //this.Size.Width
            
        }

        private void initCustomComponent()
        {
            mls1.ImageSize = new Size(120, 120);
            //mls2.ImageSize = new Size(144, 144);

            //((System.ComponentModel.ISupportInitialize)(this.advPropertyGrid1)).BeginInit();
            this.SuspendLayout();

            this.panel1.Dock = DockStyle.Fill;
            panel1.Show();
            //this.listView1.Dock = DockStyle.Fill;
            this.listView1.Hide();
            //this.listViewEx2.Dock = DockStyle.Fill;
            //this.listViewEx2.Hide();
            //this.webBrowser1.Dock = DockStyle.Fill;
            //webBrowser1.Hide();
            this.ResumeLayout(false);

        }


        public bool initPrjDetail(CRailwayProject rp)
        {
            if (rp == null) return false;
            if (rp == this.propertyGridEx1.Tag) return false;

            propertyGridEx1.Tag = rp;
            propertyGridEx1.SelectedObject = rp;
            if (rp.FXProgress.Count >= 3)
            {
                showOneFX(propertyGridEx2, rp.FXProgress[rp.selectedFXid[0]], 0);
                showOneFX(propertyGridEx3, rp.FXProgress[rp.selectedFXid[1]], 1);
                showOneFX(propertyGridEx4, rp.FXProgress[rp.selectedFXid[2]], 2);
            }

            initPrjPier(rp);
            mImageUrl =  rp.mPhotoUrl;
            //initPrjPhoto(rp);
            return true;

        }
        #region 辅助方法：根据工程初始化工程显示面板


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
            listView1.Size = new Size(175 * listView1.Items.Count,180);
            listView1.Location = new Point((panel1.Width - listView1.Width) / 2, 15);
        }


        //private DataTable asyncGetPrjPhoto(CRailwayProject rp)
        //{
        //    if (rp == null) return null;

        //    DataTable dt = null;
        //    dt = CServerWrapper.showPhotosByProjSNo(rp.mSerialNo);
        //    return dt;
        //}

        //public delegate DataTable AsyncGetPrjPhotoDelegate(CRailwayProject rp);
        //public delegate void HandlePrjPhoto(string imgUrl, int width, int height);
        //public event HandlePrjPhoto OnPrjPhotoUrl = null;

        //void prjPhotoInfoHandle(IAsyncResult ar)
        //{
        //    AsyncGetPrjPhotoDelegate dlgt = (AsyncGetPrjPhotoDelegate)ar.AsyncState;
        //    DataTable dt = dlgt.EndInvoke(ar);
        //    if (dt == null || dt.Rows.Count == 0) return;
        //    int photoW = 0, photoH = 0;
        //    mImageUrl = "";
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        mImageUrl = @"http://" + CGisDataSettings.gCurrentProject.projectUrl + dr["Url"].ToString();
        //        Console.WriteLine(mImageUrl);
        //        photoW = Convert.ToInt32(dr["Width"]);
        //        photoH = Convert.ToInt32(dr["Height"]);
        //        break;
        //    }
        //    if (!string.IsNullOrEmpty(mImageUrl))
        //    {
        //        OnPrjPhotoUrl?.Invoke(mImageUrl, photoW, photoH);
        //    }

        //}

        //private void initPrjPhoto(CRailwayProject rp)
        //{
        //    if (rp == null) return ;

        //    if (OnPrjPhotoUrl == null) return;
        //    //DataTable dt= AsyncDelegate.;
        //    AsyncGetPrjPhotoDelegate dlgt = new AsyncGetPrjPhotoDelegate(asyncGetPrjPhoto);
        //    IAsyncResult ar = dlgt.BeginInvoke(rp, new AsyncCallback(prjPhotoInfoHandle), dlgt);

        //    //DataTable dt = CServerWrapper.findPhotosByProjSNo(rp.mSerialNo);
        //    //mImageUrl = null;

        //    ////mls2.Images.Clear();
        //    ////listViewEx2.Items.Clear();

        //    //if (dt == null) return;

        //    //int i = 0;
        //    //try
        //    //{
        //    //    foreach (DataRow dr in dt.Rows)
        //    //    {
        //    //        mImageUrl = @"http://" + CGisDataSettings.gCurrentProject.projectUrl + dr["FileName"].ToString();
        //    //        break;
        //    //        //WebRequest webreq = WebRequest.Create(imgurl);
        //    //        //WebResponse webres = webreq.GetResponse();
        //    //        //Stream stream = webres.GetResponseStream();
        //    //        //Image image;
        //    //        //image = Image.FromStream(stream);
        //    //        //stream.Close();

        //    //        //mls2.Images.Add(image);
        //    //        //ListViewItem lvi = new ListViewItem();
        //    //        //lvi.Text = dr["Person"].ToString();
        //    //        //lvi.Font = defaultFont;
        //    //        //lvi.ImageIndex = i++;
        //    //        //listViewEx2.Items.Add(lvi);
        //    //    }
        //    //    //isWebBrowserVisible = false;
        //    //    //listViewEx2.View = View.LargeIcon;
        //    //    //listViewEx2.LargeImageList = mls2;
        //    //} catch (Exception ee)
        //    //{
        //    //    //foreach (DataRow dr in dt.Rows)
        //    //    //{
        //    //    //    string imgurl = @"http://" + CGisDataSettings.gCurrentProject.projectUrl + dr["FileName"].ToString();
        //    //    //    webBrowser1.Navigate(imgurl);
        //    //    //    isWebBrowserVisible = true;
        //    //    //    break;

        //    //    //}
        //    //}
        //    //CRailwayProject prj;
        //    //    for (int i = 0; i < num; i++)
        //    //    {
        //    //        prj = mRWScene.getProjectBySNo(sNo[i]);
        //    //        if (prj != null)
        //    //        {
        //    //            Console.WriteLine(prj.ProjectName);
        //    //        }
        //    //        Console.WriteLine("{0} #\t:  fileName {1}\t   Date {2}\t   Person {3}\t   Remark {4} ", i,  fileName[i], photoTime[i], person[i], remark[i]);
        //    //    }

        //}


        private void showOneFX(PropertyGridEx pg, CFXProj fx, int fxID)
        {
            pg.SelectedObject = fx;
            chart1.Series[fxID].Points.Clear();
            for (int j = 0; j < fx.strDate.Count; j++)
                chart1.Series[fxID].Points.AddXY(DateTime.ParseExact(fx.strDate[j], "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture),
            Math.Round(fx.doneAmount[j] / fx.TotalAmount * 100, 1));

        }
        #endregion

        private void PrjDetail_VisibleChanged(object sender, EventArgs e)
        {
            UserControl panel = sender as UserControl;

            if (panel.Visible)
            {
                //开启显示面板，如果有显示内容，开启计时，否则，发送展示结束消息;
                if (showNextPanel())
                {
                    timer1.Interval = timeSpan;
                    timer1.Start();
                }

            }
            else
            {
                panelStatus = 0;
                timer1.Stop();

            }

        }

        private bool showNextPanel()
        {
            bool hasNext = false;
            while (panelStatus < 2)
            {
                if (panelStatus == 0)
                {
                    NextPanelStarting?.Invoke();
                    panel1.Show();
                    this.listView1.Hide();
                    //this.listViewEx2.Hide();
                    panelStatus++;
                    hasNext = true;
                    break;
                }
                else if (panelStatus == 1)
                {
                    if (listView1.Items.Count > 0)
                    {
                        NextPanelStarting?.Invoke();
                        panel1.Hide();
                        this.listView1.Show();
                        //this.listViewEx2.Hide();
                        panelStatus++;
                        hasNext = true;
                        break;
                    }
                }
                //else if (panelStatus == 2)
                //{
                //    if (isWebBrowserVisible)
                //    {
                //        NextPanelStarting?.Invoke();
                //        panel1.Hide();
                //        this.listView1.Hide();
                //        this.listViewEx2.Hide();
                //        this.webBrowser1.Show();
                //        panelStatus++;
                //        hasNext = true;
                //        break;
                //    }
                //    else if (listViewEx2.Items.Count > 0)
                //    {
                //        NextPanelStarting?.Invoke();
                //        panel1.Hide();
                //        this.listView1.Hide();
                //        this.listViewEx2.Show();
                //        panelStatus++;
                //        hasNext = true;
                //        break;
                //    }
                //}
                panelStatus++;
            }
            return hasNext;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (showNextPanel()) {
                timer1.Interval = timeSpan;
                timer1.Start();
            }
                
            else
            {
                this.Hide();
                DetailIsStopping?.Invoke();                

            }

        }
    }
}
