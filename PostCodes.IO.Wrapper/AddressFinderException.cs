using System;

namespace PostCodes.IO.Wrapper
{
    public class AddressFinderException : Exception
    {
        public AddressFinderException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public AddressFinderException()
        {
        }
    }
}