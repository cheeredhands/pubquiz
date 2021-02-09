using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ExcelDataReader;
using MediatR;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests.Commands
{
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    public class ImportZippedExcelQuizCommand : IRequest<QuizrPackage>
    {
        public string ActorId { get; set; }
        public Stream FileStream { get; set; }
        public string FileName { get; set; }
    }
}