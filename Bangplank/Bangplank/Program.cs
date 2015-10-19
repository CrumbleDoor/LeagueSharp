using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using SharpDX;



namespace Bangplank
{
    class Program
    {
        public static String Version = "1.0.1.7";
        private static String championName = "Gangplank";
        public static Obj_AI_Hero Player;
        private static Menu _menu;
        private static Orbwalking.Orbwalker _orbwalker;
        private static Spell Q, W, E, R;
        private static float explosionRange = 390;
        private static float linkRange = 650;
        private static List<Keg>  LiveBarrels = new List<Keg>();
        
        
         private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void MenuIni()
        {
            // Main Menu
            _menu = new Menu("<font color='#FF6600'>Bang</font><font color='#FF0000'>Plank</font>", "bangplank.menu", true);

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
            
            // Harass Menu
            var harassMenu = new Menu("Harass", "bangplank.menu.harass");            
                harassMenu.AddItem(new MenuItem("bangplank.menu.harass.q", "Use Q").SetValue(true));
                harassMenu.AddItem(new MenuItem("bangplank.menu.harass.qmana", "Minimum mana for Q harass").SetValue(new Slider(30, 0, 100)));
            harassMenu.AddItem(new MenuItem("bangplank.menu.harass.separator1", "Extended EQ:"));
            harassMenu.AddItem(new MenuItem("bangplank.menu.harass.extendedqe", "Enabled"));
            harassMenu.AddItem(new MenuItem("bangplank.menu.harass.instructioneq", "Place E near your pos, then"));
            harassMenu.AddItem(new MenuItem("bangplank.menu.harass.instructionqe2", "it will E + Q to harass"));


            // Farm Menu
            var farmMenu = new Menu("Farm", "bangplank.menu.farm");
                farmMenu.AddItem(new MenuItem("bangplank.menu.farm.qlh", "Use Q to lasthit").SetValue(true));
                farmMenu.AddItem(new MenuItem("bangplank.menu.farm.qlhmana", "Minimum mana for Q lasthit").SetValue(new Slider(20, 0, 100)));
                farmMenu.AddItem(new MenuItem("bangplank.menu.farm.ewc", "Use E to waveclear").SetValue(true));
                
            // Misc Menu
            var miscMenu = new Menu("Misc", "bangplank.menu.misc");
                // Barrel Manager Options
                var barrelManagerMenu = new Menu("Barrel Manager","bangplank.menu.misc.barrelmanager");
                    barrelManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.barrelmanager.edisabled", "Block E usage").SetValue(false));
                    

                // Cleanser W Manager Menu
                var cleanserManagerMenu = new Menu("W cleanser", "bangplank.menu.misc.cleansermanager");
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.enabled", "Enabled").SetValue(true));
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.separation1", ""));
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.separation2", "Buff Types: "));
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.charm", "Charm").SetValue(true));
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.flee", "Flee").SetValue(true));
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.polymorph", "Polymorph").SetValue(true));
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.snare", "Snare").SetValue(true));
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.stun", "Stun").SetValue(true));
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.taunt", "Taunt").SetValue(true));
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.exhaust", "Exhaust(slow only)").SetValue(false));
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.suppression", "Supression").SetValue(true));

                miscMenu.AddItem(new MenuItem("bangplank.menu.misc.wheal", "Use W to heal").SetValue(true));
                miscMenu.AddItem(new MenuItem("bangplank.menu.misc.healmin", "Health %").SetValue(new Slider(30, 0, 100)));
                miscMenu.AddItem(new MenuItem("bangplank.menu.misc.healminmana", "Minimum mana to heal").SetValue(new Slider(40, 0, 100)));
                miscMenu.AddItem(new MenuItem("bangplank.menu.misc.qks", "Q KillSteal").SetValue(true));

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
                miscMenu.AddSubMenu(cleanserManagerMenu);
            _menu.AddSubMenu(drawingMenu);
            _menu.AddToMainMenu();
        }

        private static void Game_OnGameLoad(EventArgs Args)
        {
            if (ObjectManager.Player.ChampionName != championName)
            {
                return;
            }
            Game.PrintChat("<b><font color='#FF6600'>Bang</font><font color='#FF0000'>Plank</font></b> " + Version + " loaded - By <font color='#6666FF'>Baballev</font>");
            Game.PrintChat("Don't forget to <font color='#00CC00'><b>Upvote</b></font> <b><font color='#FF6600'>Bang</font><font color='#FF0000'>Plank</font></b> in the AssemblyDB if you like it ^_^");
            MenuIni();
            Player = ObjectManager.Player;       
            // Spells ranges
            Q = new Spell(SpellSlot.Q, 610);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 980);
            R = new Spell(SpellSlot.R);
            Q.SetTargetted(0.25f, 2000f);
            E.SetSkillshot(0.5f, 100, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.8f, 200, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Game.OnUpdate += Logic;
            Drawing.OnDraw += Draw;
            GameObject.OnCreate += GameObjCreate;
            GameObject.OnDelete += GameObjDelete;

        }

        private static void GameObjCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Keg")
            {
                LiveBarrels.Add(new Keg(sender as Obj_AI_Minion));
            }
        }

