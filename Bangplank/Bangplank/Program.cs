using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using SharpDX;

// By baballev
// Up to date for 5.20
namespace Bangplank
{
    class Program
    {
        public static String Version = "1.0.1.29";
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
            
            // Harass Menu
            var harassMenu = new Menu("Harass", "bangplank.menu.harass");            
                harassMenu.AddItem(new MenuItem("bangplank.menu.harass.q", "Use Q").SetTooltip("If disabled, it won't block EQ usage").SetValue(true));                
                harassMenu.AddItem(new MenuItem("bangplank.menu.harass.separator1", "Extended EQ:"));
                harassMenu.AddItem(new MenuItem("bangplank.menu.harass.extendedeq", "Enabled").SetValue(true));
                harassMenu.AddItem(new MenuItem("bangplank.menu.harass.instructioneq", "Place E near your pos, then it will auto"));
                harassMenu.AddItem(new MenuItem("bangplank.menu.harass.instructionqe2", "E in range of 1st barrel + Q to harass"));
                harassMenu.AddItem(new MenuItem("bangplank.menu.harass.qmana", "Minimum mana to use Q harass").SetTooltip("Minimum mana for Q harass & Extended EQ").SetValue(new Slider(30, 0, 100)));

            // Farm Menu
            var farmMenu = new Menu("Farm", "bangplank.menu.farm");
                farmMenu.AddItem(new MenuItem("bangplank.menu.farm.qlh", "Use Q to lasthit").SetTooltip("Recommended enable for bonus gold").SetValue(true));
                farmMenu.AddItem(new MenuItem("bangplank.menu.farm.qlhmana", "Minimum mana for Q lasthit").SetValue(new Slider(20, 0, 100)));
                farmMenu.AddItem(new MenuItem("bangplank.menu.farm.ewc", "Use E to waveclear").SetValue(true));
                
            // Misc Menu
            var miscMenu = new Menu("Misc", "bangplank.menu.misc");
                // Barrel Manager Options
                var barrelManagerMenu = new Menu("Barrel Manager","bangplank.menu.misc.barrelmanager");
                    barrelManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.barrelmanager.edisabled", "Block E usage").SetTooltip("If on, won't use E").SetValue(false));
                    

                // Cleanser W Manager Menu
                var cleanserManagerMenu = new Menu("W cleanser", "bangplank.menu.misc.cleansermanager");
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.enabled", "Enabled").SetValue(true));
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.separation1", ""));
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.separation2", "Buff Types: "));
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.charm", "Charm").SetValue(true));
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.flee", "Flee").SetTooltip("Fear").SetValue(true));
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.polymorph", "Polymorph").SetValue(true));
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.snare", "Snare").SetValue(true));
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.stun", "Stun").SetValue(true));
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.taunt", "Taunt").SetValue(true));
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.exhaust", "Exhaust").SetTooltip("Will only remove Slow").SetValue(false));
                    cleanserManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.cleansermanager.suppression", "Supression").SetValue(true));

                miscMenu.AddItem(new MenuItem("bangplank.menu.misc.wheal", "Use W to heal").SetTooltip("Enable auto W heal(won't cancel recall if low)").SetValue(true));
                miscMenu.AddItem(new MenuItem("bangplank.menu.misc.healmin", "Health %").SetTooltip("If under, will use W").SetValue(new Slider(30, 0, 100)));
                miscMenu.AddItem(new MenuItem("bangplank.menu.misc.healminmana", "Minimum Mana %").SetTooltip("Minimum mana to use W heal").SetValue(new Slider(35, 0, 100)));
                miscMenu.AddItem(new MenuItem("bangplank.menu.misc.ks", "KillSteal").SetTooltip("If off, won't try to KS").SetValue(true));
                miscMenu.AddItem(new MenuItem("bangplank.menu.misc.qks", "Use Q to KillSteal").SetTooltip("If on, will auto Q to KS").SetValue(true));
                miscMenu.AddItem(new MenuItem("bangplank.menu.misc.rks", "Use R to KillSteal").SetTooltip("If on, will try to KS on the whole map").SetValue(true));
            
