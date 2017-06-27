using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelInfo;
using TerraExplorerX;

namespace RailwayGIS.TEProject
{
    public abstract class CTERWItem
    {
        public CRailwayScene mSceneData;      
        

        public CTEScene mTEScene;

        public List<ITerraExplorerMessage66> mMSG = new List<ITerraExplorerMessage66>();
        // .ToString("u")
        //public DateTime fromDate = DateTime.Now.AddDays(-7);
        //public DateTime toDate = DateTime.Now;
        //private CRailwayScene s;
        //private CRWTEStandard t;

        public CTERWItem(CRailwayScene s, CTEScene ss  )
        {
            mSceneData = s;
            mTEScene = ss;
  
        }

        public abstract void TECreate();

        public abstract void TEUpdate();


    }
    
}
