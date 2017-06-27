using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;
using System.Web.Services.Description;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;

using System.Data.OleDb;
using System.Data;

using Microsoft.CSharp;


namespace GISUtilities
{
    public class WebServiceAgent
    {
        private object agent;
        private Type agentType;
        private const string CODE_NAMESPACE = "Beyondbit.WebServiceAgent.Dynamic";

        public WebServiceAgent(string url)
        {
            XmlTextReader reader = new XmlTextReader(url + "?wsdl");

            //创建和格式化 WSDL 文档  
            ServiceDescription sd = ServiceDescription.Read(reader);

            //创建客户端代理代理类  
            ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
            sdi.AddServiceDescription(sd, null, null);
            //sdi.Style = ServiceDescriptionImportStyle.Server;

            //使用 CodeDom 编译客户端代理类  
            CodeNamespace cn = new CodeNamespace(CODE_NAMESPACE);
            CodeCompileUnit ccu = new CodeCompileUnit();
            ccu.Namespaces.Add(cn);
            sdi.Import(cn, ccu);
            CSharpCodeProvider icc = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters(new string[] { "System.Data.dll" }); // 在此添加所缺的dll，比如"System.Data.dll"
            CompilerResults cr = icc.CompileAssemblyFromDom(cp, ccu);
            agentType = cr.CompiledAssembly.GetTypes()[0];
            agent = Activator.CreateInstance(agentType);
        }

        public object Invoke(string methodName, params object[] args)
        {
            MethodInfo mi = agentType.GetMethod(methodName);
            return this.Invoke(mi, args);
        }

        public object Invoke(MethodInfo method, params object[] args)
        {
            object obj = null;
            try
            {
                obj = method.Invoke(agent, args);
            }
            catch
            {
                //LogHelper.WriteLog("Web服务暂时不可用");
                return null;
            }
            return obj;
        }

        public MethodInfo[] Methods
        {
            get
            {
                return agentType.GetMethods();
            }
        }
    }

    public class CServerWrapper
    {
        public static string mWebUrl;
        public static bool isConnected = false;
        public static WebServiceAgent mLoginAgent = null;
        public static WebServiceAgent mProjectAgent = null;
        public static WebServiceAgent mGisAgent = null;
        
        //www.railmis.cn
        public static bool ConnectToServer(string surl )
        {
            string url;
            mWebUrl = surl;
            //isConnected = false;
            try { 
                url = "http://" + mWebUrl + "/webservice/usrlogin.asmx";
                mLoginAgent = new WebServiceAgent(url);
                url = "http://" + mWebUrl + "/webservice/ProjectService.asmx";
                mProjectAgent = new WebServiceAgent(url);
                url = "http://" + mWebUrl + "/webservice/GisDataWebService.asmx";
                mGisAgent = new WebServiceAgent(url);


            }
            catch (Exception e)
            {
                Console.WriteLine("连接服务器失败");
                //LogHelper.WriteLog("连接服务器失败");
                return false;
            }
            //isConnected = true;
            return true;
        }
        
        
        public static string webLogin(string usrName, string usrPWD)
        {
            string resultStr = "NotConnected";
            if (isConnected)
            {                
                Object obj = mLoginAgent.Invoke("CheckUsrLogin", usrName, usrPWD);
                if (obj == null)
                    resultStr = "NotConnected";
                else
                    resultStr =  obj.ToString();

            }
            return resultStr;
        }

        //public static bool get
        //    ws_GetUTMDistance (string dkpre, double mileage, out double x, out double y, out double z)
        /// <summary>
        /// ToDO 
        /// </summary>
        /// <param name="projSNo"></param>
        /// <returns></returns>
        public static DataSet findProjHistory(string projSNo)
        {
            DataSet ds = null;
            if (isConnected)
            {
                ds = (DataSet)mProjectAgent.Invoke("ws_Bind_ProjectProgress_HistoryRate_DataSet",projSNo);
            }
            return ds;
        }
    ////        [WebMethod(Description = "增量获取工点信息")]
    ////[System.Web.Services.Protocols.SoapHeader("AuthHeader")]
    //    public static List<CRailwayProject> findChangedProj(string updateTime)
    //    {
    //        List<CRailwayProject> proList = null;
    //        if (isConnected)
    //        {

    //            proList = (List<CRailwayProject>)mProjectAgent.Invoke("FindChangedProject", updateTime);
    //        }
    //        return proList;
    //    }


        public static string findProjectCode(string projStr) {
            if (isConnected)
            {
                Object obj = mGisAgent.Invoke("ws_GetSysConfigation", projStr);
                if (obj == null)
                    return null;
                else
                    return obj.ToString();
            }
            return null;
        }

        public static string getScanMsg(int instanceID)
        {
            if (isConnected)
            {
                Object obj = mGisAgent.Invoke("ws_GetScanNavByScreenID", instanceID);
                if (obj == null)
                    return null;
                else
                    return obj.ToString();
            }
            return null;
        }

        public static void setScanMsg(int instanceID, string msg)
        {
            if (isConnected)
            {
                mGisAgent.Invoke("ws_SetScanNavByScreenID", instanceID, msg);
            }
        }

        public static DataTable findProjectInfo()
        {
            DataTable dt = null;
            if (isConnected)
            {
                dt = (DataTable)mGisAgent.Invoke("ws_FindProjectInfo",null);
            }
            return dt;
        }

        public static DataTable findDWProjectInfo()
        {
            DataTable dt = null;
            if (isConnected)
            {
                //FIX ME 365临时的，应该在服务端支持空串
                dt = (DataTable)mGisAgent.Invoke("ws_FindDWProjectInfo", DateTime.Now.AddDays(-365).Date.ToString("u")); 
            }
            return dt;
        }

