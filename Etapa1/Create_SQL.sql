-- Tipos
CREATE TABLE Tipos (
  Id          INTEGER PRIMARY KEY AUTOINCREMENT,
  Nombre      VARCHAR(20) NOT NULL UNIQUE,
  Descripcion TEXT
);

-- Roles
CREATE TABLE Roles (
  Id     INTEGER PRIMARY KEY AUTOINCREMENT,
  Nombre VARCHAR(20) NOT NULL UNIQUE
);

-- Usuarios
CREATE TABLE Usuarios (
  DNI      INTEGER PRIMARY KEY,     
  Nombre   VARCHAR(50) NOT NULL,
  Apellido VARCHAR(50) NOT NULL,
  Email    VARCHAR(100) NOT NULL UNIQUE,
  Password VARCHAR(255) NOT NULL,
  Fecha    DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  F_Update DATETIME
);

-- RolesUsuarios 
CREATE TABLE RolesUsuarios (
  DNI   INTEGER NOT NULL, 
  IdRol INTEGER NOT NULL,
  Fecha DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (DNI, IdRol),   
  FOREIGN KEY (DNI)   REFERENCES Usuarios(DNI),
  FOREIGN KEY (IdRol) REFERENCES Roles(Id)
);

-- Cuentas
CREATE TABLE Cuentas (
  Numero   INTEGER PRIMARY KEY,
  DNI      INTEGER NOT NULL,
  Saldo    DECIMAL(18,2) NOT NULL DEFAULT 0.00,
  Fecha    DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  F_Update DATETIME,
  FOREIGN KEY (DNI) REFERENCES Usuarios(DNI)
);

-- Transacciones
CREATE TABLE Transacciones (
  Id          INTEGER PRIMARY KEY AUTOINCREMENT,
  CtaOrigen   INTEGER,
  CtaDestino  INTEGER NOT NULL,
  IdTipo      INTEGER NOT NULL,
  Monto       DECIMAL(18,2) NOT NULL CHECK (Monto > 0),
  Fecha       DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  Descripcion VARCHAR(255),
  FOREIGN KEY (CtaOrigen)  REFERENCES Cuentas(Numero),
  FOREIGN KEY (CtaDestino) REFERENCES Cuentas(Numero),
  FOREIGN KEY (IdTipo)     REFERENCES Tipos(Id)
);

-- Estados para Plazos Fijos
CREATE TABLE EstadosPlazoFijo (
  Id     INTEGER PRIMARY KEY AUTOINCREMENT,
  Nombre VARCHAR(20) NOT NULL UNIQUE
);

-- Plazos Fijos
CREATE TABLE PlazosFijos (
  Id              INTEGER PRIMARY KEY AUTOINCREMENT,
  IdTransaccion   INTEGER NOT NULL,
  Monto           DECIMAL(18,2) NOT NULL CHECK (Monto > 0),
  TNA             DECIMAL(5,2) NOT NULL DEFAULT 33.00,
  F_Inicio        DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  F_Fin           DATETIME NOT NULL,
  InteresEsperado DECIMAL(18,2),
  IdEstado        INTEGER NOT NULL DEFAULT 1,  
  FOREIGN KEY (IdEstado)       REFERENCES EstadosPlazoFijo(Id),
  FOREIGN KEY (IdTransaccion)  REFERENCES Transacciones(Id),
);

-- Índices
CREATE INDEX idx_cuentas_usuario ON Cuentas(DNI);
CREATE INDEX idx_trans_origen    ON Transacciones(CtaOrigen);
CREATE INDEX idx_trans_destino   ON Transacciones(CtaDestino);
CREATE INDEX idx_trans_tipo      ON Transacciones(IdTipo);