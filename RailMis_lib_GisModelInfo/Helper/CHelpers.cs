using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ThoughtWorks.QRCode.Codec;

namespace ModelInfo.Helper
{
    public class CGeneralHelpers
    {
        //This method converts the values to RGB
        public static void HslToRgb(int Hue, double Saturation, double Lightness, out int r, out int g, out int b)
        {
            double num4 = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            double num = ((double)Hue) % 360.0;
            double num2 = Saturation;
            double num3 = Lightness;
            if (num2 == 0.0)
            {
                num4 = num3;
                num5 = num3;
                num6 = num3;
            }
            else
            {
                double d = num / 60.0;
                int num11 = (int)Math.Floor(d);
                double num10 = d - num11;
                double num7 = num3 * (1.0 - num2);
                double num8 = num3 * (1.0 - (num2 * num10));
                double num9 = num3 * (1.0 - (num2 * (1.0 - num10)));
                switch (num11)
                {
                    case 0:
                        num4 = num3;
                        num5 = num9;
                        num6 = num7;
                        break;
                    case 1:
                        num4 = num8;
                        num5 = num3;
                        num6 = num7;
                        break;
                    case 2:
                        num4 = num7;
                        num5 = num3;
                        num6 = num9;
                        break;
                    case 3:
                        num4 = num7;
                        num5 = num8;
                        num6 = num3;
                        break;
                    case 4:
                        num4 = num9;
                        num5 = num7;
                        num6 = num3;
                        break;
                    case 5:
                        num4 = num3;
                        num5 = num7;
                        num6 = num8;
                        break;
                }
            }
            r = (int)(num4 * 255);
            g = (int)(num5 * 255);
            b = (int)(num6 * 255);

            //return (uint)((num4 * 255.0)* 65536 + (num5 * 255.0) * 256 + (num6 * 255.0) )* 256+ 255;
        }

        /// <summary>
        /// 拼接定位、指令 字符串 @"http://jqmis.cn/N/" + mInstanceID + "/" + rp.mSerialNo
        /// 拼接手机同步串 @"http://jqmis.cn/S/" + rp.mSerialNo
        /// </summary>
        /// <param name="srcStr"></param>
        /// <returns></returns>
        public static Image generate2DCode(string srcStr)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();

            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;

            qrCodeEncoder.QRCodeScale = 4;

            qrCodeEncoder.QRCodeVersion = 4;

            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;

            Image image;

            image = qrCodeEncoder.Encode(srcStr);

            return image;
        }
    }
}
