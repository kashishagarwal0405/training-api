using TrainingAPI.Models;

namespace TrainingAPI.Services
{
    public class TrainingService : ITrainingService
    {
        private readonly string _requestsFile = "MockData/trainingrequests.json";
        private readonly string _sessionsFile = "MockData/trainingsessions.json";
        private readonly string _participantsFile = "MockData/trainingparticipants.json";
        private readonly ILogger<TrainingService> _logger;

        public TrainingService(ILogger<TrainingService> logger)
        {
            _logger = logger;
        }

        // Training Requests
        public async Task<IEnumerable<TrainingRequest>> GetAllTrainingRequestsAsync()
        {
            return await JsonFileHelper.ReadListAsync<TrainingRequest>(_requestsFile);
        }

        public async Task<TrainingRequest?> GetTrainingRequestByIdAsync(int id)
        {
            var requests = await JsonFileHelper.ReadListAsync<TrainingRequest>(_requestsFile);
            return requests.FirstOrDefault(r => r.Id == id);
        }

        public async Task<IEnumerable<TrainingRequest>> GetTrainingRequestsByUserAsync(int userId)
        {
            var requests = await JsonFileHelper.ReadListAsync<TrainingRequest>(_requestsFile);
            return requests.Where(r => r.RequesterId == userId);
        }

        public async Task<IEnumerable<TrainingRequest>> GetTrainingRequestsByStatusAsync(string status)
        {
            var requests = await JsonFileHelper.ReadListAsync<TrainingRequest>(_requestsFile);
            return requests.Where(r => r.Status == status);
        }

        public async Task<TrainingRequest> CreateTrainingRequestAsync(TrainingRequest request)
        {
            var requests = await JsonFileHelper.ReadListAsync<TrainingRequest>(_requestsFile);
            request.Id = requests.Any() ? requests.Max(r => r.Id) + 1 : 1;
            request.Status = "pending";
            request.CreatedAt = DateTime.UtcNow;
            requests.Add(request);
            await JsonFileHelper.WriteListAsync(_requestsFile, requests);
            return request;
        }

        public async Task<TrainingRequest> UpdateTrainingRequestStatusAsync(int id, string status)
        {
            var requests = await JsonFileHelper.ReadListAsync<TrainingRequest>(_requestsFile);
            var idx = requests.FindIndex(r => r.Id == id);
            if (idx == -1) throw new ArgumentException("Training request not found");
            requests[idx].Status = status;
            requests[idx].UpdatedAt = DateTime.UtcNow;
            await JsonFileHelper.WriteListAsync(_requestsFile, requests);
            return requests[idx];
        }

        public async Task<bool> DeleteTrainingRequestAsync(int id)
        {
            var requests = await JsonFileHelper.ReadListAsync<TrainingRequest>(_requestsFile);
            var req = requests.FirstOrDefault(r => r.Id == id);
            if (req == null) return false;
            requests.Remove(req);
            await JsonFileHelper.WriteListAsync(_requestsFile, requests);
            return true;
        }

        // Training Sessions
        public async Task<IEnumerable<TrainingSession>> GetAllTrainingSessionsAsync()
        {
            return await JsonFileHelper.ReadListAsync<TrainingSession>(_sessionsFile);
        }

        public async Task<TrainingSession?> GetTrainingSessionByIdAsync(int id)
        {
            var sessions = await JsonFileHelper.ReadListAsync<TrainingSession>(_sessionsFile);
            return sessions.FirstOrDefault(s => s.Id == id);
        }

        public async Task<IEnumerable<TrainingSession>> GetTrainingSessionsByRequestAsync(int requestId)
        {
            var sessions = await JsonFileHelper.ReadListAsync<TrainingSession>(_sessionsFile);
            var requests = await JsonFileHelper.ReadListAsync<TrainingRequest>(_requestsFile);
            var sessionIds = requests.Where(r => r.Id == requestId && r.TrainingSessionId.HasValue).Select(r => r.TrainingSessionId.Value);
            return sessions.Where(s => sessionIds.Contains(s.Id));
        }

        public async Task<TrainingSession> CreateTrainingSessionAsync(TrainingSession session)
        {
            var sessions = await JsonFileHelper.ReadListAsync<TrainingSession>(_sessionsFile);
            session.Id = sessions.Any() ? sessions.Max(s => s.Id) + 1 : 1;
            session.Status = "scheduled";
            session.CurrentParticipants = 0;
            session.CreatedAt = DateTime.UtcNow;
            sessions.Add(session);
            await JsonFileHelper.WriteListAsync(_sessionsFile, sessions);
            return session;
        }

        public async Task<TrainingSession> UpdateTrainingSessionAsync(TrainingSession session)
        {
            var sessions = await JsonFileHelper.ReadListAsync<TrainingSession>(_sessionsFile);
            var idx = sessions.FindIndex(s => s.Id == session.Id);
            if (idx == -1) throw new ArgumentException("Session not found");
            session.UpdatedAt = DateTime.UtcNow;
            sessions[idx] = session;
            await JsonFileHelper.WriteListAsync(_sessionsFile, sessions);
            return session;
        }

        public async Task<bool> DeleteTrainingSessionAsync(int id)
        {
            var sessions = await JsonFileHelper.ReadListAsync<TrainingSession>(_sessionsFile);
            var session = sessions.FirstOrDefault(s => s.Id == id);
            if (session == null) return false;
            sessions.Remove(session);
            await JsonFileHelper.WriteListAsync(_sessionsFile, sessions);
            return true;
        }

