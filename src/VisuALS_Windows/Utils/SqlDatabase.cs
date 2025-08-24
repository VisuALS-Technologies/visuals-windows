using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace VisuALS_WPF_App
{
    public class SqlRecord : Dictionary<string, object>
    {
        public string ConditionString()
        {
            string conditionStr = "";

            foreach (KeyValuePair<string, object> c in this)
            {
                conditionStr += c.Key + " = " + SqlDatabase.SqliteFriendlyString(c.Value) + " and ";
            }

            conditionStr = conditionStr.Substring(0, conditionStr.Length - 5);
            return conditionStr;
        }
    }
    public class SqlColumnDefinitions : Dictionary<string, string> { }

    public class SqlDatabase
    {
        string databaseFile;
        public enum Sort { NONE, ASC, DESC };
        SQLiteConnection connection;

        /// <summary>
        /// "opens" a database
        /// </summary>
        /// <param name="path"></param>
        public void Open(string path)
        {
            databaseFile = path;
        }

        /// <summary>
        /// opens a connection to the database that persists until "EndPersistentConnection" is
        /// called. This bypasses the typical behavior which opens and closes a connection
        /// every time an SQLite command is executed
        /// </summary>
        public void StartPersistentConnection()
        {
            connection = new SQLiteConnection("Data Source=" + databaseFile + "; Version = 3;");
            connection.Open();
        }

        /// <summary>
        /// Closes the persistent connection created with the "StartPersistentConnection" method
        /// </summary>
        public void EndPersistentConnection()
        {
            connection.Close();
        }

        /// <summary>
        /// executes the sqlite command provided in the string cmd as a non-query
        /// </summary>
        /// <param name="cmd">command to execute</param>
        public void ExecNonQuery(string cmd)
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                (new SQLiteCommand(cmd, connection)).ExecuteNonQuery();
            }
            else
            {
                using (SQLiteConnection c = new SQLiteConnection("Data Source=" + databaseFile + "; Version = 3;"))
                {
                    c.Open();
                    (new SQLiteCommand(cmd, c)).ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// executes the sqlite command provided in the string cmd as a scalar execution, and returns the
        /// selected object
        /// </summary>
        /// <param name="cmd">command to execute</param>
        /// <returns></returns>
        public object ExecScalar(string cmd)
        {
            object obj;
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                obj = (new SQLiteCommand(cmd, connection)).ExecuteScalar();
            }
            else
            {
                using (SQLiteConnection c = new SQLiteConnection("Data Source=" + databaseFile + "; Version = 3;"))
                {
                    c.Open();
                    obj = (new SQLiteCommand(cmd, c)).ExecuteScalar();
                }
            }
            return obj;
        }

        /// <summary>
        /// execute an sql command and get all results
        /// </summary>
        /// <param name="cmd">command to execute</param>
        /// <returns></returns>
        public List<SqlRecord> ExecAllResults(string cmd)
        {
            List<SqlRecord> result = new List<SqlRecord>();
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                using (SQLiteDataReader reader = (new SQLiteCommand(cmd, connection)).ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SqlRecord record = new SqlRecord();
                        int i = 0;
                        bool done = false;
                        string name;
                        while (!done)
                        {
                            try
                            {
                                name = reader.GetName(i);
                                record.Add(name, reader[i]);
                                i++;
                            }
                            catch
                            {
                                done = true;
                            }
                        }
                        result.Add(record);
                    }
                }
            }
            else
            {
                using (SQLiteConnection c = new SQLiteConnection("Data Source=" + databaseFile + "; Version = 3;"))
                {
                    c.Open();
                    using (SQLiteDataReader reader = (new SQLiteCommand(cmd, c)).ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SqlRecord record = new SqlRecord();
                            int i = 0;
                            bool done = false;
                            string name;
                            while (!done)
                            {
                                try
                                {
                                    name = reader.GetName(i);
                                    record.Add(name, reader[i]);
                                    i++;
                                }
                                catch
                                {
                                    done = true;
                                }
                            }
                            result.Add(record);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// gets and entire record that meets the given conditions
        /// </summary>
        /// <param name="table">table to get the record from</param>
        /// <param name="conditions">conditions that must be met to get the record</param>
        /// <returns></returns>
        public SqlRecord GetRecord(string table, string conditions)
        {
            SqlRecord result = new SqlRecord();
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                using (SQLiteDataReader reader = (new SQLiteCommand("select * from " + table + " where (" + conditions + ")", connection)).ExecuteReader())
                {
                    reader.Read();
                    int i = 0;
                    bool done = false;
                    string name;
                    while (!done)
                    {
                        try
                        {
                            name = reader.GetName(i);
                            result.Add(name, reader[i]);
                            i++;
                        }
                        catch
                        {
                            done = true;
                        }
                    }
                }
            }
            else
            {
                using (SQLiteConnection c = new SQLiteConnection("Data Source=" + databaseFile + "; Version = 3;"))
                {
                    c.Open();
                    using (SQLiteDataReader reader = (new SQLiteCommand("select * from " + table + " where (" + conditions + ")", c)).ExecuteReader())
                    {
                        reader.Read();
                        int i = 0;
                        bool done = false;
                        string name;
                        while (!done)
                        {
                            try
                            {
                                name = reader.GetName(i);
                                result.Add(name, reader[i]);
                                i++;
                            }
                            catch
                            {
                                done = true;
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// adds a record to a table
        /// </summary>
        /// <param name="table">table to add it to</param>
        /// <param name="record">record contents to add</param>
        public void AddRecord(string table, SqlRecord record)
        {
            string columns = "", values = "";

            foreach (KeyValuePair<string, object> column in record)
            {
                columns += column.Key + ", ";
                values += SqliteFriendlyString(column.Value) + ", ";
            }

            columns = columns.TrimEnd(' ', ',');
            values = values.TrimEnd(' ', ',');

            ExecNonQuery("insert into " + table + " ( " + columns + " ) values ( " + values + " )");
        }

        /// <summary>
        /// removes any records that meet the given conditions
        /// </summary>
        /// <param name="table">table to remove records from</param>
        /// <param name="conditions">conditions that must be met to remove a given record</param>
        public void RemoveRecord(string table, string conditions)
        {
            ExecNonQuery("delete from " + table + " where ( " + conditions + " )");
        }

        /// <summary>
        /// udates multiple values in multiple records
        /// </summary>
        /// <param name="table">table the record(s) are in</param>
        /// <param name="changes">SqlRecord object containing the new values for any columns to change</param>
        /// <param name="conditions">conditions to meet to change a records values</param>
        public void UpdateRecord(string table, SqlRecord changes, string conditions)
        {
            string changeStr = "";

            foreach (KeyValuePair<string, object> column in changes)
            {
                changeStr += column.Key + " = " + SqliteFriendlyString(column.Value) + ", ";
            }

            changeStr = changeStr.TrimEnd(' ', ',');

            ExecNonQuery("update " + table + " set " + changeStr + " where ( " + conditions + " );");
        }

        /// <summary>
        /// update a single value in a record
        /// </summary>
        /// <param name="table">table the record is in</param>
        /// <param name="column">column to change</param>
        /// <param name="newVal">new value for that column</param>
        /// <param name="recToChange">the SqlRecord version of the record you want to change</param>
        public void UpdateRecord(string table, string column, object newVal, SqlRecord recToChange)
        {
            string conditionStr = "";

            foreach (KeyValuePair<string, object> c in recToChange)
            {
                conditionStr += c.Key + " = " + SqliteFriendlyString(c.Value) + " and ";
            }

            conditionStr = conditionStr.Substring(0, conditionStr.Length - 5);

            ExecNonQuery("update " + table + " set " + column + " = " + newVal.ToString() + " where ( " + conditionStr + " );");
        }

        /// <summary>
        /// add a table to the database, does nothing if the table already exists
        /// </summary>
        /// <param name="tableName">name of table</param>
        /// <param name="definitions">column definitions for table</param>
        public void AddTable(string tableName, SqlColumnDefinitions definitions)
        {
            string columns = "";

            foreach (KeyValuePair<string, string> column in definitions)
            {
                columns += column.Key + " " + column.Value + ", ";
            }

            columns = columns.TrimEnd(' ', ',');

            ExecNonQuery("create table if not exists " + tableName + " ( " + columns + " );");
        }

        /// <summary>
        /// gets a single value from a record
        /// </summary>
        /// <param name="table">table the record is in</param>
        /// <param name="columnToSelect">column the value is in</param>
        /// <param name="conditions">conditions that the record must meet</param>
        /// <returns></returns>
        public object GetValue(string table, string columnToSelect, string conditions)
        {
            return ExecScalar("select " + columnToSelect + " from " + table + (conditions == "" ? "" : " where (" + conditions + ")") + ";");
        }

        /// <summary>
        /// gets the number of records in a table that meet the given conditions
        /// </summary>
        /// <param name="table">table to check</param>
        /// <param name="conditions">conditions that must be met to count it</param>
        /// <returns></returns>
        public int Count(string table, string conditions)
        {
            return Convert.ToInt32((long)ExecScalar("select count(*) from " + table + (conditions == "" ? "" : " where (" + conditions + ")") + ";"));
        }

        /// <summary>
        /// gets the maximum value that meets the conditions in a given column
        /// </summary>
        /// <param name="table">table to get it from</param>
        /// <param name="column">column to get it from</param>
        /// <param name="conditions">conditions that must be met for it to be considered</param>
        /// <returns></returns>
        public object Max(string table, string column, string conditions)
        {
            return ExecScalar("select max(" + column + ") from " + table + (conditions == "" ? "" : " where (" + conditions + ")") + ";");
        }

        /// <summary>
        /// gets the minimum value that meets the conditions in a given column
        /// </summary>
        /// <param name="table">table to get it from</param>
        /// <param name="column">column to get it from</param>
        /// <param name="conditions">conditions that must be met for it to be considered</param>
        /// <returns></returns>
        public object Min(string table, string column, string conditions)
        {
            return ExecScalar("select min(" + column + ") from " + table + (conditions == "" ? "" : " where (" + conditions + ")") + ";");
        }

        /// <summary>
        /// checks if a value exists in a given table and column with given conditions
        /// </summary>
        /// <param name="table">table to check</param>
        /// <param name="column">column to check</param>
        /// <param name="conditions">conditions that must be met</param>
        /// <returns></returns>
        public bool Exists(string table, string column, string conditions)
        {
            return ExecScalar("select " + column + " from " + table + " where exists(select " + column + " from " + table + (conditions == "" ? "" : " where (" + conditions + ")") + ");") != null;
        }

        /// <summary>
        /// retrieve all values in a column that meet the conditions
        /// </summary>
        /// <typeparam name="T">column type</typeparam>
        /// <param name="table">table to get the values from</param>
        /// <param name="column">column to get the values from</param>
        /// <param name="conditions">conditions that must be met to take the value</param>
        /// <param name="sort">sort type (NONE, ASC, DESC)</param>
        /// <param name="sortByColumn">column to use for sorting the results</param>
        /// <returns></returns>
        public List<T> GetMultipleVals<T>(string table, string column, string conditions, bool uniqueValsOnly = false, Sort sort = Sort.NONE, string sortByColumn = "", int limitResults = -1)
        {
            List<T> result = new List<T>();
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                using (SQLiteDataReader reader = (sort == Sort.NONE ? (new SQLiteCommand("select " + (uniqueValsOnly ? "distinct " : "") + column + " from " + table + " where " + conditions + ";", connection)).ExecuteReader() : (new SQLiteCommand("select " + (uniqueValsOnly ? "distinct " : "") + column + " from " + table + " where (" + conditions + ") order by " + sortByColumn + " " + sort.ToString().ToLower() + (limitResults < 0 ? "" : (" limit " + limitResults.ToString())) + ";", connection)).ExecuteReader()))
                {
                    while (reader.Read())
                    {
                        SqlRecord record = new SqlRecord();
                        int i = 0;
                        bool done = false;
                        string name;
                        while (!done)
                        {
                            try
                            {
                                name = reader.GetName(i);
                                record.Add(name, reader[i]);
                                i++;
                            }
                            catch
                            {
                                done = true;
                            }
                        }
                        result.Add((T)record[column]);
                    }
                }
            }
            else
            {
                using (SQLiteConnection c = new SQLiteConnection("Data Source=" + databaseFile + "; Version = 3;"))
                {
                    c.Open();
                    using (SQLiteDataReader reader = (sort == Sort.NONE ? (new SQLiteCommand("select " + (uniqueValsOnly ? "distinct " : "") + column + " from " + table + " where " + conditions + ";", c)).ExecuteReader() : (new SQLiteCommand("select " + (uniqueValsOnly ? "distinct " : "") + column + " from " + table + " where (" + conditions + ") order by " + sortByColumn + " " + sort.ToString().ToLower() + (limitResults < 0 ? "" : (" limit " + limitResults.ToString())) + ";", c)).ExecuteReader()))
                    {
                        while (reader.Read())
                        {
                            SqlRecord record = new SqlRecord();
                            int i = 0;
                            bool done = false;
                            string name;
                            while (!done)
                            {
                                try
                                {
                                    name = reader.GetName(i);
                                    record.Add(name, reader[i]);
                                    i++;
                                }
                                catch
                                {
                                    done = true;
                                }
                            }
                            result.Add((T)record[column]);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// adds apostrophes on either side of the string and escapes all apostrophes
        /// already in the string (by doubling them) so that the string is ready for 
        /// use in an sql command
        /// </summary>
        /// <param name="obj">string, non string objects get turned into a string</param>
        /// <returns></returns>
        static public string SqliteFriendlyString(object obj)
        {
            string str = "";

            if (obj.GetType() == str.GetType())
            {
                str = "'" + ((string)obj).Replace("'", "''") + "'";
            }
            else
            {
                str = obj.ToString();
            }
            return str;
        }
        /// <summary>
        /// finds the lowest non-negative value that is not present in an integer valued column
        /// </summary>
        /// <param name="table">table the column is in</param>
        /// <param name="col">column to search</param>
        /// <param name="conditions">conditions that must be met for a record to affect the results</param>
        /// <returns></returns>
        public int GetLowestAvailableID(string table, string col, string conditions)
        {
            List<SqlRecord> results = ExecAllResults("select " + col + " from " + table + (conditions == "" ? "" : " where (" + conditions + ")") + "  order by " + col + " asc;");
            if (results.Count > 0)
            {
                for (int i = 0; i < results.Count - 1; i++)
                {
                    if ((Convert.ToInt32(results[i][col])) < (Convert.ToInt32(results[i + 1][col]) - 1))
                    {
                        return Convert.ToInt16(results[i][col]) + 1;
                    }
                }
            }
            else if (results.Count == 0)
            {
                return 0;
            }
            return Convert.ToInt16(results[results.Count - 1][col]) + 1;
        }
        /// <summary>
        /// shifts all the values in an integer valued column a specified amount
        /// </summary>
        /// <param name="table">table to make the changes in</param>
        /// <param name="col">column to shift the values of</param>
        /// <param name="conditions">conditions that must be met to shift its value</param>
        /// <param name="displacement">integer amount of displacement (can be negative)</param>
        public void MoveAllIDs(string table, string col, string conditions, int displacement)
        {
            List<SqlRecord> results = ExecAllResults("select * from " + table + " where (" + conditions + ") order by " + col + " asc;");
            List<int> ids = new List<int>();
            foreach (SqlRecord s in results)
            {
                ids.Add(0);
            }

            if (displacement > 0)
            {
                for (int i = results.Count - 1; i >= 0; i--)
                {
                    ids[i] = (int)results[i][col] + displacement;
                }
            }
            else if (displacement < 0)
            {
                for (int i = 0; i < results.Count - 1; i++)
                {
                    ids[i] = (int)results[i][col] + displacement;
                }
            }

            for (int i = 0; i <= results.Count - 1; i++)
            {
                UpdateRecord(table, col, ids[i], results[i]);
            }
        }
    }
}
