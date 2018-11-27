using System;
using LenesKlinik.Core.ApplicationServices;
using LenesKlinik.Core.ApplicationServices.Implementation;
using LenesKlinik.Core.DomainServices;
using LenesKlinik.Core.Entities;
using Moq;
using Xunit;

namespace WorkTest
{
    public class WorkServiceTest
    {
        [Fact]
        public void CreateWorkSuccessTest()
        {

            Work w = new Work
            {
                Title = "Massage",
                Description = "A nice massage on location",
                Duration = 30,
                Price = 200
            };

            var mock = new Mock<IWorkRepository>();
            IWorkService service = new WorkService(mock.Object);
            mock.Setup(repo => repo.CreateWork(It.IsAny<Work>())).Returns(new Work
            {
                Id = 1,
                Title = w.Title,
                Description = w.Description,
                Duration = w.Duration,
                Price = w.Price,
                ImageUrl = "url.png"
            });
            var returnWork = service.CreateWork(w);

            mock.Verify(repo => repo.CreateWork(w), Times.Once);
            Assert.Equal(1, returnWork.Id);
        }

        [Fact]
        public void CreateWorkNoTitleExpectArgumentExceptionTest()
        {
            IWorkService service = GetWorkService();
            Work w = new Work
            {
                Description = "A nice massage on location",
                Duration = 30,
                Price = 200,
                ImageUrl = "url.png"
            };
            Exception e = Assert.Throws<ArgumentException>(() => service.CreateWork(w));

            Assert.Equal("Title empty or null!", e.Message);
        }

        [Fact]
        public void CreateWorkNoDescriptionExpectArgumentExceptionTest()
        {
            IWorkService service = GetWorkService();
            Work w = new Work
            {
                Title = "Massage",
                Duration = 30,
                Price = 200,
                ImageUrl = "url.png"
            };

            Exception e = Assert.Throws<ArgumentException>(() => service.CreateWork(w));
            Assert.Equal("Description empty or null!", e.Message);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void CreateWorkInvalidDurationExpectArgumentOutOfRangeExceptionTest(int duration)
        {
            IWorkService service = GetWorkService();
            Work w = new Work
            {
                Title = "Massage",
                Description = "A nice massage",
                Duration = duration,
                Price = 200.00,
                ImageUrl = "url.png"
            };
            Exception e = Assert.Throws<ArgumentOutOfRangeException>(() => service.CreateWork(w));
            Assert.Contains("Duration cannot be 0 or less!", e.Message);
        }

        [Theory]
        [InlineData(-1.00)]
        [InlineData(0.00)]
        public void CreateWorkInvalidPriceExpectArgumentOutOfRangeExceptionTest(double price)
        {
            IWorkService service = GetWorkService();
            Work w = new Work
            {
                Title = "Massage",
                Description = "A nice massage",
                Duration = 30,
                Price = price,
                ImageUrl = "url.png"
            };
            Exception e = Assert.Throws<ArgumentOutOfRangeException>(() => service.CreateWork(w));
            Assert.Contains("Price cannot be 0 or less!", e.Message);
        }
    

    private IWorkService GetWorkService()
        {
            var mock = new Mock<IWorkRepository>();
            IWorkService service = new WorkService(mock.Object);
            return service;
        }

    }
    
}