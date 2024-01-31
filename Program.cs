using Microsoft.Data.Sqlite;

class Program
{
    static void Main()
    {
        //SaveDatabase();
        LoadDatabase();
    }
    static void SaveDatabase()
    {
        string inMemoryConnectionString = "Data Source=:memory:";
        string fileConnectionString = "Data Source=mydatabase.db";

        using (var inMemoryConnection = new MyPersistance(inMemoryConnectionString))
        {
            inMemoryConnection.Open();

            using (var command = inMemoryConnection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE MyTable (Column1 TEXT, Column2 TEXT)";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO MyTable (Column1, Column2) VALUES ('Value1', 'Value2')";
                command.ExecuteNonQuery();
            }

            var fileConnection = new MyPersistance(fileConnectionString);
            inMemoryConnection.BackupDatabase(fileConnection);
        }
        Console.WriteLine("Data persisted successfully.");
    }

    static void LoadDatabase()
    {
        string fileConnectionString = "Data Source=myDatabaseOnDisk.db";
        string inMemoryConnectionString = "Data Source=:memory:";

        var diskConnection = new MyPersistance(fileConnectionString);
        diskConnection.Open();
        using (var command = diskConnection.CreateCommand())
        {
            command.CommandText = "CREATE TABLE IF NOT EXISTS MyTable (Column1 TEXT, Column2 TEXT)";
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO MyTable (Column1, Column2) VALUES ('Value11', 'Value22')";
            command.ExecuteNonQuery();
        }

        var memConnection = new MyPersistance(inMemoryConnectionString);
        diskConnection.SaveAs(memConnection);
        diskConnection.Close();
        Console.WriteLine("Data persisted successfully.");

        memConnection.Open();
        ReadDataCommon("select * from MyTable;", memConnection);
        memConnection.Close();
        Console.WriteLine("Data load successfully.");
    }

    // 从表中读取数据(通用)
    static void ReadDataCommon(string sql, MyPersistance connection)
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
