using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FAP_API.Models;

public partial class Prm392Context : DbContext
{
    public Prm392Context()
    {
    }

    public Prm392Context(DbContextOptions<Prm392Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<ExamSchedule> ExamSchedules { get; set; }

    public virtual DbSet<ExamUser> ExamUsers { get; set; }

    public virtual DbSet<Letter> Letters { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<Slot> Slots { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<SystemKey> SystemKeys { get; set; }

    public virtual DbSet<TimeTable> TimeTables { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserTimeTable> UserTimeTables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=103.142.25.34,1433;Database=PRM392;User Id=conght;Password=hathanhcong;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("class_classid_primary");

            entity.ToTable("Class");

            entity.Property(e => e.ClassId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ClassCode).HasMaxLength(255);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ExamSchedule>(entity =>
        {
            entity.HasKey(e => e.EsId).HasName("examschedule_esid_primary");

            entity.ToTable("ExamSchedule");

            entity.Property(e => e.EsId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Note).HasMaxLength(30);
            entity.Property(e => e.RoomId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Semester).HasMaxLength(30);
            entity.Property(e => e.SlotId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.SubjectId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Room).WithMany(p => p.ExamSchedules)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("examschedule_roomid_foreign");

            entity.HasOne(d => d.Slot).WithMany(p => p.ExamSchedules)
                .HasForeignKey(d => d.SlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("examschedule_slotid_foreign");

            entity.HasOne(d => d.Subject).WithMany(p => p.ExamSchedules)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("examschedule_subjectid_foreign");
        });

        modelBuilder.Entity<ExamUser>(entity =>
        {
            entity.HasKey(e => new { e.EsId, e.UserId }).HasName("examuser_esid_primary");

            entity.ToTable("ExamUser");

            entity.Property(e => e.EsId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Es).WithMany(p => p.ExamUsers)
                .HasForeignKey(d => d.EsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("examuser_esid_foreign");

            entity.HasOne(d => d.User).WithMany(p => p.ExamUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("examuser_userid_foreign");
        });

        modelBuilder.Entity<Letter>(entity =>
        {
            entity.HasKey(e => e.LetterId).HasName("letter_letterid_primary");

            entity.ToTable("Letter");

            entity.Property(e => e.LetterId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CodeSubject)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Reason).HasMaxLength(255);
            entity.Property(e => e.Response).HasMaxLength(250);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.User).WithMany(p => p.Letters)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("letter_userid_foreign");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E1231C3B97F");

            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Content).HasMaxLength(500);
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("room_roomid_primary");

            entity.ToTable("Room");

            entity.Property(e => e.RoomId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Deparment)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Floor)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Number)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.SemesterId).HasName("PK__Semester__043301DDA090C72A");

            entity.ToTable("Semester");

            entity.Property(e => e.SemesterId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.SemesterName).HasMaxLength(255);

            entity.HasMany(d => d.Subjects).WithMany(p => p.Semesters)
                .UsingEntity<Dictionary<string, object>>(
                    "SemesterSubject",
                    r => r.HasOne<Subject>().WithMany()
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_semester_subject2"),
                    l => l.HasOne<Semester>().WithMany()
                        .HasForeignKey("SemesterId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_semester_subject"),
                    j =>
                    {
                        j.HasKey("SemesterId", "SubjectId").HasName("PK__Semester__9EF2BBE7D3038091");
                        j.ToTable("SemesterSubject");
                        j.IndexerProperty<string>("SemesterId")
                            .HasMaxLength(36)
                            .IsUnicode(false)
                            .IsFixedLength();
                        j.IndexerProperty<string>("SubjectId")
                            .HasMaxLength(36)
                            .IsUnicode(false)
                            .IsFixedLength();
                    });
        });

        modelBuilder.Entity<Slot>(entity =>
        {
            entity.HasKey(e => e.SlotId).HasName("slot_slotid_primary");

            entity.ToTable("Slot");

            entity.Property(e => e.SlotId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.SlotName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("subjects_subjectid_primary");

            entity.Property(e => e.SubjectId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.SubjectCode)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.SubjectName).HasMaxLength(255);

            entity.HasMany(d => d.Users).WithMany(p => p.Subjects)
                .UsingEntity<Dictionary<string, object>>(
                    "SubjectUser",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName(" userId_foreign"),
                    l => l.HasOne<Subject>().WithMany()
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("subjectid_foreign"),
                    j =>
                    {
                        j.HasKey("SubjectId", "UserId").HasName("PK__SubjectU__7D632F6C88A3088F");
                        j.ToTable("SubjectUser");
                        j.IndexerProperty<string>("SubjectId")
                            .HasMaxLength(36)
                            .IsUnicode(false)
                            .IsFixedLength();
                        j.IndexerProperty<string>("UserId")
                            .HasMaxLength(36)
                            .IsUnicode(false)
                            .IsFixedLength();
                    });
        });

        modelBuilder.Entity<SystemKey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("systemkey_id_primary");

            entity.ToTable("SystemKey");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CodeKey)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<TimeTable>(entity =>
        {
            entity.HasKey(e => e.TimeTableId).HasName("PK__TimeTabl__C087BD0AB6B67E29");

            entity.ToTable("TimeTable");

            entity.Property(e => e.TimeTableId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ClassId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.RoomId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Semester)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.SlotId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.SubjectId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Class).WithMany(p => p.TimeTables)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("timetable_classid_foreign");

            entity.HasOne(d => d.Room).WithMany(p => p.TimeTables)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("timetable_roomid_foreign");

            entity.HasOne(d => d.SemesterNavigation).WithMany(p => p.TimeTables)
                .HasForeignKey(d => d.Semester)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("timetable_semesterid_foreign");

            entity.HasOne(d => d.Slot).WithMany(p => p.TimeTables)
                .HasForeignKey(d => d.SlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("timetable_slotid_foreign");

            entity.HasOne(d => d.Subject).WithMany(p => p.TimeTables)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("timetable_subjectid_foreign");

            entity.HasOne(d => d.User).WithMany(p => p.TimeTables)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("timetable_userid_foreign");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_userid_primary");

            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.StudentCode)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(255);

            entity.HasMany(d => d.Classes).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UsersClass",
                    r => r.HasOne<Class>().WithMany()
                        .HasForeignKey("ClassId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("classdi_foreign"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("userid_foreign"),
                    j =>
                    {
                        j.HasKey("UserId", "ClassId").HasName("PK__UsersCla__0B395E3017528F0B");
                        j.ToTable("UsersClass");
                        j.IndexerProperty<string>("UserId")
                            .HasMaxLength(36)
                            .IsUnicode(false)
                            .IsFixedLength();
                        j.IndexerProperty<string>("ClassId")
                            .HasMaxLength(36)
                            .IsUnicode(false)
                            .IsFixedLength();
                    });
        });

        modelBuilder.Entity<UserTimeTable>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.TimeTableId }).HasName("PK__UserTime__AB80B79C643A3BAE");

            entity.ToTable("UserTimeTable");

            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TimeTableId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ClassName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RollCallStatus).HasComment("19-P, 20-A, 21-N");
            entity.Property(e => e.SubjectId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.SubjectName)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.TimeTable).WithMany(p => p.UserTimeTables)
                .HasForeignKey(d => d.TimeTableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_timetable");

            entity.HasOne(d => d.User).WithMany(p => p.UserTimeTables)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_userid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
