using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordTopRPG.Database
{
    public class Inventory
    {
		public double Money { get; set; } = 0f;
		public List<Wearable> Worn { get; set; } = new List<Wearable>();
		public Weapon Weapon { get; set; } = null;
		public List<Item> Items { get; set; } = new List<Item>();
		public bool Wear(Wearable wearable)
		{
			try
			{
				if (Worn.Exists(x => x.Slot == wearable.Slot))
				{
					var I = Worn.Find(x => x.Slot == wearable.Slot);
					Worn.Remove(I);
					Items.Add(I);
				}
				Worn.Add(wearable);
				Items.Remove(wearable);
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
					Items.Add(Weapon);
				}
				Weapon = weapon;
				Items.Remove(weapon);
				return true;
			}
			catch
			{
				return false;
			}
		}
		public bool Consume(Consumable item)
		{
			var Is = Items.Where(x => x.GetType() == typeof(Consumable));
			var I = Items.FindIndex(x => x.Name == item.Name && x.Level == item.Level);
			if (((Consumable)Items[I]).Quantity >= 0) return false;
			((Consumable)Items[I]).Quantity--;
			return true;
		}
    }
	public enum BodySlot { None,Head,Body,Gloves,Boots,MainHand,OffHand,Accessory}
}
