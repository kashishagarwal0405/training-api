using TrainingAPI.Models;

namespace TrainingAPI.Services
{
    public class ReportService : IReportService
    {
        private readonly string _requestsFile = "MockData/trainingrequests.json";
        private readonly string _usersFile = "MockData/users.json";
        private readonly string _sessionsFile = "MockData/trainingsessions.json";
        private readonly string _participantsFile = "MockData/trainingparticipants.json";
        private readonly string _attendanceFile = "MockData/attendance.json";
        private readonly ILogger<ReportService> _logger;

        public ReportService(ILogger<ReportService> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<object>> GetTrainingRequestReportAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var requests = await JsonFileHelper.ReadListAsync<TrainingRequest>(_requestsFile);
            var filtered = requests
                .Where(r => (!startDate.HasValue || r.CreatedAt >= startDate) && (!endDate.HasValue || r.CreatedAt <= endDate))
                .GroupBy(r => new { r.Status, r.Department, r.TrainingType, Month = r.CreatedAt.ToString("yyyy-MM") })
                .Select(g => new {
                    TotalRequests = g.Count(),
                    g.Key.Status,
                    g.Key.Department,
                    g.Key.TrainingType,
                    g.Key.Month
                });
            return filtered.ToList();
        }

        public async Task<IEnumerable<object>> GetDepartmentReportAsync()
        {
            var users = await JsonFileHelper.ReadListAsync<User>(_usersFile);
            var requests = await JsonFileHelper.ReadListAsync<TrainingRequest>(_requestsFile);
            var report = users.GroupBy(u => u.Department)
                .Select(g => new {
                    Department = g.Key,
                    UserCount = g.Count(),
                    RequestCount = requests.Count(r => r.Department == g.Key),
                    ActiveUsers = g.Count(u => u.IsActive),
                    TrainingTypes = requests.Where(r => r.Department == g.Key).Select(r => r.TrainingType).Distinct().Count()
                });
            return report.ToList();
        }

        public async Task<IEnumerable<object>> GetBudgetReportAsync()
        {
            var requests = await JsonFileHelper.ReadListAsync<TrainingRequest>(_requestsFile);
            var report = requests.GroupBy(r => new { r.Department, r.Status })
                .Select(g => new {
                    Message = "Budget tracking not available in current schema",
                    TotalRequests = g.Count(),
                    g.Key.Department,
                    g.Key.Status
                });
            return report.ToList();
        }

        public async Task<object> GetTrainingSessionReportAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var sessions = await JsonFileHelper.ReadListAsync<TrainingSession>(_sessionsFile);
            var filtered = sessions
                .Where(s => (!startDate.HasValue || s.StartDate >= startDate) && (!endDate.HasValue || s.EndDate <= endDate))
                .GroupBy(s => new { s.Status, s.Trainer })
                .Select(g => new {
                    TotalSessions = g.Count(),
                    g.Key.Status,
                    g.Key.Trainer,
                    AverageParticipants = g.Average(s => s.CurrentParticipants),
                    TotalParticipants = g.Sum(s => s.CurrentParticipants),
                    UpcomingSessions = g.Count(s => s.StartDate > DateTime.Now),
                    CompletedSessions = g.Count(s => s.EndDate < DateTime.Now)
                });
            return filtered.ToList();
        }

        public async Task<object> GetUserParticipationReportAsync(int? userId = null)
        {
            var participants = await JsonFileHelper.ReadListAsync<TrainingParticipant>(_participantsFile);
            var users = await JsonFileHelper.ReadListAsync<User>(_usersFile);
            var filtered = participants
                .Where(p => !userId.HasValue || p.UserId == userId)
                .GroupBy(p => new { p.Status, p.UserId })
                .Select(g => new {
                    TotalRegistrations = g.Count(),
                    g.Key.Status,
                    UserId = g.Key.UserId,
                    UserName = users.FirstOrDefault(u => u.Id == g.Key.UserId)?.Name,
                    Department = users.FirstOrDefault(u => u.Id == g.Key.UserId)?.Department,
                    RegistrationCount = g.Count(),
                    AttendedCount = g.Count(p => p.Status == "attended"),
                    RegisteredCount = g.Count(p => p.Status == "registered")
                });
            return filtered.ToList();
        }

        public async Task<IEnumerable<object>> GetAttendanceReportAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var sessions = await JsonFileHelper.ReadListAsync<TrainingSession>(_sessionsFile);
            var attendance = await JsonFileHelper.ReadListAsync<Attendance>(_attendanceFile);
            var filtered = sessions
                .Where(ts => (!startDate.HasValue || ts.StartDate >= startDate) && (!endDate.HasValue || ts.EndDate <= endDate))
                .GroupJoin(attendance, ts => ts.Id, a => a.TrainingSessionId, (ts, aList) => new { ts, aList })
                .Select(x => new {
                    SessionTitle = x.ts.Title,
                    x.ts.Trainer,
                    TotalAttendance = x.aList.Count(),
                    PresentCount = x.aList.Count(a => a.Status == "present"),
                    AbsentCount = x.aList.Count(a => a.Status == "absent"),
                    AttendanceRate = x.aList.Any() ? Math.Round(x.aList.Count(a => a.Status == "present") * 100.0 / x.aList.Count(), 2) : 0
                });
            return filtered.ToList();
        }

        public async Task<IEnumerable<object>> GetTrainerPerformanceReportAsync()
        {
            var sessions = await JsonFileHelper.ReadListAsync<TrainingSession>(_sessionsFile);
            var attendance = await JsonFileHelper.ReadListAsync<Attendance>(_attendanceFile);
            var report = sessions.GroupBy(s => s.Trainer)
                .Select(g => new {
                    Trainer = g.Key,
                    TotalSessions = g.Count(),
                    AverageAttendance = g.Average(s => attendance.Count(a => a.TrainingSessionId == s.Id && a.Status == "present")),
                    TotalAttendance = g.Sum(s => attendance.Count(a => a.TrainingSessionId == s.Id && a.Status == "present"))
                });
            return report.ToList();
        }
    }
} 
 