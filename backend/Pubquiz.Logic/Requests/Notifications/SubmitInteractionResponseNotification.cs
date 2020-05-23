using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Rebus.Bus;
using InteractionResponseAdded = Pubquiz.Logic.Messages.InteractionResponseAdded;

namespace Pubquiz.Logic.Requests.Notifications
{
    /// <summary>
    /// Notification to submit an interaction response for a certain <see cref="Interaction"/> in a <see cref="QuizItem"/>
    /// </summary>
    [ValidateEntity(EntityType = typeof(Team), IdPropertyName = "TeamId")]
    [ValidateEntity(EntityType = typeof(QuizItem), IdPropertyName = "QuizItemId")]
    public class SubmitInteractionResponseNotification : Notification
    {
        public string TeamId { get; set; }
        public string QuizItemId { get; set; }
        public int InteractionId { get; set; }
        public List<int> ChoiceOptionIds { get; set; }
        public string Response { get; set; }

        public SubmitInteractionResponseNotification(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
        {
        }

        protected override async Task DoExecute()
        {
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var team = await teamCollection.GetAsync(TeamId);
            var quizItemCollection = UnitOfWork.GetCollection<QuizItem>();
            var quizItem = await quizItemCollection.GetAsync(QuizItemId);
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(team.CurrentGameId);
            var quizId = game.QuizId;

            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var quiz = await quizCollection.GetAsync(quizId);

            var quizSectionId = quiz.QuizSections
                .FirstOrDefault(qs => qs.QuestionItemRefs.Any(q => q.Id == QuizItemId))?.Id;
            if (string.IsNullOrWhiteSpace(quizSectionId))
            {
                throw new DomainException(ResultCode.QuestionNotInQuiz, "This question doesn't belong to the quiz.",
                    true);
            }

            if (game.CurrentSectionId != quizSectionId)
            {
                throw new DomainException(ResultCode.QuestionNotInCurrentQuizSection,
                    "This question doesn't belong to the current quiz section.", true);
            }

            if (quizItem.Interactions.All(i => i.Id != InteractionId))
            {
                throw new DomainException(ResultCode.InvalidInteractionId, "Invalid InteractionId.", true);
            }


            // save response
            var answer = team.Answers.FirstOrDefault(a => a.QuizItemId == QuizItemId);
            if (answer == null)
            {
                answer = new Answer(quizSectionId, QuizItemId);
                team.Answers.Add(answer);
            }

            answer.SetInteractionResponse(InteractionId, ChoiceOptionIds, Response);

            UnitOfWork.Commit();

            var response = string.IsNullOrWhiteSpace(Response)
                ? GetChoiceOptionTexts(quizItem, ChoiceOptionIds)
                : Response;
            await Bus.Publish(
                new InteractionResponseAdded(game.Id, TeamId, QuizItemId, InteractionId, response));
        }

        private string GetChoiceOptionTexts(QuizItem question, List<int> choiceOptionIds)
        {
            if (choiceOptionIds == null)
            {
                return string.Empty;
            }

            var choiceOptionTexts = new List<string>();
            foreach (var choiceOptionId in choiceOptionIds)
            {
                choiceOptionTexts.Add(question.Interactions[InteractionId].ChoiceOptions[choiceOptionId].Text);
            }

            return string.Join(", ", choiceOptionTexts);
        }
    }
}