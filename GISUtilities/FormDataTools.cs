using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using ModelInfo;
//using ModelInfo.Helper;

namespace GISUtilities
{
    public partial class FormDataTools : Form
    {
        CRailwayScene gRWScene = null;

        public FormDataTools()
        {
            InitializeComponent();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (gRWScene == null)
                gRWScene =  new CRailwayScene(@"C:\GISData\jiqing\gisDB.db", "JQMIS.CN");//new CRailwayScene();//
            new CMiddleLineForMax(gRWScene);
            //CRailwayLineList.CreateLinelistFromSqlite(@"C:\GISData\jiqing\gisdb.db");
            //CRailwayLineList.testMainLine();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //if (gRWScene == null)
            //gRWScene = new CRailwayScene(@"C:\GISData\jiqing\gisDB.db", "JQMIS.CN");
            //gRWScene.savePierLocal(@"C:\GISData\jiqing\gisdb.db");
          
            btnHeight.Enabled = false;
            //CRailwayLineList.CreateLinelistFromSqlite(@"C:\GISData\jiqing\gisdb.db");
            CRailwayLineList.CreateLinelistFromExcel(@"C:\GISData\Common\chainInfo.xlsx");
            CRailwayLineList.createFloydPath(@"C:\GISData\Common\pd.xlsx");

            Console.WriteLine("subline计算完毕");
            btnHeight.Enabled = true;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            double x, y, z;
            CRailwayLineList.getGPSbyDKCodeFromDB("DK", 10000, out x, out y, out z);

        }
    }
}
