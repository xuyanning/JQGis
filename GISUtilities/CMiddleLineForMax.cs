using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using EarthView.Globe.Core; // WorldSettings
using EarthView.Globe.MathLib; // MathEngine
using EarthView.RawTypeDef; // Vector3d

//using ModelInfo;
//using ModelInfo.Helper;

namespace GISUtilities
{
    struct LatLonAl
    {
        public double latitude; // 角度
        public double longitude; // 角度
        public double altitude; // 米

        public LatLonAl(double la, double lon, double al)
        {
            latitude = la;
            longitude = lon;
            altitude = al;
        }
    }

    struct EndPointsPair : IComparable<EndPointsPair>
    {
        public double ana;
        public double kata;
        public EndPointsPair(double a, double b)
        {
            ana = a;
            kata = b;
        }

        public int CompareTo(EndPointsPair other)
        {
            if (this.ana > other.ana)
                return 1;
            else if (this.ana < other.ana)
                return -1;
            else return 0;
        }
    }



    public class CMiddleLineForMax
    {
        const double PI = 3.1415926535897932384626433832795;
        const int END_MILEAGE = 28695; // 27106-1，第一行--0，又因为第一行是标题。
        const int NUM_BRIDGES = 15;
        const int NUM_TUNNELS = 3;
        const double Threshold = 1e-3;

        static Vector3d earthCoreSys_YAxis = new Vector3d(0, 1, 0);
        //static StreamWriter 路基中线TXT = new StreamWriter(@"..\..\..\路基中线建模坐标.txt");
        //static StreamWriter 路基起点TXT = new StreamWriter(@"..\..\..\路基起点经纬高.csv");
        //static StreamWriter 路基起止里程TXT = new StreamWriter(@"..\..\..\路基起止里程.csv");
        //static StreamWriter 路基长TXT = new StreamWriter(@"..\..\..\路基长.txt");
        //static StreamWriter 桥长TXT = new StreamWriter(@"..\..\..\桥长.txt");
        //static StreamWriter 隧道长TXT = new StreamWriter(@"..\..\..\隧道长.txt");
        //static StreamWriter 路基中心点TXT = new StreamWriter(@"..\..\..\路基中心点经纬高.csv");
        //StreamWriter 建模中线TXT = new StreamWriter(@"bridge中线建模坐标.txt");

        CRailwayScene mRWScene;
        List<OnePrj> projectList = new List<OnePrj>();
        int dbridgeCount = 0;
        int sbridgeCount = 0;
        int droadCount = 0;
        int sroadCount = 0;
        int dtunnelCount = 0;
        int stunnelCount = 0;

