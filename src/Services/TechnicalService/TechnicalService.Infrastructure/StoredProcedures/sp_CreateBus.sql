DELIMITER //

DROP PROCEDURE IF EXISTS sp_CreateBus//

CREATE PROCEDURE sp_CreateBus(
    IN p_CountryNumber VARCHAR(20),
    IN p_BoardingNumber VARCHAR(10),
    IN p_Brand VARCHAR(50),
    IN p_PassengerCapacity INT,
    IN p_YearOfManufacture INT,
    IN p_Mileage DECIMAL(10,2),
    IN p_DateOfReceipt DATE,
    IN p_CurrentStatusId INT
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    INSERT INTO Bus (
        CountryNumber, BoardingNumber, Brand, PassengerCapacity,
        YearOfManufacture, Mileage, DateOfReceipt, CurrentStatusId
    ) VALUES (
        p_CountryNumber, p_BoardingNumber, p_Brand, p_PassengerCapacity,
        p_YearOfManufacture, p_Mileage, p_DateOfReceipt, p_CurrentStatusId
    );

    COMMIT;
END//

DELIMITER ;