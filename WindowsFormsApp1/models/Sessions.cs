using System;
using System.Collections.Generic;
using WindowsFormsApp1.models;

public class Session
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Coach Coach { get; set; }
    public int MaxCapacity { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<string> selectedDays { get; set; } = new List<string>();
    public string Description { get; set; }
    public List<Member> Members { get; private set; }

    // Constructor to initialize the session
    public Session( string name, Coach coach, int maxCapacity, TimeSpan startTime, TimeSpan endTime, List<string> selectedDay, string description)
    {
       
        Name = name;
        Coach = coach;
        MaxCapacity = maxCapacity;
        StartTime = startTime;
        EndTime = endTime;
        selectedDays = selectedDays;
        Description = description;
        
    }

    // Method to check if the session occurs on a specific day of the week
    //public bool OccursOnDayOfWeek(List<string> selectedDay)
    //{
    //    if (string.IsNullOrEmpty(DaysOfWeek))
    //        return false;

    //    string[] days = DaysOfWeek.Split(',');
    //    foreach (var day in days)
    //    {
    //        if (Enum.TryParse(day, true, out DayOfWeek parsedDay) && parsedDay == dayOfWeek)
    //            return true;
    //    }
    //    return false;
    //}

    // Method to add a member to the session
    public bool AddMember(Member member)
    {
        if (Members.Count < MaxCapacity)
        {
            Members.Add(member);
            return true;
        }
        else
        {
            return false; // Maximum capacity reached
        }
    }

    
}
