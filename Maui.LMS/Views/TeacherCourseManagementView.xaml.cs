using Maui.LMS.ViewModels;

namespace Maui.LMS.Views;

public partial class TeacherCourseManagementView : ContentPage
{
	private TeacherCourseManagementViewViewModel viewModel;

	public TeacherCourseManagementView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		BindingContext = null;
		viewModel = new TeacherCourseManagementViewViewModel();
		BindingContext = viewModel;

		UpdateEmptyState();
	}

	private void UpdateEmptyState()
	{
		bool hasCourses = viewModel?.Courses?.Count > 0;
		EmptyStateLabel.IsVisible = !hasCourses;
		CourseListView.IsVisible = hasCourses;
	}

	private void OnSelectClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var courseId = viewModel.GetCourseId(button.CommandParameter);

		Shell.Current.GoToAsync($"//TeacherCourseDetail?courseId={courseId}");
	}

	private void OnEditClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var courseId = viewModel.GetCourseId(button.CommandParameter);

		Shell.Current.GoToAsync($"//TeacherEditCourse?courseId={courseId}");
	}

	private void OnDeleteClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		viewModel.DeleteCourse(button.CommandParameter);

		BindingContext = null;
		viewModel = new TeacherCourseManagementViewViewModel();
		BindingContext = viewModel;

		UpdateEmptyState();
	}

	private void OnCopyClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var courseId = viewModel.GetCourseId(button.CommandParameter);

		Shell.Current.GoToAsync($"//TeacherCopyCourse?courseId={courseId}");
	}

	private void OnAddCourseClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//TeacherAddCourse");
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//TeacherMenu");
	}
}