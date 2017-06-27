using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using ModelInfo;
using RailwayGIS.TEProject;
using System.Data.OleDb; // <- for database methods

using TerraExplorerX;
using ThoughtWorks.QRCode.Codec;
//using ThoughtWorks.QRCode.Codec.Data;
//using ThoughtWorks.QRCode.Codec.Util;

namespace RailwayGIS
{
    public partial class GISForm : DevComponents.DotNetBar.OfficeForm
    {
        DevComponents.DotNetBar.RadialMenu menuSG = new DevComponents.DotNetBar.RadialMenu();
        private DevComponents.DotNetBar.RadialMenuItem itemNavCont;
        private DevComponents.DotNetBar.RadialMenuItem itemSaveDB;
        private DevComponents.DotNetBar.RadialMenuItem itemNextPro;
        private DevComponents.DotNetBar.RadialMenuItem itemPrevPro;

        private Progress2DPanel panel2D;
        private CTEPresentation mContbeamNav;
        private int mInstanceID;
        
        

        private static double gpsX = 117;
        private static double gpsY = 35;
        private static double mileageCenter = -1;
        private static double mileageCiewR = -1;

        //private static CRailwayProject currentProject = null;

        CRailwayScene gRWScene;

        CTEScene mTEScene;
        List<CHotSpot> curNavList = null;
        //FullScreenForm f = new FullScreenForm();

        public SGWorld66 sgworld;

        //GMap.NET.WindowsForms.Markers.GMarkerGoogle marker;
        //GMap.NET.WindowsForms.Markers.GMarkerGoogle markerTrain;
        
        //int flyingTick = 0;
        //bool isFlying = false;
        //bool _bFullScreenMode = false;

        private DateTime sTime = Convert.ToDateTime("2016-01-01");
        private DateTime eTime = DateTime.Now;
        private DateTime curTime = DateTime.Now;
        private bool isTimeChanged = false;
        private CRailwayProject curProject=null;

        public IPosition66 mCurPos;

        //private int SH = Screen.PrimaryScreen.Bounds.Height;
        //private int SW = Screen.PrimaryScreen.Bounds.Width;
        private static Font defaultFont = new Font("宋体", 12);
        private static Pen defaultPen = new Pen(Color.Black, 1);
        private static Pen redPen = new Pen(Color.Red, 1);

        List<string> gNavCmd = new List<string>();


        public GISForm()
        {
            //GlobalSettings.LoadConfig();

            LoginForm login = new LoginForm();
            if (login.ShowDialog() != DialogResult.OK)
            {
                Environment.Exit(-1); 
            }
            WelcomeFormJQ wf = new WelcomeFormJQ();
            wf.Show(this);

            Random ran = new Random();
            mInstanceID = ran.Next(100, 999);
            //g = Guid.NewGuid();
            //GlobalSettings.InitGlobal();

            gRWScene = new CRailwayScene(CGisDataSettings.gLocalDB, CGisDataSettings.gCurrentProject.projectUrl);
            //GlobalVar.gScene = new ModelInformation.CRailwayScene();


            InitializeComponent();

            panel2D = new Progress2DPanel(gRWScene);
            mainContainer.Panel2.Controls.Add(panel2D);
            this.skinAnimator1.SetDecoration(this.panel2D, CCWin.SkinControl.DecorationType.None);
            this.panel2D.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2D.Location = new System.Drawing.Point(0, 0);
            this.panel2D.Name = "panel2D";
            this.panel2D.Size = new System.Drawing.Size(1527, 183);
            this.panel2D.TabIndex = 0;
            this.panel2D.PanelSelectClicked += Panel2D_PanelSelectClicked;


            sgworld = new SGWorld66();
            this.Text = "新建济青高速铁路工程:" + mInstanceID;
            //this.axTE3DWindow1.Text = "新建济青高速铁路";

            //axTE3DWindow1.Caption = "新建济青高速铁路";

            ribbonBar3.Enabled = CServerWrapper.isConnected;
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲



            //GlobalSettings.gGForm = this;

            showProject();
            addDVG_Event();
            //bar1.Visible = false;
            //bar1.Hide();
            createMenuSG();
            menuSG.Visible = false;
            //mainContainer.Panel2Collapsed = true;
            //trackTime.Text = curTime.ToLongDateString();
            gNavCmd.Add(@"cmd_Nav_ContBeam");
            gNavCmd.Add("");

            wf.Close();
        }

        private void Panel2D_PanelSelectClicked(CRailwayProject prj, CRailwayDWProj dwprj)
        {
           if (prj != null) {                     
                //skinAnimator1.WaitAllAnimations();
                //skinAnimator1.Hide(advPropertyGrid1);

                pictureBox2.Image = generate2DCode(@"http://jqmis.cn/S/" + prj.mSerialNo);
                showProjectDetail(prj);
                //skinAnimator1.WaitAllAnimations();
                //skinAnimator1.Show(advPropertyGrid1);
           }
           else if (dwprj !=null)
           {

                //skinAnimator1.WaitAllAnimations();
                //skinAnimator1.Hide(advPropertyGrid1);

                pictureBox2.Image = generate2DCode(@"http://jqmis.cn/S/" + dwprj.mSerialNo);
                advPropertyGrid1.SelectedObject = dwprj;
                //skinAnimator1.WaitAllAnimations();
                //skinAnimator1.Show(advPropertyGrid1);
            }
        }

        private void GISForm_Load(object sender, EventArgs e)
        {
            // Register to OnLoadFinished globe event

            sgworld.OnLoadFinished += new _ISGWorld66Events_OnLoadFinishedEventHandler(OnProjectLoadFinished);

            // Open Project in asynchronous mode

            string tProjectUrl = CGisDataSettings.gDataPath + CGisDataSettings.gCurrentProject.projectLocalPath + "Default.fly";
            sgworld.Project.Open(tProjectUrl, true, null, null);//          sgworld.Project.Open(@"F:\地图\MNGis.mpt", true, null, null);
            //sgworld.DateTime.DisplaySun = false;
            sgworld.Command.Execute(1026, 0);  // sun
            sgworld.Command.Execute(1065, 4);


        }

