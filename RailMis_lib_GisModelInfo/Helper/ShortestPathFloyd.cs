using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInfo.Helper
{
    public class OneConnection
    {
        public int fromIndex;
        public int toIndex;
        //public bool isRealConnect;
        //public double mileageOffset;
    }

    public class MGraph                    //图的定义
    {
        public double[,] edges;       //邻接矩阵
        public CRailwayLine[] rline;  // railwayLine的编号
        public int n;             //顶点数,弧数
        public int[,] path;
        //public int[] vexs;          //存放顶点编号
    };                               //图的邻接矩阵类型

    public class ShortestPathFloyd
    {
        //private const int MaxSize = 6;
        //private const int INF = 32767;    //INF表示∞
        //int vertexNum = 4;    //最大顶点个数
        //结构体的成员定义里不能直接赋值，也就是等号后的应该移除，在你后面实例化整个结构体以后，
        //再对Study_Data[n].input=new double[50] 其他成员类似。顺便说下其实用class简单得多。

        //struct VertexType
        //{
        //    public int no;                        //顶点编号
        //    public int info;                    //顶点其他信息
        //};                               //顶点类型


        public static void initPathGraph(List<CRailwayLine> ls, MGraph gNav, List<OneConnection> conList = null)
        {
            int i, j;

            gNav.n = ls.Count * 2;
            gNav.edges = new double[gNav.n, gNav.n];
            gNav.path = new int[gNav.n, gNav.n];
            gNav.rline = new CRailwayLine[ls.Count];

            i = 0;
            foreach (CRailwayLine rl in ls)
            {
                gNav.rline[i++] = rl;
                //gNav.lineID[i++] = rl.mIndex ;
            }


            for (i = 0; i < gNav.n; i++)        //建立图的图的邻接矩阵
                for (j = 0; j < gNav.n; j++)
                {
                    gNav.path[i, j] = -1;
                    if (i == j) // 相同顶点
                        gNav.edges[i, j] = 0;
                    else if (i / 2 == j / 2)  //同一条线路上的两个点
                    {
                        //if (i < j)
                        gNav.edges[i, j] = gNav.edges[j, i] = gNav.rline[i / 2].mLength;
                        //else
                        //    g.edges[i, j] = Double.MaxValue/10;
                    }
                    //else { if (Math.Abs(1)) ;
                    else
                    {
                        double p1x, p1y, p2x, p2y;
                        if (i % 2 == 0)
                        {
                            p1x = gNav.rline[i / 2].startLongitude;
                            p1y = gNav.rline[i / 2].startLatitude;
                        }
                        else
                        {
                            p1x = gNav.rline[i / 2].endLongitude;
                            p1y = gNav.rline[i / 2].endLatitude;
                        }

                        if (j % 2 == 0)
                        {
                            p2x = gNav.rline[j / 2].startLongitude;
                            p2y = gNav.rline[j / 2].startLatitude;
                        }
                        else
                        {
                            p2x = gNav.rline[j / 2].endLongitude;
                            p2y = gNav.rline[j / 2].endLatitude;
                        }

                        if (Math.Abs(p1x - p2x) < 0.0000001 && Math.Abs(p1y - p2y) < 0.0000001)
                            gNav.edges[i, j] = 0;
                        else
                            gNav.edges[i, j] = Double.MaxValue / 1000;

                    }
                }
            //Console.WriteLine("各顶点的最短路径:");
            if (conList != null)
            {
                //int id1, id2;
                foreach (OneConnection c in conList)
                {
                    i = lineid2Nodeid(gNav, c.fromIndex);
                    j = lineid2Nodeid(gNav, c.toIndex);
                    gNav.edges[i * 2 + 1, j * 2] = 0;
                }
            }
            Floyd(gNav);

        }

        public static int lineid2Nodeid(MGraph g, int lineid)
        {
            for (int i = 0; i < g.rline.Length; i++)
            {
                if (lineid == g.rline[i].mIndex)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 有些抽象，获取两个链之间的路径
        /// </summary>
        /// <param name="fromID">起始链的编号</param>
        /// <param name="toID">终止链的编号</param>
        /// <param name="dis">之间的距离，不准确，可能包含起始链或终止链，也可能不包含</param>
        /// <returns>路径节点，0，2，4，6下标为路径编号，1，3，5，7等标注正序还是逆序，0-正序，1-逆序</returns>
        public static List<int> getNavPath(int fromID, int toID, MGraph gNav, out double dis)
        {
            int id1, id2;
            id1 = id2 = 0;
            dis = 0;
            List<int> pathList = new List<int>();
            if (fromID == toID)
            {
                pathList.Add(fromID);
                pathList.Add(0);
                return pathList;

            }

            for (int i = 0; i < gNav.n; i++)
            {
                if (gNav.rline[i].mIndex == fromID)
                {
                    id1 = i * 2;
                    break;
                }
            }
            for (int i = 0; i < gNav.n; i++)
            {
                if (gNav.rline[i].mIndex == toID)
                {
                    id2 = i * 2 + 1;
                    break;
                }
            }

            Stack<int> pathStack = new Stack<int>();

            dis = gNav.edges[id1, id2];
            if (dis > Double.MaxValue / 10000)
                return pathList;
            pathStack.Push(id2);
            while (gNav.path[id1, id2] != -1)
            {
                pathStack.Push(gNav.path[id1, id2]);
                id2 = gNav.path[id1, id2];
            }
            pathStack.Push(id1);

            //Console.WriteLine(pathStack);
            //if (isNodeIndex)
            //    return pathStack.ToList();
            //pathList = pathStack.ToList();
            while (pathStack.Count > 1)
            {
                int id = pathStack.Pop();
                int x = id / 2;
                int nextid = pathStack.Pop();
                int nextx = nextid / 2;
                if (x == nextx)
                {
                    pathList.Add(x);
                    pathList.Add(nextid > id ? 0 : 1);

                }
                else
                {
                    pathList.Add(x);
                    pathList.Add(id % 2 == 1 ? 0 : 1);
                    pathList.Add(nextx);
                    pathList.Add(nextid % 2 == 0 ? 0 : 1);
                    if (pathStack.Count > 0)
                        pathStack.Pop();

                }
            }
            if (pathStack.Count == 1)
            {
                int id = pathStack.Pop();
                int x = id / 2;
                pathList.Add(x);
                pathList.Add(id % 2 == 0 ? 0 : 1);

            }

            return pathList;
            //        return path;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idLs">途经路径的线路id</param>
        /// <param name="g"></param>        
        /// <returns></returns>
        public static List<int> getNavPath(int[] idLs, MGraph g)
        {
            int id1, id2;
            id1 = id2 = 0;
            double dis = 0;
            List<int> pathList = new List<int>();
            if (idLs == null || idLs.Length == 0)
                return pathList;

            if (idLs.Length == 1)
            {
                pathList.Add(idLs[0]);
                pathList.Add(0);
                return pathList;

            }

            int[] idxsls = new int[idLs.Length * 2];
            int xs, xe, xnext;
            int j = 0;
            for (j = 0; j < (idLs.Length - 1) * 2; j += 2)
            {
                xs = lineid2Nodeid(g, idLs[j / 2]) * 2;
                xe = lineid2Nodeid(g, idLs[j / 2]) * 2 + 1;
                xnext = lineid2Nodeid(g, idLs[j / 2 + 1]) * 2;
                if (g.edges[xs, xnext] > g.edges[xe, xnext])
                {
                    idxsls[j] = xs;
                    idxsls[j + 1] = xe;
                }
                else
                {
                    idxsls[j] = xe;
                    idxsls[j + 1] = xs;
                }
            }
            j = (idLs.Length - 1) * 2;
            int xpre = lineid2Nodeid(g, idLs[j / 2 - 1]) * 2;
            xs = lineid2Nodeid(g, idLs[j / 2]) * 2;
            xe = lineid2Nodeid(g, idLs[j / 2]) * 2 + 1;
            if (g.edges[xpre, xs] < g.edges[xpre, xe])
            {
                idxsls[j] = xs;
                idxsls[j + 1] = xe;
            }
            else
            {
                idxsls[j] = xe;
                idxsls[j + 1] = xs;
            }

            Stack<int> pathStack = new Stack<int>();

            for (j = idxsls.Length - 2; j >= 0; j--)
            {
                id1 = idxsls[j];
                id2 = idxsls[j + 1];
                dis += g.edges[id1, id2];
                if (dis > Double.MaxValue / 10000)
                    return pathList;
                if (pathStack.Count == 0)
                    pathStack.Push(id2);
                else if (pathStack.Peek() != id2)
                    pathStack.Push(id2);
                while (g.path[id1, id2] != -1)
                {
                    pathStack.Push(g.path[id1, id2]);
                    id2 = g.path[id1, id2];
                }

            }
            pathStack.Push(id1);

            //Console.WriteLine(pathStack);
            //if (isNodeIndex)
            //    return pathStack.ToList();
            //pathList = pathStack.ToList();
            while (pathStack.Count > 1)
            {
                int id = pathStack.Pop();
                int x = g.rline[id / 2].mIndex;
                int nextid = pathStack.Pop();
                int nextx = g.rline[nextid / 2].mIndex;
                if (x == nextx)
                {
                    pathList.Add(x);
                    pathList.Add(nextid > id ? 0 : 1);

                }
                else
                {
                    pathList.Add(x);
                    pathList.Add(id % 2 == 1 ? 0 : 1);
                    pathList.Add(nextx);
                    pathList.Add(nextid % 2 == 0 ? 0 : 1);
                    if (pathStack.Count > 0)
                        pathStack.Pop();

                }
            }
            if (pathStack.Count == 1)
            {
                int id = pathStack.Pop();
                int x = g.rline[id / 2].mIndex;
                pathList.Add(x);
                pathList.Add(id % 2 == 0 ? 0 : 1);

            }

            return pathList;
            //        return path;
        }

        //private static Stack<int> shortestPathBetween(int lineid1, int lineid2) {
        //    Stack<int> pathStack = new Stack<int>();
        //}

        //static void getXY(List<CRailwayLine> ls,int i, out double x, out double y)
        //{
        //    if (i % 2 == 0)
        //    {
        //        x = ls[i / 2].longitude[0];
        //        y = ls[i / 2].latitude[0];
        //    }
        //    else
        //    {
        //        int m = ls[i / 2].mPointNum;
        //        x = ls[i / 2].longitude[m - 1];
        //        y = ls[i / 2].latitude[m - 1];
        //    }
        //}

        static void Floyd(MGraph g)
        {
            double[,] A = g.edges; // new double[g.n, g.n];//A用于存放当前顶点之间的最短路径长度,分量A[i][j]表示当前顶点vi到顶点vj的最短路径长度。
            int[,] path = g.path;//从顶点vi到顶点vj的路径上所经过的顶点编号不大于k的最短路径长度。
            int i, j, k;
            //for (i = 0; i < g.n; i++)
            //{
            //    for (j = 0; j < g.n; j++)//对各个节点初始已经知道的路径和距离
            //    {
            //        A[i, j] = g.edges[i, j];
            //        path[i, j] = -1;
            //    }
            //}
            for (k = 0; k < g.n; k++)
            {
                for (i = 0; i < g.n; i++)
                    for (j = 0; j < g.n; j++)
                        if (A[i, j] > A[i, k] + A[k, j])//从i到j的路径比从i经过k到j的路径长
                        {
                            A[i, j] = A[i, k] + A[k, j];//更改路径长度
                            path[i, j] = k;//更改路径信息经过k
                        }
            }

            //Dispath(A, path, g.n);   //输出最短路径
        }

        /*
        void Dispath(double[,] A, int[,] path, int n)
        {

            int i, j;
            for (i = 0; i < n; i++)
                for (j = 0; j < n; j++)
                {
                    if (A[i, j] >= Double.MaxValue / 11)
                    {
                        if (i != j)
                        {

                            Console.WriteLine("从{0}到{1}没有路径\n", i, j);
                        }

                    }
                    else
                    {
                        Console.Write("从{0}到{1}=>路径长度:{2} 路径:", i, j, A[i, j]);

                        Console.Write("{0},", i);    //输出路径上的起点
                        Ppath(path, i, j);            //输出路径上的中间点
                        Console.WriteLine("{0}", j);    //输出路径上的终点
                    }
                }
        }

        void Ppath(int[,] path, int i, int j)  //前向递归查找路径上的顶点
        {
            int k;
            k = path[i, j];
            if (k == -1) return;    //找到了起点则返回
            Ppath(path, i, k);    //找顶点i的前一个顶点k

            Console.Write("{0},", k);    //输出路径上的终点
            Ppath(path, k, j);    //找顶点k的前一个顶点j
        }
        */

        //static void Main(string[] args)
        //{
        //    initdata();

        //    //Console.Write("{0}", MaxSize);
        //    //Console.Write("{0}", vertexNum);
        //    Console.ReadKey();
        //}
    }
}
