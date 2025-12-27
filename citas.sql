-- =========================================================
-- BD CITAS - SCRIPT FINAL LIMPIO (SIN DUPLICADOS)
-- =========================================================

-- =========================================================
-- 1) RECREAR BD
-- =========================================================
DROP DATABASE IF EXISTS citas;
CREATE DATABASE citas
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_general_ci;
USE citas;

-- =========================================================
-- 2) TABLAS MAESTRAS
-- =========================================================
CREATE TABLE roles (
  id_rol INT AUTO_INCREMENT PRIMARY KEY,
  nombre VARCHAR(50) NOT NULL UNIQUE
);

INSERT INTO roles (nombre) VALUES
('ADMIN'), ('MEDICO'), ('PACIENTE');

CREATE TABLE especialidades (
  id_especialidad INT AUTO_INCREMENT PRIMARY KEY,
  nombre VARCHAR(80) NOT NULL UNIQUE
);

INSERT INTO especialidades (nombre) VALUES
('Medicina General'), ('Pediatría'), ('Cardiología');

CREATE TABLE estados_cita (
  id_estado_cita INT AUTO_INCREMENT PRIMARY KEY,
  nombre VARCHAR(30) NOT NULL UNIQUE
);

INSERT INTO estados_cita (nombre) VALUES
('PENDIENTE'), ('ATENDIDA'), ('CANCELADA');

-- =========================================================
-- 3) ENTIDADES
-- =========================================================
CREATE TABLE pacientes (
  id_paciente INT AUTO_INCREMENT PRIMARY KEY,
  dni VARCHAR(10) NOT NULL UNIQUE,
  nombre VARCHAR(100) NOT NULL,
  apellido VARCHAR(100) NOT NULL,
  fecha_nacimiento DATE NULL,
  genero CHAR(1),
  telefono VARCHAR(20),
  correo VARCHAR(150),
  direccion VARCHAR(200),
  activo TINYINT(1) DEFAULT 1,
  fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  fecha_actualizacion DATETIME NULL,
  fecha_baja DATETIME NULL
);

CREATE TABLE medicos (
  id_medico INT AUTO_INCREMENT PRIMARY KEY,
  nombre VARCHAR(100) NOT NULL,
  apellido VARCHAR(100) NOT NULL,
  id_especialidad INT NOT NULL,
  activo TINYINT(1) DEFAULT 1,
  FOREIGN KEY (id_especialidad) REFERENCES especialidades(id_especialidad)
);

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

-- =========================================================
-- 4) CITAS (precio va en tabla)
-- =========================================================
CREATE TABLE citas (
  id_cita INT AUTO_INCREMENT PRIMARY KEY,
  id_paciente INT NOT NULL,
  id_medico INT NOT NULL,
  fecha DATE NOT NULL,
  hora_inicio TIME NOT NULL,
  motivo VARCHAR(300),
  precio DECIMAL(10,2) NOT NULL DEFAULT 0.00,
  id_estado_cita INT NOT NULL,
  fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (id_paciente) REFERENCES pacientes(id_paciente),
  FOREIGN KEY (id_medico) REFERENCES medicos(id_medico),
  FOREIGN KEY (id_estado_cita) REFERENCES estados_cita(id_estado_cita),
  UNIQUE KEY uq_medico_fecha_hora (id_medico, fecha, hora_inicio)
);

-- =========================================================
-- 5) DATA DE PRUEBA (OPCIONAL)
-- =========================================================
INSERT INTO pacientes (dni, nombre, apellido, fecha_nacimiento, genero, telefono, correo)
VALUES
('70123456', 'Ana', 'Pérez', '1995-06-12', 'F', '999111222', 'ana@mail.com'),
('70987654', 'Luis', 'Gómez', '1990-02-20', 'M', '999333444', 'luis@mail.com');

INSERT INTO medicos (nombre, apellido, id_especialidad)
VALUES
('Carlos', 'Ramírez', 1),
('María', 'Lozano', 2);

INSERT INTO usuarios (username, password, id_rol)
VALUES ('admin', 'admin123', 1);

INSERT INTO usuarios (username, password, id_rol, id_medico)
VALUES ('medico1', 'medico123', 2, 1);

INSERT INTO usuarios (username, password, id_rol, id_paciente)
VALUES ('paciente1', 'paciente123', 3, 1);

INSERT INTO citas (id_paciente, id_medico, fecha, hora_inicio, motivo, precio, id_estado_cita)
VALUES
(1, 1, CURDATE(), '09:00:00', 'Consulta general', 0.00, 1),
(2, 2, CURDATE(), '10:00:00', 'Control pediátrico', 0.00, 1);

