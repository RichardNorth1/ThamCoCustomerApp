namespace ThamCoCustomerApp.Dtos
{
    public class CustomerAccountDto
    {

        public string AuthId { get; set; }
        public string Surname { get; set; }
        public string Forename { get; set; }
        public string FullName => Forename + " " + Surname;
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string PostalCode { get; set; }
        public double Balance { get; set; }
    }
}
