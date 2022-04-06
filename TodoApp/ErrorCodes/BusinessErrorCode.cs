namespace TodoApp.ErrorCodes
{
    public struct BusinessErrorCode
    {
        public const string UnKnownFail = "bus-100";
        public const string UserNotInRole = "bus-101";
        public const string InvalidLoginCredentials = "bus-102";
        public const string TokenNotExpiredYet = "bus-103";
        public const string TokenNotExist = "bus-104";
        public const string TokenHasBeenUsed = "bus-105";
        public const string TokenHasBeenRevoked = "bus-106";
        public const string TokenDoesNotMatch = "bus-107";
        public const string InvalidToken = "bus-108";
    }
}