            // Items Manager Menu
            var itemManagerMenu = new Menu("Items Manager", "bangplank.menu.item");
                var potionManagerMenu = new Menu("Potions", "bangplank.menu.item.potion");
                    potionManagerMenu.AddItem(new MenuItem("bangplank.menu.item.potion.enabled", "Enabled").SetTooltip("If off, won't use potions").SetValue(true));
                    potionManagerMenu.AddItem(new MenuItem("bangplank.menu.item.potion.hp", "Health Potion").SetValue(true));
                    potionManagerMenu.AddItem(new MenuItem("bangplank.menu.item.potion.hphealth", "Health %").SetTooltip("If under, will use Health potion").SetValue(new Slider(50, 0, 100)));
                    potionManagerMenu.AddItem(new MenuItem("bangplank.menu.item.potion.mp", "Mana Potion").SetValue(true));
                    potionManagerMenu.AddItem(new MenuItem("bangplank.menu.item.potion.mana", "Mana %").SetTooltip("If under, will use Mana potion").SetValue(new Slider(30, 0, 100)));
                    potionManagerMenu.AddItem(new MenuItem("bangplank.menu.item.potion.biscuit", "Biscuit").SetValue(true));
                    potionManagerMenu.AddItem(new MenuItem("bangplank.menu.item.potion.biscuithealth", "Health %").SetTooltip("If under, will use Biscuit of rejuvenation").SetValue(new Slider(50, 0, 100)));
                    potionManagerMenu.AddItem(new MenuItem("bangplank.menu.item.potion.cryst", "Crystalline Flask").SetValue(true));
                    potionManagerMenu.AddItem(new MenuItem("bangplank.menu.item.potion.crysthealth", "Health %").SetTooltip("If under, will use Crystalline Flask").SetValue(new Slider(50, 0, 100)));
                    potionManagerMenu.AddItem(new MenuItem("bangplank.menu.item.potion.crystmana", "Mana %").SetTooltip("If under, will use Crystalline Flask").SetValue(new Slider(30, 0, 100)));

            itemManagerMenu.AddItem(new MenuItem("bangplank.menu.item.youmuu", "Use Youmuu's Ghostblade").SetTooltip("Use Youmuu in Combo").SetValue(true));
            itemManagerMenu.AddItem(new MenuItem("bangplank.menu.item.hydra", "Use Ravenous Hydra").SetTooltip("Use Hydra to clear and in Combo").SetValue(true));

            // Drawing Menu
            Menu drawingMenu = new Menu("Drawing", "bangplank.menu.drawing");
                drawingMenu.AddItem(new MenuItem("bangplank.menu.drawing.enabled", "Enabled").SetTooltip("If off, will block bangplank drawings").SetValue(true));
                drawingMenu.AddItem(new MenuItem("bangplank.menu.drawing.q", "Draw Q range").SetValue(true));
                drawingMenu.AddItem(new MenuItem("bangplank.menu.drawing.e", "Draw E range").SetValue(true));
           
            _menu.AddSubMenu(orbwalkerMenu);
            _menu.AddSubMenu(targetSelectorMenu);
            _menu.AddSubMenu(comboMenu);
            _menu.AddSubMenu(harassMenu);
            _menu.AddSubMenu(farmMenu);
            _menu.AddSubMenu(miscMenu);
            _menu.AddSubMenu(itemManagerMenu);
                itemManagerMenu.AddSubMenu(potionManagerMenu);
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
            if (sender.Name == "Barrel")
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
            if (GetBool("bangplank.menu.misc.ks"))
            {
                KillSteal();
            }
            if (GetBool("bangplank.menu.misc.barrelmanager.edisabled") == false)
            {
                BarrelManager();
            }
            if (GetBool("bangplank.menu.item.potion.enabled"))
            {
                Potion();
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
            
            var target = TargetSelector.GetTarget(Q.Range + explosionRange, TargetSelector.DamageType.Physical);

            // TODO 
          //  LeagueSharp.Common.Items.UseItem(3142); //yumuu
          //  LeagueSharp.Common.Items.UseItem(3074); //hydra
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
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            // Q lasthit minions
            var minions = MinionManager.GetMinions(Player.ServerPosition, Q.Range);
            if (GetBool("bangplank.menu.farm.qlh") && Q.IsReady() && Player.ManaPercent >= Getslider("bangplank.menu.farm.qlhmana") && target == null)
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
            // Q
            if (GetBool("bangplank.menu.harass.q") && Q.IsReady() && Player.ManaPercent >= Getslider("bangplank.menu.harass.qmana") && TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical) != null && LiveBarrels.Count == 0)
            {

                Q.Cast(TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical));
            }

