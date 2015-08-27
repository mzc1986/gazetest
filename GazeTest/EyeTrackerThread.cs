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
    class EyeTrackerThread : IGazeListener
    {
        public double Lx;
        public double Rx;
        public double distance;

        public void Run()
        {
            GazeManager.Instance.Activate(GazeManager.ApiVersion.VERSION_1_0, GazeManager.ClientMode.Push);

            GazeManager.Instance.AddGazeListener(this);

            //Thread.Sleep(5000); // simulate app lifespan (e.g. OnClose/Exit event)

            //GazeManager.Instance.Deactivate();
        }

        public void Stop()
        {
            GazeManager.Instance.Deactivate();
        }

        public void OnGazeUpdate(GazeData gazeData)
        {
            //Console.WriteLine(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
            //                                CultureInfo.InvariantCulture));

            double gX = gazeData.SmoothedCoordinates.X;
            double gY = gazeData.SmoothedCoordinates.Y;

            Console.WriteLine("ET Gaze points: " + gX + "," + gY);
            //getting the distance in pixel
            distance = GazeUtils.getDistancePoint2D(gazeData.LeftEye.SmoothedCoordinates, gazeData.RightEye.SmoothedCoordinates);

            Lx = gazeData.LeftEye.SmoothedCoordinates.X;// *0.26328125; // 0.4013671875; //correction for mm
            Rx = gazeData.RightEye.SmoothedCoordinates.X;// *0.26328125; // 0.4013671875; // correction for mm
        }

    }
}
