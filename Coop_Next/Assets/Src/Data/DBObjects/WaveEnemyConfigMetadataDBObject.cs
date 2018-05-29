using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
public class WaveEnemyConfigMetadataDBObject {
	public int waveNumber;
	public int averageEnemyQuantity;
	public int defendEnemyQuantity;
	public int attackEnemyQuantity;
	public int smallBossQuantity;
	public int bigBossQuantity;
	public WaveEnemyConfigMetadataDBObject(SqliteDataReader reader) {
		waveNumber = reader.GetInt32(0);
		averageEnemyQuantity = reader.GetInt32(1);
		defendEnemyQuantity = reader.GetInt32(2);
		attackEnemyQuantity = reader.GetInt32(3);
		smallBossQuantity = reader.GetInt32(4);
		bigBossQuantity = reader.GetInt32(5);
	}
}
