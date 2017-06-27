using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace GISUtilities
{
    public class CCurveHeight
    {
        // 改dk17600~右JQDK26+621.08
        public string[] dkmileage;
        public double[] m;// = { 11,180,340,474.862,751.254};
        public double[] h;// = { 201.53, 202.63, 206.36, 210.92, 216.9};
        public double[] r; // = { 0, 3000, 3000,3500};
        public double[] i1;// = {0, 0, 0, 0 }; 前坡度
        public double[] i2;// = {0, 0, 0, 0 }; 后坡度
        public double[] sm;// = {0, 0, 0, 0 };  起始变高
        public double[] em;// = {0, 0, 0, 0 }; 终止变高
        public void test() {
            for (int i = 1; i<= 3; i++)
            {
                mainParams(i);
            }
            double h1, h2;
            h1 = getHeight(340, 2);
            h2 = getHeight(480,3);
            Console.WriteLine(h1 + "\t" + h2);
        }

        public void getHeightList()
        {
            DataTable dt1;
            string code;
            double mileage;
            double dis;
            int count;
            int i = 0;
            dt1 = ExcelWrapper.LoadDataTableFromExcel(@"d:\pd.xlsx", @"select mileage, altitude, radius from [sheet1$] order by id");
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
            //foreach (DataRow dr in dt1.Rows)
            //{
            //    dkmileage[i] = dr["mileage"].ToString();
            //    CRailwayLineList.parseDKCode(dkmileage[i], out code, out mileage);
            //    path.getPathMileageByDKCode( code,  mileage,out m[i],out dis);
            //    h[i] = Convert.ToDouble(dr["altitude"]);
            //    r[i] = Convert.ToDouble(dr["radius"]);
            //    i++;

            //}
            for (i = 1; i< m.Length - 1; i++)
            {
                mainParams(i);
            }
            int j = 1;
            //for (i = 0; i < path.mm.Length; i++)
            //{
            //    if (path.mm[i] < sm[j])
            //    {
            //    }
            //}
        }

        public void mainParams(int id)
        {
            i1[id] = (h[id] - h[id-1]) / (m[id] - m[id-1]);
            i2[id] = (h[id + 1] - h[id]) / (m[id + 1] - m[id]);
            double t = r[id] * Math.Abs(i2[id ] - i1[id ]) / 2;
            double e = t * t / r[id] / 2;
            sm[id] = m[id] - t;
            em[id] = m[id] + t;
        }

        public double getHeight(double mileage,int id)
        {
            double res = 0;
            if (mileage < sm[id])
            {
                res = h[id] + (mileage - m[id]) * i1[id];
            }
            else if (mileage > em[id])
            {
                res = h[id] + (mileage - m[id]) * i2[id];
            }
            else
            {
                res = h[id] + (sm[id] - m[id]) * i1[id] + (mileage - sm[id]) * i1[id]
                    + Math.Sign(i2[id ] - i1[id]) * (mileage - sm[id]) * (mileage - sm[id]) / r[id] / 2;
            }
            return res;
        }
    }
}
