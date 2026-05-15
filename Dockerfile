# Використовуємо SDK для збірки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копіюємо файл проєкту і відновлюємо пакети
COPY ["oop better.csproj", "./"]
RUN dotnet restore "oop better.csproj"

# Копіюємо решту файлів і публікуємо
COPY . .
RUN dotnet publish "oop better.csproj" -c Release -o /app/publish

# Використовуємо рантайм для запуску
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Azure/Render зазвичай кидають трафік на 80 або 8080 порт
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "oop better.dll"]