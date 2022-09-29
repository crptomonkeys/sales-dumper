namespace System
{
    public static class Extensions
    {
        public static DateTimeOffset Truncate(this DateTimeOffset value, TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero)
            {
                return value;
            }

            if (value == DateTimeOffset.MinValue || value == DateTimeOffset.MaxValue)
            {
                return value; // do not modify "guard" values
            }

            return value.AddTicks(-(value.Ticks % timeSpan.Ticks));
        }

        public static TimeSpan Truncate(this TimeSpan value, TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero)
            {
                return value;
            }

            if (value == TimeSpan.MinValue || value == TimeSpan.MaxValue)
            {
                return value; // do not modify "guard" values
            }

            return value - TimeSpan.FromTicks(value.Ticks % timeSpan.Ticks);
        }

    }
}
