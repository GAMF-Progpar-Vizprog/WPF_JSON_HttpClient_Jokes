using System.Net.Http;
using System.Text.Json;
using System.Windows;
namespace WPF_Jokes
{
	public partial class MainWindow : Window
	{
		// Egy HttpClient példány, amelyet az API hívásokhoz használunk			
		// A HttpClient instance used for API calls
		private readonly HttpClient cl = new();
		// Egy lista, amely a letöltött vicceket fogja tárolni (Joke típusú objektumokból áll)
		// A list that will store the downloaded jokes (consisting of Joke type objects)
		private List<Joke?> JokeList = [];
		// Az ablak konstruktora, ahol inicializálódik a felhasználói felület
		// The window constructor, where the user interface is initialized
		public MainWindow()
		{
			InitializeComponent();
		}
		
		private async void miFetchJokes_Click(object sender, RoutedEventArgs e)
		{
			// Frissítjük a státusz szöveget a felhasználói felületen
			// We update the status text on the user interface
			tbStatusText.Text = "Fetching jokes...";
			// Töröljük a TextBox tartalmát (ahol a viccek jelennek meg)
			// We clear the content of the TextBox (where the jokes are displayed)
			tbJokes.Text="";
			// Töröljük a JokeList tartalmát (a program belső listája)
			// We clear the content of JokeList (the program's internal list)
			JokeList.Clear();
			try
			{
				// Lekérünk 5 viccet	egy ciklusban
				// We fetch 5 jokes in a loop
				for (int i = 0; i < 5; i++)
				{
					// Meghívjuk a FetchJokeAsync metódust, ami egy viccet kér le az API-ról
					// We call the FetchJokeAsync method, which retrieves a joke from the API
					var joke = await FetchJokeAsync();
					// Ha a vicc sikeresen le lett kérve (nem null)
					// If the joke was successfully retrieved (not null)
					if (joke != null && joke.type!="error")
					{
						// Hozzáadjuk a viccet a listához
						// We add the joke to the list
						JokeList.Add(joke);
						// Megjelenítjük a vicc "setup" és "punchline" részét a TextBox-ban
						// We display the "setup" and "punchline" parts of the joke in the TextBox
						tbJokes.Text += $"{joke.setup} — {joke.punchline}\n";
					}
				}
				// Elmentjük a vicceket JSON formátumba egy fájlba
				// We save the jokes to a file in JSON format
				await SaveJokesToJsonAsync();
				// Sikeres végrehajtás esetén frissítjük a státusz szöveget
				// In case of successful execution, we update the status text
				tbStatusText.Text = $"Successfully fetched {JokeList.Count} jokes";
			}
			catch (Exception ex) // Ha bármilyen hiba történik
			// If any error occurs
			{
				// Megjelenítjük a hibaüzenetet	a státusz mezőben
				// We display the error message in the status field
				tbStatusText.Text = $"Error: {ex.Message}";
			}
		}
		// Aszinkron metódus, amely egy viccet kér le az API-ról
		// Asynchronous method that retrieves a joke from the API
		private async Task<Joke?> FetchJokeAsync()
		{
			try
			{
				// Az API elérési címe
				// The API endpoint address
				string url = "https://official-joke-api.appspot.com/jokes/random";
				// Indítjuk a HTTP GET kérést
				// We initiate the HTTP GET request
				var response = await cl.GetAsync(url);
				// Ellenőizzük, hogy a válasz sikeres volt-e (pl. 200 OK)
				// We check whether the response was successful (e.g. 200 OK)
				response.EnsureSuccessStatusCode();
				// Beolvasjuk a válasz törzsét szövegként (ez JSON formátumú lesz)
				// We read the response body as text (this will be in JSON format)
				var json = await response.Content.ReadAsStringAsync();
				// A JSON szöveg deszerializálása Joke objektummá
				// Deserializing the JSON text into a Joke object
				Joke? res = JsonSerializer.Deserialize<Joke>(json);
				// Visszatérünk a vicc objektummal
				// We return the joke object
				return res;
			}
			catch // Általános hibakezelés: ha bármi hiba történik, null-t ad vissza
			// General error handling: if any error occurs, it returns null
			{
				return null;
			}
		}
		// Metódus, amely a vicceket JSON fájlba menti
		// Method that saves the jokes to a JSON file
		private async Task SaveJokesToJsonAsync()
		{
			// JSON formázási beállítások (pl. szépen behúzott JSON)
			// JSON formatting options (e.g. nicely indented JSON)
			var options = new JsonSerializerOptions { WriteIndented = true };
			// A JokeList listát JSON szöveggé alakítja
			// Converts the JokeList list into a JSON string
			var json = JsonSerializer.Serialize(JokeList, options);
			// A fájl neve, amibe mentünk
			// The name of the file we are saving to
			string filePath = "jokes.json";
			// A JSON szöveget fájlba írjuk
			// We write the JSON text to a file
			await System.IO.File.WriteAllTextAsync(filePath, json);
			// Sikeres végrehajtás esetén frissítjük a státusz szöveget
			// In case of successful execution, we update the status text
			tbStatusText.Text = $"Successfully saved {JokeList.Count} jokes to {filePath}";
		}
		private async void miSaveJokes_Click(object sender, RoutedEventArgs e)
		{
			// Ellenőrizzük, hogy van-e vicc a listában
			// We check whether there are any jokes in the list
			if (JokeList.Count == 0)
			{
				// Ha nincs, tájékoztatjuk a felhasználót és kilépünk a metódusból
				// If not, we inform the user and exit the method
				tbStatusText.Text = "No jokes to save.";
				return;
			}
			// Létrehozunk egy fájlmentés párbeszédablakot
			// We create a file save dialog
			var dialog = new Microsoft.Win32.SaveFileDialog
			{
				// Csak JSON fájlokat engedünk menteni
				// We only allow saving JSON files
				Filter = "JSON files (*.json)|*.json",
				// Alapértelmezett kiterjesztés, ha a felhasználó nem adja meg
				// Default extension if the user does not provide one
				DefaultExt = ".json",
				// Alapértelmezett fájlnév a párbeszédablakban
				// Default file name in the dialog
				FileName = "jokes"
			};
			// Megjelenítjük a párbeszédablakot, és csak akkor folytatjuk, ha a felhasználó az "OK"-ra kattintott
			// We display the dialog and only continue if the user clicked "OK"
			if (dialog.ShowDialog() == true)
			{
				// JSON formátumban
				// In JSON format
				var options = new JsonSerializerOptions { WriteIndented = true };
				var json = JsonSerializer.Serialize(JokeList, options);
				// A JSON szöveget aszinkron módon fájlba írjuk a felhasználó által megadott elérési útra
				// We write the JSON text asynchronously to the file path specified by the user
				await System.IO.File.WriteAllTextAsync(dialog.FileName, json);
				// Tájékoztatjuk a felhasználót a sikeres mentésről, megjelenítve a mentett viccek számát és a fájl elérési útját
				// We inform the user of the successful save, showing the number of saved jokes and the file path
				tbStatusText.Text = $"Saved {JokeList.Count} jokes to '{dialog.FileName}'";
			}
		}
	}
	// A Joke osztály – ez ír le egy vicc objektumot
	// The Joke class – this describes a joke object
	public class Joke
	{
		// A vicc típusa (pl. "general", "programming")
		// The type of the joke (e.g. "general", "programming")
		public string type { get; set; }
		// A vicc bevezetése (setup)
		// The setup of the joke
		public string setup { get; set; }
		// A vicc poénja (punchline)
		// The punchline of the joke
		public string punchline { get; set; }
		// A vicc azonosítója (id)
		// The identifier of the joke (id)
		public int id { get; set; }
	}
}