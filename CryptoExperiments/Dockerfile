FROM centos AS base
RUN dnf update -y
RUN dnf install -y dotnet-runtime-5.0 lsb

# install csp
COPY linux-amd64.tgz /tmp

RUN tar -zxf /tmp/linux-amd64.tgz -C /tmp \
        && chmod +x /tmp/linux-amd64/install.sh \
        && /tmp/linux-amd64/install.sh \
        && rm -rf /tmp/linux-amd64.tgz \
        && rm -rf /tmp/linux-amd64

RUN /opt/cprocsp/sbin/amd64/cpconfig -license -view
#

WORKDIR /app

FROM centos AS build
RUN dnf update -y
RUN dnf install -y dotnet-sdk-5.0

WORKDIR /src
COPY ["CryptoExperiments.csproj", "CryptoExperiments/"]
RUN dotnet restore "CryptoExperiments/CryptoExperiments.csproj"

WORKDIR "/src/CryptoExperiments"
COPY . .

RUN dotnet build "CryptoExperiments.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CryptoExperiments.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CryptoExperiments.dll"]