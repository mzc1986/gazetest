using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TETCSharpClient;
using TETCSharpClient.Data;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IGazeListener
    {
        public MainWindow()
        {
            InitializeComponent();

            GazePoint();
        }

        public void GazePoint()
     {
         // Connect client
         GazeManager.Instance.Activate(GazeManager.ApiVersion.VERSION_1_0, GazeManager.ClientMode.Push);
        
         // Register this class for events
         GazeManager.Instance.AddGazeListener(this);

         Thread.Sleep(5000); // simulate app lifespan (e.g. OnClose/Exit event)

         // Disconnect client
         GazeManager.Instance.Deactivate();

     }

        double d0;
        double v0;
        double angle;
        double Lx;
        double Rx;
        int i = 0;
        double k;

     public void OnGazeUpdate(GazeData gazeData)
     {
         double gX = gazeData.SmoothedCoordinates.X;
         double gY = gazeData.SmoothedCoordinates.Y;


         //getting the distance in pixel
         double distance = GazeUtils.getDistancePoint2D(gazeData.LeftEye.SmoothedCoordinates, gazeData.RightEye.SmoothedCoordinates);

         Lx = gazeData.LeftEye.SmoothedCoordinates.X *0.26328125; // 0.4013671875; //correction for mm
         Rx = gazeData.RightEye.SmoothedCoordinates.X *0.26328125; // 0.4013671875; // correction for mm

         //just to set initial angle and distance
         if (i < 10)
         {
             d0 = Math.Abs(Lx - Rx);
             v0 = (180.0 / Math.PI) * Math.Atan((d0) / 600);
             k = v0 / d0;
         }
         else
            angle = (Math.Abs(Lx - Rx) * k);

         Console.WriteLine("mm dt = " + string.Format("{0:0.00}", d0) + ", pix dt = " + string.Format("{0:0.00}", distance) + " , angle = " + string.Format("{0:0.00}", angle));

         //Console.WriteLine(gazeData.LeftEye.RawCoordinates.X);
         //Console.WriteLine(gazeData.LeftEye.SmoothedCoordinates.Y);

         //double avg = (gazeData.LeftEye.SmoothedCoordinates.X + gazeData.RightEye.SmoothedCoordinates.X)/2;
         //Console.WriteLine("x  " + gX + ",   avg" + avg);

         //Console.WriteLine(string.Format("{0:0.00}",angle));
         //Console.WriteLine(string.Format("{0:0.00}", d0));
         //Console.WriteLine(string.Format("{0:0.00}", distance));
         //Console.WriteLine(gazeData.LeftEye.RawCoordinates.X + " " + gazeData.RightEye.RawCoordinates.X + " distance =" + distance);

         i++;

         // Move point, do hit-testing, log coordinates etc.
     }
    }
}
