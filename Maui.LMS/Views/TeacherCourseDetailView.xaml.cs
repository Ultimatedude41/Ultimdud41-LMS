using CLI.LMS.Model;
using Library.LMS.Services;
using System.Text;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class TeacherCourseDetailView : ContentPage
{
	private Course currentCourse;

	public int CourseId { get; set; }

	public TeacherCourseDetailView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		// Force refresh of course from service
		currentCourse = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == CourseId);

		// Force evaluation of Students list
		if (currentCourse != null)
		{
			var studentCount = currentCourse.Students?.Count ?? 0;
			CourseNameLabel.Text = $"{currentCourse.Name} ({currentCourse.Semester})";
		}
	}

	private void OnManageRosterClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherManageRoster?courseId={CourseId}");
	}

	private async void OnExportRosterClicked(object sender, EventArgs e)
	{
		if (currentCourse == null) return;

		try
		{
			var csv = new StringBuilder();
			csv.AppendLine("StudentCode,Name,Classification");

			// Get students directly from StudentServiceProxy and filter by course
			var enrolledStudents = StudentServiceProxy.Current.Students
				.Where(s => CourseServiceProxy.Current.Courses.Any(c => c.Id == CourseId && c.Students.Any(st => st.Id == s.Id)))
				.ToList();

			foreach (var student in enrolledStudents)
			{
				csv.AppendLine($"{student.Code},{student.Name},{student.Classification}");
			}

			string fileName = $"{currentCourse.Code}_{currentCourse.Section}_Roster.csv";
			string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

			await File.WriteAllTextAsync(filePath, csv.ToString());

			string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", fileName);
			await File.WriteAllTextAsync(downloadsPath, csv.ToString());
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Export error: {ex.Message}");
		}
	}

	private void OnImportRosterClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherImportRoster?courseId={CourseId}");
	}

	private void OnManageAssignmentsClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherManageAssignments?courseId={CourseId}");
	}

	private async void OnExportAssignmentsClicked(object sender, EventArgs e)
	{
		// Re-fetch the course to ensure fresh data
		currentCourse = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == CourseId);

		if (currentCourse == null) return;

		// Force property evaluation
		var assignmentCount = currentCourse.Assignments?.Count ?? 0;

		try
		{
			var csv = new StringBuilder();
			csv.AppendLine("AssignmentName,Description,AvailablePoints,DueDate");

			foreach (var assignment in currentCourse.Assignments)
			{
				// Escape commas and quotes in description
				string description = assignment.Description?.Replace("\"", "\"\"") ?? "";
				if (description.Contains(",") || description.Contains("\"") || description.Contains("\n"))
				{
					description = $"\"{description}\"";
				}

				csv.AppendLine($"{assignment.Name},{description},{assignment.AvailablePoints},{assignment.DueDate}");
			}

			string fileName = $"{currentCourse.Code}_{currentCourse.Section}_Assignments.csv";
			string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

			await File.WriteAllTextAsync(filePath, csv.ToString());

			string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", fileName);
			await File.WriteAllTextAsync(downloadsPath, csv.ToString());
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Export assignments error: {ex.Message}");
		}
	}

	private void OnImportAssignmentsClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherImportAssignments?courseId={CourseId}");
	}

	private void OnManageAssignmentGroupsClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherManageAssignmentGroups?courseId={CourseId}");
	}

	private async void OnExportGradebookClicked(object sender, EventArgs e)
	{
		// Re-fetch the course to ensure fresh data
		currentCourse = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == CourseId);

		if (currentCourse == null) return;

		// Force property evaluation
		var assignmentCount = currentCourse.Assignments?.Count ?? 0;
		var studentCount = currentCourse.Students?.Count ?? 0;

		try
		{
			var csv = new StringBuilder();

			// Build header row: StudentCode, StudentName, then each assignment name
			csv.Append("StudentCode,StudentName");
			foreach (var assignment in currentCourse.Assignments)
			{
				csv.Append($",{assignment.Name}");
			}
			csv.AppendLine();

			// Get students directly from StudentServiceProxy and filter by course
			var enrolledStudents = StudentServiceProxy.Current.Students
				.Where(s => CourseServiceProxy.Current.Courses.Any(c => c.Id == CourseId && c.Students.Any(st => st.Id == s.Id)))
				.OrderBy(s => s.Code)
				.ToList();

			// Build data rows: one row per student
			foreach (var student in enrolledStudents)
			{
				csv.Append($"{student.Code},{student.Name}");

				// Add grade for each assignment
				foreach (var assignment in currentCourse.Assignments)
				{
					var submission = assignment.Submissions.FirstOrDefault(s => s.StudentId == student.Id);

					if (submission != null && submission.Grade.HasValue)
					{
						// Calculate percentage
						double percentage = (submission.Grade.Value / (double)assignment.AvailablePoints) * 100;
						csv.Append($",{percentage:F2}%");
					}
					else
					{
						csv.Append(","); // Empty cell for no grade
					}
				}

				csv.AppendLine();
			}

			string fileName = $"{currentCourse.Code}_{currentCourse.Section}_Gradebook.csv";
			string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

			await File.WriteAllTextAsync(filePath, csv.ToString());

			string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", fileName);
			await File.WriteAllTextAsync(downloadsPath, csv.ToString());
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Export gradebook error: {ex.Message}");
		}
	}

	private void OnManageModulesClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherManageModules?courseId={CourseId}");
	}

	private void OnManageAnnouncementsClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherManageAnnouncements?courseId={CourseId}");
	}

	private void OnCourseSettingsClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherCourseSettings?courseId={CourseId}");
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//TeacherCourseManagement");
	}
}