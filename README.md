### Setup Development Environment
- You need to have [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed, up and running.

### Running in Development Environment
```powershell
git clone https://github.com/georgy-kirilov/modular-monolith.git ; `
cd ./modular-monolith/; `
cp example.env .env; `
docker-compose up --build
```

### Template Installation
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

## Debugging in VS Code

1. Inside of the `.env` file change the value of `DEBUG_MODE` to `true`
2. Place breakpoints at the places in the code you want to debug
3. Run `docker-compose up api` to start the container
4. Click on `Run and Debug` > Click the green play button next to `ModMonTemplate` > Select `Api` from the dropdown list

## pgAdmin

### Adding the database server

1. Right click on `Servers` on the left hand side of the screen
2. Select `Register` > `Server`
3. Under the `General` tab
  - `Name` - `modmontemplate`
4. Under the `Connection` tab
  - `Host name/address` - `host.docker.internal`
  - `Port` - `49156`
  - `Maintenance database` - `postgres`
  - `Username` - `admin`
  - `Password` - `String1@`
  - `Save password?` - `enabled`
5. Click `Save` button
