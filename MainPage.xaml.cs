using System.Collections.ObjectModel;
using System.Text.Json;

namespace ristinolla;

public partial class MainPage : ContentPage
{

    public ObservableCollection<PersonInfo> People { get; set; }
	public PersonInfo ComputerStats { get; set; } = new PersonInfo("Tietokone", "", "");
    public MainPage()
	{
		InitializeComponent();
        People = new ObservableCollection<PersonInfo>();

		//Load players from the saved file
		LoadPeopleFromFile();

		//Ensure there is computer as a persistent entry
		//if (!People.Any(p => p.Etunimi == "Tietokone"))
		//{
	//		var computer = new PersonInfo("Tietokone", "", "");
//			People.Add(computer);
//			SavePeopleToFileAsync(); 
//		}		

        BindingContext = this;
	}

	// Handle button click
	private async void OnAddInfoClicked(object sender, EventArgs e)
	{
		//Read fields
		string etunimi = EtunimiEntry.Text;
		string sukunimi = SukunimiEntry.Text;
		string syntymavuosi = SyntymavuosiEntry.Text;
	
		//Validate and force user to input something to all entry fields
		if (string.IsNullOrWhiteSpace(etunimi) ||
        string.IsNullOrWhiteSpace(sukunimi) ||
        string.IsNullOrWhiteSpace(syntymavuosi))
		{
			await DisplayAlert(
				"Täytä kaikki kentät",
				"Etunimi, sukunimi ja syntymävuosi ovat pakollisia.",
				"OK");
			return;
		}

		// Add fields to list
		var newPerson = new PersonInfo(etunimi, sukunimi, syntymavuosi);
		People.Add(newPerson);

		// Save updated list
		await SavePeopleToFileAsync();
	}

	public async Task SavePeopleToFileAsync()
	{
		var all = People.ToList();
		//Add computer with stats
		all.Add(ComputerStats);

		var options = new JsonSerializerOptions { WriteIndented = true };
		string json = JsonSerializer.Serialize(all, options);

		// Filepath
        string filePath = Path.Combine(FileSystem.AppDataDirectory, "persons.json");

		// Save new file
		await File.WriteAllTextAsync(filePath, json);
	}

	void LoadPeopleFromFile()
	{
        string filePath = Path.Combine(FileSystem.AppDataDirectory, "persons.json");
		// Return empty if no player file saved
		if (!File.Exists(filePath))
			return;
		
        else
		{
		string json = File.ReadAllText(filePath);
		var list = JsonSerializer.Deserialize<List<PersonInfo>>(json) ?? new();
			
        //Separate computer and people 
        var computer = list.FirstOrDefault(p => p.Etunimi == "Tietokone");
        ComputerStats = computer ?? ComputerStats;
        OnPropertyChanged(nameof(ComputerStats));

        foreach (var p in list.Where(p => p.Etunimi != "Tietokone"))
            People.Add(p);
		}
	}

	//Item selection
	private async void OnPlayer1Tapped(object sender, ItemTappedEventArgs e)
	{
		if (e.Item is not PersonInfo player1)
			return;
		await Navigation.PushAsync(new Player2Page(player1));
	}

    // Reload players for updated stats after a game
    protected override void OnAppearing()
    {
        base.OnAppearing();
		//Clear out old
		People.Clear();
		//Reload from file
		LoadPeopleFromFile();
    }
}