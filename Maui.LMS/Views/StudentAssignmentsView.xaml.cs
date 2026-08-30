using Maui.LMS.ViewModels;

namespace Maui.LMS.Views;

[QueryProperty(nameof(StudentId), "studentId")]
[QueryProperty(nameof(CourseId), "courseId")]
public partial class StudentAssignmentsView : ContentPage
{
	private StudentAssignmentsViewViewModel viewModel;

	public int StudentId { get; set; }
	public int CourseId { get; set; }

	public StudentAssignmentsView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		viewModel = new StudentAssignmentsViewViewModel(StudentId, CourseId);

		// Force property evaluation BEFORE setting BindingContext
		var assignmentCount = viewModel.Assignments?.Count ?? 0;

		BindingContext = null;
		BindingContext = viewModel;
	}

	private void OnSubmitClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var assignmentId = viewModel.GetAssignmentId(button.CommandParameter);

		Shell.Current.GoToAsync($"//StudentSubmit?studentId={StudentId}&courseId={CourseId}&assignmentId={assignmentId}");
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//StudentCourseDetail?studentId={StudentId}&courseId={CourseId}");
	}
}