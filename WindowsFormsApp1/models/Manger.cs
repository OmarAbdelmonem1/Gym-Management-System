using System;

namespace WindowsFormsApp1.models
{
    public class Manager : Employee
    {
        public int TeamBudget { get; set; }

        public Manager(int id, string name, int age, string gender, string contactNumber, double salary, int workingDays, string address, int teamBudget)
            : base(id, name, age, gender, contactNumber, salary, workingDays, address)
        {
            TeamBudget = teamBudget;
        }

        public Manager(int id, string name) : base(id, name)
        {
        }

        public Manager()
        {
        }

        public virtual double CalculateTotalSalary()
        {
            return Salary * WorkingDays;
        }
    }
    
}
