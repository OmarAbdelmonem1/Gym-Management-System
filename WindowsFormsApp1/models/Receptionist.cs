using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.models
{
    class Receptionist : Employee, ISubscriptionObserver
    {
        public Receptionist(int it, string name, int age, string gender, string contactNumber, double salary, int workingDays, string address) 
            : base(it, name, age, gender, contactNumber, salary, workingDays, address)
        {
        }

        public void NotifySubscriptionExpiring(Subscription subscription)
        {
            Console.WriteLine($"Receptionist {Name} received notification: Subscription '{subscription.Name}' is expiring soon.");
        }
    }
}
