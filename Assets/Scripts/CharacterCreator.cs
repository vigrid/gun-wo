using UnityEngine;
using UnityEngine.UI;


public class CharacterCreator : MonoBehaviour
{
	public GameObject[] BodyPrefabs;
	public GameObject[] HatPrefabs;
	public GameObject[] PantPrefabs;
	public GameObject[] ShirtPrefabs;
	public GameObject[] RightHandPrefabs;


	public Transform BodyPivot;
	public Transform HatPivot;
	public Transform PantsPivot;
	public Transform ShirtPivot;
	public Transform RightHandPivot;


	private ArrayIterator<GameObject> _bodyIterator;
	private ArrayIterator<GameObject> _hatIterator;
	private ArrayIterator<GameObject> _pantsIterator;
	private ArrayIterator<GameObject> _shirtIterator;
	private ArrayIterator<GameObject> _rightHandIterator;


	private GameObject _body;
	private GameObject _hat;
	private GameObject _pants;
	private GameObject _shirt;
	private GameObject _rightHand;


	public Text BodyLabel;
	public Text HatLabel;
	public Text PantsLabel;
	public Text ShirtLabel;
	public Text RightHandLabel;


	void Awake()
	{
		_bodyIterator = new ArrayIterator<GameObject>(BodyPrefabs);
		_hatIterator = new ArrayIterator<GameObject>(HatPrefabs);
		_pantsIterator = new ArrayIterator<GameObject>(PantPrefabs);
		_shirtIterator = new ArrayIterator<GameObject>(ShirtPrefabs);
		_rightHandIterator = new ArrayIterator<GameObject>(RightHandPrefabs);

		DontDestroyOnLoad(gameObject);
	}


	private Sprite GetSprite(ArrayIterator<GameObject> iterator, out GameObject item)
	{
		item = iterator.Current;

		if (item != null)
		{
			var spriteRenderer = item.GetComponent<SpriteRenderer>();
			if (spriteRenderer != null)
			{
				return spriteRenderer.sprite;
			}
		}

		return null;
	}


	void Start()
	{
		Refresh();
	}


	private bool TrySetCharacter()
	{
		if (Application.loadedLevelName == "Dungeon")
		{
			var player = FindObjectOfType<Player>();

			if (player != null)
			{
				GameObject bodyItem;
				var bodySprite = GetSprite(_bodyIterator, out bodyItem);
				GameObject hatItem;
				var hatSprite = GetSprite(_hatIterator, out hatItem);
				GameObject shirtItem;
				var shirtSprite = GetSprite(_shirtIterator, out shirtItem);
				GameObject pantsItem;
				var pantsSprite = GetSprite(_pantsIterator, out pantsItem);
				GameObject rightHandItem;
				var rightHandSprite = GetSprite(_rightHandIterator, out rightHandItem);

				player.SetBody(bodySprite, hatSprite, shirtSprite, pantsSprite, rightHandSprite,
					bodyItem, hatItem, shirtItem, pantsItem, rightHandItem
					);

				Destroy(gameObject);

				return true;
			}
		}

		return false;
	}


	void Update()
	{
		TrySetCharacter();
	}


	private void Refresh()
	{
		if (_body != null) Destroy(_body);
		if (_hat != null) Destroy(_hat);
		if (_pants != null) Destroy(_pants);
		if (_shirt != null) Destroy(_shirt);
		if (_rightHand != null) Destroy(_rightHand);

		_body = Create(_bodyIterator.Current, BodyPivot);
		_hat = Create(_hatIterator.Current, HatPivot);
		_pants = Create(_pantsIterator.Current, PantsPivot);
		_shirt = Create(_shirtIterator.Current, ShirtPivot);
		_rightHand = Create(_rightHandIterator.Current, RightHandPivot);

		SetLabel(BodyLabel, _bodyIterator);
		SetLabel(HatLabel, _hatIterator);
		SetLabel(PantsLabel, _pantsIterator);
		SetLabel(ShirtLabel, _shirtIterator);
		SetLabel(RightHandLabel, _rightHandIterator);
	}


	private void SetLabel(Text text, ArrayIterator<GameObject> iterator)
	{
		text.text = iterator.Current != null ? iterator.Current.GetComponent<BodyPart>().GetFullName() : "None";
	}


	private GameObject Create(GameObject prefab, Transform pivot)
	{
		if (prefab != null)
		{
			var instance = (GameObject) Instantiate(prefab);
			instance.transform.parent = pivot;
			instance.transform.localPosition = Vector3.zero;
			instance.transform.localRotation = Quaternion.identity;

			return instance;
		}

		return null;
	}


	public void NGUI_BodyLeft()
	{
		_bodyIterator.Prev();
		Refresh();
	}

	public void NGUI_BodyRight()
	{
		_bodyIterator.Next();
		Refresh();
	}

	public void NGUI_HatLeft()
	{
		_hatIterator.Prev();
		Refresh();
	}

	public void NGUI_HatRight()
	{
		_hatIterator.Next();
		Refresh();
	}

	public void NGUI_PantsLeft()
	{
		_pantsIterator.Prev();
		Refresh();
	}

	public void NGUI_PantsRight()
	{
		_pantsIterator.Next();
		Refresh();
	}

	public void NGUI_ShirtLeft()
	{
		_shirtIterator.Prev();
		Refresh();
	}

	public void NGUI_ShirtRight()
	{
		_shirtIterator.Next();
		Refresh();
	}

	public void NGUI_RHLeft()
	{
		_rightHandIterator.Prev();
		Refresh();
	}

	public void NGUI_RHRight()
	{
		_rightHandIterator.Next();
		Refresh();
	}


	public void StartGame()
	{
		Application.LoadLevel("Dungeon");
	}
}
