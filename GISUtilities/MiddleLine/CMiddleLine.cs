using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.Drawing;
//using ModelInfo.Helper;
//using lib_GIS.Service;
//using Microsoft.Office.Interop.Excel;

namespace GISUtilities
{


    // 规则，规范化里程从0开始，查找时利用里程的变换公式
    public class CRailwayLine
    {
        // 10米中线
        public int mPointNum;

        // 如果是双线，保存左线，如果是单线，保存中线
        public double[] meter;
        public double[] longitude; 
        public double[] latitude;
        public double[] altitude;
        public double[] heading;  // 控制方向,虽然可以通过 longitude以及latitude计算，但需要转化为utm,计算耗时
        //public double[] pitching;
        //public double[] utmX;  // 保留
        //public double[] utmY;  // 保留

        // 如果是双线，保存中线，即桥墩以及接触网放置位置；如果是单线，保存线杆
        public double[] mOffsetX;
        public double[] mOffsetY;

        public double mOffset;

        //public double[] utmXMars;
        //public double[] utmYMars;
        //public double[] longitudeMars;
        //public double[] latitudeMars;
        public double mStart;
        public double mEnd;

        //public List<ChainNode> mNodeList ;
        //public List<RectangleF> mBBoxList = new List<RectangleF>(); // 保留

        public double mLength;
        public bool mIsAuxiliary; // 保留
        public bool mIsReverse;  //里程是否由小到大
        public bool mIsRight; // 是否右线
        public bool mIsDouble; // 是否双线
        public string mDKCode; // "DK" "DIK"
        public string mOtherDKCode; //其他DK码，有的路线有两个dkcode
        public int mLineType; // 0 - 双线 ，1-单线左线， 2-单线右线
        public int mIndex;  //所在链的下标
        public double mStepm ; // 采样里程

        public double startLongitude
        {
            get { return longitude[0]; }
        }
        public double endLongitude
        {
            get { return longitude.Last(); }
        }

        public double startLatitude
        {
            get { return latitude[0]; }
        }

        public double endLatitude
        {
            get { return latitude.Last(); }
        }

