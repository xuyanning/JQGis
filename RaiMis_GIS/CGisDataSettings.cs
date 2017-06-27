using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Xml;
using System.IO;
using ModelInfo.Helper;

namespace RailwayGIS
{
    public class ProjectConfig
    {
        public string projectName;
        public string projectUrl;
        public string userName;
        public string userPSD;
        public string projectLocalPath;
    }

    public class CGisDataSettings
    {
        public static string gDataPath;
        public static string gLocalDB;
        public static List<ProjectConfig> gProjectList = new List<ProjectConfig>();
        public static ProjectConfig gCurrentProject;
        public static string TrainPath;
        public static string TexturePath;
        public static int AppSpeed;


        public static void initGisDataSettings(){
            try
            {
                gDataPath = ConfigurationManager.AppSettings["DataPath"];
                gLocalDB = ConfigurationManager.AppSettings["DatabasePath"];
                TrainPath = ConfigurationManager.AppSettings["TrainPath"]; ;
                TexturePath = ConfigurationManager.AppSettings["TexturePath"];
                AppSpeed = Convert.ToInt32(ConfigurationManager.AppSettings["NavigationSpeed"]);
                // 五级速度，1，2，4，8，16倍速，对应20，40，80，160，320KM
                AppSpeed = Math.Min(5,Math.Max(AppSpeed, 1));
                
                //ModelInfo.Helper.LogHelper.WriteLog("数据路径为：" + gDataPath);
                string sConfigFile = "Project.config";
                if (!File.Exists(sConfigFile))
                {
                    ModelInfo.Helper.LogHelper.WriteLog("配置文件不存在：" + sConfigFile);
                    Environment.Exit(-1);
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(sConfigFile);

                XmlNodeList nodeList = xmlDoc.SelectNodes("/Configuration/Project");

                foreach (XmlNode xxNode in nodeList)
                {
                    //XmlNodeList childList = xxNode.ChildNodes; //取得row下的子节点集合
                    ProjectConfig pc = new ProjectConfig();
                    pc.projectName = xxNode.Attributes["ProjectName"].Value;
                    pc.projectUrl = xxNode.Attributes["ProjectUrl"].Value;
                    pc.userName = xxNode.Attributes["UserName"].Value;
                    pc.projectLocalPath = xxNode.Attributes["ProjectLocalPath"].Value;
                    gProjectList.Add(pc);

                }
                //FIXME 取第一个工程，如果一个工程也没有如何处理
                gCurrentProject = gProjectList.First();
            }
            catch (Exception ee)
            {
                LogHelper.WriteLog("配置文件.config读取错误", ee);
                Environment.Exit(-1);
            }
        }



        //FIXME 程序退出时保存用户状态
        public static void UpdateConfigInfo()
        {
            gLocalDB = gDataPath + gCurrentProject.projectLocalPath + gLocalDB;
            if (!File.Exists(gLocalDB))
            {
                ModelInfo.Helper.LogHelper.WriteLog("本地数据库不存在：" + gLocalDB);
                Environment.Exit(-1);
            }
            //else
            //{
            //    ModelInfo.Helper.LogHelper.WriteLog("本地数据库为" + gLocalDB);
            //}

        }
    }
}
