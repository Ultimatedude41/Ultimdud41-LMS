using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class TeacherImportRosterView : ContentPage
{
	private Course currentCourse;

	public int CourseId { get; set; }

	public TeacherImportRosterView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		currentCourse = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == CourseId);

		if (currentCourse != null)
		{
			CourseNameLabel.Text = $"Importing to: {currentCourse.Name}";
		}

		FilePathEntry.Text = string.Empty;
		ResultLabel.Text = string.Empty;
	}

	private void OnImportClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(FilePathEntry.Text) || currentCourse == null)
		{
			ResultLabel.Text = "Please enter a valid file path.";
			return;
		}

		try
		{
			string filePath = FilePathEntry.Text.Trim();

			if (!File.Exists(filePath))
			{
				ResultLabel.Text = "File not found.";
				return;
			}

			var lines = File.ReadAllLines(filePath);
			int addedCount = 0;
			int skippedCount = 0;

			// Skip header line
			for (int i = 1; i < lines.Length; i++)
			{
				var parts = lines[i].Split(',');
				if (parts.Length < 3) continue;

				string code = parts[0].Trim();
				string name = parts[1].Trim();
				string classification = parts[2].Trim();

				// Check if student already exists in system
				var existingStudent = StudentServiceProxy.Current.Students
					.FirstOrDefault(s => s.Code == code);

				if (existingStudent == null)
				{
					// Create new student
					existingStudent = new Student
					{
						Code = code,
						Name = name,
						Classification = classification
					};
					StudentServiceProxy.Current.AddOrUpdate(existingStudent);
				}

				// Check if student is already in this course (idempotent)
				if (!currentCourse.Students.Contains(existingStudent))
				{
					currentCourse.Students.Add(existingStudent);
					addedCount++;
				}
				else
				{
					skippedCount++;
				}
			}

			ResultLabel.Text = $"Import complete!\nAdded: {addedCount} students\nSkipped: {skippedCount} (already enrolled)";
		}
		catch (Exception ex)
		{
			ResultLabel.Text = $"Error: {ex.Message}";
		}
	}

	private void OnCancelClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherCourseDetail?courseId={CourseId}");
	}
}