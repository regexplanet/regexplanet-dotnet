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

#
# final image
#
FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble-chiseled-extra

ARG COMMIT="(not set)"
ARG LASTMOD="(not set)"
ENV COMMIT=$COMMIT
ENV LASTMOD=$LASTMOD

WORKDIR /app
COPY --from=builder /source/bin/Release/net8.0/publish /app
COPY ./wwwroot /app/wwwroot

CMD ["/app/regexplanet-dotnet.dll"]