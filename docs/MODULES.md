# Moduły aplikacji

## Interfejs użytkownika (UI)
- Zbudowany w Blazor WebAssembly
- Widoki: Login, Register, Lista zasobów (dla Admina i Usera), formularze edycji
- Łączy się z API przez `HttpClient`

## Backend (API)
- ASP.NET Core Web API
- Kontrolery:
  - `AuthController` – rejestracja i logowanie, operacje CRUD na użytkownikach
  - `ResourceController` – operacje CRUD na zasobach
  - `WeatherController` - pomocniczy kontroler do testów autoryzacji.
- Baza danych: SQL Server + EF Core (Code First)
- Autoryzacja: JWT

## SharedLibrary
Biblioteka klas wspólnych dla frontend i backend.

### Zawartość:
- **DTOs** – obiekty transferowe używane do komunikacji między warstwami (np. `UserDto`, `ResourceDto`)
- **Interfaces** – kontrakty do implementacji przez backend, wykorzystywane np. do wstrzykiwania zależności
- **GenericsModel** – klasa statyczna zawierająca metody pomocnicze wykorzystywane w całej aplikacji – głównie do:
    - przetwarzania JWT (tworzenie i odczyt ClaimsPrincipal),
    - konwersji obiektów do/z JSON (System.Text.Json),
    - generowania obiektu StringContent do komunikacji przez HttpClient.

### Rola:
- Gwarantuje spójność danych między Blazor a Web API
- Redukuje powielanie kodu

## Testy
- **API**: testy jednostkowe kontrolerów z `xUnit` i `Moq`
- **UI**: testy end-to-end przy użyciu `Playwright`
- Pokrycie testami podstawowej funkcjonalności aplikacji i komunikacji między komponentami
