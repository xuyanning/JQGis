using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelInfo;
using TerraExplorerX;

namespace RailwayGIS.TEProject
{
    class CTEPointNav
    {
        public delegate void StopAround();
        public static event StopAround NavigationFinished = null;
        //private static SGWorld66 sgworld;
        //private static IPosition66 mPointNav;
        private static IPosition66[] mPointNear = new IPosition66[4];

        static string mPresentationGroupID;
        public static IPresentation66 mPresentation= null;

        public static void showPoint(SGWorld66 sgworld, double cx,double cy, double cz, double cdir)
        {

            if (mPresentation == null)
            {
                mPresentationGroupID = sgworld.ProjectTree.FindItem("Presentation");
                if (string.IsNullOrEmpty(mPresentationGroupID))
                {
                    mPresentationGroupID = sgworld.ProjectTree.CreateGroup("Presentation");
                }
                mPresentation = sgworld.Creator.CreatePresentation(mPresentationGroupID, "PointNav");
                sgworld.OnPresentationFlyToReachedDestination += Sgworld_OnPresentationFlyToReachedDestination;
            }
            mPresentation = sgworld.Creator.CreatePresentation(mPresentationGroupID, "Navigating");

            mPresentation.PlayAlgorithm = PresentationPlayAlgorithm.PPA_SPLINE;
            if (CGisDataSettings.AppSpeed <= 2)
                mPresentation.PlaySpeedFactor = PresentationPlaySpeed.PPS_VERYSLOW;
            else if (CGisDataSettings.AppSpeed < 5)
                mPresentation.PlaySpeedFactor = PresentationPlaySpeed.PPS_SLOW;
            else if (CGisDataSettings.AppSpeed < 7)
                mPresentation.PlaySpeedFactor = PresentationPlaySpeed.PPS_NORMAL;
            else if (CGisDataSettings.AppSpeed < 9)
                mPresentation.PlaySpeedFactor = PresentationPlaySpeed.PPS_FAST;
            else
                mPresentation.PlaySpeedFactor = PresentationPlaySpeed.PPS_VERYFAST;
            mPresentation.PlaySpeedFactor = PresentationPlaySpeed.PPS_NORMAL;

            mPresentation.PlayMode = PresentationPlayMode.PPM_AUTOMATIC;
            mPresentation.LoopRoute = true;
            

            double[] x = { 1, -1, 0, 0 };
            double[] y = { 0, 0, 1, -1 };
            Random ran = new Random();
            //double[] z = { cz, cz, cz, cz };
            //double[] d = { cdir, cdir, cdir, cdir }; 
            if (mPointNear[0] == null)
            {
                //* (1 + ran.NextDouble())
                //mPointNav = sgworld.Creator.CreatePosition(x, y, z, AltitudeTypeCode.ATC_TERRAIN_RELATIVE, dir, 0, 0, 350);
                for (int i = 0; i < x.Length; i++)
                    mPointNear[i] = sgworld.Creator.CreatePosition(cx + x[i] * 0.005 ,
                        cy + y[i] * 0.005 , 350 + ran.Next(1, 100), AltitudeTypeCode.ATC_TERRAIN_RELATIVE, -i * 90, -60, 0, 1300);
            }
            else
            {
                for (int i = 0; i < x.Length; i++)
                {
                    mPointNear[i].X = cx + x[i] * 0.005;
                    mPointNear[i].Y = cy + y[i] * 0.005;
                    mPointNear[i].Altitude = 350 + ran.Next(1, 100);

                }
                    
            }
            for (int i = mPresentation.Steps.Count - 1; i >= 0; i--)
                mPresentation.DeleteStep(i);

            for (int i = 0; i<mPointNear.Length; i++)
                mPresentation.CreateLocationStep(PresentationStepContinue.PSC_WAIT, 0, "Starting",mPointNear[i]    );
            mPresentation.CreateLocationStep(PresentationStepContinue.PSC_WAIT, 0, "Ended", mPointNear[0]);
            mPresentation.Play(0);

        }

        private static void Sgworld_OnPresentationFlyToReachedDestination(string PresentationID, IPresentationStep66 Step)
        {
            if (PresentationID.Equals(mPresentation.ID))
            {

                if (Step.Description.Equals("Ended"))
                {
                    mPresentation.Stop();
                    NavigationFinished?.Invoke();

                }
            }
        }
    }
}
