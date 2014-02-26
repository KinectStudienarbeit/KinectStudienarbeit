using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Microsoft.Kinect;

namespace KinectStudienarbeitWpf
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor MainKinect;

        public MainWindow()
        {
            InitializeComponent();

            //create a listener for status changes of thr kinect
            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;

            //try to select the first kinect connected atm (it is possible to use more than one at the same time)
            MainKinect = KinectSensor.KinectSensors.FirstOrDefault();
            if (MainKinect == null)     //if no sensor is connected show the message
            {
                MessageBox.Show("Kein Kinect-Sensor erkannt!");
            }
            KinectInit(); //initilize the sensor

        }

        //deals with status changes of the kinect
        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Connected:
                    MessageBox.Show("New Kinect konnected");
                    MainKinect = e.Sensor;
                    KinectInit();
                    break;
                default:
                    MessageBox.Show("Kinect status: " + e.Status);
                    break;
            }
        }

        //deals with initilization of the kinect
        void KinectInit()
        {
            if (MainKinect == null)
                return;     //no kinect is connected so just stop
            //adds a listener for the AllFramesReady event (color, depth and skeletal frame ready)
            MainKinect.AllFramesReady += kinect_AllFramesReady;
            MainKinect.ColorStream.Enable();    //enables the color stream (normal camera)
            MainKinect.Start();     //starts the sensor
        }

        //do stuff with the image when all frames are ready
        void kinect_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using (ColorImageFrame frame = e.OpenColorImageFrame())
            {
                if (frame == null)  //do nothing if a frame is dropped
                    return;

                //the image will be stored in a byte array
                byte[] pixels = new byte[frame.PixelDataLength];
                //copy the kinect image into the byte array
                frame.CopyPixelDataTo(pixels);

                int stride = frame.Width * 4; //because of R G B + blank

                ImageColorStream.Source = BitmapSource.Create(frame.Width, frame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);
            }

        }

    }
}
