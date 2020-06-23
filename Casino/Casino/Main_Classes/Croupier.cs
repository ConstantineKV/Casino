using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

namespace Casino
{
    class Croupier
    {
        
        //Мастерство
        private string name;
        private uint mastery;
        private uint salary;
        public bool IsOnDuty;
        //Свойства
        public string Name
        {
            get
            {
                return name;
            }
        }
        public uint Mastery
        {
            get
            {
                return mastery;
            }
            set
            {
                if ((value >= 0) && (value <= 10))
                    mastery = value;
            }
        }

        public uint Salary
        {
            get
            {
                return salary;
            }
            set
            {
                if((value>=100) && (value <=1000))
                    salary = value;
                else
                {
                    Console.WriteLine("Зарплата должна быть в диапазоне 100 - 1000 долларов. Зарплата установлена в районе 100 долларов");
                    salary = 100;
                }
                salary = value;
            }
        }

        //Конструкторы
        public Croupier()
        {
            name = "John";
            Mastery = 5;
            Salary = 100;
            IsOnDuty = false;

        }
        public Croupier(string new_name, uint new_mastery, uint new_salary)
        {
            name = new_name;
            Mastery = new_mastery;
            Salary = new_salary;
            IsOnDuty = false;
        }

        //Операторы

        public static bool operator >(Croupier croupier1, Croupier croupier2)
        {
            if (croupier1.Mastery == croupier2.Mastery)
                return true;
            else
                return false;
        }
        public static bool operator <(Croupier croupier1, Croupier croupier2)
        {
            if (croupier1.Mastery == croupier2.Mastery)
                return true;
            else
                return false;
        }
    }
}
