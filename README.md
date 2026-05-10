# OnionSocialApp - Social Network Web Application

## Description
OnionSocialApp is a full-stack web application developed using ASP.NET Core MVC. It is a social networking platform where users can interact through posts, comments, friendships, and a built-in Battleship game.

The project follows Onion Architecture principles and uses Entity Framework Core with a Code-First approach.

## Technologies
- C#
- ASP.NET Core MVC (.NET 8/9)
- Entity Framework Core (Code-First)
- ASP.NET Identity
- AutoMapper
- Bootstrap
- SQL Server

## Features

### Authentication & Security
- User registration with email confirmation
- Secure login system
- Password recovery via email token
- Route protection using `[Authorize]`

### User Profile
- Edit personal information
- Upload profile picture
- Change password

### Posts System
- Create, edit, and delete posts
- Support for:
  - Text + Image
  - Text + YouTube video (embedded playback)
- Like / Dislike system
- Comment and reply (threaded discussions)

### Friends System
- Send, accept, and reject friend requests
- View friends list
- See friends’ posts
- Mutual friends logic

### Friend Requests
- Incoming and outgoing requests
- Status tracking (Pending, Accepted, Rejected)
- Validation to avoid duplicate requests

### Battleship Game
- Turn-based multiplayer system
- Ship positioning system (with validation)
- Attack system with turn validation
- Game states:
  - Active matches
  - Match history
- Auto-win if opponent is inactive (48h rule)
- Visual boards for attacks and ship placement

### Architecture
The project follows Onion Architecture:

- **Core.Domain** → Entities
- **Core.Application** → Business logic
- **Infrastructure** → Persistence, Identity, Email
- **WebApp (MVC)** → Controllers, Views, UI

## How to Run
1. Clone the repository
2. Open the solution in Visual Studio
3. Configure `appsettings.json`
4. Apply migrations (if needed)
5. Run the project

## Notes
This project was originally developed as an academic project under a different internal name. Some solution folders, namespaces, or project references may still contain legacy naming conventions.
