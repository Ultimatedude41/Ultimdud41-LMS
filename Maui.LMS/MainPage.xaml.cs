namespace Maui.LMS
{
	public partial class MainPage : ContentPage
	{

		public MainPage()
		{
			InitializeComponent();
		}

		private void StudentClicked(object sender, EventArgs e)
		{
			Shell.Current.GoToAsync("//StudentMenu");
		}

		private void TeachClicked(object sender, EventArgs e)
		{
			Shell.Current.GoToAsync("//TeacherMenu");
		}


	}
}
