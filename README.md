# Mobile-App-logger
A robust logging solution for mobile applications that captures and monitors user activities in real-time.
## 🚀 Features

- **User Activity Tracking**: Monitor logins, screen views, and user journeys
- **Real-time Logs**: Instant log visibility in console and file storage
- **WordPress Integration**: WebView logging for WordPress sites
- **Device Info Capture**: Platform, OS version, device model tracking
- **Health Monitoring**: Built-in health check endpoint
- **Rotating Log Files**: Automatic log rotation with Serilog

## 📋 What Gets Logged

- Login attempts (success/failure)
- Screen navigation
- Session start/end
- User actions and clicks
- Errors and exceptions
- Device information

## 🛠 Tech Stack

- .NET Core Web API
- Serilog (file logging)
- React Native client
- CORS enabled
- RESTful architecture

## 🔧 Setup

1. Clone the repository
2. Run `dotnet restore`
3. Configure CORS for your domain
4. Run `dotnet run --urls "http://0.0.0.0:5000"`

## 📁 Project Structure

- `/Controllers` - API endpoints
- `/Models` - Data models
- `/logs` - Rotating log files
- `Program.cs` - Application configuration

## ✅ Status

Working: Login tracking, session monitoring, health checks
In Progress: WebView integration, navigation tracking
