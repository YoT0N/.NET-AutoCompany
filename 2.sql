DROP DATABASE IF EXISTS RoutesDB;
CREATE DATABASE RoutesDB
    CHARACTER SET utf8mb4 
    COLLATE utf8mb4_unicode_ci;

USE RoutesDB;

-- ============================
-- Table: Route
-- ============================
CREATE TABLE Route (
    RouteId INT AUTO_INCREMENT PRIMARY KEY,
    RouteNumber VARCHAR(20) NOT NULL UNIQUE,
    Name VARCHAR(150) NOT NULL,
    DistanceKm DECIMAL(6,2) CHECK (DistanceKm > 0),

    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- ============================
-- Table: RouteStop
-- STOP belongs to many routes → but we show M:N through linking table Route_Stop
-- ============================
CREATE TABLE RouteStop (
    StopId INT AUTO_INCREMENT PRIMARY KEY,
    StopName VARCHAR(150) NOT NULL,
    Latitude DECIMAL(9,6),
    Longitude DECIMAL(9,6)
);

-- M:N → Route ↔ RouteStop
CREATE TABLE Route_Stop (
    RouteId INT NOT NULL,
    StopId INT NOT NULL,
    StopOrder INT NOT NULL CHECK (StopOrder > 0),

    PRIMARY KEY (RouteId, StopId),

    FOREIGN KEY (RouteId) REFERENCES Route(RouteId) ON DELETE CASCADE,
    FOREIGN KEY (StopId) REFERENCES RouteStop(StopId) ON DELETE CASCADE
);

CREATE INDEX IX_RouteStop_Order ON Route_Stop(RouteId, StopOrder);

-- ============================
-- Table: BusInfo (duplicated from Project1)
-- ============================
CREATE TABLE BusInfo (
    BusId INT AUTO_INCREMENT PRIMARY KEY,
    CountryNumber VARCHAR(50) NOT NULL UNIQUE,
    Brand VARCHAR(50),
    Capacity INT CHECK (Capacity > 0),
    YearOfManufacture INT CHECK (YearOfManufacture >= 1900)
);

-- ============================
-- Table: RouteSheet
-- 1 RouteSheet = 1 Bus + 1 Route + specific day
-- ============================
CREATE TABLE RouteSheet (
    SheetId INT AUTO_INCREMENT PRIMARY KEY,
    RouteId INT NOT NULL,
    BusId INT NOT NULL,
    SheetDate DATE NOT NULL,

    UNIQUE (RouteId, BusId, SheetDate),

    FOREIGN KEY (RouteId) REFERENCES Route(RouteId) ON DELETE CASCADE,
    FOREIGN KEY (BusId) REFERENCES BusInfo(BusId) ON DELETE CASCADE
);

-- ============================
-- Table: Schedule
-- 1 Route has many schedules (1:N)
-- ============================
CREATE TABLE Schedule (
    ScheduleId INT AUTO_INCREMENT PRIMARY KEY,
    RouteId INT NOT NULL,
    DepartureTime TIME NOT NULL,
	ArrivalTime TIME NOT NULL,
    
    FOREIGN KEY (RouteId) REFERENCES Route(RouteId) ON DELETE CASCADE
);

DELIMITER //
CREATE TRIGGER trg_schedule_check_insert
BEFORE INSERT ON Schedule
FOR EACH ROW
BEGIN
    IF NEW.ArrivalTime <= NEW.DepartureTime THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'ArrivalTime must be greater than DepartureTime';
    END IF;
END;

CREATE TRIGGER trg_schedule_check_update
BEFORE UPDATE ON Schedule
FOR EACH ROW
BEGIN
    IF NEW.ArrivalTime <= NEW.DepartureTime THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'ArrivalTime must be greater than DepartureTime';
    END IF;
END //
DELIMITER ;

-- ============================
-- Table: Trip
-- 1 RouteSheet -> many Trips (1:N)
-- ============================
CREATE TABLE Trip (
    TripId INT AUTO_INCREMENT PRIMARY KEY,
    SheetId INT NOT NULL,
    ScheduledDeparture TIME NOT NULL,
    ActualDeparture TIME,
    Completed BOOLEAN DEFAULT FALSE,

    FOREIGN KEY (SheetId) REFERENCES RouteSheet(SheetId) ON DELETE CASCADE
);

-- ============================
-- Seed Data
-- ============================

INSERT INTO Route (RouteNumber, Name, DistanceKm)
VALUES 
('12А', 'Центр → вокзал', 12.5),
('25', 'Кільцевий маршрут', 25.0);

INSERT INTO RouteStop (StopName, Latitude, Longitude) VALUES
('Центральна площа', 49.123456, 24.123456),
('Проспект Свободи', 49.124444, 24.125555),
('Залізничний вокзал', 49.119999, 24.110000);

INSERT INTO Route_Stop (RouteId, StopId, StopOrder) VALUES
(1, 1, 1),
(1, 2, 2),
(1, 3, 3),
(2, 1, 1),
(2, 3, 2);

INSERT INTO BusInfo (CountryNumber, Brand, Capacity, YearOfManufacture)
VALUES 
('AA1234BB', 'Mercedes-Benz', 45, 2018),
('AA5678CC', 'MAN', 40, 2020);

INSERT INTO RouteSheet (RouteId, BusId, SheetDate)
VALUES 
(1, 1, '2024-11-18'),
(2, 2, '2024-11-18');

INSERT INTO Schedule (RouteId, DepartureTime, ArrivalTime)
VALUES
(1, '08:00', '08:30'),
(1, '09:00', '09:30'),
(2, '07:00', '08:00');

INSERT INTO Trip (SheetId, ScheduledDeparture)
VALUES
(1, '08:00'),
(1, '09:00'),
(2, '07:00');

SELECT 'RoutesDB created!' AS Status;
