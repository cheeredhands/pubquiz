namespace Pubquiz.Domain
{
    public enum ResultCode
    {
        /* Error codes */
        InvalidCode,
        TeamNameIsTaken,
        InvalidTeamId,
        InvalidQuizItemId,
        InvalidQuizId,
        QuestionNotInQuiz,
        QuestionNotInCurrentQuizSection,
        InvalidInteractionId,
        InvalidGameStateTransition,
        InvalidGameId,
        InvalidUserNameOrPassword,
        InvalidUserId,
        QuizMasterUnauthorizedForGame,
        QuizMasterUnauthorizedForQuizItem,
        TeamCantAccessQuizItemOtherThanTheCurrent,
        LobbyUnavailableBecauseOfGameState,
        NoRoleClaimForUser,
        // NoCurrentGameIdClaimForUser,
        UnauthorizedRole,
        TeamAlreadyLoggedIn,
        ValidationError,

        /* Success codes */
        Ok,
        ThatsYou,
        LoggedOut
    }
}