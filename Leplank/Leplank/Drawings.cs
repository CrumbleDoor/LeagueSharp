using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace Leplank
{
    class Drawings
    {
        public static bool Assisting = false;
        public static void _OnDraw (EventArgs args)
        {
            if (Menus.GetBool("Leplank.drawing.enabled"))
            {
                #region SpellsDrawings
                if (Menus.GetBool("Leplank.drawing.onlyReady"))
                {
                    if(Menus.GetColorBool("Leplank.drawing.q") && (Program.Q.IsReady() && Program.Q.Level > 0))
                        Render.Circle.DrawCircle(Program.Player.Position, Program.Q.Range, Menus.GetColor("Leplank.drawing.q"));

                    if (Menus.GetColorBool("Leplank.drawing.e") && (Program.E.IsReady() && Program.E.Level > 0))
                        Render.Circle.DrawCircle(Program.Player.Position, Program.E.Range, Menus.GetColor("Leplank.drawing.e"));

                    if(Menus.GetColorBool("Leplank.drawing.w") && Program.W.IsReady() && Program.Player.HealthPercent < 95)
                    {
                        float Heal = new int[] { 50, 75, 100, 125, 150 }[Program.W.Level - 1] +
                                         (Program.Player.MaxHealth - Program.Player.Health) * 0.15f + Program.Player.FlatMagicDamageMod * 0.9f;
                        float mod = Math.Max(100f, Program.Player.Health + Heal) / Program.Player.MaxHealth;
                        float xPos = (float)((double)Program.Player.HPBarPosition.X + 36 + 103.0 * mod);
                        Drawing.DrawLine(
                            xPos, Program.Player.HPBarPosition.Y + 8, xPos, (float)((double)Program.Player.HPBarPosition.Y + 17), 2f,
                            Menus.GetColor("Leplank.drawing.w"));
                    }

                    if (Menus.GetColorBool("Leplank.drawing.r") && (Program.R.IsReady() && Program.R.Level > 0))
                    {
                        Render.Circle.DrawCircle(Game.CursorPos, Program.Rzone, Menus.GetColor("Leplank.drawing.r"));
                        if (Program.Player.HasBuff("GangplankRUpgrade2"))
                            Render.Circle.DrawCircle(Game.CursorPos, Program.RdeathDaughter, Menus.GetColor("Leplank.drawing.r"));
                    }

                }
                else
                {
                    if(Menus.GetColorBool("Leplank.drawing.q"))
                        Render.Circle.DrawCircle(Program.Player.Position, Program.Q.Range, Menus.GetColor("Leplank.drawing.q"));

                    if (Menus.GetColorBool("Leplank.drawing.e"))
                        Render.Circle.DrawCircle(Program.Player.Position, Program.E.Range, Menus.GetColor("Leplank.drawing.e"));

                    if (Menus.GetColorBool("Leplank.drawing.w") && Program.Player.HealthPercent < 95)
                    {
                        float Heal = new int[] { 50, 75, 100, 125, 150 }[Program.W.Level - 1] +
                                         (Program.Player.MaxHealth - Program.Player.Health) * 0.15f + Program.Player.FlatMagicDamageMod * 0.9f;
                        float mod = Math.Max(100f, Program.Player.Health + Heal) / Program.Player.MaxHealth;
                        float xPos = (float)((double)Program.Player.HPBarPosition.X + 36 + 103.0 * mod);
                        Drawing.DrawLine(
                            xPos, Program.Player.HPBarPosition.Y + 8, xPos, (float)((double)Program.Player.HPBarPosition.Y + 17), 2f,
                            Menus.GetColor("Leplank.drawing.w"));
                    }

                    if (Menus.GetColorBool("Leplank.drawing.r"))
                    {
                        Render.Circle.DrawCircle(Game.CursorPos, Program.Rzone, Menus.GetColor("Leplank.drawing.r"));
                        if (Program.Player.HasBuff("GangplankRUpgrade2"))
                            Render.Circle.DrawCircle(Game.CursorPos, Program.RdeathDaughter, Menus.GetColor("Leplank.drawing.r"));
                    }
                }
                #endregion SpellsDrawings

                #region barrelsAssistantTM
                if (Menus.GetBool("Leplank.assistant.enabled"))
                {
                    //Draw E zone
                    if (Menus.GetColorBool("Leplank.assistant.DrawEZone") && Program.Player.Distance(Game.CursorPos) <= Menus.GetSlider("Leplank.assistant.MaxRange") && !Assisting)
                        Render.Circle.DrawCircle(Game.CursorPos, Program.Ezone, Menus.GetColor("Leplank.assistant.DrawEZone"), Menus.GetSlider("Leplank.assistant.Thickness"));

                    //Connection Circle helper
                    if (Menus.GetBool("Leplank.assistant.DrawECircle") && Program.Player.Distance(Game.CursorPos) <= Menus.GetSlider("Leplank.assistant.MaxRange") && Game.CursorPos.Distance(BarrelsManager.closestToPosition(Game.CursorPos).barrel.Position) <= Program.Econnection + 200 && BarrelsManager.savedBarrels.Count > 0)
                    {
                        Assisting = true;
                        if (BarrelsManager.closestToPosition(Game.CursorPos).barrel.Distance(Game.CursorPos) <= Program.Econnection)
                        {
                            Render.Circle.DrawCircle(Game.CursorPos, Program.Ezone, Color.LawnGreen, Menus.GetSlider("Leplank.assistant.Thickness"));
                        }
                        else
                        {
                            Render.Circle.DrawCircle(Game.CursorPos, Program.Ezone, Color.Red, Menus.GetSlider("Leplank.assistant.Thickness"));
                        }

                    }
                    else
                    {
                        Assisting = false;
                    }

                    //Connection Line helper
                    if (Menus.GetBool("Leplank.assistant.DrawEConnection") && Program.Player.Distance(Game.CursorPos) <= Menus.GetSlider("Leplank.assistant.MaxRange") && Game.CursorPos.Distance(BarrelsManager.closestToPosition(Game.CursorPos).barrel.Position) <= Program.Econnection+200 && BarrelsManager.savedBarrels.Count > 0)
                    {
                        Assisting = true;
                        if (BarrelsManager.closestToPosition(Game.CursorPos).barrel.Distance(Game.CursorPos) <= Program.Econnection)
                        {
                            Drawing.DrawLine(Drawing.WorldToScreen(Game.CursorPos), Drawing.WorldToScreen(BarrelsManager.closestToPosition(Game.CursorPos).barrel.Position), Menus.GetSlider("Leplank.assistant.Thickness"), Color.LawnGreen);
                        }
                        else
                        {
                            Drawing.DrawLine(Drawing.WorldToScreen(Game.CursorPos), Drawing.WorldToScreen(BarrelsManager.closestToPosition(Game.CursorPos).barrel.Position), Menus.GetSlider("Leplank.assistant.Thickness"), Color.Red);
                        }

                    }

                    //E extension
                    if (Menus.GetColorBool("Leplank.assistant.DrawExtended") && Program.Player.Distance(Game.CursorPos) <= Menus.GetSlider("Leplank.assistant.MaxRange") && Game.CursorPos.Distance(BarrelsManager.closestToPosition(Game.CursorPos).barrel.Position) <= Program.Econnection + 200 && BarrelsManager.savedBarrels.Count > 0)
                        Render.Circle.DrawCircle(BarrelsManager.closestToPosition(Game.CursorPos).barrel.Position, Program.Econnection+Program.Ezone, Menus.GetColor("Leplank.assistant.DrawExtended"), Menus.GetSlider("Leplank.assistant.Thickness"));

                    //Re-draw barrels connections
                    if (Menus.GetColorBool("Leplank.assistant.DrawEBConnection") && BarrelsManager.barrelChains.Count > 0 && BarrelsManager.savedBarrels.Count > 0)
                    {
                        for (int i = 0; i < BarrelsManager.barrelChains.Count; i++)
                        {
                            if (BarrelsManager.barrelChains[i].Count > 1)
                            {
                                for (int k = 0; k < BarrelsManager.barrelChains[i].Count; k++)
                                {
                                    Render.Circle.DrawCircle(BarrelsManager.barrelChains[i][k].barrel.Position, Program.Ezone, Menus.GetColor("Leplank.assistant.DrawEBConnection"));
                                }
                            }
                        }
                    }
                }

                #endregion barrelsAssistantTM

            }
        }
    }
}
