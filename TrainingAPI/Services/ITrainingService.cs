using TrainingAPI.Models;

namespace TrainingAPI.Services
{
    public interface ITrainingService
    {
        Task<IEnumerable<TrainingRequest>> GetAllTrainingRequestsAsync();
        Task<TrainingRequest?> GetTrainingRequestByIdAsync(int id);
        Task<IEnumerable<TrainingRequest>> GetTrainingRequestsByUserAsync(int userId);
        Task<IEnumerable<TrainingRequest>> GetTrainingRequestsByStatusAsync(string status);
        Task<TrainingRequest> CreateTrainingRequestAsync(TrainingRequest request);
        Task<TrainingRequest> UpdateTrainingRequestStatusAsync(int id, string status);
        Task<bool> DeleteTrainingRequestAsync(int id);
        
        Task<IEnumerable<TrainingSession>> GetAllTrainingSessionsAsync();
        Task<TrainingSession?> GetTrainingSessionByIdAsync(int id);
        Task<IEnumerable<TrainingSession>> GetTrainingSessionsByRequestAsync(int requestId);
        Task<TrainingSession> CreateTrainingSessionAsync(TrainingSession session);
        Task<TrainingSession> UpdateTrainingSessionAsync(TrainingSession session);
        Task<bool> DeleteTrainingSessionAsync(int id);
        
        Task<IEnumerable<TrainingParticipant>> GetSessionParticipantsAsync(int sessionId);
        Task<TrainingParticipant> RegisterForSessionAsync(int userId, int sessionId);
        Task<bool> UnregisterFromSessionAsync(int userId, int sessionId);
        Task<TrainingParticipant> UpdateParticipantStatusAsync(int userId, int sessionId, string status);
        
        Task<object> GetDashboardStatsAsync(string role, int? userId = null);
        Task<IEnumerable<TrainingSession>> GetRegisteredSessionsByUserAsync(int userId);
        Task<IEnumerable<TrainingSession>> GetSessionsAsTrainerAsync(int userId);
        Task<TrainingSession?> UpdateTrainerRequestStatusAsync(int sessionId, string status);
        Task<TrainingRequest> UpdateTrainingRequestTrainerStatusAsync(int id, string trainerStatus);
        Task<TrainingRequest> UpdateTrainingRequestAsync(TrainingRequest request);
        Task<TrainingRequest> UpdateTrainingRequestSessionAsync(int id, int trainingSessionId);

    }
} 