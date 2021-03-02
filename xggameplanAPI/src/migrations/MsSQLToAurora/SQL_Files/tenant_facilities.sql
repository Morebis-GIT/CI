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
-- Dumping data for table `facilities`
--

LOCK TABLES `facilities` WRITE;
/*!40000 ALTER TABLE `facilities` DISABLE KEYS */;
INSERT INTO `facilities` VALUES (1,'ABAREA','Allow split/joint spots in Automated Booking','\0'),(2,'ABBNEV','Automated Booking Include Bonus Spot Delivery','\0'),(3,'ABBONS','Autobook Bonus Value','\0'),(4,'ABBTRQ','Automated Booking Book to Target Requirement',''),(5,'ABDBUG','Automated Booking Debug File Generation',''),(6,'ABDEMO','Automated Booking Process Active Demographics Only','\0'),(7,'ABDPYT','Use Campaign Daypart in Addition to AB Daypart',''),(8,'ABEVTH','Automated Booking - Evaluation Threading',''),(9,'ABFAIL','Automated Booking - All Pass Failure Reporting',''),(10,'ABPRC2','Automated Booking Pricing Spot Based Campaign Only','\0'),(11,'ABPRCE','Autoboook Spot Pricing','\0'),(12,'ABPREQ','Automated Booking - Programme Inclusion with Slotting Controls','\0'),(13,'ABSPLG','Autobook by Length by Daypart by Strike Weight','\0'),(14,'ABSTAT','Store Run Statistics for Request',''),(15,'ABTZRB','Autobook Target Zero Rated Break',''),(16,'ABWGHT','Random Automated Booking Weightings','\0'),(17,'ABZBON','Automated Booking - Zero Price Bonus Spots','\0'),(18,'ABZERO','Book Campaigns with Zero Requirement','\0'),(19,'ABZRTG','Zero Rated Breaks Count for Rating Delivery','\0'),(20,'ADCLSH','Advertiser Clash','\0'),(21,'AGREG','Autogeneration Nat/Reg Booking',''),(22,'AUTSAG','Autobooking by Sales Area group','\0'),(23,'AUTSPN','Autobook Sponsorship campaigns by programme','\0'),(24,'B2BCLH','Back To Back Clash Available','\0'),(25,'BRKBON','Bonus Restriction on Break','\0'),(26,'BRKPGR','Break Position Groups',''),(27,'BTYPLN','Book spots by campaign Break Type and Length','\0'),(28,'CPPBKP','CPP Break Pricing','\0'),(29,'CURFIL','Earlier Automated Filling date','\0'),(30,'DECARE','Deal Campaign Regional Exclusions','\0'),(31,'DEMRST','Demographic Booking Restrictions','\0'),(32,'DISSUR','Campaign Discount and Surcharge','\0'),(33,'DLBOTY','Deal Bonus Types','\0'),(34,'DPSTWT','Define Dayparts by Deal and Campaign period','\0'),(35,'EFFFAC','Efficiency Factors',''),(36,'EFFRAN','Autogen Random Efficiency at 100eff',''),(37,'EPGTGT','EPG (Target) Sales Area Processing',''),(38,'EXCLSH','EXTEND CLASH',''),(39,'EXPDIS','Expected Expenditure Discounts','\0'),(40,'FLRPRC','Floor Pricing','\0'),(41,'HFSSIX','Enable Index Restrictions for HFSS or Age Group',''),(42,'LCKPRC','Lock Spot Price','\0'),(43,'MCNPSD','Update Spot from Nominal Prices','\0'),(44,'MNPRDC','Apply Discounts to Manually Priced Spots - Bulk','\0'),(45,'NATREG','National Regional Breaks','\0'),(46,'NODISC','Discount Processing Not Required','\0'),(47,'PGRCLA','Programme Classification','\0'),(48,'RATING','Hide Ratings','\0'),(49,'REGBRK','Regional Break - Store Reduced Break Information','\0'),(50,'RNDPRC','Spot Price - Round to Sales Area Ratecard Rounding','\0'),(51,'RTCTCA','Rate Card Tariff Category','\0'),(52,'SALREG','Regional Sales Availability','\0'),(53,'SANOEX','No of clash exposures defined by sales area',''),(54,'SBSPBP','Spot Book Starting Price from Break Price','\0'),(55,'SPNADV','Sponsorship Entitlement Group By Advertiser','\0'),(56,'SPNCHK','Sponsership Checking Required (y/n)',''),(57,'SPNENT','Sponsorship entitlement','\0'),(58,'SPTMSD','Spots map to scheduled payments','\0'),(59,'TOPRTG','Tot Spot Ratings Only','\0'),(60,'TOTEFF','Total efficiency for all Sales Areas within Break','\0'),(61,'XGCSVO','xG Gameplan - Produce Output Files with Header information',''),(62,'XGRTIM','xG Gameplan - Receive Ratings as Impressions','\0');
/*!40000 ALTER TABLE `facilities` ENABLE KEYS */;
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