-- =========================================================
-- 6) STORED PROCEDURES (VERSIÓN ÚNICA)
-- =========================================================
DROP PROCEDURE IF EXISTS usp_usuarios_login;
DROP PROCEDURE IF EXISTS usp_medicos_listar;
DROP PROCEDURE IF EXISTS usp_pacientes_listar;
DROP PROCEDURE IF EXISTS usp_pacientes_listar_sin_filtro;
DROP PROCEDURE IF EXISTS usp_citas_horarios_disponibles;
DROP PROCEDURE IF EXISTS usp_citas_reservar;
DROP PROCEDURE IF EXISTS usp_citas_listar;
DROP PROCEDURE IF EXISTS usp_citas_buscar_por_id;
DROP PROCEDURE IF EXISTS usp_citas_crear;
DROP PROCEDURE IF EXISTS usp_citas_actualizar;
DROP PROCEDURE IF EXISTS usp_citas_eliminar;

DELIMITER $$

-- -------------------------
-- LOGIN
-- -------------------------
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

-- -------------------------
-- MEDICOS (combo)
-- -------------------------
CREATE PROCEDURE usp_medicos_listar()
BEGIN
  SELECT
    m.id_medico AS IdMedico,
    CONCAT(m.nombre,' ',m.apellido) AS Nombre,
    e.nombre AS Especialidad,
    m.activo AS Activo
  FROM medicos m
  INNER JOIN especialidades e ON e.id_especialidad = m.id_especialidad
  WHERE m.activo = 1
  ORDER BY m.apellido, m.nombre;
END$$

-- -------------------------
-- PACIENTES (combo / filtro)
-- -------------------------
CREATE PROCEDURE usp_pacientes_listar(IN p_filtro VARCHAR(100))
BEGIN
  SELECT
    p.id_paciente AS IdPaciente,
    CONCAT(p.nombre,' ',p.apellido) AS Nombre,
    p.dni AS Dni,
    IFNULL(p.telefono,'') AS Telefono,
    p.activo AS Activo
  FROM pacientes p
  WHERE p.activo = 1
    AND (
      p_filtro IS NULL OR p_filtro = ''
      OR p.nombre LIKE CONCAT('%',p_filtro,'%')
      OR p.apellido LIKE CONCAT('%',p_filtro,'%')
      OR p.dni LIKE CONCAT('%',p_filtro,'%')
    )
  ORDER BY p.apellido, p.nombre;
END$$

CREATE PROCEDURE usp_pacientes_listar_sin_filtro()
BEGIN
  CALL usp_pacientes_listar('');
END$$

-- -------------------------
-- HORARIOS DISPONIBLES (MySQL 8+)
-- -------------------------
CREATE PROCEDURE usp_citas_horarios_disponibles(
  IN p_id_medico INT,
  IN p_fecha DATE
)
BEGIN
  WITH RECURSIVE slots AS (
    SELECT CAST('08:00:00' AS TIME) AS hora
    UNION ALL
    SELECT ADDTIME(hora, '00:30:00')
    FROM slots
    WHERE hora < '17:30:00'
  )
  SELECT DATE_FORMAT(s.hora, '%H:%i') AS hora
  FROM slots s
  WHERE NOT EXISTS (
    SELECT 1
    FROM citas c
    WHERE c.id_medico = p_id_medico
      AND c.fecha = p_fecha
      AND c.hora_inicio = s.hora
      AND c.id_estado_cita IN (
        SELECT id_estado_cita
        FROM estados_cita
        WHERE nombre IN ('PENDIENTE','ATENDIDA')
      )
  )
  ORDER BY s.hora;
END$$

-- -------------------------
-- RESERVAR (por paciente)
-- devuelve: 1 ok, -1 ocupado, -2 inválida
-- -------------------------
CREATE PROCEDURE usp_citas_reservar(
  IN p_id_paciente INT,
  IN p_id_medico INT,
  IN p_fecha DATE,
  IN p_hora_inicio TIME,
  IN p_motivo VARCHAR(300)
)
BEGIN
  IF p_hora_inicio < '08:00:00' OR p_hora_inicio > '17:30:00' THEN
    SELECT -2 AS resultado;
  ELSEIF (MINUTE(p_hora_inicio) NOT IN (0,30)) THEN
    SELECT -2 AS resultado;
  ELSE
    IF EXISTS (
      SELECT 1
      FROM citas
      WHERE id_medico = p_id_medico
        AND fecha = p_fecha
        AND hora_inicio = p_hora_inicio
        AND id_estado_cita IN (
          SELECT id_estado_cita
          FROM estados_cita
          WHERE nombre IN ('PENDIENTE','ATENDIDA')
        )
    ) THEN
      SELECT -1 AS resultado;
    ELSE
      INSERT INTO citas (id_paciente, id_medico, fecha, hora_inicio, motivo, precio, id_estado_cita)
      VALUES (
        p_id_paciente, p_id_medico, p_fecha, p_hora_inicio, p_motivo, 0.00,
        (SELECT id_estado_cita FROM estados_cita WHERE nombre='PENDIENTE' LIMIT 1)
      );
      SELECT 1 AS resultado;
    END IF;
  END IF;
