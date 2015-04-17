using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Data.Game;
using LitJson;
using SimpleSQL;
using Zedarus.ToolKit.Math;

namespace Zedarus.ToolKit.Data.Game.Models
{
	public class Enemy : Model
	{
		#region Properties
		private string _name;
		private string _image;
		private string _gameUUID;
		private int _health;
		private int _initiative;
		private int _attackDelay;
		private int _damage;
		private float _lifeForce;
		private int _lifeForceKillBonus;
		private string _extraSlotsSource;
		private int _scriptID;
		private int _typeID;
		private string _scriptParams;
		private int _abilityOrder;
		private int _abilityChance;
		private int _abilityLevelUp;
		private int _locationID;
		private Point[] _slots;
		#endregion
		
		#region Init
		public Enemy(int id, bool enabled, bool deleted, string name, string game_uuid, string image, int health, int initiative, int attackDelay, int damage, float lifeForce, int lifeForceKillBonus, string extraSlots, int typeID, int scriptID, string scriptParams, int abilityOrder, int abilityChange, int abilityLevelUp, int locationID) : base(id, enabled, deleted) 
		{
			_name = name;
			_gameUUID = game_uuid;
			_image = image;
			_health = health;
			_initiative = initiative;
			_attackDelay = attackDelay;
			_damage = damage;
			_lifeForce = lifeForce;
			_lifeForceKillBonus = lifeForceKillBonus;
			_extraSlotsSource = extraSlots;
			_scriptID = scriptID;
			_typeID = typeID;
			_scriptParams = scriptParams;
			_abilityOrder = abilityOrder;
			_abilityChance = abilityChange;
			_abilityLevelUp = abilityLevelUp;
			_locationID = locationID;

			ProcessExtraSlots(_extraSlotsSource);
		}
		
		public Enemy(JsonData json) : base(json) 
		{
			_name = GetString(json, "name");
			_gameUUID = GetString(json, "game_uuid");
			_image = GetString(json, "image");
			_health = GetInt(json, "health");
			_initiative = GetInt(json, "initiative");
			_attackDelay = GetInt(json, "attack_delay");
			_damage = GetInt(json, "damage");
			_lifeForce = GetFloat(json, "life_force");
			_lifeForceKillBonus = GetInt(json, "life_force_kill_bonus");
			_extraSlotsSource = GetString(json, "extra_slots");
			_scriptID = GetInt(json, "lua_script_id");
			_typeID = GetInt(json, "enemy_type_id");
			_scriptParams = GetString(json, "lua_script_parameters");
			_abilityOrder = GetInt(json, "ability_order");
			_abilityChance = GetInt(json, "ability_chance");
			_abilityLevelUp = GetInt(json, "ability_level_up");
			_locationID = GetInt(json, "location_id");

			ProcessExtraSlots(_extraSlotsSource);
		}
		
		public Enemy(List<SimpleDataColumn> columns, SimpleDataRow row) : base(columns, row)
		{
			_name = GetString(columns, row, "name");
			_gameUUID = GetString(columns, row, "game_uuid");
			_image = GetString(columns, row, "image");
			_health = GetInt(columns, row, "health");
			_initiative = GetInt(columns, row, "initiative");
			_attackDelay = GetInt(columns, row, "attack_delay");
			_damage = GetInt(columns, row, "damage");
			_lifeForce = GetFloat(columns, row, "life_force");
			_lifeForceKillBonus = GetInt(columns, row, "life_force_kill_bonus");
			_extraSlotsSource = GetString(columns, row, "extra_slots");
			_scriptID = GetInt(columns, row, "lua_script_id");
			_typeID = GetInt(columns, row, "enemy_type_id");
			_scriptParams = GetString(columns, row, "lua_script_parameters");
			_abilityOrder = GetInt(columns, row, "ability_order");
			_abilityChance = GetInt(columns, row, "ability_chance");
			_abilityLevelUp = GetInt(columns, row, "ability_level_up");
			_locationID = GetInt(columns, row, "location_id");

			ProcessExtraSlots(_extraSlotsSource);
		}

		private void ProcessExtraSlots(string input)
		{
			if (input.Length > 1)
			{
				string[] slots = input.Split(' ');

				_slots = new Point[slots.Length + 1];
				_slots[0] = new Point(0, 0);

				for (int i = 0; i < slots.Length; i++)
				{
					string[] point = slots[i].Split('|');
					if (point.Length == 2)
						_slots[i + 1] = new Point(int.Parse(point[0]), -int.Parse(point[1]));
				}
			}
			else
			{
				_slots = new Point[1];
				_slots[0] = new Point(0, 0);
			}
		}
		#endregion
		
		#region Queries
		public string Name               { get { return _name; } }
		public string UUID               { get { return _gameUUID; } }
		public string Image              { get { return _image; } }
		public int Health                { get { return _health; } }
		public int Initiative            { get { return _initiative; } }
		public int AttackDelay           { get { return _attackDelay; } }
		public int Damage                { get { return _damage; } }
		public float LifeForce           { get { return _lifeForce; } }
		public int LifeForceKillBonus    { get { return _lifeForceKillBonus; } }
		public string ExtraSlotsSource   { get { return _extraSlotsSource; } }
		public int ScriptID              { get { return _scriptID; } }
		public int TypeID                { get { return _typeID; } }
		public string[] ScriptParameters { get { return _scriptParams.Split(','); } }
		public int AbilityOrder          { get { return _abilityOrder; } }
		public int AbilityChance         { get { return _abilityChance; } }
		public int AbilityLevelUp        { get { return _abilityLevelUp; } }
		public int LocationID            { get { return _locationID; } }
		public Point[] Slots             { get { return _slots; } }

		//public Script Script { get { return DataManager.Instance.Game.Scripts.Get(_scriptID); } }
		//public EnemyType Type { get { return DataManager.Instance.Game.EnemyTypes.Get(_typeID); } }
		//public Location Location { get { return DataManager.Instance.Game.Locations.Get(_locationID); } }
		#endregion

		#region Settings
		public enum Fields
		{
			Name,
			Image,
			Health,
			Initiative,
			AttackDelay,
			Damage,
			LifeForce,
			LifeForceKillBonus,
			//ExtraSlots,
			ScriptID,
			TypeID,
			AbilityOrder,
			AbilityChance,
			AbilityLevelUp,
			LocationID,
			UUID,
		}
		
		public override string[] GetIndexes()
		{
			return AddArrayToArray(base.GetIndexes(), System.Enum.GetNames(typeof(Fields)));
		}
		
		protected override string[] GetDBFields()
		{
			return AddToArray(base.GetDBFields(), "name", "game_uuid", "image", "health", "initiative", "attack_delay", "damage", "life_force", "life_force_kill_bonus", "extra_slots", "lua_script_id", "enemy_type_id",
			                  "lua_script_parameters", "ability_order", "ability_chance", "ability_level_up", "location_id");
		}
		
		protected override string[] GetDBValues()
		{
			return AddToArray(base.GetDBValues(), _name, _gameUUID, _image, _health.ToString(), _initiative.ToString(), _attackDelay.ToString(), _damage.ToString(), _lifeForce.ToString(), _lifeForceKillBonus.ToString(), 
			                  _extraSlotsSource, _scriptID.ToString(), _typeID.ToString(), _scriptParams, _abilityOrder.ToString(), _abilityChance.ToString(), _abilityLevelUp.ToString(), _locationID.ToString());
		}
		
		public static new string GetDBTable()
		{
			return "enemies";
		}
		#endregion
	}
}
