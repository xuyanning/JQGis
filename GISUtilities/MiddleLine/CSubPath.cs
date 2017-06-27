using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Data;
//using ModelInfo.Helper;

namespace GISUtilities
{
    public class PathNode
    {
        public CRailwayLine mRL;
        public double fromMileage;
        public double toMileage;
        public double mLength;
        public bool isReverse;
        public PathNode(CRailwayLine rl, double fromM, double toM, bool re)
        {
            mRL = rl;
            fromMileage = fromM;
            toMileage = toM;
            isReverse = re;
            mLength = Math.Abs(fromM - toM);

        }
    }

    /// <summary>
    /// 两点之间的线路
    /// </summary>
    /// <param name="fromDK"></param>
    /// <param name="fromMileage"></param>
    /// <param name="toDK"></param>
    /// <param name="toMileage"></param>
    public class CSubPath
    {
        List<PathNode> mPathNodeList = null;
        public double[] mNodeLength;
        public bool hasPath;
        public double[] mm, mx, my, mz, md;
        public double[] mxoff, myoff;
        public double moff;
        string mFromDK, mToDK;
        double mFromMileage, mToMileage;
        //double mPercent; // 中间点
        public double mLength = 0;
        double mStepM;
        public int mPointCount = 0;

        #region 高度计算
        public string[] dkmileage;
        public double[] m;// = { 11,180,340,474.862,751.254};
        public double[] h;// = { 201.53, 202.63, 206.36, 210.92, 216.9};
        public double[] r; // = { 0, 3000, 3000,3500};
        public double[] i1;// = {0, 0, 0, 0 }; 前坡度
        public double[] i2;// = {0, 0, 0, 0 }; 后坡度
        public double[] sm;// = {0, 0, 0, 0 };  起始变高
        public double[] em;// = {0, 0, 0, 0 }; 终止变高
        public double[] heightM;

        public void initHeightCurve(string pdPath = @"C:\GISData\Common\pd.xlsx")
        {
            DataTable dt1;
            string code;
            double mileage;
            //double dis;
            int count;
            int i = 0;
            dt1 = ExcelWrapper.LoadDataTableFromExcel(pdPath, @"select mileage, altitude, radius from [sheet1$] order by id");
            count = dt1.Rows.Count;
            dkmileage = new string[count];
            m = new double[count];
            h = new double[count];
            r = new double[count];
            i1 = new double[count];
            i2 = new double[count];
            sm = new double[count];
            em = new double[count];

            //CSubPath path = new CSubPath(CRailwayLineList.gMileage, "DK", 299000, "右改DK", 17600, 10);
            //path.outputPathInfo();
            foreach (DataRow dr in dt1.Rows)
            {
                dkmileage[i] = dr["mileage"].ToString();
                CRailwayLineList.parseDKCode(dkmileage[i], out code, out mileage);
                m[i] = getPathMileageByDKCode(code, mileage);
                if (m[i] >= 0)
                {
                    h[i] = Convert.ToDouble(dr["altitude"]);
                    r[i] = Convert.ToDouble(dr["radius"]);
                }
                else
                {
                    Console.WriteLine(dkmileage[i] + "不存在");
                    
                }
                i++;

            }
            for (i = 1; i < m.Length - 1; i++)
            {
                heightCurveParams(i);
            }
            sm[0] = 0;
            em[0] = 0;
            sm[m.Length - 1] = em[m.Length - 1] = m[m.Length - 1];

        }
        private void createHeightSegment(int typeSeg, int id, double length, double fromM, out double[] mSeg, out double[] hSeg, bool includeLastP = true)
        {
            if (typeSeg == 0) // 前直线
                mSeg = CSubLine.generateSamplePoints(length, 50, fromM, includeLastP);
            else // 圆曲线
                mSeg = CSubLine.generateSamplePoints(length, 10, fromM, includeLastP);
            if (mSeg == null)
            {
                hSeg = null;
                return;
            }
            hSeg = new double[mSeg.Length];
            for (int i = 0; i < mSeg.Length; i++)
            {
                if (typeSeg == 0)
                    hSeg[i] = h[id] + (mSeg[i] - m[id]) * i1[id];
                else
                    hSeg[i] = h[id] + (sm[id] - m[id]) * i1[id] + (mSeg[i] - sm[id]) * i1[id]
                + Math.Sign(i2[id] - i1[id]) * (mSeg[i] - sm[id]) * (mSeg[i] - sm[id]) / r[id] / 2;
            }


        }

