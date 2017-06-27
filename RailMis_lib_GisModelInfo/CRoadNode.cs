using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInfo
{

    class CRoadNode
    {
        CRailwayLine mRL;  // 所在线路
        double mLocalMileage;  // 所在线路里程
        double mLongitude, mLatitude;
        //CRoadNode mPre, mNext; // 所在线路前一个节点，所在线路后一个节点
        List<CRoadNode> mAlias = new List<CRoadNode>();  // 同名的其他线路，通过计算，误差1米认为是相同点
    }
}