            // Extended EQ
            if (Q.IsReady() && E.IsReady() && GetBool("bangplank.menu.harass.extendedeq") && GetBool("bangplank.menu.misc.barrelmanager.edisabled") == false)
            {
                if (LiveBarrels.Count == 0) return;

                Keg nbar = NearestKeg(Player.ServerPosition.To2D());
                if (nbar == null) return;
                if ((Player.ServerPosition.Distance(nbar.KegObj.Position) < Q.Range && nbar.KegObj.Health < 2) || (Player.ServerPosition.Distance(nbar.KegObj.Position) < Q.Range && nbar.KegObj.Health < 3 && Player.Level >= 13))
                {
                    if (target != null)
                    {
                        var prediction = Prediction.GetPrediction(target, 0.7f).CastPosition;                     
                        if (nbar.KegObj.Distance(prediction) < linkRange)
                        {                          
                            E.Cast(prediction);
                            if (Player.Level < 13)
                            {
                                Utility.DelayAction.Add((int) (30 + Game.Ping), () =>
                                {
                                    Q.Cast(nbar.KegObj);
                                }
                                    );
                            }
                            if (Player.Level >= 13)
                            {
                                Utility.DelayAction.Add((int)(450 - Game.Ping), () =>
                                {
                                    Q.Cast(nbar.KegObj);
                                }
                                   );

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

        // W heal
        private static void HealManager()
        {
            if (Player.InFountain()) return;
            if (Player.IsRecalling()) return;
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

        // Ks logic, - Wtf u sayin? there's no logic here brah - shhh they won't see - stupid moron 
        private static void KillSteal()
        {
            var kstarget = HeroManager.Enemies;
            if (GetBool("bangplank.menu.misc.qks") && Q.IsReady())
            {
                if (kstarget != null)
                {
                    foreach (var ks in kstarget)
                    {
                        if (ks != null)
                        {
                            if (ks.Health <= Player.GetSpellDamage(ks, SpellSlot.Q) && ks.Health > 0 && Q.IsInRange(ks))
                            {
                               
                                Q.CastOnUnit(ks);
                            }
                        }
                    }
                }
            }
            if (GetBool("bangplank.menu.misc.rks") && R.IsReady())
            {
                if (kstarget != null)
                    foreach (var ks in kstarget)
                    {
                        if (ks != null)
                        {
                            if (ks.Health <= Player.GetSpellDamage(ks, SpellSlot.R)*9 && ks.Health > 0)
                            {
                                var ksposition = Prediction.GetPrediction(ks, 0.7f).CastPosition;

                                if (ksposition.Distance(ks.Position) < 400 && ks.IsMoving)
                                {
                                    ksposition = ks.Position.Extend(ksposition, 400);
                                }
                                if (ksposition.IsValid())
                                {
                                    R.Cast(ksposition);
                                }
                            }
                        }
                    }
            }
        }

        private static void BarrelManager()
        {
           
            
        }

        private static void Potion()
        {
            if (Player.InFountain()) return;
            if (Player.IsRecalling()) return;

            if (GetBool("bangplank.menu.item.potion.hp") &&
                Player.HealthPercent <= Getslider("bangplank.menu.item.potion.hphealth") &&
                LeagueSharp.Common.Items.HasItem(2003))
            {
                if (Player.HasBuff("RegenerationPotion")) return;

                LeagueSharp.Common.Items.UseItem(2003);
            }
            if (GetBool("bangplank.menu.item.potion.mp") &&
                Player.ManaPercent <= Getslider("bangplank.menu.item.potion.mana") &&
                LeagueSharp.Common.Items.HasItem(2004))
            {
                if (Player.HasBuff("FlaskOfCrystalWater")) return;

                LeagueSharp.Common.Items.UseItem(2004);
            }
            if (GetBool("bangplank.menu.item.potion.biscuit") &&
            Player.HealthPercent <= Getslider("bangplank.menu.item.potion.biscuithealth") &&
            LeagueSharp.Common.Items.HasItem(2010))
            {
                if (Player.HasBuff("ItemMiniRegenPotion")) return;

                LeagueSharp.Common.Items.UseItem(2010);
            }
            if (GetBool("bangplank.menu.item.potion.cryst") &&
            (Player.HealthPercent <= Getslider("bangplank.menu.item.potion.crysthealth") ||
            Player.ManaPercent <= Getslider("bangplank.menu.item.potion.crystmana") ||
            Player.HealthPercent <= Getslider("bangplank.menu.item.potion.crysthealth") / 2 ||
            Player.ManaPercent <= Getslider("bangplank.menu.item.potion.crystmana") / 2) &&
            LeagueSharp.Common.Items.HasItem(2041))
            {
                if (Player.HasBuff("ItemCrystalFlask")) return;

                LeagueSharp.Common.Items.UseItem(2041);
            }
        }

        private static Keg NearestKeg(Vector2 pos)
        {
            if (LiveBarrels.Count == 0)
            {
                return null;
            }
            return LiveBarrels.OrderBy(k => k.KegObj.ServerPosition.Distance(pos.To3D())).FirstOrDefault();
        }
        // Get Values code
        private static bool GetBool(string name)
        {
            return _menu.Item(name).GetValue<bool>();
        }
        private static int Getslider(string itemname)
        {
            return _menu.Item(itemname).GetValue<Slider>().Value;
        }
    }

    internal class Keg
    {
        public Obj_AI_Minion KegObj;
        
        
        public Keg(Obj_AI_Minion obj)
        {
            KegObj = obj;
            
        }


    }


}