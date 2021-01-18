using System;
using MediatR;
using Pubquiz.Domain.Models;

namespace Pubquiz.Logic.Messages
{
    public class GameStateChanged : INotification
    {
        public string GameId { get; }
        public GameState OldGameState { get; }
        public GameState NewGameState { get; }

        public GameStateChanged(string gameId, GameState oldGameState, GameState newGameState)
        {
            GameId = gameId;
            OldGameState = oldGameState;
            NewGameState = newGameState;
        }
    }
}