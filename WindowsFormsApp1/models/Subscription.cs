using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsFormsApp1.models
{

    public abstract class Subscription
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double MonthlyFee { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double TotalPrice { get; set; }
        public string Status { get; set; }
        public List<Services> SelectedServices { get; set; } = new List<Services>();

       
        // Constructor for Subscription class
        public Subscription()
        {
            StartDate = DateTime.Today; // Default to today's date
            EndDate = DateTime.Today.AddMonths(1); // Default to one month from today
            TotalPrice = 0; // Default total price
        }

        // Calculate and return total price based on subscription type
        public virtual double CalculateTotalPrice()
        {
            // Calculate the total number of months between StartDate and EndDate
            int totalMonths = (EndDate.Year - StartDate.Year) * 12 + (EndDate.Month - StartDate.Month);

            // Adjust for the day of the month to determine if we're in a partial month
            if (EndDate.Day < StartDate.Day)
            {
                totalMonths--; // Reduce by one month if end day is less than start day
            }

            // Calculate the total price
            double totalPrice = MonthlyFee * totalMonths;

       

            return totalPrice;
        }
        //public void AddObserver(ISubscriptionObserver observer)
        //{
        //    observers.Add(observer);
        //}

        //public void RemoveObserver(ISubscriptionObserver observer)
        //{
        //    observers.Remove(observer);
        //}

        //protected void NotifyObserversSubscriptionExpiring()
        //{
        //    if (IsExpiringSoon())
        //    {
        //        foreach (var observer in observers)
        //        {
        //            observer.NotifySubscriptionExpiring(this);
        //        }
        //    }
        //}
        //protected abstract bool IsExpiringSoon();

    }

    public class SilverSubscription : Subscription
    {
        public SilverSubscription()
        {
            Name = "Silver";
            MonthlyFee = 100.00;
        }
        public override double CalculateTotalPrice()
        {
            // Calculate the base price using the base class method
            double basePrice = base.CalculateTotalPrice();
            double totalPrice = basePrice;
            return totalPrice;
        }

      
    }

    public class GoldSubscription : Subscription
    {
        public GoldSubscription(List<Services> selectedServices)
        {
            Name = "Gold";
            MonthlyFee = 200.00;
            SelectedServices = selectedServices;
        }

        public override double CalculateTotalPrice()
        {
            int totalMonths = (EndDate.Year - StartDate.Year) * 12 + (EndDate.Month - StartDate.Month);

            // Adjust for the day of the month to determine if we're in a partial month
            if (EndDate.Day < StartDate.Day)
            {
                totalMonths--; // Reduce by one month if end day is less than start day
            }

            // Calculate the total price
            double basePrice = MonthlyFee * totalMonths;
            // Calculate the total price of selected services over the subscription duration
            double servicesPrice = 0.0;

            // Only calculate services price if the subscription duration is greater than 0 months
            if (totalMonths > 0)
            {
                // Calculate the total price of selected services for the actual subscription duration (totalMonths)
                foreach (var service in SelectedServices)
                {
                    servicesPrice += Convert.ToDouble(service.Price);
                }
            }

            // Calculate the total price by adding the base price and the services price
            double totalPrice = basePrice + (servicesPrice * totalMonths);
            return totalPrice;
        }
    }



        public class PlatinumSubscription : Subscription
    {
        public Coach Coach { get; set; }

        public PlatinumSubscription(List<Services> selectedServices, Coach coach)
        {
            Name = "Platinum";
            MonthlyFee = 250.00;
            SelectedServices = selectedServices;
            Coach = coach;
        }

        public override double CalculateTotalPrice()
        {
            int totalMonths = (EndDate.Year - StartDate.Year) * 12 + (EndDate.Month - StartDate.Month);

            // Adjust for the day of the month to determine if we're in a partial month
            if (EndDate.Day < StartDate.Day)
            {
                totalMonths--; // Reduce by one month if end day is less than start day
            }

            // Calculate the total price
            double basePrice = MonthlyFee * totalMonths;
            // Calculate the total price of selected services over the subscription duration
            double servicesPrice = 0.0;

            // Only calculate services price if the subscription duration is greater than 0 months
            if (totalMonths > 0)
            {
                // Calculate the total price of selected services for the actual subscription duration (totalMonths)
                foreach (var service in SelectedServices)
                {
                    servicesPrice += Convert.ToDouble(service.Price);
                }
            }
           
            // Calculate the total price by adding the base price and the services price
            double totalPrice = basePrice + (servicesPrice * totalMonths);
            return totalPrice;
        }
    }

    public interface ISubscriptionFactory
    {
        Subscription CreateSubscription( List<Services> selectedServices, Coach coach);
    }
    public class SilverSubscriptionFactory : ISubscriptionFactory
    {
        public Subscription CreateSubscription(List<Services> selectedServices, Coach coach)
        {
            // Create a Silver subscription
            return new SilverSubscription();
        }
    }

    public class GoldSubscriptionFactory : ISubscriptionFactory
    {
        public Subscription CreateSubscription(List<Services> selectedServices, Coach coach)
        {
            // Create a Gold subscription
            return new GoldSubscription(selectedServices);
        }
    }
    public class PlatinumSubscriptionFactory : ISubscriptionFactory
    {
        public Subscription CreateSubscription(List<Services> selectedServices, Coach coach)
        {
            // Create a Platinum subscription
            return new PlatinumSubscription(selectedServices, coach);
        }
    }

}
