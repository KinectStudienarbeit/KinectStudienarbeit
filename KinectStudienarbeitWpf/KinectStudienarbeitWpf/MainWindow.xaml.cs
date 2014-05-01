using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace KinectStudienarbeitWpf
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BlenderResourceDictionary mainDictionary = null;
        BlenderModel mainModel = null;
        KinectSensor mainKinect = null;
        Skeleton[] mainSkeletonData;
        int oldx = -1;
        int oldy = -1;
        int oldz = -1;

        const int RANGE_FINGERS_X = 100;
        const int RANGE_FINGERS_Y = 100;
        

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            initializeKinect();

        }

        /// <summary>
        /// Initilizes the Kinect
        /// </summary>
        private void initializeKinect()
        {
            mainKinect = KinectSensor.KinectSensors.FirstOrDefault();
            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
            Label_KinectStatus.Content = mainKinect.Status;

            //var parameters = new TransformSmoothParameters
            //{
            //    Smoothing = 0.3f,
            //    Correction = 0.0f,
            //    Prediction = 0.0f,
            //    JitterRadius = 1.0f,
            //    MaxDeviationRadius = 0.5f
            //};

            mainKinect.SkeletonStream.Enable();
            mainSkeletonData = new Skeleton[mainKinect.SkeletonStream.FrameSkeletonArrayLength];

            mainKinect.DepthStream.Enable();
            mainKinect.AllFramesReady += mainKinect_AllFramesReady;
            mainKinect.Start();
        }

        Skeleton GetFirstSkeleton(AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                {
                    return null;
                }


                skeletonFrameData.CopySkeletonDataTo(mainSkeletonData);

                //get the first tracked skeleton
                Skeleton first = (from s in mainSkeletonData
                                  where s.TrackingState == SkeletonTrackingState.Tracked
                                  select s).FirstOrDefault();

                return first;

            }
        }

        void mainKinect_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            Skeleton first = GetFirstSkeleton(e);
            if (first == null) return;

            CoordinateMapper mapper = new CoordinateMapper(mainKinect);

            DepthImagePoint pointRight = mapper.MapSkeletonPointToDepthPoint(first.Joints[JointType.HandRight].Position, DepthImageFormat.Resolution640x480Fps30);
            Label_RightHand.Content = "X: " + pointRight.X + "   Y: " + pointRight.Y + "   Z:" + pointRight.Depth;
            if (oldx == -1)
            {
                oldx = pointRight.X;
            }
            if (oldy == -1)
            {
                oldy = pointRight.Y;
            }
            if (oldz == -1)
            {
                oldz = pointRight.Depth;
            }
            if (Radio_Kinect.IsChecked.Value)
            {
                mainModel.translate((pointRight.X - oldx) / 5, (oldy - pointRight.Y) / 5, (pointRight.Depth - oldz) / 5);// Math.Abs(oldy - pointRight.Y), Math.Abs(oldz - pointRight.Depth));
            }
            oldx = pointRight.X;
            oldy = pointRight.Y;
            oldz = pointRight.Depth;
            //getFingers(pointRight, e);
            DepthImagePoint pointLeft = mapper.MapSkeletonPointToDepthPoint(first.Joints[JointType.HandLeft].Position, DepthImageFormat.Resolution640x480Fps30);
            Label_LeftHand.Content = "X: " + pointLeft.X + "   Y: " + pointLeft.Y + "   Z:" + pointRight.Depth;
            
        }

        DepthImagePoint[] getFingers(DepthImagePoint handPosition, AllFramesReadyEventArgs e)
        {
            DepthImageFrame depthImageFrame = e.OpenDepthImageFrame();
            DepthImagePixel[] depthPixelData = new DepthImagePixel[depthImageFrame.PixelDataLength];
            depthImageFrame.CopyDepthImagePixelDataTo(depthPixelData);

            int startx = handPosition.X - RANGE_FINGERS_X;
            int starty = handPosition.Y - RANGE_FINGERS_Y;

            if (startx < 0 || startx >= depthImageFrame.Width - RANGE_FINGERS_X * 2 || starty < 0 || starty >= depthImageFrame.Height - RANGE_FINGERS_Y * 2)
            {
                depthImageFrame.Dispose();
                return null;
            }

            int[,] depths = new int[200, 200];
            for (int y = 0; y < 200; y++)
            {
                for (int x = 0; x < 200; x++)
                {
                    int value = 0;


                    value = depthPixelData[(starty + y) * depthImageFrame.Width + startx + x].Depth;


                    if (value <= handPosition.Depth) value = 0;
                    depths[x, y] = value;
                }
            }
            int count = 0;

            for (int i = 0; i < 200; i++)
            {
                for (int j = 0; j < 200; j++)
                {
                    if (depths[i, j] > 0) count++;
                }
            }
            Label_RightHandFingers.Content = count.ToString();
            depthImageFrame.Dispose();
            return null;
        }

        DepthImagePixel getDepthImagePixel(DepthImagePixel[] depthImagePixels, int x, int y, int width)
        {
            return depthImagePixels[y * width + x];
        }

        /// <summary>
        /// Deals with status changes of the Kinect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status == KinectStatus.Connected)
            {
                //mainKinect = e.Sensor;
                initializeKinect();
            }
            Label_KinectStatus.Content = e.Status;
        }



        /// <summary>
        /// Deals with the keyboard input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            if (mainModel == null) return;

            Console.WriteLine(mainModel.getBounds().IntersectsWith(Quadrat.Bounds));

            if (Radio_Rotate.IsChecked.Value)
            {
                switch (e.Key)
                {
                    case Key.W:
                        mainModel.rotate(-20, 0, 0);
                        break;

                    case Key.S:
                        mainModel.rotate(20, 0, 0);
                        break;

                    case Key.D:
                        mainModel.rotate(0, 20, 0);
                        break;

                    case Key.A:
                        mainModel.rotate(0, -20, 0);
                        break;

                    case Key.E:
                        mainModel.rotate(0, 0, -20);
                        break;

                    case Key.Q:
                        mainModel.rotate(0, 0, 20);
                        break;
                }
            }
            else if (Radio_Translate.IsChecked.Value)
            {
                switch (e.Key)
                {
                    case Key.W:
                        mainModel.translate(0, 0.5, 0);
                        break;

                    case Key.S:
                        mainModel.translate(0, -0.5, 0);
                        break;

                    case Key.D:
                        mainModel.translate(0.5, 0, 0);
                        break;

                    case Key.A:
                        mainModel.translate(-0.5, 0, 0);
                        break;

                    case Key.E:
                        mainModel.translate(0, 0, -0.5);
                        break;

                    case Key.Q:
                        mainModel.translate(0, 0, 0.5);
                        break;
                }
            }
        }

        private void Button_Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XAML files (*.xaml)|*.xaml";
            openFileDialog.ShowDialog();
            Textbox_File.Text = openFileDialog.FileName;
        }

        private void Button_Load_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(Textbox_File.Text))
            {
                MessageBox.Show("Could not find file " + Textbox_File.Text + "!");
            }
            else
            {
                mainDictionary = new BlenderResourceDictionary(Textbox_File.Text);
                foreach (String s in mainDictionary.keyList)
                {
                    ComboBox_Models.Items.Add(s);
                }
                ComboBox_Models.SelectedItem = ComboBox_Models.Items[0];
                MessageBox.Show("File loaded successfully!");
            }

        }

        private void ComboBox_Models_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainModel != null)
            {
                mainModel.removeFromViewport(this.mainViewPort);
            }

            mainModel = new BlenderModel(mainDictionary, ComboBox_Models.SelectedItem as String);
            mainModel.addToViewport(this.mainViewPort);
        }

    }
}
