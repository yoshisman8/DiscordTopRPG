using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordTopRPG.Database
{
	public abstract class Item
	{
		public string Name { get; set; }
		public int Level { get; set; }
		public string Description { get; set; }
	}
	public class Wearable : Item
	{
		public int Block { get; set; }
		public int Penalty { get; set; }
		public BodySlot Slot { get; set; }
	}
	public class Weapon : Item
	{
		public string DamageDice { get; set; }
		public AbilityScore Score { get; set; }
	}
	public class Consumable : Item
	{
		public int Quantity { get; set; }
	}
	public class MiscItem : Item
	{

	}
}
