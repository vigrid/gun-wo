using UnityEngine;


// [RequireComponent(typeof(Unit))]
public class Entity : MonoBehaviour
{
	protected Unit Unit;
	protected int PosX;
	protected int PosZ;


	private Dungeon _dungeon;
	public Dungeon Dungeon
	{
		get
		{
			_dungeon = _dungeon ?? GameObject.Find("Dungeon").GetComponent<Dungeon>();
			return _dungeon;
		}
	}


	void Awake()
	{
		Unit = GetComponent<Unit>();
	}


	public void SetPosition(int x, int z)
	{
		PosX = x;
		PosZ = z;
		transform.position = new Vector3(PosX, 0, PosZ);

		if (Unit != null)
		{
			Dungeon.RegisterUnit(x, z, Unit);
		}
	}
}
