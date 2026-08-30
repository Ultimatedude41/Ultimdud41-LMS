using CLI.LMS.Model;

namespace API.CLI.Database
{
	public class FakeDatabase
	{

		private List<Student> studentDatas;

		private FakeDatabase() {
			studentDatas = new List<Student>
		{
			new Student { Id = 1, Name = "Alice Johnson", Code = "S1001", Classification = "Freshman" },
			new Student { Id = 2, Name = "Bob Smith", Code = "S1002", Classification = "Sophomore" },
			new Student { Id = 3, Name = "Charlie Brown", Code = "S1003", Classification = "Junior" },
			new Student { Id = 4, Name = "Diana Prince", Code = "S1004", Classification = "Senior" } };
		}

		private static FakeDatabase? instance;
		public static FakeDatabase Current
		{
			get
			{
				if (instance == null)
				{
					instance = new FakeDatabase();
				}
				return instance;
			}
		}

		public List<Student> StudentDatas => studentDatas;
	}
}
