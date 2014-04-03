using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace WPFTest
{
    class BlenderModel
    {
        private ModelVisual3D modelVisual3D;
        private Transform3DGroup transformations = new Transform3DGroup();
        private double offsetX = 0;
        private double offsetY = 0;
        private double offsetZ = 0;


        public BlenderModel(BlenderResourceDictionary blenderResourceDictionary, String index)
            : this(blenderResourceDictionary.resourceDictionary, index)
        {
        }

        public BlenderModel(ResourceDictionary resourceDictionary, String index)
        {
            Model3DGroup model3DGroup = resourceDictionary[index] as Model3DGroup;
            modelVisual3D = new ModelVisual3D();
            modelVisual3D.Content = model3DGroup;
        }

        public void addToViewPort(Viewport3D viewport)
        {
            viewport.Children.Add(modelVisual3D);
        }

        public void removeFromViewport(Viewport3D viewport)
        {
            viewport.Children.Remove(modelVisual3D);
        }


        public void rotate(double x, double y, double z)
        {
            MatrixTransform3D matrixTransform = new MatrixTransform3D();
            matrixTransform.Matrix = CalculateRotationMatrix(x, y, z);
            
            transformations.Children.Add(matrixTransform);
            modelVisual3D.Transform = transformations;
        }

        public void translate(double x, double y, double z)
        {
            MatrixTransform3D matrixTransform = new MatrixTransform3D();
            matrixTransform.Matrix = calculateTranslationMatrix(x, y, z);
            offsetX += matrixTransform.Matrix.OffsetX;
            offsetY += matrixTransform.Matrix.OffsetY;
            offsetZ += matrixTransform.Matrix.OffsetZ;
            transformations.Children.Add(matrixTransform);
            modelVisual3D.Transform = transformations;
        }

        Matrix3D CalculateRotationMatrix(double x, double y, double z)      //taken from http://stackoverflow.com/questions/2042214/wpf-3d-rotate-a-model-around-its-own-axes
                                                                            //adjusted for offset by Dawid Rusin
        {
            Matrix3D matrix = new Matrix3D();
            matrix.RotateAt(new Quaternion(new Vector3D(1, 0, 0), x), new Point3D(offsetX, offsetY, offsetZ));
            matrix.RotateAt(new Quaternion(new Vector3D(0, 1, 0) * matrix, y), new Point3D(offsetX, offsetY, offsetZ));
            matrix.RotateAt(new Quaternion(new Vector3D(0, 0, 1) * matrix, z), new Point3D(offsetX, offsetY, offsetZ));

            return matrix;
        }

        Matrix3D calculateTranslationMatrix(double x, double y, double z)
        {
            Matrix3D matrix = new Matrix3D();

            matrix.Translate(new Vector3D(x, y, z));

            return matrix;
        }
    }
}
