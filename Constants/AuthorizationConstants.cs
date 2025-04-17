namespace EndavaGrowthspace.Constants
{
    public static class AuthorizationConstants
    {
        public static class Roles
        {
            public const string Admin = "Admin";
            public const string User = "User";
            public const string Administrator = "Administrator";
        }

        public static class Policies
        {
            public const string RequireAdminRole = "RequireAdminRole";
            public const string RequireAdministratorRole = "ReguireAdministratorRole";
        }

        public static class ClaimTypes
        {
            public const string UserId = "uid";
            public const string RefreshToken = "refresh_token";
        }
    }
}
