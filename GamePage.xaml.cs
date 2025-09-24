using System;
using System.Text.Json;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace ristinolla
{
    public partial class GamePage : ContentPage
    {
        private readonly Stopwatch _gameTimer = new Stopwatch();
        private readonly PersonInfo _player1;
        private readonly PersonInfo _player2;
        private PersonInfo _currentPlayer;

        //List of the clicked cells
        private List<Button> _cells;

        public GamePage(PersonInfo player1, PersonInfo player2, bool firstIsPlayer1)
        {
            InitializeComponent();

            //Start timer as soon as page is shown
            _gameTimer.Start();

            _player1 = player1;
            _player2 = player2;
            _currentPlayer = firstIsPlayer1 ? _player1 : player2;

            // Shoe chosen names
            Player1Label.Text = $"{_player1.Etunimi} {_player1.Sukunimi}";
            Player2Label.Text = $"{_player2.Etunimi} {_player2.Sukunimi}";
            // Whose turn it is
            CurrentTurnLabel.Text = $"Vuorossa: {_currentPlayer.Etunimi}";

            // Build the gameboard
            BuildBoard();
        }

        private void BuildBoard()
        {
            _cells = new List<Button>(9);

            // Iterate rows
            for (int r = 0; r < 3; r++)
            {
                // Iterate columns
                for (int c =0; c < 3; c++)
                {
                    // Create a clickable button to each cell
                    var btn = new Button
                    {
                        BackgroundColor = Colors.White,
                        FontSize = 48
                    };
                    btn.Clicked += Cell_Clicked;
                    Grid.SetRow(btn, r);
                    Grid.SetColumn(btn, c);
                    BoardGrid.Children.Add(btn);
                    _cells.Add(btn);
                }
            }
        }

        // Logic for clicking
        private void Cell_Clicked(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            string symbol = _currentPlayer == _player1 ? "X" : "O";
            btn.Text = symbol;
            btn.IsEnabled = false;
            // Check winner
            if (CheckForWin(symbol))
            {
                //Update the stats before notifying player to make sure they get updated
                UpdateStats(_currentPlayer);

                if (_currentPlayer.Etunimi == "Tietokone")
                {
                    DisplayAlert("Häviö!", "Hävisit tietokoneelle!", "OK");
                }
                else                      
                {
                    DisplayAlert("Voitto!", $"{_currentPlayer.Etunimi} voitti!", "OK");
                }
                // Reveal the "Palaa alkuun" -button
                BackButton.IsVisible = true;
                return;
            }

            //Chec draws
            if (BoardFull())
            {
                UpdateStats(null);
                DisplayAlert("Tasapeli", "Lauta täynnä", "OK");
                BackButton.IsVisible = true;
                return;
            }
            
            //If no one won and board is not full then switch turn
            SwitchTurn();
        }

        private bool CheckForWin(string symbol)
        {
            // Winning rows as indexes
            int[][] wins = new[]
            {
            //Rivit
            new[]{0,1,2}, new[]{3,4,5}, new[]{6,7,8}, 
            //Kolumnit
            new[]{0,3,6}, new[]{1,4,7}, new[]{2,5,8},
            //Vinot
            new[]{0,4,8}, new[]{2,4,6}
            };

            //If any combination is true then curret player has won
            return wins.Any(w => 
                _cells[w[0]].Text == symbol && _cells[w[1]].Text == symbol && _cells[w[2]].Text == symbol
            );
        }

        private bool BoardFull()
        {
            // Check if any cell is not empty
            return _cells.All(b => !string.IsNullOrEmpty(b.Text));
            
        }

        private void SwitchTurn()
        {
            // Swithing turn from current to the other
            _currentPlayer = _currentPlayer == _player1 ? _player2 : _player1;
            CurrentTurnLabel.Text = $"Vuorossa: {_currentPlayer.Etunimi}";

            // If the new current player is the computer then rename and launch the logic
            if (_currentPlayer.Etunimi == "Tietokone")
                _ = ComputerTurnAsync();
        }

        //Button to navigate to MainPage after ending the game
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            // Navigate to MainPage
            await Navigation.PopToRootAsync();
        }

        private async Task ComputerTurnAsync()
        {
            // Simulate computer "thinking" 0.5-2s
            var rnd = new Random();
            await Task.Delay(rnd.Next(500, 2001));

            // Choose a free cell
            var emptyButtons = BoardGrid.Children.OfType<Button>().Where(b => string.IsNullOrEmpty(b.Text)).ToList();
            // If no free cells then return. Should not happen
            if (emptyButtons.Count == 0)
                return;

            var choice = emptyButtons[rnd.Next(emptyButtons.Count)];

            // Mark the cell
            Cell_Clicked(choice, EventArgs.Empty);
        }

        private bool IsSamePerson(PersonInfo a, PersonInfo b)
        => a.Etunimi == b.Etunimi && a.Sukunimi == b.Sukunimi && a.Syntymavuosi == b.Syntymavuosi; 

        // Update stats
        private void UpdateStats(PersonInfo? winner = null)
        {
            //Stop timer and capture elapsed time
            _gameTimer.Stop();
            var elapsed = _gameTimer.Elapsed;
            
            string filePath = Path.Combine(FileSystem.AppDataDirectory, "persons.json");
            var json = File.ReadAllText(filePath);
            var list = JsonSerializer.Deserialize<List<PersonInfo>>(json) ?? new List<PersonInfo>();
            
            // Ensure both sides exist in the JSON list. Especially for the computer on first game
            foreach (var pl in new[]{ _player1, _player2 })
            {
                if (!list.Any(p => IsSamePerson(p, pl)))
                {
                    // this will give the computer 0,0,0 initial stats if missing
                    list.Add(new PersonInfo(pl.Etunimi, pl.Sukunimi, pl.Syntymavuosi));
                }
            }
            // Find info
            var p1 = list.First(p => IsSamePerson(p, _player1));
            var p2 = list.First(p => IsSamePerson(p, _player2));

            // Add ticks to total of each player
            p1.PlayTimeTicks += elapsed.Ticks;
            p2.PlayTimeTicks += elapsed.Ticks;

            // Update ending stats to players
            // Draw
            if (winner is null)
            {
                p1.Draws++;
                p2.Draws++;
            }
            else
            {
                var loser = winner == _player1 ? p2 : p1;
                var winPlayer = winner == _player1 ? p1 : p2;
                winPlayer.Wins++;
                loser.Losses++;
            }

            // Save to JSON
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(filePath, JsonSerializer.Serialize(list, options));
        }
    }
}