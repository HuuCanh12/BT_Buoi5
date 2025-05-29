using System.ComponentModel.DataAnnotations;

namespace BT_Buoi5.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Họ là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Họ không được quá 50 ký tự.")]
        [Display(Name = "Họ")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Tên là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Tên không được quá 50 ký tự.")]
        [Display(Name = "Tên")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn lớp học.")]
        [Display(Name = "Lớp học")]
        public int GradeId { get; set; }

        // Navigation property
        public Grade? Grade { get; set; }
    }
}