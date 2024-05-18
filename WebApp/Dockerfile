FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY . ./
RUN dotnet publish -c Release -o output
FROM nginx:alpine
WORKDIR /var/www/web
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=build /app/output/wwwroot .
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]