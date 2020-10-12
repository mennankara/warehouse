# Known issues

- For the ease of portability purposes, the database choice is SQLite and the connection string is hardcoded in the application.
- In the provided [products.json](https://github.com/mennankara/warehouse/blob/master/assignment/products.json) file there is no `productId`, therefore the `Name` field is used as key and the orders are created by the product name.
- Unit Test coverage is minimal due to time restriction for the delivery.

# Repository

[Github](https://github.com/mennankara/warehouse)

# Hosted Version

The latest version of the app is deployed to the Azure: 

[Swagger](https://warehouseapimmk.azurewebsites.net/swagger/index.html)

# Running the application

First, you need to clone the repository to the machine you are planning to run the application.

## Without Visual Studio 2019

1. Install [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1)
2. Open your favorite cli (cmd, powershell, bash) and navigate to the `src/` folder.
3. Run the following commands:
    ```
    dotnet restore
    dotnet build
    dotnet test
4. After these commands, to run the `Warehouse.Api`, run the following commands and then navigate to https://localhost:5001/swagger to see the API playground.
    ```
    cd Warehouse.Api:
    dotnet run
## With Visual Studio 2019
1. Make sure your Visual Studio is up-to-date.
3. Launch Visual Studio and open `src/Warehouse.sln`.
4. Build the solution.
4. Use the Test / Run / All Tests menu to run the unit tests.
5. Make sure the startup project is Warehouse.Api, then launch the application. To see the API playground, navigate to https://localhost:5001/swagger in your browser.