        /// <summary>
        /// 利用竖曲线创建高程，写到txt文件中
        /// </summary>
        /// <param name="pdPath">竖曲线坡度表</param>
        public void createHeightSample(string pdPath)
        {
            initHeightCurve(pdPath);
            List<PathNode> ls = mPathNodeList;
            double[][] mlist = new double[ls.Count][];
            double[][] hlist = new double[ls.Count][];
            List<double> mls = new List<double>();
            List<double> hls = new List<double>();

            int j = 0;
            double fromM = 0;
            double[] mtmp, htmp;
            for (int id = 1; id < m.Length; id++)
            {
                while (j < ls.Count && mNodeLength[j] < sm[id])
                {
                    createHeightSegment(0, id, mNodeLength[j] - fromM, fromM, out mtmp, out htmp, true);
                    fromM = mNodeLength[j];
                    mls.AddRange(mtmp);
                    hls.AddRange(htmp);
                    mlist[j] = mls.ToArray();
                    hlist[j] = hls.ToArray();
                    mls.Clear();
                    hls.Clear();
                    j++;
                }
                if (id < m.Length - 1)
                    createHeightSegment(0, id, sm[id] - fromM, fromM, out mtmp, out htmp, false);
                else
                    createHeightSegment(0, id, sm[id] - fromM, fromM, out mtmp, out htmp, true);
                fromM = sm[id];
                mls.AddRange(mtmp);
                hls.AddRange(htmp);
                //double[] mm1 = CSubLine.generateSamplePoints(sm[id] - em[id - 1], 50, em[id - 1]);
                //double[] hh1 = new double[mm1.Length];
                //for (int k = 0; k < hh1.Length; k++)
                //{
                //    hh1[k] = h[id] + (mm1[k] - m[id]) * i1[id];
                //    Console.WriteLine(Math.Round(mm1[k], 2) + "\t" + Math.Round(hh1[k], 2));
                //}
                //Console.WriteLine();
                if (em[id] - sm[id] < 0.1) continue;  // 一般是最后一个点

                while (j < ls.Count && mNodeLength[j] < em[id])
                {
                    createHeightSegment(1, id, mNodeLength[j] - fromM, fromM, out mtmp, out htmp, true);
                    fromM = mNodeLength[j];
                    mls.AddRange(mtmp);
                    hls.AddRange(htmp);
                    mlist[j] = mls.ToArray();
                    hlist[j] = hls.ToArray();
                    mls.Clear();
                    hls.Clear();
                    j++;
                }
                createHeightSegment(1, id, em[id] - fromM, fromM, out mtmp, out htmp, false);
                fromM = em[id];
                mls.AddRange(mtmp);
                hls.AddRange(htmp);
                //double[] mm2 = CSubLine.generateSamplePoints(em[id] - sm[id], 10, sm[id]);
                //double[] hh2 = new double[mm2.Length];
                //for (int k = 0; k < hh2.Length; k++)
                //{
                //    hh2[k] = h[id] + (sm[id] - m[id]) * i1[id] + (mm2[k] - sm[id]) * i1[id]
                //+ Math.Sign(i2[id] - i1[id]) * (mm2[k] - sm[id]) * (mm2[k] - sm[id]) / r[id] / 2;
                //    Console.WriteLine(Math.Round(mm2[k], 2) + "\t" + Math.Round(hh2[k], 2));
                //}
                //Console.WriteLine();

            }
            mlist[mlist.Length - 1] = mls.ToArray();
            hlist[mlist.Length - 1] = hls.ToArray();
            int i = 0;
            double offsetm;
            foreach (PathNode pn in ls)
            {
                if (i == 0)
                    offsetm = 0;
                else
                    offsetm = mNodeLength[i - 1];
                if (!pn.isReverse)
                    offsetm += -Math.Abs(pn.fromMileage - pn.mRL.mStart);
                //else
                //    offsetm += Math.Abs(pn.fromMileage - pn.mRL.mEnd);
                for (j = 0; j < mlist[i].Length; j++)
                {
                    mlist[i][j] = mlist[i][j] - offsetm;
                    //Console.WriteLine(Math.Round(mlist[i][j], 2) + "\t" + Math.Round(hlist[i][j], 2));
                }
                pn.mRL.createExcelData(mlist[i], hlist[i], new System.IO.StreamWriter(pn.mRL.mIndex + ".txt"));
                Console.WriteLine();
                i++;
            }

        }

