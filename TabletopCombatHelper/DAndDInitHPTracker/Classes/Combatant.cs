using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAndDInitHPTracker.Classes
{
    public class Combatant
    {
        public Combatant() {}

        public Combatant(string name, int hp, int initiative, Guid? id = null)
        {
            Name = name;
            HP = hp;
            Initiative = initiative;

            DisplayInformation = $"Name: {name} \t\t\t HP: {HP} \t\t\t Initiative: {Initiative}";
            ID = id != null ? id : Guid.NewGuid();
        }


        public string Name { get; set; }

        public int HP { get; set; }

        public int Initiative { get; set; }

        public string DisplayInformation { get; set; }

        public Guid? ID { get; private set; }
    }
}
