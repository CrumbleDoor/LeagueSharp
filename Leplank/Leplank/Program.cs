using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;


namespace Leplank
{
    class Program
    {
        #region declarations
        static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        private static Spell Q, W, E, R;
        static string champName = "Gangplank";
        public const string version = "1.0.0.0";
        #endregion declarations

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Game.PrintChat("<b><font color='#FF6600'>Leplank</font></b> " + version + " loaded - By <font color='#6666FF'>Brikovich</font> & <font color='#6666FF'>Baballev</font>");
            Game.PrintChat("Don't forget to <font color='#00CC00'><b>Upvote</b></font> <b><font color='#FF6600'>Leplank</font></b> in the Assembly DB if you like it ^_^");
            Menus.MenuIni();

            //Events
            

        }


    }
}
