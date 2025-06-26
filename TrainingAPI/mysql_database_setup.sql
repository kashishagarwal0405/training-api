-- Training Management System Database Setup for MySQL
-- Run this script in MySQL Workbench or phpMyAdmin

-- Create database if it doesn't exist
CREATE DATABASE IF NOT EXISTS abhyas;
USE abhyas;

-- Create Roles table
CREATE TABLE IF NOT EXISTS Roles (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(50) NOT NULL UNIQUE -- e.g. 'employee', 'ld', 'admin'
);

-- Create Users table
CREATE TABLE IF NOT EXISTS Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Department VARCHAR(50) NOT NULL,
    RoleId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- Create Courses table
CREATE TABLE IF NOT EXISTS Courses (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(200) NOT NULL,
    Description VARCHAR(1000) NULL,
    Department VARCHAR(50) NULL,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

-- Create TrainingSessions table
CREATE TABLE IF NOT EXISTS TrainingSessions (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(200) NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    Trainer VARCHAR(100) NOT NULL,
    Location VARCHAR(200) NULL,
    Description VARCHAR(500) NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'scheduled',
    MaxParticipants INT NOT NULL,
    CurrentParticipants INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL
);

-- Create TrainingRequests table
CREATE TABLE IF NOT EXISTS TrainingRequests (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(200) NOT NULL,
    Department VARCHAR(50) NOT NULL,
    TrainingType VARCHAR(50) NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'pending',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL,
    RequesterId INT NOT NULL,
    TrainingSessionId INT NULL,
    FOREIGN KEY (RequesterId) REFERENCES Users(Id),
    FOREIGN KEY (TrainingSessionId) REFERENCES TrainingSessions(Id)
);

-- Create TrainingParticipants table
CREATE TABLE IF NOT EXISTS TrainingParticipants (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    TrainingSessionId INT NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'registered',
    RegisteredAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    AttendedAt DATETIME NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (TrainingSessionId) REFERENCES TrainingSessions(Id) ON DELETE CASCADE,
    UNIQUE KEY UQ_User_Session (UserId, TrainingSessionId)
);

-- Create Trainers table
CREATE TABLE IF NOT EXISTS Trainers (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Expertise VARCHAR(200) NULL,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

-- Create Attendance table
CREATE TABLE IF NOT EXISTS Attendance (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    TrainingSessionId INT NOT NULL,
    UserId INT NOT NULL,
    AttendedAt DATETIME NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'absent', -- e.g. 'present', 'absent'
    FOREIGN KEY (TrainingSessionId) REFERENCES TrainingSessions(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Insert sample roles
INSERT IGNORE INTO Roles (Name) VALUES
('employee'),
('ld'),
('admin');

-- Insert sample users
INSERT IGNORE INTO Users (Name, Email, Department, RoleId) VALUES
('Admin User', 'admin@company.com', 'IT', 3),
('John Doe', 'john.doe@company.com', 'Engineering', 1),
('Jane Smith', 'jane.smith@company.com', 'HR', 2),
('Bob Wilson', 'bob.wilson@company.com', 'Marketing', 1),
('Alice Johnson', 'alice.johnson@company.com', 'Sales', 1),
('Mike Brown', 'mike.brown@company.com', 'Finance', 2);

-- Insert sample courses
INSERT IGNORE INTO Courses (Name, Description, Department) VALUES
('React Development', 'Advanced React training for developers', 'Engineering'),
('Leadership Skills', 'Leadership and management training', 'HR'),
('Sales Techniques', 'Advanced sales techniques and strategies', 'Sales'),
('Project Management', 'Project management fundamentals', 'IT');

-- Insert sample trainers
INSERT IGNORE INTO Trainers (Name, Email, Expertise) VALUES
('Sarah Johnson', 'sarah.johnson@company.com', 'React, JavaScript, Frontend Development'),
('David Wilson', 'david.wilson@company.com', 'Leadership, Management, Soft Skills'),
('Lisa Chen', 'lisa.chen@company.com', 'Sales, Marketing, Customer Relations'),
('Mike Rodriguez', 'mike.rodriguez@company.com', 'Project Management, Agile, Scrum');

-- Insert sample training sessions
INSERT IGNORE INTO TrainingSessions (Title, StartDate, EndDate, Trainer, Location, Description, MaxParticipants) VALUES
('React Development Training', '2024-03-15 09:00:00', '2024-03-17 17:00:00', 'Sarah Johnson', 'Conference Room A', 'Comprehensive React training', 15),
('Leadership Skills Workshop', '2024-03-20 09:00:00', '2024-03-21 17:00:00', 'David Wilson', 'Training Room B', 'Leadership development workshop', 20),
('Sales Techniques Training', '2024-03-25 09:00:00', '2024-03-25 17:00:00', 'Lisa Chen', 'Conference Room C', 'Advanced sales training', 12);

-- Insert sample training requests
INSERT IGNORE INTO TrainingRequests (Title, Department, TrainingType, RequesterId) VALUES
('React Development Training', 'Engineering', 'Technical', 2),
('Leadership Skills Workshop', 'HR', 'Soft Skills', 3),
('Sales Techniques Training', 'Sales', 'Sales', 5);

-- Insert sample participants
INSERT IGNORE INTO TrainingParticipants (UserId, TrainingSessionId) VALUES
(2, 1), -- John Doe in React training
(4, 1), -- Bob Wilson in React training
(5, 2), -- Alice Johnson in Leadership workshop
(2, 2); -- John Doe in Leadership workshop

-- Insert sample attendance
INSERT IGNORE INTO Attendance (TrainingSessionId, UserId, Status) VALUES
(1, 2, 'present'),
(1, 4, 'present'),
(2, 5, 'present'),
(2, 2, 'absent');

SELECT 'Database setup completed successfully!' as Status; 