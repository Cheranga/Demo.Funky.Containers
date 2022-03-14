# Azure functions in containers

## CancelHotelReservation Function

## SendEmail function

## Generate report function

## References

* Problem when building the Docker image for Azure functions V4 with .NET 6

https://giters.com/Azure/azure-functions-dotnet-worker/issues/718
https://github.com/Azure/azure-functions-dotnet-worker/issues/718

If you just create the function app using `func init` commands and so forth, at the time of the docker image build
you'll face some issues.

Best way I found to resolve this was to create the solution using `Rider` and in it's command window to execute the
command `func init --docker-only`. This will create the correct docker file for you.

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS installer-env

# Build requires 3.1 SDK
COPY --from=mcr.microsoft.com/dotnet/core/sdk:3.1 /usr/share/dotnet /usr/share/dotnet

COPY . /src/dotnet-function-app
RUN cd /src/dotnet-function-app && \
    mkdir -p /home/site/wwwroot && \
    dotnet publish *.csproj --output /home/site/wwwroot

# To enable ssh & remote debugging on app service change the base image to the one below
# FROM mcr.microsoft.com/azure-functions/dotnet:4-appservice
FROM mcr.microsoft.com/azure-functions/dotnet:4
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

COPY --from=installer-env ["/home/site/wwwroot", "/home/site/wwwroot"]
```

* Installing the last `Azure Functions Core Tools`

    - Uninstall what you have currently. I used `chocolatey`

`choco uninstall azure-functions-core-tools`

- The install the latest using this [link](https://github.com/Azure/azure-functions-core-tools)

* Building docker images without cache

If you would like to create the image without using the cached images and would like to pull the base images back use
the below commands.

`docker build --no-cache`

`docker build --no-cache --pull`

https://www.codegrepper.com/code-examples/shell/how+to+clear+docker+build+cache
