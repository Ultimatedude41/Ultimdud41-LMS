using Maui.LMS.ViewModels;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(AssignmentId), "assignmentId")]
public partial class TeacherGradeSubmissionsView : ContentPage
{
	private TeacherGradeSubmissionsViewViewModel viewModel;

	public int CourseId { get; set; }
	public int AssignmentId { get; set; }

	public TeacherGradeSubmissionsView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);
		RefreshData();
	}

	private void RefreshData()
	{
		viewModel = new TeacherGradeSubmissionsViewViewModel(CourseId, AssignmentId);

		var submissionCount = viewModel.Submissions?.Count ?? 0;

		if (viewModel.CurrentAssignment != null)
		{
			AssignmentNameLabel.Text = $"Grade: {viewModel.CurrentAssignment.Name}";
		}

		LoadSubmissions();

		EmptyStateLabel.IsVisible = submissionCount == 0;
	}

	private void LoadSubmissions()
	{
		SubmissionsContainer.Clear();

		foreach (var submission in viewModel.Submissions)
		{
			var student = viewModel.GetStudent(submission.StudentId);

			var studentLabel = new Label
			{
				Text = $"Student: {student?.Name ?? "Unknown"}",
				FontSize = 16,
				HorizontalOptions = LayoutOptions.Start,
				FontAttributes = FontAttributes.Bold
			};

			SubmissionsContainer.Add(studentLabel);

			// Text content
			if (!string.IsNullOrWhiteSpace(submission.Content))
			{
				SubmissionsContainer.Add(new Label
				{
					Text = "Text Response:",
					HorizontalOptions = LayoutOptions.Start
				});
				SubmissionsContainer.Add(new Label
				{
					Text = submission.Content,
					HorizontalOptions = LayoutOptions.Start
				});
			}

			// File submission
			if (!string.IsNullOrWhiteSpace(submission.FilePath))
			{
				SubmissionsContainer.Add(new Label
				{
					Text = "File Submission:",
					HorizontalOptions = LayoutOptions.Start
				});
				SubmissionsContainer.Add(new Label
				{
					Text = submission.FilePath,
					HorizontalOptions = LayoutOptions.Start
				});

				if (IsTextFile(submission.FilePath))
				{
					try
					{
						SubmissionsContainer.Add(new Label
						{
							Text = File.ReadAllText(submission.FilePath),
							HorizontalOptions = LayoutOptions.Start,
							TextColor = Colors.DarkGray
						});
					}
					catch { }
				}

				var openFileButton = new Button
				{
					Text = "Open File",
					CommandParameter = submission.FilePath
				};
				openFileButton.Clicked += OnOpenFileClicked;
				SubmissionsContainer.Add(openFileButton);
			}

			var dateLabel = new Label
			{
				Text = $"Submitted: {submission.SubmissionDate}",
				HorizontalOptions = LayoutOptions.Start
			};

			var gradeLabel = new Label
			{
				Text = "Grade (out of " + viewModel.CurrentAssignment.AvailablePoints + "):",
				HorizontalOptions = LayoutOptions.Start
			};

			var gradeEntry = new Entry
			{
				Placeholder = "Enter grade",
				Keyboard = Keyboard.Numeric,
				Text = submission.Grade?.ToString() ?? ""
			};

			var commentLabel = new Label
			{
				Text = "Feedback:",
				HorizontalOptions = LayoutOptions.Start
			};

			var commentEditor = new Editor
			{
				Placeholder = "Enter feedback/comments",
				HeightRequest = 80,
				Text = submission.Comment ?? ""
			};

			var saveButton = new Button
			{
				Text = "Save Grade",
				CommandParameter = submission
			};

			saveButton.Clicked += (s, e) =>
			{
				if (int.TryParse(gradeEntry.Text, out int grade))
				{
					submission.Grade = grade;
					submission.Comment = commentEditor.Text;
				}
			};

			var separator = new BoxView
			{
				HeightRequest = 2,
				Color = Colors.Gray,
				Margin = new Thickness(0, 10, 0, 10)
			};

			SubmissionsContainer.Add(dateLabel);
			SubmissionsContainer.Add(gradeLabel);
			SubmissionsContainer.Add(gradeEntry);
			SubmissionsContainer.Add(commentLabel);
			SubmissionsContainer.Add(commentEditor);
			SubmissionsContainer.Add(saveButton);
			SubmissionsContainer.Add(separator);
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
		var button = sender as Button;
		var filePath = button?.CommandParameter as string;

		if (!string.IsNullOrWhiteSpace(filePath))
		{
			try
			{
				System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
				{
					FileName = filePath,
					UseShellExecute = true
				});
			}
			catch { }
		}
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherManageAssignments?courseId={CourseId}");
	}
}