using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

public class SQLiteHelper {
    private SqliteConnection sqliteConnection;
    private string dbPath;

    public void InitializeDBConnection() {
    #if UNITY_IOS
        dbPath = "URI=file:" + Application.dataPath + "/Raw" + "/Metadata/Metadata.db";
        
    #elif UNITY_ANDROID
        //TODO(Huayu): load sql from jar on android
        dbPath = "jar:file://" + Application.dataPath + "!/assets/" + "/Metadata/Metadata.db";
    #else
        dbPath = "URI=file:" + Application.dataPath + "/StreamingAssets" + "/Metadata/Metadata.db";
    #endif
        Debug.Log("InitializeDBConnection: " + dbPath);
        sqliteConnection = new SqliteConnection(dbPath);
    }

    public SqliteCommand CreateTextCommand(string query) {
        SqliteCommand cmd = sqliteConnection.CreateCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = query;
        return cmd; 
    }

    public SqliteDataReader ExecuteCommand(SqliteCommand cmd) {
        sqliteConnection.Open();
        SqliteDataReader reader = cmd.ExecuteReader();
        cmd.Dispose();
        return reader;
    }

    public void CloseResultReader(SqliteDataReader reader) {
        reader.Close();
        sqliteConnection.Close();
    }
}
