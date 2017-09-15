using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace room
{
    class Mob
    {

        public ConsoleColor Color { get; set; }
        public char Icon { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public int HomeX { get; set; }
        public int HomeY { get; set; }

        public int HP { get; set; }
        public int BaseHP { get; set; }

        public Weapon Weapon { get; set; }
        public Armor Armor { get; set; }

        public Mob()
        {

        }

        public AI CurrentAI { get; set; }

        public void Think(World w)
        {
            if (CurrentAI != null)
            {
                CurrentAI.Think(w, this);
            }
        }

        public void Aggro()
        {
            CurrentAI = new Aggro();
            Color = ConsoleColor.Red;
        }

        public void LoseAggro()
        {
            CurrentAI = new GoHomeAI(HomeX, HomeY);
            Color = ConsoleColor.Yellow;
        }
    }

    class Weapon
    {
        public int MinDamage { get; set; }
        public int MaxDamage { get; set; }
        public string Description { get; set; }
    }

    class Armor
    {
        public int Mitigation { get; set; }
        public string Description { get; set; }
    }
}
