#region

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

#endregion

namespace Tp.Components
{
    public class ImageHelper
    {
        private static readonly Pen LightGreyPen = new Pen(Brushes.LightSteelBlue, 2.0F);

        #region IImageHelper Members

        public static Bitmap ResizeAndRound(Bitmap bitmap, int maxWidth, int maxHeight)
        {
            using (var bitmapNew = ResizeImage(bitmap, maxWidth, maxHeight, false))
            {
                var rounded = RoundCorners(bitmapNew, maxWidth / 6, Color.Transparent);
                return rounded;
            }
        }

        public static Bitmap ResizeImage(Bitmap bitmap, int maxWidth, int maxHeight, bool withBorder)
        {
            if (bitmap.Width <= maxWidth && bitmap.Height <= maxHeight)
                return (Bitmap) bitmap.Clone();
            return ResizeImageAnyway(bitmap, maxWidth, maxHeight, withBorder);
        }

        public static Bitmap ResizeImageAnyway(Image image, int width, int height, bool withBorder)
        {
            var sourceWidth = image.Width;
            var sourceHeight = image.Height;

            var nPercentW = (width / (double) sourceWidth);
            var nPercentH = (height / (double) sourceHeight);

            var nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;

            var destWidth = (int) (sourceWidth * nPercent);
            var destHeight = (int) (sourceHeight * nPercent);

            var bitmap = new Bitmap(destWidth, destHeight);

            bitmap.SetResolution(image.HorizontalResolution,
                image.VerticalResolution);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(image, 0, 0, destWidth, destWidth);

                if (withBorder)
                {
                    graphics.DrawRectangle(LightGreyPen, 1, 1, bitmap.Width - 2, bitmap.Height - 2);
                }
            }

            return bitmap;
        }

        #endregion

        public static Bitmap RoundCorners(Bitmap startImage, int radious, Color color)
        {
            radious *= 2;
            var rounderImage = new Bitmap(startImage.Width, startImage.Height);
            using (var g = Graphics.FromImage(rounderImage))
            {
                g.Clear(color);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (Brush brush = new TextureBrush(startImage))
                {
                    using (var gp = new GraphicsPath())
                    {
                        gp.AddArc(0, 0, radious, radious, 180, 90);
                        gp.AddArc(0 + rounderImage.Width - radious, 0, radious, radious, 270, 90);
                        gp.AddArc(0 + rounderImage.Width - radious, 0 + rounderImage.Height - radious, radious, radious, 0, 90);
                        gp.AddArc(0, 0 + rounderImage.Height - radious, radious, radious, 90, 90);
                        g.FillPath(brush, gp);
                    }
                }
            }
            return rounderImage;
        }

        public static bool IsImage(Stream stream)
        {
            if (stream == null)
                return false;

            try
            {
                using (new Bitmap(stream))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static Bitmap CropImageToSquare(Bitmap bitmap)
        {
            if (bitmap.Height == bitmap.Width)
                return (Bitmap) bitmap.Clone();

            int size;

            var x = 0;
            var y = 0;

            if (bitmap.Width > bitmap.Height)
            {
                size = bitmap.Height;
                x = (bitmap.Width - bitmap.Height) / 2;
            }
            else
            {
                size = bitmap.Width;
                y = (bitmap.Height - bitmap.Width) / 2;
            }

            var rectangle = new Rectangle(x, y, size, size);
            return bitmap.Clone(rectangle, bitmap.PixelFormat);
        }

        public static Bitmap CropImage(Bitmap bitmap, Rectangle cropRect)
        {
            var rectangle = cropRect;
            var bitmapRect = new Rectangle(new Point(0, 0), bitmap.Size);
            rectangle.Intersect(bitmapRect);
            return bitmap.Clone(rectangle, bitmap.PixelFormat);
        }

        public static Bitmap GetThumbnail(Bitmap initImage, int templateWidth, int templateHeight)
        {
            int initWidth = initImage.Width;
            int initHeight = initImage.Height;

            if (initWidth <= templateWidth && initHeight <= templateHeight)
            {
                var clone = new Bitmap(initImage);
                return clone;
            }

            var templateRate = (1.0 * templateWidth) / templateHeight;
            var initRate = (1.0 * initWidth) / initHeight;
            if (Math.Abs(templateRate - initRate) < 1e-7)
            {
                return DrawImage(initImage, templateWidth, templateHeight, initWidth, initHeight);
            }

            var fromR = new RectangleF(0, 0, 0, 0);
            var toR = new RectangleF(0, 0, 0, 0);

            int width = templateRate > initRate ? initWidth : (int) (initHeight * templateRate);
            int height = templateRate > initRate ? (int) (initWidth / templateRate) : initHeight;

            using (var pickedImage = new Bitmap(width, height))
            {
                using (var pickedG = Graphics.FromImage(pickedImage))
                {
                    if (templateRate > initRate)
                    {
                        fromR.X = 0;
                        fromR.Y = (float) Math.Floor((initHeight - initWidth / templateRate) / 2);
                        fromR.Width = initWidth;
                        fromR.Height = (float) Math.Floor(initWidth / templateRate);

                        toR.X = 0;
                        toR.Y = 0;
                        toR.Width = initWidth;
                        toR.Height = (float) Math.Floor(initWidth / templateRate);
                    }
                    else
                    {
                        fromR.X = (float) Math.Floor((initWidth - initHeight * templateRate) / 2);
                        fromR.Y = 0;
                        fromR.Width = (float) Math.Floor(initHeight * templateRate);
                        fromR.Height = initHeight;

                        toR.X = 0;
                        toR.Y = 0;
                        toR.Width = (float) Math.Floor(initHeight * templateRate);
                        toR.Height = initHeight;
                    }

                    pickedG.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    pickedG.SmoothingMode = SmoothingMode.HighQuality;

                    pickedG.DrawImage(initImage, toR, fromR, GraphicsUnit.Pixel);

                    return DrawImage(pickedImage, templateWidth, templateHeight, pickedImage.Width, pickedImage.Height);
                }
            }
        }

        private static Bitmap DrawImage(Bitmap initImage, int templateWidth, int templateHeight, int initWidth, int initHeight)
        {
            Bitmap image = new Bitmap(templateWidth, templateHeight);
            using (var g = Graphics.FromImage(image))
            {
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.Clear(Color.White);
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(initImage, new Rectangle(0, 0, templateWidth, templateHeight),
                    new Rectangle(0, 0, initWidth, initHeight), GraphicsUnit.Pixel);
            }
            return image;
        }

        public static Bitmap GetThumbnail(Stream stream, int templateWidth, int templateHeight)
        {
            using (var bitmap = new Bitmap(stream))
            {
                return GetThumbnail(bitmap, templateWidth, templateHeight);
            }
        }

        public static Bitmap ResizeImage(Stream stream, int maxWidth, int maxHeight, bool withBorder)
        {
            using (var bitmap = new Bitmap(stream))
            {
                return ResizeImage(bitmap, maxWidth, maxHeight, withBorder);
            }
        }
    }
}
