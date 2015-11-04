using aspnet5_beta8_LOJ_test.Data;
using aspnet5_beta8_LOJ_test.Domain;
using Microsoft.Data.Entity;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LOJ_Tests
{
    public abstract class _DbContextServiceTestBase
    {
		private readonly IServiceProvider _serviceProvider;
		private readonly ApplicationDbContext _dbContext;

		public _DbContextServiceTestBase(Func<IServiceCollection, IServiceCollection> addServices)
		{
			IServiceCollection services = new ServiceCollection();

			services = addServices(services);

			if (services == null)
			{
				services = new ServiceCollection();
			}

			services.AddEntityFramework()
				.AddInMemoryDatabase()
				.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase());

			_serviceProvider = services.BuildServiceProvider();

			_dbContext = _serviceProvider.GetRequiredService<ApplicationDbContext>();
		}

		public ApplicationDbContext DbContext
		{
			get
			{
				return _dbContext;
			}
		}

		public IServiceProvider ServiceProvider
		{
			get
			{
				return _serviceProvider;
			}
		}

		protected void Detach(object entity)
		{
			this.DbContext.Entry(entity).State = EntityState.Detached;
		}

		protected void Detach<T>(T entity) where T : class
		{
			this.DbContext.Entry(entity).State = EntityState.Detached;
		}

		protected void DetachRange(IEnumerable<object> entities)
		{
			foreach (object entity in entities)
			{
				this.Detach(entity);
			}
		}

		protected void DetachRange<T>(IEnumerable<T> entities) where T : class
		{
			foreach (T entity in entities)
			{
				this.Detach(entity);
			}
		}

		#region Global / Common Test Data Setup

		protected void Setup_ExamInstance_Full_Stack_Test_Data(DateTime statsThresholdDate, bool detachObjects = false)
		{
			this.Setup_ExamInstance_Test_Data_Questions(detachObjects);
			this.Setup_ExamInstance_Test_Data_ExamInstances(detachObjects);
			this.Setup_ExamInstance_Test_Data_ExamInstanceQuestions(statsThresholdDate, detachObjects);
		}

		private void Setup_ExamInstance_Test_Data_Questions(bool detachObjects)
		{
			var rootQuestions = new List<Question>();

			for (int i = 1; i <= 1000; i++)
			{
				var q = new Question() { Text = "q" + i.ToString(), Version = DateTime.UtcNow, IsActive = true };
				q.OrderedAnswers = i % 3 == 0;	// every 3rd question has ordered answers
				rootQuestions.Add(q);
			}

			this.DbContext.AddRange(rootQuestions);
			this.DbContext.SaveChanges();

			if (detachObjects)
			{
				this.DetachRange(rootQuestions);
			}

			Thread.Sleep(1);

			var versionedQuestions = new List<Question>(rootQuestions.Count / 2 + 1);

			for (int i = 0; i < rootQuestions.Count; i++)
			{
				// create versioned questions - all odd numbered questions have versions
				if (i % 2 > 0)
				{
					versionedQuestions.Add(
						new Question()
						{
							RootQuestionId = rootQuestions[i].Id,
							Text = rootQuestions[i].Text + "v2",
							Version = DateTime.UtcNow,
							IsActive = true
						}
					);
				}
			}

			this.DbContext.AddRange(versionedQuestions);
			this.DbContext.SaveChanges();

			if (detachObjects)
			{
				this.DetachRange(versionedQuestions);
			}
		}

		private void Setup_ExamInstance_Test_Data_ExamInstances(bool detachObjects)
		{
			var instances = new ExamInstance[]
			{
				// student 1 templates (3, 4, 9, 10) - no instances of template 10
				new ExamInstance() { StudentId = 1, ExamTemplateId = 3 },
				new ExamInstance() { StudentId = 1, ExamTemplateId = 4 },
				new ExamInstance() { StudentId = 1, ExamTemplateId = 9 },

				// student 2 templates (5, 6, 11, 12) - no instances of template 12
				new ExamInstance() { StudentId = 2, ExamTemplateId = 5 },
				new ExamInstance() { StudentId = 2, ExamTemplateId = 6 },
				new ExamInstance() { StudentId = 2, ExamTemplateId = 11 },
			};

			this.DbContext.AddRange(instances);
			this.DbContext.SaveChanges();

			if (detachObjects)
			{
				this.DetachRange(instances);
			}
		}

		private void Setup_ExamInstance_Test_Data_ExamInstanceQuestions(DateTime statsThresholdDate, bool detachObjects)
		{
			/*
			questions 41 - 1000 belong to topics
			questions 1 - 1000 are root questions (odd numbered questions have one additional version)
			questions 1001 - 1500 are versioned questions (based on odd numbered root questions)

			root questions divisible by 3 have ordered answers (reverse order)
			the first answer is the correct answer (last for ordered answers)
			*/

			// questions   21 -   75 => answered correctly before threshold		(exam 1)
			// questions   76 -  143 => answered correctly after threshold		(exam 1)
			// questions  144 -  200 => answered incorrectly before threshold	(exam 1)
			// questions  201 -  265 => answered incorrectly after threshold	(exam 1)
			// questions  266 -  388 => unanswered (no record)					(exam 1)
			// questions  389 -  510 => unanswered (blank answer)				(exam 1)
			// questions  511 -  575 => answered correctly before threshold		(exam 2)
			// questions  576 -  633 => answered correctly after threshold		(exam 2)
			// questions  634 -  695 => answered incorrectly before threshold	(exam 2)
			// questions  696 -  755 => answered incorrectly after threshold	(exam 2)
			// questions  756 - 878 => unanswered (no record)					(exam 2)
			// questions  879 - 1000 => unanswered (blank answer)				(exam 2)
			// questions 1001 - 1500 => ??? (versioned questions)

			var beforeThreshold = statsThresholdDate.AddDays(-20);
			var afterThreshold = statsThresholdDate.AddDays(-5);

			var questions = new List<ExamInstanceQuestion>();

			for (int instanceId = 1; instanceId < 7; instanceId++)
			{
				for (int qId = 21; qId < 1001; qId++)
				{
					bool unansweredNoRecord = (qId > 265 && qId < 389) || (qId > 755 && qId < 879);

					if (!unansweredNoRecord)
					{
						var q = new ExamInstanceQuestion() { ExamInstanceId = instanceId, QuestionId = qId };
						var unanswered = (qId > 388 && qId < 511) || (qId > 878 && qId < 1001);

						if (!unanswered)
						{
							bool isCorrect = qId < 144 || (qId < 634 && qId > 510);
							bool isAfterThreshold = (qId > 75 && qId < 144) || (qId > 200 && qId < 266) || (qId > 575 && qId < 634) || (qId > 695 && qId < 756);
							int lastAnswerId = qId * 4;
							int firstAnswerId = lastAnswerId - 3;
							int correctAnswerId = qId % 3 == 0 ? lastAnswerId : firstAnswerId;

							q.AnswerId = isCorrect ? correctAnswerId : firstAnswerId + 1;
							q.UtcDateUpdated = isAfterThreshold ? afterThreshold : beforeThreshold;
						}

						questions.Add(q);
					}
				}
				// TODO: versioned questions?
			}

			this.DbContext.AddRange(questions);
			this.DbContext.SaveChanges();

			if (detachObjects)
			{
				this.DetachRange(questions);
			}
		}

		#endregion
	}
}
