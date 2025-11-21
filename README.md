# InvenEdu - University Inventory Management System

A comprehensive, production-ready ASP.NET Core MVC web application for managing university inventory and material issuances.

## ?? Overview

InvenEdu is a full-featured inventory management system designed for university staff and administrators. It provides complete CRUD operations for inventory items, material issuance tracking, user management, and detailed reporting capabilities.

## ? Features

### Admin Features
- **Dashboard**: Real-time statistics and insights
  - Total inventory items
  - Low stock and out-of-stock alerts
  - Active and overdue issuances
  - User count
- **Inventory Management**: Complete CRUD operations for inventory items
  - Add, edit, and delete items
  - Category management
  - Stock level tracking
  - Low stock alerts
- **Issuance Management**: Track material distribution
  - Issue materials to users
  - Mark items as returned
  - View comprehensive issuance history
  - Overdue tracking
- **User Management**: View and manage system users
- **Reports**: Generate PDF reports for inventory and issuances
- **Advanced Search & Filtering**: Multi-criteria search with pagination

### User Features
- **Personal Dashboard**: Overview of issued materials
- **Browse Inventory**: Search and view available items
- **My Issuances**: View personal issuance history
- **Item Details**: Detailed information about inventory items

### Security & Authentication
- Role-based access control (Admin/User)
- ASP.NET Core Identity integration
- Secure authentication and authorization
- Default admin account for initial setup

## ??? Technology Stack

- **Framework**: ASP.NET Core 9.0 MVC
- **ORM**: Entity Framework Core 9.0
- **Database**: SQL Server (LocalDB)
- **Authentication**: ASP.NET Core Identity
- **PDF Generation**: iTextSharp.LGPLv2.Core
- **UI**: Custom CSS with light/dark mode support
- **Icons**: Font Awesome 6.4.0

## ?? Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server or SQL Server Express (LocalDB included with Visual Studio)
- Visual Studio 2022 (recommended) or Visual Studio Code

## ?? Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd inven-edu
```

### 2. Update Connection String (Optional)

Edit `appsettings.json` if you want to use a different database:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=InvenEduDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Apply Database Migrations

```bash
dotnet ef database update
```

This will create the database and seed it with:
- Default admin user
- Sample categories
- Sample inventory items
- Demo issuances

### 5. Run the Application

```bash
dotnet run
```

Or press F5 in Visual Studio.

### 6. Access the Application

Open your browser and navigate to:
```
https://localhost:5001
```

## ?? Default Credentials

**Administrator Account:**
- Email: `admin@invenedu.com`
- Password: `Admin123!`

**Test User Account:**
- Email: `user@invenedu.com`
- Password: `User123!`

> **Important**: Change these passwords in production!

## ?? Project Structure

```
InvenEdu/
??? Controllers/
?   ??? AccountController.cs      # Authentication & registration
?   ??? AdminController.cs        # Admin functionality
?   ??? HomeController.cs         # Home & public pages
?   ??? UserController.cs         # Regular user functionality
??? Models/
?   ??? Entities/                 # EF Core models
?   ?   ??? ApplicationUser.cs
?   ?   ??? Category.cs
?   ?   ??? InventoryItem.cs
?   ?   ??? IssuanceRecord.cs
?   ??? ViewModels/              # UI models
?       ??? AccountViewModels.cs
?       ??? CategoryViewModel.cs
?       ??? DashboardViewModels.cs
?       ??? InventoryViewModels.cs
?       ??? IssuanceViewModels.cs
?       ??? UserViewModel.cs
??? Data/
?   ??? ApplicationDbContext.cs   # Database context
?   ??? DbSeeder.cs              # Seed data
??? Services/                     # Business logic
?   ??? CategoryService.cs
?   ??? InventoryService.cs
?   ??? IssuanceService.cs
?   ??? PdfService.cs
?   ??? Interfaces/
??? Views/                        # Razor views
?   ??? Account/
?   ??? Admin/
?   ??? Home/
?   ??? Shared/
?   ??? User/
??? wwwroot/                      # Static files
    ??? css/
    ?   ??? styles.css           # Custom styles
    ??? js/
    ?   ??? site.js              # JavaScript utilities
    ??? images/
