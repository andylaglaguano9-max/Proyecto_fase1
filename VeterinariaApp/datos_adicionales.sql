-- ============================================================
-- VetCare - Script de datos adicionales de prueba
-- Ejecutar en pgAdmin 4 > Query Tool > VeterinariaDB
-- ============================================================

-- DUEÑOS ADICIONALES (30 registros)
INSERT INTO "Duenos" ("Id","Activo","Apellido","Correo","Direccion","FechaCreacion","Nombre","Telefono") VALUES
(6,  TRUE, 'Mora',      'pablo.mora@gmail.com',     'Av. Colón 456, Quito',          NOW(), 'Pablo',    '0991234560'),
(7,  TRUE, 'Vega',      'sofia.vega@gmail.com',     'Calle Sucre 789, Guayaquil',    NOW(), 'Sofía',    '0987654320'),
(8,  TRUE, 'Castro',    'andrés.castro@gmail.com',  'Jr. Bolívar 321, Cuenca',       NOW(), 'Andrés',   '0976543210'),
(9,  TRUE, 'Flores',    'valentina.flores@mail.com','Av. 6 de Diciembre, Quito',      NOW(), 'Valentina','0965432100'),
(10, TRUE, 'Ríos',      'diego.rios@mail.com',      'Calle Chile 111, Ambato',       NOW(), 'Diego',    '0954321000'),
(11, TRUE, 'Salazar',   'isabel.salazar@mail.com',  'Av. Amazonas 222, Quito',       NOW(), 'Isabel',   '0943210000'),
(12, TRUE, 'Mendoza',   'jorge.mendoza@mail.com',   'Calle Rocafuerte 333, Loja',    NOW(), 'Jorge',    '0932100000'),
(13, TRUE, 'Guerrero',  'camila.guerrero@mail.com', 'Av. 10 de Agosto 444, Quito',   NOW(), 'Camila',   '0921000000'),
(14, TRUE, 'Cárdenas',  'mateo.cardenas@mail.com',  'Calle García Moreno 555, Ibarra',NOW(),'Mateo',   '0910000001'),
(15, TRUE, 'Ponce',     'lucia.ponce@mail.com',     'Av. Patria 666, Quito',         NOW(), 'Lucía',    '0900000012'),
(16, TRUE, 'Alvarado',  'roberto.alvarado@mail.com','Calle Espejo 777, Riobamba',    NOW(), 'Roberto',  '0891234567'),
(17, TRUE, 'Ochoa',     'daniela.ochoa@mail.com',   'Av. República 888, Quito',      NOW(), 'Daniela',  '0881234567'),
(18, TRUE, 'Benítez',   'miguel.benitez@mail.com',  'Calle Flores 999, Santo Domingo',NOW(),'Miguel',  '0871234567'),
(19, TRUE, 'Vargas',    'mariana.vargas@mail.com',  'Av. Universitaria 100, Quito',  NOW(), 'Mariana',  '0861234567'),
(20, TRUE, 'Herrera',   'felipe.herrera@mail.com',  'Calle Caldas 200, Machala',     NOW(), 'Felipe',   '0851234567'),
(21, TRUE, 'Naranjo',   'natalia.naranjo@mail.com', 'Av. Granados 300, Quito',       NOW(), 'Natalia',  '0841234567'),
(22, TRUE, 'Cevallos',  'christian.cevallos@mail.com','Calle Guayaquil 400, Ibarra', NOW(), 'Christian','0831234567'),
(23, TRUE, 'Espinoza',  'andrea.espinoza@mail.com', 'Av. Naciones Unidas 500, Quito',NOW(), 'Andrea',  '0821234567'),
(24, TRUE, 'Jaramillo', 'xavier.jaramillo@mail.com','Calle Velasco 600, Riobamba',   NOW(), 'Xavier',   '0811234567'),
(25, TRUE, 'Proaño',    'paola.proano@mail.com',    'Av. Eloy Alfaro 700, Quito',    NOW(), 'Paola',    '0801234567'),
(26, TRUE, 'Suárez',    'esteban.suarez@mail.com',  'Calle Montúfar 800, Latacunga', NOW(), 'Esteban',  '0791234567'),
(27, TRUE, 'Romero',    'karina.romero@mail.com',   'Av. América 900, Quito',        NOW(), 'Karina',   '0781234567'),
(28, TRUE, 'Delgado',   'ivan.delgado@mail.com',    'Calle Sucre 101, Tulcán',       NOW(), 'Iván',     '0771234567'),
(29, TRUE, 'Aguirre',   'carolina.aguirre@mail.com','Av. De la Prensa 202, Quito',   NOW(), 'Carolina', '0761234567'),
(30, TRUE, 'Molina',    'david.molina@mail.com',    'Calle Chimborazo 303, Ambato',  NOW(), 'David',    '0751234567'),
(31, TRUE, 'León',      'emily.leon@mail.com',      'Av. Mariscal Sucre 404, Cuenca',NOW(), 'Emily',    '0741234567'),
(32, TRUE, 'Cruz',      'alex.cruz@mail.com',       'Calle Tarqui 505, Manta',       NOW(), 'Alex',     '0731234567'),
(33, TRUE, 'Peñafiel',  'veronica.penafiel@mail.com','Av. Maldonado 606, Quito',     NOW(), 'Verónica', '0721234567'),
(34, TRUE, 'Ibarra',    'bryan.ibarra@mail.com',    'Calle Reina Victoria 707, Quito',NOW(),'Bryan',   '0711234567'),
(35, TRUE, 'Carrillo',  'tania.carrillo@mail.com',  'Av. González Suárez 808, Quito',NOW(), 'Tania',    '0701234567');

