using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KinectStudienarbeitWpf
{
    class KinectHandler
    {
        KinectSensor mainKinectSensor;
        double oldx = -1;
        double oldy = -1;
        double oldz = -1;
        double oldAngle = -1;
        double zMax = -1;
        double zMin = -1;
        public bool absolute = true;
        MainWindow mainWindow;
        Room room;
        bool corrected = false;
        public bool playing = false;
        public bool tutorial = false;
        public bool rotation = false;

        private const double TILT_FACTOR = 9.5;
        const int HANDS_DISTANCE = 150;

        public KinectHandler(MainWindow mainWindow, Room room)
        {
            this.mainWindow = mainWindow;
            this.room = room;
            initializeKinect();
        }

        /// <summary>
        /// Initilizes the Kinect
        /// </summary>
        private void initializeKinect()
        {
            try
            {
                mainKinectSensor = KinectSensor.KinectSensors.FirstOrDefault();
                KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
                mainWindow.displayKinectStatus(mainKinectSensor.Status.ToString());


                //mainKinect.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;

                mainKinectSensor.ColorStream.Enable();

                mainKinectSensor.SkeletonStream.Enable();


                mainKinectSensor.DepthStream.Enable();
                mainKinectSensor.AllFramesReady += mainKinect_AllFramesReady;
                mainKinectSensor.ColorFrameReady += mainKinect_ColorFrameReady;
                mainKinectSensor.Start();
            }
            catch (Exception)
            {


            }
        }

        /// <summary>
        /// Gets called if a RGB frame is ready
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mainKinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
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

                mainWindow.displayColorFrame(BitmapSource.Create(frame.Width, frame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride));
            }
        }

        /// <summary>
        /// Returns the first skeleton (the kinect recognizes up to two)
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        Skeleton getFirstSkeleton(AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                {
                    return null;
                }

                Skeleton[] mainSkeletonData = new Skeleton[mainKinectSensor.SkeletonStream.FrameSkeletonArrayLength];

                skeletonFrameData.CopySkeletonDataTo(mainSkeletonData);

                //get the first tracked skeleton
                Skeleton first = (from s in mainSkeletonData
                                  where s.TrackingState == SkeletonTrackingState.Tracked
                                  select s).FirstOrDefault();

                return first;

            }
        }

        /// <summary>
        /// Gets called if the RGB, depth and skeleton frame is ready
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mainKinect_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (!tutorial) return;


            Skeleton mainSkeleton = getFirstSkeleton(e);    //get the first skeleton
            if (mainSkeleton == null) return;               //return if the Kinect does not recognize any skeletons

            CoordinateMapper mapper = new CoordinateMapper(mainKinectSensor);     //mapper between skeleton and depth image

            if (!corrected)
            {
                DepthImagePoint pointHead = mapper.MapSkeletonPointToDepthPoint(mainSkeleton.Joints[JointType.Head].Position, DepthImageFormat.Resolution640x480Fps30);


                //if (pointHead.Y < 100)
                //{
                //    mainKinectSensor.ElevationAngle += Convert.ToInt32((120 - pointHead.Y) / TILT_FACTOR);
                //    corrected = true;
                //    return;
                //}
                //if (pointHead.Y > 140)
                //{
                //    mainKinectSensor.ElevationAngle -= Convert.ToInt32((pointHead.Y - 120) / TILT_FACTOR);
                //    corrected = true;
                //    return;
                //}

            }
            corrected = true;

            DepthImagePoint pointRight = mapper.MapSkeletonPointToDepthPoint(mainSkeleton.Joints[JointType.HandRight].Position, DepthImageFormat.Resolution640x480Fps30); //get the right hand
            DepthImagePoint pointLeft = mapper.MapSkeletonPointToDepthPoint(mainSkeleton.Joints[JointType.HandLeft].Position, DepthImageFormat.Resolution640x480Fps30); //get the left hand

            if (rotation & Math.Abs(pointLeft.X - pointRight.X) < HANDS_DISTANCE)
            {
                mainWindow.setHandMarkers(pointRight.X, pointRight.Y, pointLeft.X, pointLeft.Y, true);
                if (!playing) return;
                if (oldAngle == -1)
                {
                    oldAngle = pointLeft.Y - pointRight.Y;
                }
                room.rotateCurrentElement(((pointLeft.Y - pointRight.Y) - oldAngle) / 2);
                oldAngle = pointLeft.Y - pointRight.Y;
            }
            else
            {
                oldAngle = -1;
                mainWindow.setHandMarkers(pointRight.X, pointRight.Y, pointLeft.X, pointLeft.Y, false);
            }

            if (!playing) return;

            if (absolute)
            {
                if (zMax == -1 && zMin == -1)
                {

                    zMax = pointRight.Depth + 400;
                    zMin = pointRight.Depth - 400;
                }

                if (pointRight.Depth > zMax)     //adapt the Z-Range if the player goes out of it
                {
                    zMax = pointRight.Depth;
                    zMin = pointRight.Depth - 800;
                }
                if (pointRight.Depth < zMin)
                {
                    zMax = pointRight.Depth + 800;
                    zMin = pointRight.Depth;
                }

                Console.WriteLine(pointRight.Depth - zMin);
                room.translateCurrentElementAbsolute(pointRight.X, pointRight.Y, pointRight.Depth - zMin);  //absolute movement
            }
            else
            {
                if (oldx == -1)             //relative movement
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

                room.translateCurrentElementRelative((pointRight.X - oldx) * Room.FACTOR_X, 0, 0);
                room.translateCurrentElementRelative(0, (oldy - pointRight.Y) * Room.FACTOR_Y, 0);
                room.translateCurrentElementRelative(0, 0, (pointRight.Depth - oldz) * Room.FACTOR_Z);

                oldx = pointRight.X;
                oldy = pointRight.Y;
                oldz = pointRight.Depth;
            }
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
                initializeKinect();
            }
            mainWindow.displayKinectStatus(e.Status.ToString());
        }

        public void changeModus()
        {
            if (absolute)
            {
                absolute = false;
            }
            else
            {
                absolute = true;
            }
        }
    }
}
