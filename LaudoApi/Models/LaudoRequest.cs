namespace LaudoApi.Models
{
    public class LaudoRequest
    {
        public string Temperamento { get; set; } = string.Empty; // Temperamento (s, f, m, c)
        public int Eneagrama { get; set; } // Eneagrama (de 1 a 9)
    }
}
