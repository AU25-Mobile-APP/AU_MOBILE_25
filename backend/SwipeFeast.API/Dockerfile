# https://github.com/dotnet/dotnet-docker/blob/main/samples/aspnetapp/Dockerfile

# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:latest AS build
ARG GOOGLE_API_KEY
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore "SwipeFeast.API.csproj"


RUN dotnet user-secrets set "GoogleApiToken" ${GOOGLE_API_KEY}

# copy and publish app and libraries
COPY . .
RUN dotnet publish "SwipeFeast.API.csproj" --no-restore -o /app


# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:latest
EXPOSE 8080
WORKDIR /app
COPY --from=build /app .
COPY --from=build /root/.microsoft/ /home/app/.microsoft/
RUN chown -R $APP_UID:$APP_UID /home/app/.microsoft/
USER $APP_UID
ENTRYPOINT ["./SwipeFeast.API"]