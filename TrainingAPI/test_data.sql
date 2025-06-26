-- Complete Test Data for Training Management System
-- Run this after creating the tables with mysql_database_setup.sql

USE abhyas;

-- Clear existing data (optional - remove if you want to keep existing data)
-- DELETE FROM TrainingParticipants;
-- DELETE FROM TrainingSessions;
-- DELETE FROM TrainingRequests;
-- DELETE FROM Users;

-- 1. USERS TABLE - Create users for all roles
INSERT INTO Users (Name, Email, Department, Role, IsActive) VALUES
-- Admin users
('Admin User', 'admin@company.com', 'IT', 'admin', TRUE),
('System Admin', 'system.admin@company.com', 'IT', 'admin', TRUE),

-- L&D users
('Jane Smith', 'jane.smith@company.com', 'HR', 'ld', TRUE),
('Mike Brown', 'mike.brown@company.com', 'Finance', 'ld', TRUE),
('Sarah Wilson', 'sarah.wilson@company.com', 'HR', 'ld', TRUE),

-- Employee users
('John Doe', 'john.doe@company.com', 'Engineering', 'employee', TRUE),
('Bob Wilson', 'bob.wilson@company.com', 'Marketing', 'employee', TRUE),
('Alice Johnson', 'alice.johnson@company.com', 'Sales', 'employee', TRUE),
('David Chen', 'david.chen@company.com', 'Engineering', 'employee', TRUE),
('Lisa Garcia', 'lisa.garcia@company.com', 'Marketing', 'employee', TRUE),
('Tom Anderson', 'tom.anderson@company.com', 'Sales', 'employee', TRUE),
('Emma Davis', 'emma.davis@company.com', 'Engineering', 'employee', TRUE),
('James Wilson', 'james.wilson@company.com', 'Finance', 'employee', TRUE);

-- 2. TRAINING REQUESTS TABLE - Various statuses and types
INSERT INTO TrainingRequests (Title, Department, TrainingType, Priority, ExpectedParticipants, Duration, PreferredDate, Description, BusinessJustification, Budget, Status, RequesterId) VALUES
-- Approved requests
('React Advanced Concepts', 'Engineering', 'Technical', 'high', 15, '3 days', '2024-06-20', 'Advanced React training covering hooks, context, and performance optimization', 'Improve team skills for upcoming React projects', 5000.00, 'approved', 6),
('Cloud Computing Basics', 'Engineering', 'Technical', 'medium', 20, '2 days', '2024-06-18', 'Introduction to AWS, Azure, and cloud deployment strategies', 'Prepare team for cloud migration project', 3000.00, 'approved', 6),
('Project Management Fundamentals', 'Engineering', 'Soft Skills', 'medium', 12, '1 day', '2024-06-15', 'Basic project management principles and tools', 'Improve project delivery efficiency', 2000.00, 'approved', 6),

-- Pending requests
('DevOps Fundamentals', 'Engineering', 'Technical', 'high', 18, '2 days', '2024-07-02', 'CI/CD pipelines, Docker, and Kubernetes basics', 'Streamline development and deployment processes', 4000.00, 'pending', 9),
('Sales Techniques Training', 'Sales', 'Sales', 'high', 15, '1 day', '2024-07-05', 'Advanced sales techniques and customer relationship management', 'Increase sales performance and customer satisfaction', 2500.00, 'pending', 8),
('Leadership Skills Workshop', 'HR', 'Soft Skills', 'medium', 25, '2 days', '2024-07-10', 'Leadership development for middle management', 'Develop leadership pipeline and improve team management', 3500.00, 'pending', 3),

-- Rejected requests
('Advanced Machine Learning', 'Engineering', 'Technical', 'urgent', 10, '5 days', '2024-06-25', 'Deep learning and neural networks training', 'Prepare for AI project implementation', 8000.00, 'rejected', 6),
('Blockchain Development', 'Engineering', 'Technical', 'high', 8, '3 days', '2024-06-28', 'Blockchain fundamentals and smart contract development', 'Explore blockchain technology for future projects', 6000.00, 'rejected', 9),

