-- ORDEN: Modelos -> Coches -> Rental/Purchase/Review -> RentalItems/PurchaseItems/ReviewItems


-- MODELOS
SET IDENTITY_INSERT [dbo].[Models] ON
INSERT INTO [dbo].[Models] ([Id], [Name]) VALUES (1, N'Model S')
INSERT INTO [dbo].[Models] ([Id], [Name]) VALUES (2, N'R8')
INSERT INTO [dbo].[Models] ([Id], [Name]) VALUES (3, N'Mustang')
SET IDENTITY_INSERT [dbo].[Models] OFF

-- COCHES
SET IDENTITY_INSERT [dbo].[Cars] ON
INSERT INTO [dbo].[Cars] ([Id], [CarClass], [FuelType], [Color], [Description], [Manufacturer], [PurchasingPrice], [QuantityForPurchasing], [QuantityForRenting], [RentingPrice], [ModelId]) VALUES (1, N'Lujo', N'Eléctrico', N'Blanco', NULL, N'Tesla', CAST(50000.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), CAST(15.00 AS Decimal(18, 2)), CAST(45000.00 AS Decimal(18, 2)), 1)
INSERT INTO [dbo].[Cars] ([Id], [CarClass], [FuelType], [Color], [Description], [Manufacturer], [PurchasingPrice], [QuantityForPurchasing], [QuantityForRenting], [RentingPrice], [ModelId]) VALUES (2, N'Deportivo', N'Gasolina', N'Rojo', NULL, N'Audi', CAST(70000.00 AS Decimal(18, 2)), CAST(8.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), CAST(35000.00 AS Decimal(18, 2)), 2)
INSERT INTO [dbo].[Cars] ([Id], [CarClass], [FuelType], [Color], [Description], [Manufacturer], [PurchasingPrice], [QuantityForPurchasing], [QuantityForRenting], [RentingPrice], [ModelId]) VALUES (4, N'Potente', N'Diésel', N'Negro', NULL, N'Ford', CAST(35000.00 AS Decimal(18, 2)), CAST(12.00 AS Decimal(18, 2)), CAST(12.00 AS Decimal(18, 2)), CAST(28000.00 AS Decimal(18, 2)), 3)
SET IDENTITY_INSERT [dbo].[Cars] OFF



--PURCHASES
SET IDENTITY_INSERT [dbo].[Purchases] ON
INSERT INTO [dbo].[Purchases] ([Id], [Name], [Surname], [DeliveryCarDealer], [PaymentMethod], [PurchasingDate], [PurchasingPrice], [ApplicationUserId]) VALUES (1, N'Javier ', N'Sanchez', N'Concesionrio PEPI', 0, N'2026-12-15 00:00:00', CAST(10000.00 AS Decimal(18, 2)), NULL)
INSERT INTO [dbo].[Purchases] ([Id], [Name], [Surname], [DeliveryCarDealer], [PaymentMethod], [PurchasingDate], [PurchasingPrice], [ApplicationUserId]) VALUES (2, N'Isabel ', N'Castejon ', N'Concesionrio JUAN', 0, N'2026-10-08 00:00:00', CAST(12000.00 AS Decimal(18, 2)), NULL)
INSERT INTO [dbo].[Purchases] ([Id], [Name], [Surname], [DeliveryCarDealer], [PaymentMethod], [PurchasingDate], [PurchasingPrice], [ApplicationUserId]) VALUES (3, N'Pedro ', N'Leon ', N'Concesionario PEPE', 1, N'2025-12-14 00:00:00', CAST(15000.00 AS Decimal(18, 2)), NULL)
SET IDENTITY_INSERT [dbo].[Purchases] OFF

-- RENTALS
SET IDENTITY_INSERT [dbo].[Rentals] ON
INSERT INTO [dbo].[Rentals] ([Id], [DeliveryCarDealer], [RentingPrice], [EndDate], [RentingDate], [StartDate], [PaymentMethod], [ApplicationUserId]) VALUES (1, N'Tesla Albacete', CAST(45000.00 AS Decimal(18, 2)), N'2026-12-30 00:00:00', N'2025-11-22 00:00:00', N'2026-01-01 00:00:00', 1, NULL)
INSERT INTO [dbo].[Rentals] ([Id], [DeliveryCarDealer], [RentingPrice], [EndDate], [RentingDate], [StartDate], [PaymentMethod], [ApplicationUserId]) VALUES (2, N'Coches Pepe', CAST(35000.00 AS Decimal(18, 2)), N'2026-05-10 00:00:00', N'2026-11-25 00:00:00', N'2026-03-24 00:00:00', 2, NULL)
INSERT INTO [dbo].[Rentals] ([Id], [DeliveryCarDealer], [RentingPrice], [EndDate], [RentingDate], [StartDate], [PaymentMethod], [ApplicationUserId]) VALUES (3, N'Ford Alicante', CAST(28000.00 AS Decimal(18, 2)), N'2026-05-11 00:00:00', N'2025-12-05 00:00:00', N'2025-12-17 00:00:00', 0, NULL)
SET IDENTITY_INSERT [dbo].[Rentals] OFF

--PURCHASE ITEMS
INSERT INTO [dbo].[PurchaseItems] ([PurchaseId], [CarId], [Quantity]) VALUES (1, 2, 1)
INSERT INTO [dbo].[PurchaseItems] ([PurchaseId], [CarId], [Quantity]) VALUES (2, 4, 1)
INSERT INTO [dbo].[PurchaseItems] ([PurchaseId], [CarId], [Quantity]) VALUES (3, 1, 1)

-- RENTAL ITEMS
INSERT INTO [dbo].[RentalItems] ([CarId], [RentalId], [Quantity]) VALUES (1, 1, 3)
INSERT INTO [dbo].[RentalItems] ([CarId], [RentalId], [Quantity]) VALUES (2, 2, 4)
INSERT INTO [dbo].[RentalItems] ([CarId], [RentalId], [Quantity]) VALUES (4, 3, 2)