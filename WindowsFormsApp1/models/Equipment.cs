using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1.models
{


    //    public interface ISubject
    //    {
    //        void Attach(IObserver observer);
    //        void Detach(IObserver observer);
    //        void Notify();
    //    }

    //    // Define the Observer interface
    //    public interface IObserver
    //    {
    //        void Update(string message);
    //    }

    //    // Concrete subject class
    //    public class Equipment : ISubject
    //    {
    //        private string name;
    //        private string type;
    //        private string model;
    //        private decimal price;
    //        private DateTime maintenanceDate;
    //        private List<IObserver> observers = new List<IObserver>();
    //        private string maintenanceMessage;

    //        public Equipment(string name, string type, string model, decimal price, DateTime maintenanceDate)
    //        {
    //            this.name = name;
    //            this.type = type;
    //            this.model = model;
    //            this.price = price;
    //            this.maintenanceDate = maintenanceDate;
    //        }

    //        // Getters and setters for equipment attributes
    //        public string Name
    //        {
    //            get { return name; }
    //            set { name = value; }
    //        }

    //        public string Type
    //        {
    //            get { return type; }
    //            set { type = value; }
    //        }

    //        public string Model
    //        {
    //            get { return model; }
    //            set { model = value; }
    //        }

    //        public decimal Price
    //        {
    //            get { return price; }
    //            set { price = value; }
    //        }

    //        public DateTime MaintenanceDate
    //        {
    //            get { return maintenanceDate; }
    //            set { maintenanceDate = value; }
    //        }

    //        public void Attach(IObserver observer)
    //        {
    //            observers.Add(observer);
    //        }

    //        public void Detach(IObserver observer)
    //        {
    //            observers.Remove(observer);
    //        }

    //        public void Notify()
    //        {
    //            foreach (var observer in observers)
    //            {
    //                observer.Update(maintenanceMessage);
    //            }
    //        }

    //        // Method to set maintenance message and notify observers
    //        public void SetMaintenanceMessage(string message)
    //        {
    //            maintenanceMessage = message;
    //            Notify();
    //        }
    //    }


    //    // Concrete observer class
    //    public class MaintenanceTeam : IObserver
    //    {
    //        public void Update(string message)
    //        {
    //            // Logic to handle maintenance message update
    //            Console.WriteLine("Maintenance team received message: " + message);
    //        }
    //    }
    //}








    public interface IObserver
    {
        void Update(string message);
    }

    public interface ISubject
    {
        void Attach(IObserver observer);
        void Detach(IObserver observer);
        void Notify();
    }

    public interface IMaintenanceObserver : IObserver
    {
        void UpdateMaintenance(string message);
    }

    public interface IMaintenanceNotificationObserver : IObserver
    {
        void NotifyMaintenanceOverdue(string message);
    }

    public class Equipment : ISubject
    {
        private string name;
        private string type;
        private string model;
        private decimal price;
        private DateTime maintenanceDate;
        private List<IObserver> observers = new List<IObserver>();
        private string maintenanceMessage;

        public Equipment(string name, string type, string model, decimal price, DateTime maintenanceDate)
        {
            this.name = name;
            this.type = type;
            this.model = model;
            this.price = price;
            this.maintenanceDate = maintenanceDate;
        }
        public Equipment(DateTime maintenanceDate)
        {
            this.maintenanceDate = maintenanceDate;
        }

        public DateTime MaintenanceDate
        {
            get { return maintenanceDate; }
        }
        // Getters and setters for equipment attributes
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public string Model
        {
            get { return model; }
            set { model = value; }
        }

        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }

        public void Attach(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (var observer in observers)
            {
                observer.Update(maintenanceMessage);
            }
        }

        public void SetMaintenanceMessage(string message)
        {
            maintenanceMessage = message;
            Notify();
        }

        public bool IsMaintenanceOverdue()
        {
            return DateTime.Now > maintenanceDate;
        }
    }

    public class MaintenanceTeam : IMaintenanceObserver
    {
        public void Update(string message)
        {
            Console.WriteLine("Maintenance team received message: " + message);
            NotificationForm notificationForm = new NotificationForm("Maintenance team received message: " + message);
            notificationForm.ShowNotification();
        }

        public void UpdateMaintenance(string message)
        {
            Update(message);
        }
    }

    public class MaintenanceNotificationObserver : IMaintenanceNotificationObserver
    {
        public void Update(string message)
        {
            ShowNotificationForm(message);
        }

        public void NotifyMaintenanceOverdue(string message)
        {
            Update(message);
        }

        private void ShowNotificationForm(string message)
        {
            NotificationForm notificationForm = new NotificationForm(message);
            notificationForm.ShowNotification();
        }
    }

}