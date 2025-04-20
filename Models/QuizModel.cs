using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Quiz_Management.Models
{
    public class QuizModel
    {
        public int QuizID { get; set; } // Nullable
        public string QuizName { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime QuizDate { get; set; }
        public int UserID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
    public class QuizDropdownModel
    {
        public int QuizID { get; set; }
        public string QuizName { get; set; }
    }

}
