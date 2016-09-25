using System;
using System.Collections.Generic;
using System.Linq;
using PostCodes.IO.Wrapper;

namespace LifeInsurance
{
    public class Calculator
    {
        private readonly ILocationFinder _locationFinder;
        private Dictionary<BasePrice, decimal> _basePremiums;
        private Dictionary<ExcerciseBracket, decimal> _excercisePremiums;
        private Dictionary<string, decimal> _locationPremiums;

        public readonly decimal MINIMUM_PRICE = 50.00M;
        private const decimal DEFAULT_REGIONAL_HEALTH_INDEX = 100;
        private const int PREMIUM_FOR_SMOKERS = 300;
        private const int PREMIUM_FOR_PARENTS = 50;
        private const int PREMIUM_FOR_NON_PARENTS = 0;
        private const int PREMIUM_FOR_NON_SMOKERS = 0;

        public Calculator(ILocationFinder locationFinder)
        {
            _locationFinder = locationFinder;

            InitialiseeBasePriceLookup();
            InitialiseExcercisePremiumLookup();
            InitialiseLocationsPremiumLookup();
        }

        private void InitialiseLocationsPremiumLookup()
        {
            _locationPremiums = new Dictionary<string, decimal>
            {
                {"england", 0},
                {"wales", -100},
                {"scotland", 200},
                {"northern ireland", 75},
                {"ireland", 50}
            };
        }

        private void InitialiseExcercisePremiumLookup()
        {
            _excercisePremiums = new Dictionary<ExcerciseBracket, decimal>
            {
                {new ExcerciseBracket(0, 0), +20},
                {new ExcerciseBracket(1, 2), 0},
                {new ExcerciseBracket(3, 5), -30},
                {new ExcerciseBracket(6, 8), -50},
                {new ExcerciseBracket(9, int.MaxValue), +50}
            };
        }

        private void InitialiseeBasePriceLookup()
        {
            _basePremiums = new Dictionary<BasePrice, decimal>
            {
                {new BasePrice(0, 18, Gender.Male), (decimal) 150.00},
                {new BasePrice(0, 18, Gender.Female), (decimal) 100.00},
                {new BasePrice(19, 24, Gender.Male), (decimal) 180.00},
                {new BasePrice(19, 24, Gender.Female), (decimal) 165.00},
                {new BasePrice(25, 34, Gender.Male), (decimal) 200.00},
                {new BasePrice(25, 34, Gender.Female), (decimal) 180.00},
                {new BasePrice(35, 44, Gender.Male), (decimal) 250.00},
                {new BasePrice(35, 44, Gender.Female), (decimal) 225.00},
                {new BasePrice(45, 60, Gender.Male), (decimal) 320.00},
                {new BasePrice(45, 60, Gender.Female), (decimal) 315.00},
                {new BasePrice(61, Int32.MaxValue, Gender.Male), (decimal) 500.00},
                {new BasePrice(61, Int32.MaxValue, Gender.Female), (decimal) 485.00}
            };
        }

        public LifeQuote CalculateLifeQuote(Risk customerRisk)
        {
            var customerAge = CalculateCustomerAgeFromDOB(customerRisk.DateOfBirth);
            var basePrice = GetBasePrice(customerAge, customerRisk.Gender);
            var regionalHealthIndex = GetRegionalHealthIndex(customerRisk.Postcode);
            var childrenPremium = GetChildrenPremium(customerRisk.HaveChildren);
            var smokerPremium = GetSmokerPremium(customerRisk.Smoker);
            var excercisePremium = GetExcercisePremium(customerRisk.HoursOfExercisePerWeek);

            var price = CalculateTotalPremium(basePrice, regionalHealthIndex, childrenPremium, smokerPremium, excercisePremium);

            return new LifeQuote(price);
        }

        private decimal CalculateTotalPremium(decimal basePrice, decimal regionalHealthIndex, decimal childrenPremium, decimal                                                   smokerPremium, decimal excercisePremium)
        {
            var price = basePrice + regionalHealthIndex;
            price += ModifyPriceByPercentage(price, childrenPremium);
            price += ModifyPriceByPercentage(price, smokerPremium);
            price += ModifyPriceByPercentage(price, excercisePremium);

            return ApplyMinimumPriceRule(price);
        }

        private static decimal ModifyPriceByPercentage(decimal originalPrice, decimal percentage)
        {
            return (originalPrice/100)*percentage;
        }

        private decimal ApplyMinimumPriceRule(decimal price)
        {
            if (price < MINIMUM_PRICE)
                price = MINIMUM_PRICE;
            return price;
        }

        private decimal GetExcercisePremium(int hoursOfExercisePerWeek)
        {
            var excercisePremium = _excercisePremiums.Where(excerciseBracket =>
                                                                excerciseBracket.Key.MinHours <= hoursOfExercisePerWeek &&
                                                                excerciseBracket.Key.MaxHours >= hoursOfExercisePerWeek);

            if (!excercisePremium.Any())
                throw new CalculatorException($"No matching Excercise Bracket was found for {hoursOfExercisePerWeek} hourse of excerise per week.");

            return excercisePremium.ElementAt(0).Value;
        }

        private decimal GetSmokerPremium(bool smoker)
        {
             return smoker ? PREMIUM_FOR_SMOKERS : PREMIUM_FOR_NON_SMOKERS;
        }

        private decimal GetChildrenPremium(bool haveChildren)
        {
            return haveChildren ? PREMIUM_FOR_PARENTS : PREMIUM_FOR_NON_PARENTS;
        }

        private decimal GetRegionalHealthIndex(string postcode)
        {
            if (_locationFinder == null)
                throw new ArgumentNullException("_addressFinder");

            var address = new LocationDetails("other");
            try
            {
                address = _locationFinder.LookupAddressByPostcode(postcode);
            }
            catch
            {
                // ignored
            }

            return LookupRegionalHealthIndexByCountry(address.Country);
        }

        private decimal LookupRegionalHealthIndexByCountry(string country)
        {
            return _locationPremiums.ContainsKey(country.ToLower()) ? _locationPremiums[country.ToLower()] : DEFAULT_REGIONAL_HEALTH_INDEX;
        }

        private decimal GetBasePrice(int customerAge, Gender customerGender)
        {
            var basePremium = _basePremiums.Where(basePrice =>    basePrice.Key.StartingAge <= customerAge
                                                               && basePrice.Key.EndingAge >= customerAge
                                                               && basePrice.Key.Gender == customerGender);

            if (!basePremium.Any())
                throw new CalculatorException(
                    $"No base price could be found for the Age and Gender Specified. Age: {customerAge}, Gender{customerGender}");

            return basePremium.ElementAt(0).Value;

        }

        private int CalculateCustomerAgeFromDOB(DateTime dateOfBirth)
        {
            return DateTime.Today.Year - dateOfBirth.Year;
        }

        private class ExcerciseBracket
        {

            public ExcerciseBracket(int minHours, int maxHours)
            {
                MinHours = minHours;
                MaxHours = maxHours;
            }

            public int MinHours { get; }
            public int MaxHours { get; }
        }

        private class BasePrice
        {
            public BasePrice(int startingAge, int endAge, Gender gender)
            {
                StartingAge = startingAge;
                EndingAge = endAge;
                Gender = gender;
            }

            public int StartingAge { get; }
            public int EndingAge { get; }
            public Gender Gender { get; }
        }

    }
}