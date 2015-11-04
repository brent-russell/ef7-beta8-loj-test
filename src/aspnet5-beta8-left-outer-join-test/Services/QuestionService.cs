using aspnet5_beta8_LOJ_test.Data;
using aspnet5_beta8_LOJ_test.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnet5_beta8_LOJ_test.Services
{
    public class QuestionService
    {
		private readonly ApplicationDbContext _dbContext;

		public QuestionService(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		protected ApplicationDbContext DbContext
		{
			get
			{
				return _dbContext;
			}
		}

		public Question[] Baseline_LOJ()
		{
			var query = from qRoot in this.DbContext.Questions
						join iq in this.DbContext.ExamInstanceQuestions
							on qRoot.Id equals iq.QuestionId
							into InstanceQuestions
						from iq in InstanceQuestions
							.DefaultIfEmpty()
						where iq == null
						select qRoot;

			return query.ToArray();
		}

		public IQueryable<ExamInstanceQuestion> GetQuery_StudentAnswers(int studentId, DateTime? statisticsThreshold)
		{
			var query = from instance in this.DbContext.ExamInstances
						join answered in this.DbContext.ExamInstanceQuestions
							on new
							{
								StudentId = instance.StudentId,
								ExamInstanceId = instance.Id,
								HasAnswer = true
							}
							equals new
							{
								StudentId = studentId,
								ExamInstanceId = answered.ExamInstanceId,
								HasAnswer = answered.AnswerId != null
							}
						where statisticsThreshold.GetValueOrDefault(DateTime.MinValue) < answered.UtcDateUpdated
						select answered;

			return query;
		}

		public IQueryable<Question> GetQuery_AnsweredVersionedQuestions(int studentId, DateTime? statisticsThreshold)
		{
			var query = from answered in this.GetQuery_StudentAnswers(studentId, statisticsThreshold)
						join qVersion in this.DbContext.Questions
							on answered.QuestionId equals qVersion.Id
						select qVersion;

			return query;
		}

		public Question[] GetQuery_UnansweredRootQuestions_vBaseline(int studentId, DateTime? statisticsThreshold)
		{
			var query = from qRoot in this.DbContext.Questions
						join qAnswered in this.GetQuery_AnsweredVersionedQuestions(studentId, statisticsThreshold)
							on qRoot.Id equals qAnswered.RootQuestionId ?? qAnswered.Id
							into AnsweredQuestions
						from qAnswered in AnsweredQuestions
							.DefaultIfEmpty()
						where qRoot.RootQuestionId == null
							// no reference to LOJ'd object...
							//&& qAnswered == null // EF7 is buggy with outer join where condition
						select qRoot;

			return query.ToArray();
		}

		public Question[] GetQuery_UnansweredRootQuestions_v1(int studentId, DateTime? statisticsThreshold)
		{
			var query = from qRoot in this.DbContext.Questions
						join qAnswered in this.GetQuery_AnsweredVersionedQuestions(studentId, statisticsThreshold)
							on qRoot.Id equals qAnswered.RootQuestionId ?? qAnswered.Id
							into AnsweredQuestions
						from qAnswered in AnsweredQuestions
							.DefaultIfEmpty()
						where qRoot.RootQuestionId == null
							&& qAnswered == null // EF7 is buggy with outer join where condition
						select qRoot;

			return query.ToArray();
		}

		public Question[] GetQuery_UnansweredRootQuestions_v2(int studentId, DateTime? statisticsThreshold)
		{
			var query = from qRoot in this.DbContext.Questions
						join qAnswered in this.GetQuery_AnsweredVersionedQuestions(studentId, statisticsThreshold)
							on qRoot.Id equals qAnswered.RootQuestionId ?? qAnswered.Id
							into AnsweredQuestions
						from qAnswered in AnsweredQuestions
							.DefaultIfEmpty()
						where qRoot.RootQuestionId == null
							&& (qAnswered ?? new Question() { Id = -1 }) == null // EF7 is buggy with outer join where condition
						select qRoot;

			return query.ToArray();
		}

		public Question[] GetQuery_UnansweredRootQuestions_v3(int studentId, DateTime? statisticsThreshold)
		{
			var query = from qRoot in this.DbContext.Questions
						join qAnswered in this.GetQuery_AnsweredVersionedQuestions(studentId, statisticsThreshold)
							on qRoot.Id equals qAnswered.RootQuestionId ?? qAnswered.Id
							into AnsweredQuestions
						from qAnswered in AnsweredQuestions
							.DefaultIfEmpty()
						where qRoot.RootQuestionId == null
							&& (qAnswered == null ? -1 : qAnswered.Id) == -1 // EF7 is buggy with outer join where condition
						select qRoot;

			return query.ToArray();
		}

		public Question[] GetQuery_UnansweredRootQuestions_v4(int studentId, DateTime? statisticsThreshold)
		{
			var query = from qRoot in this.DbContext.Questions
						join qAnswered in this.GetQuery_AnsweredVersionedQuestions(studentId, statisticsThreshold)
							on qRoot.Id equals qAnswered.RootQuestionId ?? qAnswered.Id
							into AnsweredQuestions
						from qAnswered in AnsweredQuestions
							.DefaultIfEmpty()
						where qRoot.RootQuestionId == null
							&& (qAnswered == null ? (Question)null : qAnswered) == (Question)null // EF7 is buggy with outer join where condition
						select qRoot;

			return query.ToArray();
		}

		public Question[] GetQuery_UnansweredRootQuestions_v5(int studentId, DateTime? statisticsThreshold)
		{
			var query = from qRoot in this.DbContext.Questions
						join qAnswered in this.GetQuery_AnsweredVersionedQuestions(studentId, statisticsThreshold)
							on qRoot.Id equals qAnswered.RootQuestionId ?? qAnswered.Id
							into AnsweredQuestions
						from qAnswered in AnsweredQuestions
							.DefaultIfEmpty(new Question() { Id = int.MinValue })
						where qRoot.RootQuestionId == null
							&& qAnswered.Id == int.MinValue
						select qRoot;

			return query.ToArray();
		}

		public Question[] GetQuery_UnansweredRootQuestions_v5p5(int studentId, DateTime? statisticsThreshold)
		{
			var query = from qRoot in this.DbContext.Questions
						join qAnswered in this.GetQuery_AnsweredVersionedQuestions(studentId, statisticsThreshold)
							on qRoot.Id equals qAnswered.RootQuestionId ?? qAnswered.Id
							into AnsweredQuestions
						from qAnswered in AnsweredQuestions
							.DefaultIfEmpty(new Question())
						where qRoot.RootQuestionId == null
							&& qAnswered.Id == 0
						select qRoot;

			return query.ToArray();
		}

		public Question[] GetQuery_UnansweredRootQuestions_v6(int studentId, DateTime? statisticsThreshold)
		{
			var query = from qRoot in this.DbContext.Questions
                        join qAnswered in
						(
							from instance in this.DbContext.ExamInstances
							join answered in this.DbContext.ExamInstanceQuestions
								on new
								{
									StudentId = instance.StudentId,
									ExamInstanceId = instance.Id,
									HasAnswer = true
								}
								equals new
								{
									StudentId = studentId,
									ExamInstanceId = answered.ExamInstanceId,
									HasAnswer = answered.AnswerId != null
								}
							join qVersion in this.DbContext.Questions
								on answered.QuestionId equals qVersion.Id
							where statisticsThreshold.GetValueOrDefault(DateTime.MinValue) < answered.UtcDateUpdated
							select qVersion
						)
							on qRoot.Id equals qAnswered.RootQuestionId ?? qAnswered.Id
							into AnsweredQuestions
						from qAnswered in AnsweredQuestions
							.DefaultIfEmpty()
						where qRoot.RootQuestionId == null
							&& qAnswered == null
						select qRoot;

			return query.ToArray();
		}
	}
}
