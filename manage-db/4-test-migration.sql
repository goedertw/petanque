-- MySQL dump 10.13  Distrib 8.4.5, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: petanque
-- ------------------------------------------------------
-- Server version	8.4.5

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
-- Table structure for table `aanwezigheid`
--

DROP TABLE IF EXISTS `aanwezigheid`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aanwezigheid` (
  `aanwezigheidId` int NOT NULL AUTO_INCREMENT,
  `speeldagId` int DEFAULT NULL,
  `spelerId` int DEFAULT NULL,
  `spelerVolgnr` int NOT NULL,
  PRIMARY KEY (`aanwezigheidId`),
  UNIQUE KEY `Aanwezigheid_uk` (`speeldagId`,`spelerVolgnr`),
  KEY `speeldagId` (`speeldagId`),
  KEY `spelerId` (`spelerId`),
  CONSTRAINT `Aanwezigheid_ibfk_1` FOREIGN KEY (`speeldagId`) REFERENCES `speeldag` (`speeldagId`),
  CONSTRAINT `Aanwezigheid_ibfk_2` FOREIGN KEY (`spelerId`) REFERENCES `speler` (`spelerId`)
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aanwezigheid`
--

LOCK TABLES `aanwezigheid` WRITE;
/*!40000 ALTER TABLE `aanwezigheid` DISABLE KEYS */;
INSERT INTO `aanwezigheid` VALUES (3,1,14,3),(4,1,12,4),(7,1,7,7),(8,1,1,8),(9,1,15,9),(11,1,16,11),(12,1,27,12),(13,1,18,13),(16,1,25,16),(17,1,19,17),(20,1,26,20),(23,1,2,23),(27,1,10,24);
/*!40000 ALTER TABLE `aanwezigheid` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `dagklassement`
--

DROP TABLE IF EXISTS `dagklassement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dagklassement` (
  `dagklassementId` int NOT NULL AUTO_INCREMENT,
  `speeldagId` int DEFAULT NULL,
  `spelerId` int DEFAULT NULL,
  `hoofdpunten` int NOT NULL,
  `plus_min_punten` int NOT NULL,
  PRIMARY KEY (`dagklassementId`),
  KEY `speeldagId` (`speeldagId`),
  KEY `spelerId` (`spelerId`),
  CONSTRAINT `Dagklassement_ibfk_1` FOREIGN KEY (`speeldagId`) REFERENCES `speeldag` (`speeldagId`),
  CONSTRAINT `Dagklassement_ibfk_2` FOREIGN KEY (`spelerId`) REFERENCES `speler` (`spelerId`)
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dagklassement`
--

LOCK TABLES `dagklassement` WRITE;
/*!40000 ALTER TABLE `dagklassement` DISABLE KEYS */;
INSERT INTO `dagklassement` VALUES (12,1,12,2,22),(13,1,14,4,17),(14,1,14,3,13),(15,1,12,3,21),(16,1,7,1,-39),(17,1,1,3,13),(18,1,15,2,-5),(19,1,16,3,13),(20,1,27,3,13),(21,1,18,1,-39),(22,1,25,3,13),(23,1,19,3,13),(24,1,26,3,5),(25,1,2,3,13),(26,1,10,3,5);
/*!40000 ALTER TABLE `dagklassement` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `seizoen`
--

DROP TABLE IF EXISTS `seizoen`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `seizoen` (
  `seizoensId` int NOT NULL AUTO_INCREMENT,
  `startdatum` date NOT NULL,
  `einddatum` date NOT NULL,
  PRIMARY KEY (`seizoensId`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `seizoen`
--

LOCK TABLES `seizoen` WRITE;
/*!40000 ALTER TABLE `seizoen` DISABLE KEYS */;
INSERT INTO `seizoen` VALUES (1,'2025-01-01','2025-12-31');
/*!40000 ALTER TABLE `seizoen` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `speeldag`
--

DROP TABLE IF EXISTS `speeldag`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `speeldag` (
  `speeldagId` int NOT NULL AUTO_INCREMENT,
  `datum` date NOT NULL,
  `seizoensId` int DEFAULT NULL,
  PRIMARY KEY (`speeldagId`),
  KEY `seizoensId` (`seizoensId`),
  CONSTRAINT `Speeldag_ibfk_1` FOREIGN KEY (`seizoensId`) REFERENCES `seizoen` (`seizoensId`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `speeldag`
--

LOCK TABLES `speeldag` WRITE;
/*!40000 ALTER TABLE `speeldag` DISABLE KEYS */;
INSERT INTO `speeldag` VALUES (1,'2025-07-01',1);
/*!40000 ALTER TABLE `speeldag` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `spel`
--

DROP TABLE IF EXISTS `spel`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `spel` (
  `spelId` int NOT NULL AUTO_INCREMENT,
  `speeldagId` int DEFAULT NULL,
  `terrein` varchar(100) NOT NULL,
  `spelerVolgnr` int NOT NULL,
  `scoreA` int NOT NULL,
  `scoreB` int NOT NULL,
  PRIMARY KEY (`spelId`),
  KEY `speeldagId` (`speeldagId`),
  CONSTRAINT `Spel_ibfk_1` FOREIGN KEY (`speeldagId`) REFERENCES `speeldag` (`speeldagId`)
) ENGINE=InnoDB AUTO_INCREMENT=47 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `spel`
--

LOCK TABLES `spel` WRITE;
/*!40000 ALTER TABLE `spel` DISABLE KEYS */;
INSERT INTO `spel` VALUES (38,1,'Terrein 1',4,13,0),(39,1,'Terrein 2',12,13,0),(40,1,'Terrein 3',16,13,0),(41,1,'Terrein 1',16,13,0),(42,1,'Terrein 2',23,13,0),(43,1,'Terrein 3',24,13,8),(44,1,'Terrein 1',24,13,0),(45,1,'Terrein 2',4,13,0),(46,1,'Terrein 3',17,13,0);
/*!40000 ALTER TABLE `spel` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `speler`
--

DROP TABLE IF EXISTS `speler`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `speler` (
  `spelerId` int NOT NULL AUTO_INCREMENT,
  `voornaam` varchar(100) NOT NULL,
  `naam` varchar(100) NOT NULL,
  PRIMARY KEY (`spelerId`)
) ENGINE=InnoDB AUTO_INCREMENT=45 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `speler`
--

LOCK TABLES `speler` WRITE;
/*!40000 ALTER TABLE `speler` DISABLE KEYS */;
INSERT INTO `speler` VALUES (1,'Jacqueline','Ardeneus'),(2,'Annie','Adam'),(3,'JeanMarc','Becqaert'),(4,'Micheline','Bonnier'),(5,'Dominicque','Deblauwe'),(6,'Pascal','Cotteneye'),(7,'Luc','Delobel'),(8,'Geert','Decock'),(9,'Ghislaine','Descammps'),(10,'Diana','Deput'),(11,'Nadia','Decroi'),(12,'Marijke','Dobbels'),(13,'Noella','Engelbergh'),(14,'Saseyane','Ghesqueire'),(15,'Magda','Goos'),(16,'Jo','Hugo'),(17,'Ren??','Kenes'),(18,'Josephine','Kennes'),(19,'Rudy','Lautrie'),(20,'Marnix','Lammertijn'),(21,'Marleen','Laperre'),(22,'Leona','Legrand'),(23,'Bernard','LeBlon'),(24,'Ghislain','Librecht'),(25,'Marc','Maes'),(26,'Henri','Mille'),(27,'Jonhy','Nisen'),(28,'Ghislain','Nuytten'),(29,'Jacqueline','Platteau'),(30,'Richard','Platteau'),(31,'Linda','Pacco'),(32,'Lydia','Pacco'),(33,'Rika','Pacco'),(34,'Fleurette','Peeters'),(35,'Ghslain','Salaets'),(36,'Luc','Saelens'),(37,'Therese','Soen'),(38,'Monicque','Traen'),(39,'Leopold','Vandaele'),(40,'Raymond','Vandecasteele'),(41,'Gilbert','Vanoppilinus'),(42,'Marianne','Vanheule'),(43,'Willy','Veldeman'),(44,'Jim','Vermeulen');
/*!40000 ALTER TABLE `speler` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `spelverdeling`
--

DROP TABLE IF EXISTS `spelverdeling`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `spelverdeling` (
  `spelverdelingsId` int NOT NULL AUTO_INCREMENT,
  `spelId` int DEFAULT NULL,
  `team` varchar(50) NOT NULL,
  `spelerPositie` varchar(50) NOT NULL,
  `spelerVolgnr` int NOT NULL,
  `SpelerId` int DEFAULT NULL,
  PRIMARY KEY (`spelverdelingsId`),
  KEY `FK_Spelverdeling_Speler` (`SpelerId`),
  KEY `FK_Spelverdeling_Spel` (`spelId`),
  CONSTRAINT `FK_Spelverdeling_Spel` FOREIGN KEY (`spelId`) REFERENCES `spel` (`spelId`),
  CONSTRAINT `FK_Spelverdeling_Speler` FOREIGN KEY (`SpelerId`) REFERENCES `speler` (`spelerId`)
) ENGINE=InnoDB AUTO_INCREMENT=184 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `spelverdeling`
--

LOCK TABLES `spelverdeling` WRITE;
/*!40000 ALTER TABLE `spelverdeling` DISABLE KEYS */;
INSERT INTO `spelverdeling` VALUES (145,38,'Team A','P1',4,4),(146,38,'Team A','P2',9,9),(147,38,'Team A','P3',20,20),(148,38,'Team B','P1',11,11),(149,38,'Team B','P2',3,3),(150,39,'Team A','P1',12,12),(151,39,'Team A','P2',17,17),(152,39,'Team B','P1',24,24),(153,39,'Team B','P2',13,13),(154,40,'Team A','P1',16,16),(155,40,'Team A','P2',8,8),(156,40,'Team B','P1',23,23),(157,40,'Team B','P2',7,7),(158,41,'Team A','P1',16,16),(159,41,'Team A','P2',12,12),(160,41,'Team A','P3',11,11),(161,41,'Team B','P1',17,17),(162,41,'Team B','P2',8,8),(163,42,'Team A','P1',23,23),(164,42,'Team A','P2',3,3),(165,42,'Team B','P1',13,13),(166,42,'Team B','P2',7,7),(167,43,'Team A','P1',24,24),(168,43,'Team A','P2',20,20),(169,43,'Team B','P1',9,9),(170,43,'Team B','P2',4,4),(171,44,'Team A','P1',24,24),(172,44,'Team A','P2',23,23),(173,44,'Team A','P3',11,11),(174,44,'Team B','P1',7,7),(175,44,'Team B','P2',20,20),(176,45,'Team A','P1',4,4),(177,45,'Team A','P2',8,8),(178,45,'Team B','P1',13,13),(179,45,'Team B','P2',9,9),(180,46,'Team A','P1',17,17),(181,46,'Team A','P2',3,3),(182,46,'Team B','P1',16,16),(183,46,'Team B','P2',12,12);
/*!40000 ALTER TABLE `spelverdeling` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Seizoensklassement`
--
DROP TABLE IF EXISTS `Seizoensklassement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Seizoensklassement` (
  `seizoensklassementId` int NOT NULL AUTO_INCREMENT,
  `spelerId` int DEFAULT NULL,
  `seizoensId` int DEFAULT NULL,
  `hoofdpunten` int NOT NULL,
  `plus_min_punten` int NOT NULL,
  PRIMARY KEY (`seizoensklassementId`),
  KEY `spelerId` (`spelerId`),
  KEY `seizoensId` (`seizoensId`),
  CONSTRAINT `Seizoensklassement_ibfk_1` FOREIGN KEY (`spelerId`) REFERENCES `Speler` (`spelerId`),
  CONSTRAINT `Seizoensklassement_ibfk_2` FOREIGN KEY (`seizoensId`) REFERENCES `Seizoen` (`seizoensId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-07-31  1:44:36
