namespace Pubquiz.Domain
{
    public struct ErrorCodes
    {
        public const int InvalidCode = 1;
        public const int TeamNameIsTaken = 2;
        public const int InvalidTeamId = 3;
        public const int InvalidQuestionId = 4;
        public const int QuestionNotInQuiz = 5;
        public const int QuestionNotInCurrentQuizSection = 6;
        public const int InvalidInteractionId = 7;
        public const int InvalidGameStateTransition = 8;
        public const int InvalidGameId = 9;
        public const int InvalidUserNameOrPassword = 10;
        public const int InvalidUserId = 11;
        public const int QuizMasterUnauthorizedForGame = 12;
        public const int LobbyUnavailableBecauseOfGameState = 13;
        public const int NoRoleClaimForUser = 14;
        public const int NoCurrentGameIdClaimForUser = 15;
        public const int UnauthorizedRole = 16;
        public const int TeamAlreadyLoggedIn = 17;
    }
}