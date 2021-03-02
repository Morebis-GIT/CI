-- MySQL dump 10.13  Distrib 5.6.49, for Win64 (x86_64)
--
-- Host: localhost    Database: xggptenant
-- ------------------------------------------------------
-- Server version	5.6.49-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Dumping data for table `outputfiles`
--

LOCK TABLES `outputfiles` WRITE;
/*!40000 ALTER TABLE `outputfiles` DISABLE KEYS */;
INSERT INTO `outputfiles` VALUES (1,'LMKII_BKPO_REQM.out','Campaign, Booking Position','LMKII_BKPO_REQM.out'),(2,'BASE_RTGS.out','Base Ratings','BASE_RTGS.out'),(3,'EFFE.out','Break Efficiency','EFFE.out'),(4,'SPSP.out','\"Link between Top and Member spots within a multipart','SPSP.out'),(5,'SPRG.out','Regional Booking. For non-regional systems is a one to one match with the SPOT data.','SPRG.out'),(6,'SPOT.out','Main Spot table, one row per spot in the system','SPOT.out'),(7,'SDIS.out','One row per discount applied to Spot, optional data, generally omitted for most clients/bookings','SDIS.out'),(8,'LMKII_SSLG_REQM.out','Campaign, Sales Area, Spot Length','LMKII_SSLG_REQM.out'),(9,'LMKII_SPOT_REQM.out','Spot','LMKII_SPOT_REQM.out'),(10,'LMKII_SCEN_CAMP_REQM_SUMM.out','Scenario Campaign Results','LMKII_SCEN_CAMP_REQM_SUMM.out'),(11,'LMKII_SARE_REQM.out','Campaign, Sales Area','LMKII_SARE_REQM.out'),(12,'LMKII_PART_REQM.out','Campaign, Sales Area, Strike Weight, Daypart','LMKII_PART_REQM.out'),(13,'LMKII_FAIL_REPT.out','Failures','LMKII_FAIL_REPT.out'),(14,'LMKII_DDPD_SSLG_REQM.out','Campaign, Sales Area, Strike Weight, Spot Length','LMKII_DDPD_SSLG_REQM.out'),(15,'LMKII_DDPD_REQM.out','Campaign, Sales Area, Strike Weight','LMKII_DDPD_REQM.out'),(16,'LMKII_DCDP_SSLG_REQM.out','Campaign, Sales Area, Daypart, Spot Length','LMKII_DCDP_SSLG_REQM.out'),(17,'LMKII_DCDP_REQM.out','Campaign, Sales Area, Daypart','LMKII_DCDP_REQM.out'),(18,'LMKII_CEND_REQM.out','Campaign, Position in Programme','LMKII_CEND_REQM.out'),(19,'LMKII_CAMP_REQM.out','Campaign level','LMKII_CAMP_REQM.out'),(20,'LMKII_PART_SSLG_REQM.out','Campaign, Sales Area, Strike Weight, Daypart, Spot Length','LMKII_PART_SSLG_REQM.out'),(21,'XG_CAMP_FAIL_REPT.out','Scenario Campaign Failures','XG_CAMP_FAIL_REPT.out');
/*!40000 ALTER TABLE `outputfiles` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-09-28  9:40:13
