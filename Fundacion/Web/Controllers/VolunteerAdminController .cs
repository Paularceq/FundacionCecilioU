using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Constants;
using Shared.Enums;
using Web.Extensions;
using Web.Models.Volunteer;
using Web.Services;

namespace Web.Controllers
{
    [Authorize(Roles = Roles.AdminSistema)]
    public class VolunteerAdminController : Controller
    {
        private readonly VolunteerRequestService _volunteerRequestService;
        private readonly VolunteerHoursService _volunteerHoursService;

        public VolunteerAdminController(
            VolunteerRequestService volunteerRequestService,
            VolunteerHoursService volunteerHoursService)
        {
            _volunteerRequestService = volunteerRequestService;
            _volunteerHoursService = volunteerHoursService;
        }

        // ===== GESTIÓN DE SOLICITUDES =====
        public async Task<IActionResult> Index(string filter = "all")
        {
            var allRequests = await _volunteerRequestService.GetAllRequestsAsync();

            var filteredRequests = filter switch
            {
                "pending" => allRequests.Where(r => r.State == VolunteerState.Pending).ToList(),
                "approved" => allRequests.Where(r => r.State == VolunteerState.Approved).ToList(),
                "rejected" => allRequests.Where(r => r.State == VolunteerState.Rejected).ToList(),
                _ => allRequests
            };

            var viewModel = new AdminVolunteerViewModel
            {
                PendingRequests = allRequests.Where(r => r.State == VolunteerState.Pending).ToList(),
                ApprovedRequests = allRequests.Where(r => r.State == VolunteerState.Approved).ToList(),
                RejectedRequests = allRequests.Where(r => r.State == VolunteerState.Rejected).ToList(),
                PendingHours = await _volunteerHoursService.GetPendingHoursAsync(),
                FilterState = filter switch
                {
                    "pending" => VolunteerState.Pending,
                    "approved" => VolunteerState.Approved,
                    "rejected" => VolunteerState.Rejected,
                    _ => null
                }
            };

            ViewBag.CurrentFilter = filter;
            ViewBag.FilteredRequests = filteredRequests;

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _volunteerRequestService.GetRequestByIdAsync(id);
            if (result.IsFailure)
            {
                this.SetErrorMessage("Solicitud no encontrada");
                return RedirectToAction(nameof(Index));
            }

            return View(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var approverId = GetCurrentUserId();
            var approverName = GetCurrentUserName();

            var result = await _volunteerRequestService.ApproveRequestAsync(id, approverId, approverName);

            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
            }
            else
            {
                this.SetSuccessMessage("Solicitud aprobada correctamente");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                this.SetErrorMessage("Debe proporcionar una razón para el rechazo");
                return RedirectToAction(nameof(Index));
            }

            var approverId = GetCurrentUserId();
            var approverName = GetCurrentUserName();

            var result = await _volunteerRequestService.RejectRequestAsync(id, approverId, approverName, reason);

            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
            }
            else
            {
                this.SetSuccessMessage("Solicitud rechazada correctamente");
            }

            return RedirectToAction(nameof(Index));
        }

        // ===== GESTIÓN DE HORAS =====
        public async Task<IActionResult> PendingHours()
        {
            var pendingHours = await _volunteerHoursService.GetPendingHoursAsync();
            return View(pendingHours);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveHours(int hoursId)
        {
            var approverId = GetCurrentUserId();
            var approverName = GetCurrentUserName();

            var result = await _volunteerHoursService.ApproveHoursAsync(hoursId, approverId, approverName);

            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
            }
            else
            {
                this.SetSuccessMessage("Horas aprobadas correctamente");
            }

            return RedirectToAction(nameof(PendingHours));
        }

        [HttpPost]
        public async Task<IActionResult> RejectHours(int hoursId, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                this.SetErrorMessage("Debe proporcionar una razón para el rechazo");
                return RedirectToAction(nameof(PendingHours));
            }

            var approverId = GetCurrentUserId();
            var approverName = GetCurrentUserName();

            var result = await _volunteerHoursService.RejectHoursAsync(hoursId, approverId, approverName, reason);

            if (result.IsFailure)
            {
                this.SetErrorMessage(result.Errors);
            }
            else
            {
                this.SetSuccessMessage("Horas rechazadas correctamente");
            }

            return RedirectToAction(nameof(PendingHours));
        }

        // ===== API ENDPOINTS PARA AJAX =====
        [HttpPost]
        public async Task<JsonResult> QuickApprove(int id)
        {
            var approverId = GetCurrentUserId();
            var approverName = GetCurrentUserName();

            var result = await _volunteerRequestService.ApproveRequestAsync(id, approverId, approverName);

            return Json(new
            {
                success = result.IsSuccess,
                message = result.IsSuccess ? "Solicitud aprobada" : string.Join(", ", result.Errors)
            });
        }

        [HttpPost]
        public async Task<JsonResult> QuickReject(int id, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                return Json(new
                {
                    success = false,
                    message = "Debe proporcionar una razón para el rechazo"
                });
            }

            var approverId = GetCurrentUserId();
            var approverName = GetCurrentUserName();

            var result = await _volunteerRequestService.RejectRequestAsync(id, approverId, approverName, reason);

            return Json(new
            {
                success = result.IsSuccess,
                message = result.IsSuccess ? "Solicitud rechazada" : string.Join(", ", result.Errors)
            });
        }

        [HttpPost]
        public async Task<JsonResult> QuickApproveHours(int hoursId)
        {
            var approverId = GetCurrentUserId();
            var approverName = GetCurrentUserName();

            var result = await _volunteerHoursService.ApproveHoursAsync(hoursId, approverId, approverName);

            return Json(new
            {
                success = result.IsSuccess,
                message = result.IsSuccess ? "Horas aprobadas" : string.Join(", ", result.Errors)
            });
        }

        [HttpPost]
        public async Task<JsonResult> QuickRejectHours(int hoursId, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                return Json(new
                {
                    success = false,
                    message = "Debe proporcionar una razón para el rechazo"
                });
            }

            var approverId = GetCurrentUserId();
            var approverName = GetCurrentUserName();

            var result = await _volunteerHoursService.RejectHoursAsync(hoursId, approverId, approverName, reason);

            return Json(new
            {
                success = result.IsSuccess,
                message = result.IsSuccess ? "Horas rechazadas" : string.Join(", ", result.Errors)
            });
        }

        // ===== BÚSQUEDA BÁSICA =====
        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm, string state)
        {
            VolunteerState? filterState = state switch
            {
                "pending" => VolunteerState.Pending,
                "approved" => VolunteerState.Approved,
                "rejected" => VolunteerState.Rejected,
                _ => null
            };

            var requests = await _volunteerRequestService.SearchRequestsAsync(searchTerm, filterState);

            var viewModel = new AdminVolunteerViewModel
            {
                PendingRequests = requests.Where(r => r.State == VolunteerState.Pending).ToList(),
                ApprovedRequests = requests.Where(r => r.State == VolunteerState.Approved).ToList(),
                RejectedRequests = requests.Where(r => r.State == VolunteerState.Rejected).ToList(),
                SearchTerm = searchTerm,
                FilterState = filterState
            };

            ViewBag.CurrentFilter = state ?? "all";
            ViewBag.FilteredRequests = requests;
            ViewBag.SearchTerm = searchTerm;

            return View("Index", viewModel);
        }

        // ===== MÉTODOS AUXILIARES =====
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }

        private string GetCurrentUserName()
        {
            return User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Administrador";
        }
    }
}