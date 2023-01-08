namespace Api.Features.V1.Core;

public static class ApiRoutes
{
    public const string Base = "api/v1";

    public static class User
    {
        public const string Create = Base + "/user";

        public const string Me = Base + "/user/me";
    }

    public static class Auth
    {
        public const string SignIn = Base + "/auth";

        public const string RefreshToken = Base + "/auth/refresh";

        public const string SignOut = Base + "/auth/signout";

        public const string SignOutAll = Base + "/auth/signoutall";
    }
}