```

## ?? Features in Detail

### Inventory Management
- Add new items with category, quantity, and minimum stock level
- Edit existing items
- Delete items (if not referenced in issuances)
- Track stock levels with automatic low stock detection
- Search and filter by multiple criteria
- Export inventory data to PDF

### Issuance Tracking
- Issue materials to users with quantity and return date
- Automatic inventory quantity adjustment
- Mark items as returned
- Track overdue items
- Filter by status, date range, and search term
- Generate issuance reports in PDF format

### User Management
- View all system users
- Filter by active/inactive status
- Search by name or email
- Track user issuance history

### Dashboard Analytics
- **Admin Dashboard**:
  - Total items count
  - Low stock alerts
  - Out of stock items
  - Active issuances
  - Overdue items
  - Total users
  - Recent activity
  
- **User Dashboard**:
  - Total items issued
  - Currently holding
  - Overdue items
  - Recent history

### UI/UX Features
- **Responsive Design**: Mobile-first approach
- **Light/Dark Mode**: Toggle between themes
- **Clean Interface**: Professional university-grade design
- **Color Scheme**:
  - Primary: #3674B5
  - Secondary: #578FCA
  - Accent: #A1E3F9
  - Light: #D1F8EF
- **Accessibility**: Semantic HTML and ARIA labels
- **Performance**: Optimized queries and pagination

## ?? Database Schema

### Tables
- **AspNetUsers**: Extended Identity user with FirstName, LastName, IsActive
- **Categories**: Inventory item categories
- **InventoryItems**: Items with quantity tracking
- **IssuanceRecords**: Material distribution records
- **AspNetRoles**: User roles (Admin, User)

### Relationships
- Category ? InventoryItems (One-to-Many)
- ApplicationUser ? IssuanceRecords (One-to-Many)
- InventoryItem ? IssuanceRecords (One-to-Many)

## ?? Configuration

### Logging Levels
Configured in `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  }
}
```

### Identity Options
- Password requirements: Minimum 6 characters, requires digit, uppercase, lowercase, and non-alphanumeric
- Lockout: 5 failed attempts, 5-minute lockout
- Cookie expiration: 24 hours with sliding expiration

## ?? API Endpoints

### Account
- `GET /Account/Login` - Login page
- `POST /Account/Login` - Process login
- `GET /Account/Register` - Registration page
- `POST /Account/Register` - Process registration
- `POST /Account/Logout` - Logout user

### Admin
- `GET /Admin/Dashboard` - Admin dashboard
- `GET /Admin/Inventory` - Inventory list
- `GET /Admin/CreateItem` - Create item form
- `POST /Admin/CreateItem` - Process item creation
- `GET /Admin/EditItem/{id}` - Edit item form
- `POST /Admin/EditItem` - Process item update
- `POST /Admin/DeleteItem/{id}` - Delete item
- `GET /Admin/Issuances` - Issuance list
- `GET /Admin/CreateIssuance` - Create issuance form
- `POST /Admin/CreateIssuance` - Process issuance
- `POST /Admin/ReturnIssuance/{id}` - Mark as returned
- `GET /Admin/Categories` - Category list
- `GET /Admin/Users` - User list
- `GET /Admin/DownloadInventoryReport` - Generate PDF
- `GET /Admin/DownloadIssuanceReport` - Generate PDF

### User
- `GET /User/Dashboard` - User dashboard
- `GET /User/MyIssuances` - Personal issuances
- `GET /User/BrowseInventory` - Browse items
- `GET /User/ItemDetails/{id}` - Item details

## ?? Testing

### Manual Testing Checklist
- [ ] User registration and login
- [ ] Admin login
- [ ] Create/Edit/Delete inventory items
- [ ] Create/Edit/Delete categories
- [ ] Issue materials to users
- [ ] Mark issuances as returned
- [ ] Search and filter functionality
- [ ] PDF report generation
- [ ] Light/Dark mode toggle
- [ ] Responsive design on mobile

## ?? Deployment

### IIS Deployment
1. Publish the application:
   ```bash
   dotnet publish -c Release
   ```
2. Copy published files to IIS directory
3. Configure connection string in production `appsettings.json`
4. Set up application pool (.NET CLR version: No Managed Code)
5. Configure IIS bindings

### Azure Deployment
1. Create Azure SQL Database
2. Update connection string
3. Create App Service
4. Deploy using Visual Studio or Azure CLI
5. Apply migrations

## ?? License

This project is licensed under the MIT License.

## ?? Contributors

- Development Team

## ?? Support

For issues, questions, or contributions, please contact the development team.

## ?? Version History

### Version 1.0.0 (Current)
- Initial release
- Complete inventory management
- Issuance tracking
- User management
- PDF reporting
- Light/Dark mode
- Responsive design

## ?? Learning Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity)

---

**InvenEdu** - Streamlining university inventory management with modern technology.
