using System;
using System.Collections.Generic;
using System.Linq;
using CLI.LMS.Model;
using CLI.LMS.Model.Plus;
using Library.LMS.Model;
using Library.LMS.Services;

namespace MyApp
{
	public static class StudentMenuHelper
	{
		public static void ShowStudentMenu()
		{
			bool exitStudentSelection = false;
			while (!exitStudentSelection)
			{
				Console.WriteLine("Select a student to proxy as:");
				StudentServiceProxy.Current.Students.ForEach(Console.WriteLine);
				Console.WriteLine("Enter Student ID (or P to exit):");

				var studentChoice = Console.ReadLine();

				if (studentChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
				{
					Console.WriteLine("Exiting student selection...");
					exitStudentSelection = true;
				}
				else if (int.TryParse(studentChoice, out int studentId))
				{
					var selectedStudent = StudentServiceProxy.Current.Students
						.FirstOrDefault(s => s.Id == studentId);

					if (selectedStudent != null)
					{
						Console.WriteLine($"You are now proxying as: {selectedStudent.Name}");
						ShowStudentMainMenu(selectedStudent);
						exitStudentSelection = true;
					}
					else
					{
						Console.WriteLine("Invalid Student ID. Please try again.");
					}
				}
				else
				{
					Console.WriteLine("Invalid input. Please enter a valid Student ID or P to exit.");
				}
			}
		}

		private static void ShowStudentMainMenu(Student selectedStudent)
		{
			bool exitStudentMenu = false;
			while (!exitStudentMenu)
			{
				Console.WriteLine("\nStudent Menu");
				Console.WriteLine("V. View My Courses");
				Console.WriteLine("E. Exit");
				var studentSubChoice = Console.ReadLine();

				if (studentSubChoice.Equals("V", StringComparison.InvariantCultureIgnoreCase))
				{
					ViewMyCourses(selectedStudent);
				}
				else if (studentSubChoice.Equals("E", StringComparison.InvariantCultureIgnoreCase))
				{
					Console.WriteLine("Exiting student menu...");
					exitStudentMenu = true;
				}
			}
		}

		private static void ViewMyCourses(Student selectedStudent)
		{
			bool exitCourseSelection = false;
			while (!exitCourseSelection)
			{
				Console.WriteLine("\nMy Enrolled Courses:");
				CourseServiceProxy.Current.Courses
					.Where(c => c.Students.Any(s => s.Id == selectedStudent.Id))
					.ToList()
					.ForEach(Console.WriteLine);
				Console.WriteLine("Enter Course ID to view (or P to go back):");

				var courseChoice = Console.ReadLine();

				if (courseChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
				{
					Console.WriteLine("Returning to student menu...");
					exitCourseSelection = true;
				}
				else if (int.TryParse(courseChoice, out int courseId))
				{
					var selectedCourse = CourseServiceProxy.Current.Courses
						.Where(c => c.Students.Any(s => s.Id == selectedStudent.Id))
						.FirstOrDefault(c => c.Id == courseId);

					if (selectedCourse != null)
					{
						ShowCourseMenu(selectedStudent, selectedCourse);
					}
					else
					{
						Console.WriteLine("Invalid Course ID. Please try again.");
					}
				}
				else
				{
					Console.WriteLine("Invalid input. Please enter a valid Course ID or P to go back.");
				}
			}
		}

		private static void ShowCourseMenu(Student selectedStudent, Course selectedCourse)
		{
			bool exitCourseMenu = false;
			while (!exitCourseMenu)
			{
				Console.WriteLine($"\n{selectedCourse.Name} [{selectedCourse.Code}] Menu:");
				Console.WriteLine("M. View Modules");
				Console.WriteLine("A. View Assignments");
				Console.WriteLine("S. View Other Students");
				Console.WriteLine("C. View Course Schedule");
				Console.WriteLine("T. Submit Assignment");
				Console.WriteLine("G. View My Grades");
				Console.WriteLine("U. Unenroll from Course");
				Console.WriteLine("E. Exit");
				var courseMenuChoice = Console.ReadLine();

				if (courseMenuChoice.Equals("M", StringComparison.InvariantCultureIgnoreCase))
				{
					ViewModules(selectedStudent, selectedCourse);
				}
				else if (courseMenuChoice.Equals("A", StringComparison.InvariantCultureIgnoreCase))
				{
					ViewAssignments(selectedStudent, selectedCourse);
				}
				else if (courseMenuChoice.Equals("S", StringComparison.InvariantCultureIgnoreCase))
				{
					ViewOtherStudents(selectedStudent, selectedCourse);
				}
				else if (courseMenuChoice.Equals("C", StringComparison.InvariantCultureIgnoreCase))
				{
					ViewCourseSchedule(selectedCourse);
				}
				else if (courseMenuChoice.Equals("T", StringComparison.InvariantCultureIgnoreCase))
				{
					SubmitAssignment(selectedStudent, selectedCourse);
				}
				else if (courseMenuChoice.Equals("G", StringComparison.InvariantCultureIgnoreCase))
				{
					ViewMyGrades(selectedStudent, selectedCourse);
				}
				else if (courseMenuChoice.Equals("U", StringComparison.InvariantCultureIgnoreCase))
				{
					if (UnenrollFromCourse(selectedStudent, selectedCourse))
					{
						exitCourseMenu = true;
					}
				}
				else if (courseMenuChoice.Equals("E", StringComparison.InvariantCultureIgnoreCase))
				{
					Console.WriteLine("Exiting course menu...");
					exitCourseMenu = true;
				}
			}
		}

		private static void ViewModules(Student selectedStudent, Course selectedCourse)
		{
			bool exitModuleMenu = false;
			while (!exitModuleMenu)
			{
				Console.WriteLine($"\n{selectedCourse.Name} - Modules:");
				foreach (var module in selectedCourse.Modules)
				{
					Console.WriteLine($"  Module {module.Id} - {module.Content.Count} item(s)");
				}
				Console.WriteLine("Enter Module ID to view content (or P to go back):");

				var moduleChoice = Console.ReadLine();

				if (moduleChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
				{
					exitModuleMenu = true;
				}
				else if (int.TryParse(moduleChoice, out int moduleId))
				{
					var selectedModule = selectedCourse.Modules.FirstOrDefault(m => m.Id == moduleId);

					if (selectedModule != null)
					{
						ViewModuleContent(selectedModule);
					}
					else
					{
						Console.WriteLine("Invalid Module ID. Please try again.");
					}
				}
				else
				{
					Console.WriteLine("Invalid input. Please enter a valid Module ID or P to go back.");
				}
			}
		}

		private static void ViewModuleContent(Module selectedModule)
		{
			Console.WriteLine($"\nModule {selectedModule.Id} Content:");
			for (int i = 0; i < selectedModule.Content.Count; i++)
			{
				Console.WriteLine($"  {i + 1}. {selectedModule.Content[i]}");
			}

			Console.WriteLine("\nEnter content number to view details (or P to go back):");
			var contentChoice = Console.ReadLine();

			if (contentChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
			{
				return;
			}
			else if (int.TryParse(contentChoice, out int contentNumber) &&
					 contentNumber > 0 && contentNumber <= selectedModule.Content.Count)
			{
				var contentIndex = contentNumber - 1;
				var selectedContent = selectedModule.Content[contentIndex];

				Console.WriteLine($"\n--- {selectedContent.Name} ---");
				Console.WriteLine(selectedContent.Display());

				if (selectedContent is FilePlus filePlus)
				{
					Console.WriteLine("\nPress O to open file, or any other key to continue:");
					var openChoice = Console.ReadLine();
					if (openChoice.Equals("O", StringComparison.InvariantCultureIgnoreCase))
					{
						filePlus.OpenFile();
					}
				}
				else if (selectedContent is AssignPlus assignPlus)
				{
					Console.WriteLine("\nThis is an embedded assignment.");
					Console.WriteLine("You can submit this assignment from the main assignments page.");
				}

				Console.WriteLine("\n--- End of Content ---");
			}
			else
			{
				Console.WriteLine("Invalid content number. Please try again.");
			}
		}

		private static void ViewAssignments(Student selectedStudent, Course selectedCourse)
		{
			Console.WriteLine($"\n{selectedCourse.Name} - Assignments:");
			foreach (var assignment in selectedCourse.Assignments)
			{
				var mySubmission = assignment.Submissions
					.FirstOrDefault(s => s.StudentId == selectedStudent.Id);

				var submissionStatus = "";
				if (mySubmission != null)
				{
					if (mySubmission.Grade.HasValue)
					{
						submissionStatus = $"[Grade: {mySubmission.Grade}/{assignment.AvailablePoints}]";
					}
					else
					{
						submissionStatus = "[Submitted - Not Graded Yet]";
					}
				}
				else
				{
					submissionStatus = "[Not Submitted]";
				}

				Console.WriteLine($"  [{assignment.Id}] {assignment.Name} - {assignment.Description}");
				Console.WriteLine($"      Points: {assignment.AvailablePoints}, Due: {assignment.DueDate} {submissionStatus}");
			}
		}

		private static void ViewOtherStudents(Student selectedStudent, Course selectedCourse)
		{
			Console.WriteLine($"\n{selectedCourse.Name} - Other Students:");
			var otherStudents = StudentServiceProxy.Current.Students
				.Where(s => s.Id != selectedStudent.Id && selectedCourse.Students.Any(st => st.Id == s.Id));
			foreach (var student in otherStudents)
			{
				Console.WriteLine($"  {student}");
			}
		}

		private static void ViewCourseSchedule(Course selectedCourse)
		{
			Console.WriteLine($"\n{selectedCourse.Name} - Course Schedule:");
			var schedule = selectedCourse.Assignments.OrderBy(a => a.DueDate);
			foreach (var assignment in schedule)
			{
				Console.WriteLine($"  [{assignment.DueDate}] {assignment.Name} - {assignment.AvailablePoints} points");
			}
		}

		private static void SubmitAssignment(Student selectedStudent, Course selectedCourse)
		{
			Console.WriteLine($"\n{selectedCourse.Name} - Assignments:");
			if (selectedCourse.Assignments.Any())
			{
				foreach (var assignment in selectedCourse.Assignments)
				{
					Console.WriteLine($"  [{assignment.Id}] {assignment.Name} - {assignment.Description} (Points: {assignment.AvailablePoints}, Due: {assignment.DueDate})");
				}
				Console.WriteLine("Enter Assignment ID to submit (or P to go back):");

				var assignmentChoice = Console.ReadLine();

				if (assignmentChoice.Equals("P", StringComparison.InvariantCultureIgnoreCase))
				{
					return;
				}
				else if (int.TryParse(assignmentChoice, out int assignmentId))
				{
					var selectedAssignment = selectedCourse.Assignments
						.FirstOrDefault(a => a.Id == assignmentId);

					if (selectedAssignment != null)
					{
						var existingSubmission = selectedAssignment.Submissions
							.FirstOrDefault(sub => sub.StudentId == selectedStudent.Id);

						if (existingSubmission != null)
						{
							Console.WriteLine($"You have already submitted this assignment on {existingSubmission.SubmissionDate}.");
							Console.WriteLine("Do you want to resubmit? (Y/N):");
							var resubmit = Console.ReadLine();

							if (!resubmit.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
							{
								Console.WriteLine("Submission cancelled.");
								return;
							}
						}

						Console.WriteLine($"Submitting for: {selectedAssignment.Name}");
						Console.WriteLine("Enter your submission content (type END on a new line when finished):");

						var contentLines = new List<string>();
						string line;
						while ((line = Console.ReadLine()) != "END")
						{
							contentLines.Add(line);
						}
						var content = string.Join("\n", contentLines);

						var newSubmission = new Submission
						{
							StudentId = selectedStudent.Id,
							AssignmentId = selectedAssignment.Id,
							Content = content,
							SubmissionDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
						};

						SubmitServiceProxy.Current.Add(newSubmission);

						if (existingSubmission != null)
						{
							selectedAssignment.Submissions.Remove(existingSubmission);
						}

						selectedAssignment.Submissions.Add(newSubmission);

						Console.WriteLine($"Successfully submitted assignment: {selectedAssignment.Name}!");
						Console.WriteLine($"Submission Date: {newSubmission.SubmissionDate}");
					}
					else
					{
						Console.WriteLine("Invalid Assignment ID. Please try again.");
					}
				}
				else
				{
					Console.WriteLine("Invalid input. Please enter a valid Assignment ID or P to go back.");
				}
			}
			else
			{
				Console.WriteLine("No assignments available for this course.");
			}
		}

		private static void ViewMyGrades(Student selectedStudent, Course selectedCourse)
		{
			Console.WriteLine($"\n{selectedCourse.Name} [{selectedCourse.Code}] - My Grades");
			Console.WriteLine("=".PadRight(70, '='));

			if (!selectedCourse.Assignments.Any())
			{
				Console.WriteLine("No assignments in this course yet.");
				return;
			}

			// Show individual assignment grades
			Console.WriteLine("\nIndividual Assignment Grades:");
			Console.WriteLine("-".PadRight(70, '-'));

			double totalEarnedPoints = 0;
			double totalAvailablePoints = 0;
			int gradedCount = 0;

			foreach (var assignment in selectedCourse.Assignments.OrderBy(a => a.DueDate))
			{
				var submission = assignment.Submissions.FirstOrDefault(s => s.StudentId == selectedStudent.Id);

				if (submission != null)
				{
					if (submission.Grade.HasValue)
					{
						var percentage = (submission.Grade.Value / (double)assignment.AvailablePoints) * 100;
						Console.WriteLine($"{assignment.Name}:");
						Console.WriteLine($"  Grade: {submission.Grade}/{assignment.AvailablePoints} ({percentage:F2}%)");
						Console.WriteLine($"  Submitted: {submission.SubmissionDate}");

						if (!string.IsNullOrWhiteSpace(submission.Comment))
						{
							Console.WriteLine($"  Feedback: {submission.Comment}");
						}

						totalEarnedPoints += submission.Grade.Value;
						totalAvailablePoints += assignment.AvailablePoints;
						gradedCount++;
					}
					else
					{
						Console.WriteLine($"{assignment.Name}:");
						Console.WriteLine($"  Submitted: {submission.SubmissionDate}");
						Console.WriteLine($"  Status: Not graded yet");
					}
				}
				else
				{
					Console.WriteLine($"{assignment.Name}:");
					Console.WriteLine($"  Status: Not submitted");
					Console.WriteLine($"  Available Points: {assignment.AvailablePoints}");
				}
				Console.WriteLine();
			}

			// Show simple average if there are graded assignments
			if (gradedCount > 0)
			{
				Console.WriteLine("-".PadRight(70, '-'));
				var simpleAverage = (totalEarnedPoints / totalAvailablePoints) * 100;
				Console.WriteLine($"Simple Average: {simpleAverage:F2}% ({totalEarnedPoints}/{totalAvailablePoints} points)");
				Console.WriteLine($"Graded Assignments: {gradedCount} of {selectedCourse.Assignments.Count}");
			}

			// Show weighted grade if assignment groups exist
			if (selectedCourse.AGroups.Any())
			{
				Console.WriteLine("\n" + "=".PadRight(70, '='));
				Console.WriteLine("WEIGHTED GRADE BREAKDOWN:");
				Console.WriteLine("=".PadRight(70, '='));

				double totalWeightedGrade = 0;
				double totalWeight = 0;
				bool hasWeightedGrades = false;

				foreach (var agroup in selectedCourse.AGroups.OrderBy(g => g.Name))
				{
					// Get all assignments in this group that belong to this course
					var groupAssignments = agroup.Assignments
						.Where(a => selectedCourse.Assignments.Contains(a))
						.ToList();

					if (!groupAssignments.Any())
					{
						Console.WriteLine($"\n{agroup.Name} (Weight: {agroup.Weight * 100}%):");
						Console.WriteLine("  No assignments in this group");
						continue;
					}

					// Calculate average grade for this group
					double groupTotalPoints = 0;
					double groupEarnedPoints = 0;
					int groupGradedCount = 0;

					Console.WriteLine($"\n{agroup.Name} (Weight: {agroup.Weight * 100}%):");

					foreach (var assignment in groupAssignments)
					{
						var submission = assignment.Submissions.FirstOrDefault(s => s.StudentId == selectedStudent.Id);
						if (submission != null && submission.Grade.HasValue)
						{
							groupTotalPoints += assignment.AvailablePoints;
							groupEarnedPoints += submission.Grade.Value;
							groupGradedCount++;
							hasWeightedGrades = true;

							var percentage = (submission.Grade.Value / (double)assignment.AvailablePoints) * 100;
							Console.WriteLine($"  - {assignment.Name}: {submission.Grade}/{assignment.AvailablePoints} ({percentage:F2}%)");
						}
					}

					if (groupGradedCount > 0)
					{
						double groupPercentage = (groupEarnedPoints / groupTotalPoints) * 100;
						double weightedContribution = (groupEarnedPoints / groupTotalPoints) * agroup.Weight;

						Console.WriteLine($"  Group Average: {groupPercentage:F2}% ({groupEarnedPoints}/{groupTotalPoints} points)");
						Console.WriteLine($"  Weighted Contribution: {weightedContribution * 100:F2}%");

						totalWeightedGrade += weightedContribution;
						totalWeight += agroup.Weight;
					}
					else
					{
						Console.WriteLine($"  No graded assignments yet in this group");
					}
				}

				if (hasWeightedGrades)
				{
					Console.WriteLine("\n" + "=".PadRight(70, '='));
					double finalGrade = (totalWeightedGrade / totalWeight) * 100;
					Console.WriteLine($"FINAL WEIGHTED GRADE: {finalGrade:F2}%");
					Console.WriteLine($"Letter Grade: {GetLetterGrade(finalGrade)}");
					Console.WriteLine("=".PadRight(70, '='));
				}
				else
				{
					Console.WriteLine("\n" + "=".PadRight(70, '='));
					Console.WriteLine("No graded assignments yet. Cannot calculate weighted grade.");
					Console.WriteLine("=".PadRight(70, '='));
				}
			}
		}

		private static string GetLetterGrade(double percentage)
		{
			if (percentage >= 90) return "A";
			if (percentage >= 80) return "B";
			if (percentage >= 70) return "C";
			if (percentage >= 60) return "D";
			return "F";
		}

		private static bool UnenrollFromCourse(Student selectedStudent, Course selectedCourse)
		{
			Console.WriteLine($"Are you sure you want to unenroll from {selectedCourse.Name}? (Y/N):");
			var confirm = Console.ReadLine();

			if (confirm.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
			{
				selectedCourse.Students.Remove(selectedStudent);

				Console.WriteLine($"Successfully unenrolled from {selectedCourse.Name}.");
				return true;
			}
			else
			{
				Console.WriteLine("Unenrollment cancelled.");
				return false;
			}
		}
	}
}