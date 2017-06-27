using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.Drawing;
using ModelInfo.Helper;

namespace ModelInfo
{
    /// <summary>
    /// 多链铁路线，FIXME，目前多链首尾相连，后面需要处理中间有分叉的情况。
    /// </summary>
    public class CRailwayLineList
    {
        public static List<CRailwayLine> mLineList = new List<CRailwayLine>();
        public static List<CRailwayLine> mRightLineList = new List<CRailwayLine>();

        //public static ShortestPathFloyd mPath, mRightPath;

        public static MGraph gRealConnection; //用于生成漫游路径
        public static MGraph gMileageConnection; // 用于计算里程

        /// <summary>
        /// 由数据库或远程服务器读入三维中线的数据,这儿无效，在工具中使用，
        /// </summary>
        /// <param name="fileName"></param>
        //public static void CreateLinelistFromExcel(string dbPath)
        //{
        //    double[] m, x, y, z;
        //    double fromMeter, toMeter;
        //    //double fromX, toX, fromY, toY;
        //    int chainIndex;
        //    int chainType;
        //    string dkcode, dkcode2;
        //    //bool isRight = false;
        //    //bool isDouble = true;
        //    int count;

        //    DataTable dt1, dt2, dt3;

        //    //本地sqlite数据库读取里程数据
        //    //#if DEBUG
        //    //            Helper.LogHelper.WriteLog("SQlite Database" + dbPath);
        //    //#endif

        //    dt1 = ExcelWrapper.LoadDataTableFromExcel(dbPath, @"select chainIndex, fromMeter, toMeter,DKCode,DKCode2,chainType from [ChainInfo$] order by chainIndex");
        //    dt2 = ExcelWrapper.LoadDataTableFromExcel(dbPath, @"select fromChain, toChain from [ExtraConnection$] ");
        //    List<OneConnection> connectionList = new List<OneConnection>();
        //    foreach (DataRow dr in dt2.Rows)
        //    {
        //        OneConnection aCon = new OneConnection();
        //        aCon.fromIndex = Convert.ToInt32(dr["fromChain"]);
        //        aCon.toIndex = Convert.ToInt32(dr["toChain"]);
        //        //aCon.isRealConnect = Convert.ToInt32(dr["isConnect"])==0 ? true:false;
        //        //aCon.mileageOffset = Convert.ToDouble(dr["mileageOffset"]);
        //        connectionList.Add(aCon);
        //    }
        //    //int index = 0;

        //    foreach (DataRow dr in dt1.Rows)
        //    {
        //        chainIndex = Convert.ToInt32(dr["chainIndex"]);
        //        fromMeter = Convert.ToDouble(dr["fromMeter"]);
        //        toMeter = Convert.ToDouble(dr["toMeter"]);
        //        dkcode = (string)dr["DKCode"];
        //        dkcode2 = dr["DKCode2"].ToString();
        //        chainType = Convert.ToInt32(dr["chainType"]);
        //        //  isReverse = Convert.ToBoolean(dr["IsReverse"]);
        //        string strOrder = (fromMeter < toMeter) ? "" : " desc ";
        //        string tableName = "sheet" + chainIndex;
        //        dt3 = ExcelWrapper.LoadDataTableFromExcel(dbPath, @"select Mileage, Longitude,Latitude,Altitude from [" + tableName + "$] order by mileage " + strOrder);
        //        count = dt3.Rows.Count;
        //        if (count < 2) continue;

        //        m = new double[count];
        //        x = new double[count];
        //        y = new double[count];
        //        z = new double[count];
        //        //xMars = new double[count];
        //        //yMars = new double[count];
        //        int j = 0;
        //        foreach (DataRow dr2 in dt3.Rows)
        //        {

        //            m[j] = Math.Abs(Convert.ToDouble(dr2["Mileage"]) - fromMeter);
        //            x[j] = Convert.ToDouble(dr2["Longitude"]);
        //            y[j] = Convert.ToDouble(dr2["Latitude"]);
        //            z[j] = Convert.ToDouble(dr2["Altitude"]);
        //            j++;
        //            //xMars[j] = Convert.ToDouble(dr2["LongitudeMars"]);
        //            //yMars[j] = Convert.ToDouble(dr2["LatitudeMars"]);
        //        }
        //        mLineList.Add(new CRailwayLine(chainIndex, dkcode, dkcode2, fromMeter, toMeter, chainType, count, m, x, y, z));
        //    }


        //    gRealConnection = new MGraph();
        //    gMileageConnection = new MGraph();

