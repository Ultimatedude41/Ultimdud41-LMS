using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.Views;

public partial class TeacherAddStudentView : ContentPage
{
	public TeacherAddStudentView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		CodeEntry.Text = string.Empty;
		NameEntry.Text = string.Empty;
		ClassificationPicker.SelectedIndex = -1;
	}

	private void OnAddStudentClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(CodeEntry.Text) ||
			string.IsNullOrWhiteSpace(NameEntry.Text) ||
			ClassificationPicker.SelectedIndex == -1)
		{
			return;
		}

		var newStudent = new Student
		{
			Code = CodeEntry.Text,
			Name = NameEntry.Text,
			Classification = ClassificationPicker.SelectedItem.ToString()
		};

		StudentServiceProxy.Current.AddOrUpdate(newStudent);

		Shell.Current.GoToAsync("//TeacherStudentManagement");
	}

	private void OnCancelClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//TeacherStudentManagement");
	}
}