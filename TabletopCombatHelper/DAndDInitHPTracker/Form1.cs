using DAndDInitHPTracker.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DAndDInitHPTracker
{
    public partial class Form1 : Form
    {
        #region Global Variables
        BindingList<Combatant> Combatants
        {
            get
            {
                var NewCombatants = new BindingList<Combatant>();
                return listBox1 != null && listBox1.DataSource != null ? (BindingList<Combatant>)listBox1.DataSource : NewCombatants;
            }
            set
            {
                Combatants.Clear();
                foreach (Combatant combatant in value)
                {
                    Combatants.Add(combatant);
                }
            }
        }
        BindingList<Combatant> NonCombatants = new BindingList<Combatant>();
        int CurrentRound = 1;
        Guid? CurrentTurn = null;
        Guid? NextTurn = null;
        bool IsCombatBegun = false;
        #endregion

        #region Form Methods
        public Form1()
        {
            InitializeComponent();
            Bind();
            ClearTextboxes();
        }

        //Selected Index Changed
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                textBox1.Text = Combatants[listBox1.SelectedIndex].Name;
                textBox2.Text = Combatants[listBox1.SelectedIndex].HP.ToString();
                textBox3.Text = Combatants[listBox1.SelectedIndex].Initiative.ToString();
            }
        }

        //Right Button 
        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                var selectedCombatant = Combatants[listBox1.SelectedIndex];
                Combatants.RemoveAt(listBox1.SelectedIndex);
                NonCombatants.Add(selectedCombatant);

                if (GetCurrentTurnIndex() < 0 && Combatants.Any() && IsCombatBegun)
                {
                    if (NextTurn != null)
                    {
                        CurrentTurn = NextTurn;
                        SetNextTurn();
                        SetCurrentTurnLabel();
                    }
                    else
                    {
                        SetCurrentTurn();
                        SetNextTurn();
                    }
                }
            }
        }

        //Left Button Clicked
        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex >= 0)
            {
                var selectedCombatant = NonCombatants[listBox2.SelectedIndex];
                NonCombatants.RemoveAt(listBox2.SelectedIndex);
                Combatants.Add(selectedCombatant);
            }
        }

        //Up Button Clicked
        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 1)
            {
                var selectedIndex = listBox1.SelectedIndex;

                Combatants.Insert(listBox1.SelectedIndex - 1, Combatants[listBox1.SelectedIndex]);
                Combatants.RemoveAt(listBox1.SelectedIndex);
                listBox1.SelectedIndex = selectedIndex - 1;
            }
        }

        //Down Button Clicked
        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < Combatants.Count - 1)
            {
                var selectedIndex = listBox1.SelectedIndex;
                var selectedCombatant = Combatants[listBox1.SelectedIndex];
                Combatants.RemoveAt(listBox1.SelectedIndex);
                Combatants.Insert(listBox1.SelectedIndex + 1, selectedCombatant);           
                listBox1.SelectedIndex = selectedIndex + 1;
            }
        }

        //Update Button Clicked
        private void button5_Click(object sender, EventArgs e)
        {
            if (AssessTextboxes() && listBox1.SelectedIndex >= 0)
            {
                Combatant combatant = Combatants[listBox1.SelectedIndex];

                //Handle + or - to HP
                var hpText = textBox2.Text;

                if (hpText.StartsWith("+") || hpText.StartsWith("-"))
                {
                    var oper = char.ToString(hpText[0]);

                    switch (oper)
                    {
                        case "+":
                            textBox2.Text = (combatant.HP + Convert.ToInt32(hpText.Substring(1))).ToString();
                            break;
                        case "-":
                            textBox2.Text = (combatant.HP - Convert.ToInt32(hpText.Substring(1))).ToString();
                            break;
                    }
                }

                var updatedCombatant = new Combatant(textBox1.Text, Convert.ToInt32(textBox2.Text), Convert.ToInt32(textBox3.Text), 
                                                     combatant.FriendlyId, combatant.ID);
                Combatants[listBox1.SelectedIndex] = updatedCombatant;
            }
        }

        //Add Button Clicked
        private void button6_Click(object sender, EventArgs e)
        {            
            if (AssessTextboxes())
            {
                int friendlyId = 1;
                var sameNameCombatants = Combatants.Where(x => x.Name == textBox1.Text).ToList();
                sameNameCombatants.AddRange(NonCombatants.Where(x => x.Name == textBox1.Text).ToList());
                foreach (var combatant in sameNameCombatants)
                {
                    if (combatant.FriendlyId >= friendlyId) friendlyId = combatant.FriendlyId + 1;
                }

                Combatants.Add(new Combatant(textBox1.Text, Convert.ToInt32(textBox2.Text), Convert.ToInt32(textBox3.Text), friendlyId));
            }
        }

        //Clear Button Clicked
        private void button7_Click(object sender, EventArgs e)
        {
            ClearTextboxes();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        //Re-Order Combatants
        private void button8_Click(object sender, EventArgs e)
        {
            var orderedCombatantsList = Combatants.OrderByDescending(x => x.Initiative).ToList();
            BindingList<Combatant> orderedCombatants = new BindingList<Combatant>();

            foreach (var combatant in orderedCombatantsList)
            {
                orderedCombatants.Add(combatant);
            }

            Combatants = orderedCombatants;
        }

        //Next Turn
        private void button9_Click(object sender, EventArgs e)
        {
            if (Combatants.Any())
            {
                if (!IsCombatBegun)
                {
                    IsCombatBegun = true;
                    button9.Text = "Next Turn";
                }
                SetCurrentTurn();
                SetNextTurn();

                SetCurrentTurnLabel();

                listBox1.SelectedIndex = GetCurrentTurnIndex();
            }
        }
        #endregion

        #region Private Methods
        private void Bind()
        {
            listBox1.DataSource = Combatants;
            listBox1.DisplayMember = "DisplayInformation";
            listBox1.ValueMember = "Initiative";
            listBox2.DataSource = NonCombatants;
            listBox2.DisplayMember = "DisplayInformation";
            listBox2.ValueMember = "Initiative";
        }

        //Checks if textboxes are usable for adding/updating combatants
        private bool AssessTextboxes()
        {
            if (!String.IsNullOrEmpty(textBox1.Text) && !String.IsNullOrEmpty(textBox2.Text) && !String.IsNullOrEmpty(textBox3.Text)
                && int.TryParse(textBox2.Text, out int result) && int.TryParse(textBox3.Text, out int result2))
            {
                return true;
            }
            else return false;
        }

        private int GetCurrentTurnIndex()
        {
            return Combatants.ToList().FindIndex(x => x.ID == CurrentTurn);
        }

        private void ClearTextboxes()
        {
            textBox1.Text = String.Empty;
            textBox2.Text = String.Empty;
            textBox3.Text = String.Empty;
        }

        private void SetCurrentTurnLabel()
        {
            if (Combatants.Where(x => x.ID == CurrentTurn).Any())
            {
                var currentCombatant = Combatants.Where(x => x.ID == CurrentTurn).FirstOrDefault();
                string friendlyIdString = currentCombatant.FriendlyId > 1 ? currentCombatant.FriendlyId.ToString() : String.Empty;
                label5.Text = currentCombatant.Name + " " + friendlyIdString;
            }
        }

        private void SetCurrentTurn()
        {
            int currentTurnIndex = GetCurrentTurnIndex();
            if (currentTurnIndex >= 0 && currentTurnIndex < Combatants.Count() - 1)
            {
                currentTurnIndex++;
            }
            else currentTurnIndex = 0;

            CurrentTurn = Combatants[currentTurnIndex].ID;
        }

        private void SetNextTurn()
        {
            int currentTurnIndex = GetCurrentTurnIndex();
            if (Combatants.Count >= 2 && currentTurnIndex + 1 >= 1 && currentTurnIndex + 1 <= Combatants.Count() - 1)
            {
                NextTurn = Combatants[currentTurnIndex + 1].ID;
            }
            else if (Combatants.Count >= 2) NextTurn = Combatants[0].ID;
            else NextTurn = null;
        }
        #endregion
    }
}
