namespace PostCodes.IO.Wrapper
{
    public class LocationDetails
    {
        public LocationDetails(string country)
        {
            this.country = country;
        }
         
        internal string country { get; set; }
        public string Country => country;

        internal string postcode { get; set; }
        public string Postcode => postcode;

        internal string parish { get; set; }
        public string Parish => parish;
    }
}