        /// <summary>
        /// 导出桥梁，隧道或路基的中线
        /// </summary>
        /// <param name="latlonPoints"></param>
        /// <param name="TXT"></param>
        private void exportMiddleLine(CRailwayProject prj, StreamWriter DoubleFile,StreamWriter SingleFile, string prjType)
        {
            string fileNameExt = ".max.x";
            StreamWriter TXT;
            OnePrj aprj;
            List<Vector3d> earthCoreSysPoints = new List<Vector3d>();
            List<Vector3d> dsMaxPoints = new List<Vector3d>();
            List<int> sIndex = new List<int>(); // 如果工点里程过长，拆分为不超过3.5 KM作为一段生成模型
            int startN = 0;
            // int n = 0;
            if (!prj.mIsValid )
                return;

            for ( startN = 0; startN < prj.mPath.mPointCount - 350; startN += 300)
                sIndex.Add(startN);
            sIndex.Add(startN);

            double[] m,x,y,z,d;
            bool isDouble;
            string sp;
            isDouble = prj.mPath.getMiddleLine(out m,out x, out y, out z, out d );
            if (isDouble)
            {
                TXT = DoubleFile;
                sp = "d";
            }
            else
            {
                TXT = SingleFile;
                sp = "s";
            }
            for (int j = 0; j < sIndex.Count; j++)
            {

               int fromIdx = sIndex[j];
                int toIdx;
                if (j == sIndex.Count - 1)
                    toIdx = m.Length - 1;
                else
                    toIdx = sIndex[j + 1];
                double prjLength = prj.mPath.mLength;
                switch (prjType)
                {
                    case "bridge":
                        if (isDouble)
                        {
                            dbridgeCount++;
                            aprj = new OnePrj(j, prjType, prj.ProjectName, sp + prjType + dbridgeCount + fileNameExt, prjLength,
                                x[sIndex[j]], y[sIndex[j]], z[sIndex[j]]);
                        }
                        else
                        {
                            sbridgeCount++;
                            aprj = new OnePrj(j, prjType, prj.ProjectName, sp + prjType + sbridgeCount + fileNameExt, prjLength,
                                x[sIndex[j]], y[sIndex[j]], z[sIndex[j]]);
                        }
                        projectList.Add(aprj);
                        break;
                    case "road":
                        if (isDouble)
                        {
                            droadCount++;
                            aprj = new OnePrj(j, prjType, prj.ProjectName, sp + prjType + droadCount + fileNameExt, prjLength,
                                x[sIndex[j]], y[sIndex[j]], z[sIndex[j]]);
                        }
                        else
                        {
                            sroadCount++;
                            aprj = new OnePrj(j, prjType, prj.ProjectName, sp + prjType + sroadCount + fileNameExt, prjLength,
                                x[sIndex[j]], y[sIndex[j]], z[sIndex[j]]);

                        }
                        projectList.Add(aprj);
                        break;
                    case "tunnel":
                        if (isDouble)
                        {
                            dtunnelCount++;
                            aprj = new OnePrj(j, prjType, prj.ProjectName, sp + prjType + dtunnelCount + fileNameExt, prjLength,
                                x[sIndex[j]], y[sIndex[j]], z[sIndex[j]]);
                        }
                        else
                        {
                            stunnelCount++;
                            aprj = new OnePrj(j, prjType, prj.ProjectName, sp + prjType + stunnelCount + fileNameExt, prjLength,
                                x[sIndex[j]], y[sIndex[j]], z[sIndex[j]]);

                        }
                        projectList.Add(aprj);
                        break;

                }

             
                TXT.Write(prjLength + "  ");
                TXT.Write(prj.ProjectName + "  ");
                TXT.WriteLine(j);
                earthCoreSysPoints.Clear();
                dsMaxPoints.Clear();
                for (int i = fromIdx; i <= toIdx; i++)
                {
                    double radius = WorldSettings.EquatorialRadius/* 米 */ + z[i];
                    earthCoreSysPoints.Add(MathEngine.SphericalToCartesianD(y[i], x[i], radius)); // 米
                }
                //foreach (LatLonAl p in latlonPoints)
                //{
                //    double radius = WorldSettings.EquatorialRadius/* 米 */ + p.altitude;
                //    earthCoreSysPoints.Add(MathEngine.SphericalToCartesianD(p.latitude, p.longitude, radius)); // 米
                //}

                // 经测试 地球窗口的世界坐标系原点在地球球心，x轴指向0纬0经，右手系，z轴指向北极点，测试语句：
                // Vector3 latlon_test = MathEngine.CartesianToSpherical(5001,1, 1);
                Vector3d k_prime = new Vector3d(earthCoreSysPoints[0]); k_prime = k_prime.Normalize();
                Vector3d i_prime = Vector3d.Transform(earthCoreSys_YAxis, Matrix4d.RotationZ((prj.mPath.mx[fromIdx] * PI / 180)));
                Vector3d j_prime = Vector3d.Cross(k_prime, i_prime); j_prime = j_prime.Normalize();
                Matrix4d mm = new Matrix4d();


                // mm是正交矩阵，求逆就是求转置。
                mm.M11 = i_prime.X; mm.M12 = j_prime.X; mm.M13 = k_prime.X;
                mm.M21 = i_prime.Y; mm.M22 = j_prime.Y; mm.M23 = k_prime.Y;
                mm.M31 = i_prime.Z; mm.M32 = j_prime.Z; mm.M33 = k_prime.Z;
                mm.M41 = mm.M42 = mm.M43 = 0;
                mm.M14 = mm.M24 = mm.M34 = 0;
                mm.M44 = 1;

                // 把三维中线（地球球心坐标系）上 起点之外的点 转换为 3ds max世界坐标 (起点作原点)            
                for (int i =  1; i <= toIdx-fromIdx; i++)
                {
                    Vector3d vv = new Vector3d(earthCoreSysPoints[i].X - earthCoreSysPoints[0].X, earthCoreSysPoints[i].Y - earthCoreSysPoints[0].Y, earthCoreSysPoints[i].Z - earthCoreSysPoints[0].Z);
                    dsMaxPoints.Add(Vector3d.Transform(vv, mm));
                }
                // 输出
                List<Vector3d> straight_opt = new List<Vector3d>();
                straight_opt.Add(dsMaxPoints[0]);
                straight_opt.Add(dsMaxPoints[1]);

                for (int i = 2; i < dsMaxPoints.Count; i++)
                {
                    Vector3d a = dsMaxPoints[i] - straight_opt[straight_opt.Count - 1];
                    Vector3d b = straight_opt[straight_opt.Count - 1] - straight_opt[straight_opt.Count - 2];
                    double costheta = Vector3d.Dot(a, b) / a.Length / b.Length;
                    double sintheta = Math.Sqrt(1 - costheta * costheta);
                    if (sintheta < Threshold)
                    {
                        straight_opt.Last().X = dsMaxPoints[i].X;
                        straight_opt.Last().Y = dsMaxPoints[i].Y;
                        straight_opt.Last().Z = dsMaxPoints[i].Z;
                    }
                    else
                        straight_opt.Add(dsMaxPoints[i]);
                }
                foreach (Vector3d p in straight_opt)
                {
                    TXT.Write(p.X);
                    TXT.Write(' ');
                    TXT.Write(p.Y);
                    TXT.Write(' ');
                    TXT.Write(p.Z);
                    TXT.Write(' ');
                }
                TXT.WriteLine();
            }
        }

