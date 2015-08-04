using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TETCSharpClient;
using TETCSharpClient.Data;

namespace WpfApplication1
{
    class FaceTrackerThread
    {
        private bool running;

        private PXCMSenseManager senseManager;
        private PXCMFaceModule face;
        private PXCMFaceConfiguration faceConfiguration;
        private PXCMFaceConfiguration.ExpressionsConfiguration expressionConfiguration;
        private Int32 numberTrackedFaces;
        public float faceAverageDepth = 0;
        private const int TotalExpressions = 5;
        private int[] expressionScore = new int[TotalExpressions];
        private float initialVal = 0;

        public FaceTrackerThread()
        {
            running = true;

            senseManager = PXCMSenseManager.CreateInstance();
            senseManager.EnableStream(PXCMCapture.StreamType.STREAM_TYPE_COLOR, 640, 480, 30);
            senseManager.EnableFace();
            senseManager.Init();
            face = senseManager.QueryFace();
            faceConfiguration = face.CreateActiveConfiguration();
            faceConfiguration.detection.isEnabled = true;

            expressionConfiguration = faceConfiguration.QueryExpressions();
            expressionConfiguration.Enable();
            expressionConfiguration.EnableAllExpressions();

            faceConfiguration.EnableAllAlerts();
            faceConfiguration.ApplyChanges();
        }

        public bool isFaceDistanceChanged()
        {
            if (faceAverageDepth == initialVal)
            {
                return false;
            } 
            else
            {
                initialVal = faceAverageDepth;
                //Console.WriteLine("From changed(): " + initialVal +" / "+ faceAverageDepth);
                return true;
            }
        }

        public void printFaceDistance()
        {
            Console.WriteLine(faceAverageDepth);
            Console.WriteLine("===============");
        }
        public void Run()
        {
            while (running && senseManager.AcquireFrame(true) >= pxcmStatus.PXCM_STATUS_NO_ERROR)
            {
                //Console.WriteLine(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                //                            CultureInfo.InvariantCulture));
                // Get a face instance
                face = senseManager.QueryFace();

                if (face != null)
                {
                    // Get face tracking processed data
                    PXCMFaceData faceData = face.CreateOutput();
                    faceData.Update();
                    numberTrackedFaces = faceData.QueryNumberOfDetectedFaces();
                    //Console.WriteLine("numberTrackedFaces = " + numberTrackedFaces);

                    // Retrieve the face location data instance
                    PXCMFaceData.Face faceDataFace = faceData.QueryFaceByIndex(0);

                    if (faceDataFace != null)
                    {
                        // Retrieve face location data
                        PXCMFaceData.DetectionData faceDetectionData = faceDataFace.QueryDetection();
                        if (faceDetectionData != null) 
                        {
                            //faceAverageDepth = 0;
                            bool b = faceDetectionData.QueryFaceAverageDepth(out faceAverageDepth);
                            //faceAverageDepth = (Int32)faceAverageDepthFloat;
                            //Console.WriteLine("b = " + b);
                            //Console.Out.WriteLine("Depth:" + faceAverageDepth);//face depth in inches + (j++) + "."
                        }

                    }
                    faceData.Dispose();
                }

                senseManager.ReleaseFrame();
            }
        }

        public void Clean()
        {
            faceConfiguration.Dispose();
            senseManager.Dispose();
        }

        public void Stop()
        {
            running = false;
        }

    }
}
