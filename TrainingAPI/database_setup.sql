-- Training Management System Database Setup
-- Run this script in SQL Server Management Studio or Azure Data Studio

USE TrainingManagementDB;
GO

-- Create Users table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Department NVARCHAR(50) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('employee', 'ld', 'admin')),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    IsActive BIT DEFAULT 1
);

-- Create TrainingRequests table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TrainingRequests' AND xtype='U')
CREATE TABLE TrainingRequests (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Department NVARCHAR(50) NOT NULL,
    TrainingType NVARCHAR(50) NOT NULL,
    Priority NVARCHAR(20) NOT NULL CHECK (Priority IN ('low', 'medium', 'high', 'urgent')),
    ExpectedParticipants INT NOT NULL,
    Duration NVARCHAR(50) NOT NULL,
    PreferredDate DATE NOT NULL,
    Description NVARCHAR(MAX),
    BusinessJustification NVARCHAR(MAX),
    Budget DECIMAL(10,2),
    Status NVARCHAR(20) DEFAULT 'pending' CHECK (Status IN ('pending', 'approved', 'rejected', 'completed')),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    RequesterId INT NOT NULL,
    FOREIGN KEY (RequesterId) REFERENCES Users(Id)
);

-- Create TrainingSessions table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TrainingSessions' AND xtype='U')
CREATE TABLE TrainingSessions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NOT NULL,
    Trainer NVARCHAR(100) NOT NULL,
    Location NVARCHAR(100),
    Description NVARCHAR(MAX),
    Status NVARCHAR(20) DEFAULT 'scheduled' CHECK (Status IN ('scheduled', 'in-progress', 'completed', 'cancelled')),
    MaxParticipants INT NOT NULL,
    CurrentParticipants INT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    TrainingRequestId INT NOT NULL,
    FOREIGN KEY (TrainingRequestId) REFERENCES TrainingRequests(Id)
);

-- Create TrainingParticipants table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TrainingParticipants' AND xtype='U')
CREATE TABLE TrainingParticipants (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    TrainingSessionId INT NOT NULL,
    Status NVARCHAR(20) DEFAULT 'registered' CHECK (Status IN ('registered', 'attended', 'no-show', 'cancelled')),
    RegisteredAt DATETIME2 DEFAULT GETUTCDATE(),
    AttendedAt DATETIME2,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (TrainingSessionId) REFERENCES TrainingSessions(Id),
    UNIQUE(UserId, TrainingSessionId)
);

-- Insert sample data
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'admin@company.com')
BEGIN
    INSERT INTO Users (Name, Email, Department, Role) VALUES
    ('Admin User', 'admin@company.com', 'IT', 'admin'),
    ('John Doe', 'john.doe@company.com', 'Engineering', 'employee'),
    ('Jane Smith', 'jane.smith@company.com', 'HR', 'ld'),
    ('Bob Wilson', 'bob.wilson@company.com', 'Marketing', 'employee'),
    ('Alice Johnson', 'alice.johnson@company.com', 'Sales', 'employee'),
    ('Mike Brown', 'mike.brown@company.com', 'Finance', 'ld');
END

-- Insert sample training requests
IF NOT EXISTS (SELECT * FROM TrainingRequests WHERE Title = 'React Development Training')
BEGIN
    INSERT INTO TrainingRequests (Title, Department, TrainingType, Priority, ExpectedParticipants, Duration, PreferredDate, Description, BusinessJustification, Budget, RequesterId) VALUES
    ('React Development Training', 'Engineering', 'Technical', 'high', 15, '3 days', '2024-03-15', 'Advanced React training for the development team', 'Improve team skills for upcoming projects', 5000.00, 2),
    ('Leadership Skills Workshop', 'HR', 'Soft Skills', 'medium', 20, '2 days', '2024-03-20', 'Leadership and management training', 'Develop leadership pipeline', 3000.00, 3),
    ('Sales Techniques Training', 'Sales', 'Sales', 'high', 12, '1 day', '2024-03-25', 'Advanced sales techniques and strategies', 'Increase sales performance', 2000.00, 5);
END

-- Insert sample training sessions
IF NOT EXISTS (SELECT * FROM TrainingSessions WHERE Title = 'React Development Training')
BEGIN
    INSERT INTO TrainingSessions (Title, StartDate, EndDate, Trainer, Location, Description, MaxParticipants, TrainingRequestId) VALUES
    ('React Development Training', '2024-03-15 09:00:00', '2024-03-17 17:00:00', 'Sarah Johnson', 'Conference Room A', 'Comprehensive React training', 15, 1),
    ('Leadership Skills Workshop', '2024-03-20 09:00:00', '2024-03-21 17:00:00', 'David Wilson', 'Training Room B', 'Leadership development workshop', 20, 2),
    ('Sales Techniques Training', '2024-03-25 09:00:00', '2024-03-25 17:00:00', 'Lisa Chen', 'Conference Room C', 'Advanced sales training', 12, 3);
END

-- Insert sample participants
IF NOT EXISTS (SELECT * FROM TrainingParticipants WHERE UserId = 2 AND TrainingSessionId = 1)
BEGIN
    INSERT INTO TrainingParticipants (UserId, TrainingSessionId) VALUES
    (2, 1), -- John Doe in React training
    (4, 1), -- Bob Wilson in React training
    (5, 2), -- Alice Johnson in Leadership workshop
    (2, 2); -- John Doe in Leadership workshop
END

PRINT 'Database setup completed successfully!'; 