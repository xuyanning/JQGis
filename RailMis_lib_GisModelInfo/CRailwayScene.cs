using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.Drawing;
//using lib_GIS.Service;
//using Microsoft.Office.Interop.Excel;
using ModelInfo.Helper;
using System.Threading;


namespace ModelInfo
{
    public class CRailwayScene
    {

        //  string sqlstr;
        //public ITerrainDynamicObject66 mDynamicTrain;
        //public ITerrainModel66 mTrain;
        //public CRailwayLineList mMiddleLines = new CRailwayLineList();
        public CSubPath mMainPath = null; 
        public string mDataPath, mProjectPath;  // 数据存放路径与工程存放路径
        public bool mUseLocalDB;
        //CServerWrapper mWSAgent;

        //const int MAX_MID_P = 28400;
        public CRailwayProject rootProj; // = new CRailwayProject(this);

        //public List<CHotSpot> mNoneProjectSpotList = new List<CHotSpot>(); // mHotSpotList
        public List<CRailwayProject> mBridgeList = new List<CRailwayProject>();
        public List<CRailwayProject> mRoadList = new List<CRailwayProject>();
        public List<CRailwayProject> mTunnelList = new List<CRailwayProject>();
        public List<CRailwayProject> mContBeamList = new List<CRailwayProject>();
        public List<CRailwayProject> mTotalProjectList = new List<CRailwayProject>();


        public List<CRailwayFirm> mFirmList = new List<CRailwayFirm>();

        public List<CHotSpot> mConsSpotList = new List<CHotSpot>();
        public List<ConsLocation> mConsLoc = new List<ConsLocation>();

        public void updateSceneFromServer(bool saveToLocal = false)
        {
            CServerWrapper.isConnected = CServerWrapper.ConnectToServer(mProjectPath);
            if (CServerWrapper.isConnected)
            {
                initProjects(mDataPath, false);
                initDWProjects(mDataPath, false);

                foreach (CContBeam cb in mContBeamList)
                {
                    cb.AdjustMileage();
                }
                initFirms(mDataPath, false);
                CConsLog.clusterConsFromWebByProj(this, mDataPath, mConsLoc, mConsSpotList,
                    DateTime.Now.AddDays(-7).ToString("u"), DateTime.Now.ToString("u"), false);

                if (saveToLocal)
                {
                    saveProjectLocal(mDataPath);
                    saveProjFXLocal(mDataPath);
                    savePierLocal(mDataPath);
                    saveFirmLocal(mDataPath);
                    saveConsLocal(mDataPath);
                }
            }
        }

        public CRailwayScene(string dbPath, string prjUrl)
        {

            mDataPath = dbPath;
            mProjectPath = prjUrl;

            //1. 初始化线路信息
            CRailwayLineList.CreateLinelistFromSqlite(dbPath);

            // FIXME xyn 主路径硬编码，需要可配置
            //2. 初始化主线位;
            mMainPath = new CSubPath(CRailwayLineList.gMileageConnection, "GSJDK", 426000, "JQDK", 17600, 10);

            rootProj = new CRailwayProject(this);

            //3. 初始化工点
            initProjects(dbPath, true);

            //CServerWrapper.isConnected = CServerWrapper.ConnectToServer(mProjectPath);

            //if (CServerWrapper.isConnected)
            //{
            //    string resultStr = CServerWrapper.webLogin("xushiyang", "wl");
            //    if (!resultStr.Equals("登录成功"))
            //        return;
            //    //4. 初始化单位工程
            //    initDWProjects(dbPath, false);
            //}

            //4. 初始化单位工程
            initDWProjects(dbPath, true);
            //5. 修正连续梁里程信息
            foreach (CContBeam cb in mContBeamList)
            {
                //if (cb.mProjectID == 392)
                //    cb.AdjustMileage();
                //else
                    cb.AdjustMileage();
            }

            //6. 初始化建设单位，参建单位，监理单位
            initFirms(dbPath, true);

            //7. 初始化实名日志
            CConsLog.clusterConsFromWebByProj(this, mDataPath, mConsLoc, mConsSpotList,
                DateTime.Now.AddDays(-7).ToString("u"), DateTime.Now.ToString("u"), true);
            //proj2Txt(new StreamWriter("project.txt"));
            //savePierLocal(dbPath);
            //DatabaseWrapper.SaveLineListToSqlite(CGisDataSettings.gDataPath + @"jiqing\JQGis.db", mMiddleLines.mLineList);
            //DatabaseWrapper.SaveProjectToSqlite(CGisDataSettings.gDataPath + @"jiqing\JQGis.db",mTotalProjectList);
            //DatabaseWrapper.SavePierToSqlite("PierInfo", mBridgeList, CGisDataSettings.gDataPath + @"jiqing\JQGis.db");
            //DatabaseWrapper.SavePierToSqlite("PierContInfo", mContBeamList, CGisDataSettings.gDataPath + @"jiqing\JQGis.db");
            //DatabaseWrapper.SaveProgressToSqlite(CGisDataSettings.gDataPath + @"jiqing\JQGis.db", mTotalProjectList);
            //DatabaseWrapper.SaveFirmToSqlite(CGisDataSettings.gDataPath + @"jiqing\JQGis.db", mFirmList);
            //{
            //    string[] fileName;
            //    string[] photoTime;
            //    string[] sNo;
            //    string[] person;
            //    string[] remark;

            //    int num;
            //    num = CConsPhoto.findConsPhoto(5, out fileName, out photoTime, out sNo, out person, out remark);
            //    //num = CConsLog.findLast365Cons(out usrName, out projName, out consDate, out x, out y);
            //    CRailwayProject prj;
            //    for (int i = 0; i < num; i++)
            //    {
            //        prj = getProjectBySNo(sNo[i]);
            //        if (prj != null)
            //        {
            //            Console.WriteLine(prj.ProjectName);
            //        }
            //        Console.WriteLine("{0} #\t: fileName {1}\t   Date {2}\t   Person {3}\t   Remark {4} ", i, fileName[i], photoTime[i],person[i], remark[i]);
            //    }
            //}
        }

        #region save data to local sqlite
        public void savePierLocal(string dbPath)
        {
            DatabaseWrapper.SavePierToSqlite(dbPath, mBridgeList,mContBeamList);

        }