        //    ShortestPathFloyd.initPathGraph(mLineList, gRealConnection, null);
        //    ShortestPathFloyd.initPathGraph(mLineList, gMileageConnection, connectionList);
        //}

        /// <summary>
        /// 由数据库或远程服务器读入三维中线的数据
        /// </summary>
        /// <param name="fileName"></param>
        public static void CreateLinelistFromSqlite(string dbPath)
        {
            double[] m, x, y, z;
            double fromMeter, toMeter;
            //double fromX, toX, fromY, toY;
            int chainIndex;
            int chainType;
            string dkcode, dkcode2;
            //bool isRight = false;
            //bool isDouble = true;
            int count;

            DataTable dt1, dt2, dt3;

            //本地sqlite数据库读取里程数据
            //#if DEBUG
            //            Helper.LogHelper.WriteLog("SQlite Database" + dbPath);
            //#endif

            dt1 = DatabaseWrapper.ExecuteDataTable(dbPath, @"select chainIndex, fromMeter, toMeter,DKCode,DKCode2,chainType from ChainInfo order by chainIndex; ");
            dt2 = DatabaseWrapper.ExecuteDataTable(dbPath, @"select fromChain, toChain from ExtraConnection; ");

            //dt1 = ExcelWrapper.LoadDataTableFromExcel(dbPath, @"select chainIndex, fromMeter, toMeter,DKCode,DKCode2,chainType from [ChainInfo$] order by chainIndex");
            //dt2 = ExcelWrapper.LoadDataTableFromExcel(dbPath, @"select fromChain, toChain from [ExtraConnection$] ");
            List<OneConnection> connectionList = new List<OneConnection>();
            foreach (DataRow dr in dt2.Rows)
            {
                OneConnection aCon = new OneConnection();
                aCon.fromIndex = Convert.ToInt32(dr["fromChain"]);
                aCon.toIndex = Convert.ToInt32(dr["toChain"]);
                //aCon.isRealConnect = Convert.ToInt32(dr["isConnect"])==0 ? true:false;
                //aCon.mileageOffset = Convert.ToDouble(dr["mileageOffset"]);
                connectionList.Add(aCon);
            }
            //int index = 0;

            foreach (DataRow dr in dt1.Rows)
            {
                chainIndex = Convert.ToInt32(dr["chainIndex"]);
                fromMeter = Convert.ToDouble(dr["fromMeter"]);
                toMeter = Convert.ToDouble(dr["toMeter"]);
                dkcode = (string)dr["DKCode"];
                if (dr["DKCode2"] == null)
                    dkcode2 = "";
                else
                    dkcode2 = dr["DKCode2"].ToString();
                chainType = Convert.ToInt32(dr["chainType"]);
                //  isReverse = Convert.ToBoolean(dr["IsReverse"]);
                string strOrder = (fromMeter < toMeter) ? "" : " desc ";
                //string tableName = "sheet" + chainIndex;
                dt3 = DatabaseWrapper.ExecuteDataTable(dbPath, @"select Mileage, Longitude,Latitude,Altitude from MileageInfo where MileagePrefix=" + chainIndex + "  order by mileage " + strOrder);
                count = dt3.Rows.Count;
                if (count < 2) continue;

                m = new double[count];
                x = new double[count];
                y = new double[count];
                z = new double[count];
                //xMars = new double[count];
                //yMars = new double[count];
                int j = 0;
                foreach (DataRow dr2 in dt3.Rows)
                {

                    m[j] = Math.Abs(Convert.ToDouble(dr2["Mileage"]) - fromMeter);
                    x[j] = Convert.ToDouble(dr2["Longitude"]);
                    y[j] = Convert.ToDouble(dr2["Latitude"]);
                    z[j] = Convert.ToDouble(dr2["Altitude"]);
                    j++;
                    //xMars[j] = Convert.ToDouble(dr2["LongitudeMars"]);
                    //yMars[j] = Convert.ToDouble(dr2["LatitudeMars"]);
                }
                mLineList.Add(new CRailwayLine(chainIndex, dkcode, dkcode2, fromMeter, toMeter, chainType, count, m, x, y, z));
            }

            gRealConnection = new MGraph();
            gMileageConnection = new MGraph();

            ShortestPathFloyd.initPathGraph(mLineList, gRealConnection, null);
            ShortestPathFloyd.initPathGraph(mLineList, gMileageConnection, connectionList);
        }

        public static void testMainLine()
        {
            CSubPath p = new CSubPath(gMileageConnection, "GSJDK", 426000, "JQDK", 17600, 10);
            Console.WriteLine(p);
        }

