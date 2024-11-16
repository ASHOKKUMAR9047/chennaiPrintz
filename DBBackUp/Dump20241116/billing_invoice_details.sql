CREATE DATABASE  IF NOT EXISTS `billing` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `billing`;
-- MySQL dump 10.13  Distrib 8.0.38, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: billing
-- ------------------------------------------------------
-- Server version	8.0.39

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `invoice_details`
--

DROP TABLE IF EXISTS `invoice_details`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `invoice_details` (
  `InvoiceId` varchar(255) NOT NULL,
  `CustomerId` int NOT NULL,
  `Notes` text,
  `Invoicedate` date NOT NULL,
  `DeliveryCharge` decimal(10,2) NOT NULL,
  `GstAmount` decimal(10,2) NOT NULL,
  `GrandTotal` decimal(10,2) NOT NULL,
  `CreatedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  `PaymentStatus` varchar(255) NOT NULL,
  `InvoiceOutStandingAmount` decimal(10,2) NOT NULL,
  `PaymentMode` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`InvoiceId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `invoice_details`
--

LOCK TABLES `invoice_details` WRITE;
/*!40000 ALTER TABLE `invoice_details` DISABLE KEYS */;
INSERT INTO `invoice_details` VALUES ('CP_20241025185010372',7,NULL,'2024-10-25',0.00,0.00,0.00,'2024-10-25 18:50:39','',0.00,NULL),('CP_20241025185930111',2,NULL,'2024-10-25',100.00,50.00,300.00,'2024-10-25 19:00:05','Paid',0.00,NULL),('CP_20241025190151631',8,NULL,'2024-10-25',100.00,50.00,50.00,'2024-10-25 19:03:05','',0.00,NULL),('CP_20241028114430839',3,'This Product ','2024-10-28',100.00,50.00,0.00,'2024-10-28 11:46:45','',0.00,NULL),('CP_20241028194713026',3,NULL,'2024-10-28',100.00,50.00,150.00,'2024-10-28 19:50:26','',0.00,NULL),('CP_20241106112901968',2,NULL,'2024-11-06',100.00,0.00,300.00,'2024-11-06 11:33:02','PartiallyPaid',50.00,NULL),('CP_20241106115651438',4,NULL,'2024-11-06',100.00,0.00,8500.00,'2024-11-06 11:58:06','UnPaid',8500.00,NULL),('CP_20241106122014056',4,NULL,'2024-11-06',100.00,0.00,4000.00,'2024-11-06 12:21:58','UnPaid',8500.00,NULL),('CP_20241106122706923',9,NULL,'2024-11-06',100.00,0.00,8500.00,'2024-11-06 12:32:05','',0.00,NULL),('CP_20241106124757454',3,NULL,'2024-11-06',100.00,0.00,8500.00,'2024-11-06 12:48:48','',0.00,NULL),('CP_20241106131146675',9,NULL,'2024-11-06',100.00,0.00,8500.00,'2024-11-06 13:12:49','',0.00,NULL),('CP_20241106132801828',9,NULL,'2024-11-06',100.00,0.00,8500.00,'2024-11-06 13:29:36','',0.00,NULL),('CP_20241106143521341',4,NULL,'2024-11-06',200.00,0.00,30200.00,'2024-11-06 14:37:23','UnPaid',8500.00,NULL),('CP_20241108110746058',4,'THIS IS MY BEST PRODUCT','2024-11-08',100.00,0.00,14100.00,'2024-11-08 11:09:13','UnPaid',8500.00,NULL),('CP_20241108111334940',3,NULL,'2024-11-08',200.00,0.00,4800.00,'2024-11-08 11:14:10','',0.00,NULL),('CP_20241111144946397',7,NULL,'2024-11-11',100.00,0.00,8600.00,'2024-11-11 14:50:50','',0.00,NULL),('CP_20241111145406895',3,NULL,'2024-11-11',100.00,0.00,4100.00,'2024-11-11 14:54:28','',0.00,NULL),('CP_20241116102414054',4,NULL,'2024-11-16',200.00,0.00,2300.00,'2024-11-16 10:25:46','Paid',0.00,'Cash'),('CP_20241116132225620',4,NULL,'2024-11-16',50.00,0.00,1650.00,'2024-11-16 13:24:35','UnPaid',1650.00,NULL);
/*!40000 ALTER TABLE `invoice_details` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-11-16 13:59:15
