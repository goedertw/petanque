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
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

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
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

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
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

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
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

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
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

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
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

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
INSERT INTO `Speler` VALUES (1,'Jan','Jansen'),(2,'Pieter','Pietersen'),(3,'Sophie','Vermeulen'),(4,'Emma','De Vries'),(5,'Seba','Achternaam'),(6,'Nicolas','Nikolaas'),(7,'Friet','Mayonaise'),(8,'Bob','Hanseeeeeeeeeenss'),(9,'Marilou','Vansteeberge'),(10,'Lentel','Opsomer'),(11,'Artjom','zijn zus'),(12,'Bakker','Sam'),(13,'Stephanie','Degrote'),(14,'Artjom','Van de Velde'),(15,'Jarne','Vercruysse'),(16,'Jorden','Rommens'),(17,'jelle','vd'),(18,'kesti','vos'),(19,'mika','dehaese'),(20,'Noah','B'),(21,'Nathan','Heeeeeeensens'),(22,'Anne','DM'),(23,'Test','Test'),(24,'oma','christine'),(25,'meneer','goedertier'),(26,'Nieuwe','Naam'),(27,'kasper','krapes');
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
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-07-01 11:58:27
