# TrainingLog

Microservice der håndterer medlemmers personlige træningsprogrammer i FitLife.

## Hvad servicen kan

- Oprette og hente træningsprogrammer — et medlem kan have flere programmer
- Tilføje, opdatere og slette øvelser i et program (navn, sæt, reps, vægt)
- Omdøbe et træningsprogram

Alle endpoints kræver et gyldigt JWT token. Medlemmer kan kun tilgå egne programmer — member-id hentes direkte fra tokenet.

## Struktur

- `Controllers/` — HTTP endpoints
- `Services/` — forretningslogik
- `Repositories/` — MongoDB-adgang
- `Models/` — domæneobjekter (`WorkoutProgram`, `Exercise`)
- `FitLife.TrainingLog.Tests/` — unit tests med NUnit