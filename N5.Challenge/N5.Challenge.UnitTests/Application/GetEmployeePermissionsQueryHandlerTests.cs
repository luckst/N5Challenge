using Moq;
using N5.Challenge.Application.Commands;
using N5.Challenge.Application.Queries;
using N5.Challenge.Domain;
using N5.Challenge.Entities.Dtos;
using N5.Challenge.Infrasctructure.ElasticSearch;
using N5.Challenge.Infrasctructure.Exceptions;
using N5.Challenge.Infrasctructure.KafkaConfig.Producers;
using N5.Challenge.Infrasctructure.Repositories;
using N5.Challenge.Infrasctructure.RepositoryPattern;

namespace N5.Challenge.UnitTests.Application
{
    [TestFixture]
    public class GetEmployeePermissionsQueryHandlerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IOperationProducer> _operationProducerMock;
        private GetEmployeePermissionsQueryHandler.Handler _handler;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _operationProducerMock = new Mock<IOperationProducer>();
            _handler = new GetEmployeePermissionsQueryHandler.Handler(_unitOfWorkMock.Object, _operationProducerMock.Object);
        }

        [Test]
        public async Task ShouldReturnEmployeePermissionsList()
        {
            var query = new GetEmployeePermissionsQueryHandler.Query(Guid.NewGuid());

            var permissionRepositoryMock = new Mock<IPermissionRepository>();
            permissionRepositoryMock.Setup(x => x.GetEmployeePermissions(It.IsAny<Guid>())).Returns(Task.FromResult(new List<EmployeePermissionDto>
            {
                new EmployeePermissionDto
                {
                    EmployeeId = query.EmployeeId,
                    EmployeeName = "Test 1",
                    Enabled = true,
                    PermissionTypeId  = Guid.NewGuid(),
                    PermissionTypeName = "Permission Name"
                }
            }));

            _unitOfWorkMock.Setup(x => x.GetPermissionRepository()).Returns(permissionRepositoryMock.Object);

            var response = await _handler.Handle(query, CancellationToken.None);

            permissionRepositoryMock.Verify(x => x.GetEmployeePermissions(It.IsAny<Guid>()), Times.Once);
            _operationProducerMock.Verify(x => x.SendOperationAsync(It.IsAny<string>()), Times.Once);
            Assert.That(response.Count, Is.EqualTo(1));
        }

        [Test]
        public void ShouldThrowApplicationExceptionWhenErrorOccurs()
        {
            var query = new GetEmployeePermissionsQueryHandler.Query(Guid.NewGuid());

            var permissionRepositoryMock = new Mock<IPermissionRepository>();
            permissionRepositoryMock.Setup(x => x.GetEmployeePermissions(It.IsAny<Guid>())).ThrowsAsync(new Exception("Database error"));

            _unitOfWorkMock.Setup(x => x.GetPermissionRepository()).Returns(permissionRepositoryMock.Object);

            Assert.ThrowsAsync<ApplicationException>(async () => await _handler.Handle(query, CancellationToken.None));
        }

        [Test]
        public async Task ShouldHandleExceptionWhenSendingOperationAsync()
        {
            var query = new GetEmployeePermissionsQueryHandler.Query(Guid.NewGuid());

            var permissionRepositoryMock = new Mock<IPermissionRepository>();
            permissionRepositoryMock.Setup(x => x.GetEmployeePermissions(It.IsAny<Guid>())).Returns(Task.FromResult(new List<EmployeePermissionDto>
            {
                new EmployeePermissionDto
                {
                    EmployeeId = query.EmployeeId,
                    EmployeeName = "Test 1",
                    Enabled = true,
                    PermissionTypeId  = Guid.NewGuid(),
                    PermissionTypeName = "Permission Name"
                }
            }));

            _unitOfWorkMock.Setup(x => x.GetPermissionRepository()).Returns(permissionRepositoryMock.Object);

            _operationProducerMock.Setup(x => x.SendOperationAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Kafka error"));

            var response = await _handler.Handle(query, CancellationToken.None);

            permissionRepositoryMock.Verify(x => x.GetEmployeePermissions(It.IsAny<Guid>()), Times.Once);
            _operationProducerMock.Verify(x => x.SendOperationAsync(It.IsAny<string>()), Times.Once);
            Assert.That(response.Count, Is.EqualTo(1));
        }
    }
}