        public static DataTable findChainInfo()
        {
            DataTable dt = null;
            if (isConnected)
            {
                dt = (DataTable)mGisAgent.Invoke("ws_FindChainInfo");
            }
            return dt;
        }

        public static DataTable findMileageInfo()
        {
            DataTable dt = null;
            if (isConnected)
            {
                dt = (DataTable)mGisAgent.Invoke("ws_FindMileageInfo");
            }
            return dt;
        }

        /// <summary>
        /// 由服务器获取实名制信息,取得实名制某日之后的人员工作情况,日期格式yyyy-mm-dd hh:mi:ss
        /// </summary>
        /// <param name="updateTime"></param>
        /// <returns></returns>
        public static DataTable findConsInfo(string fromTime = null, string toTime = null) {
            DataTable dt = null;
            if (isConnected)
            {
                dt = (DataTable)mGisAgent.Invoke("ws_FindConsInfo", fromTime,toTime);
            }
            return dt;
        }

        public static DataTable findClusterConsByProj(string fromTime = null, string toTime = null)
        {
            DataTable dt = null;
            if (isConnected)
            {
                dt = (DataTable)mGisAgent.Invoke("ws_FindClusterdConsbyProj", fromTime, toTime);
            }
            return dt;
        }

        public static DataTable findMsgLog(DateTime fromTime)
        {
            DataTable dt = null;
            if (isConnected)
            {
                dt = (DataTable)mGisAgent.Invoke("ws_GetProjectConsLogDescriptionByLastUpdateTime",fromTime);
            }
            return dt;
        }

        public static DataTable findClusterConsByPDW(string fromTime = null, string toTime = null)
        {
            DataTable dt = null;
            if (isConnected)
            {
                dt = (DataTable)mGisAgent.Invoke("ws_FindClusterdConsbyDW", fromTime, toTime);
            }
            return dt;
        }

        /// <summary>
        /// 由服务器获取最近的topName个施工影像信息
        /// </summary>
        /// <param name="updateTime"></param> 
        /// <returns>
        /// <xs:sequence>
        //<xs:element name="PhotoName" type="xs:string" minOccurs="0"/>
        //<xs:element name="FileName" type="xs:string" minOccurs="0"/>
        //<xs:element name="FileSize" type="xs:double" minOccurs="0"/>
        //<xs:element name="PhotoTime" type="xs:dateTime" minOccurs="0"/>
        //<xs:element name="Longitude" type="xs:double" minOccurs="0"/>
        //<xs:element name="Latitude" type="xs:double" minOccurs="0"/>
        //<xs:element name="ReferenceSerialNo" type="xs:string" minOccurs="0"/>
        //<xs:element name="Person" type="xs:string" minOccurs="0"/>
        //<xs:element name="Remark" type="xs:string" minOccurs="0"/>
        ///</xs:sequence>
        /// </returns>
        public static DataTable findLatestImage(int topNum = 5)
        {
            DataTable dt = null;
            if (isConnected)
            {
                dt = (DataTable)mGisAgent.Invoke("ws_FindImageInfoTop", topNum);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["FileName"] = "/" + dr["FileName"].ToString();

                    }
                }
            }
            return dt;
        }

        public static DataTable findPhotosByProjSNo(string sno)
        {
            string sql = @"select Top 5 FileName, PhotoTime, ReferenceSerialNo, Person, Remark from PhotoInfo where ReferenceSerialNo = '" + sno + @"' order by PhotoTime desc;";
            DataTable dt = null;
            if (isConnected)
            {
                dt = CServerWrapper.execSqlQuery(sql);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["FileName"] = "/ProjectPhoto/" + dr["ReferenceSerialNo"].ToString().Substring(0, 3) + "/" + Convert.ToDateTime(dr["PhotoTime"].ToString()).Year + "/" + dr["FileName"].ToString().Replace("#", "%23"); // HttpUtility.UrlEncode(dr["FileName"].ToString());

                    }
                }
            }
            return dt;
        }

        public static DataTable showPhotosByProjSNo(string sno)
        {
            //string sql = @"select Top 5 FileName, PhotoTime, ReferenceSerialNo, Person, Remark from PhotoInfo where ReferenceSerialNo = '" + sno + @"' order by PhotoTime desc;";
            DataTable dt = null;
            if (isConnected)
            {
                dt = (DataTable)mGisAgent.Invoke("ws_GetMediaSlideShowByProjectSerialNo", sno);
                //dt = CServerWrapper.execSqlQuery(sql);
                //if (dt != null)
                //{
                //    foreach (DataRow dr in dt.Rows)
                //    {
                //        dr["FileName"] = "/ProjectPhoto/" + dr["ReferenceSerialNo"].ToString().Substring(0, 3) + "/" + Convert.ToDateTime(dr["PhotoTime"].ToString()).Year + "/" + dr["FileName"].ToString().Replace("#", "%23"); // HttpUtility.UrlEncode(dr["FileName"].ToString());

                //    }
                //}
            }
            return dt;
        }
        /// <summary>
        /// 由服务器执行查询sql语句，调试用
        /// </summary>
        /// <param name="sql"></param>
        /// select * from (select * from FirmInfo)a, (select FirmTypeID,FirmTypeCategoryName from FirmTypeInfo)b where a.FirmTypeID=b.FirmTypeID and FirmTypeCategoryName='单位' or FirmTypeCategoryName='分支机构'
        /// <returns></returns>
        public static DataTable execSqlQuery(string sql)
        {


            DataTable dt = null;
            if (isConnected)
            {
                dt = (DataTable)mGisAgent.Invoke("ws_FindGeneralInfo", sql);
            }
            return dt;
        }

    }


}
