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
            comboMenu.AddItem(new MenuItem("Leplank.combo.logic", "Combo mode").SetValue(new StringList(new []{"Classic", "Barrel lord™"})));
            comboMenu.AddItem(new MenuItem("Leplank.combo.q", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("Leplank.combo.e", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("Leplank.combo.r", "Use R").SetValue(true));
            comboMenu.AddItem(new MenuItem("Leplank.combo.rmin", "Minimum enemies to cast R").SetTooltip("Minimum enemies to hit with R in combo").SetValue(new Slider(2, 1, 5)));
            // Harass Menu
            var harassMenu = new Menu("Harass", "Leplank.harass");
            harassMenu.AddItem(new MenuItem("Leplank.harass.toggle", "Toggle harass").SetTooltip("To harass you can use mixed key or this toggle").SetValue(new KeyBind(67, KeyBindType.Toggle)));
            harassMenu.AddItem(new MenuItem("Leplank.harass.q", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("Leplank.harass.extendedeq", "Extended EQ").SetValue(true));
            harassMenu.AddItem(new MenuItem("Leplank.harass.qmana", "Minimum mana to use Q harass").SetValue(new Slider(30, 0, 100)));
            // Laneclear Menu
            var laneclearMenu = new Menu("Laneclear", "Leplank.lc");
            laneclearMenu.AddItem(new MenuItem("Leplank.lc.e", "Use E to Laneclear").SetTooltip("Also used in Jungle").SetValue(true));
            laneclearMenu.AddItem(new MenuItem("Leplank.lc.emin", "Minimum minions to use E").SetValue(new Slider(3, 1, 15)));
            laneclearMenu.AddItem(new MenuItem("Leplank.lc.qone", "Use Q on E").SetValue(true));
            laneclearMenu.AddItem(new MenuItem("Leplank.lc.qonemana", "Minimum mana to use Q on E").SetValue(new Slider(5, 0, 100)));
            // Lasthit Menu
            var lasthitMenu = new Menu("Lasthit", "Leplank.lh");
            lasthitMenu.AddItem(new MenuItem("Leplank.lh.q", "Use Q").SetValue(true));
            lasthitMenu.AddItem(new MenuItem("Leplank.lh.qmana", "Minimum mana for Q lasthit").SetValue(new Slider(5, 0, 100)));
            // Barrel Manager 
            var barrelManagerMenu = new Menu("Barrel Manager", "Leplank.misc.barrelmanager");
            barrelManagerMenu.AddItem(new MenuItem("Leplank.misc.barrelmanager.edisabled", "Block E usage").SetValue(false));
            barrelManagerMenu.AddItem(new MenuItem("Leplank.misc.barrelmanager.estacks", "Number of stacks to keep for combo").SetTooltip("If Set to 0, it won't keep any stacks").SetValue(new Slider(1, 0, 4)));
           // barrelManagerMenu.AddItem(new MenuItem("Leplank.misc.barrelmanager.autoexplode", "Auto explode when enemy in explosion range").SetValue(true));
            // Cleanser W Manager Menu
            var cleanserManagerMenu = new Menu("W cleanser", "Leplank.cleansermanager");
            cleanserManagerMenu.AddItem(new MenuItem("Leplank.cleansermanager.enabled", "Enabled").SetValue(true));
            cleanserManagerMenu.AddItem(new MenuItem("Leplank.cleansermanager.mana", "Minimum mana").SetValue(new Slider(10, 0, 100)));
            cleanserManagerMenu.AddItem(new MenuItem("Leplank.cleansermanager.health", "Maximum Health to use W").SetTooltip("If above won't use W cleanser").SetValue(new Slider(100, 0, 100)));
            cleanserManagerMenu.AddItem(new MenuItem("Leplank.cleansermanager.delay", "Dealay (ms)").SetValue(new Slider(50, 0, 500)));
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
            var miscMenu = new Menu("Misc", "Leplank.misc");

            miscMenu.AddItem(new MenuItem("Leplank.misc.rksnotif", "R killable notification").SetValue(true));
            miscMenu.AddItem(new MenuItem("Leplank.misc.fleekey", "Flee").SetValue(new KeyBind(65, KeyBindType.Press)));
            // Auto Events
            var eventsMenu = new Menu("Events", "Leplank.misc.events");
            eventsMenu.AddItem(new MenuItem("Leplank.misc.events.qlhtoggle", "Q LastHit toggle").SetValue(new KeyBind(66, KeyBindType.Toggle)));
            eventsMenu.AddItem(new MenuItem("Leplank.misc.events.wheal", "Use W to heal").SetTooltip("Enable auto W heal(won't cancel recalls)").SetValue(true));
            eventsMenu.AddItem(new MenuItem("Leplank.misc.events.healmin", "Health %").SetValue(new Slider(25, 0, 100)));
            eventsMenu.AddItem(new MenuItem("Leplank.misc.events.healminmana", "Minimum Mana %").SetTooltip("Minimum mana to use W heal").SetValue(new Slider(20, 0, 100)));
            eventsMenu.AddItem(new MenuItem("Leplank.misc.events.qks", "Q to KillSecure").SetValue(true));
            // Items Manager Menu
            var itemManagerMenu = new Menu("Items Manager", "Leplank.item");
            var potionManagerMenu = new Menu("Potions", "Leplank.item.potion");
            potionManagerMenu.AddItem(new MenuItem("Leplank.item.potion.enabled", "Enabled").SetTooltip("If off, won't use any potions").SetValue(true));
            potionManagerMenu.AddItem(new MenuItem("Leplank.item.potion.hp", "Health Potion").SetValue(true));
            potionManagerMenu.AddItem(new MenuItem("Leplank.item.potion.hphealth", "Health %").SetValue(new Slider(60, 0, 100)));
            potionManagerMenu.AddItem(new MenuItem("Leplank.item.potion.biscuit", "Biscuit").SetValue(true));
            potionManagerMenu.AddItem(new MenuItem("Leplank.item.potion.biscuithealth", "Health %").SetValue(new Slider(60, 0, 100)));
            potionManagerMenu.AddItem(new MenuItem("Leplank.item.potion.refpot", "Refillable Potion").SetValue(true));
            potionManagerMenu.AddItem(new MenuItem("Leplank.item.potion.repothealth", "Health %").SetValue(new Slider(60, 0, 100)));
            potionManagerMenu.AddItem(new MenuItem("Leplank.tem.potion.corrupt", "Corrupting Potion").SetValue(true));
            potionManagerMenu.AddItem(new MenuItem("Leplank.item.potion.corrupthealth", "Health %").SetValue(new Slider(60, 0, 100)));
            potionManagerMenu.AddItem(new MenuItem("Leplank.tem.potion.hunter", "Hunter's Potion").SetValue(true));
            potionManagerMenu.AddItem(new MenuItem("Leplank.item.potion.hunterhealth", "Health %").SetValue(new Slider(60, 0, 100)));

            itemManagerMenu.AddItem(new MenuItem("Leplank.item.youmuu", "Use Youmuu's Ghostblade").SetTooltip("Use Youmuu in Combo").SetValue(true));
            itemManagerMenu.AddItem(new MenuItem("Leplank.item.hydra", "Use Ravenous Hydra").SetTooltip("Use Hydra to clear and in Combo").SetValue(true));
            itemManagerMenu.AddItem(new MenuItem("Leplank.item.tiamat", "Use Tiamat").SetTooltip("Use Tiamat to clear and in Combo").SetValue(true));
            // Drawing Menu
            Menu drawingMenu = new Menu("Drawings", "Leplank.drawing");
            drawingMenu.AddItem(new MenuItem("Leplank.drawing.enabled", "Enabled").SetTooltip("If off, will block ALL Leplank drawings").SetValue(true));
            drawingMenu.AddItem(new MenuItem("Leplank.drawing.q", "Draw Q range").SetValue(new Circle(true, Color.DarkBlue)));
            drawingMenu.AddItem(new MenuItem("Leplank.drawing.e", "Draw E range").SetValue(new Circle(true, Color.DarkGreen)));
            drawingMenu.AddItem(new MenuItem("Leplank.drawing.w", "Draw W on healthbar").SetValue(new Circle(true, Color.Orange)));
            drawingMenu.AddItem(new MenuItem("Leplank.drawing.r", "Draw R zone").SetValue(new Circle(true, Color.Black)));
            drawingMenu.AddItem(new MenuItem("Leplank.drawing.onlyReady", "Draw only ready").SetValue(true));

            //Barrel assistant
            Menu barrelAssistant = new Menu("Barrels assistant™", "Leplank.barrelassistant");
            barrelAssistant.AddItem(new MenuItem("Leplank.assistant.enabled", "Enabled").SetValue(true));
            barrelAssistant.AddItem(new MenuItem("Leplank.assistant.DrawEConnection", "Draw E connection line helper").SetTooltip("Green line = barrels will connect, Red line = barrels won't connect").SetValue(true));
            barrelAssistant.AddItem(new MenuItem("Leplank.assistant.DrawECircle", "Draw E connection circle helper").SetTooltip("Green Circle = barrels will connect, Red Circle = barrels won't connect").SetValue(true));
            barrelAssistant.AddItem(new MenuItem("Leplank.assistant.DrawEZone", "Draw E zone").SetValue(new Circle(true, Color.Gray)));
            barrelAssistant.AddItem(new MenuItem("Leplank.assistant.DrawExtended", "Draw E extension").SetValue(new Circle(true, Color.White)));
            barrelAssistant.AddItem(new MenuItem("Leplank.assistant.DrawEBConnection", "Draw barrels chains circles").SetValue(new Circle(true, Color.ForestGreen)));
            barrelAssistant.AddItem(new MenuItem("Leplank.assistant.Thickness", "Lines thickness").SetValue(new Slider(3, 1, 20)));
            barrelAssistant.AddItem(new MenuItem("Leplank.assistant.MaxRange", "Drawings max range").SetTooltip("Max mouse distance from player to draw helpers on mouse").SetValue(new Slider(2000, 100, 5000)));

            _menu.AddSubMenu(orbwalkerMenu);
            _menu.AddSubMenu(targetSelectorMenu);
            _menu.AddSubMenu(comboMenu);
            _menu.AddSubMenu(harassMenu);
            _menu.AddSubMenu(laneclearMenu);
            _menu.AddSubMenu(lasthitMenu);
            _menu.AddSubMenu(miscMenu);
            miscMenu.AddSubMenu(barrelManagerMenu);
            miscMenu.AddSubMenu(eventsMenu);

            _menu.AddSubMenu(cleanserManagerMenu);
            _menu.AddSubMenu(itemManagerMenu);
            itemManagerMenu.AddSubMenu(potionManagerMenu);
            drawingMenu.AddSubMenu(barrelAssistant);
            _menu.AddSubMenu(drawingMenu);
            _menu.AddToMainMenu();
        }

        #region GetValues

        public static bool GetBool(string name)
        {
            return _menu.Item(name).GetValue<bool>();
        }
        public static bool GetColorBool(string name)
        {
            return _menu.Item(name).GetValue<Circle>().Active;
        }

        public static Color GetColor(string name)
        {
            return _menu.Item(name).GetValue<Circle>().Color;
        }

        public static int GetSlider(string name)
        {
            return _menu.Item(name).GetValue<Slider>().Value;
        }

        #endregion GetValues
    }

}
