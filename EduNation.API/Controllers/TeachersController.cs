using Microsoft.AspNetCore.Mvc;
using EduNation.API.Models;
using EduNation.Application.SearchTeachers;
using EduNation.Application.Teachers;

namespace EduNation.API.Controllers
{
    [Route("api/teachers")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly CreateTeacherUseCase _createTeacherUseCase;
        private readonly GetTeacherByIdUseCase _getTeacherByIdUseCase;
        private readonly GetAllTeachersUseCase _getAllTeachersUseCase;
        private readonly UpdateTeacherUseCase _updateTeacherUseCase;
        private readonly DeactivateTeacherUseCase _deactivateTeacherUseCase;
        private readonly SearchTeachersUseCase _searchTeachersUseCase;

        public TeachersController(
            CreateTeacherUseCase createTeacherUseCase,
            GetTeacherByIdUseCase getTeacherByIdUseCase,
            GetAllTeachersUseCase getAllTeachersUseCase,
            UpdateTeacherUseCase updateTeacherUseCase,
            DeactivateTeacherUseCase deactivateTeacherUseCase,
            SearchTeachersUseCase searchTeachersUseCase)
        {
            _createTeacherUseCase = createTeacherUseCase;
            _getTeacherByIdUseCase = getTeacherByIdUseCase;
            _getAllTeachersUseCase = getAllTeachersUseCase;
            _updateTeacherUseCase = updateTeacherUseCase;
            _deactivateTeacherUseCase = deactivateTeacherUseCase;
            _searchTeachersUseCase = searchTeachersUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeacher(TeacherRequest request, CancellationToken cancellationToken)
        {
            var teacher = await _createTeacherUseCase.ExecuteAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetTeacherById), new { id = teacher.Id }, teacher);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTeacherById(Guid id, CancellationToken cancellationToken)
        {
            var teacher = await _getTeacherByIdUseCase.ExecuteAsync(id, cancellationToken);
            return Ok(teacher);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTeachers(CancellationToken cancellationToken)
        {
            var teachers = await _getAllTeachersUseCase.ExecuteAsync(cancellationToken);
            return Ok(teachers);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchTeachers(
            [FromQuery] TeacherSearchRequest request,
            CancellationToken cancellationToken)
        {
            var criteria = new TeacherSearchCriteria
            {
                Search = request.Search,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            var result = await _searchTeachersUseCase.ExecuteAsync(criteria, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateTeacher(Guid id, TeacherRequest request, CancellationToken cancellationToken)
        {
            var teacher = await _updateTeacherUseCase.ExecuteAsync(id, request, cancellationToken);
            return Ok(teacher);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeactivateTeacher(Guid id, CancellationToken cancellationToken)
        {
            await _deactivateTeacherUseCase.ExecuteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
