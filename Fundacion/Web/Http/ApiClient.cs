using Shared.Models;

namespace Web.Http
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET con respuesta
        public async Task<Result<TResponse>> GetAsync<TResponse>(string url)
        {
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var value = await response.Content.ReadFromJsonAsync<TResponse>();
                return Result<TResponse>.Success(value!);
            }

            var errors = await response.Content.ReadFromJsonAsync<List<string>>();
            return Result<TResponse>.Failure(errors?.ToArray() ?? ["Error desconocido"]);
        }

        // POST sin respuesta
        public async Task<Result> PostAsync<TRequest>(string url, TRequest data)
        {
            var response = await _httpClient.PostAsJsonAsync(url, data);

            if (response.IsSuccessStatusCode)
                return Result.Success();
            string content = await response.Content.ReadAsStringAsync();
            var errors = await response.Content.ReadFromJsonAsync<List<string>>();
            return Result.Failure(errors?.ToArray() ?? ["Error desconocido"]);
        }

        // POST con respuesta
        public async Task<Result<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var response = await _httpClient.PostAsJsonAsync(url, data);

            if (response.IsSuccessStatusCode)
            {
                var value = await response.Content.ReadFromJsonAsync<TResponse>();
                return Result<TResponse>.Success(value!);
            }

            var errors = await response.Content.ReadFromJsonAsync<List<string>>();
            return Result<TResponse>.Failure(errors?.ToArray() ?? ["Error desconocido"]);
        }

        // PUT sin respuesta
        public async Task<Result> PutAsync<TRequest>(string url, TRequest data)
        {
            var response = await _httpClient.PutAsJsonAsync(url, data);

            if (response.IsSuccessStatusCode)
                return Result.Success();

            var errors = await response.Content.ReadFromJsonAsync<List<string>>();
            return Result.Failure(errors?.ToArray() ?? ["Error desconocido"]);
        }

        // PUT con respuesta
        public async Task<Result<TResponse>> PutAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var response = await _httpClient.PutAsJsonAsync(url, data);

            if (response.IsSuccessStatusCode)
            {
                var value = await response.Content.ReadFromJsonAsync<TResponse>();
                return Result<TResponse>.Success(value!);
            }

            var errors = await response.Content.ReadFromJsonAsync<List<string>>();
            return Result<TResponse>.Failure(errors?.ToArray() ?? ["Error desconocido"]);
        }

        // DELETE sin respuesta
        public async Task<Result> DeleteAsync(string url)
        {
            var response = await _httpClient.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
                return Result.Success();

            var errors = await response.Content.ReadFromJsonAsync<List<string>>();
            return Result.Failure(errors?.ToArray() ?? ["Error desconocido"]);
        }

        // DELETE con respuesta
        public async Task<Result<TResponse>> DeleteAsync<TResponse>(string url)
        {
            var response = await _httpClient.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var value = await response.Content.ReadFromJsonAsync<TResponse>();
                return Result<TResponse>.Success(value!);
            }

            var errors = await response.Content.ReadFromJsonAsync<List<string>>();
            return Result<TResponse>.Failure(errors?.ToArray() ?? ["Error desconocido"]);
        }
    }
}
