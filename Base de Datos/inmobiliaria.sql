-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 14-10-2025 a las 20:20:43
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmobiliaria`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contrato`
--

CREATE TABLE `contrato` (
  `IdContrato` int(11) NOT NULL,
  `FechaInicio` date NOT NULL,
  `FechaFin` date NOT NULL,
  `MontoMensual` decimal(10,0) NOT NULL,
  `IdInmueble` int(11) NOT NULL,
  `IdInquilino` int(11) NOT NULL,
  `QuienCreo` int(11) DEFAULT NULL,
  `QuienElimino` int(11) DEFAULT NULL,
  `Estado` tinyint(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contrato`
--

INSERT INTO `contrato` (`IdContrato`, `FechaInicio`, `FechaFin`, `MontoMensual`, `IdInmueble`, `IdInquilino`, `QuienCreo`, `QuienElimino`, `Estado`) VALUES
(1, '2025-10-07', '2025-10-08', 1000, 1, 1, 1, NULL, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `imagen`
--

CREATE TABLE `imagen` (
  `IdImagen` int(11) NOT NULL,
  `IdInmueble` int(11) NOT NULL,
  `Url` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `imagen`
--

INSERT INTO `imagen` (`IdImagen`, `IdInmueble`, `Url`) VALUES
(1, 1, '/Uploads/Inmuebles/1/9fd4f5cf-6f0d-4360-8a1b-9fd7237def62.jpeg');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmueble`
--

CREATE TABLE `inmueble` (
  `IdInmueble` int(11) NOT NULL,
  `Direccion` varchar(100) NOT NULL,
  `IdTipo` int(11) NOT NULL,
  `IdPropietario` int(11) NOT NULL,
  `Uso` enum('Comercial','Residencial') NOT NULL,
  `Latitud` decimal(10,0) NOT NULL,
  `Longitud` decimal(10,0) NOT NULL,
  `Precio` decimal(10,2) NOT NULL,
  `Ambiente` int(11) NOT NULL,
  `PortadaUrl` varchar(100) NOT NULL,
  `Estado` tinyint(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmueble`
--

INSERT INTO `inmueble` (`IdInmueble`, `Direccion`, `IdTipo`, `IdPropietario`, `Uso`, `Latitud`, `Longitud`, `Precio`, `Ambiente`, `PortadaUrl`, `Estado`) VALUES
(1, 'Calle falsa 123', 1, 1, 'Comercial', 4140338, 217403, 1000.00, 1, '/Uploads/Inmuebles/portada_1.jpg', 1),
(2, 'Calle San Martin 234', 1, 1, 'Residencial', 41222, 43224, 150000.00, 3, '', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilino`
--

CREATE TABLE `inquilino` (
  `IdInquilino` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Dni` char(8) NOT NULL,
  `Telefono` varchar(30) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Estado` tinyint(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilino`
--

INSERT INTO `inquilino` (`IdInquilino`, `Nombre`, `Apellido`, `Dni`, `Telefono`, `Email`, `Estado`) VALUES
(1, 'Josias', 'Tolaba', '11111111', '02282406411', 'josiastolaba12@gmail.com', 1),
(2, 'Marcos Antonio', 'Sosa Chirino', '46807958', '02664553401', 'marcosasosa@sanluis.edu.ar', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pago`
--

CREATE TABLE `pago` (
  `IdPago` int(11) NOT NULL,
  `FechaPago` date NOT NULL,
  `Monto` decimal(10,0) NOT NULL,
  `Mes` date NOT NULL,
  `NumeroPago` varchar(50) NOT NULL,
  `Concepto` varchar(100) NOT NULL,
  `IdContrato` int(11) NOT NULL,
  `QuienCreo` int(11) DEFAULT NULL,
  `QuienElimino` int(11) DEFAULT NULL,
  `Estado` tinyint(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `pago`
--

INSERT INTO `pago` (`IdPago`, `FechaPago`, `Monto`, `Mes`, `NumeroPago`, `Concepto`, `IdContrato`, `QuienCreo`, `QuienElimino`, `Estado`) VALUES
(1, '2025-10-10', 900, '2025-10-10', '1', 'prueba', 1, 1, NULL, 1),
(2, '2025-10-17', 1200, '2025-10-17', '2', 'prueba', 1, 2, NULL, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietario`
--

CREATE TABLE `propietario` (
  `IdPropietario` int(11) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Dni` char(8) NOT NULL,
  `Telefono` varchar(50) NOT NULL,
  `Email` varchar(50) NOT NULL,
  `Estado` tinyint(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietario`
--

INSERT INTO `propietario` (`IdPropietario`, `Nombre`, `Apellido`, `Dni`, `Telefono`, `Email`, `Estado`) VALUES
(1, 'Josias', 'Tolaba', '11111111', '02282406411', 'josiastolaba12@gmail.com', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `reserva`
--

CREATE TABLE `reserva` (
  `IdReserva` int(11) NOT NULL,
  `IdInmueble` int(11) NOT NULL,
  `IdContrato` int(11) NOT NULL,
  `FechaDesde` date NOT NULL,
  `FechaHasta` date NOT NULL,
  `Estado` tinyint(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `reserva`
--

INSERT INTO `reserva` (`IdReserva`, `IdInmueble`, `IdContrato`, `FechaDesde`, `FechaHasta`, `Estado`) VALUES
(1, 1, 1, '2025-10-07', '2025-10-08', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tipo_inmueble`
--

CREATE TABLE `tipo_inmueble` (
  `IdTipo` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Estado` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tipo_inmueble`
--

INSERT INTO `tipo_inmueble` (`IdTipo`, `Nombre`, `Estado`) VALUES
(1, 'Casa', 1),
(2, 'Local', 1),
(3, 'Salon', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuario`
--

CREATE TABLE `usuario` (
  `IdUsuario` int(11) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Contrasena` varchar(255) NOT NULL,
  `Rol` enum('Administrador','Empleado') NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Apellido` varchar(100) NOT NULL,
  `Dni` char(8) NOT NULL,
  `Estado` tinyint(4) NOT NULL,
  `Avatar` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuario`
--

INSERT INTO `usuario` (`IdUsuario`, `Email`, `Contrasena`, `Rol`, `Nombre`, `Apellido`, `Dni`, `Estado`, `Avatar`) VALUES
(1, 'josiastolaba12@gmail.com', '/D5kC6DBbvyRW3eIzT4HAEq0LvWdQ3T/MqqGnaP/l4U=', 'Administrador', 'Josias', 'Tolaba', '11111111', 1, ''),
(2, 'abeltolaba@gmail.com', '/D5kC6DBbvyRW3eIzT4HAEq0LvWdQ3T/MqqGnaP/l4U=', 'Empleado', 'Abel', 'Tolaba', '22222222', 1, ''),
(3, 'admin@local.test', 'rZD4tWeiA20YC5IG+4eYvB7t4AFrToiHejlPWvAHOAo=', 'Administrador', 'Admin', 'Local', '00000000', 1, '/imagenes/usuarios/default-avatar.png'),
(4, 'marcosasosa@gmail.com', 'ARUH36sJbhMWWhuCvJNqJ631oybN/2yNsq1l8lr1uh8=', 'Administrador', 'Marcos Antonio', 'Sosa Chirino', '11111112', 1, '/imagenes/usuarios/default-avatar.png'),
(5, 'Empleado@gmail.com', 'viNlp0mz1xbzK0kAWG09Qs1CZvPItigPA47naMqKhHU=', 'Empleado', 'Empleado', 'Uno', '10000009', 1, '/imagenes/usuarios/default-avatar.png');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `contrato`
--
ALTER TABLE `contrato`
  ADD PRIMARY KEY (`IdContrato`),
  ADD KEY `id_inmueble` (`IdInmueble`),
  ADD KEY `id_inquilino` (`IdInquilino`),
  ADD KEY `QuienCreo` (`QuienCreo`),
  ADD KEY `QuienElimino` (`QuienElimino`);

--
-- Indices de la tabla `imagen`
--
ALTER TABLE `imagen`
  ADD PRIMARY KEY (`IdImagen`),
  ADD KEY `IdInmueble` (`IdInmueble`);

--
-- Indices de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD PRIMARY KEY (`IdInmueble`),
  ADD KEY `id_propietario` (`IdPropietario`),
  ADD KEY `IdTipo` (`IdTipo`);

--
-- Indices de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  ADD PRIMARY KEY (`IdInquilino`);

--
-- Indices de la tabla `pago`
--
ALTER TABLE `pago`
  ADD PRIMARY KEY (`IdPago`),
  ADD KEY `id_contrato` (`IdContrato`),
  ADD KEY `QuienCreo` (`QuienCreo`),
  ADD KEY `QuienElimino` (`QuienElimino`);

--
-- Indices de la tabla `propietario`
--
ALTER TABLE `propietario`
  ADD PRIMARY KEY (`IdPropietario`);

--
-- Indices de la tabla `reserva`
--
ALTER TABLE `reserva`
  ADD PRIMARY KEY (`IdReserva`),
  ADD KEY `IdContrato` (`IdContrato`),
  ADD KEY `IdInmueble` (`IdInmueble`);

--
-- Indices de la tabla `tipo_inmueble`
--
ALTER TABLE `tipo_inmueble`
  ADD PRIMARY KEY (`IdTipo`);

--
-- Indices de la tabla `usuario`
--
ALTER TABLE `usuario`
  ADD PRIMARY KEY (`IdUsuario`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `contrato`
--
ALTER TABLE `contrato`
  MODIFY `IdContrato` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de la tabla `imagen`
--
ALTER TABLE `imagen`
  MODIFY `IdImagen` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  MODIFY `IdInmueble` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  MODIFY `IdInquilino` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `pago`
--
ALTER TABLE `pago`
  MODIFY `IdPago` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `propietario`
--
ALTER TABLE `propietario`
  MODIFY `IdPropietario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de la tabla `reserva`
--
ALTER TABLE `reserva`
  MODIFY `IdReserva` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de la tabla `tipo_inmueble`
--
ALTER TABLE `tipo_inmueble`
  MODIFY `IdTipo` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `usuario`
--
ALTER TABLE `usuario`
  MODIFY `IdUsuario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contrato`
--
ALTER TABLE `contrato`
  ADD CONSTRAINT `contrato_ibfk_1` FOREIGN KEY (`IdInmueble`) REFERENCES `inmueble` (`IdInmueble`),
  ADD CONSTRAINT `contrato_ibfk_2` FOREIGN KEY (`IdInquilino`) REFERENCES `inquilino` (`IdInquilino`),
  ADD CONSTRAINT `contrato_ibfk_3` FOREIGN KEY (`QuienCreo`) REFERENCES `usuario` (`IdUsuario`),
  ADD CONSTRAINT `contrato_ibfk_4` FOREIGN KEY (`QuienElimino`) REFERENCES `usuario` (`IdUsuario`);

--
-- Filtros para la tabla `imagen`
--
ALTER TABLE `imagen`
  ADD CONSTRAINT `imagen_ibfk_1` FOREIGN KEY (`IdInmueble`) REFERENCES `inmueble` (`IdInmueble`);

--
-- Filtros para la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD CONSTRAINT `inmueble_ibfk_1` FOREIGN KEY (`IdPropietario`) REFERENCES `propietario` (`IdPropietario`),
  ADD CONSTRAINT `inmueble_ibfk_2` FOREIGN KEY (`IdTipo`) REFERENCES `tipo_inmueble` (`IdTipo`);

--
-- Filtros para la tabla `pago`
--
ALTER TABLE `pago`
  ADD CONSTRAINT `pago_ibfk_1` FOREIGN KEY (`IdContrato`) REFERENCES `contrato` (`IdContrato`),
  ADD CONSTRAINT `pago_ibfk_2` FOREIGN KEY (`QuienCreo`) REFERENCES `usuario` (`IdUsuario`),
  ADD CONSTRAINT `pago_ibfk_3` FOREIGN KEY (`QuienElimino`) REFERENCES `usuario` (`IdUsuario`);

--
-- Filtros para la tabla `reserva`
--
ALTER TABLE `reserva`
  ADD CONSTRAINT `reserva_ibfk_1` FOREIGN KEY (`IdContrato`) REFERENCES `contrato` (`IdContrato`),
  ADD CONSTRAINT `reserva_ibfk_2` FOREIGN KEY (`IdInmueble`) REFERENCES `inmueble` (`IdInmueble`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
