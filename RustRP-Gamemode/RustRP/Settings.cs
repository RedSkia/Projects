#define CONSOLE_LOGGING




namespace CoreRP
{
    public static class Settings
    {
        public const string Name = "RustRP";
        public const string ConsoleCommand = "rp";


        public static class Images
        {
            internal const string Logo = "https://infinitynet.dk/public/custom/images/logo.png";
        }

        public static class Groups /*Custom group definitions*/
        {
            public readonly static Group Administrator = new Group
            {
                minuimAuthLevel = 2,
                allowedIds = new ulong[] { 76561198066857380, },
                requiredGroups = new string[] { "staff_administrator", },
                requiredPermissions = new string[] { "rustrp.staff.admin", },
                allowedIPs = new string[] { "10.0.0.126" },
            };
            public sealed class Group
            {
                public uint minuimAuthLevel { get; set; }
                public ulong[] allowedIds { get; set; }
                public string[] requiredPermissions { get; set; }
                public string[] requiredGroups { get; set; }
                public string[] allowedIPs { get; set; }
            }
        }
    }
}