using System.Collections.Generic;

namespace Pubquiz.Domain.Models
{
    public class Interaction
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int MaxScore { get; set; }
        public List<ChoiceOption> ChoiceOptions { get; set; }
        public InteractionType InteractionType { get; set; }
        public Solution Solution { get; set; }

        public Interaction(int id)
        {
            Id = id;
            ChoiceOptions = new List<ChoiceOption>();
        }

        public Interaction()
        {
        }
    }
}