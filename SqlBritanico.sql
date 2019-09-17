USE master;
GO

IF EXISTS(SELECT * FROM sys.databases WHERE name='britanicoAppDB')  
BEGIN  
    DROP DATABASE britanicoAppDB;  
END  

CREATE DATABASE britanicoAppDB;
GO

USE britanicoAppDB;
GO

SET DATEFORMAT DMY;
GO

CREATE TABLE Empresa (
ID NUMERIC(10) IDENTITY(1,1),
Rut VARCHAR(15) NOT NULL,
RazonSoc VARCHAR(50) NOT NULL,
Nombre VARCHAR(30),
Email VARCHAR(30),
Direccion VARCHAR(30),
Tel VARCHAR(20),
Logo VARCHAR(200),
LogoImagen VARCHAR(200)

CONSTRAINT PK_Empresa PRIMARY KEY (ID),
CONSTRAINT UK_Empresa_RUT UNIQUE (Rut)
);
GO
CREATE INDEX IDX_Empresa_Rut ON Empresa (Rut);
GO


CREATE TABLE Sucursal (
ID NUMERIC(10) IDENTITY (1,1),
Nombre VARCHAR(20),
Email VARCHAR(30),
Direccion VARCHAR(30),
Tel VARCHAR(20),
Ciudad VARCHAR(20),
Encargado VARCHAR(50)

CONSTRAINT PK_Sucursal PRIMARY KEY (ID)
);
GO


CREATE TABLE Convenio (
ID NUMERIC(10) IDENTITY (1,1),
Nombre VARCHAR(20) NOT NULL,
FechaHora DATETIME,
Anio NUMERIC (4),
AsociadoNombre VARCHAR (50) NOT NULL,
AsociadoTel VARCHAR (25),
AsociadoMail VARCHAR (30),
AsociadoDireccion VARCHAR (30),
Descuento NUMERIC (5,2)

CONSTRAINT PK_Conenvenio PRIMARY KEY (ID),
CONSTRAINT UK_Convenio_AnioNombre UNIQUE (Nombre, Anio)
);
GO


CREATE TABLE Email (
ID NUMERIC (10) IDENTITY (1,1),
DestinatarioEmail VARCHAR (30) NOT NULL,
DestinatarioNombre VARCHAR (30),
Asunto VARCHAR (200),
CuerpoHTML VARCHAR (MAX),
FechaHora DATETIME NOT NULL,
Enviado BIT NOT NULL

CONSTRAINT PK_Email PRIMARY KEY (ID)
);
GO
CREATE INDEX IDX_Email_DestinatarioEmail ON Email (DestinatarioEmail);
GO
CREATE INDEX IDX_Email_FechaHora ON Email (FechaHora);
GO


CREATE TABLE Materia (
ID NUMERIC (10),
SucursalID NUMERIC (10) NOT NULL,
Nombre VARCHAR (50) NOT NULL,
Precio NUMERIC (10, 2),
NotaFinalOralMax NUMERIC (3),
NotaFinalWrittingMax NUMERIC (3),
NotaFinalListeningMax NUMERIC (3),

CONSTRAINT PK_Materia PRIMARY KEY (ID),
CONSTRAINT FK_Materia_SucursalID FOREIGN KEY (SucursalID) REFERENCES Sucursal (ID)
);
GO
CREATE INDEX IDX_Materia_Nombre ON Materia (Nombre);
GO


CREATE TABLE MateriaHistorial (
ID NUMERIC (10) IDENTITY (1, 1),
MateriaID NUMERIC (10) NOT NULL,
SucursalID NUMERIC (10) NOT NULL,
Anio NUMERIC (5) NOT NULL,
ExamenPrecio NUMERIC (10, 2),
MensualidadPrecio NUMERIC (10, 2),
CantidadGrupos NUMERIC (5),
CantidadAlumnos NUMERIC (5),

CONSTRAINT PK_MateriaHistorial PRIMARY KEY (ID),
CONSTRAINT FK_MateriaHistorial_MateriaID FOREIGN KEY (MateriaID) REFERENCES Materia (ID),
CONSTRAINT FK_MateriaHistorial_SucursalID FOREIGN KEY (SucursalID) REFERENCES Sucursal (ID)
);
GO


