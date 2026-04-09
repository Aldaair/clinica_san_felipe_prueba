# Clínica San Felipe - Microservicios

Este proyecto está compuesto por múltiples microservicios desarrollados en **.NET 8.0**, un **frontend en React** y comunicación asíncrona mediante **RabbitMQ**.

## Requisitos previos

Antes de ejecutar el proyecto, asegúrate de tener instalado lo siguiente:

- [.NET SDK 8.0](https://dotnet.microsoft.com/)
- [Node.js y npm](https://nodejs.org/)
- [Docker](https://www.docker.com/)
- SQL Server

---

## 1. Creación de bases de datos

Primero, se deben crear las siguientes bases de datos en SQL Server:

- `auth_csf_db`
- `movements_csf_db`
- `orchestator_csf_db`
- `product_csf_db`
- `purchase_csf_db`
- `sales_csf_db`

Una vez creadas, se debe ejecutar el script ubicado en la ruta:

```bash
database/scripts/script.sql

Importante

No es necesario preocuparse por ejecutar el script en una base de datos específica, ya que el propio script crea las tablas correspondientes para cada base de datos.

2. Usuario administrador por defecto

No es necesario crear manualmente un usuario administrador para ingresar a la aplicación web.

El sistema ya incluye un seeder con las credenciales iniciales:

Usuario: Admin
Contraseña: Admin123*

3. Restaurar y compilar la solución

Ubícate en la carpeta raíz de la solución:

clinica_san_felipe

Luego ejecuta los siguientes comandos:

dotnet restore
dotnet build

4. Levantar RabbitMQ con Docker

Para la comunicación entre microservicios, el sistema utiliza RabbitMQ.

Se ha incluido el archivo:

docker-compose.rabbitmq.yml

Ejecuta el siguiente comando desde la raíz del proyecto:

docker compose -f docker-compose.rabbitmq.yml up -d

Si deseas detener RabbitMQ:

docker compose -f docker-compose.rabbitmq.yml down

5. Ejecutar los microservicios

Desde la carpeta raíz clinica_san_felipe, puedes ejecutar cada microservicio con el comando:

dotnet run --project <ruta-del-proyecto-api>

Ejemplo para KardexQueryService:

dotnet run --project src/Services/KardexQueryService/KardexQueryService.Api/KardexQueryService.Api.csproj

Repite el mismo procedimiento para cada microservicio, cambiando la ruta del proyecto correspondiente.

Ejemplo general
dotnet run --project src/Services/AuthService/AuthService.Api/AuthService.Api.csproj
dotnet run --project src/Services/ProductService/ProductService.Api/ProductService.Api.csproj
dotnet run --project src/Services/PurchaseService/PurchaseService.Api/PurchaseService.Api.csproj
dotnet run --project src/Services/SalesService/SalesService.Api/SalesService.Api.csproj
dotnet run --project src/Services/MovementService/MovementService.Api/MovementService.Api.csproj
dotnet run --project src/Services/SagaOrchestratorService/SagaOrchestratorService.Api/SagaOrchestratorService.Api.csproj
dotnet run --project src/Services/KardexQueryService/KardexQueryService.Api/KardexQuerySe

6. Ejecutar el frontend

El frontend fue desarrollado en React y se encuentra dentro de la carpeta:

/frontend

Desde la raíz del proyecto, ingresa a esa carpeta:

cd frontend

Luego ejecuta:

npm install
npm run dev
7. Pruebas de endpoints con Postman

Se adjunta una colección de Postman para facilitar las pruebas de los endpoints.

La colección ya contempla variables para:

URLs de las APIs
Token de autenticación

Se recomienda:

Importar la colección en Postman.
Configurar las variables de entorno según los puertos o URLs locales de cada microservicio.
Autenticarse con el usuario administrador por defecto.
Usar el token generado para probar los endpoints protegidos.
8. Flujo recomendado de ejecución

El orden recomendado para levantar el sistema es el siguiente:

Crear las bases de datos.
Ejecutar el script database/scripts/script.sql.
Levantar RabbitMQ con Docker.
Restaurar y compilar la solución con dotnet restore y dotnet build.
Levantar los microservicios.
Levantar el frontend con npm run dev.
Probar endpoints con la colección de Postman.