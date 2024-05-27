using PetSpa.Models.Domain;


namespace PetSpa.Models.DTO.CustomerDTO
{
    public class CustomerDTO
    {

        public Guid AccId { get; set; }

        

        public string FullName { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string CusRank { get; set; } = null!;

        public virtual Account Acc { get; set; } = null!;


        public  List<PetDTO.PetDTO> Pets { get; set; } 
    }
}
