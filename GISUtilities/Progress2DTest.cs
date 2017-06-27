using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Drawing.Drawing2D;
using CCWin;


namespace GISUtilities
{
    //public partial class Form2 : Form
    //{
    //    public Form2()
    //    {
    //        InitializeComponent();
    //    }
    //}

    public partial class Progress2DTest : Skin_Mac
    {
        public const int num = 10;
        //定义点集
        public PointF[] points = new PointF[num];
        //判定是否为中心点
        public int[] iscenter = new int[num];
        //定义相似度矩阵
        public double[,] similarmatrix = new double[num, num];
        //定义消息
        public double[,] msga = new double[num, num];
        public double[,] msgr = new double[num, num];
        public double[,] oldmsga = new double[num, num];
        public double[,] oldmsgr = new double[num, num];
        //定义中值
        public double pk = 0;
        //定义阻尼系数
        public const double dampcv = 0.5;
        public Progress2DTest()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.AllPaintingInWmPaint, true);
            //pictureBox1.ImageLocation = @"http://124.128.9.254:8888/jqmis/ProjectPhoto/066/2015/%E8%AE%B8%E4%B8%96%E6%89%AC20151231001902.jpg";
            //InitPoints();
            //InitMsga();

            ////第一步，创建相似度矩阵
            //CreateSimilarMatrix();
            //int i = 0;
            //while (i < 2000)
            //{
            //    //第二步，更新消息
            //    GetOldMsg();
            //    UpdateMsg();
            //    DampMsg();
            //    //第三步，判断中心点
            //    JudgeCenter();
            //    i++;
            //}
            //for (i = 0; i < num; i++) {
            //    Console.Write(iscenter[i] + "\t");
            //}
            //MessageBox.Show("计算结束  ");
        }

        #region useless


        /// <summary>
        /// 初始化点集
        /// </summary>
        private void InitPoints()
        {
            points[0].X = (float)0.1;
            points[0].Y = (float)0.1;
            points[1].X = (float)0.3;
            points[1].Y = (float)0.1;
            points[2].X = (float)0.3;
            points[2].Y = (float)0.3;
            points[3].X = (float)0.1;
            points[3].Y = (float)0.3;
            points[4].X = (float)0.2;
            points[4].Y = (float)0.2;
            points[5].X = (float)0.5;
            points[5].Y = (float)0.5;
            points[6].X = (float)0.7;
            points[6].Y = (float)0.5;
            points[7].X = (float)0.7;
            points[7].Y = (float)0.7;
            points[8].X = (float)0.5;
            points[8].Y = (float)0.7;
            points[9].X = (float)0.6;
            points[9].Y = (float)0.6;

            for (int k = 0; k < num; k++)
                iscenter[k] = 0;
        }
        /// <summary>
        /// 初始化消息
        /// </summary>
        private void InitMsga()
        {
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    msga[i, j] = 0;
                    msgr[i, j] = 0;
                    oldmsga[i, j] = 0;
                    oldmsgr[i, j] = 0;
                }
            }
        }

        /// <summary>
        /// 计算欧式距离的相反数
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private double CalDistant(PointF p1, PointF p2)
        {
            return -(p1.X - p2.X) * (p1.X - p2.X) - (p1.Y - p2.Y) * (p1.Y - p2.Y);
        }

        /// <summary>
        /// 创建相似度矩阵.这里有点疑惑，是不是先按照常规计算再去算Pk？？
        /// 而且这个中值是不是要排除0？先不管它，当不排除
        /// </summary>
        private void CreateSimilarMatrix()
        {
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    similarmatrix[i, j] = CalDistant(points[i], points[j]);
                }
            }
            double median = ReturnMedian(similarmatrix);
            pk = median / 2;                                         //取中值作为preference的值
            for (int i = 0; i < num; i++)
            {
                similarmatrix[i, i] = pk;
            }
        }
        /// <summary>
        /// 更新消息
        /// </summary>
        private void UpdateMsg()
        {
            //更新msgr.R(i,k)=S(i,k)- max{A(i,j)+S(i,j)}(j {1,2,……,N,但j≠k})
            for (int i = 0; i < num; i++)
            {
                for (int k = 0; k < num; k++)
                {
                    //首先得到max{A(i,j)+S(i,j)}(j!=k)
                    double max_tmp;
                    //赋初值
                    if (k == 0)
                    {
                        max_tmp = msga[i, 1] + similarmatrix[i, 1];
                    }
                    else
                    {
                        max_tmp = msga[i, 0] + similarmatrix[i, 0];
                    }
                    for (int j = 0; j < num; j++)
                    {
                        if (j != k)
                        {
                            if (max_tmp < (msga[i, j] + similarmatrix[i, j]))
                            {
                                max_tmp = msga[i, j] + similarmatrix[i, j];
                            }
                        }
                    } //end get the max

                    msgr[i, k] = similarmatrix[i, k] - max_tmp;
                }
            }
            //更新msgr[k,k].R(k,k)=P(k)-max{A(k,j)+S(k,j)} (j {1,2,……,N,但j≠k})
            for (int i = 0; i < num; i++)
            {
                double max_tmp;
                //赋初值
                if (i == 0)
                {
                    max_tmp = msga[i, 1] + similarmatrix[i, 1];
                }
                else
                {
                    max_tmp = msga[i, 0] + similarmatrix[i, 0];
                }
                for (int j = 0; j < num; j++)
                {
                    if (j != i)
                    {
                        if ((msga[i, j] + similarmatrix[i, j]) > max_tmp)
                        {
                            max_tmp = msga[i, j] + similarmatrix[i, j];
                        }
                    }
                }
                msgr[i, i] = pk - max_tmp;
            }
            //为容易看先分开写
            //更新msga.A(i,k)=min{0,R(k,k)+  (j {1,2,……,N,但j≠i且j≠k})
            for (int i = 0; i < num; i++)
            {
                for (int k = 0; k < num; k++)
                {
                    //求得max部分
                    double sum_tmp = 0;
                    for (int j = 0; j < num; j++)
                    {
                        if (j != i)
                        {
                            if (msgr[j, k] > 0)
                            {
                                sum_tmp += msgr[j, k];
                            }
                        }
                    } //end 求max部分
                    double addtmp = msgr[k, k] + sum_tmp;
                    if (addtmp < 0)
                    {
                        msga[i, k] = addtmp;
                    }
                    else
                    {
                        msga[i, k] = 0;
                    }
                }
            }//end 更新msga
        } //end updatemsg
        /// <summary>
        /// 保存到旧消息数组
        /// </summary>
        private void GetOldMsg()
        {
            for (int i = 0; i < num; i++)
            {
                for (int k = 0; k < num; k++)
                {
                    oldmsga[i, k] = msga[i, k];
                    oldmsgr[i, k] = msgr[i, k];
                }
            }
        }

        /// <summary>
        /// 加入阻尼系数
        /// </summary>
        private void DampMsg()
        {
            for (int i = 0; i < num; i++)
            {
                for (int k = 0; k < num; k++)
                {
                    msga[i, k] = (1 - dampcv) * msga[i, k] + dampcv * oldmsga[i, k]; ;
                    msgr[i, k] = (1 - dampcv) * msgr[i, k] + dampcv * oldmsgr[i, k];
                }
            }
        }

        /// <summary>
        /// 判断是否为中心点。
        /// </summary>
        private void JudgeCenter()
        {
            for (int k = 0; k < num; k++)
            {
                if (msga[k, k] + msgr[k, k] >= 0)  //判定条件。。
                    iscenter[k] = 1;
            }
        }

        /// <summary>
        /// 返回二维数组的中值
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private double ReturnMedian(double[,] matrix)
        {
            double median = 0;
            double[] list = new double[matrix.GetLength(0) * matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    list[matrix.GetLength(0) * i + j] = matrix[i, j];
                }
            }
            Array.Sort(list);
            if (list.Length % 2 == 0)
            {
                median = (list[list.Length / 2] + list[list.Length / 2 - 1]) * 0.5;
            }
            else
            {
                median = list[Convert.ToInt32((list.Length - 1) * 0.5)];
            }
            return median;
        }

        /// <summary>
        /// 冒泡排序法
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public double[] BubbleSort(double[] list)
        {
            int i;
            double temp;
            for (int j = 0; j < list.Length; j++)
            {
                for (i = list.Length - 1; i > j; i--)
                {
                    if (list[j] < list[i])
                    {
                        temp = list[j];
                        list[j] = list[i];
                        list[i] = temp;
                    }
                }
            }
            return list;
        }
        #endregion

        private void CreatePictures()
        {
            try
            {
                //Graphics graphics = this.panel1.CreateGraphics();
                //IntPtr hRefDC = graphics.GetHdc();
                //Metafile metaFile = new Metafile(hRefDC,
                //    new RectangleF(0, 0, this.panel1.ClientSize.Width, this.panel1.ClientSize.Height),
                //    MetafileFrameUnit.Pixel,
                //    EmfType.EmfPlusDual);
                //graphics.ReleaseHdc();

                for (int i = 0; i <= 100; i++)
                {
                    Bitmap bmp = new Bitmap(32, 32);

                    Graphics g = Graphics.FromImage(bmp);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;


                    //graphics.DrawRectangle(Pens.Blue,
                    //    100 - (this.panel1.Width - this.panel1.ClientSize.Width) / 2,
                    //    100 - (this.panel1.Height - this.panel1.ClientSize.Height) / 2,
                    //    100,
                    //    100);
                    //HatchBrush hb = new HatchBrush(HatchStyle.LightUpwardDiagonal, Color.Black, Color.White);

                    g.FillEllipse(Brushes.Gray, 1f, 1f, 30, 30);
                    g.DrawEllipse(new Pen(Color.Black, 1f), 1f, 1f, 30, 30);
                    if (i > 0)
                        g.FillPie(Brushes.Yellow, 1f, 1f, 30, 30, -90, (int)(3.6 * i + 0.5));
                    g.DrawString(i.ToString(), new Font("宋体", 12), Brushes.Red, new PointF(5f, 5f));

                    //g.Dispose();
                    bmp.Save(@"D:\Pie" + i + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    //g.Clear(co)
                    //metaFile.Save(@"D:\Pie"+i+".png");
                }
                //metaFile.GetMetafileHeader()
                //int size = metaFile.GetMetafileHeader().MetafileSize;
                //byte[] p = new byte[size];
                //IntPtr hEnhmetafile = metaFile.GetHenhmetafile();
                //int ret = GetEnhMetaFileBits(hEnhmetafile, size, ref p[0]);
                //if (ret == 0)
                //{
                //    metaFile.Dispose();
                //    return null;
                //}
                //metaFile.Dispose();

                //System.Runtime.InteropServices.ComTypes.IStream stream = null;
                //CreateStreamOnHGlobal(IntPtr.Zero, true, out stream);
                //if (stream != null)
                //{
                //    stream.Write(p, p.Length, IntPtr.Zero);
                //}
                Console.WriteLine("Done");
                //return stream;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error");
                //return null;
            }
        }

        Matrix getTransM(float viewCenterX, float viewRadius, float winWidth, out float scaleX, float windowHOffset = 0)
        {
            Matrix m = new Matrix();
            scaleX = winWidth / 2 / viewRadius;
            m.Translate(winWidth / 2, windowHOffset);
            m.Scale(winWidth / 2, 1);
            m.Scale(1 / viewRadius, 1);
            m.Translate(-viewCenterX, 0);   
            return m;
        }
        Matrix getInvTransM(float viewCenterX, float viewRadius, float winWidth,  float windowHOffset = 0)
        {
            Matrix m = new Matrix();
            m.Translate(viewCenterX, 0);
            m.Scale( viewRadius, 1);
            m.Scale(2/winWidth , 1);
            m.Translate(-winWidth / 2, -windowHOffset);    
            return m;
        }

        private List<PointF> drawHoleArray(float r, float w, float x, float y, int num)
        {
            List<PointF> pls = new List<PointF>();
            float yt = y + r;
            float yb = y + w - r;
            float my = yt + (yb - yt) / 2;
            float my1 = yt +  (yb - yt) / 3;
            float my2 = yt + 2 * (yb - yt) / 3;

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
            return pls;
        }

        private void drawOnePier(Graphics g, float cx, float ty, float w, List<int> zhuangType)
        {
            if (w < 4 || zhuangType.Count < 2)
                return;
            float lx = cx - w / 2;
            float rx = cx + w / 2;

            float lx1 = lx + w / 4;
            float rx1 = rx - w / 4;

            float duntaiH = 3;
            float dunshenH = 17;
            float zhuangH = 10;
            float offY = ty + duntaiH + dunshenH + zhuangH + 2;

            float r = w / 8;

            float xl = lx + r;
            float xr = rx - 2 * r;
            float mx = (xl + xr) / 2;
            float mx1 = xl + (xr - xl) / 3;
            float mx2 = xl + 2 * (xr - xl) / 3;

            Pen p = new Pen(Color.Black, 1);
            if (r <= 1)
            {
                g.DrawRectangle(p, lx, ty, w, 50);
                return;
            }

            g.DrawRectangle(p, lx, ty, w, duntaiH);

            ty += duntaiH;
            PointF[] pa = new PointF[] { new PointF(lx1, ty), new PointF(rx1, ty), new PointF(rx, ty + dunshenH), new PointF(lx, ty + dunshenH) };
            g.DrawPolygon(p, pa);

            ty += dunshenH;

            g.DrawRectangle(p, xl, ty, r, zhuangH);
            g.DrawRectangle(p, xr, ty, r, zhuangH);
            if (zhuangType.Count == 3)
            {
                g.DrawRectangle(p, mx, ty, r, zhuangH);
            }
            else if (zhuangType.Count == 4)
            {
                g.DrawRectangle(p, mx1, ty, r, zhuangH);
                g.DrawRectangle(p, mx2, ty, r, zhuangH);
            }

            g.DrawRectangle(p, lx, offY, w, w+r);

            List<PointF> pls = new List<PointF>();

            pls.AddRange(drawHoleArray(r, w, xl, offY, zhuangType[0]));
            pls.AddRange(drawHoleArray(r, w, xr, offY, zhuangType[0])); // 最后一列一定与第一列桩的个数相同
            if (zhuangType.Count == 3)
            {
                pls.AddRange(drawHoleArray(r, w, mx, offY, zhuangType[1]));
            }
            else if (zhuangType.Count == 4)
            {
                pls.AddRange(drawHoleArray(r, w, mx1, offY, zhuangType[1]));
                pls.AddRange(drawHoleArray(r, w, mx2, offY, zhuangType[2]));
            }

            foreach (PointF pf in pls)
            {
                g.DrawEllipse(p, pf.X, pf.Y, r, r);
            }

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

            Rectangle rect = e.ClipRectangle;
            BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
            BufferedGraphics myBuffer = currentContext.Allocate(e.Graphics, e.ClipRectangle);
            Graphics g = myBuffer.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            g.Clear(this.BackColor);
            //foreach (IShape drawobject in doc.drawObjectList)
            //{
            //    if (rect.IntersectsWith(drawobject.Rect))
            //    {
            //        drawobject.Draw(g);
            //        if (drawobject.TrackerState == config.Module.Core.TrackerState.Selected
            //            && this.CurrentOperator == Enum.Operator.Transfrom)//仅当编辑节点操作时显示图元热点
            //        {
            //            drawobject.DrawTracker(g);
            //        }
            //    }
            //}

            
            
            
            
            //Graphics myGraphics = e.Graphics;
            //List<double> ld = new List<double>();
            //ld.Add(0.5);
            //ld.Add(0.6);
            //ld.Add(0.3);
            //List<string> ls = new List<string>();
            //ls.Add("测试1");
            //ls.Add("测试2");
            //ls.Add("测试3");



            g.DrawString("青阳隧道", new Font("宋体", 12), Brushes.Red, new PointF(5, 5));
            for (float f = 50; f < 500; f += 50)
                g.DrawLine(Pens.Black, 0, f, 500, f);
            //myGraphics.TranslateTransform(50, 50);
            float wh = 600;
            float scaleX;


            Matrix m = getTransM(5000, 2000, 500, out scaleX,150);
            Matrix im = getInvTransM(5000, 2000, 500, 150);
            drawOnePier(g, 100, 50, 3, new List<int> { 4, 4, 4, 4 });
            drawOnePier(g, 150, 50, 7, new List<int> { 4, 4, 4, 4 });
            drawOnePier(g, 200, 50, 14, new List<int> { 4, 4, 4, 4 });
            drawOnePier(g, 250, 50, 30, new List<int> { 4, 4, 4, 4 });
            drawOnePier(g, 300, 50, 20, new List<int> { 4, 4 });
            drawOnePier(g, 350, 50, 20, new List<int> { 4, 2, 4 });
            drawOnePier(g, 400, 50, 20, new List<int> { 4, 4, 4, 4 });

            myBuffer.Render(e.Graphics);
            g.Dispose();
            myBuffer.Dispose();//释放资源 

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
            double width = 80;
            //myGraphics.Transform = m;
            //draw_SubTunnelWithoutDate(myGraphics, ls, ld, width, m, 2000, 2000, 10, 0, scaleX);
            //draw_SubTunnelWithoutDate(myGraphics, ls, ld, width, m, 5000, -1000, 100, 0, scaleX);
            //draw_SubTunnelWithoutDate(myGraphics, ls, ld, width, m, 5000, 1500, 10, 0, scaleX);
            //draw_SubTunnelWithoutDate(myGraphics, ls, ld, width, m, 7500, -1000, 100, 0, scaleX);
            //draw_SubTunnelWithoutDate(myGraphics, ls, ld, width, m, 5000, 300, 10, 1, scaleX);
            //draw_SubTunnelWithoutDate(myGraphics, ls, ld, width, m, 450, 600, 10, 1);
            //draw_SubTunnelWithoutDate(myGraphics, ls, ld, width, m, 250, 300, 10, 2);
            //draw_SubTunnelWithoutDate(myGraphics, ls, ld, width, m, 400, 480, 10, 3);
            //Matrix mId = new Matrix();

            //myGraphics.Transform = mId;



            //myGraphics.RotateTransform(30);
            //myGraphics.Transform = m;
            //draw_SubTunnel(myGraphics, ld, ls, 100, 150, 0);
            //Pen myPen = new Pen(Color.Red, 5);
            //myGraphics.DrawEllipse(myPen, 0, 0, 100, 50);
            //myGraphics.ScaleTransform(1, 0.5f);
            //myGraphics.TranslateTransform(50, 0);
            //myGraphics.RotateTransform(30);
            //myPen.Color = Color.Blue;
            //myGraphics.DrawEllipse(myPen, 0, 0, 100, 50);
            //myPen.Dispose();

            //GraphicsPath myGraphicsPath = new GraphicsPath();
            //Rectangle myRectangle = new Rectangle(0, 0, 60, 60);
            ////myGraphicsPath.
            //myGraphicsPath.AddRectangle(myRectangle);

            // Fill the path on the new coordinate system.
            // No local transformation
            //myGraphics.FillPath(new SolidBrush(Color.Yellow), myGraphicsPath);
            //Pen widenPen = new Pen(Color.Black, 10);
            //Matrix widenMatrix = new Matrix();
            //widenMatrix.Translate(50, 50);
            //myPath.Widen(widenPen, widenMatrix, 1.0f);
            //// Draw the widened path to the screen in red.
            //e.Graphics.FillPath(new SolidBrush(Color.Red), myPath);
        }

        //private double bwidth = 400;
        //private double bheight = 150;
        //private float scale = 1.0f;
        private Color[] backColor= new Color[]{Color.Red,Color.Green,Color.Blue,Color.Yellow,Color.Purple};
        
        //private double 
        /// <summary>
        /// 绘制隧道进度
        /// </summary>
        /// <param name="myGraphics"></param>
        /// <param name="description">分项工程描述</param>
        /// <param name="done">分项完成百分比</param>
        /// <param name="th">绘制宽度</param>
        /// <param name="pm">隧道坐标系矩阵</param>
        /// <param name="startM">起始里程，米</param>
        /// <param name="endM">终止里程，米</param>
        /// <param name="unitM">围岩等级，米</param>
        /// <param name="mode">绘制模式，小里程-》大里程，角度</param>
        private void draw_SubTunnelWithoutDate(Graphics myGraphics, List<string> description, List<double> done, double th, Matrix pm, double startM, double totalLen ,double unitM, int mode, float scale = 1)
        {
            //float totalLen = (float)Math.Abs(endM - startM) ;
            double unitLen = unitM * scale ;
            bool isReverse = totalLen < 0;
            
            double sx,ex;
            if (isReverse)
            {
                totalLen = -totalLen;
                sx = startM - totalLen;
                ex = startM;
            }
            else
            {
                sx = startM;
                ex = startM + totalLen;
            }
            PointF[] sd = { new PointF(0, 100), new PointF(0, 100) }; // 窗口
            sd[0].X =(float)sx;
            sd[1].X = (float)ex;
            pm.TransformPoints(sd);
            if (sd[1].X < 0 || sd[0].X > 700) // 700 window width
                return;
            
            float sy  = 30;
            
            //PointF pf = new PointF((float)sx, sy);
            //button2.Location = new System.Drawing.Point((int)(sd[0].X),50);
            //button2.Size = new System.Drawing.Size((int)(sd[1].X - sd[0].X),20);
            //button2.Text = sx + "~" + ex;
            //int doneLen;
            float unitHeight = (float)(th / (done.Count+1));
            
            SolidBrush notDoneB = new SolidBrush(Color.LightGray);
            SolidBrush doneB = new SolidBrush(Color.FromArgb(50, Color.Red));

            Pen p = new Pen(Color.Black,2);
            //p.StartCap = LineCap.RoundAnchor;
            //p.EndCap = LineCap.ArrowAnchor;            
            //p.DashStyle = DashStyle.DashDot;

            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;  // 更正： 垂直居中
            format.Alignment = StringAlignment.Center;      // 水平居中

            RectangleF areaf;
            areaf = new RectangleF(sd[0].X, sy, sd[1].X - sd[0].X, unitHeight);
            

            switch (mode)
            {
                case 0:  // 小里程到大里程
             
                    
                    

                    //if (isReverse)
                    //    myGraphics.DrawLine(p, sd[1].X, sy + (float)unitHeight / 2, sd[0].X, sy + unitHeight / 2);
                    //else
                    //    myGraphics.DrawLine(p, sd[0].X, sy + (float)unitHeight / 2, sd[1].X, sy + unitHeight / 2);

                    myGraphics.DrawLine(p,sd[0].X, sy, sd[0].X, sy + unitHeight);

                    myGraphics.DrawString(sx + "~" + ex, new Font("宋体", 12), Brushes.Black, areaf, format);
                    
                    sy += unitHeight;
                    myGraphics.Transform = pm;
                    // 逐条绘制分项工程
                    for (int i = 0; i < done.Count; i++)
                    {

                        myGraphics.DrawRectangle(Pens.Black, (float)sx, sy, (float)totalLen, unitHeight);

                        if (unitLen < 4)
                        {
                            if (isReverse)
                            {
                                myGraphics.FillRectangle(notDoneB, (float)sx, sy, (float)(totalLen * (1-done[i])), unitHeight);
                                myGraphics.FillRectangle(new SolidBrush(backColor[i]), (float)(sx + totalLen * (1-done[i])) + 1, sy, (float)(totalLen * done[i]), unitHeight);
                            }
                            else
                            {
                                myGraphics.FillRectangle(new SolidBrush(backColor[i]), (float)sx, sy, (float)(totalLen * done[i]), unitHeight);
                                myGraphics.FillRectangle(notDoneB, (float)(sx + totalLen * done[i]) + 1, sy, (float)(totalLen * (1 - done[i])), unitHeight);
                            }
                        }
                        else
                        {
                            float j;
                            if (isReverse)
                            {
                                for (j = 0; j < (float)(totalLen *(1- done[i])); j += (float)unitM)
                                {
                                    
                                    myGraphics.DrawRectangle(Pens.Black, (float)sx + j, sy + 1, (float)unitM - 2, unitHeight - 3);
                                }

                                //myGraphics.DrawString(description[i] + " " + Math.Round(done[i] * 100, 2) + "%", new Font("宋体", 12), Brushes.Red, new PointF(5, y + 5));
                                for (; j < (float)(totalLen); j += (float)unitM)
                                {
                                    myGraphics.FillRectangle(new SolidBrush(backColor[i]), (float)sx + j, sy + 1, (float)unitM - 2, unitHeight - 3);
                                    myGraphics.DrawRectangle(Pens.Black, (float)sx + j, sy + 1, (float)unitM - 2, unitHeight - 3);
                                }
                            }
                            else
                            {


                                for (j = 0; j < (float)(totalLen * done[i]); j += (float)unitM)
                                {
                                    myGraphics.FillRectangle(new SolidBrush(backColor[i]), (float)sx + j, sy + 1, (float)unitM - 2, unitHeight - 3);
                                    myGraphics.DrawRectangle(Pens.Black, (float)sx + j, sy + 1, (float)unitM - 2, unitHeight - 3);
                                }

                                //myGraphics.DrawString(description[i] + " " + Math.Round(done[i] * 100, 2) + "%", new Font("宋体", 12), Brushes.Red, new PointF(5, y + 5));
                                for (; j < (float)(totalLen); j += (float)unitM)
                                {
                                    myGraphics.DrawRectangle(Pens.Black, (float)sx + j, sy + 1, (float)unitM - 2, unitHeight - 3);
                                }
                            }
                        }
                        //myGraphics.DrawRectangle(Pens.Black, (int)(totalLen * done[i]) + 1, sy, (int)(totalLen * done[i]) % height - 1, height - 1);
                        sy += unitHeight;
                    }
                    break;
                case 1:// 大里程到小里程
                    //m.Translate(sx, 0);
                    //pm.Multiply(m);
                    //myGraphics.Transform = pm;

                    //areaf = new RectangleF(0, sy, totalLen,unitHeight);

                    //p.StartCap = LineCap.ArrowAnchor;
                    //p.EndCap = LineCap.RoundAnchor;
                    //myGraphics.DrawLine(p, 0, sy+ unitHeight /2, totalLen, sy + unitHeight /2);
                    //myGraphics.DrawString(startM + "~" + endM, new Font("宋体", 12), Brushes.Black, areaf, format);
                    //sy += unitHeight;

                    //for (int i = 0; i < done.Count; i++)
                    //{
                    //    myGraphics.DrawRectangle(Pens.Black, 0, sy, totalLen, unitHeight);
                    //    if (unitLen < 4)
                    //    {
                    //        myGraphics.FillRectangle(notDoneB, 0, sy, (float)(totalLen * (1 - done[i])), unitHeight);
                    //        myGraphics.FillRectangle(new SolidBrush(backColor[i]), (float)(totalLen * (1 - done[i])) + 1, sy, (float)(totalLen * done[i]), unitHeight);
                    //    }
                    //    else
                    //    {
                    //        float j;
                    //        for (j = 1; j < (float)(totalLen * done[i]); j += unitLen)
                    //        {

                    //            myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, unitHeight - 3);
                    //        }


                    //        for (; j < (float)(totalLen); j += unitLen)
                    //        {
                    //            myGraphics.FillRectangle(new SolidBrush(backColor[i]), j, sy + 1, unitLen - 2, unitHeight - 3);
                    //            myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, unitHeight - 3);
                    //        }
                    //    }
                    //    //myGraphics.DrawString(description[i] + " " + Math.Round(done[i] * 100, 2) + "%", new Font("宋体", 12), Brushes.Red, new PointF(5, y + 5));
                    //    sy += unitHeight;
                    //}
                    break;
                case 2:// 45度 斜井
                   //m.Translate(sx, (int)(th / 2));
                   //m.Rotate(90);
                   //m.Shear(0, -0.5f);
                   // pm.Multiply(m);
                   // myGraphics.Transform = pm;

                   // areaf = new RectangleF(0, sy, totalLen,unitHeight);

                   // p.StartCap = LineCap.ArrowAnchor;
                   // p.EndCap = LineCap.RoundAnchor;
                   // myGraphics.DrawLine(p, 0, sy+ unitHeight /2, totalLen, sy + unitHeight /2);
                   // myGraphics.DrawString(startM + "~" + endM, new Font("宋体", 12), Brushes.Black, areaf, format);
                   // sy += unitHeight;
                   // //height = height / 2;
                   // //sy = sy / 2;
                   // for (int i = 0; i < done.Count; i++)
                   // {
                   //     myGraphics.DrawRectangle(Pens.Black, 0, sy, totalLen, unitHeight);

                   //     if (unitLen < 4)
                   //     {
                   //         myGraphics.FillRectangle(notDoneB, 0, sy, (float)(totalLen * (1 - done[i])), unitHeight);
                   //         myGraphics.FillRectangle(new SolidBrush(backColor[i]), (float)(totalLen * (1 - done[i])) + 1, sy, (float)(totalLen * done[i]), unitHeight);
                   //     }
                   //     else
                   //     {
                   //         float j;
                   //         for (j = 1; j < (float)(totalLen * done[i]); j += unitLen)
                   //         {

                   //             myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, unitHeight - 3);
                   //         }


                   //         for (; j < (float)(totalLen); j += unitLen)
                   //         {
                   //             myGraphics.FillRectangle(new SolidBrush(backColor[i]), j, sy + 1, unitLen - 2, unitHeight - 3);
                   //             myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, unitHeight - 3);
                   //         }
                   //     }
                   //     //myGraphics.FillRectangle(notDoneB, new Rectangle(0, sy, (int)(totalLen * (1 - done[i])), height));
                   //     //myGraphics.DrawRectangle(Pens.BlanchedAlmond, new Rectangle((int)(totalLen * (1 - done[i])) + 1, sy, (int)(totalLen * done[i]), height));
                   //     //myGraphics.FillRectangle(new SolidBrush(Color.LightBlue), new Rectangle((int)(totalLen * (1 - done[i])) + 1, sy, (int)(totalLen * done[i]), height));
                   //     //myGraphics.DrawString(Math.Round(done[i] * 100, 2) + "%", new Font("宋体", 12), Brushes.Red, new PointF(5, y + 5));
                   //     sy += unitHeight;
                   // }
                    break;
                case 3: // 305度 斜井
                   //m.Translate(sx, (int)(- th / 2));
                   //m.Rotate(270);
                   //m.Shear(0, 0.5f);
                   // pm.Multiply(m);
                   // myGraphics.Transform = pm;
                   // areaf = new RectangleF(0, sy, totalLen,unitHeight);

                   // p.StartCap = LineCap.ArrowAnchor;
                   // p.EndCap = LineCap.RoundAnchor;
                   // myGraphics.DrawLine(p, 0, sy+ unitHeight /2, totalLen, sy + unitHeight /2);
                   // myGraphics.DrawString(startM + "~" + endM, new Font("宋体", 12), Brushes.Black, areaf, format);
                   // sy += unitHeight;
                   // //height = height / 2;
                   // //sy = sy / 2;
                   // for (int i = 0; i < done.Count; i++)
                   // {
                   //     myGraphics.DrawRectangle(Pens.Black, 0, sy, totalLen, unitHeight);

                   //     if (unitLen < 4)
                   //     {
                   //         myGraphics.FillRectangle(notDoneB, 0, sy, (float)(totalLen * (1 - done[i])), unitHeight);
                   //         myGraphics.FillRectangle(new SolidBrush(backColor[i]), (float)(totalLen * (1 - done[i])) + 1, sy, (float)(totalLen * done[i]), unitHeight);
                   //     }
                   //     else
                   //     {
                   //         float j;
                   //         for (j = 1; j < (float)(totalLen * done[i]); j += unitLen)
                   //         {

                   //             myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, unitHeight - 3);
                   //         }


                   //         for (; j < (float)(totalLen); j += unitLen)
                   //         {
                   //             myGraphics.FillRectangle(new SolidBrush(backColor[i]), j, sy + 1, unitLen - 2, unitHeight - 3);
                   //             myGraphics.DrawRectangle(Pens.Black, j, sy + 1, unitLen - 2, unitHeight - 3);
                   //         }
                   //     }
                   //     //myGraphics.FillRectangle(notDoneB, new Rectangle(0, sy, (int)(totalLen * (1 - done[i])), height));
                   //     //myGraphics.DrawRectangle(Pens.BlanchedAlmond, new Rectangle((int)(totalLen * (1 - done[i])) + 1, sy, (int)(totalLen * done[i]), height));
                   //     //myGraphics.FillRectangle(new SolidBrush(Color.LightBlue), new Rectangle((int)(totalLen * (1 - done[i])) + 1, sy, (int)(totalLen * done[i]), height));
                   //     //myGraphics.DrawString(Math.Round(done[i] * 100, 2) + "%", new Font("宋体", 12), Brushes.Red, new PointF(5, y + 5));
                   //     sy += unitHeight;
                   // }
                    break;       
                //case 4:
                //    m.Translate(x, 0);
                //    m.Multiply(pm);
                //    myGraphics.Transform = m;
                //    for (int i = 0; i < done.Count; i++)
                //    {
                        
                //        myGraphics.DrawRectangle(Pens.BlanchedAlmond, new Rectangle(0, y, (int)(totalLen * done[i]), height));
                //        myGraphics.FillRectangle(new SolidBrush(Color.LightBlue), new Rectangle(0, y, (int)(totalLen * done[i]), height));
                //        myGraphics.FillRectangle(notDoneB, new Rectangle((int)(totalLen * done[i]) + 1, y, (int)(totalLen * (1 - done[i])), height));
                //        myGraphics.DrawString(description[i] + " " + Math.Round(done[i] * 100, 2) + "%", new Font("宋体", Math.Abs(height - 10) / 2), Brushes.Red, new PointF(5, y + 5));
                //        y += height;
                //    }
                //    break;
                default:
                    //Console.WriteLine(startM + "\t" + endM + " tunnel type error");
                    break;
            }
            Matrix om = new Matrix();
            myGraphics.Transform = om;
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            panel1.Hide();
            dataGridView1.Hide();
            //skinAnimator1.WaitAllAnimations();
            //skinAnimator1.
            skinAnimator1.ShowSync(panel1);
            skinAnimator1.ShowSync(dataGridView1);
        }

        private void skinButton2_Click(object sender, EventArgs e)
        {
            skinAnimator1.Show(panel1);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //CreatePictures();
            //CServerWrapper.ConnectToServer();

            ////    string sqlstr = @"select FirmName, FirmTypeID,from (select * from FirmInfo)a, (select FirmTypeID,FirmTypeCategoryName from FirmTypeInfo)b where a.FirmTypeID=b.FirmTypeID and( FirmTypeCategoryName='单位' or FirmTypeCategoryName='分支机构');";  and Longitude > 10 AND Latitude > 10
            //string sqlstr = @"select FirmName ,a.FirmTypeID, CategoryCode, SerialNo, UpdateTime, Longitude, Latitude from (select * from FirmInfo)a, (select FirmTypeID,FirmTypeCategoryName from FirmTypeInfo)b where a.FirmTypeID=b.FirmTypeID and( FirmTypeCategoryName='单位' or FirmTypeCategoryName='分支机构') and Longitude > 10 AND Latitude > 10 order by a.FirmTypeID asc ;";

            //// + " where a.FirmTypeID=b.FirmTypeID and (FirmTypeCategoryName='单位' or FirmTypeCategoryName='分支机构') and Longitude > 10 AND latitude > 10 ;";
            //dataGridView1.DataSource = CServerWrapper.execSqlQuery(sqlstr);
            panel1.Hide();
            dataGridView1.Hide();
            //skinAnimator1.WaitAllAnimations();
            //skinAnimator1.
            skinAnimator1.ShowSync(panel1);
            skinAnimator1.ShowSync(dataGridView1);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            skinAnimator1.Hide(panel1);
            skinAnimator1.Hide(dataGridView1);
            skinAnimator1.WaitAllAnimations();
            //skinAnimator1.
            skinAnimator1.Show(panel1);
            skinAnimator1.Show(dataGridView1);
        }



    }
    public class ProgressGIS
    {
        private double bwidth = 400;
        private double bheight = 150;
        private double scale = 1.0;
        private void draw_Progress(Graphics myGraphics, List<double> progress, List<string> description, double tw, double th, int mode)
        {
            int totalLen = (int)tw;
            //int doneLen;
            int height = (int)(th / progress.Count);
            int y = -(int)(th / 2);
            SolidBrush notDoneB = new SolidBrush(Color.LightGray);
            for (int i = 0; i < progress.Count; i++)
            {
                myGraphics.DrawRectangle(Pens.BlanchedAlmond, new Rectangle(0, y, (int)(totalLen * progress[i]), height));
                myGraphics.FillRectangle(new SolidBrush(Color.LightBlue), new Rectangle(0, y, (int)(totalLen * progress[i]), height));
                myGraphics.FillRectangle(notDoneB, new Rectangle((int)(totalLen * progress[i]) + 1, y, (int)(totalLen * (1 - progress[i])), height));
                myGraphics.DrawString(description[i] + " " + Math.Round(progress[i] * 100, 2) + "%", new Font("宋体", Math.Abs(height - 10) / 2), Brushes.Red, new PointF(5, y + 5));
                y += height;
            }

        }
    }

}
