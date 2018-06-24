using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Sql;
using System.Data.SqlClient;

namespace LCK_DBcommunication
{
    public class DBcommunication
    {
        private SqlConnection conn = new SqlConnection();
        public string ConnectionString { get; set; }

        public DBcommunication(string conString)
        {
            this.ConnectionString = conString;
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

        public SqlDataReader ExecuteCommand(string cmd)
        {
            try
            {
                Open();
                SqlCommand sql = new SqlCommand(cmd, conn);
                SqlDataReader rdr = sql.ExecuteReader();
                return rdr;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