CREATE TABLE Libro (
ID NUMERIC (10) NOT NULL,
MateriaID NUMERIC (10) NOT NULL,
Nombre VARCHAR (50) NOT NULL,
Precio NUMERIC (10, 2) NOT NULL,
Autor VARCHAR (50),
Editorial VARCHAR (50)

CONSTRAINT PK_Libro PRIMARY KEY (ID, MateriaID),
CONSTRAINT FK_Libro_MateriaID FOREIGN KEY (MateriaID) REFERENCES Materia (ID)
);
GO


CREATE TABLE Funcionario (
ID NUMERIC (10) IDENTITY (1, 1),
SucursalID NUMERIC (10),
CI NUMERIC (9),
Email VARCHAR (30),
Nombre VARCHAR (30) NOT NULL,
Telefono VARCHAR (20),
TelefonoAux VARCHAR (20),
Direccion VARCHAR (30),
FechaNac DATETIME,
Clave VARCHAR (500),
Activo BIT NOT NULL,
TipoFuncionario NUMERIC (1),
DebeModificarPassword BIT NOT NULL

CONSTRAINT PK_Funcionario PRIMARY KEY (ID),
CONSTRAINT FK_Funcionario_SucursalID FOREIGN KEY (SucursalID) REFERENCES Sucursal (ID),
CONSTRAINT UK_Funcionario_CI UNIQUE (CI)
);
GO
CREATE INDEX IDX_Funcionario_CI ON Funcionario (CI);
GO


CREATE TABLE Grupo (
ID NUMERIC (10) NOT NULL,
MateriaID NUMERIC (10) NOT NULL,
SucursalID NUMERIC (10),
FuncionarioID NUMERIC (10),
HoraInicio CHARACTER (10),
HoraFin CHARACTER (10),
Precio NUMERIC (10, 2),
Anio NUMERIC (4) NOT NULL,
Activo BIT NOT NULL

CONSTRAINT PK_Grupo PRIMARY KEY (ID, MateriaID),
CONSTRAINT FK_Grupo_SucursalID FOREIGN KEY (SucursalID) REFERENCES Sucursal (ID),
CONSTRAINT FK_Grupo_MateriaID FOREIGN KEY (MateriaID) REFERENCES Materia (ID),
CONSTRAINT FK_Grupo_FuncionarioID FOREIGN KEY (FuncionarioID) REFERENCES Funcionario (ID)
);
GO
CREATE INDEX IDX_Grupo_Anio ON Grupo (Anio);
GO


CREATE TABLE GrupoDias (
GrupoID NUMERIC (10) NOT NULL,
ID NUMERIC (10) NOT NULL,
Dia VARCHAR (20) NOT NULL

CONSTRAINT PK_GrupoDias PRIMARY KEY (GrupoID, ID)
);
GO


CREATE TABLE Estudiante (
ID NUMERIC (10) IDENTITY (1, 1),
Nombre VARCHAR (30) NOT NULL,
TipoDocumento NUMERIC (1) NOT NULL,
CI NUMERIC (13),
Tel VARCHAR (20),
Email VARCHAR (30),
Direccion VARCHAR (30),
FechaNac DATETIME,
Alergico BIT NOT NULL,
Alergias VARCHAR (200),
ContactoAlternativoUno VARCHAR (30),
ContactoAlternativoUnoTel VARCHAR (30),
ContactoAlternativoDos VARCHAR (30),
ContactoAlternativoDosTel VARCHAR (30),
ConvenioID NUMERIC (10),
GrupoID NUMERIC (10),
MateriaID NUMERIC (10),
Activo BIT NOT NULL,
Validado BIT NOT NULL,
Deudor BIT NOT NULL,
TipoPublicidad NUMERIC (2) NOT NULL,
FechaIngreso DATETIME

CONSTRAINT PK_Estudiante PRIMARY KEY (ID),
CONSTRAINT FK_Estudiante_ConvenioID FOREIGN KEY (ConvenioID) REFERENCES Convenio (ID),
CONSTRAINT FK_Estudiante_Grupo FOREIGN KEY (GrupoID, MateriaID) REFERENCES Grupo (ID, MateriaID)
);
GO
CREATE INDEX IDX_Estudiante_CI ON Estudiante (CI);
GO
CREATE INDEX IDX_Estudiante_Nombre ON Estudiante (Nombre);
GO
CREATE INDEX IDX_Estudiante_Activo ON Estudiante (Activo);
GO