        private static void GameObjDelete(GameObject sender, EventArgs args)
        {
            for (int i = 0; i < LiveBarrels.Count; i++)
            {
                if (LiveBarrels[i].KegObj.NetworkId == sender.NetworkId)
                {
                    LiveBarrels.RemoveAt(i);
                    return;
                }
            }
        }

        // Draw Manager
        static void Draw(EventArgs args)
        {
            if (GetBool("bangplank.menu.drawing.enabled") == false)
            {
                return;
            }
            if (GetBool("bangplank.menu.drawing.q") && Q.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.SteelBlue);
            }
            if (GetBool("bangplank.menu.drawing.e") && E.Level > 0)
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
            if (GetBool("bangplank.menu.misc.cleansermanager.enabled"))
            {
                CleanserManager();
            }
            if (GetBool("bangplank.menu.misc.wheal"))
            {
                HealManager();
            }
            if (GetBool("bangplank.menu.misc.qks"))
            {
                KillSteal();
            }
            if (GetBool("bangplank.menu.misc.barrelmanager.edisabled") == false)
            {
                BarrelManager();
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
            var target = TargetSelector.GetTarget(1200, TargetSelector.DamageType.Physical);

            // TODO
        }
        private static void WaveClear()
        {
            // TODO 
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
            var jungleMobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral);
            minions.AddRange(jungleMobs);

        }

        private static void Mixed()
        {

            
            

            // harass
            // Extended QE
            if (Q.IsReady() && E.IsReady())
            {
                
            }


            // Q
            else if (GetBool("bangplank.menu.harass.q") && Q.IsReady() && Player.ManaPercent >= Getslider("bangplank.menu.harass.qmana") && TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical) != null)
            {
                Q.Cast(TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical));
            }


            // Q lasthit
            var minions = MinionManager.GetMinions(Player.ServerPosition, Q.Range);

            if (GetBool("bangplank.menu.farm.qlh") && Q.IsReady() && Player.ManaPercent >= Getslider("bangplank.menu.farm.qlhmana"))
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

        private static void LastHit()
        {
            // LH Logic
            var minions = MinionManager.GetMinions(Player.ServerPosition, Q.Range);

            // Q Last Hit
            if (GetBool("bangplank.menu.farm.qlh") && Q.IsReady() && Player.ManaPercent >= Getslider("bangplank.menu.farm.qlhmana"))
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

        private static void HealManager()
        {
            if (W.IsReady() && Player.HealthPercent <= Getslider("bangplank.menu.misc.healmin") &&
                Player.ManaPercent >= Getslider("bangplank.menu.misc.healminmana"))
            {
                W.Cast();
            }


        }

        private static void CleanserManager()
        {
            // List of disable buffs
            if
                (W.IsReady() && (               
                (Player.HasBuffOfType(BuffType.Charm) && GetBool("bangplank.menu.misc.cleansermanager.charm")) 
                || (Player.HasBuffOfType(BuffType.Flee) && GetBool("bangplank.menu.misc.cleansermanager.flee"))
                || (Player.HasBuffOfType(BuffType.Polymorph) && GetBool("bangplank.menu.misc.cleansermanager.polymorph"))
                || (Player.HasBuffOfType(BuffType.Snare) && GetBool("bangplank.menu.misc.cleansermanager.snare"))
                || (Player.HasBuffOfType(BuffType.Stun) && GetBool("bangplank.menu.misc.cleansermanager.stun"))                
                || (Player.HasBuffOfType(BuffType.Taunt) && GetBool("bangplank.menu.misc.cleansermanager.taunt"))
                || (Player.HasBuff("summonerexhaust") && GetBool("bangplank.menu.misc.cleansermanager.exhaust"))
                || (Player.HasBuffOfType(BuffType.Suppression) && GetBool("bangplank.menu.misc.cleansermanager.suppression"))
                ))              
            {
                W.Cast();
            }
        }

        private static void KillSteal()
        {
            var kstarget = HeroManager.Enemies;

            if (Q.IsReady())
            {
                if (kstarget != null)
                {
                    foreach (var ks in kstarget)
                    {
                        if (ks != null)
                        {
                            if (ks.Health <= Player.GetSpellDamage(ks, SpellSlot.Q))
                            {
                               
                                Q.CastOnUnit(ks);
                            }
                        }
                    }
                }
            }
        }

        private static void BarrelManager()
        {
           
            
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

    internal class Keg
    {
        public Obj_AI_Minion KegObj;
        public bool IsReady;
        
        public Keg(Obj_AI_Minion obj)
        {
            KegObj = obj;
            Ready();
        }

        private void Ready()
        {
            // int time = Bangplank.Program.Player.Level > 6 ? (Player.Level > 12 ? 1000 : 2000) : 4000;


            if (Bangplank.Program.Player.Level > 12)
            {
                Utility.DelayAction.Add(1000, () => this.IsReady = true);
            }
            else if (Bangplank.Program.Player.Level > 6)
            {
                Utility.DelayAction.Add(2000, () => this.IsReady = true);
            }
            else
            {
                Utility.DelayAction.Add(1000, () => this.IsReady = true);
            }


        }

    }


}