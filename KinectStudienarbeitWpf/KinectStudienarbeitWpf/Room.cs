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
        public const double FACTOR_X = Room.BOARDER_X_P / 320D * 2;
        public const double FACTOR_Y = Room.BOARDER_Y_P / 240D * 2;
        public const double FACTOR_Z = (Room.BOARDER_Z_P - BOARDER_Z_N) / 800;
        public bool difficult;

        MainWindow mainWindow;
        BlenderResourceDictionary mainDictionairy;
        List<Element> elementList = new List<Element>();
        public Element currentElement;
        

        public Room(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            mainDictionairy = new BlenderResourceDictionary(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Spielraumblend15.xaml");
            BlenderModel roomModel = new BlenderModel(mainDictionairy, "Raum");
            roomModel.rotate(-90, 0, 0);
            roomModel.rotate(0, -90, 0);
            roomModel.translate(0, -0.5, 3);
            roomModel.scale(-0.2, 0, 0);

            BlenderModel wallModel = new BlenderModel(mainDictionairy, "Wand");
            wallModel.rotate(0, -90, 0);
            wallModel.rotate(0, 0, -90);
            wallModel.scale(-0.75, -0.75, -0.75);
            wallModel.translate(0, 0, 1.9);

            BlenderModel wallPartModel = new BlenderModel(mainDictionairy, "Wand_Teil");
            wallPartModel.rotate(0, -90, 0);
            wallPartModel.rotate(0, 0, -90);
            wallPartModel.scale(-0.75, -0.75, -0.75);
            wallPartModel.translate(0, 0, 1.9);
            wallPartModel.translate(-0.5, 0.788, 0);

            roomModel.addToViewport(mainWindow.mainViewPort);
            wallModel.addToViewport(mainWindow.mainViewPort);
            wallPartModel.addToViewport(mainWindow.mainViewPort);

            populateElementList();
        }

        void populateElementList()
        {
            elementList.Add(new Element(mainDictionairy, "Kreis"));
            elementList.Add(new Element(mainDictionairy, "Quader"));
            elementList.Add(new Element(mainDictionairy, "Fuenfeck"));
            elementList.Add(new Element(mainDictionairy, "Kreuz"));
            elementList.Add(new Element(mainDictionairy, "Sechseck"));
            elementList.Add(new Element(mainDictionairy, "Dreieck"));
        }

        public void chooseNewElement()
        {
            Random r = new Random();
            int i = r.Next(elementList.Count);
            currentElement = elementList[i];
            currentElement.translate(0, 0, 3);
            currentElement.model.addToViewport(mainWindow.mainViewPort);
            elementList.RemoveAt(i);
            if (difficult)
            {
                currentElement.model.rotate(0, 0, r.Next(360));
            }
            mainWindow.displayElement(6 - elementList.Count);
        }

        public void rotateCurrentElement(double angle)
        {
            if(currentElement.model.getZ() > WALL_Z) currentElement.rotate(angle);
        }

        public void translateCurrentElementAbsolute(double x, double y, double z)
        {
            currentElement.translateAbsolute(x, y, z);
            checkZ();
        }

        public void translateCurrentElementRelative(double x, double y, double z)
        {
            currentElement.translate(x, y, z);
            checkZ();
        }

        public void skipElement()
        {
            if (elementList.Count == 0)
            {
                currentElement.model.removeFromViewport(mainWindow.mainViewPort);
                populateElementList();
                mainWindow.startNextRound();
            }
            else
            {
                currentElement.model.removeFromViewport(mainWindow.mainViewPort);
                chooseNewElement();
            }
        }

        private void checkZ()
        {
            if (currentElement.model.getZ() < WALL_Z - 0.35)
            {
                if (elementList.Count == 0)
                {
                    populateElementList();
                    mainWindow.startNextRound();
                }
                else
                {
                    chooseNewElement();
                }
            }
        }
    }
}
