namespace PostCodes.IO.Wrapper
{
    public interface IAddressFinder
    {
        Address LookupAddressByPostcode(string postcode);
    }
}