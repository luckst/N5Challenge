using Moq;
using N5.Challenge.Application.Commands;
using N5.Challenge.Application.Queries;
using N5.Challenge.Domain;
using N5.Challenge.Entities.Dtos;
using N5.Challenge.Infrasctructure.ElasticSearch;
using N5.Challenge.Infrasctructure.Exceptions;
using N5.Challenge.Infrasctructure.Repositories;
using N5.Challenge.Infrasctructure.RepositoryPattern;

namespace N5.Challenge.UnitTests.Application
{
    [TestFixture]
    public class GetEmployeePermissionsQueryHandlerTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private GetEmployeePermissionsQueryHandler.Handler _handler;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new GetEmployeePermissionsQueryHandler.Handler(_unitOfWorkMock.Object);
        }

        [Test]
        public async Task ShouldReturnEMployeePermissionsList()
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
            Assert.That(response.Count(), Is.EqualTo(1));
        }
    }
}
