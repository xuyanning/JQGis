using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using ModelInfo;
using TerraExplorerX;

namespace RailwayGIS.TEProject
{
    public enum ShowFormStatus  {eReadyForNext, eStarting, eClosing}
    public class CTEScene
    {
        public CRailwayScene mSceneData;
        //public CRWTEStandard mRWStandard;
        private SGWorld66 sgworld;

        public MessageForm prjInfoForm;

        public DateTime fromDate = DateTime.Now.AddDays(-7);
        public DateTime toDate = DateTime.Now;
  

        public List<CTEProject> mTEProjList = new List<CTEProject>();
        public List<CTEFirm> mTEFirmList = new List<CTEFirm>();
        public List<CTECons> mTEConsList = new List<CTECons>();
        public CTEPier mTEPiers = null;

        //public delegate void StopAround();
        //public event StopAround ShowFinished = null;


        public CTEScene(CRailwayScene s, System.Windows.Forms.Timer timer)
        {
            mSceneData = s;
            //timerFly = timer;
            //timerFly.Stop();
            //timerFly.Tick += TimerFly_Tick;
            //mRWStandard = new CRWTEStandard();
            sgworld = new SGWorld66();
            CRWTEStandard.Init();
            //CTEFeature.linesPointsLoad("JqPoints1");
            //CTEFeature.linesPointsLoad("JqPoints2");
            //initTEContactLine();
            initTEProjectList();
            initTEFirmList();
            initTEConsList(CGisDataSettings.gLocalDB);
            mTEPiers = new CTEPier(mSceneData, this);
            mTEPiers.TECreate();
            //CTEFirm.NavigationFinished += NotifyShowFinish;
            //CTEPointNav.NavigationFinished += NotifyShowFinish;

            prjInfoForm = new MessageForm();
            

            //panel1_Paint(p);
            //mTEItemList.Add(new CTEKML(s, this, mRWStandard));
            //mTEItemList.Add(new CTEStation(s, this, mRWStandard));

            //mTEItemList.Add(new CTENavTrain(s, this, mRWStandard));

            //mTEItemList.Add(new CTEProject(s, this, mRWStandard));
            //mTEItemList.Add(new CTECons(s, this, mRWStandard));
            //mTEItemList.Add(new CTEMiddleLine(s, this, mRWStandard));
            //mTEItemList.Add(new CTEFirm(s, this, mRWStandard));



            //mTEItemList.Add(new CTEConsPhoto(s, mRWStandard));

            //mTEKml = new CTEKML(s, mRWStandard);
            //mTEStation = new CTEStation(s, mRWStandard);
            //mTEFact = new CTEFactory(s, mRWStandard);
            //mTETrain = new CTENavTrain(s, this, mRWStandard); 
            //mTEPier = new CTEPier(s, mRWStandard);
            //mTEProj = new CTEProject(s, mRWStandard);
            //mTECons = new CTECons(s, mRWStandard);
            //mTEMiddleLine = new CTEMiddleLine(s, mRWStandard);
            //mTEFirm = new CTEFirm(s, mRWStandard);
            //mTEPhotos = new CTEConsPhoto(s, this);
        }


        //public void NotifyShowFinish()
        //{

        //    ShowFinished?.Invoke();
        //}

        //private void TimerFly_Tick(object sender, EventArgs e)
        //{
        //    timerFly.Stop();
        //    NotifyShowFinish();
        //}
        private void initTEContactLine() {
            string contactGID="";
            ITerrainPolyline66 polyline;
            if (string.IsNullOrEmpty(contactGID))
                contactGID = sgworld.ProjectTree.CreateGroup("ContactLine");
            else
            {
                sgworld.ProjectTree.DeleteItem(contactGID);
                contactGID = sgworld.ProjectTree.CreateGroup("ContactLine");
            }

            foreach (CRailwayProject p in mSceneData.mTotalProjectList)
            {
                if (p.mAvgProgress < 0.01 || p is CContBeam)
                    continue;

                //count = proj.getSubLine(out cVerticesArray);
                int count;
                double[] xx, yy, zz,xx2,yy2,zz2;
                double[] cVerticesArray = null;
                p.mPath.getContactLine(out xx, out yy, out zz,out xx2,out yy2,out zz2);
                if (xx == null || xx.Length < 2)
                    continue;
                count = xx.Length;
                cVerticesArray = CTEObject.getVerArray(xx, yy, zz,5.6);
                polyline = sgworld.Creator.CreatePolylineFromArray(cVerticesArray, sgworld.Creator.CreateColor(0, 0, 0, 255),
                        AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, contactGID,  p.ProjectName+1);
                polyline.Spline = true;
                polyline.Visibility.MaxVisibilityDistance = 10000;
                polyline.Visibility.MinVisibilityDistance = 10;
                polyline.LineStyle.Width = -2;
                cVerticesArray = CTEObject.getVerArray(xx, yy, zz, 6.6);
                polyline = sgworld.Creator.CreatePolylineFromArray(cVerticesArray, sgworld.Creator.CreateColor(0, 0, 0, 255),
                        AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, contactGID, p.ProjectName+2);
                if (xx2 == null || xx2.Length < 2)
                    continue;
                count = xx2.Length;
                cVerticesArray = CTEObject.getVerArray(xx2, yy2, zz2, 5.6);
                polyline = sgworld.Creator.CreatePolylineFromArray(cVerticesArray, sgworld.Creator.CreateColor(0, 0, 0, 255),
                        AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, contactGID, p.ProjectName + 3);
                polyline.Spline = true;
                polyline.Visibility.MaxVisibilityDistance = 10000;
                polyline.Visibility.MinVisibilityDistance = 10;
                polyline.LineStyle.Width = -2;
                cVerticesArray = CTEObject.getVerArray(xx2, yy2, zz2, 6.6);
                polyline = sgworld.Creator.CreatePolylineFromArray(cVerticesArray, sgworld.Creator.CreateColor(0, 0, 0, 255),
                        AltitudeTypeCode.ATC_TERRAIN_ABSOLUTE, contactGID, p.ProjectName + 4);


            }
            Console.WriteLine(sgworld.ProjectTree.SaveAsKml("TELine", contactGID));
        }


