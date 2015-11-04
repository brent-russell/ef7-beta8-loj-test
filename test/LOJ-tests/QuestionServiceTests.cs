using Microsoft.Data.Entity;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;
using System.Threading;
using System.Diagnostics;
using aspnet5_beta8_LOJ_test.Services;
using aspnet5_beta8_LOJ_test.Domain;

namespace LOJ_Tests
{
    public class QuestionServiceTests : _DbContextServiceTestBase
    {
		private readonly QuestionService _questionService;
		private DateTime _statsThresholdDate;

        public QuestionServiceTests()
			: base((services) =>services.AddScoped<QuestionService>())
        {
			_questionService = this.ServiceProvider.GetRequiredService<QuestionService>();

			_statsThresholdDate = DateTime.UtcNow.AddDays(-10);
			this.Setup_ExamInstance_Full_Stack_Test_Data(_statsThresholdDate);
		}

		#region Asserts

		private void Assert_ExistsInCollection(IEnumerable<int> expected, IEnumerable<int> actual, string errorMessage)
		{
			Assert.All(actual, k => Assert.True(expected.Contains(k), errorMessage));
		}

		private void Assert_QuestionsAreNotInDiagnosticOrTopiclessRange(IEnumerable<Question> collection)
		{
			Assert.All(collection, q => Assert.True(q.Id > 40, "Diagnostic or topic-less question included."));
		}

		private void Assert_QuestionsAreLatestVersions(IEnumerable<Question> actualQuestions)
		{
			IEnumerable<Question> allQuestions = this.DbContext.Questions.AsEnumerable();

			var query = from qActual in actualQuestions
						join q in allQuestions
							on qActual.RootQuestionId equals q.RootQuestionId
						where q.Version > qActual.Version
						select qActual;

			Assert.Empty(query.ToArray());
		}

		private void Assert_QuestionsAreInUnansweredRange(IEnumerable<Question> actualQuestions)
		{
			// questions should be unanswered
			Assert.All(actualQuestions, q => Assert.True((q.Id > 265 && q.Id < 511) || (q.Id > 755 & q.Id < 1001), "Question has already been answered"));
		}

		#endregion

		[Fact]
		public void Baseline_LOJ_Should_Not_Throw()
		{
			_questionService.Baseline_LOJ();
		}

		[Fact]
		public void GetQuery_UnansweredRootQuestions_vBaseline_Should_Not_Throw()
		{
			_questionService.GetQuery_UnansweredRootQuestions_vBaseline(studentId: 1, statisticsThreshold: _statsThresholdDate);
		}

		[Fact]
		public void GetQuery_UnansweredRootQuestions_v1_Should_Not_Throw()
		{
			_questionService.GetQuery_UnansweredRootQuestions_v1(studentId: 1, statisticsThreshold: _statsThresholdDate);
		}

		[Fact]
		public void GetQuery_UnansweredRootQuestions_v2_Should_Not_Throw()
		{
			_questionService.GetQuery_UnansweredRootQuestions_v2(studentId: 1, statisticsThreshold: _statsThresholdDate);
		}

		[Fact]
		public void GetQuery_UnansweredRootQuestions_v3_Should_Not_Throw()
		{
			_questionService.GetQuery_UnansweredRootQuestions_v3(studentId: 1, statisticsThreshold: _statsThresholdDate);
		}

		[Fact]
		public void GetQuery_UnansweredRootQuestions_v4_Should_Not_Throw()
		{
			_questionService.GetQuery_UnansweredRootQuestions_v4(studentId: 1, statisticsThreshold: _statsThresholdDate);
		}

		[Fact]
		public void GetQuery_UnansweredRootQuestions_v5_Should_Not_Throw()
		{
			_questionService.GetQuery_UnansweredRootQuestions_v5(studentId: 1, statisticsThreshold: _statsThresholdDate);
		}

		[Fact]
		public void GetQuery_UnansweredRootQuestions_v5p5_Should_Not_Throw()
		{
			_questionService.GetQuery_UnansweredRootQuestions_v5p5(studentId: 1, statisticsThreshold: _statsThresholdDate);
		}

		[Fact]
		public void GetQuery_UnansweredRootQuestions_v6_Should_Not_Throw()
		{
			_questionService.GetQuery_UnansweredRootQuestions_v6(studentId: 1, statisticsThreshold: _statsThresholdDate);
		}
	}
}
