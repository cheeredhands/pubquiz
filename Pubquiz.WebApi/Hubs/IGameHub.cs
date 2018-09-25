using System.Threading.Tasks;
using Pubquiz.Domain.Models;

namespace Pubquiz.WebApi.Hubs
{
    public interface IGameHub
    {
        /// <summary>
        /// Called when a team registered for a game. Notifies other teams and the quizmaster
        /// within the current game.
        /// </summary>
        /// <param name="team">The team </param>
        /// <returns></returns>
        Task TeamRegisteredAsync(Team team);
        
        /// <summary>
        /// Called when a team updates its name.
        /// </summary>
        /// <param name="team">The team with the new name.</param>
        /// <returns></returns>
        Task TeamNameUpdatedAsync(Team team);
        
        /// <summary>
        /// Called when a team is typing an answer.
        /// </summary>
        /// <param name="team">The current team.</param>
        /// <param name="question">The current question the team is typing an answer for.</param>
        /// <param name="isTyping">true, if the team is typing an answer, otherwise false.</param>
        /// <returns></returns>
        Task TeamIsTypingAsync(Team team, Question question, bool isTyping);
        
        /// <summary>
        /// Called when an answer can not be scored automatically and the quiz master must
        /// score manually.
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        Task AnswerRequiresReviewAsync(Answer answer);
       
        /// <summary>
        /// Called when the quiz master changes the state of the game.
        /// </summary>
        /// <param name="game">The game with the updated <see cref="GameState"/>.</param>
        /// <returns></returns>
        Task GameStateChangedAsync(Game game);
        
        /// <summary>
        /// Called when the quiz master navigates to the another question.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        Task CurrentQuestionIndexChangedAsync(Game game);
        
        /// <summary>
        /// Called when the quiz master releases the scores for the game.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        Task ScoresReleasedAsync(Game game);
    }
}