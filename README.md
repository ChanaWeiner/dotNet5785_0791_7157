# Private Teachers Management System

A **WPF desktop application** built with **.NET Core**, designed to manage private teachers and students efficiently.  
The system supports tutor registration, lesson requests by administrators, manual teacher matching, and real-time tracking of lesson status.

---

## 🧩 Project Overview

This project simulates a private teaching management platform with a clean **three-layer architecture (DAL, BL, PL)**:

- **DAL (Data Access Layer):** Handles reading and writing data using **XML files** instead of a traditional database.  
- **BL (Business Logic Layer):** Implements the system’s business rules, such as allowing administrators or teachers to assign lessons based on availability, subject, and other criteria.  
- **PL (Presentation Layer):** A modern **WPF interface** that enables users (students, teachers, and administrators) to interact with the system intuitively.

### Key Features
- Student and teacher registration  
- Lesson request creation and management  
- Manual teacher assignment by administrators or teachers  
- Tracking and updating lesson request statuses (pending, assigned, expired)  
- Persistent data storage using **XML files**  
- Clear architectural separation for maintainability and scalability  

---

## 🛠️ Technologies & Tools
- **.NET Core / WPF**
- **C#**
- **MVVM design pattern**
- **LINQ to XML**
- **Visual Studio 2022**

---

## ⚙️ How to Run
1. Clone the repository:
   ```bash
   git clone https://github.com/ChanaWeiner/dotNet5785_0791_7157.git