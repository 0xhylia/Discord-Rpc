﻿using DiscordRPC.Message;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using DiscordRPC.Logging;
using DiscordRPC;
using System.Timers;
using System.Net.Http;
using System.Net.Http.Headers;
using Discord_Rpc.App;
using Discord_Rpc.Parsers;
using Discord_Rpc.Version;

namespace Discord_Rpc
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // == Check if the user is running the latest version
            if (!VerionManager.isLatestVersion())
            {
                Console.WriteLine("You are not running the latest version of Discord RPC!");
                Console.WriteLine($"Please update to the latest version at https://github.com/0xhylia/Discord-Rpc/releases/tag/{VerionManager.getLatestVersion()}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(0);
            }

            // == Create the client
            var client = new DiscordRpcClient(ConfigParser.GetConfigValue("client_id"));


            // == Subscribe to some events
            client.OnReady += (sender, msg) =>
            {
                //Create some events so we know things are happening
                Console.WriteLine("Connected to discord with user {0}", msg.User.Username);

            };

            // == Initialize
            client.Initialize();

            // == Set the presence
            client.SetPresence(new RichPresence()
            {
                Details = "At Her Desktop",
                State = $"Just Chilling",
                Timestamps = Timestamps.Now,
                Assets = new Assets()
                {
                    LargeImageKey = "pfp",
                    LargeImageText = "Hylia",
                },
                Buttons = new Button[]
                {
                    new Button() { Label = "Website", Url = "https://hylia.dev/" },
                    new Button() { Label = "Discord Bot", Url = "https://bot.hylia.dev" }
                }
            });

            while (true)
            {
                Thread.Sleep(5000);
                LogActiveAppDetails(client);
            }

        }


        // Define a global variable to store the previous app name
        private static string prevApp = "";

        public static void LogActiveAppDetails(DiscordRpcClient client)
        {
            string app = App.AppManager.getActiveApp(); // This is the app name (e.g. "Code")
            string windowName = App.AppManager.GetActiveAppWindowName(app); // This is the window name (e.g. "Program.cs")
            string imageKey = App.AppManager.getActiveAppImage(app); // This is the image key (e.g. "visual-studio-code")
            string details = App.AppManager.getAppDetails(app); // This is the app detail (e.g. "Writing Code")
            string[] detailsArray = App.AppManager.getAppDetailsArray(app); // This is an array of the app details (e.g. "Writing Code")
            string fullAppName = detailsArray[3]; // This is the full app name (e.g. "Visual Studio Code")


            // check if the app name has changed before calling updatePresence
            if (app != prevApp)
            {
                var newPresence = updatePresence(client, app, windowName, imageKey, details, fullAppName);
                client.SetPresence(newPresence);
            }

            // Store the current app name as the previous app name for the next time the method is called
            prevApp = app;
        }

        public static RichPresence updatePresence(DiscordRpcClient client, string app, string windowName, string imageKey, string details, string fullAppName)
        {
            if (app == "None")
            {
                return new RichPresence()
                {
                    Details = "At Her Desktop",
                    State = $"Just Chilling",
                    Timestamps = Timestamps.Now,
                    Assets = new Assets()
                    {
                        LargeImageKey = "pfp",
                        LargeImageText = "Hylia",
                    },
                    Buttons = new Button[]
                    {
                new Button() { Label = "Website", Url = "https://hylia.dev/" },
                new Button() { Label = "Discord Bot", Url = "https://bot.hylia.dev" }
                    }
                };
            }

            var richPresence = new RichPresence()
            {
                Details = "Using " + fullAppName,
                State = details,
                Timestamps = Timestamps.Now,
                Assets = new Assets()
                {
                    LargeImageKey = "pfp",
                    LargeImageText = "Hylia",
                    SmallImageKey = imageKey,
                    SmallImageText = $"Playing {app}",
                },
                Buttons = new Button[]
                {
            new Button() { Label = "Website", Url = "https://hylia.dev/" },
            new Button() { Label = "Discord Bot", Url = "https://bot.hylia.dev" }
                }
            };

            return richPresence;
        }




    }
}