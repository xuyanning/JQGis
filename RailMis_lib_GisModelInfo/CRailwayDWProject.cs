using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel; 

namespace ModelInfo
{
    // 单位工程, 目前要求起止里程都在一个DKCode中
    public class CRailwayDWProj
    {
        public CRailwayProject mParentProj;
        public int mParentID;

        [CategoryAttribute("桥墩信息"), DisplayName("01所属桥梁")] 
        public string BridgeName
        {
            get { return mParentProj.ProjectName;  }
        }
        public int mDWID;
        
        private string mDWName;

        [CategoryAttribute("桥墩信息"), DisplayName("02桥墩编号")] 
        public string DWName
        {
            get { return mDWName; }
            set { mDWName = value; }
        }

        private double mMileage_Start;
        [CategoryAttribute("桥墩信息"), DisplayName("05里程数")]
        public double Mileage_Start
        {
            get { return mMileage_Start; }
            set { mMileage_Start = value; }
        }
        private string mDKCode_Start;
        
        [CategoryAttribute("桥墩信息"), DisplayName("04里程编码")] 
        public string DKCode_Start
        {
            get { return mDKCode_Start; }
            set { mDKCode_Start = value; }
        }
        //public double mMileage_End;  // 起止里程，中点里程
        //public string mDKCode_End;
        public double mLength;  // 工点长度，如果是梁，有长度

        public double mLongitude_Mid;
        public double mLatitude_Mid;
        public double mAltitude_Mid;  // 中点经纬度，高度
        public double mHeading_Mid;

        public bool mIsValid;

        private DateTime mFinishTime; // 更新时间
        [CategoryAttribute("桥墩信息"), DisplayName("06更新时间")]
        public DateTime FinishTime
        {
            get { return mFinishTime; }
            set { mFinishTime = value; }
        }
        //private double mAvgProgress; // 进度     
        //[CategoryAttribute("桥墩信息"), DisplayName("07整体进度")]
        //public double AvgProgress
        //{
        //    get { return mAvgProgress; }
        //    set { mAvgProgress = value; }
        //}
        public bool mIsDone = false;

        public string mSerialNo;  // 序列号编码

        public double mMainMileage;

        public bool mIsOnMainPath;
        public double mdistanceToMainPath;

        //public int mType1;
        //public int mType2;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pp"></param>
        /// <param name="SerialNo"></param>
        /// <param name="name"></param>
        /// <param name="startM"></param>
        /// <param name="length"></param>
        /// <param name="dt"></param>
        /// <param name="AvgProgress"></param>
        /// <param name="dkcode"></param>
        public CRailwayDWProj(CRailwayProject pp,int pid, string SerialNo,int dwid, string name, double startM, double length, DateTime dt, bool isFinish, string dkcode )
        {
            mParentProj = pp;
            mParentID = pid;
            mDWID = dwid;
            mDWName = name;           
            mSerialNo = SerialNo;

            mMileage_Start = startM;
            mDKCode_Start = dkcode;

            mLength = length;

            mFinishTime = dt;
            //mAvgProgress = AvgProgress;
            mIsDone = isFinish;

            mIsValid = CRailwayLineList.getGPSbyDKCode(dkcode, mMileage_Start,
                out mLongitude_Mid, out mLatitude_Mid, out mAltitude_Mid, out mHeading_Mid);
            mIsOnMainPath = mParentProj.mScene.mMainPath.getPathMileageByDKCode(dkcode, mMileage_Start,out mMainMileage,out mdistanceToMainPath );

        }
      

    }

    //public class CRailwayBeam: CRailwayDWProj
    //{
    //    public List<String> mBeamSNOs = new List<String>();  // 用于获取桩的详细信息
    //    public List<bool> mBeamDone = new List<bool>(); // 桩是否已经完成
    //    public CRailwayBeam(CRailwayProject pp, int pid, string SerialNo, int dwid, string name, string dwtype, double startM, double endM, DateTime dt, bool isFinish, string dkcode)
    //        : base(pp, pid, SerialNo, dwid, name, dwtype, startM, endM, dt, isFinish, dkcode = "DK")
    //    {
    //        //string sqlstr = @"select * from (SELECT   AutoID, Project_B_DW_ID, PropertyID, Value " +
    //        //                       "FROM ProjectPropertyConfigInfo)a, " +
    //        //                       "(SELECT AutoID, Property, Unit, UpdateTime, CrtUSrID " +
    //        //                       "FROM      ProjectPropertyInfo)b where a.PropertyID = b.AutoID and a.Project_B_DW_ID=" + dwid;
    //        //System.Data.DataTable dtt = CServerWrapper.execSqlQuery(sqlstr);
    //        //DatabaseWrapper.PrintDataTable(dtt);

    //        string[] ss = name.Split('#');
    //        try
    //        {
    //            DWName = ss[0];

    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(name + "：桥墩名称解析错误:");
    //        }
    //        //getPierType(dwtype);


    //    }
    //}

    public class CRailwayPier : CRailwayDWProj
    {
        //SELECT Name, ParentID, ProjectID, IsFinish, SerialNo
        //public int mStubNum; // 桩的个数
        public List<String> mStubSNOs = new List<String>();  // 用于获取桩的详细信息
        public List<bool> mStubDone = new List<bool>(); // 桩是否已经完成
        public List<String> mBeamSNOs = new List<string>(); // 梁序列号
        public List<bool> mBeamDone = new List<bool>(); // 梁是否完工
        public List<String> mBeamName = new List<string>(); // 梁的名字
        //public List<double> mBeamLength = new List<double>();

        public CRailwayPier(CRailwayProject pp, int pid, string SerialNo, int dwid, string name,  double startM, double endM, DateTime dt, bool isFinish, string dkcode )
            : base(pp, pid, SerialNo, dwid, name,  startM, endM, dt, isFinish, dkcode )
        {
            //string sqlstr = @"select * from (SELECT   AutoID, Project_B_DW_ID, PropertyID, Value " +
            //                       "FROM ProjectPropertyConfigInfo)a, " +
            //                       "(SELECT AutoID, Property, Unit, UpdateTime, CrtUSrID " +
            //                       "FROM      ProjectPropertyInfo)b where a.PropertyID = b.AutoID and a.Project_B_DW_ID=" + dwid;
            //System.Data.DataTable dtt = CServerWrapper.execSqlQuery(sqlstr);
            //DatabaseWrapper.PrintDataTable(dtt);
            
            string[] ss = name.Split('#');
            try
            {
                DWName = ss[0];

            }
            catch (Exception e)
            {
                Console.WriteLine(name + "：桥墩名称解析错误:");
            }
            //getPierType(dwtype);


        }
        //private void getPierType(string stype){
        //    string[] ss = stype.Split('|');
        //    if (ss.Length == 2) {
        //        try
        //        {
        //            mType1 = Convert.ToInt32(ss[0]);
        //            mType2 = Convert.ToInt32(ss[1]);
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine( this.DWName+"：桥墩参数解析错误:" + stype);
        //        }
        //    }

        //}
    }

    public class CRailwayBeam : CRailwayDWProj
    {
        public CRailwayBeam(CRailwayProject pp,int pid, string SerialNo, int dwid, string name, double startM, double endM, DateTime dt, bool isFinish, string dkcode)
            : base(pp,pid, SerialNo,dwid, name,  startM, endM, dt, isFinish, dkcode = "DK")
        {

        }
    }
}
