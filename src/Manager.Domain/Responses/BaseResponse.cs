namespace Manager.Domain.Responses
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }     // ✅ Nuevo: mensaje genérico (éxito o error)
        public string? ErrorCode { get; set; }   // ✅ Opcional: para manejar códigos de error estándar
        public int StatusCode { get; set; }      // ✅ Útil para los controladores
        public string? Details { get; set; }
    }

    public class BaseResponseGeneric<T> : BaseResponse
    {
        public T? Data { get; set; }
    }
}