-- Completed requests
('Git Version Control', 'Engineering', 'Technical', 'medium', 30, '1 day', '2024-05-15', 'Git basics, branching strategies, and collaboration', 'Standardize version control practices', 1500.00, 'completed', 6),
('Communication Skills', 'Marketing', 'Soft Skills', 'medium', 20, '1 day', '2024-05-20', 'Effective communication and presentation skills', 'Improve team communication and client presentations', 2000.00, 'completed', 10);

-- 3. TRAINING SESSIONS TABLE - Scheduled sessions for upcoming trainings
INSERT INTO TrainingSessions (Title, StartDate, EndDate, Trainer, Location, Description, Status, MaxParticipants, CurrentParticipants, TrainingRequestId) VALUES
-- Upcoming sessions (next 30 days)
('React Advanced Workshop', '2024-06-28 10:00:00', '2024-06-30 17:00:00', 'Sarah Johnson', 'Conference Room A', 'Advanced React concepts and best practices', 'scheduled', 15, 8, 1),
('DevOps Fundamentals', '2024-07-02 14:00:00', '2024-07-03 17:00:00', 'David Chen', 'Training Room B', 'CI/CD, Docker, and Kubernetes basics', 'scheduled', 18, 12, 4),
('Sales Techniques Training', '2024-07-05 09:00:00', '2024-07-05 17:00:00', 'Lisa Garcia', 'Conference Room C', 'Advanced sales techniques and CRM', 'scheduled', 15, 10, 5),
('Leadership Skills Workshop', '2024-07-10 09:00:00', '2024-07-11 17:00:00', 'Mike Brown', 'Training Room A', 'Leadership development for managers', 'scheduled', 25, 15, 6),

-- Past completed sessions
('Git Version Control Workshop', '2024-05-15 09:00:00', '2024-05-15 17:00:00', 'John Doe', 'Conference Room B', 'Git basics and collaboration', 'completed', 30, 25, 9),
('Communication Skills Training', '2024-05-20 10:00:00', '2024-05-20 16:00:00', 'Jane Smith', 'Training Room C', 'Effective communication techniques', 'completed', 20, 18, 10),

-- In-progress sessions
('Cloud Computing Basics', '2024-06-18 09:00:00', '2024-06-19 17:00:00', 'Tom Anderson', 'Conference Room A', 'AWS and Azure fundamentals', 'in-progress', 20, 20, 2),
('Project Management Fundamentals', '2024-06-15 09:00:00', '2024-06-15 17:00:00', 'Emma Davis', 'Training Room B', 'Project management principles', 'in-progress', 12, 12, 3);

-- 4. TRAINING PARTICIPANTS TABLE - User registrations
INSERT INTO TrainingParticipants (UserId, TrainingSessionId, Status, RegisteredAt) VALUES
-- React Advanced Workshop participants
(6, 1, 'registered', '2024-06-20 10:00:00'), -- John Doe
(9, 1, 'registered', '2024-06-20 11:00:00'), -- David Chen
(12, 1, 'registered', '2024-06-20 14:00:00'), -- Emma Davis
(7, 1, 'registered', '2024-06-21 09:00:00'), -- Bob Wilson
(8, 1, 'registered', '2024-06-21 10:00:00'), -- Alice Johnson
(10, 1, 'registered', '2024-06-21 11:00:00'), -- Lisa Garcia
(11, 1, 'registered', '2024-06-21 14:00:00'), -- Tom Anderson
(13, 1, 'registered', '2024-06-22 09:00:00'), -- James Wilson

