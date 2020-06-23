using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casino
{
    class Player
    {
        //поля
        private string name;
        private uint money;
        private uint luck;
        //свойства
        public string Name
        {
            get
            {
                return name;
            }
        }
        public uint Money
        {
            get
            {
                return money;
            }
            set
            {
                money = value;
            }
        }
        public uint Luck
        {
            get
            {
                return luck;
            }
            set
            {
                if ((value >= 0) && (value <= 10))
                    luck = value;
                else
                    luck = 0;

            }
        }    


        //конструкторы
        public Player()
        {
            name = "John";
            money = 1000;
            luck = 5;
        }
        public Player(string new_name, uint new_money, uint new_luck)
        {
            name = new_name;
            Money = new_money;
            Luck = new_luck;

        }

        //операторы
  
        public static bool operator >(Player player1, Player player2)
        {
 
            if ((player1.Luck) > (player2.Luck))
                return true;
            else
                return false;
        }
        public static bool operator <(Player player1, Player player2)
        {
            if ((player1.Luck) < (player2.Luck))
                return true;
            else
                return false;
        }
      
    }
}
