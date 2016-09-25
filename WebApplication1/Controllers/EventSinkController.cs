using System.Web.Mvc;
using Microsoft.Owin.BuilderProperties;

namespace WebApplication1.Controllers
{
    public class EventSinkController : Controller
    {
        [HttpPost]
        public ActionResult EnquireyRequested(Person person, bool male)
        {
            var gender = "male";

            if (male == false)
                gender = "female";

            return Json("{name: '" + person.Name + "', surname: '" + person.Surname + "', gender: '" + gender + "'}"); 
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        public CustomerAddress Address { get; set; }
    }

    public class CustomerAddress
    {
        public string PostCode { get; set; }
    }
}