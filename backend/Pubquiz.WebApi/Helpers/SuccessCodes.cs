namespace Pubquiz.WebApi.Helpers
{
    public struct SuccessCodes
    {
        public const int TeamRegisteredAndLoggedIn = 1;
        public const int LoggedOut = 2;
        public const int TeamRenamed = 4;
        public const int TeamMembersChanged = 5;
        public const int TeamDeleted = 6;
        public const int UserLoggedIn = 7;
        public const int InteractionResponseSubmitted = 8;
        
        public const int AuthSuccesfullyTested = 42;
        
    }
}