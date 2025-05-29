using BT_Buoi5.Data;
using BT_Buoi5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BT_Buoi5.Controllers
{
    public class GradeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GradeController> _logger;

        public GradeController(ApplicationDbContext context, ILogger<GradeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Action xem danh sách các lớp học
        public IActionResult Index()
        {
            // Lấy danh sách tất cả các lớp học từ cơ sở dữ liệu và bao gồm thông tin học sinh
            List<Grade> listGrade = _context.Grades
                .Include(g => g.Students) // Bao gồm danh sách học sinh
                .ToList();

            // Trả về view "Index" và truyền danh sách lớp học vào view để hiển thị
            return View(listGrade);
        }

        // Action thêm mới lớp học (GET)
        public IActionResult Create()
        {
            return View();
        }

        // Action thêm mới lớp học (POST) - Enhanced với debug
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("GradeName")] Grade grade)
        {
            // Debug: Log thông tin về model state
            _logger.LogInformation("Create POST called with GradeName: {GradeName}", grade?.GradeName);

            // Debug: Kiểm tra từng validation error
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid. Errors:");
                foreach (var error in ModelState)
                {
                    _logger.LogWarning("Key: {Key}, Errors: {Errors}",
                        error.Key,
                        string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage)));
                }
            }

            // Validation bổ sung (nếu cần)
            if (string.IsNullOrWhiteSpace(grade?.GradeName))
            {
                ModelState.AddModelError("GradeName", "Tên lớp học không được để trống.");
            }
            else if (grade.GradeName.Length > 100) // Giả sử có giới hạn độ dài
            {
                ModelState.AddModelError("GradeName", "Tên lớp học không được quá 100 ký tự.");
            }
            else if (_context.Grades.Any(g => g.GradeName.ToLower() == grade.GradeName.ToLower()))
            {
                ModelState.AddModelError("GradeName", "Tên lớp học đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(grade);
                    _context.SaveChanges();
                    _logger.LogInformation("Grade created successfully: {GradeName}", grade.GradeName);

                    // Thêm thông báo thành công
                    TempData["SuccessMessage"] = "Tạo lớp học thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating grade: {GradeName}", grade.GradeName);
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo lớp học. Vui lòng thử lại.");
                }
            }

            // Nếu có lỗi, trả về view với model để hiển thị errors
            _logger.LogWarning("Returning Create view due to validation errors");
            return View(grade);
        }

        // Action xem chi tiết lớp học
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Grades == null)
            {
                return NotFound();
            }

            var grade = await _context.Grades
                .FirstOrDefaultAsync(m => m.GradeId == id);
            if (grade == null)
            {
                return NotFound();
            }

            return View(grade);
        }

        // Action chỉnh sửa lớp học (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Grades == null)
            {
                return NotFound();
            }

            var grade = await _context.Grades.FindAsync(id);
            if (grade == null)
            {
                return NotFound();
            }
            return View(grade);
        }

        // Action chỉnh sửa lớp học (POST) - Enhanced với debug
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GradeId,GradeName")] Grade grade)
        {
            // Debug: Log thông tin
            _logger.LogInformation("Edit POST called for ID {Id} with GradeName: {GradeName}",
                id, grade?.GradeName);

            if (id != grade.GradeId)
            {
                _logger.LogWarning("ID mismatch: URL ID {UrlId} vs Model ID {ModelId}", id, grade.GradeId);
                return NotFound();
            }

            // Debug: Kiểm tra validation errors
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Edit ModelState is invalid. Errors:");
                foreach (var error in ModelState)
                {
                    _logger.LogWarning("Key: {Key}, Errors: {Errors}",
                        error.Key,
                        string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage)));
                }
            }

            // Validation bổ sung
            if (string.IsNullOrWhiteSpace(grade?.GradeName))
            {
                ModelState.AddModelError("GradeName", "Tên lớp học không được để trống.");
            }
            else if (grade.GradeName.Length > 100)
            {
                ModelState.AddModelError("GradeName", "Tên lớp học không được quá 100 ký tự.");
            }
            else if (_context.Grades.Any(g => g.GradeName.ToLower() == grade.GradeName.ToLower() && g.GradeId != id))
            {
                ModelState.AddModelError("GradeName", "Tên lớp học đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(grade);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Grade updated successfully: ID {Id}, Name {Name}", id, grade.GradeName);

                    TempData["SuccessMessage"] = "Cập nhật lớp học thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!GradeExists(grade.GradeId))
                    {
                        _logger.LogWarning("Grade not found during update: ID {Id}", grade.GradeId);
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, "Concurrency error updating grade: ID {Id}", grade.GradeId);
                        ModelState.AddModelError("", "Có lỗi đồng thời xảy ra. Vui lòng thử lại.");
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating grade: ID {Id}", grade.GradeId);
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật lớp học. Vui lòng thử lại.");
                }
            }

            _logger.LogWarning("Returning Edit view due to validation errors");
            return View(grade);
        }

        // Action xóa lớp học (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Grades == null)
            {
                return NotFound();
            }

            var grade = await _context.Grades
                .FirstOrDefaultAsync(m => m.GradeId == id);
            if (grade == null)
            {
                return NotFound();
            }

            return View(grade);
        }

        // Action xác nhận xóa lớp học (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (_context.Grades == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Grades' is null.");
                }

                var grade = await _context.Grades.FindAsync(id);
                if (grade != null)
                {
                    _context.Grades.Remove(grade);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Grade deleted successfully: ID {Id}", id);
                    TempData["SuccessMessage"] = "Xóa lớp học thành công!";
                }
                else
                {
                    _logger.LogWarning("Grade not found for deletion: ID {Id}", id);
                    TempData["ErrorMessage"] = "Không tìm thấy lớp học để xóa.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting grade: ID {Id}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa lớp học. Vui lòng thử lại.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Action xem danh sách sinh viên theo lớp
        // GET: /Grade/Students/5
        public async Task<IActionResult> Students(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy thông tin lớp học
            var grade = await _context.Grades
                .Include(g => g.Students) // Bao gồm danh sách học sinh
                .FirstOrDefaultAsync(g => g.GradeId == id);

            if (grade == null)
            {
                return NotFound();
            }

            // Truyền tên lớp học vào ViewBag để hiển thị trong view
            ViewBag.GradeName = grade.GradeName;
            ViewBag.GradeId = id; // Truyền GradeId vào ViewBag cho nút Thêm mới

            // Trả về view "Students" với danh sách học sinh của lớp này
            return View(grade.Students.ToList());
        }

        private bool GradeExists(int id)
        {
            return (_context.Grades?.Any(e => e.GradeId == id)).GetValueOrDefault();
        }
    }
}