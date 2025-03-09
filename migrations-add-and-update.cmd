dotnet-ef database drop -f -c AccountsDbContext -p .\src\Accounts\VZOR.Accounts.Infrastructure\ -s .\src\VZOR.Web\

dotnet-ef migrations remove -c AccountsDbContext -p .\src\Accounts\VZOR.Accounts.Infrastructure\ -s .\src\VZOR.Web\

dotnet-ef migrations add Init -c AccountsDbContext -p .\src\Accounts\VZOR.Accounts.Infrastructure\ -s .\src\VZOR.Web\

dotnet-ef database update -c AccountsDbContext -p .\src\Accounts\VZOR.Accounts.Infrastructure\ -s .\src\VZOR.Web\
