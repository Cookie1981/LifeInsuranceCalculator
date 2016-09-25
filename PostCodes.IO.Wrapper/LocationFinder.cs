using System;
using System.Configuration;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PostCodes.IO.Wrapper
{
    public class LocationFinder : ILocationFinder
    {
        public LocationDetails LookupAddressByPostcode(string postcode)
        {
            LocationDetails locationDetails;

            try
            {
                var request = WebRequest.Create(PostcodesIoUrl + postcode) as HttpWebRequest;
                using (var response = request.GetResponse() as HttpWebResponse)
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    var json = JObject.Parse(sr.ReadToEnd());
                    locationDetails = JsonConvert.DeserializeObject<LocationDetails>(json["result"].ToString());
                }
            }
            catch (Exception ex)
            {
                throw new AddressFinderException(postcode, ex);
            }

            return locationDetails;
        }

        private string PostcodesIoUrl
        {
            get
            {
                var postcodesIoUrl = ConfigurationManager.AppSettings["PostcodesIoURL"];

                if (postcodesIoUrl == string.Empty)
                    postcodesIoUrl = "https://api.postcodes.io/postcodes/";

                return postcodesIoUrl;
            }
        }
    }
}