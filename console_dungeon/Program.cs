using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace console_dungeon
{
    internal class Program
    {
        public abstract class character
        {
            public string name;
            public string[] texture; //17 x 6
            public int health;
            public int max_health;
            public int armor;
            public int max_armor;
            public int damage;
            public bool alive = true;
            public int position;
            public int stamina = 20;

            public character(string name, string[] texture, int health, int armor, int damage)
            {
                this.name = name;
                this.texture = texture;
                this.health = health;
                this.max_health = health;
                this.armor = armor;
                this.max_armor = armor;
                this.damage = damage;
            }

            public character(character ch)
            {
                this.name = ch.name;
                this.texture = ch.texture;
                this.health = ch.health;
                this.max_health = ch.health;
                this.armor = ch.armor;
                this.max_armor = ch.armor;
                this.damage = ch.damage;
            }

            public virtual void TakeDamage(int damage)
            {
                if (armor > 0)
                {
                    armor -= damage / 2;
                    if (armor <= 0)
                        armor = 0;
                }
                else
                {
                    health -= damage;
                    if (health <= 0)
                        alive = false;
                }
            }

        }

        public class hero : character
        {
            public string[] description;
            public string weapon;
            public string[] slots;
            public hero(string name, string[] texture, int health, int armor, int damage, string[] description, string weapon) : base(name, texture, health, armor, damage)
            {
                this.description = description;
                this.weapon = weapon;
                position = 0;
            }

            public hero(hero h) : base(h)
            {
                this.description = h.description;
                this.weapon = h.weapon;
                position = 0;
            }

            public void moveLeft()
            {
                if (stamina >= 10)
                {
                    if (position >= 1)
                    {
                        position -= 1;
                        stamina -= 10;
                        LogEvent(name + " se posunul doleva");
                    }
                    else
                        BadActionMessage = true;
                }
                else
                    NotEnoughStaminaMessage = true;
            }

            public void moveRight(enemy e)
            {
                if (stamina >= 10)
                {
                    if (position < 6 && e.position != position + 1)
                    {
                        position += 1;
                        stamina -= 10;
                        LogEvent(name + " se posunul doprava");
                    }
                    else
                        BadActionMessage = true;
                }
                else
                    NotEnoughStaminaMessage = true;
            }
            public virtual void attack(enemy e)
            {

            }
            public virtual void defend(enemy e)
            {

            }
            public virtual void special(enemy e)
            {

            }

            public void regenerateStamina()
            {
                stamina = 20;
            }

            public virtual void card(int row)
            {
                if (row == 0)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("  ┌─────────────────────────────────────┐  ");
                }

                if (row >= 1 && row != 2 && row != 6 && row < 9)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("  │");
                }

                if (row == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(TextPad(name, 37));
                }

                if (row == 2)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("  ├─────────────────────────────────────┤  ");
                }

                if (row == 3)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(TextPad(description[0], 37));
                }
                if (row == 4)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(TextPad(description[1], 37));
                }
                if (row == 5)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(TextPad(description[2], 37));
                }

                if (row == 6)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("  ├──────────────────┬──────────────────┤  ");
                }

                if (row == 7)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(TextPad(max_health + " <3", 18));

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("│");

                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(TextPad(damage + " --+", 18));
                }

                if (row == 8)
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.Write(TextPad(armor + " 'H'", 18));

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("│");

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write(TextPad(weapon, 18));
                }

                if (row == 9)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("  └──────────────────┴──────────────────┘  ");
                }

                if (row >= 1 && row != 2 && row != 6 && row < 9)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("│  ");
                }

                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public class warrior : hero
        {
            private bool using_shield = false;
            public warrior(string name, string[] texture, int health, int armor, int damage, string[] description, string weapon) : base(name, texture, health, armor, damage, description, weapon)
            {
                slots = new string[] { "left", "right", "sword", "shield", "potion" };
            }

            public warrior(warrior w) : base(w)
            {
                slots = new string[] { "left", "right", "sword", "shield", "potion" };
            }
            public override void attack(enemy e)
            {
                if (stamina >= 5)
                {
                    if (position + 1 == e.position)
                    {
                        stamina -= 5;
                        e.TakeDamage(damage);
                        LogEvent(name + " zaútočil na " + e.name);
                    }
                    else
                        BadActionMessage = true;
                }
                else
                    NotEnoughStaminaMessage = true;
            }

            public override void defend(enemy e)
            {
                if (stamina >= 10)
                {
                    stamina -= 10;
                    using_shield = true;
                    LogEvent(name + " používá štít");
                }
                else
                    NotEnoughStaminaMessage = true;
            }

            public override void special(enemy e)
            {
                if (stamina >= 15)
                {
                    if (health < max_health)
                    {
                        stamina -= 15;
                        health += 5;
                        if (health >= max_health)
                            health = max_health;
                        LogEvent(name + " použil lektvar");
                    }
                    else
                        BadActionMessage = true;
                }
                else
                    NotEnoughStaminaMessage = true;
            }

            public override void TakeDamage(int damage)
            {
                if (using_shield)
                {
                    using_shield = false;
                }
                else
                {
                    base.TakeDamage(damage);
                }
            }
        }

        public class wizard : hero
        {
            public wizard(string name, string[] texture, int health, int armor, int damage, string[] description, string weapon) : base(name, texture, health, armor, damage, description, weapon)
            {
                slots = new string[] { "left", "right", "stick", "fireball", "curse" };
            }

            public wizard(wizard w) : base(w)
            {
                slots = new string[] { "left", "right", "stick", "fireball", "curse" };
            }

            public override void attack(enemy e)
            {
                if (stamina >= 5)
                {
                    if (position + 1 == e.position)
                    {
                        stamina -= 5;
                        e.TakeDamage(damage / 2);
                        LogEvent(name + " zaútočil na " + e.name);
                    }
                    else
                        BadActionMessage = true;
                }
                else
                    NotEnoughStaminaMessage = true;
            }

            public override void defend(enemy e)
            {
                if (stamina >= 10)
                {
                    if (position + 3 >= e.position && position + 1 != e.position)
                    {
                        stamina -= 10;
                        e.TakeDamage(damage);
                        LogEvent(name + " vyvolal ohnivou kouli");
                    }
                    else
                        BadActionMessage = true;
                }
                else
                    NotEnoughStaminaMessage = true;
            }

            public override void special(enemy e)
            {
                if (stamina >= 15)
                {
                    if (health < max_health)
                    {
                        stamina -= 15;
                        health += 5;
                        if (health >= max_health)
                            health = max_health;
                        e.health -= 5;
                        LogEvent(name + " použil prokletí");
                    }
                    else
                        BadActionMessage = true;
                }
                else
                    NotEnoughStaminaMessage = true;
            }
        }

        public class archer : hero
        {
            public archer(string name, string[] texture, int health, int armor, int damage, string[] description, string weapon) : base(name, texture, health, armor, damage, description, weapon)
            {
                slots = new string[] { "left", "right", "bow", "strong_bow", "stunning_knife" };
            }

            public archer(archer a) : base(a)
            {
                slots = new string[] { "left", "right", "bow", "strong_bow", "stunning_knife" };
            }

            public override void attack(enemy e)
            {
                if (stamina >= 5)
                {
                    if (position + 3 >= e.position && position + 1 != e.position)
                    {
                        stamina -= 5;
                        e.TakeDamage(damage);
                        LogEvent(name + " zaútočil na " + e.name);
                    }
                    else
                        BadActionMessage = true;
                }
                else
                    NotEnoughStaminaMessage = true;
            }

            public override void defend(enemy e)
            {
                if (stamina >= 10)
                {
                    if (position + 3 <= e.position)
                    {
                        stamina -= 10;
                        e.TakeDamage(damage + (damage /2));
                        LogEvent(name + " zaútočil na " + e.name);
                    }
                    else
                        BadActionMessage = true;
                }
                else
                    NotEnoughStaminaMessage = true;
            }

            public override void special(enemy e)
            {
                if (stamina >= 15)
                {
                    if (position + 1 == e.position)
                    {
                        stamina -= 15;
                        e.TakeDamage(damage / 2);
                        if (e.damage > 2)
                            e.damage -= 1;
                        LogEvent(name + " použil jedovatou dýku");
                    }
                    else
                        BadActionMessage = true;
                }
                else
                    NotEnoughStaminaMessage = true;
            }
        }

        public class berserk : hero
        {
            private int wait_until_regen = 0;
            public berserk(string name, string[] texture, int health, int armor, int damage, string[] description, string weapon) : base(name, texture, health, armor, damage, description, weapon)
            {
                slots = new string[] { "left", "right", "axe", "brutal_axe", "potion" };
            }

            public berserk(berserk b) : base(b)
            {
                slots = new string[] { "left", "right", "axe", "brutal_axe", "potion" };
            }

            public override void attack(enemy e)
            {
                if (stamina >= 5)
                {
                    if (position + 1 == e.position)
                    {
                        stamina -= 5;
                        e.TakeDamage(damage);
                        LogEvent(name + " zaútočil na " + e.name);
                    }
                    else
                        BadActionMessage = true;
                }
                else
                    NotEnoughStaminaMessage = true;
            }

            public override void defend(enemy e)
            {
                if (stamina >= 10)
                {
                    if (position + 1 == e.position && health > 2)
                    {
                        stamina -= 10;
                        e.TakeDamage(damage * 3);
                        health -= 2;
                        LogEvent(name + " zaútočil na " + e.name);
                    }
                    else
                        BadActionMessage = true;
                }
                else
                    NotEnoughStaminaMessage = true;
            }

            public override void special(enemy e)
            {
                if (stamina >= 15)
                {
                    if (health < max_health)
                    {
                        stamina -= 15;
                        health += 5;
                        if (health >= max_health)
                            health = max_health;
                        LogEvent(name + " použil lektvar");
                    }
                    else
                        BadActionMessage = true;
                }
                else
                    NotEnoughStaminaMessage = true;
            }
        }


        public class enemy : character
        {
            public enemy(string name, string[] texture, int health, int armor, int damage) : base(name, texture, health, armor, damage)
            {
                position = 6;
            }
            public enemy(enemy e) : base(e)
            {
                position = 6;
            }
            public virtual void Turn(hero player)
            {
                if (stamina <= 0)
                {
                    player.regenerateStamina();
                    stamina = 20;
                    playerTurn = true;
                }
            }
            protected void moveLeft(hero player)
            {
                if (position != player.position + 1 && stamina >= 10)
                {
                    position -= 1;
                    stamina -= 10;
                    LogEvent(name + " se posunul doleva");
                }
                else
                    stamina -= 2;
            }
            protected void moveRight(hero player)
            {
                if (position < 6 && stamina >= 10)
                {
                    position += 1;
                    stamina -= 10;
                    LogEvent(name + " se posunul doprava");
                }
                else
                    stamina -= 2;
            }
            protected void nearAttack(hero player, int multiply = 1)
            {
                if (player.position + 1 == position && stamina >= 7)
                {
                    player.TakeDamage(damage * multiply);
                    stamina -= 7;
                    LogEvent(name + " zaútočil na " + player.name);
                }
                else
                    stamina -= 2;
            }
            protected void longAttack(hero player, int multiply = 1)
            {
                if (player.position >= position - 3 && stamina >= 10)
                {
                    player.TakeDamage(damage * multiply);
                    stamina -= 10;
                    LogEvent(name + " zaútočil na " + player.name);
                }
                else
                    stamina -= 2;
            }
            protected void heal()
            {
                stamina -= 20;
                health += 5;
                if (health >= max_health)
                    health = max_health;
                LogEvent(name + " se vyléčil");
            }

        }

        public class rat : enemy
        {
            public rat(string name, string[] texture, int health, int armor, int damage) : base(name, texture, health, armor, damage)
            {
                position = 6;
            }
            public rat(rat r) : base(r)
            {

            }

            public override void Turn(hero player)
            {
                if (player.position + 1 != position)
                {
                    moveLeft(player);
                }
                else
                {
                    nearAttack(player);
                }
                base.Turn(player);
            }
        }

        public class skeleton : enemy
        {
            public skeleton(string name, string[] texture, int health, int armor, int damage) : base(name, texture, health, armor, damage)
            {
                position = 6;
            }
            public skeleton(skeleton s) : base(s)
            {
                position = 6;
            }

            public override void Turn(hero player)
            {
                if (player.position < position - 2)
                {
                    moveLeft(player);
                }
                if (player.position == position - 2)
                {
                    longAttack(player, 2);
                }
                if (player.position == position - 1)
                {
                    if (rand.Next(0, 4) == 0)
                        moveRight(player);
                    else
                        nearAttack(player);
                }
                base.Turn(player);
            }
        }

        // Main Game Variables
        static string playerName = "Hráč";
        static string option = "";
        static bool playerTurn = true;
        static bool HelpMessage = false;
        static bool NotEnoughStaminaMessage = false;
        static bool BadActionMessage = false;
        static int mode = 0;
        static int level = 1;
        static int max_levels = 1;
        static int difficulty = 1;
        static int new_enemy = 0;
        static int start_page = 0;
        static Random rand = new Random();

        static void Main(string[] args)
        {
            // Window Setup
            Console.Title = "Console Dungeon";
            Console.SetWindowSize(120, 30);

            // Objects Setup
            hero playedHero;
            enemy Enemy;
            warrior hero1 = new warrior("Gladiátor", new string[] { "     ___         ", "    |o o|        ", " # _|_-_|_       ", "### |   | \\      ", "'#'/|___|\\_|====>", "    |_|_|        " }, 30, 10, 5, new string[] { "Nebojácný bojovník, který útočí", "nablízko s mečem, brání se štítem", "a uzdravuje se lektvarem" }, "Meč  ");
            wizard hero2 = new wizard("Čaroděj", new string[] { "     ___         ", "    |o o|  @@    ", "   _|_-_|_ ||    ", "  / | @ | \\||    ", " /_/|___|\\_||    ", "    |_|_|  ||    " }, 20, 5, 6, new string[] { "Mocný čaroděj utočící nablízko", "holí, nadálku vyvolává ohnivou kouli", "a taky může proklít nepřítele" }, "Kouzelná hůl");
            archer hero3 = new archer("Lukostřelec", new string[] { "    \\\\\\\\         ", "    |o o|  |\\    ", "   _|_-_|__| \\   ", "  / | V |__| ||  ", " /_/|___|  | /   ", "    |_|_|  |/    " }, 20, 5, 3, new string[] { "Lukostřelec s přesnou muškou", "útočící na dálku", "sdf" }, "Luk a šípy");
            berserk hero4 = new berserk("Bersekr", new string[] { "    .!!!.  ____  ", "    |o o| <__  \\ ", "   _|_-_|_ ||\\_| ", "  / |   | \\||    ", " /_/|___|\\_||    ", "    |_|_|  ||    " }, 15, 20, 5, new string[] { "Tvrdohlavý bojovník", "sdf", "vbdf" }, "Sekera");


            rat Rat = new rat("Krysa", new string[] { "                 ", "                 ", "  ()()________   ", " /oo \\        \\  ", " \\x    ___    /) ", "  /_/_/  /_/_/   " }, 10, 0, 2);
            rat strongRat = new rat("Velka Krysa", new string[] { "                 ", "                 ", "  ()()________   ", " /oo \\        \\  ", " \\x   |___/   /) ", "  /_/_/  /_/_/   " }, 15, 0, 4);
            skeleton Skeleton = new skeleton("Kostlivec", new string[] { "        ___      ", "       |x x|     ", "      _|_#_|_    ", "     / |_|_| \\   ", "<===|_/|#__|\\_\\  ", "       |_|_|     " }, 15, 5, 2);

            playedHero = new warrior(hero1);
            Enemy = new rat(Rat);

            // Main Game Loop
            while (true)
            {
                if (mode == 0) //Main Menu
                {
                    option = MainMenu();
                    switch (option)
                    {
                        case "h":
                            mode = 2;
                            break;
                        case "p":
                            mode = 1;
                            break;
                        default:
                            HelpMessage = true;
                            break;
                    }
                    if (option == "k")
                        break;
                }

                if (mode == 1) // Help Menu
                {
                    option = HelpMenu();
                    switch (option)
                    {
                        case "1":
                            start_page = 0;
                            break;
                        case "2":
                            start_page = 16;
                            break;
                        case "3":
                            start_page = 16 * 2;
                            break;
                        case "4":
                            start_page = 16 * 3;
                            break;
                        case "5":
                            start_page = 16 * 4;
                            break;
                        case "z":
                            mode = 0;
                            break;

                        default:
                            HelpMessage = true;
                            break;
                    }
                }

                if (mode == 2) // Choose Hero
                {
                    option = ChooseHeroMenu(hero1, hero2, hero3, hero4);
                    switch (option)
                    {
                        case "1":
                            playedHero = new warrior(hero1);
                            playerName = hero1.name;
                            mode = 3;
                            break;
                        case "2":
                            playedHero = new wizard(hero2);
                            playerName = hero2.name;
                            mode = 3;
                            break;
                        case "3":
                            playedHero = new archer(hero3);
                            playerName = hero3.name;
                            mode = 3;
                            break;
                        case "4":
                            playedHero = new berserk(hero4);
                            playerName = hero4.name;
                            mode = 3;
                            break;
                        case "z":
                            mode = 0;
                            break;
                        default:
                            HelpMessage = true;
                            break;
                    }

                }

                if (mode == 3) // Difficulty Menu
                {
                    option = DifficultyMenu();
                    switch (option)
                    {
                        case "1":
                            difficulty = 1;
                            max_levels = 3;
                            mode = 4;
                            break;
                        case "2":
                            difficulty = 2;
                            max_levels = 5;
                            mode = 4;
                            break;
                        case "3":
                            difficulty = 3;
                            max_levels = 8;
                            mode = 4;
                            break;
                        case "z":
                            mode = 0;
                            break;
                        default:
                            HelpMessage = true;
                            break;

                    }
                }

                if (mode == 4) // Level Menu
                {
                    option = LevelMenu(playedHero);
                    if (level < max_levels)
                    {
                        if (difficulty == 1)
                        {
                            new_enemy = rand.Next(0, 2);
                            if (new_enemy == 0)
                                Enemy = new rat(Rat);
                            if (new_enemy == 1)
                                Enemy = new rat(strongRat);
                        }
                        if (difficulty == 2)
                        {
                            new_enemy = rand.Next(0, 4);
                            if (new_enemy <= 1)
                                Enemy = new skeleton(Skeleton);
                            if (new_enemy == 2)
                                Enemy = new rat(Rat);
                            if (new_enemy == 3) 
                                Enemy = new rat(strongRat);
                        }
                        mode = 5;
                    }
                    if (level == max_levels)
                    {
                        mode = 6;
                    }

                }

                if (mode == 5) // Fight Menu
                {
                    option = FightMenu(playedHero, Enemy);
                    switch (option)
                    {
                        case "l":
                            playedHero.moveLeft();
                            break;
                        case "r":
                            playedHero.moveRight(Enemy);
                            break;

                        case "u":
                            playedHero.attack(Enemy);
                            break;

                        case "o":
                            playedHero.defend(Enemy);
                            break;

                        case "s":
                            playedHero.special(Enemy);
                            break;

                        case "k":
                            playerTurn = false;
                            System.Threading.Thread.Sleep(rand.Next(1000, 4000));
                            Enemy.Turn(playedHero);
                            break;

                        default:
                            HelpMessage = true;
                            break;
                    }
                    if (!playedHero.alive)
                    {
                        mode = 6;
                    }
                    if (!Enemy.alive)
                    {
                        playedHero.position = 0;
                        playedHero.regenerateStamina();
                        ClearLogEvent();
                        level += 1;
                        mode = 4;
                    }
                }

                if (mode == 6)
                {
                    option = EndMenu(playedHero);
                    mode = 0;
                    level = 1;
                    playerName = "Hráč";
                }
            }
        }

        static int screen_width = 120;

        static string MainMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("                       ____                      _        ____                                     ");
            Console.WriteLine("                      / ___|___  _ __  ___  ___ | | ___  |  _ \\ _   _ _ __   __ _  ___  ___  _ __  ");
            Console.WriteLine("                     | |   / _ \\| '_ \\/ __|/ _ \\| |/ _ \\ | | | | | | | '_ \\ / _` |/ _ \\/ _ \\| '_ \\ ");
            Console.WriteLine("                     | |__| (_) | | | \\__ \\ (_) | |  __/ | |_| | |_| | | | | (_| |  __/ (_) | | | |");
            Console.WriteLine("                      \\____\\___/|_| |_|___/\\___/|_|\\___| |____/ \\__,_|_| |_|\\__, |\\___|\\___/|_| |_|");
            Console.WriteLine("                                                                            |____/                  \n\n");




            GenerateButton("  Hrát   ", "H");
            GenerateButton("  Pomoc  ", "P");
            GenerateButton("  Konec  ", "K");

            Console.Write("\n\n\n\n\n\n\n");
            return PlayerInput();
        }

        static string helpTextTitle = "                             Jak Hrát hru?                          "; // 68 char
        static string[] helpText =
        {
            " Po zapnutí hry si hráč vybere jednoho z vybraných hrdinů, následně ",
            " obtížnost. Cílem hry je porazit několik nepřátel, nacházejících se ",
            " v jednotlivých místnostech.                                        ",
            "                                                                    ",
            " Souboje probíhají po tazích, v každém tahu může hráč zvolit jednu  ",
            " z možností pro pohyb útok, obranu atd. podle toho kolik mu zbývá   ",
            " staminy, každá akce požaduje různou staminu.                       ",
            "                                                                    ",
            " Ikonky:  <3   - Životy                                             ",
            "         --+   - Síla útoku                                         ",
            "         'H'   - Brnění                                             ",
            "         \\/\\   - Stamina                                            ",
            "                                                                    ",
            "                                                                    ",
            "                                                                    ",
            "                                                        strana 1/5  ",
            " Gladiátor                                                          ",
            "                                                                    ",
            " Útok mečem - je nutné aby se nepřítel nacházel o políčko vedle     ",
            "                                                                    ",
            " Štít - hráč po nasazení štítu se stává imunní vůči veškerému       ",
            "        poškození do doby než nepřítel se na hráče pokusí zaútočit, ",
            "        potom je štít zase neaktivní, hráč tudíž může dostat        ",
            "        poškození                                                   ",
            "                                                                    ",
            " Uzdravující lektvar - uzdraví hráči 5 životů                       ",
            "                                                                    ",
            "                                                                    ",
            "                                                                    ",
            "                                                                    ",
            "                                                                    ",
            "                                                        strana 2/5  ",
            " Čaroděj                                                            ",
            "                                                                    ",
            " Útok holí - je nutné aby se nepřítel nacházel o políčko vedle,     ",
            "             dává poloviční poškození                               ",
            "                                                                    ",
            " Ohnivá koule - nepřítel se nesmí nacházet o políčko vedle a        ",
            "                maximálně o 3 políčka od hráče, dává celé poškození ",
            "                                                                    ",
            " Prokletí - hráč si vezme 5 životů od nepřítele (obnoví si 5 životů ",
            "            nepříteli způsobí poškození 5 životů a to bez ohledu na ",
            "            jeho brnění) na jakoukoliv vzdálenost                   ",
            "                                                                    ",
            "                                                                    ",
            "                                                                    ",
            "                                                                    ",
            "                                                        strana 3/5  ",
            " Lukostřelec                                                        ",
            "                                                                    ",
            " Výstřel z luku - nepřítel se nesmí nacházet o políčko vedle a      ",
            "                  maximálně o 3 políčka od hráče, dává plné         ",
            "                  poškození                                         ",
            "                                                                    ",
            " Silný výstřel z luku - nepřítel se nesmí nacházet o políčko vedle  ",
            "                        a maximálně o 3 políčka od hráče,           ",
            "                        dává 50% bonusového poškození               ",
            "                                                                    ",
            " Útok otrávenou dýkou - nepřítel se musí nacházet o políčko vedle,  ",
            "                        jed nepřítele oslabí a to zmenšením jeho    ",
            "                        poškozením (nejmenší poškození, které může  ",
            "                        nepřítel mít je 2)                          ",
            "                                                                    ",
            "                                                        strana 4/5  ",
            " Bersekr                                                            ",
            "                                                                    ",
            " Útok sekerou - je nutné aby se nepřítel nacházel o políčko vedle,  ",
            "                dává plné poškození                                 ",
            "                                                                    ",
            " Naštvaný útok sekerou - je nutné aby se nepřítel nacházel o        ",
            "                         políčko vedle, dává 3x poškození, ale hráč ",
            "                         se zraní o 2 životy                        ",
            "                                                                    ",
            " Uzdravující lektvar - uzdraví hráči 5 životů                       ",
            "                                                                    ",
            "                                                                    ",
            "                                                                    ",
            "                                                                    ",
            "                                                                    ",
            "                                                        strana 5/5  ",
        };

        static string HelpMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGray;

            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    Console.Write(" ");
                }

                if (i == 0)
                {
                    Console.Write("╔");
                    for (int k = 0; k < 68; k++)
                    {
                        Console.Write("═");
                    }
                    Console.Write("╗\n");
                }

                if (i == 1)
                {
                    Console.Write("║");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(helpTextTitle);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("║\n");
                }

                if (i == 2)
                {
                    Console.Write("╠");
                    for (int k = 0; k < 68; k++)
                    {
                        Console.Write("═");
                    }
                    Console.Write("╣\n");
                }

                if (i >= 3 && i <= 18)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("║");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(helpText[i - 3 + start_page]);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("║\n");
                }

                if (i == 19)
                {
                    Console.Write("╚");
                    for (int k = 0; k < 68; k++)
                    {
                        Console.Write("═");
                    }
                    Console.Write("╝\n");
                }
            }
            Console.Write("                                                                                     ");
            GenerateUse("1 - 5", 0, true);
            GenerateButton("   Zpátky   ", "Z");
            return PlayerInput();
        }

        static string ChooseHeroMenu(hero hero1, hero hero2, hero hero3, hero hero4)
        {
            Console.Clear();

            for (int i = 0; i < 10; i++)
            {
                Console.Write("               ");
                hero1.card(i);
                hero2.card(i);
                Console.Write("\n");
            }
            Console.Write("               ");
            GenerateUse("1", 20, false);
            GenerateUse("2", 20, true);

            for (int i = 0; i < 10; i++)
            {
                Console.Write("               ");
                hero3.card(i);
                hero4.card(i);
                Console.Write("\n");
            }
            Console.Write("               ");
            GenerateUse("3", 20, false);
            GenerateUse("4", 20, true);
            Console.Write("\n");

            GenerateUse("Z - zpátky", 5, false);
            GenerateUse("Vyber si postavu", 26, true, true);

            return PlayerInput();
        }

        static string DifficultyMenu()
        {
            Console.Clear();
            Console.Write("\n\n\n\n");
            GenerateButton("   Lehká       ", "1");
            GenerateButton("   Normální    ", "2");
            GenerateButton("   Těžká       ", "3");
            Console.Write("\n\n\n\n");

            GenerateUse("Vyber si obtížnost", 48, true, true);

            Console.Write("\n\n\n\n\n");
            GenerateUse("Z - zpátky", 5, true);

            return PlayerInput();
        }

        static string LevelMenu(hero player)
        {
            Console.Clear();

            GenerateButton("   " + player.name + " se nachází v " + level + ". podloží   ", "", false);
            Console.Write("\n\n\n\n");
            GenerateMap(max_levels, level);
            Console.Write("\n\n\n\n\n\n\n");
            GenerateUse("pokračovat", 50, true, true);
            Console.Write("\n\n\n\n");
            return PlayerInput();
        }

        static string FightMenu(hero player, enemy enemy)
        {
            Console.Clear();

            if (playerTurn)
                GenerateButton("   " + player.name + " hraje!   ", "", false);
            else
                GenerateButton("   " + enemy.name + " hraje!   ", "", false);
            Console.Write("\n\n");

            BattleField(player, enemy);

            PlayerInventory(player);
            if (playerTurn)
            {
                return PlayerInput();
            }
            else
            {
                return "k";
            }
        }

        static void GenerateMap(int rooms, int position_room)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            int offset = (120 - ((rooms * 10) + ((rooms - 1) * 3))) / 2;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < offset; j++)
                    Console.Write(" ");

                if (i == 0)
                {
                    for (int k = 0; k <= rooms; k++)
                    {
                        if (k != rooms)
                            Console.Write("╔════════╗");
                        else
                            Console.Write("\n");
                        if (k < rooms - 1)
                            Console.Write("   ");
                    }
                }
                if (i == 1)
                {
                    for (int k = 0; k <= rooms; k++)
                    {
                        if (k != rooms)
                            Console.Write("║        ║");
                        else
                            Console.Write("\n");
                        if (k < rooms - 1)
                            Console.Write("───");
                    }
                }
                if (i == 2)
                {
                    for (int k = 1; k <= rooms + 1; k++)
                    {
                        if (k != rooms + 1)
                        {
                            if (k < position_room)
                                Console.Write("║        ║");
                            if (k == position_room)
                            {
                                Console.Write("║  ");
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.Write("<[]>");
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write("  ║");
                            }
                            if (k > position_room)
                            {
                                Console.Write("║   ");
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("[]");
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write("   ║");
                            }
                        }
                        else
                            Console.Write("\n");
                        if (k < rooms)
                            Console.Write("   ");
                    }
                }
                if (i == 3)
                {
                    for (int k = 0; k <= rooms; k++)
                    {
                        if (k != rooms)
                            Console.Write("║        ║");
                        else
                            Console.Write("\n");
                        if (k < rooms - 1)
                            Console.Write("───");
                    }
                }
                if (i == 4)
                {
                    for (int k = 0; k <= rooms; k++)
                    {
                        if (k != rooms)
                            Console.Write("╚════════╝");
                        else
                            Console.Write("\n");
                        if (k < rooms - 1)
                            Console.Write("   ");
                    }
                }
            }
        }

        static string EndMenu(hero player)
        {
            Console.Clear();
            Console.Write("\n\n\n");
            if (player.alive)
            {
                GenerateButton("    " + player.name + " vyhrál!    ", "", false);
            }
            else
            {
                GenerateButton("    " + player.name + " prohrál    ", "", false);
            }
            Console.Write("\n\n\n\n\n\n\n\n\n\n\n\n\n");
            GenerateUse("Vrátit se do hlavního menu", 45, true, true);
            Console.Write("\n\n\n");

            return PlayerInput();
        }

        // Vygeneruje "tlačítko" na obrazovce
        static void GenerateButton(string text, string use, bool generateUse = true)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < ((screen_width - text.Length) / 2); j++)
                {
                    Console.Write(" ");
                }

                if (i == 0)
                {
                    Console.Write("╔");
                    for (int a = 0; a < text.Length; a++)
                    {
                        Console.Write("═");
                    }
                    Console.Write("╗\n");
                }

                if (i == 1)
                {
                    Console.Write("║");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(text);
                    if (generateUse)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("║ <");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(use);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(">\n");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("║\n");
                    }
                }

                if (i == 2)
                {
                    Console.Write("╚");
                    for (int a = 0; a < text.Length; a++)
                    {
                        Console.Write("═");
                    }
                    Console.Write("╝\n");
                }

            }

        }

        static void GenerateUse(string use, int space, bool new_line, bool ask = false)
        {
            string space_char = "";
            for (int i = 0; i < space; i++)
                space_char += " ";
            Console.ForegroundColor = ConsoleColor.DarkGray;
            if (!ask)
                Console.Write(space_char + "<");
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (ask)
                Console.Write(space_char + "[  ");
            Console.Write(use);
            if (ask)
                Console.Write("  ]" + space_char);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            if (!ask)
                Console.Write(">" + space_char);

            if (new_line)
                Console.Write("\n");
        }


        static string TextPad(string text, int char_long, bool toMiddle = false)
        {
            if (toMiddle)
                return new string(' ', (char_long - text.Length) / 2) + text + new string(' ', char_long - text.Length - (char_long - text.Length) / 2);
            else
                return text.PadRight(char_long);
        }


        static void BattleField(hero player, enemy enemy)
        {
            //7
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (player.position > j)
                        Console.Write("                 ");
                }
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(player.texture[i]);

                for (int j = player.position + 1; j < 6; j++)
                {
                    if (enemy.position > j)
                        Console.Write("                 ");
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(enemy.texture[i]);
                Console.Write("\n");
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            for (int i = 0; i < 7; i++)
            {
                Console.Write("  --==%###%==--  ");
            }
            Console.Write("\n\n");

            Console.Write("                                                                                    ╔════════════════════╦═════╦═════╗\n");
            Console.Write("                                                                                    ║");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(TextPad(enemy.name, 20));

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("║");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(TextPad(enemy.health + " <3", 5));

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("║");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(TextPad(enemy.armor + "'H'", 5));

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("║\n");

            Console.Write("                                                                                    ╚════════════════════╩═════╩═════╝\n");
        }

        public static string[] logText = { "", "", "", "" };
        static void LogEvent(string log)
        {
            for (int i = 3; i > 0; i--)
            {
                logText[i] = logText[i - 1];
            }
            if (log.Length <= 32)
                logText[0] = log;
            else 
            {
                logText[0] = "";
                for (int j = 0; j < 32; j++)
                    logText[0] += log[j];
            }
        }
        static void ClearLogEvent()
        {
            for (int i = 0; i < 4; i++)
            {
                logText[i] = "";
            }
        }

        static void PlayerInventory(hero player)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("╔════════════════════════════╗\n");
            Console.Write("║");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(TextPad(player.name, 28));
            Console.ForegroundColor = ConsoleColor.DarkGray;

            Console.Write("╠══════════╦══════════╦══════════╦══════════╦══════════╦════════════════════════════════╗\n");
            Console.Write("╠════════════════════════════╣");
            GenerateSlots(player.slots, 0, false);
            Console.Write(TextPad(logText[3], 32) + "║\n║");
            GenerateBar(player.health, player.max_health, 0);
            GenerateSlots(player.slots, 1);
            Console.Write(TextPad(logText[2], 32) + "║\n║");
            GenerateBar(player.armor, player.max_armor, 1);
            GenerateSlots(player.slots, 2);
            Console.Write(TextPad(logText[1], 32) + "║\n║");
            GenerateBar(player.stamina, 20, 2);
            GenerateSlots(player.slots, 3);
            Console.Write(TextPad(logText[0], 32) + "║\n");
            Console.Write("╚════════════════════════════╩══════════╩══════════╩══════════╩══════════╩══════════╩════════════════════════════════╝\n");
            GenerateUse("K - konec tahu", 7, false);
            GenerateUse("L 10S", 2, false);
            GenerateUse("R 10S", 2, false);
            GenerateUse("U  5S", 2, false);
            GenerateUse("O 10S", 2, false);
            GenerateUse("S 15S", 2, true);
        }

        static void GenerateSlots(string[] args, int row, bool start_line = true)
        {
            if (start_line)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("║");
            }
            foreach (string i in args)
            {
                if (i == "left")
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    if (row == 0)
                        Console.Write("  //      ");
                    if (row == 1)
                        Console.Write(" //------ ");
                    if (row == 2)
                        Console.Write(" \\\\------ ");
                    if (row == 3)
                        Console.Write("  \\\\      ");
                }
                if (i == "right")
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    if (row == 0)
                        Console.Write("      \\\\  ");
                    if (row == 1)
                        Console.Write(" ------\\\\ ");
                    if (row == 2)
                        Console.Write(" ------// ");
                    if (row == 3)
                        Console.Write("      //  ");
                }
                if (i == "sword")
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    if (row == 0)
                        Console.Write("    /\"\\   ");
                    if (row == 1)
                        Console.Write("  _/  /   ");
                    if (row == 2)
                        Console.Write(" \\/__/    ");
                    if (row == 3)
                        Console.Write("  /_/     ");
                }
                if (i == "shield")
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    if (row == 0)
                        Console.Write(" ..----.. ");
                    if (row == 1)
                        Console.Write(" ||    || ");
                    if (row == 2)
                        Console.Write(" \\\\    // ");
                    if (row == 3)
                        Console.Write("  \\\\__//  ");
                }
                if (i == "potion")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    if (row == 0)
                        Console.Write("    ##    ");
                    if (row == 1)
                        Console.Write("  __||__  ");
                    if (row == 2)
                        Console.Write(" | +   +| ");
                    if (row == 3)
                        Console.Write("  \\__+_/  ");
                }
                if (i == "fireball")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    if (row == 0)
                        Console.Write(" '     #@ ");
                    if (row == 1)
                        Console.Write("  '  '@@@#");
                    if (row == 2)
                        Console.Write("'   '  @@ ");
                    if (row == 3)
                        Console.Write("  '       ");
                }
                if (i == "stick")
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    if (row == 0)
                        Console.Write("    @@    ");
                    if (row == 1)
                        Console.Write("   //     ");
                    if (row == 2)
                        Console.Write("  //      ");
                    if (row == 3)
                        Console.Write(" //       ");
                }
                if (i == "curse")
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    if (row == 0)
                        Console.Write("   .||.   ");
                    if (row == 1)
                        Console.Write("  ======  ");
                    if (row == 2)
                        Console.Write("   '||'   ");
                    if (row == 3)
                        Console.Write("    ||    ");
                }
                if (i == "bow")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    if (row == 0)
                        Console.Write("    |\\\\   ");
                    if (row == 1)
                        Console.Write("    | \\\\  ");
                    if (row == 2)
                        Console.Write("    | //  ");
                    if (row == 3)
                        Console.Write("    |//   ");
                }
                if (i == "strong_bow")
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    if (row == 0)
                        Console.Write("   <|\\\\   ");
                    if (row == 1)
                        Console.Write("    | \\@\\ ");
                    if (row == 2)
                        Console.Write("    | /@/ ");
                    if (row == 3)
                        Console.Write("   <|//   ");
                }
                if (i == "stunning_knife")
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    if (row == 0)
                        Console.Write("   ? /\\   ");
                    if (row == 1)
                        Console.Write("    /?/   ");
                    if (row == 2)
                        Console.Write("  ./_/. ? ");
                    if (row == 3)
                        Console.Write("  /_/?    ");
                }
                if (i == "axe")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    if (row == 0)
                        Console.Write(" /'\\||/'\\ ");
                    if (row == 1)
                        Console.Write(" \\_/||\\_/ ");
                    if (row == 2)
                        Console.Write("    ||    ");
                    if (row == 3)
                        Console.Write("    ||    ");
                }
                if (i == "brutal_axe")
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    if (row == 0)
                        Console.Write(" /'\\||/'\\ ");
                    if (row == 1)
                        Console.Write(" \\./||\\./ ");
                    if (row == 2)
                        Console.Write("  ' || '  ");
                    if (row == 3)
                        Console.Write("    ||    ");
                }


                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("║");
            }
        }

        static void GenerateBar(int value, int max_value, int mode)
        {
            if (mode == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" <3 ");
            }

            if (mode == 1)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("'H' ");
            }

            if (mode == 2)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\\/\\ ");
            }

            double procentage = (double)value / (double)max_value;
            int bar_lenght = (int)(procentage * 20);

            for (int i = 0; i < 20; i++)
            {
                if (i < bar_lenght)
                    Console.Write("#");
                else
                    Console.Write(".");
            }

            Console.Write(" " + TextPad(Convert.ToString(value), 3));
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        // Vstup hráče
        static string PlayerInput()
        {
            if (HelpMessage)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Špatný vstup - zadejte jednu z možností uvedených v <?>\n");
                HelpMessage = false;
            }
            else
            {
                if (NotEnoughStaminaMessage)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Nedostatek Staminy!\n");
                    NotEnoughStaminaMessage = false;
                }
                else
                {
                    if (BadActionMessage)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Nemožné provést danou akci!\n");
                        BadActionMessage = false;
                    }
                    else
                    {
                        Console.Write("\n");
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(playerName + "> ");
            Console.ForegroundColor = ConsoleColor.White;
            return Console.ReadLine().ToString().ToLower();
        }



    }
}
