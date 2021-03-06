using System;
using System.Collections.Generic;
using System.Linq;
using Pubquiz.Domain.Models;

namespace Pubquiz.Domain.ViewModels
{
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