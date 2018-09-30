using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class Dust: IDisposable
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


        public Dust(Image<Bgr, byte> orgImage, VectorOfVectorOfPoint conturMatrix)
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

        public Image<Bgr, byte> RemoveDustDefects()
        {
            int xSetp = _kernelSize / 2;
            int yStep = _kernelSize / 2;
            int width = _orgImage.Width - _kernelSize;
            int height = _orgImage.Height - _kernelSize;

            SetDefectsCountours();            
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

        private void SetDefectsCountours()
        {
            CvInvoke.FillPoly(_orgImage, _conturMatrix, _defectsColor, LineType.FourConnected, 0);
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

        public void Dispose()
        {
            _conturMatrix = null;
            _orgImage.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
