using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Leplank
{
    class BarrelsManager
    {
        #region Definitions
        //Lists
        public static List<Barrel> savedBarrels = new List<Barrel>(); //Liste contenant le barrils vivants
        public static List<List<Barrel>> barrelChains = new List<List<Barrel>>(); //Liste contenant les chaines de barrils (liste)

        //Barrel class
        internal class Barrel
        {
            public Obj_AI_Minion barrel;
            public float time;
            public Barrel(Obj_AI_Minion objAiBase, int tickCount)
            {
                barrel = objAiBase;
                time = tickCount;
            }
        }
        #endregion Definitions
        
        #region OnBarrelCreation
        //On barrel spawn += 1 barrel
        public static void _OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Barrel")
            {
                savedBarrels.Add(new Barrel(sender as Obj_AI_Minion, Environment.TickCount));
                chainManagerOnCreate();
                //debugBarrels(); //Debug
            }
            

        }
        #endregion OnBarrelCreation

        #region OnBarrelDelete
        //On barrel delete (no health, Game_onDelete have huge delay ~1sec, to put on Game_OnUpdate)
        public static void _OnDelete(EventArgs args)
        {
            for (int i = 0; i < savedBarrels.Count; i++)
            {
                if (savedBarrels[i].barrel.Health < 1)
                {
                    chainManagerOnDelete(savedBarrels[i]);
                    savedBarrels.RemoveAt(i);
                    //debugBarrels(); //Debug
                    return;
                }
                
            }

        }
        #endregion OnBarrelDelete

        #region Debug&TestsZone
        //Debug zone (for tests)
        /*
        public static void _DebugZone (EventArgs args)
        {

            

        }

        public static void debugBarrels()
        {
            //Debug
            Game.PrintChat("[ ---------- Il y a " + barrelChains.Count.ToString() + " chaines ----------]");
            for (int i = 0; i < barrelChains.Count; i++)
            {
                Game.PrintChat("Dans la chaine portant l'index " + i.ToString() + " il y'a " + barrelChains[i].Count.ToString() + " barrils");
            }
           

        }*/
        #endregion Debug&TestsZone

        #region ChainManager
        //Chain manager
        public static void chainManagerOnCreate()
        {
            //Partie I : On mets le barril dans la chaine connecté à lui (au moins un barril de cette chaine est connecté à lui)
            Barrel lastBarrelAdded = savedBarrels[savedBarrels.Count - 1];

            bool addedAtLeastOnce = false; //il est pas ajouté
            //Scan la liste à la recherche d'un barril connecté au notre
            for (int i = 0; i < barrelChains.Count; i++) //1) scan les chaines
            {

                for (int j = 0; j < barrelChains[i].Count; j++) //2 scan les barrils dans la chaine et verifie si on est connecté à un
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
            if (!addedAtLeastOnce) //S'il rentre dans aucune liste on rajoute une nouvelle chaine
            {
                barrelChains.Add(new List<Barrel> { lastBarrelAdded });
            }
            //Merge duplicate

            //Pour chaque chaine
            for (int i=0; i<barrelChains.Count;i++)
            {
                //Pour chaque deuxieme chaine differente de la premiere
                for (int j = 0; j < barrelChains.Count; j++)
                {
                    if (i!=j)
                    {
                        //Pour chaque barril
                        for (int k=0;k<savedBarrels.Count;k++)
                        {
                            if (barrelChains[i].Contains(savedBarrels[k]) && barrelChains[j].Contains(savedBarrels[k])) //Si le barril existe dans les deux
                            {
                                //On mix
                                barrelChains[i].AddRange(barrelChains[j].Where(x => !barrelChains[i].Contains(x)));
                                barrelChains.RemoveAt(j);
                            }
                        }
                    }
                }
            }

           


        }
        public static void chainManagerOnDelete(Barrel deletedBarrel)
        {
            //Pour chaque chaine de barrils
            for (int i=0;i<barrelChains.Count;i++)
            {
                if(barrelChains[i].Contains(deletedBarrel)) //Si la chaine contient ce barril
                {
                    //On l'enleve
                    int index = barrelChains[i].IndexOf(deletedBarrel);
                    barrelChains[i].RemoveAt(index);
                    //Si la chaine contient que ce barril, on enleve toute la chaine
                    if(barrelChains[i].Count==0)
                    {
                        barrelChains.RemoveAt(i);
                    }
                }
            }
 
        }
        #endregion ChainManager

        #region MiscFonctions
        //Return closest barrel to a position
        public static Barrel closestToPosition(Vector3 position)
        {
            if (!savedBarrels.Any())
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
            double aX = Math.Round(barrelToConnect.barrel.Position.X + vX / magV * 670); //680 = range for connection
            double aY = Math.Round(barrelToConnect.barrel.Position.Y + vY / magV * 670);
            Vector2 newPosition = new Vector2(Convert.ToInt32(aX), Convert.ToInt32(aY));
            return newPosition;
        }

        //Donne le barril le plus proche d'une position pour enchainer jusuqu'au barril donné
        public static Barrel giveClosestToChainToBarrel(Vector3 closestToThisPosition, Barrel barrelToChainTo)
        {
            Barrel closest = null;
            //Cherche la chaine contenant ce barril
            for (int i=0;i<barrelChains.Count;i++)
            {
                if (barrelChains[i].Contains(barrelToChainTo))
                {
                    for (int k=0;k<barrelChains[i].Count;k++)
                    {
                        if (!barrelChains[i].Any())
                            return null;
                        float bestSoFar = -1;


                        for (int j = 0; j < barrelChains[i].Count; j++)
                        {
                            if (bestSoFar == -1 || barrelChains[i][j].barrel.Distance(closestToThisPosition) < bestSoFar)
                            {
                                bestSoFar = barrelChains[i][j].barrel.Distance(closestToThisPosition);
                                closest = barrelChains[i][j];
                            }
                        }
                    }
                }
            }
            return closest;

        }
        #endregion MiscFonction

    }
}