#!/bin/bash
echo Update Sql database? y or n
read ans
if [ "$ans" == "y" ] || [ "$ans" == "Y" ]; then
	cd Projects/SqlResultUpdater2
	dotnet restore
	dotnet run
	cd ../..
fi
cd Projects/MLFooty3/GetFixtures
dotnet restore
dotnet run
cd ../CreateModifyData
dotnet restore
dotnet run
cd ../ConsumeModelApp
dotnet restore
dotnet run
cd ../TestingConsoleApp
dotnet restore
dotnet run
cd ../../..
