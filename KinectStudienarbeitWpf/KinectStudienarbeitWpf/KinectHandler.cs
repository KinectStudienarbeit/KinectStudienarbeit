using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace KinectStudienarbeitWpf
{
    class KinectHandler
    {
        public KinectSensor mainKinect;

        public KinectHandler()
        {
            //mainKinect = KinectSensor.KinectSensors.FirstOrDefault();
            //KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
        }

        //private abstract void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        //{
        //}
    }
}
