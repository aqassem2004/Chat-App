# Chat Application

This project is a simple chat application built using C#. It consists of a server and multiple clients, allowing users to send and receive messages in real-time. The project is implemented using TCP sockets for communication.

## Features
- **Client Registration:** Clients register using their phone numbers.
- **Real-time Messaging:** Clients can send messages to other registered users.
- **Unread Messages:** Keeps track of unread messages and displays them upon request.
- **Multithreading:** Both server and client utilize multithreading for efficient communication and UI updates.

## How It Works
### Server
1. Listens for incoming client connections on a specified port.
2. Maintains a dictionary of connected clients identified by their phone numbers.
3. Relays messages between clients.

### Client
1. Connects to the server and registers with a unique phone number.
2. Allows users to:
   - Open a chat with another user.
   - View unread messages.
   - Exit the application.
3. Handles incoming messages and stores them as unread until viewed.

## Getting Started
### Prerequisites
- .NET Framework or .NET Core installed on your system.
- A code editor like Visual Studio or Visual Studio Code.

### Running the Server
1. Open the `Server` project in your code editor.
2. Build and run the project.
3. The server will start listening for client connections on port `5000`.

### Running the Client
1. Open the `Client` project in your code editor.
2. Build and run the project.
3. Enter your phone number when prompted.
4. Use the menu options to interact with the application.

## Project Structure
- **Server:** Handles client connections and message routing.
- **Client:** Provides a user interface for sending and receiving messages.

## Usage
1. Start the server.
2. Run multiple instances of the client application.
3. Register each client with a unique phone number.
4. Send messages between clients using the provided menu options.
