FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build-env
WORKDIR /app

RUN git clone https://github.com/arkhivania/NetAssScanner.git
WORKDIR /app/NetAssScanner/Nailhang.Blazor
RUN dotnet publish -c Release -r linux-x64 -o /app/out/
RUN ls /app/out/
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["/app/Nailhang.Blazor"]


