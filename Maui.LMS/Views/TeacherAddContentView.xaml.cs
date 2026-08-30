using CLI.LMS.Model;
using Library.LMS.Services;
using Maui.LMS.ViewModels;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(ModuleId), "moduleId")]
public partial class TeacherAddContentView : ContentPage
{
	private TeacherAddContentViewViewModel viewModel;
	private Assignment selectedAssignment;

	public int CourseId { get; set; }
	public int ModuleId { get; set; }

	public TeacherAddContentView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		ContentTypePicker.SelectedIndex = 0;
		PageNameEntry.Text = string.Empty;
		PageContentEditor.Text = string.Empty;
		FileNameEntry.Text = string.Empty;
		FilePathEntry.Text = string.Empty;
		selectedAssignment = null;

		PageSection.IsVisible = true;
		FileSection.IsVisible = false;
		AssignmentSection.IsVisible = false;

		viewModel = new TeacherAddContentViewViewModel(CourseId);
		var assignmentCount = viewModel.Assignments?.Count ?? 0;
		BindingContext = viewModel;

		EmptyAssignmentLabel.IsVisible = assignmentCount == 0;
		AssignmentListView.IsVisible = assignmentCount > 0;
	}

	private void OnContentTypeChanged(object sender, EventArgs e)
	{
		PageSection.IsVisible = false;
		FileSection.IsVisible = false;
		AssignmentSection.IsVisible = false;

		switch (ContentTypePicker.SelectedIndex)
		{
			case 0:
				PageSection.IsVisible = true;
				break;
			case 1:
				FileSection.IsVisible = true;
				break;
			case 2:
				AssignmentSection.IsVisible = true;
				break;
		}
	}

	private async void OnBrowseFileClicked(object sender, EventArgs e)
	{
		var result = await FilePicker.Default.PickAsync();
		if (result != null)
		{
			FilePathEntry.Text = result.FullPath;
		}
	}

	private void OnSelectAssignmentClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		selectedAssignment = button.CommandParameter as Assignment;
	}

	private void OnAddContentClicked(object sender, EventArgs e)
	{
		var module = ModuleServiceProxy.Current.Modules.FirstOrDefault(m => m.Id == ModuleId);
		if (module == null) return;

		ContentPlus newContent = null;

		switch (ContentTypePicker.SelectedIndex)
		{
			case 0:
				if (string.IsNullOrWhiteSpace(PageNameEntry.Text) ||
					string.IsNullOrWhiteSpace(PageContentEditor.Text))
					return;

				newContent = new CLI.LMS.Model.Page
				{
					Name = PageNameEntry.Text,
					Content = PageContentEditor.Text
				};
				break;

			case 1:
				if (string.IsNullOrWhiteSpace(FileNameEntry.Text) ||
					string.IsNullOrWhiteSpace(FilePathEntry.Text))
					return;

				newContent = new FilePlus
				{
					Name = FileNameEntry.Text,
					FilePath = FilePathEntry.Text
				};
				break;

			case 2:
				if (selectedAssignment == null)
					return;

				newContent = new AssignPlus
				{
					Name = selectedAssignment.Name,
					Assignment = selectedAssignment
				};
				break;
		}

		if (newContent != null)
		{
			newContent.Id = viewModel.NextContentKey;
			module.Content.Add(newContent);
		}

		Shell.Current.GoToAsync($"//TeacherManageModuleContent?courseId={CourseId}&moduleId={ModuleId}");
	}

	private void OnCancelClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherManageModuleContent?courseId={CourseId}&moduleId={ModuleId}");
	}
}