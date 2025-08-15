namespace Manager.API.RequestModels
{
    public class PaginationRequestModel
    {
        private readonly int maxPageSize = 50;

        private int pageSize = 10; // Valor por defecto
        public int PageSize
        {
            get => pageSize;
            set => pageSize = (value > maxPageSize) ? maxPageSize : (value <= 0 ? 1 : value);
        }

        private int pageIndex = 0; // Valor por defecto
        public int PageIndex
        {
            get => pageIndex;
            set => pageIndex = value < 0 ? 0 : value;
        }
    }
}
