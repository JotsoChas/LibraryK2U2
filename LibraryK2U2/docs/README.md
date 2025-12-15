<h1 align="center">
  <b>Library (K2U2)</b>
</h1>

## Bibliotekssystem

Det här projektet är ett enkelt bibliotekssystem som hanterar böcker, medlemmar, utlåning och återlämning. 
Systemet bygger på en relationsdatabas där varje lån kopplar en bok till en medlem under en begränsad period.

Lösningen stödjer registrering av nya lån, återlämning av böcker, visning av aktiva lån samt historik över tidigare lån. 
Fokus har legat på att modellera databasen korrekt, säkerställa dataintegritet och visa hur transaktioner, indexering och grundläggande konkurrenshantering fungerar i praktiken.


## Dokumentation och arbetslogg

---

### 2025-11-22
- Skapade ER modell med tabellerna Book, Member, Loan.

#### Dokumentation
- [ER-diagram över Book, Member och Loan](docs/images/ER.png)

---

### 2025-11-23
- Påbörjade att bygga databasen i SSMS.

---

### 2025-11-24
- Skapade ett repo för projektet och pushade min ER modell.
- Genereade fram testdata till Book, Member och Loan.

---

### 2025-11-25
- Skrev Query Designer.

#### Dokumentation
- [Query Designer – lån och återlämningar](docs/images/queries/Query_AllLoansAndReturns_Result.jpg)  
- [Query Designer – bokinformation](docs/images/queries/Query_BookInformation_Result.jpg)

---

### 2025-11-26 - 2025-12-10
- Skapade två views:
  - v_ActiveLoans (ReturnDate IS NULL)
  - v_ReturnedLoans (ReturnDate IS NOT NULL)

#### Dokumentation
- [v_ActiveLoans.sql](views/v_ActiveLoans.sql)  
- [v_ReturnedLoans.sql](views/v_ReturnedLoans.sql)

- Skapade två stored procedures:
  - RegisterLoan (Lånar bok och sätter DueDate +14 dagar)
  - ReturnBook (Återlämnar bok och skriver ReturnDate)

#### Dokumentation
- [sp_RegisterLoan.sql](procedures/sp_RegisterLoan.sql)  
- [sp_ReturnBook.sql](procedures/sp_ReturnBook.sql)

- Skapade trigger:
  - trg_OnReturnBook (aktiveras när ReturnDate uppdateras)

#### Dokumentation
- [triggers.sql](schema/triggers.sql)

- Strukturerade mappstruktur med sql-filer

---

### 2025-12-12
- Implementerade EF Database First genom att scaffolda hela databasen
- Säkerställde fungerande anslutning mellan EF Core och databasen
- Verifierade att tabellen Book scaffoldades korrekt

#### Dokumentation
- [LibraryDBContext.cs](data/LibraryDBContext.cs)  
- [Book.cs](models/Book.cs)  
- [Loan.cs](models/Loan.cs)  
- [Member.cs](models/Member.cs)  
- [ActiveLoan.cs](models/ActiveLoan.cs)  
- [ReturnedLoan.cs](models/ReturnedLoan.cs)

---

### 2025-12-13
- Verifierade att tabeller och vyer (Member, Loan, ActiveLoans, ReturnedLoans) scaffoldades korrekt
- Testade läsning av samtliga DbSet via ett enkelt konsolprogram för att bekräfta korrekt koppling
- Testet säkerställde att anslutningen upprätades korrekt

#### Dokumentation
- [Program.cs](Program.cs)  
- [LibraryDBContext.cs](data/LibraryDBContext.cs)

---

### 2025-12-14
## Transaktioner

Transaktioner har testats för att säkerställa att databasen hanterar både lyckade och avbrutna operationer korrekt.

Testerna genomfördes genom att:
- registrera ett lån som avslutades med COMMIT
- verifiera att ändringen sparades i databasen
- genomföra ett nytt försök som avslutades med ROLLBACK
- verifiera att inga ändringar sparades efter ROLLBACK

