using System.Data;
using System.Data.SqlClient;
using System.Windows.Input;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Quiz_Management.Models;

namespace Quiz_Management.Controllers
{
    [CheckAccess]
    public class QuizWiseQuestionController : Controller
    {
        private IConfiguration configuration;

        public QuizWiseQuestionController(IConfiguration _configuration)
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
            sqlCommand.CommandText = "PR_MST_QuizWiseQuestion_SelectAll";
            //sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable data = new DataTable();
            data.Load(sqlDataReader);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DataSheet");

                // Add headers
                worksheet.Cells[1, 1].Value = "QuizWiseQuestionID";
                worksheet.Cells[1, 2].Value = "QuizID";
                worksheet.Cells[1, 3].Value = "QuestionID";
                worksheet.Cells[1, 4].Value = "UserID";
                worksheet.Cells[1, 5].Value = "Created";
                worksheet.Cells[1, 5].Value = "Modified";

                // Add data
                int row = 2;
                foreach (DataRow item in data.Rows)
                {
                    worksheet.Cells[row, 1].Value = item["QuizWiseQuestionID"];
                    worksheet.Cells[row, 2].Value = item["QuizID"];
                    worksheet.Cells[row, 3].Value = item["QuestionID"];
                    worksheet.Cells[row, 4].Value = item["UserID"];
                    worksheet.Cells[row, 5].Value = item["Created"];
                    worksheet.Cells[row, 5].Value = item["Modified"];
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = "QuizWiseQuestion.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        #region Quiz Wise Question List
        public IActionResult QuizWiseQuestionList()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection=new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_MST_QuizWiseQuestions_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        #endregion

        #region Quiz wise Question Save
        public IActionResult QuizWiseQuestionSave(QuizWiseQuestionModel model)
        {
            if (ModelState.IsValid)
            {
                QuizDropDown();
                QuestionDropDown();
                string connectionString = configuration.GetConnectionString("ConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                SqlCommand command = sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (model.QuizWiseQuestionsID == 0)
                {
                    command.CommandText = "PR_MST_QuizWiseQuestions_Insert";
                }
                else
                {
                    command.CommandText = "PR_MST_QuizWiseQuestions_Update";
                    command.Parameters.Add("@QuizWiseQuestionsID", SqlDbType.Int).Value = model.QuizWiseQuestionsID;
                }
                command.Parameters.Add("@QuizID", SqlDbType.Int).Value = model.QuizID;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = model.UserID;
                command.Parameters.Add("@QuestionID", SqlDbType.Int).Value = model.QuestionID;
                command.ExecuteNonQuery();

                return RedirectToAction("QuizWiseQuestionList");
            }
            else
            {
                QuizDropDown();
                QuestionDropDown();
                return View("AddEditQuizWiseQuestion", model);
            }
        }
        #endregion

        #region Add Edit Quiz Wise Question
        public IActionResult AddEditQuizWiseQuestion(int QuizWiseQuestionsID)
        {
            QuizDropDown();
            QuestionDropDown();
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_MST_QuizWiseQuestions_SelectByID";
            command.Parameters.AddWithValue("@QuizWiseQuestionsID", QuizWiseQuestionsID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            QuizWiseQuestionModel model = new QuizWiseQuestionModel();
            foreach(DataRow row in dataTable.Rows)
            {
                model.QuizID = Convert.ToInt32(@row["QuizID"]);
                model.QuestionID = Convert.ToInt32(@row["QuestionID"]);
                model.UserID = Convert.ToInt32(@row["UserID"]);
            }
            return View("AddEditQuizWiseQuestion",model);
        }
        #endregion

        #region Quiz Wise Question Delete
        public IActionResult QuizWiseQuestionDelete(int QuizWiseQuestionsID)
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_MST_QuizWiseQuestions_Delete";
            command.Parameters.AddWithValue("@QuizWiseQuestionsID", QuizWiseQuestionsID);
            command.ExecuteNonQuery();
            return RedirectToAction("QuizWiseQuestionList");
        }
        #endregion 

        #region Question DropDown
        public void QuestionDropDown()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Dropdown_MST_Question";
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            List<QuestionDropdownModel> list = new List<QuestionDropdownModel>();
            foreach (DataRow data in dataTable.Rows)
            {
                QuestionDropdownModel model = new QuestionDropdownModel();
                model.QuestionID = Convert.ToInt32(data["QuestionID"]);
                model.QuestionText = data["QuestionText"].ToString();
                list.Add(model);
            }
            ViewBag.Question = list;
        }
        #endregion



        #region Quiz DropDown
        public void QuizDropDown()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Dropdown_MST_Quiz";
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            List<QuizDropdownModel> list = new List<QuizDropdownModel>();
            foreach (DataRow data in dataTable.Rows)
            {
                QuizDropdownModel model = new QuizDropdownModel();
                model.QuizID = Convert.ToInt32(data["QuizID"]);
                model.QuizName = data["QuizName"].ToString();
                list.Add(model);
            }
            ViewBag.Quiz = list;
        }
        #endregion
    }
}