        public void heightCurveParams(int id)
        {
            i1[id] = (h[id] - h[id - 1]) / (m[id] - m[id - 1]);
            i2[id] = (h[id + 1] - h[id]) / (m[id + 1] - m[id]);
            double t = r[id] * Math.Abs(i2[id] - i1[id]) / 2;
            double e = t * t / r[id] / 2;
            sm[id] = m[id] - t;
            em[id] = m[id] + t;
        }

        //public double getHeight(double mileage, int id)
        //{


        //    double res = 0;
        //    if (mileage < sm[id])
        //    {
        //        res = h[id] + (mileage - m[id]) * i1[id];
        //    }
        //    else if (mileage > em[id])
        //    {
        //        res = h[id] + (mileage - m[id]) * i2[id];
        //    }
        //    else
        //    {
        //        res = h[id] + (sm[id] - m[id]) * i1[id] + (mileage - sm[id]) * i1[id]
        //            + Math.Sign(i2[id] - i1[id]) * (mileage - sm[id]) * (mileage - sm[id]) / r[id] / 2;
        //    }
        //    return res;
        //}

        #endregion

        public CSubPath(MGraph graph, string fromDK, double fromMileage, string toDK, double toMileage, double stepm)
        {
            List<string> strls = new List<string>();
            List<double> dls = new List<double>();
            strls.Add(fromDK);
            strls.Add(toDK);
            dls.Add(fromMileage);
            dls.Add(toMileage);
            CreateSubPathFromNodeList(graph, strls, dls, stepm);


        }


        public CSubPath(MGraph g, List<string> DKlist, List<double> milelist, double stepm)
        {
            CreateSubPathFromNodeList(g, DKlist, milelist, stepm);


        }

        public override string ToString() {
            StringBuilder str = new StringBuilder(256);
            str.Append("起点" + mFromDK + "\t" + Math.Round(mFromMileage, 2)+"\n");
            str.Append("终点" + mToDK + "\t" + Math.Round(mToMileage, 2) + "\n");
            foreach (PathNode p in mPathNodeList)
            {
                str.Append(p.mRL + "\n");
            }
            return str.ToString();
        }
        /// <summary>
        /// 生成子路径
        /// </summary>
        /// <param name="g">寻径的floyd图</param>
        /// <param name="DKlist">路径上的dkcode列表，至少2个</param>
        /// <param name="milelist">路径上的里程列表，至少2个</param>
        /// <param name="stepm">细分里程间隔</param>        
        private void CreateSubPathFromNodeList(MGraph g, List<string> DKlist, List<double> milelist, double stepm)
        {
            if (DKlist == null || DKlist.Count < 2)
                return;
            mFromDK = DKlist.First();
            mToDK = DKlist.Last();
            mFromMileage = milelist.First();
            mToMileage = milelist.Last();
            mStepM = stepm;
            mLength = 0;
            List<PathNode> ls = new List<PathNode>();
            // 路径节点，0，2，4，6下标为链路编号，1，3，5，7等标注正序还是逆序，0-正序，1-逆序
            List<int> idls = null;
            CRailwayLine tmpLine;

            //存放所有路径上的链路编号
            int[] rlls = new int[DKlist.Count];

            for (int i = 0; i < DKlist.Count; i++)
            {
                tmpLine = CRailwayLineList.getRailwayLineByDKCode(DKlist[i], milelist[i]);
                if (tmpLine == null)
                    return;
                rlls[i] = tmpLine.mIndex;
            }


            // 路径节点，0，2，4，6下标为路径编号，1，3，5，7等标注正序还是逆序，0-正序，1-逆序
            idls = ShortestPathFloyd.getNavPath(rlls, g);
            if (idls == null)
            {
                Console.WriteLine(DKlist[0] + "等路径不存在");
                return;
            }
            if (DKlist.Count == 2 && rlls[0] == rlls[1])  // 起点和终点在同一个链路中
            {
                CRailwayLine rl = CRailwayLineList.getRailwayLineByIndex(idls[0]);
                mLength = Math.Abs(milelist[0] - milelist.Last());
                //if (idls[1] == 0)
                    ls.Add(new PathNode(rl, milelist[0], milelist.Last(), false));
                //else
                //    ls.Add(new PathNode(rl, milelist.Last(), milelist[0], true));
               
            }
            else if (idls.Count > 2)
            {
                mLength = 0;
                mNodeLength = new double[idls.Count / 2];
                // path中的第一条链
                CRailwayLine rl = CRailwayLineList.getRailwayLineByIndex(idls[0]);
                if (idls[1] == 0)
                {
                    ls.Add(new PathNode(rl, milelist[0], rl.mEnd, false));
                    mLength += Math.Abs(milelist[0] - rl.mEnd);
                }
                else
                {
                    ls.Add(new PathNode(rl, milelist[0], rl.mStart, true));
                    mLength += Math.Abs(milelist[0] - rl.mStart);
                }
                mNodeLength[0] = mLength;
                if (idls.Count > 2)
                {
                    // path的中间链
                    for (int i = 1; i < idls.Count / 2 - 1; i++)
                    {
                        rl = CRailwayLineList.getRailwayLineByIndex(idls[2 * i]);
                        if (idls[2 * i + 1] == 0)
                            ls.Add(new PathNode(rl, rl.mStart, rl.mEnd, false));
                        else
                            ls.Add(new PathNode(rl, rl.mEnd, rl.mStart, true));
                        mLength += Math.Abs(rl.mLength);
                        mNodeLength[i] = mLength;
                    }

                    // path的最后一条链
                    rl = CRailwayLineList.getRailwayLineByIndex(idls[idls.Count - 2]);
                    if (idls.Last() == 0)
                    {
                        ls.Add(new PathNode(rl, rl.mStart, milelist.Last(), false));
                        mLength += Math.Abs(rl.mStart - milelist.Last());
                    }
                    else
                    {
                        ls.Add(new PathNode(rl, rl.mEnd, milelist.Last(), true));
                        mLength += Math.Abs(rl.mEnd - milelist.Last());
                    }
                    mNodeLength[mNodeLength.Length - 1] = mLength;
                }
            }

            mPathNodeList = ls;

            hasPath = mPathNodeList.Count > 0 && mLength > 1;

            if (hasPath)
                createSubLine();
        }

