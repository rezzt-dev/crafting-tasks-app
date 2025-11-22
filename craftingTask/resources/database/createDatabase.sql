PRAGMA foreign_keys = ON;

-- TABLA BOARD -----------------------------------
CREATE TABLE IF NOT EXISTS Board (
	BoardId INTEGER PRIMARY KEY AUTOINCREMENT,
	Name TEXT NOT NULL,
	Color TEXT,
	CreationDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	ModificationDate DATETIME
);

-- TABLA PANEL -----------------------------------
CREATE TABLE IF NOT EXISTS Panel (
	PanelId INTEGER PRIMARY KEY AUTOINCREMENT,
	BoardId INTEGER NOT NULL,
	Name TEXT NOT NULL,
	Color TEXT NOT NULL DEFAULT '#1A1A1A',
	"Order" INTEGER NOT NULL,
	CreationDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	FOREIGN KEY (BoardId) REFERENCES Board (BoardId)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);

-- TABLA STATUS -----------------------------------
CREATE TABLE IF NOT EXISTS Status (
	StatusId INTEGER PRIMARY KEY AUTOINCREMENT,
	Name TEXT NOT NULL
);

-- TABLA TASK -------------------------------------
CREATE TABLE IF NOT EXISTS Task (
	TaskId INTEGER PRIMARY KEY AUTOINCREMENT,
	PanelId INTEGER NOT NULL,
	Title TEXT NOT NULL,
	Description TEXT,
	Tag TEXT,
	CreationDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	EndDate DATETIME,
	Priority INTEGER DEFAULT 0,
	StatusId INTEGER NOT NULL,
	Color TEXT,
	FOREIGN KEY (PanelId) REFERENCES Panel (PanelId)
		ON DELETE CASCADE
		ON UPDATE CASCADE,
	FOREIGN KEY (StatusId) REFERENCES Status (StatusId)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);

-- TABLA SUBTASK -------------------------------------
CREATE TABLE IF NOT EXISTS Subtask (
  SubtaskId INTEGER PRIMARY KEY AUTOINCREMENT,
  ParentTaskId INTEGER NOT NULL,
  Title TEXT NOT NULL,
  IsCompleted INTEGER DEFAULT 0, -- 0: No, 1: Sí
  "Order" INTEGER NOT NULL,
  FOREIGN KEY (ParentTaskId) REFERENCES Task(TaskId) ON DELETE CASCADE
);

-- TABLA ATTACHMENT -------------------------------------
CREATE TABLE Attachment (
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  TaskId INTEGER NOT NULL,
  FileName TEXT NOT NULL,
  ContentType TEXT NOT NULL,
  FilePath TEXT,
  Data BLOB,
  CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (TaskId) REFERENCES Task(Id)
);

-- INDICES PARA OPTIMIZAR CONSULTAS ---------------
CREATE INDEX IF NOT EXISTS idx_panel_board ON Panel(BoardId);
CREATE INDEX IF NOT EXISTS idx_task_panel ON Task(PanelId);
CREATE INDEX IF NOT EXISTS idx_task_status ON Task(StatusId);
CREATE INDEX IF NOT EXISTS idx_task_title ON Task(Title);
CREATE INDEX IF NOT EXISTS idx_task_tag ON Task(Tag);
CREATE INDEX IF NOT EXISTS idx_task_priority ON Task(Priority);
CREATE INDEX IF NOT EXISTS idx_task_enddate ON Task(EndDate);
CREATE INDEX IF NOT EXISTS IDX_Subtask_ParentTaskId ON Subtask(ParentTaskId);
CREATE INDEX idx_attachment_task ON Attachment(TaskId);
CREATE INDEX idx_attachment_type ON Attachment(ContentType);

-- TABLA SAVEDFILTER ----------------------------
CREATE TABLE IF NOT EXISTS SavedFilter (
	FilterId INTEGER PRIMARY KEY AUTOINCREMENT,
	Name TEXT NOT NULL,
	CriteriaJson TEXT NOT NULL,
	CreationDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- VALORES POR DEFECTO ----------------------------
INSERT INTO Status (Name) VALUES ('Pendiente'), ('En Progreso'), ('Realizada'), ('Archivada'), ('Personalizada');