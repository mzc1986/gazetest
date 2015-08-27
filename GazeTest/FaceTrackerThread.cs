using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private PXCMFaceConfiguration.GazeConfiguration gazec;

        public FaceTrackerThread()
        {
            running = true;

            senseManager = PXCMSenseManager.CreateInstance();
            senseManager.EnableStream(PXCMCapture.StreamType.STREAM_TYPE_COLOR, 640, 480, 60);
            senseManager.EnableFace();
            senseManager.Init();
            face = senseManager.QueryFace();
            faceConfiguration = face.CreateActiveConfiguration();
            faceConfiguration.detection.isEnabled = true;

            expressionConfiguration = faceConfiguration.QueryExpressions();
            expressionConfiguration.Enable();
            expressionConfiguration.EnableAllExpressions();


            //Gaze detection
            gazec = faceConfiguration.QueryGaze();
            gazec.isEnabled = true;
            faceConfiguration.ApplyChanges();

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
                    Console.WriteLine("in face");
                    // Get face tracking processed data
                    PXCMFaceData faceData = face.CreateOutput();
                    faceData.Update();
                    numberTrackedFaces = faceData.QueryNumberOfDetectedFaces();
                    //Console.WriteLine("numberTrackedFaces = " + numberTrackedFaces);

                    for (Int32 i = 0; i < numberTrackedFaces; i++)
                    {
                        // Retrieve the face location data instance
                        PXCMFaceData.Face faceDataFace = faceData.QueryFaceByIndex(i);

                        if (faceDataFace != null)
                        {
                            // Retrieve face location data
                            PXCMFaceData.DetectionData faceDetectionData = faceDataFace.QueryDetection();

                            PXCMFaceData.GazeCalibData gazeCalibData = faceDataFace.QueryGazeCalibration();
                            Byte[] buffer;
                            PXCMFaceData.GazeCalibData.CalibrationStatus calibStatus = gazeCalibData.QueryCalibData(out buffer);

                            PXCMFaceData.GazeCalibData.CalibrationState state;
                            PXCMPointI32 calibp;

                            Console.WriteLine("calib status:" + calibStatus);

                            if (faceDetectionData != null)
                            {
                                PXCMFaceData.GazeData gazed = faceDataFace.QueryGaze();
                                if (gazed != null)
                                {
                                    PXCMFaceData.GazePoint gazep = gazed.QueryGazePoint();

                                    Console.WriteLine("RS Gaze points: " + gazep.screenPoint.x + "," + gazep.screenPoint.y);
                                }
                                //faceAverageDepth = 0;
                                //bool b = faceDetectionData.QueryFaceAverageDepth(out faceAverageDepth);
                                //faceAverageDepth = (Int32)faceAverageDepthFloat;
                                //Console.WriteLine("b = " + b);
                                //Console.Out.WriteLine("Depth:" + faceAverageDepth);//face depth in inches + (j++) + "."
                            }

                            if (gazeCalibData != null)
                            {
                                state = gazeCalibData.QueryCalibrationState();
                                switch (state)
                                {
                                    case PXCMFaceData.GazeCalibData.CalibrationState.CALIBRATION_IDLE:
                                        // Visual clue to the user that the calibration process starts, or LoadCalibData.
                                        Console.WriteLine("in idle");
                                        break;
                                    case PXCMFaceData.GazeCalibData.CalibrationState.CALIBRATION_NEW_POINT:
                                        // Visual cue to the user that a new calibration point is available.

                                        calibp = gazeCalibData.QueryCalibPoint();
                                        Console.WriteLine("in new point");
                                        Console.WriteLine(calibp.x + "," + calibp.y);
                                        break;
                                    case PXCMFaceData.GazeCalibData.CalibrationState.CALIBRATION_SAME_POINT:
                                        // Continue visual cue to the user at the same location.
                                        //Console.WriteLine("presenting same point");
                                        break;
                                    case PXCMFaceData.GazeCalibData.CalibrationState.CALIBRATION_DONE:
                                        // Visual cue to the user that the calibration process is complete or calibration data is loaded.
                                        // Optionally save the calibration data.
                                        //Console.WriteLine("in calibration done");
                                        Byte[] buffer2;
                                        calibStatus = gazeCalibData.QueryCalibData(out buffer2);

                                        Console.WriteLine(calibStatus);
                                        //m_oWorker.ReportProgress(100);
                                        break;
                                }
                            }
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
