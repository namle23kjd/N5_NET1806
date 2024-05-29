namespace PetSpa.Models.DTO
{
    public class ApiResponse<T>
    {
        public bool Status { get; set; }
        public string Msg { get; set; }
        public T? Data { get; set; }
    }
}
