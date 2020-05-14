export enum ResultCode {
    /* Error codes */
    InvalidCode = 'InvalidCode',
    TeamNameIsTaken = 'TeamNameIsTaken',
    InvalidTeamId = 'InvalidTeamId',
    InvalidQuestionId = 'InvalidQuestionId',
    QuestionNotInQuiz = 'QuestionNotInQuiz',
    QuestionNotInCurrentQuizSection = 'QuestionNotInCurrentQuizSection',
    InvalidInteractionId = 'InvalidInteractionId',
    InvalidGameStateTransition = 'InvalidGameStateTransition',
    InvalidGameId = 'InvalidGameId',
    InvalidUserNameOrPassword = 'InvalidUserNameOrPassword',
    InvalidUserId = 'InvalidUserId',
    QuizMasterUnauthorizedForGame = 'QuizMasterUnauthorizedForGame',
    LobbyUnavailableBecauseOfGameState = 'LobbyUnavailableBecauseOfGameState',
    NoRoleClaimForUser = 'NoRoleClaimForUser',
    NoCurrentGameIdClaimForUser = 'NoCurrentGameIdClaimForUser',
    UnauthorizedRole = 'UnauthorizedRole',
    TeamAlreadyLoggedIn = 'TeamAlreadyLoggedIn',
    InvalidItemNavigation = 'InvalidItemNavigation',
    ValidationError = 'ValidationError',
    /* Success codes */
    Ok = 'Ok',
    ThatsYou = 'ThatsYou',
    LoggedOut = 'LoggedOut'
}
