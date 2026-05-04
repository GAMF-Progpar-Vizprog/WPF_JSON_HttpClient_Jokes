# WPF Jokes

Egy WPF alkalmazás, amely véletlenszerű vicceket tölt le egy nyilvános API-ról, megjeleníti azokat a felhasználói felületen, majd JSON fájlba menti őket.

## Funkciók

- 5 véletlenszerű vicc lekérése a `https://official-joke-api.appspot.com/jokes/random` végpontról
- A viccek megjelenítése egy listában (bevezetés — poén formátumban)
- A letöltött viccek mentése egy JSON fájlba aszinkron módon
- Aszinkron hálózati kommunikáció: a felhasználói felület a letöltés közben is válaszkész marad
- Állapotjelző szöveg (pl. „Fetching jokes..." vagy sikeres/hibaüzenet)

## Használt technológiák

| Technológia | Leírás |
|---|---|
| .NET 10 | Futtatókörnyezet |
| WPF (Windows Presentation Foundation) | Felhasználói felület |
| `HttpClient` | HTTP GET kérések küldése |
| `System.Text.Json` | JSON sorosítás és deszerializálás |
| `async` / `await` | Aszinkron programozás |

## Az alkalmazás felépítése

#### `FetchJokeAsync`
Aszinkron metódus, amely egyetlen viccet kér le az alábbi API végpontról:
```
GET https://official-joke-api.appspot.com/jokes/random
```
A kapott JSON választ `Joke` objektummá alakítja. Hiba esetén `null`-t ad vissza.

#### `SaveJokesToJsonAsync`
A memóriában tárolt vicclistát (`JokeList`) JSON formátumba sorosítja (behúzott, olvasható formában), majd elmenti a `jokes.json` fájlba.

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

## Kimenet

A program futása után létrejön egy JSON fájl az alkalmazás könyvtárában, például:

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




