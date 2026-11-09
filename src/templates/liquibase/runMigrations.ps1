# Запускать с Working Dir = ./main/dev, ./main/merge, ./main/remote, т.е. конкретного окружения
$timestamp = Get-Date -Format "yyyyMMddHHmmss";
dotnet build ../../../Tmobis/ODataBackend/Tmobis.Backend.csproj && dotnet build ../../../Tmobis/ODataBackend/Tmobis.Backend.csproj && liquibase update && liquibase tag --tag=$timestamp
