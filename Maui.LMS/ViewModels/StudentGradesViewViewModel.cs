using System;
using System.Collections.Generic;
using System.Linq;
using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.ViewModels
{
	public class StudentGradesViewViewModel
	{
		private int studentId;
		private int courseId;

		public StudentGradesViewViewModel(int studentId, int courseId)
		{
			this.studentId = studentId;
			this.courseId = courseId;
		}

		public Student CurrentStudent
		{
			get
			{
				return StudentServiceProxy.Current.Students.FirstOrDefault(s => s.Id == studentId);
			}
		}

		public Course CurrentCourse
		{
			get
			{
				return CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == courseId);
			}
		}

		public string CourseTitle
		{
			get
			{
				if (CurrentCourse == null)
					return "My Grades";
				return $"{CurrentCourse.Name} [{CurrentCourse.Code}]";
			}
		}

		public class IndividualGrade
		{
			public int AssignmentId { get; set; }
			public string DisplayText { get; set; }
			public bool HasSubmission { get; set; }
		}

		public List<IndividualGrade> IndividualGrades
		{
			get
			{
				var grades = new List<IndividualGrade>();

				if (CurrentCourse == null || CurrentStudent == null)
					return grades;

				foreach (var assignment in CurrentCourse.Assignments)
				{
					var submission = assignment.Submissions.FirstOrDefault(s => s.StudentId == studentId);

					var grade = new IndividualGrade
					{
						AssignmentId = assignment.Id,
						HasSubmission = submission != null
					};

					if (submission != null && submission.Grade.HasValue)
					{
						grade.DisplayText = $"{assignment.Name} ({submission.Grade}/{assignment.AvailablePoints} points)";
					}
					else if (submission != null)
					{
						grade.DisplayText = $"{assignment.Name} (Submitted - Not Graded)";
					}
					else
					{
						grade.DisplayText = $"{assignment.Name} (Not Submitted - {assignment.AvailablePoints} points available)";
					}

					grades.Add(grade);
				}

				return grades;
			}
		}

		public class WeightedGroupGrade
		{
			public string GroupName { get; set; }
			public double Weight { get; set; }
			public List<string> AssignmentGrades { get; set; } = new List<string>();
			public string GroupSummary { get; set; }
		}

		public List<WeightedGroupGrade> WeightedGrades
		{
			get
			{
				var groupGrades = new List<WeightedGroupGrade>();

				if (CurrentCourse == null || CurrentStudent == null)
					return groupGrades;

				foreach (var agroup in CurrentCourse.AGroups)
				{
					var groupGrade = new WeightedGroupGrade
					{
						GroupName = agroup.Name,
						Weight = agroup.Weight
					};

					var groupAssignments = agroup.Assignments.Where(a => CurrentCourse.Assignments.Contains(a)).ToList();

					if (!groupAssignments.Any())
					{
						groupGrade.GroupSummary = "No assignments";
					}
					else
					{
						double totalPoints = 0;
						double earnedPoints = 0;
						int gradedCount = 0;

						foreach (var assignment in groupAssignments)
						{
							var submission = assignment.Submissions.FirstOrDefault(s => s.StudentId == studentId);
							if (submission != null && submission.Grade.HasValue)
							{
								totalPoints += assignment.AvailablePoints;
								earnedPoints += submission.Grade.Value;
								gradedCount++;

								var percentage = (submission.Grade.Value / (double)assignment.AvailablePoints) * 100;
								groupGrade.AssignmentGrades.Add($"{assignment.Name}: {submission.Grade}/{assignment.AvailablePoints} ({percentage:F2}%)");
							}
						}

						if (gradedCount > 0)
						{
							double groupPercentage = (earnedPoints / totalPoints) * 100;
							groupGrade.GroupSummary = $"Average: {groupPercentage:F2}% ({earnedPoints}/{totalPoints} points)";
						}
						else
						{
							groupGrade.GroupSummary = "No graded assignments yet";
						}
					}

					groupGrades.Add(groupGrade);
				}

				return groupGrades;
			}
		}

		public string FinalGradeText
		{
			get
			{
				if (CurrentCourse == null || CurrentStudent == null)
					return "No graded assignments yet";

				if (!CurrentCourse.AGroups.Any())
					return "No assignment groups configured";

				double totalWeightedGrade = 0;
				double totalWeight = 0;
				bool hasGrades = false;

				foreach (var agroup in CurrentCourse.AGroups)
				{
					var groupAssignments = agroup.Assignments.Where(a => CurrentCourse.Assignments.Contains(a)).ToList();

					if (!groupAssignments.Any())
						continue;

					double totalPoints = 0;
					double earnedPoints = 0;
					int gradedCount = 0;

					foreach (var assignment in groupAssignments)
					{
						var submission = assignment.Submissions.FirstOrDefault(s => s.StudentId == studentId);
						if (submission != null && submission.Grade.HasValue)
						{
							totalPoints += assignment.AvailablePoints;
							earnedPoints += submission.Grade.Value;
							gradedCount++;
							hasGrades = true;
						}
					}

					if (gradedCount > 0)
					{
						double weightedContribution = (earnedPoints / totalPoints) * agroup.Weight;
						totalWeightedGrade += weightedContribution;
						totalWeight += agroup.Weight;
					}
				}

				if (!hasGrades)
					return "No graded assignments yet";

				double finalGrade = (totalWeightedGrade / totalWeight) * 100;
				string letterGrade = GetLetterGrade(finalGrade);
				return $"Final Weighted Grade: {finalGrade:F2}% ({letterGrade})";
			}
		}

		private string GetLetterGrade(double percentage)
		{
			if (CurrentCourse == null)
			{
				// Fallback to default ranges
				if (percentage >= 90) return "A";
				if (percentage >= 80) return "B";
				if (percentage >= 70) return "C";
				if (percentage >= 60) return "D";
				return "F";
			}

			// Use course-specific grade ranges
			if (percentage >= CurrentCourse.GradeRangeA) return "A";
			if (percentage >= CurrentCourse.GradeRangeB) return "B";
			if (percentage >= CurrentCourse.GradeRangeC) return "C";
			if (percentage >= CurrentCourse.GradeRangeD) return "D";
			return "F";
		}

		public int GetAssignmentId(object gradeItem)
		{
			return (gradeItem as IndividualGrade)?.AssignmentId ?? 0;
		}
	}
}