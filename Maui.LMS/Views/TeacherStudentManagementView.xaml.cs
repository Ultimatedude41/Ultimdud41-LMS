using Maui.LMS.ViewModels;

namespace Maui.LMS.Views;

public partial class TeacherStudentManagementView : ContentPage
{
	private TeacherStudentManagementViewViewModel viewModel;

	public TeacherStudentManagementView()
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
		viewModel = new TeacherStudentManagementViewViewModel();

		// Force property evaluation
		var studentCount = viewModel.Students?.Count ?? 0;

		BindingContext = null;
		BindingContext = viewModel;

		UpdateEmptyState();
	}

	private void UpdateEmptyState()
	{
		bool hasStudents = viewModel?.Students?.Count > 0;
		EmptyStateLabel.IsVisible = !hasStudents;
		StudentListView.IsVisible = hasStudents;
	}

	private void OnEditClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var studentId = viewModel.GetStudentId(button.CommandParameter);

		Shell.Current.GoToAsync($"//TeacherEditStudent?studentId={studentId}");
	}

	private void OnDeleteClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		viewModel.DeleteStudent(button.CommandParameter);

		RefreshData();
	}

	private void OnAddStudentClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//TeacherAddStudent");
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//TeacherMenu");
	}
}