using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel; 

namespace ModelInfo
{
    public class CRailwayFirm 
    {
        public CRailwayScene mScene;
        public CRailwayLine mLine= null;
        public bool misInside = false;
        CHotSpot mHotSpot;

        //public String mFirmType { get; set; } 
        
        private string mFirmType; //单位类型  :
        [CategoryAttribute("工点信息"), DisplayName("03工点类型")] 
        public string FirmType
        {
            get { return mFirmType; }
            set { mFirmType = value; }
        }

        public int mFirmID; // 单位编号
        private string mFirmName;  // 单位名称

        [CategoryAttribute("工点信息"), DisplayName("01单位名称")] 
        public string FirmName
        {
            get { return mFirmName; }
            set { mFirmName = value; }
        }

        private int mNumStaff; // 实名制人数
        [CategoryAttribute("工点信息"),  DisplayName("02实名制人数")] 
        public int NumStaff
        {
            get { return mNumStaff; }
            set { mNumStaff = value; }
        }

        //public string mSegmentName;  // 所属标段

        //public DateTime mUpdateTime; // 更新时间
        //public double mAvgProgress; // 进度      

        // 空间位置信息
        private double mLongitude_Mid;
        [CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("中心点经度"), Browsable(false)]
        public double CenterLongitude
        {
            get { return mLongitude_Mid; }
            set { mLongitude_Mid = value; }
        }
        private double mLatitude_Mid;
        [CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("中心点纬度"), Browsable(false)]
        public double CenterLatitude
        {
            get { return mLatitude_Mid; }
            set { mLatitude_Mid = value; }
        }
        //public double mAltitude_Mid;  // 中点经纬度，高度

        //public string mSNo;

        public string mLabelImage;  //文件名
                                    //private string mDKCode;
                                    //[CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("DK编码"), Browsable(false)]
                                    //public string DKCode
                                    //{
                                    //    get { return mDKCode; }
                                    //    set { mDKCode = value; }
                                    //}


        private double mMileage_Mid;
        [CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("中心里程"), Browsable(false)]
        public double CenterMileage
        {
            get { return mMileage_Mid; }
            set { mMileage_Mid = value; }
        }

        private double mMainMileage;
        [CategoryAttribute("工点信息"), ReadOnlyAttribute(true), DisplayName("全局里程"), Browsable(false)]
        public double GlobalMileage
        {
            get { return mMainMileage; }
            set { mMainMileage = value; }
        }

        public double mDis;
        [CategoryAttribute("工点信息"), DisplayName("04工点基本信息")] 
        public string ShowMessage { get; set; }

        public string mPresentation;

        public CRailwayFirm(CRailwayScene s, int num, int fid, string fname,  double x, double y,string firmType,string presentation, string fileName)
        {
            mScene = s;
            mFirmType = firmType;
            mFirmName = fname;

            mNumStaff = num;
            mFirmID = fid;
            //mSNo = sNo;
            //mUpdateTime = dt;
            mPresentation = presentation;
            mLongitude_Mid = x;
            mLatitude_Mid = y;

            //if (fname.StartsWith("11标"))
            //    Console.WriteLine(fname + x + "\t" + y);
            mLine = CRailwayLineList.getMileagebyGPS(x, y, out mMileage_Mid, out mDis, out misInside);
            //mDKCode = mLine.mDKCode;
            double dis;
            mScene.mMainPath.getPathMileagebyGPS(x,y,out mMainMileage,out dis);

            mLabelImage = fileName;
            if (mDis < 8000)
                mHotSpot = new CHotSpot(mLine.mDKCode, mMileage_Mid, mLongitude_Mid, mLatitude_Mid, 100, mMainMileage,mDis, "Firm", this);
            else
                mHotSpot = null;


        }

        public CHotSpot getCenterHotSpot()
        {
            return mHotSpot;
        }

        public override string ToString()
        {
            string res = this.mFirmName + "\n实名制注册人数" + this.mNumStaff;
            return res;
            //return base.ToString();
        }


    }
}
