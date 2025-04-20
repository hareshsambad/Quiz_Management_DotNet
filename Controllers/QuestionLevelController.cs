using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Quiz_Management.Models;

namespace Quiz_Management.Controllers
{
    [CheckAccess]
    public class QuestionLevelController : Controller
    {
        private IConfiguration configuration;

        public QuestionLevelController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }


        public IActionResult QuestionLevelList()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_MST_QuestionLevel_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        public IActionResult QuestionLevelSave(QuestionLevelModel model)
        {
            if (ModelState.IsValid)
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (model.QuestionLevelID == 0)
                {
                    command.CommandText = "PR_MST_QuestionLevel_Insert";
                }
                else
                {
                    command.CommandText = "PR_MST_QuestionLevel_Update";
                    command.Parameters.AddWithValue("@QuestionLevelID", model.QuestionLevelID);
                }
                command.Parameters.AddWithValue("@QuestionLevel", model.QuestionLevel);
                command.Parameters.AddWithValue("@UserID", model.UserID);
                command.ExecuteNonQuery();
                return RedirectToAction("QuestionLevelList");
            }
            return View("AddEditQuestionLevel",model);
        }
        public IActionResult AddEditQuestionLevel(int QuestionLevelID)
        {

            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_MST_QuestionLevel_SelectByID";
            command.Parameters.AddWithValue("@QuestionLevelID", QuestionLevelID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            QuestionLevelModel questionLevelModel = new QuestionLevelModel();
            foreach(DataRow row in table.Rows)
            {
                questionLevelModel.QuestionLevel = @row["QuestionLevel"].ToString();
                questionLevelModel.UserID = Convert.ToInt32(@row["UserID"]);
            }
            return View("AddEditQuestionLevel", questionLevelModel);
        }
        public IActionResult DeleteQuestionLevel(int QuestionLevelID)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandText = "[dbo].[PR_MST_QuestionLevel_Delete]";
            sqlCommand.Parameters.Add("@QuestionLevelID", SqlDbType.Int).Value = QuestionLevelID;
            sqlCommand.ExecuteNonQuery();
            return RedirectToAction("QuestionLevelList");
        }
    }
}
