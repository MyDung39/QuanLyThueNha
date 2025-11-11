using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace RoomManagementSystem.DataLayer
{
    public class Database
    {
        private readonly string connectionString = DbConfig.ConnectionString;

        // Hàm thực thi SELECT, trả về DataTable
        public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // Hàm thực thi INSERT/UPDATE/DELETE
        public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn); cmd.CommandType = CommandType.Text;
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteNonQuery();
            }
        }

        // Hàm thực thi lệnh scalar (trả về 1 giá trị duy nhất)
        public object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteScalar();
            }
        }


        public int ExecuteNonQuery(string query, SqlTransaction transaction, SqlParameter[] parameters = null)
        {
            // Sử dụng kết nối và giao dịch (transaction) được truyền vào
            // Chúng ta KHÔNG 'using' hoặc 'Close()' kết nối ở đây
            // vì hàm gọi nó sẽ quản lý việc đó
            SqlConnection conn = transaction.Connection;

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Transaction = transaction; // Quan trọng: Gắn giao dịch vào command
            cmd.CommandType = CommandType.Text;

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            return cmd.ExecuteNonQuery();
        }

    }
}
