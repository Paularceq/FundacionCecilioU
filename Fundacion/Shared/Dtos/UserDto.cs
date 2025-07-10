using System.Numerics;

namespace Shared.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public bool Activo { get; set; } = true;

        public string NombreCompleto => $"{Nombre} {Apellidos}";
        public string Nombre { get; set; }


        public string Apellidos { get; set; }


        public string Email { get; set; }


        public string Nacionalidad { get; set; }

        public string Identificacion { get; set; }

        public IEnumerable<RoleDto> Roles { get; set; } = [];
    }
}
