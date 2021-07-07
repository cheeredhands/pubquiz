using System;
using System.Data;
using System.Linq;

namespace Pubquiz.Logic.Tools
{
    public static class DataRowExtensions
    {
        public static bool IsEmpty(this DataRow row)
        {
            return row == null || row.ItemArray.All(i => i is DBNull);
        }
    }
}