using System.Drawing;
using System.Linq.Expressions;
using LeagueSharp.Common;



namespace Leplank
{
    class Menus
    {
        public static Menu _menu;
        public static Orbwalking.Orbwalker _orbwalker;

        public static void MenuIni()
        {

            // Main Menu
            _menu = new Menu("Leplank", "Leplank", true);
            // Orbwalker Menu
            var orbwalkerMenu = new Menu("Orbwalker", "Leplank.orbwalker");
            _orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            // Target Selector Menu
            var targetSelectorMenu = new Menu("Target Selector", "Leplank.targetselector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            // Combo Menu
            var comboMenu = new Menu("Combo", "Leplank.combo");
            comboMenu.AddItem(new MenuItem("Leplank.combo.q", "Use Q"));
            comboMenu.AddItem(new MenuItem("Leplank.combo.e", "Use E"));
            comboMenu.AddItem(new MenuItem("Leplank.combo.r", "Use R").SetValue(true));
            comboMenu.AddItem(new MenuItem("Leplank.combo.rmin", "Minimum enemies to cast R").SetTooltip("Minimum enemies to hit with R in combo").SetValue(new Slider(2, 1, 5)));
            // Harass Menu
            var harassMenu = new Menu("Harass", "Leplank.harass");
            harassMenu.AddItem(new MenuItem("Leplank.harass.q", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("Leplank.harass.extendedeq", "Extended EQ").SetValue(true));
            harassMenu.AddItem(new MenuItem("Leplank.harass.qmana", "Minimum mana to use Q harass").SetValue(new Slider(15, 0, 100)));
            // Laneclear Menu
            var laneclearMenu = new Menu("Laneclear", "Leplank.lc");
            laneclearMenu.AddItem(new MenuItem("Leplank.lc.e", "Use E to Laneclear").SetTooltip("Also used in Jungle").SetValue(true));
            laneclearMenu.AddItem(new MenuItem("Leplank.lc.emin", "Minimum minions to use E").SetValue(new Slider(3, 1, 15)));
            laneclearMenu.AddItem(new MenuItem("Leplank.lc.qone", "Use Q on E").SetValue(true));
            laneclearMenu.AddItem(new MenuItem("Leplank.lc.qonecmana", "Minimum mana to use Q on E").SetValue(new Slider(5, 0, 100)));
            // Lasthit Menu
            var lasthitMenu = new Menu("Lasthit", "Leplank.lh");
            lasthitMenu.AddItem(new MenuItem("Leplank.lh.qlh", "Use Q").SetValue(true));
            lasthitMenu.AddItem(new MenuItem("Leplank.lh.qlhmana", "Minimum mana for Q lasthit").SetValue(new Slider(5, 0, 100)));
            // Barrel Manager 
            var barrelManagerMenu = new Menu("Barrel Manager", "Leplank.barrelmanager");
            barrelManagerMenu.AddItem(new MenuItem("Leplank.barrelmanager.edisabled", "Block E usage").SetValue(false));
            barrelManagerMenu.AddItem(new MenuItem("Leplank.barrelmanager.estacks", "Number of stacks to keep").SetTooltip("If Set to 0, it won't keep any stacks").SetValue(new Slider(1, 0, 4)));
            barrelManagerMenu.AddItem(new MenuItem("Leplank.barrelmanager.autoexplode", "Auto explode when enemy in explosion range").SetValue(true));
            // Cleanser W Manager Menu
            var cleanserManagerMenu = new Menu("W cleanser", "Leplank.cleansermanager");
            cleanserManagerMenu.AddItem(new MenuItem("Leplank.cleansermanager.enabled", "Enabled").SetValue(true));
            cleanserManagerMenu.AddItem(new MenuItem("Leplank.cleansermanager.mana", "Minimum mana").SetValue(new Slider(10, 0, 100)));
            cleanserManagerMenu.AddItem(new MenuItem("Leplank.cleansermanager.delay", "Dealay (ms)").SetValue(new Slider(100, 0, 500)));
            cleanserManagerMenu.AddItem(new MenuItem("Leplank.cleansermanager.separation2", "Buff Types: "));
            cleanserManagerMenu.AddItem(new MenuItem("Leplank.cleansermanager.charm", "Charm").SetValue(true));
            cleanserManagerMenu.AddItem(new MenuItem("Leplank.cleansermanager.flee", "Flee").SetTooltip("Fear").SetValue(true));
            cleanserManagerMenu.AddItem(new MenuItem("Leplank.cleansermanager.polymorph", "Polymorph").SetValue(true));
            cleanserManagerMenu.AddItem(new MenuItem("Leplank.cleansermanager.snare", "Snare").SetValue(true));
            cleanserManagerMenu.AddItem(new MenuItem("Leplank.cleansermanager.stun", "Stun").SetValue(true));
            cleanserManagerMenu.AddItem(new MenuItem("Leplank.cleansermanager.taunt", "Taunt").SetValue(true));
            cleanserManagerMenu.AddItem(new MenuItem("Leplank.cleansermanager.exhaust", "Exhaust").SetTooltip("Will only remove Slow").SetValue(false));
            cleanserManagerMenu.AddItem(new MenuItem("Leplank.cleansermanager.suppression", "Supression").SetValue(true));
            // Misc Menu
            var miscMenu = new Menu("Misc", "Leplank");
            miscMenu.AddItem(new MenuItem("Leplank.misc.wheal", "Use W to heal").SetTooltip("Enable auto W heal(won't cancel recall if low)").SetValue(true));
            miscMenu.AddItem(new MenuItem("Leplank.misc.healmin", "Health %").SetTooltip("If under, will use W").SetValue(new Slider(30)));
            miscMenu.AddItem(new MenuItem("Leplank.misc.healminmana", "Minimum Mana %").SetTooltip("Minimum mana to use W heal").SetValue(new Slider(35)));
            miscMenu.AddItem(new MenuItem("Leplank.misc.qks", "Q to KillSecure").SetValue(true));
            miscMenu.AddItem(new MenuItem("Leplank.misc.rksnotif", "R killable notification").SetValue(true));
            miscMenu.AddItem(new MenuItem("Leplank.misc.fleekey", "Flee").SetValue(new KeyBind(65, KeyBindType.Press)));
            // Items Manager Menu
            var itemManagerMenu = new Menu("Items Manager", "Leplank.item");
            var potionManagerMenu = new Menu("Potions", "Leplank.item.potion");
            potionManagerMenu.AddItem(new MenuItem("Leplank.item.potion.enabled", "Enabled").SetTooltip("If off, won't use any potions").SetValue(true));
            potionManagerMenu.AddItem(new MenuItem("Leplank.item.potion.hp", "Health Potion").SetValue(true));
            potionManagerMenu.AddItem(new MenuItem("Leplank.item.potion.hphealth", "Health %").SetValue(new Slider(35, 0, 100)));
            potionManagerMenu.AddItem(new MenuItem("Leplank.item.potion.mp", "Mana Potion").SetValue(true));
            potionManagerMenu.AddItem(new MenuItem("Leplank.item.potion.mana", "Mana %").SetValue(new Slider(30, 0, 100)));
            potionManagerMenu.AddItem(new MenuItem("Leplank.item.potion.biscuit", "Biscuit").SetValue(true));
            potionManagerMenu.AddItem(new MenuItem("Leplank.item.potion.biscuithealth", "Health %").SetValue(new Slider(35, 0, 100)));
            potionManagerMenu.AddItem(new MenuItem("Leplank.item.potion.cryst", "Crystalline Flask").SetValue(true));
            potionManagerMenu.AddItem(new MenuItem("Leplank.item.potion.crysthealth", "Health %").SetValue(new Slider(40, 0, 100)));
            potionManagerMenu.AddItem(new MenuItem("Leplank.item.potion.crystmana", "Mana %").SetValue(new Slider(40, 0, 100)));
            
            itemManagerMenu.AddItem(new MenuItem("Leplank.item.youmuu", "Use Youmuu's Ghostblade").SetTooltip("Use Youmuu in Combo").SetValue(true));
            itemManagerMenu.AddItem(new MenuItem("Leplank.item.hydra", "Use Ravenous Hydra").SetTooltip("Use Hydra to clear and in Combo").SetValue(true));
            itemManagerMenu.AddItem(new MenuItem("Leplank.item.tiamat", "Use Tiamat").SetTooltip("Use Tiamat to clear and in Combo").SetValue(true));
            // Drawing Menu
            Menu drawingMenu = new Menu("Drawing", "Leplank.drawing");
            drawingMenu.AddItem(new MenuItem("Leplank.drawing.enabled", "Enabled").SetTooltip("If off, will block Leplank drawings").SetValue(true));
            drawingMenu.AddItem(new MenuItem("Leplank.drawing.q", "Draw Q range").SetValue(true));
            drawingMenu.AddItem(new MenuItem("Leplank.drawing.e", "Draw E range").SetValue(true));

            _menu.AddSubMenu(orbwalkerMenu);
            _menu.AddSubMenu(targetSelectorMenu);
            _menu.AddSubMenu(comboMenu);
            _menu.AddSubMenu(harassMenu);
            _menu.AddSubMenu(laneclearMenu);
            _menu.AddSubMenu(lasthitMenu);
            _menu.AddSubMenu(barrelManagerMenu);
            _menu.AddSubMenu(miscMenu);
            _menu.AddSubMenu(cleanserManagerMenu);
            _menu.AddSubMenu(itemManagerMenu);
            itemManagerMenu.AddSubMenu(potionManagerMenu);         
            _menu.AddSubMenu(drawingMenu);
            _menu.AddToMainMenu();
        }




    }
    
}
