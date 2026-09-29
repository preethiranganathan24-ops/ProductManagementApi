# ProductManagementApi

# Product & Category Management API

A .NET 6 Web API application that performs CRUD operations for **Products** and **Categories** using **Entity Framework Core**, **LINQ**, and **SQL Server**.

## Technologies Used

* .NET 6
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* LINQ
* C#
* Swagger / OpenAPI

---

## Project Structure

```text
Project
│
├── Controllers
│   ├── ProductsController.cs
│   └── CategoriesController.cs
│
├── Models
│   ├── Product.cs
│   └── Category.cs
│
├── Data
│   └── ApplicationDbContext.cs
│
├── SQL
│   └── DatabaseScript.sql
│
├── appsettings.json
├── Program.cs
└── README.md
```

> Folder and file names may vary depending on the project structure.

---

# Database Design

The application uses two SQL Server tables:

## Products

| Column      | Data Type     | Description               |
| ----------- | ------------- | ------------------------- |
| ProductId   | int           | Primary Key               |
| Name        | nvarchar(100) | Product name              |
| Description | nvarchar(255) | Product description       |
| Price       | decimal       | Product price             |
| Stock       | int           | Available stock           |
| CategoryId  | int           | Foreign Key to Categories |

## Categories

| Column       | Data Type     | Description   |
| ------------ | ------------- | ------------- |
| CategoryId   | int           | Primary Key   |
| CategoryName | nvarchar(100) | Category name |

### Relationship

A **Category can have multiple Products**.

```text
Categories
    │
    │ 1
    │
    │ *
Products
```

`Products.CategoryId` is a foreign key referencing `Categories.CategoryId`.

---

# SQL Server Configuration

The SQL database creation and table creation scripts are included inside the application.

Before running the application:

1. Open SQL Server Management Studio.
2. Execute the SQL script included in the project.
3. Create the required database and tables.
4. Update the connection string in `appsettings.json`.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=YOUR_DATABASE_NAME;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Replace:

```text
YOUR_SERVER_NAME
YOUR_DATABASE_NAME
```

with your SQL Server and database details.

### Example

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ProductManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

For SQL Server authentication, the connection string can be configured according to the local SQL Server setup.

> **Important:** Do not commit real database passwords or other credentials to GitHub.

---

# Entity Framework Core

Entity Framework Core is used to communicate with SQL Server.

The application contains:

* `Product` entity
* `Category` entity
* `ApplicationDbContext`
* `DbSet<Product>`
* `DbSet<Category>`

Example:

```csharp
public DbSet<Product> Products { get; set; }
public DbSet<Category> Categories { get; set; }
```

The relationship between Product and Category is configured using Entity Framework Core.

---

# API Endpoints

## Products

### 1. Create Product

**POST**

```text
/api/products
```

Request body:

```json
{
  "name": "Laptop",
  "description": "Developer laptop",
  "price": 65000,
  "stock": 10,
  "categoryId": 1
}
```

---

### 2. Get All Products

**GET**

```text
/api/products
```

Returns all products.

The read operation uses LINQ and Entity Framework Core to retrieve the records from SQL Server.

---

### 3. Get Product by ID

**GET**

```text
/api/products/{id}
```

Example:

```text
/api/products/1
```

Returns the product with the specified ID.

If the product does not exist, an appropriate HTTP response is returned.

---

### 4. Update Product

**PUT**

```text
/api/products/{id}
```

Example:

```text
/api/products/1
```

Request body:

```json
{
  "name": "Updated Laptop",
  "description": "Updated developer laptop",
  "price": 70000,
  "stock": 15,
  "categoryId": 1
}
```

---

### 5. Delete Product

**DELETE**

```text
/api/products/{id}
```

Example:

```text
/api/products/1
```

Deletes the specified product.

---

### 6. Update Product Stock

**PUT**

```text
/api/products/{id}/stock/{stock}
```

Example:

```text
/api/products/1/stock/25
```

Updates the stock quantity of the specified product.

---

### 7. Get Products Above a Price

**GET**

```text
/api/products/price/{price}
```

Example:

```text
/api/products/price/50000
```

Returns products whose price is greater than the specified amount.

This operation uses a LINQ query similar to:

```csharp
products.Where(p => p.Price > price)
```

---

### 8. Get Products by Category

**GET**

```text
/api/products/category/{categoryId}
```

Example:

```text
/api/products/category/1
```

Returns products belonging to the specified category.

---

# Categories API

### 1. Create Category

**POST**

```text
/api/categories
```

Example request:

```json
{
  "categoryName": "Electronics"
}
```

---

### 2. Get All Categories

**GET**

```text
/api/categories
```

Returns all categories.

---

### 3. Get Category by ID

**GET**

```text
/api/categories/{id}
```

Example:

```text
/api/categories/1
```

---

### 4. Update Category

**PUT**

```text
/api/categories/{id}
```

