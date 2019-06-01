using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext.Attributes;
using RestSharp;
using KomobotV2.APIResults;
using KomobotV2.APIResults.Blizzard;
using KomobotV2.APIResults.ClashRoyale;
using KomobotV2.Logger;
using KomobotV2.Enums;
using KomobotV2.DataAccess;
using KomobotCore;
using KomobotCore.APIResults.Twitch;
using KomobotCore.Exceptions;
using System.Collections.Generic;
using Models = TwitchLib.Api.Helix.Models;
using TwitchLib.Api.V5;


namespace KomobotV2
{
    public class Commands
    {
        private static KomoLogger logger = new KomoLogger();
        private static Stopwatch stopwatch = new Stopwatch();

        #region misc
        [Command("doksi")]
        public async Task Doksi(CommandContext ctx)
        {
            logger.Debug(ctx.User.Username + "called Doksi!");
            await ctx.RespondAsync("Nézd át a doksit: " + Program.config.tutorialdoc);
        }

        [Command("random")]
        [Description("Random számokkal való játszadozás.")]
        public async Task Random(CommandContext ctx, int a, int b)
        {
            logger.Debug(ctx.User.Username + "called random!");
            Random random = new System.Random();

            if (!(a < b))
            {
                await ctx.RespondAsync($"Velem te nem packázol!");
                if (a == b)
                {
                    await ctx.RespondAsync("Ez a random számod bogárka: " + a);
                    return;
                }
                else if (a > b)
                {
                    await ctx.RespondAsync($"🎲 Ez a random számod bogárka: {random.Next(b, a)}");
                    return;
                }
            }
            else
            {
                await ctx.RespondAsync($"🎲 Ez a random számod bogárka: {random.Next(a, b)}");
            }
        }

