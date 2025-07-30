-- MySQL dump 10.13  Distrib 8.0.42, for Linux (x86_64)
--
-- Host: 127.0.0.1    Database: petanque
-- ------------------------------------------------------
-- Server version	8.4.2

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `Aanwezigheid`
--

DROP TABLE IF EXISTS `Aanwezigheid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Aanwezigheid` (
  `aanwezigheidId` int NOT NULL AUTO_INCREMENT,
  `speeldagId` int DEFAULT NULL,
  `spelerId` int DEFAULT NULL,
  `spelerVolgnr` int NOT NULL,
  PRIMARY KEY (`aanwezigheidId`),
  UNIQUE KEY `Aanwezigheid_uk` (`speeldagId`,`spelerVolgnr`),
  KEY `speeldagId` (`speeldagId`),
  KEY `spelerId` (`spelerId`),
  CONSTRAINT `Aanwezigheid_ibfk_1` FOREIGN KEY (`speeldagId`) REFERENCES `Speeldag` (`speeldagId`),
  CONSTRAINT `Aanwezigheid_ibfk_2` FOREIGN KEY (`spelerId`) REFERENCES `Speler` (`spelerId`)
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Aanwezigheid`
--

LOCK TABLES `Aanwezigheid` WRITE;
/*!40000 ALTER TABLE `Aanwezigheid` DISABLE KEYS */;
INSERT INTO `Aanwezigheid` VALUES (1,1,22,1),(2,1,11,2),(3,1,14,3),(4,1,12,4),(5,1,8,5),(6,1,4,6),(7,1,7,7),(8,1,1,8),(9,1,15,9),(10,1,17,10),(11,1,16,11),(12,1,27,12),(13,1,18,13),(14,1,10,14),(15,1,9,15),(16,1,25,16),(17,1,19,17),(18,1,21,18),(19,1,6,19),(20,1,26,20),(21,1,20,21),(22,1,24,22),(23,1,2,23),(26,1,13,24);
/*!40000 ALTER TABLE `Aanwezigheid` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Dagklassement`
--

DROP TABLE IF EXISTS `Dagklassement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Dagklassement` (
  `dagklassementId` int NOT NULL AUTO_INCREMENT,
  `speeldagId` int DEFAULT NULL,
  `spelerId` int DEFAULT NULL,
  `hoofdpunten` int NOT NULL,
  `plus_min_punten` int NOT NULL,
  PRIMARY KEY (`dagklassementId`),
  KEY `speeldagId` (`speeldagId`),
  KEY `spelerId` (`spelerId`),
  CONSTRAINT `Dagklassement_ibfk_1` FOREIGN KEY (`speeldagId`) REFERENCES `Speeldag` (`speeldagId`),
  CONSTRAINT `Dagklassement_ibfk_2` FOREIGN KEY (`spelerId`) REFERENCES `Speler` (`spelerId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Dagklassement`
--

LOCK TABLES `Dagklassement` WRITE;
/*!40000 ALTER TABLE `Dagklassement` DISABLE KEYS */;
/*!40000 ALTER TABLE `Dagklassement` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Seizoen`
--

DROP TABLE IF EXISTS `Seizoen`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Seizoen` (
  `seizoensId` int NOT NULL AUTO_INCREMENT,
  `startdatum` date NOT NULL,
  `einddatum` date NOT NULL,
  PRIMARY KEY (`seizoensId`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Seizoen`
--

LOCK TABLES `Seizoen` WRITE;
/*!40000 ALTER TABLE `Seizoen` DISABLE KEYS */;
INSERT INTO `Seizoen` VALUES (1,'2025-01-01','2025-12-31');
/*!40000 ALTER TABLE `Seizoen` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Speeldag`
--

DROP TABLE IF EXISTS `Speeldag`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Speeldag` (
  `speeldagId` int NOT NULL AUTO_INCREMENT,
  `datum` date NOT NULL,
  `seizoensId` int DEFAULT NULL,
  PRIMARY KEY (`speeldagId`),
  KEY `seizoensId` (`seizoensId`),
  CONSTRAINT `Speeldag_ibfk_1` FOREIGN KEY (`seizoensId`) REFERENCES `Seizoen` (`seizoensId`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Speeldag`
--

LOCK TABLES `Speeldag` WRITE;
/*!40000 ALTER TABLE `Speeldag` DISABLE KEYS */;
INSERT INTO `Speeldag` VALUES (1,'2025-07-01',1);
/*!40000 ALTER TABLE `Speeldag` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Spel`
--

DROP TABLE IF EXISTS `Spel`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Spel` (
  `spelId` int NOT NULL AUTO_INCREMENT,
  `speeldagId` int DEFAULT NULL,
  `terrein` varchar(100) NOT NULL,
  `spelerVolgnr` int NOT NULL,
  `scoreA` int NOT NULL,
  `scoreB` int NOT NULL,
  PRIMARY KEY (`spelId`),
  KEY `speeldagId` (`speeldagId`),
  CONSTRAINT `Spel_ibfk_1` FOREIGN KEY (`speeldagId`) REFERENCES `Speeldag` (`speeldagId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Spel`
--

LOCK TABLES `Spel` WRITE;
/*!40000 ALTER TABLE `Spel` DISABLE KEYS */;
/*!40000 ALTER TABLE `Spel` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Speler`
--

DROP TABLE IF EXISTS `Speler`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Speler` (
  `spelerId` int NOT NULL AUTO_INCREMENT,
  `voornaam` varchar(100) NOT NULL,
  `naam` varchar(100) NOT NULL,
  PRIMARY KEY (`spelerId`)
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Speler`
--

LOCK TABLES `Speler` WRITE;
/*!40000 ALTER TABLE `Speler` DISABLE KEYS */;
INSERT INTO `Speler` (spelerId,naam,voornaam) VALUES (1,'Ardeneus','Jacqueline'),(2,'Adam','Annie'),(3,'Becqaert','JeanMarc'),(4,'Bonnier','Micheline'),(5,'Deblauwe','Dominicque'),(6,'Cotteneye','Pascal'),(7,'Delobel','Luc'),(8,'Decock','Geert'),(9,'Descammps','Ghislaine'),(10,'Deput','Diana'),(11,'Decroi','Nadia'),(12,'Dobbels','Marijke'),(13,'Engelbergh','Noella'),(14,'Ghesqueire','Saseyane'),(15,'Goos','Magda'),(16,'Hugo','Jo'),(17,'Kenes','René'),(18,'Kennes','Josephine'),(19,'Lautrie','Rudy'),(20,'Lammertijn','Marnix'),(21,'Laperre','Marleen'),(22,'Legrand','Leona'),(23,'LeBlon','Bernard'),(24,'Librecht','Ghislain'),(25,'Maes','Marc'),(26,'Mille','Henri'),(27,'Nisen','Jonhy'),(28,'Nuytten','Ghislain'),(29,'Platteau','Jacqueline'),(30,'Platteau','Richard'),(31,'Pacco','Linda'),(32,'Pacco','Lydia'),(33,'Pacco','Rika'),(34,'Peeters','Fleurette'),(35,'Salaets','Ghslain'),(36,'Saelens','Luc'),(37,'Soen','Therese'),(38,'Traen','Monicque'),(39,'Vandaele','Leopold'),(40,'Vandecasteele','Raymond'),(41,'Vanoppilinus','Gilbert'),(42,'Vanheule','Marianne'),(43,'Veldeman','Willy'),(44,'Vermeulen','Jim');
/*!40000 ALTER TABLE `Speler` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Spelverdeling`
--

DROP TABLE IF EXISTS `Spelverdeling`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Spelverdeling` (
  `spelverdelingsId` int NOT NULL AUTO_INCREMENT,
  `spelId` int DEFAULT NULL,
  `team` varchar(50) NOT NULL,
  `spelerPositie` varchar(50) NOT NULL,
  `spelerVolgnr` int NOT NULL,
  `SpelerId` int DEFAULT NULL,
  PRIMARY KEY (`spelverdelingsId`),
  KEY `FK_Spelverdeling_Speler` (`SpelerId`),
  KEY `FK_Spelverdeling_Spel` (`spelId`),
  CONSTRAINT `FK_Spelverdeling_Spel` FOREIGN KEY (`spelId`) REFERENCES `Spel` (`spelId`),
  CONSTRAINT `FK_Spelverdeling_Speler` FOREIGN KEY (`SpelerId`) REFERENCES `Speler` (`spelerId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Spelverdeling`
--

LOCK TABLES `Spelverdeling` WRITE;
/*!40000 ALTER TABLE `Spelverdeling` DISABLE KEYS */;
/*!40000 ALTER TABLE `Spelverdeling` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Temporary view structure for view `vSeizoensklassement`
--

DROP TABLE IF EXISTS `vSeizoensklassement`;
/*!50001 DROP VIEW IF EXISTS `vSeizoensklassement`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `vSeizoensklassement` AS SELECT
 1 AS `seizoensId`,
 1 AS `spelerId`,
 1 AS `spelerNaam`,
 1 AS `spelerVoornaam`,
 1 AS `hoofdpunten`,
 1 AS `plus_min_punten`*/;
SET character_set_client = @saved_cs_client;

--
-- Final view structure for view `vSeizoensklassement`
--

/*!50001 DROP VIEW IF EXISTS `vSeizoensklassement`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vSeizoensklassement` AS select `s`.`seizoensId` AS `seizoensId`,`d`.`spelerId` AS `spelerId`,`s2`.`naam` AS `spelerNaam`,`s2`.`voornaam` AS `spelerVoornaam`,sum(`d`.`hoofdpunten`) AS `hoofdpunten`,sum(`d`.`plus_min_punten`) AS `plus_min_punten` from ((`Dagklassement` `d` join `Speeldag` `s` on((`d`.`speeldagId` = `s`.`speeldagId`))) join `Speler` `s2` on((`d`.`spelerId` = `s2`.`spelerId`))) group by `s`.`seizoensId`,`d`.`spelerId` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-07-01 18:27:42
