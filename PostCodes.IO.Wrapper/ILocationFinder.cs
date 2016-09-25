namespace PostCodes.IO.Wrapper
{
    public interface ILocationFinder
    {
        LocationDetails LookupAddressByPostcode(string postcode);
    }
}