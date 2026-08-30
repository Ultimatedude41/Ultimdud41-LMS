using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class TeacherAddAssignmentGroupView : ContentPage
{
	public int CourseId { get; set; }

	public TeacherAddAssignmentGroupView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		NameEntry.Text = string.Empty;
		WeightEntry.Text = string.Empty;
	}

	private void OnCreateGroupClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
			string.IsNullOrWhiteSpace(WeightEntry.Text))
		{
			return;
		}

		if (!double.TryParse(WeightEntry.Text, out double weight))
		{
			return;
		}

		var newGroup = new AGroup
		{
			Name = NameEntry.Text,
			Weight = weight
		};

		AGroupServiceProxy.Current.Add(newGroup);

		var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == CourseId);
		if (course != null)
		{
			course.AGroups.Add(newGroup);
		}

		Shell.Current.GoToAsync($"//TeacherManageAssignmentGroups?courseId={CourseId}");
	}

	private void OnCancelClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherManageAssignmentGroups?courseId={CourseId}");
	}
}