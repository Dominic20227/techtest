using AutoMapper;
using Moq;
using UserManagement.Common.Constants;
using UserManagement.Common.Exceptions;
using UserManagement.Common.LogsModels;
using UserManagement.Common.UserModels;
using UserManagement.Core.Interfaces;
using UserManagement.Data.Entities;
using UserManagement.Services.Domain.Implementations;
using UserManagement.Services.Implementations;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ILogService> _mockLogService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogService = new Mock<ILogService>();
        _mockMapper = new Mock<IMapper>();
        _userService = new UserService(_mockUserRepository.Object, _mockMapper.Object, _mockLogService.Object);
    }

    #region GetByActiveStatus Tests

    [Fact]
    public async Task GetByActiveStatus_ShouldReturnActiveUsers_WhenActiveUsersExist()
    {
        // Arrange
        var activeUsers = new List<User>
        {
            new User { Id = 1, Forename = "John", Surname = "Doe", IsActive = true },
            new User { Id = 2, Forename = "Jane", Surname = "Smith", IsActive = true }
        };

        var expectedUserModels = new List<UserModel>
        {
            new UserModel { Id = 1, Forename = "John", Surname = "Doe", IsActive = true },
            new UserModel { Id = 2, Forename = "Jane", Surname = "Smith", IsActive = true }
        };

        _mockUserRepository.Setup(x => x.GetByActiveStatusAsync(true))
            .ReturnsAsync(activeUsers);
        _mockMapper.Setup(x => x.Map<List<UserModel>>(activeUsers))
            .Returns(expectedUserModels);

        // Act
        var result = await _userService.GetByActiveStatus(true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, user => Assert.True(user.IsActive));
        _mockUserRepository.Verify(x => x.GetByActiveStatusAsync(true), Times.Once);
    }

    [Fact]
    public async Task GetByActiveStatus_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByActiveStatusAsync(true))
            .ReturnsAsync((List<User>?)null);

        // Act
        var result = await _userService.GetByActiveStatus(true);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByActiveStatus_ShouldReturnEmptyList_WhenEmptyListReturned()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByActiveStatusAsync(false))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _userService.GetByActiveStatus(false);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUserModel_WhenUserExists()
    {
        // Arrange
        var user = new User { Id = 1, Forename = "John", Surname = "Doe", Email = "john@example.com" };
        var expectedUserModel = new UserModel { Id = 1, Forename = "John", Surname = "Doe", Email = "john@example.com" };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);
        _mockMapper.Setup(x => x.Map<UserModel>(user))
            .Returns(expectedUserModel);

        // Act
        var result = await _userService.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John", result.Forename);
        _mockLogService.Verify(x => x.AddAsync<LogsModel>(It.IsAny<LogsModel>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByIdAsync(999))
            .ThrowsAsync(new InvalidOperationException("Entity not found"));

        // Act
        var result = await _userService.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockLogService.Verify(x => x.AddAsync<LogsModel>(It.IsAny<LogsModel>()), Times.Never);
    }

    #endregion

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_ShouldAddUser_WhenValidModelProvided()
    {
        // Arrange
        var addUserModel = new AddUserModel
        {
            Forename = "John",
            Surname = "Doe",
            Email = "john@example.com"
        };

        var userEntity = new User
        {
            Forename = "John",
            Surname = "Doe",
            Email = "john@example.com",
            IsActive = true
        };

        var savedUser = new User
        {
            Id = 1,
            Forename = "John",
            Surname = "Doe",
            Email = "john@example.com",
            IsActive = true
        };

        _mockMapper.Setup(x => x.Map<User>(addUserModel))
            .Returns(userEntity);
        _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(savedUser);

        // Act
        await _userService.AddAsync(addUserModel);

        // Assert
        _mockUserRepository.Verify(x => x.AddAsync(It.Is<User>(u => u.IsActive == true)), Times.Once);
        _mockLogService.Verify(x => x.AddAsync<LogsModel>(It.Is<LogsModel>(log =>
            log.LogType == LogTypeConstants.UserAdded &&
            log.UserId == savedUser.Id)), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ShouldThrowArgumentNullException_WhenModelIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _userService.AddAsync(null!));
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser_WhenValidModelProvided()
    {
        // Arrange
        var updateUserModel = new UpdateUserModel
        {
            Id = 1,
            Forename = "John",
            Surname = "Doe",
            Email = "john.updated@example.com"
        };

        var userEntity = new User
        {
            Id = 1,
            Forename = "John",
            Surname = "Doe",
            Email = "john.updated@example.com"
        };

        _mockMapper.Setup(x => x.Map<User>(updateUserModel))
            .Returns(userEntity);

        // Act
        await _userService.UpdateAsync(updateUserModel);

        // Assert
        _mockUserRepository.Verify(x => x.UpdateAsync(userEntity), Times.Once);
        _mockLogService.Verify(x => x.AddAsync<LogsModel>(It.Is<LogsModel>(log =>
            log.LogType == LogTypeConstants.Update &&
            log.UserId == userEntity.Id)), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowArgumentNullException_WhenModelIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _userService.UpdateAsync(null!));
    }

    #endregion

    #region GetUserAndLogsAsync Tests

    [Fact]
    public async Task GetUserAndLogsAsync_ShouldReturnUserLogsModel_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Forename = "John",
            Surname = "Doe",
            Logs = new List<Logs>
        {
            new Logs { Id = 1, LogType = LogTypeConstants.View, LogMessage = "User viewed", UserId = 1, DateAndTime = DateTime.UtcNow }
        }
        };

        var expectedModel = new UserLogsModel
        {
            Id = 1,
            Forename = "John",
            Surname = "Doe",
            Logs = new List<LogsModel>
        {
            new LogsModel { LogType = LogTypeConstants.View, LogMessage = "User viewed", UserId = 1, DateAndTime = DateTime.UtcNow }
        }
        };

        _mockUserRepository.Setup(x => x.GetUserWithLogsAsync(1))
            .ReturnsAsync(user);
        _mockMapper.Setup(x => x.Map<UserLogsModel>(user))
            .Returns(expectedModel);

        // Act
        var result = await _userService.GetUserAndLogsAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John", result.Forename);
        Assert.NotNull(result.Logs); // Add this line
        Assert.Single(result.Logs);
        Assert.Equal(LogTypeConstants.View, result.Logs.First().LogType);
        Assert.Equal("User viewed", result.Logs.First().LogMessage);
    }

    [Fact]
    public async Task GetUserAndLogsAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetUserWithLogsAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserAndLogsAsync(999);

        // Assert
        Assert.Null(result);
    }

    #endregion
}

