using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;

public class BuildingMetadataDBObject {
	public int buildingId;
	public string buildingName;
	public int level;
	public int hp;
	public int attack;
	public int attackFrequency;

	public BuildingMetadataDBObject(SqliteDataReader reader) {
		buildingId = reader.GetInt32(0);
		buildingName = reader.GetString(1).Replace("\n", string.Empty);
		level = reader.GetInt32(2);
		hp = reader.GetInt32(3);
		attack = reader.GetInt32(4);
		attackFrequency = reader.GetInt32(5);
	}
}
