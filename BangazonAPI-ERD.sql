CREATE TABLE `Employee`
(
  `Id` int PRIMARY KEY,
  `FirstName` varchar(255),
  `LastName` varchar(255),
  `Supervisor` boolean,
  `DepartmentId` int
);

CREATE TABLE `Department`
(
  `Id` int PRIMARY KEY,
  `DeptName` varchar(255),
  `ExpenseBudget` double
);

CREATE TABLE `Computer`
(
  `Id` int PRIMARY KEY,
  `PurchaseDate` date,
  `DecomissionDate` date,
  `Make` varchar(255),
  `Manufacturer` varchar(255)
);

CREATE TABLE `ComputerEmployee`
(
  `Id` int PRIMARY KEY,
  `EmployeeId` int,
  `ComputerId` int,
  `AssignDate` date,
  `UnassignDate` date
);

CREATE TABLE `EmployeeTraining`
(
  `Id` int PRIMARY KEY,
  `TrainingProgramId` int,
  `EmployeeId` int
);

CREATE TABLE `TrainingProgram`
(
  `Id` int PRIMARY KEY,
  `TrainingProgramName` varchar(255),
  `StartDate` varchar(255),
  `EndDate` varchar(255),
  `MaxAttendees` int
);

CREATE TABLE `ProductType`
(
  `Id` int PRIMARY KEY,
  `CategoryName` varchar(255)
);

CREATE TABLE `Product`
(
  `Id` int PRIMARY KEY,
  `Price` double,
  `Title` varchar(255),
  `Description` varchar(255),
  `Quantity` int,
  `ProductTypeId` int,
  `CustomerId` int
);

CREATE TABLE `Order`
(
  `Id` int PRIMARY KEY,
  `CustomerId` int,
  `PaymentTypeId` int
);

CREATE TABLE `OrderProduct`
(
  `Id` int PRIMARY KEY,
  `OrderId` int,
  `ProductId` int
);

CREATE TABLE `PaymentType`
(
  `Id` int PRIMARY KEY,
  `AcctTypeName` varchar(255),
  `AcctNumber` int,
  `CustomerId` int
);

CREATE TABLE `Customer`
(
  `Id` int PRIMARY KEY,
  `FirstName` varchar(255),
  `LastName` varchar(255)
);

ALTER TABLE `Employee` ADD FOREIGN KEY (`DepartmentId`) REFERENCES `Department` (`Id`);

ALTER TABLE `ComputerEmployee` ADD FOREIGN KEY (`EmployeeId`) REFERENCES `Employee` (`Id`);

ALTER TABLE `ComputerEmployee` ADD FOREIGN KEY (`ComputerId`) REFERENCES `Computer` (`Id`);

ALTER TABLE `EmployeeTraining` ADD FOREIGN KEY (`TrainingProgramId`) REFERENCES `TrainingProgram` (`Id`);

ALTER TABLE `EmployeeTraining` ADD FOREIGN KEY (`EmployeeId`) REFERENCES `Employee` (`Id`);

ALTER TABLE `Product` ADD FOREIGN KEY (`ProductTypeId`) REFERENCES `ProductType` (`Id`);

ALTER TABLE `Product` ADD FOREIGN KEY (`CustomerId`) REFERENCES `Customer` (`Id`);

ALTER TABLE `Order` ADD FOREIGN KEY (`CustomerId`) REFERENCES `Customer` (`Id`);

ALTER TABLE `Order` ADD FOREIGN KEY (`PaymentTypeId`) REFERENCES `PaymentType` (`Id`);

ALTER TABLE `OrderProduct` ADD FOREIGN KEY (`OrderId`) REFERENCES `Order` (`Id`);

ALTER TABLE `OrderProduct` ADD FOREIGN KEY (`ProductId`) REFERENCES `Product` (`Id`);

ALTER TABLE `PaymentType` ADD FOREIGN KEY (`CustomerId`) REFERENCES `Customer` (`Id`);
