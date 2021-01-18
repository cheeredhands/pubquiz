using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests.Notifications
{
    /// <summary>
    /// Notification to overrule or set the score for an interaction in a quiz item.
    /// </summary>
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    [ValidateEntity(EntityType = typeof(Team), IdPropertyName = "TeamId")]
    [ValidateEntity(EntityType = typeof(QuizItem), IdPropertyName = "QuizItemId")]
    public class CorrectInteractionNotification : Notification
    {
        public string ActorId { get; set; }
        public string TeamId { get; set; }
        public string QuizItemId { get; set; }
        public int InteractionId { get; set; }
        public bool Correct { get; set; }

        public CorrectInteractionNotification(IUnitOfWork unitOfWork, IMediator mediator) : base(unitOfWork, mediator)
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

            // save response
            team.Answers.TryGetValue(QuizItemId, out var answer);
            if (answer == null)
            {
                answer = new Answer(quizSectionId, QuizItemId);
                team.Answers.Add(QuizItemId, answer);
            }

            answer.CorrectInteraction(InteractionId, Correct);
            
            await teamCollection.UpdateAsync(team);
            await Mediator.Publish(
                new InteractionCorrected(game.Id, TeamId, QuizItemId, InteractionId, Correct));
        }
    }
}