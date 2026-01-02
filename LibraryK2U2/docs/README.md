<h1 align="center">
  <b>Library (K2U2)</b>
</h1>

## Bibliotekssystem

Det här projektet är ett konsolbaserat bibliotekssystem som hanterar böcker, medlemmar, utlåning och återlämning.
Systemet bygger på en relationsdatabas där varje lån kopplar en bok till en medlem under en bestämd period.

Applikationen innehåller både användarflöden och en administrativ meny för hantering av böcker, medlemmar och lån.
Administratören kan bland annat se aktiva och historiska lån, hantera sena lån, blockera eller avblockera medlemmar samt justera lånedatum vid behov.

Fokus har legat på korrekt databasmodellering och dataintegritet.
Genom transaktioner, index, vyer, lagrade procedurer och kontroller i applikationsflödet säkerställs att ogiltiga eller otillåtna lån inte kan skapas.

Denna README innehåller den fullständiga tekniska dokumentationen och arbetsloggen.
En mer översiktlig projektbeskrivning, installationsguide och körinstruktioner finns i projektets GitHub-README.

👉 [Projekt README](../README.md)


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
- [Query Designer - lån och återlämningar](docs/images/queries/Query_AllLoansAndReturns_Result.jpg)
- [Query Designer - bokinformation](docs/images/queries/Query_BookInformation_Result.jpg)

---

### 2025-11-26 - 2025-12-10
- Skapade två views:
  - `v_ActiveLoans` (ReturnDate IS NULL)
  - `v_ReturnedLoans` (ReturnDate IS NOT NULL)

#### Dokumentation
- [v_ActiveLoans.sql](views/v_ActiveLoans.sql)
- [v_ReturnedLoans.sql](views/v_ReturnedLoans.sql)

- Skapade två stored procedures:
  - `RegisterLoan` - lånar bok och sätter DueDate +14 dagar
  - `ReturnBook` - återlämnar bok och sätter ReturnDate

#### Dokumentation
- [sp_RegisterLoan.sql](procedures/sp_RegisterLoan.sql)
- [sp_ReturnBook.sql](procedures/sp_ReturnBook.sql)

- Skapade trigger:
  - `trg_OnReturnBook` - aktiveras när ReturnDate uppdateras

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
- [Session A - lås](docs/images/concurrency/concurrency_02_session_a_lock_open_tran.jpg)
- [Session B -  blockerad](docs/images/concurrency/concurrency_03_session_b_blocked.jpg)
- [Session A -  COMMIT](docs/images/concurrency/concurrency_04_session_a_commit.jpg)
- [Verifiering av ett aktivt lån](docs/images/concurrency/concurrency_05_verify_single_active_loan.jpg)

---

## Reflektion - optimering och dataintegritet
Projektet fokuserar på korrekt och stabil databasdesign snarare än storskalig optimering.  
Index, transaktioner och låsning har analyserats för att visa hur SQL Server säkerställer konsekvent data och korrekt beteende även vid samtidiga operationer.

---

### 2025-12-17
- Utökade konsolapplikationen med en sammanhållen menystruktur.
- Implementerade återanvändbar `MenuBuilder`.
- Förbättrade `DeleteMember`, `DeleteBook` och `ForceReturn` med tydlig status.
- Lade till `IntroScreen` och `ExitScreen` för en mer professionell helhetsupplevelse.

---

### 2025-12-18 - 2025-12-29
## Utökad lånelogik, medlemsblockering och strukturförbättringar

Under denna period vidareutvecklades både databasen och konsolapplikationen med fokus på dataintegritet, administration och tydligare arkitektur.

### Medlemsblockering
- Utökade `Member` med attributet `IsBlocked`.
- Uppdaterade databasen via `ALTER TABLE`.
- Uppdaterade ER-modellen så att blockstatus ingår i medlemsentiteten.
- Implementerade kontroll vid lånerregistrering:
  - blockerade medlemmar kan inte låna böcker
  - kontroll sker innan transaktion startas

### Medlemsblockering reflektion 
Dataintegriteten säkras genom att blockstatusen ligger på medlemsnivå och alltid kontrolleras
innan ett nytt lån skapas. På så sätt kan en blockerad medlem inte låna böcker, vilket gör att 
felaktiga eller otillåtna lån stoppas direkt i flödet och inte kan ta sig in i databasen


### SQL-samling och databasexport
- Samlade samtliga relevanta queries i:
  - [Queries.sql](schema/Queries.sql)
- Exporterade databasen inklusive schema, data, index och triggers:
  - [LibraryDB_schema_and_data.sql](schema/LibraryDB_schema_and_data.sql)

### Struktur och arkitektur
- Tydligare uppdelning mellan:
  - `menus` - ansvarar endast för navigation
  - `services` - innehåller all affärslogik
- Avvecklade överlappande `AdminService`-logik till respektive service:
  - `BookService`
  - `MemberService`
  - `LoanService`
  - `UserService`
- Rensade menyer från duplicerad och onödig logik.
- Flyttade gemensam logout-bekräftelse till helper.

### Konsolupplevelse
- Förbättrade feedback vid:
  - blockering och avblockering av medlemmar
  - nekade lån
  - administrativa åtgärder

---
	 
### 2025-12-30  
## Kodstädning, helpers och dokumentationsjusteringar

Arbetet fokuserade på att städa upp kodbasen, förbättra återanvändbarhet och 
säkerställa att dokumentationen korrekt speglar den färdiga lösningen.

### Kodstädning
- Identifierade och tog bort oanvänd kod, metoder och klasser utan referenser.
- Rensade bort överflödiga helpers och logik som inte längre används.
- Säkerställde att kvarvarande kod har tydligt ansvar och faktisk användning.
- Förbättrade läsbarhet och underhållbarhet utan att förändra funktionalitet.

### Meny- och UI-förbättringar
- Justerade inmatningsflöden för att undvika dubbla symboler och visuella artefakter.
- Säkerställde konsekvent beteende vid ESC och exit-flöden.

### ER-diagram och dokumentation
- Ersatte tidigare ER-diagram som var ofullständigt.
- Lade in ett uppdaterat och helt korrekt ER-diagram som speglar aktuell databasmodell.

#### Dokumentation
- [ER-diagram - uppdaterad och korrekt modell](docs/images/ER.png)

### Reflektion
Genom kodstädning och strukturförbättringar har lösningen blivit mer robust, lättare att förstå och enklare att vidareutveckla.  
Att konsekvent använda helpers och ta bort oanvänd kod minskar risken för fel och bidrar till en tydligare och mer professionell arkitektur.

---

### 2026-01-02
## README-struktur, dokumentationskoppling och förtydliganden

Dokumentationen delades upp i två tydliga nivåer för att förbättra läsbarhet och struktur.

- Skapade en tydlig uppdelning mellan:
  - GitHub-README (översikt, installation, körning)
  - Docs-README (teknisk dokumentation och arbetslogg)
- Länkade README-filerna till varandra för enkel navigering.

### Databasdokumentation
- Förtydligade att den fullständiga databasexporten (`LibraryDB_schema_and_data.sql`) räcker för att köra projektet.
- Dokumenterade att övriga SQL-filer i `schema`, `views` och `procedures` finns för transparens, analys och referens, men inte krävs för drift.

### Installation och testförutsättningar
- Förtydligade installationsflödet och koppling till LocalDB.
- Dokumenterade standardkonto för administrativ testning:
  - användarnamn: `admin`
  - PIN: `0000`

Syftet med ändringarna var att göra projektet lättare att förstå, testa och granska, utan att duplicera teknisk information.

---
_Joco Borghol_
---