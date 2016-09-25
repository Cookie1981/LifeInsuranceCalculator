using System;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace PostCodes.IO.Wrapper
{
    public class AddressFinder : IAddressFinder
    {
        public Address LookupAddressByPostcode(string postcode)
        {
            Address address;

            try
            {
                //TODO: Tidy this class up...It works, but it needs some love!
                var request = WebRequest.Create("https://api.postcodes.io/postcodes/" + postcode) as HttpWebRequest;
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception($"Server error (HTTP {response.StatusCode}: {response.StatusDescription}).");

                    var responseStream = response.GetResponseStream();

                    using (var sr = new StreamReader(responseStream))
                    {
                        var json = sr.ReadToEnd();
                        JObject jObject = JObject.Parse(json);

                        JToken result = jObject["result"];
                        var country = result["country"];

                        address = new Address(country.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new AddressFinderException(postcode, ex);
            }

            return address;
        }
    }
}