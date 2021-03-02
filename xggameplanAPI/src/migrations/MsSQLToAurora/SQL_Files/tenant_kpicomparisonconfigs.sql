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
-- Dumping data for table `kpicomparisonconfigs`
--

LOCK TABLES `kpicomparisonconfigs` WRITE;
/*!40000 ALTER TABLE `kpicomparisonconfigs` DISABLE KEYS */;
INSERT INTO `kpicomparisonconfigs` VALUES (1,'percent95to105',1,'',''),(2,'percentbelow95',1,'\0',''),(3,'percentbelow75',1,'\0',''),(4,'remainaudience',10000,'',''),(5,'remainingAvailability',1000,'',''),(6,'averageEfficiency',1,'',''),(7,'averagecancelEfficiency',1,'',''),(8,'percentgreater100',1,'','\0'),(9,'totalSpotsBooked',1,'','\0'),(10,'noOfCampaigns',1,'','\0'),(11,'availability',1,'',''),(12,'standardAverageCompletion',1,'',''),(13,'weightedAverageCompletion',1,'',''),(14,'totalValueDelivered',1,'',''),(15,'averageSpotsDeliveredPerDay',1,'',''),(16,'totalZeroRatedSpots',1,'\0',''),(17,'conversionEfficiencyADS',1,'',''),(18,'conversionEfficiencyMN1634',1,'',''),(19,'conversionEfficiencyCHD',1,'',''),(20,'conversionEfficiencyHWCH',1,'',''),(21,'conversionEfficiencyADABC1',1,'',''),(22,'availableRatingsADS',1,'\0',''),(23,'availableRatingsMN1634',1,'\0',''),(24,'availableRatingsCHD',1,'\0',''),(25,'availableRatingsHWCH',1,'\0',''),(26,'availableRatingsADABC1',1,'\0',''),(27,'reservedRatingsADS',1,'\0',''),(28,'ratingCampaignsRatedSpots',1,'','\0'),(29,'spotCampaignsRatedSpots',1,'','\0'),(30,'baseDemographRatings',1,'',''),(31,'totalRatingCampaignSpots',1,'','\0'),(32,'totalSpotCampaignSpots',1,'','\0'),(33,'totalNominalValue',1,'',''),(34,'differenceValue',1,'',''),(35,'differenceValueWithPayback',1,'',''),(36,'differenceValuePercentage',1,'',''),(37,'differenceValuePercentagePayback',1,'','');
/*!40000 ALTER TABLE `kpicomparisonconfigs` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-09-28  9:40:19
