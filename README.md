# Gym Management System

A comprehensive Windows Forms application for managing gym operations, including member registrations, subscriptions, sessions, equipment, and staff.

## Overview

The Gym Management System is designed to streamline the day-to-day operations of fitness centers and gyms. It provides a user-friendly interface for managing various aspects of gym management, from member registrations to equipment tracking.

## Features

### Member Management
- Register new members
- View and update member profiles
- Track membership status and history
- Manage member subscriptions

### Subscription Management
- Create and manage different subscription plans
- Track subscription renewals and payments
- Generate subscription reports

### Session Management
- Create and schedule fitness sessions
- Assign coaches to sessions
- Track session attendance
- Manage session capacity

### Equipment Management
- Track gym equipment inventory
- Monitor equipment maintenance
- Receive notifications for equipment issues
- Generate equipment reports

### Staff Management
- Manage coaches and receptionists
- Track employee information
- Assign roles and responsibilities

### Dashboard
- View key metrics and statistics
- Monitor gym performance
- Access quick links to common functions

### Authentication
- Secure login system
- Role-based access control
- Credential management

## Technical Details

### Technology Stack
- **Framework**: .NET Framework
- **UI**: Windows Forms
- **Database**: SQL Server
- **Language**: C#

### Project Structure
- **Controllers**: Business logic for different modules
  - CoachController
  - DashboardController
  - EquipmentController
  - And more...

- **Models**: Data models representing business entities
  - Member
  - Coach
  - Equipment
  - Sessions
  - Subscription
  - And more...

- **Views**: User interface forms organized by functionality
  - DashboardForms
  - EquipmentForms
  - MembersForms
  - SessionsForms
  - SubscriptionForms




# Gym Management System

A comprehensive Windows Forms application for managing gym operations, including member registrations, subscriptions, sessions, equipment, and staff.

## Overview

The Gym Management System is designed to streamline the day-to-day operations of fitness centers and gyms. It provides a user-friendly interface for managing various aspects of gym management, from member registrations to equipment tracking.

## Features

### Member Management
- Register new members
- View and update member profiles
- Track membership status and history
- Manage member subscriptions

### Subscription Management
- Create and manage different subscription plans
- Track subscription renewals and payments
- Generate subscription reports

### Session Management
- Create and schedule fitness sessions
- Assign coaches to sessions
- Track session attendance
- Manage session capacity

### Equipment Management
- Track gym equipment inventory
- Monitor equipment maintenance
- Receive notifications for equipment issues
- Generate equipment reports

### Staff Management
- Manage coaches and receptionists
- Track employee information
- Assign roles and responsibilities

### Dashboard
- View key metrics and statistics
- Monitor gym performance
- Access quick links to common functions

### Authentication
- Secure login system
- Role-based access control
- Credential management

## Technical Details

### Technology Stack
- **Framework**: .NET Framework
- **UI**: Windows Forms
- **Database**: SQL Server
- **Language**: C#

### Project Structure
- **Controllers**: Business logic for different modules
  - CoachController
  - DashboardController
  - EquipmentController
  - And more...

- **Models**: Data models representing business entities
  - Member
  - Coach
  - Equipment
  - Sessions
  - Subscription
  - And more...

- **Views**: User interface forms organized by functionality
  - DashboardForms
  - EquipmentForms
  - MembersForms
  - SessionsForms
  - SubscriptionForms

## Screenshots

### Login and Authentication
![Login Screen](WindowsFormsApp1/Images/LoginForm.jpg)
*Secure login interface 

### Dashboard
![Dashboard](WindowsFormsApp1/Images/DashBoardForms/Home.jpg)
*Main dashboard showing key metrics and quick access to all system functions*
## Installation Requirements

- .NET Framework 4.7.2
- SQL Server Express
- Visual Studio 2017 or later (for development)

## Usage

1. **Login**:
   - The application starts with a login screen
   - Use appropriate credentials based on your role

2. **Dashboard**:
   - Navigate through different modules using the dashboard
   - Access member management, subscriptions, sessions, and equipment tracking

3. **Member Registration**:
   - Add new members with personal details
   - Assign subscription plans
   - Track membership status

4. **Session Management**:
   - Create new fitness sessions
   - Assign coaches to sessions
   - Monitor session attendance

5. **Equipment Management**:
   - Track gym equipment inventory
   - Receive notifications for maintenance

## Development

- **Adding New Features**:
  - Follow the MVC-like pattern used in the project
  - Add models in the `models` folder
  - Implement business logic in the `Controller` folder
  - Create UI forms in the `views` folder

- **Database Changes**:
  - Update the Entity Framework model


