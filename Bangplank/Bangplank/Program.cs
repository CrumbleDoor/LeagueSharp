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

// By baballev, made with love and swagg
// Up to date for 5.20

// -------------------------------------------
// -/!\ Bad English under this line kappa /!\-
// -------------------------------------------

// This is my first assembly, I don't think my code is optimised yet but i'll try to improve it in the future, if u have any suggestion, please tell me :3
// 1) I know my code is a mess so if you have any recommandations, I'll be happy to listen to you :)
// 2) If you found a bogue please send me a private message or post on the realated thread on the forum.
// 3) If you have any suggestion about a new feature , please send me a private message or post on the related thread on the forum.
// 4) Do not hesitate to send me your game stats I really want to know if it is usefull and so on.
// 5) I'm open to any remarks, discussion, suggestion, ... Do not hesitate.
// Enjoy.
namespace Bangplank
{
    class Program
    {
        public static String Version = "1.0.2.10";
        private static String championName = "Gangplank";
        public static Obj_AI_Hero Player;
        private static Menu _menu;
        private static Orbwalking.Orbwalker _orbwalker;
        private static Spell Q, W, E, R;
        private static float explosionRange = 390;
        private static float linkRange = 650;
        private static List<Keg>  LiveBarrels = new List<Keg>(); // Keg means powder keg, = barrel
        private static bool qautoallowed = true;
        
         private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }
        // Devs, be aware you might have a heart attack by looking at organisation of this assembly 
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
                comboMenu.AddItem(new MenuItem("bangplank.menu.combo.q", "Use Q = ON"));
                comboMenu.AddItem(new MenuItem("bangplank.menu.combo.e", "Use E = ON"));
                comboMenu.AddItem(new MenuItem("bangplank.menu.combo.r", "Use R").SetValue(true));
                comboMenu.AddItem(new MenuItem("bangplank.menu.combo.rmin", "Minimum enemies to cast R").SetTooltip("Minimum enemies to hit with R in combo").SetValue(new Slider(2, 1, 5)));
            
            // Harass Menu
            var harassMenu = new Menu("Harass", "bangplank.menu.harass");            
                harassMenu.AddItem(new MenuItem("bangplank.menu.harass.q", "Use Q").SetTooltip("If disabled, it won't block EQ usage").SetValue(true));                
                harassMenu.AddItem(new MenuItem("bangplank.menu.harass.separator1", "Extended EQ:"));
                harassMenu.AddItem(new MenuItem("bangplank.menu.harass.extendedeq", "Enabled").SetValue(true));
                harassMenu.AddItem(new MenuItem("bangplank.menu.harass.instructioneq", "Place E near your pos, then it will auto"));
                harassMenu.AddItem(new MenuItem("bangplank.menu.harass.instructionqe2", "E in range of 1st barrel + Q to harass").SetTooltip("Tip: Place your first barrel in a bush"));
                harassMenu.AddItem(new MenuItem("bangplank.menu.harass.qmana", "Minimum mana to use Q harass").SetTooltip("Minimum mana for Q harass & Extended EQ").SetValue(new Slider(20, 0, 100)));

            // Farm Menu
            var farmMenu = new Menu("Farm", "bangplank.menu.farm");
                farmMenu.AddItem(new MenuItem("bangplank.menu.farm.qlh", "Use Q to lasthit").SetTooltip("Recommended On for bonus gold").SetValue(true));
                farmMenu.AddItem(new MenuItem("bangplank.menu.farm.qlhmana", "Minimum mana for Q lasthit").SetValue(new Slider(10, 0, 100)));
                farmMenu.AddItem(new MenuItem("bangplank.menu.farm.ewc", "Use E to Laneclear & Jungle").SetValue(true));
                farmMenu.AddItem(new MenuItem("bangplank.menu.farm.eminwc", "Minimum minions to use E").SetTooltip("If jungle mobs, it won't block E usage under value").SetValue(new Slider(5, 1, 15)));
                farmMenu.AddItem(new MenuItem("bangplank.menu.farm.qewc", "Use Q on E to clear").SetTooltip("Recommended On for bonus gold").SetValue(true));
                farmMenu.AddItem(new MenuItem("bangplank.menu.farm.qewcmana", "Minimum mana to use Q on E").SetValue(new Slider(10, 0, 100)));

