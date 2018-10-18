using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to modify the damage to a propuslion type (or structure) based on
 * weapon.
 */
public enum WEAPON_EFFECT : byte {
	None,
	ANTI_PERSONNEL,
	ANTI_TANK,
	BUNKER_BUSTER,
	ARTILLERY_ROUND,
	FLAMER,
	ANTI_AIRCRAFT,
	ALL_ROUNDER,
	NUMEFFECTS,
	/**  The number of enumerators in this enum. */
};

/**
	* Used to define which projectile model to use for the weapon.
	*/
public enum MOVEMENT_MODEL : byte {
	None,
	DIRECT,
	INDIRECT,
	HOMING_DIRECT,
	HOMING_INDIRECT
};

/// <summary>
/// Тип шасси
/// </summary>
public enum WeaponType : byte {
	None = 0 // Undifined

	//Class
	, KINETIC, HEAT

	//SubClass
	, CANNON, ENERGY, FLAME, MACHINE_GUN, ROCKET, BOMB, EMP, COMMAND, MISSILE, GAUSS, MORTARS, HOWITZERS, ELECTRONIC, A_A_GUN, LAS_SAT
}

public enum WeaponEnums : byte {
	None = 0 // Undefined
	/// <summary>
	/// Time required to build the component
	/// </summary>
	, buildPoints = 1 // 450,
	/// <summary>
	/// Power required to build the component
	/// </summary>
	, buildPower = 2 // 250,
	/// <summary>
	/// HP
	/// </summary>
	, hitpoints = 3 //1
	/// <summary>
	/// Component's weight
	/// </summary>
	, weight = 4 //10
	/// <summary>
	/// damage
	/// </summary>
	, damage = 5 // 110,
	/// <summary>
	/// size of the effect 100 = normal,	50 = half etc
	/// </summary>
	, effectSize = 7 // 100,
	/// <summary>
	/// flag to make the (explosion) effect face the	player when	drawn
	/// </summary>
	, facePlayer = 8 // 1,
	/// <summary>
	/// Pause between each shot
	/// </summary>
	, firePause = 9 // 65,
	/// <summary>
	/// speed ammo travels at
	/// </summary>
	, flightSpeed = 10 // 2250,
	/// <summary>
	/// flag to indicate whether the effect lights up the world
	/// </summary>
	, lightWorld = 11 // 1,
	, longRange = 12 // 1536,
	, longHit = 13 // 73,
	/// <summary>
	/// max amount the	turret can be elevated up
	/// </summary>
	, maxElevation = 14 // 90,
	/// <summary>
	/// min amount the	turret can be elevated down
	/// </summary>
	, minElevation = 15 // -45,
	/// <summary>
	/// Minimum amount of damage done, in percentage of damage
	/// </summary>
	, minimumDamage = 16 // 33,
	/// <summary>
	/// The number of explosions per shot
	/// </summary>
	, numExplosions = 17 // 2,
	/// <summary>
	/// Basic blast radius of weapon
	/// </summary>
	, radius = 18 // 256,
	/// <summary>
	/// "Splash damage"
	/// </summary>
	, radiusDamage = 19 // 120,
	/// <summary>
	/// How long a blast radius is visible
	/// </summary>
	, radiusLife = 20 // 100,
	/// <summary>
	/// used to compare with weight to see if recoils or not
	/// </summary>
	, recoilValue = 21 // 100,
	/// <summary>
	/// amount the weapon(turret) can rotate 0	= none
	/// </summary>
	, rotate = 22 // 180,
	/// <summary>
	/// indicates whether the droid has to stop before firing
	/// </summary>
	, fireOnMove = 23 // 0
	, minRange = 24 // 128
	/// <summary>
	/// Repeat damage each second after hit
	/// </summary>
	, periodicalDamage = 25 // 22
	/// <summary>
	/// Repeat damage radius
	/// </summary>
	, periodicalDamageRadius = 26 // 128
	/// <summary>
	/// How long the round keeps damaging
	/// </summary>
	, periodicalDamageTime = 27 // 60
	/// <summary>
	/// The number of rounds per salvo
	/// </summary>
	, numRounds = 28 // 1
	/// <summary>
	/// Time to reload the round of ammo
	/// </summary>
	, reloadTime = 29 // 10
	, numAttackRuns = 30 // 1
	/// <summary>
	/// flag to make the inflight effect	face the player when drawn
	/// </summary>
	, faceInFlight = 31 // 1
	/// <summary>
	/// flag to indicate whether pentrate droid or not
	/// </summary>
	, penetrate = 32 // 1
}

