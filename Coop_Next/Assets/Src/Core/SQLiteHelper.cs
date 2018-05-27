using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

public class SQLiteHelper {
    private SqliteConnection sqliteConnection;
    private string dbPath;

    public void InitializeDBConnection() {
        dbPath = "URI=file:" + Application.dataPath + "/Metadata/Metadata.db";
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
