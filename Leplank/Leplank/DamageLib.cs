using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using LeagueSharp;


namespace Leplank
{
    class DamageLib
    {
        #region SpellDamages
        public static float GetQDamages(Obj_AI_Base qTarget)
        {
            float qdamages;
            if (Items.HasItem(3087) && Program.Player.HasBuff("itemstatikshankcharge") &&
                Program.Player.GetBuffCount("itemstatikshankcharge") == 100) // Statik and 100 buffstacks
            {
                qdamages =
                    (float)
                        Program.Player.CalcDamage(qTarget, Damage.DamageType.Physical,
                            20 + ((Program.Q.Level - 1)*25) + Program.Player.GetAutoAttackDamage(qTarget) + 100 - 30);
            }
            else
            {
                qdamages =
                    (float)
                        Program.Player.CalcDamage(qTarget, Damage.DamageType.Physical,
                            20 + ((Program.Q.Level - 1)*25) + Program.Player.GetAutoAttackDamage(qTarget)-20);
            }

            if (Program.Player.FlatCritChanceMod > 70) // if critchance over 70%, it is considered as it will always crit
            {
                qdamages =
                    (float)
                        Program.Player.CalcDamage(qTarget, Damage.DamageType.Physical,
                            qdamages*Program.Player.FlatCritDamageMod);
            }
            if (Items.HasItem(3078) && Items.CanUseItem(3078)) // Trinity
            {
                qdamages =
                  (float)
                      Program.Player.CalcDamage(qTarget, Damage.DamageType.Physical,
                           qdamages + 2 * Program.Player.BaseAttackDamage);
            }
            else if (Items.HasItem(3057) && Items.CanUseItem(3057)) // Sheen
            {
                qdamages =
                    (float)
                        Program.Player.CalcDamage(qTarget, Damage.DamageType.Physical,
                            qdamages + 1*Program.Player.BaseAttackDamage);
            }
            else if (Items.HasItem(3025) && Items.CanUseItem(3025)) // Iceborn Gauntlet
            {
                qdamages =
                    (float)
                        Program.Player.CalcDamage(qTarget, Damage.DamageType.Physical,
                            qdamages + 1.25 * Program.Player.BaseAttackDamage);
            }
            return qdamages;
        }

        // OK, death daughter todo
        public static float GetRDamages(Obj_AI_Base rTarget)
        {
            float rdamages;
            if (Program.Player.HasBuff("gangplankrupgrade1"))
            {
                 rdamages = (float) Program.Player.CalcDamage(rTarget, Damage.DamageType.Magical,
                   (540 + (360 * Program.R.Level) + (Program.Player.FlatMagicDamageMod * 1.8)));
            }
            else 
            {
                rdamages = (float)Program.Player.CalcDamage(rTarget, Damage.DamageType.Magical,
                   (360 + (240 * Program.R.Level) + (Program.Player.FlatMagicDamageMod * 1.2)));
            }
            return rdamages;
        }

        public static float GetEDamages(Obj_AI_Base eTarget, bool usingQ)
        {
            float edamages;
            if (eTarget.IsChampion())
            {
                if (usingQ)
                {
                    edamages =
                        (float)
                            Program.Player.CalcDamage(eTarget, Damage.DamageType.Physical,
                                (30 + (30*Program.E.Level) + GetQDamages(eTarget))); // 40% armor ignored todo
                }
                else
                {
                    edamages =
                        (float)
                            Program.Player.CalcDamage(eTarget, Damage.DamageType.Physical,
                                (30 + (30*Program.E.Level) + Program.Player.GetAutoAttackDamage(eTarget)));
                    // 40% armor ignored todo
                }
            }
            else
            {
                if (usingQ)
                {
                    edamages =
                        (float)
                            Program.Player.CalcDamage(eTarget, Damage.DamageType.Physical,
                                GetQDamages(eTarget)); // 40% armor ignored todo
                }
                else
                {
                    edamages =
                        (float)
                            Program.Player.CalcDamage(eTarget, Damage.DamageType.Physical,
                                Program.Player.GetAutoAttackDamage(eTarget));
                    // 40% armor ignored todo
                }
            }
            return edamages;
        }

        #endregion SpellDamages
        public static float GetComboDamages(Obj_AI_Base comboTarget)
        {
            float combodamages = GetEDamages(comboTarget, true) + GetRDamages(comboTarget);

                return combodamages;
            }


        
    }
}

