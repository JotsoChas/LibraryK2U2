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
