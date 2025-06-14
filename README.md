# 🏠 Real Estate Web Application (JWT Edition)

A full-stack real estate web application built using **ASP.NET MVC** and **ASP.NET Web API** with a modernized **JWT-based Authentication and Authorization system**.  
This project replaces the older ASP.NET Identity-based implementation with secure, stateless access control using **JSON Web Tokens**, improving both scalability and security.

---

## 🔐 Key Features

- 🔑 **JWT Authentication & Authorization** for secure user access  
- ♻️ **Refresh Token** implementation to maintain user sessions seamlessly  
- 🔐 **Password Hashing** for safe credential storage  
- 🔐 **Data Encryption/Decryption** in business logic  
- 🧩 Modular architecture using **Layered Structure**  
- 📦 Full **CRUD** operations for properties and user accounts  
- 🧭 Public and authenticated user navigation logic  

---

## 🔧 Technologies Used

**Backend**  
- **ASP.NET MVC** – Handles server-side rendering and routing  
- **ASP.NET Web API** – Exposes RESTful endpoints  
- **JWT (JSON Web Tokens)** – Stateless authentication and authorization  
- **Entity Framework Core** – Object-relational mapping (ORM) for database access  
- **SQL Server** – Relational database for persistent storage  
- **Custom DES Encryption** – Encrypts sensitive data in business logic  
- **Token Refresh Logic** – Maintains user sessions with secure refresh tokens

**Frontend**  
- **Razor Views (ASP.NET MVC)** – Server-rendered dynamic HTML with C#  
- **HTML5, CSS3, JavaScript** – Responsive UI and client-side interactivity  

**Tools**  
- **Postman** – For testing and debugging API endpoints  
- **Git & GitHub** – Source control and project collaboration  
- **Visual Studio 2022** – Main development environment  
- **SQL Server Management Studio (SSMS)** – Database management and queries
 

---

## 🔄 JWT Auth Flow

1. **User Login:** User provides credentials → JWT + Refresh Token generated.  
2. **Storage:** AccessToken stored in HTTP-only cookies (expires in 8 minutes), RefreshToken (30 days).  
3. **Refresh:** If AccessToken expires, `/Account/RefreshToken` silently updates tokens.  
4. **Secure Claims:** User info like `UserId` is extracted from token claims.  
5. **Authorization:** Claims are used in `[ServiceFilter(typeof(JwtAuthorizeAttribute))]` to protect routes.  

---

## 🧠 Security Highlights

- **JWT Tokens:** Stateless, scalable, and signed with `SymmetricSecurityKey`  
- **ClaimsPrincipal:** Extracts user info from the token  
- **RefreshToken:** Securely rotates tokens to maintain long-lived sessions  
- **Hashed Passwords:** Protects credentials during storage  
- **Encryption & Decryption:** Applied to sensitive data using custom DES algorithm  

