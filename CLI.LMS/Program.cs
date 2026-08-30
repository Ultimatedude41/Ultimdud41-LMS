using System;
using CLI.LMS.Model;
using CLI.LMS.Model.Plus;
using Library.LMS.Services;

namespace MyApp
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			// Initialize all service proxies
			var list = CourseServiceProxy.Current.Courses;
			var studentList = StudentServiceProxy.Current.Students;
			var moduleList = ModuleServiceProxy.Current.Modules;
			var assignmentList = AssignmentServiceProxy.Current.Assignments;
			var submissionList = SubmitServiceProxy.Current.Submissions;
			var agroupList = AGroupServiceProxy.Current.AGroups;

			bool exitProgram = false;

			while (!exitProgram)
			{
				Console.WriteLine("Please select your role:");
				Console.WriteLine("1. Teacher");
				Console.WriteLine("2. Student");
				var choice = Console.ReadLine();

				if (int.TryParse(choice, out int choiceInt))
				{
					switch (choiceInt)
					{
						case 1:
							TeacherMenuHelper.ShowTeacherMenu();
							break;

						case 2:
							StudentMenuHelper.ShowStudentMenu();
							break;

						default:
							Console.WriteLine("ERROR: Unknown User Type");
							break;
					}
				}
			}
		}
	}
}