Example:

```text
/api/categories/1
```

Request body:

```json
{
  "categoryName": "Updated Electronics"
}
```

---

### 5. Delete Category

**DELETE**

```text
/api/categories/{id}
```

Example:

```text
/api/categories/1
```

Deletes the specified category according to the application's relationship and validation rules.

---

# LINQ Implementation

LINQ is used for data retrieval and manipulation.

### Products Above a Specific Price

```csharp
var products = await _context.Products
    .Where(p => p.Price > price)
    .ToListAsync();
```

### Products Within a Category

```csharp
var products = await _context.Products
    .Where(p => p.CategoryId == categoryId)
    .ToListAsync();
```

### Find Product by ID

```csharp
var product = await _context.Products
    .FirstOrDefaultAsync(p => p.ProductId == id);
```

### Update Stock

The product is retrieved using LINQ and its stock value is updated before saving changes:

```csharp
var product = await _context.Products
    .FirstOrDefaultAsync(p => p.ProductId == id);

if (product != null)
{
    product.Stock = stock;
    await _context.SaveChangesAsync();
}
```

---

# Validation

Basic validation is implemented for product and category operations.

Examples include:

* Product name should not be empty.
* Product price should be positive.
* Stock should contain a valid value.
* Category ID should exist before creating or updating a product.
* Product ID should exist before updating or deleting.
* Category ID should exist before updating or deleting a category.

Example category validation:

```csharp
var categoryExists = await _context.Categories
    .AnyAsync(c => c.CategoryId == product.CategoryId);

if (!categoryExists)
{
    return BadRequest("Category does not exist.");
}
```

---

# Pagination

The Products read operation can be extended to support pagination using LINQ with `Skip()` and `Take()`.

Example:

```csharp
var products = await _context.Products
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

Where:

* `pageNumber` specifies the page.
* `pageSize` specifies the number of products per page.

---

# Running the Application

## Prerequisites

Make sure the development environment has:

* .NET 6 SDK
* SQL Server
* SQL Server Management Studio (SSMS) or another SQL Server client
* Visual Studio 2022 or Visual Studio Code

---

## Step 1: Create the Database

Open the SQL script included in the project:

```text
SQL/DatabaseScript.sql
```

Run the script against SQL Server.

This creates the required database and tables.

---

## Step 2: Configure the Connection String

Open:

```text
appsettings.json
```

Update:

```json
"DefaultConnection": "YOUR_CONNECTION_STRING"
```

with your local SQL Server connection details.

---

## Step 3: Restore Dependencies

Run:

```bash
dotnet restore
```

---

## Step 4: Build the Project

Run:

```bash
dotnet build
```

---

## Step 5: Run the Application

Run:

```bash
dotnet run
```

The application will start on the configured HTTP/HTTPS ports.

---

# Swagger

Swagger/OpenAPI can be used to test the API endpoints.

After starting the application, open the Swagger URL shown in the application output, for example:

```text
https://localhost:<port>/swagger
```

Swagger allows you to:

* View available endpoints
* Enter request parameters
* Send POST/GET/PUT/DELETE requests
* View API responses
* Test the CRUD operations

---

# CRUD Summary

| Operation            | HTTP Method | Endpoint                              |
| -------------------- | ----------- | ------------------------------------- |
| Create Product       | POST        | `/api/products`                       |
| Get Products         | GET         | `/api/products`                       |
| Get Product          | GET         | `/api/products/{id}`                  |
| Update Product       | PUT         | `/api/products/{id}`                  |
| Delete Product       | DELETE      | `/api/products/{id}`                  |
| Update Stock         | PUT         | `/api/products/{id}/stock/{stock}`    |
| Products by Price    | GET         | `/api/products/price/{price}`         |
| Products by Category | GET         | `/api/products/category/{categoryId}` |
| Create Category      | POST        | `/api/categories`                     |
| Get Categories       | GET         | `/api/categories`                     |
| Get Category         | GET         | `/api/categories/{id}`                |
| Update Category      | PUT         | `/api/categories/{id}`                |
| Delete Category      | DELETE      | `/api/categories/{id}`                |

---

# Key Features

* RESTful Web API
* Product CRUD operations
* Category CRUD operations
* SQL Server database
* Entity Framework Core
* LINQ-based data queries
* Product/Category relationship
* Input validation
* Product filtering by price
* Product filtering by category
* Product stock update
* Pagination support
* Swagger API documentation
* SQL database creation script

---

# Deliverables

The project includes:

1. .NET 6 Web API source code
2. Entity Framework Core models
3. Database context
4. Controllers for Products and Categories
5. SQL Server database/table creation script
6. API endpoints for CRUD operations
7. LINQ-based queries
8. Input validation
9. Project documentation

---

## Notes

The application functionality is implemented using ASP.NET Core Web API, Entity Framework Core, SQL Server, and LINQ.

