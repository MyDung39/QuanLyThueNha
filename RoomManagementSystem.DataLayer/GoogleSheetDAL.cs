using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Data;
using System.IO;

namespace DAL
{
    public class GoogleSheetDAL
    {
        private readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        private readonly string ApplicationName = "QL Data App";
        private readonly string SpreadsheetId = "1LEWD6LmyBLAU4F9HMj1Aze3lEovquOOt5UuSOyhR4gw";
        private SheetsService service;

        public GoogleSheetDAL()
        {
            GoogleCredential credential;
            var path = Path.Combine(AppContext.BaseDirectory, "ServiceAccount", "confident-facet-478405-f3-6a6670539754.json");

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            }

            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
        }

        public DataTable GetSheetData(string sheetName)
        {
            var dt = new DataTable();
            var range = $"'{sheetName}'!A:Z";
            var request = service.Spreadsheets.Values.Get(SpreadsheetId, range);
            ValueRange response = request.Execute();

            if (response.Values == null || response.Values.Count == 0)
                return dt;

            // Lấy header
            foreach (var header in response.Values[0])
            {
                dt.Columns.Add(header.ToString());
            }

            // Lấy dữ liệu từ dòng 2
            for (int i = 1; i < response.Values.Count; i++)
            {
                var row = response.Values[i];
                var dr = dt.NewRow();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    dr[j] = j < row.Count ? row[j].ToString() : "";
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}