        //public static void createFloydPath(string dbPath)
        //{
        //    //CSubPath path = new CSubPath(CRailwayLineList.gMileage, "DK", 301610, "改DK", 17800, 10);
        //    List<string> dcode = new List<string>();

        //    dcode.Add("青连改DK");
        //    dcode.Add("改DK");
        //    //dcode.Add("DK");
        //    //dcode.Add("JQDK");
        //    //dcode.Add("DK");
        //    //dcode.Add("右改DK");
        //    List<double> dmile = new List<double>();
        //    dmile.Add(22693.74128);
        //    dmile.Add(25800);
        //    //dmile.Add(275950);
        //    //dmile.Add(67580);
        //    //dmile.Add(299000);
        //    //dmile.Add(17600);
        //    CSubPath path = new CSubPath(gMileageConnection, dcode, dmile, 10);
        //    path.createHeightSample(dbPath);
        //    //path.outputPathInfo();

        //    //mPath = new ShortestPathFloyd(mLineList,connectionList);
        //    //mRightPath = new ShortestPathFloyd(mRightLineList);
        //}

        /// <summary>
        /// 通过索引定位铁路线
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public static CRailwayLine getRailwayLineByIndex(int idx)
        {
            foreach (CRailwayLine rl in mLineList)
            {
                if (rl.mIndex == idx)
                    return rl;
            }
            //foreach (CRailwayLine rl in mRightLineList)
            //{
            //    if (rl.mIndex == idx)
            //        return rl;
            //}
            return null;
        }

        /// <summary>
        /// 根据DKCode定位铁路线
        /// </summary>
        /// <param name="dkcode"></param>
        /// <param name="mileage"></param>
        /// <returns></returns>
        public static CRailwayLine getRailwayLineByDKCode(string dkcode, double mileage)
        {
            foreach (CRailwayLine rl in mLineList)
            {
                double dis;
                if (rl.getLocalMileageByDKMileage(dkcode, mileage, out dis))
                {
                    return rl;
                }
            }
            //foreach (CRailwayLine rl in mRightLineList)
            //{
            //    double dis;
            //    if (rl.getLocalMileageByDKMileage(dkcode, mileage, out dis))
            //    {
            //        return rl;
            //    }
            //}
            return null;

        }

        public static bool getGPSbyDKCodeFromDB(string dk, double meter, out double x, out double y, out double z)
        {
            x = y = z = 0;
            int chainIdx;
            string str = "select chainIndex ,fromMeter, toMeter from chaininfo where dkcode = '" + dk + "' or dkcode2 ='" + dk + "'";
            DataTable dt = DatabaseWrapper.ExecuteDataTable(@"C:\gisdata\jiqing\gisdb.db", str);
            foreach (DataRow dr in dt.Rows)
            {
                if ((Convert.ToDouble(dr["fromMeter"]) - meter) * (Convert.ToDouble(dr["toMeter"]) - meter) <= 0)
                {
                    double h = meter + 50;
                    double l = meter - 50;
                    double tm, tx, ty, tz;
                    tm = tx = ty = tz = 0;
                    chainIdx = Convert.ToInt32(dr["chainIndex"]);
                    str = "select  * from mileageInfo where mileagePrefix =" + chainIdx + " and mileage >= " + l + " and mileage <= " + h;
                    DataTable dt1 = DatabaseWrapper.ExecuteDataTable(@"C:\gisdata\jiqing\gisdb.db", str);
                    foreach (DataRow dr1 in dt1.Rows)
                    {
                        tm += Convert.ToDouble(dr1["mileage"]);
                        tx += Convert.ToDouble(dr1["longitude"]);
                        ty += Convert.ToDouble(dr1["latitude"]);
                        tz += Convert.ToDouble(dr1["altitude"]);
                    }
                    double mm = meter / tm;
                    x = tx * mm;
                    y = ty * mm;
                    z = tz * mm;
                    //Console.WriteLine(x + "\t" + y + "\t" + z + "\t" + meter);
                    return true;
                }
            }
            return false;

        }

