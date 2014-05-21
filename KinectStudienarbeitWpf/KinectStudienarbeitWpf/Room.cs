using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KinectStudienarbeitWpf
{
    /// <summary>
    /// Holds all important constants for borders etc... will hold logic for the room
    /// </summary>
    class Room
    {
        public const double BOARDER_X_N = -0.87;
        public const double BOARDER_X_P = 0.87;
        public const double BOARDER_Y_N = -0.3;
        public const double BOARDER_Y_P = 0.31;
        public const double BOARDER_Z_N = 1.86;
        public const double BOARDER_Z_P = 3.39;
        public const double WALL_Z = 2.35;
        public const double LOCH_X_L = 4.1;
        public const double LOCH_Y_U = 3.3;
        public const double LOCH_X_R = 4.7;
        public const double LOCH_Y_O = 4.1;
        public const double FACTOR_X = Room.BOARDER_X_P / 320D * 2;
        public const double FACTOR_Y = Room.BOARDER_Y_P / 240D * 2;
        public const double FACTOR_Z = (Room.BOARDER_Z_P - BOARDER_Z_N) / 800;

        Viewport3D mainViewport;
        List<Element> elementList = new List<Element>();
        public Element currentElement;
        

        public Room(Viewport3D mainViewport)
        {
            this.mainViewport = mainViewport;
            BlenderResourceDictionary dictionary = new BlenderResourceDictionary(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Spielraumblend15.xaml");
            BlenderModel roomModel = new BlenderModel(dictionary, "Raum");
            roomModel.rotate(-90, 0, 0);
            roomModel.rotate(0, -90, 0);
            roomModel.translate(0, -0.5, 3);
            roomModel.scale(-0.2, 0, 0);

            BlenderModel wallModel = new BlenderModel(dictionary, "Wand");
            wallModel.rotate(0, -90, 0);
            wallModel.rotate(0, 0, -90);
            wallModel.scale(-0.75, -0.75, -0.75);
            wallModel.translate(0, 0, 1.9);         

            BlenderModel wallPartModel = new BlenderModel(dictionary, "Wand_Teil");
            wallPartModel.rotate(0, -90, 0);
            wallPartModel.rotate(0, 0, -90);
            wallPartModel.scale(-0.75, -0.75, -0.75);
            wallPartModel.translate(0, 0, 1.9);
            wallPartModel.translate(-0.5, 0.788, 0);

            roomModel.addToViewport(mainViewport);
            wallModel.addToViewport(mainViewport);
            wallPartModel.addToViewport(mainViewport);

            elementList.Add(new Element(dictionary, "Kreis"));
            elementList.Add(new Element(dictionary, "Quader"));
            elementList.Add(new Element(dictionary, "Fuenfeck"));
            elementList.Add(new Element(dictionary, "Kreuz"));
            elementList.Add(new Element(dictionary, "Sechseck"));
            elementList.Add(new Element(dictionary, "Dreieck"));

            currentElement = elementList[5];
            currentElement.model.addToViewport(mainViewport);

            currentElement.translate(0, 0, 3);
        }

        public void rotateCurrentElement(double angle)
        {
            currentElement.rotate(angle);
        }

        public void translateCurrentElementAbsolute(double x, double y, double z)
        {
            currentElement.translateAbsolute(x, y, z);
        }

        public void translateCurrentElementRelative(double x, double y, double z)
        {
            currentElement.translate(x, y, z);
        }
    }
}
