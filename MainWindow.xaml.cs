using System.Net.Http; 
using System.Text.Json; 
using System.Windows;   

namespace WPF_Jokes
{
	public partial class MainWindow : Window
	{
		// Egy HttpClient példány, amelyet az API hívásokhoz használunk
		private readonly HttpClient cl = new();

		// Egy lista, amely a letöltött vicceket fogja tárolni (Joke típusú objektumokból áll)
		private List<Joke?> JokeList = [];

		// Az ablak konstruktora, ahol inicializálódik a felhasználói felület
		public MainWindow()
		{
			InitializeComponent();
		}

		// Eseménykezelő: amikor a felhasználó rákattint a "Fetch" gombra
		private async void FetchAndSaveJokes_Click(object sender, RoutedEventArgs e)
		{
			// Frissítjük a státusz szöveget a felhasználói felületen
			tbStatusText.Text = "Fetching jokes...";
			// Töröljük a ListBox tartalmát (ahol a viccek jelennek meg)
			lbJokeListBox.Items.Clear();
			// Töröljük a JokeList tartalmát (a program belső listája)
			JokeList.Clear();
			try
			{
				// Lekérünk 10 viccet	egy ciklusban
				for (int i = 0; i < 10; i++)
				{
					// Meghívjuk a FetchJokeAsync metódust, ami egy viccet kér le az API-ról
					var joke = await FetchJokeAsync();

					// Ha a vicc sikeresen le lett kérve (nem null)
					if (joke != null)
					{
						// Hozzáadjuk a viccet a listához
						JokeList.Add(joke);

						// Megjelenítjük a vicc "setup" és "punchline" részét a ListBox-ban
						lbJokeListBox.Items.Add($"{joke.setup} — {joke.punchline}");
					}
				}

				// Elmentjük a vicceket JSON formátumba egy fájlba
				await SaveJokesToJsonAsync();

				// Sikeres végrehajtás esetén frissítjük a státusz szöveget
				tbStatusText.Text = $"Successfully fetched and saved {JokeList.Count} jokes to 'jokes.json'";
			}
			catch (Exception ex) // Ha bármilyen hiba történik
			{
				// Megjelenítjük a hibaüzenetet	a státusz mezőben
				tbStatusText.Text = $"Error: {ex.Message}";
			}
		}

		// Aszinkron metódus, amely egy viccet kér le az API-ról
		private async Task<Joke?> FetchJokeAsync()
		{
			try
			{
				// Az API elérési címe 
				string url = "https://official-joke-api.appspot.com/jokes/random";

				// Indítjuk a HTTP GET kérést
				var response = await cl.GetAsync(url);

				// Ellenőizzük, hogy a válasz sikeres volt-e (pl. 200 OK)
				response.EnsureSuccessStatusCode();

				// Beolvaszuk a válasz törzsét szövegként (ez JSON formátumú lesz)
				var json = await response.Content.ReadAsStringAsync();

				// A JSON szöveg deszerializálása Joke objektummá
				Joke? res = JsonSerializer.Deserialize<Joke>(json);

				// Visszatérünk a vicc objektummal
				return res;
			}
			catch // Általános hibakezelés: ha bármi hiba történik, null-t ad vissza
			{
				return null;
			}
		}

		// Metódus, amely a vicceket JSON fájlba menti
		private async Task SaveJokesToJsonAsync()
		{
			// JSON formázási beállítások (pl. szépen behúzott JSON)
			var options = new JsonSerializerOptions { WriteIndented = true };

			// A JokeList listát JSON szöveggé alakítja
			var json = JsonSerializer.Serialize(JokeList, options);

			// A fájl neve, amibe mentünk
			string filePath = "jokes.json";

			// A JSON szöveget fájlba írjuk
			await System.IO.File.WriteAllTextAsync(filePath, json);
		}
	}

	// A Joke osztály – ez ír le egy vicc objektumot
	public class Joke
	{
		// A vicc típusa (pl. "general", "programming")
		public string type { get; set; }

		// A vicc bevezetése (setup)
		public string setup { get; set; }

		// A vicc poénja (punchline)
		public string punchline { get; set; }

		// A vicc azonosítója (id)
		public int id { get; set; }
	}


}