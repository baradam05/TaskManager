--CREATE
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


--ISNERT
USE TaskManager;
GO

DELETE FROM Assigned;
DELETE FROM SubAssignment;
DELETE FROM Assignment;
DELETE FROM Account;
GO

DBCC CHECKIDENT ('SubAssignment', RESEED, 1);
DBCC CHECKIDENT ('Assignment', RESEED, 1);
DBCC CHECKIDENT ('Account', RESEED, 1);
GO

INSERT INTO Account (LeaderId, Username, PasswordHash, Email)
VALUES
(NULL, 'manager', 'EXGTqw1uGkWeYHpB64Nafg==', 'manager@example.com'),
(1, 'alice', 'EXGTqw1uGkWeYHpB64Nafg==', 'alice@example.com'),
(1, 'bob', 'EXGTqw1uGkWeYHpB64Nafg==', 'bob@example.com');
GO

INSERT INTO Assignment (AssignedDate, AssignedBy, StartDate, EndDate, Finished)
VALUES
('2025-07-20', 1, '2025-07-21', '2025-07-28', '2025-07-27'),
('2025-07-25', 1, '2025-07-26', '2025-08-01', '2025-08-05'),
('2025-08-10', 1, '2025-08-11', '2025-08-25', NULL),
('2025-07-22', 1, '2025-07-23', '2025-07-30', '2025-07-29'),
('2025-07-28', 1, '2025-07-29', '2025-08-05', NULL),
('2025-08-15', 1, '2025-08-16', '2025-09-01', NULL),
('2025-08-05', 1, '2025-08-06', '2025-08-20', NULL);
GO

INSERT INTO SubAssignment (AssignmentId, Assignment, Finished)
VALUES
(1, 'Napsat úvodní zprávu', 1),
(1, 'Připravit podklady', 1),
(1, 'Odevzdat manažerovi', 1),
(2, 'Nastavit databázi', 1),
(2, 'Import dat', 1),
(2, 'Dokumentace', 0),
(3, 'Testování API', 0),
(3, 'Sepsat report chyb', 0),
(4, 'Nastavit server', 1),
(4, 'Nasadit aplikaci', 1),
(5, 'Vypracovat analýzu', 1),
(5, 'Prezentace výsledků', 0),
(6, 'Naplánovat meeting', 0),
(6, 'Sepsat zápis z meetingu', 1),
(7, 'Společná prezentace', 0),
(7, 'Rozdělit úkoly mezi členy', 1),
(7, 'Finalizace dokumentace', 0);
GO

INSERT INTO Assigned (AccountId, AssignmentId)
VALUES
(2, 1),
(2, 2),
(2, 3),
(3, 4),
(3, 5),
(3, 6),
(2, 7),
(3, 7);
GO
