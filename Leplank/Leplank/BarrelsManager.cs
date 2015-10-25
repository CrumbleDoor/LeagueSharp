using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Leplank
{
    class BarrelsManager
    {
        //Barrel class
        internal class Barrel
        {
            public Obj_AI_Minion barrel;
            public Barrel(Obj_AI_Minion objAiBase)
            {
                barrel = objAiBase;
            }
        }

        //Saved barrels list (living ones)
        public static List<Barrel> savedBarrels = new List<Barrel>();

        //On barrel spawn += 1 barrel
        public static void _OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Barrel")
            {
                savedBarrels.Add(new Barrel(sender as Obj_AI_Minion));
            }
        }

        //On barrel delete (no health, Game_onDelete have huge delay ~1sec, to put on Game_OnUpdate)
        public static void _OnDelete(EventArgs args)
        {
            for (int i = 0; i < savedBarrels.Count; i++)
            {
                if (savedBarrels[i].barrel.Health < 1)
                {
                    savedBarrels.RemoveAt(i);
                    return;
                }
                Game.PrintChat(giveConnectedBarrelsTo(1).ToString());
            }

        }
        
        //Return connected barrels (chain) to a barrel indexes (including itself)
        public static List<int> giveConnectedBarrelsTo(int barrelIndex)
        {
            List<int> indexesList = new List<int>();
            //Loop untill reach the barrelIndex
            for (int i = 0; i < barrelIndex; i++)
            {
                if (i > 0)
                {
                    for (int k = 0; k < BarrelsManager.savedBarrels.Count() - 1; k++)
                    {
                        if (BarrelsManager.savedBarrels[i].barrel.Distance(BarrelsManager.savedBarrels[k].barrel.Position) <= 680) //680 = range for connection
                        {
                            if (!indexesList.Contains(i)) //Prevent duplication
                                indexesList.Add(i);
                            if (k == 0)
                            {
                                if (!indexesList.Contains(k))
                                    indexesList.Add(k);
                            }
                        }
                    }
                }

            }
            return indexesList;
        }
        
        //Return closest barrel to a position
        public static Barrel closestToPosition(Vector3 position)
        {
            if (savedBarrels.Count() == 0)
                return null;
            Barrel closest = null;
            float bestSoFar = -1;


            for (int i = 0; i < savedBarrels.Count; i++)
            {
                if (bestSoFar == -1 || savedBarrels[i].barrel.Distance(position) < bestSoFar)
                {
                    bestSoFar = savedBarrels[i].barrel.Distance(position);
                    closest = savedBarrels[i];
                }
            }
            return closest;
        }

        //Correct given position so it will connect to a barrel to that position at max range
        public static Vector2 correctThisPosition(Vector2 position, Barrel barrelToConnect)
        {
            double vX = position.X - barrelToConnect.barrel.Position.X;
            double vY = position.Y - barrelToConnect.barrel.Position.Y;
            double magV = Math.Sqrt(vX * vX + vY * vY);
            double aX = Math.Round(barrelToConnect.barrel.Position.X + vX / magV * - 680); //680 = range for connection
            double aY = Math.Round(barrelToConnect.barrel.Position.Y + vY / magV * 680);
            Vector2 newPosition = new Vector2(Convert.ToInt32(aX), Convert.ToInt32(aY));
            return newPosition;
        }


    }
}