using Maui.LMS.ViewModels;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class TeacherManageRosterView : ContentPage
{
	private TeacherManageRosterViewViewModel viewModel;

	public int CourseId { get; set; }

	public TeacherManageRosterView()
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
		viewModel = new TeacherManageRosterViewViewModel(CourseId);

		// Force property evaluation BEFORE setting BindingContext
		var enrolledCount = viewModel.EnrolledStudents?.Count ?? 0;
		var availableCount = viewModel.AvailableStudents?.Count ?? 0;

		if (viewModel.CurrentCourse != null)
		{
			CourseNameLabel.Text = $"Manage Roster: {viewModel.CurrentCourse.Name}";
		}

		// Now set BindingContext - bindings should work
		BindingContext = null;
		BindingContext = viewModel;

		UpdateEmptyStates();
	}

	private void UpdateEmptyStates()
	{
		bool hasEnrolled = viewModel?.EnrolledStudents?.Count > 0;
		bool hasAvailable = viewModel?.AvailableStudents?.Count > 0;

		NoEnrolledLabel.IsVisible = !hasEnrolled;
		EnrolledListView.IsVisible = hasEnrolled;

		NoAvailableLabel.IsVisible = !hasAvailable;
		AvailableListView.IsVisible = hasAvailable;
	}

	private void OnAddStudentClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		viewModel.AddStudent(button.CommandParameter);

		RefreshData();
	}

	private void OnRemoveStudentClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		viewModel.RemoveStudent(button.CommandParameter);

		RefreshData();
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherCourseDetail?courseId={CourseId}");
	}
}