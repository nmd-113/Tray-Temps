namespace TrayTemps
{
    internal struct FrameGraphPoint
    {
        internal double Mean, Peak;
        internal FrameGraphPoint(double mean, double peak) { Mean = mean; Peak = peak; }
    }

    internal struct FrameStatistics
    {
        internal double? AverageFps, FrameTimeMs, PeakFrameTimeMs;
        internal FrameGraphPoint[] Graph;
        internal long GraphTimestamp;
        internal long Source;
        internal int Version;
    }
}
