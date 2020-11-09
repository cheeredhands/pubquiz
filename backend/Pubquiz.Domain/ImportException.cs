using System;
using System.Collections.Generic;

namespace Pubquiz.Domain
{
    public class ImportException : Exception
    {
        public List<string> ImportErrorMessages { get; set; }

        public ImportException(List<string> importErrorMessages, string message = "") : base(message)
        {
            ImportErrorMessages = importErrorMessages;
        }
    }
}