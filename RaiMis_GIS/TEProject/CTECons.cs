using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelInfo;
using TerraExplorerX;

namespace RailwayGIS.TEProject
{
    /// <summary>
    /// 实名制可视化，可视化施工点的人员信息
    /// </summary>
    public class CTECons : CTEObject
    {
        //Dictionary<string, ConsLocation> mPairs = new Dictionary<string, ConsLocation>();

        public ConsLocation consLoc;
        ITerrainRegularPolygon66 circlePeople;
        //ITerrainPolyline66 polylineTodo;
        

        //public static string mGroupIDStatic = null;
        public static string mGroupIDDynamic = null;

        private SGWorld66 sgworld;


        public CTECons(ConsLocation cl, CRailwayScene s, CTEScene ss)
            : base(s,ss)
        {
            sgworld = new SGWorld66();
            consLoc = cl;
            IPosition66 p = sgworld.Creator.CreatePosition(cl.Longitude, cl.Latitude, 30, AltitudeTypeCode.ATC_TERRAIN_RELATIVE,
              0, -90.0, 0, 0);
            circlePeople = sgworld.Creator.CreateCircle(p, cl.Number * 2 + 200, 0xFF0000FF, 0x00FF00FF, mGroupIDDynamic, cl.ProjName + cl.Number);
            //  circle.de
            circlePeople.LineStyle.Width = -3.0;
            circlePeople.Visibility.MinVisibilityDistance = 2000;
            //circle.SetParam

            labelSign = sgworld.Creator.CreateLabel(p, cl.Number + "", CGisDataSettings.gDataPath + @"Common\Textures\working.gif", CRWTEStandard.mLabelStyleL2, mGroupIDDynamic, "Con|" + cl.ProjName);
            //labelSign.Message.MessageID = sgworld.Creator.CreateMessage(MsgTargetPosition.MTP_POPUP, cl.ToString(), MsgType.TYPE_TEXT, true).ID;       
        }

        /// <summary>
        /// TODO 丁一明，显示聚类结果
        /// </summary>
        /// <param name="groupID"></param>
        public override void TECreate()
        {
 
           

        }

        public override void TEUpdate()
        {
            var sgworld = new SGWorld66();
            //var branch = sgworld.ProjectTree.FindItem(groupID);
            //if (!string.IsNullOrEmpty(branch))
            //    sgworld.ProjectTree.DeleteItem(branch);
            sgworld.ProjectTree.DeleteItem(mGroupIDDynamic);
            mGroupIDDynamic = null;            
            this.TECreate();
            //sgworld.ProjectTree.
            //var current = sgworld.ProjectTree.GetNextItem(branch, ItemCode.CHILD);
            //while (string.IsNullOrEmpty(current) == false) { 

            //}
           // var current = sgworld.ProjectTree.GetNextItem(child, ItemCode.NEXT);

        }
    }


    //        $$PARTICLE$$UserDefine: 
    // <?xml version='1.0' encoding='UTF-8'?> 
    // <Particle ID='Custom'><ParticleEmitter ID='ring' NumParticles='130' Texture='fire.png'>
    //<Emitter Rate='13' Shape='Ring' SpeedShape='Ring' Scale='0,0,0' Speed='1,1,1' />
    //<Cycle Value='1' />
    //<Sort Value='1' />
    //<Rotation Speed='1' Time='2' Initial='0' />
    //<Render Value='Billboard' />
    //<Gravity Value='0, 1, 0' />
    //<Force Value='0' OverrideRotation='0' />
    //<Position Value='0, 0, 0' />
    //<Life Value='3.06' />
    //<Speed Value='1.41' />
    //<Color Value='20,0,255,255' />
    //<Size Value='2.4,2.4' />
    //<Drag Value='1' />
    //<Blend Type='' />
    //<Fade FadeIn='0.47' FadeOut='0.65' MaxFade='0.28' />
    //</ParticleEmitter>
    //<ParticleEmitter ID='ring' NumParticles='62' Texture='CampFireBrightSmall.png'>
    //<Emitter Rate='8' Shape='Disc' SpeedShape='Disc' Scale='0.6,0.7,0.6' Speed='1,1,1' />
    //<Cycle Value='1' />
    //<Sort Value='1' />
    //<Render Value='Billboard' />
    //<Gravity Value='0, 2, 0' />
    //<Force Value='0' OverrideRotation='0' />
    //<Position Value='0, 0, 0' />
    //<Life Value='1.12' />
    //<Speed Value='1' />
    //<Color Value='20,255,255,255' />
    //<Size Value='2.1,2.1' />
    //<Drag Value='0' />
    //<Blend Type='' />
    //<Fade FadeIn='0.16' FadeOut='0.15' MaxFade='0.07' />
    //</ParticleEmitter>
    //</Particle>
}
