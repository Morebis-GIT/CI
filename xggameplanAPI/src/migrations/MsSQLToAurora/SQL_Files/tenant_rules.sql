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
-- Dumping data for table `rules`
--

LOCK TABLES `rules` WRITE;
/*!40000 ALTER TABLE `rules` DISABLE KEYS */;
INSERT INTO `rules` VALUES (1,1,1,'Defaults','Minimum Efficiency',NULL,NULL,NULL,NULL,NULL,NULL,'general'),(2,2,1,'Defaults','Maximum Rank',NULL,NULL,NULL,NULL,NULL,NULL,'general'),(3,3,1,'Defaults','Demograph Banding Tolerance',NULL,NULL,NULL,NULL,NULL,NULL,'general'),(4,4,1,'Defaults','Default Centre Break Ratio',NULL,NULL,NULL,NULL,NULL,NULL,'general'),(5,5,1,'Defaults','Use Max Spot Ratings Set By Campaigns',NULL,NULL,NULL,NULL,NULL,NULL,'general'),(6,6,1,'Defaults','Sponsorship Exclusivity',NULL,NULL,NULL,NULL,NULL,NULL,'general'),(7,1,2,'Weightings','Campaign',NULL,NULL,NULL,NULL,NULL,NULL,'weightings'),(8,2,2,'Weightings','Campaign Sales Area',NULL,NULL,NULL,NULL,NULL,NULL,'weightings'),(9,3,2,'Weightings','Spot Length',NULL,NULL,NULL,NULL,NULL,NULL,'weightings'),(10,4,2,'Weightings','Daypart',NULL,NULL,NULL,NULL,NULL,NULL,'weightings'),(11,5,2,'Weightings','Strike Weight',NULL,NULL,NULL,NULL,NULL,NULL,'weightings'),(12,6,2,'Weightings','Fit to Requirement',NULL,NULL,NULL,NULL,NULL,NULL,'weightings'),(13,7,2,'Weightings','Fit to Spot Length',NULL,NULL,NULL,NULL,NULL,NULL,'weightings'),(14,8,2,'Weightings','Centre/End',NULL,NULL,NULL,NULL,NULL,NULL,'weightings'),(15,9,2,'Weightings','Booking Position',NULL,NULL,NULL,NULL,NULL,NULL,'weightings'),(16,1,3,'Campaign','Campaign',NULL,NULL,NULL,NULL,NULL,NULL,'tolerances'),(17,8,3,'Campaign','Booking Position',NULL,NULL,NULL,NULL,NULL,NULL,'tolerances'),(18,9,3,'Campaign','Centre/End',NULL,NULL,NULL,NULL,NULL,NULL,'tolerances'),(19,2,3,'Sales Area','Sales Area',NULL,NULL,NULL,NULL,NULL,NULL,'tolerances'),(20,3,3,'Sales Area','Spot Length',NULL,NULL,NULL,NULL,NULL,NULL,'tolerances'),(21,4,3,'Sales Area','Daypart',NULL,NULL,NULL,NULL,NULL,NULL,'tolerances'),(22,5,3,'Sales Area','Strike Weight',NULL,NULL,NULL,NULL,NULL,NULL,'tolerances'),(23,6,3,'Sales Area','Strike Wgt/Daypart',NULL,NULL,NULL,NULL,NULL,NULL,'tolerances'),(24,7,3,'Sales Area','Peak Daypart',NULL,NULL,NULL,NULL,NULL,NULL,'tolerances'),(25,12,3,'Sales Area','Daypart/Length',NULL,NULL,NULL,NULL,NULL,NULL,'tolerances'),(26,13,3,'Sales Area','Strike Weight/Length',NULL,NULL,NULL,NULL,NULL,NULL,'tolerances'),(27,14,3,'Sales Area','Strike Weight/Daypart/Length',NULL,NULL,NULL,NULL,NULL,NULL,'tolerances'),(28,10,3,'Budget','Campaign (Budget)',NULL,NULL,NULL,NULL,NULL,NULL,'tolerances'),(29,11,3,'Budget','Sales Area (Budget)',NULL,NULL,NULL,NULL,NULL,NULL,'tolerances'),(30,37,3,'Campaign','Programme',NULL,NULL,NULL,NULL,NULL,NULL,'tolerances'),(31,1,4,'Slotting Controls','Max Spots Per Day',NULL,NULL,NULL,NULL,NULL,NULL,'rules'),(32,2,4,'Slotting Controls','Max Spots Per Hour',NULL,NULL,NULL,NULL,NULL,NULL,'rules'),(33,3,4,'Slotting Controls','Max Spots Per 2 Hours',NULL,NULL,NULL,NULL,NULL,NULL,'rules'),(34,4,4,'Slotting Controls','Min Breaks Between Spots',NULL,NULL,NULL,NULL,NULL,NULL,'rules'),(35,5,4,'Slotting Controls','Min Hours Between Spots',NULL,NULL,NULL,NULL,NULL,NULL,'rules'),(36,6,4,'Slotting Controls','Max Spots Per Programme/Day',NULL,NULL,NULL,NULL,NULL,NULL,'rules'),(37,7,4,'Slotting Controls','Max Spots Per Programme/Week',NULL,NULL,NULL,NULL,NULL,NULL,'rules'),(38,17,4,'Slotting Controls','Max Spots per Prog/100 rtgs',NULL,NULL,NULL,NULL,NULL,NULL,'rules'),(39,18,4,'Slotting Controls','Max Zero Ratings',NULL,NULL,NULL,NULL,NULL,NULL,'rules'),(40,20,4,'Slotting Controls','Min Weeks Between Programmes',NULL,NULL,NULL,NULL,NULL,NULL,'rules'),(41,21,4,'Slotting Controls','Max Rating Points for Spot Campaigns',NULL,NULL,NULL,NULL,NULL,NULL,'rules'),(42,22,4,'Slotting Controls','Max Sub Area Slotting %',NULL,NULL,NULL,NULL,NULL,NULL,'rules'),(43,23,4,'Slotting Controls','Minimum Break Availability',NULL,NULL,NULL,NULL,NULL,NULL,'rules'),(44,24,4,'Slotting Controls','Max Spots Per Prog/Time',NULL,NULL,NULL,NULL,NULL,NULL,'rules'),(45,25,4,'Slotting Controls','Min Days Between Prog/Time',NULL,NULL,NULL,NULL,NULL,NULL,'rules'),(46,26,4,'Slotting Controls','Min Weeks Between Prog/Time',NULL,NULL,NULL,NULL,NULL,NULL,'rules'),(47,27,4,'Slotting Controls','Max Rating Points for Rating Campaigns',NULL,NULL,NULL,NULL,NULL,NULL,'rules');
/*!40000 ALTER TABLE `rules` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-09-28  9:40:15
