using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SQLite;
//using System.Data.SQLite.Generic;

//using Microsoft.Office.Core;
//using Microsoft.Office.Interop.Excel;

using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


//using System.Runtime.InteropServices;
//using System.Drawing.Imaging;

namespace GISUtilities
{

    public partial class Form1 : Form
    {

        DataSet ds = new DataSet();
        //List<ChainNode> chainList = new List<ChainNode>();
        
        public Form1()
        {
            InitializeComponent();
            
        }



        private void button1_Click(object sender, EventArgs e)
        {
            double x1, y1, x2, y2;
            x1 = 119;
            x2 = 120;
            y1 = y2 = 35;
            Console.WriteLine("x:" + CoordinateConverter.getUTMDistance(x1, y1, x2, y2));
            Console.WriteLine("y:" + CoordinateConverter.getUTMDistance(x1, y1, x1, y2 + 1));
            string fileName = @"D:\sevenParamsTest.xlsx";
            //if (chainList.Count > 0) 
            //    chainList.Clear();
            //LoadDataTableFromExcel(fileName, "Sheet1");
            int ptNum = 10;
            double[,] X = MatrixTool.Init(3 * ptNum,1);
            double[,] Y  = MatrixTool.Init(3 * ptNum,1);
            double[,] Y84 = MatrixTool.Init(3 * ptNum, 1);

            
            int pN = 10;
            double[] XX = new double[pN * 3] ;
            double[] YYRead = new double[pN * 3];
            double[] YY =　new double[pN * 3];
            double[] YY84 = new double[pN * 3];

            int i = 0;
            int j = 0;
            int k = 0;
            foreach (DataRow dr in ds.Tables["Sheet1"].Rows)
            {
                if (i % 20 == 0 && j < ptNum)
                {
                    X[j* 3 + 1,0] = Convert.ToDouble(dr["North"]);
                    X[j * 3 + 0,0] = Convert.ToDouble(dr["East"]);
                    X[j * 3 + 2,0] = Convert.ToDouble(dr["Altitude"]);
                    Y84[j * 3 + 0,0] = Convert.ToDouble(dr["Longitude"]);
                    Y84[j * 3 + 1,0] = Convert.ToDouble(dr["Latitude"]);
                    Y84[j * 3 + 2,0] = Convert.ToDouble(dr["Altitude"]);
                    j++;

                }
                if (i % 20 == 10 && k < ptNum)
                {
                    XX[k * 3 + 1] = Convert.ToDouble(dr["North"]);
                    XX[k * 3 + 0] = Convert.ToDouble(dr["East"]);
                    XX[k * 3 + 2] = Convert.ToDouble(dr["Altitude"]);
                    YYRead[k * 3 + 0] = Convert.ToDouble(dr["Longitude"]);
                    YYRead[k * 3 + 1] = Convert.ToDouble(dr["Latitude"]);
                    YYRead[k * 3 + 2] = Convert.ToDouble(dr["Altitude"]);
                    k++;

                }
                i++;
                if (j == ptNum && k == ptNum)
                    break;

            }

            for (i = 0; i < ptNum; i++)
            {
                CoordinateConverter.LatLonToUTMXY(Y84[3 * i + 1, 0], Y84[3 * i + 0, 0], out Y[3 * i + 0, 0], out Y[3 * i + 1, 0]);
            }

            CoordTrans7Param ct = new CoordTrans7Param();
            ct.CalculateTrans7Param(X, Y);
            for (i = 0; i < pN; i++)
            {
                ct.TransCoord(XX[i*3 + 0], XX[i*3 + 1], XX[i*3+2],out YY[i*3+0],out YY[i*3+1],out YY[i*3+2]);
                CoordinateConverter.UTMXYToLatLon(YY[i*3+0],YY[i*3+1],out YY84[i*3 + 1],out YY84[i*3+0]);
                Console.WriteLine("准确的 x {0}, y {1}, z{2}, 计算的 x {3} , y {4} , z {5}", YYRead[i * 3 + 0], YYRead[i * 3 + 1], YYRead[i * 3 + 2], YY84[i * 3 + 0], YY84[i * 3 + 1], YY[i * 3 + 2]);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            string fileName = @"D:\sevenParamsTest.xlsx";
            //if (chainList.Count > 0) 
            //    chainList.Clear();
            //LoadDataTableFromExcel(fileName, "Sheet1");
            int ptNum = 5;
            double[,] X = MatrixTool.Init(2 * ptNum, 1);
            double[,] Y84 = MatrixTool.Init(2 * ptNum, 1);
            double[,] Y = MatrixTool.Init(2 * ptNum, 1);


            int pN = 10;
            double[] XX = new double[pN * 2];
            double[] YYRead = new double[pN * 2];
            double[] YY = new double[pN * 2];
            double[] YY84 = new double[pN * 2];

            int i = 0;
            int j = 0;
            int k = 0;

            foreach (DataRow dr in ds.Tables["Sheet1"].Rows)
            {
                if (i % 40 == 0 && j < ptNum)
                {
                    X[j * 2 + 1, 0] = Convert.ToDouble(dr["North"]);
                    X[j * 2 + 0, 0] = Convert.ToDouble(dr["East"]);

                    Y84[j * 2 + 0, 0] = Convert.ToDouble(dr["Longitude"]);
                    Y84[j * 2 + 1, 0] = Convert.ToDouble(dr["Latitude"]);

                    j++;

                }
                if (i % 20 == 10 && k < pN)
                {
                    XX[k * 2 + 1] = Convert.ToDouble(dr["North"]);
                    XX[k * 2 + 0] = Convert.ToDouble(dr["East"]);

                    YYRead[k * 2 + 0] = Convert.ToDouble(dr["Longitude"]);
                    YYRead[k * 2 + 1] = Convert.ToDouble(dr["Latitude"]);

                    k++;

                }
                i++;
                if (j == ptNum && k == pN)
                    break;

            }
            for (i = 0; i< ptNum; i++){
                CoordinateConverter.LatLonToUTMXY(Y84[2 * i + 1, 0], Y84[2 * i + 0, 0], out Y[2 * i + 0, 0], out Y[2 * i + 1, 0]);
            }
            //double cx1, cy1, cx2, cy2;
            //cx1 = cy1 = cx2 = cy2 = 0;
            //double sx = 91310;
            //double sy = 111000;
            //for (i = 0; i < pN; i++)
            //{
            //    cx1 += XX[2*i];
            //    cy1 += XX[2*i+1];
            //    cx2 += YYRead[2 * i];
            //    cy2 += YYRead[2 * i + 1];
            //}
            //cx1 /= pN;
            //cy1 /= pN;
            //cx2 /= pN;
            //cy2 /= pN;

            //for (i = 0; i < ptNum; i++)
            //{
            //    X[2 * i, 0] = X[2 * i, 0] - cx1;
            //    X[2 * i + 1, 0] = X[2 * i + 1, 0] - cy1;
            //    Y[2 * i, 0] = (Y[2 * i, 0] - cx2) * sx;
            //    Y[2 * i + 1, 0] = (Y[2 * i + 1, 0] - cy2) * sy;
             
            //}

            //for (i = 0; i < pN; i++)
            //{
            //    XX[2 * i] = XX[2 * i] - cx1;
            //    XX[2 * i + 1] = XX[2 * i + 1] - cy1;  
            //}

            CoordTrans4Param ct = new CoordTrans4Param();
            ct.CalculateTrans4Param(X, Y);
            for (i = 0; i < pN; i++)
            {
                ct.TransCoord(XX[i * 2 + 0], XX[i * 2 + 1],out YY[i * 2 + 0], out YY[i *2 + 1]);
                CoordinateConverter.UTMXYToLatLon(YY[i * 2 +0], YY[i * 2 + 1], out YY84[i * 2 + 1],out YY84[i * 2 + 0]);
                Console.WriteLine("准确的 x {0}, y {1}, 计算的 x {2} , y {3} ", YYRead[i * 2 + 0], YYRead[i * 2 + 1], YY84[i * 2 + 0]  , YY84[i * 2+ 1] );
                //Console.WriteLine("准确的 x {0}, y {1}, 计算的 x {2} , y {3} ", YYRead[i * 2 + 0], YYRead[i * 2 + 1], YY[i * 2 + 0] / sx + cx2, YY[i * 2 + 1] / sy + cy2);

            }
        }

        private void button2_Click1(object sender, EventArgs e)
        {
            double x1, y1, x2, y2;
            x1 = 119;
            x2 = 120;
            y1 = y2 = 35;
            Console.WriteLine("x:" + CoordinateConverter.getUTMDistance(x1, y1, x2, y2));
            Console.WriteLine("y:" + CoordinateConverter.getUTMDistance(x1, y1, x1, y2 + 1));
            string fileName = @"D:\sevenParamsTest.xlsx";
            //if (chainList.Count > 0) 
            //    chainList.Clear();
            //LoadDataTableFromExcel(fileName, "Sheet1");
            int ptNum = 5;
            double[,] X = MatrixTool.Init(2 * ptNum, 1);
            double[,] Y = MatrixTool.Init(2 * ptNum, 1);


            int pN = 10;
            double[] XX = new double[pN * 2];
            double[] YYRead = new double[pN * 2];
            double[] YY = new double[pN * 2];

            int i = 0;
            int j = 0;
            int k = 0;

            foreach (DataRow dr in ds.Tables["Sheet1"].Rows)
            {
                if (i % 40 == 0 && j < ptNum)
                {
                    X[j * 2 + 0, 0] = Convert.ToDouble(dr["North"]);
                    X[j * 2 + 1, 0] = Convert.ToDouble(dr["East"]);

                    Y[j * 2 + 0, 0] = Convert.ToDouble(dr["Longitude"]);
                    Y[j * 2 + 1, 0] = Convert.ToDouble(dr["Latitude"]);

                    j++;

                }
                if (i % 20 == 10 && k < pN)
                {
                    XX[k * 2 + 0] = Convert.ToDouble(dr["North"]);
                    XX[k * 2 + 1] = Convert.ToDouble(dr["East"]);

                    YYRead[k * 2 + 0] = Convert.ToDouble(dr["Longitude"]);
                    YYRead[k * 2 + 1] = Convert.ToDouble(dr["Latitude"]);

                    k++;

                }
                i++;
                if (j == ptNum && k == pN)
                    break;

            }
            double cx1, cy1, cx2, cy2;
            cx1 = cy1 = cx2 = cy2 = 0;
            double sx = 91310;
            double sy = 111000;
            for (i = 0; i < pN; i++)
            {
                cx1 += XX[2 * i];
                cy1 += XX[2 * i + 1];
                cx2 += YYRead[2 * i];
                cy2 += YYRead[2 * i + 1];
            }
            cx1 /= pN;
            cy1 /= pN;
            cx2 /= pN;
            cy2 /= pN;

            for (i = 0; i < ptNum; i++)
            {
                X[2 * i, 0] = X[2 * i, 0] - cx1;
                X[2 * i + 1, 0] = X[2 * i + 1, 0] - cy1;
                Y[2 * i, 0] = (Y[2 * i, 0] - cx2) * sx;
                Y[2 * i + 1, 0] = (Y[2 * i + 1, 0] - cy2) * sy;

            }

            for (i = 0; i < pN; i++)
            {
                XX[2 * i] = XX[2 * i] - cx1;
                XX[2 * i + 1] = XX[2 * i + 1] - cy1;
            }

            CoordTrans4Param ct = new CoordTrans4Param();
            ct.CalculateTrans4Param(X, Y);
            for (i = 0; i < pN; i++)
            {
                ct.TransCoord(XX[i * 2 + 0], XX[i * 2 + 1], out YY[i * 2 + 0], out YY[i * 2 + 1]);
                Console.WriteLine("准确的 x {0}, y {1}, 计算的 x {2} , y {3} ", YYRead[i * 2 + 0], YYRead[i * 2 + 1], YY[i * 2 + 0] / sx + cx2, YY[i * 2 + 1] / sy + cy2);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            string dbPath = @"d:\test.db";//指定数据库路径
            
            using(SQLiteConnection conn = new SQLiteConnection("Data Source =" + dbPath))//创建连接
            {
                conn.Open();//打开连接




            string sql = "CREATE TABLE IF NOT EXISTS student(id integer, name varchar(20), sex varchar(2));";//建表语句
            SQLiteCommand cmdCreateTable = new SQLiteCommand(sql, conn);
            cmdCreateTable.ExecuteNonQuery();//如果表不存在，创建数据表

            //SQLiteCommand cmdInsert = new SQLiteCommand(conn);
            //cmdInsert.CommandText = "INSERT INTO student VALUES(1, '小红', '男')";//插入几条数据
            //cmdInsert.ExecuteNonQuery();
            //cmdInsert.CommandText = "INSERT INTO student VALUES(2, '小李', '女')";
            //cmdInsert.ExecuteNonQuery();
            //cmdInsert.CommandText = "INSERT INTO student VALUES(3, '小明', '男')";
            //cmdInsert.ExecuteNonQuery();








                using(SQLiteTransaction tran = conn.BeginTransaction())//实例化一个事务
                {
                    for (int i = 0; i < 100000; i++ )
                    {
                        SQLiteCommand cmd = new SQLiteCommand(conn);//实例化SQL命令
                        cmd.Transaction = tran;
                        cmd.CommandText = "insert into student values(@id, @name, @sex)";//设置带参SQL语句
                        cmd.Parameters.AddRange(new[] {//添加参数
                            new SQLiteParameter("@id", i),
                            new SQLiteParameter("@name", "中国人"),
                            new SQLiteParameter("@sex", "男")
                        });
                        cmd.ExecuteNonQuery();//执行查询
                    }
                    tran.Commit();//提交
                }
            }
        
        }
    }



   /// <summary> 
   /// 说明：这是一个针对System.Data.SQLite的数据库常规操作封装的通用类。 
   /// </summary> 
   public class SQLiteDBHelper 
   { 
     private string connectionString = string.Empty; 
     /// <summary> 
     /// 构造函数 
     /// </summary> 
     /// <param name="dbPath">SQLite数据库文件路径</param> 
     public SQLiteDBHelper(string dbPath) 
     { 
       this.connectionString = "Data Source=" + dbPath; 
     } 
     /// <summary> 
     /// 创建SQLite数据库文件 
     /// </summary> 
     /// <param name="dbPath">要创建的SQLite数据库文件路径</param> 
     public static void CreateDB(string dbPath) 
     { 
       using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbPath)) 
       { 
         connection.Open(); 
         using (SQLiteCommand command = new SQLiteCommand(connection)) 
         { 
           command.CommandText = "CREATE TABLE Demo(id integer NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE)"; 
           command.ExecuteNonQuery(); 
           command.CommandText = "DROP TABLE Demo"; 
           command.ExecuteNonQuery(); 
         } 
       } 
     } 
     /// <summary> 
     /// 对SQLite数据库执行增删改操作，返回受影响的行数。 
     /// </summary> 
     /// <param name="sql">要执行的增删改的SQL语句</param> 
     /// <param name="parameters">执行增删改语句所需要的参数，参数必须以它们在SQL语句中的顺序为准</param> 
     /// <returns></returns> 
     public int ExecuteNonQuery(string sql, SQLiteParameter[] parameters) 
     { 
       int affectedRows = 0; 
       using (SQLiteConnection connection = new SQLiteConnection(connectionString)) 
       { 
         connection.Open();
         using (SQLiteTransaction transaction = connection.BeginTransaction()) 
         { 
           using (SQLiteCommand command = new SQLiteCommand(connection)) 
           { 
             command.CommandText = sql; 
             if (parameters != null) 
             { 
               command.Parameters.AddRange(parameters); 
             } 
             affectedRows = command.ExecuteNonQuery(); 
           } 
           transaction.Commit(); 
         } 
       } 
       return affectedRows; 
     } 
     /// <summary> 
     /// 执行一个查询语句，返回一个关联的SQLiteDataReader实例 
     /// </summary> 
     /// <param name="sql">要执行的查询语句</param> 
     /// <param name="parameters">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准</param> 
     /// <returns></returns> 
     public SQLiteDataReader ExecuteReader(string sql, SQLiteParameter[] parameters) 
     { 
       SQLiteConnection connection = new SQLiteConnection(connectionString); 
       SQLiteCommand command = new SQLiteCommand(sql, connection); 
       if (parameters != null) 
       { 
         command.Parameters.AddRange(parameters); 
       } 
       connection.Open(); 
       return command.ExecuteReader(CommandBehavior.CloseConnection); 
     } 
     /// <summary> 
     /// 执行一个查询语句，返回一个包含查询结果的DataTable 
     /// </summary> 
     /// <param name="sql">要执行的查询语句</param> 
     /// <param name="parameters">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准</param> 
     /// <returns></returns> 
     public DataTable ExecuteDataTable(string sql, SQLiteParameter[] parameters) 
     { 
       using (SQLiteConnection connection = new SQLiteConnection(connectionString)) 
       { 
         using (SQLiteCommand command = new SQLiteCommand(sql, connection)) 
         { 
           if (parameters != null) 
           { 
             command.Parameters.AddRange(parameters); 
           } 
           SQLiteDataAdapter adapter = new SQLiteDataAdapter(command); 
           DataTable data = new DataTable(); 
           adapter.Fill(data); 
           return data; 
         } 
       } 
     } 
     /// <summary> 
     /// 执行一个查询语句，返回查询结果的第一行第一列 
     /// </summary> 
     /// <param name="sql">要执行的查询语句</param> 
     /// <param name="parameters">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准</param> 
     /// <returns></returns> 
     public Object ExecuteScalar(string sql, SQLiteParameter[] parameters) 
     { 
       using (SQLiteConnection connection = new SQLiteConnection(connectionString)) 
       { 
         using (SQLiteCommand command = new SQLiteCommand(sql, connection)) 
         { 
           if (parameters != null) 
           { 
             command.Parameters.AddRange(parameters); 
           } 
           SQLiteDataAdapter adapter = new SQLiteDataAdapter(command); 
           DataTable data = new DataTable(); 
           adapter.Fill(data); 
           return data; 
         } 
       } 
     } 
     /// <summary> 
     /// 查询数据库中的所有数据类型信息 
     /// </summary> 
     /// <returns></returns> 
     public DataTable GetSchema() 
     { 
       using (SQLiteConnection connection = new SQLiteConnection(connectionString)) 
       { 
         connection.Open(); 
         DataTable data=connection.GetSchema("TABLES"); 
         connection.Close(); 
         //foreach (DataColumn column in data.Columns) 
         //{ 
         //  Console.WriteLine(column.ColumnName); 
         //} 
         return data; 
       } 
     } 
   } 

}
