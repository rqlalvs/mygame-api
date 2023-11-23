using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace ApiMyGame.Database
{
    public class DBConnection
    {
        private string connectionString;
        private OracleConnection connection;

        public DBConnection(string username, string password, string tns, string sid, string porta)
        {
            connectionString = $"User Id={username};Password={password};Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={tns})(PORT={porta}))(CONNECT_DATA=(SID={sid})))";

            connection = new OracleConnection(connectionString);
        }

        public OracleConnection GetOracleConnection()
        {
            return connection;
        }

        public void OpenConnection()
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening connection: {ex.Message}");
            }
        }

        public void CloseConnection()
        {
            try
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing connection: {ex.Message}");
            }
        }

        public DataTable ExecuteQuery(string sqlQuery)
        {
            DataTable dataTable = new DataTable();

            try
            {
                OpenConnection();

                using (OracleDataAdapter adapter = new OracleDataAdapter(sqlQuery, connection))
                {
                    adapter.Fill(dataTable);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing query: {ex.Message}");
            }
            finally
            {
                CloseConnection();
            }

            return dataTable;
        }
    }
}
