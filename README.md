# SchoolSpyware

This is a mock app that illustrates a way to capture the screen of a user and remotely monitor it, employing techniques such as asymmetric-key encryption to ensure privacy and safety

Once the server is run by providing the internal network IP address, the clients can start connecting and sending images after an initial handshake:
```cmd
monitor-server.exe <ip>:<port>
```

In order to connect a client to the server, the user must provide the server IP address and port and the username they wish to use for the current session.
```cmd
monitored-app <ip>:<port> <username>
```
