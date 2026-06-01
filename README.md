# ImportCost Pro

**ImportCost Pro** es una solución de software empresarial de alto nivel diseñada para automatizar la **Liquidación de Costos de Importación**. El sistema permite a las empresas determinar con precisión quirúrgica el costo real de sus mercancías puestas en almacén, integrando gravámenes aduanales, impuestos internos y prorrateo de gastos logísticos.

---

## Arquitectura del Sistema

El proyecto sigue los principios de **Clean Architecture**, garantizando una separación clara de responsabilidades, alta testabilidad y facilidad de mantenimiento.

*   **ImportCostPro.Persistence:** Capa de acceso a datos. Contiene el contexto de base de datos (`AppDbContext`), las entidades del dominio, las configuraciones de EF Core y la implementación de repositorios optimizados.
*   **ImportCostPro.Application:** El "cerebro" del sistema. Contiene los servicios de aplicación, lógica de negocio, motor de cálculo de 15 fases, validaciones con FluentValidation, DTOs y mapeos automáticos con Mapster.
*   **ImportCostPro.WebApp:** Capa de presentación. Implementada en ASP.NET Core 9 MVC con una interfaz de usuario premium utilizando Tailwind CSS 4, animaciones interactivas y micro-servicios de frontend.

---

## Stack Tecnológico

### Backend
*   **Framework:** .NET 9 (ASP.NET Core MVC)
*   **ORM:** Entity Framework Core 9 (SQL Server)
*   **Validación:** FluentValidation
*   **Mapeo:** Mapster (Mapeo de objetos de alto rendimiento)

### Frontend
*   **Estilos:** Tailwind CSS 4 (Diseño responsivo y moderno)
*   **Interactividad:** Lucide Icons, Animate.css, SweetAlert2
*   **Scripts:** jQuery, JavaScript (ES6+)

---

## Funcionalidades Clave

1.  **Gestión de Catálogos Maestros:** Control total de Países, Monedas, Tasas de Cambio, Productos, Importadores, Proveedores y Categorías Arancelarias.
2.  **Motor de Liquidación de 15 Fases:** Algoritmo avanzado que calcula desde el Valor FOB hasta el Costo Unitario Puesto en Almacén, aplicando prorrateos por Peso, Volumen, Valor y Cantidad.
3.  **Gestión de Gastos Logísticos:** Registro de fletes, seguros y otros cargos operativos con conversión automática de divisas.
4.  **Panel de Control (Dashboard):** Métricas en tiempo real sobre inversiones liquidadas, órdenes abiertas y alertas de tasas pendientes.
5.  **Reportes Fiscales:** Generación de reportes de liquidación listos para impresión con membrete corporativo y precisión contable absoluta.

---

## The Dream Team

El desarrollo de este sistema es el resultado de la colaboración técnica y la excelencia académica de:

| Desarrollador | Rol | Matrícula | GitHub |
| :--- | :--- | :--- | :--- |
| **Angel Gonzalez Muñoz** | **Lead Developer** | 2025-1122 | [xNeuNoRo](https://github.com/xNeuNoRo) |
| **Isaias Jose Morillo F.** | Software Developer | 2025-1242 | [IsaiasMorillo](https://github.com/IsaiasMorillo) |
| **Engel Orlando Acosta D.** | Software Developer | 2025-0037 | [notengel](https://github.com/notengel) |

### Profesor
*   **Profesor:** Leonardo Enrique Tavarez Betances
*   **Rol:** Project Master / Esclavizador (Authority & Architect)

---

## Instalación y Ejecución

### Requisitos Previos
*   .NET 9 SDK
*   SQL Server

### Ejecución Local
1. Clonar el repositorio.
2. Actualizar la cadena de conexión a la base de datos en `ImportCostPro.WebApp/appsettings.json`.
3. Ejecutar las migraciones: `dotnet ef database update --project ImportCostPro.Persistence --startup-project ImportCostPro.WebApp`.
4. Correr la aplicación: `dotnet run --project ImportCostPro.WebApp`.

---

> **ITLA** • Desarrollo De Software • 2026
