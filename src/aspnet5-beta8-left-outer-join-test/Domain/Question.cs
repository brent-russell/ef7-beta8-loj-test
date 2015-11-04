using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace aspnet5_beta8_LOJ_test.Domain
{
    public class Question : _BaseEntity
    {
		public Question()
		{
		}

		public int Id { get; set; }

		public int? RootQuestionId { get; set; }

		public DateTime Version { get; set; }

		public bool IsActive { get; set; }

		public string Text { get; set; }

		public bool OrderedAnswers { get; set; }

		#region Navigation Properties

		public Question RootQuestion { get; set; }

		#endregion
	}
}
