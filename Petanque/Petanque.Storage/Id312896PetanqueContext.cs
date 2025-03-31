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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=ID312896_petanque.db.webhosting.be;port=3306;database=ID312896_petanque;user=ID312896_petanque;password=hDQf_F!KB2b9", Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.44-mysql"));

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
            entity.HasKey(e => e.SeizoensklassementId).HasName("PRIMARY");

            entity.ToTable("Seizoensklassement");

            entity.HasIndex(e => e.SeizoensId, "seizoensId");

            entity.HasIndex(e => e.SpelerId, "spelerId");

            entity.Property(e => e.SeizoensklassementId)
                .HasColumnType("int(11)")
                .HasColumnName("seizoensklassementId");
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

            entity.HasOne(d => d.Seizoens).WithMany(p => p.Seizoensklassements)
                .HasForeignKey(d => d.SeizoensId)
                .HasConstraintName("Seizoensklassement_ibfk_2");

            entity.HasOne(d => d.Speler).WithMany(p => p.Seizoensklassements)
                .HasForeignKey(d => d.SpelerId)
                .HasConstraintName("Seizoensklassement_ibfk_1");
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

            entity.Property(e => e.SpelverdelingsId)
                .HasColumnType("int(11)")
                .HasColumnName("spelverdelingsId");
            entity.Property(e => e.SpelId)
                .HasColumnType("int(11)")
                .HasColumnName("spelId");
            entity.Property(e => e.SpelerPositie)
                .HasMaxLength(50)
                .HasColumnName("spelerPositie");
            entity.Property(e => e.SpelerVolgnr)
                .HasColumnType("int(11)")
                .HasColumnName("spelerVolgnr");
            entity.Property(e => e.Team)
                .HasMaxLength(50)
                .HasColumnName("team");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
