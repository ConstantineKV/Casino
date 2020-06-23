using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Casino
{
    class Casino
    {   
        static DateTime dt = DateTime.Now;
        static Random rand = new Random(dt.Millisecond);
        static Croupier[] croupiers;
        //Игровые залы
        static Game_Room<Black_Jack> BJ_room;
        static Game_Room<Poker> Poker_room;
        static Game_Room<Roulette> Roulette_room;
        //Таймеры
        public static Timer casino_timer;
        public static Timer player_timer;
        public static Timer salary_timer;

        static uint casino_bank = 100000;  //банк казино
        //Методы
        public static uint getRandomNumber()
        { 
           
            int random_number = Math.Abs(rand.Next());
            random_number = random_number % 100;
            return (uint)random_number;
        }                   //возвращает число от 1 до 100
        public static bool getPosibility(uint number)
        {
            if (number > 100)
            {
                Console.WriteLine("Слишком большое число вероятности. Максимум - 100");
                return false;
            }
            else
            {
                uint random_number = getRandomNumber();
                if ((random_number >= 0) && (random_number <= number))
                    return true;
                else
                    return false;
            }
        }          //если случайное число попадает в диапазон от 0 до number, то возвращает true
        public static T assignRandomValue<T>(T[] values)
        {
            uint number = getRandomNumber()%10;
            if (number > 10)
                return values[10];
            return values[number];
         
        }
        public static Player getRandomPlayer()
        {
            string[] names = new string[11]{"Джон", "Питер",  "Евгений", "Пол", "Арнольд", "Джек",  "Алекс", "Александр",  "Грегори",  "Джеймс", "Ричард"};
            uint[] lucks = new uint[11]{0,1,2,3,4,5,6,7,8,9,10};
            uint[] moneys = new uint[11]{1000,1500,2000,2500,3000,3500,4000,4500,5000,5500,6000};
            string new_name = assignRandomValue(names);
            uint new_luck = assignRandomValue(lucks);
            uint new_money = assignRandomValue(moneys);
            Player new_player = new Player(new_name, new_money, new_luck);
            return new_player;

        }
        public static void writeGameRoomInformation<T>(Game_Room<T> game_room) where T : Game, new()
        {
            Console.WriteLine("Зал " + game_room.Name);
            for (int i = 0; i < game_room.games.Length; i++)
            {
                Console.WriteLine(game_room.games[i].Game_Name);
                if (game_room.games[i].croupier != null)
                    Console.WriteLine("Крупье {0}. Мастерство: {1}. Зарплата {2}",
                        game_room.games[i].croupier.Name, game_room.games[i].croupier.Mastery, game_room.games[i].croupier.Salary);
                else
                    Console.WriteLine("Нет крупье");
                if (game_room.games[i].players[0] == null)
                    Console.WriteLine("Нет игроков");
                else
                    for (int j = 0; j < game_room.games[i].GetMaxPlayers(); j++)
                        if (game_room.games[i].players[j] != null)
                            Console.WriteLine("Игрок " + (j+1) + ":{0}. Удача: {1}. Деньги {2}", game_room.games[i].players[j].Name, game_room.games[i].players[j].Luck, game_room.games[i].players[j].Money);
            }

            Console.WriteLine();
        }
        public static void addCroupierInGameRoom<T>(Game_Room<T> game_room, Croupier croup, uint game_number) where T : Game, new()
        {
            if (game_room.games[game_number].croupier != null)
            {
                Console.WriteLine("За этим столом уже работает крупье");
                
            }
            else
                game_room.games[game_number].addCroupier(croup);
        }
        public static void releaseCroupierInGameRoom<T>(Game_Room<T> game_room, uint game_number) where T : Game, new()
        {
            if (game_room.games[game_number].croupier == null)
                Console.WriteLine("За этим столом не работает крупье");
            else
                game_room.games[game_number].releaseCroupier();
        }
        public static int addPlayerInGameRoom<T>(Game_Room<T> game_room, Player player) where T : Game, new()
        {
            for(int i=0;i<game_room.games.Length;i++)
                for(int j=0;j<game_room.games[i].players.Length;j++)
                    if (game_room.games[i].players[j] == null)
                    {
                        game_room.games[i].newPlayer(player);
                        return i;
                    }
            return -1;
        }
        //Обработчики событий
        public static void betMoney(Game game, uint money)                          //событие: казино ставит деньги
        {
            Console.WriteLine();
            Console.WriteLine("За стол " + game.Game_Name + " из банка казино переводится " + money + "$");
            casino_bank -= money;
            game.Bank += money;
            Console.WriteLine("Теперь в банке казино " + casino_bank + "$");
        }
        public static void casinoWins(Game game)                                    //событие: казино побеждает
        {
            Console.WriteLine();
            Console.WriteLine("Игра " + game.Game_Name + ". Казино побеждает.");
            Console.WriteLine(game.Bank + " долларов переводится в банк казино");

            casino_bank += game.Bank;
            game.Bank = 0;
            game.releaseDesk();

            Console.WriteLine("Теперь в банке казино " + casino_bank + " долларов");
        }
        public static void playerWins(Game game, Player player)                     //событие: игрок побеждает
        {
            Console.WriteLine();
            Console.WriteLine("Игра " + game.Game_Name + ". Игрок побеждает.");
            Console.WriteLine(game.Bank + " долларов переводится на счет игрока " + player.Name);
            game.Bank = 0;
            game.releaseDesk();
            Console.WriteLine("В банке казино " + casino_bank + " долларов");
        }
        static void salary_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine();
            for (int i = 0; i < croupiers.Length; i++)
            {
                casino_bank -= croupiers[i].Salary;
                Console.WriteLine("Крупье " + croupiers[i] + " получил зарплату за час работы в размере " + croupiers[i].Salary + ". Теперь в банке " + casino_bank + "$");

            }
        }      //событие: выплата зарплаты крупье
        static void casino_timer_Elapsed(object sender, ElapsedEventArgs e)         //событие: окончание работы казино
        {
            casino_timer.Stop();
            player_timer.Stop();
            salary_timer.Stop();
            Console.WriteLine("Казино закрывается. Посетителей больше не будут пускать в казино");
        }
        static void player_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine();
            Player new_player = getRandomPlayer();
            Console.WriteLine("Игрок {0} с деньгами {1} и удачей {2} пришёл в казино", new_player.Name, new_player.Money, new_player.Luck);

            if ((BJ_room.IsFullOfPlayers()) && (Poker_room.IsFullOfPlayers()) && (Roulette_room.IsFullOfPlayers()))
            {
                Console.WriteLine("Все игровые столы в казино заняты. Игрок {0} ушёл в другое казино", new_player.Name);
                return;
            }
            //Игрок идёт в случайный игровой зал
            uint random = getRandomNumber();
            int result = 0;
            if ((random >= 0) && (random <= 33))
            {
                result = addPlayerInGameRoom(BJ_room, new_player);
                if (result != -1)
                {
                    Console.WriteLine("Игрок сел за стол " + (result + 1) + " в зале блекджека");
                    return;
                }
            }
            if ((random > 33) && (random <= 66))
            {
                result =  addPlayerInGameRoom(Poker_room, new_player);
                if (result != -1)
                {
                    Console.WriteLine("Игрок сел за стол " + (result + 1) + " в зале покера");
                    return;
                }
            }
            if ((random > 66) && (random <= 100))
            {
                result = addPlayerInGameRoom(Roulette_room, new_player);
                if (result != -1)
                {
                    Console.WriteLine("Игрок сел за стол " + (result + 1) + " в зале рулетки");
                    return;
                }
            }
            
            //Игрок ищет любой свободный игровой зал
            result = addPlayerInGameRoom(BJ_room, new_player);
            if (result != -1)
            {
                Console.WriteLine("Игрок сел за стол " + (result + 1) + " в зале блекджека");
                return;
            }

            result = addPlayerInGameRoom(Poker_room, new_player);
            if (result != -1)
            {
                Console.WriteLine("Игрок сел за стол " + (result + 1) + " в зале покера");
                return;
            }

            result = addPlayerInGameRoom(Roulette_room, new_player);
            if (result != -1)
            {
                Console.WriteLine("Игрок сел за стол " + (result + 1) + " в зале рулетки");
                return;
            }

            Console.WriteLine("Игрок не нашёл свободного столика и ушёл из казино");
        }


        //Главный метод
        static void Main(string[] args)
        {
            uint choise_main,choise_croup,choise_room,choise_game,choise_time;
            string temp_name;
            uint temp_money, temp_luck;
            
            BJ_room = new Game_Room<Black_Jack>(2, "Блэкджек", casinoWins, playerWins,betMoney); //создаём игровой зал с двумя столами для блэк джека
            Poker_room = new Game_Room<Poker>(3, "Покер", casinoWins, playerWins, betMoney); //создаем игровой зал с тремя столами для покера
            Roulette_room = new Game_Room<Roulette>(2, "Рулетка", casinoWins, playerWins, betMoney); //создаем игровой зал с двумя столами для рулетки
            croupiers = new Croupier[7];
            croupiers[0] = new Croupier("Джек", 6, 500);
            croupiers[1] = new Croupier("Джон", 4, 250);
            croupiers[2] = new Croupier("Джеймс", 8, 500);
            croupiers[3] = new Croupier("Питер", 7, 200);
            croupiers[4] = new Croupier("Леонард", 5, 300);
            croupiers[5] = new Croupier("Билл", 3, 250);
            croupiers[6] = new Croupier("Уилл", 5, 500);

            do
            {
                Console.WriteLine("        Меню");
                Console.WriteLine("1.Вывести информацию о казино");
                Console.WriteLine("2.Вывести информацию о залах");
                Console.WriteLine("3.Назначить крупье за игровой стол");
                Console.WriteLine("4.Освободить крупье от работы за игровым столом");
                Console.WriteLine("5.Ввести игрока");
                Console.WriteLine("6.Начать работу казино");
                Console.WriteLine("7.Выход");
                choise_main = uint.Parse(Console.ReadLine());
                switch (choise_main)
                {
                    case 1:
                        Console.WriteLine();
                        Console.WriteLine("Банк = " + casino_bank + " $");
                        Console.WriteLine();
                        Console.WriteLine("Крупье:");
                        foreach (Croupier croup in croupiers)
                        {
                            Console.WriteLine("{0}. Мастерство: {1}. Зарплата {2}. Занят {3}", croup.Name, croup.Mastery, croup.Salary,croup.IsOnDuty);
                        }
                        break;
                    case 2:
                        writeGameRoomInformation(BJ_room);
                        writeGameRoomInformation(Poker_room);
                        writeGameRoomInformation(Roulette_room);
                        break;
                    case 3:
                        Console.WriteLine();
                        Console.WriteLine("Выберите свободного крупье");
                        for(int i=0;i<croupiers.Length;i++)
                        {
                            Console.WriteLine((i+1) + ". {0}. Мастерство: {1}. Зарплата {2}. Занят {3}", 
                                croupiers[i].Name, croupiers[i].Mastery, croupiers[i].Salary, croupiers[i].IsOnDuty);
                        }
                        choise_croup = uint.Parse(Console.ReadLine());
                        if (croupiers[choise_croup-1].IsOnDuty)
                        {
                            Console.WriteLine("Этот крупье уже работает за игровым столом");
                            break;
                        }
                        Console.WriteLine("Выберите игровой зал");
                        Console.WriteLine("1. Блэкджек");
                        Console.WriteLine("2. Покер");
                        Console.WriteLine("3. Рулетка");
                        choise_room = uint.Parse(Console.ReadLine());
                        Console.WriteLine("Выберите стол");
                        choise_game = uint.Parse(Console.ReadLine());
                        if (choise_room == 1)
                            addCroupierInGameRoom(BJ_room, croupiers[choise_croup - 1], choise_game-1);
                        if (choise_room == 2)
                            addCroupierInGameRoom(Poker_room, croupiers[choise_croup - 1], choise_game - 1);
                        if (choise_room == 3)
                            addCroupierInGameRoom(Roulette_room, croupiers[choise_croup - 1], choise_game - 1);
                        break;
                    case 4:
                        Console.WriteLine("Выберите игровой зал");
                        Console.WriteLine("1. Блэкджек");
                        Console.WriteLine("2. Покер");
                        Console.WriteLine("3. Рулетка");
                        choise_room = uint.Parse(Console.ReadLine());
                        Console.WriteLine("Выберите стол");
                        choise_game = uint.Parse(Console.ReadLine());
                        if (choise_room == 1)
                            releaseCroupierInGameRoom(BJ_room, choise_game - 1);
                        if (choise_room == 2)
                            releaseCroupierInGameRoom(Poker_room, choise_game - 1);
                        if (choise_room == 3)
                            releaseCroupierInGameRoom(Roulette_room, choise_game - 1);
                     break;
                    case 5:
                        Console.WriteLine("Введите имя игрока");
                        temp_name = Console.ReadLine();
                        Console.WriteLine("Введите удачу игрока");
                        temp_luck = uint.Parse(Console.ReadLine());
                        if(temp_luck > 10)
                        {
                            Console.WriteLine("Удача не может быть больше 10");
                            break;
                        }
                        Console.WriteLine("Введите деньги игрока");
                        temp_money = uint.Parse(Console.ReadLine());
                        Player new_player = new Player(temp_name, temp_money, temp_luck);
                        Console.WriteLine("Выберите игровой зал");
                        Console.WriteLine("1. Блэкджек");
                        Console.WriteLine("2. Покер");
                        Console.WriteLine("3. Рулетка");
                        choise_room = uint.Parse(Console.ReadLine());
                        Console.WriteLine("Выберите стол");
                        choise_game = uint.Parse(Console.ReadLine());
                        if(choise_room == 1)
                            BJ_room.games[choise_game-1].newPlayer(new_player);
                        if (choise_room == 2)
                            Poker_room.games[choise_game - 1].newPlayer(new_player);
                        if (choise_room == 3)
                            Roulette_room.games[choise_game - 1].newPlayer(new_player);
                        break;
                    case 6:
                        Console.WriteLine("На сколько времени открыть казино?");
                        choise_time = uint.Parse(Console.ReadLine());
                        //таймер работы казино
                        casino_timer = new Timer(choise_time * 1000);
                        casino_timer.AutoReset = false;
                        casino_timer.Elapsed += casino_timer_Elapsed;
                        casino_timer.Start();
                        //Таймер прихода игроков в казино
                        player_timer = new Timer(5000);
                        player_timer.AutoReset = true;
                        player_timer.Elapsed += player_timer_Elapsed;
                        player_timer.Start();
                        
                        //таймер выплаты зарплаты
                        salary_timer = new Timer(60000);
                        salary_timer.AutoReset = true;
                        salary_timer.Elapsed += salary_timer_Elapsed;
                        salary_timer.Start();
                        Console.WriteLine("Казино начинает работу");
                        
                        break;
                    default:
                        break;
                }
            }
            while (choise_main != 7);
            Console.ReadLine();

        }





    }
}
