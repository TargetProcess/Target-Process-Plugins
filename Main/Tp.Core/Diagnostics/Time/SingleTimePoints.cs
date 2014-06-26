using System;
using System.Collections.Generic;

namespace Tp.Core.Diagnostics.Time
{
	public class SingleTimePoints : TimePointsBase
	{
		public static readonly ITimePoints Empty = new SingleTimePoints();
		private readonly TimePoint[] _start; 

		public SingleTimePoints() : this(TimePoint.Empty)
		{
		}

		public SingleTimePoints(TimePoint _)
		{
			_start = new[] {new TimePoint("-", DateTimeOffset.UtcNow, -1)};
		}

		public override IEnumerable<TimePoint> Points
		{
			get { return _start; }
		}

		public override void Add(TimePoint point)
		{
		}

		public override void AddUtcNow(string name)
		{
		}

		public override string Dump()
		{
			return _start.ToString();
		} 

		public override void CopyFrom(ITimePoints timePoints)
		{
		}

		public override ITimePoints CreateSnapshot()
		{
			return this;
		}
	}
}