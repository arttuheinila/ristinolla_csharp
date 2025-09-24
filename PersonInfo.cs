using System.Text.Json.Serialization;

namespace ristinolla
{
    public class PersonInfo
    {

        public string Etunimi { get; set; }
        public string Sukunimi { get; set; }
        public string Syntymavuosi { get; set; }


        public int Wins   { get; set; }
        public int Losses { get; set; }
        public int Draws  { get; set; }

        public long PlayTimeTicks { get; set; }

        public PersonInfo() { }

        // New player info, default wins, losses and draws
        public PersonInfo(string etunimi, string sukunimi, string syntymavuosi)
        {
            Etunimi = etunimi;
            Sukunimi = sukunimi;
            Syntymavuosi = syntymavuosi;
            Wins = 0;
            Losses = 0;
            Draws = 0;
            PlayTimeTicks = 0;
        }    

        //Ignore in JSON for easy display
        [JsonIgnore]
        public string PlayTimeFormatted => TimeSpan.FromTicks(PlayTimeTicks).ToString(@"hh\:mm\:ss");
    }
}
