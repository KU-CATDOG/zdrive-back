﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ZDrive.Data;

#nullable disable

namespace zdrive_back.Migrations
{
    [DbContext(typeof(ZDriveDbContext))]
    [Migration("20230508035537_EnumConversion")]
    partial class EnumConversion
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

            modelBuilder.Entity("ZDrive.Models.Image", b =>
                {
                    b.Property<string>("ImageSrc")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<int>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ImageSrc");

                    b.HasIndex("ProjectId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("ZDrive.Models.Member", b =>
                {
                    b.Property<string>("StudentNumber")
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.Property<string>("Role")
                        .HasColumnType("TEXT");

                    b.Property<int>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.HasKey("StudentNumber", "Role");

                    b.HasIndex("ProjectId");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("ZDrive.Models.Milestone", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DueDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsFinished")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<int>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Milestones");
                });

            modelBuilder.Entity("ZDrive.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("ZDrive.Models.StudentNum", b =>
                {
                    b.Property<string>("StudentNumber")
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("StudentNumber");

                    b.ToTable("StudentNums");
                });

            modelBuilder.Entity("ZDrive.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Authority")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.Property<string>("StudentNumber")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("StudentNumber")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ZDrive.Models.Image", b =>
                {
                    b.HasOne("ZDrive.Models.Project", "Project")
                        .WithMany("Images")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("ZDrive.Models.Member", b =>
                {
                    b.HasOne("ZDrive.Models.Project", "Project")
                        .WithMany("Members")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("ZDrive.Models.Milestone", b =>
                {
                    b.HasOne("ZDrive.Models.Project", "Project")
                        .WithMany("Milestones")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("ZDrive.Models.StudentNum", b =>
                {
                    b.HasOne("ZDrive.Models.Member", "Member")
                        .WithOne("StudentNum")
                        .HasForeignKey("ZDrive.Models.StudentNum", "StudentNumber")
                        .HasPrincipalKey("ZDrive.Models.Member", "StudentNumber")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Member");
                });

            modelBuilder.Entity("ZDrive.Models.User", b =>
                {
                    b.HasOne("ZDrive.Models.StudentNum", "StudentNum")
                        .WithOne("User")
                        .HasForeignKey("ZDrive.Models.User", "StudentNumber")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("StudentNum");
                });

            modelBuilder.Entity("ZDrive.Models.Member", b =>
                {
                    b.Navigation("StudentNum")
                        .IsRequired();
                });

            modelBuilder.Entity("ZDrive.Models.Project", b =>
                {
                    b.Navigation("Images");

                    b.Navigation("Members");

                    b.Navigation("Milestones");
                });

            modelBuilder.Entity("ZDrive.Models.StudentNum", b =>
                {
                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