        private void initTEProjectList()
        {
            if (string.IsNullOrEmpty(CTEProject.mGroupIDDynamic))
                CTEProject.mGroupIDDynamic = sgworld.ProjectTree.CreateGroup("Projects");
            else
            {
                sgworld.ProjectTree.DeleteItem(CTEProject.mGroupIDDynamic);
                CTEProject.mGroupIDDynamic = sgworld.ProjectTree.CreateGroup("Projects");
            }
            foreach (CRailwayProject p in mSceneData.mTotalProjectList)
            {
                mTEProjList.Add(new CTEProject(p,mSceneData,this));
            }
            //Console.WriteLine(sgworld.ProjectTree.SaveAsKml("TELine", CTEProject.mGroupIDDynamic));
        }

        public CTEObject showTEObject(CHotSpot hs1, CHotSpot hs2, bool isStopping = false)
        {
            CTEObject obj=null;
            switch (hs1.ObjectType)
            {
                case "Project":
                    CRailwayProject rp = (CRailwayProject)(hs1.ObjectRef);

                    //string s = "当前位置： " + rp.ToString() + "\t\t";

                    //labelRoll2.Text = s;
                    //labelRoll2.Refresh();

                    obj = showTEProject(rp,isStopping);


                    break;
                case "Firm":
                    CRailwayFirm rf = (CRailwayFirm)(hs1.ObjectRef);
                    obj = showTEFirms(rf,isStopping);
                    break;
                case "Cons":
                    ConsLocation cl = (ConsLocation)(hs1.ObjectRef);
                    obj = showTECons(cl, isStopping);
                    break;
            }
            return obj;
        }

        private CTEObject showTEProject(CRailwayProject rp, bool isStopping = false)
        {
            CTEProject tp = mTEProjList.Find(
                delegate (CTEProject tpp ) { return tpp.proj == rp; }); //findTEProject(rp);
            if (tp != null)
            {
                //double x, y, z, d;
                //tp.proj.getSpecialPoint(1, out x, out y, out z, out d);
                //CTEPointNav.showPoint(sgworld, x, y, z, d);
                if (isStopping) // && prjInfoForm.ws == WorkingStatus.Working
                    prjInfoForm.interruptShow();
                else
                {
                    prjInfoForm.setProject(rp);
                    prjInfoForm.preShow();
                }
                
            }
            return tp;
        }

        private void initTEFirmList()
        {
            if (string.IsNullOrEmpty(CTEFirm.mGroupIDStatic))
                CTEFirm.mGroupIDStatic = sgworld.ProjectTree.CreateGroup("Firms");
            else
            {
                sgworld.ProjectTree.DeleteItem(CTEFirm.mGroupIDStatic);
                CTEFirm.mGroupIDStatic = sgworld.ProjectTree.CreateGroup("Firms");
            }
            foreach (CRailwayFirm rw in mSceneData.mFirmList)
            {
                mTEFirmList.Add(new CTEFirm(rw, mSceneData, this));
            }
        }


        private CTEObject showTEFirms(CRailwayFirm rf, bool isStopping = false)
        {
            CTEFirm tf = mTEFirmList.Find(delegate(CTEFirm tff) { return tff.firm == rf; } );

            return tf;
        }

        private void initTEConsList(string dbFile)
        {
            if (string.IsNullOrEmpty(CTECons.mGroupIDDynamic))
                CTECons.mGroupIDDynamic = sgworld.ProjectTree.CreateGroup("Cons");
            else
            {
                sgworld.ProjectTree.DeleteItem(CTECons.mGroupIDDynamic);
                CTECons.mGroupIDDynamic = sgworld.ProjectTree.CreateGroup("Cons");
            }

            foreach (ConsLocation cl in mSceneData.mConsLoc)
            {
                mTEConsList.Add(new CTECons(cl, mSceneData, this));
            }
        }
        private CTEObject showTECons(ConsLocation cl, bool isStopping = false)
        {
            CTECons tf = mTEConsList.Find(delegate (CTECons tff) { return tff.consLoc == cl; });

            return tf;
        }

        public void updateProjectTree()
        {
            if (CServerWrapper.isConnected)
            {
                initTEProjectList();
                initTEFirmList();
                initTEConsList(CGisDataSettings.gLocalDB);
            }
            //foreach (CTERWItem rw in mTEItemList)
            //{
            //    if (rw is CTECons || rw is CTEProject)
            //        rw.TEUpdate();
            //}
        }

        //public void createProjectTree()
        //{
        //    //var sgworld = new SGWorld66();
            
        //    //各种铁路单位，工点，标注图标
        //    CTEFeature.linesPointsLoad("JqPoints1");
        //    CTEFeature.linesPointsLoad("JqPoints2");
        //    foreach (CTERWItem rw in mTEItemList)
        //    {
        //        rw.TECreate();
        //    }

        //    //sgworld.Terrain.Opacity = 0.3;

           

        //}
    }
}
