using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Casino
{
    class Game_Room<T> where T : Game, new()
    {
        //поля
        string name;
        public T[] games;
        Timer search_timer;

        public string Name
        {
            get
            {
                return name;
            }
        }
        //делегаты
        public delegate void CasinoWins(Game game);
        public delegate void PlayerWins(Game game, Player player);

        //Конструкторы
        public Game_Room()
        {
            games = new T[1];
            games[0] = new T();
     
            search_timer = new Timer(20000);
            search_timer.Elapsed += searchCheaters;
        }
        public Game_Room(uint number_of_games, string game_name, Game.CasinoWins casinoWinsEvent, Game.PlayerWins playerWinsEvent, Game.BetMoney betMoneyEvent)
        {
            name = game_name;
            games = new T[number_of_games];
            for (int i = 0; i < number_of_games; i++)
            {
                games[i] = new T();
                games[i].Game_Name = game_name + " " + (i+1) + "- й стол";
                games[i].CasinoWinsEvent += casinoWinsEvent;
                games[i].PlayerWinsEvent += playerWinsEvent;
                games[i].BetMoneyEvent += betMoneyEvent;
            }

        }
        //Методы
        private void searchCheaters(object sender, ElapsedEventArgs e)
        {
            
        }
        public bool IsFullOfCroupiers()
        {
            foreach(T game in games)
            {
                if(game.croupier == null)
                    return false;
            }
            return true;
        }
        public bool IsFullOfPlayers()
        {
            for (int i = 0; i < games.Length; i++)
                if (!games[i].IsDeskFull())
                    return false;
            return true;
        }

    }
}
