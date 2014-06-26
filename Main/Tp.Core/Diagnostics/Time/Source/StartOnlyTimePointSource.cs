using System.Linq;

namespace Tp.Core.Diagnostics.Time.Source
{
	public class StartOnlyTimePointSource : ITimePointsSource
	{
		public ITimePoints Create(params TimePoint[] points)
		{
			var point = points.Any() ? points.First() : TimePoint.Empty;

			return new SingleTimePoints(point);
		}
	}
}