using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;

public class BuildingMetadataDBObject {
	public int buildingId;
    public string techTreeId;
	public string buildingName;
	public int level;
	public int hp;
	public int attack;
	public int attackFrequency;
	public int attackRange;
	public int underAttackPriority;

	public BuildingMetadataDBObject(SqliteDataReader reader) {
		buildingId = reader.GetInt32(0);
        techTreeId = reader.GetString(1).Replace("\n", string.Empty);
		buildingName = reader.GetString(2).Replace("\n", string.Empty);
		level = reader.GetInt32(3);
		hp = reader.GetInt32(4);
		attack = reader.GetInt32(5);
		attackFrequency = reader.GetInt32(6);
		attackRange = reader.GetInt32(7);
		underAttackPriority = reader.GetInt32(8);
	}
}