-- DevOps Fundamentals participants
(6, 2, 'registered', '2024-06-25 10:00:00'), -- John Doe
(9, 2, 'registered', '2024-06-25 11:00:00'), -- David Chen
(12, 2, 'registered', '2024-06-25 14:00:00'), -- Emma Davis
(7, 2, 'registered', '2024-06-26 09:00:00'), -- Bob Wilson
(8, 2, 'registered', '2024-06-26 10:00:00'), -- Alice Johnson
(10, 2, 'registered', '2024-06-26 11:00:00'), -- Lisa Garcia
(11, 2, 'registered', '2024-06-26 14:00:00'), -- Tom Anderson
(13, 2, 'registered', '2024-06-27 09:00:00'), -- James Wilson
(3, 2, 'registered', '2024-06-27 10:00:00'), -- Jane Smith
(4, 2, 'registered', '2024-06-27 11:00:00'), -- Mike Brown
(5, 2, 'registered', '2024-06-27 14:00:00'), -- Sarah Wilson

-- Sales Techniques participants
(8, 3, 'registered', '2024-06-28 10:00:00'), -- Alice Johnson
(11, 3, 'registered', '2024-06-28 11:00:00'), -- Tom Anderson
(7, 3, 'registered', '2024-06-28 14:00:00'), -- Bob Wilson
(10, 3, 'registered', '2024-06-29 09:00:00'), -- Lisa Garcia
(13, 3, 'registered', '2024-06-29 10:00:00'), -- James Wilson
(6, 3, 'registered', '2024-06-29 11:00:00'), -- John Doe
(9, 3, 'registered', '2024-06-29 14:00:00'), -- David Chen
(12, 3, 'registered', '2024-06-30 09:00:00'), -- Emma Davis

-- Leadership Skills participants
(3, 4, 'registered', '2024-07-01 10:00:00'), -- Jane Smith
(4, 4, 'registered', '2024-07-01 11:00:00'), -- Mike Brown
(5, 4, 'registered', '2024-07-01 14:00:00'), -- Sarah Wilson
(6, 4, 'registered', '2024-07-02 09:00:00'), -- John Doe
(9, 4, 'registered', '2024-07-02 10:00:00'), -- David Chen
(12, 4, 'registered', '2024-07-02 11:00:00'), -- Emma Davis
(7, 4, 'registered', '2024-07-02 14:00:00'), -- Bob Wilson
(8, 4, 'registered', '2024-07-03 09:00:00'), -- Alice Johnson
(10, 4, 'registered', '2024-07-03 10:00:00'), -- Lisa Garcia
(11, 4, 'registered', '2024-07-03 11:00:00'), -- Tom Anderson
(13, 4, 'registered', '2024-07-03 14:00:00'), -- James Wilson

-- Completed sessions participants (with attendance)
(6, 5, 'attended', '2024-05-10 10:00:00'), -- John Doe
(9, 5, 'attended', '2024-05-10 11:00:00'), -- David Chen
(12, 5, 'attended', '2024-05-10 14:00:00'), -- Emma Davis
(7, 5, 'attended', '2024-05-11 09:00:00'), -- Bob Wilson
(8, 5, 'attended', '2024-05-11 10:00:00'), -- Alice Johnson
(10, 5, 'attended', '2024-05-11 11:00:00'), -- Lisa Garcia
(11, 5, 'attended', '2024-05-11 14:00:00'), -- Tom Anderson
(13, 5, 'attended', '2024-05-12 09:00:00'), -- James Wilson

-- Communication Skills participants
(7, 6, 'attended', '2024-05-15 10:00:00'), -- Bob Wilson
(8, 6, 'attended', '2024-05-15 11:00:00'), -- Alice Johnson
(10, 6, 'attended', '2024-05-15 14:00:00'), -- Lisa Garcia
(11, 6, 'attended', '2024-05-16 09:00:00'), -- Tom Anderson
(13, 6, 'attended', '2024-05-16 10:00:00'), -- James Wilson
(6, 6, 'attended', '2024-05-16 11:00:00'), -- John Doe
(9, 6, 'attended', '2024-05-16 14:00:00'), -- David Chen
(12, 6, 'attended', '2024-05-17 09:00:00'); -- Emma Davis

-- Update participant counts for sessions
UPDATE TrainingSessions SET CurrentParticipants = (
  SELECT COUNT(*) FROM TrainingParticipants WHERE TrainingSessionId = TrainingSessions.Id
);

SELECT 'Test data inserted successfully!' as Status; 