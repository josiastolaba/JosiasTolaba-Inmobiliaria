-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 11-09-2025 a las 01:42:06
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
(1, '2025-09-10', '2025-09-15', 1000, 2, 1, NULL, NULL, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmueble`
--

CREATE TABLE `inmueble` (
  `IdInmueble` int(11) NOT NULL,
  `Direccion` varchar(100) NOT NULL,
  `Tipo` varchar(100) NOT NULL,
  `IdPropietario` int(11) NOT NULL,
  `Uso` enum('Comercial','Residencial') NOT NULL,
  `Latitud` decimal(10,0) NOT NULL,
  `Longitud` decimal(10,0) NOT NULL,
  `Precio` decimal(10,2) NOT NULL,
  `Ambiente` int(11) NOT NULL,
  `Estado` tinyint(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmueble`
--

INSERT INTO `inmueble` (`IdInmueble`, `Direccion`, `Tipo`, `IdPropietario`, `Uso`, `Latitud`, `Longitud`, `Precio`, `Ambiente`, `Estado`) VALUES
(2, 'Calle falsa 123', 'Casa ', 1, 'Comercial', 4140338, 217403, 1234567.89, 1, 1);

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
(1, 'Josias Abel', 'Tolaba', '11111111', '2664000000', 'josiastolaba@gmail.com', 1),
(2, 'Marcos', 'Sosa', '22222222', '2664000001', 'marcossosa@gmail.com', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pago`
--

CREATE TABLE `pago` (
  `IdPago` int(11) NOT NULL,
  `FechaPago` date NOT NULL,
  `Monto` decimal(10,0) NOT NULL,
  `MesCorrespondiente` char(7) NOT NULL,
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

INSERT INTO `pago` (`IdPago`, `FechaPago`, `Monto`, `MesCorrespondiente`, `NumeroPago`, `Concepto`, `IdContrato`, `QuienCreo`, `QuienElimino`, `Estado`) VALUES
(14, '2025-09-10', 10001, '1', '1', 'Alquiler mes septiembre', 1, NULL, NULL, 1),
(15, '2025-09-08', 1, '1', '1', '1', 1, NULL, NULL, 1),
(16, '2025-09-08', 1, '1', '1', '1', 1, NULL, NULL, 1);

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
(1, 'Josias', 'Tolaba', '11111111', '2664000000', 'josiastolaba@gmail.com', 1),
(2, 'Marcos', 'Sosa', '22222222', '2664000001', 'marcossosa@gmail.com', 1),
(3, 'Pablo', 'Alcaraz', '33333333', '2664000002', 'pabloalcaraz@gmail.com', 1);

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
  `Estado` tinyint(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuario`
--

INSERT INTO `usuario` (`IdUsuario`, `Email`, `Contrasena`, `Rol`, `Nombre`, `Apellido`, `Dni`, `Estado`) VALUES
(1, 'josiastolaba@gmail.com', 'joasiast74', 'Administrador', 'Josias', 'Tolaba', '11111111', 1);

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
-- Indices de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD PRIMARY KEY (`IdInmueble`),
  ADD KEY `id_propietario` (`IdPropietario`);

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
  MODIFY `IdPago` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- AUTO_INCREMENT de la tabla `propietario`
--
ALTER TABLE `propietario`
  MODIFY `IdPropietario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `usuario`
--
ALTER TABLE `usuario`
  MODIFY `IdUsuario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

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
-- Filtros para la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD CONSTRAINT `inmueble_ibfk_1` FOREIGN KEY (`IdPropietario`) REFERENCES `propietario` (`IdPropietario`);

--
-- Filtros para la tabla `pago`
--
ALTER TABLE `pago`
  ADD CONSTRAINT `pago_ibfk_1` FOREIGN KEY (`IdContrato`) REFERENCES `contrato` (`IdContrato`),
  ADD CONSTRAINT `pago_ibfk_2` FOREIGN KEY (`QuienCreo`) REFERENCES `usuario` (`IdUsuario`),
  ADD CONSTRAINT `pago_ibfk_3` FOREIGN KEY (`QuienElimino`) REFERENCES `usuario` (`IdUsuario`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
