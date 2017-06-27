using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInfo
{
    class CSourceBackup
    {
    }

    #region 测试距离
    //Console.WriteLine(CoordinateConverter.getUTMDistance(120.142022774671, 36.248995887497, 120.142906127281, 36.2481853286171) + "\t 120");
    //Console.WriteLine(CoordinateConverter.getUTMDistance(120.142022774671, 36.248995887497, 120.150934476456, 36.2437588244801) + "\t 1000");
    //double x,y,z,dir,x2,y2;
    //mMiddleLines.getPosbyDKCode("DK", 21074.9, out x, out y, out z, out dir);

    //// instantiate the calculator
    //GeodeticCalculator geoCalc = new GeodeticCalculator();

    //// select a reference elllipsoid
    //Ellipsoid reference = Ellipsoid.WGS84;
    //double xx, yy;
    //xx = 117.395521;
    //yy = 36.782793;
    //// set Lincoln Memorial coordinates
    //GlobalCoordinates pierGPS;
    //pierGPS = new GlobalCoordinates(
    //    new Angle(y), new Angle(x)
    //);

    //// set Eiffel Tower coordinates
    //GlobalCoordinates peopleMars;
    //peopleMars = new GlobalCoordinates(
    //    new Angle(yy), new Angle(xx)
    //);

    //// calculate the geodetic curve
    //GeodeticCurve geoCurve = geoCalc.CalculateGeodeticCurve(reference, pierGPS, peopleMars);
    //Console.WriteLine( geoCurve.EllipsoidalDistance + "\t 准确的火星");

    //Console.WriteLine(CoordinateConverter.getUTMDistance(xx, yy, x, y) + "\t 我的火星");

    //GPSAdjust.bd_decrypt(yy, xx, out y2, out x2);
    //Console.WriteLine(x + "\t" + y + "\t" + x2 + "\t" + y2);
    //GlobalCoordinates peopleEarth;
    //peopleEarth = new GlobalCoordinates(
    //    new Angle(y2), new Angle(x2)
    //);

    //Console.WriteLine(geoCalc.CalculateGeodeticCurve(reference, pierGPS, peopleEarth).EllipsoidalDistance+ "\t 准确的地球");
    //Console.WriteLine(CoordinateConverter.getUTMDistance(x2, y2, x, y) + "\t 我的地球");
    #endregion  
 

        ////private double bwidth = 400;
        ////private double bheight = 150;
        ////private float scale = 1.0f;
        //private Color[] backColor = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Purple };
        ////private double 
        ///// <summary>
        ///// 绘制隧道进度
        ///// </summary>
        ///// <param name="myGraphics"></param>
        ///// <param name="description">分项工程描述</param>
        ///// <param name="done">分项完成百分比</param>
        ///// <param name="th">绘制宽度</param>
        ///// <param name="pm">隧道坐标系矩阵</param>
        ///// <param name="startM">起始里程，米</param>
        ///// <param name="endM">终止里程，米</param>
        ///// <param name="unitM">围岩等级，米</param>
        ///// <param name="mode">绘制模式，小里程-》大里程，角度</param>
        //public void draw_SubTunnelWithoutDate(Graphics myGraphics, List<string> description, List<double> done, double th, Matrix pm, double startM, double endM, double unitM, int mode )
        //{
        //    float totalLen = (float)Math.Abs(endM - startM );
        //    float unitLen = (float)unitM ;
        //    float sx = (float)(startM );
        //    float sy = -(float)(th / 2);
        //    //int doneLen;
        //    float height = (float)(th / (done.Count + 1));
        //    RectangleF areaf;
        //    SolidBrush notDoneB = new SolidBrush(Color.LightGray);
        //    SolidBrush doneB = new SolidBrush(Color.FromArgb(50, Color.Red));
        //    Matrix m = new Matrix();
        //    pm = pm.Clone();
        //    Pen p = new Pen(Color.Black, 2);


        //    p.DashStyle = DashStyle.DashDot;
        //    StringFormat format = new StringFormat();
        //    format.LineAlignment = StringAlignment.Center;  // 更正： 垂直居中
        //    format.Alignment = StringAlignment.Center;      // 水平居中

        //    switch (mode)
        //    {
        //        case 0:  // 小里程到大里程
        //            m.Translate(sx, 0);
        //            pm.Multiply(m);
        //            myGraphics.Transform = pm;


        //            p.StartCap = LineCap.RoundAnchor;
        //            p.EndCap = LineCap.ArrowAnchor;
        //            myGraphics.DrawLine(p, 0, sy + height / 2, totalLen, sy + height / 2);

        //            areaf = new RectangleF(0, sy, totalLen, height);
        //            myGraphics.DrawString(startM + "~" + endM, new Font("宋体", 12), Brushes.Black, areaf, format);
        //            sy += height;
        //            // 逐条绘制分项工程
        //            for (int i = 0; i < done.Count; i++)
        //            {

        //                myGraphics.DrawRectangle(Pens.Black, 0, sy, totalLen, height);

        //                if (unitLen < 4)
        //                {
        //                    myGraphics.FillRectangle(new SolidBrush(backColor[i]), 0, sy, (float)(totalLen * done[i]), height);
        //                    myGraphics.FillRectangle(notDoneB, (float)(totalLen * done[i]) + 1, sy, (float)(totalLen * (1 - done[i])), height);
        //                }
        //                else
        //                {
        //                    float j;
        //                    for (j = 1; j < (float)(totalLen * done[i]); j += unitLen)
        //                    {
        //                        myGraphics.FillRectangle(new SolidBrush(backColor[i]), j, sy + 1, unitLen - 2, height - 3);
        //                        myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, height - 3);
        //                    }



        //                    //myGraphics.DrawString(description[i] + " " + Math.Round(done[i] * 100, 2) + "%", new Font("宋体", 12), Brushes.Red, new PointF(5, y + 5));
        //                    for (; j < (float)(totalLen); j += unitLen)
        //                    {
        //                        myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, height - 3);
        //                    }
        //                }
        //                //myGraphics.DrawRectangle(Pens.Black, (int)(totalLen * done[i]) + 1, sy, (int)(totalLen * done[i]) % height - 1, height - 1);
        //                sy += height;
        //            }
        //            break;
        //        case 1:// 大里程到小里程
        //            m.Translate(sx, 0);
        //            pm.Multiply(m);
        //            myGraphics.Transform = pm;

        //            areaf = new RectangleF(0, sy, totalLen, height);

        //            p.StartCap = LineCap.ArrowAnchor;
        //            p.EndCap = LineCap.RoundAnchor;
        //            myGraphics.DrawLine(p, 0, sy + height / 2, totalLen, sy + height / 2);
        //            myGraphics.DrawString(startM + "~" + endM, new Font("宋体", 12), Brushes.Black, areaf, format);
        //            sy += height;

        //            for (int i = 0; i < done.Count; i++)
        //            {
        //                myGraphics.DrawRectangle(Pens.Black, 0, sy, totalLen, height);
        //                if (unitLen < 4)
        //                {
        //                    myGraphics.FillRectangle(notDoneB, 0, sy, (float)(totalLen * (1 - done[i])), height);
        //                    myGraphics.FillRectangle(new SolidBrush(backColor[i]), (float)(totalLen * (1 - done[i])) + 1, sy, (float)(totalLen * done[i]), height);
        //                }
        //                else
        //                {
        //                    float j;
        //                    for (j = 1; j < (float)(totalLen * done[i]); j += unitLen)
        //                    {

        //                        myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, height - 3);
        //                    }


        //                    for (; j < (float)(totalLen); j += unitLen)
        //                    {
        //                        myGraphics.FillRectangle(new SolidBrush(backColor[i]), j, sy + 1, unitLen - 2, height - 3);
        //                        myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, height - 3);
        //                    }
        //                }
        //                //myGraphics.DrawString(description[i] + " " + Math.Round(done[i] * 100, 2) + "%", new Font("宋体", 12), Brushes.Red, new PointF(5, y + 5));
        //                sy += height;
        //            }
        //            break;
        //        case 2:// 45度 斜井
        //            m.Translate(sx, (int)(th / 2));
        //            m.Rotate(90);
        //            m.Shear(0, -0.5f);
        //            pm.Multiply(m);
        //            myGraphics.Transform = pm;

        //            areaf = new RectangleF(0, sy, totalLen, height);

        //            p.StartCap = LineCap.ArrowAnchor;
        //            p.EndCap = LineCap.RoundAnchor;
        //            myGraphics.DrawLine(p, 0, sy + height / 2, totalLen, sy + height / 2);
        //            myGraphics.DrawString(startM + "~" + endM, new Font("宋体", 12), Brushes.Black, areaf, format);
        //            sy += height;
        //            //height = height / 2;
        //            //sy = sy / 2;
        //            for (int i = 0; i < done.Count; i++)
        //            {
        //                myGraphics.DrawRectangle(Pens.Black, 0, sy, totalLen, height);

        //                if (unitLen < 4)
        //                {
        //                    myGraphics.FillRectangle(notDoneB, 0, sy, (float)(totalLen * (1 - done[i])), height);
        //                    myGraphics.FillRectangle(new SolidBrush(backColor[i]), (float)(totalLen * (1 - done[i])) + 1, sy, (float)(totalLen * done[i]), height);
        //                }
        //                else
        //                {
        //                    float j;
        //                    for (j = 1; j < (float)(totalLen * done[i]); j += unitLen)
        //                    {

        //                        myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, height - 3);
        //                    }


        //                    for (; j < (float)(totalLen); j += unitLen)
        //                    {
        //                        myGraphics.FillRectangle(new SolidBrush(backColor[i]), j, sy + 1, unitLen - 2, height - 3);
        //                        myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, height - 3);
        //                    }
        //                }
        //                //myGraphics.FillRectangle(notDoneB, new Rectangle(0, sy, (int)(totalLen * (1 - done[i])), height));
        //                //myGraphics.DrawRectangle(Pens.BlanchedAlmond, new Rectangle((int)(totalLen * (1 - done[i])) + 1, sy, (int)(totalLen * done[i]), height));
        //                //myGraphics.FillRectangle(new SolidBrush(Color.LightBlue), new Rectangle((int)(totalLen * (1 - done[i])) + 1, sy, (int)(totalLen * done[i]), height));
        //                //myGraphics.DrawString(Math.Round(done[i] * 100, 2) + "%", new Font("宋体", 12), Brushes.Red, new PointF(5, y + 5));
        //                sy += height;
        //            }
        //            break;
        //        case 3: // 305度 斜井
        //            m.Translate(sx, (int)(-th / 2));
        //            m.Rotate(270);
        //            m.Shear(0, 0.5f);
        //            pm.Multiply(m);
        //            myGraphics.Transform = pm;
        //            areaf = new RectangleF(0, sy, totalLen, height);

        //            p.StartCap = LineCap.ArrowAnchor;
        //            p.EndCap = LineCap.RoundAnchor;
        //            myGraphics.DrawLine(p, 0, sy + height / 2, totalLen, sy + height / 2);
        //            myGraphics.DrawString(startM + "~" + endM, new Font("宋体", 12), Brushes.Black, areaf, format);
        //            sy += height;
        //            //height = height / 2;
        //            //sy = sy / 2;
        //            for (int i = 0; i < done.Count; i++)
        //            {
        //                myGraphics.DrawRectangle(Pens.Black, 0, sy, totalLen, height);

        //                if (unitLen < 4)
        //                {
        //                    myGraphics.FillRectangle(notDoneB, 0, sy, (float)(totalLen * (1 - done[i])), height);
        //                    myGraphics.FillRectangle(new SolidBrush(backColor[i]), (float)(totalLen * (1 - done[i])) + 1, sy, (float)(totalLen * done[i]), height);
        //                }
        //                else
        //                {
        //                    float j;
        //                    for (j = 1; j < (float)(totalLen * done[i]); j += unitLen)
        //                    {

        //                        myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, height - 3);
        //                    }


        //                    for (; j < (float)(totalLen); j += unitLen)
        //                    {
        //                        myGraphics.FillRectangle(new SolidBrush(backColor[i]), j, sy + 1, unitLen - 2, height - 3);
        //                        myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, height - 3);
        //                    }
        //                }
        //                //myGraphics.FillRectangle(notDoneB, new Rectangle(0, sy, (int)(totalLen * (1 - done[i])), height));
        //                //myGraphics.DrawRectangle(Pens.BlanchedAlmond, new Rectangle((int)(totalLen * (1 - done[i])) + 1, sy, (int)(totalLen * done[i]), height));
        //                //myGraphics.FillRectangle(new SolidBrush(Color.LightBlue), new Rectangle((int)(totalLen * (1 - done[i])) + 1, sy, (int)(totalLen * done[i]), height));
        //                //myGraphics.DrawString(Math.Round(done[i] * 100, 2) + "%", new Font("宋体", 12), Brushes.Red, new PointF(5, y + 5));
        //                sy += height;
        //            }
        //            break;
        //        //case 4:
        //        //    m.Translate(x, 0);
        //        //    m.Multiply(pm);
        //        //    myGraphics.Transform = m;
        //        //    for (int i = 0; i < done.Count; i++)
        //        //    {

        //        //        myGraphics.DrawRectangle(Pens.BlanchedAlmond, new Rectangle(0, y, (int)(totalLen * done[i]), height));
        //        //        myGraphics.FillRectangle(new SolidBrush(Color.LightBlue), new Rectangle(0, y, (int)(totalLen * done[i]), height));
        //        //        myGraphics.FillRectangle(notDoneB, new Rectangle((int)(totalLen * done[i]) + 1, y, (int)(totalLen * (1 - done[i])), height));
        //        //        myGraphics.DrawString(description[i] + " " + Math.Round(done[i] * 100, 2) + "%", new Font("宋体", Math.Abs(height - 10) / 2), Brushes.Red, new PointF(5, y + 5));
        //        //        y += height;
        //        //    }
        //        //    break;
        //        default:
        //            Console.WriteLine(startM + "\t" + endM + " tunnel type error");
        //            break;
        //    }
        //}


    //skyline 线路生成管道

    //using System;
    //using System.Collections.Generic;
    //using System.Linq;
    //using System.Reflection;
    //using System.Windows;
    //using System.Windows.Media.Media3D;
    //using TerraExplorerX;

    //namespace Eastdawn.Pipeline.Windows
    //{
    //    /// <summary>
    //    /// 生成管道模型
    //    /// </summary>
    //    public class BuildPipeCommand
    //    {
    //        public bool CanExecute(object parameter)
    //        {
    //            return true;
    //        }

    //        public void Execute(object parameter)
    //        {
    //            var selectedObject = ISGWorld6Extensions.SGWorld.ProjectTree.GetSelectedObject();
    //            if (selectedObject == null)
    //            {
    //                var action = new Action<object>(PipeLineString);
    //                ISGWorld6Extensions.SGWorld.SelectObject<object>(action);
    //            }
    //            else
    //            {
    //                var polyline = selectedObject is ILayer6 ? (selectedObject as ILayer6).SelectedFeatures[0] as IFeature6 : selectedObject;
    //                ILineString route = polyline.GetProperty<IGeometry>(polyline.GetRealType(o => o.WrapComType()), "Geometry") as ILineString;
    //                if (route == null)
    //                    return;
    //                PipeLineString(route);
    //            }
    //        }

    //        void PipeLineString(object lineString)
    //        {
    //            List<ILineString> lines = BezierLineString(lineString);
    //            lines.ForEach(line => DrawOnPolyline(line));
    //        }
    //        // 直线分段
    //        List<ILineString> BezierLineString(object lineString)
    //        {
    //            SGWorld sgWorld = ISGWorld6Extensions.SGWorld as SGWorld;

    //            IEnumerable<Point> routePoints = (lineString as ILineString).Points.Cast();
    //            List<IPosition6> positions = routePoints.Select(p => sgWorld.Creator.CreatePosition(p.X, p.Y, 0, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE)).ToList();
    //            positions.ForEach(p => p.Altitude = sgWorld.Terrain.GetTerrainAltitude(p.X, p.Y));          
    //            List<ILineString> lines = new List<ILineString>();
    //            if (positions.Count == 2)
    //            {
    //                lines.Add(lineString as ILineString);
    //                return lines;
    //            }
    //            // 拐角
    //            for (int i = 1; i < positions.Count-1; i++)
    //            {
    //                IPosition6 prePos = MidPosition(positions[i - 1], positions[i]);
    //                IPosition6 nextPos = MidPosition(positions[i+1], positions[i]);
    //                Vector3D p0 = new Vector3D(prePos.X, prePos.Y, prePos.Altitude);
    //                Vector3D p1 = new Vector3D(positions[i].X, positions[i].Y, positions[i].Altitude);
    //                Vector3D p2 = new Vector3D(nextPos.X, nextPos.Y, nextPos.Altitude);
    //                List<double> lineVertex = Bezier(p0, p1, p2);
    //                var line = sgWorld.Creator.GeometryCreator.CreateLineStringGeometry(lineVertex.ToArray());
    //                lines.Add(line);
    //            }
    //            // 直线段
    //            for (int i = 0; i < positions.Count - 1; i ++)
    //            {
    //                IPosition6 prePos = MidPosition(positions[i], positions[i+1]);
    //                IPosition6 nextPos = MidPosition(positions[i + 1], positions[i]);
    //                if(i == 0)
    //                    lines.Add(CreateLineString(positions[i],prePos));
    //                else if(i == positions.Count - 2)
    //                    lines.Add(CreateLineString(nextPos, positions[positions.Count - 1]));
    //                else
    //                    lines.Add(CreateLineString(nextPos,prePos ));
    //            }

    //            return lines;
    //        }

    //        // 取临近p1 20米的点
    //        IPosition6 MidPosition(IPosition6 p0, IPosition6 p1)
    //        {
    //            SGWorld sgWorld = ISGWorld6Extensions.SGWorld as SGWorld;
    //            double dis = p0.DistanceTo(p1);
    //            var p2 = p0.MoveToward(p1, dis - 20);
    //            p2.Altitude = sgWorld.Terrain.GetTerrainAltitude(p2.X, p2.Y);
    //            return p2;
    //        }
    //        // 两点创建内插直线
    //        ILineString CreateLineString(IPosition6 p0, IPosition6 p1)
    //        {
    //            SGWorld sgWorld = ISGWorld6Extensions.SGWorld as SGWorld;
    //            List<double> lineVertex = new List<double>();
    //            List<IPosition6> positions = new List<IPosition6>();
    //            GetIPositions(p0, p1, 100, ref positions);
    //            positions.ForEach(pos =>
    //            {
    //                lineVertex.Add(pos.X);
    //                lineVertex.Add(pos.Y);
    //                lineVertex.Add(pos.Altitude);
    //            });
    //            return sgWorld.Creator.GeometryCreator.CreateLineStringGeometry(lineVertex.ToArray());
    //        }
    //        // 直线段分段 高程内插
    //        public static void GetIPositions(IPosition6 firstPosition, IPosition6 lastPosition, double interval, ref List<IPosition6> positions)
    //        {
    //            int SegmentLength = (int)Math.Floor(firstPosition.DistanceTo(lastPosition) / interval);
    //            var dh = ((lastPosition.Altitude - firstPosition.Altitude) / SegmentLength);
    //            positions.Add(firstPosition);
    //            for (int i = 1; i < SegmentLength; i++)
    //            {
    //                var pos = firstPosition.MoveToward(lastPosition, i * interval);
    //                pos.Altitude = firstPosition.Altitude + i*dh;
    //                positions.Add(pos);
    //            }
    //            positions.Add(lastPosition);
    //        }
    //        // 生成bezier曲线
    //        List<double> Bezier(Vector3D p0, Vector3D p1, Vector3D p2)
    //        {
    //            List<double> lineVertex = new List<double>();
    //            for (int i = 0; i < 41; i++)
    //            {
    //                double t = i / 40.0;
    //                Vector3D bezierPt = (1 - t) * (1 - t) * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
    //                lineVertex.Add(bezierPt.X);
    //                lineVertex.Add(bezierPt.Y);
    //                lineVertex.Add(bezierPt.Z);
    //            }
    //            return lineVertex;
    //        }
    //        // linestring 绘制模型
    //        void DrawOnPolyline(object lineString)
    //        {
    //            SGWorld sgWorld = ISGWorld6Extensions.SGWorld as SGWorld;
    //            //IEnumerable<Point> routePoints = (lineString as ILineString).Points.Cast();
    //            //List<IPosition6> positions = routePoints.Select(p => sgWorld.Creator.CreatePosition(p.X, p.Y, 0, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE)).ToList();
    //            //positions.ForEach(p => p.Altitude = sgWorld.Terrain.GetTerrainAltitude(p.X, p.Y));
    //            List<IPosition6> positions = new List<IPosition6>();
    //            foreach (IPoint point in (lineString as ILineString).Points)
    //            {
    //                 var pos = sgWorld.Creator.CreatePosition(point.X, point.Y, point.Z, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE);
    //                 positions.Add(pos);
    //            }

    //            for (int i = 0; i < positions.Count - 1; i++)
    //            {
    //                IPosition6 currCoord = positions[i].AimTo(positions[i + 1]);
    //                var SegmentLength = currCoord.DistanceTo(positions[i + 1]);
    //                DrawPipe(currCoord, SegmentLength+0.5);
    //            }
    //        }
    //        // DrawPipe
    //        void DrawPipe(IPosition6 position, double SegmentLength)
    //        {
    //            SGWorld sgWorld = ISGWorld6Extensions.SGWorld as SGWorld;
    //            var texture = Application.Current.GetStartupPath() + @"\pipeTexture.bmp";
    //            var radius = 10;

    //            int groupId = sgWorld.ProjectTree.FindOrCreateGroup("石油管道");
    //            //position.Altitude = 0;
    //            var TEObj = sgWorld.Creator.CreateCylinder(position, radius, SegmentLength, sgWorld.Creator.CreateColor(56, 56, 56, 0), sgWorld.Creator.CreateColor(56, 56, 56, 255), 16, groupId, "");
    //            TEObj.Position.Pitch = -90 + position.Pitch;
    //            TEObj.LineStyle.Color.SetAlpha(0);
    //            TEObj.FillStyle.Texture.FileName = texture;
    //            TEObj.Visibility.MaxVisibilityDistance = 50000;
    //            TEObj.FillStyle.Texture.RotateAngle = FixTextureAngle(TEObj.Position.Pitch, TEObj.NumberOfSegments);

    //        }
    //        // FixTextureAngle
    //        double FixTextureAngle(double pitch, double NumberofSegments)
    //        {
    //            var FixAngle = Math.Atan(Math.Sin(Math.PI / NumberofSegments) / Math.Cos(pitch / 180 * Math.PI)) * 180 / Math.PI + 90;
    //            return FixAngle;
    //        }
    //    }
    //}
}
