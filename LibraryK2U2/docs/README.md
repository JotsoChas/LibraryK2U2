<h1 align="center">
  <b>Library (K2U2)</b>
</h1>

## Dokumentation och arbetslogg 

### 2025-11-22
- Skapade ER modell med tabellerna Book, Member, Loan.

### 2025-11-23
Påbörjade att bygga databasen i SSMS.

### 2025-11-24
- Skapade ett repo för projektet och pushade min ER modell.
- Genereade fram testdata till Book, Member och Loan.

### 2025-11-25
- Skrev Query Designer

### --->2025-12-10
- Skapade två views:
v_ActiveLoans (ReturnDate IS NULL)
v_ReturnedLoans (ReturnDate IS NOT NULL)

- Skapade två stored procedures:
RegisterLoan (Lånar bok och sätter DueDate +14 dagar)
ReturnBook (Återlämnar bok och skriver ReturnDate)

- Skapade trigger:
 trg_OnReturnBook (aktiveras när ReturnDate uppdateras)

- Strukturerade mappstruktur med sql-filer

### 2025-12-12
- Implementerade EF Database First genom att scaffolda hela databasen
- Säkerställde fungerande anslutning mellan EF Core och databasen
- Verifierade att tabellen Book scaffoldades korrekt

### 2025-12-13 
- Verifierade att tabeller och vyer (Member, Loan, ActiveLoans, ReturnedLoans) scaffoldades korrekt
- Testade läsning av samtliga DbSet via ett enkelt konsolprogram för att bekräfta korrekt koppling
- Testet säkerställde att anslutningen upprätades korrekt

### 2025-12-14 
### Transaktioner

## Steg 1 Registrera lån med COMMIT
- Jag testade att registrera ett lån genom att köra en transaktion som avslutades med COMMIT.
- Detta innebär att hela utlåningen sparas när allt går igenom.
- Resultatet blev att ett nytt lån skapades och databasen bekräftade att operationen lyckades.
- Bild: loan_commit

## Steg 2 Kontroll efter COMMIT
- Efter COMMIT kontrollerade jag tabellen Loan genom att läsa ut de senaste raderna.
- Resultatet visar att ett nytt lån har lagts till med ett nytt LoanId, vilket bekräftar att transaktionen sparades korrekt.
- Bild: loan_after_commit

## Steg 3 Test av ROLLBACK
- Jag testade sedan att köra samma utlåning igen i en ny transaktion, men avslutade den med ROLLBACK.
- Stored proceduren returnerade ett meddelande om att boken redan var utlånad och inga ändringar sparades i databasen.
- Bild: loan_rollback

## Steg 4 – Kontroll efter ROLLBACK
- Efter ROLLBACK kontrollerade jag återigen tabellen Loan.
- Resultatet visar att inget nytt lån har skapats. Senaste LoanId är oförändrat, vilket bekräftar att transaktionen avbröts korrekt och att databasen inte påverkades.
- Bild: loan_transaction_after_rollback

## Sammanfattning
- Testerna visar att transaktionerna fungerar som förväntat.
- COMMIT sparar ändringar.
- ROLLBACK stoppar ändringar vid fel.
- Databasen hålls konsekvent i båda fallen.
- Detta visar att transaktionshanteringen fungerar korrekt.