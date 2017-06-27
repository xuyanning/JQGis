using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelInfo;
using TerraExplorerX;

namespace RailwayGIS.TEProject
{
    public abstract class CTEObject
    {
        public CRailwayScene mSceneData;      
        

        public CTEScene mTEScene;

        public IPresentation66 mPresentation;
        public ITerrainLabel66 labelSign;

        public static double[] getVerArray(double[] x, double[] y, double[] z, double zoff = 0)
        {
            double[] verList = new double[x.Length * 3];

            for (int i = 0; i < x.Length; i++)
            {
                verList[3 * i] = x[i];
                verList[3 * i + 1] = y[i];
                verList[3 * i + 2] = z[i] + zoff;
            }
            return verList;
        }

        public CTEObject(CRailwayScene s, CTEScene ss  )
        {
            mSceneData = s;
            mTEScene = ss;
  
        }

        public abstract void TECreate();

        public abstract void TEUpdate();


    }
    
}
