using System;
using System.Timers;

namespace Casino
{
    class Poker : Game
    {
        //конструкторы
        public Poker()
        {
            game_name = "Покер";
            players = new Player[3];
            bank = 0;
            game_timer = new Timer(30000);
            game_timer.AutoReset = false;
            game_timer.Elapsed += stopGame;
        }
        public Poker(uint new_bank)
            : base(new_bank)
        {
            game_name = "Покер";
            players = new Player[3];
            game_timer = new Timer(30000);
            game_timer.AutoReset = false;
            game_timer.Elapsed += stopGame;
        }

        //события
        public override event BetMoney BetMoneyEvent;
        public override event CasinoWins CasinoWinsEvent;
        public override event PlayerWins PlayerWinsEvent;

        //методы
        public bool IsFullTable()
        {
            for (int i = 0; i < players.Length; i++)
                if (players[i] == null)
                    return false;
            return true;
        }
        public override void addCroupier(Croupier new_croupier)
        {
            croupier = new_croupier;
            croupier.IsOnDuty = true;

            if (IsFullTable())
                startGame();

        }           //ввести крупье за стол

        public override void releaseCroupier()
        {
            croupier.IsOnDuty = false;
            croupier = null;

        }
        public override void newPlayer(Player new_player)
        {
            for (int i = 0; i < players.Length; i++)
                if (players[i] == null)
                {
                    players[i] = new_player;
                    break;
                }
            if ((croupier != null)&&(IsFullTable()))
                startGame();
        }
        public override void startGame()
        {
            if ((IsFullTable()) && (croupier != null))
            {
                uint players_bet=0;
                for (int i = 0; i < players.Length; i++)
                {
                    players_bet += players[i].Money;
                    players[i].Money = 0;
                }
                BetMoneyEvent(this, players_bet);
                bank += players_bet;
                game_timer.Start();

            }
            Console.WriteLine(game_name + ": на кону " + bank + ". Игра начата");
        }

        public override void stopGame(Object source, System.Timers.ElapsedEventArgs e)          //Игра остановлена, рассчитать победителя
        {
            //определяем лучшего игрока
            Player best_player = players[0];
            uint best_player_number = 0;
            for (uint i = 1; i < players.Length; i++)
            {
                if (players[i] > best_player)
                {
                    best_player = players[i];
                    best_player_number = i;
                }
            }

            //рассчитываем вероятность выигрыша игрока
            uint victory_number = ((players[best_player_number].Luck * 10)  - (croupier.Mastery * 10));
        
             Console.WriteLine();
            Console.WriteLine("Вероятность выигрыша:" + victory_number);
            if (!Casino.getPosibility(victory_number))
                this.CasinoWinsEvent(this);
            else
                this.PlayerWinsEvent(this, players[best_player_number]);

            Console.WriteLine(game_name + ": игра остановлена");
        }

        //операторы
        public static Poker operator +(Poker poker, Player player)
        {
            poker.newPlayer(player);
            return poker;
        }
    }
}
