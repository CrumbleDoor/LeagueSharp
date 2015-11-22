using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using LeagueSharp;


namespace Leplank
{

    // This is inspired from h3h3 support AIO
    public static class Updater
    {
        public static Version Ver;
        public static void Update()
        {
            Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        using (var web = new WebClient())
                        {
                            var LiveVer = web.DownloadString("https://github.com/Brikovich/LeagueSharp/blob/master/Leplank/Leplank/Properties/AssemblyInfo.cs");

                            var regex = new Regex(@"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]").Match(LiveVer);
                            Ver = Assembly.GetExecutingAssembly().GetName().Version;

                            if (regex.Success)
                            {
                                var gitVersion =new Version(
                                    $"{regex.Groups[1]}.{regex.Groups[2]}.{regex.Groups[3]}.{regex.Groups[4]}");

                                if (gitVersion != Ver)
                                {
                                    Game.PrintChat("You are using an <b><font color='#CC0000'>Outdated</font></b> version of <b><font color='#8A008A'>Le</font><font color='#FF6600'>plank</font></b>");
                                    Game.PrintChat("Please update the assembly in your loader and reload in game");
                                }
                            }
                        }
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });
        }
    }
}