        /// <summary>
        /// 根据dk码以及里程，例如DIK12+23.4，获取经纬度高度、朝向坐标
        /// </summary>
        /// <param name="dk"></param>
        /// <param name="meter"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="dir"></param>
        /// <returns>DKCode是否有效</returns>
        public static bool getGPSbyDKCode(string dk, double meter, out double x, out double y, out double z, out double dir)
        {
            x = y = z = dir = 0;
            double dis, temp;

            // 判断左右线路
            foreach (CRailwayLine rw in mLineList)
            {
                if (rw.getLocalMileageByDKMileage(dk, meter, out dis))
                {
                    rw.getGPSbyDKMileage(meter, out x, out y, out z, out dir, out temp);
                    return true;
                }
            }
            //foreach (CRailwayLine rw in mRightLineList)
            //{
            //    if (rw.getLocalMileageByDKMileage(dk, meter, out dis))
            //    {
            //        rw.getGPSbyDKMileage(meter, out x, out y, out z, out dir, out temp);
            //        return true;
            //    }
            //}
            //Console.WriteLine(dk + " " + meter + "is not valid !");
            return false;
        }

        public static bool getGPSbyDKCode(string dkcode, out double x, out double y, out double z, out double dir)
        {
            x = y = z = dir = 0;
            string dk;
            double meter;
            parseDKCode(dkcode, out dk, out meter);
            return getGPSbyDKCode(dk, meter, out x, out y, out z, out dir);

        }


        ///// <summary>
        ///// 获取最小len
        ///// </summary>
        //public double getMinLen(double len_start, double len_mid, double len_end)
        //{
        //    double len = Math.Min(Math.Min(len_start, len_mid), len_end);
        //    return len;

        //}

        ///// <summary>
        ///// 根据dk码以及里程，例如DIK12+23.4，获取经纬度高度、朝向坐标
        ///// </summary>
        ///// <param name="dk"></param>
        ///// <param name="meter"></param>
        ///// <param name="x"></param>
        ///// <param name="y"></param>
        ///// <param name="z"></param>
        ///// <param name="dir"></param>
        ///// <returns></returns>
        //public bool getGPSAndGlobalMileageByDKCode(string dk, double meter, out double x, out double y, out double z, out double dir, out double globalmeter)
        //{
        //    x = y = z = dir = 0;
        //    double dis;
        //    globalmeter = 0;
        //    foreach (CRailwayLine rw in mLineList)
        //    {
        //        if (rw.isInSide(dk, meter, out dis))
        //        {
        //            rw.getGPSbyMileage(meter, out x, out y, out z, out dir);
        //            globalmeter += dis;
        //            return true;

        //        }
        //        globalmeter += rw.mLength;
        //    }
        //    //Console.WriteLine(dk + " " + meter + "is not valid !");
        //    return false;
        //}



        /// <summary>
        /// 根据经纬度，获取里程，如果经纬度不在铁路线上，定位铁路线上最近的一个点，返回该段铁路、该点里程以及距离
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="dis"></param>
        /// <returns></returns>
        public static CRailwayLine getMileagebyGPS(double x, double y, out double mileage, out double dis, out bool isInside)
        {
            dis = -1;
            //double mi;
            mileage = -1;
            double mi;
            double min = 100;
            CRailwayLine mrw = null;
            isInside = false;
            //bool isInside2;
            foreach (CRailwayLine rw in mLineList)
            {
                rw.getDKMileagebyGPS(x, y, out mi, out dis);

                if (mrw == null)
                {
                    min = dis;
                    mileage = mi;
                    mrw = rw;
                    //isInside = isInside2;
                }
                else if (dis < min)
                {
                    min = dis;
                    mileage = mi;
                    mrw = rw;
                    //isInside = isInside2;
                }
                //if (isInside2 && dis < 10)
                //    break;

            }
            dis = min;
            //mileage 
            return mrw;
        }

        ///// <summary>
        ///// 根据经纬度，获取里程，如果经纬度不在铁路线上，定位铁路线上最近的一个点，返回该段铁路、该点里程以及距离
        ///// </summary>
        ///// <param name="x"></param>
        ///// <param name="y"></param>
        ///// <param name="dis"></param>
        ///// <returns></returns>
        //public static CRailwayLine getMileagebyMars(double x, double y, out double mileage, out double dis, out bool isInside)
        //{
        //    dis = -1;
        //    //double mi;
        //    mileage = -1;
        //    double mi;
        //    double min = 100;
        //    CRailwayLine mrw = null;
        //    isInside = false;
        //    bool isInside2;
        //    foreach (CRailwayLine rw in mLineList)
        //    {
        //        isInside2 = rw.getMileagebyMars(x, y, out mi, out dis);
        //        if (mrw == null)
        //        {
        //            min = dis;
        //            mileage = mi;
        //            mrw = rw;
        //            isInside = isInside2;
        //        }
        //        else if (dis < min)
        //        {
        //            min = dis;
        //            mileage = mi;
        //            mrw = rw;
        //            isInside = isInside2;
        //        }
        //        if (isInside2 && dis < 10)
        //            break;

