using Microsoft.Data.Sqlite;
using O2DESNet;
using System.Data;

class O2desRTDB : Sandbox
{
    O2desBuiltInDatabase? _memInstance = null;

    public O2desRTDB(int seed = 0) : base(seed)
    {
        CreateRTDB();
    }

    public void CreateRTDB()
    {
        _memInstance = new O2desBuiltInDatabase("Data Source=:memory:");
        _memInstance.Open();
    }

    public void CloseRTDB() 
    {
        if (_memInstance != null) 
        {
            if (_memInstance.State != ConnectionState.Closed)
            {
                _memInstance.Close();
            }
            _memInstance = null;
        }
    }

    // CREATE TABLE IF NOT EXISTS MyTable (Column1 TEXT, Column2 TEXT)
    // INSERT INTO MyTable (Column1, Column2) VALUES ('Value1', 'Value2')
    public int ExecuteNonQuery(string sql)
    {
        if (_memInstance == null) 
        {
            Console.WriteLine("_memInstance is null.");
            return -1;
        }
        else if (_memInstance.State != ConnectionState.Open)
        {
            Console.WriteLine("_memInstance not open.");
            return -2;
        }
        else
        {
            var command = _memInstance.CreateCommand();
            command.CommandText = sql;
            int ret = command.ExecuteNonQuery();
            return ret;
        }
    }

    // select * from MyTable
    public int ExecuteQuery(string sql)
    {
        if (_memInstance == null)
        {
            Console.WriteLine("_memInstance is null.");
            return -1;
        }
        else if (_memInstance.State != ConnectionState.Open)
        {
            Console.WriteLine("_memInstance not open.");
            return -2;
        }
        else
        {
            _memInstance.ReadDataCommon("select * from MyTable;");
            Console.WriteLine("Data load successfully.");
            return 0;
        }
    }

    public void SaveSnapshot(string fileName)
    {
        var fileConnection = new O2desBuiltInDatabase($"Data Source={fileName}");
        _memInstance.SaveAs(fileConnection, true);
        Console.WriteLine("Data persisted successfully.");
    }

    public void LoadSnapshot(string fileName)
    {
        var diskConnection = new O2desBuiltInDatabase($"Data Source={fileName}");
        diskConnection.Open();
        CreateRTDB();
        diskConnection.SaveAs(_memInstance);
        diskConnection.Close();
    }
}
