using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Domain.Requests
{
    public class SubmitInteractionResponseNotification : Notification
    {
        public Guid TeamId { get; set; }
        public Guid QuestionId { get; set; }
        public int InteractionId { get; set; }
        public List<int> ChoiceOptionIds { get; set; }
        public string Response { get; set; }

        public SubmitInteractionResponseNotification(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override async Task DoExecute()
        {
            // check valid
            var teamCollection = UnitOfWork.GetCollection<Team>();

            var team = await teamCollection.GetAsync(TeamId);
            if (team == null)
            {
                throw new DomainException(3, "Invalid team id.", false);
            }

            var questionCollection = UnitOfWork.GetCollection<Question>();

            var question = await questionCollection.GetAsync(QuestionId);
            if (question == null)
            {
                throw new DomainException(6, "Invalid question id.", false);
            }

            var gameCollection = UnitOfWork.GetCollection<Game>();
            var game = await gameCollection.GetAsync(team.GameId);
            var quizId = game.QuizId;

            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var quiz = await quizCollection.GetAsync(quizId);

            var quizSectionId = quiz.QuizSections.FirstOrDefault(qs => qs.Questions.Any(q => q.Id == QuestionId))?.Id;
            if (!quizSectionId.HasValue)
            {
                throw new DomainException(8, "This question doesn't belong to the quiz.", true);
            }

            if (game.CurrentQuizSectionId != quizSectionId.Value)
            {
                throw new DomainException(9, "This question doesn't belong to the current quiz section.", true);
            }

            if (question.Interactions.All(i => i.Id != InteractionId))
            {
                throw new DomainException(7, "Invalid interaction id.", false);
            }


            // save response
            var answer = team.Answers.FirstOrDefault(a => a.QuestionId == QuestionId);
            if (answer == null)
            {
                answer = new Answer(quizSectionId.Value, QuestionId);
                answer.InteractionResponses.Add(new InteractionResponse(InteractionId, ChoiceOptionIds, Response));
            }


            // score it
            answer.Score(question);


        }
    }
}