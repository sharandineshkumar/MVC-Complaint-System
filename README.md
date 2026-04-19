# 🏛️ Citizen Complaint Management Portal

<div align="center">

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![SignalR](https://img.shields.io/badge/SignalR-Real--Time-00B4AB?style=for-the-badge&logo=signalr&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)

**A full-stack ASP.NET Core MVC web application that enables citizens to submit, track, and manage grievances — while giving administrators real-time oversight and control.**

🌐 **[Live Demo → citizen-complaint-portal.onrender.com](https://citizen-complaint-portal.onrender.com)**

</div>

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Screenshots](#-screenshots)
- [Project Structure](#-project-structure)
- [Getting Started](#-getting-started)
- [Database Setup](#-database-setup)
- [How It Works](#-how-it-works)
- [Role-Based Access](#-role-based-access)
- [Real-Time Features](#-real-time-features)
- [Email Notifications](#-email-notifications)

---

## 🌟 Overview

The **Citizen Complaint Management Portal** is a government-style grievance management system built with ASP.NET Core MVC (.NET 10). Citizens can register, submit complaints across various categories, and track their status in real time. Administrators receive live notifications, manage all complaints, update statuses with notes, and can redirect citizens to relevant official government portals.

This project was built as a complete learning exercise covering authentication, authorization, real-time communication, email integration, and clean MVC architecture.

---

## ✨ Features

### 👤 Citizen Features
- **Register & Login** — Secure cookie-based authentication
- **Submit Complaints** — With title, description, and category selection
- **Track Status** — View complaint status (Pending / In Progress / Resolved / Rejected)
- **Edit Complaints** — Citizens can edit their own complaint if it's still Pending
- **Admin Notes** — See notes left by the admin explaining status changes
- **Portal Redirect** — After submitting, citizens are directed to the relevant official government portal based on their complaint category

### 🛡️ Admin Features
- **View All Complaints** — Complete list from all citizens
- **Update Status** — Change complaint status with an optional note to the citizen
- **Delete Complaints** — Remove complaints from the system
- **Real-Time Bell Notifications** — Live notification badge in the navbar whenever a new complaint is submitted or edited
- **Email Alerts** — Receive an email with complaint details every time a new complaint is submitted
- **Mark Notifications as Read / Delete** — Manage the notification feed

### ⚡ Real-Time (SignalR)
- Complaint table updates live without page refresh for both Admin and Citizens
- New complaint rows appear instantly on the Admin's screen
- Status badge updates live on Citizen's screen when Admin changes status
- Rows are removed live when Admin deletes a complaint
- Bell badge count updates in real time

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| **Framework** | ASP.NET Core MVC (.NET 10) |
| **ORM** | Entity Framework Core 10 |
| **Database** | Microsoft SQL Server (LocalDB / SQL Express) |
| **Authentication** | ASP.NET Core Cookie Authentication |
| **Real-Time** | SignalR (Microsoft.AspNetCore.SignalR) |
| **Email** | System.Net.Mail (Gmail SMTP) |
| **Frontend** | Bootstrap 5, Custom CSS (CSS Variables + Sora font) |
| **Architecture** | MVC + Service Layer (Interface-based DI) |

---

## 📁 Project Structure

```
MVC_Project/
│
├── Controllers/
│   ├── AccountController.cs        # Register, Login, Logout
│   ├── ComplaintsController.cs     # All complaint CRUD + SignalR events
│   └── NotificationsController.cs  # Bell notification API endpoints
│
├── Models/
│   ├── Complaint.cs                # Complaint entity
│   ├── User.cs                     # User entity
│   └── Notification.cs             # Admin notification entity
│
├── Services/
│   ├── IComplaintService.cs        # Complaint service interface
│   ├── ComplaintService.cs         # Complaint business logic
│   ├── IUserService.cs             # User service interface
│   ├── UserService.cs              # User registration & login logic
│   ├── INotificationService.cs     # Notification interface
│   ├── NotificationService.cs      # Notification CRUD
│   ├── IEmailService.cs            # Email interface
│   └── EmailService.cs             # Gmail SMTP email sending
│
├── Data/
│   └── AppDbContext.cs             # EF Core DbContext
│
├── Hubs/
│   └── NotificationHub.cs          # SignalR Hub
│
├── Migrations/                     # EF Core migration history
│
├── Views/
│   ├── Complaints/
│   │   ├── Index.cshtml            # Complaint list (Admin sees all, Citizen sees own)
│   │   ├── Create.cshtml           # Submit new complaint
│   │   ├── Details.cshtml          # View full complaint details
│   │   ├── Edit.cshtml             # Admin: update status + note
│   │   ├── CitizenEdit.cshtml      # Citizen: edit pending complaint
│   │   └── RedirectToPortal.cshtml # Post-submit portal redirect page
│   ├── Account/
│   │   ├── Login.cshtml
│   │   └── Register.cshtml
│   └── Shared/
│       └── _Layout.cshtml          # Global layout with navbar & bell
│
├── wwwroot/
│   ├── css/site.css                # Full custom government-style CSS
│   └── js/site.js                  # SignalR client + live table logic
│
├── appsettings.json
└── Program.cs                      # App setup, DI, middleware pipeline
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or LocalDB
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code

### 1. Clone the Repository

```bash
git clone https://github.com/YOUR_USERNAME/citizen-complaint-portal.git
cd citizen-complaint-portal
```

### 2. Configure the Connection String

Open `appsettings.json` and update the connection string to match your SQL Server instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=ComplaintSystemDB;Trusted_Connection=True;TrustServerCertificate=True"
}
```

### 3. Apply Migrations

```bash
dotnet ef database update
```

This will create the `ComplaintSystemDB` database with all required tables.

### 4. Run the Application

```bash
dotnet run
```

Navigate to `https://localhost:7051` in your browser.

---

## 🗄️ Database Setup

The database is managed using **EF Core Code-First Migrations**. Three migrations are included:

| Migration | What It Creates |
|-----------|----------------|
| `InitialCreate` | `Complaints` table + `Users` table |
| `AddNotifications` | `Notifications` table |
| `AddAdminNoteToComplaint` | `AdminNote` column on Complaints |

Run `dotnet ef database update` to apply all migrations automatically.

### Tables

**Complaints**
```
Id | Title | Description | Category | Status | SubmittedBy | datetime | UpdatedOn | AdminNote
```

**Users**
```
Id | FullName | Email | PasswordHash | Role | CreatedOn
```

**Notifications** *(Admin only)*
```
Id | Message | IsRead | CreatedOn
```

---

## ⚙️ How It Works

### Complaint Lifecycle

```
Citizen Submits → Pending
                      ↓
              Admin Reviews
                /    |    \
        InProgress  Resolved  Rejected
                         ↑
               (with optional Admin Note)
```

1. Citizen submits a complaint → status is automatically set to **Pending**
2. Admin receives an **email** and a **real-time bell notification**
3. Admin can update the status and add a note visible to the citizen
4. If the complaint is still **Pending**, the citizen can edit it
5. Live updates propagate to both Admin and Citizen tables via **SignalR**

### Portal Redirect (After Submission)

After submitting, citizens are redirected to a portal selection page which links to the relevant official Tamil Nadu government portal based on the complaint category:

| Category | Official Portal |
|----------|----------------|
| Roads, Water, Sanitation | [CMWSSB Grievance Portal](https://cmwssb.tn.gov.in/complaints-grievance) |
| Electricity | [TNEB Online Portal](https://www.tnebltd.gov.in/cgrfonline/) |
| Other | [Madurai Corporation](https://maduraicorporation.co.in/) |

---

## 🔐 Role-Based Access

The application has two roles managed via **Cookie Authentication + Claims**:

| Feature | Citizen | Admin |
|---------|---------|-------|
| View own complaints | ✅ | — |
| View all complaints | ❌ | ✅ |
| Submit complaint | ✅ | ❌ |
| Edit own pending complaint | ✅ | ❌ |
| Update complaint status | ❌ | ✅ |
| Add admin note | ❌ | ✅ |
| Delete complaints | ❌ | ✅ |
| Bell notifications | ❌ | ✅ |
| Email alerts | ❌ | ✅ |

> **To create an Admin account:** Register normally, then manually change the `Role` column value from `"Citizen"` to `"Admin"` in the `Users` table via SQL Server Management Studio (SSMS).

---

## ⚡ Real-Time Features (SignalR)

SignalR is used to push live updates to all connected browser tabs without any page refresh.

### Events

| SignalR Event | Triggered When | Effect |
|---------------|---------------|--------|
| `ReceiveNotification` | New complaint submitted or edited | Admin bell badge increments + toast popup appears |
| `NewComplaintRow` | New complaint submitted | New row appears live in Admin's complaint table |
| `ComplaintStatusUpdated` | Admin updates status | Status badge updates live on Citizen's table |
| `ComplaintEdited` | Citizen edits complaint | Title and category update live on Admin's table |
| `ComplaintDeleted` | Admin deletes complaint | Row disappears live from all tables |

---

## 📧 Email Notifications

When a citizen submits a new complaint, the system automatically sends a formatted HTML email to the admin inbox using **Gmail SMTP**.

The email includes:
- Complaint Title, Category, Description
- Submitted By & Date
- A direct **clickable link** to the complaint's details page in the portal

> **Note:** The app uses a dedicated Gmail account with an App Password for SMTP. To configure your own email, update the credentials in `EmailService.cs`.

---

## 🎨 UI Design

The UI is styled to resemble an official **Government of India** web portal:

- **Sora** font (Google Fonts) for a clean, modern look
- Deep blue (`#003580`) + red accent (`#c8102e`) government color scheme
- CSS custom properties (variables) for full design consistency
- Gradient buttons, pill-shaped badges, card-based layouts
- Responsive design for mobile and tablet
- Animated toast notifications and smooth dropdown transitions

---

## 📌 Known Limitations

- Passwords are stored as plain text (no hashing). A production app should use **BCrypt** or **ASP.NET Core Identity**.
- No pagination on the complaints list.
- Admin account must be created manually via database.
- Email credentials are hardcoded in `EmailService.cs` (should use environment variables or secrets in production).

---

## 🙏 Acknowledgements

Built as a learning project to practice:
- ASP.NET Core MVC architecture
- EF Core Code-First with SQL Server
- Cookie Authentication & Claims-based Authorization
- SignalR for real-time browser communication
- Service layer pattern with Dependency Injection
- Email sending with System.Net.Mail

---

<div align="center">

Made with ❤️ as a learning project &nbsp;|&nbsp; Government Portal UI inspired by official Indian government websites

</div>
