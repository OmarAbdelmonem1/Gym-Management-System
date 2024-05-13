using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.models;

namespace WindowsFormsApp1.models
{
    internal class Member
    {
    }
}
public class Member
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public Subscription Subscription { get; set; }

    public Member(string name, int age, string gender, string email, string phoneNumber, string address, Subscription Subscription)
    {
        Name = name;
        Age = age;
        Gender = gender;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        this.Subscription = Subscription;

    }

    public Member(string name, int age, string gender, string email, string phoneNumber, string address)
    {
        Name = name;
        Age = age;
        Gender = gender;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
    }
}
