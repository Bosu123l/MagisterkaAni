using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Point = System.Drawing.Point;

namespace Domain
{
    public class CutPhoto
    {
        Size _orgImageSize;
        Size _viewWindowSize;
        Rectangle _rectangleOfCut;
        Rectangle _selectedRectangle;
        float _highWidthProportionOrgImage;
        float _highWidthProportionviewWindowSize;
        float _imageViewProportion;
        bool _isVerticalImage;//pionowy


        public CutPhoto(Size orgImageSize, Size viewWindowSize, Rectangle selectedRectangle)
        {
            if(orgImageSize!=Size.Empty)
            {
                _orgImageSize = orgImageSize;
                _highWidthProportionOrgImage = (float)orgImageSize.Height /(float)orgImageSize.Width;
            }

            if (viewWindowSize != Size.Empty)
            {
                _viewWindowSize = viewWindowSize;
                _highWidthProportionviewWindowSize = (float)_viewWindowSize.Height / (float)_viewWindowSize.Width;
            }

            if(_highWidthProportionOrgImage>1)
            {
                _imageViewProportion = (float)orgImageSize.Height / (float)viewWindowSize.Height;
                _isVerticalImage = true;
            }else
            {
                _imageViewProportion = (float)orgImageSize.Width / (float)viewWindowSize.Width;
                _isVerticalImage = true;
            }

            if(selectedRectangle != Rectangle.Empty)
            {
                _selectedRectangle = selectedRectangle;
            }

            CalculateRectangleOfCut();
        }

        private void CalculateRectangleOfCut()
        {
            Size imageAtViewSize = new Size((int)((float)_orgImageSize.Width / _imageViewProportion),
                                            (int)((float)_orgImageSize.Height / _imageViewProportion));

            _rectangleOfCut.Height = (int)((float)_selectedRectangle.Height * _imageViewProportion);
            if (_rectangleOfCut.Height > _orgImageSize.Height) _rectangleOfCut.Height = _orgImageSize.Height;
            _rectangleOfCut.Width = (int)((float)_selectedRectangle.Width * _imageViewProportion);
            if (_rectangleOfCut.Width > _orgImageSize.Width) _rectangleOfCut.Width = _orgImageSize.Width;


            if (_isVerticalImage)
            {
                float emptySpace = ((float)_viewWindowSize.Width - (float)imageAtViewSize.Width) / 2f;
                _rectangleOfCut.X = (int)(((float)_selectedRectangle.X - emptySpace) * _imageViewProportion);
                if (_rectangleOfCut.X < 0) _rectangleOfCut.X = 0;
                _rectangleOfCut.Y = (int)((float)_selectedRectangle.Y * _imageViewProportion);
            }
            else
            {
                float emptySpace = (_viewWindowSize.Height - imageAtViewSize.Height) / 2;
                _rectangleOfCut.Y = (int)(((float)_selectedRectangle.Y - emptySpace) * _imageViewProportion);
                if (_rectangleOfCut.Y < 0) _rectangleOfCut.Y = 0;
                _rectangleOfCut.X = (int)((float)_selectedRectangle.X * _imageViewProportion);
            }            
        }

        public Image<Bgr, byte> CutImageByRectangle(Image<Bgr, byte> image)
        {
            image.ROI = _rectangleOfCut;
            Image<Bgr, byte> result = image.CopyBlank();
            image.CopyTo(result);
            return result;
        }
    }
}
