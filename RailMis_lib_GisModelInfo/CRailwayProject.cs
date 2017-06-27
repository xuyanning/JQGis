using System;
using System.Collections.Generic;
using System.ComponentModel; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.OleDb;
using System.Data;
using System.Globalization;
using ModelInfo.Helper;
using System.Drawing;


namespace ModelInfo
{
    public class CRWPosition
    {
        public double meter;
        public double latitude;
        public double longitude;
        public double altitude;
        public double heading;
        public double pitching;
        public double utmX;
        public double utmY;
    }


    //public class CRailwayStation
    //{
    //    public string mStationName;
    //    public double mX;
    //    public double mY;
    //    public double mZ;
    //    public double mR = 0.001;
    //    public double mHeight = 100;
    //    //public ITerrainBuilding66 mModel;
    //    public CRailwayStation(string name, double x, double y, double z)
    //    {
    //        mStationName = name;
    //        mX = x;
    //        mY = y;
    //        mZ = z;
    //    }
    //}

    /// <summary>
    /// 分项工程
    /// </summary>
    public class CFXProj
    {
        public int fxID;
        private string fxName;

        [CategoryAttribute("分项信息"), DisplayName("01分项名称")]
        public string FxName
        {
            get { return fxName; }
            set { fxName = value; }
        }
        private double totalAmount;
        [CategoryAttribute("分项信息"), DisplayName("02设计总量")]
        public double TotalAmount
        {
            get { return totalAmount; }
            set { totalAmount = value; }
        }

        public double initAmount;
        //private string latestDate;
        [CategoryAttribute("分项信息"), DisplayName("04统计截至")]
        public string LatestDate
        {
            get
            {
                if (strDate.Count > 0)
                    return strDate.Last();
                return "20160101";
            }
            //set { latestDate = value; }
        }

        private double latestDone;
        [CategoryAttribute("分项信息"), DisplayName("03完成总量")]
        public double LatestDone
        {
            get
            {
                if (doneAmount.Count > 0)
                    return doneAmount.Last();
                return 0;
            }
            //set { latestDone = value; }
        }

        public List<string> strDate;  // 按照日期正向排序，日期格式 yyyyMMdd
        public List<double> doneAmount;
        public CFXProj(int id, string name, double total)
        {
            fxID = id;
            fxName = name;
            totalAmount = total;
            strDate = new List<string>();
            doneAmount = new List<double>();

        }
        public override string ToString()
        {
            return fxName;
            //        return fxID + "分项工程名称：" + fxName + "\t设计总量：" + totalAmount + "\t完成量：" + doneAmount.Last() + "\t完成比例：" + Math.Round(doneAmount.Last() / totalAmount, 3) * 100 +
            //"%\t最后更新日期： " + strDate.Last() + "\n";
        }

    }


    // 工点
    public class CRailwayProject
    {

        public CRailwayScene mScene;

        private string mProjectName;  // 工点名称
        [CategoryAttribute("工点信息"), DisplayName("01工点名称")]
        public string ProjectName
        {
            get { return mProjectName; }
            set { mProjectName = value; }
        }

        [CategoryAttribute("工点信息"), DisplayName("02工点类型")]
        public String ProfessionalName { get; set; }  //工点类型  :桥，路基，涵洞，站点，其他
                                                      //[CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("显示信息"), Browsable(false)]
                                                      //public string ShowMessage{ get; set; }

        private string mSegmentName;  // 所属标段
        [CategoryAttribute("工点信息"), DisplayName("03所属标段")]
        public string SegmentName
        {
            get { return mSegmentName; }
            set { mSegmentName = value; }
        }


        private DateTime mUpdateTime; // 更新时间        
        [CategoryAttribute("工点信息"), DisplayName("04更新时间"), Browsable(true)]
        public DateTime UpdateTime
        {
            get { return mUpdateTime; }
            set { mUpdateTime = value; }
        }

        public double mAvgProgress; // 进度      
        [CategoryAttribute("工点信息"), DisplayName("08整体进度")]
        public string AvgProgress
        {
            get { return (int)(mAvgProgress * 100 + 0.5) + ""; }
            //set { mAvgProgress = value; }
        }

        protected List<CHotSpot> mSpotList = null; //遍历漫游

        private string Mileage_Start_Ds;
        [CategoryAttribute("工点信息"), DisplayName("05起始里程")]
        public string Mileage_Start_Discription
        {
            get { return Mileage_Start_Ds; }
            set { Mileage_Start_Ds = value; }
        }
        //public string Mileage_Mid_Ds;

        private string Mileage_End_Ds;

