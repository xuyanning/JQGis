using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel; 

namespace ModelInfo
{
    public class CHotSpot
    {
        string dkcode;
        public string DKCode
        {
            get { return dkcode; }
        }

        double mileage;
        public double Mileage
        {
            get { return mileage; }
        }

        double mX;
        public double Longitude
        {
            get { return mX; }
        }

        double mY;
        public double Latitude
        {
            get { return mY; }
        }

        double mZ;
        public double Altitude
        {
            get { return mZ; }
        }

        double gm;
        public double GlobalMileage
        {
            get { return gm; }
        }

        string mType; //   Project Firm Cons
        public string ObjectType
        {
            get { return mType; }
        }
        public double mOffsetDistance;

        Object mo;
        public Object ObjectRef
        {
            get { return mo; }
        }

        public CHotSpot(string dk, double  mi, double x, double y, double z, double gMileage,double distance, string mt, Object o){
            dkcode = dk;
            mileage = mi;
            mX = x;
            mY = y;
            mZ = z;
            gm = gMileage;
            mOffsetDistance = distance;
            mType = mt;
            mo = o;

        }
        
        public double getUTMDistance(double x, double y)
        {
            return CoordinateConverter.getUTMDistance(mX, mY, x, y);
        }
        
    }
}
