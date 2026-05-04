# WPF Jokes

Egy WPF alkalmazás, amely véletlenszerű vicceket tölt le egy nyilvános API-ról, megjeleníti azokat a felhasználói felületen, majd JSON fájlba menti őket.  
A WPF application that downloads random jokes from a public API, displays them on the user interface, and then saves them to a JSON file.

## Funkciók - Functionality

- 5 véletlenszerű vicc lekérése a `https://official-joke-api.appspot.com/jokes/random` végpontról  
- 5 random jokes retrieved from the `https://official-joke-api.appspot.com/jokes/random` endpoint
- A viccek megjelenítése egy listában (bevezetés — poén formátumban) 
- Displaying the jokes in a list (setup — punchline format)
- A letöltött viccek mentése egy JSON fájlba aszinkron módon 
- Saving the downloaded jokes to a JSON file asynchronously
- Aszinkron hálózati kommunikáció: a felhasználói felület a letöltés közben is válaszkész marad  
- Asynchronous network communication: the user interface remains responsive during the download
- Állapotjelző szöveg (pl. „Fetching jokes..." vagy sikeres/hibaüzenet)

## Használt technológiák - Technologies Used

| Technológia/Technology | Leírás/Description |
|---|---|
| .NET 10 | Futtatókörnyezet / Runtime environment |
| WPF (Windows Presentation Foundation) | Felhasználói felület / User interface |
| `HttpClient` | HTTP GET kérések küldése / Sending HTTP GET requests |
| `System.Text.Json` | JSON sorosítás és deszerializálás / JSON serialization and deserialization |
| `async` / `await` | Aszinkron programozás / Asynchronous programming |

## Az alkalmazás felépítése - Application Structure

#### `FetchJokeAsync`  
Aszinkron metódus, amely egyetlen viccet kér le az alábbi API végpontról:  
An asynchronous method that retrieves a single joke from the following API endpoint:

```text
GET https://official-joke-api.appspot.com/jokes/random
```

A kapott JSON választ `Joke` objektummá alakítja. Hiba esetén `null`-t ad vissza.  
Converts the received JSON response into a `Joke` object. Returns `null` on error.

#### `SaveJokesToJsonAsync`  
A memóriában tárolt vicclistát (`JokeList`) JSON formátumba sorosítja (behúzott, olvasható formában), majd elmenti a `jokes.json` fájlba.  
Serializes the joke list stored in memory (`JokeList`) into formatted JSON and then saves it to the `jokes.json` file.

#### `Joke` (adatmodell)

```csharp
public class Joke
{
    public int    id        { get; set; }
    public string type      { get; set; }  // pl. "general", "programming"
    public string setup     { get; set; }  // a vicc bevezetése
    public string punchline { get; set; }  // a poén
}
```

## Kimenet - Output

A program futása után létrejön egy JSON fájl az alkalmazás könyvtárában, például:  
After running the program, a JSON file is created in the application's directory, for example:

```json
[
  {
    "id": 42,
    "type": "general",
    "setup": "Why don't scientists trust atoms?",
    "punchline": "Because they make up everything!"
  },
  ...
]
```