### Sammanfattning
- Tester visar att transaktioner fungerar som förväntat i databasen.
- COMMIT används för att spara ändringar permanent.
- ROLLBACK används för att avbryta en transaktion vid fel.
- Databasen hålls konsekvent i båda fallen.
- Detta bekräftar att transaktionshanteringen fungerar korrekt i praktiken.

#### Dokumentation
- [Registrering av lån med COMMIT](docs/images/transactions/loan_commit.png)  
- [Kontroll efter COMMIT](docs/images/transactions/loan_after_commit.jpg)  
- [Transaktion med ROLLBACK](docs/images/transactions/loan_rollback.jpg)  
- [Verifiering efter ROLLBACK](docs/images/transactions/loan_transaction_after_rollback.jpg)

---

### 2025-12-14
## Index och execution plan

Index har skapats och analyserats för att utvärdera hur SQL Server väljer execution plan vid frågor som filtrerar på aktiva lån.

Testerna genomfördes genom att:
- köra en query mot tabellen Loan utan index
- analysera execution plan
- skapa ett vanligt nonclustered index på ReturnDate
- analysera execution plan igen
- skapa ett filtered nonclustered index för ReturnDate IS NULL
- jämföra resultatet mellan de olika strategierna

### Sammanfattning
- Utan index läses hela tabellen.
- Ett vanligt nonclustered index räcker inte i detta fall.
- Ett filtered index är bättre anpassat för frågan.
- Eftersom datamängden är liten väljer SQL Server ändå ofta scan.
- Detta visar hur SQL Server väljer execution plan baserat på kostnad.

#### Dokumentation
- [Execution plan utan index](docs/images/indexes/index_before_returndate.jpeg)  
- [Execution plan med nonclustered index](docs/images/indexes/index_after_returndate.jpeg)  
- [Execution plan med filtered index](docs/images/indexes/index_after_filtered_returndate.jpg)

---

### 2025-12-15
## Konkurrenshantering

Grundläggande konkurrenshantering har testats genom att simulera samtidiga lån i två separata databassessioner i SQL Server Management Studio.

Testet genomfördes genom att:
- identifiera en bok utan aktivt lån
- starta en transaktion i en session och låsa boken
- samtidigt försöka låna samma bok i en annan session
- verifiera att endast ett aktivt lån kan skapas

När den första transaktionen höll låset blockerades den andra sessionen tills transaktionen avslutades. Efter COMMIT verifierades att endast ett aktivt lån existerade för den aktuella boken, vilket visar att databasen hanterar konkurrens korrekt och bevarar dataintegriteten.

#### Dokumentation
- [Val av bok utan aktivt lån](docs/images/concurrency/concurrency_01_find_available_book.jpg)  
- [Session A – öppen transaktion och lås](docs/images/concurrency/concurrency_02_session_a_lock_open_tran.jpg)  
- [Session B – blockerad](docs/images/concurrency/concurrency_03_session_b_blocked.jpg)  
- [Session A – COMMIT](docs/images/concurrency/concurrency_04_session_a_commit.jpg)  
- [Verifiering av ett aktivt lån](docs/images/concurrency/concurrency_05_verify_single_active_loan.jpg)

---


## Reflektion – optimering och dataintegritet

I det här projektet har fokus legat på att bygga en korrekt och stabil databaslösning snarare än att optimera för stora datamängder.
Optimering har främst hanterats genom att analysera execution plans och jämföra olika indexstrategier. Genom att testa frågor både
utan index, med nonclustered index och med filtered index blev det tydligt hur SQL Server väljer execution plan baserat på kostnad och
aktuell datamängd. Lösningen visar att rätt index blir viktigare i takt med att databasen växer.
Dataintegritet har säkerställts genom tydliga relationer mellan tabeller samt användning av transaktioner. COMMIT och ROLLBACK har
testats för att verifiera att databasen förblir konsekvent vid både lyckade och avbrutna operationer. Grundläggande konkurrenshantering
har även testats genom att simulera samtidiga lån, där låsning förhindrar att samma bok lånas ut flera gånger samtidigt.
Sammantaget visar lösningen hur indexering, transaktioner och låsning bidrar till att systemet fungerar korrekt och att datan hålls konsekvent.

