using API.CLI.Database;
using CLI.LMS.Model;

namespace API.CLI.Enterprise
{
	public class StudentDataEC
	{
		public StudentDataEC() { }

		public IEnumerable<Student> Students => FakeDatabase.Current.StudentDatas;

		public Student? Delete(int id)
		{
			var student = FakeDatabase.Current.StudentDatas.FirstOrDefault(s => s.Id == id);
			if (student != null)
			{
				FakeDatabase.Current.StudentDatas.Remove(student);
			}
			return student;
		}

		public Student? AddOrUpdate(Student student)
		{
			var existing = FakeDatabase.Current.StudentDatas.FirstOrDefault(s => s.Id == student.Id);
			if (existing != null)
			{
				var index = FakeDatabase.Current.StudentDatas.IndexOf(existing);
				FakeDatabase.Current.StudentDatas.RemoveAt(index);
				FakeDatabase.Current.StudentDatas.Insert(index, student);
			}
			else
			{
				student.Id = NextKey;
				FakeDatabase.Current.StudentDatas.Add(student);
			}
			return student;
		}

		public int NextKey
		{
			get
			{
				if (Students.Any())
				{
					return Students.Select(s => s.Id).Max() + 1;
				}
				return 1;
			}
		}
	}
}
