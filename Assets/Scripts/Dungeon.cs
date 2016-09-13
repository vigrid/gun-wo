using System.Collections.Generic;
using System.Linq;
using Gamination;
using UnityEngine;
using UnityEngine.UI;


public class Dungeon : MonoBehaviour, ITileMap
{
	public GameObject NotifyOnPlayerDied;
	public GameObject NotifyOnPlayerWon;

	public Text StatusLabel;


	void OnPlayerDied()
	{
		NotifyOnPlayerDied.SetActive(true);
	}


	public int SizeX = 40;
	public int SizeZ = 30;

	public int Rooms = 5;
	public int MinRoomSize = 5;
	public int MaxRoomSize = 10;

	public Player PlayerPrefab;
	public Tile[] TilePrefabs;
	public GameObject[] MobPrefabs;
	public GameObject[] PropPrefabs;
	public GameObject[] ItemPrefabs;

	public GameObject GoldPrefab;

	public GameObject SpatulaPrefab;

	public void Initialize(int sizeX, int sizeZ)
	{
		Tiles = new Tile[sizeX, sizeZ];
	}


	public Tile[,] Tiles { get; set; }


	public int Seed = 42;


	public void Start()
	{
		if (Seed != 0)
		{
			Random.seed = Seed;
		}

		int playerX = -1;
		int playerZ = -1;

		Initialize(SizeX, SizeZ);

		for (int i = 0; i < Rooms; i++)
		{
			var minX = Random.Range(0, SizeX - MaxRoomSize - 1);
			var minZ = Random.Range(0, SizeZ - MaxRoomSize - 1);

			var sizeX = Random.Range(MinRoomSize, MaxRoomSize);
			var sizeZ = Random.Range(MinRoomSize, MaxRoomSize);

			var maxX = minX + sizeX;
			var maxZ = minZ + sizeZ;

			CarveRoom(minX, minZ, maxX, maxZ, false);

			if (PlayerInstance == null)
			{
				playerX = (minX + maxX) / 2;
				playerZ = (minZ + maxZ) / 2;
				PlayerInstance = (Player) Instantiate(PlayerPrefab);
				PlayerInstance.Dungeon = this;
				PlayerInstance.SetPosition(playerX, playerZ);

				//var spatula = Spawn<Item>(minX, minZ, maxX, maxZ, new [] {SpatulaPrefab});
				//spatula.name = string.Format("Spatula {0}", i + 1);
			}

			if (Random.value < 0.75f)
			{
				var mob = Spawn<Mob>(minX, minZ, maxX, maxZ, MobPrefabs);
				mob.name = string.Format("Mob {0}", i + 1);

				if (Random.value < 0.25f)
				{
					if (ItemPrefabs != null && ItemPrefabs.Length > 0)
					{
						var itemPrefab = ItemPrefabs.GetRandom();
						mob.Item = itemPrefab;
					}
				}
				else
				{
					mob.Item = GoldPrefab;
				}
			}
			else
			{
				var prop = Spawn<Prop>(minX, minZ, maxX, maxZ, PropPrefabs);
				prop.name = string.Format("Prop {0}", i + 1);
			}
		}

		TopDownCamera.Instance.Target = PlayerInstance.gameObject;
	}


	public void Update()
	{
		var allMobs = FindObjectsOfType<Mob>();
		StatusLabel.text = string.Format("HP: {0}, Monsters: {1}", PlayerInstance.HP, allMobs.Length);

		if (allMobs.Length == 0)
		{
			NotifyOnPlayerWon.SetActive(true);
		}
	}


	private Tile GetTilePrefab(TileType type)
	{
		var candidates = TilePrefabs.Where(t => t.Type == type).ToArray();
		return candidates.Length > 0 ? candidates.GetRandom() : null;
	}


	public TComponent Spawn<TComponent>(int minX, int minZ, int maxX, int maxZ, GameObject[] prefabs)
		where TComponent : Entity
	{
		int x, z;

		while (true)
		{
			x = Random.Range(minX + 1, maxX - 1);
			z = Random.Range(minZ + 1, maxZ - 1);

			if (Tiles[x, z].UnitOnTile == null)
				return Spawn<TComponent>(x, z, prefabs);
		}
	}


	public TComponent Spawn<TComponent>(int x, int z, GameObject[] prefabs)
		where TComponent : Entity
	{
		var prefab = prefabs.GetRandom();

		return Spawn<TComponent>(x, z, prefab);
	}


