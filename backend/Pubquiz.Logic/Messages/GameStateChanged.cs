using System;
using Pubquiz.Domain.Models;

namespace Pubquiz.Logic.Messages
{
    public class GameStateChanged
    {
        public Guid GameId { get; }
        public GameState OldGameState { get; }
        public GameState NewGameState { get; }

        public GameStateChanged(Guid gameId, GameState oldGameState, GameState newGameState)
        {
            GameId = gameId;
            OldGameState = oldGameState;
            NewGameState = newGameState;
        }
    }
}