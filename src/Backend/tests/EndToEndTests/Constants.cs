namespace EndToEndTests;

public static class Constants
{
    public static class Routes
    {
        public static class User
        {
            public const string Base = "/api/v1/user";
            public const string CompleteRegistration = $"{Base}/register";
            public const string CheckEmailAvailability = $"{Base}/check-email-availability";
            public const string CheckUsernameAvailability = $"{Base}/check-availability";
        }
    }
}
