using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Data.SQLite;
using ModelInfo.Helper;


namespace ModelInfo
{
    public class OnePrj
    {
        public int prjIdx;
        public string prjType;
        public string prjName;
        public string ModelName;
        public double prjLength;
        public double sx, sy, sz;
        public double[] mx, my, mz;
        public OnePrj(int idx, string t, string name, string model,
            double len, double xs, double ys, double zs)
        {
            prjIdx = idx;
            prjType = t;
            prjName = name;
            ModelName = model;
            prjLength = len;
            sx = xs;
            sy = ys;
            sz = zs;
        }

    }
    public class DatabaseWrapper
    {

        /// <summary> 
        /// 执行一个查询语句，返回一个包含查询结果的DataTable 
        /// </summary> 
        /// <param name="sql">要执行的查询语句</param> 
        /// <param name="parameters">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准</param> 
        /// <returns></returns> 
        public static System.Data.DataTable ExecuteDataTable(string dbFile,string sql )
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbFile))
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    //if (parameters != null)
                    //{
                    //    command.Parameters.AddRange(parameters);
                    //}
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                    System.Data.DataTable data = new System.Data.DataTable();
                    adapter.Fill(data);
                    return data;
                }
            }
        } 


        public static void CreateSqliteDB(string dbPath)
        {
            //string  = "D:\\Demo.db3";
            //如果不存在改数据库文件，则创建该数据库文件 
            if (!System.IO.File.Exists(dbPath))
            {
                CSqliteWrapper db = new CSqliteWrapper(dbPath);
                string sql = @"CREATE TABLE ConsInfo(ProjectName integer NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,Name char(3),TypeName varchar(50),addDate datetime,UpdateTime Date,Time time,Comments blob)";
                db.ExecuteNonQuery(sql, null);
            }

        }

        /// <summary>
        /// 工具方法，批量根据wgs84坐标生成火星坐标，时间非常长
        /// </summary>
        /// <param name="dbPath"></param>
        public static void UpdateMileageMars(string dbPath)
        {
            string sqlselect = "select * from MileageInfo where longitudeMars is null " ;
            string sqlUpdate = @"update MileageInfo set LongitudeMars = @longMars, LatitudeMars = @latMars where MileageID = @mID;  ";
            System.Data.DataTable dt = ExecuteDataTable(dbPath,sqlselect);
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbPath))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = sqlUpdate;
                        double longMars, latMars;
                        foreach (DataRow dr in dt.Rows)
                        {
                            GPSAdjust.transform2Mars(Convert.ToDouble(dr["Latitude"]),Convert.ToDouble(dr["Longitude"]),
                                out latMars,out  longMars);
                            SQLiteParameter[] parameters = new SQLiteParameter[]
                            {
                                new SQLiteParameter("@longMars",longMars),
                                new SQLiteParameter("@latMars",latMars),
                                new SQLiteParameter("@mID",Convert.ToInt32(dr["MileageID"])),
                            };
                            command.Parameters.AddRange(parameters);
                            command.ExecuteNonQuery();
                            //command.ExecuteReader()
                        }
                    }
                    transaction.Commit();
                }
            }

        }

        public static void SaveConsToSqlite(string dbPath, List<ConsLocation> cl)
        {

            string sql = "INSERT INTO ConsInfo(ProjectName,DwName,Longitude,Latitude,StaffNum,fromDate,toDate)values(@ProjectName,@DwName,@Longitude,@Latitude,@StaffNum,@fromDate,@toDate)";
            CSqliteWrapper db = new CSqliteWrapper(dbPath);
            db.ExecuteNonQuery("DELETE from ConsInfo", null);

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbPath))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = sql;
                        foreach (ConsLocation p in cl)
                        {
                            SQLiteParameter[] parameters = new SQLiteParameter[]
                            { 
                                new SQLiteParameter("@ProjectName",p.ProjName), 
                                new SQLiteParameter("@DwName",p.ProjDWName), 
                                new SQLiteParameter("@Longitude",p.Longitude), 
                                new SQLiteParameter("@Latitude",p.Latitude), 
                                new SQLiteParameter("@StaffNum",p.Number), 
                                new SQLiteParameter("@fromDate",p.FromDate), 
                                new SQLiteParameter("@toDate",p.ToDate)
                            };
                            command.Parameters.AddRange(parameters);
                            command.ExecuteNonQuery();
                            //command.ExecuteReader()
                        }
                    }
                    transaction.Commit();
                }
            }


            Console.WriteLine("实名制信息存储完成");

        }


        public static void SaveFirmToSqlite(string dbPath, List<CRailwayFirm> cl)
        {

            string sql = "INSERT INTO FirmInfo(Num,firmid,ShorName,Longitude,Latitude,FirmType,FirmIcon)values(@Num,@firmid,@ShorName,@Longitude,@Latitude,@FirmType,@FirmIcon)";

            CSqliteWrapper db = new CSqliteWrapper(dbPath);
            db.ExecuteNonQuery("DELETE from FirmInfo", null);

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbPath))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = sql;
                        foreach (CRailwayFirm p in cl)
                        {
                            SQLiteParameter[] parameters = new SQLiteParameter[]
                            { 
                                new SQLiteParameter("@Num",p.NumStaff), 
                                new SQLiteParameter("@firmid",p.mFirmID), 
                                new SQLiteParameter("@ShorName",p.FirmName), 
                                new SQLiteParameter("@Longitude",p.CenterLongitude), 
                                new SQLiteParameter("@Latitude",p.CenterLatitude), 
                                new SQLiteParameter("@FirmType",p.FirmType), 
                                new SQLiteParameter("@FirmIcon",p.mLabelImage)
                            };
                            command.Parameters.AddRange(parameters);
                            command.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
            }

            Console.WriteLine("单位信息存储完成");
        }


        public static void SaveProgressToSqlite(string dbPath, List<CRailwayProject> cl)
        {

            string sql = "INSERT INTO FXDict(ProjectID,ProjectDictID,ProjectDictName,DesignNum,InitNum) values (@ProjectID,@ProjectDictID,@ProjectDictName,@DesignNum,@InitNum)";
            string sql2 = "INSERT INTO FXData(ProjectID,ProjectDictID,ReportDate,DictTotal) values (@ProjectID,@ProjectDictID,@ReportDate,@DictTotal)";

            CSqliteWrapper db = new CSqliteWrapper(dbPath);
            db.ExecuteNonQuery("DELETE from FXDict", null);
            db.ExecuteNonQuery("DELETE from FXData", null);

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbPath))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = sql;
                        foreach (CRailwayProject p in cl)
                        {
                            foreach (CFXProj fx in p.FXProgress)
                            {
                                SQLiteParameter[] parameters = new SQLiteParameter[]
                                { 
                                    new SQLiteParameter("@ProjectID",p.mProjectID), 
                                    new SQLiteParameter("@ProjectDictID",fx.fxID), 
                                    new SQLiteParameter("@ProjectDictName",fx.FxName), 
                                    new SQLiteParameter("@DesignNum",fx.TotalAmount), 
                                    new SQLiteParameter("@InitNum",fx.initAmount) 
                                };

                                command.Parameters.AddRange(parameters);
                                command.ExecuteNonQuery();

                            }
                        }


                    }

                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = sql2;
                        foreach (CRailwayProject p in cl)
                        {
                            foreach (CFXProj fx in p.FXProgress)
                            {
                                for (int i = 0; i < fx.strDate.Count; i++)
                                {
                                    SQLiteParameter[] para = new SQLiteParameter[]
                                    { 
                                        new SQLiteParameter("@ProjectID",p.mProjectID), 
                                        new SQLiteParameter("@ProjectDictID",fx.fxID), 
                                        new SQLiteParameter("@ReportDate",fx.strDate[i]), 
                                        new SQLiteParameter("@DictTotal",fx.doneAmount[i]) 
                                    };
                                    command.Parameters.AddRange(para);
                                    command.ExecuteNonQuery();

                                }
                            }
                        }


                    }
                    transaction.Commit();
                }
            }


            Console.WriteLine("分项工程信息存储完成");
        }

        public static void SavePolePosToSqlite(string dbPath, List<CRailwayLine> rlist)
        {
            CSqliteWrapper db = new CSqliteWrapper(dbPath);
            db.ExecuteNonQuery("DELETE from PolePos", null);
            string sql = "INSERT INTO PolePos";
            sql += " (longitude,latitude,altitude,yaw,name)" +
                "values(@Longitude,@Latitude,@Altitude,@YawOffset,@Name)";

            int count = 0;
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbPath))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = sql;
                        foreach (CRailwayLine  r in rlist)
                        {
                            double[] x, y, z, d;
                            r.getPolePos(out x, out y, out z, out d);
                            if (x == null)
                                continue;
                            for (int i = 0; i < x.Length; i++)
                            {

                             
                                SQLiteParameter[] parameters = new SQLiteParameter[]
                                {
                                    new SQLiteParameter("@Longitude",x[i]),
                                    new SQLiteParameter("@Latitude",y[i]),
                                    new SQLiteParameter("@Altitude",z[i]),
                                    new SQLiteParameter("@YawOffset",d[i]-90),
                                    new SQLiteParameter("@Name","" + count++),

                                };

                                command.Parameters.AddRange(parameters);
                                command.ExecuteNonQuery();
                                //command.Parameters.Clear();

                            }
                        }
                        transaction.Commit();
                    }
                }

            }
            Console.WriteLine("线杆信息存储完毕");
        }

        public static void SavePierToSqlite(string dbPath, List<CRailwayProject> cl1, List<CRailwayProject> cl2)
        {
            //string dbPath = CGisDataSettings.gDataPath + @"\jiqing\JQGis.db";
            CSqliteWrapper db = new CSqliteWrapper(dbPath);
            db.ExecuteNonQuery("DELETE from ProjFBInfo" , null);
            db.ExecuteNonQuery("DELETE from PierInfo", null);
            db.ExecuteNonQuery("DELETE from PierContInfo", null);
            db.ExecuteNonQuery("DELETE from BeamInfo", null);

            SaveBeamToSqliteHelper(dbPath, cl1);
            SavePierToSqliteHelper(dbPath, "PierInfo", cl1);
            SavePierToSqliteHelper(dbPath, "PierContInfo", cl2);

            Console.WriteLine("桥墩信息存储完成");
        }

        public static void SaveBeamToSqliteHelper(string dbPath, List<CRailwayProject> cl)
        {
            //string dbPath = CGisDataSettings.gDataPath + @"\jiqing\JQGis.db";
            string sql = "INSERT INTO BeamInfo";
            sql += " (BridgeName,Name,Longitude,Latitude,Altitude,YawOffset,MileagePrefix,Mileage,Project_B_DW_ID,ProjectID,ProjectLength,FinishTime,SerialNo,IsValid, IsFinish)" +
                "values(@BridgeName,@Name,@Longitude,@Latitude,@Altitude,@YawOffset,@MileagePrefix,@Mileage,@Project_B_DW_ID,@ProjectID,@ProjectLength,@FinishTime,@SerialNo,@IsValid,@IsFinish)";


            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbPath))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = sql;
                        foreach (CRailwayProject p in cl)
                        {
                            if (p.mBeamList.Count == 0) continue;
                            foreach (CRailwayDWProj dwp in p.mBeamList)
                            {
                                SQLiteParameter[] parameters = new SQLiteParameter[]
                                {
                                    new SQLiteParameter("@BridgeName",p.ProjectName),
                                    new SQLiteParameter("@Name",dwp.DWName),
                                    new SQLiteParameter("@Longitude",dwp.mLongitude_Mid),
                                    new SQLiteParameter("@Latitude",dwp.mLatitude_Mid),
                                    new SQLiteParameter("@Altitude",dwp.mAltitude_Mid),
                                    new SQLiteParameter("@YawOffset",dwp.mHeading_Mid),
                                    new SQLiteParameter("@MileagePrefix",dwp.DKCode_Start),
                                    new SQLiteParameter("@Mileage",dwp.Mileage_Start),
                                    new SQLiteParameter("@Project_B_DW_ID",dwp.mDWID),
                                    new SQLiteParameter("@ProjectID",dwp.mParentID),
                                    new SQLiteParameter("@ProjectLength",dwp.mLength),
                                    new SQLiteParameter("@FinishTime",dwp.FinishTime.ToShortDateString()),
                                    new SQLiteParameter("@SerialNo",dwp.mSerialNo),
                                    new SQLiteParameter("@IsValid",dwp.mIsValid),
                                    new SQLiteParameter("@IsFinish",dwp.mIsDone)

                                };

                                command.Parameters.AddRange(parameters);
                                command.ExecuteNonQuery();
                                //command.Parameters.Clear();

                            }
                        }
                        transaction.Commit();
                    }
                }

             }

        }
        public static void SavePierToSqliteHelper(string dbPath, string tableName, List<CRailwayProject> cl)
        {
            //string dbPath = CGisDataSettings.gDataPath + @"\jiqing\JQGis.db";
            string sql = "INSERT INTO ";
            sql += tableName;
            sql += " (BridgeName,Name,Longitude,Latitude,Altitude,YawOffset,MileagePrefix,Mileage,Project_B_DW_ID,ProjectID,ProjectLength,FinishTime,SerialNo,IsValid, IsFinish)" +
                "values(@BridgeName,@Name,@Longitude,@Latitude,@Altitude,@YawOffset,@MileagePrefix,@Mileage,@Project_B_DW_ID,@ProjectID,@ProjectLength,@FinishTime,@SerialNo,@IsValid,@IsFinish)";


            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbPath))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = sql;
                        foreach (CRailwayProject p in cl)
                        {
                            if (p.mPierList.Count == 0) continue;
                            foreach (CRailwayDWProj dwp in p.mPierList)
                            {
                                SQLiteParameter[] parameters = new SQLiteParameter[]
                                {
                                    new SQLiteParameter("@BridgeName",p.ProjectName),
                                    new SQLiteParameter("@Name",dwp.DWName + "#墩"),
                                    new SQLiteParameter("@Longitude",dwp.mLongitude_Mid),
                                    new SQLiteParameter("@Latitude",dwp.mLatitude_Mid),
                                    new SQLiteParameter("@Altitude",dwp.mAltitude_Mid),
                                    new SQLiteParameter("@YawOffset",dwp.mHeading_Mid),
                                    new SQLiteParameter("@MileagePrefix",dwp.DKCode_Start),
                                    new SQLiteParameter("@Mileage",dwp.Mileage_Start),
                                    new SQLiteParameter("@Project_B_DW_ID",dwp.mDWID),
                                    new SQLiteParameter("@ProjectID",dwp.mParentID),
                                    new SQLiteParameter("@ProjectLength",dwp.mLength),
                                    new SQLiteParameter("@FinishTime",dwp.FinishTime.ToShortDateString()),
                                    new SQLiteParameter("@SerialNo",dwp.mSerialNo),
                                    new SQLiteParameter("@IsValid",dwp.mIsValid),
                                    new SQLiteParameter("@IsFinish",dwp.mIsDone)

                                };

                                command.Parameters.AddRange(parameters);
                                command.ExecuteNonQuery();
                                //command.Parameters.Clear();

                            }
                        }
                        transaction.Commit();
                    }
                }

                using (SQLiteTransaction transaction2 = connection.BeginTransaction())
                {
                    sql = "INSERT INTO ProjFBInfo " +
                        " (StubIndex,ParentID,ProjectID,IsFinish,SerialNO,Name,Type)" +
                        " values(@StubIndex,@ParentID,@ProjectID,@IsFinish,@SerialNO,@Name,@Type)"; 
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = sql;
                        foreach (CRailwayProject p in cl)
                        {
                            if (p.mPierList.Count == 0) continue;

                            foreach (CRailwayPier dwp in p.mPierList)
                            {
                                for (int i = 0; i < dwp.mStubDone.Count; i++)
                                {
                                    SQLiteParameter[] parameters = new SQLiteParameter[]
                                    {
                                            new SQLiteParameter("@StubIndex",i),
                                            new SQLiteParameter("@ParentID",dwp.mDWID),
                                            new SQLiteParameter("@ProjectID",dwp.mParentID),
                                            new SQLiteParameter("@IsFinish",dwp.mStubDone[i]),
                                            new SQLiteParameter("@SerialNO",dwp.mStubSNOs[i]),
                                            new SQLiteParameter("@Name",i+""),                                            
                                            new SQLiteParameter("@Type",2+"")

                                    };
                                    command.Parameters.AddRange(parameters);
                                    command.ExecuteNonQuery();
                                    //command.Parameters.Clear();
                                }

                                for (int i = 0; i < dwp.mBeamDone.Count; i++)
                                {
                                    SQLiteParameter[] parameters = new SQLiteParameter[]
                                    {
                                            new SQLiteParameter("@StubIndex",i),
                                            new SQLiteParameter("@ParentID",dwp.mDWID),
                                            new SQLiteParameter("@ProjectID",dwp.mParentID),
                                            new SQLiteParameter("@IsFinish",dwp.mBeamDone[i]),
                                            new SQLiteParameter("@SerialNO",dwp.mBeamSNOs[i]),
                                            new SQLiteParameter("@Name",dwp.mBeamName[i]),                                            
                                            new SQLiteParameter("@Type",1+"")

                                    };
                                    command.Parameters.AddRange(parameters);
                                    command.ExecuteNonQuery();
                                    //command.Parameters.Clear();
                                }
                            }
                        }
                        transaction2.Commit();
                    }
                }
                
            }

        }

 
        public static void SaveProjectToSqlite(string dbPath, List<CRailwayProject> cl)
        {

            string sql = "INSERT INTO ProjectInfo(ProjectID,ParentID, ProjectName,ProfessionalName,ProfessionalCategoryCode,ShorName,Mileage_Start_Des,Mileage_End_Des, Mileage_Start,Mileage_End,SerialNo,UpdateTime,Direction,avgProgress,IsValid, PhotoUrl)" +
                "values(@ProjectID,@ParentID, @ProjectName,@ProfessionalName,@ProfessionalCategoryCode,@ShorName,@Mileage_Start_Des,@Mileage_End_Des,@Mileage_Start,@Mileage_End,@SerialNo,@UpdateTime,@Direction,@avgProgress,@IsValid,@PhotoUrl)";
            CSqliteWrapper db = new CSqliteWrapper(dbPath);
            db.ExecuteNonQuery("DELETE from ProjectInfo", null);
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbPath))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = sql;
                        foreach (CRailwayProject p in cl)
                        {

                            SQLiteParameter[] parameters = new SQLiteParameter[]
                            {
                                new SQLiteParameter("@ProjectID",p.mProjectID),
                                new SQLiteParameter("@ParentID",p.mParentID),
                                new SQLiteParameter("@ProjectName",p.ProjectName),
                                new SQLiteParameter("@ProfessionalName",p.ProfessionalName),
                                new SQLiteParameter("@ProfessionalCategoryCode",p.mProfessionalCode),
                                new SQLiteParameter("@ShorName",p.SegmentName),
                                new SQLiteParameter("@Mileage_Start_Des",p.Mileage_Start_Discription),
                                new SQLiteParameter("@Mileage_End_Des",p.Mileage_End_Discription),
                                new SQLiteParameter("@Mileage_Start",p.mMileage_Start),                                
                                new SQLiteParameter("@Mileage_End",p.mMileage_End),
                                new SQLiteParameter("@SerialNo",p.mSerialNo),
                                new SQLiteParameter("@UpdateTime",p.UpdateTime.ToShortDateString()),
                                new SQLiteParameter("@Direction",p.mDirection),
                                new SQLiteParameter("@avgProgress",p.mAvgProgress),
                                new SQLiteParameter("@IsValid",p.mIsValid),
                                new SQLiteParameter("@PhotoUrl",p.mPhotoUrl)
                            };

                            command.Parameters.AddRange(parameters);
                            command.ExecuteNonQuery();
                            //command.Parameters.Clear();

                        }

                        transaction.Commit();
                    }

                }
            }


            Console.WriteLine("工点信息存储完成");

        }

        public static void SavePrjectForLoaded(string dbPath, List<OnePrj> ls)
        {
            string sql = "INSERT INTO ProjectLoad(ProjectType,ProjectName,ProjectIndex,ProjectModel,LongitudeS,LatitudeS,AltitudeS,ProjectLength) " +
                "values(@ProjectType,@ProjectName,@ProjectIndex,@ProjectModel,@LongitudeS,@LatitudeS,@AltitudeS,@ProjectLength)";
            CSqliteWrapper db = new CSqliteWrapper(dbPath);
            db.ExecuteNonQuery("DELETE from ProjectLoad", null);
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbPath))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = sql;
                        foreach (OnePrj p in ls)
                        {

                            SQLiteParameter[] parameters = new SQLiteParameter[]
                            {
                                new SQLiteParameter("@ProjectType",p.prjType),
                                new SQLiteParameter("@ProjectName",p.prjName),
                                new SQLiteParameter("@ProjectIndex",p.prjIdx),
                                new SQLiteParameter("@ProjectModel",p.ModelName),
                                new SQLiteParameter("@LongitudeS",p.sx),
                                new SQLiteParameter("@LatitudeS",p.sy),
                                new SQLiteParameter("@AltitudeS",p.sz),
                                new SQLiteParameter("@ProjectLength",p.prjLength)
                            };

                            command.Parameters.AddRange(parameters);
                            command.ExecuteNonQuery();
                            //command.Parameters.Clear();

                        }

                        transaction.Commit();
                    }

                }
            }


            Console.WriteLine("模型加载信息存储完成");

        }

        /// <summary>
        /// FIXME 表格已经换了，这个需要修正
        /// </summary>
        /// <param name="dbPath"></param>
        /// <param name="cl"></param>
        public static void SaveLineListToSqlite(string dbPath, List<CRailwayLine> cl)
        {

            string sql = "INSERT INTO ChainInfo(fromMeter,toMeter,fromID,toID,DKCode) values (@fromMeter,@toMeter,@fromID,@toID,@DKCode)";
            string sql2 = "INSERT INTO MileageInfo(Mileage,Longitude,Latitude,Altitude,MileageID,MileagePrefix) values (@Mileage,@Longitude,@Latitude,@Altitude,@MileageID,@MileagePrefix)";
            //CSqliteWrapper db = new CSqliteWrapper(dbPath);

            int countID = 1;
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbPath))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = sql;

                        for (int i = 0; i < cl.Count; i++)
                        {
                            int toID = countID + cl[i].mPointNum - 1;
                            SQLiteParameter[] parameters = new SQLiteParameter[]
                            { 
                                new SQLiteParameter("@fromMeter",cl[i].mStart), 
                                new SQLiteParameter("@toMeter",cl[i].mEnd), 
                                new SQLiteParameter("@fromID",countID), 
                                new SQLiteParameter("@toID",toID), 
                                new SQLiteParameter("@DKCode",cl[i].mDKCode) 
                            };
                            command.Parameters.AddRange(parameters);
                            command.ExecuteNonQuery();
                            //db.ExecuteNonQuery(sql, parameters);
                            countID = toID + 1;

                        }


                    }
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = sql2;
                        countID = 1;
                        bool isR;
                        for (int i = 0; i < cl.Count; i++)
                        {
                            isR = cl[i].mIsReverse;
                            for (int j = 0; j < cl[i].mPointNum; j++)
                            {
                                double mil;
                                if (isR)
                                    mil = cl[i].mStart - cl[i].meter[j];
                                else
                                    mil = cl[i].mStart + cl[i].meter[j];
                                SQLiteParameter[] parameters = new SQLiteParameter[]
                                { 
                                    new SQLiteParameter("@Mileage",mil), 
                                    new SQLiteParameter("@Longitude",cl[i].longitude[j]), 
                                    new SQLiteParameter("@Latitude",cl[i].latitude[j]), 
                                    new SQLiteParameter("@Altitude",cl[i].altitude[j]), 
                                    new SQLiteParameter("@MileageID",countID), 
                                    new SQLiteParameter("@MileagePrefix",cl[i].mDKCode), 
                                    //new SQLiteParameter("@LongitudeMars",cl[i].longitudeMars[j]), 
                                    //new SQLiteParameter("@LatitudeMars",cl[i].latitudeMars[j]) 
                                };
                                command.Parameters.AddRange(parameters);
                                command.ExecuteNonQuery();
                                countID++;

                            }

                        }
                    }

                    transaction.Commit();

                }
            }
            Console.WriteLine("里程信息存储完成");
        }


        public static void PrintDataTable(System.Data.DataTable dt)
        {

            List<string> ls;
            ls = GetDataTableColumnName(dt);
            foreach (string str in ls)
            {
                Console.Write(str + "\t");
            }
            Console.WriteLine();
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < ls.Count; i++)
                {
                    Console.Write(dr[i] + "\t");
                }
                Console.WriteLine();

            }
        }

        private static List<string> GetDataTableColumnName(System.Data.DataTable dt)
        {
            List<string> al = new List<string>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                al.Add(dt.Columns[i].ColumnName);
            }
            return al;
        }
    }
}
