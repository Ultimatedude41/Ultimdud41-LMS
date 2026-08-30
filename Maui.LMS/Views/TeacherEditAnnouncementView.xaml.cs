using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(AnnouncementId), "announcementId")]
public partial class TeacherEditAnnouncementView : ContentPage
{
	private Announcement currentAnnouncement;

	public int CourseId { get; set; }
	public int AnnouncementId { get; set; }

	public TeacherEditAnnouncementView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		currentAnnouncement = AnnouncementServiceProxy.Current.Announcements.FirstOrDefault(a => a.Id == AnnouncementId);

		if (currentAnnouncement != null)
		{
			TitleEntry.Text = currentAnnouncement.Title;
			MessageEditor.Text = currentAnnouncement.Message;
		}
	}

	private void OnSaveClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(TitleEntry.Text) ||
			string.IsNullOrWhiteSpace(MessageEditor.Text))
		{
			return;
		}

		if (currentAnnouncement != null)
		{
			currentAnnouncement.Title = TitleEntry.Text;
			currentAnnouncement.Message = MessageEditor.Text;
		}

		Shell.Current.GoToAsync($"//TeacherManageAnnouncements?courseId={CourseId}");
	}

	private void OnCancelClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherManageAnnouncements?courseId={CourseId}");
	}
}