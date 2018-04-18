using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    public class HealingPotion : Item      // Child class von Item -> Item abstrakter und Healing Potion ist Differenzierter?
    {
        
        public int AmountToHeal { get; set; }

        public HealingPotion(int id, string name, string namePlural, int amountToHeal, int price) : base(id,name,namePlural, price)
        {
            AmountToHeal = amountToHeal;
        }
    }

}
