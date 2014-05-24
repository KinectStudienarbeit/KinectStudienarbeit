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

        private List<double> coordsX1 = new List<double>();
        private List<double> coordsX2 = new List<double>();
        private List<double> coordsY1 = new List<double>();
        private List<double> coordsY2 = new List<double>();
        public BlenderModel model;
        private bool cyclic = false;
        

        public Element(BlenderModel model)
        {
            this.model = model;
            setTransformations();
            setAngles();
        }

        public Element(BlenderResourceDictionary dictionary, String index)
        {
            model = new BlenderModel(dictionary, index);
            setTransformations();
            setAngles();
        }

        private void setAngles()
        {
            switch (model.index)
            {
                case "Quader":
                    for (int i = -270; i < 360; i += 90)
                    {
                        addCoords(i, -0.53, -0.22, -0.47, -0.28);
                    }
                    break;
                case "Fuenfeck":
                    addCoords(0, -0.01, 0.28, 0.02, 0.25);
                    break;
                case "Kreuz":
                    for (int i = -270; i < 360; i += 90)
                    {
                        addCoords(i, -0.03, -0.23, 0.02, -0.28);
                    }

                    break;
                case "Dreieck":
                    addCoords(5, 0.48, 0.24, 0.51, 0.21);
                    addCoords(-355, 0.48, 0.24, 0.51, 0.21);
                    break;
                case "Sechseck":
                    for (int i = -330; i < 360; i += 60)
                    {
                        addCoords(i, 0.48, -0.23, 0.52, -0.27);
                    }
                    break;
                case "Kreis":
                    cyclic = true;
                    addCoords(0, -0.53, 0.28, -0.47, 0.22);
                    break;
            }
        }

        private void setTransformations()
        {
            model.resetTransformations();

            switch (model.index)
            {
                case "Fuenfeck":
                    model.scale(-0.898, -0.882, -0.898);
                    break;
                case "Kreuz":
                    model.rotate(0, -90, 0);
                    model.scale(-0.87, -0.91, -0.87);
                    break;
                case "Sechseck":
                    model.rotate(0, 90, 0);
                    model.rotate(0, 0, 90);
                    model.scale(-0.81, -0.685, -0.81);
                    break;
                case "Dreieck":
                    model.rotate(0, 0, 5.5);
                    model.rotate(0, 2.5, 0);
                    model.scale(-0.86, -0.78, -0.86);

                    break;
                default:
                    model.scale(-0.82, -0.82, -0.82);
                    break;
            }
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

        private void addCoords(double angle, double x1, double y1, double x2, double y2)
        {
            angles.Add(angle);
            this.coordsX1.Add(x1);
            this.coordsX2.Add(x2);
            this.coordsY2.Add(y2);
            this.coordsY1.Add(y1);
        }

        public List<double> getCoords(double angle)
        {
            List<double> returnVal = new List<double>();

            if (cyclic)
            {
                returnVal.Add(coordsX1[0]);
                returnVal.Add(coordsY1[0]);
                returnVal.Add(coordsX2[0]);
                returnVal.Add(coordsY2[0]);
                return returnVal;
            }
            if (angles.Count != coordsX1.Count || coordsY2.Count != coordsY1.Count || angles.Count != coordsY1.Count)
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
            returnVal.Add(coordsX1[index]);
            returnVal.Add(coordsY1[index]);
            returnVal.Add(coordsX2[index]);
            returnVal.Add(coordsY2[index]);
            return returnVal;
        }

        private void checkCoords()
        {
            if (cyclic)
            {
                List<double> l = getCoords(0);
                model.lochX1 = l[0];
                model.lochY1 = l[1];
                model.lochX2 = l[2];
                model.lochY2 = l[3];
            }
            else
            {
                double i;
                double a;
                for (i = Math.Round(model.angle) - 10; i < Math.Round(model.angle) + 10; i++)
                {
                    if (angles.Contains((a = Math.Abs(i))))
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
}
