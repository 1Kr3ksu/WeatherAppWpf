# WeatherAppWPF

Aplikacja okienkowa umożliwiająca sprawdzanie aktualnej pogody oraz prognozy na 3 dni dla miast z całego świata.

## Funkcje aplikacji

- Wyszukiwanie miast z całego świata.
- Wyświetlanie aktualnej temperatury.
- Opis aktualnych warunków pogodowych.
- Informacje o wilgotności powietrza.
- Prędkość wiatru.
- Ciśnienie atmosferyczne.
- Prognoza pogody na 3 dni.
- Prawdopodobieństwo opadów.
- Prognozowane temperatury minimalne i maksymalne.
- Ikony pogodowe.
- Lokalna data i godzina wybranego miasta.
- Odświeżanie danych pogodowych.
- Obsługa błędów połączenia i pobierania danych.

## Działanie aplikacji
- Wpisz nazwę miasta
- Po wpisaniu nazwy miasta kliknij przycisk Odśwież
## Technologie

- **C#**
- **.NET**
- **WPF (Windows Presentation Foundation)**
- **WPF UI**
- **WeatherAPI**
- **HttpClient**
- **System.Text.Json**
- **MVVM**

## Architektura projektu

### Models

Modele przechowujące dane pogodowe, informacje o lokalizacji oraz prognozę na kolejne dni.

### Services

Warstwa odpowiedzialna za komunikację z WeatherAPI i pobieranie danych pogodowych.

### ViewModels

Warstwa zarządzająca danymi aplikacji i komunikacją pomiędzy logiką a interfejsem użytkownika.

### Views

Interfejs graficzny utworzony w XAML, umożliwiający wybór miasta oraz wyświetlanie danych pogodowych.

## Uruchomienie projektu

## Wymagania

### Obecnie

Aby uruchomić aplikację z kodu źródłowego, potrzebujesz:

- Systemu Windows 10 lub Windows 11.
- Visual Studio 2022 lub nowszego.
- Zainstalowanego składnika **.NET Desktop Development** w Visual Studio.
- Wersji .NET wymaganej przez projekt.
- Klucza API z serwisu [WeatherAPI.com](https://www.weatherapi.com/).
- Połączenia z internetem.

### W przyszłości

W niedalekiej przyszłości planowane jest udostępnienie gotowej wersji aplikacji w formacie ZIP lub jako plik wykonywalny `.exe`.

Dzięki temu korzystanie z aplikacji nie będzie wymagało instalowania Visual Studio ani samodzielnego kompilowania kodu.

Gotowa wersja będzie dostępna w sekcji [Releases](https://github.com/1Kr3ksu/WeatherAppWpf/releases) repozytorium GitHub.

### 1. Pobranie projektu - w przyszłości

Do uruchomienia aplikacji wymagany będzie system Windows oraz połączenie z internetem, niezbędne do pobierania aktualnych danych pogodowych.
