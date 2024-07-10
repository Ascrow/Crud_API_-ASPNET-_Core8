namespace Api.Models
{
    public class Usuario
    {
        public long Id { get; set; }
        public required string Nombre { get; set; }
        public required string Pass { get; set; }
        public required long Rol { get; set; }
    }
}
