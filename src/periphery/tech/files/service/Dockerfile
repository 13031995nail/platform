# TODO: вернуть после публикации образа в Nexus
# FROM repository.flexberry.org/dotnet/aspnet:10.0-alpine
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
WORKDIR /app

COPY ./publish/ .

RUN adduser --disabled-password --gecos '' newuser
RUN chown -R newuser /app
USER newuser

ENTRYPOINT ["dotnet", "Tmobis.Files.Service.dll"]
