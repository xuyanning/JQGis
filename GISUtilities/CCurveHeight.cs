using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GISUtilities
{
    public class CCurveHeight
    {
        // 改dk17600~右JQDK26+621.08
        public double[] m = { 11,180,340,474.862,751.254};
        public double[] h = { 201.53, 202.63, 206.36, 210.92, 216.9};
        public double[] r = { 0, 3000, 3000,3500};
        public double[] i1 = {0, 0, 0, 0 };
        public double[] i2 = {0, 0, 0, 0 };
        public double[] sm = {0, 0, 0, 0 };
        public double[] em = {0, 0, 0, 0 };
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
