namespace WebSite.Extensions
{
    public static class UserAgentExtensions
    {
        private static readonly List<string> BotUserAgents = new List<string>
        {
            "Googlebot",
            "Baiduspider",
            "ia_archiver",
            "R6_FeedFetcher",
            "NetcraftSurveyAgent",
            "Sogou web spider",
            "bingbot",
            "Yahoo! Slurp",
            "facebookexternalhit",
            "PrintfulBot",
            "msnbot",
            "Twitterbot",
            "UnwindFetchor",
            "urlresolver",
            "Unknown"
        };

        public static bool IsHumanUser(this string? userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
            {
                return false;
            }

            return !BotUserAgents.Any(agent =>
                userAgent.IndexOf(agent, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public static bool IsBot(this string? userAgent)
        {
            return !userAgent.IsHumanUser();
        }
    }
}