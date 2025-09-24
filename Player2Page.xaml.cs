using System.Collections.ObjectModel;
using System.Text.Json;

namespace ristinolla;

public partial class Player2Page : ContentPage
{
    public ObservableCollection<PersonInfo> FilteredPeople { get; set; }
    private readonly PersonInfo _player1;
    public PersonInfo ComputerStats { get; set; } = new PersonInfo("Tietokone", "", "");


    public Player2Page(PersonInfo player1)
    {   
        InitializeComponent();
        _player1 = player1;

        // Load all saved 
        var all = LoadPeopleFromFile();
        // Filter chosen Pelaaja 1
        FilteredPeople = new ObservableCollection<PersonInfo>(
            all.Where(p => !IsSamePerson(p, _player1) && p.Etunimi != "Tietokone")
        );

        // Get Computer Stats
        ComputerStats = all.FirstOrDefault(p => p.Etunimi == "Tietokone")
                    ?? new PersonInfo("Tietokone","","");

        BindingContext = this;
    }

    // Copy LoadPeopleFromFile-method from Mainpagesta
    private List<PersonInfo> LoadPeopleFromFile()
    {
        string filePath = Path.Combine(FileSystem.AppDataDirectory, "persons.json");
        if (!File.Exists(filePath))
            return new List<PersonInfo>();
        
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<PersonInfo>>(json)
            ?? new List<PersonInfo>();
    }

    private bool IsSamePerson(PersonInfo a, PersonInfo b)
    => a.Etunimi == b.Etunimi && a.Sukunimi == b.Sukunimi && a.Syntymavuosi == b.Syntymavuosi; 
    
    private async void OnPlayComputerClicked(object sender, EventArgs e)
    {
        //Create computer player
        var pc = new PersonInfo("Tietokone", "", "");
        await Navigation.PushAsync(new TurnSelectionPage(_player1, pc));
    }

    private async void OnPlayer2Tapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is not PersonInfo player2)
            return;
        await Navigation.PushAsync(new TurnSelectionPage(_player1, player2));
    }
}