using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelInfo;
using System.Windows.Forms;
using TerraExplorerX;


namespace RailwayGIS.TEProject
{

    //public delegate void reachToOnePlace(CHotSpot hs1, CHotSpot hs2);
    //public delegate void leaveOnePlace();

    public class CTEPresentation
    {
        //static string mModelName = CGisDataSettings.gDataPath + CGisDataSettings.TrainPath;// @"Common\Models\Train\su27.3ds";
        static string[] mModelName = {
            CGisDataSettings.gDataPath + @"Common\Models\Train\susplithead.xpl2",
            CGisDataSettings.gDataPath + @"Common\Models\Train\susplit.xpl2",
            CGisDataSettings.gDataPath + @"Common\Models\Train\susplittail.xpl2",
        };
        // 漫游速度
        static double mTopSpeed = Math.Max(0, Math.Min(Math.Pow(2,CGisDataSettings.AppSpeed)  * 20, 350));

        SGWorld66 sgworld;
        CRailwayScene mSceneData;
        CTEScene mTEScene;
        
        ITerrainDynamicObject66[] mDynamicTrain = new ITerrainDynamicObject66[5];
        ITerrainDynamicObject66 mVirtualObj = null;
        double[] rm, rx, ry, rz, rd;
        //ITerrainDynamicObject66 mDynamicWorker = null;
        //ITerrainModel66 mWorker = null;

            // 长距离漫游
        IPresentation66 mPresentation = null;
        public List<CHotSpot> mNavigationList = null;
        string mNavObjGroupID;
        string mPresentationGroupID;


        CTEObject mTeObject;

        double mStepm = 0;
        double mMainMileage = 0; // 记录火车行驶中的当前里程，用于青阳隧道42k-55k

        // 热点以及下一个热点
        public int mNavIndex;
        public int mNavNextIndex;

        // 漫游路径
        CSubPath mNavPath = null;
        int mTrainWayPointCount;
        int mTrainWayPointTotal;

        // 虚拟摄像机位置
        int mVirtualWayPointCount;
        double mDistoNext;

        bool mVirtualOnly = true;

        /// <summary>
        /// 循环状态
        /// </summary>
        private bool isOnOneWay = false;
        /// <summary>
        /// 可以开始下一目标
        /// </summary>
        public bool readyForNext = true;
        //public event reachToOnePlace OnToOnePlace = null;

        // 漫游过程中的2D形象进度
        PanelProgress2D panel2D;
        // 周边信息
        PanelInfo panelInfo;

        private Timer timerWaitingForNext;
        Random mRan = new Random();

        /// <summary>
        /// TE pro事件只注册处理一次
        /// </summary>
        static bool isTEEventRegistered = false;
        
        public CTEPresentation(CRailwayScene s,CTEScene tes, PanelProgress2D p2, PanelInfo p3, Timer timer,List<CHotSpot> hp = null)//
        {
            mSceneData = s;
            mTEScene = tes;
            mNavigationList = hp;
            panel2D = p2;
            panelInfo = p3;
            if (mNavigationList == null || mNavigationList.Count <= 1)
                return;
            mNavIndex = mNavigationList.Count-1;
            //mNavIndex = Math.Max(0, mNavIndex);
            mNavNextIndex = (mNavIndex + 1) % mNavigationList.Count;

            sgworld = new SGWorld66();

            timerWaitingForNext = timer;
            timerWaitingForNext.Tick += TimerWaitingForNext_Tick;

            if (string.IsNullOrEmpty(mNavObjGroupID))
                mNavObjGroupID = sgworld.ProjectTree.CreateGroup("TrainGroup");
            else
            {
                sgworld.ProjectTree.DeleteItem(mNavObjGroupID);
                mNavObjGroupID = sgworld.ProjectTree.CreateGroup("TrainGroup");
            }

            mPresentationGroupID = sgworld.ProjectTree.FindItem("Presentation");
            if (string.IsNullOrEmpty(mPresentationGroupID))
            {
                mPresentationGroupID = sgworld.ProjectTree.CreateGroup("Presentation");
            }

            mPresentation = sgworld.Creator.CreatePresentation(mPresentationGroupID, "Navigating");
            
            mPresentation.PlayAlgorithm = PresentationPlayAlgorithm.PPA_SPLINE;
            mPresentation.PlayMode = PresentationPlayMode.PPM_AUTOMATIC;
            mPresentation.LoopRoute = true;
            
            //mPresentation.CaptionHeight = 50;

            //mWorker = (ITerrainModel66)(sgworld.ProjectTree.GetObject(sgworld.ProjectTree.FindItem("worker")));
            //mDynamicWorker =(ITerrainDynamicObject66)(sgworld.ProjectTree.GetObject(sgworld.ProjectTree.FindItem("dynamicCamera")));
            mDynamicTrain[0] = sgworld.Creator.CreateDynamicObject(0, DynamicMotionStyle.MOTION_AIRPLANE, DynamicObjectType.DYNAMIC_3D_MODEL,
                mModelName[0], 1, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, mNavObjGroupID, "TrainObj0");
            mDynamicTrain[1] = sgworld.Creator.CreateDynamicObject(0, DynamicMotionStyle.MOTION_AIRPLANE, DynamicObjectType.DYNAMIC_3D_MODEL,
                mModelName[1], 1, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, mNavObjGroupID, "TrainObj1");
            mDynamicTrain[2] = sgworld.Creator.CreateDynamicObject(0, DynamicMotionStyle.MOTION_AIRPLANE, DynamicObjectType.DYNAMIC_3D_MODEL,
                mModelName[1], 1, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, mNavObjGroupID, "TrainObj2");
            mDynamicTrain[3] = sgworld.Creator.CreateDynamicObject(0, DynamicMotionStyle.MOTION_AIRPLANE, DynamicObjectType.DYNAMIC_3D_MODEL,
                mModelName[1], 1, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, mNavObjGroupID, "TrainObj3");
            mDynamicTrain[4] = sgworld.Creator.CreateDynamicObject(0, DynamicMotionStyle.MOTION_AIRPLANE, DynamicObjectType.DYNAMIC_3D_MODEL,
                mModelName[2], 1, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, mNavObjGroupID, "TrainObj4");
            //mDynamicTrain.Acceleration = 
            mVirtualObj = sgworld.Creator.CreateDynamicObject(0, DynamicMotionStyle.MOTION_MANUAL, DynamicObjectType.DYNAMIC_VIRTUAL,
                "", 1, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, mNavObjGroupID, "VirtualObj");

            if (!isTEEventRegistered)
            {
                sgworld.OnObjectAction += Sgworld_OnObjectAction;
                sgworld.OnPresentationFlyToReachedDestination += Sgworld_OnPresentationFlyToReachedDestination;
                isTEEventRegistered = true;
            }
            
            
        }