        public void savePolePosLocal(string dbPath)
        {
            DatabaseWrapper.SavePolePosToSqlite(dbPath, CRailwayLineList.mLineList);
        }

        public void saveProjectLocal(string dbPath)
        {
            DatabaseWrapper.SaveProjectToSqlite(dbPath, mTotalProjectList);

        }

        public void saveProjFXLocal(string dbPath)
        {
            DatabaseWrapper.SaveProgressToSqlite(dbPath, mTotalProjectList);
        }

        public void saveConsLocal(string dbPath)
        {
            DatabaseWrapper.SaveConsToSqlite(dbPath, mConsLoc);
        }

        public void saveFirmLocal(string dbPath)
        {
#if DEBUG
            LogHelper.WriteLog("当前工点信息不需要更新，重要数据没有与中心数据同步");
#endif
            //   DatabaseWrapper.SaveFirmToSqlite(dbPath, mFirmList);
        }
        #endregion

        #region get methods

        /// <summary>
        ///  根据经纬度，计算半径radius距离内的所有工点，按距离排序输出
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius">如果半径小于0，返回最近的5个工程</param>
        /// <param name="firmList"></param>
        /// <returns></returns>
        public List<CRailwayProject> getNearProject(double x, double y, double radius, List<CRailwayProject> projectList)
        {
            if (x < 10 && y < 10) return null;

            List<CRailwayProject> ls = new List<CRailwayProject>();
            double mil, dis;
            bool isInside;
            CRailwayLine rl = CRailwayLineList.getMileagebyGPS(x, y, out mil, out dis, out isInside);
            //getNearPos(x, y, out mm, out xx, out yy, out zz, out dir);
            for (int i = 0; i < projectList.Count; i++)
            {
                //if (rl.mDKCode == projectList[i].DKCode && dis < radius && ((mil - projectList[i].mMileage_Start) * (mil - projectList[i].mMileage_End)) < 0)
                //    projectList[i].mdis = projectList[i].Length / 1000; // FIXME, trick, 同时包含该点时，按长度排序，连续梁排在前面 ;
                //else
                double x1, x2, x3, x4;
                projectList[i].getSpecialPoint(1, out x1, out x2, out x3, out x4);
                projectList[i].mdis = CoordinateConverter.getUTMDistance(x1, x2, x, y);

                if (projectList[i].mdis < radius)
                    ls.Add(projectList[i]);

            }

            List<CRailwayProject> SortedProj = ls.OrderBy(o => o.mdis).ToList();

            return SortedProj;

        }
        /// <summary>
        ///  根据里程（DK123+45），计算半径radius距离内的所有单位，按距离排序输出
        /// </summary>
        /// <param name="dkCode"></param>
        /// <param name="radius"></param>
        /// <param name="firmList"></param>
        /// <returns></returns>
        public Dictionary<CRailwayFirm, double> getNearFirms(string dkCode, double radius, List<CRailwayFirm> firmList)
        {
            //double[] lenList = new double[firmList.Count];
            Dictionary<CRailwayFirm, double> dict = new Dictionary<CRailwayFirm, double>();
            double xx, yy, zz, dir, len;
            //double xx1,yy1,zz1,dir1;
            CRailwayLineList.getGPSbyDKCode(dkCode, out xx, out yy, out zz, out dir);
            for (int i = 0; i < firmList.Count; i++)
            {
                len = CoordinateConverter.getUTMDistance(firmList[i].CenterLongitude, firmList[i].CenterLatitude, xx, yy);
                if (len < radius)
                    dict.Add(firmList[i], len);

            }

            Dictionary<CRailwayFirm, double> dictSort = dict.OrderBy(o => o.Value).ToDictionary(o => o.Key, p => p.Value);

            return dictSort;

        }


        /// <summary>
        ///  根据里程（DK123+45），计算半径radius距离内的所有单位工程，按距离排序输出
        /// </summary>
        /// <param name="dkCode"></param>
        /// <param name="radius"></param>
        /// <param name="firmList"></param>
        /// <returns></returns>
        public Dictionary<CRailwayDWProj, double> getNearDWProj(string dkCode, double radius, List<CRailwayDWProj> dwProjectList)
        {
            double xx, yy, zz, dir;
            CRailwayLineList.getGPSbyDKCode(dkCode, out xx, out yy, out zz, out dir);
            return getNearDWProj(xx, yy, radius, dwProjectList);
        }

