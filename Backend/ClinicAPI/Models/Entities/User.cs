using System;

namespace ClinicAPI.Models.Entities;

public enum Role
{
    Admin,
    Patient,
    Guest
}
public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; }
    public string? PasswordHash { get; set; }
    public int? PatientId { get; set; }
    public Patient? Patient { get; set; }
}
