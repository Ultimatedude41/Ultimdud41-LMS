using Maui.LMS.ViewModels;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(ModuleId), "moduleId")]
public partial class TeacherManageModuleContentView : ContentPage
{
	private TeacherManageModuleContentViewViewModel viewModel;

	public int CourseId { get; set; }
	public int ModuleId { get; set; }

	public TeacherManageModuleContentView()
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
		viewModel = new TeacherManageModuleContentViewViewModel(CourseId, ModuleId);

		// Force property evaluation
		var contentCount = viewModel.Content?.Count ?? 0;

		ModuleNameLabel.Text = $"Module {ModuleId} - Content";

		BindingContext = null;
		BindingContext = viewModel;

		UpdateEmptyState();
	}

	private void UpdateEmptyState()
	{
		bool hasContent = viewModel?.Content?.Count > 0;
		EmptyStateLabel.IsVisible = !hasContent;
		ContentListView.IsVisible = hasContent;
	}

	private void OnEditClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var contentId = viewModel.GetContentId(button.CommandParameter);

		Shell.Current.GoToAsync($"//TeacherEditContent?courseId={CourseId}&moduleId={ModuleId}&contentId={contentId}");
	}

	private void OnDeleteClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		viewModel.DeleteContent(button.CommandParameter);

		RefreshData();
	}

	private void OnAddContentClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherAddContent?courseId={CourseId}&moduleId={ModuleId}");
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherManageModules?courseId={CourseId}");
	}
}