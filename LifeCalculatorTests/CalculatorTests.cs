using System;
using LifeInsurance;
using Moq;
using NUnit.Framework;
using PostCodes.IO.Wrapper;

namespace LifeCalculatorTests
{
    [TestFixture]
    public class CalculatorTests
    {
        private Calculator _calculator;
        private LocationDetails _defaultLocationDetails = new LocationDetails("England");
        private Mock<ILocationFinder> _fakeAddressFinder = new Mock<ILocationFinder>();

        [SetUp]
        public void RunBeforeEveryTest()
        {
            _fakeAddressFinder.Setup(x => x.LookupAddressByPostcode(It.IsAny<string>())).Returns(_defaultLocationDetails);
            _calculator = new Calculator(_fakeAddressFinder.Object);
        }

        [TestCase(Gender.Male, 240.00)]
        [TestCase(Gender.Female, 216.00)]
        public void ShouldCalculateLifeInsuranceQuoteBasedOnGender(Gender gender, decimal expectedPremium)
        {
            //ARRANGE
            var customerRisk = new RiskBuilder()
                                    .SetGender(gender)
                                    .BuildRisk();

            //ACT
            var quote = _calculator.CalculateLifeQuote(customerRisk);

            //ASSERT
            Assert.That(quote.Price, Is.EqualTo(expectedPremium));
        }

        [TestCase(true, 864.00)]
        [TestCase(false, 216.00)]
        public void ShouldCalculateLifeInsuranceQuoteForSmoker(bool smoker, decimal expectedPremium)
        {
            //ARRANGE
            var customerRisk = new RiskBuilder()
                                    .SetSmoker(smoker)
                                    .BuildRisk();

            //ACT
            var quote = _calculator.CalculateLifeQuote(customerRisk);

            //ASSERT
            Assert.That(quote.Price, Is.EqualTo(expectedPremium));
        }

        [TestCase(16, 120.00)]
        [TestCase(18, 120.00)]
        [TestCase(19, 198.00)]
        [TestCase(24, 198.00)]
        [TestCase(25, 216.00)]
        [TestCase(34, 216.00)]
        [TestCase(35, 270.00)]
        [TestCase(44, 270.00)]
        [TestCase(45, 378.00)]
        [TestCase(60, 378.00)]
        [TestCase(61, 582.00)]
        [TestCase(70, 582.00)]
        public void ShouldCalculateQuotesBasedOnAge(int age, decimal expectedPremium)
        {
            var customerRisk = new RiskBuilder()
                                    .SetDateOfBirthByAge(age)
                                    .BuildRisk();

            var quote = _calculator.CalculateLifeQuote(customerRisk);

            Assert.That(quote.Price, Is.EqualTo(expectedPremium));
        }

        [TestCase(true, 324.00)]
        [TestCase(false, 216.00)]
        public void ShouldCalculateLifeInsuranceQuoteForParent(bool parent, decimal expectedPremium)
        {
            var customerRisk = new RiskBuilder()
                                    .SetHasChildren(parent)
                                    .BuildRisk();

            var quote = _calculator.CalculateLifeQuote(customerRisk);

            Assert.That(quote.Price, Is.EqualTo(expectedPremium));
        }

        [TestCase(0,216.00)]
        [TestCase(1,180.00)]
        [TestCase(2,180.00)]
        [TestCase(3,126.00)]
        [TestCase(5,126.00)]
        [TestCase(6,90.00)]
        [TestCase(8,90.00)]
        [TestCase(9,270.00)]
        [TestCase(15,270.00)]
        public void ShouldTakeExcerciseIntoAccountWhenQuoting(int hoursExcercisePerWeek, decimal expectedPremium)
        {
            var customerRisk = new RiskBuilder()
                                    .SetHoursOfExcercise(hoursExcercisePerWeek)
                                    .BuildRisk();

            var quote = _calculator.CalculateLifeQuote(customerRisk);

            Assert.That(quote.Price, Is.EqualTo(expectedPremium));
        }

        [TestCase("England", 216.00)]
        [TestCase("Scotland", 456.00)]
        [TestCase("Wales", 96.00)]
        [TestCase("Ireland", 276.00)]
        [TestCase("Northern Ireland", 306.00)]
        [TestCase("Other", 336.00)]
        public void ShouldTakeLocationInAccountWhenQuoting(string location, decimal expectedPremium)
        {
            var customerRisk = new RiskBuilder().BuildRisk();

            _fakeAddressFinder.Setup(x => x.LookupAddressByPostcode(It.IsAny<string>())).Returns(new LocationDetails(location));
            _calculator = new Calculator(_fakeAddressFinder.Object);
//            _calculator = new Calculator(new LocationFinder());

            var quote = _calculator.CalculateLifeQuote(customerRisk);

            Assert.That(quote.Price, Is.EqualTo(expectedPremium));
        }

        [Test]
        public void ShouldNeverQuoteForLessThanMinimumQuotation()
        {
            var customerRisk = new RiskBuilder()
                .SetDateOfBirthByAge(18)
                .SetHoursOfExcercise(9)
                .BuildRisk();

            _fakeAddressFinder.Setup(x => x.LookupAddressByPostcode(It.IsAny<string>())).Returns(new LocationDetails("Wales"));
            _calculator = new Calculator(_fakeAddressFinder.Object);

            var quote = _calculator.CalculateLifeQuote(customerRisk);

            Assert.That(quote.Price, Is.EqualTo(_calculator.MINIMUM_PRICE));
        }

        [Test]
        public void ShouldThrowExceptionWhenUnableToFindBasePrice()
        {
            var risk = new RiskBuilder()
                .SetDateOfBirthByAge(-2)
                .BuildRisk();

            Assert.Throws<CalculatorException>(delegate { _calculator.CalculateLifeQuote(risk); });
        }

        [Test]
        public void ShouldThrowExceptionWhenUnableToFindExcercisePremium()
        {
            var risk = new RiskBuilder()
                .SetHoursOfExcercise(-2)
                .BuildRisk();

            Assert.Throws<CalculatorException>(delegate { _calculator.CalculateLifeQuote(risk); });
        }

        [Test]
        public void ShouldThrowExceptionIfAddressFinderDependencyIsNull()
        {
            _calculator = new Calculator(null);

            Assert.Throws<ArgumentNullException>(
                delegate { _calculator.CalculateLifeQuote(new RiskBuilder().BuildRisk()); }
                );
        }

        [Test]
        public void ShouldDefaultLocationIfLocationLookupFails()
        {
            _fakeAddressFinder.Setup(x => x.LookupAddressByPostcode(It.IsAny<string>())).Throws<AddressFinderException>();
            _calculator = new Calculator(_fakeAddressFinder.Object);

            var customerRisk = new RiskBuilder()
                .BuildRisk();

            var quote = _calculator.CalculateLifeQuote(customerRisk);

            Assert.That(quote.Price, Is.EqualTo(336.00));
        }
    }
}
