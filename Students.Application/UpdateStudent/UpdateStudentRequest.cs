using System;
using System.Collections.Generic;
using System.Text;

namespace Students.Application.UpdateStudent
{
    public class UpdateStudentRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
    }
}
