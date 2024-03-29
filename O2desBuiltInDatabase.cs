﻿using System.Data;
using static SQLitePCL.raw;

namespace Microsoft.Data.Sqlite
{
    public class O2desBuiltInDatabase : SqliteConnection
    {
        public O2desBuiltInDatabase(string? connectionString) : base(connectionString)
        {
        }

        public void SaveAs(O2desBuiltInDatabase destination, bool destinationAutoClose = false)
        {
            string destinationName = "main";
            string sourceName = "main";
            if (State != ConnectionState.Open)
            {
                Open();
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            var close = false;
            if (destination.State != ConnectionState.Open)
            {
                destination.Open();
                close = true;
            }

            try
            {
                using var backup = sqlite3_backup_init(destination.Handle, destinationName, Handle, sourceName);
                int rc;
                if (backup.IsInvalid)
                {
                    rc = sqlite3_errcode(destination.Handle);
                    SqliteException.ThrowExceptionForRC(rc, destination.Handle);
                }

                rc = sqlite3_backup_step(backup, -1);
                SqliteException.ThrowExceptionForRC(rc, destination.Handle);
            }
            finally
            {
                if (close && destinationAutoClose)
                {
                    destination.Close();
                }
            }
        }

        public void ReadDataCommon(string sql)
        {
            var command = CreateCommand();
            command.CommandText = sql;
            Console.WriteLine($">> {command.CommandText}");

            var reader = command.ExecuteReader();
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