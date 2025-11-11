namespace Shared.Dtos
{
    public class UserToListDto
    {
        public int Id { get; set; }
        public bool Activo { get; set; } = true;
        public string Identificacion { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