        /// <summary>
        ///  根据经纬度，计算半径radius距离内的所有单位工程，按距离排序输出
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="firmList"></param>
        /// <returns></returns>
        public Dictionary<CRailwayDWProj, double> getNearDWProj(double x, double y, double radius, List<CRailwayDWProj> dwProjectList)
        {
            Dictionary<CRailwayDWProj, double> dict = new Dictionary<CRailwayDWProj, double>(dwProjectList.Count);
            double len;
            //getNearPos(x, y, out mm, out xx, out yy, out zz, out dir);
            for (int i = 0; i < dwProjectList.Count; i++)
            {
                len = CoordinateConverter.getUTMDistance(dwProjectList[i].mLongitude_Mid, dwProjectList[i].mLatitude_Mid, x, y);
                if (len < radius)
                    dict.Add(dwProjectList[i], len);

            }
            Dictionary<CRailwayDWProj, double> dictSort = dict.OrderBy(o => o.Value).ToDictionary(o => o.Key, p => p.Value);

            return dictSort;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dkCode"></param>
        /// <param name="mileage"></param>
        /// <param name="radius"></param>
        /// <param name="projectList"></param>
        /// <returns></returns>
        public Dictionary<CRailwayProject, double> getNearProject(string dkCode, double mileage, double radius, List<CRailwayProject> projectList)
        {
            Dictionary<CRailwayProject, double> dict = new Dictionary<CRailwayProject, double>(projectList.Count);
            double xx, yy, zz, dir, len;

            CRailwayLineList.getGPSbyDKCode(dkCode, mileage, out xx, out yy, out zz, out dir);
            for (int i = 0; i < projectList.Count; i++)
            {
                if (dkCode == projectList[i].mStartDKCode && ((mileage - projectList[i].mMileage_Start) * (mileage - projectList[i].mMileage_End)) < 0)
                    len = projectList[i].Length / 1000; // FIXME, trick, 同时包含该点时，按长度排序，连续梁排在前面 
                else
                {
                    double x1, x2, x3, x4;
                    projectList[i].getSpecialPoint(1, out x1, out x2, out x3, out x4);
                    len = CoordinateConverter.getUTMDistance(x1, x2, xx, yy);
                }
                if (len < radius)
                    dict.Add(projectList[i], len);


            }

            //var dictSort = from d in dict orderby d.Value ascending select d;
            Dictionary<CRailwayProject, double> dictSort = dict.OrderBy(o => o.Value).ToDictionary(o => o.Key, p => p.Value);

            return dictSort;

        }


        ///// <summary>
        /////  根据经纬度，计算半径radius距离内的所有工点，按距离排序输出
        ///// </summary>
        ///// <param name="x"></param>
        ///// <param name="y"></param>
        ///// <param name="radius"></param>
        ///// <param name="firmList"></param>
        ///// <returns></returns>
        //public List<CRailwayProject> getNearProjectbyMars(double x, double y, double radius, List<CRailwayProject> projectList)
        //{
        //    if (x < 10 && y < 10) return null;

        //    List<CRailwayProject> ls = new List<CRailwayProject>();
        //    double mil, dis;
        //    bool isInside;
        //    CRailwayLine rl = CRailwayLineList.getMileagebyMars(x, y, out mil, out dis, out isInside);
        //    //getNearPos(x, y, out mm, out xx, out yy, out zz, out dir);
        //    for (int i = 0; i < projectList.Count; i++)
        //    {
        //        if (rl.mDKCode == projectList[i].mStartDKCode && dis < radius && ((mil - projectList[i].mMileage_Start) * (mil - projectList[i].mMileage_End)) < 0)
        //            projectList[i].mdis = projectList[i].Length / 1000; // FIXME, trick, 同时包含该点时，按长度排序，连续梁排在前面 ;
        //        else
        //        {
        //            double x1, x2,x3,x4;
        //            projectList[i].getSpecialPoint(1, out x1, out x2, out x3, out x4);
        //            projectList[i].mdis = CoordinateConverter.getUTMDistance(x1, x2, x, y);
        //        }

        //        if (projectList[i].mdis < radius)
        //            ls.Add(projectList[i]);

        //    }

        //    List<CRailwayProject> SortedProj = ls.OrderBy(o => o.mdis).ToList();

        //    return SortedProj;

        //}


        public CRailwayProject getProjectByName(string projName)
        {
            //    object res = null;
            foreach (CRailwayProject rp in mTotalProjectList)
            {
                if (rp.ProjectName.Equals(projName))
                    return rp;
            }
            return null;
        }

        public CRailwayProject getProjectBySNo(string sno)
        {
            //    object res = null;
            foreach (CRailwayProject rp in mTotalProjectList)
            {
                if (rp.mSerialNo.Equals(sno))
                    return rp;
            }
            return null;
        }

        public CRailwayFirm getFirmByName(string firmName)
        {
            foreach (CRailwayFirm rf in mFirmList)
            {
                if (rf.FirmName.Equals(firmName))
                    return rf;
            }
            return null;
        }

        public ConsLocation getConsByName(string consName)
        {
            foreach (ConsLocation cl in mConsLoc)
            {
                if (cl.ProjName.Equals(consName))
                    return cl;
            }
            return null;
        }

        public CRailwayPier getPierBySNo(string sno)
        {
            //CRailwayPier rp = null;
            foreach (CRailwayProject cp in mContBeamList)
            {
                foreach (CRailwayPier rp in cp.mPierList)
                {
                    if (sno == rp.mSerialNo)
                    {
                        return rp;
                    }
                }
            }
            foreach (CRailwayProject cp in mBridgeList)
            {
                foreach (CRailwayPier rp in cp.mPierList)
                {
                    if (sno == rp.mSerialNo)
                    {
                        return rp;
                    }
                }
            }
            return null;

        }

        public CRailwayProject GetProjectBy2DCode(int instanceID)
        {
            string str = CServerWrapper.getScanMsg(instanceID);
            if (str == null) return null;
            foreach (CRailwayProject rp in mTotalProjectList)
            {
                if (str.Equals(rp.mSerialNo))
                    return rp;
            }
            return null;
        }

        public List<CHotSpot> GetContBeamNavPath()
        {

            List<CHotSpot> ls = new List<CHotSpot>();
            List<CHotSpot> lsRes;
            foreach (CContBeam cb in mContBeamList)
            {
                if (cb.mIsValid && cb.mMainMileage >0)
                    ls.Add(cb.getHotSpot());
            }
            lsRes = ls.OrderBy(o => o.GlobalMileage).ToList();
            //ls = from d in mTotalSpotList orderby ((IHotSpot)d).GlobalMileage ascending select d;
            return lsRes;
        }

        public List<CHotSpot> GetFirmsNavPath()
        {
            List<CHotSpot> ls = new List<CHotSpot>();
            List<CHotSpot> lsRes = null;
            foreach (CRailwayFirm hs in mFirmList)
            {
                if (hs.getCenterHotSpot() != null)
                    ls.Add(hs.getCenterHotSpot());
            }
            lsRes = ls.OrderBy(o => o.GlobalMileage).ToList();
            return lsRes;
        }

        public List<CHotSpot> GetConsLogNavPath()
        {
            List<CHotSpot> lsRes = mConsSpotList.OrderBy(o => o.GlobalMileage).ToList();
            return lsRes;

        }

        public List<CHotSpot> GetAllNavPathbyMileage()
        {
            List<CHotSpot> ls = new List<CHotSpot>();
            foreach (CRailwayProject rp in mTotalProjectList)
            {
                if (rp.mIsValid && rp.GlobalMileage>0 && rp.mdistanceToMainPath < 6000)
                    ls.Add(rp.getHotSpot());
            }
            foreach (CRailwayFirm hs in mFirmList)
            {
                if (hs.getCenterHotSpot() != null)
                    ls.Add(hs.getCenterHotSpot());
            }
            //ls.AddRange(mConsSpotList);
            List<CHotSpot> lsRes = ls.OrderBy(o => o.GlobalMileage).ToList();

            return lsRes;
        }

        public List<CHotSpot> GetDefaultNavPathByMileage()
        {
            List<CHotSpot> ls = new List<CHotSpot>();
            List<CHotSpot> lsO = new List<CHotSpot>();
            List<CHotSpot> lsResult = new List<CHotSpot>();
            foreach (CContBeam rp in mContBeamList)
            {
                if (rp.mIsValid && rp.GlobalMileage >0 && !string.IsNullOrEmpty(rp.mPhotoUrl))
                {
                    ls.Add(rp.getHotSpot());
                }
            }
            foreach (CRailwayFirm hs in mFirmList)
            {
                if (hs.getCenterHotSpot()!=null && !string.IsNullOrEmpty(hs.mPresentation))
                {
                    ls.Add(hs.getCenterHotSpot());
                }
            }
            lsO = ls.OrderBy(o => o.GlobalMileage).ToList();
            lsResult.AddRange(lsO);
            ls.Clear();
            foreach (CRailwayFirm hs in mFirmList)
            {
                if (hs.getCenterHotSpot() != null )
                {
                    ls.Add(hs.getCenterHotSpot());
                }
            }
            foreach (CRailwayProject rp in mBridgeList)
            {
                if (rp.mIsValid && rp.GlobalMileage > 0 && !string.IsNullOrEmpty(rp.mPhotoUrl))
                {
                    ls.Add(rp.getHotSpot());
                }
            }
            foreach (CRailwayProject rp in mRoadList)
            {
                if (rp.mIsValid && rp.GlobalMileage > 0 && !string.IsNullOrEmpty(rp.mPhotoUrl))
                {
                    ls.Add(rp.getHotSpot());
                }
            }
            foreach (CRailwayProject rp in mTunnelList)
            {
                if (rp.mIsValid && rp.GlobalMileage > 0 && !string.IsNullOrEmpty(rp.mPhotoUrl))
                {
                    ls.Add(rp.getHotSpot());
                }
            }
            lsO = ls.OrderBy(o => o.GlobalMileage).ToList();
            lsResult.AddRange(lsO);
            return lsResult;
        }

        public List<CHotSpot> GetConsByGPS(double cx, double cy, double delta)
        {
            List<CHotSpot> ls = new List<CHotSpot>();
            foreach (CHotSpot hs in mConsSpotList)
            {
                double dis = hs.getUTMDistance(cx, cy);
                if (dis < delta)
                {
                    ls.Add(hs);
                }
            }
            return ls;
        }
        //            foreach (CHotSpot hs in ls)
        //            {
        //                ConsLocation cl = (ConsLocation)(hs.ObjectRef);
        //                str.Append(cl.Number);
        //                str.Append(" 人次于");
        //                str.Append(cl.ProjName);
        //                str.Append("工作\t\t");

        //            }
        public string GetStrConsByGPS(double cx, double cy, double delta)
        {
            StringBuilder sb = new StringBuilder();
            foreach (CHotSpot hs in mConsSpotList)
            {
                double dis = hs.getUTMDistance(cx, cy);
                if (dis < delta)
                {
                    ConsLocation cl = (ConsLocation)(hs.ObjectRef);
                    sb.Append(cl.Number);
                    sb.Append(" 人次于 ");
                    sb.Append(cl.ProjName);
                    sb.Append(" 工作。 ");

                }
            }
            return sb.ToString();
        }

        // 包含该里程的工点
        private CRailwayProject GetProjectByMileage(List<CRailwayProject>  ls, double mileage)
        {
            foreach (CRailwayProject p in ls)
            {
                if (p.mIsValid)
                {
                    if (p.mMainMileageS < mileage && mileage < p.mMainMileageE)
                    {
                        return p;
                    }
                }
            }
            return null;
        }
        public CRailwayProject GetProjectByMileage(double mileage)
        {
            CRailwayProject p = null;
            p = GetProjectByMileage(mContBeamList, mileage);
            if (p == null)
                p = GetProjectByMileage(mBridgeList, mileage);
            if (p == null)
                p = GetProjectByMileage(mRoadList, mileage);
            if (p == null)
                p = GetProjectByMileage(mTunnelList, mileage);

            return p;
        }

        public CRailwayPier GetPierByMileage(double mileage, double delta = 15)
        {
            foreach (CContBeam p in mContBeamList)
            {
                if (p.mIsValid)
                {
                    if (p.mMainMileageS < mileage && mileage < p.mMainMileageE)
                    {
                        foreach (CRailwayPier rp in p.mPierList)
                        {
                            if (mileage - delta < rp.mMainMileage && rp.mMainMileage < mileage + delta)
                                return rp;

                        }
                    }
                }
            }
            foreach (CRailwayBridge p in mBridgeList)
            {
                if (p.mIsValid)
                {
                    if (p.mMainMileageS < mileage && mileage < p.mMainMileageE)
                    {
                        foreach (CRailwayPier rp in p.mPierList)
                        {
                            if (mileage - 15 < rp.mMainMileage && rp.mMainMileage < mileage + 15)
                                return rp;

                        }
                    }
                }
            }

            return null;
        }

        public List<CRailwayProject> GetProjectOrderbyMileage()
        {
            List<CRailwayProject> ls = mTotalProjectList.OrderBy(o => o.mMainMileageS).ToList();
            return ls;
        }
        #endregion

        #region 初始化桥墩
        private void initDWProjects(string dbPath, bool fromLocal = true)
        {
            DataTable dt = null;
        
            if (fromLocal)
            {
                //string fileName = CGisDataSettings.gDataPath + @"jiqing\Pier1.xlsx";
                //dt = DatabaseWrapper.LoadDataTableFromExcel(CGisDataSettings.gDataPath + @"jiqing\Pier1.xlsx", @"SELECT Project_B_DW_ID, Name, ProjectID, MileagePrefix, Mileage, ProjectLength, UpdateTime, ModelType, SerialNo from [Pier$] order by ProjectID asc; ");
                dt = DatabaseWrapper.ExecuteDataTable(dbPath, @"SELECT Project_B_DW_ID, Name, ProjectID, MileagePrefix, Mileage, ProjectLength, FinishTime,  SerialNo, IsFinish from PierInfo order by ProjectID asc, Project_B_DW_ID asc; ");
                initPierList(dt, mBridgeList);
                dt = DatabaseWrapper.ExecuteDataTable(dbPath, @"SELECT Project_B_DW_ID, Name, ProjectID, MileagePrefix, Mileage, ProjectLength, FinishTime,  SerialNo, IsFinish from PierContInfo order by ProjectID asc, Project_B_DW_ID asc; ");
                initPierList(dt, mContBeamList);

                //梁
                dt = DatabaseWrapper.ExecuteDataTable(dbPath, @"SELECT Project_B_DW_ID, Name, ProjectID, MileagePrefix, Mileage, ProjectLength, FinishTime,  SerialNo, IsFinish from BeamInfo order by ProjectID asc, Project_B_DW_ID asc; ");
                initBeamList(dt, mBridgeList);

                dt = DatabaseWrapper.ExecuteDataTable(dbPath, @"SELECT ParentID, ProjectID, Name, IsFinish, SerialNo, Type from ProjFBInfo where Type = '1' order by ProjectID asc, ParentID asc");
                initContBeamList(dt, mContBeamList);

                //桩名字, 所属桥墩id, dw工程类型， 里程前缀及里程， 长度， 更新时间， 模型类型， 序列号， 是否完工
                dt = DatabaseWrapper.ExecuteDataTable(dbPath, @"SELECT ParentID, ProjectID, IsFinish, SerialNo from ProjFBInfo where Type = '2' order by ProjectID asc, ParentID asc"); // xyn 1013 桩
                initStubList(dt, mBridgeList);
                initStubList(dt, mContBeamList);
            }
            else if (CServerWrapper.isConnected)
            {
                //桥墩id, 桥墩名（7#墩）, 工点id, dw工程类型， 里程前缀及里程， 长度， 更新时间， 模型类型， 序列号， 是否完工,,,[FinishTime]???
                dt = CServerWrapper.execSqlQuery(@"SELECT Project_B_DW_ID, Name, ProjectID, MileagePrefix, Mileage, ProjectLength, FinishTime,  SerialNo, IsFinish from Project_B_DW_Info where (Type = 8 or Type = 16)  and mileage > 0 order by ProjectID asc, Project_B_DW_ID asc"); // xyn 1013 普通墩8，连续梁墩16
                if (dt == null) return;
                initPierList(dt, mBridgeList);
                initPierList(dt, mContBeamList);

                dt = CServerWrapper.execSqlQuery(@"SELECT Project_B_DW_ID, Name, ProjectID, MileagePrefix, Mileage, ProjectLength, FinishTime,  SerialNo, IsFinish from Project_B_DW_Info where Type = 13 and mileage > 0 order by ProjectID asc, Project_B_DW_ID asc"); // xyn 1026 预制梁，从属于
                initBeamList(dt, mBridgeList);

                dt = CServerWrapper.execSqlQuery(@"SELECT ParentID, ProjectID, Name, IsFinish, SerialNo from Project_B_DW_Info where Type = 6 order by ProjectID asc, ParentID asc"); // xyn 1026 连续梁块，从属于墩
                initContBeamList(dt, mContBeamList);                
                
                //桩名字, 所属桥墩id, dw工程类型， 里程前缀及里程， 长度， 更新时间， 模型类型， 序列号， 是否完工
                dt = CServerWrapper.execSqlQuery(@"SELECT ParentID, ProjectID, IsFinish, SerialNo from Project_B_DW_Info where Type = 7 order by ProjectID asc, ParentID asc"); // xyn 1013 桩
                initStubList(dt,mBridgeList);
                initStubList(dt,mContBeamList);
            }

        }

        private void initPierList(DataTable dt, List<CRailwayProject> bridgeList)
        {
            int dwID;
            string dwName;
            int projID;
            double dwMileage, dwLength;
            bool isDone;
            string dkCode;
            //string Mileage_Start_Des, Mileage_End_Des, Mileage_Mid_Des, SerialNo;
            DateTime dwFinishTime;
            string dwSNo;
            int curBridgeIndex = 0;

            //@"SELECT Project_B_DW_ID, Name, ProjectID,Type, MileagePrefix, Mileage, ProjectLength, FinishTime, ModelType, SerialNo, IsFinish from Project_B_DW_Info where Type = 8 and mileage > 0 order by ProjectID asc, Project_B_DW_ID asc"   xyn 1013 普通墩8，连续梁墩16

            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    dwID = Convert.ToInt32(dr["Project_B_DW_ID"]);
                    dwName = (string)(dr["Name"]);
                    projID = Convert.ToInt32(dr["ProjectID"]);

                    dkCode = (string)(dr["MileagePrefix"]);
                    //if (dwID == 20533)
                    //    Console.WriteLine(projID);
                    dwMileage = Convert.ToDouble(dr["Mileage"]);
                    dwLength = Convert.ToDouble(dr["ProjectLength"]);
                    
                    dwFinishTime = Convert.ToDateTime(dr["FinishTime"]);
                    dwSNo = (string)dr["SerialNo"];
                    //Console.WriteLine(dr["IsFinish"].GetType());
                    if (dr["IsFinish"] is DBNull)
                        isDone = false;
                    else { 
                        isDone = Convert.ToBoolean(dr["IsFinish"]) ;
                    }
                    
                    //bool findParent = false;
                    while (curBridgeIndex < bridgeList.Count && projID > bridgeList[curBridgeIndex].mProjectID)
                    {
                        curBridgeIndex++;

                    }

                    if (curBridgeIndex >= bridgeList.Count) break;

                    if (projID == bridgeList[curBridgeIndex].mProjectID)
                        bridgeList[curBridgeIndex].mPierList.Add(
                            new CRailwayPier(bridgeList[curBridgeIndex], projID, dwSNo, dwID, dwName, 
                            dwMileage, dwLength, dwFinishTime, isDone, dkCode));
                    //else
                    //    Console.WriteLine("桥墩无对应桥梁" + dwID);

                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(dr["Name"] + " invalid");
                }
            }
        }

