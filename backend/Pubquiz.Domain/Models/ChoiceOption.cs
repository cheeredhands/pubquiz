namespace Pubquiz.Domain.Models
{
    public class ChoiceOption
    {
        public int Id { get; set; }

        public string Text { get; set; }
        // maybe later? public Media Media { get; set; }

        public ChoiceOption(int id, string text)
        {
            Id = id;
            Text = text;
        }

        public ChoiceOption()
        {
        }
    }
}