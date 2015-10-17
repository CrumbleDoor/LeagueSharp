using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using SharpDX;



namespace Bangplank
{
    class Program
    {
        public static String version = "1.0";
        private static String championName = "Gangplank";
        public static Obj_AI_Hero Player;
        private static Menu _menu;
        private static Orbwalking.Orbwalker _orbwalker;
        private static Spell Q, W, E, R;
        private static float explosionRadius = 400;
        private static float linkRange = 650;
        
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
                comboMenu.AddItem(new MenuItem("bangplank.menu.combo.e", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("bangplank.menu.combo.r", "Use R").SetValue(true));
            
            // Harass Menu
            var harassMenu = new Menu("Harass", "bangplank.menu.harass");            
                harassMenu.AddItem(new MenuItem("bangplank.menu.harass.q", "Use Q").SetValue(true));
            
            // Farm Menu
            var farmMenu = new Menu("Farm", "bangplank.menu.farm");
                farmMenu.AddItem(new MenuItem("bangplank.menu.farm.qlh", "Use Q to lasthit").SetValue(true));
                farmMenu.AddItem(new MenuItem("bangplank.menu.farm.qlhmana", "Minimum mana for Q lasthit").SetValue(new Slider(20, 0, 100)));
                
            // Misc Menu
            var miscMenu = new Menu("Misc", "bangplank.menu.misc");
                // Barrel Manager Options
                var barrelManagerMenu = new Menu("Barrel Manager","bangplank.menu.misc.barrelmanager");
                    barrelManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.barrelmanager.edisabled", "Block E usage").SetValue(false));
                
                miscMenu.AddItem(new MenuItem("bangplank.menu.misc.wcleanser", "Use W cleanser").SetValue(true));
                miscMenu.AddItem(new MenuItem("bangplank.menu.misc.wheal", "Use W to heal").SetValue(true));
                miscMenu.AddItem(new MenuItem("bangplank.menu.misc.healmin", "Health %").SetValue(new Slider(30, 0, 100)));
                miscMenu.AddItem(new MenuItem("bangplank.menu.misc.healminmana", "Minimum mana to heal").SetValue(new Slider(40, 0, 100)));

            // Drawing Menu
            Menu drawingMenu = new Menu("Drawing", "bangplank.menu.drawing");
                drawingMenu.AddItem(new MenuItem("bangplank.menu.drawing.enabled", "Enabled").SetValue(true));
                drawingMenu.AddItem(new MenuItem("bangplank.menu.drawing.q", "Draw Q range").SetValue(true));
                drawingMenu.AddItem(new MenuItem("bangplank.menu.drawing.e", "Draw E range").SetValue(true));

            
            _menu.AddSubMenu(orbwalkerMenu);
            _menu.AddSubMenu(targetSelectorMenu);
            _menu.AddSubMenu(comboMenu);
            _menu.AddSubMenu(harassMenu);
            _menu.AddSubMenu(farmMenu);
            _menu.AddSubMenu(miscMenu);
                miscMenu.AddSubMenu(barrelManagerMenu);
            _menu.AddSubMenu(drawingMenu);
            _menu.AddToMainMenu();
        }
        static void Game_OnGameLoad(EventArgs Args)
        {
            if (ObjectManager.Player.ChampionName != championName)
            {
                return;
            }
            Game.PrintChat("<b><font color='#FF6600'>Bang</font><font color='#FF0000'>Plank</font></b> " + version + " loaded - By <font color='#6666FF'>Baballev</font>");
            MenuIni();
            Player = ObjectManager.Player;       
            // Spells ranges
            Q = new Spell(SpellSlot.Q, 625);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 1000);
            R = new Spell(SpellSlot.R);
            Q.SetTargetted(0.5f, 1500f);
            R.SetSkillshot(0.7f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Game.OnUpdate += Logic;
            Drawing.OnDraw += Draw;

        }


        // Draw Manager
        static void Draw(EventArgs args)
        {
            if (GetBool("bangplank.menu.drawing.enabled") == false)
            {
                return;
            }
            if (GetBool("bangplank.menu.drawing.q"))
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.SteelBlue);
            }
            if (GetBool("bangplank.menu.drawing.e"))
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Red);
            }
        }

        // Orbwalker Manager
        static void Logic(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }
            var activeOrbwalker = _orbwalker.ActiveMode;
            switch (activeOrbwalker)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    WaveClear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Mixed();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
            }
        }

        private static void Combo()
        {
          
            
        }
        private static void WaveClear()
        {

            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
            var jungleMobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral);
            minions.AddRange(jungleMobs);

        }

        private static void Mixed()
        {
            
        }

        private static void LastHit()
        {
            // LH Logic
            var minions = MinionManager.GetMinions(Player.ServerPosition, Q.Range);

            // Q Last Hit
            if (GetBool("bangplank.menu.farm.qlh") && Q.IsReady() && Getslider("bangplank.menu.farm.qlhmana") >= Player.ManaPercent)
            {
                if (minions != null)
                {
                    foreach (var m in minions)
                    {
                        if (m != null)
                        {
                            if (m.Health <= Player.GetSpellDamage(m, SpellSlot.Q))
                            {
                                Q.CastOnUnit(m);
                            }
                        }
                    }
                }
            }



        }

        // Get Values code
        private static bool GetBool(string name)
        {
            return _menu.Item(name).GetValue<bool>();
        }
        private static int Getslider(String itemname)
        {
            return _menu.Item(itemname).GetValue<Slider>().Value;
        }


    }
}