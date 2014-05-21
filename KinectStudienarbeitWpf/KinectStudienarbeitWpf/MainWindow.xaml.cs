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
using System.Timers;
using System.Diagnostics;
using System.Windows.Threading;

namespace KinectStudienarbeitWpf
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SolidColorBrush brushRed = new SolidColorBrush(Colors.Red);
        SolidColorBrush brushGreen = new SolidColorBrush(Colors.Green);
        SolidColorBrush brushBlue = new SolidColorBrush(Colors.Blue);
        Room mainRoom;

        BlenderResourceDictionary mainDictionary = null;
        BlenderModel mainModel = null;
        KinectSensor mainKinect = null;
        //Skeleton[] mainSkeletonData;
        //int oldx = -1;
        //int oldy = -1;
        //int oldz = -1;
        //double oldAngle = -1;
        //bool goKinect = true;
        DispatcherTimer time = null;
        int secs = 0;
        //const int HANDS_DISTANCE = 150;

        public void displayColorFrame(BitmapSource bitmapSource)
        {
            ImageColorStream.Source = bitmapSource;
        }

        public void setHandMarkers(int rx, int ry, int lx, int ly, bool turn)
        {
            if (turn)
            {
                Ellipse_LeftHand.Fill = brushRed;
                Ellipse_RightHand.Fill = brushRed;
            }
            else
            {
                Ellipse_LeftHand.Fill = brushBlue;
                Ellipse_RightHand.Fill = brushGreen;
            }
            Canvas.SetLeft(Ellipse_RightHand, rx * ImageColorStream.Width / 640);
            Canvas.SetTop(Ellipse_RightHand, ry * ImageColorStream.Height / 480);
            Canvas.SetLeft(Ellipse_LeftHand, lx * ImageColorStream.Width / 640);
            Canvas.SetTop(Ellipse_LeftHand, ly * ImageColorStream.Height / 480);
        }

        public void displayKinectStatus(String status)
        {
            Label_KinectStatus.Content = status;
        }

        public MainWindow()
        {
            InitializeComponent();
            Label_Start.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            mainRoom = new Room(this.mainViewPort);
            KinectHandler k = new KinectHandler(this, mainRoom);

            this.Left = 5;
            this.Top = 5;

            //initializeKinect();
            //Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            //mainDictionary = new BlenderResourceDictionary(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Spielraumblend4.xaml");

            //BlenderModel raum = new BlenderModel(mainDictionary, "Raum");
            //raum.addToViewport(this.mainViewPort);
            //raum.rotate(-90, 0, 0, false);
            //raum.rotate(0, -90, 0, false);
            //raum.translate(0, -0.5, -8, false);
            //raum.scale(0.05, 0.52, -0.5);
            //raum.translate(0, 0, 7, false);

            //BlenderModel wand = new BlenderModel(mainDictionary, "Wand");
            //wand.addToViewport(this.mainViewPort);
            //wand.rotate(0, -90, 0, false);
            //wand.rotate(0, 0, -90, false);
            //wand.scale(-0.8, -0.8, -0.8);
            //wand.translate(1.15, -0.6, 2.4, false);

            //mainModel = new BlenderModel(mainDictionary, "Quader");
            //mainModel.addToViewport(this.mainViewPort);
            //mainModel.scale(-0.8, -0.8, -0.8);
            //mainModel.translate(0, 0, 2.86, false);



        }

        ///// <summary>
        ///// Initilizes the Kinect
        ///// </summary>
        //private void initializeKinect()
        //{
        //    try
        //    {
        //        mainKinect = KinectSensor.KinectSensors.FirstOrDefault();
        //        KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
        //        Label_KinectStatus.Content = mainKinect.Status;


        //        //mainKinect.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;

        //        mainKinect.ColorStream.Enable();

        //        mainKinect.SkeletonStream.Enable();
                

        //        mainKinect.DepthStream.Enable();
        //        mainKinect.AllFramesReady += mainKinect_AllFramesReady;
        //        mainKinect.ColorFrameReady += mainKinect_ColorFrameReady;
        //        mainKinect.Start();
        //    }
        //    catch (Exception)
        //    {


        //    }
        //}

        ///// <summary>
        ///// Gets called if a RGB frame is ready
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void mainKinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        //{
        //    using (ColorImageFrame frame = e.OpenColorImageFrame())
        //    {
        //        if (frame == null)  //do nothing if a frame is dropped
        //            return;

        //        //the image will be stored in a byte array
        //        byte[] pixels = new byte[frame.PixelDataLength];
        //        //copy the kinect image into the byte array
        //        frame.CopyPixelDataTo(pixels);

        //        int stride = frame.Width * 4; //because of R G B + blank

        //        ImageColorStream.Source = BitmapSource.Create(frame.Width, frame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);
        //    }

        //    if (!goKinect)
        //    {
        //        if (!time.IsEnabled)
        //        {
        //            secs = 0;
        //            time.IsEnabled = true;
        //            time.Start();
        //        }
        //        else
        //        {
        //            if (secs >= 3)
        //            {
        //                time.Stop();
        //                Process.Start(Application.ResourceAssembly.Location);
        //                Application.Current.Shutdown();
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Returns the first skeleton (the kinect recognizes up to two)
        ///// </summary>
        ///// <param name="e"></param>
        ///// <returns></returns>
        //Skeleton getFirstSkeleton(AllFramesReadyEventArgs e)
        //{
        //    using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
        //    {
        //        if (skeletonFrameData == null)
        //        {
        //            return null;
        //        }

        //        Skeleton[] mainSkeletonData = new Skeleton[mainKinect.SkeletonStream.FrameSkeletonArrayLength];

        //        skeletonFrameData.CopySkeletonDataTo(mainSkeletonData);

        //        //get the first tracked skeleton
        //        Skeleton first = (from s in mainSkeletonData
        //                          where s.TrackingState == SkeletonTrackingState.Tracked
        //                          select s).FirstOrDefault();

        //        return first;

        //    }
        //}

        /// <summary>
        /// The tick event for the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void time_Tick(object sender, EventArgs e)
        {
            secs++;
        }

        ///// <summary>
        ///// Gets called if the RGB, depth and skeleton frame is ready
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void mainKinect_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        //{

        //    if (!Radio_Kinect.IsChecked.Value) return;      //return if Kinect is not selected as input
        //    if (!goKinect) return;                          //return if game is over (temporary)

        //    Skeleton mainSkeleton = getFirstSkeleton(e);    //get the first skeleton
        //    if (mainSkeleton == null) return;               //return if the Kinect does not recognize any skeletons
        //    if (time == null)                               //start the timer if not startet
        //    {
        //        time = new DispatcherTimer();
        //        time.Interval = new TimeSpan(0, 0, 1);      //intervall of a second
        //        time.Tick += time_Tick;
        //        time.Start();
        //    }


        //    if (mainModel.getCoords().Z < Room.WALL_Z - 4)  //the model is behind the wall -> player has won
        //    {
        //        time.Stop();
        //        time.IsEnabled = false;
        //        Label_Start.Content = "You won! - Time: " + secs.ToString() + "s";
        //        Label_Start.Visibility = System.Windows.Visibility.Visible;

        //        goKinect = false;
        //        return;
        //    }

        //    CoordinateMapper mapper = new CoordinateMapper(mainKinect);     //mapper between skeleton and depth image

        //    DepthImagePoint pointRight = mapper.MapSkeletonPointToDepthPoint(mainSkeleton.Joints[JointType.HandRight].Position, DepthImageFormat.Resolution640x480Fps30); //get the right hand

        //    Canvas.SetLeft(Ellipse_RightHand, pointRight.X * ImageColorStream.Width / 640);     //set the position of the right hand marker
        //    Canvas.SetTop(Ellipse_RightHand, pointRight.Y * ImageColorStream.Height / 480);

        //    if (pointRight.Depth > Room.kinectZmax)     //adapt the Z-Range if the player goes out of it
        //    {
        //        Room.kinectZmax = pointRight.Depth;
        //        Room.kinectZmin = pointRight.Depth - 500;
        //    }
        //    if (pointRight.Depth < Room.kinectZmin)
        //    {
        //        Room.kinectZmax = pointRight.Depth + 500;
        //        Room.kinectZmin = pointRight.Depth;
        //    }

        //    DepthImagePoint pointLeft = mapper.MapSkeletonPointToDepthPoint(mainSkeleton.Joints[JointType.HandLeft].Position, DepthImageFormat.Resolution640x480Fps30); //get the left hand
            
        //    Canvas.SetLeft(Ellipse_LeftHand, pointLeft.X * ImageColorStream.Width / 640);   //set the position of the left hand marker
        //    Canvas.SetTop(Ellipse_LeftHand, pointLeft.Y * ImageColorStream.Height / 480);

        //    int dx = pointLeft.X - pointRight.X;        //needed for distance calculation
        //    int dy = pointLeft.Y - pointRight.Y;
        //    if (dx * dx + dy * dy < HANDS_DISTANCE * HANDS_DISTANCE)    //if the distance is small enough
        //    {
        //        Ellipse_LeftHand.Fill = new SolidColorBrush(Colors.Red);    //paint the hand markers red...
        //        Ellipse_RightHand.Fill = new SolidColorBrush(Colors.Red);

        //        if (oldAngle == -1)
        //        {
        //            oldAngle = getAngle(pointRight.X, pointRight.Y, pointLeft.X, pointLeft.Y);
        //        }
        //        mainModel.rotate(0, 0, getAngle(pointRight.X, pointRight.Y, pointLeft.X, pointLeft.Y) - oldAngle);  //and use rotation
        //        oldAngle = getAngle(pointRight.X, pointRight.Y, pointLeft.X, pointLeft.Y);

        //    }
        //    else
        //    {

        //        Ellipse_LeftHand.Fill = new SolidColorBrush(Colors.Blue);
        //        Ellipse_RightHand.Fill = new SolidColorBrush(Colors.Green);

        //    }


        //    if (Radio_Absolute.IsChecked.Value == true)
        //    {
        //        mainModel.translateAbsolute(pointRight.X, pointRight.Y, pointRight.Depth);  //absolute movement
        //    }
        //    else
        //    {
        //        if (oldx == -1)             //relative movement
        //        {
        //            oldx = pointRight.X;
        //        }
        //        if (oldy == -1)
        //        {
        //            oldy = pointRight.Y;
        //        }
        //        if (oldz == -1)
        //        {
        //            oldz = pointRight.Depth;
        //        }
        //        mainModel.translate((pointRight.X - oldx) * Room.FACTOR_X, 0, 0);
        //        mainModel.translate(0, (oldy - pointRight.Y) * Room.FACTOR_Y, 0);
        //        mainModel.translate(0, 0, (pointRight.Depth - oldz) * Room.FACTOR_Z);
        //        oldx = pointRight.X;
        //        oldy = pointRight.Y;
        //        oldz = pointRight.Depth;
        //    }
        //}

        ///// <summary>
        ///// Returns the angle of two points
        ///// </summary>
        ///// <param name="x1">X coordinate of the first point</param>
        ///// <param name="y1">Y coordinate of the first point</param>
        ///// <param name="x2">X coordinate of the second point</param>
        ///// <param name="y2">Y coordinate of the second point</param>
        ///// <returns>The angle between the two points</returns>
        //double getAngle(int x1, int y1, int x2, int y2)
        //{
        //    double returnVal = Math.Atan2(x1 - x2, y1 - y2) * (180D / Math.PI);
        //    if (returnVal < 0) returnVal += 360;

        //    return returnVal;
        //}

        ///// <summary>
        ///// Deals with status changes of the Kinect
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        //{
        //    if (e.Status == KinectStatus.Connected)
        //    {
        //        //mainKinect = e.Sensor;
        //        initializeKinect();
        //    }
        //    Label_KinectStatus.Content = e.Status;
        //}

        /// <summary>
        /// Deals with the keyboard input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (mainModel == null)
            {
                mainModel = mainRoom.currentElement.model;
                
            }
            mainRoom.currentElement.checkCoords();
            switch (e.Key)
            {
                case Key.Y:
                    mainModel.scale(0, 0, 0.5);
                    break;
                case Key.X:
                    mainModel.scale(0, 0, -0.5);
                    break;
            }

            if (mainModel == null) return;

            if (Radio_Rotate.IsChecked.Value)
            {
                switch (e.Key)
                {
                    case Key.W:
                        mainModel.rotate(-5, 0, 0);
                        break;

                    case Key.S:
                        mainModel.rotate(5, 0, 0);
                        break;

                    case Key.D:
                        mainModel.rotate(0, 5, 0);
                        break;

                    case Key.A:
                        mainModel.rotate(0, -5, 0);
                        break;

                    case Key.E:
                        mainModel.rotate(0, 0, -5);
                        break;

                    case Key.Q:
                        mainModel.rotate(0, 0, 5);
                        break;
                }
            }
            else if (Radio_Translate.IsChecked.Value)
            {
                switch (e.Key)
                {
                    case Key.W:
                        mainModel.translate(0, 0.01, 0, true);
                        break;

                    case Key.S:
                        mainModel.translate(0, -0.01, 0, true);
                        break;

                    case Key.D:
                        mainModel.translate(0.01, 0, 0, true);
                        break;

                    case Key.A:
                        mainModel.translate(-0.01, 0, 0, true);
                        break;

                    case Key.E:
                        mainModel.translate(0, 0, -0.01, true);
                        break;

                    case Key.Q:
                        mainModel.translate(0, 0, 0.01, true);
                        break;
                }
            }
        }

        /// <summary>
        /// Logic of the test GUI...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XAML files (*.xaml)|*.xaml";
            openFileDialog.ShowDialog();
            Textbox_File.Text = openFileDialog.FileName;
        }

        /// <summary>
        /// Logic of the test GUI...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Load_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(Textbox_File.Text))
            {
                MessageBox.Show("Could not find file " + Textbox_File.Text + "!");
            }
            else
            {
                mainDictionary = new BlenderResourceDictionary(Textbox_File.Text);
                ComboBox_Models.Items.Clear();
                foreach (String s in mainDictionary.keyList)
                {
                    ComboBox_Models.Items.Add(s);
                }
                ComboBox_Models.SelectedItem = ComboBox_Models.Items[0];
                MessageBox.Show("File loaded successfully!");
            }

        }

        /// <summary>
        /// Logic of the test GUI...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_Models_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainModel != null)
            {
                mainModel.removeFromViewport(this.mainViewPort);
            }

            mainModel = new BlenderModel(mainDictionary, ComboBox_Models.SelectedItem as String);
            mainModel.addToViewport(this.mainViewPort);
        }

        /// <summary>
        /// Logic of the test GUI...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Radio_Kinect_Checked(object sender, RoutedEventArgs e)
        {
            //probably to be removed
        }

    }
}
