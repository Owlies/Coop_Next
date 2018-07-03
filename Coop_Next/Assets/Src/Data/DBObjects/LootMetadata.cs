using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;

public class LootRate
{
    public int itemId;
    public float rate;
}

public class LootMetadata {

    public int lootId;
    public string behaviour;
    public List<LootRate> lootRates;

    public LootMetadata(SqliteDataReader reader)
    {
        lootRates = new List<LootRate>();

        lootId = reader.GetInt32(0);
        behaviour = reader.GetString(1);
        string data = reader.GetString(2).Replace("\n", string.Empty);
        string[] splits = data.Split('|');
        for(int i =0; i < splits.Length; i++)
        {
            string[] splitsData = splits[i].Split(':');
            LootRate rate = new LootRate();
            rate.itemId = int.Parse(splitsData[0]);
            rate.rate = float.Parse(splitsData[1]);
            lootRates.Add(rate);
        }
    }
}
