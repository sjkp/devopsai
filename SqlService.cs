using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tryAGI.OpenAI;

namespace CSharp_OpenAI_LangChain
{

    [OpenAiFunctions]
    public interface ISqlService
    {        
        [Description("Get information about a specific SQL table")]
        Task<string> GetTableInformation(
            [Description("The table name")]
            string buildId, CancellationToken cancellationToken);

        [Description("Executes an MSSQL query against a database")]
        Task<string> ExecuteQuery(
            [Description("The MSSQL query to execute")]
            string query,
            CancellationToken cancellationToken);
    }

    public class SqlService : ISqlService
    {
        private readonly string connectionString;

        public SqlService(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public async Task<string> ExecuteQuery(string query, CancellationToken cancellationToken)
        {
            var sb = new StringBuilder();
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connectionString;
                conn.Open();

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            // Displaying the column names
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                sb.Append(reader.GetName(i) + "\t");
                            }
                            sb.AppendLine();

                            // Displaying the rows
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    sb.Append(reader[i] + "\t");
                                }
                                sb.AppendLine();
                            }
                        }
                        else
                        {
                            sb.AppendLine($"No rows found in table");
                        }
                    }

                    return sb.ToString();
                }
            }
        }        

        public async Task<string> GetTableInformation(string tableName, CancellationToken cancellationToken)
        {
            //StringBuilder script = new StringBuilder();

            //using (SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    connection.Open();

            //    string query = "sp_helptext";
            //    using (SqlCommand command = new SqlCommand(query, connection))
            //    {
            //        command.CommandType = System.Data.CommandType.StoredProcedure;
            //        command.Parameters.AddWithValue("@objname", tableName);

            //        using (SqlDataReader reader = await command.ExecuteReaderAsync())
            //        {
            //            while (reader.Read())
            //            {
            //                script.Append(reader["Text"].ToString());
            //            }
            //        }
            //    }
            //}

            StringBuilder script = new StringBuilder($"CREATE TABLE {tableName} (\n");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
                             FROM INFORMATION_SCHEMA.COLUMNS
                             WHERE TABLE_NAME = @tableName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tableName", tableName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string columnName = reader["COLUMN_NAME"].ToString();
                            string dataType = reader["DATA_TYPE"].ToString();
                            string maxLength = reader["CHARACTER_MAXIMUM_LENGTH"].ToString();
                            string isNullable = reader["IS_NULLABLE"].ToString();

                            if (dataType == "nvarchar" || dataType == "varchar")
                                dataType += $"({maxLength})";

                            if (isNullable == "NO")
                                isNullable = "NOT NULL";
                            else
                                isNullable = "NULL";

                            script.Append($"    {columnName} {dataType} {isNullable},\n");
                        }
                    }
                }
            }

            script.Remove(script.Length - 2, 2); // Remove the last comma
            script.Append("\n);");

            return $"{script.ToString()}\n\n3 rows from {tableName}\n{await ExecuteQuery($"SELECT TOP 3 * from {tableName}", cancellationToken)}";
        }      
    }
}
