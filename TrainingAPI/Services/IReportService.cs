namespace TrainingAPI.Services
{
    public interface IReportService
    {
        Task<IEnumerable<object>> GetTrainingRequestReportAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<object>> GetDepartmentReportAsync();
        Task<object> GetTrainingSessionReportAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<object> GetUserParticipationReportAsync(int? userId = null);
        Task<IEnumerable<object>> GetBudgetReportAsync();
        Task<IEnumerable<object>> GetAttendanceReportAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<object>> GetTrainerPerformanceReportAsync();
    }
} 