using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnet5_beta8_LOJ_test.Domain
{
    public class ExamInstanceQuestion : _BaseEntity
    {
		public int Id { get; set; }

		public int ExamInstanceId { get; set; }

		public int QuestionId { get; set; }

		public int? AnswerId { get; set; }

		#region Navigation Properties

		public ExamInstance ExamInstance { get; set; }

		public Question Question { get; set; }

		#endregion
	}
}
