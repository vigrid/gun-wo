using System.Collections.Generic;
using UnityEngine;


public class AutoDisable : MonoBehaviour
{
	private readonly List<SpriteRenderer> _spriteRenderers = new List<SpriteRenderer>();
	public List<SpriteRenderer> SpriteRenderers
	{
		get { return _spriteRenderers; }
	}

	private readonly List<Light> _lights = new List<Light>();
	public List<Light> Lights
	{
		get { return _lights; }
	}

	public Player Player { get; set; }


	void LateUpdate()
	{
		_rays.Clear();
		_hits.Clear();

		if (Player == null)
		{
			return;
		}

		if (SpriteRenderers != null)
		{
			foreach (var spriteRenderer in SpriteRenderers)
			{
				var direction = (Player.transform.position - transform.position);
				var hits = Raycast(direction);

				spriteRenderer.enabled = hits == null || hits.Length == 0;
			}
		}

		if (Lights != null)
		{
			foreach (var light in Lights)
			{
				var direction = (Player.transform.position - transform.position);
				var hits = Raycast(direction);

				light.enabled = hits == null || hits.Length == 0;
			}
		}
	}


	private readonly List<RaycastHit> _hits = new List<RaycastHit>();
	private readonly List<Ray> _rays = new List<Ray>();


	private RaycastHit[] Raycast(Vector3 direction)
	{
		var raycastHits = Physics.RaycastAll(transform.position + Vector3.up * 0.02f, direction.normalized, direction.magnitude);
		_rays.Add(new Ray(transform.position, direction));
		_hits.AddRange(raycastHits);
		return raycastHits;
	}


	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		foreach (var ray in _rays)
		{
			Gizmos.DrawLine(ray.origin, ray.origin + ray.direction);
		}

		Gizmos.color = Color.red;
		foreach (var raycastHit in _hits)
		{
			Gizmos.DrawLine(transform.position, raycastHit.point);
			Gizmos.DrawSphere(raycastHit.point, 0.1f);
		}
	}
}
