﻿using System.Data;
using static SQLitePCL.raw;

namespace Microsoft.Data.Sqlite
{
    public class MyPersistance : SqliteConnection
    {
        public MyPersistance(string? connectionString) : base(connectionString)
        {
        }

        public void SaveAs(SqliteConnection destination, bool autoClose = false)
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
                if (close && autoClose)
                {
                    destination.Close();
                }
            }
        }
    }
}