        public static void updatePresentationSpeed(IPresentation66 pre)
        {
            if (pre == null)
                return;
            if (CGisDataSettings.AppSpeed == 1)
                pre.PlaySpeedFactor = PresentationPlaySpeed.PPS_VERYSLOW;
            else if (CGisDataSettings.AppSpeed == 2)
                pre.PlaySpeedFactor = PresentationPlaySpeed.PPS_SLOW;
            else if (CGisDataSettings.AppSpeed == 3)
                pre.PlaySpeedFactor = PresentationPlaySpeed.PPS_NORMAL;
            else if (CGisDataSettings.AppSpeed == 4)
                pre.PlaySpeedFactor = PresentationPlaySpeed.PPS_FAST;
            else
                pre.PlaySpeedFactor = PresentationPlaySpeed.PPS_VERYFAST;
        }

        private void Sgworld_OnPresentationFlyToReachedDestination(string PresentationID, IPresentationStep66 Step)
        {

            if (PresentationID.Equals(mPresentation.ID))
            {// 动态生成的Presentation，长距离漫游定位，动态生成漫游路径

                if (Step.Description.Equals("Ended"))
                {
                    mPresentation.Stop();
                    ReachOnePlace();
                }
            }
            else if (mTeObject != null && mTeObject.mPresentation !=null && PresentationID.Equals(mTeObject.mPresentation.ID))
            {// 预定义的Presentation，目前仅火车站播放动画使用
                if (Step.Description.Equals("Ended"))
                {
                    mTeObject.mPresentation.Stop();
                    readyForNext = true;
                    mTEScene.prjInfoForm.ws = ShowFormStatus.eClosing; // 触发下一个漫游热点
                }
            }
        }

