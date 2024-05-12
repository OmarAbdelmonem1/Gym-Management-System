using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Employee(int it,string name, int age, string gender, string contactNumber, double salary, int workingDays, string address)
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
    }
}
