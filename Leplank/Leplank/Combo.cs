using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Leplank
{
    class Combo
    {
        public static void Classic()
        {
            BarrelLord();
        }

       //to rework, fcked up with new common update :(
        public static void BarrelLord()
        {
            #region BarrelLord           
            
            var enemies = HeroManager.Enemies.Where(e => e.Distance(Program.Player) < Program.E.Range && !e.IsDead).ToList();
            var target = TargetSelector.GetTarget(Program.E.Range, TargetSelector.DamageType.Physical);
            var bar = BarrelsManager.closestToPosition(Program.Player.ServerPosition);
            if (target == null) return;

            //ok
            #region R
            if (Menus.GetBool("Leplank.combo.r") && Program.R.IsReady())
            {
                var rfocus =
                    HeroManager.Enemies.FirstOrDefault(e => e.Health < DamageLib.GetRDamages(e)*2 && e.Distance(Program.Player) <= 2000 && (e.GetAlliesInRange(850).Count > 0));
                if (rfocus != null)
                {
                    Program.R.CastIfWillHit(rfocus, Menus.GetSlider("Leplank.combo.rmin")); 
                }
            }
            #endregion R
            #region Qks
            var ksQTarget = HeroManager.Enemies.FirstOrDefault(ks => ks.Health < DamageLib.GetQDamages(ks) && !ks.IsDead);
            if (ksQTarget != null && Menus.GetBool("Leplank.misc.events.qks"))
            {
                if (Program.Q.IsReady() && Program.Q.IsInRange(ksQTarget))
                {
                    Program.Q.CastOnUnit(ksQTarget);
                }
            }
            #endregion Qks
          
                    //itemsok
                    #region items
                    if (Menus.GetBool("Leplank.item.hydra") && target != null && Items.HasItem(3074))
                    {
                        if (!Orbwalking.CanAttack() && Items.CanUseItem(3074) && Program.Player.Distance(target) < LeagueSharp.Common.Data.ItemData.Ravenous_Hydra_Melee_Only.GetItem().Range)
                        {
                            Utility.DelayAction.Add(30, () =>
                            {
                                Items.UseItem(3074);
                            }
                                );
                        }
                    }
                    if (Menus.GetBool("Leplank.item.tiamat") && target != null && Items.HasItem(3077))
                    {
                        if (!Orbwalking.CanAttack() && Items.CanUseItem(3077) && Program.Player.Distance(target) < LeagueSharp.Common.Data.ItemData.Tiamat_Melee_Only.GetItem().Range)
                        {
                            Utility.DelayAction.Add(30, () =>
                            {
                                Items.UseItem(3077);
                            }
                                );
                        }
                    }
                    if (Menus.GetBool("Leplank.item.youmuu") && target != null && Items.HasItem(3142))
                    {
                        if (Items.CanUseItem(3142) && Program.Player.Distance(target) < Program.Player.AttackRange)
                        {
                            Items.UseItem(3142);
                        }
                    }
                    #endregion items

                    if (Program.E.IsReady() && (!BarrelsManager.savedBarrels.Any() || bar.barrel.Distance(Program.Player) > Program.E.Range) && target != null)
                    {
                        if (Menus.GetBool("Leplank.combo.e") && !Menus.GetBool("Leplank.misc.barrelmanager.edisabled"))
                        {
                            Program.E.Cast(Prediction.GetPrediction(target, 1f).CastPosition);
                        }
                    }

                    var coolbar = BarrelsManager.savedBarrels.FirstOrDefault(b => b.barrel.GetEnemiesInRange(Program.EexplosionRange-20).Count > 0 && b.barrel.Health < 3);
                    if (Menus.GetBool("Leplank.combo.q") && coolbar != null)
                    {
                        if (Program.Q.IsReady() && Program.Q.IsInRange(coolbar.barrel) && target != null)
                        {
                            ExplosionPrediction.castQ(coolbar);
                        }
                    }

                    //aabarrelsok
                    if (Orbwalking.CanAttack() && coolbar != null)
                    {
                        if ((!Program.Q.IsReady() || Menus.GetBool("Leplank.combo.q")) &&
                            coolbar.barrel.Distance(Program.Player) <= Program.Player.AttackRange && target != null)
                        {
                            ExplosionPrediction.autoAttack(coolbar);
                        }
                    }

                    var focusbar = BarrelsManager.savedBarrels[BarrelsManager.savedBarrels.Count - 1];
                    //QE when enemy is in E range & when the nearest barrel is in Q range and can be connected with a barrel where target can be hit
                    if (Program.Q.IsReady() && Program.E.IsReady() && focusbar.barrel.Distance(Program.Player) < Program.Q.Range && target != null && focusbar.barrel.Distance(target) > Program.EexplosionRange)
                    {
                        var pred = Prediction.GetPrediction(target, 1f).CastPosition;

                        if (focusbar.barrel.Distance(pred) <=Program.Econnection && 
                            Menus.GetBool("Leplank.combo.e") && Menus.GetBool("Leplank.combo.q") &&
                            !Menus.GetBool("Leplank.misc.barrelmanager.edisabled") &&
                            focusbar.barrel.Health < 3)
                        {         
                        ExplosionPrediction.castQ(BarrelsManager.savedBarrels[BarrelsManager.savedBarrels.Count - 1]);
                           
                                Utility.DelayAction.Add((int) (Program.Q.Delay), () =>
                                {
                                    Program.E.Cast(BarrelsManager.correctThisPosition(Prediction.GetPrediction(target, 1f).CastPosition.To2D(), focusbar));
                                }
                                    );
                        }
                    }
                    #region Q on enemy
                    if (Menus.GetBool("Leplank.combo.q") && (Program.E.Instance.Ammo == 0 || !Program.E.IsReady()) && Program.Q.IsReady() &&
                        Program.Q.IsInRange(target) &&
                        (!BarrelsManager.savedBarrels.Any() || bar.barrel.Distance(Program.Player.ServerPosition) > 1200))
                    {
                        Program.Q.CastOnUnit(target);
                    }
                    #endregion Q on enemy
                    #region R
                    if (Menus.GetBool("Leplank.combo.r") && Program.R.IsReady() && Menus.GetSlider("Leplank.combo.rmin") == 1)
                    {
                        if (target.HealthPercent < 50)
                        {
                            if (target.IsMoving)
                            {
                                Program.R.Cast(Prediction.GetPrediction(target, 1f).CastPosition);
                            }
                            else
                            {
                                Program.R.Cast(target.Position);
                            }
                        }

                    }
                    #endregion R


            #endregion BarrelLord
        }


    }
}