CREATE TABLE Matricula (
ID NUMERIC (10) IDENTITY (1, 1),
SucursalID NUMERIC (10) NOT NULL,
Anio NUMERIC (4) NOT NULL,
FechaHora DATETIME,
Precio NUMERIC (10, 2)

CONSTRAINT PK_Matricula PRIMARY KEY (ID),
CONSTRAINT FK_Matricula_SucursalID FOREIGN KEY (SucursalID) REFERENCES Sucursal (ID),
CONSTRAINT UK_Matricula_Anio UNIQUE (SucursalID, Anio)
);
GO
CREATE INDEX IDX_Matricula_Anio ON Matricula (Anio);
GO


CREATE TABLE MatriculaEstudiante (
ID NUMERIC (10) NOT NULL,
MatriculaID NUMERIC (10) NOT NULL,
SucursalID NUMERIC (10),
EstudianteID NUMERIC (10) NOT NULL,
GrupoID NUMERIC (10) NOT NULL,
MateriaID NUMERIC (10) NOT NULL,
FechaHora DATETIME,
FuncionarioID NUMERIC (10),
Descuento NUMERIC (5, 2),
Precio NUMERIC (10, 2)

CONSTRAINT PK_MatriculaEstudiante PRIMARY KEY (ID, MatriculaID, EstudianteID, GrupoID),
CONSTRAINT FK_MatriculaEstudiante_MatriculaID FOREIGN KEY (MatriculaID) REFERENCES Matricula (ID),
CONSTRAINT FK_MatriculaEstudiante_SucursalID FOREIGN KEY (SucursalID) REFERENCES Sucursal (ID),
CONSTRAINT FK_MatriculaEstudiante_EstudianteID FOREIGN KEY (EstudianteID) REFERENCES Estudiante (ID),
CONSTRAINT FK_MatriculaEstudiante_GrupoID FOREIGN KEY (GrupoID, MateriaID) REFERENCES Grupo (ID, MateriaID),
CONSTRAINT FK_MatriculaEstudiante_MateriaID FOREIGN KEY (MateriaID) REFERENCES Materia (ID),
);
GO
CREATE INDEX IDX_MatriculaEstudiante_FechaHora ON MatriculaEstudiante (FechaHora);
GO


CREATE TABLE Mensualidad (
ID NUMERIC (10) IDENTITY (1, 1),
SucursalID NUMERIC (10),
EstudianteID NUMERIC (10) NOT NULL,
FechaHora DATETIME,
GrupoID NUMERIC (10) NOT NULL,
MateriaID NUMERIC (10) NOT NULL,
MesAsociado NUMERIC (2) NOT NULL,
AnioAsociado NUMERIC (4) NOT NULL,
FuncionarioID NUMERIC (10),
Precio NUMERIC (10, 2),
Paga BIT NOT NULL,
FechaVencimiento DATETIME,
Anulado BIT NOT NULL

CONSTRAINT PK_Mensualidad PRIMARY KEY (ID),
CONSTRAINT FK_Mensualidad_SucursalID FOREIGN KEY (SucursalID) REFERENCES Sucursal (ID),
CONSTRAINT FK_Mensualidad_EstudianteID FOREIGN KEY (EstudianteID) REFERENCES Estudiante (ID),
CONSTRAINT FK_Mensualidad_GrupoID FOREIGN KEY (GrupoID, MateriaID) REFERENCES Grupo (ID, MateriaID),
CONSTRAINT FK_Mensualidad_FuncionarioID FOREIGN KEY (FuncionarioID) REFERENCES Funcionario (ID),
CONSTRAINT UK_Mensualidad UNIQUE (EstudianteID, MesAsociado, AnioAsociado, GrupoID)
);
GO
CREATE INDEX IDX_Mensualidad_MesAsociado ON Mensualidad (MesAsociado);
GO


