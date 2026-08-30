using System;
using System.Linq;
using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.ViewModels
{
	public class StudentSubmitViewViewModel
	{
		private int studentId;
		private int courseId;
		private int assignmentId;

		public StudentSubmitViewViewModel(int studentId, int courseId, int assignmentId)
		{
			this.studentId = studentId;
			this.courseId = courseId;
			this.assignmentId = assignmentId;
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

		public Assignment CurrentAssignment
		{
			get
			{
				return AssignmentServiceProxy.Current.Assignments.FirstOrDefault(a => a.Id == assignmentId);
			}
		}

		public bool IsQuiz
		{
			get
			{
				return CurrentAssignment is Quiz;
			}
		}

		public bool IsRegularAssignment
		{
			get { return !IsQuiz; }
		}

		public string AssignmentName
		{
			get
			{
				return CurrentAssignment?.Name ?? "Unknown Assignment";
			}
		}

		public string AssignmentType
		{
			get
			{
				return IsQuiz ? "[Quiz]" : "[Assignment]";
			}
		}

		public string AssignmentDescription
		{
			get
			{
				if (CurrentAssignment == null || IsQuiz)
					return "";
				return CurrentAssignment.Description ?? "";
			}
		}

		public string QuizQuestion
		{
			get
			{
				if (CurrentAssignment is Quiz quiz)
				{
					return quiz.Question ?? "";
				}
				return "";
			}
		}

		public string AssignmentPoints
		{
			get
			{
				return $"Points: {CurrentAssignment?.AvailablePoints ?? 0}";
			}
		}

		public string AssignmentDueDate
		{
			get
			{
				return $"Due Date: {CurrentAssignment?.DueDate ?? "N/A"}";
			}
		}

		public string ExistingSubmissionContent
		{
			get
			{
				var submission = CurrentAssignment?.Submissions.FirstOrDefault(s => s.StudentId == studentId);
				return submission?.Content ?? string.Empty;
			}
		}

		public bool HasExistingFile
		{
			get
			{
				return !string.IsNullOrWhiteSpace(ExistingSubmission?.FilePath);
			}
		}

		public string ExistingFilePath
		{
			get
			{
				return ExistingSubmission?.FilePath ?? string.Empty;
			}
		}

		public Submission ExistingSubmission
		{
			get
			{
				return CurrentAssignment?.Submissions.FirstOrDefault(s => s.StudentId == studentId);
			}
		}

		public void SubmitAssignment(string content, string filePath = null)
		{
			if (IsQuiz && string.IsNullOrWhiteSpace(content))
				return;

			if (!IsQuiz && string.IsNullOrWhiteSpace(content) && string.IsNullOrWhiteSpace(filePath))
				return;

			if (CurrentAssignment == null)
				return;

			var existingSubmission = ExistingSubmission;

			if (existingSubmission != null)
			{
				existingSubmission.Content = content;
				existingSubmission.FilePath = filePath;
				existingSubmission.SubmissionDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				existingSubmission.Grade = null;
				existingSubmission.Comment = null;
			}
			else
			{
				var newSubmission = new Submission
				{
					StudentId = studentId,
					AssignmentId = assignmentId,
					Content = content,
					FilePath = filePath,
					SubmissionDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
				};

				SubmitServiceProxy.Current.Add(newSubmission);
				CurrentAssignment.Submissions.Add(newSubmission);
			}
		}
	}
}