using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnet5_beta8_LOJ_test.Domain
{
    public abstract class _BaseEntity
    {
		private DateTime _utcDateCreated;
		private DateTime _utcDateUpdated;

		public DateTime UtcDateCreated
		{
			get
			{
				this.SetUnspecifiedAsUtc(ref _utcDateCreated);

				return _utcDateCreated;
			}
			set
			{
				_utcDateCreated = value;
			}
		}

		public DateTime UtcDateUpdated
		{
			get
			{
				this.SetUnspecifiedAsUtc(ref _utcDateUpdated);

				return _utcDateUpdated;
			}
			set
			{
				_utcDateUpdated = value;
			}
		}

		public string CreatedBy { get; set; }

		public string UpdatedBy { get; set; }

		/// <summary>
		/// If the specified DateTime object is <see cref="DateTimeKind.Unspecified"/> then set it to <see cref="DateTimeKind.Utc"/>
		/// </summary>
		protected DateTime ConvertUnspecifiedToUtc(DateTime value)
		{
			if (value.Kind == DateTimeKind.Unspecified)
			{
				return DateTime.SpecifyKind(value, DateTimeKind.Utc);
			}

			return value;
		}

		protected void SetUnspecifiedAsUtc(ref DateTime value)
		{
			if (value.Kind == DateTimeKind.Unspecified)
			{
				value = DateTime.SpecifyKind(value, DateTimeKind.Utc);
			}
		}
	}
}
