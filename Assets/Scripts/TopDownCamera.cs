using UnityEngine;


public class TopDownCamera : MonoBehaviour
{
	public static TopDownCamera Instance { get; private set; }


	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}


	public GameObject Target { get; set; }

	public float Height;
	public float MinHeight = 4;
	public float MaxHeight = 20;
	public float Damping;


	void Update()
	{
		var wheel = Input.GetAxis("Mouse ScrollWheel");
		Height = Mathf.Clamp(Height + wheel * 5.0f, MinHeight, MaxHeight);
	}


	void LateUpdate()
	{
		if (Target != null)
		{
			var t = Mathf.Pow(Time.deltaTime, Damping);

			var newPosition = Vector3.Lerp(transform.position, Target.transform.position + Vector3.up * Height, t);

			transform.position = newPosition;
			transform.rotation = Quaternion.LookRotation(Vector3.down.normalized, Vector3.forward);
		}
	}
}
