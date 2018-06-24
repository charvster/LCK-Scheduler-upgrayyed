using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;

namespace LCK_DBcommunication
{
    public class SQLiteComm
    {
        private SQLiteConnection conn = new SQLiteConnection();
        public string ConnectionString { get; set; }
        //private string dbFilename = @"E:\Consulting\LCCK\Scheduler upgrade\development\database\SQLite\lck.s3db";
        private string dbFilename = AppDomain.CurrentDomain.BaseDirectory + @"\SQLite\lck.s3db";

  	    /// <summary>
	    ///     Default Constructor for SQLiteDatabase Class.
	    /// </summary>
        public SQLiteComm()
        {
            this.ConnectionString = "DataSource=" + dbFilename;
        }

        /// <summary>
	    ///     Single Param Constructor for specifying the DB file.
	    /// </summary>
	    /// <param name="inputFile">The File containing the DB</param>
        public SQLiteComm(string inputFile)
        {
            dbFilename = inputFile;
            this.ConnectionString = string.Format("DataSource={0}",inputFile);
        }

        public bool Open()
        {
            bool rtn = false;

            if (conn.State == System.Data.ConnectionState.Open)
                return true;

            try
            {
                conn.ConnectionString = ConnectionString;
                conn.Open();
                rtn = true;
            }
            catch (Exception ex)
            {
                rtn = false;
            }

            return rtn;
        }

        public bool Close()
        {
            bool rtn = false;
            try
            {
                conn.Close();
                rtn = true;
            }
            catch (Exception ex)
            {
                rtn = false;
            }

            return rtn;
        }

        public SQLiteDataReader ExecuteCommand(string sql)
        {
            try
            {
                if (!Open())
                    return null;
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader rdr = cmd.ExecuteReader();
                return rdr;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

         /// <summary>
        ///     Allows the programmer to interact with the database for purposes other than a query.
	    /// </summary>
	    /// <param name="sql">The SQL to be run.</param>
	    /// <returns>An Integer containing the number of rows updated.</returns>
        public int ExecuteNonQuery(string sql)
        {
            try
            {
                if (!Open())
                    return 0;
                SQLiteCommand mycommand = new SQLiteCommand(conn);
                mycommand.CommandText = sql;
                int rowsUpdated = mycommand.ExecuteNonQuery();
                return rowsUpdated;
            }
            catch (Exception crap)
            {
                return 0;
            }
        }

        //public int ExecuteNonQuery(string sql)
        //{
        //    try
        //    {
        //        if (!Open())
        //            return 0;

        //        var transaction = conn.BeginTransaction();
        //        var mycommand = new SQLiteCommand(conn) { CommandText = sql };
        //        mycommand.Transaction = transaction;
        //        int rowsUpdated = mycommand.ExecuteNonQuery();
        //        transaction.Commit();
        //        return rowsUpdated;
        //    }
        //    catch(Exception ex)
        //    {
        //        return 0;
        //    }
        //}

	    /// <summary>
	    ///     Update rows in the DB.
	    /// </summary>
	    /// <param name="tableName">The table to update.</param>
	    /// <param name="data">A dictionary containing Column names and their new values.</param>
	    /// <param name="where">The where clause for the update statement.</param>
	    /// <returns>A boolean true or false to signify success or failure.</returns>
	    public bool Update(String tableName, Dictionary<String, String> data, String where)
	    {
	        String vals = "";
	        Boolean returnCode = true;
	        if (data.Count >= 1)
	        {
	            foreach (KeyValuePair<String, String> val in data)
	            {
                    if(val.Value != null)
	                    vals += String.Format(" {0} = '{1}',", val.Key.ToString(), val.Value.ToString());
	            }
	            vals = vals.Substring(0, vals.Length - 1);
	        }
	        try
	        {
	            this.ExecuteNonQuery(String.Format("update {0} set {1} where {2};", tableName, vals, where));
	        }
	        catch
	        {
	            returnCode = false;
	        }
	        return returnCode;
	    }

	    /// <summary>
	    ///     Delete rows from the DB.
	    /// </summary>
	    /// <param name="tableName">The table from which to delete.</param>
	    /// <param name="where">The where clause for the delete.</param>
	    /// <returns>A boolean true or false to signify success or failure.</returns>
	    public bool Delete(String tableName, String where)
	    {
	        Boolean returnCode = true;
	        try
	        {
	            this.ExecuteNonQuery(String.Format("delete from {0} where {1};", tableName, where));
	        }
	        catch (Exception fail)
	        {
                //MessageBox.Show(fail.Message);
	            returnCode = false;
	        }
	        return returnCode;
	    }

	    /// <summary>
	    ///     Allows the programmer to easily insert into the DB
	    /// </summary>
	    /// <param name="tableName">The table into which we insert the data.</param>
	    /// <param name="data">A dictionary containing the column names and data for the insert.</param>
	    /// <returns>A boolean true or false to signify success or failure.</returns>
	    public bool Insert(String tableName, Dictionary<String, String> data)
	    {
	        String columns = "";
	        String values = "";
	        Boolean returnCode = true;
	        foreach (KeyValuePair<String, String> val in data)
	        {
	            columns += String.Format(" {0},", val.Key.ToString());
	            values += String.Format(" '{0}',", val.Value);
	        }
	        columns = columns.Substring(0, columns.Length - 1);
	        values = values.Substring(0, values.Length - 1);
	        try
	        {
	            int affected = this.ExecuteNonQuery(String.Format("insert into {0}({1}) values({2});", tableName, columns, values));
	        }
	        catch(Exception fail)
	        {
                //MessageBox.Show(fail.Message);
	            returnCode = false;
	        }
	        return returnCode;
	    }

    }
}
