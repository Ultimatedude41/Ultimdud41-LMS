using CLI.LMS.Model;
using Library.LMS.Services;
using Maui.LMS.ViewModels;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class TeacherManageModulesView : ContentPage
{
	private TeacherManageModulesViewViewModel viewModel;

	public int CourseId { get; set; }

	public TeacherManageModulesView()
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
		viewModel = new TeacherManageModulesViewViewModel(CourseId);

		// Force property evaluation
		var moduleCount = viewModel.Modules?.Count ?? 0;

		if (viewModel.CurrentCourse != null)
		{
			CourseNameLabel.Text = $"{viewModel.CurrentCourse.Name} - Modules";
		}

		BindingContext = null;
		BindingContext = viewModel;

		UpdateEmptyState();
	}

	private void UpdateEmptyState()
	{
		bool hasModules = viewModel?.Modules?.Count > 0;
		EmptyStateLabel.IsVisible = !hasModules;
		ModuleListView.IsVisible = hasModules;
	}

	private void OnManageContentClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var moduleId = viewModel.GetModuleId(button.CommandParameter);

		Shell.Current.GoToAsync($"//TeacherManageModuleContent?courseId={CourseId}&moduleId={moduleId}");
	}

	private void OnDeleteClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		viewModel.DeleteModule(button.CommandParameter);

		RefreshData();
	}

	private void OnAddModuleClicked(object sender, EventArgs e)
	{
		// Create new module directly
		var newModule = new Module();

		// Add to service
		ModuleServiceProxy.Current.Add(newModule);

		// Add to course
		var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == CourseId);
		if (course != null)
		{
			course.Modules.Add(newModule);
		}

		// Refresh the list
		RefreshData();
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherCourseDetail?courseId={CourseId}");
	}
}