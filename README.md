# Citizen Complaint Management Portal

A government-themed web application where citizens submit civic complaints and admins manage them in real time.

Built with **ASP.NET Core MVC · Entity Framework Core · SignalR · SQL Server · Bootstrap 5**

---

## What It Does

Citizens register, log in, and file complaints under categories like Roads, Water, Electricity, or Sanitation. After submitting, they are redirected to the relevant official Tamil Nadu government portal.

Admins see every complaint in real time, update statuses, leave notes for citizens, and receive live bell notifications whenever something new comes in — all without refreshing the page.

---

## Features

**For Citizens**
- Register and log in securely
- Submit complaints with title, description, and category
- Edit their own complaint while it's still Pending
- Track status updates live on the complaints list

**For Admins**
- View all complaints from all citizens
- Update status — Pending → In Progress → Resolved / Rejected
- Add a note that the citizen can see on their details page
- Real-time bell notifications with badge count
- Delete complaints

**Real-Time (SignalR)**
- New complaint rows appear in the admin table instantly
- Status badges update live for everyone
- Toast pop-ups notify admin of new submissions

---

## Tech Stack

| | |
|---|---|
| Framework | ASP.NET Core MVC (.NET 10) |
| Database | SQL Server Express + EF Core |
| Auth | Cookie Authentication |
| Real-Time | SignalR |
| Frontend | Bootstrap 5 · Vanilla JS |

---



## Project Structure

```
Controllers/     → AccountController, ComplaintsController, NotificationsController
Models/          → Complaint, User, Notification
Services/        → IComplaintService, IUserService, INotificationService, IEmailService
Data/            → AppDbContext
Hubs/            → NotificationHub (SignalR)
Views/           → Complaints, Account, Shared layout
wwwroot/         → site.css, site.js, emblem image
Migrations/      → EF Core migration history
```

---

## Roles

| | Citizen | Admin |
|---|:---:|:---:|
| Submit complaint | ✅ | ❌ |
| Edit own complaint (Pending) | ✅ | ❌ |
| View all complaints | ❌ | ✅ |
| Update status + note | ❌ | ✅ |
| Delete complaint | ❌ | ✅ |
| Notifications bell | ❌ | ✅ |

---

> ⚠️ Built for learning. Passwords are stored as plain text — not suitable for production without proper hashing.
