namespace Shared.Models
{
    /// <summary>
    /// Representa el resultado de una operación, indicando si fue exitosa o fallida y conteniendo mensajes de error si los hay.
    /// </summary>
    public class Result
    {
        private readonly List<string> _errors = [];

        /// <summary>
        /// Indica si el resultado es exitoso (sin errores).
        /// </summary>
        public bool IsSuccess => _errors.Count == 0;

        /// <summary>
        /// Indica si el resultado es fallido (con errores).
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Lista de mensajes de error asociados al resultado.
        /// </summary>
        public IReadOnlyList<string> Errors => _errors;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="Result"/> sin errores.
        /// </summary>
        public Result() { }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="Result"/> con los errores especificados.
        /// </summary>
        /// <param name="errors">Colección de mensajes de error.</param>
        public Result(IEnumerable<string> errors)
        {
            if (errors != null)
                _errors.AddRange(errors.Where(e => !string.IsNullOrWhiteSpace(e)));
        }

        /// <summary>
        /// Crea un resultado exitoso.
        /// </summary>
        /// <returns>Una nueva instancia de <see cref="Result"/> representando éxito.</returns>
        public static Result Success() => new();

        /// <summary>
        /// Crea un resultado fallido con los mensajes de error especificados.
        /// </summary>
        /// <param name="errors">Mensajes de error.</param>
        /// <returns>Una nueva instancia de <see cref="Result"/> representando fallo.</returns>
        public static Result Failure(params string[] errors) => new(errors);


        /// <summary>
        /// Crea un resultado fallido con los mensajes de error especificados.
        /// </summary>
        /// <param name="errors">Mensajes de error.</param>
        /// <returns>Una nueva instancia de <see cref="Result"/> representando fallo.</returns>
        public static Result Failure(IEnumerable<string> errors) => new(errors);

        /// <summary>
        /// Agrega un mensaje de error al resultado.
        /// </summary>
        /// <param name="error">Mensaje de error a agregar.</param>
        public void AddError(string error)
        {
            if (!string.IsNullOrWhiteSpace(error))
                _errors.Add(error);
        }   
    }

    /// <summary>
    /// Representa el resultado de una operación que retorna un valor, indicando si fue exitosa o fallida y conteniendo mensajes de error si los hay.
    /// </summary>
    /// <typeparam name="T">Tipo del valor retornado en caso de éxito.</typeparam>
    public class Result<T> : Result
    {
        /// <summary>
        /// Valor retornado por la operación si fue exitosa.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="Result{T}"/> con un valor.
        /// </summary>
        /// <param name="value">Valor retornado por la operación.</param>
        public Result(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="Result{T}"/> con los errores especificados.
        /// </summary>
        /// <param name="errors">Colección de mensajes de error.</param>
        public Result(IEnumerable<string> errors) : base(errors) { }

        /// <summary>
        /// Crea un resultado exitoso con un valor.
        /// </summary>
        /// <param name="value">Valor retornado por la operación.</param>
        /// <returns>Una nueva instancia de <see cref="Result{T}"/> representando éxito.</returns>
        public static Result<T> Success(T value) => new(value);

        /// <summary>
        /// Crea un resultado fallido con los mensajes de error especificados.
        /// </summary>
        /// <param name="errors">Mensajes de error.</param>
        /// <returns>Una nueva instancia de <see cref="Result{T}"/> representando fallo.</returns>
        public static new Result<T> Failure(params string[] errors) => new(errors);

        /// <summary>
        /// Crea un resultado fallido con los mensajes de error especificados.
        /// </summary>
        /// <param name="errors">Mensajes de error.</param>
        /// <returns>Una nueva instancia de <see cref="Result{T}"/> representando fallo.</returns>
        public static new Result<T> Failure(IEnumerable<string> errors) => new(errors);


    }
}

