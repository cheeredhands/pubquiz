using System;
using System.Collections.Generic;
using System.Text;
using Pubquiz.Domain.Models;

namespace Pubquiz.Domain.ViewModels
{
    public class GameViewModel
    {
        public Guid GameId { get; set; }
        public string GameTitle { get; set; }
        public GameState State { get; set; }
    }
}