using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.models
{
    public class Gym
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public int Contact { get; set; }
        public List<Equipment> Equipment { get; set; }
        public List<Member> Members { get; set; }
        public List<Employee> Employees { get; set; }
        public int MaxFloorCapacity { get; set; }
        public int MaxMembersCount { get; set; }

        public Gym()
        {
            Equipment = new List<Equipment>();

        }
        public Gym(string name, string location, int contact, int maxFloorCapacity, int maxMembersCount)
        {
            Name = name;
            Location = location;
            Contact = contact;
            MaxFloorCapacity = maxFloorCapacity;
            MaxMembersCount = maxMembersCount;
        }
        public void AddEquipment(Equipment equipment)
        {
            Equipment.Add(equipment);
        }

        public void RemoveEquipment(Equipment equipment)
        {
            Equipment.Remove(equipment);
        }

        public bool AddMember(Member member)
        {
            if (Members.Count < MaxMembersCount)
            {
                Members.Add(member);
                return true;
            }
            return false; 
        }

      
        public void RemoveMember(Member member)
        {
            Members.Remove(member);
        }

        public void AddEmployee(Employee employee)
        {
            Employees.Add(employee);
        }

        public void RemoveEmployee(Employee employee)
        {
            Employees.Remove(employee);
        }
    }
}
