using Maui.LMS.ViewModels;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class TeacherManageAssignmentsView : ContentPage
{
	private TeacherManageAssignmentsViewViewModel viewModel;

	public int CourseId { get; set; }

	public TeacherManageAssignmentsView()
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
		viewModel = new TeacherManageAssignmentsViewViewModel(CourseId);

		// Force property evaluation
		var assignmentCount = viewModel.Assignments?.Count ?? 0;

		if (viewModel.CurrentCourse != null)
		{
			CourseNameLabel.Text = $"{viewModel.CurrentCourse.Name} - Assignments";
		}

		BindingContext = null;
		BindingContext = viewModel;

		UpdateEmptyState();
	}

	private void UpdateEmptyState()
	{
		bool hasAssignments = viewModel?.Assignments?.Count > 0;
		EmptyStateLabel.IsVisible = !hasAssignments;
		AssignmentListView.IsVisible = hasAssignments;
	}

	private void OnGradeClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var assignmentId = viewModel.GetAssignmentId(button.CommandParameter);

		Shell.Current.GoToAsync($"//TeacherGradeSubmissions?courseId={CourseId}&assignmentId={assignmentId}");
	}

	private void OnEditClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var assignmentId = viewModel.GetAssignmentId(button.CommandParameter);

		Shell.Current.GoToAsync($"//TeacherEditAssignment?courseId={CourseId}&assignmentId={assignmentId}");
	}

	private void OnDeleteClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		viewModel.DeleteAssignment(button.CommandParameter);

		RefreshData();
	}

	private void OnAddAssignmentClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherAddAssignment?courseId={CourseId}");
	}

	private void OnCopyAssignmentClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherCopyAssignment?targetCourseId={CourseId}");
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherCourseDetail?courseId={CourseId}");
	}
}