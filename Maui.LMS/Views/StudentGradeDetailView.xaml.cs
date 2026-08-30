using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(StudentId), "studentId")]
[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(AssignmentId), "assignmentId")]
public partial class StudentGradeDetailView : ContentPage
{
	private Submission currentSubmission;

	public int StudentId { get; set; }
	public int CourseId { get; set; }
	public int AssignmentId { get; set; }

	public StudentGradeDetailView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		// Reset all fields
		FileSection.IsVisible = false;
		FilePathLabel.Text = string.Empty;
		FileContentLabel.Text = string.Empty;
		FileContentLabel.IsVisible = false;

		var assignment = AssignmentServiceProxy.Current.Assignments.FirstOrDefault(a => a.Id == AssignmentId);
		currentSubmission = assignment?.Submissions.FirstOrDefault(s => s.StudentId == StudentId);

		if (assignment != null)
		{
			AssignmentNameLabel.Text = assignment.Name;

			if (currentSubmission != null)
			{
				if (currentSubmission.Grade.HasValue)
				{
					GradeLabel.Text = $"Grade: {currentSubmission.Grade}/{assignment.AvailablePoints}";
				}
				else
				{
					GradeLabel.Text = "Not Graded Yet";
				}

				SubmissionDateLabel.Text = $"Submitted: {currentSubmission.SubmissionDate}";
				FeedbackLabel.Text = !string.IsNullOrWhiteSpace(currentSubmission.Comment)
					? $"Feedback: {currentSubmission.Comment}"
					: "No feedback provided";

				SubmissionContentLabel.Text = currentSubmission.Content ?? "No content";

				// Display file if exists
				if (!string.IsNullOrWhiteSpace(currentSubmission.FilePath))
				{
					FileSection.IsVisible = true;
					FilePathLabel.Text = currentSubmission.FilePath;

					if (IsTextFile(currentSubmission.FilePath))
					{
						try
						{
							FileContentLabel.Text = File.ReadAllText(currentSubmission.FilePath);
							FileContentLabel.IsVisible = true;
						}
						catch
						{
							FileContentLabel.IsVisible = false;
						}
					}
				}
			}
			else
			{
				GradeLabel.Text = "Not Submitted";
				SubmissionDateLabel.Text = "";
				FeedbackLabel.Text = "";
				SubmissionContentLabel.Text = "";
			}
		}
	}

	private bool IsTextFile(string filePath)
	{
		if (string.IsNullOrWhiteSpace(filePath))
			return false;

		string extension = Path.GetExtension(filePath).ToLower();
		string[] textExtensions = { ".txt", ".md", ".csv", ".json", ".xml", ".html", ".css", ".js", ".cs", ".py", ".java", ".cpp", ".h" };
		return textExtensions.Contains(extension);
	}

	private void OnOpenFileClicked(object sender, EventArgs e)
	{
		if (!string.IsNullOrWhiteSpace(currentSubmission?.FilePath))
		{
			try
			{
				var processStartInfo = new System.Diagnostics.ProcessStartInfo
				{
					FileName = currentSubmission.FilePath,
					UseShellExecute = true
				};
				System.Diagnostics.Process.Start(processStartInfo);
			}
			catch
			{
				// Failed to open file
			}
		}
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//StudentGrades?studentId={StudentId}&courseId={CourseId}");
	}
}