public class LogServiceTests
{
    private readonly Mock<ILogRepository> _mockLogRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly LogService _logService;

    public LogServiceTests()
    {
        _mockLogRepository = new Mock<ILogRepository>();
        _mockMapper = new Mock<IMapper>();
        _logService = new LogService(_mockLogRepository.Object, _mockMapper.Object);
    }

    #region GetByUserIdAsync Tests

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnLogsForUser_WhenLogsExist()
    {
        // Arrange
        var logs = new List<Logs>
        {
            new Logs { Id = 1, UserId = 1, LogType = LogTypeConstants.View, LogMessage = "User viewed" },
            new Logs { Id = 2, UserId = 1, LogType = LogTypeConstants.Update, LogMessage = "User updated" }
        };

        var expectedLogModels = new List<LogsModel>
        {
            new LogsModel { UserId = 1, LogType = LogTypeConstants.View, LogMessage = "User viewed" },
            new LogsModel { UserId = 1, LogType = LogTypeConstants.Update, LogMessage = "User updated" }
        };

        _mockLogRepository.Setup(x => x.GetByUserIdAsync(1))
            .ReturnsAsync(logs);
        _mockMapper.Setup(x => x.Map<List<LogsModel>>(logs))
            .Returns(expectedLogModels);

        // Act
        var result = await _logService.GetByUserIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, log => Assert.Equal(1, log.UserId));
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnEmptyList_WhenNoLogsExist()
    {
        // Arrange
        var emptyLogs = new List<Logs>();
        var emptyLogModels = new List<LogsModel>();

        _mockLogRepository.Setup(x => x.GetByUserIdAsync(999))
            .ReturnsAsync(emptyLogs);
        _mockMapper.Setup(x => x.Map<List<LogsModel>>(emptyLogs))
            .Returns(emptyLogModels);

        // Act
        var result = await _logService.GetByUserIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion
}

public class BaseServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly BaseService<IUserRepository, User> _baseService;

