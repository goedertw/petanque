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
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

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
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

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
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

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
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

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
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

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
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

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
