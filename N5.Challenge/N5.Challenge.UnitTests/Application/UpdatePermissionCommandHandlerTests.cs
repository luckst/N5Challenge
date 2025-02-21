using Moq;
using N5.Challenge.Application.Commands;
using N5.Challenge.Domain;
using N5.Challenge.Infrasctructure.ElasticSearch;
using N5.Challenge.Infrasctructure.Exceptions;
using N5.Challenge.Infrasctructure.KafkaConfig.Producers;
using N5.Challenge.Infrasctructure.Repositories;
using N5.Challenge.Infrasctructure.RepositoryPattern;

namespace N5.Challenge.UnitTests.Application
{
    [TestFixture]
    public class UpdatePermissionCommandHandlerTests
    {
        private Mock<IElasticSearchRepository<Permission>> _elasticSearchRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IOperationProducer> _operationProducerMock;
        private UpdatePermissionCommandHandler.Handler _handler;

        [SetUp]
        public void Setup()
        {
            _elasticSearchRepositoryMock = new Mock<IElasticSearchRepository<Permission>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _operationProducerMock = new Mock<IOperationProducer>();
            _handler = new UpdatePermissionCommandHandler.Handler(_unitOfWorkMock.Object, _elasticSearchRepositoryMock.Object, _operationProducerMock.Object);
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenEmployeeIdIsEmpty()
        {
            var command = new UpdatePermissionCommandHandler.Command
            {
                EmployeeId = Guid.Empty,
                PermissionTypeId = Guid.NewGuid()
            };

            Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(command, CancellationToken.None));
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenPermissionTypeIdIsEmpty()
        {
            var command = new UpdatePermissionCommandHandler.Command
            {
                EmployeeId = Guid.NewGuid(),
                PermissionTypeId = Guid.Empty
            };

            Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(command, CancellationToken.None));
        }

        [Test]
        public void ShouldThrowPermissionNotFoundExceptionWhenEmployeePermissionDoesNotExists()
        {
            var permissionRepositoryMock = new Mock<IPermissionRepository>();
            permissionRepositoryMock.Setup(x => x.GetEmployeePermission(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(Task.FromResult((Permission)null));

            _unitOfWorkMock.Setup(x => x.GetPermissionRepository()).Returns(permissionRepositoryMock.Object);

            var command = new UpdatePermissionCommandHandler.Command
            {
                EmployeeId = Guid.NewGuid(),
                PermissionTypeId = Guid.NewGuid()
            };

            Assert.ThrowsAsync<PermissionNotFoundException>(async () => await _handler.Handle(command, CancellationToken.None));
        }

        [Test]
        public async Task ShouldUpdatePermissionAndPersistToElasticSearch()
        {
            var command = new UpdatePermissionCommandHandler.Command
            {
                EmployeeId = Guid.NewGuid(),
                PermissionTypeId = Guid.NewGuid(),
                Enabled = true
            };

            var permissionRepositoryMock = new Mock<IPermissionRepository>();

            permissionRepositoryMock.Setup(x => x.GetEmployeePermission(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(Task.FromResult(new Permission
            {
                EmployeeId = command.EmployeeId,
                CreatedOn = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                PermissionTypeId = command.PermissionTypeId,
                Enabled = false
            }));

            _unitOfWorkMock.Setup(x => x.GetPermissionRepository()).Returns(permissionRepositoryMock.Object);

            await _handler.Handle(command, CancellationToken.None);

            permissionRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Permission>(p => p.EmployeeId == command.EmployeeId && p.PermissionTypeId == command.PermissionTypeId && p.Enabled == command.Enabled)), Times.Once);
            _elasticSearchRepositoryMock.Verify(x => x.PersistAsync(It.Is<Permission>(p => p.EmployeeId == command.EmployeeId && p.PermissionTypeId == command.PermissionTypeId && p.Enabled == command.Enabled)), Times.Once);
        }

        [Test]
        public async Task ShouldSendOperationAsync()
        {
            var command = new UpdatePermissionCommandHandler.Command
            {
                EmployeeId = Guid.NewGuid(),
                PermissionTypeId = Guid.NewGuid(),
                Enabled = true
            };

            var permissionRepositoryMock = new Mock<IPermissionRepository>();

            permissionRepositoryMock.Setup(x => x.GetEmployeePermission(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(Task.FromResult(new Permission
            {
                EmployeeId = command.EmployeeId,
                CreatedOn = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                PermissionTypeId = command.PermissionTypeId,
                Enabled = false
            }));

            _unitOfWorkMock.Setup(x => x.GetPermissionRepository()).Returns(permissionRepositoryMock.Object);

            await _handler.Handle(command, CancellationToken.None);

            _operationProducerMock.Verify(x => x.SendOperationAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ShouldHandleExceptionWhenPersistingToElasticSearch()
        {
            var command = new UpdatePermissionCommandHandler.Command
            {
                EmployeeId = Guid.NewGuid(),
                PermissionTypeId = Guid.NewGuid(),
                Enabled = true
            };

            var permissionRepositoryMock = new Mock<IPermissionRepository>();

            permissionRepositoryMock.Setup(x => x.GetEmployeePermission(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(Task.FromResult(new Permission
            {
                EmployeeId = command.EmployeeId,
                CreatedOn = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                PermissionTypeId = command.PermissionTypeId,
                Enabled = false
            }));

            _unitOfWorkMock.Setup(x => x.GetPermissionRepository()).Returns(permissionRepositoryMock.Object);

            _elasticSearchRepositoryMock.Setup(x => x.PersistAsync(It.IsAny<Permission>())).ThrowsAsync(new Exception("ElasticSearch error"));

            await _handler.Handle(command, CancellationToken.None);

            _elasticSearchRepositoryMock.Verify(x => x.PersistAsync(It.IsAny<Permission>()), Times.Once);
            _operationProducerMock.Verify(x => x.SendOperationAsync(It.IsAny<string>()), Times.Once);
        }
    }
}
