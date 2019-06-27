using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordTopRPG.Database
{
    public class Inventory
    {
		public double Money { get; set; } = 0f;
		/// <summary>
		/// The currently worn gear.
		/// </summary>
		public List<Wearable> Worn { get; set; } = new List<Wearable>();
		/// <summary>
		/// The current Active weapon for the Attack command.
		/// </summary>
		public Weapon Weapon { get; set; } = null;
		public int NaturalArmor { get; set; } = 0;
		public List<Wearable> Wearables { get; set; } = new List<Wearable>();
		public List<Consumable> Consumables { get; set; } = new List<Consumable>();
		public List<Weapon> Weapons { get; set; } = new List<Weapon>();
		public List<Item> Misc { get; set; } = new List<Item>();
		/// <summary>
		/// Condensed list of all items.
		/// </summary>
		public List<Item> Items()
		{
			return new List<Item>().Concat(Wearables).Concat(Consumables).Concat(Weapons).Concat(Misc).ToList();
		}
			
		public bool Wear(Wearable wearable)
		{
			try
			{
				if (Worn.Exists(x => x.Slot == wearable.Slot))
				{
					var I = Worn.Find(x => x.Slot == wearable.Slot);
					Worn.Remove(I);
					Wearables.Add(I);
				}
				Worn.Add(wearable);
				Wearables.Remove(wearable);
				return true;
			}
			catch
			{
				return false;
			}
		}
		public bool wield(Weapon weapon)
		{
			try
			{
				if (Weapon!=null)
				{
					Weapons.Add(this.Weapon);
				}
				this.Weapon = weapon;
				Weapons.Remove(weapon);
				return true;
			}
			catch
			{
				return false;
			}
		}
		public bool Consume(Consumable item)
		{
			var i = Consumables.IndexOf(item);
			if (Consumables[i].Quantity <= 0) return false;
			Consumables[i].Quantity--;
			return true;
		}
		public int GetBlockBonus()
		{
			int bonus = 0;
			bonus += Worn.Select(x => x.Block).Sum();
			return bonus;
		}
		public int GetAgiPenalty()
		{
			int bonus = 0;
			bonus += Worn.Select(x => x.Penalty).Sum();
			return bonus;
		}
    }
	public enum BodySlot { None=-1,Head,Body,Gloves,Boots,Accessory}
}
