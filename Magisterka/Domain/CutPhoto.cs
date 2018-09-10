using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Point = System.Drawing.Point;

namespace Domain
{
    public class CutPhoto: IDisposable
    {
        private Image<Bgr, byte> _orginalImage;
        private Image<Bgr, byte> _cutedImage;
        private int _x;
        private int _y;
        private int _width;
        private int _height;

        public CutPhoto(Image<Bgr, byte> image)
        {
            if (image != null)
                _orginalImage = image;
            else
                throw new ArgumentNullException("Brak zdjęcia");

            _x = _y = _width = _height = -1;

            //SetBound(image);
        }

        public Image<Bgr, byte> Cut(Image<Bgr, byte> image)
        {
            if (_x >= 0 && _y >= 0 && _width > 0 && _height > 0)
            {
                _cutedImage = image.Copy(new Rectangle(_x, _y, _width, _height));
                return _cutedImage;
            }
            else
                throw new NullReferenceException("Brak parametrów przycinania fotografii");
        }

        private void SetBound(Image<Bgr, byte> image)
        {
            int abs = 5;
            int x1, x2, y1, y2;
            Image<Gray, byte> imageGray = image.Convert<Gray, byte>();
            LineSegment2D[][] edges = imageGray.Canny(250, 190).HoughLinesBinary(0.1, Math.PI / 150, 3, 40, 3);
            List<Point> points = new List<Point>();

            //Ustawienie wspólnych punktów
            foreach (var p in edges[0])
            {
                points.Add(p.P1);
                points.Add(p.P2);
            }
            if (points.Count > 0)
            {
                //Pobranie skrajnych
                x1 = points.Min(x => x.X);
                y1 = points.Min(y => y.Y);
                x2 = points.Max(x => x.X);
                y2 = points.Max(y => y.Y);


                x1 = (x1 - abs) < 0 ? 0 : (x1 - abs);
                y1 = (y1 - abs) < 0 ? 0 : (y1 - abs);
                x2 = (x2 + abs) > image.Width ? image.Width : (x2 + abs);
                y2 = (y2 + abs) > image.Height ? image.Height : (y2 + abs);

                _x = x1;
                _y = y1;
                _width = Math.Abs(x2 - x1);
                _height = Math.Abs(y2 - y1);
            }
        }

        public Image<Bgr, byte> AlignPhoto(Image<Bgr, byte> image)
        {
            int x1, x2, y1, y2;
            int deltaX, deltaY, minX, maxX, minY, maxY;
            Image<Gray, byte> editImage = image.Sobel(0, 1, 3).Convert<Gray, byte>();
            editImage = editImage.Canny(250, 90);
            LineSegment2D[][] edges = editImage.HoughLinesBinary(0.1, Math.PI / 150, 3, 30, 3);
            List<Point> points = new List<Point>();

            //Ustawienie wspólnych punktów
            foreach (var p in edges[0])
            {
                points.Add(p.P1);
                points.Add(p.P2);
            }

            x1 = points.Min(x => x.X);
            y1 = points.Min(y => y.Y);
            x2 = points.Max(x => x.X);
            y2 = points.Max(y => y.Y);

            deltaX = Math.Abs(x2 - x1);
            deltaY = Math.Abs(y2 - y1);

            minX = x1 + deltaX / 15;
            maxX = x2 - deltaX / 15;

            minY = y1 + deltaY / 15;
            maxY = y2 - deltaY / 15;

            //Dla dolu
            List<Point> bottom = new List<Point>();
            bottom = points.Where(x => x.Y > maxY).ToList();
            bottom = bottom.Where(x => x.X > minX).Where(x => x.X <= maxX).ToList();

            bottom = bottom.OrderBy(x => x.X).ToList();

            for (int i = 1; i < bottom.Count; i++)
            {
                var tmp = new LineSegment2D(bottom[i - 1], bottom[i]);
                image.Draw(tmp, new Bgr(0, 0, 255), 5);
            }

            //Dla gory
            List<Point> top = new List<Point>();
            top = points.Where(x => x.Y < minY).ToList();
            top = top.Where(x => x.X > minX).Where(x => x.X <= maxX).ToList();

            top = top.OrderBy(x => x.X).ToList();

            for (int i = 1; i < top.Count; i++)
            {
                var tmp = new LineSegment2D(top[i - 1], top[i]);
                image.Draw(tmp, new Bgr(0, 0, 255), 5);
            }


            //Dla boku prawego
            List<Point> right = new List<Point>();
            right = points.Where(x => x.X > maxX).ToList();
            right = right.Where(x => x.Y > minY).Where(x => x.Y < maxY).ToList();


            //right = right.OrderBy(x => x.X).ToList();

            for (int i = 1; i < right.Count; i++)
            {
                var tmp = new LineSegment2D(right[i - 1], right[i]);
                image.Draw(tmp, new Bgr(255, 0, 0), 2 * i);
            }

            foreach (var line in edges.First())
            {
                image.Draw(line, new Bgr(0, 255, 0), 5);
            }

            //Dla lewej
            List<Point> left = new List<Point>();
            left = points.Where(x => x.X < minX).ToList();
            left = left.Where(x => x.Y > minY).ToList();

            left = left.OrderBy(x => x.X).ToList();

            for (int i = 1; i < left.Count; i++)
            {
                var tmp = new LineSegment2D(left[i - 1], left[i]);
                image.Draw(tmp, new Bgr(0, 255, 0), 3 * i);
            }

            return image;
        }

