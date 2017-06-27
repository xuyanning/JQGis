using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ModelInfo;

namespace RailwayGIS
{
    public partial class PanelProgress2D : UserControl
    {
        CRailwayScene mRWScene;

        private static Font defaultFont = new Font("宋体", 12);
        private static Pen defaultPen = new Pen(Color.Black, 1);
        private static Pen redPen = new Pen(Color.Red, 1);
        private static Pen roundPen = new Pen(Color.Black, 2);
        //private static Color[] backColor = new Color[] { , , , Color.Yellow, Color.Purple, Color.Orange };
        private static Brush[] backBrush = new SolidBrush[] { new SolidBrush(Color.Red), new SolidBrush(Color.Green), new SolidBrush(Color.Blue) };

        private static SolidBrush notDoneB = new SolidBrush(Color.LightGray);
        private static SolidBrush doneB = new SolidBrush(Color.FromArgb(50, Color.Red));

        StringFormat centerFormat = new StringFormat();

        public double mileageCenter = -1;
        public double mileageViewRadius = -1;

        private string mDesc = "";

        //private CRailwayProject mSelectedProject = null;
        //private CRailwayDWProj mSelectedDWP = null;


        public PanelProgress2D(CRailwayScene s)
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.AllPaintingInWmPaint, true);

            roundPen.StartCap = LineCap.RoundAnchor;
            roundPen.EndCap = LineCap.RoundAnchor;

            centerFormat.LineAlignment = StringAlignment.Center;  // 更正： 垂直居中
            centerFormat.Alignment = StringAlignment.Center;      // 水平居中

