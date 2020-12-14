namespace Pubquiz.Logic.Messages
{
    public class ItemNavigated
    {
        public string GameId { get; }
        public string NewSectionId { get; }
        public string NewSectionTitle { get; }
        public string NewQuizItemId { get; }
        public int NewSectionIndex { get; }
        public int NewSectionQuizItemCount { get; }
        public int NewQuizItemIndexInSection { get; }
        public int NewQuizItemIndexInTotal { get; }
        public int NewQuestionIndexInTotal { get; }

        public ItemNavigated(string gameId, string newSectionId, string newSectionTitle, string newQuizItemId, int newSectionIndex,
            int newQuizItemIndexInSection, int newQuizItemIndexInTotal, int newQuestionIndexInTotal, int newSectionQuizItemCount)
        {
            GameId = gameId;
            NewSectionId = newSectionId;
            NewSectionTitle = newSectionTitle;
            NewQuizItemId = newQuizItemId;
            NewSectionIndex = newSectionIndex;
            NewQuizItemIndexInSection = newQuizItemIndexInSection;
            NewQuizItemIndexInTotal = newQuizItemIndexInTotal;
            NewQuestionIndexInTotal = newQuestionIndexInTotal;
            NewSectionQuizItemCount = newSectionQuizItemCount;
        }
    }
}