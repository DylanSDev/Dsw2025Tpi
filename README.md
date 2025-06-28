# Trabajo Práctico Integrador

## Desarrollo de Software

### Backend

## Introducción

Se desea desarrollar una plataforma de comercio electrónico (E-commerce). En esta primera etapa el objetivo es construir el módulo de Órdenes, permitiendo la gestión completa de éstas.

### Características de la Solución

- Lenguaje: C# 12.0
- Plataforma: .NET 8

## Integrantes

- 56432 - Vergara Atilio - atilio.l.vergara@gmail.com
- 56695 - Zato Sosa Celina - celinazatososa@gmail.com
- 56425 - Diaz Dylan - dylansdiaz.dev@gmail.com

## Autores

- [@Atilio-V](https://github.com/Atilio-V)
- [@celina-zts](https://github.com/celina-zts)
- [@DylanSDev](https://github.com/DylanSDev)

## Instrucciones para configurar y ejecutar el proyecto localmente

### Prerrequisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (o la versión de `localdb` que viene con Visual Studio)
- [Visual Studio Community 2022 (con ASP.NET CORE)](https://visualstudio.microsoft.com/vs/community/)
- [Git](https://git-scm.com/downloads)

### Pasos

1.  **Clonar el repositorio:**
    Abre una terminal y clona el repositorio en tu máquina local:

    ```bash
    git clone https://github.com/DylanSDev/Dsw2025Tpi
    cd Dsw2025Tpi/
    git checkout dev
    ```

2.  **Configurar la base de datos:**
    Asegúrate de que la cadena de conexión en `Dsw2025Tpi.Api/appsettings.json` y `Dsw2025Tpi.Api/appsettings.Development.json` sea correcta para tu entorno de SQL Server. La configuración actual usa `(localdb)\\mssqllocaldb`.

    ```json
    "ConnectionStrings": {
        "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=Dsw2025Tpi;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
    ```

3.  **Aplicar las migraciones y seedear los datos:**
    El proyecto está configurado para aplicar las migraciones automáticamente y cargar los datos de los clientes desde un archivo JSON al iniciar la aplicación. Puedes verificarlo en el archivo `Dsw2025Tpi.Api/Program.cs`.

    ```csharp
    // ...
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<Dsw2025TpiContext>();
            context.Database.Migrate(); // Aplica las migraciones pendientes
            context.Seedwork<Customer>("sources/Customers.json"); // Usa el método de extensión para seedear los clientes
        }
        // ...
    }
    ```

4.  **Ejecutar el proyecto:**
    Puedes ejecutar el proyecto directamente desde Visual Studio Community 2022. Simplemente abre el archivo de solución (`.sln`) y presiona `F5` o el botón "Run" para iniciar la API.

    Visual Studio debería restaurar automáticamente los paquetes NuGet. Si no lo hace, haz clic derecho en la solución en el "Explorador de soluciones" y selecciona "Restaurar paquetes NuGet".

    Alternativamente, puedes usar la línea de comandos:

    ```bash
    dotnet run --project Dsw2025Tpi.Api/Dsw2025Tpi.Api.csproj
    ```

5.  **Acceder a Swagger:**
    Una vez que la aplicación se esté ejecutando, Swagger se abrirá automáticamente en tu navegador. Si no es así, puedes acceder a la documentación de la API en `https://localhost:7138/swagger/index.html` o la URL que se muestre en tu terminal.

## Endpoints implementados (Hasta el punto 6)

La API sigue los principios RESTful, utilizando los métodos HTTP apropiados para cada operación (GET, POST, PUT). Todas las entradas son validadas rigurosamente, previniendo errores de datos y retornando respuestas 400 Bad Request claras y específicas. La API implementa un manejo robusto y consistente de errores, capturando excepciones y devolviendo respuestas HTTP y mensajes de error apropiados y descriptivos.

A continuación, se describen los endpoints implementados hasta el punto 6:

### Productos

- **`POST /api/products`**

  - **Descripción:** Crea un nuevo producto con los datos proporcionados en el cuerpo de la solicitud.
  - **Cuerpo de la Solicitud (JSON):**
    ```json
    {
      "sku": "SKU_UNICO_1",
      "internalCode": "INT-001",
      "name": "Nombre del Producto",
      "description": "Descripción detallada del producto.",
      "price": 199.99,
      "stock": 50
    }
    ```
  - **Validaciones:**
    - El SKU es obligatorio y único. Si ya existe, retorna un `400 Bad Request`.
    - El nombre es obligatorio.
    - El precio debe ser mayor a 0.
    - El stock no debe ser negativo.
    - Si los datos no son válidos, retorna `400 Bad Request`.
  - **Respuestas:**
    - `201 Created`: Si la creación es exitosa, retorna el objeto del producto creado.
    - `400 Bad Request`: Si los datos son inválidos o duplicados.

- **`GET /api/products`**

  - **Descripción:** Retorna la lista completa de productos activos disponibles.
  - **Respuestas:**
    - `200 OK`: Con una colección de objetos producto.
    - `204 No Content`: Si no hay productos activos registrados.

- **`GET /api/products/{id}`**

  - **Descripción:** Retorna los detalles completos de un producto específico.
  - **Respuestas:**
    - `200 OK`: Con el objeto del producto solicitado.
    - `404 Not Found`: Si no se encuentra un producto con el ID proporcionado o está inactivo.

- **`PUT /api/products/{id}`**

  - **Descripción:** Actualiza los datos de un producto existente con el ID proporcionado.
  - **Cuerpo de la Solicitud (JSON):** Igual que para `POST /api/products`.
  - **Respuestas:**
    - `200 OK`: Con el objeto del producto actualizado.
    - `400 Bad Request`: Si los datos enviados no son válidos.
    - `404 Not Found`: Si no se encuentra un producto con el ID proporcionado.

- **`PATCH /api/products/{id}`**
  - **Descripción:** Modifica el atributo `IsActive` para inhabilitar un producto.
  - **Respuestas:**
    - `204 No Content`: Si la operación fue exitosa.
    - `404 Not Found`: Si no se encuentra un producto con el ID proporcionado o ya está deshabilitado.

### Órdenes

- **`POST /api/orders`**
  - **Descripción:** Permite registrar una nueva orden de compra en el sistema. Antes de crear la orden, se verifica que haya suficiente stock disponible para cada producto solicitado. Si el stock es insuficiente, la orden no se crea y se retorna un error. Si la creación es exitosa, el stock de los productos se decrementa.
  - **Cuerpo de la Solicitud (JSON):**
    ```json
    {
      "customerld": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
      "shippingAddress": "Calle Falsa 123, Ciudad, Pais",
      "billingAddress": "Calle Falsa 123, Ciudad, País",
      "notes": "Notas adicionales sobre el pedido.",
      "orderItems": [
        {
          "productId": "9318924F-92DC-445E-9D5E-19EC67AFC244",
          "quantity": 1
        }
      ]
    }
    ```
  - **Validaciones:**
    - Se valida que la cantidad solicitada para cada `OrderItem` no exceda el `StockQuantity` disponible.
    - Se valida que el `CustomerId` y los `ProductId` existan.
  - **Respuestas:**
    - `201 Created`: Con los detalles completos de la orden creada.
    - `400 Bad Request`: Si los datos de la solicitud son inválidos o incompletos, o si no hay stock suficiente para uno o más productos.
    - `404 Not Found`: Si el cliente o alguno de los productos no existe.
    - `500 Internal Server Error`: En caso de un fallo inesperado del servidor.