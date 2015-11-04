using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnet5_beta8_LOJ_test.Domain
{
    public class ExamInstance : _BaseEntity
    {
		public int Id { get; set; }

		public int ExamTemplateId { get; set; }

		public int StudentId { get; set; }
	}
}
