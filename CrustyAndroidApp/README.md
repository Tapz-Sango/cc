

# 🍔 The Crusty Crab — Android App

The **Crusty Crab Android App** is the mobile application component of the Crusty Crab restaurant ordering system. It allows customers to browse the menu, manage their cart, place orders, make payments, and track their orders from an Android device.

The application communicates with the Crusty Crab REST API to retrieve and update restaurant data.

## 📱 Features

- 👤 Customer registration and login
- 🍔 Browse the restaurant menu
- 🛒 Add and remove items from the cart
- 📦 Place and manage orders
- ❌ Cancel orders where applicable
- 💳 Link and manage payment methods
- 📍 Track order status
- 🔐 Account-based customer data
- 🌐 Communication with the deployed Crusty Crab API

## 🏗️ Project Structure

```text
CrustyCrab/
├── app/
│   ├── src/
│   │   ├── main/
│   │   └── test/
│   └── build.gradle.kts
├── gradle/
├── build.gradle.kts
├── settings.gradle.kts
└── gradlew
```

## 🔌 API

The Android application communicates with the deployed Crusty Crab API.

**API Base URL:**

```text
https://crusty-crab-api-tameez-hjechmamdneee7c6.southafricanorth-01.azurewebsites.net/
```

The API handles functionality such as:

- Customer accounts
- Menu items
- Cart management
- Orders
- Payments
- Delivery addresses
- Order tracking

## 🛠️ Technologies

- **Kotlin**
- **Android Studio**
- **Android XML layouts**
- **Gradle**
- **REST API**
- **HTTPS**
- **JUnit / Android testing**

## ▶️ Running the App

### Requirements

- Android Studio
- Android SDK
- JDK compatible with the project
- Android emulator or physical Android device

### Steps

1. Clone the repository:

```bash
git clone <repository-url>
```

2. Open the Android project in **Android Studio**.

3. Allow Gradle to synchronise and download the required dependencies.

4. Select an Android emulator or connected device.

5. Click **Run ▶**.

The application will connect to the deployed Crusty Crab API automatically.

## 🧪 Testing

The project includes Android unit tests covering application functionality.

Tests can be executed through Android Studio or using Gradle:

```bash
./gradlew test
```

On Windows:

```powershell
.\gradlew.bat test
```

## 🔄 CI/CD

The Android project includes a GitHub Actions workflow that automatically builds and tests the application when changes are pushed to the Android branch.

This helps ensure that changes do not introduce build or test failures before they are integrated into the main project.

## 🔐 Security

The application communicates with the API over **HTTPS**.

Sensitive configuration and local development files should not be committed to the repository. These include files such as:

```text
local.properties
.gradle/
.idea/
build/
```

## 👥 Project

**The Crusty Crab** is a group restaurant management and ordering system consisting of:

- 🌐 Web/MVC application
- 📱 Android application
- ⚙️ .NET REST API
- 🗄️ MySQL database

The Android application provides the mobile customer experience while the REST API acts as the central backend connecting the different components of the system.

---

**The Crusty Crab**  
Restaurant Ordering & Management System 🍔