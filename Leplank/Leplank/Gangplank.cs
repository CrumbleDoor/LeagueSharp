using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
namespace Leplank
{
    class Gangplank
    {

        public static void _Orbwalking(EventArgs args)
        {
            if (Program.Player.IsDead)
            {
                return;
            }
            
            #region ks notif
            if (Menus.GetBool("Leplank.misc.rksnotif") && Program.R.IsReady())
            {
                
                var rkstarget = HeroManager.Enemies.Where(k => k.Health < (DamageLib.GetRDamages(k) / 2) && !k.IsDead && k.IsVisible).ToList();
                var kappa = 0;
                foreach (var ks in rkstarget)
                {
                    kappa++;
                    var pos = Drawing.WorldToScreen(Program.Player.Position);
                    Drawing.DrawText(pos.X - Drawing.GetTextExtent(ks.ChampionName + " KILLABLE WITH R").Width, kappa * 25 + pos.Y - Drawing.GetTextExtent(ks.ChampionName + " KILLABLE WITH R").Height, Color.DarkOrange, ks.ChampionName + " KILLABLE WITH R");

                }

                    foreach (var ks in rkstarget)
                    {
                        if (Environment.TickCount - Program.lastnotif > 10000)
                        {
                            Notifications.AddNotification(new Notification(ks.ChampionName + " IS KILLABLE WITH R", 5000, true).SetTextColor(Color.DarkOrange));
                            Program.lastnotif = Environment.TickCount;
                        }
                    }
              
            }
            #endregion ks notif
            WManager();
            Events();

            #region Orbwalker modes
            var activeOrbwalker = Menus._orbwalker.ActiveMode;
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
                case Orbwalking.OrbwalkingMode.None:
                    break;
            }
            #endregion Orbwalker modes
            if (Menus._menu.Item("Leplank.misc.events.qlhtoggle").GetValue<KeyBind>().Active) // toggle
            {
                LastHit();
            }
            if (Menus._menu.Item("Leplank.harass.toggle").GetValue<KeyBind>().Active)
            {
                Mixed();
            }
        }

        private static void Combo()
        {

            switch (Menus._menu.Item("Leplank.combo.logic").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                   Leplank.Combo.Classic();
                break;
                case 1:       
                   Leplank.Combo.BarrelLord();
                break;
            }
            
        }

