# FapAPI – FPT Academic Management API System

FapAPI is a backend system developed using ASP.NET Core Web API to simulate(FPT Academic Portal).  
The system provides APIs for attendance management, exam scheduling, timetable tracking, authentication, and notifications.  

This API is designed to be consumed by a mobile application (MobiApp) for real-time academic data interaction.

---

## 🚀 Project Overview

FAP (FPT Academic Portal) is the internal academic management system used at FPT University.  
It manages:

- Student attendance
- Exam schedules
- Timetables
- Academic notifications
- User roles (Student, Lecturer, Admin)

FapAPI replicates and extends these core features in a modular RESTful architecture.

---

## 🏗 System Architecture

Client (Mobile App - MobiApp)
        ↓
RESTful API (FapAPI - ASP.NET Core Web API)
        ↓
SQL Database (MySQL)

- Authentication handled via JWT
- Role-based authorization
- EF Core for ORM
- Service Layer pattern applied

---

## 🔑 Core Features

### 1️⃣ Authentication & Authorization
- Login with JWT token
- Role-based access control (Student, Lecturer, Admin)
- Secure password hashing

### 2️⃣ Attendance Management
- Lecturer can create attendance sessions
- Students' attendance stored and tracked
- Generate attendance reports
- Query attendance history

### 3️⃣ Exam Schedule Management
- Admin creates exam schedule
- System auto-assigns eligible students
- Prevents overlapping exam slots
- Assigns supervisors (lecturers) without schedule conflicts
- Automatically generates 2nd FE exam (if required)

### 4️⃣ Timetable Management
- Manage class schedule
- Slot-based timetable system
- User-specific timetable query

### 5️⃣ Notification System
- Send and retrieve notifications
- Linked to user accounts

---

## 🧠 Business Logic Highlights

✔ Prevent duplicate exam scheduling  
✔ Avoid overlapping time slots  
✔ Auto student selection by subject & semester  
✔ Role validation before data access  
✔ Transaction-safe data updates  

---

## 🛠 Tech Stack

- ASP.NET Core Web API
- Entity Framework Core
- SQL Service
- JWT Authentication
- Service Layer Architecture
- Git

---

## 📂 Project Structure

Controllers/        → API Endpoints  
Services/           → Business Logic Layer  
Models/             → Database Entities  
ViewModels/         → Request/Response Models  
Program.cs          → Application Configuration  

---

## 📱 Mobile App Integration

FapAPI is consumed by a mobile application (MobiApp).  
The mobile app:

- Calls login API to retrieve JWT
- Sends token in Authorization header
- Fetches attendance, timetable, exam schedules
- Displays real-time academic data

Example:

Authorization: Bearer {JWT_TOKEN}

---

## 🔒 Security Implementation

- Password hashing
- JWT expiration handling
- Role-based authorization
- Input validation
- Secure transaction processing

---

## 🌍 Deployment

- Designed for production-ready deployment
- Environment-based configuration
- Database connection string secured via appsettings

---

## 📌 Future Improvements

- Redis caching for performance optimization
- Docker containerization
- CI/CD pipeline integration
- API rate limiting
- Logging & monitoring

---
