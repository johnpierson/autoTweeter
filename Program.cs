using System;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using TweetSharp;

namespace autoTweeter
{
    class Program
    {
        private static readonly IConfiguration Configuration =
            new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

        private static readonly string[] Exclamations = new[]
        {
            "Check it out!",
            "I wrote",
            "ICYMI"
        };

        static void Main(string[] args)
        {
            string lastCommit = string.Empty;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");

                using (var response = client.GetAsync("https://api.github.com/repos/johnpierson/sixtysecondrevit/commits").Result)
                {
                    var json = response.Content.ReadAsStringAsync().Result;

                    dynamic commits = JArray.Parse(json);
                    lastCommit = commits[0].commit.message;
                }
            }

            


            var status = $"new post: https://www.sixtysecondrevit.com/{lastCommit.Replace("Create ", "")}";

            var service = new TwitterService(
                Configuration["twitter_consumer_key"],
                Configuration["twitter_consumer_secret"]
            );
            
            service.AuthenticateWith(
                Configuration["twitter_access_token"], 
                Configuration["twitter_access_token_secret"]
            );

            var result = service.SendTweet(new SendTweetOptions
            {
                Status = status
            });
            
            Console.WriteLine($"Successfully tweeted {result.IdStr}!\r\n{result.Text}");
        }
    }
}