            // Misc Menu
            var miscMenu = new Menu("Misc", "bangplank.menu.misc");
                // Barrel Manager Options
                var barrelManagerMenu = new Menu("Barrel Manager","bangplank.menu.misc.barrelmanager");
                    barrelManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.barrelmanager.edisabled", "Block E usage").SetTooltip("If on, won't use E").SetValue(false));
                    barrelManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.barrelmanager.stacks", "Number of stacks to keep").SetTooltip("If Set to 0, it won't keep any stacks; Stacks are used in combo/harass").SetValue(new Slider(1, 0, 4)));
                    barrelManagerMenu.AddItem(new MenuItem("bangplank.menu.misc.barrelmanager.autoboom", "Auto explode when enemy in explosion range").SetTooltip("Will auto Q on barrels that are near enemies").SetValue(true));

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

                // SwagPlank Menu ( Trolling functions ), I know it's useless, but fuck The Police. WIP
                var swagplankMenu = new Menu("[WIP] SwagPlank", "bangplank.menu.misc.swagplank");
                    swagplankMenu.AddItem(new MenuItem("bangplank.menu.misc.swagplank.separator", "== Better not using these functions in ranked =="));
                    swagplankMenu.AddItem(new MenuItem("bangplank.menu.misc.swagplank.enabled", "SwagPlank").SetTooltip("Enable SwagPlank features").SetValue(false));
                    swagplankMenu.AddItem(new MenuItem("bangplank.menu.misc.swagplank.acedance", "Spam Dance on Ace").SetValue(false));
                    swagplankMenu.AddItem(new MenuItem("bangplank.menu.misc.swagplank.bullshit", "Say bullshit if Triple or +").SetTooltip("Example: My swaggness, keep me safe nabs").SetValue(false));
                    swagplankMenu.AddItem(new MenuItem("bangplank.menu.misc.swagplank.acedance", "Spam Dance on Ace").SetValue(false));

