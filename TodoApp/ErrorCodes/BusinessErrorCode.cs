namespace TodoApp.ErrorCodes
{
    public struct BusinessErrorCode
    {
        public const string UnKnownFail = "bus-100";
        public const string UserNotInRole = "bus-101";
        public const string InvalidLoginCredentials = "bus-102";
    }
}