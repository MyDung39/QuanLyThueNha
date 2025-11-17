using RoomManagementSystem.DataLayer;
using System.Data;

namespace RoomManagementSystem.BusinessLayer
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