        private void Sgworld_OnObjectAction(string ObjectID, IAction66 Action)
        {
            double  d;
            if (Action.Code == ActionCode.AC_WAYPOINT_REACHED)
            {
                string strObjID = sgworld.ProjectTree.GetItemName(ObjectID);

                if (strObjID.Equals("TrainObj0"))
                {


#if DEBUG
                    Console.WriteLine("计数:" + mTrainWayPointCount + " of " + mTrainWayPointTotal);
#endif

                    mTrainWayPointCount= Math.Min(mTrainWayPointCount,mTrainWayPointTotal-1);

                    if (  mTrainWayPointCount % 15 == 14)
                        RandomFollow();

                    //double dis = CoordinateConverter.getUTMDistance(mDynamicTrain[0].Position.X, mDynamicTrain[0].Position.Y, mNavigationList[mNavNextIndex].Longitude, mNavigationList[mNavNextIndex].Latitude);
                    //根据GPS确定里程，
                    mSceneData.mMainPath.getPathMileagebyGPS(rx[mTrainWayPointCount], ry[mTrainWayPointCount], out mMainMileage, out d);
                    //mDynamicTrain[0].Waypoints[mTrainWayPointTotal- mTrainWayPointCount]
                    //mCurrentMileage = rm[mTrainWayPointCount];
                    mDistoNext = rm[mTrainWayPointTotal - 1] - rm[mTrainWayPointCount];

//#if DEBUG
//                    if (Math.Abs(mDistoNext - dis) > 100)
//                        Console.WriteLine("Something wrong in " + mTrainWayPointCount + " dis " + mDistoNext + " vs " + dis);
//#endif                    

                    string dkcode;
                    double dkm;
                    mSceneData.mMainPath.getDKCodebyPathMileage(mMainMileage, out dkcode, out dkm);

                    string str = mNavigationList[mNavNextIndex].ObjectRef.ToString();

                    // 更新显示面板
                    panelInfo.mtotalProj = mNavigationList.Count;
                    panelInfo.mcurrentProj = mNavNextIndex;
                    panelInfo.mdkCode = CRailwayLineList.CombiDKCode(dkcode, dkm);
                    panelInfo.mdistance = Math.Round(mDistoNext,2);
                    panelInfo.mnextProj = str;
                    panelInfo.mconsLog = mSceneData.GetStrConsByGPS(rx[mTrainWayPointCount], ry[mTrainWayPointCount],5000);
                    panelInfo.updatePanel();
                    
                    // 更新2D进度
                    panel2D.update2DPanel(mMainMileage, 500);

                    if (mDistoNext < 500) // 可能
                    {
                        ReachOnePlace();
                        
                        //double dis = CoordinateConverter.getUTMDistance(mDynamicTrain[0].Position.X, mDynamicTrain[0].Position.Y, mNavigationList[mNavIndex + 1].Longitude, mNavigationList[mNavIndex + 1].Latitude);
//#if DEBUG
//                        Console.WriteLine("计数:" + mTrainWayPointCount + " of " + mTrainWayPointTotal + "count dis "+ mDistoNext + "\t gis dis" + dis);
//#endif
//                        if (mTrainWayPointTotal - mTrainWayPointCount <= 1)
//                        {
//#if DEBUG
                            
//                            ModelInfo.Helper.LogHelper.WriteLog(mNavIndex + ":train到达 " + mNavigationList[mNavIndex].ObjectRef.ToString() );
//#endif
//                            //for (int j = 0; j < mDynamicTrain.Length; j++)
//                            //{
//                            //    mDynamicTrain[j].Pause = true;
//                            //}

//                            //mDistoNext = 0;

//                        }
                    }
                    mTrainWayPointCount++;

                }
                else if (strObjID.Equals("VirtualObj"))
                {
                    mVirtualWayPointCount--;

                    if (isOnOneWay && mVirtualOnly)
                    {
                        double dis = CoordinateConverter.getUTMDistance(mVirtualObj.Position.X, mVirtualObj.Position.Y, mNavigationList[mNavNextIndex].Longitude, mNavigationList[mNavNextIndex].Latitude);

                        if (dis < 100 || mVirtualWayPointCount == 0)
                        {
#if DEBUG
                            ModelInfo.Helper.LogHelper.WriteLog(mNavIndex + "virtual到达 " + mNavigationList[mNavNextIndex].ObjectRef.ToString() + " 距离为 " + dis);
#endif
                            //sgworld.OnObjectAction -= Sgworld_OnObjectAction;
                            ReachOnePlace();

                        }
                    }


                }
            }

        }

        /// <summary>
        /// 到达某处，旋转视角，弹出窗口
        /// </summary>
        private void ReachOnePlace()
        {
            
            if (isOnOneWay)
            {
                isOnOneWay = false;
                panelInfo.Visible = false;

                mNavIndex++;
                mNavIndex %= mNavigationList.Count;
                mNavNextIndex++;
                mNavNextIndex %= mNavigationList.Count;
                
                if (mNavigationList != null)
                {
                    double d;
                    mSceneData.mMainPath.getPathMileagebyGPS(mNavigationList[mNavIndex].Longitude, mNavigationList[mNavIndex].Latitude, out mMainMileage, out d);
                    // FIXME，距离人为规定500
                    panel2D.update2DPanel(mMainMileage, 500);

                    viewTEObject();

                }
            }

        }

