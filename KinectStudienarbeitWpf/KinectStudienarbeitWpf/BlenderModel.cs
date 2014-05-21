using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace KinectStudienarbeitWpf
{
    /// <summary>
    /// Makes translation and rotation of Blender-exported Models easier
    /// </summary>
    class BlenderModel
    {
        private ModelVisual3D modelVisual3D;
        private Model3DGroup model3DGroup;
        private Transform3DGroup transformations = new Transform3DGroup();
        private double offsetX = 0;
        private double offsetY = 0;
        private double offsetZ = 0;
        public double angle = 0;
        public String index;
        private Element parentElement;

        public double lochX1 = -1;
        public double lochY1 = -1;
        public double lochX2 = -1;
        public double lochY2 = -1;

        //######## right coordinates logic should be moved to room #####
        private bool rightCoords = false;
        //######## right coordinates logic should be moved to room #####

        /// <summary>
        /// Loads a model from a BlenderResourceDictionary
        /// </summary>
        /// <param name="blenderResourceDictionary">A BlenderResourceDictionary to load the model from</param>
        /// <param name="index">the name/key/index of the model  to load</param>
        public BlenderModel(BlenderResourceDictionary blenderResourceDictionary, String index)
            : this(blenderResourceDictionary.resourceDictionary, index)
        {
        }

        /// <summary>
        /// Loads a model from a (WPF-) ResourceDictionary
        /// </summary>
        /// <param name="resourceDictionary">A (WPF-)  ResourceDictionary to load the model from</param>
        /// <param name="index">the name/key/index of the model to load</param>
        public BlenderModel(ResourceDictionary resourceDictionary, String index)
        {
            model3DGroup = resourceDictionary[index] as Model3DGroup;
            modelVisual3D = new ModelVisual3D();
            modelVisual3D.Content = model3DGroup;
            this.index = index;

        }

        /// <summary>
        /// Returns the current coordinates of the model
        /// </summary>
        /// <returns>Point3D with the current coordinates of the model</returns>
        public Point3D getCoords()
        {
            return new Point3D(offsetX, offsetY, offsetZ);
        }

        /// <summary>
        /// adds the model to the given Viewport
        /// </summary>
        /// <param name="viewport">The used Viewport to add the model to</param>
        public void addToViewport(Viewport3D viewport)
        {
            viewport.Children.Add(modelVisual3D);
        }

        /// <summary>
        /// Removes the model from a Viewport
        /// </summary>
        /// <param name="viewport">The Viewport where the model has to be removed</param>
        public void removeFromViewport(Viewport3D viewport)
        {
            viewport.Children.Remove(modelVisual3D);
        }

        /// <summary>
        /// Rotates the model around its axis, ignores too high values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void rotate(double x, double y, double z)
        {
            rotate(x, y, z, false);
        }

        /// <summary>
        /// Rotates the model around its axis
        /// </summary>
        /// <param name="x">Rotation around the x-axis in degrees</param>
        /// <param name="y">Rotation around the y-axis in degrees</param>
        /// <param name="z">Rotation around the z-axis in degrees</param>
        /// <param name="safe">True if too high values should be ignored, false otherwise</param>
        public void rotate(double x, double y, double z, bool safe)
        {
            if(safe && (Math.Abs(x) > 20 || Math.Abs(y) > 20 || Math.Abs(z) > 20)) return;  //ignore if safe is checked and any operation is > 50
            MatrixTransform3D matrixTransform = new MatrixTransform3D();
            matrixTransform.Matrix = CalculateRotationMatrix(x, y, z);
            transformations.Children.Add(matrixTransform);
            model3DGroup.Transform = transformations;
            angle += z;
            angle = Math.Abs(angle) % 360;

        }

        /// <summary>
        /// Scales the model
        /// </summary>
        /// <param name="x">scale along the x-axis</param>
        /// <param name="y">scale along the y-axis</param>
        /// <param name="z">scale along the z-axis</param>
        public void scale(double x, double y, double z)
        {
            ScaleTransform3D scaleTransform = new ScaleTransform3D();
            scaleTransform.ScaleX += x;
            scaleTransform.ScaleY += y;
            scaleTransform.ScaleZ += z;
            transformations.Children.Add(scaleTransform);
            model3DGroup.Transform = transformations;
        }

        public void translateAbsolute(double x, double y, double z, bool safe)
        {
            if (x < 0 || x > 640 || y < 0 || y > 480) return;   //return if the coords are not on screen
            translate(((x - 320) * Room.FACTOR_X) - offsetX, 0, 0, safe);  //separate x/y from z so you ca still move on the surface of the wall
            translate(0, -(y - 240) * Room.FACTOR_Y - offsetY, 0, safe);
            //translate(0, 0, (z - Room.kinectZmin - 250) * Room.FACTOR_Z - offsetZ, safe);
            translate(0, 0,-(offsetZ- z * Room.FACTOR_Z - Room.BOARDER_Z_N), safe);
            //Console.WriteLine(z + " " + z * Room.FACTOR_Z);
        }

        /// <summary>
        /// Translates (moves) the model in 3D space using absolute values from the kinect for x and y, and a set value range for z
        /// </summary>
        /// <param name="x">x-coordinate to translate to</param>
        /// <param name="y">y-coordinate to translate to</param>
        /// <param name="z">z-coordinate to translate to</param>
        public void translateAbsolute(double x, double y, double z)
        {
            translateAbsolute(x, y, z, false);
        }

        /// <summary>
        /// Translates (moves) the model in the 3D-space, ignores too high values
        /// </summary>
        /// <param name="x">Movement in x-direction</param>
        /// <param name="y">Movement in y-direction</param>
        /// <param name="z">Movement in z-direction</param>
        public void translate(double x, double y, double z)
        {
            translate(x, y, z, false);
        }

        /// <summary>
        /// Translates (moves) the model in the 3D-space
        /// </summary>
        /// <param name="x">Movement in x-direction</param>
        /// <param name="y">Movement in y-direction</param>
        /// <param name="z">Movement in z-direction</param>
        /// <param name="safe">True if too high values should be ignored, false otherwise</param>
        public void translate(double x, double y, double z, bool safe)
        {          
            if (safe && (x > 20 || y > 20 || z > 20))
            {
                return;
            }
            MatrixTransform3D matrixTransform = new MatrixTransform3D();
            matrixTransform.Matrix = calculateTranslationMatrix(x, y, z);
            double xtmp = offsetX + matrixTransform.Matrix.OffsetX;
            double ytmp = offsetY + matrixTransform.Matrix.OffsetY;
            double ztmp = offsetZ + matrixTransform.Matrix.OffsetZ;

            if (!safe || (xtmp < Room.BOARDER_X_P && xtmp > Room.BOARDER_X_N && ytmp < Room.BOARDER_Y_P && ytmp > Room.BOARDER_Y_N && ztmp < Room.BOARDER_Z_P && ztmp > Room.BOARDER_Z_N))
            {
                
                //######## right coordinates logic should be moved to room #####
                if (xtmp >= lochX1 && xtmp <= lochX2 && ytmp <= lochY1 && ytmp >= lochY2)
                {
                    rightCoords = true;

                }
                else rightCoords = false;

                if (rightCoords || ztmp > Room.WALL_Z || !safe)
                {
                //######## right coordinates logic should be moved to room #####

                    offsetX += matrixTransform.Matrix.OffsetX;
                    offsetY += matrixTransform.Matrix.OffsetY;
                    offsetZ += matrixTransform.Matrix.OffsetZ;


                    transformations.Children.Add(matrixTransform);
                    model3DGroup.Transform = transformations;
                }
            }

            Console.WriteLine(offsetX + " " + offsetY + " " + offsetZ + " " + angle);
        }

        private Matrix3D CalculateRotationMatrix(double x, double y, double z)      //taken from http://stackoverflow.com/questions/2042214/wpf-3d-rotate-a-model-around-its-own-axes
        //adjusted for offset by Dawid Rusin
        {
            Matrix3D matrix = new Matrix3D();
            matrix.RotateAt(new Quaternion(new Vector3D(1, 0, 0), x), new Point3D(offsetX, offsetY, offsetZ));
            matrix.RotateAt(new Quaternion(new Vector3D(0, 1, 0) * matrix, y), new Point3D(offsetX, offsetY, offsetZ));
            matrix.RotateAt(new Quaternion(new Vector3D(0, 0, 1) * matrix, z), new Point3D(offsetX, offsetY, offsetZ));

            return matrix;
        }

        private Matrix3D calculateTranslationMatrix(double x, double y, double z)
        {
            Matrix3D matrix = new Matrix3D();
            matrix.Translate(new Vector3D(x, y, z));

            return matrix;
        }

        /// <summary>
        /// Returns the bounding box of the model
        /// </summary>
        /// <returns>The bounding box of the model</returns>
        public Rect3D getBounds()
        {
            return model3DGroup.Bounds;
        }

        

    }
}
