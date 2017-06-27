using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using System.Text;
using ModelInfo;
using ModelInfo.Helper;
using RailwayGIS.TEProject;
using System.Threading;
using System.Reflection;

using TerraExplorerX;

namespace RailwayGIS
{

    public enum ScanCommandStatus { NoCommand, Commanding, Leaving};
    public partial class FullScreenForm : Form
    {
        public FormState formState = new FormState();   

        //二维工程进度面板
        private PanelProgress2D m2DProgressPanel;     
        
        //漫游过程调度     
        private CTEPresentation mNavigationControl;
        //漫游队列以及漫游状态
        List<CHotSpot> mNavigatioinList = null;
        bool isAutoNav = false;
        
        // 当前窗口实例号，在服务器端可以标识的客户端ID
        private int mInstanceID;

        // 中心点的坐标，FIXME，比较混乱，需要优化
        private static double oldXl = 117;
        private static double oldYl = 35;
        private static double centerX = 117, centerY = 35;
        private static double mViewRadius = -1;

        private ScanCommandStatus mCmdStatus = ScanCommandStatus.NoCommand;
        //GIS客户端是否已经加载
        private static bool isGISLoaded = false;

        // GIS场景数据
        CRailwayScene gRWScene;

        // Terra Explorer 对RailwayGIS的场景展示，如果替换其他GIS系统，只修改该模块
        CTEScene mTEScene;

        // Terra Explorer 插件
        public SGWorld66 sgworld;

        // FIXME 没有用好
        public IPosition66 mInitialPosition;
        private static Font defaultFont = new Font("宋体", 12);
        private static Pen defaultPen = new Pen(Color.Black, 1);
        private static Pen redPen = new Pen(Color.Red, 1);

        //扫一扫字符串命令导航，不断读取手机端用户控制，接收手机端交互指令
        List<string> gMobileCommand = new List<string>();  // 

        //ImageList imgListCmd = new ImageList(); // 命令导航的二维码图像

        //ImageList imgListPrj = new ImageList();

        public FullScreenForm()
        {
            LoginForm login = new LoginForm();
            if (login.ShowDialog() != DialogResult.OK)
            {
                Environment.Exit(-1);
            }
            WelcomeFormJQ wf = new WelcomeFormJQ();
            wf.Show(this);

            gRWScene = new CRailwayScene(CGisDataSettings.gLocalDB, CGisDataSettings.gCurrentProject.projectUrl);

            Random ran = new Random();
            mInstanceID = ran.Next(100, 999);
            InitializeComponent();
            this.Text = "新建济青高速铁路工程:" + mInstanceID;

            sgworld = new SGWorld66();


            // 初始化二维形象进度
            m2DProgressPanel = new PanelProgress2D(gRWScene);
            mainContainer.Panel2.Controls.Add(m2DProgressPanel);
            m2DProgressPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            m2DProgressPanel.Location = new System.Drawing.Point(0, 0);
            m2DProgressPanel.Name = "panel2D";
    
            //initPrj2DImgList();
            //panelPrjOutline.Hide();

            initScanCmd();
            wf.Close();

            formState.Maximize(this);

        }


        //private void initPrj2DImgList()
        //{
        //    imgListPrj.ImageSize = new Size(144, 144);
        //    foreach (CRailwayProject prj in gRWScene.mContBeamList)
        //    {
        //        imgListPrj.Images.Add(CGeneralHelpers.generate2DCode(@"http://jqmis.cn/N/" + mInstanceID + "/" + prj.mSerialNo));
        //    }
        //}

        //private void showPrj2DListView(int id)
        //{
        //    ImageList imgList = new ImageList();
        //    imgList.ImageSize = new Size(144, 144);
        //    int fromID = (id - 6 >= 0) ? id - 6 : 0;
        //    listView1.Items.Clear();

        //    for (int i = fromID; i < fromID + 15 && i < imgListPrj.Images.Count; i++)
        //    {
        //        imgList.Images.Add(imgListPrj.Images[i]);
        //        ListViewItem lvi = new ListViewItem();
        //        lvi.Text = gRWScene.mContBeamList[i].ProjectName;
        //        lvi.ImageIndex = i - fromID;
        //        listView1.Items.Add(lvi);
        //    }