                miscMenu.AddItem(new MenuItem("bangplank.menu.misc.wheal", "Use W to heal").SetTooltip("Enable auto W heal(won't cancel recall if low)").SetValue(true));
                miscMenu.AddItem(new MenuItem("bangplank.menu.misc.healmin", "Health %").SetTooltip("If under, will use W").SetValue(new Slider(30, 0, 100)));
                miscMenu.AddItem(new MenuItem("bangplank.menu.misc.healminmana", "Minimum Mana %").SetTooltip("Minimum mana to use W heal").SetValue(new Slider(35, 0, 100)));
                miscMenu.AddItem(new MenuItem("bangplank.menu.misc.ks", "KillSteal").SetTooltip("If off, won't try to KS").SetValue(true));
                miscMenu.AddItem(new MenuItem("bangplank.menu.misc.qks", "Use Q to KillSteal").SetTooltip("If on, will auto Q to KS").SetValue(true));
                miscMenu.AddItem(new MenuItem("bangplank.menu.misc.rks", "Use R to KillSteal").SetTooltip("If on, will try to KS on the whole map").SetValue(true));
                miscMenu.AddItem(new MenuItem("bangplank.menu.misc.fleekey", "[WIP] Flee").SetValue(new KeyBind(65, KeyBindType.Press)));
            
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
            itemManagerMenu.AddItem(new MenuItem("bangplank.menu.item.tiamat", "Use Tiamat").SetTooltip("Use Tiamat to clear and in Combo").SetValue(true));

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
                miscMenu.AddSubMenu(swagplankMenu);
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
            Q.SetTargetted(0.25f, 2150f);
            E.SetSkillshot(0.5f, 40, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(1f, 100, float.MaxValue, false, SkillshotType.SkillshotCircle);
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
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.FromArgb(38, 126, 188));
            }
            if (GetBool("bangplank.menu.drawing.e") && E.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.BlueViolet);
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
                    qautoallowed = false;
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    WaveClear();
                    qautoallowed = true;
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Mixed();
                    qautoallowed = false;
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    qautoallowed = true;
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    qautoallowed = true;
                    // flee maybe inc bogue
                    break;
            }
            if (_menu.Item("bangplank.menu.misc.fleekey").GetValue<KeyBind>().Active)
            {
                // Flee();
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
            if (GetBool("bangplank.menu.misc.barrelmanager.edisabled") == false && GetBool("bangplank.menu.misc.barrelmanager.autoboom") && qautoallowed)
            {
                BarrelManager();
            }
            if (GetBool("bangplank.menu.item.potion.enabled"))
            {
                Potion();
            }

        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical, true, HeroManager.Enemies.Where(e => e.IsInvulnerable));
            var ePrediction = Prediction.GetPrediction(target, 1f).CastPosition;
            var nbar = NearestKeg(Player.ServerPosition.To2D());

            // ITEMS
            if (GetBool("bangplank.menu.item.youmuu") && HeroManager.Enemies != null && LeagueSharp.Common.Items.HasItem(3142) && LeagueSharp.Common.Items.CanUseItem(3142))
            {
                foreach (var e in HeroManager.Enemies)
                {
                    if (e.Distance(Player) <= Player.AttackRange + 50)
                    {
                        LeagueSharp.Common.Items.UseItem(3142); //youmuu gb
                    }
                }
            }
            if (GetBool("bangplank.menu.item.hydra") && HeroManager.Enemies != null &&
                LeagueSharp.Common.Items.HasItem(3074) && LeagueSharp.Common.Items.CanUseItem(3074))
            {
                foreach (var e in HeroManager.Enemies)
                {
                    if (e.Distance(Player) <= 400)
                    {
                        LeagueSharp.Common.Items.UseItem(3072); //ravenous hydra
                    }
                }
            }
            if (GetBool("bangplank.menu.item.tiamat") && HeroManager.Enemies != null &&
               LeagueSharp.Common.Items.HasItem(3077) && LeagueSharp.Common.Items.CanUseItem(3077))
            {
                foreach (var e in HeroManager.Enemies)
                {
                    if (e.Distance(Player) <= 400)
                    {
                        LeagueSharp.Common.Items.UseItem(3077); //tiamat
                    }
                }
            }


            // random wardafuq e logic op, for sure. (going to be improved)
            if (target == null) return;
            if ((E.Instance.Ammo == 0 || E.Level < 1) && Q.IsReady() && Q.IsInRange(target) && (LiveBarrels.Count == 0 || NearestKeg(Player.Position.To2D()).KegObj.Distance(Player) > Q.Range))
            {
                Q.CastOnUnit(target);
            }
            // TODO [WIP] E mechanics will be improved
            if (GetBool("bangplank.menu.misc.barrelmanager.edisabled") == false && R.Level == 0 && E.IsReady() && (LiveBarrels.Count == 0 || NearestKeg(Player.Position.To2D()).KegObj.Distance(Player) > E.Range)) // 2 keg
            {
                E.Cast(ePrediction);
            }
            if (R.Level == 1 && GetBool("bangplank.menu.misc.barrelmanager.edisabled") == false && E.IsReady()) // 3 Keg
            {
                if ((LiveBarrels.Count == 0 || nbar.KegObj.Distance(Player) > Q.Range) && E.Instance.Ammo >= 3)
                {
                    E.Cast(Player.ServerPosition);
                }
                if ((LiveBarrels.Count == 0 || nbar.KegObj.Distance(Player) > Q.Range) && E.Instance.Ammo <= 3)
                {
                    E.Cast(ePrediction);
                }
            }
            if (R.Level == 2 && GetBool("bangplank.menu.misc.barrelmanager.edisabled") == false && E.IsReady()) // 4 Keg
            {
                if ((LiveBarrels.Count == 0 || nbar.KegObj.Distance(Player) > Q.Range) && E.Instance.Ammo >= 3)
                {
                    E.Cast(Player.ServerPosition);
                }
                if ((LiveBarrels.Count == 0 || nbar.KegObj.Distance(Player) > Q.Range) && E.Instance.Ammo <= 3)
                {
                    E.Cast(ePrediction);
                }
            }
            if (R.Level == 3 && GetBool("bangplank.menu.misc.barrelmanager.edisabled") == false && E.IsReady()) // 5 Keg
            {
                if ((LiveBarrels.Count == 0 || nbar.KegObj.Distance(Player) > Q.Range) && E.Instance.Ammo >= 3 && Player.GetEnemiesInRange(E.Range).Count < 3)
                {
                    E.Cast(Player.ServerPosition);
                }
                if (((LiveBarrels.Count == 0 || nbar.KegObj.Distance(Player) > Q.Range) && E.Instance.Ammo <= 3) || Player.GetEnemiesInRange(E.Range).Count >= 3)
                {
                    E.Cast(ePrediction);
                }
            }
            //Extend if possible and if the number of enemies is below 3
            if (Player.GetEnemiesInRange(E.Range).Count < 3)
            {
                if (((Player.ServerPosition.Distance(nbar.KegObj.Position) < Q.Range && nbar.KegObj.Health < 2) ||
                     (Player.ServerPosition.Distance(nbar.KegObj.Position) < Q.Range && nbar.KegObj.Health == 2 &&
                      Player.Level >= 13)) && GetBool("bangplank.menu.misc.barrelmanager.edisabled") == false)
                {
                    if (target != null)
                    {
                        var prediction = Prediction.GetPrediction(target, 0.8f).CastPosition;
                        if (nbar.KegObj.Distance(prediction) < linkRange)
                        {
                            E.Cast(prediction);
                            if (Player.Level < 13 || nbar.KegObj.Health < 2 && Player.Level >= 13)
                            {
                                Utility.DelayAction.Add((int) (Game.Ping), () =>
                                {
                                    Q.Cast(nbar.KegObj);
                                }
                                    );
                            }
                            // Faster cast
                            if (Player.Level >= 13 && nbar.KegObj.Health == 2)
                            {
                                Utility.DelayAction.Add((int) (400 - Game.Ping), () =>
                                {
                                    Q.Cast(nbar.KegObj);
                                }
                                    );
                            }
                        }
                    }
                }
            }
            
            if (GetBool("bangplank.menu.combo.r") && R.IsReady() && target.GetEnemiesInRange(600).Count + 1 > Getslider("bangplank.menu.combo.rmin") && target.HealthPercent < 30)
            {
                R.Cast(Prediction.GetPrediction(target, R.Delay).CastPosition);
            }
            BarrelManager();
                      
        }

        private static void WaveClear()
        {
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
            var jungleMobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral);
            minions.AddRange(jungleMobs);

            if (GetBool("bangplank.menu.misc.barrelmanager.edisabled") == false && GetBool("bangplank.menu.farm.ewc") && E.IsReady())
            {
                var posE = E.GetCircularFarmLocation(minions, explosionRange);
                if (posE.MinionsHit > Getslider("bangplank.menu.farm.eminwc") && (LiveBarrels.Count == 0 || NearestKeg(Player.ServerPosition.To2D()).KegObj.Distance(Player) > E.Range) && E.Instance.Ammo > Getslider("bangplank.menu.misc.barrelmanager.stacks"))
                {
                    E.Cast(posE.Position);
                }
                // Jungle
                if (jungleMobs.Count >= 1)
                {
                    if (GetBool("bangplank.menu.misc.barrelmanager.edisabled") == false &&
                        GetBool("bangplank.menu.farm.ewc") && E.IsReady() && (LiveBarrels.Count == 0 || NearestKeg(Player.ServerPosition.To2D()).KegObj.Distance(Player) > Q.Range) && E.Instance.Ammo > Getslider("bangplank.menu.misc.barrelmanager.stacks"))
                    {
                        E.Cast(jungleMobs.FirstOrDefault().Position);
                    }
                }
            }

            if (GetBool("bangplank.menu.farm.qewc") && 
                    Player.ManaPercent > Getslider("bangplank.menu.farm.qewcmana") && 
                    Q.IsReady() &&
                    Q.IsInRange(NearestKeg(Player.ServerPosition.To2D()).KegObj) && 
                    NearestKeg(Player.ServerPosition.To2D()).KegObj.Health < 2 &&
                    NearestKeg(Player.ServerPosition.To2D()).KegObj.Distance(minions.FirstOrDefault()) <= explosionRange)
                    Q.Cast(NearestKeg(Player.ServerPosition.To2D()).KegObj);

                if (Player.ServerPosition.Distance(NearestKeg(Player.ServerPosition.To2D()).KegObj.Position) <
                    Player.AttackRange &&
                    NearestKeg(Player.ServerPosition.To2D()).KegObj.IsTargetable &&
                    NearestKeg(Player.ServerPosition.To2D()).KegObj.Health < 2 &&
                    NearestKeg(Player.ServerPosition.To2D()).KegObj.IsValidTarget())

                    Player.IssueOrder(GameObjectOrder.AttackUnit, NearestKeg(Player.ServerPosition.To2D()).KegObj);

            // Items to clear
            if (GetBool("bangplank.menu.item.hydra") &&
                (MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 390).Count > 2 || MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 390, MinionTypes.All, MinionTeam.Neutral).Count >= 1) &&
                LeagueSharp.Common.Items.HasItem(3074) && 
                LeagueSharp.Common.Items.CanUseItem(3074))
            {
                LeagueSharp.Common.Items.UseItem(3074); //hydra, range of active = 400
            }
            if (GetBool("bangplank.menu.item.tiamat") && 
                (MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 390).Count > 2 || MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 390, MinionTypes.All, MinionTeam.Neutral).Count >= 1) &&
                LeagueSharp.Common.Items.HasItem(3077) &&
                LeagueSharp.Common.Items.CanUseItem(3077))
            {
                LeagueSharp.Common.Items.UseItem(3077); //tiamat, range of active = 400
            }
        }

        private static void Mixed()
        {
            
            // harass
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            // Q lasthit minions
            var minions = MinionManager.GetMinions(Player.ServerPosition, Q.Range);
            Keg nbar = NearestKeg(Player.ServerPosition.To2D());

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
            if (GetBool("bangplank.menu.harass.q") && Q.IsReady() && Player.ManaPercent >= Getslider("bangplank.menu.harass.qmana") && TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical) != null
              
                )
            {
                if (LiveBarrels.Count == 0) Q.Cast(TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical));
                if (LiveBarrels.Count >= 1 && nbar.KegObj.Distance(Player) > Q.Range) Q.Cast(TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical));
            }

            // Extended EQ, done but still some bugs remaining, trying to fix them #TODO
            if (Q.IsReady() && E.IsReady() && GetBool("bangplank.menu.harass.extendedeq") && GetBool("bangplank.menu.misc.barrelmanager.edisabled") == false && Player.ManaPercent >= Getslider("bangplank.menu.harass.qmana"))
            {
                if (LiveBarrels.Count == 0) return;             
                if (nbar == null) return;
               
                if ((Player.ServerPosition.Distance(nbar.KegObj.Position) < Q.Range && nbar.KegObj.Health < 2) || (Player.ServerPosition.Distance(nbar.KegObj.Position) < Q.Range && nbar.KegObj.Health == 2 && Player.Level >= 13))
                {
                    if (target != null)
                    {
                        var prediction = Prediction.GetPrediction(target, 0.8f).CastPosition;                     
                        if (nbar.KegObj.Distance(prediction) < linkRange)
                        {                          
                            E.Cast(prediction);
                            if (Player.Level < 13 || nbar.KegObj.Health < 2 && Player.Level >= 13)
                            {
                                Utility.DelayAction.Add((int)(Game.Ping), () =>
                                {
                                    Q.Cast(nbar.KegObj);
                                }
                              );
                            }
                            // Faster cast
                            if (Player.Level >= 13 && nbar.KegObj.Health == 2)
                            {
                                Utility.DelayAction.Add((int)(400 - Game.Ping), () =>
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
            if (Player.InShop()) return;
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

        // Ks "logic" kappa
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
                            // Prevent overkill
                            if (ks.Health <= Player.GetSpellDamage(ks, SpellSlot.Q) && Q.IsInRange(ks)) return;

                            if (ks.Health <= Player.GetSpellDamage(ks, SpellSlot.R)*6 && ks.Health > 0)
                            {
                                var ksposition = Prediction.GetPrediction(ks, 0.9f).CastPosition;
                                
                                if (ksposition.Distance(ks.Position) < 400 && ks.IsMoving)
                                {
                                    ksposition = ks.Position.Extend(ksposition, 450);
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
           if (LiveBarrels.Count == 0) return;
            foreach (var k in LiveBarrels)
            {
                if (Q.IsReady() && Q.IsInRange(k.KegObj) && k.KegObj.GetEnemiesInRange(explosionRange).Count > 0 && k.KegObj.Health < 2)
                    Q.Cast(k.KegObj);
                if (Player.Distance(k.KegObj) <= Player.AttackRange &&
                    k.KegObj.GetEnemiesInRange(explosionRange).Count > 0 && k.KegObj.Health < 2 &&
                    k.KegObj.IsValidTarget() &&
                    k.KegObj.IsTargetable)
                    Player.IssueOrder(GameObjectOrder.AttackUnit, k.KegObj);
            }
                       
        }

        private static void Potion()
        {
            if (Player.InFountain()) return;
            if (Player.IsRecalling()) return;
            if (Player.InShop()) return;
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
            ((Player.HealthPercent <= Getslider("bangplank.menu.item.potion.crysthealth") &&
            Player.ManaPercent <= Getslider("bangplank.menu.item.potion.crystmana")) ||
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