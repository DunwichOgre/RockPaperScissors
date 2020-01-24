#/bin/bash

dotnet restore
dotnet build --no-restore

# Run the 3 console apps in different windows

dotnet run --project ./src/SiloHost --no-build & 
sleep 10
dotnet run --project ./src/OrleansClient --no-build &
sleep 10
dotnet run --project ./src/OrleansClient --no-build &