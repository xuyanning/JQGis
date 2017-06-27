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
    public partial class FormMiddleLine : Form
    {
        //CRailwayScene gRWScene = null;

        public FormMiddleLine()
        {
            InitializeComponent();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if (gRWScene == null )
            //    gRWScene=new CRailwayScene(@"C:\GISData\jiqing\gisDB.db", "JQMIS.CN");
            //new CMiddleLineForMax(gRWScene);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //if (gRWScene == null)
            //gRWScene = new CRailwayScene(@"C:\GISData\jiqing\gisDB.db", "JQMIS.CN");
            //gRWScene.savePierLocal(@"C:\GISData\jiqing\gisdb.db");
            //CRailwayLineList.CreateLinelistFromExcel(@"d:\chain0616.xlsx");
            CRailwayLineList.CreateLinelistFromSqlite(@"C:\GISData\jiqing\gisdb.db");
            CRailwayLineList.createFloydPath();
        }

 

        private void buttonX1_Click(object sender, EventArgs e)
        {
            //if (gRWScene == null)
            //    gRWScene = new CRailwayScene(@"C:\GISData\jiqing\gisDB.db", "JQMIS.CN");
            //gRWScene.savePolePosLocal(@"C:\GISData\jiqing\gisdb.db");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new CCurveHeight().test();
        }
    }
}
