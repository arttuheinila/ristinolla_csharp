# Ristinolla (Tic-Tac-Toe)

Ristinolla-sovellus, joka on toteutettu .NET MAUI -alustalla C#:llä ja XAML:llä.  
Pelaaja voi pelata joko toista ihmistä tai tietokonetta vastaan, ja pelitilastot tallentuvat automaattisesti.

## Ominaisuudet
- **Kaikki vaaditut toiminnot:** Pelin perussäännöt (3×3-ruudukko, X/O-merkinnät, vuorotus) toteutettu sekä ihmistä että tietokonetta vastaan.
- **Tilastotiedot ja tiedostotallennus:** Voitot, tappiot ja tasapelit tallentuvat JSON-muodossa ja päivittyvät käyttöliittymässä reaaliaikaisesti.
- **Navigointi ja palautuminen:** Selkeä navigointirakenne `MainPage → Player2Page → TurnSelectionPage → GamePage`, ja “Palaa alkuun” -toiminto vie aina takaisin juureen ilman sovelluksen sulkemista.
- **Käyttäjäkokemus:** Sovellus estää tyhjät syötteet uuden pelaajan lisäämisessä ja näyttää informatiiviset `DisplayAlert`-ilmoitukset.

## Koodin laatu ja arkkitehtuuri
- **Modulaarisuus ja DRY-periaate:** Toiminnot on pilkottu selkeisiin metodeihin (esim. `BuildBoard`, `CheckForWin`, `SwitchTurn`), ja koodi välttää toistoa.
- **Luettavuus ja ylläpidettävyys:** Luokkien vastuut ovat selkeitä, side-efektit minimoitu, ja `INotifyPropertyChanged`-käyttö tekee tilastojen päivityksestä sujuvaa. Kommentointi tukee kokonaisuuden ymmärtämistä.

## Tekniikat
- C# ja XAML (.NET MAUI)
- JSON-tallennus
- Asynkroninen ohjelmointi (`Task.Delay`) tietokonepelaajan vuoroon
