### Install template

```
dotnet new install . --force
```

### Use template

```
dotnet new modmon -n MyCoolApplication
```

Working with database migrations

Enter the running API container:

```powershell
cd /your/docker-compose/file/path
docker-compose up --build
docker-compose exec -it api /bin/bash
dotnet ef migrations add InitialCreate --project ../Accounts -o Database/Migrations
dotnet ef database update
```

Generate Seq password hash:
```powershell
echo 'String1@' | docker run --rm -i datalust/seq config hash
```
