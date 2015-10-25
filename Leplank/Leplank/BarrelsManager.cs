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
        public static List<List<Barrel>> barrelChains = new List<List<Barrel>>();

        //On barrel spawn += 1 barrel
        public static void _OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Barrel")
            {
                savedBarrels.Add(new Barrel(sender as Obj_AI_Minion));
                chainMangerOnCreate();
                Game.PrintChat(barrelChains.Count.ToString());
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
                
            }

        }

       //Debug zone (for tests)
       public static void _DebugZone (EventArgs args)
        {
            

            
        }
        


        //Chain manager
        public static void chainMangerOnCreate()
        {
            //Partie I : On mets le barril dans la chaine connecté à lui (au moins un barril de cette chaine est connecté à lui)
            Barrel lastBarrelAdded = savedBarrels[savedBarrels.Count-1];

            bool addedAtLeastOnce = false; //il est pas ajouté
            //Scan la liste à la recherche d'un barril connecté au notre
            for (int i=0;i<barrelChains.Count;i++) //1) scan les chaines
            {
                
                for (int j=0;j<barrelChains[i].Count;j++) //2 scan les barrils dans la chaine et verifie si on est connecté à un
                {
                    
                    if (lastBarrelAdded.barrel.Distance(barrelChains[i][j].barrel) <= 680) 
                    {
                        
                        //Rajoute à la liste si on y est pas dejà
                        if (!barrelChains[i].Contains(lastBarrelAdded))
                        {
                            barrelChains[i].Add(lastBarrelAdded);
                            addedAtLeastOnce = true;
                        }
                    }
                }
            }
            if(!addedAtLeastOnce) //S'il rentre dans aucune liste on rajoute une nouvelle chaine
            {
                barrelChains.Add(new List<Barrel> { lastBarrelAdded });
            }

            //Partie II netoyyage de la liste de chaines (si un barril existe dans deux liste, on mix les deux chaines)
           

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
            double aX = Math.Round(barrelToConnect.barrel.Position.X + vX / magV * -680); //680 = range for connection
            double aY = Math.Round(barrelToConnect.barrel.Position.Y + vY / magV * 680);
            Vector2 newPosition = new Vector2(Convert.ToInt32(aX), Convert.ToInt32(aY));
            return newPosition;
        }


    }
}