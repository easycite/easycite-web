# Setup

## SQL database

We ran our SQL database in a Docker container for simple installation. To create the container on a Docker host, run the following script:
```bash
$ docker run \
    -e "ACCEPT_EULA=Y" \
    -e "SA_PASSWORD=YOUR_PASSWORD" \
    -p 1433:1433 \
    -v /home/easycite-sqldb:/var/opt/mssql \
    -u 0:0 \
    -d \
    --restart always \
    --name easycite-sqldb \
    mcr.microsoft.com/mssql/server:2019-latest
```

Then change your database connection string in the web project in the `appsettings.json` file. Change the `Server=` to the location of your SQL Server.

When running the web app, you provide the password the password as an environment variable. At the bottom of the README is a full list of required environment variables. When running locally, you can also use [User Secrets in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows).

## Neo4j database

We also ran our Neo4j database in a Docker container. To create the container on a Docker host, run the following script:
```sh
$ docker run \
    --name easycite-neo4j \
    -p 7474:7474 -p 7687:7687 \
    -d \
    -v $HOME/neo4j/conf:/conf \
    -v $HOME/neo4j/data:/data \
    -v $HOME/neo4j/logs:/logs \
    -v $HOME/neo4j/import:/var/lib/neo4j/import \
    -v $HOME/neo4j/plugins:/plugins \
    -e "NEO4J_AUTH=USERNAME/PASSWORD" \
    --restart always \
    neo4j:3.5.14
```

Additionally, you'll need to install the Graph Data sciences library from Neo4j. [See here](https://neo4j.com/docs/graph-data-science/current/installation/) for installation instructions. Running the script above, your plugins folder will be in `$HOME/neo4j/plugins`.

## Google application

You will also need to create a Google application to run the OAuth login. Go the [developer dashboard](https://console.developers.google.com/), create a new app, and then create a new OAuth 2.0 client. Later below you pass the client ID and client secret as an environment variable.

## Azure Service Bus

You will also need to set up an Azure service bus namespace. The Standard tier contains the required features to run this app. You will also need to create one queue called `scrape` with the following properties:

- Name: scrape
- Max queue size: 1 GB
- Message TTL: 14 days
- Lock duration: 5 minutes
- Enable dead lettering on message expiration
- Enable partitioning

Additionally, the connection string you provide to the application needs the Manage, Send, and Listen permissions.

## Required environment variables

Below are the required environment variables. The first name is for shell environment variables, and the second is when using `dotnet user-secrets set [name] [value]`. See the [ASP.NET Core documentation](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows) for more information on user secrets.

| Environment variable name | User secret name | Description |
| - | - | - |
| `neo4j__userName` | `neo4j:userName` | The username for the Neo4j database. |
| `neo4j__password` | `neo4j:password` | The password for the Neo4j database. |
| `EasyCitePassword` | `EasyCitePassword` | The password for the SQL database. |
| `Authentication__Google__ClientId` | `Authentication:Google:ClientId` | The client ID for your Google app. |
| `Authentication__Google__ClientSecret` | `Authentication:Google:ClientSecret` | The client secret for your Google app. |
| `ServiceBusConnectionString` | `ServiceBusConnectionString` | The connection string for the Service Bus. |
