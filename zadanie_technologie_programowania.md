
# Cel

Celem jest stworzenie multiplatformowej aplikacji na środowiska desktopowe oraz mobilne takie jak Windows 10, Windows 10 Mobile. Całość logiki biznesowej oraz logiki odpowiedzialnej za komunikację z widokiem jest współdzielona i niezależna od platformy pod warunkiem że wspiera .Net framework.

Wykorzystane technologie:
 * Universal Windows Plarform (UWP)
   * Projekty targtujące ```Universal Windows```
 * .Net Portable
   * Projekty typu ```Portable Class Library``` (PCL)

Stworzona aplikacja posłuży jako natywny klient do usługi internetowej (https://myanimelist.net/) pozwalającej na zarządzanie listą obejrzanych japońskich animacji oraz przeczytanej mangi. Umożliwi dodatkowo funkcjonalności społecznościowe takie jak: forum, prywatne wiadomości czy "feed'y" z ostatnią aktywnością użytkownika. Jako, że interfejs graficzny owej strony pozostawia wiele do życzenia użytkownicy zyskają na wygodzie.

# Opis zadania

Ponieważ projekt jest już od pewnego czasu rozwijany zostanie przedstawiona lista prac, które muszą zostać wykonane aby spełnić wymagania minimalne po rozmowie z kierownikiem przedmiotu:
* Zamiana mechanizmu cache'owania pobranych danych z serwera za pośrednictwem REST'owego API na podejście bazodanowe. Dotychczasowo było to realizowane plikowo z danymi serializowanymi do JSON'a.
* Dodanie ręcznej serializacji wiadomości na forum, tak aby przedstawić serializację grafu.
  * Wiadomości mogą być tworzone przez tych samych użytkowników forum, którzy są następnie definiowani przez użytkowników całego serwisu.

# Wytyczne do realizacji

## Wprowadzenie

Cała solucja to ponad 20 projektów podzielonych na 4 foldery, sugeruję użycie do weryfikacji projektów UWP umieszczonych w odpowiednim folderze.
Wymagać to będzie Windows'a 10 oraz Visual Studio 2017 z racji zastosowania C#7 w pewnych miejscach.

Głównymi bliźniaczymi projektami będą "MALClient.UWP.Mobile" oraz "MALClient.UWP.Desktop" można zbudować dowolny z nich. Może być wymagane wygenerowania lokalnego certyfikatu nie załączonego do repozytorium ze względów bezpieczeństwa, generacja jest dostępna oknie właściwości wybranego projektu (zakładka "Signing").

Struktura projektowa wygląda następująco:
* Projekty Widoku - folder "UWP"
  * Projekt(y) główne
  * Adaptery
    * Implementacje interfejsów umożliwiające logice biznesowej wywoływanie natywnych instrukcji, takich jak wywoływanie okien dialogowych, zapis do pliku etc. Są one następnie wstrzykiwane przez konstruktor do wybranych ViewModeli.
  * Projekty dodatkowe
    * Wymagane do osiągnięcia pewnych funkcjonalności platformy lub pomocnicze.
* Projekty Modelu
  * Projekt definicji adapterów (MALClient.Adapters)
  * Projekt definicji surowych klas będących definicjami danych (MALClient.Models) oraz typów wyliczających wraz z ich atrybutami.
  * Projekt zawierający główną logikę całej aplikacji (MALClient.XShared)

Całość jest łączona przy pomocy kontenera IoC do którego rejestrowane są odpowiednie implementacje przy uruchomieniu aplikacji. Możemy to zaobserwować w klasach z "Locator" w nazwie (ViewModelLocator,DesktopViewModelLocator etc.)

Jako baza danych zostało wykorzystane rozwiązane SQLLight ze względu na możliwość współdzielenia możliwie największej ilości kodu pomiędzy platformami. Kontekst bazy danych otrzymywany jest z adaptera ```IDatabaseContextProvider``` a dane są przetwarzane w klasie ```DatabaseService```.

# Lista źródeł

http://www.mvvmlight.net/doc/

https://github.com/praeclarum/sqlite-net/wiki

# Zaliczenie

## Realizacja zakresu zadania

1. Brak błędów kompilatora po wyciągnięciu nowej kopii kodu z repozytorium.
2. Pozytywne wyniki wymaganych testów jednostkowych.
	* Testy dotyczące bazy danych.
	* Testy sprawdzjące działanie serializacji grafu.
3. Wyświetlenie określonych wyników z wykorzystaniem wybranego interfejsu użytkownika.
	* Raz odczytane z internetu dane będą ładowały się szybciej przy wykorzystniu cache'a w postaci bazy danych.

## Osiągnięcie celu - przykładowe zagadnienia

W trakcie zaliczenia mogą być poruszane zagadnienie związane z:
* umiejętnością wytłumaczenia działania multiplatformowej solucji
* wytłumaczeniem działania serializacji

## Harmonogram

Baza danych - 8 zajęcia

Realizacja serializacji - 11 zajęcia
 