        [CategoryAttribute("工点信息"), DisplayName("06终止里程")]
        public string Mileage_End_Discription
        {
            get { return Mileage_End_Ds; }
            set { Mileage_End_Ds = value; }
        }

        public string mStartDKCode;
        public string mEndDKCode;
        public string mMidDKCode;


        public double mMileage_Start;
        protected double mMileage_Mid;

        [CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("中心里程"), Browsable(false)]
        public double CenterMileage
        {
            get { return mMileage_Mid; }
            set { mMileage_Mid = value; }
        }
        public double mMileage_End;  // 起止里程，中点里程

        //public string mDKCode_Mid;
        //public string mDKCode_End;

        protected double mLength;  // 工点长度
        [CategoryAttribute("工点信息"), DisplayName("工点长度"), Browsable(false)]
        public double Length
        {
            get { return mLength; }
            set { mLength = value; }
        }

        [CategoryAttribute("工点信息"), DisplayName("07工点长度")]
        public string DisplayLength
        {
            get { return String.Format("{0:F}", mLength); }
        }

        public bool mIsValid;

        public double mDirection;  // 朝向

        public string mLabelImage;  //文件名

        public string mSerialNo;  // 序列号编码
        public int mProjectID;
        public int mParentID;

        public List<CRailwayPier> mPierList = new List<CRailwayPier>();
        public List<CRailwayBeam> mBeamList = new List<CRailwayBeam>();

        private List<CFXProj> mfx = new List<CFXProj>();
        [CategoryAttribute("工点信息"), DisplayName("09分项工程进度"), Browsable(false)] //ReadOnlyAttribute(true),
        public List<CFXProj> FXProgress
        {
            get { return mfx; }
            //set { mfx = value; }
        }

        public int[] selectedFXid = new int[3];

        public string mProfessionalCode;

        //private DataSet mds = null;
        public CSubPath mPath = null;
        public bool mIsOnMainPath = true;
        public double mdistanceToMainPath = 0;
        public double mMainMileage = 0;
        public double mMainMileageS = 0;
        public double mMainMileageE = 0;
        public string mPhotoUrl = "";

        public double mdis; // 距离，用于比较某点距离
        public double mDistanceToMainPath;

        [CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("全局里程"), Browsable(false)]
        public double GlobalMileage
        {
            get { return mMainMileage; }
            set { mMainMileage = value; }
        }