        [Command("hanyas")]
        public async Task Hanyas(CommandContext ctx)
        {
            try
            {
                logger.Debug(ctx.User.Username + "called hanyas!");
                await ctx.RespondAsync("Egyes.");
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        [Command("sessionInfoSub")]
        [Description("Feliratkozás értesitésre, hogy mennyit függtél.")]
        public async Task SessionInfoSub(CommandContext ctx)
        {
            logger.Debug(ctx.User.Username + "called sessionInfoSub!");
            Komobase.SubscribeUser(ctx.User.Username);

            await ctx.RespondAsync("Mostantól küldeni fogom mennyit függtél kedves " + ctx.User.Username + ".");
        }

        [Command("sessionInfoUnsub")]
        [Description("Leiratkozás a privát értesitésekről.")]
        public async Task SessionInfoUnsub(CommandContext ctx)
        {
            logger.Debug(ctx.User.Username + "called sessionInfoUnsub!");
            Komobase.UnsubscribeUser(ctx.User.Username);

            await ctx.RespondAsync("Nem kapsz több infót a játékmenetedről " + ctx.User.Username + ".");
        }




        #endregion

        //#region voice commands
        //[Command("join")]
        //[RequireOwner]
        //public async Task Join(CommandContext ctx)
        //{
        //    var vnext = ctx.Client.GetVoiceNextClient();

        //    var vnc = vnext.GetConnection(ctx.Guild);
        //    if (vnc != null)
        //        throw new InvalidOperationException("Already connected in this guild.");

        //    var chn = ctx.Member?.VoiceState?.Channel;
        //    if (chn == null)
        //        throw new InvalidOperationException("You need to be in a voice channel.");

        //    vnc = await vnext.ConnectAsync(chn);
        //    await ctx.RespondAsync("👌");
        //}

        //[Command("leave")]
        //[RequireOwner]
        //public async Task Leave(CommandContext ctx)
        //{
        //    var vnext = ctx.Client.GetVoiceNextClient();

        //    var vnc = vnext.GetConnection(ctx.Guild);
        //    if (vnc == null)
        //        throw new InvalidOperationException("Not connected in this guild.");

        //    vnc.Disconnect();
        //    await ctx.RespondAsync("👌");
        //}

        //[Command("play")]
        //[RequireOwner]
        //public async Task Play(CommandContext ctx, [RemainingText] string file)
        //{
        //    //if(!CheckOwnershipIsTrue(ctx))
        //    //{
        //    //    await ctx.RespondAsync("Ehhez nincs jogosultságod bogárka.");
        //    //    return;
        //    //}
        //    file = "komobot_voicelines/" + file + ".mp3";
        //    var vnext = ctx.Client.GetVoiceNextClient();

        //    var vnc = vnext.GetConnection(ctx.Guild);
        //    if (vnc == null)
        //        throw new InvalidOperationException("Not connected in this guild.");

        //    if (!File.Exists(file))
        //        throw new FileNotFoundException("File was not found.");

        //    await ctx.RespondAsync("👌");
        //    await vnc.SendSpeakingAsync(true); // send a speaking indicator

        //    var psi = new ProcessStartInfo
        //    {
        //        FileName = "ffmpeg",
        //        Arguments = $@"-i ""{file}"" -ac 2 -f s16le -ar 48000 pipe:1",
        //        RedirectStandardOutput = true,
        //        UseShellExecute = false
        //    };
        //    var ffmpeg = Process.Start(psi);
        //    var ffout = ffmpeg.StandardOutput.BaseStream;

        //    var buff = new byte[3840];
        //    var br = 0;
        //    while ((br = ffout.Read(buff, 0, buff.Length)) > 0)
        //    {
        //        if (br < buff.Length) // not a full sample, mute the rest
        //            for (var i = br; i < buff.Length; i++)
        //                buff[i] = 0;

        //        await vnc.SendAsync(buff, 20);
        //    }

        //    await vnc.SendSpeakingAsync(false); // we're not speaking anymore
        //}

        //#endregion

        //#region calendar commands
        //[Command("addevent")]
        //[Description("Esemény hozzáadása.")]
        //public async Task AddEvent(CommandContext ctx, string date, params string[] name)
        //{
        //    CalendarCredentials CalCred = new CalendarCredentials();

        //    var service = new CalendarService(new BaseClientService.Initializer()
        //    {
        //        HttpClientInitializer = CalCred.credential,
        //        ApplicationName = CalendarCredentials.ApplicationName,
        //    });

        //    Event newEvent = new Event()
        //    {
        //        Summary = GetNameFromArray(name),
        //        Start = new EventDateTime()
        //        {
        //            Date = date
        //        },
        //        End = new EventDateTime()
        //        {
        //            Date = date
        //        },
        //    };

        //    EventsResource.InsertRequest request = service.Events.Insert(newEvent, "primary");
        //    Event createdEvent = request.Execute();
        //    await ctx.RespondAsync("Az esemény el lett tárolva.");
        //}

        ////[Command("removeevent")]
        ////public async Task RemoveEvent(CommandContext ctx, int id)
        ////{

        ////}

        //[Command("upcoming")]
        //[Description("Események listázása.")]
        //public async Task Upcoming(CommandContext ctx, int maxRes = 5)
        //{
        //    CalendarCredentials CalCred = new CalendarCredentials();

        //    var service = new CalendarService(new BaseClientService.Initializer()
        //    {
        //        HttpClientInitializer = CalCred.credential,
        //        ApplicationName = CalendarCredentials.ApplicationName,
        //    });

        //    // Define parameters of request.
        //    EventsResource.ListRequest request = service.Events.List("primary");
        //    request.TimeMin = DateTime.Now;
        //    request.ShowDeleted = false;
        //    request.SingleEvents = true;
        //    request.MaxResults = maxRes;
        //    request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

        //    // List events.
        //    Events events = request.Execute();
        //    await ctx.RespondAsync("Események a közeljövőben:");
        //    if (events.Items != null && events.Items.Count > 0)
        //    {
        //        foreach (var eventItem in events.Items)
        //        {
        //            string when = eventItem.Start.DateTime.ToString();
        //            if (String.IsNullOrEmpty(when))
        //            {
        //                when = eventItem.Start.Date;
        //            }
        //            await ctx.RespondAsync(string.Format("{0} ({1})", eventItem.Summary, when));
        //        }
        //    }
        //    else
        //    {
        //        await ctx.RespondAsync("Nem találtam jövőbeni eseményt.");
        //    }
        //}

        //#endregion

        #region currency commands

        [Command("latest")]
        [Description("Árfolyam-infók.")]
        public async Task Latest(CommandContext ctx, string curr = "EUR")
        {
            logger.Debug(ctx.User.Username + "called latest!");
            FixerLatestResult result;

            if (!ValidateCurrency(curr))
            {
                await ctx.RespondAsync("Nem megfelelő valutát irtál bázisnak bogárka...");
                return;
            }

            using (var webClient = new WebClient())
            {
                string url = Program.config.fixerPrefix + ctx.Command.Name + "?access_key=" + Program.config.fixerAPIKey + "&base=" + curr + "&symbols=HUF,EUR,GBP,USD";

                var response = webClient.DownloadString(new Uri(url));

                result = new FixerLatestResult(response);
            }

            await ctx.RespondAsync("1 EUR: \n" + result.HUF + " HUF\n" + result.USD + " USD\n" + result.GBP + " GBP.");
        }

        #endregion

        #region clash royale commands
        [Group("CR")]
        [Description("Clash Royale related commands.")]
        public class CRGroupCommands
        {
            [Command("getTag")]
            [Description("Visszaadja a Clash Royale Tag-edet.")]
            public async Task GetCRName(CommandContext ctx)
            {
                logger.Debug(ctx.User.Username + "called CR getTag!");
                var retVal = Komobase.GetCRTag(ctx.User.Username);

                if (string.IsNullOrEmpty(retVal))
                {
                    await ctx.RespondAsync("Nem találni a naplóban Clash Royale Tag-et! A setTag paranccsal beállithatod.");
                }
                else
                {
                    await ctx.RespondAsync("A te Clash Royale tag-ed: " + retVal + ".");
                }

            }

            [Command("setTag")]
            [Description("Beállitom a Clash Royale tag-edet.")]
            public async Task SetCRTag(CommandContext ctx, string tag = null)
            {
                logger.Debug(ctx.User.Username + "called CR setTag!");
                if (ValidateID(tag))
                {
                    Komobase.SetCRTag(ctx.User.Username, tag.ToUpper());

                    await ctx.RespondAsync("Bevéstem a naplóba a Clash Royale tagedet " + ctx.User.Username + ".");
                }
            }

            [Command("Wins")]
            [Description("Az eddigi Clash Royale win-ed száma.")]
            public async Task GetCRWins(CommandContext ctx, string tag = null)
            {
                logger.Debug(ctx.User.Username + "called CR Wins!");
                var result = await GetCRPlayerData(ctx.User.Username, tag);

                //TODO add to cache

                if (result == null || result.wins == 0)
                {
                    await ctx.RespondAsync("Tag nélkül nem megy bogárka! Add meg paraméterként, vagy irjuk be a naplóba!");
                    return;
                }

                await ctx.RespondAsync("Hát neked bizony " + result.wins + " wined van!");
            }

            [Command("Trophy")]
            [Description("Jelenlegi trófeáid.")]
            public async Task GetCRTrophies(CommandContext ctx, string tag = null)
            {
                logger.Debug(ctx.User.Username + "called CR Trophy!");
                var result = await GetCRPlayerData(ctx.User.Username, tag);

                //TODO add to cache

                if(result == null || result.trophies == 0)
                {
                    await ctx.RespondAsync("Tag nélkül nem megy bogárka! Add meg paraméterként, vagy irjuk be a naplóba!");
                    return;
                }

                await ctx.RespondAsync("Azta! " + result.trophies + " trófeán vagy. A legtöbb " + result.bestTrophies + " volt.");
            }

            [Command("Info")]
            [Description("Általános Clash Royale információk.")]
            public async Task GetCRInfo(CommandContext ctx, string tag = null)
            {
                logger.Debug(ctx.User.Username + "called CR Info!");
                try
                {

                    StartStopWatch();
                    //logger.Info(ctx.User.Username + " called " + System.Reflection.MethodBase.GetCurrentMethod(). + ".");
                    var result = await GetCRPlayerData(ctx.User.Username, tag);

                    //TODO add to cache

                    if (result == null || result.trophies == 0)
                    {
                        await ctx.RespondAsync("Tag nélkül nem megy bogárka! Add meg paraméterként, vagy irjuk be a naplóba!");
                        return;
                    }

                    StringBuilder builder = new StringBuilder();
                    builder = builder.AppendLine("Tag: " + result.tag);
                    builder = builder.AppendLine("Név: " + result.name);
                    builder = builder.AppendLine("Clan: " + result.clan.name);
                    builder = builder.AppendLine("Trófeák: " + result.trophies);
                    builder = builder.AppendLine("Winek: " + result.wins);
                    builder = builder.AppendLine("Szint: " + result.expLevel);

                    await ctx.RespondAsync(builder.ToString());
                    TimeSpan elapsed = StopStopWatch();
                    logger.Info("GetCRInfo responded with: " + builder.ToString() + "after " + elapsed.ToString());
                } catch (Exception e) { logger.Error("GetCRInfo failed", e); }
            }
        }

        #endregion

        #region PUBG commands

        [Command("getPUBGID")]
        public async Task GetPUBGID(CommandContext ctx)
        {
            var retVal = Komobase.GetPUBGID(ctx.User.Username);

            if (string.IsNullOrEmpty(retVal))
            {
                await ctx.RespondAsync("Nem találni a naplóban a PUBG ID-dat! A setPUBGID paranccsal beállithatod.");
            }
            else
            {
                await ctx.RespondAsync("A te PUBG ID-d: " + retVal + ".");
            }

        }

        [Command("setPUBGID")]
        public async Task SetPUBGID(CommandContext ctx, string id = null)
        {
            if (ValidateID(id))
            {
                Komobase.SetPUBGID(ctx.User.Username, id);

                await ctx.RespondAsync("Beirtam a naplóba a PUBG ID-dat " + ctx.User.Username + ".");
            }
        }

        #endregion

        #region Joke commands
        [Command("vicc")]
        [RequireOwner]
        public async Task Joke(CommandContext ctx)
        {
            JokeResult jokeResult;

            using (var webClient = new WebClient())
            {
                string url = GenerateAPIURL(Program.config.jokesPrefix);

                var response = webClient.DownloadString(new Uri(url));

                jokeResult = new JokeResult(response);
            }

            await ctx.RespondAsync(jokeResult.Joke);
        }

        #endregion

        #region Football data commands
        [Command("Standing")]
        [Description("Foci bajnokságok jelenlegi állása.")]
        public async Task GetPLStanding(CommandContext ctx, string league = "")
        {
            logger.Debug(ctx.User.Username + "called Standing!");
            if (league == string.Empty)
            {
                await ctx.RespondAsync("Paraméterként add meg a liga nevét bogárka!", false, null);
                await ctx.RespondAsync("Elérhető bajnokságok: " + GetLeagueNames());
                return;
            }

            if(!Enum.GetNames(typeof(FootballLeagues)).Any(x=> x.Equals(league)))
            {
                await ctx.RespondAsync("Ejha, a liga neve hibádzik!", false, null);
                await ctx.RespondAsync("Elérhető bajnokságok: " + GetLeagueNames());
                return;
            }

            var id = Enum.Parse(typeof(FootballLeagues), league);

            string url = Program.config.footballDataEndpoint + "competitions/"+(int) id+"/standings";

            HttpWebRequest request = HttpWebRequest.CreateHttp(url);
            request.Headers.Set("X-Auth-Token", Program.config.footballDataKey);

            var response = (HttpWebResponse)await request.GetResponseAsync();

            var code = response.StatusCode;

            if (HttpStatusCode.OK == code)
            {
                System.IO.Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                string resultString = await reader.ReadToEndAsync();

                var result = JsonConvert.DeserializeObject<FootballDataStandingsResult>(resultString);

                StringBuilder builder = new StringBuilder();
                builder.AppendLine(id.ToString()+" tabella:");

                foreach(var standing in result.standings.FirstOrDefault().table)
                {
                    builder.AppendLine(standing.position + ". " + standing.team.name+"\t "+standing.points+" pont, "+standing.playedGames+" meccs");
                }

                await ctx.RespondAsync(builder.ToString());
            }
        }
        #endregion

        #region Blizzard commands
        [Group("WoW")]
        [Description("WoW related commands")]
        public class BlizzardGroupedCommands
        {
            [Command("setName")]
            public async Task SetWowRealmAndName(CommandContext ctx, string realm, string name)
            {
                logger.Debug(ctx.User.Username + "called wow setName!");
                Komobase.SetWowRealmAndName(ctx.User.Username, realm, name);

                await ctx.RespondAsync("Beirtam a naplóba a realmodat és a karaktered nevét!");
            }

            [Command("KarakterInfo")]
            [Description("Standard character info.")]
            public async Task GetCharInfo(CommandContext ctx, string server, string name)
            {
                logger.Debug(ctx.User.Username + "called WoW KarakterInfo!");
                var client = await ConstructBlizzardCharClient(server, name);

                var resp = await client.ExecuteTaskAsync(new RestRequest());
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    CharInfoResponse response = JsonConvert.DeserializeObject<CharInfoResponse>(resp.Content);

                    DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                    {
                        Title = response.name,
                        ImageUrl = @"http://render-eu.worldofwarcraft.com/character/" + response.thumbnail,
                        Description = "level " + response.level + " " + Enum.GetName(typeof(Gender), response.gender) + " " + Enum.GetName(typeof(Race), response.race) + " " +
                        Enum.GetName(typeof(@class), response.@class),
                        Url = "https://worldofwarcraft.com/en-gb/character/" + server + "/" + name,
                        Color = response.faction == 0 ? DiscordColor.Blue : DiscordColor.Red
                    };
                    builder.AddField("Honorable kills:", response.totalHonorableKills.ToString());
                    builder.AddField("Achievement points:", response.achievementPoints.ToString());

                    await ctx.RespondAsync(null, false, builder.Build());
                    return;
                }
                await ctx.RespondAsync("Nocsak! Ilyen karaktert nem találni!");
            }

            //[Command("KarakterInfo")]
            //[Description("Standard character info.")]
            //public async Task GetCharInfo(CommandContext ctx)
            //{
            //    Komobase.GetWowRealmAndName(ctx.User.Username);
            //}

            [Command("MennyiMount")]
            [Description("Sum of the number of aquired mounts.")]
            public async Task GetMounts(CommandContext ctx, string server, string name)
            {
                logger.Debug(ctx.User.Username + "called wow mennyimount!");
                var client = await ConstructBlizzardCharClient(server, name, new Parameter("fields", "mounts", ParameterType.QueryString));

                var response = await client.ExecuteTaskAsync(new RestRequest());
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    CharInfoWithMountResponse resp = JsonConvert.DeserializeObject<CharInfoWithMountResponse>(response.Content);

                    int numberOfMounts = resp.mounts.numCollected;

                    await ctx.RespondAsync("Ejha! " + resp.name + " " + numberOfMounts + " mounttal rendelkezik!");
                    return;
                }
                await ctx.RespondAsync("Nocsak! Ilyen karaktert nem találni!");
            }

            [Command("Feed")]
            [Description("Recent activity.")]
            public async Task GetFeed(CommandContext ctx, string server, string name, int count = 5, string filter = "")
            {
                logger.Debug(ctx.User.Username + "called "+ ctx.Command.Name);
                var client = await ConstructBlizzardCharClient(server, name, new Parameter("fields", "feed", ParameterType.QueryString));

                var response = await client.ExecuteTaskAsync(new RestRequest());
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    CharInfoWithFeedResponse resp = JsonConvert.DeserializeObject<CharInfoWithFeedResponse>(response.Content);

                    //elvileg a most recent az első és sorban van
                    filter = filter.ToUpper();
                    if(filter == "BOSSKILL" || filter == "ACHIEVEMENT" || filter == "LOOT")
                    {
                        resp.feed = (from feeds in resp.feed
                                    where feeds.type == filter
                                    select feeds).ToList();
                    }
                    var requiredRows = resp.feed.Take(count > resp.feed.Count ? resp.feed.Count : count);

                    string responseRows = string.Empty;

                    foreach (Feed feed in requiredRows)
                    {
                        string row = ConstructFeedStringDependingOnType(feed);
                        responseRows += row + "\n";
                    }

                    await ctx.RespondAsync(responseRows, false, null);
                    return;
                }

                await ctx.RespondAsync("Nocsak! Ilyen karaktert nem találni!");
            }

