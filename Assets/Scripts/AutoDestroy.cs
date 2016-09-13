using UnityEngine;


public class AutoDestroy : MonoBehaviour
{
	public float Delay = 3.0f;

	void Start()
	{
		Destroy(gameObject, Delay);
	}
}
