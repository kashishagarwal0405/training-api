using Microsoft.AspNetCore.Mvc;
using TrainingAPI.Models;
using TrainingAPI.Services;

namespace TrainingAPI.Controllers
{
    [ApiController]
    [Route("api/training")]
    public class TrainingController : ControllerBase
    {
        private readonly ITrainingService _trainingService;
        private readonly ILogger<TrainingController> _logger;

        public TrainingController(ITrainingService trainingService, ILogger<TrainingController> logger)
        {
            _trainingService = trainingService;
            _logger = logger;
        }

        // Training Requests
        [HttpGet("requests")]
        public async Task<ActionResult<IEnumerable<TrainingRequest>>> GetAllTrainingRequests()
        {
            try
            {
                var requests = await _trainingService.GetAllTrainingRequestsAsync();
                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all training requests");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("requests/{id}")]
        public async Task<ActionResult<TrainingRequest>> GetTrainingRequestById(int id)
        {
            try
            {
                var request = await _trainingService.GetTrainingRequestByIdAsync(id);
                if (request == null)
                    return NotFound();

                return Ok(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting training request {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("requests/user/{userId}")]
        public async Task<ActionResult<IEnumerable<TrainingRequest>>> GetUserTrainingRequests(int userId)
        {
            try
            {
                var requests = await _trainingService.GetTrainingRequestsByUserAsync(userId);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting training requests for user {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("sessions")]
        public async Task<ActionResult<IEnumerable<TrainingSession>>> GetAllTrainingSessions()
        {
            try
            {
                var sessions = await _trainingService.GetAllTrainingSessionsAsync();
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all training sessions");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("requests/status/{status}")]
        public async Task<ActionResult<IEnumerable<TrainingRequest>>> GetTrainingRequestsByStatus(string status)
        {
            try
            {
                var requests = await _trainingService.GetTrainingRequestsByStatusAsync(status);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting training requests by status {Status}", status);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("requests")]
        public async Task<ActionResult<TrainingRequest>> CreateTrainingRequest(TrainingRequest request)
        {
            try
            {
                var createdRequest = await _trainingService.CreateTrainingRequestAsync(request);
                return CreatedAtAction(nameof(GetTrainingRequestById), new { id = createdRequest.Id }, createdRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating training request");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("requests/{id}/status")]
        public async Task<ActionResult<TrainingRequest>> UpdateTrainingRequestStatus(int id, [FromBody] string status)
        {
            try
            {
                var updatedRequest = await _trainingService.UpdateTrainingRequestStatusAsync(id, status);
                return Ok(updatedRequest);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating training request status {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("requests/{id}")]
        public async Task<IActionResult> DeleteTrainingRequest(int id)
        {
            try
            {
                var success = await _trainingService.DeleteTrainingRequestAsync(id);
                if (!success)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting training request {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // Training Sessions
        [HttpGet("sessions/registered/{userId}")]
        public async Task<ActionResult<IEnumerable<TrainingSession>>> GetRegisteredSessionsByUser(int userId)
        {
            try
            {
                var sessions = await _trainingService.GetRegisteredSessionsByUserAsync(userId);
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting registered sessions for user {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("sessions/{id}")]
        public async Task<ActionResult<TrainingSession>> GetTrainingSessionById(int id)
        {
            try
            {
                var session = await _trainingService.GetTrainingSessionByIdAsync(id);
                if (session == null)
                    return NotFound();

                return Ok(session);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting training session {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("sessions/request/{requestId}")]
        public async Task<ActionResult<IEnumerable<TrainingSession>>> GetTrainingSessionsByRequest(int requestId)
        {
            try
            {
                var sessions = await _trainingService.GetTrainingSessionsByRequestAsync(requestId);
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting training sessions for request {RequestId}", requestId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("sessions")]
        public async Task<ActionResult<TrainingSession>> CreateTrainingSession(TrainingSession session)
        {
            try
            {
                var createdSession = await _trainingService.CreateTrainingSessionAsync(session);
                return CreatedAtAction(nameof(GetTrainingSessionById), new { id = createdSession.Id }, createdSession);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating training session");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("sessions/{id}")]
        public async Task<ActionResult<TrainingSession>> UpdateTrainingSession(int id, TrainingSession session)
        {
            try
            {
                if (id != session.Id)
                    return BadRequest();

                var updatedSession = await _trainingService.UpdateTrainingSessionAsync(session);
                return Ok(updatedSession);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating training session {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("sessions/{id}")]
        public async Task<IActionResult> DeleteTrainingSession(int id)
        {
            try
            {
                var success = await _trainingService.DeleteTrainingSessionAsync(id);
                if (!success)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting training session {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // Training Participants
        [HttpGet("sessions/{sessionId}/participants")]
        public async Task<ActionResult<IEnumerable<TrainingParticipant>>> GetSessionParticipants(int sessionId)
        {
            try
            {
                var participants = await _trainingService.GetSessionParticipantsAsync(sessionId);
                return Ok(participants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting participants for session {SessionId}", sessionId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("sessions/{sessionId}/register/{userId}")]
        public async Task<ActionResult<TrainingParticipant>> RegisterForSession(int sessionId, int userId)
        {
            try
            {
                var participant = await _trainingService.RegisterForSessionAsync(userId, sessionId);
                return Ok(participant);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user {UserId} for session {SessionId}", userId, sessionId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("sessions/{sessionId}/unregister/{userId}")]
        public async Task<IActionResult> UnregisterFromSession(int sessionId, int userId)
        {
            try
            {
                var success = await _trainingService.UnregisterFromSessionAsync(userId, sessionId);
                if (!success)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unregistering user {UserId} from session {SessionId}", userId, sessionId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("sessions/{sessionId}/participants/{userId}/status")]
        public async Task<ActionResult<TrainingParticipant>> UpdateParticipantStatus(int sessionId, int userId, [FromBody] string status)
        {
            try
            {
                var participant = await _trainingService.UpdateParticipantStatusAsync(userId, sessionId, status);
                return Ok(participant);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating participant status for user {UserId} in session {SessionId}", userId, sessionId);
                return StatusCode(500, "Internal server error");
            }
        }

        // Dashboard
        [HttpGet("dashboard/{role}")]
        public async Task<ActionResult<object>> GetDashboardStats(string role, [FromQuery] int? userId)
        {
            try
            {
                var stats = await _trainingService.GetDashboardStatsAsync(role, userId);
                return Ok(stats);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard stats for role {Role}", role);
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 