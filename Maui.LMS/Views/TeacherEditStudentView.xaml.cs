using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(StudentId), "studentId")]
public partial class TeacherEditStudentView : ContentPage
{
	private Student currentStudent;

	public int StudentId { get; set; }

	public TeacherEditStudentView()
	{
		InitializeComponent();
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		currentStudent = StudentServiceProxy.Current.Students.FirstOrDefault(s => s.Id == StudentId);

		if (currentStudent != null)
		{
			CodeEntry.Text = currentStudent.Code;
			NameEntry.Text = currentStudent.Name;

			switch (currentStudent.Classification)
			{
				case "Freshman": ClassificationPicker.SelectedIndex = 0; break;
				case "Sophomore": ClassificationPicker.SelectedIndex = 1; break;
				case "Junior": ClassificationPicker.SelectedIndex = 2; break;
				case "Senior": ClassificationPicker.SelectedIndex = 3; break;
			}
		}
	}

	private void OnSaveClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(CodeEntry.Text) ||
			string.IsNullOrWhiteSpace(NameEntry.Text) ||
			ClassificationPicker.SelectedIndex == -1)
		{
			return;
		}

		if (currentStudent != null)
		{
			currentStudent.Code = CodeEntry.Text;
			currentStudent.Name = NameEntry.Text;
			currentStudent.Classification = ClassificationPicker.SelectedItem.ToString();
			StudentServiceProxy.Current.AddOrUpdate(currentStudent);
		}

		Shell.Current.GoToAsync("//TeacherStudentManagement");
	}

	private void OnCancelClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//TeacherStudentManagement");
	}
}