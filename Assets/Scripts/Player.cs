using System;
using UnityEngine;


[RequireComponent(typeof(Unit))]
public class Player : MonoBehaviour
{
	public Dungeon Dungeon;

	private int _posX;
	private int _posZ;

	public int X { get { return _posX; } }
	public int Z { get { return _posZ; } }

	private Unit _unit;


	public int HP
	{
		get { return _unit.Health; }
	}


	public SpriteRenderer BodyRenderer;
	public SpriteRenderer HatRenderer;
	public SpriteRenderer ShirtRenderer;
	public SpriteRenderer PantsRenderer;
	public SpriteRenderer RightHandRenderer;


	public GameObject BodyItem;
	public GameObject HatItem;
	public GameObject ShirtItem;
	public GameObject PantsItem;
	public GameObject RightHandItem;


	public void SetBody(Sprite body, Sprite hat, Sprite shirt, Sprite pants, Sprite rightHand,
		GameObject bodyItem, GameObject hatItem, GameObject shirtItem, GameObject pantsItem, GameObject rightHandItem
		)
	{
		BodyRenderer.sprite = body;
		HatRenderer.sprite = hat;
		ShirtRenderer.sprite = shirt;
		PantsRenderer.sprite = pants;
		RightHandRenderer.sprite = rightHand;

		BodyItem = bodyItem;
		HatItem = hatItem;
		ShirtItem = shirtItem;
		PantsItem = pantsItem;
		RightHandItem = rightHandItem;
	}


	void Awake()
	{
		_unit = GetComponent<Unit>();
		Debug.Log("Player Created");
	}


	void Update()
	{
		// bool breaking = Input.GetKey(KeyCode.LeftShift);
		bool shouldTick = false, moved = false;

		var dx = 0;
		var dz = 0;

		if (Input.GetKeyDown(KeyCode.W))
			dz = 1;
		else if (Input.GetKeyDown(KeyCode.S))
			dz = -1;
		else if (Input.GetKeyDown(KeyCode.A))
			dx = -1;
		else if (Input.GetKeyDown(KeyCode.D))
			dx = 1;
		else if (Input.GetKeyDown(KeyCode.Space))
			shouldTick = true;

		if (dx != 0 || dz != 0)
		{
			shouldTick = TryBreak(dx, dz) || TryGo(dx, dz);
			//if (breaking)
			//{
			//	shouldTick = TryBreak(dx, dz);
			//}
			//else
			//{
			//	shouldTick = TryGo(dx, dz);
			//}
			moved = true;
		}

		if (shouldTick)
		{
			if (moved)
			{
				BroadcastMessage("Stepped");
			}

			Dungeon.BroadcastMessage("Tick");
		}
	}


	private bool TryBreak(int dx, int dz)
	{
		var breakX = _posX + dx;
		var breakZ = _posZ + dz;

		if (Dungeon.CanBreak(breakX, breakZ))
		{
			var minX = breakX - 1;
			var minZ = breakZ - 1;
			var maxX = breakX + 1;
			var maxZ = breakZ + 1;

			Dungeon.CarveRoom(minX, minZ, maxX, maxZ, true);

			return true;
		}

		return false;
	}


	private bool TryGo(int dx, int dz)
	{
		var mob = Dungeon.GetUnit(_posX + dx, _posZ + dz);
		if (mob != null)
		{
			_unit.Attack(mob);
			return true;
		}

		if (Dungeon.CanGoTo(_posX + dx, _posZ + dz))
		{
			SetPosition(_posX + dx, _posZ + dz);
			return true;
		}

		return false;
	}


	public void SetPosition(int x, int z)
	{
		_posX = x;
		_posZ = z;
		transform.position = new Vector3(_posX, 0, _posZ);
		Dungeon.RegisterUnit(x, z, _unit);
	}


	public GameObject ReplaceItem(Item item)
	{
		GameObject dropped = null;

		switch (item.Slot)
		{
			case Item.ItemSlot.Unknown:
			case Item.ItemSlot.Body:
				break;
			case Item.ItemSlot.Hat:
				dropped = HatItem;
				HatItem = item.gameObject;
				break;
			case Item.ItemSlot.Shirt:
				dropped = ShirtItem;
				ShirtItem = item.gameObject;
				break;
			case Item.ItemSlot.Pants:
				dropped = PantsItem;
				PantsItem = item.gameObject;
				break;
			case Item.ItemSlot.Weapon:
				dropped = RightHandItem;
				RightHandItem = item.gameObject;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		if (dropped != null)
		{
			dropped.transform.position = item.transform.position;
			dropped.transform.rotation = item.transform.rotation;
			dropped.transform.localScale = item.transform.localScale;
		}

		return dropped;
	}


	public void OnDied()
	{
		Dungeon.BroadcastMessage("OnPlayerDied");
	}
}
