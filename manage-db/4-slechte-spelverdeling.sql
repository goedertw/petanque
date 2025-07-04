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
  KEY `speeldagId` (`speeldagId`),
  KEY `spelerId` (`spelerId`),
  CONSTRAINT `Aanwezigheid_ibfk_1` FOREIGN KEY (`speeldagId`) REFERENCES `speeldag` (`speeldagId`),
  CONSTRAINT `Aanwezigheid_ibfk_2` FOREIGN KEY (`spelerId`) REFERENCES `speler` (`spelerId`)
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aanwezigheid`
--

LOCK TABLES `aanwezigheid` WRITE;
/*!40000 ALTER TABLE `aanwezigheid` DISABLE KEYS */;
INSERT INTO `aanwezigheid` VALUES (1,1,22,1),(2,1,11,2),(3,1,14,3),(4,1,12,4),(5,1,8,5),(6,1,4,6),(7,1,7,7),(8,1,1,8),(9,1,15,9),(10,1,17,10),(11,1,16,11),(12,1,27,12),(13,1,18,13),(14,1,10,14),(15,1,9,15),(16,1,25,16),(17,1,19,17),(18,1,21,18),(19,1,6,19),(20,1,26,20),(21,1,20,21),(22,1,24,22),(23,1,2,23),(26,1,13,24);
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dagklassement`
--

LOCK TABLES `dagklassement` WRITE;
/*!40000 ALTER TABLE `dagklassement` DISABLE KEYS */;
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
-- Table structure for table `seizoensklassement`
--

DROP TABLE IF EXISTS `seizoensklassement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `seizoensklassement` (
  `seizoensklassementId` int NOT NULL AUTO_INCREMENT,
  `spelerId` int DEFAULT NULL,
  `seizoensId` int DEFAULT NULL,
  `hoofdpunten` int NOT NULL,
  `plus_min_punten` int NOT NULL,
  PRIMARY KEY (`seizoensklassementId`),
  KEY `spelerId` (`spelerId`),
  KEY `seizoensId` (`seizoensId`),
  CONSTRAINT `Seizoensklassement_ibfk_1` FOREIGN KEY (`spelerId`) REFERENCES `speler` (`spelerId`),
  CONSTRAINT `Seizoensklassement_ibfk_2` FOREIGN KEY (`seizoensId`) REFERENCES `seizoen` (`seizoensId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `seizoensklassement`
--

LOCK TABLES `seizoensklassement` WRITE;
/*!40000 ALTER TABLE `seizoensklassement` DISABLE KEYS */;
/*!40000 ALTER TABLE `seizoensklassement` ENABLE KEYS */;
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
) ENGINE=InnoDB AUTO_INCREMENT=38 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `spel`
--

LOCK TABLES `spel` WRITE;
/*!40000 ALTER TABLE `spel` DISABLE KEYS */;
INSERT INTO `spel` VALUES (19,1,'Terrein 1',13,0,0),(20,1,'Terrein 2',17,0,0),(21,1,'Terrein 3',16,0,0),(22,1,'Terrein 4',23,0,0),(23,1,'Terrein 5',4,0,0),(24,1,'Terrein 1',9,0,0),(25,1,'Terrein 2',8,0,0),(26,1,'Terrein 3',19,0,0),(27,1,'Terrein 4',24,0,0),(28,1,'Terrein 5',18,0,0),(29,1,'Terrein 1',7,0,0),(30,1,'Terrein 2',21,0,0),(31,1,'Terrein 3',20,0,0),(32,1,'Terrein 4',6,0,0),(33,1,'Terrein 5',11,0,0),(34,1,'Terrein 1',4,0,0),(35,1,'Terrein 2',7,0,0),(36,1,'Terrein 3',11,0,0),(37,1,'Terrein 1',15,0,0);
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
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `speler`
--

