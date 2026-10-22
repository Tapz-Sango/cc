# 🦀 The Crusty Crab

### Restaurant Ordering & Management System

The **Crusty Crab** is a full-stack restaurant ordering and management system developed as a collaborative software project.

The system provides customers with web and mobile ordering experiences while giving restaurant staff and administrators tools to manage menu items, orders, payments and staff.

The project consists of three main components working together through a central REST API:

- 🌐 ASP.NET Core MVC Web Application
- 📱 Kotlin Android Application
- ⚙️ ASP.NET Core REST API

---

## 📋 Project Overview

The Crusty Crab simulates a modern restaurant ordering platform supporting both customers and restaurant management.

### Customer Features

- Browse available menu items
- View menu item details and images
- Add items to a shopping cart
- Adjust cart quantities
- Select spice levels and add-ons
- Choose collection or delivery
- Manage delivery addresses
- Automatically suggest addresses using **Geoapify address autocomplete**
- Manage saved payment methods
- Place orders
- View order information
- Track order status
- Receive order and payment confirmation
- Interact with **CrabbyBot** for restaurant FAQs
- Submit and manage customer reviews

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
- Manage customer reviews

---

## 🏗️ System Architecture

The project uses a multi-application architecture, with **CrustyAPI** acting as the central backend.

```text
                       ┌─────────────────────────┐
                       │        Customers        │
                       └────────────┬────────────┘
                                    │
                       ┌────────────┴────────────┐
                       │                         │
                       ▼                         ▼
             ┌──────────────────┐      ┌──────────────────┐
             │ ASP.NET Core MVC │      │  Android App     │
             │   Web Application│      │   Kotlin / XML   │
             └────────┬─────────┘      └────────┬─────────┘
                      │                         │
                      └────────────┬────────────┘
                                   │
                                   ▼
                       ┌─────────────────────┐
                       │     CrustyAPI       │
                       │ ASP.NET Core Web API│
                       └──────────┬──────────┘
                                  │
                                  ▼
                       ┌─────────────────────┐
                       │    MySQL Database   │
                       │   crusty_crab_db    │
                       └─────────────────────┘

                External Integration:
                Geoapify Address Autocomplete
```

Geoapify is used to provide address suggestions as users enter delivery addresses. The applications communicate with the central API for the system's core restaurant functionality.

---

## 💻 Technologies

### Web Application

- ASP.NET Core MVC
- .NET 10
- Razor Views
- HTML
- CSS
- JavaScript
- Bootstrap and custom styling

### REST API

- ASP.NET Core Web API
- .NET 10
- Entity Framework Core
- MySQL
- BCrypt password hashing
- RESTful endpoints

### Android Application

- Android Studio
- Kotlin
- XML layouts
- Gradle
- Android testing

### External Services & Infrastructure

- **Geoapify** — address autocomplete and address suggestions
- Microsoft Azure
- Azure App Service
- MySQL
- GitHub
- GitHub Actions
- HTTPS

---

## 📁 Project Structure

The `main` branch contains the integrated Crusty Crab project.

```text
TheCrustyCrab/
│
├── CrustyAPI/
│   ├── Controllers/
│   ├── DTOs/
│   ├── Data/
│   ├── Models/
│   ├── Services/
│   └── CrustyAPI.csproj
│
├── CrustyAPI.Tests/
│   └── API automated tests
│
├── Controllers/
│   └── MVC controllers
│
├── Models/
│   └── MVC models and API models
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
│   ├── Review/
│   └── Shared/
│
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── images/
│
├── app/
│   └── Android application source
│
├── gradle/
│
├── Documentation/
│   └── Project presentation
│
├── .github/
│   └── Workflows, where configured
│
├── CrustyCrab.csproj
├── CrustyCrab.slnx
├── TheCrustyCrab.slnx
├── Program.cs
└── README.md
```

---

## 🔌 API Integration

The **CrustyAPI** provides the central backend used by both the MVC web application and the Android application.

The API handles functionality including:

- Customer authentication and management
- Menu retrieval and management
- Menu image uploads
- Cart operations
- Order creation and management
- Delivery addresses
- Address suggestions and autocomplete integration
- Payment methods and payment status updates
- Staff management
- Administrator management
- Customer reviews
- CrabbyBot
- Order tracking

The MVC application communicates with the API through `CrustyApiClient`.

The Android application communicates with the same API, allowing both applications to work with the shared backend.

---

## 📍 Address Autocomplete with Geoapify

The project integrates **Geoapify** to improve the delivery address experience.

Address autocomplete allows users to receive address suggestions while entering a delivery address, reducing manual input and helping users select an appropriate address.

The integration supports the delivery address workflow alongside the system's existing address management functionality.

---

