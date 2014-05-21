using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectStudienarbeitWpf
{
    class Element
    {
        public List<double> angles = new List<double>();

        private List<double> coordsX_left = new List<double>();
        private List<double> coordsX_right = new List<double>();
        private List<double> coordsY_up = new List<double>();
        private List<double> coordsY_down = new List<double>();
        public BlenderModel model;
        private bool cyclic = false;

        public Element(BlenderModel model)
        {
            this.model = model;
            setTransformation();
            setAngles();
        }

        public Element(BlenderResourceDictionary dictionary, String index)
        {
            model = new BlenderModel(dictionary, index);
            setTransformation();
            setAngles();
        }

        private void setAngles()
        {
            switch (model.index)
            {
                case "Quader":
                    for (int i = 0; i < 360; i += 90)
                    {
                        addCoords(i, -0.51, -0.24, -0.49, -0.26);
                    }
                    //addCoords(0, -0.51, -0.24, -0.49, -0.26);
                    //addCoords(90, -0.51, -0.24, -0.49, -0.26);
                    //addCoords(180, -0.51, -0.24, -0.49, -0.26);
                    //addCoords(270, -0.51, -0.24, -0.49, -0.26);
                    break;
                case "Fuenfeck":
                    addCoords(0, -0.01, 0.28, 0.02, 0.25);
                    break;
                case "Kreuz":
                    addCoords(0, -0.01, -0.25, 0, -0.26);
                    addCoords(90, -0.01, -0.25, 0, -0.26);
                    addCoords(180, -0.01, -0.24, 0, -0.25);
                    addCoords(270, -0.01, -0.25, 0, -0.26);
                    break;
                case "Dreieck":
                    addCoords(5.5, 0.49, 0.23, 0.51, 0.21);
                    break;
                case "Sechseck":
                    for (int i = 30; i < 360; i += 60)
                    {
                        addCoords(i, 0.49, -0.24, 0.51, -0.26);
                    }
                    //addCoords(30, 0.49, -0.24, 0.51, -0.26);
                    //addCoords(90, 0.49, -0.24, 0.51, -0.26);
                    //addCoords(150, 0.49, -0.24, 0.51, -0.26);
                    //addCoords(210, 0.49, -0.24, 0.51, -0.26);
                    //addCoords(270, 0.49, -0.24, 0.51, -0.26);
                    //addCoords(330, 0.49, -0.24, 0.51, -0.26);
                    break;
                case "Kreis":
                    cyclic = true;
                    addCoords(0, -0.51, 0.26, -0.49, 0.24);
                    break;
            }
        }

        private void setTransformation()
        {
            switch (model.index)
            {
                case "Fuenfeck":
                    model.scale(-0.898, -0.882, -0.898);
                    break;
                case "Kreuz":
                    model.rotate(0, -90, 0);
                    model.scale(-0.85, -0.89, -0.85);
                    break;
                case "Sechseck":
                    model.rotate(0, 90, 0);
                    model.rotate(0, 0, 90);
                    model.scale(-0.8, -0.675, -0.8);
                    break;
                case "Dreieck":
                    model.rotate(0, 0, 5.5);
                    model.scale(-0.85, -0.78, -0.85);
                    break;
                default:
                    model.scale(-0.8, -0.8, -0.8);
                    break;
            }
            //if (model.index == "Fuenfeck")
            //{
            //    model.scale(-0.898, -0.882, -0.898);
            //}
            //else if (model.index == "Kreuz")
            //{
            //    model.rotate(0, -90, 0);
            //    model.scale(-0.85, -0.89, -0.85);
            //}
            //else if (model.index == "Sechseck")
            //{
            //    model.rotate(0, 90, 0);
            //    model.rotate(0, 0, 90);
            //    model.scale(-0.8, -0.675, -0.8);
            //}
            //else if (model.index == "Dreieck")
            //{
            //    model.rotate(0, 0, 5.5);
            //    model.scale(-0.85, -0.78, -0.85);
            //}
            //else
            //{
            //    model.scale(-0.8, -0.8, -0.8);
            //}
        }

        public void translate(double x, double y, double z)
        {
            checkCoords();
            model.translate(x, y, z, true);
        }

        public void translateAbsolute(double x, double y, double z)
        {
            checkCoords();
            model.translateAbsolute(x, y, z, true);
        }

        public void rotate(double angle)
        {
            model.rotate(0, 0, angle, true);
        }

        public void addCoords(double angle, double coordX_left, double coordY_up, double coordX_right, double coordY_down)
        {
            angles.Add(angle);
            this.coordsX_left.Add(coordX_left);
            this.coordsX_right.Add(coordX_right);
            this.coordsY_down.Add(coordY_down);
            this.coordsY_up.Add(coordY_up);
        }

        public List<double> getCoords(double angle)
        {
            List<double> returnVal = new List<double>();

            if (cyclic)
            {
                returnVal.Add(coordsX_left[0]);
                returnVal.Add(coordsY_up[0]);
                returnVal.Add(coordsX_right[0]);
                returnVal.Add(coordsY_down[0]);
                return returnVal;
            }
            if (angles.Count != coordsX_left.Count || coordsY_down.Count != coordsY_up.Count || angles.Count != coordsY_up.Count)
            {
                Exception e = new Exception("The angle and coordinate count does not match up! Cannot return correct coords");
                throw e;
            }
            if (!angles.Contains(angle))
            {
                ArgumentException e = new ArgumentException("Not a correct angle!", "angle");
                throw e;
            }

            int index = angles.IndexOf(angle);
            returnVal.Add(coordsX_left[index]);
            returnVal.Add(coordsY_up[index]);
            returnVal.Add(coordsX_right[index]);
            returnVal.Add(coordsY_down[index]);
            return returnVal;
        }

        private void checkCoords()
        {
            double i;
            double a;
            for (i = Math.Round(model.angle) - 5; i <  Math.Round(model.angle) + 5; i++)
            {
                if(angles.Contains((a = Math.Abs(i))))
                {
                    List<double> l = getCoords(a);
                    model.lochX1 = l[0];
                    model.lochY1 = l[1];
                    model.lochX2 = l[2];
                    model.lochY2 = l[3];

                    return;
                }
            }

            model.lochX1 = -1;
            model.lochY1 = -1;
            model.lochX2 = -1;
            model.lochY2 = -1;
        }
    }
}
