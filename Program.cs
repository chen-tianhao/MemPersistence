
class Program
{
    static void Main()
    {
        O2desRTDB rtdb = new O2desRTDB();
        rtdb.CreateRTDB();
        int ret = rtdb.ExecuteNonQuery("CREATE TABLE IF NOT EXISTS MyTable (Column1 TEXT, Column2 TEXT);");
        if (ret < 0) return;
        ret = rtdb.ExecuteNonQuery("INSERT INTO MyTable (Column1, Column2) VALUES ('Value1', 'Value2');");
        if (ret < 0) return;
        rtdb.SaveSnapshot("aaa.db");
        rtdb.CloseRTDB();

        O2desRTDB rtdb2 = new O2desRTDB();
        rtdb2.LoadSnapshot("aaa.db");
        rtdb2.ExecuteQuery("select * from MyTable;");
    }
}
