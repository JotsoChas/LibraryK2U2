-- Index strategy 1:
-- Nonclustered index on ReturnDate to improve performance for queries
-- that filter or sort loans based on return status.
CREATE NONCLUSTERED INDEX IX_Loan_ReturnDate
ON Loan (ReturnDate);

-- Index strategy 2:
-- Filtered nonclustered index that only includes active loans.
-- This index is optimized for queries that frequently search for
-- loans where ReturnDate IS NULL.
CREATE NONCLUSTERED INDEX IX_Loan_ReturnDate_Null
ON Loan (ReturnDate)
WHERE ReturnDate IS NULL;