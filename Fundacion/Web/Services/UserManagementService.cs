using Shared.Dtos;
using Shared.Models;
using Web.Http;
using Web.Models.UserManagement;

namespace Web.Services
{
    public class UserManagementService
    {
        private readonly ApiClient _apiClient;

        public UserManagementService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        public async Task<Result<IEnumerable<UserToListDto>>> GetAllUsersAsync()
        {
            var result = await _apiClient.GetAsync<IEnumerable<UserToListDto>>("UserManagement/AllUsers");
            if (result.IsFailure)
                return Result<IEnumerable<UserToListDto>>.Failure(result.Errors);
            return Result<IEnumerable<UserToListDto>>.Success(result.Value);
        }

        public async Task<Result> AddUserAsync(AddUserViewModel model)
        {
            // 1. Convertir ViewModel a DTO
            var dto = new NewUserDto
            {
                Nombre = model.Nombre,
                Apellidos = model.Apellidos,
                Email = model.Email,
                Nacionalidad = model.Nacionalidad,
                Identificacion = model.Identificacion,
                Roles = model.SelectedRoles.ToArray(),
            };

            // 2. Enviar el request al backend
            var response = await _apiClient.PostAsync("UserManagement/AddUser", dto);

            // 3. Validar el resultado
            if (response.IsFailure)
                return Result.Failure(response.Errors);

            return Result.Success();
        }
        public async Task<Result<IEnumerable<RoleDto>>> GetAllRolesAsync()
        {
            var result = await _apiClient.GetAsync<IEnumerable<RoleDto>>("UserManagement/AllRoles");
            if (result.IsFailure)
                return Result<IEnumerable<RoleDto>>.Failure(result.Errors);
            return Result<IEnumerable<RoleDto>>.Success(result.Value);
        }

    }
}

