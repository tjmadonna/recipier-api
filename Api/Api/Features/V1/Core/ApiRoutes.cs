namespace Api.Features.V1.Core;

public static class ApiRoutes
{
    public const string Base = "api/v1";

    public static class User
    {
        public const string Create = Base + "/user";

        public const string Me = Base + "/user/me";
    }
}