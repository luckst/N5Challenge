using Moq;
using N5.Challenge.Application.Commands;
using N5.Challenge.Domain;
using N5.Challenge.Infrasctructure.ElasticSearch;
using N5.Challenge.Infrasctructure.Exceptions;
using N5.Challenge.Infrasctructure.Repositories;
using N5.Challenge.Infrasctructure.RepositoryPattern;

namespace N5.Challenge.UnitTests.Application
{
    [TestFixture]
    public class AddPermissionCommandHandlerTests
    {
        private Mock<IElasticSearchRepository<Permission>> _elasticSearchRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private AddPermissionCommandHandler.Handler _handler;

        [SetUp]
        public void Setup()
        {
            _elasticSearchRepositoryMock = new Mock<IElasticSearchRepository<Permission>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new AddPermissionCommandHandler.Handler(_unitOfWorkMock.Object, _elasticSearchRepositoryMock.Object);
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenEmployeeIdIsEmpty()
        {
            var command = new AddPermissionCommandHandler.Command
            {
                EmployeeId = Guid.Empty,
                PermissionTypeId = Guid.NewGuid()
            };

            Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(command, CancellationToken.None));
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenPermissionTypeIdIsEmpty()
        {
            var command = new AddPermissionCommandHandler.Command
            {
                EmployeeId = Guid.NewGuid(),
                PermissionTypeId = Guid.Empty
            };

            Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(command, CancellationToken.None));
        }

        [Test]
        public void ShouldThrowDuplicatedPermissionExceptionWhenEmployeePermissionExists()
        {
            var employeePermission = new Permission
            {
                EmployeeId = Guid.NewGuid(),
                PermissionTypeId = Guid.NewGuid()
            };

            var permissionRepositoryMock = new Mock<IPermissionRepository>();
            permissionRepositoryMock.Setup(x => x.GetEmployeePermission(employeePermission.EmployeeId, employeePermission.PermissionTypeId))
                .Returns(Task.FromResult(employeePermission));

            _unitOfWorkMock.Setup(x => x.GetPermissionRepository()).Returns(permissionRepositoryMock.Object);

            var command = new AddPermissionCommandHandler.Command
            {
                EmployeeId = employeePermission.EmployeeId,
                PermissionTypeId = employeePermission.PermissionTypeId
            };

            Assert.ThrowsAsync<DuplicatedPermissionException>(async () => await _handler.Handle(command, CancellationToken.None));
        }

        [Test]
        public async Task ShouldCreatePermissionAndPersistToElasticSearch()
        {
            var command = new AddPermissionCommandHandler.Command
            {
                EmployeeId = Guid.NewGuid(),
                PermissionTypeId = Guid.NewGuid()
            };

            var permissionRepositoryMock = new Mock<IPermissionRepository>();
            _unitOfWorkMock.Setup(x => x.GetPermissionRepository()).Returns(permissionRepositoryMock.Object);

            await _handler.Handle(command, CancellationToken.None);

            permissionRepositoryMock.Verify(x => x.CreateAsync(It.Is<Permission>(p => p.EmployeeId == command.EmployeeId && p.PermissionTypeId == command.PermissionTypeId)), Times.Once);
            _elasticSearchRepositoryMock.Verify(x => x.PersistAsync(It.Is<Permission>(p => p.EmployeeId == command.EmployeeId && p.PermissionTypeId == command.PermissionTypeId)), Times.Once);
        }
    }
}
