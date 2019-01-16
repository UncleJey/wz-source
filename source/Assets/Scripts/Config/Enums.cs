using UnityEngine;
using UnityEditor;

public static class CellTypes
{
	public const int Disabled = 65000;
}

public static class Slots
{
	/// <summary>
	/// Чей номер слота в Body
	/// </summary>
	public static int GetSlotNum(StatType pType)
	{
		int slot = 0; // Wpn1, Sys
		switch (pType)
		{
			case StatType.Wpn2: 
				slot = 1;
			break;
			default:
			break;
		}
		return slot;
	}

	/// <summary>
	/// Подобрать нужный слот
	/// </summary>
	public static Vector3 ChooseConnector(Vector3[] connectors, StatType pType)
	{
		int slotNo = GetSlotNum(pType);
		if (connectors != null && slotNo < connectors.Length)
		{
			return connectors[slotNo];
		}
		#if UNITY_EDITOR
		Debug.LogError("Connector chooose Error");
		#endif
		return Vector3.zero;
	}
}

/// <summary>
/// Тип элемента
/// </summary>
public enum StatType : byte
{
	 None			= 0	
	,Wpn			= 1 // Оружие
	,Comp			= 2
	,Cydorg			= 3
	,Defense		= 4
	,Research		= 5
	,Struc			= 6	 // Строения
	,Sys			= 7	 // Радары
	,Vehicle		= 8	 // Тело транспорта
	,Templates		= 9  // Заготовки
	,Messages		= 10 // Тексты сообщений с настройками
	,Body			= 11 // Корпусы
	,Propulsion		= 12 // Шасси
	,Propulsion_typ	= 13 // Тип шасси
	,Propulsion_snd	= 14 // Звуки шасси
	,Repair			= 15 // Типы восстановлений
	,Construction	= 16 // Прочие сооружения (кузов ремонтника там)
	,Wpn1			= 17
	,Wpn2			= 18
	,Wpn3			= 19
	,Wpn4			= 20
}

public static class PropertyType 
{

	public const string NONE			= "NONE";

