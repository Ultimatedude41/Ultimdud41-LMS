using Maui.LMS.ViewModels;

namespace Maui.LMS.Views;

[QueryProperty(nameof(StudentId), "studentId")]
[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(AssignmentId), "assignmentId")]
public partial class StudentSubmitView : ContentPage
{
	private StudentSubmitViewViewModel viewModel;
	private string selectedFilePath;

	public int StudentId { get; set; }
	public int CourseId { get; set; }
	public int AssignmentId { get; set; }

	public StudentSubmitView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		viewModel = new StudentSubmitViewViewModel(StudentId, CourseId, AssignmentId);
		BindingContext = viewModel;

		SubmissionEditor.Text = string.Empty;
		SubmissionEditor.Text = viewModel.ExistingSubmissionContent;
		FilePathEntry.Text = string.Empty;
		FilePathEntry.Text = viewModel.ExistingFilePath;
	}

	private async void OnBrowseFileClicked(object sender, EventArgs e)
	{
		var result = await FilePicker.Default.PickAsync();
		if (result != null)
		{
			FilePathEntry.Text = result.FullPath;
		}
	}

	private void OnSubmitClicked(object sender, EventArgs e)
	{
		viewModel.SubmitAssignment(SubmissionEditor.Text, FilePathEntry.Text);
		Shell.Current.GoToAsync($"//StudentAssignments?studentId={StudentId}&courseId={CourseId}");
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//StudentAssignments?studentId={StudentId}&courseId={CourseId}");
	}
}