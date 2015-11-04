using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using aspnet5_beta8_LOJ_test.Domain;

namespace aspnet5_beta8_LOJ_test.Data
{
    public class ApplicationDbContext : DbContext
    {
		public DbSet<Question> Questions { get; set; }

		public DbSet<ExamInstance> ExamInstances { get; set; }

		public DbSet<ExamInstanceQuestion> ExamInstanceQuestions { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.UseSqlServerIdentityColumns();

			modelBuilder.Entity<ExamInstance>(
				e =>
				{
					e.Property(p => p.Id).HasColumnName("ExamInstanceId");
				}
			);

			modelBuilder.Entity<ExamInstanceQuestion>(
				e =>
				{
					e.Property(p => p.Id).HasColumnName("ExamInstanceQuestionId");
				}
			);

			modelBuilder.Entity<Question>(
				e =>
				{
					e.Property(p => p.Id).HasColumnName("QuestionId");
				}
			);
		}
	}
}
