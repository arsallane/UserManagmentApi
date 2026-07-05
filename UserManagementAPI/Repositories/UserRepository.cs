using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagementAPI.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User> AddAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
    }

    public class UserRepository : IUserRepository
    {
    // 🔧 AMÉLIORATION : List thread-safe si API multi-thread
    private readonly List<User> _users = new()
    {
        new User { Id = 1, Name = "Alice Dupont", Email = "alice.dupont@example.com" },
        new User { Id = 2, Name = "Marc Lambert", Email = "marc.lambert@example.com" },
        new User { Id = 3, Name = "Sophie Martin", Email = "sophie.martin@example.com" },
        new User { Id = 4, Name = "Thomas Leroy", Email = "thomas.leroy@example.com" },
        new User { Id = 5, Name = "Emma Bernard", Email = "emma.bernard@example.com" },
        new User { Id = 6, Name = "Lucas Moreau", Email = "lucas.moreau@example.com" },
    };

    // GET ALL
    public Task<IEnumerable<User>> GetAllAsync()
    {
        // 🔧 SÉCURITÉ : renvoyer une copie pour éviter que l'appelant modifie la liste interne
        return Task.FromResult<IEnumerable<User>>(_users.ToList());
    }

    // GET BY ID
    public Task<User?> GetByIdAsync(int id)
    {
        // 🔧 AMÉLIORATION : ne plus throw → laisser le controller gérer le 404
        var user = _users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(user);
    }

    // ADD
    public Task<User> AddAsync(User user)
    {
        // 🔧 VALIDATION INTERNE : éviter insertion de données invalides
        if (string.IsNullOrWhiteSpace(user.Name))
            throw new ArgumentException("Name cannot be empty");

        if (string.IsNullOrWhiteSpace(user.Email))
            throw new ArgumentException("Email cannot be empty");

        // 🔧 AMÉLIORATION : email déjà existant ?
        if (_users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Email already exists");

        // 🔧 AMÉLIORATION : génération d'ID plus robuste
        user.Id = _users.Count == 0 ? 1 : _users.Max(u => u.Id) + 1;

        _users.Add(user);
        return Task.FromResult(user);
    }

    // UPDATE
    public Task<bool> UpdateAsync(User user)
    {
        var existing = _users.FirstOrDefault(u => u.Id == user.Id);

        // 🔧 AMÉLIORATION : renvoyer false si user inexistant
        if (existing == null)
            return Task.FromResult(false);

        // 🔧 VALIDATION INTERNE
        if (string.IsNullOrWhiteSpace(user.Name))
            throw new ArgumentException("Name cannot be empty");

        if (string.IsNullOrWhiteSpace(user.Email))
            throw new ArgumentException("Email cannot be empty");

        existing.Name = user.Name;
        existing.Email = user.Email;

        return Task.FromResult(true);
    }

    // DELETE
    public Task<bool> DeleteAsync(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);

        // 🔧 AMÉLIORATION : renvoyer false si user inexistant
        if (user == null)
            return Task.FromResult(false);

        _users.Remove(user);
        return Task.FromResult(true);
    }
}

}