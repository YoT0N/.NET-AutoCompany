-- ============================================
-- TransportService Database Schema
-- MySQL 8.0.39 + Triggers
-- ============================================

DROP DATABASE IF EXISTS TransportServiceDB;
CREATE DATABASE TransportServiceDB 
    CHARACTER SET utf8mb4 
    COLLATE utf8mb4_unicode_ci;

USE TransportServiceDB;

-- ============================================
-- Таблиця 1: BusStatus
-- ============================================
CREATE TABLE BusStatus (
    StatusId INT AUTO_INCREMENT PRIMARY KEY,
    StatusName VARCHAR(50) NOT NULL UNIQUE,
    StatusDescription VARCHAR(200),
    
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- ============================================
-- Таблиця 2: Bus
-- ============================================
CREATE TABLE Bus (
    CountryNumber VARCHAR(20) PRIMARY KEY,
    BoardingNumber VARCHAR(10) NOT NULL UNIQUE,
    Brand VARCHAR(50) NOT NULL,
    PassengerCapacity INT NOT NULL CHECK (PassengerCapacity > 0),
    YearOfManufacture INT NOT NULL, -- CHECK видалено!
    Mileage DECIMAL(10,2) NOT NULL DEFAULT 0 CHECK (Mileage >= 0),
    DateOfReceipt DATE NOT NULL,
    WriteoffDate DATE NULL,

    CurrentStatusId INT NOT NULL,

    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(100),

    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(100),

    IsDeleted BOOLEAN DEFAULT FALSE,

    CONSTRAINT FK_Bus_BusStatus FOREIGN KEY (CurrentStatusId)
        REFERENCES BusStatus(StatusId) ON DELETE RESTRICT,

    CONSTRAINT CHK_WriteoffDate CHECK (WriteoffDate IS NULL OR WriteoffDate >= DateOfReceipt)
) ENGINE=InnoDB;


-- ============================================
-- Таблиця 3: TechnicalExamination
-- ============================================
CREATE TABLE TechnicalExamination (
    ExaminationId BIGINT AUTO_INCREMENT PRIMARY KEY,
    BusCountryNumber VARCHAR(20) NOT NULL,
    ExaminationDate DATE NOT NULL,
    ExaminationResult VARCHAR(50) NOT NULL CHECK (ExaminationResult IN ('Passed', 'Failed')),
    SentForRepair BOOLEAN NOT NULL DEFAULT FALSE,
    RepairPrice DECIMAL(10,2) DEFAULT 0 CHECK (RepairPrice >= 0),
    MechanicName VARCHAR(100),
    Notes TEXT,
    
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(100),

    CONSTRAINT FK_TechnicalExamination_Bus FOREIGN KEY (BusCountryNumber) 
        REFERENCES Bus(CountryNumber) ON DELETE CASCADE
) ENGINE=InnoDB;

-- ============================================
-- Таблиця 4: BusMaintenanceHistory
-- ============================================
CREATE TABLE BusMaintenanceHistory (
    MaintenanceId BIGINT AUTO_INCREMENT PRIMARY KEY,
    BusCountryNumber VARCHAR(20) NOT NULL,
    MaintenanceDate DATE NOT NULL,
    MaintenanceType VARCHAR(100) NOT NULL,
    Description TEXT,
    Cost DECIMAL(10,2) NOT NULL DEFAULT 0 CHECK (Cost >= 0),
    MechanicName VARCHAR(100),
    NextMaintenanceDate DATE,
    
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(100),

    CONSTRAINT FK_BusMaintenanceHistory_Bus FOREIGN KEY (BusCountryNumber) 
        REFERENCES Bus(CountryNumber) ON DELETE CASCADE
) ENGINE=InnoDB;

-- ============================================
-- Таблиця 5: RepairPart
-- ============================================
CREATE TABLE RepairPart (
    PartId INT AUTO_INCREMENT PRIMARY KEY,
    PartName VARCHAR(100) NOT NULL,
    PartNumber VARCHAR(50) UNIQUE,
    UnitPrice DECIMAL(10,2) NOT NULL CHECK (UnitPrice >= 0),
    StockQuantity INT NOT NULL DEFAULT 0 CHECK (StockQuantity >= 0),
    Supplier VARCHAR(100),

    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    IsDeleted BOOLEAN DEFAULT FALSE
) ENGINE=InnoDB;

-- ============================================
-- Таблиця 6: ExaminationRepairPart
-- ============================================
CREATE TABLE ExaminationRepairPart (
    ExaminationId BIGINT NOT NULL,
    PartId INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1 CHECK (Quantity > 0),
    TotalPrice DECIMAL(10,2),

    PRIMARY KEY (ExaminationId, PartId),

    CONSTRAINT FK_ExaminationRepairPart_Examination 
        FOREIGN KEY (ExaminationId) REFERENCES TechnicalExamination(ExaminationId) ON DELETE CASCADE,

    CONSTRAINT FK_ExaminationRepairPart_Part 
        FOREIGN KEY (PartId) REFERENCES RepairPart(PartId) ON DELETE CASCADE
) ENGINE=InnoDB;

-- ============================================
-- ТРИГЕРИ CreatedBy / UpdatedBy
-- ============================================

-- ===== Bus =====

DELIMITER //
CREATE TRIGGER trg_bus_check_year_insert
BEFORE INSERT ON Bus
FOR EACH ROW
BEGIN
    IF NEW.YearOfManufacture < 1900 OR NEW.YearOfManufacture > YEAR(CURDATE()) THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'YearOfManufacture must be between 1900 and current year';
    END IF;
END ;

CREATE TRIGGER trg_bus_check_year_update
BEFORE UPDATE ON Bus
FOR EACH ROW
BEGIN
    IF NEW.YearOfManufacture < 1900 OR NEW.YearOfManufacture > YEAR(CURDATE()) THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'YearOfManufacture must be between 1900 and current year';
    END IF;
END ;


CREATE TRIGGER trg_bus_before_insert 
BEFORE INSERT ON Bus
FOR EACH ROW
BEGIN
    SET NEW.CreatedBy = CURRENT_USER();
    SET NEW.UpdatedBy = CURRENT_USER();
END;

CREATE TRIGGER trg_bus_before_update
BEFORE UPDATE ON Bus
FOR EACH ROW
BEGIN
    SET NEW.UpdatedBy = CURRENT_USER();
END;

-- ===== TechnicalExamination =====
CREATE TRIGGER trg_exam_before_insert
BEFORE INSERT ON TechnicalExamination
FOR EACH ROW
BEGIN
    SET NEW.CreatedBy = CURRENT_USER();
END;

-- ===== BusMaintenanceHistory =====
CREATE TRIGGER trg_maint_before_insert
BEFORE INSERT ON BusMaintenanceHistory
FOR EACH ROW
BEGIN
    SET NEW.CreatedBy = CURRENT_USER();
END ;

-- ============================================
-- Тригери TotalPrice для ExaminationRepairPart
-- ============================================

CREATE TRIGGER trg_exam_part_before_insert
BEFORE INSERT ON ExaminationRepairPart
FOR EACH ROW
BEGIN
    SET NEW.TotalPrice =
        NEW.Quantity * (SELECT UnitPrice FROM RepairPart WHERE PartId = NEW.PartId);
END;

CREATE TRIGGER trg_exam_part_before_update
BEFORE UPDATE ON ExaminationRepairPart
FOR EACH ROW
BEGIN
    SET NEW.TotalPrice =
        NEW.Quantity * (SELECT UnitPrice FROM RepairPart WHERE PartId = NEW.PartId);
END //
DELIMITER ;

-- ============================================
-- Індекси
-- ============================================
CREATE INDEX IX_Bus_Status ON Bus(CurrentStatusId, CountryNumber, Brand, BoardingNumber);
CREATE INDEX IX_Examination_Date ON TechnicalExamination(ExaminationDate DESC, BusCountryNumber, ExaminationResult);
CREATE INDEX IX_Examination_Repair ON TechnicalExamination(SentForRepair, BusCountryNumber);
CREATE INDEX IX_Maintenance_Date ON BusMaintenanceHistory(BusCountryNumber, MaintenanceDate DESC);
CREATE INDEX IX_Part_Stock ON RepairPart(StockQuantity, PartName);

-- ============================================
-- Seed data
-- ============================================

INSERT INTO BusStatus (StatusName, StatusDescription) VALUES
('Active','Автобус в експлуатації'),
('InRepair','Автобус на ремонті'),
('Inactive','Автобус не в експлуатації'),
('WrittenOff','Автобус списаний');

INSERT INTO Bus (CountryNumber, BoardingNumber, Brand, PassengerCapacity, YearOfManufacture, Mileage, DateOfReceipt, CurrentStatusId) VALUES
('AA1234BB', '101', 'Mercedes-Benz Citaro', 45, 2018, 125000.50, '2018-03-15', 1),
('AA5678CC', '102', 'MAN Lion''s City', 40, 2020, 85000.00, '2020-06-10', 1),
('AA9012DD', '103', 'Iveco Crossway', 38, 2015, 250000.75, '2015-09-20', 2),
('AA3456EE', '104', 'Volvo 7900', 42, 2019, 150000.00, '2019-11-05', 1);

INSERT INTO RepairPart (PartName, PartNumber, UnitPrice, StockQuantity, Supplier) VALUES
('Гальмівні колодки', 'BRK-001', 1500.00, 25, 'AutoParts Ltd'),
('Масляний фільтр', 'FLT-002', 350.00, 50, 'FilterPro'),
('Повітряний фільтр', 'FLT-003', 280.00, 40, 'FilterPro'),
('Свічки запалювання', 'IGN-004', 120.00, 60, 'SparkPlugs Inc'),
('Ремінь ГРМ', 'BLT-005', 2500.00, 15, 'BeltMasters'),
('Акумулятор', 'BAT-006', 4500.00, 10, 'BatteryWorld'),
('Щітки склоочисника', 'WIP-007', 250.00, 35, 'AutoParts Ltd');

INSERT INTO TechnicalExamination 
(BusCountryNumber, ExaminationDate, ExaminationResult, SentForRepair, RepairPrice, MechanicName, Notes) VALUES
('AA1234BB','2024-01-15','Passed',FALSE,0,'Іванов І.І.','Технічний стан задовільний'),
('AA5678CC','2024-02-20','Passed',FALSE,0,'Петров П.П.','Всі системи працюють нормально'),
('AA9012DD','2024-03-10','Failed',TRUE,8500,'Сидоров С.С.','Потрібна заміна гальмівної системи'),
('AA1234BB','2024-06-15','Passed',TRUE,2500,'Іванов І.І.','Планове обслуговування');

INSERT INTO BusMaintenanceHistory 
(BusCountryNumber, MaintenanceDate, MaintenanceType, Description, Cost, MechanicName, NextMaintenanceDate) VALUES
('AA1234BB','2024-01-15','Regular','Заміна масла, фільтрів',3500,'Іванов І.І.','2024-07-15'),
('AA5678CC','2024-02-20','Regular','Перевірка всіх систем',2800,'Петров П.П.','2024-08-20'),
('AA9012DD','2024-03-10','Emergency','Аварійний ремонт гальм',8500,'Сидоров С.С.','2024-09-10'),
('AA3456EE','2024-04-05','Scheduled','Заміна ременя ГРМ',4200,'Коваль К.К.','2024-10-05');

INSERT INTO ExaminationRepairPart (ExaminationId, PartId, Quantity) VALUES
(3,1,4),
(3,2,2),
(4,2,1),
(4,3,1),
(4,5,1);

-- ============================================
-- Перевірка
-- ============================================
SELECT 'Database TransportServiceDB created successfully!' AS Status;
 