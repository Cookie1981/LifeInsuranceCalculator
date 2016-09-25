using System.Web.Mvc;
using CookiesFinanceSolutions;

namespace WebApplication1.Controllers
{
    public class LifeCalculatorController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CalculateLifeQuote(Risk customerRisk)
        {
            var lifeQuoteCalculator = new LifeInsuranceCalculator(new AddressFinder());

            var calculateLifeQuote = lifeQuoteCalculator.CalculateLifeQuote(customerRisk);

            return new JsonResult {Data = calculateLifeQuote};
        }
    }

    
    

   
}