/// <summary>
///  Структура данных Шасси
/// </summary>
public class WeaponClass : BaseDataClass {
	/// <summary>
	/// Тип подвески
	/// </summary>
	//public WeaponType gtype;

	public string explosionWav; // lrgexpl.ogg,
	public string hitGfx; // FXAIREXP.PIE,
	public string flightGfx; // FXCAmmo.PIE,
	public string missGfx; // FXAIREXP.PIE,
	public MOVEMENT_MODEL movement; // HOMING-DIRECT,
	public string muzzleGfx; // FxCan75m.PIE,
	public string waterGfx; // FXSSplsh.PIE,
	public WEAPON_EFFECT weaponEffect; // ANTI TANK,
	public string weaponWav; // medcan.ogg,
	public string flags; // AirOnly,
	public string periodicalDamageWeaponEffect; //": "FLAMER",

	/// <summary>
	/// the class of weapon  (KINETIC, HEAT)
	/// </summary>
	public WeaponType weaponClass;
	/// <summary>
	/// the subclass to which the weapon belongs (research class)
	/// </summary>
	public WeaponType weaponSubClass;
	/// <summary>
	/// Periodical damage weapon class by damage type (KINETIC, HEAT)
	/// </summary>
	public WeaponType periodicalDamageWeaponClass;
	/// <summary>
	/// Periodical damage weapon subclass (research class)
	/// </summary>
	public WeaponType periodicalDamageWeaponSubClass;

	public Dictionary<WeaponEnums, int> values = new Dictionary<WeaponEnums, int> ();
	public static Dictionary<WeaponEnums, int> maxValues = new Dictionary<WeaponEnums, int> ();

	public WeaponClass () { }

	public WeaponClass (JsonObject pData) {
		Init (pData);
	}

	public override void Init (JsonObject pData) {
		base.Init (pData);
		type = StatType.Wpn;
		explosionWav = pData.Get<string> ("explosionWav", "");
		hitGfx = pData.Get<string> ("hitGfx", "");
		flightGfx = pData.Get<string> ("flightGfx", "");
		missGfx = pData.Get<string> ("missGfx", "");
		movement = GetMModel (pData.Get<string> ("movement", "None"));
		muzzleGfx = pData.Get<string> ("muzzleGfx", "");
		waterGfx = pData.Get<string> ("waterGfx", "");
		weaponEffect = GetWEffect (pData.Get<string> ("weaponEffect", "None"));
		weaponWav = pData.Get<string> ("weaponWav", "");
		flags = pData.Get<string> ("flags", "");

		weaponClass = GetType (pData.Get<string> ("weaponClass", "None"));
		weaponSubClass = GetType (pData.Get<string> ("weaponSubClass", "None"));
		periodicalDamageWeaponClass = GetType (pData.Get<string> ("periodicalDamageWeaponClass", "None"));
		periodicalDamageWeaponSubClass = GetType (pData.Get<string> ("periodicalDamageWeaponSubClass", "None"));

		values.Clear ();
		foreach (WeaponEnums e in System.Enum.GetValues (typeof (WeaponEnums))) {
			int v = pData.Get<int> (e.ToString ());
			if (pData.ContainsKey (e.ToString ())) {
				values[e] = v;
				if (!maxValues.ContainsKey (e) || maxValues[e] < v)
					maxValues[e] = v;
#if CHECK_VARIABLES
				pData.Remove (e.ToString ());
#endif
			}

		}
		//type = GetType(pData.Get<string> ("type", "None"));

#if CHECK_VARIABLES
		pData.Remove ("id");
		pData.Remove ("name");
		pData.Remove ("model");

		pData.Remove ("explosionWav");
		pData.Remove ("hitGfx");
		pData.Remove ("flightGfx");
		pData.Remove ("missGfx");
		pData.Remove ("mountModel");
		pData.Remove ("movement");
		pData.Remove ("muzzleGfx");
		pData.Remove ("waterGfx");
		pData.Remove ("weaponClass");
		pData.Remove ("weaponEffect");
		pData.Remove ("weaponSubClass");
		pData.Remove ("weaponWav");
		pData.Remove ("flags");
		pData.Remove ("periodicalDamageWeaponClass");
		pData.Remove ("periodicalDamageWeaponSubClass");
		pData.Remove ("periodicalDamageWeaponEffect");
		pData.Remove ("designable");

		foreach (string s in pData.Keys)
			Debug.LogError ("unknown key " + s + " in " + name);
#endif		
	}

