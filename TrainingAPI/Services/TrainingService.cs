using Dapper;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using TrainingAPI.Models;

namespace TrainingAPI.Services
{
    public class TrainingService : ITrainingService
    {
        private readonly string _connectionString;
        private readonly ILogger<TrainingService> _logger;

        public TrainingService(IConfiguration configuration, ILogger<TrainingService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        // Training Requests
        public async Task<IEnumerable<TrainingRequest>> GetAllTrainingRequestsAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                SELECT r.*, u.Name as RequesterName, u.Email as RequesterEmail 
                FROM TrainingRequests r 
                INNER JOIN Users u ON r.RequesterId = u.Id 
                ORDER BY r.CreatedAt DESC";
            return await connection.QueryAsync<TrainingRequest>(sql);
        }

        public async Task<TrainingRequest?> GetTrainingRequestByIdAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                SELECT r.*, u.Name as RequesterName, u.Email as RequesterEmail 
                FROM TrainingRequests r 
                INNER JOIN Users u ON r.RequesterId = u.Id 
                WHERE r.Id = @Id";
            return await connection.QuerySingleOrDefaultAsync<TrainingRequest>(sql, new { Id = id });
        }

        public async Task<IEnumerable<TrainingRequest>> GetTrainingRequestsByUserAsync(int userId)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                SELECT r.*, u.Name as RequesterName, u.Email as RequesterEmail 
                FROM TrainingRequests r 
                INNER JOIN Users u ON r.RequesterId = u.Id 
                WHERE r.RequesterId = @UserId 
                ORDER BY r.CreatedAt DESC";
            return await connection.QueryAsync<TrainingRequest>(sql, new { UserId = userId });
        }

        public async Task<IEnumerable<TrainingRequest>> GetTrainingRequestsByStatusAsync(string status)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                SELECT r.*, u.Name as RequesterName, u.Email as RequesterEmail 
                FROM TrainingRequests r 
                INNER JOIN Users u ON r.RequesterId = u.Id 
                WHERE r.Status = @Status 
                ORDER BY r.CreatedAt DESC";
            return await connection.QueryAsync<TrainingRequest>(sql, new { Status = status });
        }

        public async Task<TrainingRequest> CreateTrainingRequestAsync(TrainingRequest request)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                INSERT INTO TrainingRequests (Title, Department, TrainingType, Status, CreatedAt, RequesterId) 
                VALUES (@Title, @Department, @TrainingType, @Status, @CreatedAt, @RequesterId);
                SELECT LAST_INSERT_ID()";

            var id = await connection.ExecuteScalarAsync<int>(sql, new
            {
                request.Title,
                request.Department,
                request.TrainingType,
                Status = "pending",
                CreatedAt = DateTime.UtcNow,
                request.RequesterId
            });

            request.Id = id;
            request.Status = "pending";
            request.CreatedAt = DateTime.UtcNow;
            return request;
        }

        public async Task<TrainingRequest> UpdateTrainingRequestStatusAsync(int id, string status)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                UPDATE TrainingRequests 
                SET Status = @Status, UpdatedAt = @UpdatedAt 
                WHERE Id = @Id";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                Id = id,
                Status = status,
                UpdatedAt = DateTime.UtcNow
            });

            if (rowsAffected == 0)
                throw new ArgumentException("Training request not found");

            var request = await GetTrainingRequestByIdAsync(id);
            if (request == null)
                throw new ArgumentException("Training request not found");

            request.Status = status;
            request.UpdatedAt = DateTime.UtcNow;
            return request;
        }

        public async Task<bool> DeleteTrainingRequestAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "DELETE FROM TrainingRequests WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }

        // Training Sessions
        public async Task<IEnumerable<TrainingSession>> GetAllTrainingSessionsAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "SELECT * FROM TrainingSessions ORDER BY StartDate";
            return await connection.QueryAsync<TrainingSession>(sql);
        }

        public async Task<TrainingSession?> GetTrainingSessionByIdAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "SELECT * FROM TrainingSessions WHERE Id = @Id";
            return await connection.QuerySingleOrDefaultAsync<TrainingSession>(sql, new { Id = id });
        }

        public async Task<IEnumerable<TrainingSession>> GetTrainingSessionsByRequestAsync(int requestId)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                SELECT s.* FROM TrainingSessions s 
                INNER JOIN TrainingRequests r ON s.Id = r.TrainingSessionId 
                WHERE r.Id = @RequestId 
                ORDER BY s.StartDate";
            return await connection.QueryAsync<TrainingSession>(sql, new { RequestId = requestId });
        }

        public async Task<TrainingSession> CreateTrainingSessionAsync(TrainingSession session)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                INSERT INTO TrainingSessions (Title, StartDate, EndDate, Trainer, Location, Description, Status, MaxParticipants, CurrentParticipants, CreatedAt) 
                VALUES (@Title, @StartDate, @EndDate, @Trainer, @Location, @Description, @Status, @MaxParticipants, @CurrentParticipants, @CreatedAt);
                SELECT LAST_INSERT_ID()";

            var id = await connection.ExecuteScalarAsync<int>(sql, new
            {
                session.Title,
                session.StartDate,
                session.EndDate,
                session.Trainer,
                session.Location,
                session.Description,
                Status = "scheduled",
                session.MaxParticipants,
                CurrentParticipants = 0,
                CreatedAt = DateTime.UtcNow
            });

            session.Id = id;
            session.Status = "scheduled";
            session.CurrentParticipants = 0;
            session.CreatedAt = DateTime.UtcNow;
            return session;
        }

        public async Task<TrainingSession> UpdateTrainingSessionAsync(TrainingSession session)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                UPDATE TrainingSessions 
                SET Title = @Title, StartDate = @StartDate, EndDate = @EndDate, Trainer = @Trainer, 
                    Location = @Location, Description = @Description, Status = @Status, 
                    MaxParticipants = @MaxParticipants, CurrentParticipants = @CurrentParticipants, 
                    UpdatedAt = @UpdatedAt 
                WHERE Id = @Id";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                session.Id,
                session.Title,
                session.StartDate,
                session.EndDate,
                session.Trainer,
                session.Location,
                session.Description,
                session.Status,
                session.MaxParticipants,
                session.CurrentParticipants,
                UpdatedAt = DateTime.UtcNow
            });

            if (rowsAffected == 0)
                throw new ArgumentException("Training session not found");

            session.UpdatedAt = DateTime.UtcNow;
            return session;
        }

        public async Task<bool> DeleteTrainingSessionAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "DELETE FROM TrainingSessions WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }

        // Training Participants
        public async Task<IEnumerable<TrainingParticipant>> GetSessionParticipantsAsync(int sessionId)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                SELECT p.*, u.Name as UserName, u.Email as UserEmail 
                FROM TrainingParticipants p 
                INNER JOIN Users u ON p.UserId = u.Id 
                WHERE p.TrainingSessionId = @SessionId 
                ORDER BY p.RegisteredAt";
            return await connection.QueryAsync<TrainingParticipant>(sql, new { SessionId = sessionId });
        }

        public async Task<TrainingParticipant> RegisterForSessionAsync(int userId, int sessionId)
        {
            using var connection = new MySqlConnection(_connectionString);
            
            // Check if user is already registered
            var existingRegistration = await connection.QuerySingleOrDefaultAsync<TrainingParticipant>(
                "SELECT * FROM TrainingParticipants WHERE UserId = @UserId AND TrainingSessionId = @SessionId",
                new { UserId = userId, SessionId = sessionId });

            if (existingRegistration != null)
                throw new InvalidOperationException("User is already registered for this session");

            // Check if session is full
            var session = await GetTrainingSessionByIdAsync(sessionId);
            if (session == null)
                throw new ArgumentException("Training session not found");

            if (session.CurrentParticipants >= session.MaxParticipants)
                throw new InvalidOperationException("Training session is full");

            var sql = @"
                INSERT INTO TrainingParticipants (UserId, TrainingSessionId, Status, RegisteredAt) 
                VALUES (@UserId, @SessionId, @Status, @RegisteredAt);
                SELECT LAST_INSERT_ID()";

            var id = await connection.ExecuteScalarAsync<int>(sql, new
            {
                UserId = userId,
                SessionId = sessionId,
                Status = "registered",
                RegisteredAt = DateTime.UtcNow
            });

            // Update session participant count
            await connection.ExecuteAsync(
                "UPDATE TrainingSessions SET CurrentParticipants = CurrentParticipants + 1 WHERE Id = @Id",
                new { Id = sessionId });

            return new TrainingParticipant
            {
                Id = id,
                UserId = userId,
                TrainingSessionId = sessionId,
                Status = "registered",
                RegisteredAt = DateTime.UtcNow
            };
        }

        public async Task<bool> UnregisterFromSessionAsync(int userId, int sessionId)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = "DELETE FROM TrainingParticipants WHERE UserId = @UserId AND TrainingSessionId = @SessionId";
            var rowsAffected = await connection.ExecuteAsync(sql, new { UserId = userId, SessionId = sessionId });
            
            if (rowsAffected > 0)
            {
                // Update session participant count
                await connection.ExecuteAsync(
                    "UPDATE TrainingSessions SET CurrentParticipants = CurrentParticipants - 1 WHERE Id = @Id",
                    new { Id = sessionId });
            }
            
            return rowsAffected > 0;
        }

        public async Task<TrainingParticipant> UpdateParticipantStatusAsync(int userId, int sessionId, string status)
        {
            using var connection = new MySqlConnection(_connectionString);
            var sql = @"
                UPDATE TrainingParticipants 
                SET Status = @Status, AttendedAt = @AttendedAt 
                WHERE UserId = @UserId AND TrainingSessionId = @SessionId";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                SessionId = sessionId,
                Status = status,
                AttendedAt = status == "attended" ? DateTime.UtcNow : (DateTime?)null
            });

            if (rowsAffected == 0)
                throw new ArgumentException("Participant registration not found");

            return await connection.QuerySingleOrDefaultAsync<TrainingParticipant>(
                "SELECT * FROM TrainingParticipants WHERE UserId = @UserId AND TrainingSessionId = @SessionId",
                new { UserId = userId, SessionId = sessionId });
        }

        // Dashboard Statistics
        public async Task<object> GetDashboardStatsAsync(string role, int? userId = null)
        {
            using var connection = new MySqlConnection(_connectionString);
            
            var stats = new
            {
                TotalUsers = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Users WHERE IsActive = 1"),
                TotalTrainingRequests = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM TrainingRequests"),
                TotalTrainingSessions = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM TrainingSessions"),
                PendingRequests = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM TrainingRequests WHERE Status = 'pending'"),
                UpcomingSessions = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM TrainingSessions WHERE StartDate > NOW() AND Status = 'scheduled'"),
                ActiveParticipants = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM TrainingParticipants WHERE Status = 'registered'")
            };

            return stats;
        }
    }
} 