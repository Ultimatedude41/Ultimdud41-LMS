using System;
using System.Collections.Generic;
using System.Text;

namespace CLI.LMS.Model
{
	public class Submission
	{
		public int Id { get; set; }
		public int StudentId { get; set; }
		public int AssignmentId { get; set; }
		public string Content { get; set; }
		public string SubmissionDate { get; set; }
		public int? Grade { get; set; }
		public string Comment { get; set; }
		public string FilePath { get; set; }
		public override string ToString()
		{
			var gradeStatus = Grade.HasValue ? $"Grade: {Grade}" : "Not Graded";
			return $"[{Id}] Submission by Student {StudentId} on {SubmissionDate}";
		}

		public string Display => ToString() ?? string.Empty;
	}
}
