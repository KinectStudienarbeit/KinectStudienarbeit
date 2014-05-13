using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectStudienarbeitWpf
{
    /// <summary>
    /// Holds all important constants for borders etc... will hold logic for the room
    /// </summary>
    class Room
    {
        public const int BOARDER_X_N = -15;
        public const int BOARDER_X_P = 15;
        public const int BOARDER_Y_N = -8;
        public const int BOARDER_Y_P = 8;
        public const int BOARDER_Z_N = -13;
        public const int BOARDER_Z_P = 13;
        public const double WALL_Z = -7;
        public const double LOCH_X_L = 4.1;
        public const double LOCH_Y_U = 3.3;
        public const double LOCH_X_R = 4.7;
        public const double LOCH_Y_O = 4.1;
        public static int kinectZmax = -1;
        public static int kinectZmin = -1;
        public const double FACTOR_X = Room.BOARDER_X_P / 320D * 2;
        public const double FACTOR_Y = Room.BOARDER_Y_P / 240D * 2;
        public const double FACTOR_Z = Room.BOARDER_Z_P / 250D;
    }
}