    public BaseServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockMapper = new Mock<IMapper>();
        _baseService = new BaseService<IUserRepository, User>(_mockRepository.Object, _mockMapper.Object);
    }

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_ShouldReturnMappedModel_WhenValidModelProvided()
    {
        // Arrange
        var inputModel = new UserModel { Forename = "John", Surname = "Doe" };
        var entity = new User { Forename = "John", Surname = "Doe" };
        var savedEntity = new User { Id = 1, Forename = "John", Surname = "Doe" };
        var outputModel = new UserModel { Id = 1, Forename = "John", Surname = "Doe" };

        _mockMapper.Setup(x => x.Map<UserModel, User>(inputModel)).Returns(entity);
        _mockRepository.Setup(x => x.AddAsync(entity)).ReturnsAsync(savedEntity);
        _mockMapper.Setup(x => x.Map<User, UserModel>(savedEntity)).Returns(outputModel);

        // Act
        var result = await _baseService.AddAsync(inputModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John", result.Forename);
    }

    [Fact]
    public async Task AddAsync_ShouldThrowInvalidObjectException_WhenModelIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidObjectException>(() => _baseService.AddAsync<UserModel>(null!));
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMappedModel_WhenEntityExists()
    {
        // Arrange
        var entity = new User { Id = 1, Forename = "John", Surname = "Doe" };
        var expectedModel = new UserModel { Id = 1, Forename = "John", Surname = "Doe" };

        _mockRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
        _mockMapper.Setup(x => x.Map<UserModel>(entity)).Returns(expectedModel);

        // Act
        var result = await _baseService.GetByIdAsync<UserModel>(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John", result.Forename);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedModels_WhenEntitiesExist()
    {
        // Arrange
        var entities = new List<User>
        {
            new User { Id = 1, Forename = "John", Surname = "Doe" },
            new User { Id = 2, Forename = "Jane", Surname = "Smith" }
        };

        var expectedModels = new List<UserModel>
        {
            new UserModel { Id = 1, Forename = "John", Surname = "Doe" },
            new UserModel { Id = 2, Forename = "Jane", Surname = "Smith" }
        };

        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(entities);
        _mockMapper.Setup(x => x.Map<List<User>, List<UserModel>>(entities)).Returns(expectedModels);

        // Act
        var result = await _baseService.GetAllAsync<UserModel>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ShouldCallRepositoryUpdate_WhenValidModelProvided()
    {
        // Arrange
        var model = new UserModel { Id = 1, Forename = "John", Surname = "Doe" };
        var entity = new User { Id = 1, Forename = "John", Surname = "Doe" };

        _mockMapper.Setup(x => x.Map<User>(model)).Returns(entity);

        // Act
        await _baseService.UpdateAsync(model);

        // Assert
        _mockRepository.Verify(x => x.UpdateAsync(entity), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowInvalidObjectException_WhenModelIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidObjectException>(() => _baseService.UpdateAsync<UserModel>(null!));
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ShouldCallRepositoryDelete()
    {
        // Act
        await _baseService.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(x => x.DeleteAsync(1), Times.Once);
    }

    #endregion
}
