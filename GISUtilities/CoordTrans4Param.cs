using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GISUtilities
{

    public class CoordTrans4Param
    {
        public double[,] values = new double[4, 1];
        //{{dx},{dy},{dz},{rx},{ry},{rz},{k}}; 
        //public double dx,dy,dz,rx,ry,rz,k; 
        public void Set4Param(double dx, double dy, double r, double k)
        {
            this.dx = dx;
            this.dy = dy;
            this.c = r;
            this.d = k;
            //this.rx = this.ry = this.rz = 0;
        }

        private double[,] GetM() //M=Mx*My*Mz? or M=Mz*My*Mx? 
        {
            double[,] M = new double[2, 2] { { c, -d }, { d, c} };
            return M;
        }

        private double[,] specialMulti(double[,] m, double[,] X)
        {
            int rowNumM = m.GetLength(0);
            int colNumM = m.GetLength(1);
            int rowNumX = X.GetLength(0);
            int colNumX = X.GetLength(1);
            int lines = rowNumX / colNumM;
            double[,] mt = MatrixTool.Init(rowNumM, colNumX);
            double[,] subX = MatrixTool.Init(colNumM, colNumX);
            double[,] res = MatrixTool.Init(rowNumM * lines, colNumX);

            for (int i = 0; i < lines; i++)
            {
                MatrixTool.CopySub(X, i * colNumM, 0, colNumM, colNumX, ref subX, 0, 0);
                MatrixTool.Multi(m, subX, ref mt);
                MatrixTool.CopySub(mt, 0, 0, rowNumM, colNumX, ref res, i * rowNumM, 0);
            }
            return res;
        }
        private double[,] specialSub(double[,] m, double[,] X)
        {
            int rowNumM = m.GetLength(0);
            int colNumM = m.GetLength(1);
            int rowNumX = X.GetLength(0);
            int colNumX = X.GetLength(1);
            int lines = rowNumX / rowNumM;
            double[,] subX = MatrixTool.Init(rowNumM, colNumX);
            double[,] res = MatrixTool.Init(rowNumX, colNumX);

            for (int i = 0; i < rowNumX; i += rowNumM)
            {
                MatrixTool.CopySub(X, i, 0, rowNumM, colNumX, ref subX, 0, 0);
                MatrixTool.Sub(m, subX, ref subX);
                MatrixTool.CopySub(subX, 0, 0, rowNumM, colNumX, ref res, i, 0);
            }
            return res;
        }

        /// <summary>
        /// 获取观测方程。 -DeltaX - ((1+k)*RotateMatrix*X - Y)， 或者 Y = DeltaX + (1+k)*RotateMatrix*X (布尔沙方程)
        /// </summary>
        /// <param name="X">原始点</param>
        /// <param name="Y">目标点</param>
        /// <returns></returns>
        private double[,] GetF(double[,] X, double[,] Y)
        {
            double[,] f0;
            double[,] qx = MatrixTool.Init(X.GetLength(0), 1);
            double[,] K = { { -dx }, { -dy } };
            //double[,] S = { { 1  } };

            //MatrixTool.Multi(X, S, ref qx);
            double[,] M = GetM();
            qx = specialMulti(M, X);
            MatrixTool.Sub(qx, Y, ref qx);
            f0 = specialSub(K, qx);
            return f0;
        }

        /// <summary>
        /// 最小二乘误差矩阵
        /// [1 0  -y  x]
        /// [0 1   x  y]
        /// </summary>
        /// <param name="X"></param>
        /// <returns></returns>

        private double[,] GetB(double[,] X)
        {
            int rowNum = X.GetLength(0);
            double[,] B = MatrixTool.Init(rowNum, 4);
            //double[,] M = GetM();
            double[,] mi = MatrixTool.Ident(2);
            //double[,]  MK;

            //MK = specialMulti(M, X);

            for (int i = 0; i < rowNum; i += 2)
                MatrixTool.CopySub(mi, 0, 0, 2, 2, ref B, i, 0);

            for (int i = 0; i < rowNum; i += 2)
            {
                B[i, 2] = X[i , 0];
                B[i, 3] = - X[i+ 1, 0];
                B[i + 1, 2] = X[i+1, 0];
                B[i + 1, 3] = X[i , 0];
            }

            return B;
        }

        ///// <summary>
        ///// 残差矩阵
        ///// </summary>
        ///// <returns></returns>
        //private double[,] GetA()
        //{
        //    double[,] M = GetM();
        //    double[,] I2 = MatrixTool.Ident(2);
        //    double[,] A = MatrixTool.Init(2, 4);

        //    MatrixTool.MutliConst(ref I2, -1);
        //    //MatrixTool.MutliConst(ref M, (1 + k));
        //    MatrixTool.CopySub(M, 0, 0, 2, 2, ref A, 0, 0);
        //    MatrixTool.CopySub(I2, 0, 0, 2, 2, ref A, 0, 2);
        //    return A;
        //}
        //private double[,] GetV(double[,] X, double[,] Y, CoordTrans4Param dpp)
        //{
        //    int rowNum = X.GetLength(0);

        //    double[,] B, F, A, B2, B3, F2, V;
        //    double[,] AT = MatrixTool.Init(4, 2);

        //    A = GetA();
        //    MatrixTool.AT(A, ref AT);
        //    MatrixTool.MutliConst(ref AT, 1 / (1 + (1 + k) * (1 + k)));

        //    F = GetF(X, Y);
        //    B = GetB(X);
        //    B2 = MatrixTool.Init(2, 4);
        //    B3 = MatrixTool.Init(2, 1);
        //    F2 = MatrixTool.Init(rowNum, 1);
        //    for (int i = 0; i < rowNum / 2; i++)
        //    {
        //        MatrixTool.CopySub(B, i * 2, 0, 2, 4, ref B2, 0, 0);
        //        MatrixTool.Multi(B2, dpp.values, ref B3);
        //        MatrixTool.CopySub(B3, 0, 0, 2, 1, ref F2, i * 2, 0);
        //    }
        //    MatrixTool.Sub(F, F2, ref F2);
        //    V = specialMulti(AT, F2);
        //    return V;
        //}
        /// <summary> 
        /// X矩阵如下所示 
        ///  
        ///  
        /// </summary> 
        /// <param name="X"></param> 
        /// <param name="Y"></param> 
        /// <returns></returns> 
        public double CalculateTrans4Param(double[,] X, double[,] Y)
        {
            int PtNum = X.GetLength(0) / 2;
            double[,] B;
            double[,] F;
            double[,] BT = MatrixTool.Init(4, 2 * PtNum);
            double[,] BTB = MatrixTool.Init(4, 4);
            double[,] BTF = MatrixTool.Init(4, 1);


            //init pararm 
            CoordTrans4Param dpp = new CoordTrans4Param();
            Set4Param(0, 0, 0, 0);
            //this.SetRotationParamMM(0, 0, 0);
            //debug 
            //this.TransCoord(X[0,0],X[1,0],X[2,0],out x2,out y2,out z2); 
            int round = 0;
            while (round++ < 20)
            {
                F = GetF(X, Y);
                B = GetB(X);
                MatrixTool.AT(B, ref BT);

                MatrixTool.Multi(BT, B, ref BTB);
                MatrixTool.Inv(BTB);
                MatrixTool.Multi(BT, F, ref BTF);
                MatrixTool.Multi(BTB, BTF, ref dpp.values);
                if (dpp.isSmall())
                    break;
                else
                    MatrixTool.Add(this.values, dpp.values, ref this.values);
            }
            Console.WriteLine(round);
            return -1;
            //this.TransCoord(X[0,0],X[1,0],X[2,0],out x2,out y2,out z2); 
            //double[,] V = GetV(X, Y, dpp);

            //double vMax = -1;
            //for (int i = 0; i < V.GetLength(0); i++)
            //{
            //    if (Math.Abs(V[i, 0]) > vMax)
            //        vMax = Math.Abs(V[i, 0]);
            //}

            //return vMax;
        }
        private bool isSmall()
        {
            double s = 0;
            for (int i = 0; i < values.GetLength(0); i++)
                s += Math.Abs(values[i, 0]);
            if (s < 0.000000001)
                return true;
            else
                return false;
        }
        public void TransCoord(double x1, double y1,  out double x2, out double y2)
        {

            double[,] Xi = { { x1 }, { y1 } };
            double[,] DX = { { dx }, { dy } };
            double[,] tY = new double[2, 1];
            //double[,] K = { { 1 + k } };

            double[,] M = GetM();
            //MatrixTool.Multi(Xi, K, ref tY);
            MatrixTool.Multi(M, Xi, ref tY);
            MatrixTool.Add(tY, DX, ref tY);
            x2 = tY[0, 0];
            y2 = tY[1, 0];
            
        }

        public double dx
        {
            get
            {
                return values[0, 0];
            }
            set
            {
                values[0, 0] = value;
            }
        }
        public double dy
        {
            get
            {
                return values[1, 0];
            }
            set
            {
                values[1, 0] = value;
            }
        }
        public double c
        {
            get
            {
                return values[2, 0];
            }
            set
            {
                values[2, 0] = value;
            }
        }
        public double d
        {
            get
            {
                return values[3, 0];
            }
            set
            {
                values[3, 0] = value;
            }
        }
    }
}
