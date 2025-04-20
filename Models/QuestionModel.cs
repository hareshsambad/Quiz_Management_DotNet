using System.ComponentModel.DataAnnotations;

namespace Quiz_Management.Models
{
    public class QuestionModel
    {
        public int QuestionID { get; set; }
        [Required]
        public string QuestionText { get; set; }
        [Required]
        public int QuestionLevelID { get; set; }

        public string? QuestionLevel { get; set; }
        [Required]
        public string OptionA { get; set; }
        [Required]
        public string OptionB { get; set; }
        [Required]
        public string OptionC { get; set; }
        [Required]
        public string OptionD { get; set; }
        [Required]
        public string CorrectOption { get; set; }
        [Required]
        public int QuestionMarks { get; set; }
        public int UserID { get; set; }
        public Boolean IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
    public class QuestionDropdownModel
    {
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }
    }
}
