# 🦀 The Crusty Crab

### Restaurant Ordering & Management System

The Crusty Crab is a full-stack restaurant ordering platform developed as a collaborative software project. The system provides customers with an online menu, cart and checkout functionality, while allowing restaurant staff and administrators to manage menu items, orders, payments and staff.

The project consists of multiple applications working together through a central REST API.

---

## 📋 Project Overview

**Deployed URL:**  
https://crustycrab-aze3fccnc6gvfchd.southafricanorth-01.azurewebsites.net

The Crusty Crab system is designed to simulate a modern restaurant ordering platform supporting both customers and restaurant management.

### Customer Features

- Browse available menu items
- View menu item details and images
- Add items to a shopping cart
- Adjust cart quantities
- Select spice levels and add-ons
- Choose collection or delivery
- Manage delivery addresses
- Manage saved payment methods
- Place orders
- View order information
- Receive order/payment confirmation
- Interact with **CrabbyBot** for restaurant FAQs

### Staff & Administration

- Access the management portal
- Manage menu items
- Add, edit and remove menu items
- Upload menu item images
- Toggle menu item availability
- Manage restaurant staff
- Manage administrators
- View and update orders
- Manage payment methods
- Update payment status

---

# 🏗️ System Architecture

The project uses a multi-application architecture:

```text
                         ┌─────────────────────┐
                         │     Customer        │
                         │      Browser        │
                         └──────────┬──────────┘
                                    │
                                    ▼
                         ┌─────────────────────┐
                         │   ASP.NET MVC Web   │
                         │      Frontend       │
                         └──────────┬──────────┘
                                    │
                                    │ REST API
                                    ▼
                         ┌─────────────────────┐
                         │    CrustyAPI        │
                         │   ASP.NET Web API   │
                         └──────────┬──────────┘
                                    │
                                    ▼
                         ┌─────────────────────┐
                         │     MySQL Database  │
                         │   crusty_crab_db    │
                         └─────────────────────┘


             ┌──────────────────────────────┐
             │       Android Application    │
             │        Kotlin / XML          │
             └──────────────┬───────────────┘
                            │
                            │ REST API
                            ▼
                     ┌───────────────┐
                     │  CrustyAPI    │
                     └───────────────┘
```

---

# 💻 Technologies

## Web Frontend

- ASP.NET Core MVC
- .NET 10
- Razor Views
- HTML
- CSS
- JavaScript
- Bootstrap/custom styling where applicable

## Backend

- ASP.NET Core Web API
- .NET 10
- Entity Framework Core
- MySQL
- BCrypt password hashing

## Mobile

- Android Studio
- Kotlin
- XML layouts

## Deployment

- Microsoft Azure
- Azure App Service
- Azure Database for MySQL
- GitHub Actions

---

# 🌐 Deployed Services

The project uses Azure-hosted services for its deployed environment.

### CrustyAPI

```text
https://crusty-crab-api-tameez-hjechmamdneee7c6.southafricanorth-01.azurewebsites.net/
```

### MVC Web Application

The MVC application is deployed separately through Azure App Service.

> The MVC application communicates with the deployed CrustyAPI rather than a local development server.

---

# 📁 Project Structure

The MVC application follows the standard ASP.NET Core MVC structure:

```text
CrustyCrab/
│
├── Controllers/
│   ├── AuthController.cs
│   ├── CartController.cs
│   ├── CheckoutController.cs
│   ├── CustomerController.cs
│   ├── ManagerController.cs
│   ├── MenuController.cs
│   └── ...
│
├── Models/
│   ├── ApiModels.cs
│   ├── CheckoutViewModel.cs
│   └── ...
│
├── Services/
│   └── CrustyApiClient.cs
│
├── Views/
│   ├── Auth/
│   ├── Cart/
│   ├── Checkout/
│   ├── Customer/
│   ├── Manager/
│   ├── Menu/
│   └── Shared/
│
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── images/
│
├── .github/
│   └── workflows/
│       └── deploy-mvc.yml
│
├── appsettings.json
├── Program.cs
└── CrustyCrab.csproj
```

