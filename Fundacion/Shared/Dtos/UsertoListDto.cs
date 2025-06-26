namespace Shared.Dtos
{
    public class UsertoListDto
    {
        public int Id { get; set; }
        public string Identificacion { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Nacionalidad { get; set; } = string.Empty;
        

    }
}
