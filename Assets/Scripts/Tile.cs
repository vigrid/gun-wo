using System.Collections.Generic;
using UnityEngine;


public class Tile : MonoBehaviour
{
	public TileType Type;
	public bool Breakable = false;
	public int HitsToBreak = 1;
	public Tile BreaksInto;

	public bool Walkable = true;

	public Unit UnitOnTile { get; set; }
	public List<Item> ItemsOnTile { get; set; }


	public void Hit()
	{
		if (Breakable)
		{
			if (HitsToBreak > 0)
			{
				HitsToBreak--;
			}

			if (HitsToBreak == 0)
			{
				if (BreaksInto != null)
				{
					Instantiate(BreaksInto, transform.position, transform.rotation);
				}
				 
				Destroy(gameObject);
			}
		}
	}
}
