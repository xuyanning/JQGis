using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.Web;

namespace GISUtilities
{
    public class CConsPhoto
    {
        //<xs:element name="PhotoName" type="xs:string" minOccurs="0"/>
        //<xs:element name="FileName" type="xs:string" minOccurs="0"/>
        //<xs:element name="FileSize" type="xs:double" minOccurs="0"/>
        //<xs:element name="PhotoTime" type="xs:dateTime" minOccurs="0"/>
        //<xs:element name="Longitude" type="xs:double" minOccurs="0"/>
        //<xs:element name="Latitude" type="xs:double" minOccurs="0"/>
        //<xs:element name="ReferenceSerialNo" type="xs:string" minOccurs="0"/>
        //<xs:element name="Person" type="xs:string" minOccurs="0"/>
        //<xs:element name="Remark" type="xs:string" minOccurs="0"/>

        //public static int findLatestPhotos(int topN, out string[] fileName, out string[] photoTime, out string[] sNo,out string[] person, out string[] remark )
        //{
        //    int num = 0;
        //    fileName = photoTime = sNo = person = remark = null;
            

        //    //DataTable dt = CServerWrapper.findConsInfo(DateTime.Now.AddDays(-30).Date.ToString("u"));
        //    DataTable dt = CServerWrapper.findLatestImage(topN);
        //    if (dt == null)
        //        return num;

        //    num = dt.Rows.Count;

        //    if (num == 0) return 0;

        //    fileName = new string[num];
        //    photoTime = new string[num];
        //    sNo = new string[num];
        //    person = new string[num];
        //    remark = new string[num];


        //    int i = 0;
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        //fileName[i] = CGisDataSettings.gCurrentProject.projectUrl + "/" + dr["FileName"].ToString();
        //        fileName[i] = "/" + dr["FileName"].ToString();
        //        //      Console.WriteLine(fileName[i]);
        //        photoTime[i] = dr["PhotoTime"].ToString();
        //        sNo[i] = dr["ReferenceSerialNo"].ToString();
        //        person[i] = dr["Person"].ToString();
        //        remark[i] = dr["Remark"].ToString();

        //        i++;

        //    }
        //    return num;
        //}

    }
}