        /// <summary>
        /// 计算路径上一点的里程
        /// </summary>
        /// <param name="dkcode"></param>
        /// <param name="mileage"></param>
        /// <returns></returns>
        private double getPathMileageByDKCode(string dkcode, double mileage)
        {
            double pathMileage = 0;
            bool found = false;
            foreach (PathNode pn in mPathNodeList)
            {
                // FIXME 0608 调用mRL的方法判断里程
                if (pn.mRL.isInside(dkcode, mileage))
                {
                    pathMileage += Math.Abs(mileage - pn.fromMileage);
                    found = true;
                    break;
                }
                else
                {
                    pathMileage += pn.mLength;
                }
            }
            if (!found)
                pathMileage = -1;
            return pathMileage;
        }

        public bool getPathMileageByDKCode(string dkcode, double dkmileage, out double pathMileage, out double dis)
        {
            dis = pathMileage = 0;
            bool found = true;
            pathMileage = getPathMileageByDKCode(dkcode, dkmileage);

            // 如果不在路径上，计算gps，根据gps计算到本路径的最近点的里程。
            if (pathMileage < 0)
            {
                double x, y, z, d;
                CRailwayLineList.getGPSbyDKCode(dkcode, dkmileage, out x, out y, out z, out d);
                getPathMileagebyGPS(x, y, out pathMileage, out dis);
                found = false;
            }
            return found;
        }

        public bool getDKCodebyPathMileage(double pathmileage, out string dkcode, out double dkmileage)
        {
            bool found = false;
            dkcode = "";
            dkmileage = 0;
            if (pathmileage < 0 || pathmileage > mLength)
                return found;
            foreach (PathNode pn in mPathNodeList)
            {

                if (pathmileage > pn.mLength)
                {
                    pathmileage -= pn.mLength;
                }
                else
                {
                    dkcode = pn.mRL.mDKCode;
                    if (pn.mRL.mIsReverse)
                    {
                        dkmileage = pn.fromMileage - pathmileage;
                    }
                    else
                        dkmileage = pn.fromMileage + pathmileage;
                    found = true;
                    break;
                }
            }
            return found;

        }

