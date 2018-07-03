using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
public enum EnemyTypeEnum {AVERAGE, DEFEND, ATTACK, SMALL_BOSS, BIG_BOSS}
public class EnemyMetadata {

	// Use this for initialization
	public int enemyId;
	public int waveNumber;
	public int attack;
	public int attackFrequency;
	public int hp;
	public EnemyTypeEnum enemyType;
	public float attackRange;
	public float moveSpeed;
	public float searchRange;
    public int[] lootIds;

	public EnemyMetadata(SqliteDataReader reader) {
		enemyId = reader.GetInt32(0);
		waveNumber = reader.GetInt32(1);
		attack = reader.GetInt32(2);
		attackFrequency = reader.GetInt32(3);
		hp = reader.GetInt32(4);
		enemyType = convertToEnemyTypeEnum(reader.GetString(5));
		attackRange = reader.GetFloat(6);
		moveSpeed = reader.GetFloat(7);
		searchRange = reader.GetFloat(8);

        string data = reader.GetString(9).Replace("\n", string.Empty);
        string[] splits = data.Split('|');
        lootIds = new int[splits.Length];
        for (int i = 0; i < splits.Length; i++) {
            lootIds[i] = int.Parse(splits[i]);
        }
	}

	private EnemyTypeEnum convertToEnemyTypeEnum(string enemyTypeString) {
		switch(enemyTypeString.Replace("\n", string.Empty)){
			case "average":
				return EnemyTypeEnum.AVERAGE;
			case "attack":
				return EnemyTypeEnum.ATTACK;
			case "defend":
				return EnemyTypeEnum.DEFEND;
			case "small_boss":
				return EnemyTypeEnum.SMALL_BOSS;
			case "big_boss":
				return EnemyTypeEnum.BIG_BOSS;
			default:
				return EnemyTypeEnum.AVERAGE;
		}
	}
}