-- MASCOTAS ADICIONALES
INSERT INTO "Mascotas" ("Id","Activo","DuenoId","EspecieId","FechaCreacion","FechaNacimiento","Nombre","Peso") VALUES
(6,  TRUE, 6,  1, NOW(), '2021-03-15 00:00:00Z', 'Toby',     18.0),
(7,  TRUE, 7,  2, NOW(), '2020-07-20 00:00:00Z', 'Luna',     4.5),
(8,  TRUE, 8,  1, NOW(), '2019-11-05 00:00:00Z', 'Bruno',    22.3),
(9,  TRUE, 9,  2, NOW(), '2022-01-10 00:00:00Z', 'Nala',     3.8),
(10, TRUE, 10, 3, NOW(), '2021-06-25 00:00:00Z', 'Kiwi',     0.4),
(11, TRUE, 11, 1, NOW(), '2018-09-14 00:00:00Z', 'Max',      30.1),
(12, TRUE, 12, 4, NOW(), '2023-04-01 00:00:00Z', 'Nugget',   0.2),
(13, TRUE, 13, 2, NOW(), '2020-12-20 00:00:00Z', 'Bella',    5.1),
(14, TRUE, 14, 5, NOW(), '2017-08-08 00:00:00Z', 'Drago',    3.5),
(15, TRUE, 15, 1, NOW(), '2021-02-14 00:00:00Z', 'Rocky',    26.7),
(16, TRUE, 16, 2, NOW(), '2022-09-30 00:00:00Z', 'Cleo',     4.0),
(17, TRUE, 17, 1, NOW(), '2020-05-05 00:00:00Z', 'Thor',     35.0),
(18, TRUE, 18, 3, NOW(), '2023-01-18 00:00:00Z', 'Tweety',   0.3),
(19, TRUE, 19, 4, NOW(), '2022-11-11 00:00:00Z', 'Coco',     0.25),
(20, TRUE, 20, 1, NOW(), '2019-04-22 00:00:00Z', 'Duke',     28.5),
(21, TRUE, 21, 2, NOW(), '2021-08-03 00:00:00Z', 'Mia',      3.9),
(22, TRUE, 22, 1, NOW(), '2020-10-10 00:00:00Z', 'Zeus',     40.2),
(23, TRUE, 23, 2, NOW(), '2022-03-27 00:00:00Z', 'Kitty',    4.3),
(24, TRUE, 24, 5, NOW(), '2018-07-15 00:00:00Z', 'Iggy',     2.8),
(25, TRUE, 25, 1, NOW(), '2021-12-01 00:00:00Z', 'Simba',    20.0),
(26, TRUE, 26, 3, NOW(), '2023-05-05 00:00:00Z', 'Loro',     0.45),
(27, TRUE, 27, 2, NOW(), '2020-02-28 00:00:00Z', 'Misty',    3.7),
(28, TRUE, 28, 1, NOW(), '2019-06-17 00:00:00Z', 'Rex 2',    15.8),
(29, TRUE, 29, 4, NOW(), '2022-08-19 00:00:00Z', 'Hammy',    0.28),
(30, TRUE, 30, 1, NOW(), '2021-09-09 00:00:00Z', 'Bobby',    18.9),
(31, TRUE, 31, 2, NOW(), '2020-04-04 00:00:00Z', 'Salem',    5.5),
(32, TRUE, 32, 1, NOW(), '2018-12-25 00:00:00Z', 'Pancho',   12.0),
(33, TRUE, 33, 3, NOW(), '2023-02-14 00:00:00Z', 'Periquito', 0.35),
(34, TRUE, 34, 2, NOW(), '2021-11-11 00:00:00Z', 'Garfield', 6.2),
(35, TRUE, 35, 1, NOW(), '2020-07-07 00:00:00Z', 'Buddy',    24.1);

-- Actualizar secuencias de IDs
SELECT setval(pg_get_serial_sequence('"Duenos"', 'Id'), (SELECT MAX("Id") FROM "Duenos") + 1, false);
SELECT setval(pg_get_serial_sequence('"Mascotas"', 'Id'), (SELECT MAX("Id") FROM "Mascotas") + 1, false);
