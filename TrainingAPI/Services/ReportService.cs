using Dapper;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TrainingAPI.Services
{
    public class ReportService : IReportService
    {
        private readonly string _connectionString;
        private readonly ILogger<ReportService> _logger;

        public ReportService(IConfiguration configuration, ILogger<ReportService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public async Task<IEnumerable<object>> GetTrainingRequestReportAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                var sql = @"
                    SELECT 
                        COUNT(*) as TotalRequests,
                        Status,
                        Department,
                        TrainingType,
                        DATE_FORMAT(CreatedAt, '%Y-%m') as Month
                    FROM TrainingRequests 
                    WHERE (@StartDate IS NULL OR CreatedAt >= @StartDate)
                      AND (@EndDate IS NULL OR CreatedAt <= @EndDate)
                    GROUP BY Status, Department, TrainingType, DATE_FORMAT(CreatedAt, '%Y-%m')
                    ORDER BY Month";

                return await connection.QueryAsync<object>(sql, new
                {
                    StartDate = startDate,
                    EndDate = endDate
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting training request report");
                throw;
            }
        }

        public async Task<IEnumerable<object>> GetDepartmentReportAsync()
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                var sql = @"
                    SELECT 
                        u.Department,
                        COUNT(u.Id) as UserCount,
                        COUNT(r.Id) as RequestCount,
                        COUNT(CASE WHEN u.IsActive = 1 THEN 1 END) as ActiveUsers,
                        COUNT(DISTINCT r.TrainingType) as TrainingTypes
                    FROM Users u
                    LEFT JOIN TrainingRequests r ON u.Department = r.Department
                    GROUP BY u.Department
                    ORDER BY u.Department";

                return await connection.QueryAsync<object>(sql);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting department report");
                throw;
            }
        }

        public async Task<IEnumerable<object>> GetBudgetReportAsync()
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                // Since Budget field was removed, we'll provide a placeholder report
                // or you can remove this method entirely if budget tracking is not needed
                var sql = @"
                    SELECT 
                        'Budget tracking not available in current schema' as Message,
                        COUNT(*) as TotalRequests,
                        Department,
                        Status
                    FROM TrainingRequests 
                    GROUP BY Department, Status";

                return await connection.QueryAsync<object>(sql);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting budget report");
                throw;
            }
        }

        public async Task<object> GetTrainingSessionReportAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                var sql = @"
                    SELECT 
                        COUNT(*) as TotalSessions,
                        Status,
                        Trainer,
                        AVG(CAST(CurrentParticipants AS DECIMAL(10,2))) as AverageParticipants,
                        SUM(CurrentParticipants) as TotalParticipants,
                        COUNT(CASE WHEN StartDate > NOW() THEN 1 END) as UpcomingSessions,
                        COUNT(CASE WHEN EndDate < NOW() THEN 1 END) as CompletedSessions
                    FROM TrainingSessions 
                    WHERE (@StartDate IS NULL OR StartDate >= @StartDate)
                      AND (@EndDate IS NULL OR EndDate <= @EndDate)
                    GROUP BY Status, Trainer";

                var results = await connection.QueryAsync<object>(sql, new
                {
                    StartDate = startDate,
                    EndDate = endDate
                });

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting training session report");
                throw;
            }
        }

        public async Task<object> GetUserParticipationReportAsync(int? userId = null)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                var sql = @"
                    SELECT 
                        COUNT(*) as TotalRegistrations,
                        p.Status,
                        u.Id as UserId,
                        u.Name as UserName,
                        u.Department,
                        COUNT(*) as RegistrationCount,
                        COUNT(CASE WHEN p.Status = 'attended' THEN 1 END) as AttendedCount,
                        COUNT(CASE WHEN p.Status = 'registered' THEN 1 END) as RegisteredCount
                    FROM TrainingParticipants p
                    INNER JOIN Users u ON p.UserId = u.Id
                    WHERE (@UserId IS NULL OR p.UserId = @UserId)
                    GROUP BY p.Status, u.Id, u.Name, u.Department
                    ORDER BY RegistrationCount DESC";

                var results = await connection.QueryAsync<object>(sql, new
                {
                    UserId = userId
                });

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user participation report");
                throw;
            }
        }

        // New method to get attendance report
        public async Task<IEnumerable<object>> GetAttendanceReportAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                var sql = @"
                    SELECT 
                        ts.Title as SessionTitle,
                        ts.Trainer,
                        COUNT(a.Id) as TotalAttendance,
                        COUNT(CASE WHEN a.Status = 'present' THEN 1 END) as PresentCount,
                        COUNT(CASE WHEN a.Status = 'absent' THEN 1 END) as AbsentCount,
                        ROUND((COUNT(CASE WHEN a.Status = 'present' THEN 1 END) * 100.0 / COUNT(a.Id)), 2) as AttendanceRate
                    FROM TrainingSessions ts
                    LEFT JOIN Attendance a ON ts.Id = a.TrainingSessionId
                    WHERE (@StartDate IS NULL OR ts.StartDate >= @StartDate)
                      AND (@EndDate IS NULL OR ts.EndDate <= @EndDate)
                    GROUP BY ts.Id, ts.Title, ts.Trainer
                    ORDER BY ts.StartDate DESC";

                return await connection.QueryAsync<object>(sql, new
                {
                    StartDate = startDate,
                    EndDate = endDate
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting attendance report");
                throw;
            }
        }

        // New method to get trainer performance report
        public async Task<IEnumerable<object>> GetTrainerPerformanceReportAsync()
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                var sql = @"
                    SELECT 
                        ts.Trainer,
                        COUNT(ts.Id) as TotalSessions,
                        COUNT(CASE WHEN ts.Status = 'completed' THEN 1 END) as CompletedSessions,
                        AVG(ts.CurrentParticipants) as AverageParticipants,
                        SUM(ts.CurrentParticipants) as TotalParticipants,
                        COUNT(DISTINCT ts.Id) as UniqueSessions
                    FROM TrainingSessions ts
                    GROUP BY ts.Trainer
                    ORDER BY TotalSessions DESC";

                return await connection.QueryAsync<object>(sql);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting trainer performance report");
                throw;
            }
        }
    }
} 
 