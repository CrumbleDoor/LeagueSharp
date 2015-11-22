using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Leplank
{

    //Health decay prediction, use Q  or AA (within Q range or AA range) to perfectly timed selected barrel explosion
    class ExplosionPrediction
    {
        public static void castQ (BarrelsManager.Barrel targetBarrel)
        {
            
                float time;

                if(Program.Player.Level<7)
                    time = 4f * 1000;
                else if(Program.Player.Level >=7 && Program.Player.Level < 13)
                    time = 2f * 1000;
                else
                    time = 1f * 1000;



                var qq = Environment.TickCount - targetBarrel.time + (Program.Player.Distance(targetBarrel.barrel) / 2800f + Program.Q.Delay) * 700;
                if (targetBarrel.barrel.Distance(Program.Player) <= Program.Q.Range)
                {
                
                    if (Utility.DelayAction.ActionList.Count==0)
                        Utility.DelayAction.Add(Convert.ToInt32(time - qq), () => Program.Q.CastOnUnit(targetBarrel.barrel));
                    
                }

                

        }

        public static float GetQtime(BarrelsManager.Barrel targetBarrel)
        {
            float time;
            if (Program.Player.Level < 7)
                time = 4f * 1000;
            else if (Program.Player.Level >= 7 && Program.Player.Level < 13)
                time = 2f * 1000;
            else
                time = 1f * 1000;

            var qq = Environment.TickCount - targetBarrel.time + (Program.Player.Distance(targetBarrel.barrel) / 2800f + Program.Q.Delay) * 700;
            var result = time - qq;
            return result;
        }

        public static void autoAttack (BarrelsManager.Barrel targetBarrel)
        {
            float time;
            if (Program.Player.Level < 7)
                time = 4f * 1000;
            else if (Program.Player.Level >= 7 && Program.Player.Level < 13)
                time = 2f * 1000;
            else
                time = 1f * 1000;

            var qq = Environment.TickCount - targetBarrel.time + Program.Player.AttackDelay;
            if (targetBarrel.barrel.Distance(Program.Player) <= Program.Player.AttackRange)
            {
                if (Utility.DelayAction.ActionList.Count == 0)
                    Utility.DelayAction.Add(Convert.ToInt32(time - qq), () => Program.Player.IssueOrder(GameObjectOrder.AttackUnit, targetBarrel.barrel));
            }
            
        }

        //Quickscope : Attempt to cast QE quickcombo to target position, pos : target position (use correctThisPos for optimal results), targettarget (1hp required) : the initial barrel to use, if the target barrel is within E range and distance to player is greater than 500 (avoid long backward walking) it will move to it before casting the combo
        public static void quickscope(BarrelsManager.Barrel target, Vector2 pos)
        {

            bool canconnect = false;
            bool shouldmove = false;
            bool inposition = false;
            bool done = false;

            //Verify if the wanted position is within range
            if (Program.Player.Position.Distance(pos.To3D()) < Program.E.Range)
            {
                canconnect = true;
            }
            else
            {
                canconnect = false;
            }

            //Verify if we should move in order to get the combo
            if ((Program.Player.Distance(target.barrel) > 610 && Program.Player.Distance(target.barrel) < 1000) || (Program.Player.Distance(target.barrel) < 590 && Program.Player.Distance(target.barrel) > 500))
                shouldmove = true;
            else if (Program.Player.Distance(target.barrel) >= 590 && Program.Player.Distance(target.barrel) <= 610)
            {
                shouldmove = false;
                inposition = true;
            }
            else
                shouldmove = false;

            //If requirements are K we move
            if (shouldmove && canconnect)
            {
                Vector3 position = Program.Player.Position;
                double vX = position.X - target.barrel.Position.X;
                double vY = position.Y - target.barrel.Position.Y;
                double magV = Math.Sqrt(vX * vX + vY * vY);
                double aX = Math.Round(target.barrel.Position.X + vX / magV * 600);
                double aY = Math.Round(target.barrel.Position.Y + vY / magV * 600);
                Vector2 newPosition = new Vector2(Convert.ToInt32(aX), Convert.ToInt32(aY));
                if (position.Distance(target.barrel.Position) - 580 >= 50) //If correction is far from hero
                    Program.Player.IssueOrder(GameObjectOrder.MoveTo, newPosition.To3D());
                else //If correction is within hero hitbox (wont move cauz distance too small)
                {
                    Program.Player.IssueOrder(GameObjectOrder.MoveTo, new Vector2(Program.Player.Position.X - 200, Program.Player.Position.Y - 200).To3D());
                    Utility.DelayAction.Add(50, () => Program.Player.IssueOrder(GameObjectOrder.MoveTo, newPosition.To3D()));
                }

            }
            //If all is K we quickscope
            if (inposition && target.barrel.Health == 1 && !done)
            {
                Program.Q.CastOnUnit(target.barrel);
                Program.E.Cast(pos);
                done = true;
            }

        }

    }
}
