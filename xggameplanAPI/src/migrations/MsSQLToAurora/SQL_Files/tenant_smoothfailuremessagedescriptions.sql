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
-- Dumping data for table `smoothfailuremessagedescriptions`
--

LOCK TABLES `smoothfailuremessagedescriptions` WRITE;
/*!40000 ALTER TABLE `smoothfailuremessagedescriptions` DISABLE KEYS */;
INSERT INTO `smoothfailuremessagedescriptions` VALUES (1,1,'ENG','Invalid break type'),(2,315,'ARA','Index restriction for clearance code'),(3,315,'ENG','Index restriction for clearance code'),(4,314,'ARA','Index restriction for copy'),(5,314,'ENG','Index restriction for copy'),(6,313,'ARA','Index restriction for product'),(7,313,'ENG','Index restriction for product'),(8,312,'ARA','Programme category restriction for clash'),(9,312,'ENG','Programme category restriction for clash'),(10,311,'ARA','Programme category restriction for clearance code'),(11,311,'ENG','Programme category restriction for clearance code'),(12,310,'ARA','Programme category restriction for copy'),(13,310,'ENG','Programme category restriction for copy'),(14,309,'ARA','Programme category restriction for product'),(15,309,'ENG','Programme category restriction for product'),(16,308,'ARA','Programme restriction for clash'),(17,316,'ENG','Index restriction for clash'),(18,316,'ARA','Index restriction for clash'),(19,317,'ENG','Programme classification restriction for product'),(20,317,'ARA','Programme classification restriction for product'),(21,325,'ARA','Sponsorship Restriction Applied For Competitor Spot Based On Competing Clash'),(22,325,'ENG','Sponsorship Restriction Applied For Competitor Spot Based On Competing Clash'),(23,324,'ARA','Clearance code restriction for clash'),(24,324,'ENG','Clearance code restriction for clash'),(25,323,'ARA','Clearance code restriction for clearance code'),(26,323,'ENG','Clearance code restriction for clearance code'),(27,322,'ARA','Clearance code restriction for copy'),(28,308,'ENG','Programme restriction for clash'),(29,322,'ENG','Clearance code restriction for copy'),(30,321,'ENG','Clearance code restriction for product'),(31,320,'ARA','Programme classification restriction for clash'),(32,320,'ENG','Programme classification restriction for clash'),(33,319,'ARA','Programme classification restriction for clearance code'),(34,319,'ENG','Programme classification restriction for clearance code'),(35,318,'ARA','Programme classification restriction for copy'),(36,318,'ENG','Programme classification restriction for copy'),(37,321,'ARA','Clearance code restriction for product'),(38,307,'ARA','Programme restriction for clearance code'),(39,307,'ENG','Programme restriction for clearance code'),(40,306,'ARA','Programme restriction for copy'),(41,9,'ENG','Spot bumped from previous run'),(42,8,'ARA','Cannot add Top and Tail multipart spots to the same break'),(43,8,'ENG','Cannot add Top and Tail multipart spots to the same break'),(44,7,'ARA','Multiple spots with the same requested position in break'),(45,7,'ENG','Multiple spots with the same requested position in break'),(46,6,'ARA','Multiple spots with the same requested break'),(47,6,'ENG','Multiple spots with the same requested break'),(48,9,'ARA','Spot bumped from previous run'),(49,5,'ARA','Campaign clash'),(50,4,'ARA','Product clash'),(51,4,'ENG','Product clash'),(52,3,'ARA','Insufficient break availability'),(53,3,'ENG','Insufficient break availability'),(54,2,'ARA','Time requirements for spot'),(55,2,'ENG','Time requirements for spot'),(56,1,'ARA','Invalid break type'),(57,5,'ENG','Campaign clash'),(58,326,'ENG','Sponsorship Restriction Applied For Competitor Spot Based On Competing Advertiser'),(59,10,'ENG','Advertiser clash'),(60,11,'ENG','Cannot add Same Break spots to the same break'),(61,306,'ENG','Programme restriction for copy'),(62,305,'ARA','Programme restriction for product'),(63,305,'ENG','Programme restriction for product'),(64,304,'ARA','Time restriction for clash'),(65,304,'ENG','Time restriction for clash'),(66,303,'ARA','Time restriction for clearance code'),(67,303,'ENG','Time restriction for clearance code'),(68,10,'ARA','Advertiser clash'),(69,302,'ARA','Time restriction for copy'),(70,301,'ARA','Time restriction for product'),(71,301,'ENG','Time restriction for product'),(72,201,'ARA','Spot moved between breaks'),(73,201,'ENG','Spot moved between breaks'),(74,101,'ARA','No attempt to place spot due to no break or programme data'),(75,101,'ENG','No attempt to place spot due to no break or programme data'),(76,11,'ARA','Cannot add Same Break spots to the same break'),(77,302,'ENG','Time restriction for copy'),(78,326,'ARA','Sponsorship Restriction Applied For Competitor Spot Based On Competing Advertiser');
/*!40000 ALTER TABLE `smoothfailuremessagedescriptions` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-09-28  9:40:17
