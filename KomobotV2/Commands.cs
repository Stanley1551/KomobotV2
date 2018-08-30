using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;
using DSharpPlus.CommandsNext.Attributes;
using System.IO;
using System.Diagnostics;
using Google.Apis.Calendar.v3;
using KomobotV2.DataAccess;
using Google.Apis.Services;
using Google.Apis.Calendar.v3.Data;
using System.Net;
using KomobotV2.APIResults;
using Newtonsoft.Json;
using KomobotV2.Logger;

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
            await ctx.RespondAsync("Nézd át a doksit: " + Program.config.tutorialdoc);
        }

        [Command("random")]
        [Description("Random számokkal való játszadozás.")]
        public async Task Random(CommandContext ctx, int a, int b)
        {
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
            await ctx.RespondAsync("Egyes.");
        }

        [Command("sessionInfoSub")]
        [Description("Feliratkozás értesitésre, hogy mennyit függtél.")]
        public async Task SessionInfoSub(CommandContext ctx)
        {
            Komobase.SubscribeUser(ctx.User.Username);

            await ctx.RespondAsync("Mostantól küldeni fogom mennyit függtél kedves " + ctx.User.Username + ".");
        }

        [Command("sessionInfoUnsub")]
        [Description("Leiratkozás a privát értesitésekről.")]
        public async Task SessionInfoUnsub(CommandContext ctx)
        {
            Komobase.UnsubscribeUser(ctx.User.Username);

            Program.gameStartedDictionary.Remove(ctx.Member);

            await ctx.RespondAsync("Nem kapsz több infót a játékmenetedről " + ctx.User.Username + ".");
        }




        #endregion

        #region voice commands
        [Command("join")]
        [RequireOwner]
        public async Task Join(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNextClient();

            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc != null)
                throw new InvalidOperationException("Already connected in this guild.");

            var chn = ctx.Member?.VoiceState?.Channel;
            if (chn == null)
                throw new InvalidOperationException("You need to be in a voice channel.");

            vnc = await vnext.ConnectAsync(chn);
            await ctx.RespondAsync("👌");
        }

        [Command("leave")]
        [RequireOwner]
        public async Task Leave(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNextClient();

            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
                throw new InvalidOperationException("Not connected in this guild.");

            vnc.Disconnect();
            await ctx.RespondAsync("👌");
        }

        [Command("play")]
        [RequireOwner]
        public async Task Play(CommandContext ctx, [RemainingText] string file)
        {
            //if(!CheckOwnershipIsTrue(ctx))
            //{
            //    await ctx.RespondAsync("Ehhez nincs jogosultságod bogárka.");
            //    return;
            //}
            file = "komobot_voicelines/" + file + ".mp3";
            var vnext = ctx.Client.GetVoiceNextClient();

            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
                throw new InvalidOperationException("Not connected in this guild.");

            if (!File.Exists(file))
                throw new FileNotFoundException("File was not found.");

            await ctx.RespondAsync("👌");
            await vnc.SendSpeakingAsync(true); // send a speaking indicator

            var psi = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $@"-i ""{file}"" -ac 2 -f s16le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            var ffmpeg = Process.Start(psi);
            var ffout = ffmpeg.StandardOutput.BaseStream;

            var buff = new byte[3840];
            var br = 0;
            while ((br = ffout.Read(buff, 0, buff.Length)) > 0)
            {
                if (br < buff.Length) // not a full sample, mute the rest
                    for (var i = br; i < buff.Length; i++)
                        buff[i] = 0;

                await vnc.SendAsync(buff, 20);
            }

            await vnc.SendSpeakingAsync(false); // we're not speaking anymore
        }

        #endregion

        #region calendar commands
        [Command("addevent")]
        [Description("Esemény hozzáadása.")]
        public async Task AddEvent(CommandContext ctx, string date, params string[] name)
        {
            CalendarCredentials CalCred = new CalendarCredentials();

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = CalCred.credential,
                ApplicationName = CalendarCredentials.ApplicationName,
            });

            Event newEvent = new Event()
            {
                Summary = GetNameFromArray(name),
                Start = new EventDateTime()
                {
                    Date = date
                },
                End = new EventDateTime()
                {
                    Date = date
                },
            };

            EventsResource.InsertRequest request = service.Events.Insert(newEvent, "primary");
            Event createdEvent = request.Execute();
            await ctx.RespondAsync("Az esemény el lett tárolva.");
        }

        //[Command("removeevent")]
        //public async Task RemoveEvent(CommandContext ctx, int id)
        //{

        //}

        [Command("upcoming")]
        [Description("Események listázása.")]
        public async Task Upcoming(CommandContext ctx, int maxRes = 5)
        {
            CalendarCredentials CalCred = new CalendarCredentials();

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = CalCred.credential,
                ApplicationName = CalendarCredentials.ApplicationName,
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = maxRes;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();
            await ctx.RespondAsync("Események a közeljövőben:");
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    string when = eventItem.Start.DateTime.ToString();
                    if (String.IsNullOrEmpty(when))
                    {
                        when = eventItem.Start.Date;
                    }
                    await ctx.RespondAsync(string.Format("{0} ({1})", eventItem.Summary, when));
                }
            }
            else
            {
                await ctx.RespondAsync("Nem találtam jövőbeni eseményt.");
            }
        }

        #endregion

        #region currency commands

        [Command("latest")]
        [Description("Árfolyam-infók.")]
        public async Task Latest(CommandContext ctx, string curr = "EUR")
        {
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
                try
                {

                    StartStopWatch();
                    logger.Info(ctx.User.Username + " called " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".");
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

        #region private methods
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
                    Stream stream = response.GetResponseStream();
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
        #endregion
    }
}
