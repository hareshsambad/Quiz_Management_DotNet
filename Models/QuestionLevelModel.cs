
using System.ComponentModel.DataAnnotations;

namespace Quiz_Management.Models
{
    public class QuestionLevelModel
    {
        public int QuestionLevelID { get; set; }
        public string QuestionLevel { get; set; }
        public int UserID { get; set; }
        public string? UserName { get; set; }
        public DateTime Created {  get; set; }
        public DateTime Modified { get; set; }
    }
    public class QuestionLevelDropdownModel
    {
        public int QuestionLevelID { get; set; }
        public string QuestionLevel { get; set; }
    }
}
