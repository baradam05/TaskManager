CREATE DATABASE TaskManager;
USE TaskManager;
GO

DROP TABLE IF EXISTS Assigned;
DROP TABLE IF EXISTS SubAssignment;
DROP TABLE IF EXISTS Assignment;
DROP TABLE IF EXISTS Account;
GO

CREATE TABLE Account (
    AccountId INT IDENTITY(1,1) PRIMARY KEY,
    LeaderId INT NULL,
    Username VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Email VARCHAR(255)
);

CREATE TABLE Assignment (
    AssignmentId INT IDENTITY(1,1) PRIMARY KEY,
    AssignedDate DATETIME NOT NULL,
    AssignedBy INT NULL,
    StartDate DATETIME NULL,
    EndDate DATETIME NOT NULL,
    Finished DATETIME NULL
);

CREATE TABLE SubAssignment (
    SubAssignmentId INT IDENTITY(1,1) PRIMARY KEY,
    AssignmentId INT NOT NULL,
    Assignment VARCHAR(1000) NULL,
    Finished BIT NOT NULL DEFAULT 0
);

CREATE TABLE Assigned (
    AccountId INT NOT NULL,
    AssignmentId INT NOT NULL,
    CONSTRAINT PK_Assigned PRIMARY KEY (AccountId, AssignmentId)
);

ALTER TABLE Assigned
ADD CONSTRAINT FK_AssignedAccount
FOREIGN KEY (AccountId) REFERENCES Account(AccountId) ON DELETE CASCADE;

ALTER TABLE Assigned
ADD CONSTRAINT FK_AssignedAssignment
FOREIGN KEY (AssignmentId) REFERENCES Assignment(AssignmentId) ON DELETE CASCADE;

ALTER TABLE Account
ADD CONSTRAINT FK_AccountAccount
FOREIGN KEY (LeaderId) REFERENCES Account(AccountId) ON DELETE NO ACTION;

ALTER TABLE Assignment
ADD CONSTRAINT FK_AssignmentAccount
FOREIGN KEY (AssignedBy) REFERENCES Account(AccountId) ON DELETE SET NULL;

ALTER TABLE SubAssignment
ADD CONSTRAINT FK_SubAssignmentAssignment
FOREIGN KEY (AssignmentId) REFERENCES Assignment(AssignmentId) ON DELETE CASCADE;
GO
