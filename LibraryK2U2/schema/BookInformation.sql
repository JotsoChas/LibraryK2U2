-- Retrieves basic information about all books in the library.
-- The result is ordered by category to group similar books together.
SELECT Title, Author, ISBN, Category
FROM   Book
ORDER BY Category;