        public void outputPathInfo()
        {
            string dkcode;
            double mileage;
            for (int i = 0; i < mm.Length; i++)
            {
                getDKCodebyPathMileage(mm[i], out dkcode, out mileage);
                Console.WriteLine(mm[i] + "\t" + dkcode + "\t" + mileage);
            }
        }
        /// <summary>
        /// 重采样，根据起始里程，获取距离为dis的子路径
        /// </summary>
        /// <param name="fromdkcode"></param>
        /// <param name="frommileage"></param>
        /// <param name="dis"></param>
        /// <param name="stepm"></param>
        /// <param name="mils"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public int getSubLineByDKCode(string fromdkcode, double frommileage, double dis, double stepm, out double[] mils, out double[] x, out double[] y, out double[] z, out double[] dir)
        {
            mils = x = y = z = dir = null;
            double m = getPathMileageByDKCode(fromdkcode, frommileage);
            return CSubLine.getSubLineByMileage(m, m + dis, stepm, mm, mx, my, mz, md, out mils, out x, out y, out z, out dir);
            //double m1, m2;
            //if (dis > 0)
            //{
            //    m1 = m;
            //    m2 = m + dis;
            //}
            //else
            //{
            //    m1 = m + dis;
            //    m2 = m;
            //}
            //m1 = Math.Max(m1, 0);
            //m2 = Math.Min(m2, mLength);

            //dis = m2 - m1;
            ////double dir;
            //stepm = Math.Min(1,Math.Abs(stepm));

            //int count = (int)((dis - 0.05) / stepm) + 1;
            //// 保证count至少为2
            //if (dis < stepm * 2)
            //    count = 2;

            //mils = new double[count];
            //x = new double[count];
            //y = new double[count];
            //z = new double[count];
            //dir = new double[count];
            //double temp;

            //m = m1;
            //for (int i = 0; i < count - 1; i++)
            //{
            //    mils[i] = m;
            //    getGPSbyPathMileage(m, out x[i], out y[i], out z[i], out dir[i],out temp);
            //    m += stepm;
            //}
            //mils[count - 1] = m2;
            //getGPSbyPathMileage(m2, out x[count - 1], out y[count - 1], out z[count - 1], out dir[count - 1],out temp);

            //return count;
        }

        public void getSubLinebyPathMileage(double startm, double endm, double stepm, out double[] dm, out double[] dx, out double[] dy, out double[] dz, out double[] dd)
        {
            CSubLine.getSubLineByMileage(startm, endm, stepm, mm, mx, my, mz, md, out dm, out dx, out dy, out dz, out dd, true);

        }
        // 给定里程，计算经纬度朝向坐标
        private bool getGPSbyPathMileage(double m, out double x, out double y, out double z, out double dir, out double pitch)
        {
            return CSubLine.getGPSbyMileage(m, mm, mx, my, mz, md, out x, out y, out z, out dir, out pitch);
            //bool flag = true;
            //int index;
            //x = y = z = 0; dir = 0;

            //if (m < 0.001)
            //{
            //    flag = false;
            //    index = 0;
            //    //x = mx[0];
            //    //y = my[0];
            //    //z = mz[0];
            //    //if (md != null)
            //    //    dir = md[0];
            //}
            //else if (m > mLength - 0.001)
            //{
            //    flag = false;
            //    index = mPointCount - 2;
            //    //x = mx[mPointCount - 1];
            //    //y = my[mPointCount - 1];
            //    //z = mz[mPointCount - 1];
            //    //if (md != null)
            //    //    dir = md[mPointCount - 1];
            //}
            //else
            //{
            //    flag = true;
            //    index = (int)(m / mStepM);


            //    if (index == mPointCount - 1)
            //    {
            //        index--;
            //        //x = mx[index];
            //        //y = my[index];
            //        //z = mz[index];
            //        //if (md != null)
            //        //    dir = md[index];
            //    }
            //}

            //// Important FIX，前面方法对于距离小于10米的最后一段计算错误
            //double t = (m - mm[index]) / (mm[index + 1] - mm[index]);
            //x = mx[index] * (1 - t) + mx[index + 1] * t;
            //y = my[index] * (1 - t) + my[index + 1] * t;
            //z = mz[index] * (1 - t) + mz[index + 1] * t;
            //if (md != null)
            //    dir = md[index] * (1 - t) + md[index + 1] * t;

            //return flag;
        }

