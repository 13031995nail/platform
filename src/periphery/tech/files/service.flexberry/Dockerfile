# Для локального запуска приложения требуется подкорректировать Dockerfile
FROM repository.flexberry.org/dotnet/aspnet:8.0
RUN apt-get update \
  && apt-get -y install --no-install-recommends \
  fonts-liberation fontconfig
RUN fc-cache -fv
WORKDIR /app

COPY ./publish/ .

RUN adduser --disabled-password --gecos '' newuser
RUN chown -R newuser /app
USER newuser

ENTRYPOINT ["dotnet", "Tmobis.Files.Service.Flexberry.dll"]