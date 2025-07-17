using Moq;
using System.Data;
using ConstructoraClean.Infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace ConstructoraClean.Api.Tests.Helpers
{
    public static class DatabaseMockHelper
    {
        public static Mock<DapperContext> CreateMockContext()
        {
            var mockConfig = new Mock<IConfiguration>();
            
            // En lugar de mockear el método de extensión GetConnectionString, mockeamos el indexador que usa internamente
            mockConfig.Setup(c => c["ConnectionStrings:DefaultConnection"])
                     .Returns("Host=localhost;Database=test;Username=test;Password=test");
            
            var mockContext = new Mock<DapperContext>(mockConfig.Object);
            return mockContext;
        }

        public static (Mock<DapperContext>, Mock<IDbConnection>) CreateMockContextWithConnection()
        {
            var mockContext = CreateMockContext();
            var mockConnection = new Mock<IDbConnection>();
            
            mockContext.Setup(c => c.CreateConnection()).Returns(mockConnection.Object);
            
            return (mockContext, mockConnection);
        }

        public static Mock<IDbConnection> SetupSuccessfulConnection()
        {
            var mockConnection = new Mock<IDbConnection>();
            var mockCommand = new Mock<IDbCommand>();
            
            // Setup básico del comando
            mockCommand.Setup(c => c.ExecuteScalar()).Returns(1);
            mockCommand.SetupProperty(c => c.CommandText);
            mockCommand.Setup(c => c.Dispose());
            mockCommand.Setup(c => c.Connection).Returns(mockConnection.Object);
            mockCommand.Setup(c => c.Transaction).Returns((IDbTransaction?)null);
            mockCommand.Setup(c => c.CommandType).Returns(CommandType.Text);
            mockCommand.Setup(c => c.CommandTimeout).Returns(30);
            mockCommand.Setup(c => c.Parameters).Returns(new Mock<IDataParameterCollection>().Object);
            mockCommand.Setup(c => c.UpdatedRowSource).Returns(UpdateRowSource.None);
            
            // Setup de la conexión
            mockConnection.Setup(c => c.Open());
            mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);
            mockConnection.Setup(c => c.Dispose());
            
            return mockConnection;
        }

        public static Mock<IDbConnection> SetupFailingConnection()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(c => c.Open()).Throws(new InvalidOperationException("Database connection failed"));
            return mockConnection;
        }
    }
} 