        //public void getSubLineFromDKCode(string dkcode, double mileage, double dis, out double[] x, out double[] y, out double[] z, out double[] dir)
        //{
        //    x = y = z = dir = null;
        //    double m = getPathMileageByDKCode(dkcode, mileage) ;
        //    double m1, m2;
        //    if (dis > 0)
        //    {
        //        m1 = m;
        //        m2 = m + dis;
        //    }else
        //    {
        //        m1 = m + dis;
        //        m2 = m;
        //    }
        //    int fromIdx = getMileageIndex(m1);
        //    int toIdx = getMileageIndex(m2);

        //}

        //int getMileageIndex(double mileage)
        //{
        //    if (mileage < 0 || mileage > mLength)
        //        return -1;
        //    int l = 0, r = mm.Length - 1;
        //    int mid = (l + r) / 2;
        //    while (l < r)
        //    {
        //        if (mileage < mm[mid])
        //            r = mid - 1;
        //        else if (mileage > mm[mid])
        //            l = mid + 1;
        //        else
        //            return mid;
        //        mid = (l + r) / 2;
        //    }
        //    r = Math.Max(r, 0);
        //    l = Math.Min(l, mm.Length-1);
        //    return Math.Abs(mm[r] - mileage) < Math.Abs(mm[l] - mileage) ? r :l ;


        //}


        ///// <summary>
        ///// 计算偏移一定角度和距离之后的经纬度
        ///// </summary>
        ///// <param name="startDKM"></param>
        ///// <param name="endDKM"></param>
        ///// <param name="stepm"></param>
        ///// <param name="angleOff"></param>
        ///// <param name="disOff"></param>
        ///// <param name="latout"></param>
        ///// <param name="lonout"></param>
        ///// <param name="z"></param>
        ///// <returns></returns>
        //public int getOffsetLine(double angleOff, double disOff, out double[] latout, out double[] lonout, out double[] z)
        //{
        //    lonout = latout = z = null;
        //    //double[] x = null;
        //    //double[] y = null;
        //    ////double[] z = null;
        //    //double[] d = null;

        //    //int pointNum = getSubLineByDKCode(startDKM, endDKM, stepm, out x, out y, out z, out d);
        //    if (mPointCount > 0)
        //    {
        //        latout = new double[mPointCount];
        //        lonout = new double[mPointCount];
        //        for (int i = 0; i < mPointCount; i++)
        //        {
        //            CoordinateConverter.LatLonOffest(my[i], mx[i], md[i], angleOff, disOff, out latout[i], out lonout[i]);  //lat, lon
        //        }
        //        z = mz;

        //    }

        //    return mPointCount;
        //}


        ///// <summary>
        ///// 求点与多条线段的最短距离，如果投影在多条线段内，返回t,否则f
        ///// </summary>
        ///// <param name="x">点</param>
        ///// <param name="y">点</param>
        ///// <param name="xa">多线段</param>
        ///// <param name="ya"></param>
        ///// <param name="ma"></param>
        ///// <param name="fromid">多线段起点下标</param>
        ///// <param name="toid">终点下标</param>
        ///// <param name="m">最近点</param>
        ///// <param name="dis">距离</param>
        ///// <returns></returns>
        //private bool getDistance(double x, double y, double[]xa, double[] ya, double[] ma ,int fromid, int toid, out double m, out double dis)
        //{
        //    bool found = false;
        //    m = dis = 0;
        //    double lx = xa[fromid];
        //    double ly = ya[fromid];
        //    double rx = xa[toid];
        //    double ry = ya[toid];
        //    if ((x - lx) * (rx - lx) + (y - ly) * (ry - ly) < 0)
        //    {
        //        m = ma[fromid];
        //        dis = CoordinateConverter.getUTMDistance(x, y, lx, ly);
        //    }
        //    else if ((x - rx)*(lx - rx) + (y - ry) * (ly - ry) < 0)
        //    {
        //        m = ma[toid];
        //        dis = CoordinateConverter.getUTMDistance(x, y, rx, ry);
        //    }
        //    else
        //    {
        //        found = true;
        //        int minid = fromid;
        //        dis = CoordinateConverter.getUTMDistance(x, y, lx, ly);
        //        for (int i = fromid + 1; i<= toid; i++)
        //        {
        //            double di = CoordinateConverter.getUTMDistance(x, y, xa[i], ya[i]);
        //            if (di < dis) {
        //                dis = di;
        //                minid = i; 
        //            }
        //        }
        //        int lid = minid - 1;
        //        if (lid < fromid) lid = fromid;
        //        int rid = minid + 1;
        //        if (rid > toid) rid = toid;
        //        double d1 = CoordinateConverter.getUTMDistance(x, y, xa[lid], ya[lid]);
        //        double d2 = CoordinateConverter.getUTMDistance(x, y, xa[rid], ya[rid]);