## 🛒 Ordering Flow

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
Enter or Select Delivery Address
    ↓
Use Address Suggestions, if Needed
    ↓
Choose Payment Method
    ↓
Create Order
    ↓
Create Payment
    ↓
Update Payment Status
    ↓
Order Confirmation
```

---

## 💳 Payment Handling

The system supports saved customer payment methods.

Saved cards display masked information using only the final four digits.

Example:

```text
•••• •••• •••• 1234
```

Sensitive card security information, such as CVC, is not stored with saved payment methods.

The current payment implementation uses a **simulated payment flow** for the project and does not connect to a live banking or payment gateway.

---

## 🤖 CrabbyBot

The project includes **CrabbyBot**, a controlled restaurant FAQ assistant.

CrabbyBot provides predefined responses to common questions regarding:

- Menu information
- Ordering
- Collection
- Delivery
- Payment
- Restaurant information

CrabbyBot is implemented as a predefined FAQ system rather than a generative AI service.

---

## 🔐 Authentication & Security

The system supports separate customer and administrator authentication.

Customer authentication uses:

- Email or identifier
- Password
- BCrypt password hashing

The MVC application maintains authenticated session state using server-side sessions.

Administrative functionality is protected through role-based access checks.

Communication with the deployed API uses HTTPS.

Configuration values, credentials and API keys should be stored securely and should not be committed to the repository.

---

## 🚀 Running the Project Locally

### Requirements

Install the following:

- .NET 10 SDK
- Visual Studio 2022 or later, with support for the project's target framework
- Git
- MySQL Server for a complete local backend setup
- Android Studio for the Android application
- An Android emulator or compatible Android device

### Clone the Repository

```bash
git clone <repository-url>
cd <repository-folder>
```

### Run the MVC Application

From the repository root, where `CrustyCrab.csproj` is located:

```bash
dotnet restore
dotnet build
dotnet run
```

Alternatively, open `CrustyCrab.csproj` or the appropriate solution file in Visual Studio.

### Run the REST API

Navigate to the API project:

```bash
cd CrustyAPI
dotnet restore
dotnet build
dotnet run
```

Configure the database connection and other required settings for your local environment before running the API.

### Run the Android Application

1. Open the repository's Android project in Android Studio.
2. Allow Gradle to synchronise.
3. Select an emulator or connected Android device.
4. Confirm that the API base URL is configured for the intended environment.
5. Run the application.

### Configure Geoapify

Address autocomplete requires the appropriate Geoapify configuration and API key.

Configure the required key through the application's intended configuration mechanism. Do not commit private keys or secrets to GitHub.

---

## 🧪 Testing

The repository includes an API test project and Android testing support.

### API Tests

The API test project is located at:

```text
CrustyAPI.Tests/
```

Run the tests from the repository root:

```bash
dotnet test
```

### Android Tests

From the Android project directory, run the relevant Gradle tests:

```powershell
.\gradlew.bat test
```

Additional instrumentation tests may require an emulator or connected Android device.

---

## ☁️ Continuous Integration & Deployment

GitHub Actions can be used to automate building, testing and deployment through the repository's configured workflows.

The general deployment process is:

```text
Git Push
   ↓
GitHub Actions
   ↓
Build
   ↓
Automated Tests
   ↓
Publish
   ↓
Azure App Service
```

The web application and API are designed to be hosted through Microsoft Azure.

---

## 🌿 Git Branches

The repository uses separate development branches for the major project components.

```text
main
│
├── MVC-front-end
├── CrustyAPI
└── CrustyAndroidApp
```

### `main`

The integrated branch containing the combined Crusty Crab system, including the web application, API and Android application.

### `MVC-front-end`

The ASP.NET Core MVC web application development branch.

### `CrustyAPI`

The ASP.NET Core REST API development branch.

### `CrustyAndroidApp`

The Kotlin Android application development branch.

Changes from the individual development branches are integrated into `main` when appropriate.

---

## 🎥 Demonstration Videos

### 🌐 Web Application Demonstration

[Watch the Web Application Demo](https://youtu.be/wsye5qpRHlw)

### 📱 Android Application Demonstration

[Watch the Android Application Demo](https://youtu.be/sIX8pzTgMb4)

---

## 📑 Project Presentation

[View the Crusty Crab Project Presentation](Documentation/Crusty%20Crab%20Implementation%20and%20Functionality.pdf)

---

## 👥 Project

**The Crusty Crab** was developed as a collaborative group software project.

The system combines:

- Web development
- Mobile development
- REST API development
- Database design
- Authentication and authorization
- Ordering and payment workflows
- Address autocomplete integration
- API integration
- Cloud deployment
- Automated testing
- Continuous integration and deployment

---

## 🦀 The Crusty Crab

**One restaurant. Three applications. One API.**