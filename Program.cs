using Microsoft.Data.Sqlite;

class Program
{
    static void Main()
    {
        SaveSnapshot();
        LoadSnapshot();
    }
    static void SaveSnapshot()
    {
        string memConnectionString = "Data Source=:memory:";
        string discConnectionString = "Data Source=SimSnapshot.db";

        using (var memInstance = new O2desPersistance(memConnectionString))
        {
            memInstance.Open();

            using (var command = memInstance.CreateCommand())
            {
                command.CommandText = "CREATE TABLE MyTable (Column1 TEXT, Column2 TEXT)";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO MyTable (Column1, Column2) VALUES ('Value1', 'Value2')";
                command.ExecuteNonQuery();
            }

            var fileConnection = new O2desPersistance(discConnectionString);
            memInstance.SaveAs(fileConnection, true);
        }
        Console.WriteLine("Data persisted successfully.");
    }

    static void LoadSnapshot()
    {
        string fileConnectionString = "Data Source=myDatabaseOnDisk.db";
        string inMemoryConnectionString = "Data Source=:memory:";

        var diskConnection = new O2desPersistance(fileConnectionString);
        diskConnection.Open();
        using (var command = diskConnection.CreateCommand())
        {
            command.CommandText = "CREATE TABLE IF NOT EXISTS MyTable (Column1 TEXT, Column2 TEXT)";
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO MyTable (Column1, Column2) VALUES ('Value11', 'Value22')";
            command.ExecuteNonQuery();
        }

        var memConnection = new O2desPersistance(inMemoryConnectionString);
        diskConnection.SaveAs(memConnection);
        diskConnection.Close();

        memConnection.Open();
        ReadDataCommon("select * from MyTable;", memConnection);
        memConnection.Close();
        Console.WriteLine("Data load successfully.");
    }

    // 从表中读取数据(通用)
    static void ReadDataCommon(string sql, O2desPersistance connection)
    {
        using (var command = connection.CreateCommand())
        {
            command.CommandText = sql;
            Console.WriteLine($">> {command.CommandText}");

            using (var reader = command.ExecuteReader())
            {
                int numOfRows = 0;
                while (reader.Read())
                {
                    numOfRows++;
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write($"{reader.GetString(i)},\t");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine($"{numOfRows} rows selected.");
            }
        }
    }
}
