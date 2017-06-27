using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelInfo;
using TerraExplorerX;

namespace RailwayGIS.TEProject
{
    public class CTEFirm : CTEObject
    {
        //public delegate void StopAround();
        //public static event StopAround NavigationFinished = null;
        public CRailwayFirm firm;

        //string mPresentationGroupID;
        ITerrainRegularPolygon66 circlePeople;

        public static string mGroupIDStatic = null;
        //public static string mGroupIDDynamic = null;

        private SGWorld66 sgworld;

        public CTEFirm(CRailwayFirm f, CRailwayScene s, CTEScene ss)
            : base(s, ss)
        {
            firm = f;
            sgworld = new SGWorld66();
            if (!string.IsNullOrEmpty(firm.mPresentation))
            {
                var id = sgworld.ProjectTree.FindItem("\\Presentation\\" + firm.mPresentation);
                mPresentation = (IPresentation66)(sgworld.ProjectTree.GetObject(id));

                //if (CGisDataSettings.AppSpeed <= 2)
                //    mPresentation.PlaySpeedFactor = PresentationPlaySpeed.PPS_VERYSLOW;
                //else if (CGisDataSettings.AppSpeed < 5)
                //    mPresentation.PlaySpeedFactor = PresentationPlaySpeed.PPS_SLOW;
                //else if (CGisDataSettings.AppSpeed < 7)
                //    mPresentation.PlaySpeedFactor = PresentationPlaySpeed.PPS_NORMAL;
                //else if (CGisDataSettings.AppSpeed < 9)
                //    mPresentation.PlaySpeedFactor = PresentationPlaySpeed.PPS_FAST;
                //else
                //    mPresentation.PlaySpeedFactor = PresentationPlaySpeed.PPS_VERYFAST;


            }

            ILabelStyle66 cLabelStyle;
            IPosition66 cp = sgworld.Creator.CreatePosition(f.CenterLongitude, f.CenterLatitude, 0, AltitudeTypeCode.ATC_TERRAIN_RELATIVE);
            circlePeople = sgworld.Creator.CreateCircle(cp, f.NumStaff * 2 + 200, 0xFFFFFFFF, 0x00FF00FF, mGroupIDStatic, f.FirmName + " " + f.NumStaff);
            circlePeople.LineStyle.Width = -3.0;
            circlePeople.Visibility.MinVisibilityDistance = 5000;

            if (f.FirmType.Equals("制梁场") || f.FirmType.Equals("项目部") || f.FirmType.Equals("监理单位"))
                cLabelStyle = CRWTEStandard.mLabelStyleL2;
            else
                cLabelStyle = CRWTEStandard.mLabelStyleL1;
            labelSign = sgworld.Creator.CreateLabel(cp, f.FirmName, CGisDataSettings.gDataPath + @"Common\地标图片\" + f.mLabelImage, cLabelStyle, mGroupIDStatic, "Fir|"+f.FirmName);
            labelSign.Message.MessageID = sgworld.Creator.CreateMessage(MsgTargetPosition.MTP_POPUP, f.ToString(), MsgType.TYPE_TEXT, true).ID;


        }


        public override void TECreate()
        {
            

            //ITerrainLabel66 iLabel;
            

            //ILabelStyle66 cLabelStyle = sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_STREET);
            //ILabelStyle66 cLabelStyle = sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_STATE);

            //{
            //   //uint nBGRValue = 0xff0000;                              // Blue
            //   //double dAlpha = 0.5;                                    // 50% opacity
            //   //var cBackgroundColor = cLabelStyle.BackgroundColor;     // Get label style background color
            //   //cBackgroundColor.FromBGRColor(nBGRValue);               // Set background to blue
            //   //cBackgroundColor.SetAlpha(dAlpha);                      // Set transparency to 50%
            //   //cLabelStyle.BackgroundColor = cBackgroundColor;         // Set label style background color
            //    cLabelStyle.FontName = "Arial";                         // Set font name to Arial
            //    cLabelStyle.Italic = true;                              // Set label style font to italic
            //    cLabelStyle.Scale = 200;                                // Set label style scale
            //    cLabelStyle.TextOnImage = false;
            //    cLabelStyle.TextColor = sgworld.Creator.CreateColor(0, 255, 255, 255);
            //    //cLabelStyle.SmallestVisibleSize = 7;
            //}


            //
            // F. Add Message to created circle
            //
            //{
            //    ITerraExplorerMessage66 cMessage = null;
            //    // F1. Set message input parameters
            //    MsgTargetPosition eMsgTarget = MsgTargetPosition.MTP_POPUP;
            //    string tMessage = "Hello Circle";
            //    MsgType eMsgType = MsgType.TYPE_TEXT;
            //    bool bIsBringToFront = true;

            //    // F2. Create message and add to circle
            //    cMessage = sgworld.Creator.CreateMessage(eMsgTarget, tMessage, eMsgType, bIsBringToFront);
            //    //iLabel.Message.MessageID = cMessage.ID;
            //}
            //foreach (CRailwayStaff rs in mSceneData.mStaffList)
            //{
            //    IPosition66 cp = sgworld.Creator.CreatePosition(rs.longitude, rs.latitude, 10, AltitudeTypeCode.ATC_TERRAIN_RELATIVE);
            //    ITerrainRegularPolygon66 circle = sgworld.Creator.CreateCircle(cp, rs.numStaff * 2 + 200, 0xFFFF00FF, 0x00FF00FF, branch, rs.shorName + " " + rs.numStaff);
            //    circle.LineStyle.Width = -5.0;
            //    //circle.Tooltip = rs.shorName + " " + rs.numStaff;

            //}
        }

        public override void TEUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
