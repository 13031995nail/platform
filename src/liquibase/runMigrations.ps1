# Запускать с Working Dir = ./main/dev, ./main/merge, ./main/remote, т.е. конкретного окружения
$timestamp = Get-Date -Format "yyyyMMddHHmmss";
liquibase update && liquibase tag --tag=$timestamp
