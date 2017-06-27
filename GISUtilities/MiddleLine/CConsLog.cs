using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
//using ModelInfo.Helper;
using System.ComponentModel;
//using lib_GIS.Service;

namespace GISUtilities
{
    /// <summary>
    /// 施工日志，或者周志，记录最近施工情况
    /// </summary>
    /// Sample
    /// <UsrID>105</UsrID>
    //<UsrName>高世超</UsrName>
    //<ConsDate>2016-03-11T10:10:57+08:00</ConsDate>
    //<Longitude>117.12375700</Longitude>
    //<Latitude>36.68019100</Latitude>
    //<ProjectID>56</ProjectID>
    //<ProjectName>济南特大桥8#～13#墩（18+3*24+18）</ProjectName>
    //<DwName>8#墩</DwName>

    //public class CConsRecord{
    //    string usrName;
    //    string consDate;
    //    decimal longitude;
    //    decimal latitude;
    //    string projName;
    //}

    public class ConsLocation
    {
        private double latitude;
        [CategoryAttribute("实名信息"), ReadOnlyAttribute(true), DisplayName("中心点纬度"), Browsable(false)]
        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }

        private double longitude;
        [CategoryAttribute("实名信息"), ReadOnlyAttribute(true), DisplayName("中心点经度"), Browsable(false)]
        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }
        private string projName;

        [CategoryAttribute("实名信息"), DisplayName("1-工点名称")]
        public string ProjName
        {
            get { return projName; }
            set { projName = value; }
        }

        private string projDWName;
        [CategoryAttribute("实名信息"), DisplayName("2-单位工程名称")]
        public string ProjDWName
        {
            get { return projDWName; }
            set { projDWName = value; }
        }

        private string fromD;
        [CategoryAttribute("实名信息"), DisplayName("3-开始日期")]
        public string FromDate
        {
            get { return fromD; }
            set { fromD = value; }
        }

        private string toD;
        [CategoryAttribute("实名信息"), DisplayName("4-截止日期")]
        public string ToDate
        {
            get { return toD; }
            set { toD = value; }
        }
        //public string day;
        //public string usrName;
        private int number;
        [CategoryAttribute("实名信息"), ReadOnlyAttribute(true), DisplayName("5-实名人次")]
        public int Number
        {
            get { return number; }
            set { number = value; }
        }

        public ConsLocation(double x, double y)
        {
            longitude = x;
            latitude = y;
        }
        public ConsLocation(string pn, string pdwn, double x, double y, string fd, string td, int n)
            : this(x, y)
        {
            projName = pn;
            projDWName = pdwn;
            fromD = fd;
            toD = td;
            number = n;


        }
        public override string ToString()
        {
            string result = "日期：" + fromD + " 至 " + toD + "\n";
            //result += usrName + "\n";
            result += projName + "\n" + projDWName + "\n";
            result += number + " 人次 ";
            return result;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class CConsLog
    {
        //public List<CConsRecord> consList = new List<CConsRecord>();
        /// <summary>
        /// 返回当天，最近3天，7天，30天，365天情况，注意日期格式
        /// </summary>
        /// <param name="usrName"></param>
        /// <param name="projName"></param>
        /// <param name="consDate"></param>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        //public static int findTodayCons(out string[] usrName, out string[] projName, out string[] consDate, out double[] longitude, out double[] latitude)
        //{
        //    return findConsFromDate(DateTime.Now.Date.ToString("u"),out usrName,out projName,out consDate, out longitude, out latitude);
        //}

        //public static int findLast3Cons(out string[] usrName, out string[] projName, out string[] consDate, out double[] longitude, out double[] latitude)
        //{
        //    return findConsFromDate(DateTime.Now.AddDays(-3).Date.ToString("u"), out usrName, out projName, out consDate, out longitude, out latitude);
        //}

        //public static int findLast7Cons(out string[] usrName, out string[] projName, out string[] projDWName,  out double[] longitude, out double[] latitude)
        //{
        //    return findConsFromDate(DateTime.Now.AddDays(-7).Date.ToString("u"), out usrName, out projName, out projDWName,  out longitude, out latitude);
        //}

        //public static int findLast30Cons(out string[] usrName, out string[] projName, out string[] consDate, out double[] longitude, out double[] latitude)
        //{
        //    return findConsFromDate(DateTime.Now.AddDays(-30).Date.ToString("u"), out usrName, out projName, out consDate, out longitude, out latitude);
        //}

        //public static int findLast365Cons(out string[] usrName, out string[] projName, out string[] consDate, out double[] longitude, out double[] latitude)
        //{
        //    return findConsFromDate(DateTime.Now.AddDays(-365).Date.ToString("u"), out usrName, out projName, out consDate, out longitude, out latitude);
        //}


        //private static int findConsFromDate(string date, out string[] usrName, out string[] projName, out string[] projDWName, out double[] longitude, out double[] latitude)
        //{
        //    int num = 0;
        //    usrName = projName = projDWName= null;
        //    longitude = latitude = null;

        //    //DataTable dt = CServerWrapper.findConsInfo(DateTime.Now.AddDays(-30).Date.ToString("u"));
        //    DataTable dt = CServerWrapper.findConsInfo(date);
        //    //DatabaseWrapper.PrintDataTable(dt);
        //    num = dt.Rows.Count;

        //    if (num == 0) return 0;

        //    usrName = new string[num];
        //    projName = new string[num];
        //    projDWName = new string[num];
        //    longitude = new double[num];
        //    latitude = new double[num];

        //    int i = 0;
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        usrName[i] = dr["UsrName"].ToString();
        //        projName[i] = dr["ProjectName"].ToString();
        //        projDWName[i] = dr["DwName"].ToString();
        //        longitude[i] = Convert.ToDouble(dr["Longitude"]);
        //        latitude[i] = Convert.ToDouble(dr["Latitude"]);
        //        i++;


        //    }
        //    return num;
        //}

        /// <summary>
        /// 如果绑定控件，DataTable方式可以减少数据复制，但本应用需要对数据做聚类处理，需要用上面方法做数据的初步解析
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        //private static DataTable findConsFromDate(string date)
        //{


        //    //DataTable dt = CServerWrapper.findConsInfo(DateTime.Now.AddDays(-30).Date.ToString("u"));
        //    DataTable dt = CServerWrapper.findConsInfo(date);

        //    return dt;
        //}


        /// <summary>
        /// 按照工点聚类，单位工程显示第一个
        /// </summary>
        /// <returns></returns>
        //public static List<ConsLocation> clusterConsByProj() {
        //    int numP;
        //    double[] px,py;
        //    string[] usrName;
        //    string[] projName;
        //    string[] projDWName;
        //    List<ConsLocation> ls = new List<ConsLocation>();
        //    int cn = 0;
        //    //double ax, ay;
        //    string cp = null;
        //    string fromD = DateTime.Now.AddDays(-7).ToShortDateString();
        //    string toD = DateTime.Now.ToShortDateString();

        //    //List<double> ax = new List<double>();
        //    //List<double> ay = new List<double>();

        //    numP = findLast7Cons(out usrName, out projName, out projDWName, out px, out py);
        //    //cp = projName[0];
        //    for (int i = 0; i < numP; i++) {
        //        if (px[i] < 100 || py[i] < 20) continue;
        //        if (!projName[i].Equals(cp))  // 新工点
        //        {
        //            cp = projName[i];
        //            cn = 1;
        //            ls.Add(new ConsLocation(px[i],py[i]));
        //            ls[ls.Count - 1].number = cn;
        //            ls[ls.Count - 1].ProjName = cp;
        //            ls[ls.Count - 1].FromDate = fromD;
        //            ls[ls.Count - 1].ToDate = toD;
        //            ls[ls.Count - 1].ProjDWName = projDWName[i];
        //            //if (cp.Equals("邹平梁场"))
        //            //    Console.WriteLine(ls.Last().ToString());

        //        }
        //        else  // 已存在工点
        //        {
        //            //if (cp.Equals("临淄梁场") )
        //            //    Console.WriteLine(usrName[i] + "\t" + projName[i] + "\t" + projDWName[i] + "\t" + px[i] + "\t" + py[i]);
        //            ls[ls.Count - 1].Longitude = (ls[ls.Count - 1].Longitude * cn + px[i]) / (cn + 1);
        //            ls[ls.Count - 1].Latitude = (ls[ls.Count - 1].Latitude * cn + py[i]) / (cn + 1);
        //            cn++;
        //            ls[ls.Count - 1].number = cn;
        //        }

        //    }
        //    //for (int i = 0; i < ax.Count; i++) {
        //    //    Console.WriteLine("x: {0}\t y {1}", ax[i], ay[i]);
        //    //}

        //    return ls;

        //    //new StaticCluster().clusterProcess();
        //}

        public static List<String> mLogInfo = new List<string>();
        public static List<DateTime> mLogTime = new List<DateTime>();
        public static List<double> mLogX = new List<double>();
        public static List<double> mLogY = new List<double>();


        public static int refreshMsgLog()
        {
            if (CServerWrapper.isConnected)
            {
                DataTable dt = CServerWrapper.findMsgLog(DateTime.Now.AddHours(-24));

                if (dt == null || dt.Rows.Count == 0) return 0;
                //DatabaseWrapper.PrintDataTable(dt);
                //if (mLogInfo.Count == 0 || mLogInfo.Count > 0 && dt.Rows[0]["CreateTime"].ToString() != mLogTime[0].ToString())
                //{
                    //refreshd = true;
                    mLogInfo.Clear();
                    mLogTime.Clear();
                    mLogX.Clear();
                    mLogY.Clear();
                    foreach (DataRow dr in dt.Rows)
                    {

                        mLogInfo.Add(dr["Description"].ToString());
                        mLogTime.Add(Convert.ToDateTime(dr["CreateTime"]));
                    }
                //}

                return dt.Rows.Count;
            }
            return 0;
        }
        /// <summary>
        /// 按照工点聚类，单位工程显示第一个
        /// </summary>
        /// <returns></returns>
        public static void clusterConsFromWebByProj(CRailwayScene s,string dbFile, List<ConsLocation> ls, List<CHotSpot> ls2,  string fromDate = null, string toDate = null, bool fromLocal = true)
        {
            //double cx, cy;
            double tx, ty, tz, td;

            CRailwayLine rl;
            double mileage, dis, gm;
            bool isInside;
            //string fromD = DateTime.Now.AddDays(-7).ToShortDateString();
            //string toD = DateTime.Now.ToShortDateString();

            //dt2.Columns.Add("ProjectName", typeof(string));
            //dt2.Columns.Add("DwName", typeof(string));
            //dt2.Columns.Add("Longitude", typeof(double));
            //dt2.Columns.Add("Latitude", typeof(double));
            //dt2.Columns.Add("StaffNum", typeof(int));
            // 2016-05-06 00:00:00Z
            //DataTable dt = CServerWrapper.findClusterConsByPDW(DateTime.Now.AddDays(-7).Date.ToString("u"));
            //DataTable dt = CServerWrapper.findClusterConsByPDW(fromDate, toDate);
            if (ls == null)
            {
                ls = new List<ConsLocation>();
            }
            else
            {
                ls.Clear();
            }

            if (ls2 == null)
            {
                ls2 = new List<CHotSpot>();
            }
            else
            {
                ls2.Clear();
            }

            DataTable dt = null;

            if (fromLocal)
            {
                string localPath = dbFile;
                dt = DatabaseWrapper.ExecuteDataTable(dbFile, @"SELECT * from ConsInfo ;");
            }
            else if (CServerWrapper.isConnected)
            {
                dt = CServerWrapper.findClusterConsByProj(fromDate, toDate);
            }

            if (dt == null) return ;

            foreach (DataRow dr in dt.Rows)
            {
                //GPSAdjust.bd_decrypt(Convert.ToDouble(dr["Latitude"]), Convert.ToDouble(dr["Longitude"]), out cy, out cx);

                rl = CRailwayLineList.getMileagebyGPS(Convert.ToDouble(dr["Longitude"]), Convert.ToDouble(dr["Latitude"]), out mileage, out dis, out isInside);
                CRailwayLineList.getGPSbyDKCode(rl.mDKCode, mileage, out tx, out ty, out tz, out td);
                s.mMainPath.getPathMileageByDKCode(rl.mDKCode, mileage,out gm, out dis);
                //FIXME xyn 加入两次，比较实际坐标和火星坐标的差别
                //ConsLocation cl = new ConsLocation("Mars" + dr["ProjectName"].ToString(), dr["DwName"].ToString(), Convert.ToDouble(dr["Longitude"]), Convert.ToDouble(dr["Latitude"]), //tx, ty,
                //    fromDate.Substring(0, 11), toDate.Substring(0, 11), Convert.ToInt32(dr["StaffNum"]));
                //ls.Add(cl);
                ConsLocation cl = new ConsLocation(dr["ProjectName"].ToString(), dr["DwName"].ToString(), Convert.ToDouble(dr["Longitude"]), Convert.ToDouble(dr["Latitude"]),
                    fromDate, toDate, Convert.ToInt32(dr["StaffNum"]));
                ls.Add(cl);


                ls2.Add(new CHotSpot(rl.mDKCode, mileage, tx, ty, tz, gm,dis, "Cons", cl));
                //            ls.Add(new ConsLocation(dr["ProjectName"].ToString(), dr["DwName"].ToString(), Convert.ToDouble(dr["Longitude"]), Convert.ToDouble(dr["Latitude"]),
                //fromDate.Substring(0, 11), toDate.Substring(0, 11), Convert.ToInt32(dr["StaffNum"])));
            }

            //new StaticCluster().clusterProcess();
        }
    }
}