        private void initBeamList(DataTable dt, List<CRailwayProject> bridgeList)
        {
            int dwID;
            string dwName;
            int projID;
            double dwMileage, dwLength;
            bool isDone;
            string dkCode;
            //string Mileage_Start_Des, Mileage_End_Des, Mileage_Mid_Des, SerialNo;
            DateTime dwFinishTime;
            string dwSNo;
            int curBridgeIndex = 0;

            //@"SELECT Project_B_DW_ID, Name, ProjectID,Type, MileagePrefix, Mileage, ProjectLength, FinishTime, ModelType, SerialNo, IsFinish from Project_B_DW_Info where Type = 8 and mileage > 0 order by ProjectID asc, Project_B_DW_ID asc"   xyn 1013 普通墩8，连续梁墩16

            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    dwID = Convert.ToInt32(dr["Project_B_DW_ID"]);
                    dwName = (string)(dr["Name"]);
                    projID = Convert.ToInt32(dr["ProjectID"]);

                    dkCode = (string)(dr["MileagePrefix"]);
                    dwMileage = Convert.ToDouble(dr["Mileage"]);
                    dwLength = Convert.ToDouble(dr["ProjectLength"]);

                    dwFinishTime = Convert.ToDateTime(dr["FinishTime"]);
                    dwSNo = (string)dr["SerialNo"];
                    //Console.WriteLine(dr["IsFinish"].GetType());
                    if (dr["IsFinish"] is DBNull)
                        isDone = false;
                    else
                    {
                        isDone = Convert.ToBoolean(dr["IsFinish"]);
                    }

                    //bool findParent = false;
                    while (curBridgeIndex < bridgeList.Count && projID > bridgeList[curBridgeIndex].mProjectID)
                    {
                        curBridgeIndex++;

                    }

                    if (curBridgeIndex >= bridgeList.Count) break;

                    if (projID == bridgeList[curBridgeIndex].mProjectID)
                        bridgeList[curBridgeIndex].mBeamList.Add(
                            new CRailwayBeam(bridgeList[curBridgeIndex], projID, dwSNo, dwID, dwName,
                            dwMileage, dwLength, dwFinishTime, isDone, dkCode));
                    //else
                    //    Console.WriteLine("桥墩无对应桥梁" + dwID);

                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(dr["Name"] + " invalid");
                }
            }

     
        }

        private void initContBeamList(DataTable dt, List<CRailwayProject> bridgeList)
        {
            //DataTable dt = null;
            int bridgeID;
            int pierID;
            bool isDone;

            //if (CServerWrapper.isConnected)
            {
                CRailwayProject bridge;
                CRailwayPier pier;
                int curBridgeIndex = 0;
                int curPierIndex = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        bridgeID = Convert.ToInt32(dr["ProjectID"]);
                        pierID = Convert.ToInt32(dr["ParentID"]);

                        // 定位桥梁
                        while (curBridgeIndex < bridgeList.Count && bridgeID > bridgeList[curBridgeIndex].mProjectID)
                        {
                            curBridgeIndex++;
                            curPierIndex = 0; // 下一座桥的桥墩
                        }
                        if (curBridgeIndex == bridgeList.Count) break;
                        if (bridgeID < bridgeList[curBridgeIndex].mProjectID)
                        {
                            //Console.WriteLine("桩无对应桥梁" + bridgeID);
                            continue;
                        }

                        bridge = bridgeList[curBridgeIndex];
                        // 定位所属的桥墩
                        while (curPierIndex < bridge.mPierList.Count && pierID > bridge.mPierList[curPierIndex].mDWID)
                        {
                            curPierIndex++;
                        }
                        if (curPierIndex == bridge.mPierList.Count)
                        {
                            continue;
                        }
                        if (pierID < bridge.mPierList[curPierIndex].mDWID)
                        {
                            //Console.WriteLine("梁无对应桥墩" + bridgeID + "\t" + pierID);
                            continue;
                        }

                        pier = (CRailwayPier)bridge.mPierList[curPierIndex];

                        pier.mBeamSNOs.Add(dr["SerialNo"].ToString());
                        if (dr["IsFinish"] is DBNull)
                            isDone = false;
                        else
                        {
                            isDone = Convert.ToBoolean(dr["IsFinish"]);
                        }
                        pier.mBeamDone.Add(isDone);
                        pier.mBeamName.Add(dr["Name"].ToString());


                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine("ContBeam" + curPierIndex + "\t" + dr["SerialNo"].ToString() + " invalid");
                    }
                }

            }
        }

        private void initStubList(DataTable dt, List<CRailwayProject> bridgeList)
        {
            //DataTable dt = null;
            int bridgeID;
            int pierID;
            bool isDone;

            //if (CServerWrapper.isConnected)
            {
                CRailwayProject bridge;
                CRailwayPier pier;
                int curBridgeIndex = 0;
                int curPierIndex = 0;
                if (dt == null) return;
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        bridgeID = Convert.ToInt32(dr["ProjectID"]);
                        pierID = Convert.ToInt32(dr["ParentID"]);

                        // 定位桥梁
                        while (curBridgeIndex < bridgeList.Count && bridgeID > bridgeList[curBridgeIndex].mProjectID)
                        {
                            curBridgeIndex++;
                            curPierIndex = 0; // 下一座桥的桥墩
                        }
                        if (curBridgeIndex == bridgeList.Count) break;
                        if (bridgeID < bridgeList[curBridgeIndex].mProjectID)
                        {
                            //Console.WriteLine("桩无对应桥梁" + bridgeID);
                            continue;
                        }

                        bridge = bridgeList[curBridgeIndex];
                        // 定位所属的桥墩
                        while (curPierIndex < bridge.mPierList.Count && pierID > bridge.mPierList[curPierIndex].mDWID)
                        {
                            curPierIndex++;
                        }
                        if (curPierIndex == bridge.mPierList.Count) {                            
                            continue;
                        }
                        if (pierID < bridge.mPierList[curPierIndex].mDWID)
                        {
                            //Console.WriteLine("桩无对应桥墩" + bridgeID + "\t" + pierID);
                            continue;
                        }

                        pier = (CRailwayPier)bridge.mPierList[curPierIndex];

                        pier.mStubSNOs.Add(dr["SerialNo"].ToString());
                        if (dr["IsFinish"] is DBNull)
                            isDone = false;
                        else
                        {
                            isDone = Convert.ToBoolean(dr["IsFinish"]);
                        }
                        pier.mStubDone.Add(isDone);

                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine("Stub" + curPierIndex + "\t" + dr["SerialNo"].ToString()+ " invalid");
                    }
                }               


            }
        }


        #endregion


        #region 初始化工点
        /// <summary>
        ///giscode_station	-1-42-28-84-
        ///giscode_bridge	-1-42-26-
        ///giscode_road	-1-42-28-
        ///giscode_liangchang	-1-42-31-
        ///giscode_tunnel	-1-42-27-
        ///giscode_handong	-1-42-74-
        /// </summary>
        private void initProjects(string dbPath, bool fromLocal = true)
        {

            System.Data.DataTable dt = null;

            if (CServerWrapper.isConnected && !fromLocal)
            {
                ///修正桥梁，AND ParentID > 0
                string ProjectSQL = @"SELECT ProjectID,ParentID, ProjectName, ProfessionalName, ProfessionalCategoryCode, ShorName,Mileage_Start_Des,Mileage_End_Des, Mileage_Start, Mileage_End, 
                  MileagePrefix , SerialNo, UpdateTime, Direction from vw_ProjectInfo where ProfessionalCategoryCode like '-1-42-26-%'  OR
    ProfessionalCategoryCode like '-1-42-27-%' OR ProfessionalCategoryCode like '-1-42-28-%' order by ProjectID asc;";
                dt = CServerWrapper.execSqlQuery(ProjectSQL);
                initSomeProj(dbPath,dt, false);
            }
            else if (fromLocal)
            {
                //桥梁
                //string localPath = CGisDataSettings.gDataPath + CGisDataSettings.gCurrentProject.projectLocalPath;

//                dt = DatabaseWrapper.LoadDataTableFromExcel(localPath + @"ProjectInfo.xlsx",
//@"SELECT ProjectID, ProjectName, ProfessionalName, ProfessionalCategoryCode, ShorName,Mileage_Start,Mileage_Mid, Mileage_End, MileagePrefix , SerialNo, UpdateTime, Direction , avgProgress from [Project$] order by ProjectID asc;");
                dt = DatabaseWrapper.ExecuteDataTable(dbPath, @"SELECT ProjectID,ParentID, ProjectName, ProfessionalName, ProfessionalCategoryCode, ShorName,Mileage_Start_Des, Mileage_End_Des, Mileage_Start, Mileage_End, SerialNo, UpdateTime, Direction, PhotoUrl from ProjectInfo order by ProjectID asc;");
                initSomeProj(dbPath, dt,true);

            }

        }

        private void initSomeProj(String dbPath,DataTable dt, bool fromLocal = true)
        {
            //List<CRailwayProject> ls = new List<CRailwayProject>();
            string projectName, professionalName, ProfessionalCategoryCode, ShorName;
            int projID,parentID;
            double Mileage_Start, Mileage_End;
            string DKCodeS, DKCodeE;
            string  SerialNo;
            DateTime UpdateTime;
            double Direction, avgProgress;
            string photoUrl = "";

            CRailwayProject sItem = null;
            mTotalProjectList.Clear();
            mBridgeList.Clear();
            mTunnelList.Clear();
            mRoadList.Clear();
            mContBeamList.Clear();

            try
            {
                //DataTable dt1 = DatabaseWrapper.LoadDataTableFromExcel(CGisDataSettings.gDataPath + @"jiqing\ProjFX.xlsx", @"SELECT * from [fxDict$] where ProjectID = " + this.mProjectID);
                DataTable dt1 = null;
                DataTable dt2 = null;
                if (!CServerWrapper.isConnected || fromLocal)
                {
                    //dt1 = DatabaseWrapper.LoadDataTableFromExcel(CGisDataSettings.gDataPath + @"jiqing\ProjFX.xlsx", @"SELECT * from [fxDict$]; ");
                    //dt2 = DatabaseWrapper.LoadDataTableFromExcel(CGisDataSettings.gDataPath + @"jiqing\ProjFX.xlsx", @"SELECT * from [fxData$]; ");
                    dt1 = DatabaseWrapper.ExecuteDataTable(dbPath,@"SELECT * from FXDict; ");
                    dt2 = DatabaseWrapper.ExecuteDataTable(dbPath,@"SELECT * from FXData; ");
                }
                foreach (DataRow dataReader in dt.Rows)
                {
                    projID = Convert.ToInt32(dataReader["ProjectID"]);
                    parentID = Convert.ToInt32(dataReader["ParentID"]);
                    Mileage_Start = Convert.ToDouble(dataReader["Mileage_Start"]);
                    Mileage_End = Convert.ToDouble(dataReader["Mileage_End"]);
                    DKCodeS = (string)dataReader["Mileage_Start_Des"];
                    DKCodeE = (string)dataReader["Mileage_End_Des"];
                    avgProgress = 0;
                    Direction = Convert.ToDouble(dataReader["Direction"]);
                    projectName = (string)dataReader["ProjectName"];
                    professionalName = (string)dataReader["ProfessionalName"];
                    ProfessionalCategoryCode = (string)dataReader["ProfessionalCategoryCode"];
                    ShorName = (string)dataReader["ShorName"];
                    SerialNo = (string)dataReader["SerialNo"];
                    UpdateTime = Convert.ToDateTime(dataReader["UpdateTime"]);
                    
                    if (!CServerWrapper.isConnected || fromLocal )
                    {
                        if (dataReader["PhotoUrl"] != null)                            
                            photoUrl = dataReader["PhotoUrl"].ToString();
                    }
                    else
                    {
                        DataTable dttmp = null;
                        dttmp = CServerWrapper.showPhotosByProjSNo(SerialNo);
                        if (dttmp != null && dttmp.Rows.Count > 0)
                        {
                            photoUrl = dttmp.Rows[0]["Url"].ToString();
                        }
                        else
                        {
                            photoUrl = "";
                        }
                    }


                    if (ProfessionalCategoryCode.StartsWith("-1-42-26-32-")) // 非连续梁桥梁
                    {


                        // FIXME 跨395省道大桥 错误，
                        //if (projectName.StartsWith("跨395省道大桥")) continue;
                        //济南特大桥 abc段，数据多余
                        //if (projID == 140 || projID == 141 || projID == 142) continue;
                        sItem = new CRailwayBridge(this, SerialNo, projID,parentID, professionalName, ProfessionalCategoryCode, projectName, ShorName, 
                            DKCodeS,DKCodeE, Mileage_Start, Mileage_End, UpdateTime, avgProgress, Direction, @"桥梁.png", photoUrl, dt1, dt2);
                        
                        mBridgeList.Add((CRailwayBridge)sItem);

                        mTotalProjectList.Add(sItem);


                    }
                    else if (ProfessionalCategoryCode.StartsWith("-1-42-26-81-")) // 连续梁
                    {
                        sItem = new CContBeam(this, SerialNo, projID, parentID, professionalName, ProfessionalCategoryCode, projectName, ShorName,
                            DKCodeS, DKCodeE, Mileage_Start, Mileage_End, UpdateTime, avgProgress, Direction, @"桥梁.png", photoUrl, dt1, dt2);
                        
                        mContBeamList.Add((CContBeam)sItem);
                        mTotalProjectList.Add(sItem);
                    }
                    else if (ProfessionalCategoryCode.StartsWith("-1-42-27-")) // 隧道，涵洞
                    {
                        // FIXME 机场隧道77，青阳隧道 207，数据多余。
                        //if (projID == 77 || projID == 207) continue;
                        sItem = new CRailwayTunnel(this, SerialNo, projID, parentID, professionalName, ProfessionalCategoryCode, projectName, ShorName,
                            DKCodeS, DKCodeE, Mileage_Start, Mileage_End, UpdateTime, avgProgress, Direction, @"涵洞.png", photoUrl, dt1, dt2);
                        
                        
                        mTunnelList.Add((CRailwayTunnel)sItem);

                        mTotalProjectList.Add(sItem);

                    }
                    else if (ProfessionalCategoryCode.StartsWith("-1-42-28-")) // 路基
                    {
                        sItem = new CRailwayRoad(this, SerialNo, projID, parentID, professionalName, ProfessionalCategoryCode, projectName, ShorName,
                            DKCodeS, DKCodeE, Mileage_Start, Mileage_End, UpdateTime, avgProgress, Direction, @"路基.png", photoUrl, dt1, dt2);
                        
                        
                        mRoadList.Add((CRailwayRoad)sItem);
                        mTotalProjectList.Add(sItem);


                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            //return ls;

        }


        #endregion

        #region 初始化单位
        /// <summary>
        ///   string sqlstr = @"select FirmName ,a.FirmTypeID, CategoryCode, SerialNo, UpdateTime, Longitude, Latitude from (select * from FirmInfo)a, (select FirmTypeID,FirmTypeCategoryName from FirmTypeInfo)b where a.FirmTypeID=b.FirmTypeID and( FirmTypeCategoryName='单位' or FirmTypeCategoryName='分支机构') and Longitude > 10 AND Latitude > 10 order by a.FirmTypeID asc ;"; 
        /// </summary>
        private void initFirms(string dbPath, bool fromLocal = true) //string projCode, List<CRailwayProject> projList, string fileName
        {

            DataTable dt = null;
            CRailwayFirm sItem = null;

            try
            {
                mFirmList.Clear();
                if (fromLocal)
                {
                    //string localPath = CGisDataSettings.gDataPath + CGisDataSettings.gCurrentProject.projectLocalPath;

                    //dt = DatabaseWrapper.LoadDataTableFromExcel(localPath + @"FirmInfo.xlsx", @"SELECT * from [Firm$] ;");
                    dt = DatabaseWrapper.ExecuteDataTable(dbPath, @"SELECT * from FirmInfo;");
                    foreach (DataRow dr in dt.Rows)
                    {
                        sItem = new CRailwayFirm(this, Convert.ToInt32(dr["Num"]), Convert.ToInt32(dr["firmid"]), dr["ShorName"].ToString(),
                            Convert.ToDouble(dr["Longitude"]), Convert.ToDouble(dr["Latitude"]), dr["FirmType"].ToString(),dr["Presentation"].ToString(), dr["FirmIcon"].ToString());

                        mFirmList.Add(sItem);
                    }

                } else if (CServerWrapper.isConnected)
                {
                    string ProjectSQL;

                    int firmID;
                    string fileName = null;
                    string firmType = null;

                    ProjectSQL = @"select a.*,b.ShorName,b.Latitude,b.Longitude,b.FirmTypeID, c.FirmTypeCategoryName from 
                    (select count(usrid) as num,firmid from UsrInfo 
                    where  idcardno is not null group by firmid)a,
                    (select latitude,longitude,firmid,ShorName,FirmTypeID from FirmInfo
                    where Latitude!=0 and Longitude!=0)b,
                    (select FirmTypeID,FirmTypeCategoryName from FirmTypeInfo
                    where FirmTypeCategoryName='单位' or FirmTypeCategoryName='分支机构')c
                    where a.firmid=b.FirmID and b.FirmTypeID=c.FirmTypeID";
                    //ProjectSQL = @"select FirmName ,a.FirmTypeID, CategoryCode, SerialNo, UpdateTime, Longitude, Latitude from (select * from FirmInfo)a, (select FirmTypeID,FirmTypeCategoryName from FirmTypeInfo)b where a.FirmTypeID=b.FirmTypeID and( FirmTypeCategoryName='单位' or FirmTypeCategoryName='分支机构') and Longitude > 10 AND Latitude > 10 order by a.FirmTypeID asc ;"; 
                    //ProjectSQL += CServerWrapper.findProjectCode(projCode) + @"%' order by ProjectID asc;"; 
                    //ProjectSQL += projCode + @"%' order by ProjectID asc;";
                    dt = CServerWrapper.execSqlQuery(ProjectSQL);
                    foreach (DataRow dataReader in dt.Rows)
                    {
                        firmID = Convert.ToInt32(dataReader["FirmTypeID"]);
                        switch (firmID)
                        {
                            case 2:
                                firmType = "施工单位";
                                fileName = @"施工单位.png";
                                break;
                            case 5:
                                firmType = "建设单位";
                                fileName = @"单位.png";
                                break;
                            case 7:
                                firmType = "制梁场";
                                fileName = @"LC.png";
                                break;
                            case 8:
                                firmType = "监理单位";
                                fileName = @"监理单位.png";
                                break;
                            case 10:
                                firmType = "项目部";
                                fileName = @"工程.png";
                                break;
                        }

                        sItem = new CRailwayFirm(this, Convert.ToInt32(dataReader["Num"]), Convert.ToInt32(dataReader["firmid"]), dataReader["ShorName"].ToString(),
                            Convert.ToDouble(dataReader["Longitude"]), Convert.ToDouble(dataReader["Latitude"]), firmType,"", fileName);

                        mFirmList.Add(sItem);
                    }

                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }


        }
        #endregion


        //class CProjectCompareProgress : IComparer<CRailwayProject>
        //{
        //    public int Compare(CRailwayProject p1, CRailwayProject p2)
        //    {
        //        if (p1.AvgProgress > p2.AvgProgress)
        //        {
        //            return 1;
        //        }
        //        else if (p1.AvgProgress == p2.AvgProgress)
        //        {
        //            return 0;
        //        }
        //        else
        //        {
        //            return -1;
        //        }
        //    }
        //}bai

    }
}
