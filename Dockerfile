FROM mcr.microsoft.com/dotnet/sdk:9.0-noble AS builder
ARG TARGETARCH

# Copy project file and restore as distinct layers
WORKDIR /source
#COPY --link ./*.csproj .
#RUN dotnet restore -a $TARGETARCH

# Copy source code and publish app
COPY --link . .
#RUN dotnet publish -a $TARGETARCH --no-restore -c Release
RUN dotnet publish -c Release

RUN find .

#CMD ["/source/bin/Release/net8.0/publish/regexplanet-dotnet.dll"]
#
# final image
#
FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble-chiseled-extra
WORKDIR /app
COPY --from=builder /source/bin/Release/net8.0/publish /app
COPY ./wwwroot /app/wwwroot

CMD ["/app/regexplanet-dotnet.dll"]