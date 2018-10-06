using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests
{
    public class SubmitInteractionResponse : Notification
    {
        public Guid TeamId { get; set; }
        public Guid QuestionId { get; set; }
        public int InteractionId { get; set; }
        public List<int> ChoiceOptionIds { get; set; }
        public string Response { get; set; }

        public SubmitInteractionResponse(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
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
                team.Answers.Add(answer);
            }

            answer.SetInteractionResponse(InteractionId, ChoiceOptionIds, Response);
            UnitOfWork.Commit();
            
            // send a domain event: InteractionResponseAdded, which will be picked up by:
            // - the scoring handler
            // - a client notification handler
            var response = string.IsNullOrEmpty(Response) ? GetChoiceOptionTexts(question, ChoiceOptionIds) : Response;
            await Bus.Publish(
                new InteractionResponseAdded(TeamId, team.Name, quizSectionId.Value, QuestionId, response));
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