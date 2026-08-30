using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class TeacherAddAnnouncementView : ContentPage
{
	public int CourseId { get; set; }

	public TeacherAddAnnouncementView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		TitleEntry.Text = string.Empty;
		MessageEditor.Text = string.Empty;
	}

	private void OnPostAnnouncementClicked(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(TitleEntry.Text) ||
				string.IsNullOrWhiteSpace(MessageEditor.Text))
			{
				return;
			}

			var newAnnouncement = new Announcement
			{
				Title = TitleEntry.Text,
				Message = MessageEditor.Text,
				PostDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
			};

			AnnouncementServiceProxy.Current.Add(newAnnouncement);

			var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == CourseId);
			if (course != null)
			{
				course.Announcements.Add(newAnnouncement);
			}

			Shell.Current.GoToAsync($"//TeacherManageAnnouncements?courseId={CourseId}");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"ERROR in OnPostAnnouncementClicked: {ex.Message}");
			System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
		}
	}

	private void OnCancelClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherManageAnnouncements?courseId={CourseId}");
	}
}