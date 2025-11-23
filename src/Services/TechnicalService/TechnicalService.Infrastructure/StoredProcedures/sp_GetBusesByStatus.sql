DELIMITER //

DROP PROCEDURE IF EXISTS sp_GetBusesByStatus//

CREATE PROCEDURE sp_GetBusesByStatus(
    IN p_StatusId INT
)
BEGIN
    SELECT 
        b.CountryNumber,
        b.BoardingNumber,
        b.Brand,
        b.PassengerCapacity,
        b.YearOfManufacture,
        b.Mileage,
        b.DateOfReceipt,
        b.WriteoffDate,
        b.CurrentStatusId,
        bs.StatusName,
        bs.StatusDescription
    FROM Bus b
    INNER JOIN BusStatus bs ON b.CurrentStatusId = bs.StatusId
    WHERE b.CurrentStatusId = p_StatusId
      AND b.IsDeleted = FALSE
    ORDER BY b.CountryNumber;
END//

DELIMITER ;