            mRWScene = s;
        }

        public void update2DPanel(double center, double radius, string desc = "")
        {
            mileageCenter = center;
            mileageViewRadius = Math.Min (200000,radius);
            mDesc = desc;
            Refresh();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (mileageCenter >= 0)
            {
                Rectangle rect = e.ClipRectangle;
                BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
                BufferedGraphics myBuffer = currentContext.Allocate(e.Graphics, e.ClipRectangle);
                Graphics g = myBuffer.Graphics;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                g.Clear(Color.White);

                draw2DProjects(g, panel1.Size.Width, mileageCenter, mileageViewRadius);

                myBuffer.Render(e.Graphics);
                g.Dispose();
                myBuffer.Dispose();//释放资源 

            }

        }

        ////绘图
        //private void Draw(Graphics formGp)
        //{
        //    Bitmap bitmap = new Bitmap(this.Width, this.Height);
        //    Graphics gp = Graphics.FromImage(bitmap);
        //    this.DrawBg(gp);
        //    if (this.list != null)
        //    {
        //        this.list.Draw(gp);
        //    }
        //    this.DrawCountClick(gp);

        //    formGp.DrawImage(bitmap, 0, 0);
        //}
        //定义事件
        //定义委托
        public delegate void PanelSelectHandle(CRailwayProject prj, CRailwayDWProj dwprj);
        public event PanelSelectHandle PanelSelectClicked = null;

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (PanelSelectClicked != null)
            {
                //int fxid;
                CRailwayProject p = null;
                CRailwayDWProj dwp = null;

                getGlobalPos(panel1.Size.Width, mileageCenter, mileageViewRadius, e.X, e.Y, out p, out dwp);
                PanelSelectClicked(p, dwp);
            }

        }

        #region 绘制二维形象进度
        const float Cap1StartY = 5;
        const float prjStartY = 25;
        const float prjEndY = 110;
        const float MileageAxisStartY = 140;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="canvasWidth">画布宽度</param>
        /// <param name="centerMileage">中心里程</param>
        /// <param name="radiusMileage">里程半径</param>
        private void draw2DProjects(Graphics g, float canvasWidth, double centerMileage, double radiusMileage)
        {
            // 如果视点距离过近，视野半径只有20米，不再绘制
            if (radiusMileage < 10) return;
            g.DrawString("济青高铁", defaultFont, Brushes.Red, new PointF(canvasWidth - 100, Cap1StartY));

            //float titleHeight = 30;
            float scaleX;

            //float width = panel1.Size.Width;

            Matrix m = getTransM((float)centerMileage, (float)radiusMileage, canvasWidth, out scaleX, 50);
            //Matrix im = getInvTransM((float)centerMileage, (float)radiusMileage, canvasWidth, 50);

            // 左右50个像素空白
            PointF[] sd = { new PointF(50, MileageAxisStartY), new PointF(canvasWidth / 2, MileageAxisStartY), new PointF(canvasWidth - 50, MileageAxisStartY) }; // 窗口
            double[] sdMileage = { centerMileage - radiusMileage, centerMileage, centerMileage + radiusMileage };
            //im.TransformPoints(sdMileage);
            string dkcode;
            double dkm;

            RectangleF areaf;


            //绘制里程坐标
            g.DrawLine(roundPen, sd[0], sd[1]);
            g.DrawLine(roundPen, sd[1], sd[2]);
            string str;
            // 写里程坐标
            for (int i = 0; i < 3; i++)
            {
                mRWScene.mMainPath.getDKCodebyPathMileage(sdMileage[i], out dkcode, out dkm);
                
                str = CRailwayLineList.CombiDKCode(dkcode, dkm);
                if (i == 1)
                {
                    str = "当前里程" + str;
                    if (string.IsNullOrEmpty(mDesc))
                        areaf = new RectangleF(sd[i].X - 60, MileageAxisStartY+5, 250, 20);
                    else
                    {
                        str += "。 下一目标：" + mDesc;
                        areaf = new RectangleF(sd[i].X - 60, MileageAxisStartY + 5, 520, 20);                        
                    }
                    g.DrawString(str, defaultFont, Brushes.Black, areaf);
                }
                else
                {
                    areaf = new RectangleF(sd[i].X - 40, MileageAxisStartY + 5, 80, 20);
                    g.DrawString(str, defaultFont, Brushes.Black, areaf, centerFormat);
                }
            }

            //myGraphics.Transform = m;
            foreach (CRailwayTunnel t in mRWScene.mTunnelList)
                draw_TunnelFX(g, t, 60, canvasWidth, m, scaleX);
            foreach (CRailwayRoad r in mRWScene.mRoadList)
                draw_RoadFX(g, r, 60, canvasWidth, m, scaleX);
            foreach (CRailwayBridge b in mRWScene.mBridgeList)
                draw_BridgeFX(g, b, 60, canvasWidth, m, scaleX);
            foreach (CContBeam cb in mRWScene.mContBeamList)
                draw_ContBeam(g, cb, 60, canvasWidth, m, scaleX);
            foreach (CHotSpot hs in mRWScene.mConsSpotList)
            {
                draw_ConsSpot(g, hs, 60, canvasWidth, m, scaleX);
            }
            //draw_TunnelFX(myGraphics, ls, ld, width, m, 5000, -1000, 100,  scaleX);
            //draw_TunnelFX(myGraphics, ls, ld, width, m, 5000, 1500, 10,  scaleX);
            //draw_TunnelFX(myGraphics, ls, ld, width, m, 7500, -1000, 100, scaleX);
            //PointF[] sd = { new PointF(4000, 100), new PointF(6000, 100) }; // 窗口
            //m.TransformPoints(sd);
            //Console.WriteLine(sd[0]);
            //Console.WriteLine(sd[1]);
            //im.TransformPoints(sd);
            //Console.WriteLine(sd[0]);
            //Console.WriteLine(sd[1]);

            //float scale = wh / 2 / vw;
            //Matrix rm = new Matrix();
            //rm.Scale(2, 1);
            //rm.Translate(-50, -150);
            //rm.TransformPoints(sd);
        }

        private void draw_ConsSpot(Graphics myGraphics, CHotSpot hspot, float winHeight, float winWidth, Matrix pm, float scaleX, float unitM = 1000)
        {

            PointF[] sd = { new PointF(0, 100) }; // 窗口
            sd[0].X = (float)hspot.GlobalMileage;
            //sd[1].X = (float)ex;
            pm.TransformPoints(sd);
            if (sd[0].X < 0 || sd[0].X > winWidth) // 如果道路不在绘制区域，返回
                return;

            float unitPixelLength = unitM * scaleX;
            float PixelX = sd[0].X;

            RectangleF areaf;
            areaf = new RectangleF(PixelX - 12, 155, 24, 24);
            try
            {
                Image workingIcon = Image.FromFile(CGisDataSettings.gDataPath + @"Common\地标图片\work.gif");
                myGraphics.DrawImage(workingIcon, areaf);
            }
            catch (Exception fe)
            {
                Console.WriteLine();
            }

        }


        /// <summary>
        /// 绘制分项工程进度
        /// </summary>
        /// <param name="myGraphics"></param>
        /// <param name="tunnel"></param>
        /// <param name="winHeight"> 隧道绘制宽度（像素），一般60</param>
        /// <param name="winWidth">窗口panel的宽度（像素），包围盒，用于判断是否该图形越界</param>
        /// <param name="pm">变换矩阵</param>
        /// <param name="scaleX">x方向上 里程长度 * scaleX = 像素长度</param>
        /// <param name="unitM">单位里程（米）</param>

        private bool draw_PrjFX(Graphics myGraphics, CRailwayProject prj, string prjType, float winHeight, float winWidth, Matrix pm, float scaleX, bool fxOnly = true, float unitM = 8)
        {
            bool needDrawDetails = false;
            float prjLength = (float)prj.Length;
            float prjPixelLength = prjLength * scaleX;
            float done;

            bool isReverse = prj.mMileage_End < prj.mMileage_Start;
            float sx, ex;
            if (isReverse)
            {
                sx = (float)prj.mMainMileageE;
                ex = (float)prj.mMainMileageS;
            }
            else
            {
                sx = (float)prj.mMainMileageS;
                ex = (float)prj.mMainMileageE;
            }


            PointF[] sd = { new PointF(0, 100), new PointF(0, 100) }; // 窗口
            sd[0].X = (float)sx;
            sd[1].X = (float)ex;
            pm.TransformPoints(sd);
            if (sd[1].X < 0 || sd[0].X > winWidth) // 如果桥梁不在绘制区域，返回false，也不需要绘制细节
                return needDrawDetails;

            float unitPixelLength = unitM * scaleX;
            float startPixelX = sd[0].X;
            float endPixelX = sd[1].X;
            float actualPixelS = Math.Max(startPixelX, 0);
            float actualPixelE = Math.Min(endPixelX, winWidth);  // 屏幕内的起始终止位置
            float unitHeight = (winHeight / 3);

            Pen p = defaultPen;

            RectangleF areaf;
            float sy = prjStartY + 25;

            // 1、绘制左右边界，上方线，以及工程名
            areaf = new RectangleF(actualPixelS, prjStartY, actualPixelE - actualPixelS, 20);
            if (prj.mAvgProgress > 0)
            {
                myGraphics.DrawLine(p, endPixelX, prjStartY, endPixelX, prjEndY);
                myGraphics.DrawLine(p, startPixelX, 47, endPixelX, 47);
                myGraphics.DrawLine(p, startPixelX, prjStartY, startPixelX, prjEndY);
                if (areaf.Width > 200)
                {
                    if (isReverse)
                    {
                        myGraphics.DrawString("<<< " + prj.ProjectName, defaultFont, Brushes.Black, areaf, centerFormat);
                    }
                    else
                    {
                        myGraphics.DrawString(prj.ProjectName + ">>>", defaultFont, Brushes.Black, areaf, centerFormat);
                    }
                }
                else if (areaf.Width > 20)
                {

                    myGraphics.DrawString(prjType, defaultFont, Brushes.Black, areaf, centerFormat);

                }
            }

            // 如果8米对应3个像素以内，逐条绘制分项工程
            if (fxOnly || unitPixelLength < 4)
            {
                if (!(prj.FXProgress == null || prj.FXProgress.Count == 0))
                {
                    for (int i = 0; i < 3; i++) //road.FXProgress.Count, FIXME 选3项绘制
                    {
                        int sIndex = prj.selectedFXid[i];
                        if (sIndex < 0) continue;
                        CFXProj proj = prj.FXProgress[sIndex];
                        int lastIndex = proj.doneAmount.Count - 1;
                        done = (float)(proj.doneAmount[lastIndex] / proj.TotalAmount);
                        //areaf = new RectangleF(sx, sy, tunnelLength, unitHeight);
                        myGraphics.DrawRectangle(Pens.Black, startPixelX, sy, prjPixelLength, unitHeight);


                        if (isReverse)
                        {
                            myGraphics.FillRectangle(notDoneB, startPixelX, sy + 1, prjPixelLength * (1 - done), unitHeight - 2);
                            myGraphics.FillRectangle(backBrush[i], startPixelX + prjPixelLength * (1 - done) + 1, sy + 1, prjPixelLength * done, unitHeight - 2);
                        }
                        else
                        {
                            myGraphics.FillRectangle(backBrush[i], startPixelX, sy + 1, prjPixelLength * done, unitHeight - 2);
                            myGraphics.FillRectangle(notDoneB, startPixelX + prjPixelLength * done + 1, sy + 1, prjPixelLength * (1 - done), unitHeight - 2);
                        }


                        areaf = new RectangleF(actualPixelS, sy, actualPixelE - actualPixelS, unitHeight);
                        if (areaf.Width > 100)
                            myGraphics.DrawString(Math.Round(done, 3) * 100 + "%", defaultFont, Brushes.Black, areaf, centerFormat);
                        sy += unitHeight;
                    }
                }
                return false;
            }

            needDrawDetails = true;
            return needDrawDetails;

        }

        /// <summary>
        /// 绘制隧道分项工程进度，开挖，仰拱，二衬
        /// </summary>
        /// <param name="myGraphics"></param>
        /// <param name="tunnel"></param>
        /// <param name="winHeight"> 隧道绘制宽度（像素），一般60</param>
        /// <param name="winWidth">窗口panel的宽度（像素），包围盒，用于判断是否该图形越界</param>
        /// <param name="pm">变换矩阵</param>
        /// <param name="scaleX">x方向上 里程长度 * scaleX = 像素长度</param>
        /// <param name="unitM">单位里程（米）</param>
        private void draw_TunnelFX(Graphics myGraphics, CRailwayTunnel tunnel, float winHeight, float winWidth, Matrix pm, float scaleX, float unitM = 10)
        //private void draw_TunnelFX(Graphics myGraphics, List<string> description, List<double> done, double th, Matrix pm, double startM, double totalLen, double unitM, int mode, float scale = 1)
        {
            //float totalLen = (float)Math.Abs(endM - startM) ;
            if (tunnel.FXProgress == null || tunnel.FXProgress.Count == 0) return;
            if (tunnel.FXProgress[0].fxID != 472) return; // FIXME，只绘制正洞
            float tunnelLength = (float)tunnel.Length;
            float tunnelPixelLength = tunnelLength * scaleX;

            //   List<string> description = tunnel.FXProgress.;
            float done;

            bool isReverse = tunnel.mMileage_End < tunnel.mMileage_Start;

            float sx, ex;
            if (isReverse)
            {
                sx = (float)tunnel.mMainMileageE;
                ex = (float)tunnel.mMainMileageS;
            }
            else
            {
                sx = (float)tunnel.mMainMileageS;
                ex = (float)tunnel.mMainMileageE;
            }

            PointF[] sd = { new PointF(0, 100), new PointF(0, 100) }; // 窗口
            sd[0].X = (float)sx;
            sd[1].X = (float)ex;
            pm.TransformPoints(sd);
            if (sd[1].X < 0 || sd[0].X > winWidth) // 如果隧道不在绘制区域，返回
                return;

            float unitPixelLength = unitM * scaleX;
            float startPixelX = sd[0].X;
            float endPixelX = sd[1].X;

            float unitHeight = (winHeight / tunnel.FXProgress.Count);

            SolidBrush notDoneB = new SolidBrush(Color.LightGray);
            SolidBrush doneB = new SolidBrush(Color.FromArgb(50, Color.Red));

            Pen p = new Pen(Color.Black, 1);
            //p.StartCap = LineCap.RoundAnchor;
            //p.EndCap = LineCap.ArrowAnchor;            
            //p.DashStyle = DashStyle.DashDot;


            // 绘制竖线边界以及隧道名称

            float actualPixelS = Math.Max(startPixelX, 0);
            float actualPixelE = Math.Min(endPixelX, winWidth);  // 屏幕内的起始终止位置
            RectangleF areaf;
            areaf = new RectangleF(actualPixelS, 25, actualPixelE - actualPixelS, 20);

            myGraphics.DrawLine(p, startPixelX, 25, startPixelX, 50);
            myGraphics.DrawLine(p, endPixelX, 25, endPixelX, 50);
            myGraphics.DrawLine(p, startPixelX, 47, endPixelX, 47);
            if (areaf.Width > 200)
            {
                if (isReverse)
                {
                    myGraphics.DrawString("<<< " + tunnel.ProjectName, defaultFont, Brushes.Black, areaf,centerFormat );
                }
                else
                {
                    myGraphics.DrawString(tunnel.ProjectName + ">>>", defaultFont, Brushes.Black, areaf, centerFormat);
                }
            }
            else if (areaf.Width > 20)
            {

                myGraphics.DrawString("隧", defaultFont, Brushes.Black, areaf, centerFormat);

            }

            float sy = 50;
            //myGraphics.Transform = pm;
            // 逐条绘制分项工程
            for (int i = 0; i < tunnel.FXProgress.Count; i++)
            {
                int lastIndex = tunnel.FXProgress[i].doneAmount.Count - 1;
                done = (float)(tunnel.FXProgress[i].doneAmount[lastIndex] / tunnel.FXProgress[i].TotalAmount);
                //areaf = new RectangleF(sx, sy, tunnelLength, unitHeight);
                myGraphics.DrawRectangle(Pens.Black, startPixelX, sy, tunnelPixelLength, unitHeight);

                //if (unitPixelLength < 4)
                {
                    if (isReverse)
                    {
                        myGraphics.FillRectangle(notDoneB, startPixelX, sy + 1, tunnelPixelLength * (1 - done), unitHeight - 2);
                        myGraphics.FillRectangle(backBrush[i], startPixelX + tunnelPixelLength * (1 - done) + 1, sy + 1, tunnelPixelLength * done, unitHeight - 2);
                    }
                    else
                    {
                        myGraphics.FillRectangle(backBrush[i], startPixelX, sy + 1, tunnelPixelLength * done, unitHeight - 2);
                        myGraphics.FillRectangle(notDoneB, startPixelX + tunnelPixelLength * done + 1, sy + 1, tunnelPixelLength * (1 - done), unitHeight - 2);
                    }
                }
                //else
                //if (unitPixelLength > 4)
                //{
                //    float j;
                //    if (isReverse)
                //    {
                //        for (j = tunnelPixelLength - unitPixelLength; j > 0; j -= unitPixelLength)
                //        {
                //            myGraphics.DrawEllipse(p, startPixelX + j, sy + 1, unitPixelLength - 1, unitPixelLength);
                //        }
                //    }
                //    else
                //    {
                //        for (j = 0; j < tunnelPixelLength; j += unitPixelLength)
                //        {
                //            myGraphics.DrawEllipse(p, startPixelX + j, sy + 1, unitPixelLength - 1, unitPixelLength);
                //        }
                //    }
                //    //if (isReverse)
                //    //{
                //    //    for (j = 0; j < tunnelLength * (1 - done); j += unitM)
                //    //    {

                //    //        myGraphics.DrawRectangle(Pens.Black, (float)sx + j, sy + 1, unitM - 2, unitHeight - 3);
                //    //    }

                //    //    //myGraphics.DrawString(description[i] + " " + Math.Round(done[i] * 100, 2) + "%", defaultFont, Brushes.Red, new PointF(5, y + 5));
                //    //    for (; j < tunnelLength; j += unitM)
                //    //    {
                //    //        myGraphics.FillRectangle(new SolidBrush(CRWTEStandard.backColor[i]), sx + j, sy + 1, unitM - 2, unitHeight - 3);
                //    //        myGraphics.DrawRectangle(Pens.Black, sx + j, sy + 1, unitM - 2, unitHeight - 3);
                //    //    }
                //    //}
                //    //else
                //    //{


                //    //    for (j = 0; j < tunnelLength * done; j += unitM)
                //    //    {
                //    //        myGraphics.FillRectangle(new SolidBrush(CRWTEStandard.backColor[i]), sx + j, sy + 1, unitM - 2, unitHeight - 3);
                //    //        myGraphics.DrawRectangle(Pens.Black, sx + j, sy + 1, unitM - 2, unitHeight - 3);
                //    //    }

                //    //    //myGraphics.DrawString(description[i] + " " + Math.Round(done[i] * 100, 2) + "%", defaultFont, Brushes.Red, new PointF(5, y + 5));
                //    //    for (; j < tunnelLength; j += unitM)
                //    //    {
                //    //        myGraphics.DrawRectangle(Pens.Black, sx + j, sy + 1, unitM - 2, unitHeight - 3);
                //    //    }
                //    //}
                //}
                areaf = new RectangleF(actualPixelS, sy, actualPixelE - actualPixelS, unitHeight);
                if (areaf.Width > 100)
                    myGraphics.DrawString(Math.Round(done, 3) * 100 + "%", defaultFont, Brushes.Black, areaf, centerFormat);
                //myGraphics.DrawRectangle(Pens.Black, (int)(totalLen * done[i]) + 1, sy, (int)(totalLen * done[i]) % height - 1, height - 1);
                sy += unitHeight;
            }

            //Matrix om = new Matrix();
            //myGraphics.Transform = om;
        }

        private void draw_RoadFX(Graphics myGraphics, CRailwayProject road, float winHeight, float winWidth, Matrix pm, float scaleX, float unitM = 100)
        {
            //float sy = prjStartY + 25;
            //float unitPixelLength = unitM * scaleX;
            //Pen p = defaultPen;
            draw_PrjFX(myGraphics, road, "路", winHeight, winWidth, pm, scaleX,true, unitM);

        }


        private void draw_BridgeFX(Graphics myGraphics, CRailwayProject bridge, float winHeight, float winWidth, Matrix pm, float scaleX, float unitM = 8)
        {
            float sy = prjStartY + 25;
            float unitPixelLength = unitM * scaleX;
            Pen p = defaultPen;
            if (draw_PrjFX(myGraphics, bridge, "桥", winHeight, winWidth, pm, scaleX,false, unitM))
            {
                try
                {
                    // 绘制桥墩
                    if (bridge.mPierList != null && bridge.mPierList.Count > 0)
                    {
                        PointF[] pls = new PointF[bridge.mPierList.Count];
                        CRailwayDWProj[] dwls = new CRailwayDWProj[bridge.mPierList.Count];
                        int ii = 0;
                        foreach (CRailwayDWProj dwp in bridge.mPierList)  // 遍历里程合法的桥墩
                        {
                            if (dwp.mIsValid)
                            {
                                pls[ii] = new PointF((float)dwp.mMainMileage, 0);
                                dwls[ii] = dwp;
                                ii++;
                            }
                        }
                        pm.TransformPoints(pls);
                        List<int> tls = new List<int> { 4, 4 };
                        for (int j = 0; j < ii; j++)
                        {
                            if (pls[j].X < 0 || pls[j].X > winWidth) continue;
                            drawOnePier(myGraphics, pls[j].X, sy + 10, unitPixelLength, (CRailwayPier)dwls[j]);

                            myGraphics.DrawString(dwls[j].DWName, defaultFont, Brushes.Black, pls[j].X - unitPixelLength, sy + 70);

                        }
                    }

                    //绘制梁
                    if (bridge.mBeamList != null && bridge.mBeamList.Count > 0)
                    {
                        PointF[] pls = new PointF[bridge.mBeamList.Count];
                        PointF[] ple = new PointF[bridge.mBeamList.Count];
                        CRailwayDWProj[] dwls = new CRailwayDWProj[bridge.mBeamList.Count];
                        int ii = 0;
                        foreach (CRailwayDWProj dwp in bridge.mBeamList)  // 遍历里程合法的梁
                        {
                            if (dwp.mIsValid)
                            {
                                pls[ii] = new PointF((float)dwp.mMainMileage, 0);
                                ple[ii] = new PointF((float)(dwp.mMainMileage + dwp.mLength), 0);
                                dwls[ii] = dwp;
                                ii++;
                            }
                        }
                        pm.TransformPoints(pls);
                        pm.TransformPoints(ple);

                        for (int j = 0; j < ii; j++)
                        {
                            if (pls[j].X < 0 || pls[j].X > winWidth) continue;

                            if (dwls[j].mIsDone)
                            {
                                p = redPen;
                            }
                            else
                                p = defaultPen;
                            if (j + 1 < ii)
                            {
                                myGraphics.DrawRectangle(p, pls[j].X, sy, Math.Abs(pls[j + 1].X - pls[j].X), 10);
                            }
                            else
                                myGraphics.DrawRectangle(p, pls[j].X, sy, Math.Abs(ple[j].X - pls[j].X), 10);

                            //myGraphics.DrawString(dwls[j].DWName, defaultFont, Brushes.Black, pls[j].X - unitPixelLength, sy + 70);

                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(bridge.ProjectName + "绘制异常，桥梁");
                }
            }
                //myGraphics.DrawRectangle(Pens.Black, pls[j].X, sy, pls[j + 1].X - pls[j].X - 1, 10);  

        }

        private void draw_ContBeam(Graphics myGraphics, CRailwayProject bridge, float winHeight, float winWidth, Matrix pm, float scaleX, float unitM = 8)
        {
            if (bridge.mPierList.Count < 2)
            {// FIXME xu
                //Console.WriteLine("连续梁无桥墩错误" + bridge.ProjectName);
                return;
            }
            float sy = prjStartY + 25;
            float unitPixelLength = unitM * scaleX;
            Pen p = defaultPen;
            if (draw_PrjFX(myGraphics, bridge, "连", winHeight, winWidth, pm, scaleX,false, unitM))
            {
                try
                {
                    PointF[] pls = new PointF[bridge.mPierList.Count];
                    CRailwayDWProj[] dwls = new CRailwayDWProj[bridge.mPierList.Count];
                    int ii = 0;
                    foreach (CRailwayDWProj dwp in bridge.mPierList)
                    {
                        if (dwp.mIsValid)
                        {
                            pls[ii] = new PointF((float)dwp.mMainMileage, 0);
                            dwls[ii] = dwp;
                            ii++;
                        }
                    }
                    pm.TransformPoints(pls);
                    //List<int> tls = new List<int> { 4, 2, 4 };
                    for (int j = 0; j < ii; j++)
                    {
                        if (pls[j].X < 0 || pls[j].X > winWidth)
                            continue;
                        drawOnePier(myGraphics, pls[j].X, sy + 10, unitPixelLength, (CRailwayPier)dwls[j]);

                        // 绘制连续墩的梁
                        int beamCount;
                        if (j + 1 < ii && ((CRailwayPier)dwls[j + 1]).mBeamDone.Count > 0)// 0号墩台和最后一个墩台不会向两边架梁，n+1个墩，n个梁
                        {
                            beamCount = ((CRailwayPier)dwls[j + 1]).mBeamDone.Count;
                            float w = pls[j + 1].X - pls[j].X;
                            float uw = w / beamCount;
                            float ssx = pls[j].X;
                            for (int i = 0; i < beamCount; i++)
                            {
                                if (((CRailwayPier)dwls[j + 1]).mBeamDone[i])
                                {
                                    p = redPen;
                                }
                                else
                                    p = defaultPen;

                                myGraphics.DrawRectangle(p, ssx, sy, uw, 10);
                                ssx += uw;
                            }
                        }
                        myGraphics.DrawString(dwls[j].DWName, defaultFont, Brushes.Black, pls[j].X - unitPixelLength, sy + 70);

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(bridge.ProjectName + "绘制异常，连续梁");
                }
                //myGraphics.DrawRectangle(Pens.Black, pls[j].X, sy, pls[j + 1].X - pls[j].X - 1, 10);  
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r">桩直径</param>
        /// <param name="x">一行桩的x坐标</param>
        /// <param name="y">一行桩的y坐标</param>
        /// <param name="num">一行桩的数量</param>
        /// <returns></returns>
        private List<PointF> drawHoleArray(float r, float x, float y,float h, int num)
        {
            List<PointF> pls = new List<PointF>();
            float yt = y + r;  // top 
            float yb = y + h - 2* r;
            float my = (yt + yb) / 2 ;
            float my1 = yt + 2 * r;
            float my2 = yb - 2 * r;

            if (num == 2)
            {
                pls.Add(new PointF(x, my1));
                pls.Add(new PointF(x, my2));
            }
            else if (num == 3)
            {
                pls.Add(new PointF(x, yt));
                pls.Add(new PointF(x, my));
                pls.Add(new PointF(x, yb));
            }
            else if (num == 4)
            {
                pls.Add(new PointF(x, yt));
                pls.Add(new PointF(x, my1));
                pls.Add(new PointF(x, my2));
                pls.Add(new PointF(x, yb));
            }
            else if (num == 5)
            {
                pls.Add(new PointF(x, yt));
                pls.Add(new PointF(x, my1));
                pls.Add(new PointF(x, my));
                pls.Add(new PointF(x, my2));
                pls.Add(new PointF(x, yb));
            }
            return pls;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cx">center X</param>
        /// <param name="ty">top Y</param>
        /// <param name="w">width</param>
        /// <param name="pie"></param>
        private void drawOnePier(Graphics g, float cx, float ty, float w, CRailwayPier pie)
        {
            List<int> tls = null;
            // 根据桩的数量确定分布
            switch (pie.mStubDone.Count)
            {
                case 8:
                    tls = new List<int> { 4, 4 };
                    break;
                case 10:
                    tls = new List<int> { 4, 2, 4 };
                    break;
                case 12:
                    tls = new List<int> { 4, 4, 4 };
                    break;
                case 16:
                    tls = new List<int> { 4, 4, 4, 4 };
                    break;
                default:
                    tls = new List<int> { 5, 5, 5, 5, 5 };
                    break;

            }
            if (w < 4 || tls.Count < 2)
                return;
            w = w * tls.Count / 3; // 三行桩作为基准宽度
            float lx = cx - w / 2;
            float rx = cx + w / 2;

            float lx1 = lx + w / 4;
            float rx1 = rx - w / 4;

            float duntaiH = 2;
            float dunshenH = 10;
            float zhuangH = 18;
            float offY = ty + duntaiH + dunshenH + zhuangH + 2;

            // r 桩的直径，从左到右，间隔 + 桩 + 间隔 + 桩 + 间隔，n行桩，n+1行间隔
            float r = w / (tls.Count * 2 + 1);

            float xl = lx + r; // 最左一行桩
            float xr = rx - 2 * r; // 最右一行桩
            float mx = (xl + xr) / 2;  // 三行桩时，中间一行
            float mx1 = xl + 2 * r; // 四行、五行桩时的二行
            float mx2 = xr - 2 * r; // 四行、五行桩时的三行

            Pen p;
            if (pie.mIsDone)
                p = defaultPen;
            else
                p = redPen;

            if (r <= 1) // 面积太小，绘制一个矩形，墩高度50像素
            {
                g.DrawRectangle(p, lx, ty, w, 50);
                return;
            }

            // 绘制墩台
            g.DrawRectangle(p, lx, ty, w, duntaiH);

            // 绘制墩身，梯形
            ty += duntaiH;
            PointF[] pa = new PointF[] { new PointF(lx1, ty), new PointF(rx1, ty), new PointF(rx, ty + dunshenH), new PointF(lx, ty + dunshenH) };
            g.DrawPolygon(p, pa);

            ty += dunshenH;

            //绘制桩
            g.DrawRectangle(p, xl, ty, r, zhuangH);
            g.DrawRectangle(p, xr, ty, r, zhuangH);
            if (tls.Count == 3)
            {
                g.DrawRectangle(p, mx, ty, r, zhuangH);
            }
            else if (tls.Count == 4)
            {
                g.DrawRectangle(p, mx1, ty, r, zhuangH);
                g.DrawRectangle(p, mx2, ty, r, zhuangH);
            }
            else if (tls.Count ==5)
            {
                g.DrawRectangle(p, mx, ty, r, zhuangH);
                g.DrawRectangle(p, mx1, ty, r, zhuangH);
                g.DrawRectangle(p, mx2, ty, r, zhuangH);

            }

            // 承台正视图
            float chengtaiH = r * (tls[0] * 2 + 1);
            g.DrawRectangle(p, lx, offY, w, chengtaiH);

            List<PointF> pls = new List<PointF>();

            // drawHoleArray，生成若干桩的中心点坐标
            pls.AddRange(drawHoleArray(r, xl, offY, chengtaiH, tls[0]));

            if (tls.Count == 3)
            {
                pls.AddRange(drawHoleArray(r, mx, offY, chengtaiH, tls[1]));
            }
            else if (tls.Count == 4)
            {
                pls.AddRange(drawHoleArray(r, mx1, offY, chengtaiH, tls[1]));
                pls.AddRange(drawHoleArray(r, mx2, offY, chengtaiH, tls[2]));
            }
            else if (tls.Count == 5)
            {
                pls.AddRange(drawHoleArray(r, mx1, offY, chengtaiH, tls[1]));
                pls.AddRange(drawHoleArray(r, mx, offY, chengtaiH, tls[2]));
                pls.AddRange(drawHoleArray(r, mx2, offY, chengtaiH, tls[3]));
            }
            pls.AddRange(drawHoleArray(r, xr, offY, chengtaiH, tls[tls.Count - 1]));

            int i = 0;
            foreach (PointF pf in pls)
            {
                if (i < pie.mStubDone.Count && pie.mStubDone[i])
                    p = redPen;
                else
                    p = defaultPen;
                g.DrawEllipse(p, pf.X, pf.Y, r, r);
                i++;
            }

        }

        public Matrix getTransM(float viewCenterX, float viewRadius, float winWidth, out float scaleX, float windowHOffset = 0)
        {
            Matrix m = new Matrix();
            scaleX = winWidth / 2 / viewRadius;
            m.Translate(winWidth / 2, windowHOffset);
            m.Scale(winWidth / 2, 1);
            m.Scale(1 / viewRadius, 1);
            m.Translate(-viewCenterX, 0);
            return m;
        }

        Matrix getInvTransM(float viewCenterX, float viewRadius, float winWidth, float windowHOffset = 0)
        {
            Matrix m = new Matrix();
            m.Translate(viewCenterX, 0);
            m.Scale(viewRadius, 1);
            m.Scale(2 / winWidth, 1);
            m.Translate(-winWidth / 2, -windowHOffset);
            return m;
        }

        private void getGlobalPos(float panelwidth, double centerMileage, double radiusMileage, int mx, int my, out CRailwayProject prj, out CRailwayDWProj dwp)
        {

            //float titleHeight = 30;
            //float scaleX;
            float height = 60;
            prj = null;
            dwp = null;
            Pen p = new Pen(Color.Black, 2);
            p.StartCap = LineCap.RoundAnchor;
            p.EndCap = LineCap.RoundAnchor;

            //Matrix m = getTransM((float)centerMileage, (float)radiusMileage, width, out scaleX, 50);
            Matrix im = getInvTransM((float)centerMileage, (float)radiusMileage, panelwidth, 50);

            PointF[] sd = { new PointF(0, 0) }; // 窗口
            sd[0].X = mx;
            sd[0].Y = my;
            im.TransformPoints(sd);
            int FXindex = (int)(sd[0].Y / height * 3);
            //Console.WriteLine("Click x=" + mx + " y= " + my + "\t global mileage = " + sd[0].X + " FXindex =" + FXindex);
            if (FXindex >= 2)
            {
                dwp = mRWScene.GetPierByMileage(sd[0].X);

                //if (rp == null)
                //    return;
                //skinAnimator1.WaitAllAnimations();
                //skinAnimator1.Hide(advPropertyGrid1);

                //pictureBox2.Image = generate2DCode(@"http://jqmis.cn/S/" + rp.mSerialNo);
                //advPropertyGrid1.SelectedObject = rp;
                //showProjectDetail(rp);
                //skinAnimator1.WaitAllAnimations();
                //skinAnimator1.Show(advPropertyGrid1);
            }
            if (dwp == null)
            {
                prj = mRWScene.GetProjectByMileage(sd[0].X);
                //if (rp == null)

                //skinAnimator1.WaitAllAnimations();
                //skinAnimator1.Hide(advPropertyGrid1);

                //pictureBox2.Image = generate2DCode(@"http://jqmis.cn/S/" + rp.mSerialNo);
                //advPropertyGrid1.SelectedObject = rp;
                //skinAnimator1.WaitAllAnimations();
                //skinAnimator1.Show(advPropertyGrid1);
            }
            //gRWScene.GetAllNavPathbyMileage
            //return sd[0].X;

        }
        #endregion

    }
}
