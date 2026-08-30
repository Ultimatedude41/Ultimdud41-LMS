using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(StudentId), "studentId")]
[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(ModuleId), "moduleId")]
[QueryProperty(nameof(ContentId), "contentId")]
public partial class StudentModulesDetailView : ContentPage
{
	private ContentPlus currentContent;

	public int StudentId { get; set; }
	public int CourseId { get; set; }
	public int ModuleId { get; set; }
	public int ContentId { get; set; }

	public StudentModulesDetailView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		// CLEAR ALL FIELDS FIRST
		ContentNameLabel.Text = string.Empty;
		ContentDisplayLabel.Text = string.Empty;
		FileContentLabel.Text = string.Empty;
		FileContentLabel.IsVisible = false;
		OpenFileButton.IsVisible = false;
		currentContent = null;

		// Find the specific module first, THEN find content in that module
		var module = ModuleServiceProxy.Current.Modules.FirstOrDefault(m => m.Id == ModuleId);

		if (module != null)
		{
			currentContent = module.Content.FirstOrDefault(c => c.Id == ContentId);
		}

		if (currentContent != null)
		{
			ContentNameLabel.Text = currentContent.Name;
			ContentDisplayLabel.Text = currentContent.Display();

			// Handle file content
			if (currentContent is FilePlus filePlus)
			{
				OpenFileButton.IsVisible = true;

				// Try to read and display text files
				if (IsTextFile(filePlus.FilePath))
				{
					try
					{
						string fileContent = File.ReadAllText(filePlus.FilePath);
						FileContentLabel.Text = fileContent;
						FileContentLabel.IsVisible = true;
					}
					catch
					{
						// If can't read, just show the Open File button
						FileContentLabel.IsVisible = false;
					}
				}
			}
			else
			{
				OpenFileButton.IsVisible = false;
			}
		}
	}

	private bool IsTextFile(string filePath)
	{
		if (string.IsNullOrWhiteSpace(filePath))
			return false;

		string extension = Path.GetExtension(filePath).ToLower();

		// Common text file extensions
		string[] textExtensions = { ".txt", ".md", ".csv", ".json", ".xml", ".html", ".css", ".js", ".cs", ".py", ".java", ".cpp", ".h" };

		return textExtensions.Contains(extension);
	}

	private void OnOpenFileClicked(object sender, EventArgs e)
	{
		var filePlus = currentContent as FilePlus;
		if (filePlus != null)
		{
			filePlus.OpenFile();
		}
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//StudentModules?studentId={StudentId}&courseId={CourseId}");
	}
}