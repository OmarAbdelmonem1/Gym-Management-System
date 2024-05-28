using System;

namespace WindowsFormsApp1.models
{
    public abstract class Employee
    {
        public int id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string ContactNumber { get; set; }
        public double Salary { get; set; }
        public int WorkingDays { get; set; }
        public string Address { get; set; }

        // Full constructor
        public Employee(int id, string name, int age, string gender, string contactNumber, double salary, int workingDays, string address)
        {
            id = id;
            Name = name;
            Age = age;
            Gender = gender;
            ContactNumber = contactNumber;
            Salary = salary;
            WorkingDays = workingDays;
            Address = address;
        }

        // Constructor with id and name
        protected Employee(int id, string name)
        {
            id = id;
            Name = name;
        }

        // Default constructor
        public Employee() { }

        // Method to calculate total salary
        public virtual double CalculateTotalSalary()
        {
            return Salary * WorkingDays;
        }
    }
}