        private static void WaveClear()
        {
            var minionswc =
                MinionManager.GetMinions(Program.Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                    .Where(mwc => mwc.SkinName != "GangplankBarrel")
                    .OrderByDescending(mlh => mlh.Distance(Program.Player)).ToList();
            if (!minionswc.Any())
            {
                return;
            }
            #region Items
            // Items
            if (Menus.GetBool("Leplank.item.hydra") &&
                (MinionManager.GetMinions(ObjectManager.Player.ServerPosition, LeagueSharp.Common.Data.ItemData.Ravenous_Hydra_Melee_Only.GetItem().Range).Count > 2 ||
                 MinionManager.GetMinions(ObjectManager.Player.ServerPosition, LeagueSharp.Common.Data.ItemData.Ravenous_Hydra_Melee_Only.GetItem().Range, MinionTypes.All, MinionTeam.Neutral)
                     .Count >= 1) && Items.HasItem(3074) && Items.CanUseItem(3074) && !Orbwalking.CanAttack())
            {
                Items.UseItem(3074); //hydra, range of active = 400
            }
            if (Menus.GetBool("Leplank.item.tiamat") &&
                (MinionManager.GetMinions(ObjectManager.Player.ServerPosition, LeagueSharp.Common.Data.ItemData.Tiamat_Melee_Only.GetItem().Range).Count > 2 ||
                 MinionManager.GetMinions(ObjectManager.Player.ServerPosition, LeagueSharp.Common.Data.ItemData.Tiamat_Melee_Only.GetItem().Range, MinionTypes.All, MinionTeam.Neutral)
                     .Count >= 1) && Items.HasItem(3077) && Items.CanUseItem(3077) && !Orbwalking.CanAttack())
            {
                Items.UseItem(3077); 
            }
            #endregion Items
            if (Menus.GetBool("Leplank.misc.barrelmanager.edisabled") == false &&
                Menus.GetBool("Leplank.lc.e") && Program.E.IsReady())
            {
                var posE = Program.E.GetCircularFarmLocation(minionswc, Program.EexplosionRange);
                if (posE.MinionsHit >= Menus.GetSlider("Leplank.lc.emin") &&
                    (!BarrelsManager.savedBarrels.Any() ||
                     BarrelsManager.closestToPosition(Program.Player.ServerPosition).barrel.Distance(Program.Player) > Program.Q.Range) &&
                    Program.E.Instance.Ammo > Menus.GetSlider("Leplank.misc.barrelmanager.estacks"))
                {
                    Program.E.Cast(posE.Position);
                }
                
             
            }

            if (BarrelsManager.savedBarrels.Any() ||
                BarrelsManager.closestToPosition(Program.Player.ServerPosition).barrel.Distance(Program.Player) <
                Program.Q.Range + 100) // Extra range
            {
                var minionsInERange =
                    MinionManager.GetMinions(
                        BarrelsManager.closestToPosition(Program.Player.ServerPosition).barrel.Position,
                        Program.EexplosionRange, MinionTypes.All, MinionTeam.NotAlly);

                if (Menus.GetBool("Leplank.lc.qone") &&
                    Program.Q.IsInRange(BarrelsManager.closestToPosition(Program.Player.ServerPosition).barrel) &&
                    Program.Q.IsReady() && Program.Player.ManaPercent >= Menus.GetSlider("Leplank.lc.qonemana"))               
                {
                    if ((Program.Q.Level >= 3 &&
                         minionsInERange.Where(m => m.Health < DamageLib.GetEDamages(m, true)).ToList().Count >= 3) ||
                        (Program.Q.Level == 2 &&
                         minionsInERange.Where(m => m.Health < DamageLib.GetEDamages(m, true)).ToList().Count >= 2) ||
                        (Program.Q.Level == 1 &&
                         minionsInERange.Where(m => m.Health < DamageLib.GetEDamages(m, true)).ToList().Any()) ||
                        (Program.Q.Level == 1 && minionsInERange.Count < 2))
                    {
                        ExplosionPrediction.castQ(BarrelsManager.closestToPosition(Program.Player.ServerPosition));
                    }
                }
                if ((!Program.Q.IsReady() || !Menus.GetBool("Leplank.lc.qone") || Program.Player.ManaPercent < Menus.GetSlider("Leplank.lc.qonemana")) &&
                    Program.Player.Distance(BarrelsManager.closestToPosition(Program.Player.ServerPosition).barrel) <
                    Program.Player.AttackRange)
                {
                    ExplosionPrediction.autoAttack(BarrelsManager.closestToPosition(Program.Player.ServerPosition));
                }
            }
        }

        private static void Mixed()
        {
            var target = TargetSelector.GetTarget(Program.E.Range, TargetSelector.DamageType.Physical);
            if (target == null && !Menus._menu.Item("Leplank.harass.toggle").GetValue<KeyBind>().Active) LastHit();
            
            if ((!BarrelsManager.savedBarrels.Any() ||
                 BarrelsManager.closestToPosition(Program.Player.ServerPosition).barrel.Distance(Program.Player) >
                 Program.E.Range) && Program.Q.IsInRange(target) && Program.Q.IsReady() &&
                Menus.GetBool("Leplank.harass.q") &&
                Program.Player.ManaPercent >= Menus.GetSlider("Leplank.harass.qmana"))
            {
                Program.Q.CastOnUnit(target);
            }
            var pred = Prediction.GetPrediction(target, 1f).CastPosition;
            if (BarrelsManager.savedBarrels.Any() && Program.Q.IsReady() && Program.E.IsReady() &&
                Program.Q.IsInRange(BarrelsManager.closestToPosition(Program.Player.ServerPosition).barrel) &&
                Menus.GetBool("Leplank.harass.extendedeq") && !Menus.GetBool("Leplank.misc.barrelmanager.edisabled") &&
                target != null && Program.Player.ManaPercent >= Menus.GetSlider("Leplank.harass.qmana") 
                && BarrelsManager.closestToPosition(Program.Player.ServerPosition).barrel.Health < 3)
            {
                Program.E.Cast(pred);
                ExplosionPrediction.castQ(BarrelsManager.closestToPosition(Program.Player.ServerPosition));
            }
                         
        }
        private static void LastHit()
        {
            var minionlhtarget =
                MinionManager.GetMinions(Program.Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                    .Where(
                        mlh =>
                            mlh.SkinName != "GangplankBarrel" && // It makes the program check if it's not a barrel because Powder Kegs 
                            mlh.Health < DamageLib.GetQDamages(mlh)) // are considered as Obj ai minions so it may cause some bugs if not checked
                    .OrderByDescending(mlh => mlh.MaxHealth)//.ThenBy() // Prioritize minions that's are far from the player
                    .ThenByDescending(mlh => Program.Player.Distance(mlh))
                    .FirstOrDefault();
            if (Menus.GetBool("Leplank.lh.q") && Program.Player.ManaPercent >= Menus.GetSlider("Leplank.lh.qmana") &&
                Program.Q.IsReady() && minionlhtarget != null) // Check config
            {
                Program.Q.CastOnUnit(minionlhtarget);
            }
        }

        private static void WManager()
        {
            if (!Program.W.IsReady() || Program.Player.InFountain() || Program.Player.IsRecalling())
            {
                return;
            }
            #region Cleanser
            if (Menus.GetBool("Leplank.cleansermanager.enabled"))
            {
               if ((
                    (Program.Player.HasBuffOfType(BuffType.Charm) && Menus.GetBool("Leplank.cleansermanager.charm"))
                    || (Program.Player.HasBuffOfType(BuffType.Flee) && Menus.GetBool("Leplank.cleansermanager.flee"))
                    || (Program.Player.HasBuffOfType(BuffType.Polymorph) && Menus.GetBool("Leplank.cleansermanager.polymorph"))
                    || (Program.Player.HasBuffOfType(BuffType.Snare) && Menus.GetBool("Leplank.cleansermanager.snare"))
                    || (Program.Player.HasBuffOfType(BuffType.Stun) && Menus.GetBool("Leplank.cleansermanager.stun"))
                    || (Program.Player.HasBuffOfType(BuffType.Taunt) && Menus.GetBool("Leplank.cleansermanager.taunt"))
                    || (Program.Player.HasBuff("summonerexhaust") && Menus.GetBool("Leplank.cleansermanager.exhaust"))
                    || (Program.Player.HasBuffOfType(BuffType.Suppression) && Menus.GetBool("Leplank.cleansermanager.suppression"))
                   ) && Program.Player.ManaPercent >= Menus.GetSlider("Leplank.cleansermanager.mana") && Program.Player.HealthPercent < Menus.GetSlider("Leplank.cleansermanager.health"))
               {
                   Utility.DelayAction.Add(Menus.GetSlider("Leplank.cleansermanager.delay") + Game.Ping, () =>
                   {
                       Program.W.Cast();
                   });
               }
            }
            #endregion Cleanser

            #region Healer
            if (Menus.GetBool("Leplank.misc.events.wheal") && Program.Player.HealthPercent <= Menus.GetSlider("Leplank.misc.events.healmin") &&
                Program.Player.ManaPercent >= Menus.GetSlider("Leplank.misc.events.healminmana"))
            {
                Utility.DelayAction.Add(50 + Game.Ping, () =>
                {
                    Program.W.Cast();
                }
                );
            }
            #endregion Healer
        }

        private static void Events()
        {
            #region Q KillSecure
            var ksQTarget = HeroManager.Enemies.FirstOrDefault(ks => ks.Health < DamageLib.GetQDamages(ks) && !ks.IsDead);
            if (ksQTarget != null && Menus.GetBool("Leplank.misc.events.qks"))
            {
                if (Program.Q.IsReady() && Program.Q.IsInRange(ksQTarget))
                {
                    Program.Q.CastOnUnit(ksQTarget);
                }
            }
            #endregion Q KillSecure
            #region Potions
            if (Menus.GetBool("Leplank.item.potion.enabled") && !Program.Player.InFountain() && !Program.Player.IsRecalling())
            {
                if (Menus.GetBool("Leplank.item.potion.hp") &&
                Program.Player.HealthPercent <= Menus.GetSlider("Leplank.item.potion.hphealth") &&
                Items.HasItem(2003))
                {
                    if (!Program.Player.HasBuff("RegenerationPotion"))
                    {
                        Items.UseItem(2003);
                    }
                }
                if (Menus.GetBool("Leplank.item.potion.refpot") &&
                    Program.Player.HealthPercent <= Menus.GetSlider("Leplank.item.potion.refpothealth") &&
                    Items.HasItem(2031))
                {
                    if (!Program.Player.HasBuff("ItemCrystalFlask") && Items.CanUseItem(2031)) 
                    {
                        Items.UseItem(2031);
                    }
                }
                if (Menus.GetBool("Leplank.item.potion.corrupt") &&
                Program.Player.HealthPercent <= Menus.GetSlider("Leplank.item.potion.corrupthealth") &&
                Items.HasItem(2033))
                {
                    if (!Program.Player.HasBuff("ItemDarkCrystalFlask") && Items.CanUseItem(2033)) 
                    {
                        Items.UseItem(2033);
                    }
                }
                if (Menus.GetBool("Leplank.item.potion.hunter") &&
                    Program.Player.HealthPercent <= Menus.GetSlider("Leplank.item.potion.hunterhealth") &&
                    Items.HasItem(2032))
                {
                    if (!Program.Player.HasBuff("ItemCrystalFlaskJungle") && Items.CanUseItem(2032))
                    {
                        Items.UseItem(2032);
                    }
                }

                if (Menus.GetBool("Leplank.item.potion.biscuit") &&
                Program.Player.HealthPercent <= Menus.GetSlider("Leplank.item.potion.biscuithealth") &&
                Items.HasItem(2010))
                {
                    if (!Program.Player.HasBuff("ItemMiniRegenPotion"))
                    {
                        Items.UseItem(2010);
                    }
                }              
        }
            #endregion Potions

        }

    }
}
