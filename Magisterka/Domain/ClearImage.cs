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
    public class ClearImage : IDisposable
    {
        struct MatElement
        {
            public int _h;
            public int _w;
            public MCvScalar _color;
        }

        private Image<Bgr, byte> _orgImage;
        MCvScalar _defectsColor = new MCvScalar(255, 0, 255);
        private int _kernelSize = 9;

        public ClearImage(Image<Bgr, byte> orgImage)
        {
            if (orgImage != null)
            {
                _orgImage = orgImage.Copy();
            }
            else
            {
                throw new ArgumentNullException(nameof(orgImage));
            }
        }

        public Image<Bgr, byte> ClearImageByDefects()
        {
            int xSetp = _kernelSize / 2;
            int yStep = _kernelSize / 2;
            int width = _orgImage.Width - _kernelSize;
            int height = _orgImage.Height - _kernelSize;

            for (int x = 0; x < height; x += xSetp)
            {
                for (int y = 0; y < width; y += yStep)
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

        private List<MatElement> GetMat(int min_h, int min_w)
        {
            List<MatElement> mat = new List<MatElement>();
            bool isDefect = false;

            for (int h = 0; h < _kernelSize; h++)
            {
                int H = min_h + h;
                for (int w = 0; w < _kernelSize; w++)
                {
                    
                    int W = min_w + w;
                    try
                    {
                        if (_orgImage[H, W].MCvScalar.Equals(_defectsColor) && isDefect == false)
                        {
                            isDefect = true;
                        }
                        mat.Add(new MatElement() { _h = H, _w = W, _color = _orgImage[H, W].MCvScalar });
                    }
                    catch (Exception ex)
                    {
                        var exe = ex.Message;
                    }  
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

                mat.Where(x => x._color.Equals(_defectsColor)).ToList().ForEach(x =>
                {
                    _orgImage.Data[x._h, x._w, 0] = (byte)V0;
                    _orgImage.Data[x._h, x._w, 1] = (byte)V1;
                    _orgImage.Data[x._h, x._w, 2] = (byte)V2;
                });
            }
        }

        public Image<Bgr, byte> SpiralClean()
        {
            int step = _kernelSize / 2;
            int minH, minW, maxH, maxW;
            int maxImgH, maxImgW;
            List<Point> defect = new List<Point>();

            maxImgH = (_orgImage.Height - _kernelSize);
            maxImgW = (_orgImage.Width - _kernelSize);

            minH = 0;
            maxH = _orgImage.Height - _kernelSize;

            minW = 0;
            maxW = _orgImage.Width - _kernelSize;

            int h = minH, w = minW;

            while (minH < maxH || minW < maxW)
            {
                for (w = minW; w < maxW; w += step)
                {
                    SpiralCleanMat(h, w);
                }

                w = maxW > 0 ? maxW : 0;

                for (h = minH; h < maxH; h += step)
                {
                    SpiralCleanMat(h, w);
                }

                h = maxH > 0 ? maxH : 0;

                for (; w > minW; w -= step)
                {
                    SpiralCleanMat(h, w);
                }

                w = minW < maxImgW ? minW : maxImgW;

                for (; h > minH; h -= step)
                {
                    SpiralCleanMat(h, w);
                }

                h = minH < maxImgH ? minH : maxImgH;

                minH += step;
                minW += step;

                maxH -= step;
                maxW -= step;
            }

            return _orgImage;
        }

        public Image<Bgr,byte> SpiralCleanLargeDefects(VectorOfVectorOfPoint defects)
        {
            for (int i = 0; i < defects.Size; i++)
            {
                SpiralClean(defects[i]);
            }
            return _orgImage;
        }

        public Image<Bgr, byte> SpiralClean(VectorOfPoint defectContur)
        {
            int step = _kernelSize / 2;
            int minH, minW, maxH, maxW;
            int maxImgH, maxImgW;
            List<Point> defect = new List<Point>();

            CvInvoke.FillPoly(_orgImage, new VectorOfVectorOfPoint(defectContur), _defectsColor, LineType.FourConnected);

            for (int i = 0; i < defectContur.Size; i++)
            {
                defect.Add(defectContur[i]);
            }

            maxImgH = (_orgImage.Height - _kernelSize);
            maxImgW = (_orgImage.Width - _kernelSize); 

            minH = defect.Min(dh => dh.Y);
            minH = (minH - step) > 0 ? (minH - step) : 0;

            maxH = defect.Max(dh => dh.Y);
            maxH = (maxH + step + _kernelSize) <= _orgImage.Height ? (maxH + step) : maxImgH;


            minW = defect.Min(dw => dw.X);
            minW = (minW - step) > 0 ? (minW - step) : 0;
            maxW = defect.Max(dw => dw.X);
            maxW = (maxW + step + _kernelSize) <= _orgImage.Width ? (maxW + step) : maxImgW; 

            int h = minH, w = minW;

            while (minH < maxH || minW < maxW)
            {
                for (w = minW; w < maxW; w += step)
                {
                    SpiralCleanMat(h, w);
                }

                w = maxW > 0? maxW : 0;

                for (h = minH; h < maxH; h += step)
                {
                    SpiralCleanMat(h, w);
                }

                h = maxH > 0? maxH : 0;

                for (; w > minW; w -= step)
                {
                    SpiralCleanMat(h, w);
                }

                w = minW < maxImgW? minW : maxImgW;

                for (; h > minH; h -= step)
                {
                    SpiralCleanMat(h, w);
                }

                h = minH < maxImgH?  minH : maxImgH;

                minH += step;
                minW += step;

                maxH -= step;
                maxW -= step;
            }

            return _orgImage;
        }

        private void SpiralCleanMat(int h, int w)
        {
            List<MatElement> mat = GetMat(h, w);
            if (mat != null)
            {
                RepairMat(mat);
            }
        }

        public Image<Bgr,byte> InPaintMethod(VectorOfVectorOfPoint defects, InpaintType ip)
        {
            for (int i = 0; i < defects.Size; i++)
            {
                InPaintCleaner(defects[i], ip);
            }
            return _orgImage;
        }

        private void InPaintCleaner(VectorOfPoint defect, InpaintType ip)
        {
            var mask = GetMaskOfDefect(defect);
            CvInvoke.Inpaint(_orgImage, mask, _orgImage, 1.0, ip);
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
