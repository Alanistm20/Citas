DROP DATABASE IF EXISTS citas;
CREATE DATABASE citas
CHARACTER SET utf8mb4
COLLATE utf8mb4_general_ci;
USE citas;
CREATE TABLE roles (
  id_rol INT AUTO_INCREMENT PRIMARY KEY,
  nombre VARCHAR(50) NOT NULL UNIQUE
);
INSERT INTO roles (nombre) VALUES
('ADMIN'),
('MEDICO'),
('PACIENTE');
CREATE TABLE especialidades (
  id_especialidad INT AUTO_INCREMENT PRIMARY KEY,
  nombre VARCHAR(80) NOT NULL UNIQUE
);
INSERT INTO especialidades (nombre) VALUES
('Medicina General'),
('Pediatría'),
('Cardiología');
CREATE TABLE estados_cita (
  id_estado_cita INT AUTO_INCREMENT PRIMARY KEY,
  nombre VARCHAR(30) NOT NULL UNIQUE
);
INSERT INTO estados_cita (nombre) VALUES
('PENDIENTE'),
('ATENDIDA'),
('CANCELADA');
CREATE TABLE pacientes (
  id_paciente INT AUTO_INCREMENT PRIMARY KEY,
  dni VARCHAR(10) NOT NULL UNIQUE,
  nombre VARCHAR(100) NOT NULL,
  apellido VARCHAR(100) NOT NULL,
  fecha_nacimiento DATE NOT NULL,
  genero CHAR(1),
  telefono VARCHAR(20),
  correo VARCHAR(150),
  direccion VARCHAR(200),
  activo TINYINT(1) DEFAULT 1,
  fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  fecha_actualizacion DATETIME NULL,
  fecha_baja DATETIME NULL
);

INSERT INTO pacientes (dni, nombre, apellido, fecha_nacimiento, genero, telefono, correo)
VALUES
('70123456', 'Ana', 'Pérez', '1995-06-12', 'F', '999111222', 'ana@mail.com'),
('70987654', 'Luis', 'Gómez', '1990-02-20', 'M', '999333444', 'luis@mail.com');
CREATE TABLE medicos (
  id_medico INT AUTO_INCREMENT PRIMARY KEY,
  nombre VARCHAR(100) NOT NULL,
  apellido VARCHAR(100) NOT NULL,
  id_especialidad INT NOT NULL,
  activo TINYINT(1) DEFAULT 1,
  FOREIGN KEY (id_especialidad) REFERENCES especialidades(id_especialidad)
);
INSERT INTO medicos (nombre, apellido, id_especialidad)
VALUES
('Carlos', 'Ramírez', 1),
('María', 'Lozano', 2);
CREATE TABLE usuarios (
  id_usuario INT AUTO_INCREMENT PRIMARY KEY,
  username VARCHAR(50) NOT NULL UNIQUE,
  password VARCHAR(100) NOT NULL,
  id_rol INT NOT NULL,
  id_paciente INT NULL,
  id_medico INT NULL,
  activo TINYINT(1) DEFAULT 1,
  FOREIGN KEY (id_rol) REFERENCES roles(id_rol),
  FOREIGN KEY (id_paciente) REFERENCES pacientes(id_paciente),
  FOREIGN KEY (id_medico) REFERENCES medicos(id_medico)
);
INSERT INTO usuarios (username, password, id_rol)
VALUES ('admin', 'admin123', 1);
INSERT INTO usuarios (username, password, id_rol, id_medico)
VALUES ('medico1', 'medico123', 2, 1);
INSERT INTO usuarios (username, password, id_rol, id_paciente)
VALUES ('paciente1', 'paciente123', 3, 1);
CREATE TABLE citas (
  id_cita INT AUTO_INCREMENT PRIMARY KEY,
  id_paciente INT NOT NULL,
  id_medico INT NOT NULL,
  fecha DATE NOT NULL,
  hora_inicio TIME NOT NULL,
  motivo VARCHAR(300),
  id_estado_cita INT NOT NULL,
  fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (id_paciente) REFERENCES pacientes(id_paciente),
  FOREIGN KEY (id_medico) REFERENCES medicos(id_medico),
  FOREIGN KEY (id_estado_cita) REFERENCES estados_cita(id_estado_cita),
  UNIQUE KEY uq_medico_fecha_hora (id_medico, fecha, hora_inicio)
);
INSERT INTO citas (id_paciente, id_medico, fecha, hora_inicio, motivo, id_estado_cita)
VALUES
(1, 1, CURDATE(), '09:00:00', 'Consulta general', 1),
(2, 2, CURDATE(), '10:00:00', 'Control pediátrico', 1);
DROP PROCEDURE IF EXISTS usp_usuarios_login;
DELIMITER $$
CREATE PROCEDURE usp_usuarios_login(
  IN p_username VARCHAR(50),
  IN p_password VARCHAR(100)
)
BEGIN
  SELECT 
    u.id_usuario,
    u.username,
    u.id_rol,
    r.nombre AS rol,
    IFNULL(u.id_paciente, 0) AS id_paciente,
    IFNULL(u.id_medico, 0) AS id_medico,
    u.activo
  FROM usuarios u
  JOIN roles r ON r.id_rol = u.id_rol
  WHERE u.username = p_username
    AND u.password = p_password
    AND u.activo = 1
  LIMIT 1;
END$$
DELIMITER ;
