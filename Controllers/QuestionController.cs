using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Quiz_Management.Models;
using OfficeOpenXml;

namespace Quiz_Management.Controllers
{
    [CheckAccess]
    public class QuestionController : Controller
    {
        private IConfiguration configuration;

        public QuestionController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IActionResult ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_MST_Question_SelectAll";
            //sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable data = new DataTable();
            data.Load(sqlDataReader);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DataSheet");

                // Add headers
                worksheet.Cells[1, 1].Value = "QuestionID";
                worksheet.Cells[1, 2].Value = "QuestionText";
                worksheet.Cells[1, 3].Value = "OptionA";
                worksheet.Cells[1, 4].Value = "OptionB";
                worksheet.Cells[1, 5].Value = "OptionC";
                worksheet.Cells[1, 6].Value = "OptionD";
                worksheet.Cells[1, 7].Value = "CorrectOption";
                worksheet.Cells[1, 8].Value = "Created";

                // Add data
                int row = 2;
                foreach (DataRow item in data.Rows)
                {
                    worksheet.Cells[row, 1].Value = item["QuestionID"];
                    worksheet.Cells[row, 2].Value = item["QuestionText"];
                    worksheet.Cells[row, 3].Value = item["OptionA"];
                    worksheet.Cells[row, 4].Value = item["OptionB"];
                    worksheet.Cells[row, 5].Value = item["OptionC"];
                    worksheet.Cells[row, 6].Value = item["OptionD"];
                    worksheet.Cells[row, 7].Value = item["CorrectOption"];
                    worksheet.Cells[row, 8].Value = item["Created"];

                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"QuestionList.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        #region QuestionList
        public IActionResult QuestionList()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_MST_Question_SelectAll";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable table = new DataTable();
                        table.Load(reader);
                        return View(table);
                    }
                }
            }
        }
        #endregion

        #region Question Save
        public IActionResult QuestionSave(QuestionModel model)
        {
            if (ModelState.IsValid)
            {
                QuestionLevelDropDown();
                string connectionString = configuration.GetConnectionString("ConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (model.QuestionID == 0)
                {
                    command.CommandText = "PR_MST_Question_Insert";
                    command.Parameters.Add("@QuestionText", SqlDbType.VarChar).Value = model.QuestionText;
                    command.Parameters.Add("@QuestionLevelID", SqlDbType.Int).Value = model.QuestionLevelID;
                    command.Parameters.Add("@OptionA", SqlDbType.VarChar).Value = model.OptionA;
                    command.Parameters.Add("@OptionB", SqlDbType.VarChar).Value = model.OptionB;
                    command.Parameters.Add("@OptionC", SqlDbType.VarChar).Value = model.OptionC;
                    command.Parameters.Add("@OptionD", SqlDbType.VarChar).Value = model.OptionD;
                    command.Parameters.Add("@CorrectOption", SqlDbType.VarChar).Value = model.CorrectOption;
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = model.UserID;
                    command.Parameters.Add("@QuestionMarks", SqlDbType.Int).Value = model.QuestionMarks;
                }
                else
                {
                    command.CommandText = "PR_MST_Question_Update";
                    command.Parameters.Add("@QuestionID", SqlDbType.Int).Value = model.QuestionID;
                    command.Parameters.Add("@QuestionText", SqlDbType.VarChar).Value = model.QuestionText;
                    command.Parameters.Add("@QuestionLevelID", SqlDbType.Int).Value = model.QuestionLevelID;
                    command.Parameters.Add("@OptionA", SqlDbType.VarChar).Value = model.OptionA;
                    command.Parameters.Add("@OptionB", SqlDbType.VarChar).Value = model.OptionB;
                    command.Parameters.Add("@OptionC", SqlDbType.VarChar).Value = model.OptionC;
                    command.Parameters.Add("@OptionD", SqlDbType.VarChar).Value = model.OptionD;
                    command.Parameters.Add("@CorrectOption", SqlDbType.VarChar).Value = model.CorrectOption;
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = model.UserID;
                    command.Parameters.Add("@QuestionMarks", SqlDbType.Int).Value = model.QuestionMarks;
                    command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = model.IsActive;
                }
  
                command.ExecuteNonQuery();

                return RedirectToAction("QuestionList");
            }
            else
            {
                QuestionLevelDropDown();
                return View("AddEditQuestion", model);
            }
        }
        #endregion

        #region Add or Edit Question
        public IActionResult AddEditQuestion(int QuestionID)
        {
            QuestionLevelDropDown();
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection=new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_MST_Question_SelectByID";
            command.Parameters.AddWithValue("@QuestionID", QuestionID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            QuestionModel model = new QuestionModel();
            foreach(DataRow row in table.Rows)
            {
                model.QuestionID = Convert.ToInt32(@row["QuestionID"]);
                model.QuestionText = @row["QuestionText"].ToString();
                model.QuestionLevelID = Convert.ToInt32(@row["QuestionLevelID"]);
                model.OptionA = @row["OptionA"].ToString();
                model.OptionB = @row["OptionB"].ToString();
                model.OptionC = @row["OptionC"].ToString();
                model.OptionD = @row["OptionD"].ToString();
                model.CorrectOption = @row["CorrectOption"].ToString();
                model.UserID = Convert.ToInt32(@row["UserID"]);
                model.QuestionMarks = Convert.ToInt32(@row["QuestionMarks"]);
                model.IsActive = Convert.ToBoolean(@row["IsActive"]);
            }
            return View("AddEditQuestion",model);
        }
        #endregion

        #region Question Delete
        public IActionResult QuestionDelete(int QuestionID)
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_MST_Question_Delete";
            command.Parameters.AddWithValue("@QuestionID", QuestionID);
            command.ExecuteNonQuery();
            return RedirectToAction("QuestionList");
        }
        #endregion

        #region Question DropDown
        public void QuestionLevelDropDown()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Dropdown_MST_QuestionLevel";
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            List<QuestionLevelDropdownModel> list = new List<QuestionLevelDropdownModel>();
            foreach(DataRow data in dataTable.Rows)
            {
                QuestionLevelDropdownModel model = new QuestionLevelDropdownModel();
                model.QuestionLevelID = Convert.ToInt32(data["QuestionLevelID"]);
                model.QuestionLevel = data["QuestionLevel"].ToString();
                list.Add(model);
            }
            ViewBag.QuestionLevel = list;
        }
        #endregion
    }
}
