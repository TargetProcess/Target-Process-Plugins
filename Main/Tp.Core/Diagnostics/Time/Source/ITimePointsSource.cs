namespace Tp.Core.Diagnostics.Time.Source
{
	public interface ITimePointsSource
	{
		ITimePoints Create(params TimePoint[] points);
	}
}