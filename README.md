<h1 align="center">🐾 VetCare - Sistema de Gestión Veterinaria</h1>

<p align="center">
  <img src="https://img.shields.io/badge/.NET_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET Core">
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#">
  <img src="https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white" alt="PostgreSQL">
  <img src="https://img.shields.io/badge/Bootstrap-563D7C?style=for-the-badge&logo=bootstrap&logoColor=white" alt="Bootstrap">
</p>

## 📌 Descripción del Proyecto

**VetCare** es una aplicación web integral desarrollada bajo la arquitectura **Modelo-Vista-Controlador (MVC)** utilizando **ASP.NET Core** y **PostgreSQL**. Este sistema está diseñado para digitalizar y optimizar todos los procesos administrativos y clínicos de un centro veterinario.

El proyecto implementa buenas prácticas de desarrollo de software, incluyendo autenticación basada en sesiones, Control de Acceso Basado en Roles (RBAC), herencia de clases para auditorías automáticas y eliminación lógica (Soft Delete).

---

## 🚀 Características Principales

*   **👥 Control de Acceso por Roles (RBAC):** Sistema de login seguro con encriptación SHA-256. El acceso a módulos específicos está restringido por los roles: `Admin`, `Doctor` y `Usuario`.
*   **🏥 Gestión Clínica:** Administración de registros de Mascotas, Dueños, Citas Médicas y Tratamientos con relaciones interconectadas (Eager Loading).
*   **🩺 Plantilla de Médicos:** Directorio de Veterinarios y asignación de turnos a Sucursales físicas.
*   **🔒 Borrado Lógico (Soft Delete):** Ningún registro se elimina permanentemente de la base de datos para mantener la integridad referencial. Se utiliza una clase abstracta `EntityBase` para manejar el estado de activación.
*   **🎨 Diseño Premium y Responsivo:** Interfaz moderna desarrollada con Bootstrap 5, variables CSS dinámicas e iconos integrados, adaptándose perfectamente a dispositivos móviles y de escritorio.

---

## 🛠️ Stack Tecnológico

*   **Backend:** C# / ASP.NET Core 8.0 MVC
*   **Frontend:** HTML5, CSS3, JavaScript, Razor Pages (`.cshtml`), Bootstrap 5
*   **Base de Datos:** PostgreSQL
*   **ORM:** Entity Framework Core (Code-First)
*   **Seguridad:** Atributos de autorización personalizados (`SessionAuthorizeAttribute`) y Criptografía nativa.

---

## ⚙️ Requisitos Previos

Para ejecutar este proyecto de forma local, necesitas tener instalado:
*   [.NET 8.0 SDK](https://dotnet.microsoft.com/download) o superior.
*   [PostgreSQL](https://www.postgresql.org/download/) (Asegúrate de tener el servicio en ejecución).
*   pgAdmin 4 (Opcional, para visualizar la base de datos).
*   Visual Studio Code o Visual Studio 2022.

---

## 🔧 Instalación y Ejecución

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/andylaglaguano9-max/Proyecto.git
   cd Proyecto/VeterinariaApp
   ```

2. **Configurar la cadena de conexión**
   Abre el archivo `appsettings.json` y asegúrate de que el bloque `ConnectionStrings` tenga las credenciales correctas de tu servidor local PostgreSQL (Usuario y Contraseña).
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=VeterinariaDB;Username=postgres;Password=TU_PASSWORD"
   }
   ```

3. **Restaurar las dependencias NuGet**
   ```bash
   dotnet restore
   ```

4. **Crear y poblar la Base de Datos**
   Ejecuta las migraciones de Entity Framework Core para crear el esquema en PostgreSQL:
   ```bash
   dotnet ef database update
   ```
   *(Nota: También puedes usar el archivo `db_script.sql` para generar la base de datos manualmente).*

5. **Ejecutar la aplicación**
   ```bash
   dotnet run
   ```

6. **Acceso al Sistema**
   * Abre tu navegador y dirígete a: `http://localhost:5200` (o el puerto que se asigne).
   * Ingresa con las credenciales por defecto (Configuradas en el DataSeeder):
     * **Usuario Admin:** `admin` | **Contraseña:** `admin123`

---

## 🏛️ Arquitectura

El sistema respeta la estructura clásica de **ASP.NET Core MVC**:
*   `Models/`: Define las entidades del dominio de negocio. Todas heredan de `EntityBase`.
*   `Data/`: Contiene el contexto de la base de datos (`AppDbContext.cs`).
*   `Controllers/`: Intercepta las solicitudes HTTP, aplica la lógica mediante LINQ y retorna Vistas o redirecciones.
*   `Views/`: Vistas de usuario construidas en sintaxis Razor.

---

<p align="center">
  <b>Desarrollado en el marco académico de la Universidad de las Fuerzas Armadas-ESPE.</b>
</p>