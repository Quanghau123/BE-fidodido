namespace FidoDino.Common.Authorization
{
    public static class Permissions
    {
        public const string User_View = "user.view";
        public const string User_Create = "user.create";
        public const string User_Update = "user.update";
        public const string User_Delete = "user.delete";
        public const string User_Export = "user.export";

        public static readonly List<string> All =
        [
            User_View,
            User_Create,
            User_Update,
            User_Delete,
            User_Export
        ];
    }
}