---

# 🔌 API Integration

The MVC application communicates with the CrustyAPI through `CrustyApiClient`.

The API client handles operations such as:

- Customer authentication
- Customer management
- Menu retrieval and management
- Cart operations
- Order creation and management
- Delivery addresses
- Payment methods
- Saved customer payment methods
- Payment creation and status updates
- Staff management
- Administrator management
- CrabbyBot requests
- Menu image uploads

The API base URL is configured through:

```json
{
  "Api": {
    "BaseUrl": "https://crusty-crab-api-tameez-hjechmamdneee7c6.southafricanorth-01.azurewebsites.net/"
  }
}
```

This allows the MVC application to communicate with the deployed backend without hardcoding the API address throughout the application.

---

# 🛒 Ordering Flow

A typical customer order follows this process:

```text
Browse Menu
     ↓
Select Item
     ↓
Choose Spice Level / Add-ons
     ↓
Add to Cart
     ↓
Checkout
     ↓
Choose Collection / Delivery
     ↓
Choose Payment Method
     ↓
Create Order
     ↓
Create Payment
     ↓
Update Payment Status
     ↓
Generate Receipt / Confirmation
```

Customers can also proceed directly to checkout for individual menu items.

---

# 💳 Payment Handling

The application supports payment methods stored through the backend.

Saved customer payment methods display masked card information using the final four digits rather than exposing the complete card number.

For example:

```text
•••• •••• •••• 1234
```

Card security information such as CVC is not stored with saved payment methods and must be entered when required.

The current payment implementation is a **simulated payment flow** for the project rather than a connection to a live banking/payment gateway.

---

# 🤖 CrabbyBot

The application includes **CrabbyBot**, a hardcoded restaurant FAQ assistant.

CrabbyBot provides predefined responses for common restaurant questions such as:

- Menu information
- Ordering
- Collection
- Delivery
- Payment
- Restaurant information

The bot is intentionally implemented as a controlled FAQ system rather than a generative AI service.

---

# 🔐 Authentication

The application supports separate customer and administrator login flows.

Customer authentication uses:

- Email/identifier
- Password
- BCrypt password hashing

The MVC frontend maintains the authenticated user's state using server-side session data.

Administrative functionality is restricted through role checks within the MVC application.

---

# 🚀 Running the Project Locally

## Requirements

Install:

- .NET 10 SDK
- Visual Studio 2022 or later
- Git
- MySQL Server if running the complete stack locally

## Clone the Repository

```bash
git clone <repository-url>
cd CrustyCrab
```

## Configure the API

Ensure `appsettings.json` contains the correct API URL:

```json
{
  "Api": {
    "BaseUrl": "https://crusty-crab-api-tameez-hjechmamdneee7c6.southafricanorth-01.azurewebsites.net/"
  }
}
```

## Run the MVC Application

```bash
dotnet restore
dotnet build
dotnet run
```

Alternatively, open:

```text
CrustyCrab.csproj
```

in Visual Studio and run the application from there.

---

# ☁️ Continuous Deployment

The MVC application uses **GitHub Actions** to automate deployment to Azure App Service.

The workflow is located at:

```text
.github/workflows/deploy-mvc.yml
```

The workflow:

1. Checks out the repository
2. Installs .NET 10
3. Restores NuGet dependencies
4. Builds the MVC project
5. Publishes the application
6. Deploys the published application to Azure App Service

The deployment uses an Azure App Service publish profile stored securely as a GitHub repository secret.

```text
GitHub
   ↓
Push to MVC-front-end
   ↓
GitHub Actions
   ↓
Build & Publish
   ↓
Azure App Service
```

---

# 🌿 Branches

The MVC development work is maintained on:

```text
MVC-front-end
```

The backend API is maintained separately from the MVC application.

This separation allows the web frontend and backend API to be developed and deployed independently.

---

# 👥 Project

**The Crusty Crab** was developed as a collaborative group software project for a client.

The project combines:

- Web development
- REST API development
- Database design
- Mobile development
- Cloud deployment
- API integration
- Authentication
- Restaurant ordering workflows

