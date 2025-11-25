namespace PersonnelService.Core.DTOs
{
    public class PersonnelDto
    {
        public string? Id { get; set; }
        public int PersonnelId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Position { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public PersonnelContactsDto Contacts { get; set; } = new PersonnelContactsDto();
        public List<PersonnelDocumentInfoDto> Documents { get; set; } = new List<PersonnelDocumentInfoDto>();
    }

    public class PersonnelContactsDto
    {
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

    public class PersonnelDocumentInfoDto
    {
        public string Type { get; set; } = string.Empty;
        public string? Number { get; set; }
        public string? IssuedBy { get; set; }
        public DateTime? IssuedOn { get; set; }
        public string? Category { get; set; }
        public DateTime? ValidUntil { get; set; }
    }

    public class CreatePersonnelDto
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Position { get; set; } = string.Empty;
        public PersonnelContactsDto Contacts { get; set; } = new PersonnelContactsDto();
        public List<PersonnelDocumentInfoDto>? Documents { get; set; }
    }

    public class UpdatePersonnelDto
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Position { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public PersonnelContactsDto Contacts { get; set; } = new PersonnelContactsDto();
    }
}