CREATE TABLE Pago (
ID NUMERIC (10) IDENTITY (1, 1),
SucursalID NUMERIC (10),
FechaHora DATETIME,
Concepto NUMERIC (2),
Monto NUMERIC (12, 2),
FuncionarioID NUMERIC (10),
Observacion VARCHAR (200)

CONSTRAINT PK_Pago PRIMARY KEY (ID),
CONSTRAINT FK_Pago_SucursalID FOREIGN KEY (SucursalID) REFERENCES Sucursal (ID),
CONSTRAINT FK_Pago_FuncionarioID FOREIGN KEY (FuncionarioID) REFERENCES Funcionario (ID)
);
GO


CREATE TABLE VentaLibro (
ID NUMERIC (10) NOT NULL,
LibroID NUMERIC (10) NOT NULL,
MateriaID NUMERIC (10) NOT NULL,
EstudianteID NUMERIC (10) NOT NULL,
FechaHora DATETIME,
Precio NUMERIC (10, 2),
Estado NUMERIC (1)

CONSTRAINT PK_VentaLibro PRIMARY KEY (ID, LibroID, MateriaID, EstudianteID),
CONSTRAINT FK_VentaLibro_LibroID FOREIGN KEY (LibroID, MateriaID) REFERENCES Libro (ID, MateriaID),
CONSTRAINT FK_VentaLibro_EstudianteID FOREIGN KEY (EstudianteID) REFERENCES Estudiante (ID),
);
GO
CREATE INDEX IDX_VentaLibro_FechaHora ON VentaLibro (FechaHora);
GO


CREATE TABLE Examen (
ID NUMERIC (10) NOT NULL,
GrupoID NUMERIC (10) NOT NULL,
MateriaID NUMERIC (10) NOT NULL,
FechaHora DATETIME,
AnioAsociado NUMERIC (4),
NotaMinima NUMERIC (2),
Precio NUMERIC (10, 2),
Calificado BIT NOT NULL

CONSTRAINT PK_Examen PRIMARY KEY (ID, GrupoID),
CONSTRAINT FK_Examen_GrupoID FOREIGN KEY (GrupoID, MateriaID) REFERENCES Grupo (ID, MateriaID),
CONSTRAINT UK_Examen UNIQUE (GrupoID, MateriaID, AnioAsociado)
);
GO
CREATE INDEX IDX_Examen_FechaHora ON Examen (FechaHora);
GO


CREATE TABLE ExamenEstudiante (
ID NUMERIC (10) NOT NULL,
ExamenID NUMERIC (10) NOT NULL,
GrupoID NUMERIC (10) NOT NULL,
EstudianteID NUMERIC (10) NOT NULL,
FechaInscripcion DATETIME,
NotaFinal NUMERIC (3),
NotaFinalLetra CHARACTER (1),
Aprobado BIT NOT NULL,
CantCuotas NUMERIC (2),
FormaPago NUMERIC (1),
Pago BIT NOT NULL,
Precio NUMERIC (10, 2),
FuncionarioID NUMERIC (10),
Anulado BIT NOT NULL,
FaltasEnClase NUMERIC (3),
NotaFinalOral NUMERIC (5, 2),
NotaFinalWritting NUMERIC (5, 2),
NotaFinalListening NUMERIC (5, 2),
InternalAssessment NUMERIC (5, 2)

CONSTRAINT PK_ExamenEstudiante PRIMARY KEY (ID, ExamenID, GrupoID, EstudianteID),
CONSTRAINT FK_ExamenEstudiante_ExamenID FOREIGN KEY (ExamenID, GrupoID) REFERENCES Examen (ID, GrupoID),
CONSTRAINT FK_ExamenEstudiante_EstudianteID FOREIGN KEY (EstudianteID) REFERENCES Estudiante (ID),
CONSTRAINT FK_ExamenEstudiante_FuncionarioID FOREIGN KEY (FuncionarioID) REFERENCES Funcionario (ID)
);
GO


