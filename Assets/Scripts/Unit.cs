using UnityEngine;


[RequireComponent(typeof(AutoDisable))]
public class Unit : MonoBehaviour
{
	public int Health = 20;
	public int MinAttack = 4;
	public int MaxAttack = 6;
	public int Defense = 2;


	public AudioClip AttackSound;
	public AudioClip DeathSound;
	public AudioClip[] StepSounds;

	public GameObject DamagePrefab;


	public void TakeDamage(int damage, Transform from)
	{
		var realDamage = Mathf.Max(0, damage - Defense);

		if (realDamage > 0)
		{
			Health -= realDamage;
			if (DamagePrefab != null)
			{
				var damageObject = (GameObject) Instantiate(DamagePrefab, from.position, Quaternion.identity);
				damageObject.transform.LookAt(transform.position);
				damageObject.transform.position = transform.position;
				damageObject.transform.position += Vector3.up * 0.25f;
			}
		}

		if (Health <= 0)
		{
			SendMessage("Break");
			SendMessage("OnDied");
			SoundDie();
			DestroyImmediate(gameObject);
		}
	}


	private void SoundDie()
	{
		if (DeathSound != null)
		{
			AudioSource.PlayClipAtPoint(DeathSound, transform.position, 20.0f);
		}
	}


	public void Attack(Unit unit)
	{
		if (AttackSound != null)
		{
			AudioSource.PlayClipAtPoint(AttackSound, transform.position, 20.0f);
		}

		var damage = Random.Range(MinAttack, MaxAttack);
		unit.GetComponent<Unit>().TakeDamage(damage, transform);
	}


	public void Stepped()
	{
		if (StepSounds != null && StepSounds.Length > 0)
		{
			AudioSource.PlayClipAtPoint(StepSounds.GetRandom(), transform.position, 5.0f);
		}
	}
}
