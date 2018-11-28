using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly List<Work> _work;
        private readonly IWorkService _service;
        private readonly Mock<IWorkRepository> mock;

        // This constructor is called for each test, so that they may run simultaneously
        public WorkServiceTest()
        {
            _work = GetMockWork().ToList();
            mock = new Mock<IWorkRepository>();
            _service = new WorkService(mock.Object);
            Work w = _work[0];

            mock.Setup(repo => repo.DeleteWork(It.IsAny<int>()));
            mock.Setup(repo => repo.GetAllWork()).Returns(GetMockWork);
            mock.Setup(repo => repo.CreateWork(It.IsAny<Work>())).Returns(new Work
            {
                Id = 1,
                Title = w.Title,
                Description = w.Description,
                Duration = w.Duration,
                Price = w.Price,
                ImageUrl = "url.png"
            });

        }
        #region CREATE
        [Fact]
        public void CreateWorkSuccessTest()
        {
            Work w = _work[0];
            var returnWork = _service.CreateWork(w);
            mock.Verify(repo => repo.CreateWork(w), Times.Once);
            Assert.Equal(1, returnWork.Id);
        }

        [Fact]
        public void CreateWorkNoTitleExpectArgumentExceptionTest()
        {
            Work w = _work[0];
            w.Title = null;
            Exception e = Assert.Throws<ArgumentException>(() => _service.CreateWork(w));
            Assert.Equal("Title empty or null!", e.Message);
            mock.Verify(repo => repo.CreateWork(w), Times.Never);
        }

        [Fact]
        public void CreateWorkNoDescriptionExpectArgumentExceptionTest()
        {
            Work w = _work[0];
            w.Description = null;
            Exception e = Assert.Throws<ArgumentException>(() => _service.CreateWork(w));
            Assert.Equal("Description empty or null!", e.Message);
            mock.Verify(repo => repo.CreateWork(w), Times.Never);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void CreateWorkInvalidDurationExpectArgumentExceptionTest(int duration)
        {
            Work w = _work[0];
            w.Duration = duration;
            Exception e = Assert.Throws<ArgumentException>(() => _service.CreateWork(w));
            Assert.Equal("Duration cannot be 0 or less!", e.Message);
            mock.Verify(repo => repo.CreateWork(w), Times.Never);
        }

        [Theory]
        [InlineData(-1.00)]
        [InlineData(0.00)]
        public void CreateWorkInvalidPriceExpectArgumentExceptionTest(double price)
        {
            Work w = _work[0];
            w.Price = price;
            Exception e = Assert.Throws<ArgumentException>(() => _service.CreateWork(w));
            Assert.Equal("Price cannot be 0 or less!", e.Message);
            mock.Verify(repo => repo.CreateWork(w), Times.Never);
        }

        #endregion

        #region READ
        [Fact]
        public void GetAllWorkSuccessTest()
        {
            var returnWork = _service.GetAllWork();
            mock.Verify(repo => repo.GetAllWork(), Times.Once);
            Assert.Equal(2, returnWork.Count());
        }
        #endregion

        #region DELETE
        [Fact]
        public void DeleteWorkSuccessTest()
        {
            _service.DeleteWork(1);
            mock.Verify(repo => repo.DeleteWork(1), Times.Once);
        }


        #endregion
        private IWorkService GetWorkService()
        {
            return null;
        }

        private IEnumerable<Work> GetMockWork()
        {
            return new List<Work>()
            {
                new Work
                {
                    Id = 1,
                    Title = "Massage",
                    Description = "A nice massage",
                    Duration = 30,
                    Price = 299.99,
                    ImageUrl = "Image.png"
                },
                new Work
                {
                    Id = 2,
                    Title = "Raindrop Massage",
                    Description = "A nice massage",
                    Duration = 30,
                    Price = 299.99,
                    ImageUrl = "Image.png"
                },
            };
        }

    }

}