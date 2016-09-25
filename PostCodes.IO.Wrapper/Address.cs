namespace PostCodes.IO.Wrapper
{
    public class Address
    {
        public Address(string country)
        {
            Country = country;
        }

        public string Country { get; }
    }
}