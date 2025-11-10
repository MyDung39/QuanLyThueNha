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
                return @"Data Source=THANHNHA-PC\SQLEXPRESS;Initial Catalog=QLTN;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name=""SQL Server Management Studio"";Connect Timeout=30";
            }
        }
    }
}
