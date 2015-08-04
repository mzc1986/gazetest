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
using System.Drawing;
using System.Diagnostics;
using System.Collections.Specialized;


namespace WpfApplication1
{
    public partial class MainWindow : Window
    {
        private EyeTrackerThread eyeTrackerThread;
        private FaceTrackerThread faceTrackerThread;
        private Thread thread1;
        private Thread thread2;

        //variable for angle calculation
        private double d0;
        private double v0;
        private double angle = -1;
        private double k;
        private int i = 0;

        public MainWindow()
        {
            InitializeComponent();

            eyeTrackerThread = new EyeTrackerThread();
            thread1 = new Thread(eyeTrackerThread.Run);
            //faceTrackerThread = new FaceTrackerThread();
            //thread2 = new Thread(faceTrackerThread.Run);

            thread1.Start();
            //thread2.Start();
            //Thread.Sleep(3000);

            // Create new stopwatch
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            for (double i = 0; i < 500000000; i++ )
            {
                //calculateVergence();

                calculateDistane();

                //Console.WriteLine(eyeTrackerThread.Lx);
                //Console.WriteLine(faceTrackerThread.faceAverageDepth);
            }

            // Stop timing
            stopwatch.Stop();

            // Write result
            Console.WriteLine("Time elapsed: {0}",
                stopwatch.Elapsed);

            //just printing the distance and associated angles
            foreach (string myKey in mCollection.AllKeys)
            {
                Console.Write(myKey + " ");
                foreach (string myValue in mCollection.GetValues(myKey))
                {
                    Console.Write(myValue + " ");
                }
                Console.WriteLine();
            }

            //foreach (string myKey in mCollection.AllKeys)
            //  {
            //    Console.WriteLine(myKey + ": " + mCollection[myKey]);
            //  }

        }

        double initAngle = 0;
        NameValueCollection mCollection = new NameValueCollection();

        double mDepth;

        private void calculateDistane()
        {
            d0 = Math.Abs(eyeTrackerThread.Lx - eyeTrackerThread.Rx);

            Thread.Sleep(100);
            Console.WriteLine(eyeTrackerThread.Lx);
            Console.WriteLine(eyeTrackerThread.Rx);
        }
        private void calculateVergence()
        {
                //calculate initial distance
                d0 = Math.Abs(eyeTrackerThread.Lx - eyeTrackerThread.Rx);

                //check if the face plane has changed
                if (faceTrackerThread.isFaceDistanceChanged())
                {
                    mDepth = faceTrackerThread.faceAverageDepth;
                    v0 = (180.0 / Math.PI) * Math.Atan((d0) / mDepth);

                    Console.WriteLine(mDepth);

                    if(v0 != 0 && d0 != 0)
                        k = v0 / d0;
                }

                //calculate angle
                angle = (Math.Abs(eyeTrackerThread.Lx - eyeTrackerThread.Rx) * k);

                
                
            //if angle is changed print that angle for this face plane
            if(angle!= initAngle)
            {
                initAngle = angle;
                mCollection.Add(mDepth.ToString(), angle.ToString());
                Console.WriteLine(angle);
                Console.WriteLine("-----");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //new data
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Console.WriteLine("In close");
            eyeTrackerThread.Stop();
            faceTrackerThread.Stop();
        }

    }
}
