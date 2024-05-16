using System;

namespace WindowsFormsApp1.models
{
    public class Coach : Employee
    {

         public int private_training_cost { get; set; }
        
       

       

        // Constructor for Coach class (using same signature as Employee)
        public Coach(int id,string name, int age, string gender, string contactNumber, double salary, int workingDays, string address,int private_training_cost)
            : base(id,name, age, gender, contactNumber, salary, workingDays, address)
        {
            this.private_training_cost = private_training_cost;
        }
        public Coach(int id, string name) : base(id, name)
        {
           
        }

        public Coach()
        {
        }
    }
}
