namespace ristinolla;

public partial class TurnSelectionPage : ContentPage
{
    private readonly PersonInfo _player1;
    private readonly PersonInfo _player2;

    public TurnSelectionPage(PersonInfo player1, PersonInfo player2)
    {
        InitializeComponent();
        _player1 = player1;
        _player2 = player2;
    }

    private async void OnPlayer1Clicked(object sender, EventArgs e)
    {
        // Player 1 starts
        await Navigation.PushAsync(new GamePage(_player1, _player2, firstIsPlayer1: true));
    }

    private async void OnPlayer2Clicked(object sender, EventArgs e)
    {
        // Player 2 starts
        await Navigation.PushAsync(new GamePage(_player1, _player2, firstIsPlayer1: false));
    }

    private async void OnRandomClicked(object sender, EventArgs e)
    {
        // Random starter
        Random random = new Random();
        int randomNumber = random.Next(2);
        bool firstIsPlayer1 = randomNumber == 0;
        await Navigation.PushAsync(new GamePage(_player1, _player2, firstIsPlayer1));
    }
}
