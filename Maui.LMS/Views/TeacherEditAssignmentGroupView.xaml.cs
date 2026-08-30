using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(GroupId), "groupId")]
public partial class TeacherEditAssignmentGroupView : ContentPage
{
	private AGroup currentGroup;

	public int CourseId { get; set; }
	public int GroupId { get; set; }

	public TeacherEditAssignmentGroupView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		currentGroup = AGroupServiceProxy.Current.AGroups.FirstOrDefault(g => g.Id == GroupId);

		if (currentGroup != null)
		{
			NameEntry.Text = currentGroup.Name;
			WeightEntry.Text = currentGroup.Weight.ToString();
		}
	}

	private void OnSaveClicked(object sender, EventArgs e)
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

		if (currentGroup != null)
		{
			currentGroup.Name = NameEntry.Text;
			currentGroup.Weight = weight;
		}

		Shell.Current.GoToAsync($"//TeacherManageAssignmentGroups?courseId={CourseId}");
	}

	private void OnCancelClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync($"//TeacherManageAssignmentGroups?courseId={CourseId}");
	}
}