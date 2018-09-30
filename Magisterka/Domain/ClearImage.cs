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
    public class ClearImage: IDisposable
    {
        struct MatElement
        {
            public int _x;
            public int _y;
            public MCvScalar _color;
        }

        private Image<Bgr, byte> _orgImage;
        MCvScalar _defectsColor = new MCvScalar(255, 0, 255);
        private int _kernelSize = 5;

        public ClearImage(Image<Bgr, byte> orgImage)
        {
            _orgImage = orgImage;
        }

        public Image<Bgr, byte> ClearImageByDefects()
        {
            int xSetp=_kernelSize/2;
            int yStep= _kernelSize / 2;
            int width = _orgImage.Width - _kernelSize;
            int height = _orgImage.Height - _kernelSize;

            for (int x = 0; x < height; x+=xSetp)
            {
                for (int y = 0; y < width; y+=yStep)
                {
                    List<MatElement> mat = GetMat(x, y);
                    if (mat != null)
                    {
                        RepairMat(mat);
                    }
                }
            }

            return _orgImage;
        }
        
        private List<MatElement> GetMat(int sx, int sy)
        {
            List<MatElement> mat = new List<MatElement>();
            bool isDefect = false;
            for (int x = 0; x < _kernelSize; x ++)
            {
                for (int y = 0; y < _kernelSize; y++)
                {
                    int X = sx + x;
                    int Y = sy + y;
                    if(_orgImage[X, Y].MCvScalar.Equals(_defectsColor) && isDefect==false)
                    {
                        isDefect = true;
                    }
                    mat.Add(new MatElement() { _x = X, _y = Y, _color = _orgImage[X, Y].MCvScalar });
                }
            }

            if(isDefect)
            {
                return mat;
            }
            return null;
        }

        private void RepairMat(List<MatElement> mat)
        {
            var clearMat = mat.Where(x => !x._color.Equals(_defectsColor)).ToList();

            if(clearMat.Count>0)
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
                } );
            }
        }

        public void SpiralClean(VectorOfPoint defectContur)
        {
            int step = _kernelSize / 2;
            int minH, minW, maxH, maxW;
            List<Point> defect = new List<Point>();

           // CvInvoke.FillPoly(_orgImage, new VectorOfVectorOfPoint(defectContur), _defectsColor, LineTwpe.FourConnected);

            for (int i = 0; i < defectContur.Size; i++)
            {
                defect.Add(defectContur[i]);
            }

            minH = defect.Min(dh => dh.X);
            minH = (minH - step) > 0 ? (minH - step) : 0;

            maxH = defect.Max(dh => dh.Y);
            maxH = (maxH + step +_kernelSize) <= _orgImage.Height ? (maxH + step) : _orgImage.Height;


            minW = defect.Min(dw => dw.Y);
            minW = (minW - step) > 0 ? (minW - step) : 0;
            maxW = defect.Max(dw => dw.Y);
            maxW = (maxW + step + _kernelSize) <= _orgImage.Width ? (maxW + step) : _orgImage.Width;

            int h = minH, w = minW;

            while (true)
            {

                for (w = minW; w < maxW; w += step)
                {
                    SpiralCleanMat(h, w);
                }

                for (h = minH; h < maxH; h += step)
                {
                    SpiralCleanMat(h, w);
                }

                for (; w > minW; w -= step)
                {
                    SpiralCleanMat(h, w);
                }


                for (; h > minH; h -= step)
                {
                    SpiralCleanMat(h, w);
                }

                minH += step;
                minW += step;

                maxH -= step;
                maxW -= step;
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

        private Image<Gray, byte> GetMaskOfDefect(VectorOfPoint defect)
        {
            Image<Gray, byte> mask = _orgImage[0].CopyBlank();
            CvInvoke.FillPoly(mask, new VectorOfVectorOfPoint(defect), new MCvScalar(255, 255, 255), LineType.FourConnected);
            return mask;
        }

        public void Dispose()
        {
            
        }
    }
}
