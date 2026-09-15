# Citizen Complaint Management Portal

A full-stack, government-themed citizen grievance and complaint management web application built with **ASP.NET Core MVC**, **Entity Framework Core**, **PostgreSQL**, and **SignalR**. Citizens can submit and track civic complaints in real time, while administrators manage, triage, and resolve issues dynamically.

---

## 🚀 Features

### 👤 Citizen Experience
- **Secure Authentication**: Citizen registration and login with salted **BCrypt** password hashing and HTTP-only cookie authentication.
- **Complaint Submission**: File grievances categorized under *Roads*, *Water*, *Electricity*, *Sanitation*, etc.
- **Government Portal Routing**: Automatic guidance and redirection to relevant state civic utility portals upon filing.
- **Editable Submissions**: Citizens can edit their complaints while status is still *Pending*.
- **Real-Time Status Tracking**: Dynamic live status updates (Pending → In Progress → Resolved / Rejected) without page refresh.

### 🛡️ Administrator Operations
- **Centralized Dashboard**: Live view of all complaints submitted across the portal.
- **Status & Feedback Management**: Update complaint resolution stages and add administrative notes visible to the citizen.
- **Live Bell Notifications & Badges**: Real-time SignalR toast popups and unread count badges for incoming grievances.
- **Complaint Management**: Ability to review, update, and remove complaint records.

### ⚡ Real-Time Features (SignalR)
- **Instant Admin Table Updates**: New grievances dynamically appear on the admin panel in real time.
- **Live Status Badges**: Status badge colors and labels update live across connected sessions.
- **Instant Alerts**: Interactive toast notifications notify administrators when new grievances are filed.

---

## 🛠️ Tech Stack

| Component | Technology |
|---|---|
| **Framework** | ASP.NET Core 8 MVC (.NET 8) |
| **Database** | PostgreSQL + Entity Framework Core (`Npgsql`) |
| **Security** | BCrypt.Net-Next Password Hashing, Anti-CSRF Tokens, Cookie Auth |
| **Real-Time** | ASP.NET Core SignalR |
| **Frontend** | Bootstrap 5, Vanilla JavaScript, Razor Views |
| **Deployment** | Docker container ready |

---

## 📂 Project Structure

```
Controllers/     → AccountController, ComplaintsController, NotificationsController
Models/          → Complaint, User, Notification, ErrorViewModel
Services/        → IComplaintService, IUserService, INotificationService, IEmailService
Data/            → AppDbContext, EF Core Database Configurations
Hubs/            → NotificationHub (SignalR live duplex connection)
Views/           → Razor views for Complaints, Authentication, and Shared Layouts
wwwroot/         → Stylesheets (site.css), Client scripts (site.js), Media assets
Migrations/      → EF Core code-first database migrations
```

---

## ⚙️ Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/)

### Setup Instructions

1. **Clone the repository:**
   ```bash
   git clone https://github.com/sharandineshkumar/MVC-Complaint-System.git
   cd MVC-Complaint-System
   ```

2. **Configure Database Connection:**
   In `appsettings.json` (or `appsettings.Development.json`), configure your PostgreSQL connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=ComplaintSystemDB;Username=postgres;Password=YOUR_PASSWORD"
   }
   ```

3. **Run Database Migrations:**
   ```bash
   dotnet ef database update
   ```

4. **Build and Run:**
   ```bash
   dotnet run
   ```
   Open your browser and navigate to `http://localhost:5000` (or the URL shown in the console).

---

## 👥 Role Permissions

| Action | Citizen | Administrator |
|---|:---:|:---:|
| Register / Login | ✅ | ✅ |
| Submit New Grievance | ✅ | ❌ |
| Edit Own Grievance (*Pending* only) | ✅ | ❌ |
| View All Citizen Grievances | ❌ | ✅ |
| Update Status & Add Admin Note | ❌ | ✅ |
| Delete Grievance | ❌ | ✅ |
| Real-time Notification Bell | ❌ | ✅ |