        public Image<Bgr, byte> SetLines()
        {
            int abs = 5;
            int x1, x2, y1, y2;
            Image<Bgr, byte> image = _orginalImage;
            Image<Gray, byte> imageGray = image.Convert<Gray, byte>();
            LineSegment2D[][] edges = imageGray.Canny(250, 190).HoughLinesBinary(0.1, Math.PI / 150, 3, 40, 3);
            List<Point> points = new List<Point>();

            //Ustawienie wspólnych punktów
            foreach (var p in edges[0])
            {
                points.Add(p.P1);
                points.Add(p.P2);
            }

            points = points.OrderBy(x => x.X).ToList();

            Point[] brzeg = new Point[20];
            points.CopyTo(2, brzeg, 0, 20);

            brzeg.OrderBy(x => x.Y);
            

            if (points.Count > 0)
            {
                //Pobranie skrajnych
                x1 = points.Min(x => x.X);
                y1 = points.Min(y => y.Y);
                x2 = points.Max(x => x.X);
                y2 = points.Max(y => y.Y);

                //Wyrysowanie krotkich linni
                int i = 0;

                edges[0] = edges[0].OrderBy(x => x.P1.Y).ToArray();

                foreach (var line in edges.First())
                {
                    image.Draw(line, new Bgr(255 - i, 0, i++), 5);
                }

                var T = new LineSegment2D(brzeg.First(), brzeg.Last());
                image.Draw(T, new Bgr(0, 255, 0), 5);

                ////Zadanie punktow skrajnych
                //var A = new LineSegment2D(new Point(x2, y2), new Point(x1, y2));
                //var B = new LineSegment2D(new Point(x1, y2), new Point(x1, y1));
                //var C = new LineSegment2D(new Point(x1, y1), new Point(x2, y1));
                //var D = new LineSegment2D(new Point(x2, y1), new Point(x2, y2));

                ////Wyrysowanie ramki ciecia
                //image.Draw(A, new Bgr(0, 255, 0), 3);
                //image.Draw(B, new Bgr(0, 255, 0), 3);
                //image.Draw(C, new Bgr(0, 255, 0), 3);
                //image.Draw(D, new Bgr(0, 255, 0), 3);
            }

            return image;
        }

        public Image<Bgr, byte> CuttingPhoto()
        {
            Image<Bgr, byte> editingImage;
            SetBound(_orginalImage);
            _cutedImage = Cut(_orginalImage);
            //editingImage = AlignPhoto(_orginalImage);


            //var imageGray = image.Convert<Gray, byte>();
            //imageGray.InRange(new Gray(0.7), new Gray(0.3));
            //imageGray = imageGray.Sobel(1, 0, 3).Convert<Gray, byte>();
            //imageGray = imageGray.Canny(150, 60);

            //var test = imageGray.Canny(250, 190).HoughLinesBinary(0.1, Math.PI / 150, 3, 30, 3);
            //imageGray.HoughLinesBinary(0.1, Math.PI / 180.0, 1, 20, 3);
            //var test = imageGray.HoughLinesBinary(1, Math.PI / 45, 40, 50, 0);

            //double max = test[0].Max(x => x.Length);
            //List<Point> points = new List<Point>();

            //foreach (var p in test[0])
            //{
            //    points.Add(p.P1);
            //    points.Add(p.P2);
            //}

            //int max_x = points.Max(x => x.X);
            //int max_y = points.Max(y => y.Y);
            //int min_x = points.Min(x => x.X);
            //int min_y = points.Min(y => y.Y);


            //var A = new LineSegment2D(new Point(min_x, min_y), new Point(max_x, min_y));
            //var B = new LineSegment2D(new Point(max_x, min_y), new Point(max_x, max_y));
            //var C = new LineSegment2D(new Point(max_x, max_y), new Point(min_x, max_y));
            //var D = new LineSegment2D(new Point(min_x, max_y), new Point(min_x, min_y));


            //test[0] = test[0].OrderByDescending(x => x.Length).ToArray();

            //for (int i = 0; i < test[0].Length; i++)
            //{
            //    image.Draw(test[0][i], new Bgr(0, 0, 255), 3);
            //}

            //image.Draw(A, new Bgr(0, 255, 0), 3);
            //image.Draw(B, new Bgr(0, 255, 0), 3);
            //image.Draw(C, new Bgr(0, 255, 0), 3);
            //image.Draw(D, new Bgr(0, 255, 0), 3);


            //foreach (var line in test.First())
            //{
            //    image.Draw(line, new Bgr(0, 0, 255), 3);
            //}

            //image = Cut(image);

            return _cutedImage;
        }

        public void Dispose()
        {
            _cutedImage.Dispose();
            _orginalImage.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
