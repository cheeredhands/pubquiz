using System;

namespace Pubquiz.Repository.Helpers
{
    [Flags]
    public enum IndexConstraints
    {
        Normal = 0x00000001, // Ascending, non-indexed
        Descending = 0x00000010,
        Unique = 0x00000100,
        Sparse = 0x00001000 // allows nulls in the indexed fields
    }

    // Applied to a member
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class EnsureIndexAttribute : Attribute
    {
        public EnsureIndexAttribute(IndexConstraints ic = IndexConstraints.Normal)
        {
            Descending = (ic & IndexConstraints.Descending) != 0;
            Unique = (ic & IndexConstraints.Unique) != 0;
            Sparse = (ic & IndexConstraints.Sparse) != 0;
        }

        public bool Descending { get; private set; }
        public bool Unique { get; private set; }
        public bool Sparse { get; private set; }
    }
}