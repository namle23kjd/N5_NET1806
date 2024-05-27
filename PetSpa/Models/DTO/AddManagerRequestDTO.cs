using System.Text;

namespace PetSpa.Models.DTO
{
    public class AddManagerRequestDTO
    {
        private static int currentManaId;

        static void AddAccountRequestDTO()
        {
            currentManaId = 1000;
        }

        public void AddAccountRequestDTO(Guid Accid, string fullName, string gender, string phoneNumber)
        {
            AccId = Accid;
            ManaId = ++currentManaId; 
            FullName = fullName;
            Gender = gender;
            PhoneNumber = phoneNumber;
        }
        public Guid AccId { get; set; }

        public int ManaId { get; set; }

        public string FullName { get; set; }

        public string Gender { get; set; }

        public string PhoneNumber { get; set; } = null!;
    }
}
