using Gamination;
using UnityEngine;


[RequireComponent(typeof(Unit))]
public class Mob : Entity
{
	public GameObject BreaksInto;
	public GameObject Item { get; set; }


	void Awake()
	{
		Unit = GetComponent<Unit>();
	}


	public void Tick()
	{
		var dijkstra = new TileDijkstra(Dungeon);

		var path = dijkstra.FindPath(new Vector2I(PosX, PosZ), CheckIfNearToPlayer);

		if (path != null && path.Count > 1)
		{
			BroadcastMessage("Stepped");
			SetPosition(path[1].X, path[1].Z);
		}
		else
		{
			if (CheckIfNearToPlayer(new Vector2I(PosX, PosZ)))
			{
				Unit.Attack(Dungeon.PlayerInstance.GetComponent<Unit>());
			}
		}
	}


	private bool CheckIfNearToPlayer(Vector2I position)
	{
		var tx = Dungeon.PlayerInstance.X;
		var tz = Dungeon.PlayerInstance.Z;

		var dx = Mathf.Abs(tx - position.X);
		var dz = Mathf.Abs(tz - position.Z);

		return dx + dz == 1;
	}


	void Break()
	{
		if (BreaksInto != null)
		{
			//if (Item != null)
			//{
			//	Dungeon.Spawn<Item>(PosX, PosZ, new [] {Item});
			//}

			if (BreaksInto.GetComponent<Prop>() != null)
			{
				Dungeon.Spawn<Prop>(PosX, PosZ, BreaksInto);
				return;
			}

			var instance = (GameObject) Instantiate(BreaksInto, transform.position + Vector3.down * 0.01f, transform.rotation);

			var autoDisable = instance.GetComponentInChildren<AutoDisable>();
			if (autoDisable != null)
			{
				autoDisable.Player = Dungeon.PlayerInstance;

				var spriteRenderers = instance.GetComponentsInChildren<SpriteRenderer>();
				var lights = instance.GetComponentsInChildren<Light>();

				autoDisable.SpriteRenderers.AddRange(spriteRenderers);
				autoDisable.Lights.AddRange(lights);
			}
		}
	}
}
