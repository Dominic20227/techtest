using Microsoft.EntityFrameworkCore;
using UserManagement.Core.Implementations;
using UserManagement.Data;
using UserManagement.Data.Entities;

namespace UserManagement.Core.Tests;

public class RepositoryTests : IDisposable
{
    private readonly UserManagementDbContext _context;
    private readonly UserRepository _userRepository;
    private readonly LogRepository _logRepository;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<UserManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new UserManagementDbContext(options);
        _userRepository = new UserRepository(_context);
        _logRepository = new LogRepository(_context);
    }

    #region UserRepository Tests

    [Fact]
    public async Task UserRepository_GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Forename = "John",
            Surname = "Doe",
            Email = "john@example.com",
            IsActive = true,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-30))
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.Forename);
        Assert.Equal("Doe", result.Surname);
        Assert.Equal("john@example.com", result.Email);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task UserRepository_GetByIdAsync_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userRepository.GetByIdAsync(999));
    }

    [Fact]
    public async Task UserRepository_GetByActiveStatusAsync_ShouldReturnActiveUsers()
    {
        // Arrange
        var activeUser = new User
        {
            Id = 1,
            Forename = "Active",
            Surname = "User",
            Email = "active@example.com",
            IsActive = true,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-25))
        };

        var inactiveUser = new User
        {
            Id = 2,
            Forename = "Inactive",
            Surname = "User",
            Email = "inactive@example.com",
            IsActive = false,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-35))
        };

        _context.Users.AddRange(activeUser, inactiveUser);
        await _context.SaveChangesAsync();

        // Act
        var activeUsers = await _userRepository.GetByActiveStatusAsync(true);
        var inactiveUsers = await _userRepository.GetByActiveStatusAsync(false);

        // Assert
        Assert.NotNull(activeUsers);
        Assert.Single(activeUsers);
        Assert.Equal("Active", activeUsers.First().Forename);

        Assert.NotNull(inactiveUsers);
        Assert.Single(inactiveUsers);
        Assert.Equal("Inactive", inactiveUsers.First().Forename);
    }

    [Fact]
    public async Task UserRepository_GetByActiveStatusAsync_ShouldReturnEmptyList_WhenNoUsersWithStatusExist()
    {
        // Arrange - Add only active users
        var activeUser = new User
        {
            Id = 1,
            Forename = "Active",
            Surname = "User",
            Email = "active@example.com",
            IsActive = true,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-25))
        };

        _context.Users.Add(activeUser);
        await _context.SaveChangesAsync();

        // Act
        var inactiveUsers = await _userRepository.GetByActiveStatusAsync(false);

        // Assert
        Assert.NotNull(inactiveUsers);
        Assert.Empty(inactiveUsers);
    }

    [Fact]
    public async Task UserRepository_GetUserWithLogsAsync_ShouldReturnUserWithLogs()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Forename = "John",
            Surname = "Doe",
            Email = "john@example.com",
            IsActive = true,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-30))
        };

        var log1 = new Logs
        {
            Id = 1,
            UserId = 1,
            LogType = "View",
            LogMessage = "User viewed",
            DateAndTime = DateTime.UtcNow
        };

        var log2 = new Logs
        {
            Id = 2,
            UserId = 1,
            LogType = "Update",
            LogMessage = "User updated",
            DateAndTime = DateTime.UtcNow.AddMinutes(30)
        };

        _context.Users.Add(user);
        _context.Logs.AddRange(log1, log2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetUserWithLogsAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.Forename);
        Assert.Equal("Doe", result.Surname);
        Assert.Equal(2, result.Logs.Count);
        Assert.Contains(result.Logs, l => l.LogMessage == "User viewed");
        Assert.Contains(result.Logs, l => l.LogMessage == "User updated");
    }

    [Fact]
    public async Task UserRepository_GetUserWithLogsAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Act
        var result = await _userRepository.GetUserWithLogsAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UserRepository_AddAsync_ShouldAddUser()
    {
        // Arrange
        var user = new User
        {
            Forename = "New",
            Surname = "User",
            Email = "new@example.com",
            IsActive = true,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-28))
        };

        // Act
        var result = await _userRepository.AddAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("New", result.Forename);
        Assert.Equal("User", result.Surname);

        var userInDb = await _context.Users.FindAsync(result.Id);
        Assert.NotNull(userInDb);
        Assert.Equal("New", userInDb.Forename);
    }

    [Fact]
    public async Task UserRepository_UpdateAsync_ShouldUpdateUser()
    {
        // Arrange
        var user = new User
        {
            Forename = "Original",
            Surname = "Name",
            Email = "original@example.com",
            IsActive = true,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-30))
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Get the assigned ID after saving
        var savedUserId = user.Id;

        // Create a new entity for update to avoid tracking conflicts
        var userToUpdate = new User
        {
            Id = savedUserId,
            Forename = "Updated",
            Surname = "Name",
            Email = "updated@example.com",
            IsActive = true,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-30))
        };

        // Act
        _context.ChangeTracker.Clear();
        await _userRepository.UpdateAsync(userToUpdate);

        // Assert
        // Create fresh context to verify update
        _context.ChangeTracker.Clear();
        var updatedUser = await _context.Users.FindAsync(savedUserId);
        Assert.NotNull(updatedUser);
        Assert.Equal("Updated", updatedUser.Forename);
        Assert.Equal("updated@example.com", updatedUser.Email);
    }

    [Fact]
    public async Task UserRepository_DeleteAsync_ShouldDeleteUser()
    {
        // Arrange
        var user = new User
        {
            Forename = "User",
            Surname = "ToDelete",
            Email = "delete@example.com",
            IsActive = true,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-25))
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Get the assigned ID after saving
        var savedUserId = user.Id;

        // Act
        await _userRepository.DeleteAsync(savedUserId);

        // Assert
        var deletedUser = await _context.Users.FindAsync(savedUserId);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task UserRepository_GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Forename = "User", Surname = "One", Email = "user1@example.com", IsActive = true, DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-25)) },
            new User { Forename = "User", Surname = "Two", Email = "user2@example.com", IsActive = false, DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-30)) },
            new User { Forename = "User", Surname = "Three", Email = "user3@example.com", IsActive = true, DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-35)) }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Contains(result, u => u.Forename == "User" && u.Surname == "One");
        Assert.Contains(result, u => u.Forename == "User" && u.Surname == "Two");
        Assert.Contains(result, u => u.Forename == "User" && u.Surname == "Three");
    }

    #endregion

    #region LogRepository Tests

    [Fact]
    public async Task LogRepository_GetByUserIdAsync_ShouldReturnLogsForUser()
    {
        // Arrange
        var user1Logs = new List<Logs>
        {
            new Logs { UserId = 1, LogType = "View", LogMessage = "User viewed", DateAndTime = DateTime.UtcNow },
            new Logs { UserId = 1, LogType = "Update", LogMessage = "User updated", DateAndTime = DateTime.UtcNow.AddMinutes(5) }
        };

        var user2Logs = new List<Logs>
        {
            new Logs { UserId = 2, LogType = "Add", LogMessage = "User added", DateAndTime = DateTime.UtcNow }
        };

        _context.Logs.AddRange(user1Logs);
        _context.Logs.AddRange(user2Logs);
        await _context.SaveChangesAsync();

        // Act
        var user1Result = await _logRepository.GetByUserIdAsync(1);
        var user2Result = await _logRepository.GetByUserIdAsync(2);

        // Assert
        Assert.Equal(2, user1Result.Count);
        Assert.All(user1Result, log => Assert.Equal(1, log.UserId));

        Assert.Single(user2Result);
        Assert.Equal(2, user2Result.First().UserId);
    }

    [Fact]
    public async Task LogRepository_GetByUserIdAsync_ShouldReturnEmptyList_WhenNoLogsExist()
    {
        // Act
        var result = await _logRepository.GetByUserIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task LogRepository_GetByIdAsync_ShouldReturnLog_WhenLogExists()
    {
        // Arrange
        var log = new Logs
        {
            UserId = 1,
            LogType = "View",
            LogMessage = "User viewed",
            DateAndTime = DateTime.UtcNow
        };

        _context.Logs.Add(log);
        await _context.SaveChangesAsync();

        var savedLogId = log.Id;

        // Act
        var result = await _logRepository.GetByIdAsync(savedLogId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("View", result.LogType);
        Assert.Equal("User viewed", result.LogMessage);
        Assert.Equal(1, result.UserId);
    }

    [Fact]
    public async Task LogRepository_GetByIdAsync_ShouldThrowException_WhenLogDoesNotExist()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _logRepository.GetByIdAsync(999));
    }

    [Fact]
    public async Task LogRepository_AddAsync_ShouldAddLog()
    {
        // Arrange
        var log = new Logs
        {
            UserId = 1,
            LogType = "Add",
            LogMessage = "User added",
            DateAndTime = DateTime.UtcNow
        };

        // Act
        var result = await _logRepository.AddAsync(log);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Add", result.LogType);
        Assert.Equal("User added", result.LogMessage);

        var logInDb = await _context.Logs.FindAsync(result.Id);
        Assert.NotNull(logInDb);
        Assert.Equal("User added", logInDb.LogMessage);
    }

    [Fact]
    public async Task LogRepository_GetAllAsync_ShouldReturnAllLogs()
    {
        // Arrange
        var logs = new List<Logs>
        {
            new Logs { UserId = 1, LogType = "Add", LogMessage = "User 1 added", DateAndTime = DateTime.UtcNow },
            new Logs { UserId = 2, LogType = "View", LogMessage = "User 2 viewed", DateAndTime = DateTime.UtcNow.AddMinutes(5) },
            new Logs { UserId = 1, LogType = "Update", LogMessage = "User 1 updated", DateAndTime = DateTime.UtcNow.AddMinutes(10) }
        };

        _context.Logs.AddRange(logs);
        await _context.SaveChangesAsync();

        // Act
        var result = await _logRepository.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Contains(result, l => l.LogMessage == "User 1 added");
        Assert.Contains(result, l => l.LogMessage == "User 2 viewed");
        Assert.Contains(result, l => l.LogMessage == "User 1 updated");
    }

    #endregion

    public void Dispose()
    {
        _context.Dispose();
    }
}