        public override string ToString()
        {
            string str;
            str = mIndex + "\t" + mDKCode + mStart + " to " + mEnd;
            return str;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="dkcode"></param>
        /// <param name="dkcode2"></param>
        /// <param name="fromM"></param>
        /// <param name="toM"></param>
        /// <param name="lineType"></param>
        /// <param name="num"></param>
        /// <param name="m">传入的数据必须从起始里程到终止里程</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="stepm"></param>
        /// <param name="otherDKCode"></param>
        public CRailwayLine(int idx, string dkcode,string dkcode2, double fromM, double toM,int lineType, int num, double[] m, double[] x, double[] y, double[] z, double stepm = 10, string otherDKCode = "")
        {
            mIndex = idx;
            mStart = fromM;
            mEnd = toM;
            if (mStart < mEnd)
                mIsReverse = false;
            else
                mIsReverse = true;
            mDKCode = dkcode;
            mOtherDKCode = dkcode2;

            //mIsAuxiliary = isA;

            mIsRight = (lineType == 2);
            mIsDouble = (lineType == 0);

            if (mIsDouble)
            {
                mOffset = 2.5; // 根据济青铁路轨道间距5米测算
            }
            else
            {
                mOffset = 5.4; // 线杆偏移位置
            }

            mStepm = Math.Max(1, Math.Abs(stepm)); //
            //mFromID = fromID;
            //mToID = toID;

            // 利用stepM重采样,0513,FIXED，采样非10米时，mPointNum计算错误
            mLength = m[num - 1] - m[0];
            if (mLength < 2 * mStepm)
                mPointNum = 2;
            else
                mPointNum = (int)((mLength - 2 * stepm) / stepm) + 3;  //最后一段一般大于10米，避免最后一段过短，
            meter = new double[mPointNum];

            meter[0] = m[0];
            meter[mPointNum - 1] = m[num - 1];
            //for (int j = 0; j < num - 1; j++)
            //    if (m[j] >= m[j+ 1])
            //        Console.WriteLine("mileage error " + m[j] + "\t" + m[j+1]); 

            for (int i = 1; i < mPointNum - 1; i++)
                meter[i] = meter[i - 1] + mStepm;

            longitude = CubicSpline.Compute(num, m, x, meter);
            latitude = CubicSpline.Compute(num, m, y, meter);
            altitude = CubicSpline.Compute(num, m, z, meter);

            //for (int i = 0; i < mPointNum - 1; i += (int)(1000/mStepm))
            //    mBBoxList.Add(new RectangleF((float)(longitude[i] - 0.01),(float)(latitude[i] -0.01),0.02f,0.02f));
            //if (mPointNum % 100 > 20)
            //    mBBoxList.Add(new RectangleF((float)(longitude[mPointNum -1] - 0.01), (float)(latitude[mPointNum -1] - 0.01), 0.02f, 0.02f));
            //          RectangleF rec = mBBoxList[0]; 
            //CoordinateConverter.LatLonToUTMXYList(mPointNum, latitude, longitude, out utmX, out utmY);
            //mOtherDKCode = otherDKCode;
            CoordinateConverter.LatLonToOffsetYawList(latitude, longitude, 90, mOffset, out mOffsetX, out mOffsetY, out heading);
            //CoordinateConverter.LatLonToUTMXYList(mPointNum, latitudeMars, longitudeMars, out utmXMars, out utmYMars);


        }

        public void createExcelData(double[] m, double[] z, StreamWriter sw)
        {
            //for (int i = 0; i < m.Length -1; i++)
            //{
            //    if (m[i + 1] - m[i] < 1)
            //        Console.WriteLine(i+" error");
            //}
            double[] x = CubicSpline.Compute(meter.Length,meter,longitude,m);
            double[] y = CubicSpline.Compute(meter.Length, meter, latitude, m);
            if (mIsReverse)
                for (int i = 0; i < m.Length; i++)
                    sw.WriteLine((mStart - m[i])+ "\t" + x[i] + "\t" + y[i] + "\t" + z[i] + "\t" + mIndex);
            else
                for (int i = 0; i < m.Length; i++)
                    sw.WriteLine((mStart + m[i]) + "\t" + x[i] + "\t" + y[i] + "\t" + z[i] + "\t" + mIndex);
            sw.Close();
        }
        /// <summary>
        /// 旧版，没有dkcode2
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="dkcode"></param>
        /// <param name="fromM"></param>
        /// <param name="toM"></param>
        /// <param name="isA"></param>
        /// <param name="isR"></param>
        /// <param name="isD"></param>
        /// <param name="num"></param>
        /// <param name="m"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="stepm"></param>
        /// <param name="otherDKCode"></param>
        public CRailwayLine(int idx, string dkcode, double fromM, double toM, bool isA,bool isR,bool isD, int num, double[] m, double[] x, double[] y, double[] z, double stepm = 10,string otherDKCode="")
        {
            mIndex = idx;
            mStart = fromM;
            mEnd = toM;
            if (mStart < mEnd)
                mIsReverse = false;
            else
                mIsReverse = true;
            mDKCode = dkcode;

            mIsAuxiliary = isA;

            mIsRight = isR;
            mIsDouble = isD;
            if (mIsDouble)
            {
                mOffset = 2.5; // 根据济青铁路轨道间距5米测算
            }
            else
            {
                mOffset = 5.4; // 线杆偏移位置
            }

            mStepm = Math.Max(1,Math.Abs (stepm)); //
            //mFromID = fromID;
            //mToID = toID;

            // 利用stepM重采样,0513,FIXED，采样非10米时，mPointNum计算错误
            mLength = m[num - 1] - m[0];
            if (mLength < 2 * mStepm)
                mPointNum = 2;
            else 
                mPointNum = (int)((mLength - 2 * stepm) / stepm) + 3;  //最后一段一般大于10米，避免最后一段过短，
            meter = new double[mPointNum];

            meter[0] = m[0];
            meter[mPointNum - 1] = m[num - 1];
            //for (int j = 0; j < num - 1; j++)
            //    if (m[j] >= m[j+ 1])
            //        Console.WriteLine("mileage error " + m[j] + "\t" + m[j+1]); 
               
            for (int i = 1; i < mPointNum - 1; i++)
                    meter[i] = meter[i - 1] + mStepm;

            longitude = CubicSpline.Compute(num, m, x, meter);
            latitude = CubicSpline.Compute(num, m, y, meter);
            altitude = CubicSpline.Compute(num, m, z, meter);

            //for (int i = 0; i < mPointNum - 1; i += (int)(1000/mStepm))
            //    mBBoxList.Add(new RectangleF((float)(longitude[i] - 0.01),(float)(latitude[i] -0.01),0.02f,0.02f));
            //if (mPointNum % 100 > 20)
            //    mBBoxList.Add(new RectangleF((float)(longitude[mPointNum -1] - 0.01), (float)(latitude[mPointNum -1] - 0.01), 0.02f, 0.02f));
            //          RectangleF rec = mBBoxList[0]; 
            //CoordinateConverter.LatLonToUTMXYList(mPointNum, latitude, longitude, out utmX, out utmY);
            mOtherDKCode = otherDKCode;
            CoordinateConverter.LatLonToOffsetYawList(latitude, longitude,90,mOffset, out mOffsetX, out mOffsetY, out heading);
            //CoordinateConverter.LatLonToUTMXYList(mPointNum, latitudeMars, longitudeMars, out utmXMars, out utmYMars);


            //CoordinateConverter.LatLonOffest(longitude,latitude,heading,90,mOffset,out mOffsetX,out mOffsetY );
#if DEBUG

            validateHeading();
#endif
        }

        private void validateHeading()
        {
            double deltaHeading;
            for (int i = 0; i < mPointNum - 1; i++)
            {
                deltaHeading = Math.Abs(heading[i] - heading[i + 1]);
                if (deltaHeading > 20 )
                {
                    Console.WriteLine("里程数据异常：" + mDKCode + meter[i] + "\t heading " + deltaHeading );
                }

            }
        }
        /// <summary>
        /// 根据距离起点的局部里程，求DK里程
        /// </summary>
        /// <param name="lm"></param>
        /// <returns></returns>
        private double getDKMileagebyLocalMileage(double lm)
        {
            if (mIsReverse)
                lm = mStart - lm;
            else
                lm = mStart + lm;
            return lm;
        }

        public bool isInside(string dkcode, double meter)
        {
            //0513 xyn dkcode
            if (dkcode.Equals(mDKCode, StringComparison.CurrentCultureIgnoreCase) ||
                dkcode.Equals(mOtherDKCode, StringComparison.CurrentCultureIgnoreCase))
            {
                if ((meter - mStart) * (meter - mEnd) > 0.01)
                    return false;
                else
                    return true;

            }
            return false;
        }
        /// <summary>
        /// 根据输入的DK代码以及里程，判断是否在该线路内，返回距离起点的里程
        /// </summary>
        /// <param name="dkcode"></param>
        /// <param name="meter"></param>
        /// <param name="dis"></param>
        /// <returns></returns>
        public bool getLocalMileageByDKMileage(string dkcode, double meter, out double dis)
        {
            dis = 0;
            if (mIsReverse)
            {
                dis = mStart - meter;
            }
            else
            {
                dis = meter - mStart;
            }
            //0513 xyn dkcode
            if (dkcode.Equals(mDKCode, StringComparison.CurrentCultureIgnoreCase) || 
                dkcode.Equals(mOtherDKCode, StringComparison.CurrentCultureIgnoreCase))
            {
                if ((meter - mStart) * (meter - mEnd) > 0.01)
                    return false;
                else
                    return true;

            }
            return false;
        }



        //获取千米标与百米标，
        public int getKML(out double[] x, out double[] y, out double[] z, out double[] dir, out string[] meterFlag)
        {
            //double startm, deltam;
            int count = 0;
            int s, e;
            double[] m;
            x = y = z = dir = null;
            meterFlag = null;
            int stepm = 100;
            //if (isKML) stepm = 1000;

            if (mIsReverse)
            {
                s = (int)(mStart / stepm) ;
                e = (int)(mEnd / stepm) + 1;
                stepm = -100;
            }
            else
            {
                s = (int)(mStart / stepm) + 1;
                e = (int)(mEnd / stepm); 
            }
            
            count = Math.Abs(e - s) + 1;
            m = new double[count];
            x = new double[count];
            y = new double[count];
            z = new double[count];
            dir = new double[count];
            meterFlag = new string[count];

            string str = "";
            double temp;
            for (int i = s; i <= e; i++)
            {
                m[i] = i * 100;
                getGPSbyDKMileage(m[i],out x[i],out y[i],out z[i], out dir[i],out temp);
                
                if (i % 10 == 0)
                {
                    str = mDKCode + i / 10;
                }
                else
                {
                    str += i % 10;
                }
                meterFlag[i] = str;
                //startm += deltam;

            }
            return count;
        }

        // 左线向右为正方向
        //
        // 获取线路的一部分，输入参数，起始终止里程与采样间隔， 输出参数，线路上的经纬高度坐标数组，最后一段大于等于stepm，避免出现过小的数值      
        // stepm必须大于等于1米,includeLast 是否包含最后一个点
        //
        public int getSubLineByDKMileage(double startm, double endm, double stepm, out double[] mileage, out double[] x, out double[] y, out double[] z, out double[] dir, bool includeLastPoint = false)
        {
            getLocalMileageByDKMileage(this.mDKCode, startm, out startm);
            getLocalMileageByDKMileage(this.mDKCode, endm, out endm);
            return CSubLine.getSubLineByMileage(startm, endm, stepm, meter, longitude, latitude, altitude, heading, out mileage, out x, out y, out z, out dir, includeLastPoint);
        }

        /// <summary>
        /// 获取第二条线路的一部分，复线-中线。单线-线杆
        /// </summary>
        /// <param name="startm"></param>
        /// <param name="endm"></param>
        /// <param name="stepm"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="includeLastPoint"></param>
        /// <returns></returns>
        public int getSubLine2ByDKMileage(double startm, double endm, double stepm, out double[] x, out double[] y,  bool includeLastPoint = false)
        {
            double[] z, d,m;
            getLocalMileageByDKMileage(this.mDKCode, startm, out startm);
            getLocalMileageByDKMileage(this.mDKCode, endm, out endm);
            return CSubLine.getSubLineByMileage(startm, endm, stepm, meter, mOffsetX, mOffsetY, altitude, heading, out m, out x, out y, out z, out d, includeLastPoint);
        }

        public int getOffsetLineByDKMileage(double startm, double endm, double stepm,double offsetm, out double[] mileage, out double[] x, out double[] y, out double[] z, out double[] dir, bool includeLastPoint = false) {
            getLocalMileageByDKMileage(mDKCode, startm, out startm);
            getLocalMileageByDKMileage(mDKCode, endm, out endm);
            return CSubLine.getOffsetLineByMileage(startm, endm, stepm, meter, longitude, latitude, altitude, heading, mOffset, mOffsetX, mOffsetY, offsetm,
                out mileage, out x, out y, out z, out dir, includeLastPoint);
        }
        public int getMiddleLineByDKMileage(double startm, double endm, double stepm, out double[] mileage, out double[] x, out double[] y, out double[] z, out double[] dir, bool includeLastPoint = false)
        {
            getLocalMileageByDKMileage(this.mDKCode, startm, out startm);
            getLocalMileageByDKMileage(this.mDKCode, endm, out endm);

            if (mIsDouble)
            {
                return CSubLine.getSubLineByMileage(startm, endm, stepm, meter, mOffsetX, mOffsetY, altitude, heading, out mileage, out x, out y, out z, out dir, includeLastPoint);

            }
            else
            {
                return CSubLine.getSubLineByMileage(startm, endm, stepm, meter, longitude, latitude, altitude, heading, out mileage, out x, out y, out z, out dir, includeLastPoint);
            }
            //z = altitude;
            //dir = heading;

        }

        // 给定里程，计算经纬度朝向坐标
        public bool getGPSbyDKMileage(double m, out double x, out double y, out double z, out double dir,out double pitch)
        {
            double dis = 0;
            bool isFind = getLocalMileageByDKMileage(mDKCode,m,out dis);
            CSubLine.getGPSbyMileage(dis, meter, longitude, latitude, altitude, heading, out x, out y, out z, out dir,out pitch);
            return isFind;
        }

        public bool getPierPosbyDKMileage(double m, out double x, out double y, out double z, out double dir, out double pitch)
        {
            double dis = 0;
            x = y = z = dir = pitch =0;
            bool isFind = getLocalMileageByDKMileage(mDKCode, m, out dis);
            if (isFind)
            {
                if (mIsDouble)
                    CSubLine.getGPSbyMileage(dis, meter, mOffsetX, mOffsetY, altitude, heading, out x, out y, out z, out dir, out pitch);
                else
                    CSubLine.getGPSbyMileage(dis, meter, longitude, latitude, altitude, heading, out x, out y, out z, out dir, out pitch);

            }
            return isFind;
        }

        public void getPolePos( out double[] x, out double[] y, out double[] z, out double[] dir)
        {
            //double dis = 0;
            double[] m;
            m = x = y = z = dir = null;
            if (meter == null || meter.Length == 0)
                return ;
            //x = new double[m.Length];
            //y = new double[m.Length];
            //z = new double[m.Length];
            if (mIsDouble)
                CSubLine.getOffsetLineByMileage(0, Math.Abs(mEnd - mStart), 50, meter, longitude, latitude, altitude, heading, mOffset, mOffsetX, mOffsetY, mOffset * 2,
                out m, out x, out y, out z, out dir, false);
        }

        /// <summary>
        /// 如果复线，返回两条接触网线，如果单线，返回一条
        /// </summary>
        /// <param name="m"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="dir"></param>
        /// <param name="m2"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="z2"></param>
        /// <param name="dir2"></param>
        /// <returns></returns>
        public bool getContactLineByMileage(out double[] m, out double[] x, out double[] y, out double[] z, out double[] dir,
            out double[] m2, out double[] x2, out double[] y2, out double[] z2, out double[] dir2)
        {
            m = x = y = z = dir = null;
            m2 = x2 = y2 = z2 = dir2 = null;
            CSubLine.getSubLineByMileage(0, Math.Abs(mEnd - mStart), 50, meter, longitude, latitude, altitude, heading, out m, out x, out y, out z, out dir);

            if (mIsDouble)
            {                
                CSubLine.getOffsetLineByMileage(0, Math.Abs(mEnd - mStart), 50, meter, longitude, latitude, altitude, heading, mOffset, mOffsetX, mOffsetY, mOffset * 2,
                    out m2, out x2, out y2, out z2, out dir2, false);
            }

            return mIsDouble;

        }
        /// <summary>
        /// 给定经纬度，输出里程与距离
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mileage"></param>
        /// <param name="distance"></param>
        public void getDKMileagebyGPS(double x, double y, out double mileage, out double distance)
        {
            CSubLine.getMileagebyGPS(x, y, meter, longitude, latitude, out mileage, out distance);
            mileage = getDKMileagebyLocalMileage(mileage);
           
        }

        //public void 


            //CoordinateConverter.LatLonToUTMXY(y, x, out ux, out uy);
            //distance = 0;
            //mileage = 0;
            //try
            //{
            //    // 在起点之前
            //    if ((utmX[mPointNum - 1] - utmX[0]) * (ux - utmX[0]) + (utmY[mPointNum - 1] - utmY[0]) * (uy - utmY[0]) < 0)
            //    {
            //        mileage = mStart;
            //        distance = Math.Sqrt(Math.Pow(ux - utmX[0], 2) + Math.Pow(uy - utmY[0], 2));
            //        return false;
            //    }
            //    // 在终点之后
            //    if ((utmX[0] - utmX[mPointNum - 1]) * (ux - utmX[mPointNum - 1]) + (utmY[0] - utmY[mPointNum - 1]) * (uy - utmY[mPointNum - 1]) < 0)
            //    {
            //        mileage = mEnd;
            //        distance = Math.Sqrt(Math.Pow(ux - utmX[mPointNum - 1], 2) + Math.Pow(uy - utmY[mPointNum - 1], 2));
            //        return false;
            //    }

            //    mileage = 0;

            //    int count = mPointNum;
            //    double mindist = (utmX[0] - ux) * (utmX[0] - ux) +
            //        (utmY[0] - uy) * (utmY[0] - uy);
            //    double dist = 10;
            //    int step = (int)Math.Sqrt(count) + 1;
            //    //int i;
            //    int index = 0;
            //    for (int i = step; i < count; i += step)
            //    {
            //        dist = (utmX[i] - ux) * (utmX[i] - ux) + (utmY[i] - uy) * (utmY[i] - uy);
            //        if (dist < mindist)
            //        {
            //            mindist = dist;
            //            index = i;
            //        }
            //        errorNum = i;

            //    }

            //    int index2 = index;
            //    int j = index - step;
            //    if (j < 0) j = 0;
            //    for (; j < index + step && j < count; j++)
            //    {
            //        dist = (utmX[j] - ux) * (utmX[j] - ux) + (utmY[j] - uy) * (utmY[j] - uy);
            //        if (dist < mindist)
            //        {
            //            mindist = dist;
            //            index2 = j;
            //        }
            //        errorNum = -j;
            //    }
            //    if (index2 > 0 && index2 < count - 1)
            //    {
            //        double d1, d2;
            //        d1 = Math.Sqrt(Math.Pow(utmX[index2 - 1] - ux, 2) + Math.Pow(utmY[index2 - 1] - uy, 2));
            //        d2 = Math.Sqrt(Math.Pow(utmX[index2 + 1] - ux, 2) + Math.Pow(utmY[index2 + 1] - uy, 2));
            //        mileage = meter[index2 - 1] + (meter[index2 + 1] - meter[index2 - 1]) * d1 / (d1 + d2);
            //    }
            //    else if (index2 == 0)
            //    {
            //        double d1, d2;
            //        d1 = Math.Sqrt(Math.Pow(utmX[0] - ux, 2) + Math.Pow(utmY[0] - uy, 2));
            //        d2 = Math.Sqrt(Math.Pow(utmX[1] - ux, 2) + Math.Pow(utmY[1] - uy, 2));
            //        mileage = (meter[1] - meter[0]) * d1 / (d1 + d2);
            //    }
            //    else
            //    {
            //        double d1, d2;
            //        d1 = Math.Sqrt(Math.Pow(utmX[count - 2] - ux, 2) + Math.Pow(utmY[count - 2] - uy, 2));
            //        d2 = Math.Sqrt(Math.Pow(utmX[count - 1] - ux, 2) + Math.Pow(utmY[count - 1] - uy, 2));
            //        mileage = meter[count - 2] + (meter[count - 1] - meter[count - 2]) * d1 / (d1 + d2);
            //    }
            //    double mx,my,mz,md;
            //    getGPSbyLocalMileage(mileage, out mx, out my, out mz, out md);
            //    if (mIsReverse)
            //        mileage = mStart - mileage;
            //    else
            //        mileage = mStart + mileage;
            //    distance = CoordinateConverter.getUTMDistance(mx, my, x, y);                
            //    //return mileage;
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("error num" + errorNum);
            //    //return -1;
            //}
            //return true;
        //}

    }
}
