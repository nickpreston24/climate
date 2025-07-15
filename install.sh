dotnet build
dotnet pack
dotnet tool install --global climate --add-source ./nupkg --ignore-failed-sources

