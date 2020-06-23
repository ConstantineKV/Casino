using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;

namespace Casino
{
    abstract class Game
    {
        //поля
        protected string game_name;
        protected uint bank;
        protected Timer game_timer;
        public Player[] players;
        public Croupier croupier;

        //свойства
        public string Game_Name
        {
            get
            {
                return game_name;
            }
            set
            {
                game_name = value;
            }
        }
        public uint Bank
        {
            get
            {
                return bank;
            }
            set
            {
                bank = value;
            }
        }
        //делегат
        public delegate void BetMoney(Game game, uint money);
        public delegate void CasinoWins(Game game);
        public delegate void PlayerWins(Game game, Player player);

        //события
        public abstract event BetMoney BetMoneyEvent;
        public abstract event CasinoWins CasinoWinsEvent;
        public abstract event PlayerWins PlayerWinsEvent;
        
        //конструкторы
        public Game()
        {
            bank = 0;
            
            
        }
        public Game(uint new_bank)
        {
            bank = new_bank;
        }


        //методы
        public string GetPlayerName(uint player_number)
        {
            return players[player_number].Name;
        }
        public int GetMaxPlayers()
        {
            return players.Length;
        }
        public bool IsDeskFull()
        {
            for (int i = 0; i < players.Length; i++)
                if (players[i] == null)
                    return false;
            return true;
        }
        public void releaseDesk()
        {
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = null;
              
            }
        }                                //убрать игроков из-за стола

        public virtual void addCroupier(Croupier new_croupier)
        {
            croupier = new_croupier;
            croupier.IsOnDuty = true;

        }           //ввести крупье за стол
        public virtual void releaseCroupier()
        {
            croupier.IsOnDuty = false;
            croupier = null;

        }                            //увести крупье из-за стола
        public abstract void startGame();
        public abstract void stopGame(Object source, System.Timers.ElapsedEventArgs e);
        public abstract void newPlayer(Player player);
    
    }

 
}
