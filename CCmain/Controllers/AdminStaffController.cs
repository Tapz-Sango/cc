using CrustyCrab.Models;
using CrustyCrab.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrustyCrab.Controllers
{
    public class AdminStaffController : Controller
    {
        private readonly CrustyApiClient _api;

        public AdminStaffController(CrustyApiClient api)
        {
            _api = api;
        }

        // ADMIN STAFF PAGE
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Manager");

            var model = new AdminStaffViewModel
            {
                Staff = await _api.GetStaff() ?? new List<ApiStaff>(),
                Admins = await _api.GetAdmins() ?? new List<ApiAdmin>(),
                Customers = await _api.GetCustomers() ?? new List<ApiCustomer>()
            };

            return View(model);
        }

        // STAFF FORM ACTIONS

        [HttpPost]
        public async Task<IActionResult> CreateStaffForm(StaffRequest request)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Manager");

            var staff = await _api.CreateStaff(
                request.FirstName,
                request.LastName,
                request.Role,
                request.PhoneNumber);

            TempData[staff == null ? "Error" : "Success"] =
                staff == null
                    ? "Staff member could not be created."
                    : "Staff member created.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStaffForm(int id, StaffRequest request)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Manager");

            var staff = await _api.UpdateStaff(
                id,
                request.FirstName,
                request.LastName,
                request.Role,
                request.PhoneNumber);

            TempData[staff == null ? "Error" : "Success"] =
                staff == null
                    ? "Staff member could not be updated."
                    : "Staff member updated.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStaffForm(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Manager");

            var deleted = await _api.DeleteStaff(id);

            TempData[deleted ? "Success" : "Error"] =
                deleted
                    ? "Staff member deleted."
                    : "Staff member could not be deleted.";

            return RedirectToAction(nameof(Index));
        }

        // ADMIN FORM ACTIONS

        [HttpPost]
        public async Task<IActionResult> CreateAdminForm(AdminRequest request)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Manager");

            var admin = await _api.CreateAdmin(
                request.Username,
                request.EmailAddress,
                request.Password,
                request.StaffId);

            TempData[admin == null ? "Error" : "Success"] =
                admin == null
                    ? "Admin account could not be created."
                    : "Admin account created.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAdminForm(
            int id,
            AdminUpdateRequest request)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Manager");

            var admin = await _api.UpdateAdmin(
                id,
                request.Username,
                request.EmailAddress,
                request.StaffId);

            TempData[admin == null ? "Error" : "Success"] =
                admin == null
                    ? "Admin account could not be updated."
                    : "Admin account updated.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAdminForm(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Manager");

            var deleted = await _api.DeleteAdmin(id);

            TempData[deleted ? "Success" : "Error"] =
                deleted
                    ? "Admin account deleted."
                    : "Admin account could not be deleted.";

            return RedirectToAction(nameof(Index));
        }

        // STAFF API-STYLE ACTIONS

        [HttpGet]
        public async Task<IActionResult> Staff()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Manager");

            var staff = await _api.GetStaff();

            return staff == null
                ? StatusCode(502)
                : Json(staff);
        }

        [HttpGet]
        public async Task<IActionResult> GetStaff(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Manager");

            var staff = await _api.GetStaff(id);

            return staff == null
                ? NotFound()
                : Json(staff);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff(
            [FromBody] StaffRequest request)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Manager");

            var staff = await _api.CreateStaff(
                request.FirstName,
                request.LastName,
                request.Role,
                request.PhoneNumber);

            return staff == null
                ? BadRequest()
                : Json(staff);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStaff(
            int id,
            [FromBody] StaffRequest request)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Manager");

            var staff = await _api.UpdateStaff(
                id,
                request.FirstName,
                request.LastName,
                request.Role,
                request.PhoneNumber);

            return staff == null
                ? NotFound()
                : Json(staff);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Manager");

            return await _api.DeleteStaff(id)
                ? NoContent()
                : NotFound();
        }

        // ADMIN API-STYLE ACTIONS

        [HttpGet]
        public async Task<IActionResult> Admins()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Manager");

            var admins = await _api.GetAdmins();

            return admins == null
                ? StatusCode(502)
                : Json(admins);
        }

        [HttpGet]
        public async Task<IActionResult> GetAdmin(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Manager");

            var admin = await _api.GetAdmin(id);

            return admin == null
                ? NotFound()
                : Json(admin);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdmin(
            [FromBody] AdminRequest request)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Manager");

            var admin = await _api.CreateAdmin(
                request.Username,
                request.EmailAddress,
                request.Password,
                request.StaffId);

            return admin == null
                ? BadRequest()
                : Json(admin);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAdmin(
            int id,
            [FromBody] AdminUpdateRequest request)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Manager");

            var admin = await _api.UpdateAdmin(
                id,
                request.Username,
                request.EmailAddress,
                request.StaffId);

            return admin == null
                ? BadRequest()
                : Json(admin);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Manager");

            return await _api.DeleteAdmin(id)
                ? NoContent()
                : NotFound();
        }

        // ADMIN CHECK

        private bool IsAdmin()
        {
            return HttpContext.Session.GetInt32("UserId").HasValue &&
                   string.Equals(
                       HttpContext.Session.GetString("Role"),
                       "Admin",
                       StringComparison.OrdinalIgnoreCase);
        }
    }
}