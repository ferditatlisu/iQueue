#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["iUtility/iUtility.csproj", "iUtility/"]
COPY ["iModel/iModel.csproj", "iModel/"]
RUN dotnet restore "iUtility/iUtility.csproj"
COPY . .
WORKDIR "/src/iUtility"
RUN dotnet build "iUtility.csproj" -c Release -o /app/build

# Install Cron
RUN apt-get update -qq && apt-get -y install cron -qq --force-yes

# Add export environment variable script and schedule
COPY *.sh ./
COPY schedule /etc/cron.d/schedule
RUN sed -i 's/\r//' export_env.sh \
    && sed -i 's/\r//' run_app.sh \
    && sed -i 's/\r//' /etc/cron.d/schedule \
    && chmod +x export_env.sh run_app.sh \
    && chmod 0644 /etc/cron.d/schedule

    # Create log file
RUN touch /var/log/cron.log
RUN chmod 0666 /var/log/cron.log

# Run Cron
CMD /app/export_env.sh && /usr/sbin/cron && tail -f /var/log/cron.log

FROM build AS publish
RUN dotnet publish "iUtility.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "iUtility.dll"]