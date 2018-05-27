using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

public class SQLiteHelper : Singleton<SQLiteHelper> {
    private SqliteConnection sqliteConnection;
    private string dbPath;

    public void InitializeDBConnection() {
        dbPath = "URI=file:" + Application.dataPath + "/Metadata/Metadata.db";
        sqliteConnection = new SqliteConnection(dbPath);
        sqliteConnection.Open();
        SqliteCommand cmd = sqliteConnection.CreateCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = "SELECT * FROM Enemy";
        SqliteDataReader reader = cmd.ExecuteReader();
        while (reader.Read()) {
            Debug.Log(reader.GetInt32(0));
            Debug.Log(reader.GetInt32(1));
            Debug.Log(reader.GetInt32(2));
            Debug.Log(reader.GetInt32(3));
            Debug.Log(reader.GetInt32(4));
            Debug.Log(reader.GetString(5));
        }
        sqliteConnection.Close();
    }


}
