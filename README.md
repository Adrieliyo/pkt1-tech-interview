# pkt1-tech-interview

API REST desarrollada en **.NET Core 8.0** para gestionar operaciones relacionadas con los envíos de la una paquetería.

## Características

- Gestión de envíos.
- Control de modificación del estado del envío con validaciones para evitar cambios bruscos y sin sentido.
- Documentación OpenAPI/Swagger.
- Capas bien establecidas que cumplen con Clean Architecture, siguiendo estándares actuales del desarrollo dentro de .NET.

---

## Requisitos previos

Asegúrate de tener instalado:

- **Visual Studio Code 2022**
- **SDK de .NET 8**
- **SQL Server 2022 (Express o Developer edition)**
- **SQL Server Management Studio**

## Configuración Inicial del Proyecto
```bash
git clone <repositorio-url>
cd pkt1-tech-interview
```

### 2. Configurar la Base de Datos SQL Server
#### Crear la base de datos:

```sql
CREATE DATABASE pkt1; 
```
**Nota:** La instancia por defecto está configurada en `ShipmentTracker.Web\appsettings.json`. Tienes que asegurarte que la cadena de conexión apunte a tu propio SQL Server.

### 3. Ejecutar las migraciones necesarias
1. Haz clic derecho sobre ShipmentTracker.Web en el explorador de soluciones y seleccionalo como proyecto de inicio.
2. Abre la consola del Administrador de Paquetes.
3. En el menú desplegable específicamente donde dice "Proyecto determinado" (arriba de la consola), selecciona ShipmentTracker.Infrastructure.
4. Ejecuta el siguiente comando:
```bash
Update-Database
```
Entity Framework Core va a revisar la base de datos. Al ver que está vacía o no existe, va a leer la migración **InitialModel** para crear la tabla **Shipments**, y luego leerá la migración **SeedInitialData** para insertar automáticamente los 5 registros de prueba que se configuraron dentro de `ShipmentTracker.Infrastructure\Migrations\20260728103816_SeedInitialData.cs`.

---

## Documentación de API

Una vez que la aplicación esté corriendo, se accederá automáticamente a la documentación Swagger que se localiza en el siguiente enlace:

http://localhost:7156/swagger

---

## Arquitectura y Decisiones de Diseño
Manejé un patrón de capas para mantener los modelos, la lógica del negocio y los controladores separados. Esto con la finalidad de separar las responsabilidades y facilitar el mantenimiento de la API.
Me basé en este artículo de Medium para poder recordar y seguir buenas prácticas dentro del proyecto:
[Implementando REST Api con .NET Core utilizando Clean Architecture](https://medium.com/@Afaik_/paso-a-paso-implementando-rest-api-con-net-core-utilizando-clean-architecture-d5cb04c4c79)<br>

A pesar de estar la API hecha en .NET 5.0, me las apañé en conjunto de la IA para solucionar problemas del entorno para poder tener todo estable en la versión 8.0 que es con la que más cómodo me siento para desarrollar APIs dentro del entorno de C#. Creo que fue en eso en lo que más tiempo me consumió. Porque cada paso que daba en el artículo tenía que tener noción de que estaba moviendo a mi proyecto para no tener problemas de compatibilidad.

Sinceramente me faltó el manejo de paginación para los resultados de mis endpoints GET, lo cual es esencial, pero ya no me daba cabeza para implementarlo, fue algo que se me ocurrió ya que había terminado con el frontend por la forma en que se me mostraba la tabla.

A este proyecto como backend le invertí 8 horas, teniendo en cuenta las modificaciones que tuve que hacerle para poder que funcionara en conjunto con el frontend.




