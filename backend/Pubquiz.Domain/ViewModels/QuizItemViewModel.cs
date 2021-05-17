using System.Collections.Generic;
using System.Linq;
using Pubquiz.Domain.Models;

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Pubquiz.Domain.ViewModels
{
    public class QuizItemViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public List<MediaObject> MediaObjects { get; set; }

        public QuizItemType QuizItemType { get; set; }
        public int MaxScore { get; set; }
        public List<InteractionViewModel> Interactions { get; set; }

        public QuizItemViewModel()
        {
        }

        public QuizItemViewModel(QuizItem quizItem, GameState gameState, Answer answer = null)
        {
            Id = quizItem.Id;
            Title = quizItem.Title;
            Body = quizItem.Body;
            MediaObjects = quizItem.MediaObjects.Where(m => m.TeamVisible).ToList();
            MediaObjects = gameState == GameState.Reviewing
                ? MediaObjects.Where(o => o.IsSolution).ToList()
                : MediaObjects.Where(o => !o.IsSolution).ToList();
            QuizItemType = quizItem.QuizItemType;
            MaxScore = quizItem.MaxScore;

            if (answer != null)
            {
                Interactions = quizItem.Interactions
                    .Select((i, index) => new InteractionViewModel(i, answer.InteractionResponses[index])).ToList();
            }
            else
            {
                Interactions = quizItem.Interactions
                    .Select((i, index) => new InteractionViewModel(i)).ToList();
            }
        }
    }
}