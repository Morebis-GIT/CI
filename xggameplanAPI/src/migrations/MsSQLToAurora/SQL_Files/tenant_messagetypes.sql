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
-- Dumping data for table `messagetypes`
--

LOCK TABLES `messagetypes` WRITE;
/*!40000 ALTER TABLE `messagetypes` DISABLE KEYS */;
INSERT INTO `messagetypes` VALUES ('IBulkBookingPositionGroupCreated','IBulkBookingPositionGroupCreated',9,NULL,1,NULL),('IBulkBookingPositionGroupDeleted','IBulkBookingPositionGroupDeleted',10,NULL,1,NULL),('IBulkBreakCreated','Bulk Break Created',5,NULL,2,NULL),('IBulkBreaksDeleted','Bulk Breaks Deleted',6,1,2,NULL),('IBulkBreakTypeCreated','IBulkBreakTypeCreated',6,NULL,3,NULL),('IBulkBreakTypeDeleted','IBulkBreakTypeDeleted',7,NULL,3,NULL),('IBulkCampaignCreatedOrUpdated','Campaign Created Or Updated',0,50,4,NULL),('IBulkCampaignDeleted','Campaign Deleted',10,NULL,4,NULL),('IBulkClashCreatedOrUpdated','Bulk Clash Created Or Updated',3,NULL,5,NULL),('IBulkClashDeleted','Clash Deleted',5,NULL,5,NULL),('IBulkClashExceptionCreated','Bulk Clash Exception Created',2,NULL,6,NULL),('IBulkClashExceptionDeleted','Bulk Clash Exception Deleted',3,NULL,6,NULL),('IBulkDemographicCreatedOrUpdated','Bulk Demographics Created',9,NULL,9,NULL),('IBulkDemographicDeleted','Demographic Deleted',10,NULL,9,NULL),('IBulkHolidayCreated','Holiday Created',2,NULL,10,NULL),('IBulkHolidayDeleted','Holiday Deleted',5,NULL,10,NULL),('IBulkInventoryLockCreated','IBulkInventoryLockCreated',3,NULL,11,NULL),('IBulkInventoryLockDeleted','IBulkInventoryLockDeleted',9,NULL,11,NULL),('IBulkInventoryTypeCreated','IBulkInventoryTypeCreated',2,NULL,11,NULL),('IBulkInventoryTypeDeleted','IBulkInventoryTypeDeleted',7,NULL,11,NULL),('IBulkLengthFactorCreated','IBulkLengthFactorCreated',6,NULL,22,NULL),('IBulkLengthFactorDeleted','IBulkLengthFactorDeleted',7,NULL,22,NULL),('IBulkLockTypeCreated','IBulkLockTypeCreated',4,NULL,11,NULL),('IBulkLockTypeDeleted','IBulkLockTypeDeleted',8,NULL,11,NULL),('IBulkProductCreatedOrUpdated','Product Created Or Updated',9,NULL,12,NULL),('IBulkProductDeleted','Product Deleted',10,NULL,12,NULL),('IBulkProgrammeCategoryCreated','IBulkProgrammeCategoryCreated',6,NULL,14,NULL),('IBulkProgrammeCategoryDeleted','IBulkProgrammeCategoryDeleted',7,NULL,14,NULL),('IBulkProgrammeClassificationCreated','Bulk Programme Classification Created',6,NULL,15,NULL),('IBulkProgrammeCreated','Bulk Programme Created',5,200,13,NULL),('IBulkProgrammeDeleted','Programmes Deleted',6,10,13,NULL),('IBulkRatingsPredictionSchedulesCreated','Bulk Ratings Prediction Schedules Created',1,NULL,16,NULL),('IBulkRatingsPredictionSchedulesDeleted','Bulk Ratings Prediction Schedules Deleted',5,NULL,16,NULL),('IBulkRestrictionCreatedOrUpdated','Restriction Created Or Updated',3,NULL,17,NULL),('IBulkRestrictionDeleted','Restriction Deleted',4,NULL,17,NULL),('IBulkSalesAreaCreatedOrUpdated','Sales Area Created',8,NULL,18,NULL),('IBulkSalesAreaDeleted','Sales Area Updated',9,NULL,18,NULL),('IBulkSpotBookingRuleCreated','Bulk Spot Booking Rule Created',6,NULL,23,NULL),('IBulkSpotBookingRuleDeleted','Bulk Spot Booking Rule Deleted',7,NULL,23,NULL),('IBulkSpotCreatedOrUpdated','Spot Created Or Updated',5,NULL,19,NULL),('IBulkSpotDeleted','Bulk Spot Deleted',9,NULL,19,NULL),('IBulkStandardDayPartCreated','IBulkStandardDayPartCreated',6,NULL,8,NULL),('IBulkStandardDayPartDeleted','IBulkStandardDayPartDeleted',7,NULL,8,NULL),('IBulkStandardDayPartGroupCreated','IBulkStandardDayPartGroupCreated',6,NULL,7,NULL),('IBulkStandardDayPartGroupDeleted','IBulkStandardDayPartGroupDeleted',7,NULL,7,NULL),('IBulkTotalRatingCreated','IBulkTotalRatingCreated',2,NULL,20,NULL),('IBulkTotalRatingDeleted','IBulkTotalRatingDeleted',4,NULL,20,NULL),('IBulkUniverseCreated','Bulk Universe Created',0,NULL,21,NULL),('IBulkUniverseDeleted','Universes Deleted',1,NULL,21,NULL),('IClashUpdated','Clash Updated',4,NULL,5,NULL),('IDeleteAllProgrammeClassification','Delete All Programme Classification',8,NULL,15,NULL),('IDemographicUpdated','Demographic Updated',3,NULL,9,NULL),('IPayloadReference','IPayloadReference',2,NULL,11,NULL),('ISalesAreaUpdated','Sales Area Updated',3,NULL,18,NULL);
/*!40000 ALTER TABLE `messagetypes` ENABLE KEYS */;
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
