using UnityEngine;

public class Prop : Entity
{
	public GameObject[] BreaksInto;

	void Break()
	{
		if (BreaksInto != null && BreaksInto.Length > 0)
		{
			Debug.Log(string.Format("Breaking: {0}, {1}", PosX, PosZ));

			var newObject = BreaksInto.GetRandom();

			if (newObject.GetComponent<Mob>() != null)
			{
				Dungeon.Spawn<Mob>(PosX, PosZ, BreaksInto);
			}
			else if (newObject.GetComponent<Prop>() != null)
			{
				Dungeon.Spawn<Prop>(PosX, PosZ, BreaksInto);
			}
		}
	}
}
