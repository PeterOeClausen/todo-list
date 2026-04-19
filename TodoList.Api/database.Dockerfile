# Microsoft SQL Server 2022 on Ubuntu 22.04
FROM mcr.microsoft.com/mssql/server:2025-latest

# Set environment variables
ENV ACCEPT_EULA=Y
ENV MSSQL_SA_PASSWORD=D3v3l0p3rPassw0rd!
ENV MSSQL_PID=StandardDeveloper

# Expose the default SQL Server port
EXPOSE 1433