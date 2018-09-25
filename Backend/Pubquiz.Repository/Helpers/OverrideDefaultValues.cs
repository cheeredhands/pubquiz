using System;

namespace Pubquiz.Repository.Helpers
{
    /// <summary>
    ///     Using this class you can set modified by/date created by/date yourself
    /// </summary>
    public class OverrideDefaultValues : IDisposable
    {
        internal static bool FillDefaulValues = true;

        public OverrideDefaultValues()
        {
            FillDefaulValues = false;
        }

        public void Dispose()
        {
            FillDefaulValues = true;
        }
    }
}