        //    listView1.View = View.LargeIcon;
        //    listView1.LargeImageList = imgList;
        //    if (id - fromID < listView1.Items.Count)
        //        listView1.Items[id - fromID].Selected = true;
        //}


        private void GISForm_Load(object sender, EventArgs e)
        {
            // Register to OnLoadFinished globe event

            sgworld.OnLoadFinished += new _ISGWorld66Events_OnLoadFinishedEventHandler(OnProjectLoadFinished);

            // Open Project in asynchronous mode

            string tProjectUrl = CGisDataSettings.gDataPath + CGisDataSettings.gCurrentProject.projectLocalPath + "Default.fly";
            sgworld.Project.Open(tProjectUrl, true, null, null);//          sgworld.Project.Open(@"F:\地图\MNGis.mpt", true, null, null);
            //sgworld.DateTime.DisplaySun = false;
            sgworld.Command.Execute(1026, 0);  // sun
            //sgworld.Command.Execute(2118, 0);
            //sgworld.Command.Execute(1065, 4);

        }

        void OnProjectLoadFinished(bool bSuccess)
        {

            mInitialPosition = sgworld.Creator.CreatePosition(118.6, 36.6, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE,
                341, -90.0, 0, 445000);
            //Camera.initial_latitude, Camera.initial_longitude, Camera.initial_heading, Camera., Camera.initial_tilt
            sgworld.Navigate.FlyTo(mInitialPosition, ActionCode.AC_FLYTO);
            if (mTEScene == null)
                mTEScene = new CTEScene(gRWScene,timerFlyaround);
            //mTEScene.ShowFinished += MTEScene_ShowFinished;
            //mTEScene.createProjectTree();      

            //sgworld.OnMouseWheel += new _ISGWorld66Events_OnMouseWheelEventHandler(OnMouseWheel);
            //sgworld.OnProjectTreeAction += new _ISGWorld66Events_OnProjectTreeActionEventHandler(OnProjectTreeAction);

            //sgworld.OnLButtonUp += OnLButtonUp;
            //sgworld.OnRButtonUp += sgworld_OnRButtonUp;
            //sgworld.OnDateTimeChanged += sgworld_OnDateTimeChanged;
            //sgworld.OnSGWorldMessage += sgworld_OnSGWorldMessage;
            //sgworld.OnObjectAction += sgworld_OnObjectAction;
            //sgworld.OnDrawHUD += sgworld_OnDrawHUD;
            timer2DSyncronize.Start();

            mNavigatioinList = gRWScene.GetDefaultNavPathByMileage();
            //curNavList.RemoveAt(0); // FIXME
            //mNavigatioinList.AddRange( gRWScene.GetContBeamNavPath());
            //mNavigatioinList = gRWScene.GetContBeamNavPath();
            mNavigationControl = new CTEPresentation(gRWScene, mTEScene, m2DProgressPanel,panelInfo1, timerFlyaround, mNavigatioinList);//,timerPresentation

            //mNavigationControl.OnToOnePlace += nav_ToOnePlace;
            //sgworld.OnPresentationFlyToReachedDestination += Sgworld_OnPresentationFlyToReachedDestination;

            //mContbeamNav.startPresentation();
            //auto_Nav(ls);

        }

        //private void MTEScene_ShowFinished()
        //{
        //    mNavigationControl.startNextPresentation();
        //}


