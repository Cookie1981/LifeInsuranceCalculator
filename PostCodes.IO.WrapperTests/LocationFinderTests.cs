using NUnit.Framework;
using PostCodes.IO.Wrapper;

namespace PostCodes.IO.WrapperTests
{
    [TestFixture]
    public class LocationFinderTests
    {
        [Test]
        public void ShouldLookupLocationDetailsByPostcode()
        {
            var locationFinder = new LocationFinder();

            var locationDetails = locationFinder.LookupAddressByPostcode("PE86PR");

            Assert.That(locationDetails.Country, Is.EqualTo("England"));
        }
    }
}
