<h1 align="center">
  <b>Library (K2U2)</b>
</h1>

## Bibliotekssystem

Det här projektet är ett enkelt bibliotekssystem som hanterar böcker, medlemmar, utlåning och återlämning.  
Systemet bygger på en relationsdatabas där varje lån kopplar en bok till en medlem under en begränsad period.

Lösningen stödjer registrering av nya lån, återlämning av böcker, visning av aktiva lån samt historik över tidigare lån.  
Fokus har legat på att modellera databasen korrekt, säkerställa dataintegritet och visa hur transaktioner, indexering och grundläggande konkurrenshantering fungerar i praktiken.

---

## Dokumentation och arbetslogg

### 2025-11-22
- Skapade ER-modell med tabellerna Book, Member och Loan.

#### Dokumentation
- [ER-diagram över Book, Member och Loan](docs/images/ER.png)

---

### 2025-11-23
- Påbörjade att bygga databasen i SSMS.

---

### 2025-11-24
- Skapade ett repo för projektet och pushade ER-modellen.
- Genererade testdata till Book, Member och Loan.

---

### 2025-11-25
- Skrev Query Designer.

#### Dokumentation
- [Query Designer – lån och återlämningar](docs/images/queries/Query_AllLoansAndReturns_Result.jpg)
- [Query Designer – bokinformation](docs/images/queries/Query_BookInformation_Result.jpg)

---

### 2025-11-26 – 2025-12-10
- Skapade två views:
  - `v_ActiveLoans` (ReturnDate IS NULL)
  - `v_ReturnedLoans` (ReturnDate IS NOT NULL)

#### Dokumentation
- [v_ActiveLoans.sql](views/v_ActiveLoans.sql)
- [v_ReturnedLoans.sql](views/v_ReturnedLoans.sql)

- Skapade två stored procedures:
  - `RegisterLoan` – lånar bok och sätter DueDate +14 dagar
  - `ReturnBook` – återlämnar bok och sätter ReturnDate

#### Dokumentation
- [sp_RegisterLoan.sql](procedures/sp_RegisterLoan.sql)
- [sp_ReturnBook.sql](procedures/sp_ReturnBook.sql)

- Skapade trigger:
  - `trg_OnReturnBook` – aktiveras när ReturnDate uppdateras

#### Dokumentation
- [triggers.sql](schema/triggers.sql)

- Strukturerade mappstruktur för SQL-filer.

---

### 2025-12-12
- Implementerade EF Core Database First genom scaffold.
- Säkerställde korrekt koppling mellan EF Core och databasen.
- Verifierade att tabellen Book scaffoldades korrekt.

#### Dokumentation
- [LibraryDBContext.cs](data/LibraryDBContext.cs)
- [Book.cs](models/Book.cs)
- [Loan.cs](models/Loan.cs)
- [Member.cs](models/Member.cs)
- [ActiveLoan.cs](models/ActiveLoan.cs)
- [ReturnedLoan.cs](models/ReturnedLoan.cs)

---

### 2025-12-13
- Verifierade att tabeller och vyer scaffoldades korrekt.
- Testade läsning av samtliga DbSet via konsolprogram.
- Bekräftade stabil databasanslutning.

#### Dokumentation
- [Program.cs](Program.cs)
- [LibraryDBContext.cs](data/LibraryDBContext.cs)

---

### 2025-12-14
## Transaktioner

Transaktioner testades för att säkerställa korrekt hantering av lyckade och avbrutna operationer.

Testerna genomfördes genom att:
- registrera ett lån med COMMIT
- verifiera sparad ändring
- genomföra ett nytt försök med ROLLBACK
- verifiera att inga ändringar sparades

#### Sammanfattning
- COMMIT sparar ändringar permanent.
- ROLLBACK avbryter ändringar vid fel.
- Databasen hålls konsekvent i båda fallen.

#### Dokumentation
- [Registrering av lån med COMMIT](docs/images/transactions/loan_commit.png)
- [Kontroll efter COMMIT](docs/images/transactions/loan_after_commit.jpg)
- [Transaktion med ROLLBACK](docs/images/transactions/loan_rollback.jpg)
- [Verifiering efter ROLLBACK](docs/images/transactions/loan_transaction_after_rollback.jpg)

---

### 2025-12-14
## Index och execution plan

Index skapades och analyserades för att utvärdera execution plans vid filtrering av aktiva lån.

Testerna omfattade:
- query utan index
- nonclustered index på ReturnDate
- filtered nonclustered index (ReturnDate IS NULL)

#### Sammanfattning
- Utan index används table scan.
- Filtered index är bäst anpassat.
- SQL Server väljer plan baserat på kostnad.

#### Dokumentation
- [Execution plan utan index](docs/images/indexes/index_before_returndate.jpeg)
- [Execution plan med nonclustered index](docs/images/indexes/index_after_returndate.jpeg)
- [Execution plan med filtered index](docs/images/indexes/index_after_filtered_returndate.jpg)

---

### 2025-12-15
## Konkurrenshantering

Konkurrens testades genom samtidiga lån i två databassessioner.

Testet visade att:
- endast ett aktivt lån kan skapas
- andra sessioner blockeras tills COMMIT
- dataintegriteten bibehålls

#### Dokumentation
- [Val av bok utan aktivt lån](docs/images/concurrency/concurrency_01_find_available_book.jpg)
- [Session A – lås](docs/images/concurrency/concurrency_02_session_a_lock_open_tran.jpg)
- [Session B – blockerad](docs/images/concurrency/concurrency_03_session_b_blocked.jpg)
- [Session A – COMMIT](docs/images/concurrency/concurrency_04_session_a_commit.jpg)
- [Verifiering av ett aktivt lån](docs/images/concurrency/concurrency_05_verify_single_active_loan.jpg)

---

## Reflektion – optimering och dataintegritet

Projektet fokuserar på korrekt och stabil databasdesign snarare än storskalig optimering.  
Index, transaktioner och låsning har analyserats för att visa hur SQL Server säkerställer konsekvent data och korrekt beteende även vid samtidiga operationer.

---

### 2025-12-17
- Utökade konsolapplikationen med en sammanhållen menystruktur.
- Implementerade återanvändbar MenuBuilder.
- Förbättrade Delete Member, Delete Book och Force Return med tydlig status.
- Lade till IntroScreen och ExitScreen för professionell helhetsupplevelse.
