using System.ComponentModel.DataAnnotations;

namespace BT_Buoi5.Models
{
    public class Grade
    {
        public int GradeId { get; set; }

        [Required(ErrorMessage = "Tên lớp học là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên lớp học không được quá 100 ký tự.")]
        [Display(Name = "Tên lớp học")]
        public string GradeName { get; set; } = string.Empty; // Initialize with a default value

        public List<Student> Students { get; set; } = new List<Student>();
    }
}