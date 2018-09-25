using System;

namespace Pubquiz.Repository.Helpers
{
    public class CacheAttribute : Attribute
    {
        public CacheAttribute(int secondsToCache)
        {
            SecondsToCache = secondsToCache;
        }
        public int SecondsToCache { get; set; }
    }
}