        public bool startNextPresentation()
        {
            bool fromCurrent = true;
            if (mNavigationList == null || mNavigationList.Count == 0)
                return false;

            readyForNext = false;

            // 屏幕中心坐标
            IPosition66 cp = sgworld.Window.CenterPixelToWorld().Position;
            // 视点坐标
            IPosition66 viewp = sgworld.Navigate.GetPosition(AltitudeTypeCode.ATC_TERRAIN_RELATIVE);

            //起点与目标点距离
            double len1 = mNavigationList[mNavIndex].getUTMDistance(cp.X, cp.Y) + viewp.Altitude;

            //如果漫游起点距离屏幕中心过远，从当前点漫游到起点
            double fromX, fromY, fromZ;
            //if (mRan.Next(1,20) == 10)
            //{
            //    fromX = 118.6;
            //    fromY = 36.6;
            //    fromZ = 400000;
            //    fromCurrent = false;

            //}
            //else 
            if (len1 > 5000)
            {
                fromX = cp.X;
                fromY = cp.Y;
                fromZ = viewp.Altitude;
                fromCurrent = false;
            }
            else  //否则从极点到下一点
            {
                fromX = mNavigationList[mNavIndex].Longitude;
                fromY = mNavigationList[mNavIndex].Latitude;
                fromZ = mNavigationList[mNavIndex].Altitude;
            }

            double length = mNavigationList[mNavNextIndex].getUTMDistance(fromX, fromY);
#if DEBUG
            Console.WriteLine(mNavNextIndex + " 目的地 " +  mNavigationList[mNavNextIndex].ToString());
#endif
            if (length < 50)
            {
                ReachOnePlace();
                return true;
            }

            mTopSpeed = Math.Max(0, Math.Min(Math.Pow(2, CGisDataSettings.AppSpeed) * 20, 350));
            double speed = Math.Min(length / 3.5, mTopSpeed);

            clearTrain();

            isOnOneWay = true;
            mVirtualOnly = false;

            if (!fromCurrent || length > 30000)
            {
                double x, y;

                x = (fromX + mNavigationList[mNavNextIndex].Longitude) / 2;
                y = (fromY + mNavigationList[mNavNextIndex].Latitude) / 2;
                for (int i = mPresentation.Steps.Count - 1; i >= 0; i--)
                    mPresentation.DeleteStep(i);

                mPresentation.CreateLocationStep(PresentationStepContinue.PSC_WAIT, 1, "Starting",
                    sgworld.Creator.CreatePosition(fromX, fromY, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -89, 0, Math.Max(350, fromZ)));

                // 如果距离过长，先到世界中心，再到当前工点
                mPresentation.CreateCaptionStep(PresentationStepContinue.PSC_WAIT, 0, "Txt", "开始漫游", 3);
                if (length > 60000)
                {
                    mPresentation.CreateLocationStep(PresentationStepContinue.PSC_WAIT, 1, "Ending",
                        sgworld.Creator.CreatePosition(118.6, 36.6, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE,
                            341, -90.0, 0, 445000));

                }
                else
                {
                    mPresentation.CreateLocationStep(PresentationStepContinue.PSC_WAIT, 1, "Ending",
                        sgworld.Creator.CreatePosition(x, y, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -89, 0, Math.Max(length, 5000)));

                }
                mPresentation.CreateLocationStep(PresentationStepContinue.PSC_WAIT, 1, "Ended",
                    sgworld.Creator.CreatePosition(mNavigationList[mNavNextIndex].Longitude, mNavigationList[mNavNextIndex].Latitude, 350, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -89, 0, 0));

                updatePresentationSpeed(mPresentation);

                mPresentation.Play(0);
                //mVirtualObj.Waypoints.AddWaypoint(sgworld.Creator.CreateRouteWaypoint(mPlaceList[nextIndex].Longitude, mPlaceList[nextIndex].Latitude, mPlaceList[nextIndex].Altitude + 150));

                return true;
            }
            //isPresentation = true;

            //sgworld.OnObjectAction += Sgworld_OnObjectAction;

            mStepm = length < 250 ? length / 5 : speed;
            mNavPath = new CSubPath(CRailwayLineList.gRealConnection, mNavigationList[mNavIndex].DKCode, mNavigationList[mNavIndex].Mileage,
                mNavigationList[mNavNextIndex].DKCode, mNavigationList[mNavNextIndex].Mileage, 10);

            if (!mNavPath.hasPath)
            {
                // 如果没有路径，直接飞过去
                double midx, midy, midz;
                midx = (fromX + mNavigationList[mNavNextIndex].Longitude) /2;
                midy = (fromY + mNavigationList[mNavNextIndex].Latitude) /2;
                midz = (fromZ + mNavigationList[mNavNextIndex].Altitude) / 2;
                mVirtualWayPointCount = 3;
                mVirtualObj.Waypoints.AddWaypoint(sgworld.Creator.CreateRouteWaypoint(fromX, fromY, fromZ + 150, speed));
                mVirtualObj.Waypoints.AddWaypoint(sgworld.Creator.CreateRouteWaypoint(midx , midy , midz  + 150, speed));
                mVirtualObj.Waypoints.AddWaypoint(sgworld.Creator.CreateRouteWaypoint(mNavigationList[mNavNextIndex].Longitude, mNavigationList[mNavNextIndex].Latitude, mNavigationList[mNavNextIndex].Altitude + 150, speed));
#if DEBUG
                ModelInfo.Helper.LogHelper.WriteLog(mNavIndex + "\t" + midx + "\t" + midy + "\t" + midz );
#endif
                mVirtualOnly = true;
                mVirtualObj.CircularRoute = false;
                mVirtualObj.Pause = false;
                mVirtualObj.RestartRoute();
                sgworld.Navigate.FlyTo(mVirtualObj, ActionCode.AC_FOLLOWABOVE);
                // ReachOnePlace();
                return true;
            }

            panelInfo.Visible = true;
            prepareTrain(mNavPath, 0);
//            double[][] mm,mx, my, mz, md;
//            mx = new double[5][];
//            my = new double[5][];
//            mz = new double[5][];
//            md = new double[5][];
//            mm = new double[5][];
//            double[] ms = { 0, 37, 86, 135, 172 };
//            //mx[0] = mNavPath.mx;
//            //my[0] = mNavPath.my;
//            //mz[0] = mNavPath.mz;
//            //md[0] = mNavPath.md;
//            mTrainWayPointTotal = mTrainWayPointCount = mNavPath.mPointCount;
//            mDistoNext = mNavPath.mLength;
//            for (int i = 0; i< 5; i++)
//            {
//                ModelInfo.Helper.CSubLine.getSubLineByMileage(-ms[i], mNavPath.mLength-ms[i], mStepm, mNavPath.mm, mNavPath.mx, mNavPath.my, mNavPath.mz, mNavPath.md,
//                    out mm[i], out mx[i], out my[i], out mz[i], out md[i], true);

//            }
            
//            //mDynamicTrain.Acceleration = 1;
//            double localspeed;
//            for (int j = 0; j < 5; j++)
//            {
//                for (int i = 0; i < mTrainWayPointCount; i++)
//                {
//                    // 逐渐加速，减速
//                    localspeed = Math.Min(5 + (mTrainWayPointCount - Math.Abs(mTrainWayPointCount - i / 2)) * 10, speed);
//#if DEBUG
//                    //Console.WriteLine(curIndex + "\t" + mx[i] + "\t" + my[i] + "\t" + mz[i] + "\t" + md[i]);
//                    //ModelInfo.Helper.LogHelper.WriteLog(curIndex + "\t" + mx[i] + "\t" + my[i] + "\t" + mz[i] + "\t" + md[i]);
//                    mDynamicTrain[j].Waypoints.AddWaypoint(sgworld.Creator.CreateRouteWaypoint(mx[j][i], my[j][i], mz[j][i], speed, md[j][i]));
//#else
//                mDynamicTrain.Waypoints.AddWaypoint(sgworld.Creator.CreateRouteWaypoint(mx[j][i], my[j][i], mz[j][i], localspeed, md[j][i]));
//#endif
//                }

//                mDynamicTrain[j].CircularRoute = false;
//                mDynamicTrain[j].Pause = false;
//                mDynamicTrain[j].RestartRoute();
//            }
//            if (mTrainWayPointTotal < 15)
//            {
//                sgworld.Navigate.FlyTo(mDynamicTrain[0], ActionCode.AC_FOLLOWBEHINDANDABOVE);
//            }
//            else
//                sgworld.Navigate.FlyTo(mDynamicTrain[0], ActionCode.AC_FOLLOWCOCKPIT);


            return true;
        }