	/// <summary>
	/// Возвращает тип подвески по имени
	/// </summary>
	public static WeaponType GetType (string pName) {
		try {
			return (WeaponType) System.Enum.Parse (typeof (WeaponType), pName.Replace ("-", "_").Replace (" ", "_"));
		} catch {
			Debug.LogError ("unknown type " + pName);
		}
		return WeaponType.None;
	}

	/// <summary>
	/// MOVEMENT_MODEL
	/// </summary>
	public static MOVEMENT_MODEL GetMModel (string pName) {
		try {
			return (MOVEMENT_MODEL) System.Enum.Parse (typeof (MOVEMENT_MODEL), pName.Replace ("-", "_").Replace (" ", "_"));
		} catch {
			Debug.LogError ("unknown type " + pName);
		}
		return MOVEMENT_MODEL.None;
	}

	/// <summary>
	/// WEAPON_EFFECT
	/// </summary>
	public static WEAPON_EFFECT GetWEffect (string pName) {
		try {
			return (WEAPON_EFFECT) System.Enum.Parse (typeof (WEAPON_EFFECT), pName.Replace ("-", "_").Replace (" ", "_"));
		} catch {
			Debug.LogError ("unknown type " + pName);
		}
		return WEAPON_EFFECT.None;
	}

	/// <summary>
	/// Набор параметров для сравнения и отображения
	/// </summary>
	public override List<StatClass> stats () {
		List<StatClass> stats = new List<StatClass> ();
		foreach (WeaponEnums e in System.Enum.GetValues (typeof (WeaponEnums))) {
			if (values.ContainsKey (e))
				stats.Add (new StatClass (e.ToString (), values[e], maxValues[e]));
		}
		return stats;
	}

	public override string ToString () {
		string v = "\nspecific\n" + string.Format(
		" explosionWav: [{0}]\n hitGfx: [{1}]\n flightGfx: [{2}]\n missGfx: [{3}]\n movement: [{4}]\n muzzleGfx: [{5}]\n waterGfx: [{6}]\n"+
		" weaponEffect: [{7}]\n weaponWav: [{8}]\n flags: [{9}]\n weaponClass: [{10}]\n weaponSubClass: [{11}]\n periodicalDamageWeaponClass: [{12}]\n"+
		" periodicalDamageWeaponSubClass: [{13}]\n",
		explosionWav, hitGfx, flightGfx, missGfx, movement, muzzleGfx, waterGfx, 
		weaponEffect, weaponWav, flags, weaponClass, weaponSubClass, periodicalDamageWeaponClass, 
		periodicalDamageWeaponSubClass);

		foreach (WeaponEnums e in System.Enum.GetValues (typeof (WeaponEnums))) {
			if (values.ContainsKey (e))
				v += " "+e.ToString () + ": [" + values[e].ToString () + "]\n";
		}
		return base.ToString() + v;
	}

}