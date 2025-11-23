DELIMITER //

DROP PROCEDURE IF EXISTS sp_GetBusMaintenanceHistory//

CREATE PROCEDURE sp_GetBusMaintenanceHistory(
    IN p_BusCountryNumber VARCHAR(20)
)
BEGIN
    SELECT 
        MaintenanceId,
        BusCountryNumber,
        MaintenanceDate,
        MaintenanceType,
        Description,
        Cost,
        MechanicName,
        NextMaintenanceDate,
        CreatedAt,
        CreatedBy
    FROM BusMaintenanceHistory
    WHERE BusCountryNumber = p_BusCountryNumber
    ORDER BY MaintenanceDate DESC;
END//

DELIMITER ;