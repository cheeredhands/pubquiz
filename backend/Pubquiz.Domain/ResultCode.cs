namespace Pubquiz.Domain
{
    public enum ResultCode
    {
        /* Error codes */
        InvalidCode,
        TeamNameIsTaken,
        InvalidEntityId,
        QuestionNotInQuiz,
        QuestionNotInCurrentQuizSection,
        InvalidGameStateTransition,
        InvalidUserNameOrPassword,
        QuizMasterUnauthorizedForGame,
        QuizMasterUnauthorizedForQuizItem,
        TeamCantAccessQuizItemOtherThanTheCurrent,
        LobbyUnavailableBecauseOfGameState,
        NoRoleClaimForUser,
        UnauthorizedRole,
        ValidationError,
        GameIsPausedOrFinished,

        /* Success codes */
        Ok,
        ThatsYou,
        LoggedOut
    }
}