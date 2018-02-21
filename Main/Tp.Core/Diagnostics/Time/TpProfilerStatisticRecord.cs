namespace Tp.Core.Diagnostics.Time
{
    public struct TpProfilerStatisticRecord
    {
        public ProfilerTarget Target { get; set; }
        public string Name { get; set; }
        public string Context { get; set; }
        public string ElapsedTimeRaw { get; set; }
        public int ElapsedTimeTotal { get; set; }
    }
}
