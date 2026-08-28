# Sales Billing System

## Project Overview

Sales Billing System is a web-based application developed using **ASP.NET MVC 5, C#, Entity Framework 6, and SQL Server**.

The project is designed to manage products, customers, and sales invoices in a structured and user-friendly way. Users can create and manage products and customers, generate sales invoices with multiple products, automatically calculate GST and invoice amounts, view invoice details, cancel invoices, and print invoices.

The project follows a layered architecture using:

```text
Views
   ↓
Controllers
   ↓
Services
   ↓
Repositories
   ↓
Entity Framework
   ↓
SQL Server Database
```

---

# Features

## Product Master

- Add Product
- View Product List
- Edit Product
- Delete Product
- Search Products

Product details include:

- Product Name
- SKU
- Unit
- Selling Price
- GST Percentage
- Active Status

---

## Customer Master

- Add Customer
- View Customer List
- Edit Customer
- Delete Customer
- Search Customers

Customer details include:

- Customer Name
- Mobile Number
- Address
- GSTIN

---

## Sales Invoice

The system allows users to create sales invoices with multiple products.

Invoice Header:

- Invoice Number
- Invoice Date
- Customer

Invoice Items:

- Product
- Quantity
- Rate
- Discount
- GST Percentage
- Taxable Amount
- GST Amount
- Total Amount

The system automatically calculates:

```text
Taxable Amount = (Quantity × Rate) - Discount

GST Amount = Taxable Amount × GST Percentage / 100

Total Amount = Taxable Amount + GST Amount
```

The invoice summary includes:

- Total Taxable Amount
- Total GST Amount
- Grand Total

---

## Invoice Management

The application provides the following invoice features:

- Create Sales Invoice
- Add Multiple Invoice Items
- View Invoice Details
- Search Invoices
- Filter Invoices by Date Range
- Print Invoice
- Cancel Invoice

Cancelled invoices are not deleted from the database. Their status is updated to `Cancelled`.

---

# Technologies Used

- ASP.NET MVC 5
- C#
- .NET Framework
- Entity Framework 6
- SQL Server
- HTML
- CSS
- Bootstrap
- JavaScript
- jQuery

---

# Project Structure

```text
Sales_Billing_System
│
├── Controllers
│   ├── ProductController.cs
│   ├── CustomerController.cs
│   └── SalesInvoiceController.cs
│
├── Data
│   └── SalesBillingDbContext.cs
│
├── Models
│   ├── Product_Master.cs
│   ├── Customer_Master.cs
│   ├── Sales_Invoice.cs
│   └── Sales_Invoice_Item.cs
│
├── Repositories
│
├── Services
│
├── Views
│   ├── Product
│   ├── Customer
│   └── SalesInvoice
│
├── Migrations
│
├── README.md
│
└── SalesBillingDatabase.sql
```

---

# How to Run the Project

## Prerequisites

Make sure the following software is installed:

- Visual Studio 2022
- SQL Server
- SQL Server Management Studio (SSMS)
- .NET Framework required by the project

---

## Step 1: Clone or Download the Project

Clone the GitHub repository or download the project source code.

---

## Step 2: Create the Database

A SQL database script is included in the project:

```text
SalesBillingDatabase.sql
```

Open **SQL Server Management Studio (SSMS)** and execute this script to create the required database and tables.

---

## Step 3: Configure the Database Connection

Open the following file:

```text
Web.config
```

Locate the connection string:

```xml
<connectionStrings>
    <add name="SalesBillingDbContext"
         connectionString="Data Source=YOUR_SERVER_NAME;
         Initial Catalog=YOUR_DATABASE_NAME;
         Integrated Security=True;
         MultipleActiveResultSets=True"
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

Replace:

```text
YOUR_SERVER_NAME
```

with your SQL Server instance name.

Also replace:

```text
YOUR_DATABASE_NAME
```

with the name of the database created using the provided SQL script.

Example:

```xml
<connectionStrings>
    <add name="SalesBillingDbContext"
         connectionString="Data Source=.\SQLEXPRESS;
         Initial Catalog=SalesBillingDb;
         Integrated Security=True;
         MultipleActiveResultSets=True"
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

---

## Step 4: Restore NuGet Packages

Open the project solution in Visual Studio.

Then restore the required NuGet packages.

The project uses:

```text
Entity Framework 6
```

Visual Studio should automatically restore the required packages.

---

## Step 5: Build the Project

In Visual Studio, click:

```text
Build → Build Solution
```

Make sure the project builds successfully without errors.

---

## Step 6: Run the Application

Press:

```text
Ctrl + F5
```

or click the **IIS Express** button in Visual Studio.

The application will open in your browser.

---

# Database Tables

The application uses the following main tables:

```text
Product_Master

Customer_Master

Sales_Invoice

Sales_Invoice_Item
```

Database relationships:

```text
Customer
   │
   │ One Customer
   │
   ▼
Sales Invoice
   │
   │ One Invoice
   │
   ▼
Multiple Invoice Items
   │
   └──────────► Product
```

---

# Author

**Gouri Rajmane**