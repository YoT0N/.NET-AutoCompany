DELIMITER //

DROP PROCEDURE IF EXISTS sp_CalculateMaintenanceCost//

CREATE PROCEDURE sp_CalculateMaintenanceCost(
    IN p_BusCountryNumber VARCHAR(20),
    OUT p_TotalCost DECIMAL(10,2)
)
BEGIN
    SELECT COALESCE(SUM(Cost), 0)
    INTO p_TotalCost
    FROM BusMaintenanceHistory
    WHERE BusCountryNumber = p_BusCountryNumber;
END//

DELIMITER ;