# service-bus-explorer
This repo provides a basic utility for browsing and working with messages on azure service bus.


# Dependencies
- Docker
- Docker Compose V2
- Azure Service Bus Emulator


# Getting started
1. Start the containers for the service bus eumlator
```
docker compose --env-file dev.env up -d
```

2. Build the solution
```
dotnet build
```

3. Run one of the App Projects
    - WebApi
    ```
    dotnet run --launch-profile "https" --no-build
    ```

4. Clean up containers and images when done
```
docker compose --env-file dev.env down --rmi 'all'
```

# References
- [Azure Service Bus Emulator](https://docs.azure.cn/en-us/service-bus-messaging/test-locally-with-service-bus-emulator?tabs=automated-script)