CREATE TABLE ExamenEstudianteCuota (
ID NUMERIC (10) NOT NULL,
ExamenEstudianteID NUMERIC (10) NOT NULL,
ExamenID NUMERIC (10) NOT NULL,
GrupoID NUMERIC (10) NOT NULL,
EstudianteID NUMERIC (10) NOT NULL,
NroCuota NUMERIC (2),
Precio NUMERIC (10, 2),
FechaPago DATETIME,
CuotaPaga BIT NOT NULL

CONSTRAINT PK_ExamenEstudianteCuota PRIMARY KEY (ID, ExamenEstudianteID, ExamenID, GrupoID, EstudianteID),
CONSTRAINT FK_ExamenEstudianteCuota_ExamenEstudianteID FOREIGN KEY (ExamenEstudianteID, ExamenID, GrupoID, EstudianteID) REFERENCES ExamenEstudiante (ID, ExamenID, GrupoID, EstudianteID),
);
GO


CREATE TABLE LogError (
ID NUMERIC (10) IDENTITY (1, 1),
FechaHora DATETIME,
Location VARCHAR (50),
Msg VARCHAR (500),
Tipo NUMERIC (1)

CONSTRAINT PK_LogError PRIMARY KEY (ID)
);
GO
CREATE INDEX IDX_LogError_FechaHora ON LogError (FechaHora);
GO


CREATE TABLE Numerador (
Tipo CHARACTER (6) NOT NULL,
Valor NUMERIC (15, 2)

CONSTRAINT PK_Numerador PRIMARY KEY (Tipo)
);
GO


CREATE TABLE Parametro (
ID NUMERIC (10) NOT NULL,
Nombre VARCHAR (20),
Valor VARCHAR (200)

CONSTRAINT PK_Parametro PRIMARY KEY (ID)
);
GO



/*--------------- INSERTS --------------- */

INSERT INTO Empresa VALUES ('140185180014', 'Instituto Britanico', 'Instituto Britanico', 'bbrizolara7@gmail.com', 'Joaquin Suarez 526', '462 24260', '', '');


SET IDENTITY_INSERT Sucursal ON
GO
INSERT INTO Sucursal (ID, Nombre, Email, Direccion, Tel, Ciudad, Encargado) VALUES (0, 'Sin Sucursal', '', '', '', '', '');
GO
SET IDENTITY_INSERT Sucursal OFF
GO
INSERT INTO Sucursal VALUES ('Centro', 'administracion@britanico.uy', 'Joaquin Suarez 526', '462 24260', 'Rivera', 'Nara Fagundez');
INSERT INTO Sucursal VALUES ('Rivera Chico', 'administracion@britanico.uy', 'Cuaro 123', '462 24260', 'Rivera', 'Nara Fagundez');


SET IDENTITY_INSERT Funcionario ON
GO
INSERT INTO Funcionario (ID, SucursalID, CI, Email, Nombre, Telefono, TelefonoAux, Direccion, FechaNac, Clave, Activo, TipoFuncionario, DebeModificarPassword) 
VALUES (0, 0, 0, '', 'Sin Funcionario', '', '', '', '01/01/1950', '', 0, 0, 0);

INSERT INTO Funcionario (ID, SucursalID, CI, Email, Nombre, Telefono, TelefonoAux, Direccion, FechaNac, Clave, Activo, TipoFuncionario, DebeModificarPassword) 
VALUES (1, 1, 48437782, 'bbrizolara7@gmail.com', 'Bruno Brizolara', '099057586', '', '', '14/08/1990', 'q8l8oO7EBgcSvz/SQKDbyg==', 1, 1, 0);

