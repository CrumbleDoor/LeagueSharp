using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

namespace Bangplank
{
    class Program
    {
        private static String championName = "Gangplank";
        public static Obj_AI_Hero Player;
        private static Menu _menu;
        private static Orbwalking.Orbwalker _orbwalker;
        private static Spell Q, W, E, R;


        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
           
        }

        private static void MenuIni()
        {
            // Main Menu
            _menu = new Menu("BangPlank", "bangplank.menu", true);

            // Orbwalker Menu
            var orbwalkerMenu = new Menu("Orbwalker", "bangplank.menu.orbwalker");
            _orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            // Target Selector Menu
            var targetSelectorMenu = new Menu("Target Selector", "bangplank.menu.targetselector");
            TargetSelector.AddToMenu(targetSelectorMenu);

            // Combo Menu
            var comboMenu = new Menu("Combo", "bangplank.menu.combo");  
                comboMenu.AddItem(new MenuItem("bangplank.menu.combo.q", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("bangplank.menu.combo.w", "Use W").SetValue(true));
                comboMenu.AddItem(new MenuItem("bangplank.menu.combo.e", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("bangplank.menu.combo.r", "Use R").SetValue(true));
            
            // Harass Menu
            var harassMenu = new Menu("Harass", "bangplank.menu.harass");            
                harassMenu.AddItem(new MenuItem("bangplank.menu.harass.q", "Use Q").SetValue(true));
            
            // Farm Menu
            var farmMenu = new Menu("Farm", "bangplank.menu.farm");

            // Misc Menu


            // Drawing Menu



            _menu.AddSubMenu(orbwalkerMenu);
            _menu.AddSubMenu(targetSelectorMenu);
            _menu.AddSubMenu(comboMenu);
            _menu.AddSubMenu(harassMenu);
            _menu.AddSubMenu(farmMenu);
            _menu.AddToMainMenu();
        }


        static void Game_OnGameLoad(EventArgs Args)
        {
            if (ObjectManager.Player.ChampionName != championName)
            {
                return;
            }
            Game.PrintChat("<b><font color='#FF6600'>Bang</font><font color='#FF0000'>Plank</font></b> loaded - By <font color='#6666FF'>Baballev</font>");
            MenuIni();
            // Spells ranges
            Q = new Spell(SpellSlot.Q, 100);
            W = new Spell(SpellSlot.W, 100);
            E = new Spell(SpellSlot.E, 100);
            R = new Spell(SpellSlot.R, 100);


            
        }
    }
}
