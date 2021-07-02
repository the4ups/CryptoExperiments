FROM centos AS base
RUN dnf update -y
RUN dnf install -y dotnet-runtime-5.0 lsb

# install csp
COPY linux-amd64.tgz /tmp/linux-amd64.tgz

RUN tar -zxf /tmp/linux-amd64.tgz -C /tmp \
        && chmod +x /tmp/linux-amd64/install.sh \
        && /tmp/linux-amd64/install.sh \
        && rm -rf /tmp/linux-amd64.tgz \
        && rm -rf /tmp/linux-amd64

RUN /opt/cprocsp/sbin/amd64/cpconfig -license -view
COPY test.000 /var/opt/cprocsp/keys/root/test.000
RUN chmod 700 /var/opt/cprocsp/keys/root/test.000/ \
    && chmod 600 /var/opt/cprocsp/keys/root/test.000/*
RUN /opt/cprocsp/bin/amd64/csptest -absorb -certs -autoprov
RUN /opt/cprocsp/bin/amd64/certmgr -list

COPY SampleData.txt /tmp
RUN /opt/cprocsp/bin/amd64/csptest -sfsign -sign -add -detached -cades_disable -in /tmp/SampleData.txt -out /tmp/SampleDataSign.txt -my test
RUN /opt/cprocsp/bin/amd64/csptest -sfsign -verify -detached -in /tmp/SampleData.txt -signature /tmp/SampleDataSign.txt -my test

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

RUN dotnet build "CryptoExperiments.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "CryptoExperiments.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CryptoExperiments.dll"]