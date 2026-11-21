using System;

namespace wktime
{
    public struct BreakInterval
    {
        public TimeSpan BreakStart { get; }
        public TimeSpan BreakEnd { get; }

        public BreakInterval(TimeSpan breakStart, TimeSpan breakEnd)
        {
            BreakStart = breakStart;
            BreakEnd = breakEnd;
        }
    }
}