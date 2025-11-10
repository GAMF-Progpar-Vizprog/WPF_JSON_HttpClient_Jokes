using System.Net.Http; // HttpClient osztályt tartalmazza, amellyel HTTP kéréseket lehet indítani
using System.Text.Json; // JSON szerializálás és deszerializálás támogatása
using System.Windows;   // WPF alapvető osztályai, például Window, RoutedEventArgs stb.

namespace WPF_Jokes
{
	// A MainWindow osztály egy WPF ablak, amely tartalmazza a felhasználói felületet
	public partial class MainWindow : Window
	{
		// Egy statikus HttpClient példány, amelyet az API hívásokhoz használunk
		// readonly: nem lehet újra értéket adni az objektum példányon belül
		private readonly HttpClient cl = new();

		// Egy lista, amely a letöltött vicceket fogja tárolni (Joke típusú objektumokból áll)
		private List<Joke> JokeList = new List<Joke>();

		// Az ablak konstruktora, ahol inicializálódik a felhasználói felület
		public MainWindow()
		{
			InitializeComponent();
		}

		// Eseménykezelő: amikor a felhasználó rákattint a "Fetch" gombra
		private async void FetchAndSaveJokes_Click(object sender, RoutedEventArgs e)
		{
			// Frissíti a státusz szöveget a felhasználói felületen
			StatusText.Text = "Fetching jokes...";

			// Törli a ListBox tartalmát (ahol a viccek jelennek meg)
			JokeListBox.Items.Clear();

			// Törli a JokeList tartalmát (a program belső listája)
			JokeList.Clear();

			try
			{
				// 10 vicc lekérése egy ciklusban
				for (int i = 0; i < 10; i++)
				{
					// A FetchJokeAsync metódus meghívása, amely egy viccet kér le az API-ról
					var joke = await FetchJokeAsync();

					// Ha a vicc sikeresen le lett kérve (nem null)
					if (joke != null)
					{
						// Hozzáadjuk a viccet a belső listához
						JokeList.Add(joke);

						// Megjelenítjük a vicc "setup" és "punchline" részét a ListBox-ban
						JokeListBox.Items.Add($"{joke.setup} — {joke.punchline}");
					}
				}

				// Elmenti a vicceket JSON formátumba egy fájlba
				await SaveJokesToJsonAsync();

				// Sikeres végrehajtás esetén frissíti a státusz szöveget
				StatusText.Text = $"✅ Successfully fetched and saved {JokeList.Count} jokes to 'jokes.json'";
			}
			catch (Exception ex) // Ha bármilyen hiba történik
			{
				// A hibaüzenetet megjeleníti a státusz mezőben
				StatusText.Text = $"❌ Error: {ex.Message}";
			}
		}

		// Aszinkron metódus, amely egy viccet kér le az API-ról
		private async Task<Joke> FetchJokeAsync()
		{
			try
			{
				// Az API elérési címe 
				string url = "https://official-joke-api.appspot.com/jokes/random";

				// HTTP GET kérés indítása
				var response = await cl.GetAsync(url);

				// Ellenőrzi, hogy a válasz sikeres volt-e (pl. 200 OK)
				response.EnsureSuccessStatusCode();

				// A válasz törzsének beolvasása szövegként
				var json = await response.Content.ReadAsStringAsync();

				// A JSON szöveg deszerializálása Joke objektummá
				Joke? res = JsonSerializer.Deserialize<Joke>(json);

				// Visszatér a vicc objektummal
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

	// A Joke osztály definiálása – ez fogja reprezentálni a vicc objektumot
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