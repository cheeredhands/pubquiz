using System.Collections.Generic;
using System.Linq;
using Pubquiz.Domain.Models;
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Pubquiz.Domain.ViewModels
{
    public class QuizItemViewModel
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public List<MediaObject> Media { get; set; }

        public QuizItemType QuizItemType { get; set; }
        public int MaxScore { get; set; }
        public List<InteractionViewModel> Interactions { get; set; }

        public QuizItemViewModel()
        {
        }

        public QuizItemViewModel(QuizItem quizItem)
        {
            Title = quizItem.Title;
            Body = quizItem.Body;
            Media = quizItem.MediaObjects;
            QuizItemType = quizItem.QuizItemType;
            MaxScore = quizItem.MaxScore;
            Interactions = quizItem.Interactions.Select(i => new InteractionViewModel(i)).ToList();
        }
    }

    public class InteractionViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int MaxScore { get; set; }
        public List<ChoiceOption> ChoiceOptions { get; set; }
        public InteractionType InteractionType { get; set; }

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
    }
}