        public CRailwayProject(CRailwayScene s)
        {
            mScene = s;
            ProfessionalName = "铁路工程";
            mProjectID = 1;
            mProjectName = "新建济青高速铁路工程";
            mSegmentName = "山东高速集团";
            mSerialNo = "XXXX";
            mStartDKCode = "DK";
            mEndDKCode = "DK";

            mMileage_Start = 0;
            mMileage_Mid = 158000;
            mMileage_End = 316000;

            Mileage_Start_Ds = CRailwayLineList.CombiDKCode(mStartDKCode, mMileage_Start);
            //Mileage_Mid_Ds = Mileage_Mid_Des;
            Mileage_End_Ds = CRailwayLineList.CombiDKCode(mEndDKCode, mMileage_End);
            mLength = Math.Abs(mMileage_End - mMileage_Start);
            mUpdateTime = DateTime.Now;
            // FIXME 应该进行统计
            mfx = new List<CFXProj>();
            //mfx.Add(new CFXProj(0,"桥梁",1000));
            //mfx.Add(new CFXProj(1, "路基", 10000));
            //mfx.Add(new CFXProj(2, "隧道", 10000));
        }
        /// <summary>
        /// 利用dkcode 与 里程初始化，潜在问题，要求在同一个链中，dkcode一致，目前数据库中的里程描述description不正确
        /// </summary>
        /// <param name="s"></param>
        /// <param name="SerialNo"></param>
        /// <param name="projID"></param>
        /// <param name="profName"></param>
        /// <param name="ProjectName"></param>
        /// <param name="SegmentName"></param>
        /// <param name="DKCode"></param>
        /// <param name="Mileage_Start"></param>
        /// <param name="Mileage_Mid"></param>
        /// <param name="Mileage_End"></param>
        /// <param name="dt"></param>
        /// <param name="AvgProgress"></param>
        /// <param name="dir"></param>
        /// <param name="labelFile"></param>
        /// <param name="length"></param>
        public CRailwayProject(CRailwayScene s, string SerialNo, int projID, int parentID, string profName, string profCode, string ProjectName, string SegmentName,
            string MStartDes, string MEndDes, double Mileage_Start, double Mileage_End,
            //string Mileage_Start_Des, string Mileage_Mid_Des, string Mileage_End_Des, 
            DateTime dt, double AvgProgress, double dir, string labelFile, string photoUrl, bool isContBeam = false, DataTable dt1 = null, DataTable dt2 = null)
        {
            double tmp;
            mScene = s;
            ProfessionalName = profName;
            mProfessionalCode = profCode;
            mProjectID = projID;
            mParentID = parentID;
            mProjectName = ProjectName;
            mSegmentName = SegmentName;
            mSerialNo = SerialNo;
            mIsValid = CRailwayLineList.parseDKCode(MStartDes, out mStartDKCode, out tmp); // should be Mileage_Start
            mIsValid &= CRailwayLineList.parseDKCode(MEndDes, out mEndDKCode, out tmp); // should be Mileage_End
            if (!mIsValid)
            {
                ModelInfo.Helper.LogHelper.WriteLog(mProjectName + "里程错误：" + MStartDes + "\t" + MEndDes);
            }
            //if (string.IsNullOrEmpty(dkcode2))
            //    mEndDKCode = mStartDKCode;
            //else
            //    mEndDKCode = dkcode2;

            Mileage_Start_Ds = CRailwayLineList.CombiDKCode(mStartDKCode, Mileage_Start);
            //Mileage_Mid_Ds = Mileage_Mid_Des;
            Mileage_End_Ds = CRailwayLineList.CombiDKCode(mEndDKCode, Mileage_End); ;

            mMileage_Start = Mileage_Start;
            mMileage_Mid = 0;
            mMileage_End = Mileage_End;

            //mIsValid = mScene.mMiddleLines.getGPSbyDKCode(mStartDKCode, mMileage_Mid, out mLongitude_Mid, out mLatitude_Mid, out mAltitude_Mid, out mHeading_Mid);
            //mIsValid = mScene.mMiddleLines.getGPSbyDKCode(mStartDKCode, mMileage_Start, out mLongitude_Start, out mLatitude_Start, out mAltitude_Start, out mHeading_Start);
            //mIsValid &= mScene.mMiddleLines.getGPSbyDKCode(mEndDKCode, mMileage_End, out mLongitude_End, out mLatitude_End, out mAltitude_End, out mHeading_End);


            mUpdateTime = dt;

            mDirection = dir;
            mLabelImage = labelFile;
            mLength = Math.Abs(mMileage_End - mMileage_Start); //FIXME 目前是导入，不同DKCode的线路如何求解

            mPhotoUrl = photoUrl;

            if (!isContBeam)
            {

                //mPath = new CSubPath( mStartDKCode, mMileage_Start, mEndDKCode, mMileage_End, 10);
                mPath = new CSubPath(CRailwayLineList.gMileageConnection, mStartDKCode, mMileage_Start, mEndDKCode, mMileage_End, 10);
                if (!mPath.hasPath)
                {
                    mIsValid = false;
                    //#if DEBUG
                    //                    Helper.LogHelper.WriteLog(mProjectName +"\t"+ mStartDKCode + mMileage_Start + "\t" + mEndDKCode + mMileage_End);
                    //#endif
                    return;
                }
                mIsValid = true;
                mIsOnMainPath = mScene.mMainPath.getPathMileageByDKCode(mStartDKCode, mMileage_Start, out mMainMileageS, out mdistanceToMainPath);
                mScene.mMainPath.getPathMileageByDKCode(mEndDKCode, mMileage_End, out mMainMileageE, out mdistanceToMainPath);
                mPath.getDKCodebyPathMileage(mPath.mLength * 0.5, out mMidDKCode, out mMileage_Mid);
                mScene.mMainPath.getPathMileageByDKCode(mMidDKCode, mMileage_Mid, out mMainMileage, out mdistanceToMainPath);


                mLength = Math.Abs(mPath.mLength);
            }

            initFXProgress(dt1, dt2);
        }

        public bool getSpecialPoint(int pos, out double x, out double y, out double z, out double d)
        {
            x = y = z = d = 0;
            if (!mIsValid || mPath == null)
                return false;

            if (pos == 0)
            {
                x = mPath.mx[0];
                y = mPath.my[0];
                z = mPath.mz[0];
                d = mPath.md[0];
            }
            else if (pos == 1)
            {
                int mid = mPath.mPointCount / 2;
                x = mPath.mx[mid];
                y = mPath.my[mid];
                z = mPath.mz[mid];
                d = mPath.md[mid];
            }
            else if (pos == 2)
            {
                int last = mPath.mPointCount - 1;
                x = mPath.mx[last];
                y = mPath.my[last];
                z = mPath.mz[last];
                d = mPath.md[last];
            }
            return true;
        }