END$$

-- -------------------------
-- LISTAR CITAS (Index)
-- -------------------------
CREATE PROCEDURE usp_citas_listar()
BEGIN
  SELECT
    c.id_cita,
    c.id_paciente,
    CONCAT(p.nombre,' ',p.apellido) AS paciente,
    c.id_medico,
    CONCAT(m.nombre,' ',m.apellido) AS medico,
    c.fecha,
    c.hora_inicio,
    IFNULL(c.motivo,'') AS motivo,
    c.precio,
    ec.nombre AS estado
  FROM citas c
  INNER JOIN pacientes p ON c.id_paciente = p.id_paciente
  INNER JOIN medicos m ON c.id_medico = m.id_medico
  INNER JOIN estados_cita ec ON c.id_estado_cita = ec.id_estado_cita
  ORDER BY c.fecha DESC, c.hora_inicio DESC;
END$$

-- -------------------------
-- BUSCAR POR ID (Edit/Delete)
-- -------------------------
CREATE PROCEDURE usp_citas_buscar_por_id(IN p_id_cita INT)
BEGIN
  SELECT
    c.id_cita,
    c.id_paciente,
    c.id_medico,
    c.fecha,
    c.hora_inicio,
    IFNULL(c.motivo,'') AS motivo,
    c.precio,
    ec.nombre AS estado
  FROM citas c
  INNER JOIN estados_cita ec ON ec.id_estado_cita = c.id_estado_cita
  WHERE c.id_cita = p_id_cita
  LIMIT 1;
END$$

-- -------------------------
-- CREAR CITA (Create POST)
-- -------------------------
CREATE PROCEDURE usp_citas_crear(
  IN p_id_paciente INT,
  IN p_id_medico INT,
  IN p_fecha DATE,
  IN p_hora_inicio TIME,
  IN p_motivo VARCHAR(300),
  IN p_precio DECIMAL(10,2),
  IN p_estado VARCHAR(30)
)
BEGIN
  DECLARE v_estado_id INT;

  SELECT id_estado_cita INTO v_estado_id
  FROM estados_cita
  WHERE nombre = p_estado
  LIMIT 1;

  IF v_estado_id IS NULL THEN
    SELECT id_estado_cita INTO v_estado_id
    FROM estados_cita
    WHERE nombre = 'PENDIENTE'
    LIMIT 1;
  END IF;

  INSERT INTO citas(id_paciente, id_medico, fecha, hora_inicio, motivo, precio, id_estado_cita)
  VALUES(p_id_paciente, p_id_medico, p_fecha, p_hora_inicio, p_motivo, p_precio, v_estado_id);
END$$

-- -------------------------
-- ACTUALIZAR CITA (Edit POST)
-- -------------------------
CREATE PROCEDURE usp_citas_actualizar(
  IN p_id_cita INT,
  IN p_id_paciente INT,
  IN p_id_medico INT,
  IN p_fecha DATE,
  IN p_hora_inicio TIME,
  IN p_motivo VARCHAR(300),
  IN p_precio DECIMAL(10,2),
  IN p_estado VARCHAR(30)
)
BEGIN
  DECLARE v_estado_id INT;

  SELECT id_estado_cita INTO v_estado_id
  FROM estados_cita
  WHERE nombre = p_estado
  LIMIT 1;

  IF v_estado_id IS NULL THEN
    SELECT id_estado_cita INTO v_estado_id
    FROM estados_cita
    WHERE nombre = 'PENDIENTE'
    LIMIT 1;
  END IF;

  UPDATE citas
  SET
    id_paciente = p_id_paciente,
    id_medico = p_id_medico,
    fecha = p_fecha,
    hora_inicio = p_hora_inicio,
    motivo = p_motivo,
    precio = p_precio,
    id_estado_cita = v_estado_id
  WHERE id_cita = p_id_cita;
END$$

-- -------------------------
-- ELIMINAR CITA (DeleteConfirmed)
-- -------------------------
CREATE PROCEDURE usp_citas_eliminar(IN p_id_cita INT)
BEGIN
  DELETE FROM citas WHERE id_cita = p_id_cita;
END$$

DELIMITER ;

-- =========================================================
-- 7) PRUEBAS RAPIDAS (OPCIONAL)
-- =========================================================
CALL usp_usuarios_login('admin','admin123');
CALL usp_medicos_listar();
CALL usp_pacientes_listar_sin_filtro();
CALL usp_citas_buscar_por_id(1);
CALL usp_citas_listar();
