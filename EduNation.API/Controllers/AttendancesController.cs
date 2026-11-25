using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using EduNation.API.Models;
using EduNation.Application.Attendances;
using EduNation.Application.SearchAttendances;
using EduNation.Domain.Entities;

namespace EduNation.API.Controllers
{
    [Route("api/attendances")]
    [ApiController]
    public class AttendancesController : ControllerBase
    {
        private readonly CreateAttendanceUseCase _createUseCase;
        private readonly GetAttendanceByIdUseCase _getByIdUseCase;
        private readonly GetAttendanceByStudentUseCase _getByStudentUseCase;
        private readonly GetAttendanceByCourseSectionAndDateUseCase _getBySectionAndDateUseCase;
        private readonly UpdateAttendanceUseCase _updateUseCase;
        private readonly SearchAttendancesUseCase _searchUseCase;

        public AttendancesController(
            CreateAttendanceUseCase createUseCase,
            GetAttendanceByIdUseCase getByIdUseCase,
            GetAttendanceByStudentUseCase getByStudentUseCase,
            GetAttendanceByCourseSectionAndDateUseCase getBySectionAndDateUseCase,
            UpdateAttendanceUseCase updateUseCase,
            SearchAttendancesUseCase searchUseCase)
        {
            _createUseCase = createUseCase;
            _getByIdUseCase = getByIdUseCase;
            _getByStudentUseCase = getByStudentUseCase;
            _getBySectionAndDateUseCase = getBySectionAndDateUseCase;
            _updateUseCase = updateUseCase;
            _searchUseCase = searchUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAttendance(AttendanceRequest request, CancellationToken cancellationToken)
        {
            var attendance = await _createUseCase.ExecuteAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetAttendanceById), new { id = attendance.Id }, attendance);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetAttendanceById(Guid id, CancellationToken cancellationToken)
        {
            var attendance = await _getByIdUseCase.ExecuteAsync(id, cancellationToken);
            return Ok(attendance);
        }

        [HttpGet("student/{studentId:guid}")]
        public async Task<IActionResult> GetAttendanceByStudent(Guid studentId, CancellationToken cancellationToken)
        {
            var attendances = await _getByStudentUseCase.ExecuteAsync(studentId, cancellationToken);
            return Ok(attendances);
        }

        [HttpGet("course-section/{courseSectionId:guid}/date/{date}")]
        public async Task<IActionResult> GetAttendanceByCourseSectionAndDate(Guid courseSectionId, DateOnly date, CancellationToken cancellationToken)
        {
            var attendances = await _getBySectionAndDateUseCase.ExecuteAsync(courseSectionId, date, cancellationToken);
            return Ok(attendances);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAttendances(
            [FromQuery] AttendanceSearchRequest request,
            CancellationToken cancellationToken)
        {
            var criteria = new AttendanceSearchCriteria
            {
                EnrollmentId = request.EnrollmentId,
                StudentId = request.StudentId,
                CourseSectionId = request.CourseSectionId,
                Date = request.Date,
                Status = request.Status,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            var result = await _searchUseCase.ExecuteAsync(criteria, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAttendance(Guid id, [FromBody] JsonElement body, CancellationToken cancellationToken)
        {
            AttendanceStatus status;
            if (body.ValueKind == JsonValueKind.Number)
            {
                status = (AttendanceStatus)body.GetInt32();
            }
            else if (body.ValueKind == JsonValueKind.String)
            {
                status = Enum.Parse<AttendanceStatus>(body.GetString()!, true);
            }
            else if (body.ValueKind == JsonValueKind.Object && body.TryGetProperty("status", out var property))
            {
                if (property.ValueKind == JsonValueKind.Number)
                    status = (AttendanceStatus)property.GetInt32();
                else
                    status = Enum.Parse<AttendanceStatus>(property.GetString()!, true);
            }
            else
            {
                return BadRequest();
            }

            var attendance = await _updateUseCase.ExecuteAsync(id, status, cancellationToken);
            return Ok(attendance);
        }
    }
}
