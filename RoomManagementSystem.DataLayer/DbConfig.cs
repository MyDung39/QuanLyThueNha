using System.Configuration;

namespace RoomManagementSystem.DataLayer
{
    public static class DbConfig
    {
        public static string ConnectionString
        {
            get
            {
                var fromConfig = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
                if (!string.IsNullOrWhiteSpace(fromConfig)) return fromConfig;
                // Fallback to current known working connection for development.
                // Thay đổi "Data Source" thành tên SQL Server của bạn
                // Ví dụ: localhost\SQLEXPRESS hoặc .\SQLEXPRESS hoặc (local)\SQLEXPRESS
                return @"Data Source=.\SQLEXPRESS;Initial Catalog=QLTN;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connect Timeout=30";
            }
        }
    }
}