        public void startNextPresentation(int offsetIndex)
        {
            if (Math.Abs(offsetIndex) >= mNavigationList.Count)
                return;
            mNavIndex = (mNavIndex + mNavigationList.Count + offsetIndex) % mNavigationList.Count;
            mNavNextIndex = (mNavIndex + 1)% mNavigationList.Count;
            stopPresentation();
            startNextPresentation();

        }

        public void changeNavTrainSpeed()
        {
            clearTrain();
            prepareTrain(mNavPath, mTrainWayPointCount);
//            if (mDynamicTrain != null && mDynamicTrain[0].Waypoints.Count > 0)
//            {
//                mDynamicTrain[0].Pause = true;
//                mTrainWayPointTotal = mTrainWayPointCount = mDynamicTrain[0].Waypoints.Count;
//                for (int i = mDynamicTrain[0].Waypoints.Count - 1; i >= 0; i--)
//                    mDynamicTrain[0].Waypoints.DeleteWaypoint(i);

//                double[] mx, my, mz, md;

//                mx = mNavPath.mx;
//                my = mNavPath.my;
//                mz = mNavPath.mz;
//                md = mNavPath.md;

//                mDistoNext = mNavPath.mLength;
//                //mDynamicTrain.Acceleration = 1;
//                mTopSpeed = Math.Max(0, Math.Min(Math.Pow(2, CGisDataSettings.AppSpeed) * 20, 350));
//                double speed =  mTopSpeed;
//                //double localspeed;
//                int startIndex = mNavPath.mPointCount - mTrainWayPointCount;
//                for (int i = 0; i < mTrainWayPointCount; i++)
//                {

                   
//#if  DEBUG
//                    //ModelInfo.Helper.LogHelper.WriteLog(curIndex + "\t" + mx[i] + "\t" + my[i] + "\t" + mz[i] + "\t" + md[i]);
//                    mDynamicTrain[0].Waypoints.AddWaypoint(sgworld.Creator.CreateRouteWaypoint(mx[startIndex+i], my[startIndex+i], mz[startIndex+i], speed, md[startIndex+i]));
//#else
//                     // 逐渐加速，减速
//                    speed = Math.Min(5 + (mTrainWayPointCount - Math.Abs(mTrainWayPointCount - i / 2)) * 10, speed);
//                mDynamicTrain.Waypoints.AddWaypoint(sgworld.Creator.CreateRouteWaypoint(mx[startIndex+i], my[startIndex+i], mz[startIndex+i], speed, md[startIndex+i]));
//#endif
//                }
//                mDynamicTrain[0].CircularRoute = false;
//                mDynamicTrain[0].Pause = false;
//                mDynamicTrain[0].RestartRoute();
//                if (mTrainWayPointTotal < 15)
//                {
//                    sgworld.Navigate.FlyTo(mDynamicTrain, ActionCode.AC_FOLLOWBEHINDANDABOVE);
//                }
//                else
//                    sgworld.Navigate.FlyTo(mDynamicTrain, ActionCode.AC_FOLLOWCOCKPIT);
//            }
        }

