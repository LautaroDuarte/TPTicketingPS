# 🎟️ TPTicketingPS - Sistema de Ticketing

Sistema de gestión de eventos y reservas de asientos desarrollado como proyecto académico.

Permite visualizar eventos, consultar disponibilidad de asientos por sector y realizar reservas con control de concurrencia (parcial / en desarrollo).

---

# 🧱 Arquitectura del sistema

El proyecto está dividido en dos partes:

## 🔧 Backend (.NET 8 - Clean Architecture)

Arquitectura basada en capas:

* TPTicketingPS.API → Controllers, middleware, configuración
* TPTicketingPS.Application → casos de uso / lógica de aplicación
* TPTicketingPS.Domain → entidades y reglas de negocio
* TPTicketingPS.Infrastructure → persistencia y acceso a datos (EF Core)

### Tecnologías:

* ASP.NET Core Web API (.NET 8)
* Entity Framework Core 8
* SQL Server Express
* FluentValidation
* Middleware global de manejo de excepciones

---

## 💻 Frontend (React + Vite)

* React 18
* Vite
* React Router DOM
* React Icons
* Bootstrap
* Bootstrap Icons
* CSS personalizado

---

# 🗄️ Base de datos

Se utiliza SQL Server Express.

✔ La base de datos se genera automáticamente mediante EF Core
✔ No requiere creación manual
✔ Se incluyen migraciones y seed inicial

---

# 🌱 Seed de datos

El sistema incluye un seed de prueba:

* 14 eventos base (datos simples para testing rápido)
* 1 evento completo con:

  * sectores
  * filas
  * asientos
  * estados de disponibilidad

Este enfoque fue utilizado para acelerar el desarrollo iterativo del sistema.

---

# ⚙️ Requisitos previos

## Backend

* .NET SDK 8.0
* SQL Server Express

## Frontend

* Node.js 18+

---

# 🚀 Ejecución del proyecto

## 🔧 Backend

```bash
cd TPTicketingPS.API
dotnet restore
dotnet ef database update
dotnet run
```

📍 URLs:

* [https://localhost:39716](https://localhost:39716)
* [http://localhost:39717](http://localhost:39717)

---

## 💻 Frontend

```bash
cd frontend
npm install
npm run dev
```

📍 URL:
[http://localhost:5173](http://localhost:5173)

---

# 📦 Dependencias del frontend

```bash
npm install react-router-dom
npm install react-icons
npm install bootstrap
npm install bootstrap-icons
```

---

# 🎨 UI / Estilos

* Bootstrap para layout general
* CSS personalizado para mapa de asientos
* Estados visuales:

  * Disponible
  * Seleccionado
  * Reservado
  * Vendido

---

# 🎟️ Funcionalidades

## Backend

* Listado de eventos con paginación
* Detalle de evento
* Mapa de asientos por sector y fila
* Reserva de asientos
* Validación de disponibilidad
* Manejo global de errores
* Control de concurrencia (en desarrollo / próximo paso)

## Frontend

* Catálogo de eventos
* Vista de detalle
* Selección interactiva de asientos
* Resumen de reserva en tiempo real
* Confirmación de reserva
* Manejo de errores y feedback visual

---

# 🔐 Autenticación (modo desarrollo)

El sistema no implementa autenticación real (JWT pendiente).

Se utiliza un mecanismo temporal basado en header HTTP:

```
X-User-Id: 1
```

Se setea desde frontend:

```js
localStorage.setItem("userId", "1");
```

También puede modificarse desde la consola del navegador.

---

⚠️ Este mecanismo será eliminado cuando se implemente autenticación JWT.

---

# 🧩 Estructura del proyecto

```
TPTicketingPS/
│
├── TPTicketingPS.API
├── TPTicketingPS.Application
├── TPTicketingPS.Domain
├── TPTicketingPS.Infrastructure
│
├── frontend/
│   ├── src/
│   ├── pages/
│   ├── api/
│
└── README.md
```

---

# 🧪 Ejecución desde cero

## Clonar

```bash
git clone <repo>
cd TPTicketingPS
```

## Backend

```bash
cd TPTicketingPS.API
dotnet restore
dotnet ef database update
dotnet run
```

## Frontend

```bash
cd frontend
npm install
npm run dev
```

---

# 👨‍💻 Autores

Lautaro Duarte
Rodrigo Godoy
