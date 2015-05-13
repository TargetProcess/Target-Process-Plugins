using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Tp.Components
{
	public class PreviewGraphics
	{
		static public Bitmap GetThumbnail(string text, int width, int height)
		{
			var bitmap = new Bitmap(width, height);
			using (var graphics = Graphics.FromImage(bitmap))
			{
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
				graphics.TextRenderingHint = TextRenderingHint.AntiAlias;


				using (var brush = GetBackgroundBrush(width, height))
				{
					graphics.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
				}

				using (var pen = new Pen(ColorTranslator.FromHtml("#E7E7E7"), height/30f))
				{
					var paddingTop = height/15;
					var yStep = height/14;
					var xStep = width/10;

					for (var i = 1; i < 4; i++)
						graphics.DrawLine(pen, xStep, paddingTop + yStep*i, width/2, paddingTop + yStep*i);

					for (var i = 4; i < 10; i++)
					{
						var yPosition = (pen.Width) + paddingTop + yStep*i;
						graphics.DrawLine(pen, xStep, yPosition, width - xStep, yPosition);
					}
				}

				using (var pen = new Pen(ColorTranslator.FromHtml("#F2F2F2"), 2F))
				graphics.DrawRectangle(pen, 0, 0, width, height);

				using (var brush = GetTitleBrush(width, height))
				{
					graphics.FillRectangle(brush, 0, height - height/5, width, height/5);
				}

				using (var pen = new Pen(ColorTranslator.FromHtml("#195899"), 1f))
				{
					graphics.DrawLine(pen, -1, height - height / 5, width + 1, height - height / 5);
				}
				using (var pen = new Pen( ColorTranslator.FromHtml("#5193D5"), 1f))
				{
					graphics.DrawLine(pen, -1, height, width + 1, height);
				}
				using (var font = new Font("Verdana", height/8f, FontStyle.Bold))
				{
					using (var stringFormat = new StringFormat(StringFormatFlags.FitBlackBox)
					                          	{
					                          		Alignment = StringAlignment.Center,
					                          		LineAlignment = StringAlignment.Center
					                          	})
					{
						graphics.DrawString(text, font, Brushes.White, new Rectangle(-1, height - height/5, width + 1, height/5),
											stringFormat);
					}
				}
			}
			return bitmap;
		}

		private static LinearGradientBrush GetTitleBrush(int width, int height)
		{
			return new LinearGradientBrush(new Rectangle(-1, height - height/5, width + 1, height/5),
			                               ColorTranslator.FromHtml("#6DAAE9"), ColorTranslator.FromHtml("#4F7DAB"),
			                               LinearGradientMode.Vertical);
		}

		private static LinearGradientBrush GetBackgroundBrush(int width, int height)
		{
			return new LinearGradientBrush(new Rectangle(0, 0, width, height),
			                               ColorTranslator.FromHtml("#FDFDFD"), ColorTranslator.FromHtml("#FDFDFE"),
			                               1f,
			                               true);
		}
	}
}