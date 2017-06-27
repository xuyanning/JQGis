using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelInfo;
using TerraExplorerX;

namespace RailwayGIS.TEProject
{
    public class CTEStation : CTEObject
    {
        //CRailwayScene mSceneData;
        //CRailwayProject st = null;
        //List<CRailwayProject> mStList = null;

        public CTEStation(CRailwayScene s, CTEScene ss)
            : base(s, ss)
        {
            //mSceneData = s;
            //mStList = s.mStationList;
        }
        public override void TECreate()
        {
            //var sgworld = new SGWorld66();
            //if (string.IsNullOrEmpty(mGroupIDStatic))
            //    mGroupIDStatic = sgworld.ProjectTree.CreateGroup("Station");
            //double cx, cy;
            
            ////double[] p = new double[] { -1, 1, 0, 1, 1, 0, 1, -1, 0, -1, -1, 0 };
            ////IPosition66 cp;

            ////ITerrainLabel66 iLabel;
            ////double t[10];


            ////var branch = sgworld.ProjectTree.CreateGroup("站房工程");
            //foreach (CRailwayProject st in mStList)
            //{

            //    // 中点坐标
            //    if (!st.mIsValid) { 
            //        Console.WriteLine(st.ProjectName + "没有坐标");
            //        continue;
            //    }
            //    // 中点偏移50米坐标
            //    CoordinateConverter.LatLonOffest(st.CenterLatitude, st.CenterLongitude, st.mHeading_Mid , st.mDirection, 50, out cy, out cx);


            //    double[] poly;
            //    poly = CoordinateConverter.GPSRectangle(cx, cy, st.mAltitude_Mid, 200, 50, st.mHeading_Mid - 90);
 
            //    IGeometry buildingShp = null;
            //    ILinearRing cSQRing = sgworld.Creator.GeometryCreator.CreateLinearRingGeometry(poly);
            //    buildingShp = sgworld.Creator.GeometryCreator.CreatePolygonGeometry(cSQRing, null);
            //    var model = sgworld.Creator.CreateBuilding(buildingShp, 15, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, mGroupIDStatic, st.ProjectName);

            //    string fileName = CGisDataSettings.gDataPath + @"\Data\Textures\Sky_w_tan.jpg";
            //    model.Sides.Texture.FileName = CGisDataSettings.gDataPath + @"Common\Textures\Sky_w_tan.jpg";
            //    model.Roof.Texture.FileName = CGisDataSettings.gDataPath + @"Common\Textures\roof.png";



            //}
            ////catch (Exception ex)
            ////{
            ////    Console.WriteLine(ex.Message.ToString());
            ////    Console.WriteLine("Station Creation Error");
            ////}

        }

        public override void TEUpdate()
        {

        }
    }
}
