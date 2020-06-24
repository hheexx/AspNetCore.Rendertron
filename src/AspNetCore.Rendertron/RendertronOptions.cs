using System;
using System.Collections.Generic;

namespace AspNetCore.Rendertron
{
    public class RendertronOptions
    {
        static string[] BotUserAgents = new string[]
        {
            "W3C_Validator",
            "baiduspider",
            "bingbot",
            "embedly",
            "facebookexternalhit",
            "linkedinbo",
            "outbrain",
            "pinterest",
            "quora link preview",
            "rogerbo",
            "showyoubot",
            "slackbot",
            "twitterbot",
            "vkShare"
        };


        static string[] RenderExtensionBlacklist = new string[]
       {
            "xml",
            "csv",
            "txt",
            "png",
            "jpg",
            "jpeg",
            "js",
            "css"
       };

        public string RendertronUrl { get; set; }
        public string AppProxyUrl { get; set; }
        public List<string> UserAgents { get; set; } = new List<string>(BotUserAgents);
        public List<string> ExtensionBlacklist { get; set; } = new List<string>(RenderExtensionBlacklist);
        public List<string> PathPrefixBlacklist { get; set; } = new List<string>(RenderExtensionBlacklist);
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
        public bool InjectShadyDom { get; set; }
        public bool RenderMobile { get; set; }
        public TimeSpan HttpCacheMaxAge { get; set; } = TimeSpan.Zero;
        public bool AcceptCompression { get; set; }
    }
}
