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

        public Combatant(string name, int hp, int initiative, int friendlyId, Guid? id = null)
        {
            Name = name;
            HP = hp;
            Initiative = initiative;
            FriendlyId = friendlyId;

            string friendlyIdString = friendlyId > 1 ? friendlyId.ToString() : String.Empty;

            DisplayInformation = $"Name: {name + " " + friendlyIdString} {CalulateTabs(name)} HP: {HP} \t\t\t Initiative: {Initiative}";
            ID = id != null ? id : Guid.NewGuid();
        }


        public string Name { get; set; }

        public int HP { get; set; }

        public int Initiative { get; set; }

        public int FriendlyId { get; set; }

        public string DisplayInformation { get; set; }

        public Guid? ID { get; private set; }

        private string CalulateTabs(string name)
        {
            string rv = $"\t\t\t";
            if (name.Length > 7) rv = $"\t\t";
            if (name.Length > 13) rv = $"\t";
            if (name.Length > 21) rv = String.Empty;
            return rv;
        }
    }
}