LOCK TABLES `speler` WRITE;
/*!40000 ALTER TABLE `speler` DISABLE KEYS */;
INSERT INTO `speler` VALUES (1,'Jan','Jansen'),(2,'Pieter','Pietersen'),(3,'Sophie','Vermeulen'),(4,'Emma','De Vries'),(5,'Seba','Achternaam'),(6,'Nicolas','Nikolaas'),(7,'Friet','Mayonaise'),(8,'Bob','Hanseeeeeeeeeenss'),(9,'Marilou','Vansteeberge'),(10,'Lentel','Opsomer'),(11,'Artjom','zijn zus'),(12,'Bakker','Sam'),(13,'Stephanie','Degrote'),(14,'Artjom','Van de Velde'),(15,'Jarne','Vercruysse'),(16,'Jorden','Rommens'),(17,'jelle','vd'),(18,'kesti','vos'),(19,'mika','dehaese'),(20,'Noah','B'),(21,'Nathan','Heeeeeeensens'),(22,'Anne','DM'),(23,'Test','Test'),(24,'oma','christine'),(25,'meneer','goedertier'),(26,'Nieuwe','Naam'),(27,'kasper','krapes');
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
) ENGINE=InnoDB AUTO_INCREMENT=145 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `spelverdeling`
--

LOCK TABLES `spelverdeling` WRITE;
/*!40000 ALTER TABLE `spelverdeling` DISABLE KEYS */;
INSERT INTO `spelverdeling` VALUES (73,19,'Team A','P1',13,13),(74,19,'Team A','P2',21,21),(75,19,'Team B','P1',5,5),(76,19,'Team B','P2',9,9),(77,20,'Team A','P1',17,17),(78,20,'Team A','P2',1,1),(79,20,'Team B','P1',24,24),(80,20,'Team B','P2',18,18),(81,21,'Team A','P1',16,16),(82,21,'Team A','P2',6,6),(83,21,'Team B','P1',12,12),(84,21,'Team B','P2',7,7),(85,22,'Team A','P1',23,23),(86,22,'Team A','P2',3,3),(87,22,'Team B','P1',20,20),(88,22,'Team B','P2',22,22),(89,23,'Team A','P1',4,4),(90,23,'Team A','P2',10,10),(91,23,'Team B','P1',14,14),(92,23,'Team B','P2',19,19),(93,24,'Team A','P1',9,9),(94,24,'Team A','P2',3,3),(95,24,'Team B','P1',17,17),(96,24,'Team B','P2',6,6),(97,25,'Team A','P1',8,8),(98,25,'Team A','P2',16,16),(99,25,'Team B','P1',12,12),(100,25,'Team B','P2',13,13),(101,26,'Team A','P1',19,19),(102,26,'Team A','P2',5,5),(103,26,'Team B','P1',14,14),(104,26,'Team B','P2',10,10),(105,27,'Team A','P1',24,24),(106,27,'Team A','P2',21,21),(107,27,'Team B','P1',1,1),(108,27,'Team B','P2',11,11),(109,28,'Team A','P1',18,18),(110,28,'Team A','P2',22,22),(111,28,'Team B','P1',23,23),(112,28,'Team B','P2',20,20),(113,29,'Team A','P1',7,7),(114,29,'Team A','P2',4,4),(115,29,'Team B','P1',9,9),(116,29,'Team B','P2',12,12),(117,30,'Team A','P1',21,21),(118,30,'Team A','P2',19,19),(119,30,'Team B','P1',2,2),(120,30,'Team B','P2',3,3),(121,31,'Team A','P1',20,20),(122,31,'Team A','P2',17,17),(123,31,'Team B','P1',24,24),(124,31,'Team B','P2',8,8),(125,32,'Team A','P1',6,6),(126,32,'Team A','P2',5,5),(127,32,'Team B','P1',1,1),(128,32,'Team B','P2',18,18),(129,33,'Team A','P1',11,11),(130,33,'Team A','P2',13,13),(131,33,'Team B','P1',15,15),(132,33,'Team B','P2',10,10),(133,34,'Team A','P1',4,4),(134,34,'Team A','P2',16,16),(135,34,'Team B','P1',14,14),(136,34,'Team B','P2',8,8),(137,35,'Team A','P1',7,7),(138,35,'Team A','P2',23,23),(139,35,'Team B','P1',15,15),(140,35,'Team B','P2',2,2),(141,36,'Team A','P1',11,11),(142,36,'Team B','P1',22,22),(143,37,'Team A','P1',15,15),(144,37,'Team B','P1',2,2);
/*!40000 ALTER TABLE `spelverdeling` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-07-01 18:30:06
