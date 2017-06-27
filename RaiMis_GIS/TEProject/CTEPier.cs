using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ModelInfo;
using TerraExplorerX;

namespace RailwayGIS.TEProject
{
    /// <summary>
    /// 处理桥墩
    /// </summary>
    public class CTEPier: CTEObject
    {
        string mGroupIDStatic = null;
        public CTEPier(CRailwayScene s, CTEScene ss)
            : base(s, ss)
        {


        }

        public void LoadPierLabel()
        {
            var sgworld = new SGWorld66();
            //if (string.IsNullOrEmpty(mGroupIDStatic))
            //    mGroupIDStatic = sgworld.ProjectTree.CreateGroup("Pier");
            ////string modelName = CGisDataSettings.gDataPath + @"Common\Models\Pier\qiaodun.xpl2";
            ////ITerrainModel66 m = null;
            ITerrainLabel66 labelSign;

            foreach (CRailwayProject proj in mSceneData.mContBeamList)
            {
                //var branch = sgworld.ProjectTree.FindItem("Bridge\\"+cItem.mParentBridge.mProjectName);
                foreach (CRailwayPier cItem in proj.mPierList)
                {
                    var cPos = sgworld.Creator.CreatePosition(cItem.mLongitude_Mid, cItem.mLatitude_Mid, cItem.mAltitude_Mid + 2, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE,
                        cItem.mHeading_Mid, 0, 0);
                    //m = sgworld.Creator.CreateModel(cPos, modelName, 1, ModelTypeCode.MT_NORMAL, mGroupIDStatic, cItem.DWName);
                    //m.ScaleX = 0.01;
                    //m.ScaleY = 0.01;
                    //m.ScaleZ = 0.015;
                    //m.BestLOD = 1000;
                    //cPos.Altitude += 4;


                    labelSign = sgworld.Creator.CreateTextLabel(cPos, cItem.DWName, CRWTEStandard.mLabelStyleL5, mGroupIDStatic, "Pie|" + cItem.mSerialNo);
                    labelSign.Message.MessageID = sgworld.Creator.CreateMessage(MsgTargetPosition.MTP_POPUP, proj.ToString(), MsgType.TYPE_TEXT, true).ID;

                    //FIXME 添加属性随时间变动功能
                    //cItem.dte = DateTime.Now.AddDays(100); ;
                    //DateTime ds = Convert.ToDateTime("2015-06-01");
                    //cItem.dts = ds.AddDays(i);
                    //cItem.mTEModel.TimeSpan.Start = cItem.dts;
                    //cItem.mTEModel.TimeSpan.End = cItem.dte;
                    //i += 10;
                    //if (i > 180) { i = 0; }
                    //cItem.mTEModel.Terrain.Tint.abgrColor = 0xFF;
                    //cItem.mTEModel.Terrain.Tint.SetAlpha(alpha); // FIXME;

                    //alpha += 0.05;
                    //if (alpha > 1) alpha = 0.02;
                }
            }

            foreach (CRailwayProject proj in mSceneData.mBridgeList)
            {
                //var branch = sgworld.ProjectTree.FindItem("Bridge\\"+cItem.mParentBridge.mProjectName);
                foreach (CRailwayPier cItem in proj.mPierList)
                {
                    var cPos = sgworld.Creator.CreatePosition(cItem.mLongitude_Mid, cItem.mLatitude_Mid, cItem.mAltitude_Mid + 1, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE,
                        cItem.mHeading_Mid, 0, 0);
                    //m = sgworld.Creator.CreateModel(cPos, modelName, 1, ModelTypeCode.MT_NORMAL, mGroupIDStatic, cItem.DWName);
                    //m.ScaleX = 0.01;
                    //m.ScaleY = 0.01;
                    //m.ScaleZ = 0.015;
                    //m.BestLOD = 1000;
                    //cPos.Altitude += 4;

                    labelSign = sgworld.Creator.CreateTextLabel(cPos, cItem.DWName, CRWTEStandard.mLabelStyleL4, mGroupIDStatic, "Pie|" + cItem.mSerialNo);
                    labelSign.Message.MessageID = sgworld.Creator.CreateMessage(MsgTargetPosition.MTP_POPUP, proj.ToString(), MsgType.TYPE_TEXT, true).ID;
                    //FIXME 添加属性随时间变动功能
                    //cItem.dte = DateTime.Now.AddDays(100); ;
                    //DateTime ds = Convert.ToDateTime("2015-06-01");
                    //cItem.dts = ds.AddDays(i);
                    //cItem.mTEModel.TimeSpan.Start = cItem.dts;
                    //cItem.mTEModel.TimeSpan.End = cItem.dte;
                    //i += 10;
                    //if (i > 180) { i = 0; }
                    //cItem.mTEModel.Terrain.Tint.abgrColor = 0xFF;
                    //cItem.mTEModel.Terrain.Tint.SetAlpha(alpha); // FIXME;

                    //alpha += 0.05;
                    //if (alpha > 1) alpha = 0.02;
                }
            }

        }
        public override void TECreate()
        {
            var sgworld = new SGWorld66();
            if (string.IsNullOrEmpty(mGroupIDStatic))
                mGroupIDStatic = sgworld.ProjectTree.CreateGroup("Pier");

            //Thread t1 = new Thread(new ThreadStart(LoadPierLabel));
            //t1.Start();

            //ITerrainLabel66 labelSign;

            //foreach (CRailwayProject proj in mSceneData.mBridgeList)
            //{
            //    //var branch = sgworld.ProjectTree.FindItem("Bridge\\"+cItem.mParentBridge.mProjectName);
            //    foreach (CRailwayPier cItem in proj.mPierList)
            //    {
            //        var cPos = sgworld.Creator.CreatePosition(cItem.mLongitude_Mid, cItem.mLatitude_Mid, cItem.mAltitude_Mid +1, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE,
            //            cItem.mHeading_Mid, 0, 0);
            //        //m = sgworld.Creator.CreateModel(cPos, modelName, 1, ModelTypeCode.MT_NORMAL, mGroupIDStatic, cItem.DWName);
            //        //m.ScaleX = 0.01;
            //        //m.ScaleY = 0.01;
            //        //m.ScaleZ = 0.015;
            //        //m.BestLOD = 1000;
            //        //cPos.Altitude += 4;

            //        labelSign = sgworld.Creator.CreateTextLabel(cPos, cItem.DWName, CRWTEStandard.mLabelStyleL4, mGroupIDStatic, "Pie|" + cItem.mSerialNo);
            //        labelSign.Message.MessageID = sgworld.Creator.CreateMessage(MsgTargetPosition.MTP_POPUP, proj.ToString(), MsgType.TYPE_TEXT, true).ID;
            //        //FIXME 添加属性随时间变动功能
            //        //cItem.dte = DateTime.Now.AddDays(100); ;
            //        //DateTime ds = Convert.ToDateTime("2015-06-01");
            //        //cItem.dts = ds.AddDays(i);
            //        //cItem.mTEModel.TimeSpan.Start = cItem.dts;
            //        //cItem.mTEModel.TimeSpan.End = cItem.dte;
            //        //i += 10;
            //        //if (i > 180) { i = 0; }
            //        //cItem.mTEModel.Terrain.Tint.abgrColor = 0xFF;
            //        //cItem.mTEModel.Terrain.Tint.SetAlpha(alpha); // FIXME;

            //        //alpha += 0.05;
            //        //if (alpha > 1) alpha = 0.02;
            //    }
            //}
            //foreach (CRailwayProject proj in mSceneData.mContBeamList)
            //{
            //    //var branch = sgworld.ProjectTree.FindItem("Bridge\\"+cItem.mParentBridge.mProjectName);
            //    foreach (CRailwayPier cItem in proj.mPierList)
            //    {
            //        var cPos = sgworld.Creator.CreatePosition(cItem.mLongitude_Mid, cItem.mLatitude_Mid, cItem.mAltitude_Mid +2, AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE,
            //            cItem.mHeading_Mid, 0, 0);
            //        //m = sgworld.Creator.CreateModel(cPos, modelName, 1, ModelTypeCode.MT_NORMAL, mGroupIDStatic, cItem.DWName);
            //        //m.ScaleX = 0.01;
            //        //m.ScaleY = 0.01;
            //        //m.ScaleZ = 0.015;
            //        //m.BestLOD = 1000;
            //        //cPos.Altitude += 4;


            //        labelSign = sgworld.Creator.CreateTextLabel(cPos, cItem.DWName, CRWTEStandard.mLabelStyleL5, mGroupIDStatic, "Pie|" + cItem.mSerialNo);
            //        labelSign.Message.MessageID = sgworld.Creator.CreateMessage(MsgTargetPosition.MTP_POPUP, proj.ToString(), MsgType.TYPE_TEXT, true).ID;

            //        //FIXME 添加属性随时间变动功能
            //        //cItem.dte = DateTime.Now.AddDays(100); ;
            //        //DateTime ds = Convert.ToDateTime("2015-06-01");
            //        //cItem.dts = ds.AddDays(i);
            //        //cItem.mTEModel.TimeSpan.Start = cItem.dts;
            //        //cItem.mTEModel.TimeSpan.End = cItem.dte;
            //        //i += 10;
            //        //if (i > 180) { i = 0; }
            //        //cItem.mTEModel.Terrain.Tint.abgrColor = 0xFF;
            //        //cItem.mTEModel.Terrain.Tint.SetAlpha(alpha); // FIXME;

            //        //alpha += 0.05;
            //        //if (alpha > 1) alpha = 0.02;
            //    }
            //}
        }

        public override void TEUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