        public CMiddleLineForMax(CRailwayScene s)
        {
//            OnePrj aprj;
//            int prjIdx = 1;
            StreamWriter sw1 = new StreamWriter("双桥中线建模坐标.txt");
            StreamWriter sw2 = new StreamWriter("双路基中线建模坐标.txt");
            StreamWriter sw3 = new StreamWriter("双隧道中线建模坐标.txt");
            StreamWriter sw4 = new StreamWriter("单桥中线建模坐标.txt");
            StreamWriter sw5 = new StreamWriter("单路基中线建模坐标.txt");
            StreamWriter sw6 = new StreamWriter("单隧道中线建模坐标.txt");

            mRWScene = s;
            foreach (CRailwayProject rp in mRWScene.mBridgeList)
            {
                if (rp.mIsValid && rp.mParentID == 0)
                {
                    exportMiddleLine(rp, sw1,sw4,"bridge");
                }
            }
            foreach (CRailwayProject rp in mRWScene.mRoadList)
            {
                if (rp.mIsValid && rp.mParentID == 0)
                {
                    exportMiddleLine(rp, sw2,sw5,"road");
                }
            }
            foreach (CRailwayProject rp in mRWScene.mTunnelList)
            {
                if (rp.mIsValid )
                {
                    exportMiddleLine(rp, sw3,sw6,"tunnel");
                }
            }
            sw1.Close();
            sw2.Close();
            sw3.Close();
            sw4.Close();
            sw5.Close();
            sw6.Close();
            // 用于产生加载列表，存储到sqlite数据库，手工导出到excel
            DatabaseWrapper.SavePrjectForLoaded(@"C:\GISData\jiqing\gisdb.db", projectList);

        }

    }
}
