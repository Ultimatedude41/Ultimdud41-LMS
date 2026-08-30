using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLI.LMS.Model;
using Library.eCommerce.Utilities;
using Newtonsoft.Json;

namespace Library.LMS.Services
{
	public class StudentServiceProxy
	{
		private static StudentServiceProxy? instance;
		private static object instanceLock = new object();

		public static StudentServiceProxy Current
		{
			get
			{
				lock (instanceLock)
				{
					if (instance == null)
					{
						instance = new StudentServiceProxy();
					}
				}
				return instance;
			}
		}

		private List<Student> students;
		public List<Student> Students => students;

		private StudentServiceProxy()
		{
			//students = new List<Student>
   //         {
   //             new Student { Id = 1, Name = "Alice Johnson", Code = "S1001", Classification = "Freshman" },
   //             new Student { Id = 2, Name = "Bob Smith", Code = "S1002", Classification = "Sophomore" },
   //             new Student { Id = 3, Name = "Charlie Brown", Code = "S1003", Classification = "Junior" },
   //             new Student { Id = 4, Name = "Diana Prince", Code = "S1004", Classification = "Senior" }
   //         };

			var stringFromAPI = new WebRequestHandler().Get("/StudentData").Result;
			students = JsonConvert.DeserializeObject<List<Student>>(stringFromAPI) ?? new List<Student>();
		}

		public int LastKey => Students.Any() ? Students.Select(s => s.Id).Max() : 0;

		public Student? AddOrUpdate(Student? student)
		{
			if (student == null) return null;

			var stringFromAPI = new WebRequestHandler().Post("/StudentData", student).Result;
			var studentFromAPI = JsonConvert.DeserializeObject<Student>(stringFromAPI);

			var existing = students.FirstOrDefault(s => s.Id == (studentFromAPI?.Id ?? 0));
			if (existing != null && studentFromAPI != null)
			{
				var index = students.IndexOf(existing);
				students.RemoveAt(index);
				students.Insert(index, studentFromAPI);
			}
			else if (studentFromAPI != null)
			{
				students.Add(studentFromAPI);
			}

			return studentFromAPI ?? student;
		}

		public Student? Delete(Student? student)
		{
			if (student == null) return null;

			var stringResponse = new WebRequestHandler().Delete($"/StudentData/{student.Id}").Result;
			students.Remove(student);

			return student;
		}
	}
}