            [Command("Exa")]
            [Description("Number of exalted reputations.")]
            public async Task GetExaltedNumber(CommandContext ctx, string server, string name)
            {
                logger.Debug(ctx.User.Username + "called " + ctx.Command.Name);
                var client = await ConstructBlizzardCharClient(server, name, new Parameter("fields", "reputation", ParameterType.QueryString));

                var response = await client.ExecuteTaskAsync(new RestRequest());
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var replist = JsonConvert.DeserializeObject<CharInfoWithRepu>(response.Content).reputation;

                    var numberOfExas = replist.Where(x => x.standing == 7).Count();

                    await ctx.RespondAsync(name + " karakteren jelenleg " + numberOfExas + " exalted repu van!");
                    return;
                }
                await ctx.RespondAsync("Nocsak! Ilyen karaktert nem találni!");
            }
        }
        #endregion

        #region Twitch commands
        [Group("Twitch")]
        [Description("Twitch related commands")]
        public class TwitchGroupedCommands
        {
            [Command("online")]
            [Description("Megnézzük, online-e a stream.")]
            public async Task GetStreamOnline(CommandContext ctx, string channel)
            {
                try
                {
                    var response = await RetrieveTwitchChannelInfo(channel);

                    //elvégre csak 1et kérünk vissza...
                    var stream = response.Streams.FirstOrDefault();

                    if(stream is null)
                    {
                        await ctx.RespondAsync(channel + " jelenleg nem streamel!");
                    }
                    else
                    {
                        await ctx.RespondAsync(channel + " most online!" + GetTimeLapsedFormattedString(stream.StartedAt) + " streamel. " +
                        "Streamjét jelenleg " + stream.ViewerCount + " ember nézi.");
                    }
                    
                }
                catch(TwitchStreamNotFoundException) { await ctx.RespondAsync("Nem találni ilyen streamet!"); }
                catch(Exception e) { await ctx.RespondAsync("Valami hiba történt!"); logger.Error("GetStreamOnline", e); }
            }
        }
        #endregion

        #region private methods
        private static string GetTimeLapsedFormattedString(DateTime startedAt)
        {
            var timeLapsed = DateTime.UtcNow - startedAt.ToUniversalTime();
            return timeLapsed.Hours + " órája, " + timeLapsed.Minutes + " perce";
        }

        private static string ConstructFeedStringDependingOnType(Feed feed)
        {
            string retVal = GetDateTimeFromTimeStamp(long.Parse(feed.timestamp.ToString())).ToString()+" ";
            switch (feed.type)
            {
                case "BOSSKILL":
                    retVal += "Legyőzte " + feed.achievement.title + "-t (" + feed.quantity + ". alkalommal)";
                    break;
                case "LOOT":
                    retVal += "Lootolta a " + feed.itemId + " itemID-s itemet.";
                    break;
                case "ACHIEVEMENT":
                    retVal += "Megszerezte a " + feed.achievement.title + " achit! (" + feed.achievement.points + " pont)";
                    break;
                
            }

            return retVal;
        }

        private async static Task<Models.Streams.GetStreamsResponse> RetrieveTwitchChannelInfo(string channel)
        {
            //Előbb kell ID, mert csak azzal megy a hivás...
            var response = await Program.API.V5.Users.GetUserByNameAsync(channel);
            if(response.Total == 0)
            {
                throw new TwitchStreamNotFoundException("The GetUserByNameAsync returned with no data.");
            }

            var userid = response.Matches[0].Id;

            return await Program.API.Helix.Streams.GetStreamsAsync(null,null,1,null,null,"all",new List<string>() { userid },null);
        }

        private async static Task<RestClient> ConstructBlizzardCharClient(string server, string name, params Parameter[] parameters)
        {
            string token = await RetrieveAuthToken();

            string url = Program.config.blizzardCharInfoEndpoint + @"/" + server + @"/" + name;
            RestClient client = new RestClient(url);
            client.AddDefaultParameter(new Parameter("locale", "en_US", ParameterType.QueryString));
            client.AddDefaultParameter(new Parameter("access_token", token, ParameterType.QueryString));

            foreach(var parameter in parameters)
            {
                client.AddDefaultParameter(parameter);
            }

            return client;
        }

        private static async Task<string> RetrieveAuthToken()
        {
            var tokenFromDB = Komobase.GetAuthToken();

            if(tokenFromDB == string.Empty)
            {
                string token = await GetAuthTokenFromBlizzard();
                Komobase.SetAuthToken(token);
                return token;
            }

            if(await ValidateToken(tokenFromDB) != true)
            {
                string token = await GetAuthTokenFromBlizzard();
                Komobase.SetAuthToken(token);
                return token;
            }

            return tokenFromDB;
        }

        private static async Task<bool> ValidateToken(string token)
        {
            string resultString = string.Empty;

            RestClient client = new RestClient(Program.config.blizzardOauthCheckTokenEndpoint);
            RestRequest request = new RestRequest(Program.config.blizzardOauthCheckTokenEndpoint,Method.GET,DataFormat.Json);
            //ez parameter is a faszért van elírva a dokumentációban
            request.AddParameter("token",token);
            var response = await client.ExecuteTaskAsync(request);

            if(response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        private static async Task<string> GetAuthTokenFromBlizzard()
        {
            var url = Program.config.blizzardOauthAccessTokenEndpoint;

            var client = new RestClient(url);
            client.AddDefaultParameter("grant_type", "client_credentials");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", Program.config.client_id);
            request.AddParameter("client_secret", Program.config.client_secret);

            var respone = await client.ExecuteTaskAsync(request);

            return JsonConvert.DeserializeObject<AccessTokenResponse>(respone.Content).access_token;
        }

        private string GetLeagueNames()
        {
            var retVal = string.Empty;

            foreach (string name in Enum.GetNames(typeof(FootballLeagues)))
            {
                retVal += name + " ";
            }

            return retVal;
        }

        private bool CheckOwnershipIsTrue(CommandContext ctx)
        {
            if (ctx != null && ctx.Member != null)
            {
                if (ctx.Member.IsOwner)
                    return true;
            }

            return false;
        }

        private string GenerateAPIURL(string @base, string key = null, params string[] flags)
        {
            StringBuilder sb = new StringBuilder();
            sb = sb.Append(@base);
            if (key != null)
            {
                sb = sb.Append(key);
            }
            if (flags != null && flags.Count() > 0)
            {
                foreach (string flag in flags)
                    sb = sb.Append(flag);
            }

            return sb.ToString();
        }

        private bool ValidateCurrency(string curr)
        {
            if (curr == "EUR" || curr == "USD" || curr == "GBP")
                return true;
            return false;
        }

        private string GetNameFromArray(string[] array)
        {
            string retVal = string.Empty;

            if (array == null)
            {
                throw new ArgumentNullException();
            }
            foreach (string element in array)
            {
                retVal += element + " ";
            }

            return retVal;
        }
        //TODO
        private static bool ValidateID(string id)
        {
            if (id == null || id == string.Empty || !(id.Length > 2))
            {
                return false;
            }
            return true;
        }
        //TODO
        private static bool ValidateCRTag(string tag)
        {
            if (tag == null || tag == string.Empty || !(tag.Length > 2))
            {
                return false;
            }
            return true;
        }

        private static string URLEncodeCRTag(string tag)
        {
            return tag.Replace("#", "%23");
        }

        private static async Task<PlayerResult> GetCRPlayerData(string userName, string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                tag = Komobase.GetCRTag(userName).ToUpper();
            }
            if (string.IsNullOrEmpty(tag))
            {

                return new PlayerResult();
            }

            string url = Program.config.crEndpoint + "players/" + URLEncodeCRTag(tag);
            try
            {
                HttpWebRequest request = HttpWebRequest.CreateHttp(url);
                request.Method = "GET";
                request.Accept = "application/json";
                request.Headers.Set(HttpRequestHeader.Authorization, "Bearer " + Program.config.crAPIKEY);

                var response = (HttpWebResponse)await request.GetResponseAsync();

                var code = response.StatusCode;

                if (HttpStatusCode.OK == code)
                {
                    System.IO.Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);

                    string resultString = await reader.ReadToEndAsync();

                    return JsonConvert.DeserializeObject<PlayerResult>(resultString);
                }
            }
            catch (Exception e) { throw new Exception(e.Message, e.InnerException); }

            return new PlayerResult();
        }

        private static void StartStopWatch()
        {
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }

            stopwatch.Reset();

            stopwatch.Start();
        }

        private static TimeSpan StopStopWatch()
        {
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }

            TimeSpan retVal = stopwatch.Elapsed;

            stopwatch.Reset();

            return retVal;
        }

        private static DateTime GetDateTimeFromTimeStamp(long timestamp)
        {
            var epoch = new DateTime(1970, 1, 1, 1, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(timestamp);
        }
        #endregion

    }
}
