using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Rebus.Bus;
using InteractionResponseAdded = Pubquiz.Logic.Messages.InteractionResponseAdded;

namespace Pubquiz.Logic.Requests
{
    /// <summary>
    /// Notification to submit an interaction response for a certain <see cref="Interaction"/> in a <see cref="Question"/>
    /// </summary>
    [ValidateEntity(EntityType = typeof(Team), IdPropertyName = "TeamId")]
    [ValidateEntity(EntityType = typeof(Question), IdPropertyName = "QuestionId")]
    public class SubmitInteractionResponseNotification : Notification
    {
        public string TeamId { get; set; }
        public string QuestionId { get; set; }
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
            var questionCollection = UnitOfWork.GetCollection<Question>();
            var question = await questionCollection.GetAsync(QuestionId);
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(team.CurrentGameId);
            var quizId = game.QuizId;

            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var quiz = await quizCollection.GetAsync(quizId);

            var quizSectionId = quiz.QuizSections
                .FirstOrDefault(qs => qs.QuestionItems.Any(q => q.Id == QuestionId))?.Id;
            if (string.IsNullOrWhiteSpace(quizSectionId))
            {
                throw new DomainException(ResultCode.QuestionNotInQuiz, "This question doesn't belong to the quiz.",
                    true);
            }

            if (game.CurrentQuizSectionId != quizSectionId)
            {
                throw new DomainException(ResultCode.QuestionNotInCurrentQuizSection,
                    "This question doesn't belong to the current quiz section.", true);
            }

            if (question.Interactions.All(i => i.Id != InteractionId))
            {
                throw new DomainException(ResultCode.InvalidInteractionId, "Invalid InteractionId.", true);
            }


            // save response
            var answer = team.Answers.FirstOrDefault(a => a.QuestionId == QuestionId);
            if (answer == null)
            {
                answer = new Answer(quizSectionId, QuestionId);
                team.Answers.Add(answer);
            }

            answer.SetInteractionResponse(InteractionId, ChoiceOptionIds, Response);
            UnitOfWork.Commit();

            // send a domain event: InteractionResponseAdded, which will be picked up by:
            // - the scoring handler
            // - a client notification handler
            var response = string.IsNullOrEmpty(Response) ? GetChoiceOptionTexts(question, ChoiceOptionIds) : Response;
            await Bus.Publish(
                new InteractionResponseAdded(TeamId, team.Name, quizSectionId, QuestionId, response));
            //answer.Score(question);
        }

        private string GetChoiceOptionTexts(Question question, List<int> choiceOptionIds)
        {
            var choiceOptionTexts = new List<string>();
            foreach (var choiceOptionId in choiceOptionIds)
            {
                choiceOptionTexts.Add(question.Interactions[InteractionId].ChoiceOptions[choiceOptionId].Text);
            }

            return string.Join(", ", choiceOptionTexts);
        }
    }
}