using CLI.LMS.Model;
using Library.LMS.Services;
using System.Text;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class TeacherImportAssignmentsView : ContentPage
{
	private Course currentCourse;

	public int CourseId { get; set; }

	public TeacherImportAssignmentsView()
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
				var line = lines[i];

				// Handle CSV with quoted fields
				var parts = ParseCsvLine(line);
				if (parts.Length < 4) continue;

				string name = parts[0].Trim();
				string description = parts[1].Trim();
				string pointsStr = parts[2].Trim();
				string dueDate = parts[3].Trim();

				if (!int.TryParse(pointsStr, out int points))
				{
					continue;
				}

				// Check if assignment already exists in this course (by name - idempotent)
				var existingAssignment = currentCourse.Assignments
					.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

				if (existingAssignment == null)
				{
					// Create new assignment
					var newAssignment = new Assignment
					{
						Name = name,
						Description = description,
						AvailablePoints = points,
						DueDate = dueDate
					};

					AssignmentServiceProxy.Current.Add(newAssignment);
					currentCourse.Assignments.Add(newAssignment);
					addedCount++;
				}
				else
				{
					skippedCount++;
				}
			}

			ResultLabel.Text = $"Import complete!\nAdded: {addedCount} assignments\nSkipped: {skippedCount} (already exist)";
		}
		catch (Exception ex)
		{
			ResultLabel.Text = $"Error: {ex.Message}";
		}
	}

	private string[] ParseCsvLine(string line)
	{
		var result = new List<string>();
		var current = new StringBuilder();
		bool inQuotes = false;

		for (int i = 0; i < line.Length; i++)
		{
			char c = line[i];

			if (c == '"')
			{
				if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
				{
					// Escaped quote
					current.Append('"');
					i++; // Skip next quote
				}
				else
				{
					// Toggle quote mode
					inQuotes = !inQuotes;
				}
			}
			else if (c == ',' && !inQuotes)
			{
				// Field separator
				result.Add(current.ToString());
				current.Clear();
			}
			else
			{
				current.Append(c);
			}
		}

		// Add last field
		result.Add(current.ToString());

		return result.ToArray();
	}

	private void OnCancelClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherCourseDetail?courseId={CourseId}");
	}
}