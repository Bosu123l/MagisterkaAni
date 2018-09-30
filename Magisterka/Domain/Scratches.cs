using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Domain
{
    public class Scratches: IDisposable
    {
        private VectorOfVectorOfPoint _conturMatrix;
        private Image<Bgr, byte> _orgImage;

        MCvScalar _defectsColor = new MCvScalar(255, 0, 255);
        private int _kernelSize = 5;
        struct MatElement
        {
            public int _x;
            public int _y;
            public MCvScalar _color;
        }

       

        public Scratches(Image<Bgr, byte> orgImage, VectorOfVectorOfPoint conturMatrix)
        {
            if (orgImage != null)
            {
                _orgImage = orgImage.Copy();
            }
            else
            {
                throw new ArgumentNullException(nameof(orgImage));
            }

            if (conturMatrix != null)
            {
                _conturMatrix = conturMatrix;
            }
            else
            {
                throw new ArgumentNullException(nameof(conturMatrix));
            }
        }

        public Image<Bgr,byte>RemoveScrates()
        {
            for (int i = 0; i < _conturMatrix.Size; i++)
            {               
                var mask = GetMaskOfDefect(_conturMatrix[i]);
                CvInvoke.Inpaint(_orgImage, mask, _orgImage, 5, InpaintType.Telea);
            }
            return _orgImage;
        }

        private Image<Gray, byte>GetMaskOfDefect(VectorOfPoint defect)
        {
            Image<Gray, byte> mask = _orgImage[0].CopyBlank();
            CvInvoke.FillPoly(mask, new VectorOfVectorOfPoint(defect), new MCvScalar(255,255,255), LineType.FourConnected);
            return mask;
        }

        public void Dispose()
        {
            _conturMatrix = null;
            _orgImage.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        void SpiralClean(VectorOfPoint defectContur)
        {
            int step = _kernelSize / 2;
            int minX,minY, maxX, maxY;
            List<Point> defect = new List<Point>();
            
            for (int i = 0; i < defectContur.Size; i++)
            {
                defect.Add(defectContur[i]);
            }

            minX = defect.Min(dx => dx.X);
            minX = (minX - step) > 0 ? (minX - step) : 0; 

            maxX = defect.Max(dx => dx.X);
            maxX = (maxX + step) <= _orgImage.Height ? (maxX + step) : _orgImage.Height;


            minY = defect.Min(dy => dy.Y);
            minY = (minY - step) > 0 ? (minY - step) : 0;
            maxY = defect.Max(dy => dy.Y);
            maxY = (maxY + step) <= _orgImage.Width ? (maxY + step) : _orgImage.Width;

            int x= minX, y=minY;

            while (true)
            {
                for (x= minX; x < maxX; x+=step)
                {
                    SpiralCleanMat(x, y);
                }                

                for (y = minY; y < maxY; y+=step)
                {
                    SpiralCleanMat(x, y);
                }              

                for (; x > minX; x -= step)
                {
                    SpiralCleanMat(x, y);
                }               

                for (;y < minY; y -= step)
                {
                    SpiralCleanMat(x, y);
                }

                minX += step;
                minY += step;

                maxX -= step;
                maxY -= step;
            }     
        }

        private void SpiralCleanMat(int x, int y)
        {
            List<MatElement> mat = GetMat(x, y);
            if (mat != null)
            {
                RepairMat(mat);
            }
        }

        private List<MatElement> GetMat(int sx, int sy)
        {
            List<MatElement> mat = new List<MatElement>();
            bool isDefect = false;
            for (int x = 0; x < _kernelSize; x++)
            {
                for (int y = 0; y < _kernelSize; y++)
                {
                    int X = sx + x;
                    int Y = sy + y;
                    if (_orgImage[X, Y].MCvScalar.Equals(_defectsColor) && isDefect == false)
                    {
                        isDefect = true;
                    }
                    mat.Add(new MatElement() { _x = X, _y = Y, _color = _orgImage[X, Y].MCvScalar });
                }
            }

            if (isDefect)
            {
                return mat;
            }
            return null;
        }

        private void RepairMat(List<MatElement> mat)
        {
            var clearMat = mat.Where(x => !x._color.Equals(_defectsColor)).ToList();

            if (clearMat.Count > 0)
            {
                MCvScalar sum = new MCvScalar();
                double V0, V1, V2;

                clearMat.ForEach(x =>
                {
                    sum.V0 += x._color.V0;
                    sum.V1 += x._color.V1;
                    sum.V2 += x._color.V2;
                });

                V0 = sum.V0 / clearMat.Count;
                V1 = sum.V1 / clearMat.Count;
                V2 = sum.V2 / clearMat.Count;

                mat.Where(x => x._color.Equals(_defectsColor)).ToList().ForEach(x => {
                    _orgImage.Data[x._x, x._y, 0] = (byte)V0;
                    _orgImage.Data[x._x, x._y, 1] = (byte)V1;
                    _orgImage.Data[x._x, x._y, 2] = (byte)V2;
                });
            }
        }
    }
}
