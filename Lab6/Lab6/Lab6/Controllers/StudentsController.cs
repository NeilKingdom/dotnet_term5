using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab6.Data;
using Lab6.Models;

namespace Lab6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentDbContext _context;

        public StudentsController(StudentDbContext context)
        {
            _context = context;
        }

        // GET: api/Students
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)] // Returned when we return the list of Students successfully
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Returned when there is an error in processing
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return Ok(await _context.Students.ToListAsync());
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Returned if we return the Student with ID id successfully
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Returned if Student with ID id is not found in data store
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Returned when there is an error in processing
        public async Task<ActionResult<Student>> GetStudent(Guid id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        // PUT: api/Students/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Returned when we update a Student successfully
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Returned if HTTP header data is invalid
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Returned if Student with ID id is not found in the data store
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Returned when there is an error in processing
        public async Task<ActionResult<Student>> PutStudent(Guid id, Student student)
        {
            var existingStudent = await _context.Students.FindAsync(id);
            if (existingStudent == null)
            {
                return NotFound();
            }

            existingStudent.FirstName = student.FirstName;
            existingStudent.LastName = student.LastName;
            existingStudent.Program = student.Program;
            _context.Students.Update(existingStudent);
            await _context.SaveChangesAsync();

            return Ok(existingStudent);
        }

        // POST: api/Students
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] // Returned when we create a Student successfully
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Returned if HTTP header data is invalid
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Returned when there is an error in processing
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudent", new { id = student.StudentId }, student);
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Returned when we delete a Student successfully
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Returned if Student with ID id is not found in data store
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Returned when there is an error in processing
        public async Task<IActionResult> DeleteStudent(Guid id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentExists(Guid id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }
    }
}
