using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Petanque.Storage;

public partial class Id312896PetanqueContext : DbContext
{
    public Id312896PetanqueContext()
    {
    }

    public Id312896PetanqueContext(DbContextOptions<Id312896PetanqueContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aanwezigheid> Aanwezigheids { get; set; }

    public virtual DbSet<Dagklassement> Dagklassements { get; set; }

    public virtual DbSet<Seizoen> Seizoens { get; set; }

    public virtual DbSet<Seizoensklassement> Seizoensklassements { get; set; }

    public virtual DbSet<Speeldag> Speeldags { get; set; }

    public virtual DbSet<Spel> Spels { get; set; }

    public virtual DbSet<Speler> Spelers { get; set; }

    public virtual DbSet<Spelverdeling> Spelverdelings { get; set; }

    protected string _connString = "server=127.0.0.1;port=3306;database=petanque;user=root;password=root";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(_connString, Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect(_connString));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8_general_ci")
            .HasCharSet("utf8");

        modelBuilder.Entity<Aanwezigheid>(entity =>
        {
            entity.HasKey(e => e.AanwezigheidId).HasName("PRIMARY");

            entity.ToTable("Aanwezigheid");

            entity.HasIndex(e => e.SpeeldagId, "speeldagId");

            entity.HasIndex(e => e.SpelerId, "spelerId");

            entity.Property(e => e.AanwezigheidId)
                .HasColumnType("int(11)")
                .HasColumnName("aanwezigheidId");
            entity.Property(e => e.SpeeldagId)
                .HasColumnType("int(11)")
                .HasColumnName("speeldagId");
            entity.Property(e => e.SpelerId)
                .HasColumnType("int(11)")
                .HasColumnName("spelerId");
            entity.Property(e => e.SpelerVolgnr)
                .HasColumnType("int(11)")
                .HasColumnName("spelerVolgnr");

            entity.HasOne(d => d.Speeldag).WithMany(p => p.Aanwezigheids)
                .HasForeignKey(d => d.SpeeldagId)
                .HasConstraintName("Aanwezigheid_ibfk_1");

            entity.HasOne(d => d.Speler).WithMany(p => p.Aanwezigheids)
                .HasForeignKey(d => d.SpelerId)
                .HasConstraintName("Aanwezigheid_ibfk_2");
        });

        modelBuilder.Entity<Dagklassement>(entity =>
        {
            entity.HasKey(e => e.DagklassementId).HasName("PRIMARY");

            entity.ToTable("Dagklassement");

            entity.HasIndex(e => e.SpeeldagId, "speeldagId");

            entity.HasIndex(e => e.SpelerId, "spelerId");

            entity.Property(e => e.DagklassementId)
                .HasColumnType("int(11)")
                .HasColumnName("dagklassementId");
            entity.Property(e => e.Hoofdpunten)
                .HasColumnType("int(11)")
                .HasColumnName("hoofdpunten");
            entity.Property(e => e.PlusMinPunten)
                .HasColumnType("int(11)")
                .HasColumnName("plus_min_punten");
            entity.Property(e => e.SpeeldagId)
                .HasColumnType("int(11)")
                .HasColumnName("speeldagId");
            entity.Property(e => e.SpelerId)
                .HasColumnType("int(11)")
                .HasColumnName("spelerId");

            entity.HasOne(d => d.Speeldag).WithMany(p => p.Dagklassements)
                .HasForeignKey(d => d.SpeeldagId)
                .HasConstraintName("Dagklassement_ibfk_1");

            entity.HasOne(d => d.Speler).WithMany(p => p.Dagklassements)
                .HasForeignKey(d => d.SpelerId)
                .HasConstraintName("Dagklassement_ibfk_2");
        });

        modelBuilder.Entity<Seizoen>(entity =>
        {
            entity.HasKey(e => e.SeizoensId).HasName("PRIMARY");

            entity.ToTable("Seizoen");

            entity.Property(e => e.SeizoensId)
                .HasColumnType("int(11)")
                .HasColumnName("seizoensId");
            entity.Property(e => e.Einddatum).HasColumnName("einddatum");
            entity.Property(e => e.Startdatum).HasColumnName("startdatum");
        });

        modelBuilder.Entity<Seizoensklassement>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vSeizoensklassement");
            entity.Property(e => e.Hoofdpunten)
                .HasColumnType("int(11)")
                .HasColumnName("hoofdpunten");
            entity.Property(e => e.PlusMinPunten)
                .HasColumnType("int(11)")
                .HasColumnName("plus_min_punten");
            entity.Property(e => e.SeizoensId)
                .HasColumnType("int(11)")
                .HasColumnName("seizoensId");
            entity.Property(e => e.SpelerId)
                .HasColumnType("int(11)")
                .HasColumnName("spelerId");
            entity.Property(e => e.SpelerNaam)
                .HasMaxLength(100)
                .HasColumnName("spelerNaam");
            entity.Property(e => e.SpelerVoornaam)
                .HasMaxLength(100)
                .HasColumnName("spelerVoornaam");
        });

        modelBuilder.Entity<Speeldag>(entity =>
        {
            entity.HasKey(e => e.SpeeldagId).HasName("PRIMARY");

            entity.ToTable("Speeldag");

            entity.HasIndex(e => e.SeizoensId, "seizoensId");

            entity.Property(e => e.SpeeldagId)
                .HasColumnType("int(11)")
                .HasColumnName("speeldagId");
            entity.Property(e => e.Datum).HasColumnName("datum");
            entity.Property(e => e.SeizoensId)
                .HasColumnType("int(11)")
                .HasColumnName("seizoensId");

            entity.HasOne(d => d.Seizoens).WithMany(p => p.Speeldags)
                .HasForeignKey(d => d.SeizoensId)
                .HasConstraintName("Speeldag_ibfk_1");
        });

        modelBuilder.Entity<Spel>(entity =>
        {
            entity.HasKey(e => e.SpelId).HasName("PRIMARY");

            entity.ToTable("Spel");

            entity.HasIndex(e => e.SpeeldagId, "speeldagId");

            entity.Property(e => e.SpelId)
                .HasColumnType("int(11)")
                .HasColumnName("spelId");
            entity.Property(e => e.ScoreA)
                .HasColumnType("int(11)")
                .HasColumnName("scoreA");
            entity.Property(e => e.ScoreB)
                .HasColumnType("int(11)")
                .HasColumnName("scoreB");
            entity.Property(e => e.SpeeldagId)
                .HasColumnType("int(11)")
                .HasColumnName("speeldagId");
            entity.Property(e => e.SpelerVolgnr)
                .HasColumnType("int(11)")
                .HasColumnName("spelerVolgnr");
            entity.Property(e => e.Terrein)
                .HasMaxLength(100)
                .HasColumnName("terrein");

            entity.HasOne(d => d.Speeldag).WithMany(p => p.Spels)
                .HasForeignKey(d => d.SpeeldagId)
                .HasConstraintName("Spel_ibfk_1");
        });

        modelBuilder.Entity<Speler>(entity =>
        {
            entity.HasKey(e => e.SpelerId).HasName("PRIMARY");

            entity.ToTable("Speler");

            entity.Property(e => e.SpelerId)
                .HasColumnType("int(11)")
                .HasColumnName("spelerId");
            entity.Property(e => e.Naam)
                .HasMaxLength(100)
                .HasColumnName("naam");
            entity.Property(e => e.Voornaam)
                .HasMaxLength(100)
                .HasColumnName("voornaam");
        });

        modelBuilder.Entity<Spelverdeling>(entity =>
        {
            entity.HasKey(e => e.SpelverdelingsId).HasName("PRIMARY");

            entity.ToTable("Spelverdeling");

            entity.HasIndex(e => e.SpelId, "FK_Spelverdeling_Spel");

            entity.HasIndex(e => e.SpelerId, "FK_Spelverdeling_Speler");

            entity.Property(e => e.SpelverdelingsId)
                .HasColumnType("int(11)")
                .HasColumnName("spelverdelingsId");
            entity.Property(e => e.SpelId)
                .HasColumnType("int(11)")
                .HasColumnName("spelId");
            entity.Property(e => e.SpelerId).HasColumnType("int(11)");
            entity.Property(e => e.SpelerPositie)
                .HasMaxLength(50)
                .HasColumnName("spelerPositie");
            entity.Property(e => e.SpelerVolgnr)
                .HasColumnType("int(11)")
                .HasColumnName("spelerVolgnr");
            entity.Property(e => e.Team)
                .HasMaxLength(50)
                .HasColumnName("team");

            entity.HasOne(d => d.Spel).WithMany(p => p.Spelverdelings)
                .HasForeignKey(d => d.SpelId)
                .HasConstraintName("FK_Spelverdeling_Spel");

            entity.HasOne(d => d.Speler).WithMany(p => p.Spelverdelings)
                .HasForeignKey(d => d.SpelerId)
                .HasConstraintName("FK_Spelverdeling_Speler");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    protected static void ErrorAndExit(string msg1,string msg2)
    {
        Console.WriteLine($@"
*********
* ERROR *
*********
{msg1}

{msg2}

Druk een toets om de backend af te sluiten...
");
        Console.ReadKey(true);
        Environment.Exit(1);
    }
    public void TestConnection()
    {
        try
        {
            this.Database.ExecuteSqlRaw("SHOW DATABASES;");
        }
        catch (MySqlConnector.MySqlException ex)
        {
            ErrorAndExit(ex.Message, $"Zorg voor een MySQL-database met volgende connectie-details:\n{_connString}");
        }

        int cnt = this.Database.SqlQuery<string>($"SHOW TABLES").ToList().Count;
        if (cnt == 0)
        {
            ErrorAndExit("Geen enkele tabel gevonden", "Initialiseer de Databank met bvb: cd manage-db; .\\restore.ps1 0-empty-db.sql mysql.cfg.local");
        }
        else if (cnt != 8)
        {
            ErrorAndExit($"{cnt} i.p.v. 8 tabellen gevonden", "Neem een backup van je Databank en probeer manueel te fixen!");
        }
    }
    public void Migration1()
    {
        int cnt = this.Database.SqlQuery<string>($"SHOW FULL TABLES LIKE 'vSeizoensklassement';").ToList().Count;
        if (cnt == 1) return; // Migration1 already done
        if (cnt != 0) ErrorAndExit($"Onverwacht aantal voor tabel 'vSeizoensklassement' ({cnt})","Database lijkt corrupt.");

        // Migration: Stap 1: verwijder dubbels uit dagklassement
        Console.WriteLine("Dubbels in 'dagklassement' worden opgezocht");
        var badIds = this.Database.SqlQuery<Int32>($"SELECT d1.dagklassementId FROM dagklassement d1 LEFT JOIN (SELECT MAX(dagklassementId) AS maxId FROM dagklassement GROUP BY speeldagId, spelerId) d2 ON d1.dagklassementId = d2.maxId WHERE d2.maxId IS NULL;").ToList();
        if (badIds.Count != 0)
        {
            Console.WriteLine($"Er worden {badIds.Count} dubbels verwijderd");
            string query = "DELETE FROM dagklassement WHERE dagklassementId IN (" + string.Join(",", badIds) + ");";
            cnt = this.Database.ExecuteSqlRaw(query);
            if (cnt != badIds.Count)
            {
                ErrorAndExit($"Probleem met '{query}'", $"Er zijn {cnt} ipv {badIds.Count} IDs verwijderd");
            }
        }

        // Migration: Stap 2: add VIEW vSeizoensklassement
        Console.WriteLine("VIEW vSeizoensklassement wordt aangemaakt");
        this.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS `Seizoensklassement`;");
        this.Database.ExecuteSqlRaw("CREATE OR REPLACE VIEW `vSeizoensklassement` AS " +
            "SELECT s.seizoensId,d.spelerId,s2.naam AS spelerNaam,s2.voornaam AS spelerVoornaam," +
            "       sum(d.hoofdpunten) AS hoofdpunten,sum(d.plus_min_punten) AS plus_min_punten " +
            "FROM dagklassement d JOIN speeldag s ON d.speeldagId=s.speeldagId JOIN speler s2 ON d.spelerId = s2.spelerId " +
            "GROUP BY s.seizoensId,d.spelerId;");
    }
}
