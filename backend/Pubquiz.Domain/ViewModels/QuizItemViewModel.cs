using System;
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

    public class InteractionViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int MaxScore { get; set; }
        public List<ChoiceOption> ChoiceOptions { get; set; }
        public InteractionType InteractionType { get; set; }

        public string Response { get; set; }
        public List<int> ChosenOptions { get; set; }
        public int ChosenOption { get; set; } = -1;

        public InteractionViewModel()
        {
        }

        public InteractionViewModel(Interaction interaction)
        {
            Id = interaction.Id;
            Text = interaction.Text;
            MaxScore = interaction.MaxScore;
            ChoiceOptions = interaction.ChoiceOptions;
            InteractionType = interaction.InteractionType;
        }

        public InteractionViewModel(Interaction interaction, InteractionResponse interactionResponse) : this(
            interaction)
        {
            switch (InteractionType)
            {
                case InteractionType.MultipleChoice:
                    ChosenOption = interactionResponse.ChoiceOptionIds.FirstOrDefault();
                    break;
                case InteractionType.MultipleResponse:
                    ChosenOptions = interactionResponse.ChoiceOptionIds;
                    break;
                case InteractionType.ShortAnswer:
                case InteractionType.ExtendedText:
                    Response = interactionResponse.Response;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}