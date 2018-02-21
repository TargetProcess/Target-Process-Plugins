namespace Tp.Core.Diagnostics.Time.Source
{
    public class TimePointsSource : ITimePointsSource
    {
        public ITimePoints Create(params TimePoint[] points)
        {
            var timePoints = new TimePoints();

            foreach (var point in points)
            {
                timePoints.Add(point);
            }

            return timePoints;
        }
    }
}
