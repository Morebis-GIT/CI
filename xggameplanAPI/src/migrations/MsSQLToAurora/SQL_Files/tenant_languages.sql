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
-- Dumping data for table `languages`
--

LOCK TABLES `languages` WRITE;
/*!40000 ALTER TABLE `languages` DISABLE KEYS */;
INSERT INTO `languages` VALUES (1,'Description','ALPHA3','ALPHA2'),(2,'Norwegian Nynorsk; Nynorsk, Norwegian','NNO','NN'),(3,'Bokmål, Norwegian; Norwegian Bokmål','NOB','NB'),(4,'Norwegian','NOR','NO'),(5,'Chichewa; Chewa; Nyanja','NYA','NY'),(6,'Occitan (post 1500); Provençal','OCI','OC'),(7,'Ojibwa','OJI','OJ'),(8,'Oriya','ORI','OR'),(9,'Oromo','ORM','OM'),(10,'Ossetian; Ossetic','OSS','OS'),(11,'Panjabi; Punjabi','PAN','PA'),(12,'Persian','PER','FA'),(13,'Pali','PLI','PI'),(14,'Polish','POL','PL'),(15,'Portuguese','POR','PT'),(16,'Pushto; Pashto','PUS','PS'),(17,'Quechua','QUE','QU'),(18,'Romansh','ROH','RM'),(19,'Romanian; Moldavian; Moldovan','RUM','RO'),(20,'Rundi','RUN','RN'),(21,'Nepali','NEP','NE'),(22,'Russian','RUS','RU'),(23,'Ndonga','NDO','NG'),(24,'Ndebele, South; South Ndebele','NBL','NR'),(25,'Latin','LAT','LA'),(26,'Latvian','LAV','LV'),(27,'Limburgan; Limburger; Limburgish','LIM','LI'),(28,'Lingala','LIN','LN'),(29,'Lithuanian','LIT','LT'),(30,'Luxembourgish; Letzeburgesch','LTZ','LB'),(31,'Luba-Katanga','LUB','LU'),(32,'Ganda','LUG','LG'),(33,'Macedonian','MAC','MK'),(34,'Marshallese','MAH','MH'),(35,'Malayalam','MAL','ML'),(36,'Maori','MAO','MI'),(37,'Marathi','MAR','MR'),(38,'Malay','MAY','MS'),(39,'Malagasy','MLG','MG'),(40,'Maltese','MLT','MT'),(41,'Mongolian','MON','MN'),(42,'Nauru','NAU','NA'),(43,'Navajo; Navaho','NAV','NV'),(44,'Ndebele, North; North Ndebele','NDE','ND'),(45,'Sango','SAG','SG'),(46,'Sanskrit','SAN','SA'),(47,'Sinhala; Sinhalese','SIN','SI'),(48,'Tonga (Tonga Islands)','TON','TO'),(49,'Tswana','TSN','TN'),(50,'Tsonga','TSO','TS'),(51,'Turkmen','TUK','TK'),(52,'Turkish','TUR','TR'),(53,'Twi','TWI','TW'),(54,'Uighur; Uyghur','UIG','UG'),(55,'Ukrainian','UKR','UK'),(56,'Urdu','URD','UR'),(57,'Uzbek','UZB','UZ'),(58,'Venda','VEN','VE'),(59,'Vietnamese','VIE','VI'),(60,'Volapük','VOL','VO'),(61,'Welsh','WEL','CY'),(62,'Walloon','WLN','WA'),(63,'Wolof','WOL','WO'),(64,'Xhosa','XHO','XH'),(65,'Yiddish','YID','YI'),(66,'Yoruba','YOR','YO'),(67,'Tigrinya','TIR','TI'),(68,'Tibetan','TIB','BO'),(69,'Thai','THA','TH'),(70,'Tagalog','TGL','TL'),(71,'Slovak','SLO','SK'),(72,'Slovenian','SLV','SL'),(73,'Northern Sami','SME','SE'),(74,'Samoan','SMO','SM'),(75,'Shona','SNA','SN'),(76,'Sindhi','SND','SD'),(77,'Somali','SOM','SO'),(78,'Sotho, Southern','SOT','ST'),(79,'Spanish; Castilian','SPA','ES'),(80,'Lao','LAO','LO'),(81,'Sardinian','SRD','SC'),(82,'Swati','SSW','SS'),(83,'Sundanese','SUN','SU'),(84,'Swahili','SWA','SW'),(85,'Swedish','SWE','SV'),(86,'Tahitian','TAH','TY'),(87,'Tamil','TAM','TA'),(88,'Tatar','TAT','TT'),(89,'Telugu','TEL','TE'),(90,'Tajik','TGK','TG'),(91,'Serbian','SRP','SR'),(92,'Zhuang; Chuang','ZHA','ZA'),(93,'Kurdish','KUR','KU'),(94,'Korean','KOR','KO'),(95,'Bulgarian','BUL','BG'),(96,'Burmese','BUR','MY'),(97,'Catalan; Valencian','CAT','CA'),(98,'Chamorro','CHA','CH'),(99,'Chechen','CHE','CE'),(100,'Chinese','CHI','ZH'),(101,'Church Slavic; Old Slavonic; Church Slavonic; Old Bulgarian; Old Church Slavonic','CHU','CU'),(102,'Chuvash','CHV','CV'),(103,'Cornish','COR','KW'),(104,'Corsican','COS','CO'),(105,'Cree','CRE','CR'),(106,'Czech','CZE','CS'),(107,'Danish','DAN','DA'),(108,'Divehi; Dhivehi; Maldivian','DIV','DV'),(109,'Dutch; Flemish','DUT','NL'),(110,'Dzongkha','DZO','DZ'),(111,'English','ENG','EN'),(112,'Esperanto','EPO','EO'),(113,'Estonian','EST','ET'),(114,'Breton','BRE','BR'),(115,'Ewe','EWE','EE'),(116,'Bosnian','BOS','BS'),(117,'Bihari languages','BIH','BH'),(118,'Afar','AAR','AA'),(119,'Abkhazian','ABK','AB'),(120,'Afrikaans','AFR','AF'),(121,'Akan','AKA','AK'),(122,'Albanian','ALB','SQ'),(123,'Amharic','AMH','AM'),(124,'Arabic','ARA','AR'),(125,'Aragonese','ARG','AN'),(126,'Armenian','ARM','HY'),(127,'Assamese','ASM','AS'),(128,'Avaric','AVA','AV'),(129,'Avestan','AVE','AE'),(130,'Aymara','AYM','AY'),(131,'Azerbaijani','AZE','AZ'),(132,'Bashkir','BAK','BA'),(133,'Bambara','BAM','BM'),(134,'Basque','BAQ','EU'),(135,'Belarusian','BEL','BE'),(136,'Bengali','BEN','BN'),(137,'Bislama','BIS','BI'),(138,'Faroese','FAO','FO'),(139,'Fijian','FIJ','FJ'),(140,'Finnish','FIN','FI'),(141,'Inuktitut','IKU','IU'),(142,'Interlingue; Occidental','ILE','IE'),(143,'Interlingua (International Auxiliary Language Association)','INA','IA'),(144,'Indonesian','IND','ID'),(145,'Inupiaq','IPK','IK'),(146,'Italian','ITA','IT'),(147,'Javanese','JAV','JV'),(148,'Japanese','JPN','JA'),(149,'Kalaallisut; Greenlandic','KAL','KL'),(150,'Kannada','KAN','KN'),(151,'Kashmiri','KAS','KS'),(152,'Kanuri','KAU','KR'),(153,'Kazakh','KAZ','KK'),(154,'Central Khmer','KHM','KM'),(155,'Kikuyu; Gikuyu','KIK','KI'),(156,'Kinyarwanda','KIN','RW'),(157,'Kirghiz; Kyrgyz','KIR','KY'),(158,'Komi','KOM','KV'),(159,'Kongo','KON','KG'),(160,'Sichuan Yi; Nuosu','III','II'),(161,'Ido','IDO','IO'),(162,'Icelandic','ICE','IS'),(163,'Igbo','IBO','IG'),(164,'French','FRE','FR'),(165,'Western Frisian','FRY','FY'),(166,'Fulah','FUL','FF'),(167,'Georgian','GEO','KA'),(168,'German','GER','DE'),(169,'Gaelic; Scottish Gaelic','GLA','GD'),(170,'Irish','GLE','GA'),(171,'Galician','GLG','GL'),(172,'Manx','GLV','GV'),(173,'Kuanyama; Kwanyama','KUA','KJ'),(174,'Greek, Modern (1453-)','GRE','EL'),(175,'Gujarati','GUJ','GU'),(176,'Haitian; Haitian Creole','HAT','HT'),(177,'Hausa','HAU','HA'),(178,'Hebrew','HEB','HE'),(179,'Herero','HER','HZ'),(180,'Hindi','HIN','HI'),(181,'Hiri Motu','HMO','HO'),(182,'Croatian','HRV','HR'),(183,'Hungarian','HUN','HU'),(184,'Guarani','GRN','GN'),(185,'Zulu','ZUL','ZU');
/*!40000 ALTER TABLE `languages` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-09-28  9:40:14