        //        double ddd =d1 /(d1 + d2);
        //        //if (d1 > d2)
        //        //{
        //        //    ddd = disInterpolate(x, y, xa[lid], ya[lid], xa[rid], ya[rid]);
        //        //}
        //        //else
        //        //{
        //        //    ddd = 1 - disInterpolate(x,y,xa[rid],ya[rid],xa[lid],ya[lid]);
        //        //}
        //        m = ma[lid] + (ma[rid]-ma[lid])  * ddd;
        //        double xm = xa[lid] + (xa[rid]-xa[lid])  *ddd;
        //        double ym = ya[lid] + (ya[rid] - ya[lid]) * ddd;
        //        dis = CoordinateConverter.getUTMDistance(x, y, xm, ym);
        //    }
        //    return found;
        //}

        ///误差太大
        //private double disInterpolate(double x1,double y1,double x2,double y2,double x3,double y3)
        //{
        //    double dx1 = x1 - x2;
        //    double dy1 = y1 - y2;
        //    double dx2 = x3 - x2;
        //    double dy2 = y3 - y2;
        //    double len1 = Math.Sqrt(dx1 * dx1 + dy1 * dy1);
        //    double len2 = Math.Sqrt(dx2 * dx2 + dy2 * dy2);
        //    double cosa = (dx1 * dx2 + dy1 * dy2) / len1 / len2;
        //    double d = len1 * Math.Sqrt (1 - cosa * cosa);
        //    return d / len2;
        //}
        /// <summary>
        /// 给定经纬度，输出最近点里程与距离
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mileage"></param>
        /// <param name="distance"></param>
        public void getPathMileagebyGPS(double x, double y, out double mileage, out double distance)
        {
            distance = 5000000;
            mileage = -1;
            if (!hasPath)
                return;
            //double m, d;

            //int step = Math.Max(100, (int)Math.Sqrt(mPointCount));
            //int toid = 0;

            //for (int i = 0; i<mPointCount; i+= step)
            //{
            //    toid = Math.Min(i + step , mPointCount - 1);
            //    getDistance(x, y, mx, my, mm, i, toid, out m, out d);
            //    if (d<distance)
            //    {
            //        distance = d;
            //        mileage = m;
            //    }
            //}

            CSubLine.getMileagebyGPS(x, y, mm, mx, my, out mileage, out distance);

        }

        /// <summary>
        /// 获取中线，根据 pathNode 0判断单线复线，
        /// </summary>
        /// <param name="ml"></param>
        /// <param name="xl"></param>
        /// <param name="yl"></param>
        /// <param name="zl"></param>
        /// <param name="dl"></param>
        /// <returns></returns>
        public bool getContactLine(out double[] x, out double[] y, out double[] z,
            out double[] x2, out double[] y2, out double[] z2)
        {
            double[] m, m2, dir, dir2;
            m = x = y = z = dir = null;
            m2 = x2 = y2 = z2 = dir2 = null;

            List<PathNode> ls = mPathNodeList; //getSubPath(startDKCode, startMileage, endDKCode, endMileage);

            if (ls == null || ls.Count == 0)
                return false;
            //int totalCount = 0;
            // 如果是单线，接触网为中线，如果是复线，接触网为左线
            CSubLine.getSubLineByMileage(0, mLength, 50, mm, mx, my, mz, md, out m, out x, out y, out z, out dir);
            if (ls[0].mRL.mIsDouble)
            {
                // 复线，求另一个接触网
                CSubLine.getOffsetLineByMileage(0, mLength, 50, mm, mx, my, mz, md, moff, mxoff, myoff, moff * 2, out m2, out x2, out y2, out z2, out dir2, true);
                return true;
            }

            //CSubLine.getSubLineByMileage(0, mLength, mStepM, mm, mx, my, mz, md, out ml, out xl, out yl, out zl, out dl, true);
            return false;

        }