SET IDENTITY_INSERT Funcionario OFF
GO


/* Materias Centro*/
INSERT INTO Materia VALUES (0, 0, 'Default', 0, 40, 50, 0)
INSERT INTO Materia VALUES (1, 1, 'Kinder 1', 1190, 40, 50, 0)
INSERT INTO Materia VALUES (2, 1, 'Kinder 2', 1390, 40, 50, 0)
INSERT INTO Materia VALUES (3, 1, 'Preparatory 1', 1450, 40, 50, 0)
INSERT INTO Materia VALUES (4, 1, 'Preparatory 2', 1590, 40, 50, 0)
INSERT INTO Materia VALUES (5, 1, 'Children 1', 1720, 40, 50, 0)
INSERT INTO Materia VALUES (6, 1, 'Children 2', 1930, 40, 50, 0)
INSERT INTO Materia VALUES (7, 1, 'Children 3', 2150, 40, 50, 0)
INSERT INTO Materia VALUES (8, 1, 'J1', 2250, 40, 50, 0)
INSERT INTO Materia VALUES (9, 1, 'J2', 2350, 40, 50, 0)
INSERT INTO Materia VALUES (10, 1, 'J3', 2390, 40, 50, 0)
INSERT INTO Materia VALUES (11, 1, 'J4', 2450, 40, 50, 0)
INSERT INTO Materia VALUES (12, 1, 'J5', 2560, 40, 40, 10)
INSERT INTO Materia VALUES (13, 1, 'J6', 2750, 40, 40, 10)
INSERT INTO Materia VALUES (14, 1, 'Pre First Certificate of English', 2990, 40, 40, 10)
INSERT INTO Materia VALUES (15, 1, 'First Certificate of English', 3570, 40, 40, 10)
INSERT INTO Materia VALUES (16, 1, 'Cambridge Advanced English', 3850, 40, 40, 10)
INSERT INTO Materia VALUES (17, 1, 'Adultos 1/2', 2750, 40, 50, 0)
INSERT INTO Materia VALUES (18, 1, 'Adultos 3/4', 2990, 40, 40, 10)

/* Materias Anexo*/
INSERT INTO Materia VALUES (19, 2, 'Kinder 1', 1150, 40, 50, 0)
INSERT INTO Materia VALUES (20, 2, 'Kinder 2', 1250, 40, 50, 0)
INSERT INTO Materia VALUES (21, 2, 'Preparatory 1', 1390, 40, 50, 0)
INSERT INTO Materia VALUES (22, 2, 'Children 1', 1490, 40, 50, 0)
INSERT INTO Materia VALUES (23, 2, 'Children 2', 1590, 40, 50, 0)
INSERT INTO Materia VALUES (24, 2, 'Children 3', 1690, 40, 50, 0)
INSERT INTO Materia VALUES (25, 2, 'J1', 1800, 40, 50, 0)
INSERT INTO Materia VALUES (26, 2, 'J2', 1990, 40, 50, 0)
INSERT INTO Materia VALUES (27, 2, 'J3', 2150, 40, 50, 0)
INSERT INTO Materia VALUES (28, 2, 'J4', 2190, 40, 50, 0)
INSERT INTO Materia VALUES (29, 2, 'J5', 2250, 40, 40, 10)
INSERT INTO Materia VALUES (30, 2, 'J6', 2450, 40, 40, 10)