        private void initScanCmd()
        {
            gMobileCommand.Add(@"NavStart");  // 开始
            gMobileCommand.Add(@"NavAccelerate");  // 加速
            gMobileCommand.Add(@"NavDeAccelerate"); // 减速
            gMobileCommand.Add(@"NavRestart"); // 重新开始
            gMobileCommand.Add(@"NavNext5"); // 快进，下5个
            gMobileCommand.Add(@"NavPrevious3"); // 快退，之前3个
            gMobileCommand.Add(@"NavActive");  // 工点排序，有照片的优先，最近更新优先
            gMobileCommand.Add(@"NavStationOnly"); // 工点排序，只站房停留
            gMobileCommand.Add(@"NavConsLogOnly");  // 工点排序，只七天内有实名信息的工点          
            gMobileCommand.Add(@"NavAll"); // 工点分类（桥，连续梁，路基，隧道），每类按里程
            pictureBox1.Image = CGeneralHelpers.generate2DCode(@"http://jqmis.cn/N/" + mInstanceID + "/" + @"NavStart");
            //gNavCmd.Add(@"NavStart");
            //gNavCmd.Add(@"NavStop");
            //imgListCmd.ImageSize = pictureBox1.Size;
            //for (int i = 0; i < gMobileCommand.Count; i++)
            //{
            //    imgListCmd.Images.Add(CGeneralHelpers.generate2DCode(@"http://jqmis.cn/N/" + mInstanceID + "/" + gMobileCommand[i]));
            //}
            //pictureBox1.Image = imgListCmd.Images[isAutoNav ? 1 : 0];
        }



        //private void btnStop_Click(object sender, EventArgs e)
        //{
        //    CTEPresentation.stopPresentation();
        //    formState.Restore(this);
        //    Hide();
        //    //gf.MaxToMainForm();
        //}

        //public void Nav(CRailwayScene rs, List<CHotSpot> hs)
        //{
        //    CTEPresentation.initPresentation(rs, hs, OnReachToOnePlace, timerNav);
        //}

        //public void nav_ToOnePlace(CHotSpot hs1, CHotSpot hs2)
        //{

        //    if (mNavigatioinList == null) return;

        //    switch (hs1.ObjectType)
        //    {
        //        case "Project":
        //            CRailwayProject rp = (CRailwayProject)(hs1.ObjectRef);

        //            //StringBuilder str = new StringBuilder();
        //            string s = "当前位置： " + rp.ToString() + "\t\t";
        //            //if (hs2 != null)
        //            //    s += "下一目标：" + ((CRailwayProject)hs1.ObjectRef).ToString() + "\t";
        //            labelRoll2.Text = s;
        //            labelRoll2.Refresh();

        //            mTEScene.showTEProject(rp);


        //            break;
        //        case "Firm":
        //            CRailwayFirm rf = (CRailwayFirm)(hs1.ObjectRef);
        //            mTEScene.showTEFirms(rf);
        //            break;
        //        case "Cons":
        //            break;
        //    }

        //    //str.Append("下一目标 ");
        //    //str.Append(rp);

        //    //List<CHotSpot> ls;
        //    //ls = gRWScene.GetConsByMileage(centerX,centerY, mViewRadius + 1000);
        //    //str.Append(s);
        //    //foreach (CHotSpot hs in ls)
        //    //{
        //    //    ConsLocation cl = (ConsLocation)(hs.ObjectRef);

        //    //    str.Append(hs.DKCode);
        //    //    str.Append(hs.Mileage);
        //    //    str.Append(":本周 ");
        //    //    str.Append(cl.Number);
        //    //    str.Append(" 人次于");
        //    //    str.Append(cl.ProjName);
        //    //    str.Append("\t");
        //    //    str.Append(cl.ProjDWName);
        //    //    str.Append("工作\t\t");
        //    //    str.Append(s);
        //    //}

        //    //        break;
        //    //        //int height = ((double)id / curNavList.Count * listViewEx1.Height);
        //    //        //listViewEx1.AutoScrollOffset = listViewEx1.Items[gRWScene.mContBeamList.IndexOf((CRailwayProject)hs.ObjectRef)];
        //    //    }

        //    //}


        //}



        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //string urlstr = "http://" + CGisDataSettings.gCurrentProject.projectUrl + "/APP/ProjectMng.aspx?f=detail&shownav=1&sn=" + p.mSerialNo +
            //           "&uacc=" + CGisDataSettings.gCurrentProject.userName + "&upwd=" + CGisDataSettings.gCurrentProject.userPSD;//&showtab=1
            //if (p.mIsValid)
            //    CTEPresentation.initPresentation(gRWScene, p.getNavPath(), OnReachToOnePlace, timerNav);
            //else
            //    sgworld.Navigate.FlyTo(sgworld.Creator.CreatePosition(p.CenterLongitude, p.CenterLatitude, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -35.0, 0, 800));
            //showProjectDetail(p);

