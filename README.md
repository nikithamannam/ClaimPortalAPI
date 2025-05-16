# ClaimantPortal API

This is the backend API for **Claimant Portal**, a secure and scalable system designed for efficient digital insurance claims processing. Built with **ASP.NET Core** and **SQL Server**, the API handles user authentication, contact management, claim tracking, and administrative controls.

---

## 🚀 Features

- 🔐 User Registration & Login with JWT authentication
- 👤 Role-based access management (Admin, Claimant)
- 📬 Contact form submission & emergency handling
- 📂 Document upload endpoints
- 📈 Claim status tracking
- 🛠️ SQL Server integration using stored procedures

---

## 🏗️ Tech Stack

- **Backend**: ASP.NET Core Web API (.NET 6+)
- **Database**: SQL Server
- **Auth**: JSON Web Tokens (JWT)
- **ORM**: ADO.NET (with stored procedures)

---

## 📁 Project Structure

ClaimPortal.API/ │
    ├── Controllers/ # API Controllers (e.g., UserController) 
    ├── Models/ # Request/response models 
    ├── Services/ # Business logic and helpers 
    ├── appsettings.json # Configuration settings 
    ├── Program.cs # Entry point 
└── Startup.cs # Middleware and service setup 

yaml
Copy
Edit

---

## 🔧 Setup Instructions

### 1. Clone the Repository

```bash
git clone https://github.com/nikithamannam/ClaimantPortal.git
cd ClaimantPortal
2. Configure appsettings.json
Update the ConnectionStrings section to match your SQL Server instance:

json
Copy
Edit
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=ClaimPortal;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
}
3. Build and Run
From the terminal or Visual Studio:

bash
Copy
Edit
dotnet restore
dotnet build
dotnet run
📡 API Endpoints Overview

Endpoint	Method	Description
/api/auth/register	POST	Register a new user
/api/auth/login	POST	Login and get JWT token
/api/contact/submit	POST	Submit contact request
/api/contact/emergency	POST	Handle emergency request
/api/user/{email}	GET	Get user profile by email
✅ Swagger UI available at: https://localhost:{port}/swagger

🛡️ Security
Passwords are hashed using SHA-256

JWT-based stateless authentication


📜 License
MIT License – feel free to use, modify, and contribute.

🤝 Contributors
Nikitha Mannam

