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
-- Dumping data for table `brsconfigurationforkpis`
--

LOCK TABLES `brsconfigurationforkpis` WRITE;
/*!40000 ALTER TABLE `brsconfigurationforkpis` DISABLE KEYS */;
INSERT INTO `brsconfigurationforkpis` VALUES (1,1,'percent95to105',4),(2,1,'percentbelow75',4),(3,1,'averageEfficiency',4),(4,1,'totalSpotsBooked',4),(5,1,'remainaudience',4),(6,1,'remainingAvailability',4),(7,1,'standardAverageCompletion',4),(8,1,'weightedAverageCompletion',4),(9,1,'averageSpotsDeliveredPerDay',1),(10,1,'totalZeroRatedSpots',1),(11,1,'totalValueDelivered',1),(12,2,'percent95to105',2),(13,2,'percentbelow75',3),(14,2,'averageEfficiency',2),(15,2,'totalSpotsBooked',2),(16,2,'remainaudience',4),(17,2,'remainingAvailability',2),(18,2,'standardAverageCompletion',5),(19,2,'weightedAverageCompletion',2),(20,2,'totalValueDelivered',4),(21,2,'averageSpotsDeliveredPerDay',3),(22,2,'totalZeroRatedSpots',3),(23,3,'percent95to105',4),(24,3,'percentbelow75',4),(25,3,'averageEfficiency',3),(26,3,'totalSpotsBooked',6),(27,3,'remainaudience',5),(28,3,'remainingAvailability',1),(29,3,'standardAverageCompletion',1),(30,3,'weightedAverageCompletion',1),(31,3,'totalValueDelivered',5),(32,3,'averageSpotsDeliveredPerDay',4),(33,3,'totalZeroRatedSpots',4),(34,4,'percent95to105',4),(35,4,'percentbelow75',4),(36,4,'averageEfficiency',3),(37,4,'totalSpotsBooked',4),(38,4,'remainaudience',3),(39,4,'remainingAvailability',6),(40,4,'standardAverageCompletion',5),(41,4,'weightedAverageCompletion',6),(42,4,'totalValueDelivered',3),(43,4,'averageSpotsDeliveredPerDay',4),(44,4,'totalZeroRatedSpots',4),(45,1,'conversionEfficiencyADS',1),(46,1,'conversionEfficiencyMN1634',1),(47,1,'conversionEfficiencyCHD',1),(48,1,'conversionEfficiencyHWCH',1),(49,1,'conversionEfficiencyADABC1',1),(50,1,'availableRatingsADS',1),(51,1,'availableRatingsMN1634',1),(52,1,'availableRatingsCHD',1),(53,1,'availableRatingsHWCH',1),(54,1,'availableRatingsADABC1',1),(55,1,'differenceValue',1),(56,1,'differenceValuePercentage',1),(57,1,'differenceValueWithPayback',1),(58,1,'differenceValuePercentagePayback',1),(59,1,'ratingCampaignsRatedSpots',1),(60,1,'reservedRatingsADS',1),(61,1,'spotCampaignsRatedSpots',1),(62,1,'totalNominalValue',1),(63,2,'conversionEfficiencyADS',1),(64,2,'conversionEfficiencyMN1634',1),(65,2,'conversionEfficiencyCHD',1),(66,2,'conversionEfficiencyHWCH',1),(67,2,'conversionEfficiencyADABC1',1),(68,2,'availableRatingsADS',1),(69,2,'availableRatingsMN1634',1),(70,2,'availableRatingsCHD',1),(71,2,'availableRatingsHWCH',1),(72,2,'availableRatingsADABC1',1),(73,2,'differenceValue',1),(74,2,'differenceValuePercentage',1),(75,2,'differenceValueWithPayback',1),(76,2,'differenceValuePercentagePayback',1),(77,2,'ratingCampaignsRatedSpots',1),(78,2,'reservedRatingsADS',1),(79,2,'spotCampaignsRatedSpots',1),(80,2,'totalNominalValue',1),(81,3,'conversionEfficiencyADS',1),(82,3,'conversionEfficiencyMN1634',1),(83,3,'conversionEfficiencyCHD',1),(84,3,'conversionEfficiencyHWCH',1),(85,3,'conversionEfficiencyADABC1',1),(86,3,'availableRatingsADS',1),(87,3,'availableRatingsMN1634',1),(88,3,'availableRatingsCHD',1),(89,3,'availableRatingsHWCH',1),(90,3,'availableRatingsADABC1',1),(91,3,'differenceValue',1),(92,3,'differenceValuePercentage',1),(93,3,'differenceValueWithPayback',1),(94,3,'differenceValuePercentagePayback',1),(95,3,'ratingCampaignsRatedSpots',1),(96,3,'reservedRatingsADS',1),(97,3,'spotCampaignsRatedSpots',1),(98,3,'totalNominalValue',1),(99,4,'conversionEfficiencyADS',1),(100,4,'conversionEfficiencyMN1634',1),(101,4,'conversionEfficiencyCHD',1),(102,4,'conversionEfficiencyHWCH',1),(103,4,'conversionEfficiencyADABC1',1),(104,4,'availableRatingsADS',1),(105,4,'availableRatingsMN1634',1),(106,4,'availableRatingsCHD',1),(107,4,'availableRatingsHWCH',1),(108,4,'availableRatingsADABC1',1),(109,4,'differenceValue',1),(110,4,'differenceValuePercentage',1),(111,4,'differenceValueWithPayback',1),(112,4,'differenceValuePercentagePayback',1),(113,4,'ratingCampaignsRatedSpots',1),(114,4,'reservedRatingsADS',1),(115,4,'spotCampaignsRatedSpots',1),(116,4,'totalNominalValue',1),(117,5,'percent95to105',5),(118,5,'availableRatingsCHD',1),(119,5,'availableRatingsMN1634',1),(120,5,'reservedRatingsADS',1),(121,5,'availableRatingsADS',1),(122,5,'differenceValuePercentagePayback',1),(123,5,'differenceValueWithPayback',1),(124,5,'differenceValuePercentage',1),(125,5,'differenceValue',1),(126,5,'totalNominalValue',1),(127,5,'spotCampaignsRatedSpots',1),(128,5,'ratingCampaignsRatedSpots',1),(129,5,'conversionEfficiencyADABC1',1),(130,5,'availableRatingsHWCH',1),(131,5,'conversionEfficiencyHWCH',1),(132,5,'conversionEfficiencyMN1634',1),(133,5,'conversionEfficiencyADS',1),(134,5,'totalZeroRatedSpots',1),(135,5,'averageSpotsDeliveredPerDay',1),(136,5,'totalValueDelivered',1),(137,5,'weightedAverageCompletion',1),(138,5,'standardAverageCompletion',1),(139,5,'remainingAvailability',1),(140,5,'remainaudience',1),(141,5,'totalSpotsBooked',1),(142,5,'averageEfficiency',1),(143,5,'percentbelow75',1),(144,5,'conversionEfficiencyCHD',1),(145,5,'availableRatingsADABC1',1);
/*!40000 ALTER TABLE `brsconfigurationforkpis` ENABLE KEYS */;
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