        /// <summary>
        /// 获取中线，根据 pathNode 0判断单线复线，
        /// </summary>
        /// <param name="ml"></param>
        /// <param name="xl"></param>
        /// <param name="yl"></param>
        /// <param name="zl"></param>
        /// <param name="dl"></param>
        /// <returns></returns>
        public bool getMiddleLine(out double[] ml, out double[] xl, out double[] yl, out double[] zl, out double[] dl)
        {
            //bool isDoubleLine = false;
            xl = yl = zl = dl = ml = null;

            List<PathNode> ls = mPathNodeList; //getSubPath(startDKCode, startMileage, endDKCode, endMileage);

            if (ls == null || ls.Count == 0)
                return false;
            //int totalCount = 0;
            if (ls[0].mRL.mIsDouble)
            {
                CSubLine.getSubLineByMileage(0, mLength, mStepM, mm, mxoff, myoff, mz, md, out ml, out xl, out yl, out zl, out dl, true);
                return true;
            }

            CSubLine.getSubLineByMileage(0, mLength, mStepM, mm, mx, my, mz, md, out ml, out xl, out yl, out zl, out dl, true);
            return false;

        }

        private void createSubLine()
        {
            List<PathNode> ls = mPathNodeList; //getSubPath(startDKCode, startMileage, endDKCode, endMileage);

            int totalCount = 0;
            if (ls == null || ls.Count == 0)
                return;
            moff = ls[0].mRL.mOffset;
            if (ls.Count == 1)
            {
                totalCount = ls[0].mRL.getSubLineByDKMileage(ls[0].fromMileage, ls[0].toMileage, mStepM, out this.mm, out mx, out my, out mz, out md, true);
                ls[0].mRL.getSubLine2ByDKMileage(ls[0].fromMileage, ls[0].toMileage, mStepM, out mxoff, out myoff, true);
            }
            else
            {
                double[][] mm = new double[ls.Count][];
                double[][] xx = new double[ls.Count][];
                double[][] yy = new double[ls.Count][];
                double[][] zz = new double[ls.Count][];
                double[][] ddir = new double[ls.Count][];
                double[][] offx = new double[ls.Count][];
                double[][] offy = new double[ls.Count][];
                int[] count = new int[ls.Count];

                int i = 0;
                foreach (PathNode pn in ls)
                {
                    if (i == ls.Count - 1)
                    {
                        count[i] = pn.mRL.getSubLineByDKMileage(pn.fromMileage, pn.toMileage, mStepM, out mm[i], out xx[i], out yy[i], out zz[i], out ddir[i], true);
                        pn.mRL.getSubLine2ByDKMileage(pn.fromMileage, pn.toMileage, mStepM, out offx[i], out offy[i], true);
                    }
                    else
                    {
                        count[i] = pn.mRL.getSubLineByDKMileage(pn.fromMileage, pn.toMileage, mStepM, out mm[i], out xx[i], out yy[i], out zz[i], out ddir[i], false);
                        pn.mRL.getSubLine2ByDKMileage(pn.fromMileage, pn.toMileage, mStepM, out offx[i], out offy[i], false);

                    }
                    totalCount += count[i];
                    i++;
                }
                mx = new double[totalCount];
                my = new double[totalCount];
                mz = new double[totalCount];
                md = new double[totalCount];
                this.mm = new double[totalCount];
                mxoff = new double[totalCount];
                myoff = new double[totalCount];
                totalCount = 0;
                int mCount = 0;
                double glen = 0;
                for (i = 0; i < ls.Count; i++)
                {
                    //glen = mm[i][0];

                    for (int j = 0; j < count[i]; j++)
                    {
                        this.mm[mCount++] = glen + Math.Abs(mm[i][j] - mm[i][0]);
                    }
                    glen += ls[i].mLength;

                    xx[i].CopyTo(mx, totalCount);
                    yy[i].CopyTo(my, totalCount);
                    zz[i].CopyTo(mz, totalCount);
                    ddir[i].CopyTo(md, totalCount);
                    offx[i].CopyTo(mxoff, totalCount);
                    offy[i].CopyTo(myoff, totalCount);
                    totalCount += count[i];
                }
            }
            mPointCount = totalCount;

        }

    }
}
