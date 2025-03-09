dotnet clean ./

dotnet restore ./backend.sln

dotnet build ./ --configuration Release --no-restore