# CRAFTING TASKS
Gestor local de tableros y tareas con interfaz moderna desarrollada en WPF y C#.

---
### DESCRIPCION GENERAL
**CraftingTask** es una aplicación local diseñada para la gestión y administración de tableros y tareas.  
Permite organizar el flujo de trabajo mediante paneles configurables al estilo Kanban, proporcionando una visualización clara y adaptable a las necesidades del usuario.

Las principales funcionalidades incluyen:
- Creación de múltiples tableros con nombre y color personalizados.
- Gestión de paneles dentro de cada tablero, incluyendo los predeterminados: Pendientes, En proceso y Realizadas.
- Creación, edición y organización de tareas.
- Almacenamiento local mediante SQLite para garantizar independencia de conexión y velocidad.
- Interfaz moderna y minimalista desarrollada íntegramente con XAML.

---
### TECNOLOGIAS UTILIZADAS
| Categoría | Tecnologías |
|----------|-------------|
| Lenguajes | C# |
| Framework principal | .NET WPF |
| Framework alternativo / expansión futura | Avalonia |
| Almacenamiento | SQLite |
| Interfaz | XAML |
| Control de versiones | Git |

---
### ESTRUCTURA DEL SISTEMA Y MODELO DE DATOS
CraftingTask utiliza una estructura clara y extensible basada en cuatro entidades principales: Board, Panel, Task y Status.

#### 1. BOARD
Representa un tablero contenedor de paneles.
| Campo | Tipo | Descripción |
|-------|------|-------------|
| board_id | int | Identificador del tablero |
| name | string | Nombre asignado |
| color | string | Color representativo |
| creation_date | datetime | Fecha de creación |
| modification_date | datetime | Fecha de modificación |

#### 2. PANEL
Representa un panel dentro de un tablero.
| Campo | Tipo |
|-------|------|
| panel_id | int |
| board_id | int |
| name | string |
| color | string |
| order | int (posicionamiento dentro del tablero) |
| creation_date | datetime |

#### 3. TASK
Representa una tarea dentro de un panel.
| Campo | Tipo |
|-------|------|
| task_id | int |
| panel_id | int |
| title | string |
| description | string |
| tag | string |
| creation_date | datetime |
| end_date | datetime |
| priority | int |
| status_id | int |


#### 4. STATUS
Define el estado de una tarea.

| Campo | Tipo |
|-------|------|
| status_id | int |
| name | string |

---
### INSTALACION Y EJECUCION
#### 1. CLONACION DEL REPOSITORIO
```sh
git clone https://github.com/usuario/CraftingTask.git
cd CraftingTask
```

---