        //    }
        //    dis = min;
        //    //mileage 
        //    return mrw;
        //}


        /// <summary>
        /// 解析dkcode为code与里程，输入串规则“**K123 + 456.78”，根据输入铁路链，同时验证该输入语法及数值是否合法。
        /// </summary>
        /// <param name="inputCode"></param>
        /// <param name="dkCode"></param>
        /// <param name="meter"></param>
        /// <param name="validateMeter"></param>
        /// <returns></returns>
        public static bool parseDKCode(string inputCode, out string dkCode, out double meter, double validateMeter = -1)
        {
            bool isValid = true;
            string ts;
            dkCode = "DK";
            meter = 0;
            try
            {
                int indexDK = inputCode.LastIndexOf("DK");
                if (indexDK >= 0)
                {
                    dkCode = inputCode.Substring(0, indexDK + 2);
                    ts = inputCode.Substring(indexDK + 2);
                }
                else
                {
                    LogHelper.WriteLog("里程格式错误" + inputCode);
                    return false;
                }

                string[] ss = ts.Split(new char[1] { '+' });
                if (ss.Length != 2)
                {
                    LogHelper.WriteLog("里程格式错误" + inputCode);
                    return false;
                }
                meter = Double.Parse(ss[0].Trim()) * 1000 + Double.Parse(ss[1].Trim());
                if (validateMeter >= 0 && Math.Abs(meter - validateMeter) > 0.1)
                    isValid = false;
            }
            catch (Exception e)
            {
                Console.WriteLine("里程格式错误" + inputCode);
                isValid = false;
            }

            return isValid;
        }


        /// <summary>
        ///  合并code与里程为dkcode
        /// </summary>
        public static string CombiDKCode(string dkCode, double mileage)
        {
            string outputCode = "DK0+0";
            try
            {
                int km = (int)(mileage / 1000);
                outputCode = dkCode + km + "+" + Math.Round(mileage - km * 1000, 2);
            }
            catch (Exception e)
            {
                Console.WriteLine("Format error" + dkCode + mileage);
            }
            return outputCode;

        }


        ///// <summary>
        ///// 获取总里程
        ///// </summary>
        ///// <param name="dkcode"></param>
        ///// <param name="dkmeter"></param>
        ///// <returns></returns>
        //public double getGlobalMileage(string dkcode, double dkmeter)
        //{
        //    double gm = 0;
        //    double dis = 0;
        //    bool isfind = false;
        //    foreach (CRailwayLine rw in mLineList)
        //    {
        //        if (rw.isInSide(dkcode, dkmeter, out dis))
        //        {

        //            gm += dis;
        //            isfind = true;
        //            break;

        //        }
        //        gm += rw.mLength;
        //    }
        //    if (isfind)
        //        return gm;
        //    else
        //        return -1;
        //}
        //public double getGlobalMileageByGPS(double longitude, double latitude, out double offsetd, out bool isInside)
        //{
        //    CRailwayLine rl;
        //    double m;
        //    rl = getMileagebyGPS(longitude, latitude, out m, out offsetd, out isInside);
        //    if (rl != null)
        //        return getGlobalMileage(rl.mDKCode, m);
        //    else
        //        return -1;
        //}

        //public double getGlobalMileageByMars(double longitude, double latitude, out double offsetd, out bool isInside)
        //{
        //    CRailwayLine rl;
        //    double m;
        //    rl = getMileagebyMars(longitude, latitude, out m, out offsetd, out isInside);
        //    if (rl != null)
        //        return getGlobalMileage(rl.mDKCode, m);
        //    else
        //        return -1;
        //}

        //public bool getDKCodebyGlobalMileage(double gmileage, out string dkcode, out double dkmileage)
        //{
        //    dkcode = "";
        //    dkmileage = 0;
        //    foreach (CRailwayLine rw in mLineList)
        //    {
        //        if (rw.mLength > gmileage)
        //        {
        //            dkcode = rw.mDKCode;
        //            if (rw.mIsReverse)
        //                dkmileage = rw.mStart - gmileage;
        //            else
        //                dkmileage = rw.mStart + gmileage;
        //            return true;
        //        }
        //        gmileage -= rw.mLength;
        //    }
        //    if (mLineList != null)
        //    {
        //        dkcode = mLineList.Last().mDKCode;
        //        dkmileage = mLineList.Last().mEnd;
        //    }
        //    return false;
        //}
    }
}
