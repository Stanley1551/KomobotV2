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
using KomobotV2.Logger;
using KomobotCore;
using Currency;
using Unity;
using QrCodeCreation;
using Football;
using Strava;
using KomoBase;
using Wow;
using Twitch;
using Wow.Enums;
using System.Threading;
using Wow.Responses;

namespace KomobotV2
{
    public class Commands
    {
        private static KomoLogger logger = new KomoLogger();
        private static Stopwatch stopwatch = new Stopwatch();

        #region misc
        [Command("echo")]
        public async Task Echo(CommandContext ctx, string text)
        {
            logger.Debug(ctx.User.Username + " called Echo!");
            await ctx.RespondAsync(text, true);
        }

        [Command("Sorfeles")]
        public async Task Sorfeles(CommandContext ctx, int interval)
        {
            logger.Debug(ctx.User.Username + " called Sorfeles!");

            await Task.Run(async () =>
           {
               for (int i = 0; i < 10; i++)
               {
                   Thread.Sleep(interval * 1000);
                   await ctx.RespondAsync("Drink!", true);
               }
           });
        }

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
            logger.Debug(ctx.User.Username + " called sessionInfoSub!");
            using (KomoBaseAccess kba = new KomoBaseAccess())
            {
                kba.UpdateSubscription(ctx.User.Username, true);
            }

            await ctx.RespondAsync("Mostantól küldeni fogom mennyit függtél kedves " + ctx.User.Username + ".");
        }

        [Command("sessionInfoUnsub")]
        [Description("Leiratkozás a privát értesitésekről.")]
        public async Task SessionInfoUnsub(CommandContext ctx)
        {
            logger.Debug(ctx.User.Username + " called sessionInfoUnsub!");
            using (KomoBaseAccess kba = new KomoBaseAccess())
            {
                kba.UpdateSubscription(ctx.User.Username, false);
            }

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



        #region currency commands

        [Command("latest")]
        [Description("Árfolyam-infók.")]
        public async Task Latest(CommandContext ctx, string curr = "EUR")
        {
            logger.Debug(ctx.User.Username + "called latest!");

            var service = Program.Container.Resolve<ICurrencyService>();

            if(!service.Validate(curr))
            {
                await ctx.RespondAsync("Nem megfelelő valutát irtál bázisnak bogárka...");
                return;
            }
            string url = Program.config.fixerPrefix + ctx.Command.Name + "?access_key=" + Program.config.fixerAPIKey + "&base=" + curr + "&symbols=HUF,EUR,GBP,USD";

            var result = await service.GetCurrencyResultsAsync(new Uri(url));

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
                logger.Debug(ctx.User.Username + " called CR getTag!");

                string tag = string.Empty;

                using (KomoBaseAccess kba = new KomoBaseAccess())
                {
                    tag = kba.GetCRTag(ctx.User.Username);
                }

                if (string.IsNullOrEmpty(tag))
                {
                    await ctx.RespondAsync("Nem találni a naplóban Clash Royale Tag-et! A setTag paranccsal beállithatod.");
                }
                else
                {
                    await ctx.RespondAsync("A te Clash Royale tag-ed: " + tag + ".");
                }

            }

            [Command("setTag")]
            [Description("Beállitom a Clash Royale tag-edet.")]
            public async Task SetCRTag(CommandContext ctx, string tag = null)
            {
                logger.Debug(ctx.User.Username + "called CR setTag!");
                if (ValidateID(tag))
                {
                    using (KomoBaseAccess kba = new KomoBaseAccess())
                    {
                        kba.UpdateCRTag(ctx.User.Username, tag);
                    }

                    await ctx.RespondAsync("Bevéstem a naplóba a Clash Royale tagedet " + ctx.User.Username + ".");
                }
            }

            //[Command("Wins")]
            //[Description("Az eddigi Clash Royale win-ed száma.")]
            //public async Task GetCRWins(CommandContext ctx, string tag = null)
            //{
            //    logger.Debug(ctx.User.Username + " called CR Wins!");
            //    int result = 0;
            //    if(tag == null)
            //    {
            //        using (KomoBaseAccess kba = new KomoBaseAccess())
            //        {
            //            tag = kba.GetCRTag(ctx.User.Username);
            //        }
            //    }

            //    if(tag == string.Empty)
            //    {
            //        await ctx.RespondAsync("Tag nélkül nem megy bogárka! Add meg paraméterként, vagy irjuk be a naplóba!");
            //        return;
            //    }
                
            //    var service = Program.Container.Resolve<IClashRoyaleService>();
            //    result = await service.GetWins(tag, Program.config.crEndpoint, Program.config.crAPIKEY);

            //    await ctx.RespondAsync("Hát neked bizony " + result + " wined van!");
            //}

            //[Command("Trophy")]
            //[Description("Jelenlegi trófeáid.")]
            //public async Task GetCRTrophies(CommandContext ctx, string tag = null)
            //{
            //    logger.Debug(ctx.User.Username + "called CR Trophy!");
            //    int result = 0;
            //    if (tag == null)
            //    {
            //        using (KomoBaseAccess kba = new KomoBaseAccess())
            //        {
            //            tag = kba.GetCRTag(ctx.User.Username);
            //        }
            //    }

            //    if (tag == string.Empty)
            //    {
            //        await ctx.RespondAsync("Tag nélkül nem megy bogárka! Add meg paraméterként, vagy irjuk be a naplóba!");
            //        return;
            //    }

            //    var service = Program.Container.Resolve<IClashRoyaleService>();
            //    result = await service.GetWins(tag, Program.config.crEndpoint, Program.config.crAPIKEY);

            //    await ctx.RespondAsync("Hát neked bizony " + result + " wined van!");
            //}

            //[Command("Info")]
            //[Description("Általános Clash Royale információk.")]
            //public async Task GetCRInfo(CommandContext ctx, string tag = null)
            //{
            //    logger.Debug(ctx.User.Username + " called CR Info!");

            //    string result = string.Empty;
            //    if (tag == null)
            //    {
            //        using (KomoBaseAccess kba = new KomoBaseAccess())
            //        {
            //            tag = kba.GetCRTag(ctx.User.Username);
            //        }
            //    }

            //    if (tag == string.Empty)
            //    {
            //        await ctx.RespondAsync("Tag nélkül nem megy bogárka! Add meg paraméterként, vagy irjuk be a naplóba!");
            //        return;
            //    }

            //    var service = Program.Container.Resolve<IClashRoyaleService>();
            //    result = await service.GetInfo(tag, Program.config.crEndpoint, Program.config.crAPIKEY);

            //    await ctx.RespondAsync("Hát neked bizony " + result + " wined van!");
            //}
        }

