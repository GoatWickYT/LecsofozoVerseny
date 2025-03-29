﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Solution.DataBase;

#nullable disable

namespace Solution.Database.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250320082423_public id")]
    partial class publicid
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("JudgeEntityRaceEntity", b =>
                {
                    b.Property<long>("JudgesId")
                        .HasColumnType("bigint");

                    b.Property<long>("RacesId")
                        .HasColumnType("bigint");

                    b.HasKey("JudgesId", "RacesId");

                    b.HasIndex("RacesId");

                    b.ToTable("JudgeEntityRaceEntity");
                });

            modelBuilder.Entity("RaceEntityTeamEntity", b =>
                {
                    b.Property<long>("RacesId")
                        .HasColumnType("bigint");

                    b.Property<long>("TeamsId")
                        .HasColumnType("bigint");

                    b.HasKey("RacesId", "TeamsId");

                    b.HasIndex("TeamsId");

                    b.ToTable("RaceEntityTeamEntity");
                });

            modelBuilder.Entity("Solution.Database.Entities.CityEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<long>("PostalCode")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("City");
                });

            modelBuilder.Entity("Solution.Database.Entities.JudgeEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(14)
                        .HasColumnType("nvarchar(14)");

                    b.Property<string>("PublicId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WebContentLink")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Judge");
                });

            modelBuilder.Entity("Solution.Database.Entities.LocationEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("HouseNumber")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<long>("PostalCode")
                        .HasColumnType("bigint");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("PostalCode");

                    b.ToTable("Location");
                });

            modelBuilder.Entity("Solution.Database.Entities.ParticipantEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("ImageId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PublicId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("TeamId")
                        .HasColumnType("bigint");

                    b.Property<string>("WebContentLink")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("Participant");
                });

            modelBuilder.Entity("Solution.Database.Entities.PointEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("RaceId")
                        .HasColumnType("bigint");

                    b.Property<long>("TeamId")
                        .HasColumnType("bigint");

                    b.Property<long>("Value")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("RaceId");

                    b.HasIndex("TeamId");

                    b.ToTable("Point");
                });

            modelBuilder.Entity("Solution.Database.Entities.RaceEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<long>("LocationId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PublicId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Race");
                });

            modelBuilder.Entity("Solution.Database.Entities.TeamEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PublicId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Team");
                });

            modelBuilder.Entity("JudgeEntityRaceEntity", b =>
                {
                    b.HasOne("Solution.Database.Entities.JudgeEntity", null)
                        .WithMany()
                        .HasForeignKey("JudgesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Solution.Database.Entities.RaceEntity", null)
                        .WithMany()
                        .HasForeignKey("RacesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RaceEntityTeamEntity", b =>
                {
                    b.HasOne("Solution.Database.Entities.RaceEntity", null)
                        .WithMany()
                        .HasForeignKey("RacesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Solution.Database.Entities.TeamEntity", null)
                        .WithMany()
                        .HasForeignKey("TeamsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Solution.Database.Entities.LocationEntity", b =>
                {
                    b.HasOne("Solution.Database.Entities.CityEntity", "City")
                        .WithMany()
                        .HasForeignKey("PostalCode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("City");
                });

            modelBuilder.Entity("Solution.Database.Entities.ParticipantEntity", b =>
                {
                    b.HasOne("Solution.Database.Entities.TeamEntity", "Team")
                        .WithMany("Participants")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Team");
                });

            modelBuilder.Entity("Solution.Database.Entities.PointEntity", b =>
                {
                    b.HasOne("Solution.Database.Entities.RaceEntity", "Race")
                        .WithMany("Points")
                        .HasForeignKey("RaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Solution.Database.Entities.TeamEntity", "Team")
                        .WithMany("Points")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Race");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("Solution.Database.Entities.RaceEntity", b =>
                {
                    b.Navigation("Points");
                });

            modelBuilder.Entity("Solution.Database.Entities.TeamEntity", b =>
                {
                    b.Navigation("Participants");

                    b.Navigation("Points");
                });
#pragma warning restore 612, 618
        }
    }
}
