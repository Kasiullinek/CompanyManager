# Jak korzystać z aplikacji

## Role i dostęp
- **Admin** – może tworzyć, edytować, usuwać i przypisywać zasoby innym użytkownikom
- **Użytkownik** – widzi tylko przypisane zasoby i może oznaczyć je jako wykonane

## Funkcje interfejsu
- Formularz rejestracji i logowania
- Widok zasobów z podziałem na role
- Formularze edycji i tworzenia zasobów
- Obsługa błędów i walidacja danych

## Dostępne endpointy API
Zobacz `https://localhost:{PORT}/swagger` dla pełnej dokumentacji interfejsu API.

## Komunikacja
- Blazor WebAssembly korzysta z `HttpClient`, aby komunikować się z API
- Token JWT przechowywany w `localStorage` służy do autoryzacji zapytań