	// --------- Structure ----------
	public const string strength		= "strength";
	public const string sensorID		= "sensorID";
	public const string baseModel		= "baseModel";
	public const string ecmID			= "ecmID";
	public const string flags			= "flags";
	public const string flightGfx		= "flightGfx";
	public const string hitGfx			= "hitGfx";
	public const string id				= "id";
	public const string missGfx			= "missGfx";
	public const string model			= "model";
	public const string mountModel		= "mountModel";
	public const string movement		= "movement";
	public const string muzzleGfx		= "muzzleGfx";
	public const string name			= "name";
	public const string waterGfx		= "waterGfx";
	public const string weaponClass		= "weaponClass";
	public const string weaponEffect	= "weaponEffect";
	public const string weaponSubClass	= "weaponSubClass";
	public const string weaponWav		= "weaponWav";
	public const string explosionWav	= "explosionWav";
	public const string periodicalDamageWeaponClass		= "periodicalDamageWeaponClass";
	public const string periodicalDamageWeaponEffect	= "periodicalDamageWeaponEffect";
	public const string periodicalDamageWeaponSubClass	= "periodicalDamageWeaponSubClass";
	public const string location		= "location";
	public const string sensorModel		= "sensorModel";
	public const string body			= "body";
	public const string propulsion		= "propulsion";
	public const string repair			= "repair";
	public const string construct		= "construct";
	public const string brain			= "brain";
	public const string available		= "available";
	public const string sensor			= "sensor";
	public const string class_			= "class";
	public const string size			= "size";
	public const string intVal			= "intVal";
	public const string armour			= "armour";
	public const string breadth			= "breadth";
	public const string height			= "height";
	public const string resistance		= "resistance";
	public const string width			= "width";
	public const string type			= "type";
	public const string productionPoints			= "productionPoints";
	public const string powerPoints					= "powerPoints";
	public const string modulePowerPoints			= "modulePowerPoints";
	public const string moduleProductionPoints		= "moduleProductionPoints";
	public const string repairPoints				= "repairPoints";
	public const string researchPoints				= "researchPoints";
	public const string moduleResearchPoints		= "moduleResearchPoints";
	public const string rearmPoints		= "rearmPoints";
	public const string buildPoints		= "buildPoints";
	public const string buildPower		= "buildPower";
	public const string damage			= "damage";
	public const string designable		= "designable";
	public const string effectSize		= "effectSize";
	public const string facePlayer		= "facePlayer";
	public const string firePause		= "firePause";
	public const string flightSpeed		= "flightSpeed";
	public const string hitpoints		= "hitpoints";
	public const string longRange		= "longRange";
	public const string longHit			= "longHit";
	public const string maxElevation	= "maxElevation";
	public const string minElevation	= "minElevation";
	public const string minimumDamage	= "minimumDamage";
	public const string numExplosions	= "numExplosions";
	public const string recoilValue		= "recoilValue";
	public const string rotate			= "rotate";
	public const string weight			= "weight";
	public const string lightWorld		= "lightWorld";
	public const string penetrate		= "penetrate";
	public const string periodicalDamageTime		= "periodicalDamageTime";
	public const string periodicalDamageRadius		= "periodicalDamageRadius";
	public const string periodicalDamage			= "periodicalDamage";
	public const string minRange		= "minRange";
	public const string fireOnMove		= "fireOnMove";
	public const string radius			= "radius";
	public const string radiusDamage	= "radiusDamage";
	public const string radiusLife		= "radiusLife";
	public const string numRounds		= "numRounds";
	public const string reloadTime		= "reloadTime";
	public const string faceInFlight	= "faceInFlight";
	public const string power			= "power";
	public const string range			= "range";
	public const string time			= "time";
	public const string armourHeat		= "armourHeat";
	public const string armourKinetic	= "armourKinetic";
	public const string powerOutput		= "powerOutput";
	public const string weaponSlots		= "weaponSlots";
	public const string speed			= "speed";
	public const string spinAngle		= "spinAngle";
	public const string skidDeceleration	= "skidDeceleration";
	public const string arrayVal			= "arrayVal";
	public const string structureModel		= "structureModel";
	public const string weapons				= "weapons";
	public const string propulsionExtraModels	= "propulsionExtraModels";

}

/// <summary>
/// Типы строений
/// </summary>
public enum StructTypes : byte
{
	 NONE 				= 0 // Не определено
	,HQ					= 1 // Коммандный центр
	,DEMOLISH			= 2
	,DEFENSE			= 3
	,FACTORY			= 4
	,WALL				= 5
	,POWER_GENERATOR	= 6
	,CORNER_WALL		= 7
	,COMMAND_RELAY		= 8
	,CYBORG_FACTORY		= 9
	,FACTORY_MODULE		= 10
	,GATE				= 11
	,GENERIC			= 12
	,POWER_MODULE		= 13
	,REPAIR_FACILITY	= 14
	,RESEARCH			= 15
	,RESEARCH_MODULE	= 16
	,RESOURCE_EXTRACTOR	= 17
	,SAT_UPLINK			= 18
	,VTOL_FACTORY		= 19
	,REARM_PAD			= 20
}

/* The different types of droid */
// NOTE, if you add to, or change this list then you'll need
// to update the DroidSelectionWeights lookup table in Display.c
public enum DROID_TYPE : byte
{
	DROID_WEAPON,           ///< Weapon droid
	DROID_SENSOR,           ///< Sensor droid
	DROID_ECM,              ///< ECM droid
	DROID_CONSTRUCT,        ///< Constructor droid
	DROID_PERSON,           ///< person
	DROID_CYBORG,           ///< cyborg-type thang
	DROID_TRANSPORTER,      ///< guess what this is!
	DROID_COMMAND,          ///< Command droid
	DROID_REPAIR,           ///< Repair droid
	DROID_DEFAULT,          ///< Default droid
	DROID_CYBORG_CONSTRUCT, ///< cyborg constructor droid - new for update 28/5/99
	DROID_CYBORG_REPAIR,    ///< cyborg repair droid - new for update 28/5/99
	DROID_CYBORG_SUPER,     ///< cyborg repair droid - new for update 7/6/99
	DROID_SUPERTRANSPORTER,	///< SuperTransport (MP)
	DROID_ANY,              ///< Any droid. Used as a parameter for various stuff.
};