        #endregion

        #region Football data commands
        [Command("Standing")]
        [Description("Foci bajnokságok jelenlegi állása.")]
        public async Task GetLeagueStanding(CommandContext ctx, string league = "")
        {
            logger.Debug(ctx.User.Username + "called Standing!");
            var service = Program.Container.Resolve<IFootballDataService>();

            if (league == string.Empty)
            {
                await ctx.RespondAsync("Paraméterként add meg a liga nevét bogárka!", false, null);
                await ctx.RespondAsync("Elérhető bajnokságok: " + service.GetLeagueNames());
                return;
            }

            var validLeagueName = service.ValidateLeagueName(league);
            if(!validLeagueName)
            {
                await ctx.RespondAsync("Ejha! A liga neve hibádzik!");
                return;
            }
            try
            {
                var result = await service.GetLeagueStanding(league, Program.config.footballDataEndpoint, Program.config.footballDataKey);
                await ctx.RespondAsync(result);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                await ctx.RespondAsync("Hiba történt az adatok lekérésében!");
            }

            
        }
        #endregion

        #region Blizzard commands
        [Group("WoW")]
        [Description("WoW related commands")]
        public class BlizzardGroupedCommands
        {
            [Command("setName")]
            public async Task SetWowName(CommandContext ctx, string name)
            {
                logger.Debug(ctx.User.Username + " called wow setName!");
                try
                {
                    using (KomoBaseAccess kba = new KomoBaseAccess())
                    {
                        kba.UpdateWowCharName(ctx.User.Username, name);
                    }
                    await ctx.RespondAsync("Beirtam a naplóba a karaktered nevét!");
                }
                catch (Exception) { await ctx.RespondAsync("Valami hiba történt!"); }
            }

            [Command("setRealm")]
            public async Task SetWowRealm(CommandContext ctx, string realm)
            {
                logger.Debug(ctx.User.Username + " called SetWowRealm!");
                try
                {
                    using (KomoBaseAccess kba = new KomoBaseAccess())
                    {
                        kba.UpdateWowRealm(ctx.User.Username, realm);
                    }
                    await ctx.RespondAsync("Beirtam a naplóba a realmodat!");
                }
                catch (Exception) { await ctx.RespondAsync("Valami hiba történt!"); }
            }

            [Command("setRealmCharname")]
            public async Task SetWowRealmAndName(CommandContext ctx, string realm, string charname)
            {
                logger.Debug(ctx.User.Username + " called setRealmCharname!");
                try
                {
                    using (KomoBaseAccess kba = new KomoBaseAccess())
                    {
                        kba.UpdateWowRealm(ctx.User.Username, realm);
                        kba.UpdateWowCharName(ctx.User.Username, charname);
                    }
                    await ctx.RespondAsync("Beirtam a naplóba a realmodat és a nevedet!");
                }
                catch (Exception) { await ctx.RespondAsync("Valami hiba történt!"); }
            }