        private void clearTrain() {
            if (mVirtualObj != null) // 用于无线路时的漫游
            {
                mVirtualObj.Pause = true;
                for (int i = mVirtualObj.Waypoints.Count - 1; i >= 0; i--)
                {
                    mVirtualObj.Waypoints.DeleteWaypoint(i);
                }
            }
            if (mDynamicTrain != null && mDynamicTrain[0] != null) // 用于火车线路漫游，
            {
                for (int j = 0; j < mDynamicTrain.Length; j++)
                {
                    mDynamicTrain[j].Pause = true;
                    for (int i = mDynamicTrain[j].Waypoints.Count - 1; i >= 0; i--)
                        mDynamicTrain[j].Waypoints.DeleteWaypoint(i);
                }

            }
        }

        private void prepareTrain(CSubPath navPath, int fromIndex = 0)
        {
            
           
            mTopSpeed = Math.Max(0, Math.Min(Math.Pow(2, CGisDataSettings.AppSpeed) * 20, 350));

            double[][] mm, mx, my, mz, md;
            mx = new double[5][];
            my = new double[5][];
            mz = new double[5][];
            md = new double[5][];
            mm = new double[5][];
            double[] ms = { 0, 37, 86, 135, 172 };

            fromIndex = Math.Max(fromIndex, 0);
            fromIndex = Math.Min(fromIndex, navPath.mPointCount-3);
            
            
            for (int i = 0; i < mDynamicTrain.Length; i++)
            {
                // 由起始计算路径
                if (fromIndex == 0)
                {
                    mDistoNext = navPath.mLength ;
                    ModelInfo.Helper.CSubLine.getSubLineByMileage( - ms[i], navPath.mLength - ms[i], mStepm / 2, navPath.mm, navPath.mx, navPath.my, navPath.mz, navPath.md,
                        out mm[i], out mx[i], out my[i], out mz[i], out md[i], true);
                }
                else // 由中途计算路径
                {
                    mDistoNext = rm.Last() - rm[fromIndex];
                    ModelInfo.Helper.CSubLine.getSubLineByMileage(rm[fromIndex]-ms[i], rm.Last() - ms[i], mStepm / 2, rm, rx, ry, rz, rd,
                        out mm[i], out mx[i], out my[i], out mz[i], out md[i], true);

                }
            }
            
            mTrainWayPointTotal =  mm[0].Length;
            mTrainWayPointCount = 0;

            rm = new double[mTrainWayPointTotal];
            rx = new double[mTrainWayPointTotal];
            ry = new double[mTrainWayPointTotal];
            rz = new double[mTrainWayPointTotal]; 
            rd = new double[mTrainWayPointTotal];
            mm[0].CopyTo(rm, 0);
            mx[0].CopyTo(rx, 0);
            my[0].CopyTo(ry, 0);
            mz[0].CopyTo(rz, 0);
            md[0].CopyTo(rd, 0);
            //mDynamicTrain.Acceleration = 1;
            double localspeed;
            for (int j = 0; j < mDynamicTrain.Length; j++)
            {
                for (int i = 0; i < mTrainWayPointTotal; i++)
                {

#if DEBUG
                    //Console.WriteLine(curIndex + "\t" + mx[i] + "\t" + my[i] + "\t" + mz[i] + "\t" + md[i]);
                    //ModelInfo.Helper.LogHelper.WriteLog(curIndex + "\t" + mx[i] + "\t" + my[i] + "\t" + mz[i] + "\t" + md[i]);
                    mDynamicTrain[j].Waypoints.AddWaypoint(sgworld.Creator.CreateRouteWaypoint(mx[j][i], my[j][i], mz[j][i], mTopSpeed, md[j][i]));
#else
                                        // 逐渐加速，减速
                    localspeed = Math.Min(5 + (mTrainWayPointTotal - Math.Abs(mTrainWayPointTotal - i / 2)) * 10, mTopSpeed);
                    mDynamicTrain[j].Waypoints.AddWaypoint(sgworld.Creator.CreateRouteWaypoint(mx[j][i], my[j][i], mz[j][i], localspeed, md[j][i]));
#endif
                }

                mDynamicTrain[j].CircularRoute = false;
                mDynamicTrain[j].Pause = false;
                mDynamicTrain[j].RestartRoute();
            }
            for (int i=0; i < mTrainWayPointTotal; i++)
            {
                mVirtualObj.Waypoints.AddWaypoint(sgworld.Creator.CreateRouteWaypoint(mx[4][i],my[4][i],mz[4][i]+ 20,mTopSpeed,md[4][i]));
            }
            mVirtualWayPointCount = mTrainWayPointTotal;
            mVirtualObj.CircularRoute = false;
            mVirtualObj.Pause = false;
            mVirtualObj.RestartRoute();
            //mVirtualObj.Waypoints.AddWaypoint(sgworld.Creator.CreateRouteWaypoint(fromX, fromY, fromZ + 150, speed));
            //mVirtualObj.Waypoints.AddWaypoint(sgworld.Creator.CreateRouteWaypoint(midx, midy, midz + 150, speed));
            //mVirtualObj.Waypoints.AddWaypoint(sgworld.Creator.CreateRouteWaypoint(mNavigationList[mNavNextIndex].Longitude, mNavigationList[mNavNextIndex].Latitude, mNavigationList[mNavNextIndex].Altitude + 150, speed));


            //if (mTrainWayPointTotal  < 15)
            //{
            //    sgworld.Navigate.FlyTo(mDynamicTrain[0], ActionCode.AC_FOLLOWBEHINDANDABOVE);
            //}
            //else
            //    sgworld.Navigate.FlyTo(mDynamicTrain[0], ActionCode.AC_FOLLOWCOCKPIT);
            if (mTrainWayPointTotal < 15)
            {
                sgworld.Navigate.FlyTo(mVirtualObj, ActionCode.AC_FOLLOWBEHINDANDABOVE);
            }
            else
                sgworld.Navigate.FlyTo(mVirtualObj, ActionCode.AC_FOLLOWCOCKPIT);
        }