/* Materias Centro*/
INSERT INTO MateriaHistorial VALUES (1, 1, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (2, 1, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (3, 1, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (4, 1, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (5, 1, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (6, 1, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (7, 1, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (8, 1, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (9, 1, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (10, 1, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (11, 1, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (12, 1, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (13, 1, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (14, 1, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (15, 1, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (16, 1, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (17, 1, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (18, 1, 2019, 0, 0, 0, 0)

/* Materias Anexo*/
INSERT INTO MateriaHistorial VALUES (19, 2, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (20, 2, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (21, 2, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (22, 2, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (23, 2, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (24, 2, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (25, 2, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (26, 2, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (27, 2, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (28, 2, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (29, 2, 2019, 0, 0, 0, 0)
INSERT INTO MateriaHistorial VALUES (30, 2, 2019, 0, 0, 0, 0)


INSERT INTO Grupo VALUES (0, 0, 0, 0, '', '', 0, 0, 0);


INSERT INTO Matricula VALUES (1, 2019, '20190101', 990);
INSERT INTO Matricula VALUES (2, 2019, '20190101', 890);


SET IDENTITY_INSERT Convenio ON
INSERT INTO Convenio (ID, Nombre, FechaHora, Anio, AsociadoNombre, AsociadoTel, AsociadoMail, AsociadoDireccion, Descuento) VALUES (0, 'Sin Convenio', '01/01/1950', 1950, '', '', '', '', 0);
SET IDENTITY_INSERT Convenio OFF

INSERT INTO Convenio VALUES ('Circulo Policial', '19/05/2019', 2019, 'Circulo Policial de Rivera', '123456', 'circulo@policial.com', 'Artigas 123', 17);
INSERT INTO Convenio VALUES ('CASMER', '19/05/2019', 2019, 'CASMER Femi Rivera', '123456', 'casmer@femi.com', 'Carambula 123', 17);
INSERT INTO Convenio VALUES ('COMERI', '19/05/2019', 2019, 'COMERI Rivera', '123456', 'comeri@gmail.com', 'Rodo 123', 17);


INSERT INTO Libro VALUES (0, 0, 'Sin Libro', 0, '', '');
INSERT INTO Libro VALUES (1, 1, 'Kinder 1 Practico', 300, '', '');
INSERT INTO Libro VALUES (2, 1, 'Kinder 1 Teorico', 350, '', '');
INSERT INTO Libro VALUES (3, 2, 'Kinder 2 Practico', 300, '', '');
INSERT INTO Libro VALUES (4, 2, 'Kinder 2 Teorico', 350, '', '');
INSERT INTO Libro VALUES (5, 8, 'J1 Teorico', 350, '', '');
INSERT INTO Libro VALUES (6, 8, 'J1 Practico', 300, '', '');
INSERT INTO Libro VALUES (7, 9, 'J2 Teorico', 350, '', '');
INSERT INTO Libro VALUES (8, 9, 'J2 Practico', 300, '', '');
INSERT INTO Libro VALUES (9, 10, 'J3 Teorico', 350, '', '');
INSERT INTO Libro VALUES (10, 10, 'J3 Practico', 300, '', '');
INSERT INTO Libro VALUES (11, 11, 'J4 Teorico', 350, '', '');
INSERT INTO Libro VALUES (12, 11, 'J4 Practico', 300, '', '');
INSERT INTO Libro VALUES (13, 12, 'J5 Teorico', 350, '', '');
INSERT INTO Libro VALUES (14, 12, 'J5 Practico', 300, '', '');


/* Numeradores */
INSERT INTO Numerador VALUES ('LIBRO', 14);
INSERT INTO Numerador VALUES ('GRUPO', 0);
INSERT INTO Numerador VALUES ('MATER', 30);
INSERT INTO Numerador VALUES ('MATRES', 0);
INSERT INTO Numerador VALUES ('VENLIB', 0);
INSERT INTO Numerador VALUES ('EXAMEN', 0);
INSERT INTO Numerador VALUES ('EXAMES', 0);


/* Parametros */
INSERT INTO Parametro VALUES (1, 'Email', 'brunobrizolara1408@gmail.com');
INSERT INTO Parametro VALUES (2, 'Email Nombre', 'Instituto Britanico');
INSERT INTO Parametro VALUES (3, 'Email clave', 'BritanicoApp!');
INSERT INTO Parametro VALUES (4, 'Recargo Mensualidad', '10');

