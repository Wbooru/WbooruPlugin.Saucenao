using System;

namespace WbooruPlugin.Saucenao
{
    public class RequestLimit
    {
        public RequestLimit(TimeSpan time,int count)
        {
            Time = time;
            Count = count;
        }

        public TimeSpan Time { get; }
        public int Count { get; }

        internal void CheckLimit()
        {
            throw new NotImplementedException();
        }
    }
}