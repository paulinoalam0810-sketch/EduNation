using Microsoft.AspNetCore.Mvc;
using EduNation.API.Models;
using EduNation.Application.CreateStudent;
using EduNation.Application.DeactivateStudent;
using EduNation.Application.GetAllStudents;
using EduNation.Application.GetStudentById;
using EduNation.Application.SearchStudents;
using EduNation.Application.UpdateStudent;

namespace EduNation.API.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly CreateStudentUseCase _createStudentUseCase;
        private readonly GetStudentByIdUseCase _getStudentByIdUseCase;
        private readonly GetAllStudentsUseCase _getAllStudentsUseCase;
        private readonly SearchStudentsUseCase _searchStudentsUseCase;
        private readonly UpdateStudentUseCase _updateStudentUseCase;
        private readonly DeactivateStudentUseCase _deactivateStudentUseCase;
        public StudentsController(CreateStudentUseCase createStudentUseCase,
               GetStudentByIdUseCase getStudentByIdUseCase,
               GetAllStudentsUseCase getAllStudentsUseCase,
               SearchStudentsUseCase searchStudentsUseCase,
               UpdateStudentUseCase updateStudentUseCase,
               DeactivateStudentUseCase deactivateStudentUseCase)
        {
            _createStudentUseCase = createStudentUseCase;
            _getStudentByIdUseCase = getStudentByIdUseCase;
            _getAllStudentsUseCase = getAllStudentsUseCase;
            _searchStudentsUseCase = searchStudentsUseCase;
            _updateStudentUseCase = updateStudentUseCase;
            _deactivateStudentUseCase = deactivateStudentUseCase;
        }


        [HttpPost]
        public async Task<IActionResult> CreateStudent(CreateStudentRequest request, CancellationToken cancellationToken)
        {
            var response = await _createStudentUseCase.ExecuteAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetStudentById), new { id = response.Id }, response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetStudentById(Guid id, CancellationToken cancellationToken)
        {
            var student = await _getStudentByIdUseCase.ExecuteAsync(id, cancellationToken);

            return Ok(student);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStudents(CancellationToken cancellationToken)
        {
            var students = await _getAllStudentsUseCase.ExecuteAsync(cancellationToken);

            return Ok(students);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchStudents(
            [FromQuery] StudentSearchRequest request,
            CancellationToken cancellationToken)
        {
            var criteria = new StudentSearchCriteria
            {
                Search = request.Search,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            var result = await _searchStudentsUseCase.ExecuteAsync(criteria, cancellationToken);

            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateStudent(Guid id, UpdateStudentRequest request, CancellationToken cancellationToken)
        {
            var student = await _updateStudentUseCase.ExecuteAsync(id, request, cancellationToken);

            return Ok(student);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeactivateStudent(Guid id, CancellationToken cancellationToken)
        {
            await _deactivateStudentUseCase.ExecuteAsync(id, cancellationToken);

            return NoContent();
        }

    }
}
