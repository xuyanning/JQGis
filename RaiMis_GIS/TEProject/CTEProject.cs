using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelInfo;
using TerraExplorerX;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace RailwayGIS.TEProject
{
    /// <summary>
    /// 工点信息可视化，目前仅仅是语法正确了，逻辑很多待修正，例如不支持多链。
    /// </summary>
    public class CTEProject: CTEObject
    {
        public CRailwayProject proj;
        public ITerrainPolyline66 polylineDone;
        public ITerrainPolyline66 polylineTodo;


        //public static string mGroupIDStatic = null;
        public static string mGroupIDDynamic = null;

        private SGWorld66 sgworld;

        public CTEProject(CRailwayProject p, CRailwayScene s, CTEScene ss )
            : base(s, ss)
        {

            proj = p;           
            
            if (!proj.mIsValid)
                return;
            double lat, lon, z, dir;
            //double cx, cy;
            
            sgworld = new SGWorld66();
            double[] cVerticesArray = null;
            //double[] cVA2 = null;
            int count;

            proj.getSpecialPoint(1, out lon, out lat, out z, out dir);
            //double[] xx,yy,zz,ddir;

            //CoordinateConverter.LatLonOffest(lat, lon, dir, 90, 20, out cy, out cx);

            var cPos2 = sgworld.Creator.CreatePosition(lon, lat, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -75, 0, 2000);
            //sgworld.Creator.CreateCircle(cPos2, 140, sgworld.Creator.CreateColor(127, 127, 127, 127), sgworld.Creator.CreateColor(127, 127, 127, 127), mGroupIDStatic);
            labelSign = sgworld.Creator.CreateTextLabel(cPos2, proj.ProjectName, CRWTEStandard.mLabelStyleL3, mGroupIDDynamic, "Prj|" + proj.ProjectName);
            //ilabel.Message = sgworld.Creator.CreateMessage();
            labelSign.ImageFileName = CGisDataSettings.gDataPath + @"Common\progress\Pie" + proj.AvgProgress + ".png";
            labelSign.Visibility.MaxVisibilityDistance = 20000;
            labelSign.Style.LineToGround = true;
            //labelSign.Message.MessageID = sgworld.Creator.CreateMessage(MsgTargetPosition.MTP_POPUP, proj.ToString(), MsgType.TYPE_TEXT, true).ID;

            if (proj.mAvgProgress < 0.01)
                return;

            //count = proj.getSubLine(out cVerticesArray);
            double[] xx, yy, zz;
            count = proj.getMiddleLine(out xx, out yy, out zz);
            cVerticesArray = getVerArray(xx, yy, zz);
            //proj.getMiddleLine(out cVA2);

            if (count > 1)
            {

                polylineDone = sgworld.Creator.CreatePolylineFromArray(cVerticesArray, sgworld.Creator.CreateColor((int)(255 * proj.mAvgProgress),(int)(255 -255 * proj.mAvgProgress) , 0, 255),
                    AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, mGroupIDDynamic, "Done" + proj.ProjectName);
                //sgworld.Creator.CreatePolylineFromArray(cVA2, sgworld.Creator.CreateColor((int)(255 ), (int)(255 ), 255, 255),
                //    AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, mGroupIDDynamic, "Middle" + proj.ProjectName);
                polylineDone.Spline = true;
                polylineDone.Visibility.MaxVisibilityDistance = 10000000;
                polylineDone.Visibility.MinVisibilityDistance = 100000;
                if (proj is CContBeam)
                    polylineDone.LineStyle.Width = -5;
                else
                    polylineDone.LineStyle.Width = -2;
                
            }
            //count = proj.getSubLine(out cVerticesArray);

            //polylineTodo = sgworld.Creator.CreatePolylineFromArray(cVerticesArray, sgworld.Creator.CreateColor(255, 255, 0, 255),
            //    AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, mGroupIDDynamic, "ToDo" + proj.ProjectName);
            //polylineTodo.Spline = true;
            //polylineTodo.Visibility.MaxVisibilityDistance = 10000000;
            //polylineTodo.Visibility.MinVisibilityDistance = 10;
            //polylineTodo.LineStyle.Width = -2;



        }

        public override void TECreate()
        {
            //createProjectLabel();
            //createProjectLineAndLabel();
            //createTest();
            //createMiddles();
            //createCircle();
            //createRibbon();
        }

        public override void TEUpdate()
        {
            var sgworld = new SGWorld66();
            //var branch = sgworld.ProjectTree.FindItem(groupID);
            //if (!string.IsNullOrEmpty(branch))
            //    sgworld.ProjectTree.DeleteItem(branch);
            sgworld.ProjectTree.DeleteItem(mGroupIDDynamic);
            mGroupIDDynamic = null;
            //createProjectLabel();
            //createCircle();
            //createRibbon();
        }

        #region No In Use Now  xyn


        //private void displayProject(CRailwayProject item)
        //{

        //    double lat, lon, z, dir;
        //    double cx, cy;

        //    double[] cVerticesArray = null;
        //    int count;
        //    ITerrainPolyline66 polyline1;
        //    //var cPos = sgworld.Creator.CreatePosition(item.CenterLongitude, item.CenterLatitude, 10, AltitudeTypeCode.ATC_TERRAIN_RELATIVE);
        //    //sgworld.Creator.CreateLabel(cPos, item.ProjectName, CGisDataSettings.gDataPath + @"\Data\Textures\桥梁.png", cLabelStyle, mGroupIDStatic, item.ProjectName);

        //    if (!CRailwayLineList.getGPSbyDKCode(item.mMidDKCode, item.CenterMileage, out lon, out lat, out z, out dir))
        //    {
        //        Console.WriteLine(item.ProjectName + " 工点 坐标错误");
        //        return;
        //    }
        //    //item.getFXProgressByDate(mTEScene.toDate);
        //    count = item.getDoneSubLine(out cVerticesArray);

        //    polyline1 = sgworld.Creator.CreatePolylineFromArray(cVerticesArray, sgworld.Creator.CreateColor(0, 255, 0, 255),
        //        AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, mGroupIDDynamic, item.ProjectName + "Done");
        //    polyline1.Spline = true;
        //    polyline1.Visibility.MaxVisibilityDistance = 10000000;
        //    polyline1.Visibility.MinVisibilityDistance = 150;
        //    polyline1.LineStyle.Width = -5;

        //    count = item.getToDoSubLine(out cVerticesArray);

        //    polyline1 = sgworld.Creator.CreatePolylineFromArray(cVerticesArray, sgworld.Creator.CreateColor(255, 255, 0, 255),
        //        AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, mGroupIDDynamic, item.ProjectName + "ToDo");
        //    polyline1.Spline = true;
        //    polyline1.Visibility.MaxVisibilityDistance = 10000000;
        //    polyline1.Visibility.MinVisibilityDistance = 150;
        //    polyline1.LineStyle.Width = -5;

        //    CoordinateConverter.LatLonOffest(lat, lon, dir, 90, 20, out cy, out cx);
        //    var cPos2 = sgworld.Creator.CreatePosition(cx, cy, 15, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -75, 0, 2000);
        //    //sgworld.Creator.CreateCircle(cPos2, 140, sgworld.Creator.CreateColor(127, 127, 127, 127), sgworld.Creator.CreateColor(127, 127, 127, 127), mGroupIDStatic);
        //    var ilabel = sgworld.Creator.CreateTextLabel(cPos2, item.ProjectName, CRWTEStandard.mLabelStyleL3, mGroupIDDynamic, "进度" + item.ProjectName);
        //    //ilabel.Message = sgworld.Creator.CreateMessage();
        //    ilabel.ImageFileName = CGisDataSettings.gDataPath + @"Common\progress\Pie" + item.AvgProgress + ".png";
        //}

        /// <summary>
        /// 添加桥梁、路基、隧道模型与标签，FIXME目前模型与工点表ProjectInof不一致（有些工点没有模型，有些工点应该对应多个模型），模型生成程序有一个单独的Excel表TEProj，不通过程序添加
        /// </summary>
        /// <param name="groupID"></param>
        //private void createProjectLabel()
        //{

        //    if (string.IsNullOrEmpty(mGroupIDDynamic))
        //        mGroupIDDynamic = sgworld.ProjectTree.CreateGroup("Project");
        //    //List<CRailwayProject> ls = mSceneData.GetProjectOrderbyMileage();
        //    //foreach (CRailwayProject rp in ls)
        //    //{
        //    //    Console.WriteLine(rp.ToString());
        //    //}

        //    foreach (CRailwayProject item in mSceneData.mBridgeList)
        //    {
        //        displayProject(item);

        //    }

        //    foreach (CRailwayProject item in mSceneData.mRoadList)
        //    {
        //        displayProject(item);

        //    }

        //    //foreach (CRailwayProject item in mSceneData.mTunnelList)
        //    //{

        //    //    displayProject(item);
        //    //}

        //    foreach (CRailwayProject item in mSceneData.mContBeamList)
        //    {

        //        displayProject(item);
        //    }

        //}

        ///// <summary>
        ///// 用条带的方式，展示分项工程进度
        ///// </summary>
        ///// <param name="rw"></param>
        ///// <param name="sgworld"></param>
        ///// <param name="branch"></param>
        ///// <param name="m"></param>
        //private void polylineMiddleLine(CRailwayProject rw, SGWorld66 sgworld, string branch, Dictionary<string, double> m)
        //{
        //    ITerrainPolyline66 polyline1;
        //    ITerrainPolyline66 polyline2;
        //    ITerrainPolyline66 polyline3;
        //    ITerrainPolyline66 polyline4;
        //    IColor66 lineColor1 = sgworld.Creator.CreateColor(242, 174, 17, 255);
        //    IColor66 lineColor2 = sgworld.Creator.CreateColor(32, 183, 81, 255);
        //    IColor66 lineColor3 = sgworld.Creator.CreateColor(183, 9, 9, 255);
        //    IColor66 lineColor4 = sgworld.Creator.CreateColor(50, 50, 50, 255);
        //    double[] x, y, z, dir;
        //    int count = mSceneData.mMiddleLines.getSubLineByDKCode(rw.Mileage_Start_Discription, rw.Mileage_End_Discription, 10, out x, out y, out z, out dir);
        //    double[] mArray1 = new double[count * 3];
        //    double[] mArray2 = new double[count * 3];
        //    double[] mArray3 = new double[count * 3];
        //    double[] mArray4 = new double[count * 3];
        //    for (int i = 0; i < count; i++)
        //    {
        //        mArray1[3 * i] = x[i];
        //        mArray1[3 * i + 1] = y[i];
        //        mArray1[3 * i + 2] = z[i] + 10;

        //        mArray2[3 * i] = x[i];
        //        mArray2[3 * i + 1] = y[i];
        //        mArray2[3 * i + 2] = z[i] + 20;

        //        mArray3[3 * i] = x[i];
        //        mArray3[3 * i + 1] = y[i];
        //        mArray3[3 * i + 2] = z[i] + 30;

        //        mArray4[3 * i] = x[i];
        //        mArray4[3 * i + 1] = y[i];
        //        mArray4[3 * i + 2] = z[i] + 40;
        //    }
        //    if (m.Count >= 4)
        //    {
        //        KeyValuePair<string, double> kvp = m.ElementAt(0);
        //        polyline1 = sgworld.Creator.CreatePolylineFromArray(mArray1, lineColor1, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, branch, rw.ProjectName + "-" + kvp.Key);
        //        polyline1.Spline = true;
        //        polyline1.Visibility.MaxVisibilityDistance = 10000000;
        //        polyline1.Visibility.MinVisibilityDistance = 150;
        //        polyline1.LineStyle.Width = 50.0;
        //        polyline1.Visibility.Show = true;
        //        polyline1.LineStyle.Color.SetAlpha(kvp.Value);

        //        kvp = m.ElementAt(1);
        //        polyline2 = sgworld.Creator.CreatePolylineFromArray(mArray2, lineColor2, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, branch, rw.ProjectName + "-" + kvp.Key);
        //        polyline2.Spline = true;
        //        polyline2.Visibility.MaxVisibilityDistance = 10000000;
        //        polyline2.Visibility.MinVisibilityDistance = 150;
        //        polyline2.LineStyle.Width = 30.0;
        //        polyline2.Visibility.Show = true;
        //        polyline2.LineStyle.Color.SetAlpha(kvp.Value);
        //        kvp = m.ElementAt(2);
        //        polyline3 = sgworld.Creator.CreatePolylineFromArray(mArray3, lineColor3, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, branch, rw.ProjectName + "-" + kvp.Key);
        //        polyline3.Spline = true;
        //        polyline3.Visibility.MaxVisibilityDistance = 10000000;
        //        polyline3.Visibility.MinVisibilityDistance = 150;
        //        polyline3.LineStyle.Width = 15.0;
        //        polyline3.Visibility.Show = true;
        //        polyline3.LineStyle.Color.SetAlpha(kvp.Value);
        //        kvp = m.ElementAt(3);
        //        polyline4 = sgworld.Creator.CreatePolylineFromArray(mArray4, lineColor4, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, branch, rw.ProjectName + "-" + kvp.Key);
        //        polyline4.Spline = true;
        //        polyline4.Visibility.MaxVisibilityDistance = 10000000;
        //        polyline4.Visibility.MinVisibilityDistance = 150;
        //        polyline4.LineStyle.Width = 5;
        //        polyline4.Visibility.Show = true;
        //        polyline4.LineStyle.Color.SetAlpha(kvp.Value);
        //    }
        //}


        //private void createRibbon()
        //{
        //    var sgworld = new SGWorld66();
        //    if (string.IsNullOrEmpty(mGroupIDDynamic))
        //        mGroupIDDynamic = sgworld.ProjectTree.CreateGroup("ProjectProgress");

        //    foreach (CRailwayProject item in mSceneData.mBridgeList) // FIXME xyn
        //    {
        //        double[] p = new double[20];       // 工点分项进度，色带透明度
        //        String[] FXName = new String[20];  // 工点分项名字
        //        Dictionary<string, double> mFXProj = item.getFXProgressByDate(mTEScene.toDate);
        //        polylineMiddleLine(item, sgworld, mGroupIDDynamic, mFXProj);
        //    }
        //}
        /// <summary>
        /// 测试进度circle
        /// </summary>
        private void createCircle()
        {
            var sgworld = new SGWorld66();

            if (string.IsNullOrEmpty(mGroupIDDynamic))
                mGroupIDDynamic = sgworld.ProjectTree.CreateGroup("ProjectProgress");
            ITerrainPolyline66 polyline;

            //ILabelStyle66 cLabelStyle = sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);
            //{
            //    cLabelStyle.FontName = "Arial";                         // Set font name to Arial
            //    cLabelStyle.Italic = true;                              // Set label style font to italic
            //    cLabelStyle.Scale = 3;                                  // Set label style scale
            //    cLabelStyle.TextOnImage = false;
            //}
            //CRailwayProject item = mSceneData.mProjectList[0];
            foreach (CRailwayProject item in mSceneData.mBridgeList) // FIXME xyn
            {
                //if (item.mMileage_End - item.mMileage_Start > 15)
                //{
                //item.getFXProgressByDate(mTEScene.toDate);
                //double[] cVerticesArray=null;

                //double latcen;
                //double loncen;
                double lat;
                double lon;
                double z;
                double dir;
                //int zone;
                //double x, y, xd, yd;
                double cx, cy;
                //double globalmeter;
                //bool isValue = mSceneData.mMiddleLines.getPosbyMeter(item.Mileage_Start_Ds, out lat, out lon, out z, out dir);
                //CoordinateConverter.LatLonOffest(lat, lon, z, 270, 300, out latcen, out loncen);
                //CoordinateConverter.LatLonToUTMXY(latcen, loncen, out x, out y, out zone);


                //int ccount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(item.AvgProgress * 180.0)));

                if (!CRailwayLineList.getGPSbyDKCode(item.mMidDKCode, item.CenterMileage, out lon, out lat, out z, out dir))
                {
                    Console.WriteLine(item.ProjectName + " 工点 坐标错误");
                    continue;
                }
                CoordinateConverter.LatLonOffest(lat, lon, dir, 90, 20, out cy, out cx);
                //int angle = (int)(item.AvgProgress * 360);
                //int count = 0;
                //cVerticesArray = CTERWItem.TEDrawProgressCircle(cx, cy, z, 15, angle, out count);

                //try
                //{
                //    //sgworld.Creator.createpol
                //    //polyline = sgworld.Creator.CreatePolygonFromArray(cVerticesArray, sgworld.Creator.CreateColor(255, 215, 0, 255), sgworld.Creator.CreateColor(255, 215, 0, 255),
                //           //AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, mTEID, item.ProjectName + "工点");
                //    //polyline.Visibility.MinVisibilityDistance = 1250;
                //    //polyline.Spline = true;
                //    //polyline.LineStyle.Width = -5;
                //    polyline = sgworld.Creator.CreatePolylineFromArray(cVerticesArray, sgworld.Creator.CreateColor(255, 215, 0, 255), AltitudeTypeCode.ATC_TERRAIN_RELATIVE, mGroupIDDynamic, "工点" + item.ProjectName);
                //    polyline.Visibility.MaxVisibilityDistance = 10000000;
                //    polyline.FillStyle.Color = sgworld.Creator.CreateColor(255, 215, 0, 255);
                //    polyline.LineStyle.Width = 130.0;
                //    polyline.Visibility.Show = true;

                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine("Creation Failed" + item.ProjectName);
                //}


                var cPos = sgworld.Creator.CreatePosition(cx, cy, 10, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -75, 0, 2000);
                var ilabel = sgworld.Creator.CreateTextLabel(cPos, item.AvgProgress + "%", CRWTEStandard.mLabelStyleL3, mGroupIDDynamic, "进度" + item.ProjectName);
                ilabel.ImageFileName = CGisDataSettings.gDataPath + @"Common\progress\Pie" + item.AvgProgress  + ".png";
            }
        }
 
        #endregion
    }
}
