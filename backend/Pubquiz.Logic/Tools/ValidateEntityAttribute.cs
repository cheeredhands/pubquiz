using System;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Tools
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ValidateEntityAttribute : Attribute
    {
        public Type EntityType { get; set; }
        public string IdPropertyName { get; set; }
    }
}