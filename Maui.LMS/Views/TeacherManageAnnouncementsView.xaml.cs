using Maui.LMS.ViewModels;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class TeacherManageAnnouncementsView : ContentPage
{
	private TeacherManageAnnouncementsViewViewModel viewModel;

	public int CourseId { get; set; }

	public TeacherManageAnnouncementsView()
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
		viewModel = new TeacherManageAnnouncementsViewViewModel(CourseId);

		// Force property evaluation
		var announcementCount = viewModel.Announcements?.Count ?? 0;

		if (viewModel.CurrentCourse != null)
		{
			CourseNameLabel.Text = $"{viewModel.CurrentCourse.Name} - Announcements";
		}

		BindingContext = null;
		BindingContext = viewModel;

		UpdateEmptyState();
	}

	private void UpdateEmptyState()
	{
		bool hasAnnouncements = viewModel?.Announcements?.Count > 0;
		EmptyStateLabel.IsVisible = !hasAnnouncements;
		AnnouncementListView.IsVisible = hasAnnouncements;
	}

	private void OnEditClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		var announcementId = viewModel.GetAnnouncementId(button.CommandParameter);

		Shell.Current.GoToAsync($"//TeacherEditAnnouncement?courseId={CourseId}&announcementId={announcementId}");
	}

	private void OnDeleteClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		viewModel.DeleteAnnouncement(button.CommandParameter);

		RefreshData();
	}

	private void OnAddAnnouncementClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherAddAnnouncement?courseId={CourseId}");
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherCourseDetail?courseId={CourseId}");
	}
}