        public void stopPresentation()
        {
            if (isOnOneWay)
            {
                isOnOneWay = false;
                clearTrain();
                mPresentation.Stop();
                //sgworld.OnObjectAction -= Sgworld_OnObjectAction;
            }
            else 
                interruptViewTEObject(); // 关掉展示窗口或者停止presentation
            IPosition66 resPos = sgworld.Navigate.GetPosition(AltitudeTypeCode.ATC_TERRAIN_RELATIVE);
            resPos = sgworld.Creator.CreatePosition(resPos.X, resPos.Y, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, resPos.Yaw, resPos.Pitch, resPos.Roll, 1500);//resPos.Altitude
            sgworld.Navigate.FlyTo(resPos);
        }

        public void reStartPresentation()
        {
            if (mNavigationList != null)
            {
                stopPresentation();
                mNavIndex = mNavigationList.Count ;

                mNavNextIndex = (mNavIndex + 1) % mNavigationList.Count;

                startNextPresentation();
            }
        }

        public void setNavPath(List<CHotSpot> hp, int startIndex = 0)
        {
            stopPresentation();
            mNavigationList = hp;
            mNavIndex = Math.Min(startIndex, mNavigationList.Count - 1);
            mNavIndex = Math.Max(0, mNavIndex);
            mNavNextIndex = (mNavIndex + 1) % mNavigationList.Count;
        }

        public bool setStartPosition(int index)
        {
            if (mNavigationList == null) return false;
            if (readyForNext)
            {
                mNavIndex = Math.Min(index, mNavigationList.Count - 1);
                mNavIndex = Math.Max(0, mNavIndex);
                mNavNextIndex = (mNavIndex + 1) % mNavigationList.Count;
            }
            return true;
        }

        public int getCurrentPlaceIndex()
        {
            if (mNavigationList == null || mNavigationList.Count <= mNavIndex)
                return -1;
            else
                return mNavIndex;
        }

        private void RandomFollow()
        {
            if (!isOnOneWay)
                return;
            ActionCode ac = ActionCode.AC_FOLLOWCOCKPIT;
            
            int tmpcode = mRan.Next(1, 4);
            if (mMainMileage > 42000 && mMainMileage < 54000)
                tmpcode = tmpcode % 4;
            if (mDistoNext < 2000)
            {
                ac = ActionCode.AC_FOLLOWBEHINDANDABOVE;
                //mDynamicWorker.RestartRoute();
                //sgworld.Navigate.FlyTo(mDynamicWorker, ActionCode.AC_FOLLOWCOCKPIT);
                //return;
            }
            else
            {
                switch (tmpcode)
                {
                    case 1:
                        ac = ActionCode.AC_FOLLOWCOCKPIT; //ActionCode.AC_FOLLOWBEHIND;
                        break;
                    case 2:
                        ac = ActionCode.AC_FOLLOWBEHINDANDABOVE;
                        break;
                    case 3:
                        ac = ActionCode.AC_FOLLOWBEHIND;
                        //sgworld.Navigate.FlyTo(mDynamicWorker, ActionCode.AC_FOLLOWCOCKPIT);
                        return;
                    case 4:
                        ac = ActionCode.AC_FOLLOWLEFT;
                        break;
                        //case 5:
                        //    ac = ActionCode.AC_FOLLOWRIGHT;
                        //    break;

                }
            }
            //sgworld.Navigate.FlyTo(mDynamicTrain[0], ac);  // FIXME Debug
            sgworld.Navigate.FlyTo(mVirtualObj, ac);
            //sgworld.Navigate.FlyTo(mDynamicTrain[0], ActionCode.AC_FOLLOWBEHINDANDABOVE);


        }