        // Participants
        public async Task<IEnumerable<TrainingParticipant>> GetSessionParticipantsAsync(int sessionId)
        {
            var participants = await JsonFileHelper.ReadListAsync<TrainingParticipant>(_participantsFile);
            return participants.Where(p => p.TrainingSessionId == sessionId);
        }

        public async Task<TrainingParticipant> RegisterForSessionAsync(int userId, int sessionId)
        {
            var participants = await JsonFileHelper.ReadListAsync<TrainingParticipant>(_participantsFile);
            var participant = new TrainingParticipant
            {
                Id = participants.Any() ? participants.Max(p => p.Id) + 1 : 1,
                UserId = userId,
                TrainingSessionId = sessionId,
                Status = "registered",
                RegisteredAt = DateTime.UtcNow
            };
            participants.Add(participant);
            await JsonFileHelper.WriteListAsync(_participantsFile, participants);
            return participant;
        }

        public async Task<bool> UnregisterFromSessionAsync(int userId, int sessionId)
        {
            var participants = await JsonFileHelper.ReadListAsync<TrainingParticipant>(_participantsFile);
            var participant = participants.FirstOrDefault(p => p.UserId == userId && p.TrainingSessionId == sessionId);
            if (participant == null) return false;
            participants.Remove(participant);
            await JsonFileHelper.WriteListAsync(_participantsFile, participants);
            return true;
        }

        public async Task<TrainingParticipant> UpdateParticipantStatusAsync(int userId, int sessionId, string status)
        {
            var participants = await JsonFileHelper.ReadListAsync<TrainingParticipant>(_participantsFile);
            var idx = participants.FindIndex(p => p.UserId == userId && p.TrainingSessionId == sessionId);
            if (idx == -1) throw new ArgumentException("Participant not found");
            participants[idx].Status = status;
            if (status == "attended")
                participants[idx].AttendedAt = DateTime.UtcNow;
            await JsonFileHelper.WriteListAsync(_participantsFile, participants);
            return participants[idx];
        }

        public async Task<object> GetDashboardStatsAsync(string role, int? userId = null)
        {
            var users = await JsonFileHelper.ReadListAsync<User>("MockData/users.json");
            var requests = await JsonFileHelper.ReadListAsync<TrainingRequest>("MockData/trainingrequests.json");
            var sessions = await JsonFileHelper.ReadListAsync<TrainingSession>("MockData/trainingsessions.json");
            var participants = await JsonFileHelper.ReadListAsync<TrainingParticipant>("MockData/trainingparticipants.json");

            if (role != null && role.ToLower().Contains("employee") && userId.HasValue)
            {
                // Employee-specific stats
                var myRequests = requests.Where(r => r.RequesterId == userId.Value).ToList();
                var myParticipants = participants.Where(p => p.UserId == userId.Value).ToList();
                var mySessionsRegistered = myParticipants.Count;
                var mySessionsAttended = myParticipants.Count(p => p.Status == "attended");
                var myUpcomingSessions = myParticipants
                    .Join(sessions, p => p.TrainingSessionId, s => s.Id, (p, s) => new { p, s })
                    .Count(x => x.s.StartDate > DateTime.UtcNow && x.s.Status == "scheduled");

                return new
                {
                    MyTrainingRequests = myRequests.Count,
                    MySessionsRegistered = mySessionsRegistered,
                    MySessionsAttended = mySessionsAttended,
                    MyUpcomingSessions = myUpcomingSessions
                };
            }
            else if (role != null && (role.ToLower().Contains("l&d") || role.ToLower().Contains("ld")))
            {
                // L&D department: overall stats
                return new
                {
                    TotalUsers = users.Count(u => u.IsActive),
                    TotalTrainingRequests = requests.Count(),
                    TotalTrainingSessions = sessions.Count(),
                    PendingRequests = requests.Count(r => r.Status == "pending"),
                    UpcomingSessions = sessions.Count(s => s.StartDate > DateTime.UtcNow && s.Status == "scheduled"),
                    ActiveParticipants = participants.Count(p => p.Status == "registered")
                };
            }
            else
            {
                // Default: overall stats
                return new
                {
                    TotalUsers = users.Count(u => u.IsActive),
                    TotalTrainingRequests = requests.Count(),
                    TotalTrainingSessions = sessions.Count(),
                    PendingRequests = requests.Count(r => r.Status == "pending"),
                    UpcomingSessions = sessions.Count(s => s.StartDate > DateTime.UtcNow && s.Status == "scheduled"),
                    ActiveParticipants = participants.Count(p => p.Status == "registered")
                };
            }
        }

        public async Task<IEnumerable<TrainingSession>> GetRegisteredSessionsByUserAsync(int userId)
        {
            var participants = await JsonFileHelper.ReadListAsync<TrainingParticipant>(_participantsFile);
            var sessions = await JsonFileHelper.ReadListAsync<TrainingSession>(_sessionsFile);
            var registeredSessionIds = participants
                .Where(p => p.UserId == userId && (p.Status == "registered" || p.Status == "attended"))
                .Select(p => p.TrainingSessionId)
                .Distinct()
                .ToList();
            return sessions.Where(s => registeredSessionIds.Contains(s.Id));
        }
    }
} 