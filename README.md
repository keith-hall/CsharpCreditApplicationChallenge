# CreditApplication Code Challenge

This project was written in .NET 7, you will need the appropriate .NET Core toolchain installed to continue.

To run the webapi, execute:
```bash
dotnet run --project ./CreditApplication/CreditApplication.csproj
```

then open http://localhost:5158/swagger/index.html in your browser for the Swagger interface.

To run the tests:
```bash
dotnet test
```

alternatively, use your favorite IDE.

## Future improvements

- add tests to prove config is read and parsed correctly
- add tests to prove we get a 400 Bad Request response from the API when sending negative values in the request
- parse config into correctly typed variables immediately instead of doing the logic in the service
