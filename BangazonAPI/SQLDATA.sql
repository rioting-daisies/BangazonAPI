SELECT * FROM Computer;
SELECT * FROM ComputerEmployee;
SELECT * FROM Customer;
SELECT * FROM Department;
SELECT * FROM Employee;
SELECT * FROM EmployeeTraining;
SELECT * FROM [Order];
SELECT * FROM OrderProduct;
SELECT * FROM PaymentType;
SELECT * FROM Product;
SELECT * FROM ProductType;
SELECT * FROM TrainingProgram;



-- INSERT INTO ProductType ([Name]) VALUES ('Computers');
-- INSERT INTO ProductType ([Name]) VALUES ('Printers');
-- INSERT INTO ProductType ([Name]) VALUES ('Appliances');
-- INSERT INTO ProductType ([Name]) VALUES ('TVs');


-- INSERT into Customer (FirstName, LastName) VALUES ('Sermour', 'Butts');

-- INSERT into PaymentType (AcctNumber, [Name], CustomerId) VALUES (123456, 'Visa', 1);
-- INSERT into PaymentType (AcctNumber, [Name], CustomerId) VALUES (678902, 'MasterCard', 1);

-- INSERT into Product (ProductTypeId, CustomerId, Price, Title, [Description], Quantity) VALUES (1, 1, 222, 'Vaccuum of Destiny', 'Sucks you into your destiny. Youll prolly die oops.', 2);
-- INSERT into Product (ProductTypeId, CustomerId, Price, Title, [Description], Quantity) VALUES (1, 1, 420666.0000, 'Infinity Gauntlet', 'Just snap your fingers... itll be fine.', 1);

-- INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) VALUES ('20190618 10:34:09 AM', '20190618 10:34:09 AM', 'Latitude', 'Dell');

-- INSERT INTO Department ([Name], Budget) VALUES ('HR', 25000);

-- INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor) VALUES ('Firey', 'Dragon', 1, 1);

-- INSERT INTO [Order] (CustomerId, PaymentTypeId) VALUES (1, 1);

-- INSERT INTO TrainingProgram ([Name], StartDate, EndDate, MaxAttendees) VALUES ('Dragon Training','20190618 10:34:09 AM', '20190618 10:34:09 AM', 2);

-- INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) VALUES (1, 1,'20190618 10:34:09 AM', '20190618 10:34:09 AM');

-- INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (1, 1);

-- INSERT INTO OrderProduct (OrderId, ProductId) VALUES (1, 1);

<<<<<<< HEAD
=======
INSERT INTO OrderProduct (OrderId, ProductId) VALUES (1, 1);

>>>>>>> 1caedad9995be6032badc75121fefa167c623100
