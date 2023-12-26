## Template

### Installation
```powershell
dotnet new uninstall
cd modular-monolith/
dotnet new install . --force
```
### Usage
```powershell
mkdir ./my-projects/my-modular-monolith-app
cd ./my-projects/my-modular-monolith-app
dotnet new modmon -n MyModularMonolithApp
```

## Database Migrations

```powershell
cd modular-monolith/
docker-compose up --build
docker-compose exec -it api /bin/bash
dotnet ef migrations add InitialCreate --project ../Accounts -o Database/Migrations
dotnet ef database update
```

## Seq

### Generate password hash
```powershell
echo 'String1@' | docker run --rm -i datalust/seq config hash
```

## Certificates

### Generate data protection certificate files
- data-protection.pfx
- data-protection.key
- data-protection.crt

```bash
cd {VOLUMES_PATH}
mkdir ./backend/shared/data-protection/certificates
cd ./backend/shared/data-protection/certificates
openssl req -x509 -newkey rsa:4096 -sha256 -days 36525 -nodes -keyout data-protection.key -out data-protection.crt -subj "/CN=local"
openssl pkcs12 -export -out data-protection.pfx -inkey data-protection.key -in data-protection.crt
```
