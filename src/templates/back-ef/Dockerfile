FROM repository.flexberry.org/timepath/tmobis/aspnet:10.0-alpine
WORKDIR /app
COPY ./publish/ .

RUN adduser --disabled-password --gecos '' newuser
RUN chown -R newuser /app
USER newuser


ENTRYPOINT ["dotnet", "Tmobis.Backend.EF.dll"]