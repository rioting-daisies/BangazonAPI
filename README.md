# Bangazon

This is a .NET Web API that makes each resource in Bangazon available to application developers throughout the entire company.

The resources currently within the databases are:

1. Products
1. Product types
1. Customers
1. Orders
1. Payment types
1. Employees
1. Computers
1. Training programs
1. Departments

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

1. `git clone git@github.com:rioting-daisies/BangazonAPI.git`
2. You will be using the [Official Bangazon SQL](https://github.com/rioting-daisies/BangazonAPI/blob/master/bangazon.sql) file to create your database. Create the database using Azure Data Studio, create a new SQL script for that database, copy the contents of the SQL file into your script, and then execute it.
3. Once the database is created, find the SQLDATA.sql file located in /BangazonAPI directory within the BangazonAPI.sln. Import this file into Azure Data Studio (or similar application) and run the insert queries in order from the top (i.e. start with INSERT INTO Customer on line 14 within the file).
4. Run the SELECT statements at the very top after this in order to ensure that the tables have been populated with the correct data, which is referenced in the Unit Tests for each resource in the /TestBangazonAPI directory.

### Prerequisites

What things you need to install the software and how to install them:
1. Visual Studio 2019 [click here to view installation instructions](https://visualstudio.microsoft.com/downloads/)
2. Azure Data Studio [click here to view installation instructions](https://docs.microsoft.com/en-us/sql/azure-data-studio/download?view=sql-server-2017)

## Running the automated unit tests for all resources

1. Open the BangazonAPI.sln file within Visual Studio
1. Check to make sure that the `IIS Express` is changed to `Bangazon API` within the dropdown menu of the Run (play arrow) button on your toolbar within Visual Studio.
1. Begin automated tests by selecting `Test` on the toolbar, and then click `Run all tests` within Visual Studio.

## Running manual tests
1. Open the BangazonAPI.sln file within Visual Studio.
1. Check to make sure that the `IIS Express` is changed to `Bangazon API` within the dropdown menu of the Run (play arrow) button on your toolbar within Visual Studio. Now run the server by clicking the Run button.
1. Open up your browser, and run the following links in order to make `GetAll` and `GetOne` requests for the resources:
* `GetAll` [Payment Type](http://localhost:5000/api/paymenttype)
* `GetOne` [Payment Type](http://localhost:5000/api/paymenttype/1)
* `GetAll` [Customer](http://localhost:5000/api/customer)
* `GetOne` [Customer](http://localhost:5000/api/customer/1)
* `GetAll` [Customer with Products](http://localhost:5000/api/customer?_include=products)
* `GetOne` [Customer with Products](http://localhost:5000/api/customer/1?_include=products)
* `GetAll` [Customer with Payments](http://localhost:5000/api/customer?_include=payments)
* `GetOne` [Customer with Payments](http://localhost:5000/api/customer/1?_include=payments)
* `GetAll` [Customer with Query](http://localhost:5000/api/customer?q=ser)
* `GetAll` [Customer with Products and Query](http://localhost:5000/api/customer?_include=products&q=ser)
* `GetAll` [Customer with Payments and Query](http://localhost:5000/api/customer?_include=payments&q=ser)
* `GetAll` [Product Type](http://localhost:5000/api/producttype)
* `GetOne` [Product Type](http://localhost:5000/api/producttype/1)
* `GetAll` [Product](http://localhost:5000/api/product)
* `GetOne` [Product](http://localhost:5000/api/product/1)
* `GetAll` [Order](http://localhost:5000/api/order)
* `GetOne` [Order](http://localhost:5000/api/order/1)
* `GetAll` [Order with Products](http://localhost:5000/api/order?_include=products)
* `GetOne` [Order with Products](http://localhost:5000/api/order/1?_include=products)
* `GetAll` [Order with Customers](http://localhost:5000/api/order?_include=customers)
* `GetOne` [Order with Customers](http://localhost:5000/api/order?/1_include=customers)
* `GetAll` [Non-Completed Orders](http://localhost:5000/api/order?completed=false)
* `GetAll` [Completed Orders](http://localhost:5000/api/order?completed=true)
* `GetAll` [Department](http://localhost:5000/api/department)
* `GetOne` [Department](http://localhost:5000/api/department/1)
* `GetAll` [Employees in Department](http://localhost:5000/api/department?_include=employees)
* `GetOne` [Employees in Department](http://localhost:5000/api/department/1?_include=employees)
* `GetAll` [Department with Budget Greater than $20,000](http://localhost:5000/api/department?_filter=budget&_gt=20000)
* `GetOne` [Department with Budget Greater than $20,000](http://localhost:5000/api/department/1?_filter=budget&_gt=20000)
* `GetAll` [Employee](http://localhost:5000/api/employee)
* `GetOne` [Employee](http://localhost:5000/api/employee/1)
* `GetAll` [Training Program](http://localhost:5000/api/trainingprogram)
* `GetOne` [Training Program](http://localhost:5000/api/trainingprogram/1)
* `GetAll` [Training Programs that are completed](http://localhost:5000/api/trainingprogram?completed=true)
* `GetAll` [Training Programs that are not completed](http://localhost:5000/api/trainingprogram?completed=false)
* `GetAll` [Computer](http://localhost:5000/api/computer)
* `GetOne` [Computer](http://localhost:5000/api/computer/1)
 

## Authors

* **Chris Morgan**
* **Billy Mitchell**
* **Clif Matuszewski**
* **Alex Thacker**

## Acknowledgments

* Hat tip to **Steve Brownley**, **Leah Hoefling**, and **Jisie David** for helping out throughout the project!
* Kudos to **Andy Collins** for laying the c# foundation for us!