            switch (keyData)
            {
                case Keys.Escape:

                    if (isAutoNav)
                    {
                        if (MessageBox.Show("是否退出漫游?", "退出", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            mNavigationControl.stopPresentation();
                            isAutoNav = false;
                            //pictureBox1.Image = imgListCmd.Images[isAutoNav ? 1 : 0];

                        }

                    }
                    else if (MessageBox.Show("是否退出系统?", "退出", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Application.Exit();
                    }

                    return true;
                case Keys.Up:
                    handleScanCmd(@"NavAccelerate");
                    return true;
                case Keys.Down:
                    handleScanCmd(@"NavDeAccelerate");
                    return true;
                case Keys.Left:
                    handleScanCmd(@"NavPrevious3");
                    return true;
                case Keys.Right:
                    handleScanCmd(@"NavNext5");
                    return true;
                case Keys.PageDown:
                    handleScanCmd(@"NavRestart");
                    return true;
                //case Keys.F1:
                //    handleScanCmd(@"NavRestart");
                //    //gMobileCommand.Add(@"NavAccelerate");  // 加速
                //    //gMobileCommand.Add(@"NavDeAccelerate"); // 减速
                //    //gMobileCommand.Add(@"NavRestart"); // 重新开始
                //    //gMobileCommand.Add(@"NavNext5"); // 快进，下5个
                //    //gMobileCommand.Add(@"NavPrevious3"); // 快退，之前3个
                //    //gMobileCommand.Add(@"NavActive");  // 工点排序，有照片的优先，最近更新优先
                //    //gMobileCommand.Add(@"NavStationOnly"); // 工点排序，只站房停留
                //    //gMobileCommand.Add(@"NavConsLogOnly");  // 工点排序，只七天内有实名信息的工点          
                //    //gMobileCommand.Add(@"NavAll"); // 工点分类（桥，连续梁，路基，隧道），每类按里程

                //    return true;
                //case Keys.F2:
                //    handleScanCmd(@"NavAccelerate");
                //    return true;
                //case Keys.F3:
                //    handleScanCmd(@"NavNext5");
                //    return true;
                //case Keys.F4:
                //    handleScanCmd(@"NavConsLogOnly");
                //    return true;
                case Keys.F5:
                    {
                        if (!isAutoNav)
                        {
                            //mPre?.Stop();
                            //mNavigationControl.setStartPosition(0);
                            mNavigationControl.startNextPresentation();
                            isAutoNav = true;
                            //pictureBox1.Image = imgListCmd.Images[isAutoNav ? 1 : 0];
                        }
#if DEBUG
                        Console.WriteLine("Navigate Started:" + DateTime.Now.ToShortTimeString());
#endif 
                    }
                    return true;
                case Keys.F6:
                    //{
                    //    gRWScene.saveProjectLocal(CGisDataSettings.gLocalDB);
                    //    gRWScene.saveProjFXLocal(CGisDataSettings.gLocalDB);
                    //    gRWScene.savePierLocal(CGisDataSettings.gLocalDB);
                    //    gRWScene.saveFirmLocal(CGisDataSettings.gLocalDB);
                    //    gRWScene.saveConsLocal(CGisDataSettings.gLocalDB);
                    //    Console.WriteLine("Save Completed:" + DateTime.Now.ToShortTimeString());
                    //}
                    return true;
                case Keys.F7:
                    if (isAutoNav)
                    {
                        isAutoNav = false;
                        mNavigationControl.stopPresentation();
                        gRWScene.updateSceneFromServer();
                        //    mNavigatioinList = gRWScene.GetContBeamNavPath();
                        //    mNavigationControl.mNavigationList = mNavigatioinList;
                        mTEScene.updateProjectTree();
                        isAutoNav = true;
                        mNavigationControl.startNextPresentation();
                    }
                    else
                    {
                        gRWScene.updateSceneFromServer();
                        mTEScene.updateProjectTree();
                    }

                    return true;
                case Keys.F8:
                    //gRWScene.saveProjectLocal(CGisDataSettings.gLocalDB);
                    //gRWScene.saveProjFXLocal(CGisDataSettings.gLocalDB);
                    //gRWScene.savePierLocal(CGisDataSettings.gLocalDB);
                    //gRWScene.saveFirmLocal(CGisDataSettings.gLocalDB);
                    //gRWScene.saveConsLocal(CGisDataSettings.gLocalDB);
                    return true;

            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


        // 刷新2D形象进度面板
        private void timer2DSynchronize_Tick(object sender, EventArgs e)
        {
            try
            {
                bool isInside = true;
                IPosition66 cp = sgworld.Window.CenterPixelToWorld().Position;// 检测屏幕中心经纬度
                centerX = cp.X;
                centerY = cp.Y;
                if (centerX < 115 || centerX > 125 || centerY < 32 || centerY > 40)
                    isInside = false; 
                else
                    isGISLoaded = true;

                if (isGISLoaded && isAutoNav && mNavigationControl.readyForNext && mTEScene.prjInfoForm.ws == ShowFormStatus.eClosing)
                {
                    mTEScene.prjInfoForm.ws = ShowFormStatus.eReadyForNext;
                    mNavigationControl.startNextPresentation();
                }
                if (isGISLoaded)
                {
                    labelRoll1.Refresh();
                    //labelRoll2.Refresh();
                }

                if (!isInside)
                    return;

                IWorldPointInfo66 w0 = sgworld.Window.PixelToWorld(axTE3DWindow1.Size.Width - 10, axTE3DWindow1.Size.Height - 10);
                double lx = w0.Position.X;
                double ly = w0.Position.Y;

                double distanceOffset = CoordinateConverter.getUTMDistance(lx, ly, oldXl, oldYl);
                if (distanceOffset < 10)
                    return;

                oldXl = lx;
                oldYl = ly;

                double dis;
                //bool isInside;

                double mileageCenter;
                gRWScene.mMainPath.getPathMileagebyGPS(cp.X, cp.Y, out mileageCenter,out dis);
                //gRWScene.mMiddleLines.get
                mViewRadius = CoordinateConverter.getUTMDistance(cp.X, cp.Y, lx, ly);  //屏幕半径对应的实际距离
                mViewRadius = Math.Min(mViewRadius, 200000);

                if ( m2DProgressPanel.Visible && !isAutoNav) // mileageCenter >= 0 &&
                {
                    m2DProgressPanel.update2DPanel(mileageCenter, mViewRadius);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("工点状态每秒定时更新错误");
            }


        }

        //        private bool isNewCmd = false;

        public delegate bool AsyncScanCmdDelegate();
        private bool asyncScanCmd()
        {
            try
            {
                string str = CServerWrapper.getScanMsg(mInstanceID);
                if (str == null) return false;
                if (str == "") return false;

                if (timerScanCommand.Tag != null && timerScanCommand.Tag.ToString().Equals(str))
                    return false;
                timerScanCommand.Tag = str;
                Console.WriteLine("扫描指令"+str);
                //timerCommand.InvokeIfRequired(l => l.Tag = str); 
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        void handleScanCmd(IAsyncResult ar)
        {
            //AsyncScanCmdDelegate dlgt = (AsyncScanCmdDelegate)ar.AsyncState;
            //int speed;
            if (mdlgt.EndInvoke(ar))
            {
                if (handleScanCmd(timerScanCommand.Tag.ToString()))
                {
                    CServerWrapper.setScanMsg(mInstanceID, "NavCmdSettled");
                    //timerScanCommand.Stop();
                    //timerScanCommand.Interval = 1000;
                    //timerScanCommand.Start();
                    mCmdStatus = ScanCommandStatus.Commanding;
                }
            }
        }

        private void timerNavCmdResponse_Tick(object sender, EventArgs e)
        {
            if (mCmdStatus == ScanCommandStatus.Commanding)
            {
                ToastNotification.Show(this, "10分钟没有指令了，1分钟内没有指令将进入自动控制状态", null, 5000,
            (eToastGlowColor)(eToastGlowColor.Red), (eToastPosition)(eToastPosition.TopCenter));
                timerScanCommand.Interval = 60000;
                timerScanCommand.Start();
                mCmdStatus = ScanCommandStatus.Leaving;
            }
            else
            {
                CGisDataSettings.AppSpeed = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["NavigationSpeed"]);
                // 五级速度，1，2，4，8，16倍速，对应20，40，80，160，320KM
                CGisDataSettings.AppSpeed = Math.Min(5, Math.Max(CGisDataSettings.AppSpeed, 1));
                mCmdStatus = ScanCommandStatus.NoCommand;
            }

        }

        private bool handleScanCmd(string cmd)
        {
            string str;
            switch (cmd)
            {
                case @"NavStartPause":
                    if (!isAutoNav)
                    {
                        mNavigationControl.startNextPresentation();
                        isAutoNav = true;
                        ToastNotification.Show(this, "启动线路漫游", null, 5000,
                                           (eToastGlowColor)(eToastGlowColor.Red), (eToastPosition)(eToastPosition.TopCenter));
                    }
                    else {
                        mNavigationControl.stopPresentation();
                        isAutoNav = false;
                        ToastNotification.Show(this, "暂停线路漫游", null, 5000,
                   (eToastGlowColor)(eToastGlowColor.Red), (eToastPosition)(eToastPosition.TopCenter));

                    }
                    return true;
                case @"NavStart":
                    if (mCmdStatus == ScanCommandStatus.NoCommand)
                    {
                        ToastNotification.Show(this, "欢迎参观新建济青铁路工程,请使用移动端控制", null, 5000,
                    (eToastGlowColor)(eToastGlowColor.Red), (eToastPosition)(eToastPosition.TopCenter));
                    }
                    else if (mCmdStatus == ScanCommandStatus.Leaving)
                    {
                        ToastNotification.Show(this, "欢迎参观新建济青铁路工程,收到继续指令", null, 5000,
                    (eToastGlowColor)(eToastGlowColor.Red), (eToastPosition)(eToastPosition.TopCenter));

                    } else  // 指令状态
                    {
                        ToastNotification.Show(this, "已经建立连接，请使用移动端控制", null, 5000,
                    (eToastGlowColor)(eToastGlowColor.Red), (eToastPosition)(eToastPosition.TopCenter));
                    }

                    return true;
                case @"NavSpeedSlow":
                    CGisDataSettings.AppSpeed=3;
                    mNavigationControl.changeNavTrainSpeed();
                    return true;
                case @"NavSpeedNomal":
                    CGisDataSettings.AppSpeed = 4;
                    mNavigationControl.changeNavTrainSpeed();
                    return true;
                case @"NavSpeedFast":
                    CGisDataSettings.AppSpeed = 5;
                    mNavigationControl.changeNavTrainSpeed();
                    return true;
                case @"NavAccelerate":
                    if (CGisDataSettings.AppSpeed == 5)
                    {
                        ToastNotification.Show(this, "当前已经达到最高速320KM", null, 5000,
                    (eToastGlowColor)(eToastGlowColor.Red), (eToastPosition)(eToastPosition.TopCenter));
                    }
                    else
                    {
                        CGisDataSettings.AppSpeed++;
                        switch (CGisDataSettings.AppSpeed)
                        {
                            case 1: str = "最慢速（20KM）";
                                break;
                            case 2: str = "慢速（40KM）";
                                break;
                            case 3: str = "中速（80KM）";
                                break;
                            case 4: str = "快速（160KM）";
                                break;
                            default:
                                str = "最高速（320KM）";
                                break;

                        }
                        ToastNotification.Show(this, "当前调整为" + str, null, 5000,
(eToastGlowColor)(eToastGlowColor.Red), (eToastPosition)(eToastPosition.TopCenter));
                        mNavigationControl.changeNavTrainSpeed();
                    }
                    return true; 

                case "NavDeAccelerate":
                    if (CGisDataSettings.AppSpeed == 1)
                    {
                        ToastNotification.Show(this, "当前已经达到最慢速20KM", null, 5000,
                    (eToastGlowColor)(eToastGlowColor.Red), (eToastPosition)(eToastPosition.TopCenter));
                    }
                    else
                    {
                        CGisDataSettings.AppSpeed--;
                        switch (CGisDataSettings.AppSpeed)
                        {
                            case 1:
                                str = "最慢速（20KM）";
                                break;
                            case 2:
                                str = "慢速（40KM）";
                                break;
                            case 3:
                                str = "中速（80KM）";
                                break;
                            case 4:
                                str = "快速（160KM）";
                                break;
                            default:
                                str = "最高速（320KM）";
                                break;

                        }
                        ToastNotification.Show(this, "当前调整为" + str, null, 5000,
(eToastGlowColor)(eToastGlowColor.Red), (eToastPosition)(eToastPosition.TopCenter));
                        
                        mNavigationControl.changeNavTrainSpeed();
                    }

                    return true; 
                case "NavRestart":
                    mNavigationControl.reStartPresentation();
                    return true; 
                case "NavNext5":
                case "NavNext":
                    mNavigationControl.startNextPresentation(5);
                    return true;
                case "NavPrevious3":
                case "NavPrev":
                    mNavigationControl.startNextPresentation(-3);
                    return true;
                case "NavActive":
                    return true;
                case "NavStationOnly":
                    return true;
                case "NavConsLogOnly":
                    mNavigatioinList = gRWScene.GetConsLogNavPath();
                    mNavigationControl.setNavPath(mNavigatioinList);
                    mNavigationControl.startNextPresentation();
                    return true;
                case "NavAll":
                    mNavigatioinList = gRWScene.GetAllNavPathbyMileage();
                    mNavigationControl.setNavPath(mNavigatioinList);
                    mNavigationControl.startNextPresentation();
                    return true;
            }
            return false;
            //gMobileCommand.Add(@"NavPrevious3"); // 快退，之前3个
            //gMobileCommand.Add(@"NavActive");  // 工点排序，有照片的优先，最近更新优先
            //gMobileCommand.Add(@"NavStationOnly"); // 工点排序，只站房停留
            //gMobileCommand.Add(@"NavConsLogOnly");  // 工点排序，只七天内有实名信息的工点          
            //gMobileCommand.Add(@"NavAll"); // 工点分类（桥，连续梁，路基，隧道），每类按里程
            //foreach (CRailwayProject prj in gRWScene.mContBeamList)
            //{
            //    if (str.Equals(prj.mSerialNo))  //&& prj != timerNav.Tag
            //    {
            //        mNavigationControl.stopPresentation();
            //        //double x1, x2, x3, x4;
            //        //prj.getSpecialPoint(1, out x1, out x2, out x3, out x4);
            //        //IPosition66 pos = sgworld.Creator.CreatePosition(x1, x2, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -70, 0, 2000);
            //        //sgworld.Navigate.FlyTo(pos, ActionCode.AC_FLYTO);

            //        //         string urlstr = "http://" + CGisDataSettings.gCurrentProject.projectUrl + "/APP/ProjectMng.aspx?f=detail&shownav=1&sn=" + prj.mSerialNo +
            //        //"&uacc=" + CGisDataSettings.gCurrentProject.userName + "&upwd=" + CGisDataSettings.gCurrentProject.userPSD;//&showtab=1
            //        //         Console.WriteLine(urlstr);

            //        if (mNavigationControl.setStartPresentation(prj))
            //            mNavigationControl.startNextPresentation();

            //        //panelPrjOutline.Hide();

            //        //pictureBox1.Image = imgListCmd.Images[1];
            //        isAutoNav = true;

            //        return;
            //    }
            //}

        }

        // 异步调用web service
        IAsyncResult ar = null;
        AsyncScanCmdDelegate mdlgt;
        private void timerScanCmd_Tick(object sender, EventArgs e)
        {
            if (ar != null && ar.IsCompleted)
            {
                handleScanCmd(ar);
                ar = null;
            }
            else if (ar == null)
            {
                mdlgt = new AsyncScanCmdDelegate(asyncScanCmd);
                //ar = dlgt.BeginInvoke(new AsyncCallback(scanCmdHandle), dlgt);
                ar = mdlgt.BeginInvoke(null, null);
            }

        }

        //private void timerMessageBox_Tick(object sender, EventArgs e)
        //{
        //    //服务器读取消息;

        //    int count = CConsLog.refreshConsLog();
        //    if (count > 0)
        //    {
        //        Random random = new Random();

        //        string str = " ";
        //        for (int i = 0; i < 3; i++)
        //        {
        //            int n = random.Next(count) + random.Next(count) - count;
        //            if (n < 0) n = -n;
        //            if (CConsLog.mLogInfo[n] == null) continue;
        //            str += CConsLog.mLogInfo[n] + "\t" + CConsLog.mLogTime[n].ToShortTimeString() + "\n";
        //        }
        //        ToastNotification.Show(this, str, null, 3000,
        //            (eToastGlowColor)(eToastGlowColor.Red), (eToastPosition)(eToastPosition.BottomCenter));
        //    }
        //}

        //private void panelPrjOutline_VisibleChanged(object sender, EventArgs e)
        //{
        //    if (isGISLoaded)
        //    {
        //        if (panelPrjOutline.Visible)
        //        {
        //            int id = mNavigationControl.getCurrentPlaceIndex();
        //            showPrj2DListView(id);
        //            isAutoNav = false;
        //            timerNavCmdResponse.Interval = 20000;
        //            timerNavCmdResponse.Start();

        //        }
        //        else
        //        {
        //            isAutoNav = true;
        //            pictureBox1.Image = imgListCmd.Images[1];
        //            mNavigationControl.startNextPresentation();
        //            timerNavCmdResponse.Stop();
        //        }
        //    }

        //}




 
        private int asyncGetMsg()
        {
            return CConsLog.refreshMsgLog();
        }

        public delegate int AsyncDelegate();

        void msgHandle(IAsyncResult ar)
        {
            AsyncDelegate dlgt = (AsyncDelegate)ar.AsyncState;
            int count = dlgt.EndInvoke(ar);
            string str = "";
            for (int i = 0; i < count && i < 5; i++)
            {
                str += CConsLog.mLogInfo[i] + " 时间：" + CConsLog.mLogTime[i].ToShortTimeString() + "\t\t";
            }
            labelRoll1.Invoke(new EventHandler(delegate
            {
                labelRoll1.Text = str;
            }));
            //labelRoll1.Refresh();
        }

        private void labelRoll1_MsgRepeated()
        {
            AsyncDelegate dlgt = new AsyncDelegate(asyncGetMsg);
            IAsyncResult ar = dlgt.BeginInvoke(new AsyncCallback(msgHandle), dlgt);
            //int count = CConsLog.refreshMsgLog();
        }

        //private void labelRoll2_MsgRepeated()
        //{

        //    if (isGISLoaded)
        //    {
        //        StringBuilder str = new StringBuilder("");
        //        //string s = "";
        //        //MyStringBuilder.Append(" What a beautiful day.");



        //        //else  // 非漫游状态，显示全线实名情况
        //        if (!isAutoNav)
        //        {
        //            List<CHotSpot> ls;
        //            ls = gRWScene.GetConsByMileage(centerX,centerY, mViewRadius + 1000);
        //            foreach (CHotSpot hs in ls)
        //            {
        //                ConsLocation cl = (ConsLocation)(hs.ObjectRef);
        //                str.Append(cl.Number);
        //                str.Append(" 人次于");
        //                str.Append(cl.ProjName);
        //                str.Append("工作\t\t");

        //            }
        //            labelRoll2.Text = str.ToString();
                    
        //        }
        //        labelRoll2.Refresh();

        //    }
        //}
    }
}
