using System;
using System.Linq;
using UnityEngine;


public class BodyPart : MonoBehaviour
{
	public string PartName;

	public int HealthModifier;
	public int AttackModifier;
	public int DefenseModifier;

	public string GetFullName()
	{
		var mods = new[] {
			HealthModifier != 0 ? string.Format("{0:+0;-0} Badass", HealthModifier) : "",
			AttackModifier != 0 ? string.Format("{0:+0;-0} Kickass", AttackModifier) : "",
			DefenseModifier != 0 ? string.Format("{0:+0;-0} Cowardass", DefenseModifier) : ""
		};

		var modsText = string.Join(", ", mods.Where(m => !string.IsNullOrEmpty(m)).ToArray());

		return string.Format("{0}{1}{2}", PartName, modsText.Length > 0 ? Environment.NewLine : "", modsText);
	}
}