        private void interruptViewTEObject()
        {
            if (!readyForNext)
            {
                //if (mNavIndex + 1 < mNavigationList.Count)
                //{
                    mTeObject = mTEScene.showTEObject(mNavigationList[mNavIndex], mNavigationList[mNavNextIndex], true);
                //}
                //else
                //{
                //    mTeObject = mTEScene.showTEObject(mNavigationList[navIndex], mNavigationList[0], true);
                //}
                if (mTeObject != null)
                {
                    if (mTeObject.mPresentation != null)
                    {
#if DEBUG
                        ModelInfo.Helper.LogHelper.WriteLog("stop to view " + mNavIndex);
#endif
                        if (mTeObject.mPresentation.PresentationStatus != PresentationStatus.PS_NOTPLAYING)
                            mTeObject.mPresentation.Stop();

                    }
                }

            }
        }

        private void viewTEObject()
        {
            //if (navIndex + 1 < mNavigationList.Count)
            //{
                mTeObject = mTEScene.showTEObject(mNavigationList[mNavIndex], mNavigationList[mNavNextIndex]);
            //}
            //else
            //{
            //    mTeObject = mTEScene.showTEObject(mNavigationList[navIndex], mNavigationList[0]);
            //}

            if (mTeObject == null)
            {
#if DEBUG
                ModelInfo.Helper.LogHelper.WriteLog("NO Navigation Object" + mNavIndex);
#endif
                return;
            }

            if (mTeObject.mPresentation != null)
            {
#if DEBUG
                Console.WriteLine("begin to view " + mNavIndex);
#endif
                updatePresentationSpeed(mTeObject.mPresentation);
                mTeObject.mPresentation.Play(0);
                return;
            }

            if (mTeObject.labelSign != null)
            {
                ActionCode ac; // = ActionCode.AC_CIRCLEPATTERN;
                Random ran = new Random();
                int tmpcode = ran.Next(1, 4);

                switch (tmpcode)
                {
                    case 1:
                        ac = ActionCode.AC_CIRCLEPATTERN; //ActionCode.AC_FOLLOWBEHIND;
                        break;
                    case 2:
                        ac = ActionCode.AC_ARCPATTERN;
                        break;
                    case 3:
                        ac = ActionCode.AC_OVALPATTERN;
                        break;
                    case 4:
                        ac = ActionCode.AC_LINEPATTERN;
                        break;
                    default:
                        ac = ActionCode.AC_CIRCLEPATTERN; //ActionCode.AC_FOLLOWBEHIND;
                        break;

                }
                sgworld.Navigate.FlyTo(mTeObject.labelSign, ac);
            }
            int timeInterval = (int)(Math.Pow((6 - CGisDataSettings.AppSpeed), 2) / 9 * 10000);
#if DEBUG
            timeInterval = 5000;
#endif
            timerWaitingForNext.Interval = timeInterval;
            timerWaitingForNext.Start();
        }

        private void TimerWaitingForNext_Tick(object sender, EventArgs e)
        {
            timerWaitingForNext.Stop();
            readyForNext = true;
        }
        //public void RandomViewStyle()
        //{
        //    if (mVirtualObj != null && mVirtualObj.Waypoints.Count > 0)
        //    {
        //        ActionCode ac = ActionCode.AC_CIRCLEPATTERN;
        //        Random ran = new Random();
        //        int tmpcode = ran.Next(1, 5);

        //        switch (tmpcode)
        //        {
        //            case 1:
        //                ac = ActionCode.AC_CIRCLEPATTERN; //ActionCode.AC_FOLLOWBEHIND;
        //                break;
        //            case 2:
        //                ac = ActionCode.AC_ARCPATTERN;
        //                break;
        //            //case 3:
        //            //    ac = ActionCode.AC_LINEPATTERN;
        //            //    break;
        //            case 4:
        //                ac = ActionCode.AC_OVALPATTERN;
        //                break;
        //            //case 5:
        //            //    ac = ActionCode.AC_LINEPATTERN;
        //            //    break;
        //            default:
        //                ac = ActionCode.AC_CIRCLEPATTERN; //ActionCode.AC_FOLLOWBEHIND;
        //                break;


        //        }
        //        //sgworld.Navigate.FlyTo(sgworld.Creator.CreatePosition(mPlaceList[mNavIndex].Longitude, mPlaceList[mNavIndex].Latitude, mPlaceList[mNavIndex].Altitude + 5, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, 0, -75, 0, 0), ac);
        //        sgworld.Navigate.FlyTo(mVirtualObj, ac);
        //    }


        //}
    }
}
