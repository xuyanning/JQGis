using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraExplorerX;

using System.Drawing;

namespace RailwayGIS.TEProject
{
    public class CRWTEStandard
    {
        public static ILabelStyle66 mLabelStyleL1 = null;  // 全线 12KM分辨率 ，应用于建设单位，施工单位，监理单位， 车站
        public static ILabelStyle66 mLabelStyleL2 = null; //标段 3KM 分辨率， 制梁场，项目部，
        public static ILabelStyle66 mLabelStyleL3 = null; // 工点 600M 分辨率 工点，千米标
        public static ILabelStyle66 mLabelStyleL4 = null; // 单位 20M 分辨率 墩，梁，百米标
        public static ILabelStyle66 mLabelStyleL5 = null; // 单位 20M 连续梁墩

        public static Color[] backColor = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Purple, Color.Orange };
        private static IColor66[] mColor;
        private static int colorIndex = 0;

        public static void Init(){
            SGWorld66 sgworld = new SGWorld66();

            mLabelStyleL1= sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);
            {
                mLabelStyleL1.FontName = "Arial";                         // Set font name to Arial
                mLabelStyleL1.Italic = false;                              // Set label style font to italic
                mLabelStyleL1.Scale = 200;                                // Set label style scale
                mLabelStyleL1.TextOnImage = false;
                mLabelStyleL1.TextColor = sgworld.Creator.CreateColor(255, 255, 255, 255);
                mLabelStyleL1.SmallestVisibleSize = 7;
                mLabelStyleL1.FontSize = 12;
                mLabelStyleL1.LineToGround = true;        
            }

            mLabelStyleL2 = sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);
            {
                mLabelStyleL2.FontName = "Arial";                         // Set font name to Arial
                mLabelStyleL2.Italic = false;                              // Set label style font to italic
                mLabelStyleL2.Scale = 50;                                // Set label style scale
                mLabelStyleL2.TextOnImage = false;
                mLabelStyleL2.TextColor = sgworld.Creator.CreateColor(0, 255, 0, 255);
                mLabelStyleL2.SmallestVisibleSize = 7;
                mLabelStyleL2.FontSize = 12;
                mLabelStyleL2.LineToGround = true;
            }

            mLabelStyleL3 = sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);
            {
                mLabelStyleL3.FontName = "Arial";                         // Set font name to Arial
                mLabelStyleL3.Italic = false;                              // Set label style font to italic
                mLabelStyleL3.Scale = 10;                                // Set label style scale
                mLabelStyleL3.TextOnImage = false;
                mLabelStyleL3.TextColor = sgworld.Creator.CreateColor(0, 255, 255, 255);
                mLabelStyleL3.SmallestVisibleSize = 7;
                mLabelStyleL3.FontSize = 12;
            }

            mLabelStyleL4 = sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);
            {
                mLabelStyleL4.FontName = "Arial";                         // Set font name to Arial
                mLabelStyleL4.Italic = false;                              // Set label style font to italic
                mLabelStyleL4.Scale = 1;                                // Set label style scale
                mLabelStyleL4.TextOnImage = false;
                mLabelStyleL4.TextColor = sgworld.Creator.CreateColor(0, 0, 255, 255);
                mLabelStyleL4.SmallestVisibleSize = 7;
                mLabelStyleL4.FontSize = 12;
            }
            
            mLabelStyleL5 = sgworld.Creator.CreateLabelStyle(SGLabelStyle.LS_DEFAULT);
            {
                mLabelStyleL5.FontName = "Arial";                         // Set font name to Arial
                mLabelStyleL5.Italic = false;                              // Set label style font to italic
                mLabelStyleL5.Scale = 2;                                // Set label style scale
                mLabelStyleL5.TextOnImage = false;
                mLabelStyleL5.TextColor = sgworld.Creator.CreateColor(255, 0, 0, 255);
                mLabelStyleL5.SmallestVisibleSize = 7;
                mLabelStyleL5.FontSize = 12;
            } 
            
            mColor = new IColor66[6];
            mColor[0] = sgworld.Creator.CreateColor(255, 0, 0, 255);
            mColor[1] = sgworld.Creator.CreateColor(0, 255, 0, 255);
            mColor[2] = sgworld.Creator.CreateColor(0, 0, 255, 255);
            mColor[3] = sgworld.Creator.CreateColor(0, 255, 255, 255);
            mColor[4] = sgworld.Creator.CreateColor(255, 0, 255, 255);
            mColor[5] = sgworld.Creator.CreateColor(255, 255, 0, 255);  
        }

        public static IColor66 nextColor()
        {
            //IColor66 cl;
            if (colorIndex == mColor.Length)
                colorIndex = 0;
            return mColor[colorIndex++];
            //colorIndex++;


        }

    }
}
