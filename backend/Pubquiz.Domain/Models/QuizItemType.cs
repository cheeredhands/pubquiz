namespace Pubquiz.Domain.Models
{
    public enum QuizItemType
    {
        /// <summary>
        /// Multiple options, one correct answer
        /// </summary>
        MultipleChoice,
        /// <summary>
        /// Multiple options, more than one answer to be chosen for max score
        /// </summary>
        MultipleResponse,
        /// <summary>
        /// One line answer, often automatically scoreable
        /// </summary>
        ShortAnswer,
        /// <summary>
        /// Multiline answer, usually not automatically scoreable
        /// </summary>
        ExtendedText,

        /// <summary>
        /// Mixed (multiple interaction at the question level)
        /// </summary>
        Mixed,
        /// <summary>
        /// An informational quiz item, so not a question.
        /// Can be used as a divider between rounds or as a header
        /// and footer of the quiz.
        /// </summary>
        Information
    }
}