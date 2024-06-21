using PetSpa.Models.Domain;

public partial class Admin
{
    public Guid Id { get; set; }
    public Guid AdminId { get; set; }
    public ApplicationUser User { get; set; }
    public virtual List<Manager> Managers { get; set; } = new List<Manager>();
    public virtual List<Customer> Customers { get; set; } = new List<Customer>();
}
