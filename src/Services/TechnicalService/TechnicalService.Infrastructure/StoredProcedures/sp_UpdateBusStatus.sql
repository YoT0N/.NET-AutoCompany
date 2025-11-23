DELIMITER //

DROP PROCEDURE IF EXISTS sp_UpdateBusStatus//

CREATE PROCEDURE sp_UpdateBusStatus(
    IN p_CountryNumber VARCHAR(20),
    IN p_NewStatusId INT
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    UPDATE Bus
    SET CurrentStatusId = p_NewStatusId
    WHERE CountryNumber = p_CountryNumber
      AND IsDeleted = FALSE;

    IF ROW_COUNT() = 0 THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Bus not found or already deleted';
    END IF;

    COMMIT;
END//

DELIMITER ;