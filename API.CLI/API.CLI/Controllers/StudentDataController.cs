using API.CLI.Database;
using API.CLI.Enterprise;
using CLI.LMS.Model;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace API.CLI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class StudentDataController : ControllerBase
	{

		[HttpGet]
		public IEnumerable<Student> Get()
		{
			return new StudentDataEC().Students;
		}

		[HttpDelete("{id}")]
		public Student? Delete(int id)
		{
			return new StudentDataEC().Delete(id);
		}

		[HttpPost]
		public Student? AddOrUpdate([FromBody] Student student)
		{
			return new StudentDataEC().AddOrUpdate(student);
		}
	}
}