            [Command("Karakter")]
            [Description("Standard character info.")]
            public async Task GetCharInfo(CommandContext ctx)
            {
                logger.Debug(ctx.User.Username + " called WoW KarakterInfo!");
                var service = Program.Container.Resolve<IWoWService>();
                CharInfoResponse response;

                try
                {
                    response = await service.GetCharInfo(ctx.Member.Username);
                } catch (ArgumentException) { await ctx.RespondAsync("Nem találni a karaktered adatait! Mentsük el előtte, vagy add ki a parancsot specifikusan!"); return; }

                if (response != null)
                {
                    DiscordEmbed embed = GetWowCharInfoEmbed(response);

                    await ctx.RespondAsync(null, false, embed);
                    return;
                }

                await ctx.RespondAsync("Nocsak! Ilyen karaktert nem találni!");
            }

            [Command("KarakterInfo")]
            [Description("Standard character info.")]
            public async Task GetCharInfo(CommandContext ctx, string server, string name)
            {
                logger.Debug(ctx.User.Username + " called WoW KarakterInfo!");
                var service = Program.Container.Resolve<IWoWService>();
                
                var response = await service.GetCharInfo(server, name);

                if(response != null)
                {
                    DiscordEmbed embed = GetWowCharInfoEmbed(response);

                    await ctx.RespondAsync(null, false, embed);
                    return;
                }

                await ctx.RespondAsync("Nocsak! Ilyen karaktert nem találni!");
            }

            [Command("Mount")]
            [Description("Sum of the number of aquired mounts.")]
            public async Task GetMounts(CommandContext ctx)
            {
                logger.Debug(ctx.User.Username + " called wow MennyiMount!");
                var service = Program.Container.Resolve<IWoWService>();
                try
                {
                    var response = await service.GetMounts(ctx.User.Username);

                    await ctx.RespondAsync("Ejha! " + response + " mounttal rendelkezel!");

                }
                catch (ArgumentException) { await ctx.RespondAsync("Nem találni a karaktered adatait! Mentsük el előtte, vagy add ki a parancsot specifikusan!"); return; }

            }

            [Command("MennyiMount")]
            [Description("Sum of the number of aquired mounts.")]
            public async Task GetMounts(CommandContext ctx, string server, string name)
            {
                logger.Debug(ctx.User.Username + " called wow mennyimount!");
                var service = Program.Container.Resolve<IWoWService>();
                try
                {
                    var response = await service.GetMounts(server, name);

                    await ctx.RespondAsync("Ejha! " + name + " " + response + " mounttal rendelkezik!");

                } catch(Exception) { await ctx.RespondAsync("Nocsak! Ilyen karaktert nem találni!"); }
            }

            //[Command("Feed")]
            //[Description("Recent activity.")]
            //public async Task GetFeed(CommandContext ctx, string server, string name, int count = 5, string filter = "")
            //{
            //    logger.Debug(ctx.User.Username + "called " + ctx.Command.Name);
            //    var client = await ConstructBlizzardCharClient(server, name, new Parameter("fields", "feed", ParameterType.QueryString));

            //    var response = await client.ExecuteTaskAsync(new RestRequest());
            //    if (response.StatusCode == HttpStatusCode.OK)
            //    {
            //        CharInfoWithFeedResponse resp = JsonConvert.DeserializeObject<CharInfoWithFeedResponse>(response.Content);

            //        //elvileg a most recent az első és sorban van
            //        filter = filter.ToUpper();
            //        if (filter == "BOSSKILL" || filter == "ACHIEVEMENT" || filter == "LOOT")
            //        {
            //            resp.feed = (from feeds in resp.feed
            //                         where feeds.type == filter
            //                         select feeds).ToList();
            //        }
            //        var requiredRows = resp.feed.Take(count > resp.feed.Count ? resp.feed.Count : count);

            //        string responseRows = string.Empty;

            //        foreach (Feed feed in requiredRows)
            //        {
            //            string row = ConstructFeedStringDependingOnType(feed);
            //            responseRows += row + "\n";
            //        }

            //        await ctx.RespondAsync(responseRows, false, null);
            //        return;
            //    }

            //    await ctx.RespondAsync("Nocsak! Ilyen karaktert nem találni!");
            //}

            [Command("Exalted")]
            [Description("Number of exalted reputations.")]
            public async Task GetExaltedNumber(CommandContext ctx, string server, string name)
            {
                logger.Debug(ctx.User.Username + " called " + ctx.Command.Name);

                var service = Program.Container.Resolve<IWoWService>();

                try
                {
                    int retVal = await service.GetExalted(server, name);

                    await ctx.RespondAsync(name + " karakteren jelenleg " + retVal + " exalted repu van!");
                } catch (Exception e) { await ctx.RespondAsync("Nocsak! Ilyen karaktert nem találni!"); logger.Error(e.Message); }
            }

