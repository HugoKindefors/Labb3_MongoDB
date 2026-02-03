# Labb 3 – Quiz-applikation med MongoDB

Detta projekt är en WPF-baserad Quiz-applikation utvecklad i C# som använder **MongoDB** för att lagra och hantera data.  
Projektet är gjort som en del av **Labb 3 i databaskursen**.

Applikationen använder **MongoDB.Driver** och skapar samt hanterar databasen automatiskt vid körning.

---

## 👥 Grupparbete
Detta projekt är genomfört i par.

Studenter:
- Vendela Magnusson
- Hugo Kindefors

---

## 🧠 Funktionalitet

Applikationen erbjuder följande funktioner:

- Skapa, visa, uppdatera och ta bort **Question Packs**
- Varje Question Pack innehåller:
  - namn
  - svårighetsgrad
  - betänketid
  - frågor med svarsalternativ
- Hantering av **kategorier**:
  - lägga till kategorier
  - ta bort kategorier
  - välja kategori via dropdown
- All data lagras i **MongoDB** istället för JSON-filer

---

## ⚙️ Tekniker & verktyg

- C#
- WPF
- MongoDB
- MongoDB.Driver
- MVVM-arkitektur
- Asynkrona databas-anrop (`async/await`)
- Git & GitHub

---

## ▶️ Köra projektet

### Förutsättningar
- .NET SDK installerat
- MongoDB körs lokalt på `localhost:27017`

### Steg
1. Klona repot:
   ```bash
   git clone <repo-url>
2. Öppna projektet i Visual Studio

3. Starta MongoDB (lokalt eller via Docker)

4. Kör applikationen (F5)

Databasen och demodata skapas automatiskt vid första körningen.

### 📝 Övrigt

- Projektet är uppdelat i Models, Services, ViewModels och Views

- JSON-lagring har ersatts av MongoDB

- All kommunikation med databasen sker asynkront
