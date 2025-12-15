--1
CREATE NONCLUSTERED INDEX IX_Loan_ReturnDate
ON Loan (ReturnDate);

--2
CREATE NONCLUSTERED INDEX IX_Loan_ReturnDate_Null
ON Loan (ReturnDate)
WHERE ReturnDate IS NULL;
