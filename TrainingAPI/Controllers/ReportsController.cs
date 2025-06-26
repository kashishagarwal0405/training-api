using Microsoft.AspNetCore.Mvc;
using TrainingAPI.Services;
using Microsoft.Extensions.Logging;

namespace TrainingAPI.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(IReportService reportService, ILogger<ReportsController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        [HttpGet("training-requests")]
        public async Task<ActionResult<IEnumerable<object>>> GetTrainingRequestReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var report = await _reportService.GetTrainingRequestReportAsync(startDate, endDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting training request report");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("departments")]
        public async Task<ActionResult<IEnumerable<object>>> GetDepartmentReport()
        {
            try
            {
                var report = await _reportService.GetDepartmentReportAsync();
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting department report");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("training-sessions")]
        public async Task<ActionResult<object>> GetTrainingSessionReport(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var report = await _reportService.GetTrainingSessionReportAsync(startDate, endDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting training session report");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("participation")]
        public async Task<ActionResult<object>> GetParticipationReport([FromQuery] int? userId = null)
        {
            try
            {
                var report = await _reportService.GetUserParticipationReportAsync(userId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting participation report");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("budget")]
        public async Task<ActionResult<IEnumerable<object>>> GetBudgetReport()
        {
            try
            {
                var report = await _reportService.GetBudgetReportAsync();
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting budget report");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("attendance")]
        public async Task<ActionResult<IEnumerable<object>>> GetAttendanceReport(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var report = await _reportService.GetAttendanceReportAsync(startDate, endDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting attendance report");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("trainer-performance")]
        public async Task<ActionResult<IEnumerable<object>>> GetTrainerPerformanceReport()
        {
            try
            {
                var report = await _reportService.GetTrainerPerformanceReportAsync();
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting trainer performance report");
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 