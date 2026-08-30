using System;
using System.Collections.Generic;
using System.Text;
using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.ViewModels
{
	public class StudentMainViewViewModel 
	{
		public List<Student> Students
		{
			get
			{
				return StudentServiceProxy.Current.Students;
			}
		}

		public int GetStudentId(object student)
		{
			return (student as Student)?.Id ?? 0;
		}
	}
}
