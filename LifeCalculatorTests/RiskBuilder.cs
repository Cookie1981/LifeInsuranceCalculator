using System;
using LifeInsurance;

namespace LifeCalculatorTests
{
    internal class RiskBuilder
    {
        //Set DOB based on age. This means tests will ALWAYS execute in the same way.
        //using specific dates would result in tests failing over time, as the customer ages!
        private DateTime _dateOfBirth = GetDateOfBirthForAge(34);
        private Gender _gender = Gender.Female;
        private string _postcode = "PE26YY";
        private bool _smoker;
        private bool _hasKids;
        private int _hoursExcercisePerWeek;

        public Risk BuildRisk()
        {
            return new Risk
            {
                DateOfBirth = _dateOfBirth,
                Gender = _gender,
                Postcode = _postcode,
                Smoker = _smoker,
                HaveChildren = _hasKids,
                HoursOfExercisePerWeek = _hoursExcercisePerWeek
            };
        }

        public RiskBuilder SetGender(Gender gender)
        {
            _gender = gender;
            return this;
        }

        public RiskBuilder SetSmoker(bool smoker)
        {
            _smoker = smoker;
            return this;
        }

        public RiskBuilder SetHasChildren(bool hasChildren)
        {
            _hasKids = hasChildren;
            return this;
        }

        public RiskBuilder SetHoursOfExcercise(int hoursExcercisePerWeek)
        {
            _hoursExcercisePerWeek = hoursExcercisePerWeek;
            return this;
        }

        public RiskBuilder SetDateOfBirthByAge(int age)
        {
            _dateOfBirth = GetDateOfBirthForAge(age);
            return this;
        }

        private static DateTime GetDateOfBirthForAge(int age)
        {
            return new DateTime(DateTime.Today.Year - age, DateTime.Today.Month, DateTime.Today.Day - 1);
        }

        public RiskBuilder WithPostcode(string postcode)
        {
            _postcode = postcode;
            return this;
        }
    }
}