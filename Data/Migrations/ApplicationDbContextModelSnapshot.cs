﻿// <auto-generated />
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0-rc.2.23480.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Data.Entities.Discord.DiscordGuild", b =>
                {
                    b.Property<ulong>("Id")
                        .HasColumnType("numeric(20,0)");

                    b.Property<bool>("InlineTagsEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("TagPrefix")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)")
                        .HasDefaultValue("$");

                    b.HasKey("Id");

                    b.ToTable("Guilds");
                });

            modelBuilder.Entity("Data.Entities.Discord.DiscordUser", b =>
                {
                    b.Property<ulong>("Id")
                        .HasColumnType("numeric(20,0)");

                    b.Property<byte>("Access")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Data.Entities.Tags.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("character varying(13)");

                    b.Property<ulong?>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<ulong?>("OwnerId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("GuildId", "Name");

                    b.ToTable("Tags");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Tag");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Data.Entities.Tags.AliasTag", b =>
                {
                    b.HasBaseType("Data.Entities.Tags.Tag");

                    b.Property<int>("ReferencedTagId")
                        .HasColumnType("integer");

                    b.HasIndex("ReferencedTagId");

                    b.HasDiscriminator().HasValue("AliasTag");
                });

            modelBuilder.Entity("Data.Entities.Tags.MessageTag", b =>
                {
                    b.HasBaseType("Data.Entities.Tags.Tag");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.HasDiscriminator().HasValue("MessageTag");
                });

            modelBuilder.Entity("Data.Entities.Tags.Tag", b =>
                {
                    b.HasOne("Data.Entities.Discord.DiscordGuild", "Guild")
                        .WithMany("Tags")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Data.Entities.Discord.DiscordUser", "Owner")
                        .WithMany("Tags")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Guild");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Data.Entities.Tags.AliasTag", b =>
                {
                    b.HasOne("Data.Entities.Tags.Tag", "ReferencedTag")
                        .WithMany()
                        .HasForeignKey("ReferencedTagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ReferencedTag");
                });

            modelBuilder.Entity("Data.Entities.Discord.DiscordGuild", b =>
                {
                    b.Navigation("Tags");
                });

            modelBuilder.Entity("Data.Entities.Discord.DiscordUser", b =>
                {
                    b.Navigation("Tags");
                });
#pragma warning restore 612, 618
        }
    }
}
