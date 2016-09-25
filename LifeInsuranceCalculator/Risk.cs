using System;

namespace LifeInsurance
{
    public class Risk
    {
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string Postcode { get; set; }
        public bool Smoker { get; set; }
        public int HoursOfExercisePerWeek { get; set; }
        public bool HaveChildren { get; set; }
    }
}