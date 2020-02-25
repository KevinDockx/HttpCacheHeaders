using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marvin.Cache.Headers.Sample.Controllers
{
    [Route("api/preconditiontests")]
    public class PreconditionTestsController : ControllerBase
    {
        public class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public static List<Person> People { get; set; } = new List<Person>
            { new Person { Id = 1, Name = "Kevin" }, new Person { Id = 2, Name = "Sven" } };


        [HttpGet]
        public IActionResult Get()
        {
            return Ok(People);
        } 

        [HttpGet("{id}", Name = "GetPerson")] 
        public IActionResult Get(int id)
        {
            var personFromStore = People.FirstOrDefault(p => p.Id == id);
            if (personFromStore == null)
            {
                return NotFound();
            }

            return Ok(personFromStore);
        }
         
        [HttpPost]
        public IActionResult Post([FromBody] Person person)
        {
            if (person == null)
            {
                return BadRequest();
            }

            People.Add(person);

            return CreatedAtRoute("GetPerson", person, new { id = person.Id });
        } 

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Person person)
        {
            if (person == null)
            {
                return BadRequest();
            }

            var personFromStore = People.FirstOrDefault(p => p.Id == id);
            if (personFromStore == null)
            {
                return NotFound();
            }

            // don't do this at home :)
            People[id-1].Name = person.Name;

            return NoContent();
        } 

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var personFromStore = People.FirstOrDefault(p => p.Id == id);
            if (personFromStore == null)
            {
                return NotFound();
            }

            People.Remove(personFromStore);
            return NoContent();
        }
    }
}
