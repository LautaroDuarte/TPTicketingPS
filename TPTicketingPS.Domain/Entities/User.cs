using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPTicketingPS.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string? PhoneNumber { get; private set; }

    public string PasswordHash { get; private set; } = string.Empty;

    public bool IsActive { get; private set; } = true; // soft delete

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    // navegación
    public List<Reservation> Reservations { get; private set; } = new();

    public List<AuditLog> AuditLogs { get; private set; } = new();

    private User() { } // EF Core

    public User(string name, string email, string passwordHash, string? phoneNumber = null)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        PhoneNumber = phoneNumber;
    }

    public void ChangePassword(string newHash)
    {
        PasswordHash = newHash;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void UpdatePhone(string phone)
    {
        PhoneNumber = phone;
    }
}
