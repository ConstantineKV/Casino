using System;
using System.Text;
using System.Timers;

namespace Casino
{
    sealed class Black_Jack : Game
    {
        public Black_Jack()
        {
            game_name = "Блэкджек";
            players = new Player[1];
            bank = 0;
            game_timer = new Timer(10000);
            game_timer.AutoReset = false;
            game_timer.Elapsed += stopGame;
            
        }
        public Black_Jack(uint new_bank) : base(new_bank)
        {
            bank = new_bank;
            game_name = "Блэкджек";
            players = new Player[1];
            game_timer = new Timer(10000);
            game_timer.AutoReset = false;
            game_timer.Elapsed += stopGame;
        }

        //события
        public override event BetMoney BetMoneyEvent;
        public override event CasinoWins CasinoWinsEvent;
        public override event PlayerWins PlayerWinsEvent;
        //методы
        public override void addCroupier(Croupier new_croupier)
        {
            croupier = new_croupier;
            croupier.IsOnDuty = true;

            if (players[0] != null)
                startGame();

            

        }           //ввести крупье за стол
        public override void releaseCroupier()
        {
            croupier.IsOnDuty = false;
            croupier = null;

        }  
        public override void startGame()
        {
            if ((players[0] != null) && (croupier != null))
            {
                BetMoneyEvent(this, players[0].Money);
                bank += players[0].Money;
                players[0].Money = 0;
                game_timer.Start();

            }
            Console.WriteLine(game_name + ": на кону " + bank + ". Игра начата");
        }

        public override void newPlayer(Player new_player)
        {
            for (int i = 0; i < players.Length; i++)
                if (players[i] == null)
                    players[i] = new_player;
            if(croupier != null)
                startGame();
        }

        public override void stopGame(Object source, System.Timers.ElapsedEventArgs e)          //Игра остановлена, рассчитать победителя
        {
            uint victory_number = ((players[0].Luck * 10) - (croupier.Mastery * 10));

            Console.WriteLine();
            Console.WriteLine("Вероятность выигрыша:" + victory_number);
            if (!Casino.getPosibility(victory_number))
                this.CasinoWinsEvent(this);
            else
                this.PlayerWinsEvent(this,players[0]);

            Console.WriteLine(game_name + ": игра остановлена");
        }

        //операторы
        public static Black_Jack operator +(Black_Jack bj, Player player)
        {
            bj.newPlayer(player);
            return bj;
        }
    }
}