public enum COMPONENT_TYPE : byte
{
	COMP_BODY,
	COMP_BRAIN,
	COMP_PROPULSION,
	COMP_REPAIRUNIT,
	COMP_ECM,
	COMP_SENSOR,
	COMP_CONSTRUCT,
	COMP_WEAPON,
	COMP_NUMCOMPONENTS,			/** The number of enumerators in this enum.	 */
};

/**
 * LOC used for holding locations for Sensors and ECM's
 */
public enum LOC : byte
{
	LOC_DEFAULT,
	LOC_TURRET,
};

/**
 * SIZE used for specifying body size
 */
public enum BODY_SIZE : byte
{
	SIZE_LIGHT,
	SIZE_MEDIUM,
	SIZE_HEAVY,
	SIZE_SUPER_HEAVY,
	SIZE_NUM
};

/**
 * SIZE used for specifying weapon size
 */
public enum WEAPON_SIZE : byte
{
	WEAPON_SIZE_LIGHT,
	WEAPON_SIZE_HEAVY,
	WEAPON_SIZE_ANY,
	WEAPON_SIZE_NUM
};

/**
 * Basic weapon type
 */
public enum WEAPON_CLASS : byte
{
	WC_KINETIC,					///< bullets etc
	WC_HEAT,					///< laser etc
	WC_NUM_WEAPON_CLASSES		/** The number of enumerators in this enum.	 */
};

/**
 * weapon subclasses used to define which weapons are affected by weapon upgrade
 * functions
 */
public enum WEAPON_SUBCLASS : byte
{
	WSC_MGUN,
	WSC_CANNON,
	WSC_MORTARS,
	WSC_MISSILE,
	WSC_ROCKET,
	WSC_ENERGY,
	WSC_GAUSS,
	WSC_FLAME,
	//WSC_CLOSECOMBAT,
	WSC_HOWITZERS,
	WSC_ELECTRONIC,
	WSC_AAGUN,
	WSC_SLOWMISSILE,
	WSC_SLOWROCKET,
	WSC_LAS_SAT,
	WSC_BOMB,
	WSC_COMMAND,
	WSC_EMP,
	WSC_NUM_WEAPON_SUBCLASSES,	/** The number of enumerators in this enum.	 */
};
	

/**
 * Defines the left and right sides for propulsion IMDs
 */
public enum PROP_SIDE : byte
{
	LEFT_PROP,
	RIGHT_PROP,
	NUM_PROP_SIDES,			/**  The number of enumerators in this enum. */
};

public enum PROPULSION_TYPE : byte
{
	PROPULSION_TYPE_WHEELED,
	PROPULSION_TYPE_TRACKED,
	PROPULSION_TYPE_LEGGED,
	PROPULSION_TYPE_HOVER,
	PROPULSION_TYPE_LIFT,
	PROPULSION_TYPE_PROPELLOR,
	PROPULSION_TYPE_HALF_TRACKED,
	PROPULSION_TYPE_NUM,	/**  The number of enumerators in this enum. */
};

public enum SENSOR_TYPE : byte
{
	STANDARD_SENSOR,
	INDIRECT_CB_SENSOR,
	VTOL_CB_SENSOR,
	VTOL_INTERCEPT_SENSOR,
	SUPER_SENSOR,			///< works as all of the above together! - new for updates
	RADAR_DETECTOR_SENSOR,
};

public enum TRAVEL_MEDIUM : byte
{
	GROUND,
	AIR,
};