        #region 右键菜单，暂时屏蔽
        private void createMenuSG(){

            this.itemNavCont = new DevComponents.DotNetBar.RadialMenuItem();
            this.itemSaveDB = new DevComponents.DotNetBar.RadialMenuItem();
            this.itemNextPro = new DevComponents.DotNetBar.RadialMenuItem();
            this.itemPrevPro = new DevComponents.DotNetBar.RadialMenuItem();

            // 
            // itemAround
            // 
            this.itemNavCont.Name = "itemAround";
            this.itemNavCont.Symbol = "";
            this.itemNavCont.Text = "巡航";
            this.itemNavCont.TextVisible = true;
            this.itemNavCont.Tooltip = "连续梁巡航";
            // 
            // itemPano
            // 
            this.itemSaveDB.Name = "itemPano";
            this.itemSaveDB.Symbol = "57779";
            this.itemSaveDB.SymbolSet = DevComponents.DotNetBar.eSymbolSet.Material;
            this.itemSaveDB.Text = "存数据";
            this.itemSaveDB.TextVisible = true;
            this.itemSaveDB.Tooltip = "数据离线";
            // 
            // itemNextPro
            // 
            this.itemNextPro.Name = "itemNextPro";
            this.itemNextPro.Symbol = "";
            this.itemNextPro.Text = "NextPro";
            this.itemNextPro.TextVisible = false;
            this.itemNextPro.Tooltip = "Next Project";
            // 
            // itemPrevPro
            // 
            this.itemPrevPro.Name = "itemPrevPro";
            this.itemPrevPro.Symbol = "";
            this.itemPrevPro.Text = "PrevPro";
            this.itemPrevPro.TextVisible = false;
            this.itemPrevPro.Tooltip = "Previous Project";

            this.menuSG.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.itemNavCont,
            this.itemSaveDB,
            this.itemNextPro,
            this.itemPrevPro});
            menuSG.ItemClick += menuSG_ItemClick;
        }

        private void menuSG_ItemClick(object sender, EventArgs e)
        {
            RadialMenuItem item = sender as RadialMenuItem;

            if (item != null && !string.IsNullOrEmpty(item.Text))
            {
                switch (item.Text)
                {
                    case "巡航":
                        ////var cPos = sgworld.Navigate.GetPosition(AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE);
                        //IPosition66 cPos = gRWScene.mMiddleLines.getNearPos();//CRailwayScene.mMiddleLine.findNearestPos();
                        //this.menuSG.Visible = false;
                        ////CRailwayScene.mMiddleLine.findPosbyMeter(meters, out x, out y, out z, out dir);
                        //sgworld.Navigate.FlyTo(cPos, ActionCode.AC_ARCPATTERN);
                        auto_Nav(gRWScene.GetContBeamNavPath());
                        menuSG.Visible = false;
                        break;
                    case "存数据":
                        gRWScene.saveProjectLocal(CGisDataSettings.gLocalDB);
                        gRWScene.saveProjFXLocal(CGisDataSettings.gLocalDB);
                        gRWScene.savePierLocal(CGisDataSettings.gLocalDB);
                        gRWScene.saveFirmLocal(CGisDataSettings.gLocalDB);
                        gRWScene.saveConsLocal(CGisDataSettings.gLocalDB);
                        break;
                    default:
                        break;

                }
                //MessageBox.Show(string.Format("{0} Menu item clicked: {1}\r\n", DateTime.Now, item.Text));
            }
        }

        #endregion

        #region 处理DataGridView
        private void showProject()
        {
            fillRoadGrid();
            fillBridgeGrid();
            fillTunnelGrid();
            fillContBeamGrid();
            fillOtherList();
        }

        //private void fillEventGrid()
        //{
        //    if (CConsLog.refreshConsLog()) {
        //        dataGridViewX1.Rows.Clear();
        //        for (int i = 0; i < CConsLog.mLogInfo.Count; i++)
        //        {
        //            addConsItem(CConsLog.mLogTime[i].ToString(), CConsLog.mLogInfo[i], dataGridViewX1);
        //        }
        //    }
        //}

        private void fillBridgeGrid()
        {
            //DataGridViewRow dr = new DataGridViewRow();
            foreach (CRailwayProject p in gRWScene.mBridgeList)
            {
                addItem(p.ProjectName, p.SegmentName, Convert.ToInt32(p.AvgProgress), p, dgvBridge);
            }
            dgvBridge.SelectionMode  = DataGridViewSelectionMode.FullRowSelect;
            dgvBridge.Sort(dgvBridge.Columns[2], ListSortDirection.Descending);
        }

        private void fillRoadGrid()
        {
            //DataGridViewRow dr = new DataGridViewRow();
            foreach (CRailwayProject p in gRWScene.mRoadList)
            {
                //addItem((int)(p.mMileage_Mid / 1000), p.mProjectName, p.SegmentName, p.mProfessionalName, p, dgvRoad);
                addItem(p.ProjectName, p.SegmentName, Convert.ToInt32(p.AvgProgress), p, dgvRoad);
            }
            dgvRoad.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRoad.Sort(dgvRoad.Columns[2], ListSortDirection.Descending);
        }

        private void fillTunnelGrid()
        {
            //DataGridViewRow dr = new DataGridViewRow();
            foreach (CRailwayProject p in gRWScene.mTunnelList)
            {
                //addItem((int)(p.mMileage_Mid / 1000), p.mProjectName, p.SegmentName, p.mProfessionalName, p, dgvTunnel);
                addItem(p.ProjectName, p.SegmentName, Convert.ToInt32(p.AvgProgress), p, dgvTunnel);
            }
        }

        private void fillContBeamGrid()
        {
            //DataGridViewRow dr = new DataGridViewRow();
            foreach (CRailwayProject p in gRWScene.mContBeamList)
            {
                //addItem((int)(p.mMileage_Mid / 1000), p.mProjectName, p.SegmentName, p.mProfessionalName, p, dgvStation);
                addItem(p.ProjectName, p.SegmentName, Convert.ToInt32(p.AvgProgress), p, dgvContBeam);
            }
            dgvContBeam.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvContBeam.Sort(dgvContBeam.Columns[2], ListSortDirection.Descending);
        }
        private void fillOtherList()
        {
            //DataGridViewRow dr = new DataGridViewRow();
            //foreach (CRailwayProject p in gRWScene.mOtherList)
            //{
            //    //addItem((int)(p.mMileage_Mid / 1000), p.mProjectName, p.SegmentName, p.mProfessionalName, p, dgvOther);
            //    addItem( p.ProjectName, p.SegmentName, (int)(p.AvgProgress*100), p, dgvOther);
            //}
        }

        private void addConsItem(string s1, string s2, DataGridView dgv)
        {
            DataGridViewRow dr = new DataGridViewRow();
            foreach (DataGridViewColumn c in dgv.Columns)
            {
                dr.Cells.Add(c.CellTemplate.Clone() as DataGridViewCell);
            }
            dr.Cells[0].Value = s1;
            dr.Cells[1].Value = s2;
            dgv.Rows.Add(dr);
        }
        private void addItem(string s1, string s2, int s3,  CRailwayProject p, DataGridView dgv)
        {
            DataGridViewRow dr = new DataGridViewRow();
            foreach (DataGridViewColumn c in dgv.Columns)
            {
                dr.Cells.Add(c.CellTemplate.Clone() as DataGridViewCell);
            }
            dr.Cells[0].Value = s1;
            dr.Cells[1].Value = s2;
            dr.Cells[2].Value = s3;
            //dr.Cells[3].Value = s4;
            //dr.Cells[2].
            dr.Tag = p;
            dgv.Rows.Add(dr);
        }



        private void dgvRoad_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            CRailwayProject p = null;
            if (e.RowIndex < 0) return;

            string sname = (sender as DevComponents.DotNetBar.Controls.DataGridViewX).Name;
            switch (sname)
            {
                case "dgvRoad":
                    p = dgvRoad.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                case "dgvBridge":
                    p = dgvBridge.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                case "dgvTunnel":
                    p = dgvTunnel.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                case "dgvContBeam":
                    p = dgvContBeam.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                default:
                    return;

            }
            //if (GlobalVar.gCurrentPro != null && GlobalVar.gCurrentPro.mPolyline != null)
            //{
            //    GlobalVar.gCurrentPro.mPolyline.Visibility.Show = false;
            //}
            ////if (p.mPolyline != null)
            ////    p.mPolyline.Visibility.Show = true;
            //GlobalVar.gCurrentPro = p;
            //sgworld.Navigate.FlyTo(sgworld.Creator.CreatePosition(p.mLongitude_Mid, p.mLatitude_Mid, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, p.mDirection + 90, -35.0, 0, 800));
            //      sgworld.Navigate.FlyTo(sgworld.Creator.CreatePosition(p.CenterLongitude, p.CenterLatitude, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -35.0, 0, 800));
            // http://railmis.imwork.net/jqmis/webservice/ProjectService.asmx
            if (p != null)
            {
                sgworld.Navigate.FlyTo(sgworld.Creator.CreatePosition(p.CenterLongitude, p.CenterLatitude, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -35.0, 0, 800));
                //string urlstr = "http://" + CGisDataSettings.gCurrentProject.projectUrl + "/APP/ProjectMng.aspx?f=detail&shownav=1&sn=" + p.mSerialNo +
                //           "&uacc=" + CGisDataSettings.gCurrentProject.userName + "&upwd=" + CGisDataSettings.gCurrentProject.userPSD;//&showtab=1
                showProjectDetail(p);
            }

        }

        public void OnReachToOnePlace(CHotSpot hs1, CHotSpot hs2)
        {

            //if (hs1.ObjectRef is CRailwayProject)
            showProjectDetail(hs1.ObjectRef);
            //else 
            //advPropertyGrid1.SelectedObject = hs1.ObjectRef;
            //if (hs2 != null)
            //{
            //    advPropertyGrid2.SelectedObject = hs1.ObjectRef;
            //}
        }

        private void dgvRoad_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            CRailwayProject p = null;
            if (e.RowIndex < 0) return;
            string sname = (sender as DevComponents.DotNetBar.Controls.DataGridViewX).Name;
            switch (sname)
            {
                case "dgvRoad":
                    p = dgvRoad.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                case "dgvBridge":
                    p = dgvBridge.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                case "dgvTunnel":
                    p = dgvTunnel.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                case "dgvContBeam":
                    p = dgvContBeam.Rows[e.RowIndex].Tag as CRailwayProject;
                    break;
                //case "dgvOther":
                //    p = dgvOther.Rows[e.RowIndex].Tag as CRailwayProject;
                //break;
                default:
                    return;

            }
            if (p == null)
                return;

            //ProjectService.ProjectServiceSoapClient ws = new ProjectService.ProjectServiceSoapClient();
            //Console.WriteLine(p.mProjectName + 　ws.ws_Get_ProjectProgressRate(p.mSerialNo));
            //DataTable dt;
            //dt = ws.ws_Bind_ProjectProgress_HistoryRate_DataTable(p.mSerialNo);
            //foreach (DataRow dr in dt.Rows)
            //{
            //    Console.WriteLine((string)dr["Date"] + "\t" + (double)dr["Rate"]);
            //}
            if (p.mIsValid)
                mContbeamNav.startNextPresentation();
            else
                sgworld.Navigate.FlyTo(sgworld.Creator.CreatePosition(p.CenterLongitude, p.CenterLatitude, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -35.0, 0, 800));
            showProjectDetail(p);
            //pnlTrain.Expanded = true;
            //mainContainer.Panel2Collapsed = false;
            //webBrowser1.Navigate(urlstr);
        }


        private void addDVG_Event()
        {

            dgvRoad.CellMouseClick += dgvRoad_RowHeaderMouseClick;
            dgvBridge.CellMouseClick += dgvRoad_RowHeaderMouseClick;
            dgvTunnel.CellMouseClick += dgvRoad_RowHeaderMouseClick;
            dgvContBeam.CellMouseClick += dgvRoad_RowHeaderMouseClick;
            //dgvOther.CellMouseClick += dgvRoad_RowHeaderMouseClick;

            dgvRoad.CellMouseDoubleClick += dgvRoad_RowHeaderMouseDoubleClick;
            dgvBridge.CellMouseDoubleClick += dgvRoad_RowHeaderMouseDoubleClick;
            dgvTunnel.CellMouseDoubleClick += dgvRoad_RowHeaderMouseDoubleClick;
            dgvContBeam.CellMouseDoubleClick += dgvRoad_RowHeaderMouseDoubleClick;
            //dgvOther.CellMouseDoubleClick += dgvRoad_RowHeaderMouseDoubleClick;


            this.btnMessage.Click += btnMessage_Click;

            this.btnSubgrade.Click += new System.EventHandler(this.btnProject_Click);
            this.btnBridge.Click += new System.EventHandler(this.btnProject_Click);
            this.btnTunnel.Click += new System.EventHandler(this.btnProject_Click);
            this.btnContBeam.Click += new System.EventHandler(this.btnProject_Click);
            this.btnFirms.Click += new System.EventHandler(this.btnProject_Click);
            //this.btnConsLog.Click += btnConsLog_Click;
            this.btnAll.Click += btnAll_Click;
            this.btnManuNav.Click += btnManuNav_Click;
            //this.btnFirms.Click += btnFirms_Click;
        }

        #endregion



        private void timerSyncronize_Tick(object sender, EventArgs e)
        {
            try
            {
                IWorldPointInfo66 w0 = sgworld.Window.PixelToWorld(axTE3DWindow1.Size.Width - 10, axTE3DWindow1.Size.Height - 10);
                double lx = w0.Position.X;
                double ly = w0.Position.Y;
                if (lx < 115 || lx > 125 || ly < 32 || ly > 40) return;
                if (CoordinateConverter.getUTMDistance(lx,ly,gpsX,gpsY)< 10)
                {
                    gpsX = lx;
                    gpsY = ly;
                    return;
                }
                
                IPosition66 cp = sgworld.Window.CenterPixelToWorld().Position;                

                //GMap.NET.PointLatLng LatLng = marker.Position;
                //LatLng.Lat = cp.Y;
                //LatLng.Lng = cp.X;
                //marker.Position = LatLng;
                
                List<CRailwayProject> ls = null;
                
                mileageCiewR = CoordinateConverter.getUTMDistance(cp.X, cp.Y, lx, ly);

                    ls = gRWScene.getNearProject(cp.X, cp.Y, mileageCiewR / 20+ 100, gRWScene.mTotalProjectList);
                    if (ls != null && ls.Count > 0)
                    {
                        showProjectDetail(ls[0]);                    


                    }
                    else
                        showProjectDetail(gRWScene.rootProj);
                    
  
                gpsX = lx;
                gpsY = ly;               
                double dis;
                bool isInside;

                mileageCenter = 0; // gRWScene.mMiddleLines.getGlobalMileageByGPS(cp.X, cp.Y,out dis, out isInside);
                //gRWScene.mMiddleLines.get
                mileageCiewR = Math.Min(mileageCiewR, 200000);
                //gaugeControl2.Visible = CTEPresentation.isPresentation;
                //advPropertyGrid5.Visible = !CTEPresentation.isPresentation;
                gaugeControl2.LinearScales[0].Pointers[0].Value = mileageCenter / 1000;
                //gaugeControl1.CircularScales[1].Pointers[0].Value = CTEPresentation.mSpeed;
                if (mileageCenter >= 0)
                {
                    panel2D.mileageCenter = mileageCenter;
                    panel2D.mileageViewRadius = mileageCiewR;
                    panel2D.Refresh();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("工点状态每秒定时更新错误");
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //if (_bFullScreenMode) 
            //    return true;
            switch (keyData)
            {
                case Keys.Escape:
                    

                        if (MessageBox.Show("是否退出漫游?", "退出", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            mContbeamNav.stopPresentation();
                            //IPosition66 resPos = sgworld.Navigate.GetPosition(AltitudeTypeCode.ATC_TERRAIN_RELATIVE);
                            //resPos = sgworld.Creator.CreatePosition(resPos.X, resPos.Y, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, resPos.Yaw + 5, resPos.Pitch, resPos.Roll, 1500);//resPos.Altitude
                            //sgworld.Navigate.FlyTo(resPos);
                        }
                        
                    
                    else if (MessageBox.Show("是否退出系统?", "退出", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Application.Exit();
                    }

                    return true;
                case Keys.F5:
                    {
                        auto_Nav(gRWScene.GetContBeamNavPath());
                    }
                    break;
                case Keys.F1:
                    gRWScene.saveProjectLocal(CGisDataSettings.gLocalDB);
                    gRWScene.saveProjFXLocal(CGisDataSettings.gLocalDB);
                    gRWScene.savePierLocal(CGisDataSettings.gLocalDB);
                    gRWScene.saveFirmLocal(CGisDataSettings.gLocalDB);
                    gRWScene.saveConsLocal(CGisDataSettings.gLocalDB);
                    break;
                //case Keys.F5:
                //    beginFly(); return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        //public void MaxToMainForm()
        //{
           
        //    mainContainer.Panel1.Controls.Add(this.axTE3DWindow1);
        //    _bFullScreenMode = false;
            
        //}

        void OnProjectLoadFinished(bool bSuccess)
        {

            mCurPos = sgworld.Creator.CreatePosition(118.6, 36.6, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE,
                341, -90.0, 0, 445000);
            //Camera.initial_latitude, Camera.initial_longitude, Camera.initial_heading, Camera., Camera.initial_tilt
            sgworld.Navigate.FlyTo(mCurPos, ActionCode.AC_FLYTO);
            if (mTEScene == null)
                mTEScene = new CTEScene(gRWScene);
            //mTEScene.createProjectTree();      

            //sgworld.OnMouseWheel += new _ISGWorld66Events_OnMouseWheelEventHandler(OnMouseWheel);
            sgworld.OnLButtonUp += new _ISGWorld66Events_OnLButtonUpEventHandler(OnLButtonUp);
            sgworld.OnRButtonUp += sgworld_OnRButtonUp;
            //sgworld.OnProjectTreeAction += new _ISGWorld66Events_OnProjectTreeActionEventHandler(OnProjectTreeAction);
            sgworld.OnDateTimeChanged += sgworld_OnDateTimeChanged;
            sgworld.OnSGWorldMessage += sgworld_OnSGWorldMessage;
            //sgworld.OnObjectAction += sgworld_OnObjectAction;

            mContbeamNav = new CTEPresentation(gRWScene, curNavList); //, timerNav
            mContbeamNav.OnToOnePlace += OnReachToOnePlace;
            //sgworld.OnDrawHUD += sgworld_OnDrawHUD;
            timerSyncronize.Start();

        }


        bool sgworld_OnRButtonUp(int Flags, int X, int Y)
        {
            //GlobalVar.gCurPos = sgworld.Window.PixelToWorld(X, Y).Position;
            //GlobalVar.gCurPos = sgworld.Navigate.GetPosition(AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE);
            this.menuSG.Location = new Point(X, Y);
            menuSG.Visible = true;
            return true;
        }

        bool OnLButtonUp(int Flags, int X, int Y)
        {
            mCurPos = sgworld.Window.PixelToWorld(X, Y).Position;

            return false;
        }

        //void OnProjectTreeAction(string ID, IAction66 Action)
        //{
        //    string projectName;
        //    if (Action.Code.Equals(ActionCode.AC_SELCHANGED))
        //    {
        //        try
        //        {
        //            projectName = sgworld.ProjectTree.GetItemName(ID);
        //            //var branch = sgworld.ProjectTree.FindItem("MiddleLine");
        //            //if (projectName.Equals("Train"))
        //            //{

        //            //    //sgworld.ProjectTree.SetVisibility(branch, false);
        //            //    sgworld.Navigate.FlyTo(GlobalVar.gScene.mDynamicTrain, ActionCode.AC_FLYTO);
        //            //    flyingTick++;
        //            //    return;

        //            //}

        //            //sgworld.ProjectTree.SetVisibility(branch, true);
        //            foreach (CRailwayProject item in gRWScene.mBridgeList)
        //            //for (int j = 0; j < CRailwayScene.mProjectList.Count; j++)
        //            {
        //                if (item.ProjectName.Equals(projectName, StringComparison.InvariantCultureIgnoreCase))
        //                {
        //                    sgworld.Navigate.FlyTo(sgworld.Creator.CreatePosition(item.CenterLongitude, item.CenterLatitude, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE));
        //                    //textBox2.Text = "Project Name\t" + item.mProjectName + "\r\nProject Type\t" + item.mProfessionalName +
        //                    //    "\r\nStart Position\t" + item.mMileage_Start_Des + "\r\nEnd_Position\t" + item.mMileage_End_Des;
        //                    //timerFlyto.Tag = item.mSerialNo;
        //                    //timerFlyto.Start();
        //                    //System.Threading.Thread.Sleep(2000);
        //                    //showWebPage(item.mSerialNo);
        //                    break;

        //                }
        //            }
        //            //flyingTick = 0;
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message.ToString());
        //        }

        //    }


        //}



        //private void gMapControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        //{
        //    marker.Position = gMapControl1.FromLocalToLatLng(e.X, e.Y);
        //    double x, y, z, dir;
        //    //var cPos = sgworld.Navigate.GetPosition(AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE);
        //    double meters;
        //    double dis;
        //    CRailwayLine crwline = gRWScene.mMiddleLines.getMileagebyGPS(marker.Position.Lng, marker.Position.Lat, out meters,out dis);
        //    gRWScene.mMiddleLines.getGPSbyDKCode(crwline.mDKCode, meters, out x, out y, out z, out dir);
        //    IPosition66 dPos = sgworld.Creator.CreatePosition(x, y, z, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, dir, -70, 0, 1500);
        //    //CRailwayScene.mMiddleLine.findNearestPos(cPos);
        //    sgworld.Navigate.FlyTo(dPos);
        //}

        //private void gMapControl1_Load(object sender, EventArgs e)
        //{
        //    GMap.NET.WindowsForms.GMapOverlay markersOverlay = new GMap.NET.WindowsForms.GMapOverlay("markers");

        //    marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(new GMap.NET.PointLatLng(36.6, 118.6),
        //      GMap.NET.WindowsForms.Markers.GMarkerGoogleType.red_dot);
        //    markerTrain = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(new GMap.NET.PointLatLng(36.6, 118.6),
        //        GMap.NET.WindowsForms.Markers.GMarkerGoogleType.blue_small);
        //    markersOverlay.Markers.Add(marker);
        //    markersOverlay.Markers.Add(markerTrain);

        //    marker.IsHitTestVisible = false;

        //    gMapControl1.Overlays.Add(markersOverlay);
        //}




        //private void trackZoom_ValueChanged(object sender, EventArgs e)
        //{
        //    IPosition66 ipos = sgworld.Window.CenterPixelToWorld().Position;
        //    double x, y, z, dir;
        //    //var cPos = sgworld.Navigate.GetPosition(AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE);
        //    double meters;
        //    double dist;
        //    CRailwayLine crwline = gRWScene.mMiddleLines.getMileagebyGPS(marker.Position.Lng, marker.Position.Lat, out meters, out dist);
        //    gRWScene.mMiddleLines.getGPSbyDKCode(crwline.mDKCode, meters, out x, out y, out z, out dir);
        //    double dis = Math.Pow(10,trackZoom.Value+1);
        //    if (trackZoom.Value == 5)
        //    {
        //        dis = dis / 2;
        //    }
        //    IPosition66 dPos = sgworld.Creator.CreatePosition(x, y, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, dir, -70, 0, dis);
        //    //CRailwayScene.mMiddleLine.findNearestPos(cPos);
        //    sgworld.Navigate.FlyTo(dPos);

        //    //double t = (double)(trackZoom.Value - trackZoom.Minimum) / (trackZoom.Maximum - trackZoom.Minimum);

        //    //int t2 = (int)((eTime.DayOfYear - sTime.DayOfYear) * t);
        //    //DateTime dt = sTime.AddDays(t2);
        //    //trackZoom.Tooltip = dt.ToString();

        //    //sgworld.DateTime.Current = dt;
        //    //trackZoom.ShowToolTip();

        //}





        #region 工点漫游事件处理
        private void btnProject_Click(object sender, EventArgs e)
        {
            //this.mainContainer.Width = this.Width - 400;
            //this.axTE3DWindow1.Width = this.Width - 400;
            //this.bar1.Width =  360;
            //this.bar1.Show();
            this.bar1.Visible = true;
            bar1.SelectedDockTab = 0;
            string sname = (sender as DevComponents.DotNetBar.ButtonItem).Name;
            switch (sname)
            {
                case "btnSubgrade":
                    bar1.SelectedDockTab = 0;
                    break;
                case "btnBridge":
                    bar1.SelectedDockTab = 1;
                    break;
                case "btnTunnel":
                    bar1.SelectedDockTab = 2;
                    break;
                case "btnContBeam":
                    
                    //MainToMaxForm();
                    //f.Nav(gRWScene, gRWScene.GetContBeamNavPath());
                    //List<CHotSpot> ls = gRWScene.GetContBeamNavPath();
                    auto_Nav(gRWScene.GetContBeamNavPath());


                    break;
                default:
                    return;

            }
        }

        void auto_Nav(List<CHotSpot> ls)
        {
            mContbeamNav.startNextPresentation();
            //CTEPresentation.initPresentation(gRWScene, ls, OnReachToOnePlace, timerNav);
            gaugeControl2.LinearScales[0].CustomLabels.Clear();
            Image workingIcon = Image.FromFile(CGisDataSettings.gDataPath + @"Common\Textures\icon.gif");
            DevComponents.Instrumentation.GaugeCustomLabel gaugeCustomLabel1;

            //gaugeCustomLabel1 = new DevComponents.Instrumentation.GaugeCustomLabel();
            //gaugeCustomLabel1.TickMark.Layout.Image = workingIcon;
            //gaugeCustomLabel1.Text = "济南";
            //gaugeCustomLabel1.Value = 0;
            //gaugeControl2.LinearScales[0].CustomLabels.Add(gaugeCustomLabel1);

            //gaugeCustomLabel1 = new DevComponents.Instrumentation.GaugeCustomLabel();
            //gaugeCustomLabel1.TickMark.Layout.Image = workingIcon;
            //gaugeCustomLabel1.Text = "青岛";
            //gaugeCustomLabel1.Value = 300;
            //gaugeControl2.LinearScales[0].CustomLabels.Add(gaugeCustomLabel1);

            foreach (CHotSpot hs in ls)
            {
                gaugeCustomLabel1 = new DevComponents.Instrumentation.GaugeCustomLabel();
                gaugeCustomLabel1.TickMark.Layout.Image = workingIcon;
                gaugeCustomLabel1.Text = "";
                gaugeCustomLabel1.Value = hs.GlobalMileage / 1000;
                gaugeControl2.LinearScales[0].CustomLabels.Add(gaugeCustomLabel1);
            }
        }

        void btnAll_Click(object sender, EventArgs e)
        {
            //MainToMaxForm();
            //f.Nav(gRWScene, gRWScene.GetAllNavPathbyMileage());
            List<CHotSpot> ls = gRWScene.GetAllNavPathbyMileage();
            //CTEPresentation.initPresentation(gRWScene, ls, OnReachToOnePlace, timerNav);
            mContbeamNav.startNextPresentation();
            gaugeControl2.LinearScales[0].CustomLabels.Clear();
            Image workingIcon = Image.FromFile(CGisDataSettings.gDataPath + @"Common\Textures\icon.gif");
            foreach (CHotSpot hs in ls)
            {
                DevComponents.Instrumentation.GaugeCustomLabel gaugeCustomLabel1 = new DevComponents.Instrumentation.GaugeCustomLabel();
                gaugeCustomLabel1.TickMark.Layout.Image = workingIcon;
                gaugeCustomLabel1.Text = "";
                gaugeCustomLabel1.Value = hs.GlobalMileage / 1000;
                gaugeControl2.LinearScales[0].CustomLabels.Add(gaugeCustomLabel1);
            } 
        }

        void btnManuNav_Click(object sender, EventArgs e)
        {
            //if (mContbeamNav.isPresentation)
                mContbeamNav.stopPresentation();
            //else
            //    mContbeamNav.startNextPresentation();
        }

        //void btnConsLog_Click(object sender, EventArgs e)
        //{
        //    //MainToMaxForm();
        //    //f.Nav(gRWScene, gRWScene.GetConsLogNavPath());
        //    //CTEPresentation.initPresentation(gRWScene, gRWScene.GetConsLogNavPath(), OnReachToOnePlace, timerNav);
        //    List<CHotSpot> ls = gRWScene.GetConsLogNavPath();
        //    CTEPresentation.initPresentation(gRWScene, ls, OnReachToOnePlace, timerNav);
        //    gaugeControl2.LinearScales[0].CustomLabels.Clear();
        //    Image workingIcon = Image.FromFile(CGisDataSettings.gDataPath + @"Common\Textures\icon.gif");
        //    foreach (CHotSpot hs in ls)
        //    {
        //        DevComponents.Instrumentation.GaugeCustomLabel gaugeCustomLabel1 = new DevComponents.Instrumentation.GaugeCustomLabel();
        //        gaugeCustomLabel1.TickMark.Layout.Image = workingIcon;
        //        gaugeCustomLabel1.Text = "";
        //        gaugeCustomLabel1.Value = hs.GlobalMileage / 1000;
        //        gaugeControl2.LinearScales[0].CustomLabels.Add(gaugeCustomLabel1);
        //    }
        //    //this.bar1.Visible = false;
            
        //}
        //void btnFirms_Click(object sender, EventArgs e)
        //{
        //    //MainToMaxForm();
        //    //f.Nav(gRWScene, gRWScene.GetFirmsNavPath());
        //    //CTEPresentation.initPresentation(gRWScene, gRWScene.GetFirmsNavPath(), OnReachToOnePlace, timerNav);
        //    List<CHotSpot> ls = gRWScene.GetFirmsNavPath();
        //    CTEPresentation.initPresentation(gRWScene, ls, OnReachToOnePlace, timerNav);
        //    gaugeControl2.LinearScales[0].CustomLabels.Clear();
        //    Image workingIcon = Image.FromFile(CGisDataSettings.gDataPath + @"Common\Textures\icon.gif");
        //    foreach (CHotSpot hs in ls)
        //    {
        //        DevComponents.Instrumentation.GaugeCustomLabel gaugeCustomLabel1 = new DevComponents.Instrumentation.GaugeCustomLabel();
        //        gaugeCustomLabel1.TickMark.Layout.Image = workingIcon;
        //        gaugeCustomLabel1.Text = "";
        //        gaugeCustomLabel1.Value = hs.GlobalMileage / 1000;
        //        gaugeControl2.LinearScales[0].CustomLabels.Add(gaugeCustomLabel1);
        //    }
        //    //this.bar1.Visible = false;
        //}

        //private void MainToMaxForm()
        //{
        //    if (!_bFullScreenMode)
        //    {
        //        f.gf = this;
        //        f.Controls.Add(this.axTE3DWindow1);
        //        f.formState.Maximize(f);
        //        f.Show();
        //        _bFullScreenMode = true;
        //    }
        //}

        #endregion

        void btnRotate_Click(object sender, EventArgs e)
        {
            //var sgworld = new SGWorld66();
            IPosition66 resPos = sgworld.Navigate.GetPosition(AltitudeTypeCode.ATC_TERRAIN_RELATIVE); 
            resPos = sgworld.Creator.CreatePosition(resPos.X, resPos.Y, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, resPos.Yaw + 5, resPos.Pitch, resPos.Roll,1500 );//resPos.Altitude
            sgworld.Navigate.FlyTo(resPos);
        }

        //void btnFullScr_Click(object sender, EventArgs e)
        //{
        //    if (!_bFullScreenMode)
        //    {
        //        f.gf = this;
        //        f.Controls.Add(axTE3DWindow1);
        //        f.formState.Maximize(f);
        //        f.Show();
        //        _bFullScreenMode = true;
        //    }
        //}

        //void btnTimeCtrl_Click(object sender, EventArgs e)
        //{
        //    btnTimeCtrl.Checked = !btnTimeCtrl.Checked;
        //    sgworld.Command.Execute(1065, 4);  // time slider

        //}

        //void btnCloseDetail_Click(object sender, EventArgs e)
        //{
        //    mainContainer.Panel2Collapsed = true;
        //    bar1.Visible = false;
        //}

        //void btnTrain_Click(object sender, EventArgs e)
        //{
        //    btnTrain.Checked = !btnTrain.Checked;
 
        //}



        private void btnMessage_Click(object sender, EventArgs e)
        {
            btnMessage.Checked = !btnMessage.Checked;
            //pnlMessage.Expanded = true;
            //pnlMessage.Visible = btnMessage.Checked;
        }





        bool sgworld_OnSGWorldMessage(string MessageID, string SourceObjectID)
        {
            try
            {
                string projName = sgworld.ProjectTree.GetItemName(SourceObjectID);
                string[] ss = projName.Split('|');
                CRailwayProject rp;
                CRailwayPier pier;
                if (ss.Length == 2)
                {
                    //pnlMessage.Expanded = true;
                    //pnlMessage.Visible = true;
                    btnMessage.Checked = true;
                    switch (ss[0])
                    {
                        case "Con" :
                            skinAnimator1.WaitAllAnimations();
                            skinAnimator1.Hide(advPropertyGrid1);
                            advPropertyGrid1.SelectedObject = gRWScene.getConsByName(ss[1]);
                            skinAnimator1.WaitAllAnimations();
                            skinAnimator1.Show(advPropertyGrid1);
                            break;
                        case "Fir":
                            skinAnimator1.WaitAllAnimations();
                            skinAnimator1.Hide(advPropertyGrid1);
                            advPropertyGrid1.SelectedObject = gRWScene.getFirmByName(ss[1]);
                            skinAnimator1.WaitAllAnimations();
                            skinAnimator1.Show(advPropertyGrid1);
                            break;
                        case "Prj":
                            skinAnimator1.WaitAllAnimations();
                            skinAnimator1.Hide(advPropertyGrid1);
                            rp = gRWScene.getProjectByName(ss[1]);
                            pictureBox2.Image = generate2DCode(@"http://jqmis.cn/S/" + rp.mSerialNo);
                            advPropertyGrid1.SelectedObject = rp ;
                            skinAnimator1.WaitAllAnimations();
                            skinAnimator1.Show(advPropertyGrid1);
                            break;            
                        case "Pie":
                            skinAnimator1.WaitAllAnimations();
                            skinAnimator1.Hide(advPropertyGrid1);
                            pier = gRWScene.getPierBySNo(ss[1]);
                            pictureBox2.Image = generate2DCode(@"http://jqmis.cn/S/" +  pier.mSerialNo);
                            advPropertyGrid1.SelectedObject = pier ;
                            skinAnimator1.WaitAllAnimations();
                            skinAnimator1.Show(advPropertyGrid1);
                            break;   
                            
                        default:
                            break;
                    }
                }

                

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return true;
        }
        /// <summary>
        /// 拼接定位、指令 字符串 @"http://jqmis.cn/N/" + mInstanceID + "/" + rp.mSerialNo
        /// 拼接手机同步串 @"http://jqmis.cn/S/" + rp.mSerialNo
        /// </summary>
        /// <param name="srcStr"></param>
        /// <returns></returns>
        private Image generate2DCode(string srcStr)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            //String encoding = cboEncoding.Text;
            //if (encoding == "Byte")
            //{
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            //}
            //else if (encoding == "AlphaNumeric")
            //{
            //    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
            //}
            //else if (encoding == "Numeric")
            //{
            //    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.NUMERIC;
            //}
            //try
            //{
                //int scale = Convert.ToInt16(txtSize.Text);
                qrCodeEncoder.QRCodeScale = 4;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Invalid size!");
            //    return;
            //}
            //try
            //{
            //    int version = Convert.ToInt16(cboVersion.Text);
                qrCodeEncoder.QRCodeVersion = 4;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Invalid version !");
            //}

            //string errorCorrect = cboCorrectionLevel.Text;
            //if (errorCorrect == "L")
            //    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
            //else if (errorCorrect == "M")
            //    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            //else if (errorCorrect == "Q")
            //    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
            //else if (errorCorrect == "H")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;

            Image image;
            //String data = txtEncodeData.Text;
            image = qrCodeEncoder.Encode(srcStr);
            
            return image;  
        }

        
        //void auto_Nav_Panel()
        //{
        //    //for ()
        //    listView1.Clear();
        //    listView1.Items.Clear();
        //    ImageList imgList = new ImageList();

        //    imgList.ImageSize = new Size(1, 20);// 设置行高 20 //分别是宽和高

        //    listView1.SmallImageList = imgList; //这里设置listView的SmallImageList ,用imgList将其撑大
        //}

        void sgworld_OnDateTimeChanged(object DateTime)
        {
            Console.WriteLine(DateTime);
            //mTEScene.mTECons.fromDate = DateTime.Now.AddDays(-30).Date.ToString("u");
            //mTEScene.mTECons.TEUpdate("Cons");
        }

        //private void trackTime_ValueChanged(object sender, EventArgs e)
        //{
        //    double t = (double)(trackTime.Value - trackTime.Minimum) / (trackTime.Maximum - trackTime.Minimum);

        //    int t2 = (int)((eTime.DayOfYear - sTime.DayOfYear) * t);
        //    DateTime dt = sTime.AddDays(t2);
        //    if (dt.ToShortDateString().Equals(curTime.ToShortDateString()))
        //        return;

        //    curTime = dt;
        //    isTimeChanged = true;

        //    trackTime.Tooltip = dt.ToString();
        //    trackTime.Text = dt.ToLongDateString();

        //    trackTime.ShowToolTip();
        //    sgworld.DateTime.Current = dt.AddHours(12);
            

            
        //    mTEScene.fromDate = dt.AddDays(-7);
        //    mTEScene.toDate = dt;

        //}

        private void trackTime_MouseUp(object sender, MouseEventArgs e)
        {
            if (isTimeChanged)
            {
                isTimeChanged = false;
                mTEScene.updateProjectTree();
            }
        }

        //private void sgworld_OnObjectAction(string ObjectID, IAction66 Action)
        //{
        //    //throw new NotImplementedException();
        //    //Console.WriteLine("ObjectID:" + ObjectID + ",action:" + Action.Code.ToString());
        //    //if (Action.Code == ActionCode.AC_STOP)
        //    //{
        //    //    //isNav = false;
        //    //    //Console.WriteLine("fly to ..." + sgworld.ProjectTree.GetItemName(ObjectID));
        //    //    //IPosition66 resPos = sgworld.Navigate.GetPosition(AltitudeTypeCode.ATC_TERRAIN_RELATIVE);
        //    //    //resPos = sgworld.Creator.CreatePosition(resPos.X, resPos.Y, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, resPos.Yaw + 5, resPos.Pitch, resPos.Roll, 1500);//resPos.Altitude
        //    //    //sgworld.Navigate.FlyTo(resPos, ActionCode.AC_ARCPATTERN);

        //    //}
        //    if (Action.Code == ActionCode.AC_WAYPOINT_REACHED)
        //    //if (sgworld.ProjectTree.GetItemName(ObjectID) == "Train" && Action.Code == ActionCode.AC_WAYPOINT_REACHED)
        //    {
        //        //Console.WriteLine(sgworld.ProjectTree.GetItemName(ObjectID));
        //        mContbeamNav.GotoNextWayPoint(ObjectID);

        //    }
        //}


        #region 分项施工进度面板
        private void panel2_Resize(object sender, EventArgs e)
        {
            int x = panel2.Location.X;
            int y = panel2.Location.Y;
            int w = panel2.Size.Width;
            int h = panel2.Size.Height;
            
            panel3.Location = new Point(0,0);
            panel3.Size = new System.Drawing.Size(w,h / 3);
            panel4.Location = new Point(0, h * 2/ 3);
            panel4.Size = new System.Drawing.Size(w, h / 3);
            panel5.Location = new Point(0, h / 3);
            panel5.Size = new System.Drawing.Size(w, h / 3);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboxChange(comboBox1, advPropertyGrid2, 0);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboxChange(comboBox2, advPropertyGrid3, 2);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboxChange(comboBox3, advPropertyGrid4, 1);
        }

        private void comboxChange(ComboBox cb, AdvPropertyGrid pg, int cbID)
        {
            if (cb.Tag != null && cb.Tag == advPropertyGrid1.SelectedObject)
            {
                CRailwayProject pro = (CRailwayProject)cb.Tag;
                if (cb.SelectedIndex >=0 && cb.SelectedIndex < pro.FXProgress.Count){
                    pro.selectedFXid[cbID] = cb.SelectedIndex;
                    CFXProj fx = pro.FXProgress[pro.selectedFXid[cbID]];
                    pg.Hide();
                    pg.SelectedObject = fx;
                    skinAnimator1.Show(pg);
                    chart1.Series[cbID].Points.Clear();
                    for (int j = 0; j < fx.strDate.Count; j++)
                        chart1.Series[cbID].Points.AddXY(DateTime.ParseExact(fx.strDate[j], "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture),
                    Math.Round(fx.doneAmount[j] / fx.TotalAmount * 100, 1));
                }
             }
        }

        private void showProjectDetail(Object obj)
        {

            //           series1.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.LeftRight;
            //series1.ChartArea = "ChartArea1";
            //series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            //series1.Name = "分项工程1";
            //series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Date;
            //series1.YValuesPerPoint = 10;
            //series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            try
            {
                
                if (!(obj is CRailwayProject) || ((CRailwayProject)obj).FXProgress.Count==0)
                {
                    //comboBox1.Enabled = false;
                    //comboBox2.Enabled = false;
                    //comboBox3.Enabled = false;
                    panel2.Visible = false;
                    chart1.Visible = false;
                    return;
                }
                panel2.Visible = true;
                chart1.Visible = true;
                if (obj == advPropertyGrid1.SelectedObject) return;
                advPropertyGrid1.SelectedObject = obj;
                CRailwayProject pro = (CRailwayProject)obj;
                //currentProject = pro;
                Color[] sc = new Color[] { Color.Red, Color.Green, Color.Blue };


                if (pro.FXProgress != null && pro.FXProgress.Count > 0)
                {
                    //int ilimit = 1;
                    //List<string> ls = new List<string>();
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;

                    comboBox1.Items.Clear();
                    comboBox1.Tag = pro;
                    comboBox2.Items.Clear();
                    comboBox2.Tag = pro;
                    comboBox3.Items.Clear();
                    comboBox3.Tag = pro;
                    foreach (CFXProj fx in pro.FXProgress)
                    {
                        comboBox1.Items.Add(fx.FxName);
                        comboBox2.Items.Add(fx.FxName);
                        comboBox3.Items.Add(fx.FxName);
                    }

                    comboBox1.SelectedIndex = pro.selectedFXid[0];
                    //advPropertyGrid2.SelectedObject = pro.FXProgress[pro.selectedFXid[0]];
                    if (pro.selectedFXid[1] != -1)
                    {
                        comboBox3.SelectedIndex = pro.selectedFXid[1];
                        //advPropertyGrid4.SelectedObject = pro.FXProgress[pro.selectedFXid[1]];
                        //ilimit = 2;
                    }
                    if (pro.selectedFXid[2] != -1)
                    {
                        comboBox2.SelectedIndex = pro.selectedFXid[2];
                        //advPropertyGrid3.SelectedObject = pro.FXProgress[pro.selectedFXid[2]];
                        //ilimit = 3;
                    }

                }
                

            }
            catch (Exception ex2)
            {
                Console.WriteLine(obj.ToString()+ "无分项工程");
            }
        }
        #endregion


        private void btnTimeCtrl_Click(object sender, EventArgs e)
        {

        }
        #region 存储数据到本地sqlite数据库


        private void btnSave_Click(object sender, EventArgs e)
        {
            gRWScene.saveProjectLocal(CGisDataSettings.gLocalDB);
        }

        private void btnSaveDW_Click(object sender, EventArgs e)
        {
            gRWScene.savePierLocal(CGisDataSettings.gLocalDB);
        }

        private void btnProgress_Click(object sender, EventArgs e)
        {
            gRWScene.saveProjFXLocal(CGisDataSettings.gLocalDB);
        }

        private void btnSaveFirm_Click(object sender, EventArgs e)
        {
            gRWScene.saveFirmLocal(CGisDataSettings.gLocalDB);
        }

        private void btnSaveCons_Click(object sender, EventArgs e)
        {
            gRWScene.saveConsLocal(CGisDataSettings.gLocalDB);
        }

        #endregion

        private void timerScan_Tick(object sender, EventArgs e)
        {
            
            CRailwayProject rp = gRWScene.GetProjectBy2DCode(mInstanceID);
            if (rp != null && rp != curProject)
            {
                IPosition66 pos = sgworld.Creator.CreatePosition(rp.CenterLongitude, rp.CenterLatitude, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -70, 0, 2000);
                sgworld.Navigate.FlyTo(pos, ActionCode.AC_FLYTO);
                curProject = rp;
            }
            //fillEventGrid();
            int count = CConsLog.refreshMsgLog();
            if (count>0)
            {
                Random random = new Random();
                
                string str = " ";
                for (int i = 0; i < 3; i++)
                {
                    int n = random.Next(count) + random.Next(count) - count;
                    if (n < 0) n = -n;
                    if (CConsLog.mLogInfo[n] == null) continue;
                    str += CConsLog.mLogInfo[n] + "\t" + CConsLog.mLogTime[n].ToShortTimeString() + "\n";
                }
                ToastNotification.Show(this,str, null,   3000,
                    (eToastGlowColor)( eToastGlowColor.Red), (eToastPosition)( eToastPosition.TopCenter));
            }
        }

    }
}