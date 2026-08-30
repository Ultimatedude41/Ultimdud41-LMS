using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(ModuleId), "moduleId")]
[QueryProperty(nameof(ContentId), "contentId")]
public partial class TeacherEditContentView : ContentPage
{
	private ContentPlus currentContent;

	public int CourseId { get; set; }
	public int ModuleId { get; set; }
	public int ContentId { get; set; }

	public TeacherEditContentView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		// Reset all fields
		NameEntry.Text = string.Empty;
		ContentTypeLabel.Text = string.Empty;
		PageContentSection.IsVisible = false;
		FilePathSection.IsVisible = false;
		PageContentEditor.Text = string.Empty;
		FilePathEntry.Text = string.Empty;

		var module = ModuleServiceProxy.Current.Modules.FirstOrDefault(m => m.Id == ModuleId);
		currentContent = module?.Content.FirstOrDefault(c => c.Id == ContentId);

		if (currentContent != null)
		{
			NameEntry.Text = currentContent.Name;

			if (currentContent is CLI.LMS.Model.Page page)
			{
				ContentTypeLabel.Text = "Type: Page";
				PageContentSection.IsVisible = true;
				PageContentEditor.Text = page.Content;
			}
			else if (currentContent is FilePlus file)
			{
				ContentTypeLabel.Text = "Type: File";
				FilePathSection.IsVisible = true;
				FilePathEntry.Text = file.FilePath;
			}
			else if (currentContent is AssignPlus)
			{
				ContentTypeLabel.Text = "Type: Assignment Link (Name only)";
			}
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

	private void OnSaveClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(NameEntry.Text))
			return;

		if (currentContent != null)
		{
			currentContent.Name = NameEntry.Text;

			if (currentContent is CLI.LMS.Model.Page page)
			{
				page.Content = PageContentEditor.Text;
			}
			else if (currentContent is FilePlus file)
			{
				file.FilePath = FilePathEntry.Text;
			}
		}

		Shell.Current.GoToAsync($"//TeacherManageModuleContent?courseId={CourseId}&moduleId={ModuleId}");
	}

	private void OnCancelClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherManageModuleContent?courseId={CourseId}&moduleId={ModuleId}");
	}
}