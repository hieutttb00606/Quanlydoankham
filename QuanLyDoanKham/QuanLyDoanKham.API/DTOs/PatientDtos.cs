using System;

namespace QuanLyDoanKham.API.DTOs
{
    public class PatientDto
    {
        public int PatientId { get; set; }
        public int HealthContractId { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string IDCardNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Department { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class ExamResultDto
    {
        public int ExamResultId { get; set; }
        public int PatientId { get; set; }
        public int? GroupId { get; set; }
        public string ExamType { get; set; }
        public string Result { get; set; }
        public string Diagnosis { get; set; }
        public int? DoctorStaffId { get; set; }
        public string DoctorName { get; set; }
        public DateTime ExamDate { get; set; }
    }
}
