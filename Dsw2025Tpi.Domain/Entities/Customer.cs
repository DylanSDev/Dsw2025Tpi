namespace Dsw2025Tpi.Domain.Entities
{
    public class Customer : EntityBase
    {
        public Guid idUser { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public Customer() 
        { }
        public Customer(Guid idUser,string name, string email, string phoneNumber)
        {
            Id = idUser;
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}