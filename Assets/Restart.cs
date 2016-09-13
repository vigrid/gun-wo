using UnityEngine;
using System.Collections;

public class Restart : MonoBehaviour
{
	void Update()
	{
		if (Input.anyKeyDown)
		{
			Application.LoadLevel("CharacterCreator");
		}
	}
}