            [Command("Exa")]
            [Description("Number of exalted reputations.")]
            public async Task GetExaltedNumber(CommandContext ctx)
            {
                logger.Debug(ctx.User.Username + " called " + ctx.Command.Name);

                var service = Program.Container.Resolve<IWoWService>();

                try
                {
                    int retVal = await service.GetExalted(ctx.User.Username);

                    await ctx.RespondAsync("Jelenleg " + retVal + " exalted repud van!");
                }
                catch (ArgumentException) { await ctx.RespondAsync("Nem találni a karaktered adatait! Mentsük el előtte, vagy add ki a parancsot specifikusan!"); return; }
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
                 var service = Program.Container.Resolve<ITwitchService>();
                 var response = await service.GetStreamOnline(channel);

                 if(response.Online)
                 {
                     await ctx.RespondAsync(channel + " most online! " + response.TimeLapsedString + " streamel. " +
                     "Streamjét jelenleg " + response.ViewerCount + " ember nézi.");
                 }
                 else
                 {
                     await ctx.RespondAsync(channel + " jelenleg nem online!");
                 }
            }
        }
        #endregion

        #region QrCode related commands
        [Group("QRcode")]
        [Description("QR kód generálására használatos parancsok.")]
        public class QrCodeGroupedCommands
        {
            [Command("url")]
            public async Task GetQrCodeFromUrl(CommandContext ctx, string url)
            {
                try
                {
                    var service = Program.Container.Resolve<IQrCodeCreatorService>();
                    var result = service.UriToQrCode(url);

                    await ctx.RespondWithFileAsync(Path.Combine(Directory.GetCurrentDirectory(),@"temp.jpg"), "Parancsolj!");
                }
                catch(Exception e) { Console.WriteLine(e.Message); }
                
            }
        }
        #endregion

        #region Strava commands

        [Group("Strava", CanInvokeWithoutSubcommand = false)]
        public class StravaGroupedCommands
        {
            [Command("info")]
            [Description("Általános információk.")]
            public async Task StravaInfo(CommandContext ctx, int id)
            {
                var service = Program.Container.Resolve<IStravaService>();
                var result = await service.StravaInfo(id, Program.config.stravaSecret, Program.config.stravaClientID,
                    Program.config.stravaRefreshToken);
                await ctx.RespondAsync(result);
            }
        }

        #endregion

        #region ServerMaintainingCommands
        [Group("Kick",CanInvokeWithoutSubcommand = false)]
        public class KickGroupedCommands
        {
            [Command("id")]
            [RequireOwner]
            [Description("Kickel id alapján egy membert.")]
            public async Task KickMember(CommandContext ctx, ulong id)
            {
                var user = ctx.Channel.Guild.Members.FirstOrDefault(x => x.Id == id);
                if (user == null)
                {
                    await ctx.RespondAsync("Hibás ID! Nem tudtam kit kickelni!");
                    return;
                }
                await user.BanAsync(0,ctx.Member.Username +" adta ki a parancsot.");
                await ctx.RespondAsync("Hahaha! " + user.Username + " olyan banhammert kapott hogy ihaj!");
            }

            [Command("name")]
            [RequireOwner]
            [Description("Kickel name alapján egy membert.")]
            public async Task KickMember(CommandContext ctx, string username)
            {
                var users = ctx.Channel.Guild.Members.Where(x => x.Username == username);

                if (users == null || users.Count() == 0)
                {
                    await ctx.RespondAsync("Hibás név! Nem tudtam kit kickelni!");
                    return;
                }
                if(users.Count() > 1)
                {
                    await ctx.RespondAsync("Megegyező nevek! Add ki a parancsot ID alapján!");
                }

                await users.FirstOrDefault().BanAsync(0, ctx.Member.Username + " adta ki a parancsot.");
                await ctx.RespondAsync("Hahaha! " + username + " olyan banhammert kapott hogy ihaj!");
            }
        }
        #endregion

        #region private methods     

        private static DiscordEmbed GetWowCharInfoEmbed(CharInfoResponse response)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
            {
                Title = response.name,
                ImageUrl = @"http://render-eu.worldofwarcraft.com/character/" + response.thumbnail,
                Description = "level " + response.level + " " + Enum.GetName(typeof(Gender), response.gender) + " " + Enum.GetName(typeof(Race), response.race) + " " +
                        Enum.GetName(typeof(@class), response.@class),
                Url = "https://worldofwarcraft.com/en-gb/character/" + response.realm + "/" + response.name,
                Color = response.faction == 0 ? DiscordColor.Blue : DiscordColor.Red
            };
            builder.AddField("Honorable kills:", response.totalHonorableKills.ToString());
            builder.AddField("Achievement points:", response.achievementPoints.ToString());

            return builder.Build();
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
