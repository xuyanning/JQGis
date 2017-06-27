using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelInfo;
using TerraExplorerX;

namespace RailwayGIS.TEProject
{
    public class CTEConsPhoto:CTEObject
    {
        public static string mGroupIDStatic = null;

        public CTEConsPhoto(CRailwayScene s, CTEScene ss)
            : base(s, ss)
        {

            //string[] fileName;
            //string[] photoTime;
            //string[] sNo;
            //string[] person;
            //string[] remark;

            //var sgworld = new SGWorld66();
            //if (string.IsNullOrEmpty(mGroupIDStatic))
            //    mGroupIDStatic = sgworld.ProjectTree.CreateGroup("Photo");

            //int num;
            //num = CConsPhoto.findLatestPhotos(5, out fileName, out photoTime, out sNo, out person, out remark);
            ////num = CConsLog.findLast365Cons(out usrName, out projName, out consDate, out x, out y);
            //for (int i = 0; i < num; i++)
            //{
            //    Console.WriteLine("{0} #\t: fileName {1}\t  Date {2}", i,  fileName[i], photoTime[i]);
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupID"></param>
        public override void TECreate()
        {
            //    xs = cVideo.mLongitude_Mid;
            //    ys = cVideo.mLatitude_Mid;
            //    zs = 250;
            //    cp = sgworld.Creator.CreatePosition(xs, ys, zs, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, 0, -80, 0, 2000);

            //    iVideo = sgworld.Creator.CreateVideoOnTerrain(surl + @"Common\Textures\liangchang.jpg", cp, mGroupIDStatic, cVideo.ProjectName);
            //    //iVideo.PlayVideoOnStartup = false;
            //    iVideo.ProjectionFieldOfView = 45;
            //    //if (count < 2)
            //    //{
            //    //    iVideo.VideoFileName = @"D:\GISData\Common\Textures\x1.avi";
            //    //    count++;
            //    //}

            //    //  iVideo.pl
        }

        public override void TEUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
