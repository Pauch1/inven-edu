# ?? QUICK START GUIDE - InvenEdu

## Get Running in 3 Steps!

### Step 1: Apply Database Migrations
```bash
cd inven-edu
dotnet ef database update
```

**If LocalDB is not installed or not working:**
1. Open `appsettings.json`
2. Change the connection string to your SQL Server instance:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=InvenEduDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### Step 2: Run the Application
```bash
dotnet run
```

### Step 3: Open in Browser
Navigate to: **https://localhost:5001**

---

## ?? Default Login Credentials

### Administrator
- **Email:** `admin@invenedu.com`
- **Password:** `Admin123!`
- **Access:** Full system access

### Regular User
- **Email:** `user@invenedu.com`
- **Password:** `User123!`
- **Access:** Limited to personal records

---

## ?? Try These Features

### As Admin:
1. ? View Dashboard with live statistics
2. ? Add a new inventory item
3. ? Create a category
4. ? Issue material to a user
5. ? Generate PDF reports
6. ? Toggle dark mode
7. ? Search and filter inventory

### As User:
1. ? View personal dashboard
2. ? Browse available inventory
3. ? Check your issuances
4. ? View item details

---

## ?? Quick Feature Tour

| Feature | URL Path | Role Required |
|---------|----------|---------------|
| Admin Dashboard | `/Admin/Dashboard` | Admin |
| Manage Inventory | `/Admin/Inventory` | Admin |
| Manage Issuances | `/Admin/Issuances` | Admin |
| User Dashboard | `/User/Dashboard` | User |
| Browse Items | `/User/BrowseInventory` | User |

---

## ??? Troubleshooting

### Database Connection Issues?
```bash
# Try creating database manually
dotnet ef database update --verbose
```

### Build Errors?
```bash
# Clean and rebuild
dotnet clean
dotnet build
```

### Migration Issues?
```bash
# Remove and recreate migration
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## ?? Sample Data Included

The database seeder automatically creates:
- **2 User Accounts** (1 Admin, 1 User)
- **5 Categories** (Electronics, Furniture, Stationery, Laboratory Equipment, Sports Equipment)
- **15 Inventory Items** with realistic data
- **Sample Issuances** to demonstrate the system

---

## ?? Theme Toggle

Click the **moon/sun icon** in the navigation bar to switch between light and dark modes!

---

## ?? Need More Help?

- Check `README.md` for detailed documentation
- Review `IMPLEMENTATION_STATUS.md` for complete feature list
- All code includes XML documentation comments

---

## ? Verification Checklist

After running the application, verify:

- [ ] Login page loads correctly
- [ ] Admin login works with default credentials
- [ ] Admin dashboard shows statistics
- [ ] Can create/edit/delete inventory items
- [ ] Can issue materials
- [ ] PDF generation works
- [ ] User login works
- [ ] User can browse inventory
- [ ] Dark mode toggle works
- [ ] Responsive design works on mobile

---

**You're all set! Enjoy using InvenEdu! ??**
