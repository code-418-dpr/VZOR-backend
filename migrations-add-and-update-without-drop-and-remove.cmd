dotnet-ef migrations add Init -c AccountsDbContext -p .\src\Accounts\VZOR.Accounts.Infrastructure\ -s .\src\VZOR.Web\

dotnet-ef database update -c AccountsDbContext -p .\src\Accounts\VZOR.Accounts.Infrastructure\ -s .\src\VZOR.Web\

dotnet-ef migrations add Init -c ApplicationDbContext -p .\src\Images\VZOR.Images.Infrastructure\ -s .\src\VZOR.Web\

dotnet-ef database update -c ApplicationDbContext -p .\src\Images\VZOR.Images.Infrastructure\ -s .\src\VZOR.Web\
