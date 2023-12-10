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

### Generate data protection certificate

```
openssl req -x509 -newkey rsa:4096 -sha256 -days 36525 -nodes -keyout data-protection.key -out data-protection.crt -subj "/CN=local"

openssl pkcs12 -export -out data-protection.pfx -inkey data-protection.key -in data-protection.crt
```
