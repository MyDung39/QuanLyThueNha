using DAL;
using System.Data;

namespace BLL
{
    public class GoogleSheetBL
    {
        private GoogleSheetDAL dal;

        public GoogleSheetBL()
        {
            dal = new GoogleSheetDAL();
        }

        public DataTable GetData(string sheetName)
        {
            return dal.GetSheetData(sheetName);
        }
    }
}
