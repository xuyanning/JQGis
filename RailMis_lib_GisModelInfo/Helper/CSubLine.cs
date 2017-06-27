using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInfo.Helper
{
    public class CSubLine
    {
        public static double[] getHeightList(double[] mileage, double[] height, double[] radius, double[] m)
        {
            double[] newHeight = null;
            //m = null;
            int j = 0;
            if (mileage == null || mileage.Length <= 2 || m == null || m.Length == 0)
                return null;
            newHeight = new double[m.Length];
            double d1, d2, t, e, lastpoint;
            double[] mpre, mnext;
            mpre = new double[mileage.Length];
            mnext = new double[mileage.Length];
            for (int i = 1; i < mileage.Length - 1; i++)
            {
                d1 = (height[i] - height[i - 1]) / (mileage[i] - mileage[i - 1]);
                d2 = (height[i + 1] - height[i]) / (mileage[i - 1] - mileage[i]);
                t = radius[i] * Math.Abs(d1 - d2) / 2;
                e = radius[i] * (d1 - d2) * (d1 - d2) / 8;
                mpre[i] = m[i] - t;
                mnext[i] = m[i] + t;

                if (j == m.Length)
                    return newHeight;
                if (i == mileage.Length - 2)
                {
                    lastpoint = mileage[mileage.Length - 1];
                }
                else
                {
                    lastpoint = mnext[i];
                }
                while (m[j] <= lastpoint)
                {
                    if (m[j] < mpre[i])
                    {
                        newHeight[j] = height[i] + (m[j] - mileage[i]) * d1;
                    }
                    else if (m[j] > mnext[i])
                    {
                        newHeight[j] = height[i] + (m[j] - mileage[i]) * d2;
                    }
                    else
                    {
                        newHeight[j] = height[i] + (mpre[i] - mileage[i]) * d1 + Math.Sign(d2 - d1) * (m[j] - mpre[i]) * (m[j] - mpre[i]) / 2 / radius[i];
                    }
                    j++;
                }
            }
            return newHeight;
        }

        public static double[] getPitchList(double[] meter, double[] altitude)
        {
            double[] pitching = new double[meter.Length];

            for (int i = 1; i < meter.Length; i++)
            {
                pitching[i] = (altitude[i] - altitude[i - 1]) / (meter[i] - meter[i - 1]);

            }
            pitching[0] = pitching[1];
            return pitching;
        }

        /// <summary>
        /// 获取偏移线，
        /// </summary>
        /// <param name="startm">起始里程</param>
        /// <param name="endm">终止里程</param>
        /// <param name="stepm">偏移线的采样步长</param>
        /// <param name="sm">里程数组</param>
        /// <param name="sx">经度x坐标数组</param>
        /// <param name="sy">纬度y坐标数组</param>
        /// <param name="sz">高度z坐标数组</param>
        /// <param name="sd">朝向dir数组</param>
        /// <param name="offdis">偏移距离</param>
        /// <param name="offx"></param>
        /// <param name="offy"></param>
        /// <param name="offdisNew"></param>
        /// <param name="mileage"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="dir"></param>
        /// <param name="includeLastPoint"></param>
        /// <returns></returns>
        public static int getOffsetLineByMileage(double startm, double endm, double stepm, double[] sm, double[] sx, double[] sy, double[] sz, double[] sd,
            double offdis, double[] offx, double[] offy, double offdisNew, out double[] mileage, out double[] x, out double[] y, out double[] z, out double[] dir, bool includeLastPoint = false)
        {
            mileage = x = y = z = dir = null;

            stepm = Math.Max(1, Math.Abs(stepm));
            double dis = endm - startm;
            if (dis > 0)
            {
                if (!includeLastPoint)
                    endm = endm - stepm;
            }
            else // 初始里程大于结束里程
            {
                if (!includeLastPoint)
                    endm = endm + stepm;
                dis = -dis;
                stepm = -stepm;

            }

            int count = (int)((dis - 0.05) / Math.Abs(stepm)) + 1;
            // 保证count至少为2
            if (dis < Math.Abs(stepm) * 2)
                count = 2;

            double m;
            mileage = new double[count];
            x = new double[count];
            y = new double[count];
            z = new double[count];
            dir = new double[count];

            m = startm;
            for (int i = 0; i < count - 1; i++)
            {
                mileage[i] = Math.Abs(m - startm);
                getGPSOffsetbyMileage(m, sm, sx, sy, sz, sd, offdis, offx, offy, offdisNew, out x[i], out y[i], out z[i], out dir[i]);
                m += stepm;
            }
            // 最后一个点的步长不是严格的stepm，介于stepm~2*stepm之间
            mileage[count - 1] = Math.Abs(endm - startm);
            getGPSOffsetbyMileage(endm, sm, sx, sy, sz, sd, offdis, offx, offy, offdisNew, out x[count - 1], out y[count - 1], out z[count - 1], out dir[count - 1]);

            return count;
        }

        public static double[] generateSamplePoints(double lengthd, double stepd, double fromd = 0, bool includeLastPoint = true)
        {
            double[] res = null;
            int count = 0;
            if (lengthd < 1)
            {
                if (includeLastPoint)
                {
                    res = new double[1];
                    res[0] = fromd;
                }
                return res;
            }
            count = (int)((lengthd - 1) / stepd) + 2;
            if (includeLastPoint)
                res = new double[count];
            else
                res = new double[count - 1];
            res[0] = fromd;
            for (int i = 1; i < count - 1; i++)
            {
                res[i] = res[i - 1] + stepd;
            }
            if (includeLastPoint)
                res[count - 1] = fromd + lengthd;

            return res;
        }

        public static int getSubLineByMileage(double startm, double endm, double stepm, double[] sm, double[] sx, double[] sy, double[] sz, double[] sd,
            out double[] mileage, out double[] x, out double[] y, out double[] z, out double[] dir, bool includeLastPoint = false)
        {
            mileage = x = y = z = dir = null;

            stepm = Math.Max(1, Math.Abs(stepm));
            double dis = endm - startm;
            if (dis > 0)
            {
                if (!includeLastPoint)
                    endm = endm - stepm;
            }
            else // 初始里程大于结束里程
            {
                if (!includeLastPoint)
                    endm = endm + stepm;
                dis = -dis;
                stepm = -stepm;

            }

            int count = (int)((dis - 0.05) / Math.Abs(stepm)) + 1;
            // 保证count至少为2
            if (dis < Math.Abs(stepm) * 2)
                count = 2;

            double m, temp;
            mileage = new double[count];
            x = new double[count];
            y = new double[count];
            z = new double[count];
            dir = new double[count];


            m = startm;
            for (int i = 0; i < count - 1; i++)
            {
                mileage[i] = Math.Abs(m - startm);
                getGPSbyMileage(m, sm, sx, sy, sz, sd, out x[i], out y[i], out z[i], out dir[i], out temp);
                m += stepm;
            }
            // 最后一个点的步长不是严格的stepm，介于stepm~2*stepm之间
            mileage[count - 1] = Math.Abs(endm - startm);
            getGPSbyMileage(endm, sm, sx, sy, sz, sd, out x[count - 1], out y[count - 1], out z[count - 1], out dir[count - 1], out temp);

            return count;
        }
        /// <summary>
        /// 获取里程数组sm中第一个小于里程m的下标
        /// </summary>
        /// <param name="m"></param>
        /// <param name="sm"></param>
        /// <returns>如果都大于m，返回-1</returns>
        public static int getMileageIndex(double m, double[] sm)
        {
            if (m <= sm[0])
                return -1;
            if (m >= sm[sm.Length - 1])
                return sm.Length - 1;
            int l = 0;
            int r = sm.Length - 1;
            int mid = (l + r) / 2;
            while (r >= l && mid > 0 && mid < sm.Length - 1)
            {
                if (sm[mid] <= m && m <= sm[mid + 1])
                {
                    return mid;
                }
                else if (sm[mid - 1] <= m && m <= sm[mid])
                {
                    return mid - 1;
                }

                if (sm[mid] <= m)
                {
                    l = mid + 1;
                }
                else
                {
                    r = mid - 1;
                }
                mid = (l + r) / 2;
            }
            //Console.WriteLine("find id error" + mid);
            return mid;
        }

        public static bool getGPSOffsetbyMileage(double m, double[] sm, double[] sx, double[] sy, double[] sz, double[] sd,
            double offdis, double[] offx, double[] offy, double offdisNew, out double x, out double y, out double z, out double dir)
        {
            bool flag = true;
            x = y = z = 0; dir = 0;
            double x2, y2, temp;
            flag = getGPSbyMileage(m, sm, sx, sy, sz, sd, out x, out y, out z, out dir, out temp);
            if (Math.Abs(offdis) < 0.01 || Math.Abs(offdisNew) < 0.01)
                return flag;
            double t = offdisNew / offdis;
            getGPSbyMileage(m, sm, offx, offy, sz, sd, out x2, out y2, out z, out dir, out temp);
            x = x * (1 - t) + x2 * t;
            y = y * (1 - t) + y2 * t;
            return flag;
        }
        // 给定里程，计算经纬度朝向坐标
        public static bool getGPSbyMileage(double m, double[] sm, double[] sx, double[] sy, double[] sz, double[] sd,
            out double x, out double y, out double z, out double dir, out double pitch)
        {
            bool flag = true;
            x = y = z = 0; dir = 0;

            int id = getMileageIndex(m, sm);
            int index = id;
            if (index < 0)
                index = 0;
            else if (index >= sm.Length - 1)
                index = sm.Length - 2;
            flag = m > sm[0] && m < sm[sm.Length - 1];
            double t = (m - sm[index]) / (sm[index + 1] - sm[index]);
            x = sx[index] * (1 - t) + sx[index + 1] * t;
            y = sy[index] * (1 - t) + sy[index + 1] * t;
            z = sz[index] * (1 - t) + sz[index + 1] * t;

            dir = sd[index] * (1 - t) + sd[index + 1] * t;
            pitch = (sz[index + 1] - sz[index]) / (sm[index + 1] - sm[index]);
            //if (flag && mIsDouble)
            //{
            //    double xx, yy;
            //    // FIXME 数据库中加入单轨，双轨。 单独写一个工具，双线，间距 5米，中线，左右线间距中线2.5米，电线杆距离中线5.5米，行道树间距中线10米，存储 里程，经纬，height,yaw, pitch. 接触网高5.6米 6.6米。电线杆 50米间距
            //    CoordinateConverter.LatLonOffest(y, x, dir, 90, 2.5, out yy, out xx);  //lat, lon
            //    x = xx;
            //    y = yy;
            //}
            return flag;
        }

        /// <summary>
        /// 给定经纬度，输出最近点里程与距离
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mileage"></param>
        /// <param name="distance"></param>
        public static void getMileagebyGPS(double x, double y, double[] mm, double[] mx, double[] my, out double mileage, out double distance)
        {
            distance = 5000000;
            mileage = -1;

            double m, d;

            int step = Math.Max(100, (int)Math.Sqrt(mm.Length));
            int toid = 0;

            for (int i = 0; i < mm.Length; i += step)
            {
                toid = Math.Min(i + step, mm.Length - 1);
                getDistance(x, y, mx, my, mm, i, toid, out m, out d);
                if (d < distance)
                {
                    distance = d;
                    mileage = m;
                }
            }

        }

        /// <summary>
        /// 求点与多条线段的最短距离，如果投影在多条线段内，返回t,否则f
        /// </summary>
        /// <param name="x">点</param>
        /// <param name="y">点</param>
        /// <param name="xa">多线段</param>
        /// <param name="ya"></param>
        /// <param name="ma"></param>
        /// <param name="fromid">多线段起点下标</param>
        /// <param name="toid">终点下标</param>
        /// <param name="m">最近点</param>
        /// <param name="dis">距离</param>
        /// <returns></returns>
        private static bool getDistance(double x, double y, double[] xa, double[] ya, double[] ma, int fromid, int toid, out double m, out double dis)
        {
            bool found = false;
            m = dis = 0;
            double lx = xa[fromid];
            double ly = ya[fromid];
            double rx = xa[toid];
            double ry = ya[toid];
            if ((x - lx) * (rx - lx) + (y - ly) * (ry - ly) < 0)
            {
                m = ma[fromid];
                dis = CoordinateConverter.getUTMDistance(x, y, lx, ly);
            }
            else if ((x - rx) * (lx - rx) + (y - ry) * (ly - ry) < 0)
            {
                m = ma[toid];
                dis = CoordinateConverter.getUTMDistance(x, y, rx, ry);
            }
            else
            {
                found = true;
                int minid = fromid;
                dis = CoordinateConverter.getUTMDistance(x, y, lx, ly);
                for (int i = fromid + 1; i <= toid; i++)
                {
                    double di = CoordinateConverter.getUTMDistance(x, y, xa[i], ya[i]);
                    if (di < dis)
                    {
                        dis = di;
                        minid = i;
                    }
                }
                int lid = minid - 1;
                if (lid < fromid) lid = fromid;
                int rid = minid + 1;
                if (rid > toid) rid = toid;
                double d1 = CoordinateConverter.getUTMDistance(x, y, xa[lid], ya[lid]);
                double d2 = CoordinateConverter.getUTMDistance(x, y, xa[rid], ya[rid]);

                double ddd = d1 / (d1 + d2);
                //if (d1 > d2)
                //{
                //    ddd = disInterpolate(x, y, xa[lid], ya[lid], xa[rid], ya[rid]);
                //}
                //else
                //{
                //    ddd = 1 - disInterpolate(x,y,xa[rid],ya[rid],xa[lid],ya[lid]);
                //}
                m = ma[lid] + (ma[rid] - ma[lid]) * ddd;
                double xm = xa[lid] + (xa[rid] - xa[lid]) * ddd;
                double ym = ya[lid] + (ya[rid] - ya[lid]) * ddd;
                dis = CoordinateConverter.getUTMDistance(x, y, xm, ym);
            }
            return found;
        }
    }
}
