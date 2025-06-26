namespace DraftSimulator.API.Models
{
    public class Player
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Team { get; set; } = string.Empty;
        public double Points { get; set; } 
    }
}