	public TComponent Spawn<TComponent>(int x, int z, GameObject prefab) where TComponent : Entity
	{
		Debug.Log(string.Format("Spawning {0} from {1}", typeof(TComponent), prefab.name));

		var instance = (GameObject) Instantiate(prefab);

		instance.gameObject.transform.parent = transform;

		var entity = instance.GetComponent<TComponent>();
		entity.SetPosition(x, z);

		var autoDisable = instance.GetComponentInChildren<AutoDisable>();
		if (autoDisable != null)
		{
			autoDisable.Player = PlayerInstance;

			var spriteRenderers = instance.GetComponentsInChildren<SpriteRenderer>();
			var lights = instance.GetComponentsInChildren<Light>();

			autoDisable.SpriteRenderers.AddRange(spriteRenderers);
			autoDisable.Lights.AddRange(lights);
		}

		if (typeof (TComponent) == typeof (Item))
		{
			// Tiles[x, z].ItemsOnTile.Add((Item) (object) entity);
		}

		if (entity.GetComponent<BodyPart>() != null)
		{
			//entity.transform.Rotate(Vector3.right, 90.0f);
			//entity.transform.localScale = Vector3.one / 2.0f;
			//entity.transform.position += new Vector3(0.0f, 0.0f, -0.5f);
		}

		return entity;
	}


	public void Tick()
	{
		var itemsOnTile = Tiles[PlayerInstance.X, PlayerInstance.Z].ItemsOnTile;
		if (itemsOnTile != null && itemsOnTile.Count > 0)
		{
			PickUpItems(itemsOnTile);
		}
	}


	private void PickUpItems(List<Item> itemsOnTile)
	{
		Debug.Log(string.Format("Picking up {0} item(s)", itemsOnTile.Count));

		var allDropped = new List<GameObject>();

		foreach (var item in itemsOnTile)
		{
			var dropped = PlayerInstance.ReplaceItem(item);
			if (dropped != null)
			{
				allDropped.Add(dropped);
			}
		}

		itemsOnTile.Clear();
		itemsOnTile.AddRange(allDropped.Select(x => x.GetComponent<Item>()));
	}


	private TileType GetTileType(int x, int z)
	{
		var tile = Tiles[x, z];
		return tile != null ? tile.Type : TileType.Empty;
	}


	public void CarveRoom(int minX, int minZ, int maxX, int maxZ, bool digging)
	{
		for (int x = minX; x <= maxX; x++)
		{
			for (int z = minZ; z <= maxZ; z++)
			{
				if (x < 0 || z < 0 || x >= SizeX || z >= SizeZ)
					continue;

				var edge = x == minX || x == maxX || z == minZ || z == maxZ;
				var existing = GetTileType(x, z);

				if (!digging)
				{
					if (existing == TileType.Empty || existing == TileType.Wall)
					{
						SetTileType(x, z, edge ? TileType.Wall : TileType.Floor);
					}
				}
				else
				{
					if (!edge)
					{
						SetTileType(x, z, TileType.Floor);
					}
					else
					{
						if (existing == TileType.Empty)
						{
							SetTileType(x, z, TileType.Wall);
						}
					}
				}
			}
		}
	}


	private void SetTileType(int x, int z, TileType tileType)
	{
		var existingTile = Tiles[x, z];

		if (existingTile == null)
		{
			CreateTile(x, z, tileType, null);
		}
		else if (existingTile.Type != tileType)
		{
			CreateTile(x, z, tileType, existingTile);
			Destroy(existingTile.gameObject);
		}
	}


	private void CreateTile(int x, int z, TileType tileType, Tile existingTile)
	{
		var newTile = (Tile) Instantiate(GetTilePrefab(tileType), new Vector3(x, 0, z), Quaternion.identity);
		newTile.transform.parent = transform;

		if (existingTile != null)
		{
			newTile.UnitOnTile = existingTile.UnitOnTile;
			newTile.ItemsOnTile = existingTile.ItemsOnTile;
		}
		else
		{
			newTile.ItemsOnTile = new List<Item>();
		}

		Tiles[x, z] = newTile;
	}


	public bool CanGoTo(int x, int z	)
	{
		return GetTileType(x, z) != TileType.Wall;
	}


	public bool CanBreak(int x, int z)
	{
		if (x < 1 || x >= SizeX - 1 || z < 1 || x >= SizeZ - 1)
			return false;

		return Tiles[x, z].Breakable;
	}


	public int Rows
	{
		get { return SizeX; }
	}


	public int Cols
	{
		get { return SizeZ; }
	}


	public Player PlayerInstance { get; set; }


	public Tile this[int x, int z]
	{
		get { return Tiles[x, z]; }
	}


	public bool IsPassable(int x, int z)
	{
		if (x < 0 || z < 0 || x >= SizeX || z >= SizeZ)
		{
			return false;
		}

		var tile = Tiles[x, z];

		return tile != null && tile.Walkable && tile.UnitOnTile == null;
	}


	private readonly Dictionary<Unit, Tile> _unitsToTiles = new Dictionary<Unit, Tile>(); 


	public void RegisterUnit(int x, int z, Unit unit)
	{
		if (_unitsToTiles.ContainsKey(unit))
		{
			_unitsToTiles[unit].UnitOnTile = null;
		}

		Tiles[x, z].UnitOnTile = unit;
		_unitsToTiles[unit] = Tiles[x, z];
	}


	public Unit GetUnit(int x, int z)
	{
		if (x < 0 || z < 0 || x >= SizeX || z >= SizeZ)
			return null;

		return Tiles[x, z].UnitOnTile;
	}
}
