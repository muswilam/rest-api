namespace TodoApp.ErrorCodes
{
    public struct ValidationErrorCode
    {
        public const string Required = "val-100";
        public const string AlreadyExist = "val-101";
        public const string InvalidUserName = "val-102";
        public const string PasswordRequiresDigit = "val-103";
        public const string PasswordRequiresLower = "val-104";
        public const string PasswordRequiresNonAlphanumeric = "val-105";
        public const string PasswordRequiresUniqueChars = "val-106";
        public const string PasswordRequiresUpper = "val-107";
        public const string PasswordTooShort = "val-108";
        public const string InvalidFormat = "val-109";
        public const string NotFound = "val-110";
    }
}