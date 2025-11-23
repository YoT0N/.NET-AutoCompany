DELIMITER //

DROP PROCEDURE IF EXISTS sp_CreateExamination//

CREATE PROCEDURE sp_CreateExamination(
    IN p_BusCountryNumber VARCHAR(20),
    IN p_ExaminationDate DATE,
    IN p_ExaminationResult VARCHAR(50),
    IN p_SentForRepair BOOLEAN,
    IN p_RepairPrice DECIMAL(10,2),
    IN p_MechanicName VARCHAR(100),
    IN p_Notes TEXT,
    OUT p_ExaminationId BIGINT
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    INSERT INTO TechnicalExamination (
        BusCountryNumber, ExaminationDate, ExaminationResult,
        SentForRepair, RepairPrice, MechanicName, Notes
    ) VALUES (
        p_BusCountryNumber, p_ExaminationDate, p_ExaminationResult,
        p_SentForRepair, p_RepairPrice, p_MechanicName, p_Notes
    );

    SET p_ExaminationId = LAST_INSERT_ID();

    -- Автоматично оновлюємо статус автобуса, якщо він відправлений на ремонт
    IF p_SentForRepair = TRUE THEN
        UPDATE Bus
        SET CurrentStatusId = (SELECT StatusId FROM BusStatus WHERE StatusName = 'InRepair')
        WHERE CountryNumber = p_BusCountryNumber;
    END IF;

    COMMIT;
END//

DELIMITER ;