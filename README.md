# Villa Management System

 **Villa Management System**, a scalable web application built with ASP.NET Core MVC. This project demonstrates modern software architecture principles, including Clean Architecture, Unit of Work, and Repository Pattern, to manage hotel operations efficiently.

## Overview
The Villa Management System allows users (Customers) and admins to manage villa bookings, user profiles, and settings. Key features include:
- **User Authentication:** Register, login, and manage user profiles using ASP.NET Core Identity.
- **Villa Management:** Admins can create, update, and delete villas with image uploads.
- **Booking System:** Users can book villas, view booking history, and cancel bookings.
- **Admin Dashboard:** Manage villas, villa numbers, and amenities (admin role only).
- **Payment Integration:** Stripe integration for secure payments.
- **Responsive Design:** Built with Bootstrap 5 for a responsive UI across devices.
- **Notifications:** Toastr for success/error notifications and SweetAlert2 for confirmation dialogs.

  
## Demo Link
- https://villa-management.runasp.net/

## Demo Video
Check out this video to see the system in action:

[![Villa Management System Demo](https://img.youtube.com/vi/H6Wnc7t4Hdo/0.jpg)](https://www.youtube.com/watch?v=H6Wnc7t4Hdo)



## Technologies Used
- **Backend:** ASP.NET Core MVC, Entity Framework Core
- **Architecture:** Clean Architecture, Unit of Work, Repository Pattern
- **Database:** SQL Server
- **Frontend:** Bootstrap, HTML/CSS, JavaScript, jQuery, SweetAlert2 Lib
- **Payment:** Stripe
- **Tools:** Visual Studio, Git

## Project Structure

Hotel/
├── Hotel.Web/                   # ASP.NET Core Web Application
│   ├── Controllers/            # MVC Controllers (e.g., AccountController, BookingController)
│   ├── Views/                  # Razor Views (e.g., Setting.cshtml, BookingHistory.cshtml)
│   ├── wwwroot/                # Static files (CSS, JS, images, favicon)
│   │   ├── favicon.ico         # Favicon for browser tab
│   │   ├── images/             # Images for villas, sliders, and logo
│   │   ├── css/                # Custom CSS files
│   │   └── js/                 # Custom JavaScript files
│   ├── Program.cs              # Application startup and middleware configuration
│   └── appsettings.json        # Configuration settings (database, Stripe, etc.)
├── Hotel.Application/           # Application logic and interfaces
├── Hotel.Domain/                # Domain entities (e.g., Villa, Booking, ApplicationUser)
├── Hotel.Infrastructure/        # Data access layer (EF Core, repositories)
└── README.md                    # Project documentation

### Details of Future Plans

- **Email Verification:** Enhances security by ensuring users verify their email addresses during registration.
- **Two-Factor Authentication (2FA):** Adds an extra layer of security using email or SMS-based verification.
- **Booking Verification Emails:** Improves user experience by sending automated emails with booking details.
- **Payment Integration:** Adds real-world applicability with a payment gateway for villa bookings.
- **Notification System:** Keeps users and admins informed with real-time updates.
- **Multi-Language Support:** Makes the app accessible to a global audience.
- **Unit Testing:** Ensures code reliability and maintainability, a best practice for professional development.

  
