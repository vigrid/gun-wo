public class Item : Entity
{
	public enum ItemSlot
	{
		Unknown,
		Body,
		Hat,
		Shirt,
		Pants,
		Weapon
	}

	public ItemSlot Slot;
}