        public double getUTMDistance(double x, double y)
        {
            double dis, mil;
            if (mPath == null)
                return -1;

            mPath.getPathMileagebyGPS(x, y, out mil, out dis);
            return dis;
        }
        /// <summary>
        /// 获取分项进度
        /// </summary>
        /// <returns>返回值：分项名称，分项进度 的队列</returns>
        public void initFXProgress(DataTable dt1 = null, DataTable dt2 = null)
        {
            int fid;
            int n = 0;
            double avgProgress = 0;
            string dt;
            CFXProj fxp;

            if (dt1 != null && dt2 != null)
            {
                DataRow[] drsp = dt1.Select("ProjectID =" + mProjectID);
                foreach (DataRow dr in drsp)
                {
                    fid = Convert.ToInt32(dr["ProjectDictID"]);
                    fxp = new CFXProj(fid, dr["ProjectDictName"].ToString(), Convert.ToDouble(dr["DesignNum"]));
                    fxp.initAmount = Convert.ToDouble(dr["InitNum"]);
                    DataRow[] drs = dt2.Select("ProjectID =" + mProjectID + " and ProjectDictID = " + fid, "ReportDate asc");
                    if (drs.Count() == 1)
                    {  // 如果只有一条数据，无法图表显示
                        fxp.strDate.Add(
                            DateTime.ParseExact(drs[0]["ReportDate"].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture).AddDays(-30).ToString("yyyyMMdd"));
                        fxp.doneAmount.Add(0);
                    }
                    foreach (DataRow dr2 in drs)
                    {
                        fxp.strDate.Add(dr2["ReportDate"].ToString());
                        fxp.doneAmount.Add(Convert.ToDouble(dr2["DictTotal"]));
                    }
                    if (drs.Count() > 0)
                    {
                        dt = fxp.strDate.Last();
                        if (dt.CompareTo(mUpdateTime.ToString("yyyyMMdd")) > 0)  // 数据库的工程表的updateDate字段逻辑有误
                            mUpdateTime = DateTime.ParseExact(dt, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                        avgProgress += fxp.doneAmount.Last() / fxp.TotalAmount;
                    }
                    else
                    {
                        fxp.strDate.Add(DateTime.Now.ToString("yyyyMMdd"));
                        fxp.doneAmount.Add(Convert.ToDouble(dr["InitNum"]));  // 添加初始化量，保证每个分项工程都有数据
                        avgProgress += fxp.initAmount / fxp.TotalAmount;
                    }
                    mfx.Add(fxp);
                    n++;

                }
                //createMFX(dlt1.Select("ProjectID =" + mProjectID) , dlt2.Select("ProjectID =" + mProjectID, "ReportDate asc"));
            }
            else if (CServerWrapper.isConnected)
            {
                DataSet ds = CServerWrapper.findProjHistory(mSerialNo);
                dt1 = ds.Tables[0];
                dt2 = ds.Tables[1];
                foreach (DataRow dr in dt1.Rows)
                {
                    fid = Convert.ToInt32(dr["ProjectDictID"]);
                    fxp = new CFXProj(fid, dr["ProjectDictName"].ToString(), Convert.ToDouble(dr["DesignNum"]));
                    fxp.initAmount = Convert.ToDouble(dr["InitNum"]);
                    DataRow[] drs = dt2.Select("ProjectDictID = " + fid, "ReportDate asc");
                    foreach (DataRow dr2 in drs)
                    {
                        fxp.strDate.Add(dr2["ReportDate"].ToString());
                        fxp.doneAmount.Add(Convert.ToDouble(dr2["DictTotal"]));
                    }
                    if (drs.Count() > 0)
                    {
                        dt = fxp.strDate.Last();
                        if (dt.CompareTo(mUpdateTime.ToString("yyyyMMdd")) > 0)  // 数据库的工程表的updateDate字段逻辑有误
                            mUpdateTime = DateTime.ParseExact(dt, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                        avgProgress += fxp.doneAmount.Last() / fxp.TotalAmount;
                    }
                    else
                    {
                        fxp.strDate.Add(DateTime.Now.ToString("yyyyMMdd"));
                        fxp.doneAmount.Add(Convert.ToDouble(dr["InitNum"]));  // 添加初始化量，保证每个分项工程都有数据
                        avgProgress += fxp.initAmount / fxp.TotalAmount;
                    }
                    mfx.Add(fxp);
                    n++;

                }

            }

            if (n > 0)
            {
                avgProgress /= n;
                mAvgProgress = avgProgress;
            }
            else
                mAvgProgress = 0;

        }

        public virtual List<CHotSpot> getNavPath()
        {
            if (mSpotList != null)
                return mSpotList;
            mSpotList = new List<CHotSpot>();

            if (!mIsValid)
            {
                return mSpotList;
            }

            double x, y, z, d;
            //for (int i = 0; i < 3; i++) {
            getSpecialPoint(0, out x, out y, out z, out d);
            mSpotList.Add(new CHotSpot(mStartDKCode, mMileage_Start, x, y, z, mMainMileageS, mdistanceToMainPath, "Project", this));
            getSpecialPoint(1, out x, out y, out z, out d);
            mSpotList.Add(new CHotSpot(mMidDKCode, mMileage_Mid, x, y, z, mMainMileage, mdistanceToMainPath, "Project", this));
            getSpecialPoint(2, out x, out y, out z, out d);
            mSpotList.Add(new CHotSpot(mEndDKCode, mMileage_End, x, y, z, mMainMileageE, mdistanceToMainPath, "Project", this));
            //}
            return mSpotList;


        }
        public virtual CHotSpot getHotSpot()
        {
            if (mIsValid)
            {
                if (mSpotList == null)
                    getNavPath();
                return mSpotList[mSpotList.Count / 2];
            }
            else
                return null;
        }

        /// <summary>
        /// 工点的获取line方法
        /// </summary>
        /// <param name="stepm"></param>
        /// <param name="cVerticesArray"></param>
        /// <returns></returns>
        public int getSubLine(out double[] cVerticesArray)
        {
            cVerticesArray = null;
            //double[] x;
            //double[] y;
            //double[] z;
            //double[] d;
            ////CRailwayLineList oo=null;
            //int vertexCount = mPath.getSubLine(out x, out y, out z, out d);
            if (mPath != null && mPath.mPointCount > 0)
            {
                cVerticesArray = new double[mPath.mPointCount * 3];

                for (int i = 0; i < mPath.mPointCount; i++)
                {
                    cVerticesArray[3 * i] = mPath.mx[i];
                    cVerticesArray[3 * i + 1] = mPath.my[i];
                    cVerticesArray[3 * i + 2] = mPath.mz[i];
                }
                return mPath.mPointCount;
            }
            return 0;
        }

        public int getMiddleLine(out double[] m, out double[] x, out double[] y, out double[] z, out double[] d)
        {
            //verList = null;
            x = y = z = d = m = null;
            if (mPath != null && mPath.mPointCount > 0)
            {
                mPath.getMiddleLine(out m, out x, out y, out z, out d);
                //mPath.getOffsetLine(90, 2.50, out y, out x, out z);

                return m.Length;
            }

            return 0;
        }

        public int getMiddleLine(out double[] x, out double[] y, out double[] z)
        {
            double[] m, d;
            return getMiddleLine(out m, out x, out y, out z, out d);
        }

        public int getToDoSubLine(out double[] m, out double[] x, out double[] y, out double[] z, out double[] d)
        {
            //cVerticesArray = null;
            x = y = z = d = m = null;
            if (mPath == null)
                return 0;
            //double[] m,x,y,z,dir;
            //double m;
            int vertexCount = 0;  // 0,180
            if (mDirection < 10)
            {
                mPath.getSubLineByDKCode(mEndDKCode, mMileage_End, -mPath.mLength * (1 - mAvgProgress), 10, out m, out x, out y, out z, out d);
            }
            else
                mPath.getSubLineByDKCode(mStartDKCode, mMileage_Start, mPath.mLength * (1 - mAvgProgress), 10, out m, out x, out y, out z, out d);
            vertexCount = x.Length;

            return vertexCount;
        }

        public int getDoneSubLine(out double[] m, out double[] x, out double[] y, out double[] z, out double[] d)
        {
            x = y = z = d = m = null;
            if (mPath == null)
                return 0;
            int vertexCount = 0;  // 0,180
            if (mDirection < 10)
            {
                mPath.getSubLineByDKCode(mStartDKCode, mMileage_Start, mPath.mLength * mAvgProgress, 10, out m, out x, out y, out z, out d);
            }
            else
                mPath.getSubLineByDKCode(mEndDKCode, mMileage_End, -mPath.mLength * mAvgProgress, 10, out m, out x, out y, out z, out d);
            vertexCount = x.Length;
            return vertexCount;
        }
        ///// <summary>
        ///// 工点的获取偏移方法
        ///// </summary>
        ///// <param name="stepm"></param>
        ///// <param name="angleOff"></param>
        ///// <param name="disOff"></param>
        ///// <param name="cVerticesArray"></param>
        ///// <returns></returns>
        //public int getOffsetLine(double stepm, double angleOff, double disOff, out double[] cVerticesArray)
        //{
        //    cVerticesArray = null;
        //    if (mPath == null)
        //        return 0;
        //    double[] latout;
        //    double[] lonout;
        //    double[] height;

        //    int pointNum = mPath.getOffsetLine(  angleOff, disOff, out  latout, out  lonout, out height);
        //    if (pointNum > 0)
        //    {
        //        cVerticesArray = new double[pointNum * 3];

        //        for (int i = 0; i < pointNum; i++)
        //        {
        //            cVerticesArray[3 * i] = lonout[i];
        //            cVerticesArray[3 * i + 1] = latout[i];
        //            cVerticesArray[3 * i + 2] = height[i] + 10;
        //        }
        //    }
        //    return pointNum;
        //}



        public override string ToString()
        {
            return String.Format("工点名：{0}，里程{1} ~ {2},长度{3}，完成进度{4}%", mProjectName, Mileage_Start_Ds, Mileage_End_Ds, DisplayLength, AvgProgress);
        }

    }

    public class CRailwayBridge : CRailwayProject
    {
        //public List<CRailwayPier> mPier = new List<CRailwayPier>();
        //public List<CRailwayBeam> mBeam = new List<CRailwayBeam>();
        public CRailwayBridge(CRailwayScene s, string SerialNo, int projID, int parentID, string ProfessionalName, string profCode, string ProjectName, string SegmentName,
            string MStartDes, string MEndDes, double Mileage_Start, double Mileage_End,
            DateTime dt, double AvgProgress, double dir, string labelFile, string photourl, DataTable dt1 = null, DataTable dt2 = null)
            : base(s, SerialNo, projID, parentID, ProfessionalName, profCode, ProjectName, SegmentName,
             MStartDes, MEndDes, Mileage_Start, Mileage_End,
             dt, AvgProgress, dir, labelFile, photourl, false, dt1, dt2)
        {
            int i;

            if (FXProgress == null) return;
            switch (FXProgress.Count)
            {
                case 0:
                    for (i = 0; i < 3; i++)
                        selectedFXid[i] = -1;
                    break;
                case 1:
                    selectedFXid[0] = 0;
                    selectedFXid[1] = -1;
                    selectedFXid[2] = -1;
                    break;
                case 2:
                    selectedFXid[0] = 0;
                    selectedFXid[1] = 1;
                    selectedFXid[2] = -1;
                    break;
                default:
                    for (i = 0; i < 3; i++)
                        selectedFXid[i] = i;
                    break;

            }
            // 桥梁预定义关键分项工程 桩基根数， 墩-台身 数量， 预制梁 孔数
            i = 0;
            foreach (CFXProj fx in FXProgress)
            {
                if (fx.fxID == 5) selectedFXid[0] = i;
                else if (fx.fxID == 376) selectedFXid[1] = i;
                else if (fx.fxID == 463) selectedFXid[2] = i;
                i++;
            }


        }
    }

    public class CRailwayRoad : CRailwayProject
    {
        public CRailwayRoad(CRailwayScene s, string SerialNo, int projID, int parentID, string ProfessionalName, string profCode, string ProjectName, string SegmentName,
            string MStartDes, string MEndDes, double Mileage_Start, double Mileage_End,
            DateTime dt, double AvgProgress, double dir, string labelFile, string photourl, DataTable dt1 = null, DataTable dt2 = null)
            : base(s, SerialNo, projID, parentID, ProfessionalName, profCode, ProjectName, SegmentName,
             MStartDes, MEndDes, Mileage_Start, Mileage_End,
             dt, AvgProgress, dir, labelFile, photourl, false, dt1, dt2)
        {
            int i;

            if (FXProgress == null) return;
            switch (FXProgress.Count)
            {
                case 0:
                    for (i = 0; i < 3; i++)
                        selectedFXid[i] = -1;
                    break;
                case 1:
                    selectedFXid[0] = 0;
                    selectedFXid[1] = -1;
                    selectedFXid[2] = -1;
                    break;
                case 2:
                    selectedFXid[0] = 0;
                    selectedFXid[1] = 1;
                    selectedFXid[2] = -1;
                    break;
                default:
                    for (i = 0; i < 3; i++)
                        selectedFXid[i] = i;
                    break;

            }

            // 路基预定义关键分项工程 管桩、挖方、填方
            i = 0;
            foreach (CFXProj fx in FXProgress)
            {
                if (fx.fxID == 583) selectedFXid[0] = i;
                else if (fx.fxID == 597) selectedFXid[1] = i;
                else if (fx.fxID == 599) selectedFXid[2] = i;
                i++;
            }

        }
    }

    public class CRailwayTunnel : CRailwayProject
    {
        public CRailwayTunnel(CRailwayScene s, string SerialNo, int projID, int parentID, string ProfessionalName, string profCode, string ProjectName, string SegmentName,
            string MStartDes, string MEndDes, double Mileage_Start, double Mileage_End,
            DateTime dt, double AvgProgress, double dir, string labelFile, string photourl, DataTable dt1 = null, DataTable dt2 = null)
            : base(s, SerialNo, projID, parentID, ProfessionalName, profCode, ProjectName, SegmentName,
             MStartDes, MEndDes, Mileage_Start, Mileage_End,
             dt, AvgProgress, dir, labelFile, photourl, false, dt1, dt2)
        {
            //DatabaseWrapper.PrintDataTable(mds.Tables[0]);
            //DatabaseWrapper.PrintDataTable(mds.Tables[1]);
            foreach (CFXProj fx in FXProgress)
            {
                //FIXME 隧道里程标记方法特殊，暂时认为在一个链中
                if (fx.fxID == 472 || fx.fxID == 475) // 正洞或斜井的开挖
                {
                    if (dir < 90)
                    {
                        mMileage_End = mMileage_Start + fx.TotalAmount;

                    }
                    else
                    {
                        mMileage_End = mMileage_Start - fx.TotalAmount;
                    }
                    mPath = new CSubPath(CRailwayLineList.gMileageConnection, mStartDKCode, mMileage_Start, mEndDKCode, mMileage_End, 10);
                    if (!mPath.hasPath)
                    {
                        mIsValid = false;
                        return;
                    }
                    mIsValid = true;
                    mScene.mMainPath.getPathMileageByDKCode(mStartDKCode, mMileage_Start, out mMainMileageS, out mdistanceToMainPath);
                    mScene.mMainPath.getPathMileageByDKCode(mEndDKCode, mMileage_End, out mMainMileageE, out mdistanceToMainPath);
                    mPath.getDKCodebyPathMileage(mPath.mLength * 0.5, out mMidDKCode, out mMileage_Mid);
                    mIsOnMainPath = mScene.mMainPath.getPathMileageByDKCode(mMidDKCode, mMileage_Mid, out mMainMileage, out mdistanceToMainPath);
                    mLength = mPath.mLength;

                    //mMileage_Mid = (mMileage_Start + mMileage_End) / 2;
                    //mLength = fx.TotalAmount;
                    //mIsValid = mScene.mMiddleLines.getGPSbyDKCode(mStartDKCode, mMileage_Mid, out mLongitude_Mid, out mLatitude_Mid, out mAltitude_Mid, out mHeading_Mid);
                    //mIsValid &= mScene.mMiddleLines.getGPSbyDKCode(mStartDKCode, mMileage_Start, out mLongitude_Start, out mLatitude_Start, out mAltitude_Start, out mHeading_Start);
                    //mIsValid &= mScene.mMiddleLines.getGPSbyDKCode(mStartDKCode, mMileage_End, out mLongitude_End, out mLatitude_End, out mAltitude_End, out mHeading_End);
                    //mMainMileage = mScene.mMainPath.getPathMileageByDKCode(mStartDKCode, mMileage_Mid);
                    //mMainMileageS = mScene.mMainPath.getPathMileageByDKCode(mStartDKCode, mMileage_Start);
                    //mMainMileageE = mScene.mMainPath.getPathMileageByDKCode(mStartDKCode, mMileage_End);

                    //if (mIsValid)
                    //{
                    //    if (mSpotList != null)
                    //        mSpotList.Clear();
                    //    else
                    //        mSpotList = new List<CHotSpot>();
                    //    mSpotList.Add(new CHotSpot(mStartDKCode, mMileage_Start, mLongitude_Start, mLatitude_Start, mAltitude_Start, mMainMileageS, "Tunnel", this));
                    //    mSpotList.Add(new CHotSpot(mStartDKCode, mMileage_Mid, mLongitude_Mid, mLatitude_Mid, mAltitude_Mid, mMainMileage, "Tunnel", this));
                    //    mSpotList.Add(new CHotSpot(mStartDKCode, mMileage_End, mLongitude_End, mLatitude_End, mAltitude_End, mMainMileageE, "Tunnel", this));
                    //}
                    break;
                }
            }

        }

    }


    public class CContBeam : CRailwayProject
    {
        public CContBeam(CRailwayScene s, string SerialNo, int projID, int parentID, string ProfessionalName, string profCode, string ProjectName, string SegmentName,
            string MStartDes, string MEndDes, double Mileage_Start, double Mileage_End,
            DateTime dt, double AvgProgress, double dir, string labelFile, string photourl, DataTable dt1 = null, DataTable dt2 = null)
            : base(s, SerialNo, projID, parentID, ProfessionalName, profCode, ProjectName, SegmentName,
             MStartDes, MEndDes, Mileage_Start, Mileage_End,
             dt, AvgProgress, dir, labelFile, photourl, true, dt1, dt2)
        {
            int i;
            switch (FXProgress.Count)
            {
                case 0:
                    for (i = 0; i < 3; i++)
                        selectedFXid[i] = -1;
                    break;
                case 1:
                    selectedFXid[0] = 0;
                    selectedFXid[1] = -1;
                    selectedFXid[2] = -1;
                    break;
                case 2:
                    selectedFXid[0] = 0;
                    selectedFXid[1] = 1;
                    selectedFXid[2] = -1;
                    break;
                default:
                    for (i = 0; i < 3; i++)
                        selectedFXid[i] = i;
                    break;

            }
            // 连续梁预定义关键分项工程 ，节段， 墩-台身 数量， 桩基根数
            i = 0;
            foreach (CFXProj fx in FXProgress)
            {
                if (fx.fxID == 470) selectedFXid[2] = i;
                else if (fx.fxID == 468) selectedFXid[0] = i;
                else if (fx.fxID == 69) selectedFXid[1] = i;
                i++;
            }
        }

        public void AdjustMileage()
        {
            if (mPierList.Count > 0)
            {
                mMileage_Start = mPierList[0].Mileage_Start;
                mStartDKCode = mPierList[0].DKCode_Start;
                mMileage_End = mPierList.Last().Mileage_Start;
                mEndDKCode = mPierList.Last().DKCode_Start;
                //if (mParentID == 392)
                //    Console.WriteLine("暂停调试");
                mPath = new CSubPath(CRailwayLineList.gMileageConnection, mStartDKCode, mMileage_Start, mEndDKCode, mMileage_End, 10);
                if (!mPath.hasPath)
                {
                    mIsValid = false;
                    return;
                }
                mIsValid = true;
                mIsOnMainPath = mScene.mMainPath.getPathMileageByDKCode(mStartDKCode, mMileage_Start, out mMainMileageS, out mdistanceToMainPath);

                mScene.mMainPath.getPathMileageByDKCode(mEndDKCode, mMileage_End, out mMainMileageE, out mdistanceToMainPath);
                mPath.getDKCodebyPathMileage(mPath.mLength * 0.5, out mMidDKCode, out mMileage_Mid);
                mScene.mMainPath.getPathMileageByDKCode(mMidDKCode, mMileage_Mid, out mMainMileage, out mdistanceToMainPath);

                //mLength = mPath.mLength;

                //mLongitude_Start = mPierList[0].mLongitude_Mid;
                //mLatitude_Start = mPierList[0].mLatitude_Mid;
                //mAltitude_Start = mPierList[0].mAltitude_Mid;
                //mHeading_Start = mPierList[0].mHeading_Mid;

                Mileage_Start_Discription = CRailwayLineList.CombiDKCode(mStartDKCode, mMileage_Start);
                Mileage_End_Discription = CRailwayLineList.CombiDKCode(mEndDKCode, mMileage_End);
                mLength = Math.Abs(mMileage_End - mMileage_Start);

                //if (mIsValid)
                //{
                //    if (mSpotList != null)
                //        mSpotList.Clear();
                //    else
                //        mSpotList = new List<CHotSpot>();
                //    mSpotList.Add(new CHotSpot(mStartDKCode, mMileage_Start, mLongitude_Start, mLatitude_Start, mAltitude_Start, mMainMileageS, "ContBeam", this));
                //    mSpotList.Add(new CHotSpot(mMidDKCode, mMileage_Mid, mLongitude_Mid, mLatitude_Mid, mAltitude_Mid, mMainMileage, "ContBeam", this));
                //    mSpotList.Add(new CHotSpot(mEndDKCode, mMileage_End, mLongitude_End, mLatitude_End, mAltitude_End, mMainMileageE, "ContBeam", this));
                //}
            }
            else
            {
                mIsValid = false;
                LogHelper.WriteLog("错误：连续梁 " + ProjectName + " 无桥墩信息");
            }
        }


    }


}


