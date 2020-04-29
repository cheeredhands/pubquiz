namespace Pubquiz.Domain
{
    public enum ResultCode
    {
        /* Error codes */
        InvalidCode,
        TeamNameIsTaken,
        InvalidTeamId,
        InvalidQuestionId,
        QuestionNotInQuiz,
        QuestionNotInCurrentQuizSection,
        InvalidInteractionId,
        InvalidGameStateTransition,
        InvalidGameId,
        InvalidUserNameOrPassword,
        InvalidUserId,
        QuizMasterUnauthorizedForGame,
        LobbyUnavailableBecauseOfGameState,
        NoRoleClaimForUser,
        NoCurrentGameIdClaimForUser,
        UnauthorizedRole,
        TeamAlreadyLoggedIn,
        ValidationError,
        /* Success codes */
        ThatsYou,
        TeamRegisteredAndLoggedIn,
        LoggedOut,
        TeamRenamed,
        TeamMembersChanged,
        TeamDeleted,
        UserLoggedIn,
        InteractionResponseSubmitted,
        AuthSuccessfullyTested,
        GameStateChanged,
        GameSelected,
    }
}