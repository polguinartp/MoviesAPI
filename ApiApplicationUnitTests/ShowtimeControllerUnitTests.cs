using ApiApplication.Controllers;
using ApiApplication.DTOs.API;
using ApiApplication.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiApplicationUnitTests
{
    [TestClass]
    public class ShowtimeControllerUnitTests
    {
        private Mock<IShowtimeService> _service;
        private Mock<IMapper> _mapper;
        private IEnumerable<ShowtimeEntity> _showtimeEntities;

        [TestInitialize]
        public void Initialize()
        {
            _showtimeEntities = new List<ShowtimeEntity>()
            {
                new ShowtimeEntity()
                {
                    Id = 1,
                    Movie = new MovieEntity()
                    {
                        Id = 1,
                        ImdbId = "movie1",
                        Title = "Movie 1"                        
                    }                    
                },
                new ShowtimeEntity()
                {
                    Id = 2,
                    Movie = new MovieEntity()
                    {
                        Id = 2,
                        ImdbId = "movie2",
                        Title = "Movie 2"
                    }
                }
            };

            _service = new Mock<IShowtimeService>();            

            _mapper = new Mock<IMapper>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorShouldThrowArgumenNullExceptionWhenServiceParameterIsNull()
        {
            // Arrange
            var subjectUnderTest = new ShowtimeController(null, _mapper.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorShouldThrowArgumenNullExceptionWhenMapperParameterIsNull()
        {
            // Arrange
            var subjectUnderTest = new ShowtimeController(_service.Object, null);
        }

        [TestMethod]
        public async Task GetShouldReturnOkObjectResult()
        {
            // Arrange
            var showtimeResult = new Showtime()
            {
                Id = 1,
                Movie = new Movie()
                {
                    ImdbId = "movie1",
                    Title = "Movie 1"
                }
            };

            _service.Setup(x => x.GetAsync(It.IsAny<DateTime>(), It.IsAny<string>()))
                .ReturnsAsync((DateTime datetime, string movieTitle) => new List<ShowtimeEntity>() { _showtimeEntities.First() });

            _mapper.Setup(x => x.Map<IEnumerable<Showtime>>(It.IsAny<IEnumerable<ShowtimeEntity>>()))
                .Returns(new List<Showtime>() { showtimeResult });

            // Act
            var subjectUnderTest = new ShowtimeController(_service.Object, _mapper.Object);
            var result = await subjectUnderTest.GetAsync(DateTime.Now, "movie 1");
            
            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task GetShouldReturnExpectedResult()
        {
            // Arrange
            var showtimeResult = new Showtime()
            {
                Id = 1,
                Movie = new Movie()
                {
                    ImdbId = "movie1",
                    Title = "Movie 1"
                }
            };

            _service.Setup(x => x.GetAsync(It.IsAny<DateTime>(), It.IsAny<string>()))
                .ReturnsAsync((DateTime datetime, string movieTitle) => new List<ShowtimeEntity>() { _showtimeEntities.First() });

            _mapper.Setup(x => x.Map<IEnumerable<Showtime>>(It.IsAny<IEnumerable<ShowtimeEntity>>()))
                .Returns(new List<Showtime>() { showtimeResult });

            // Act
            var subjectUnderTest = new ShowtimeController(_service.Object, _mapper.Object);
            
            var result = await subjectUnderTest.GetAsync(DateTime.Now, "movie 1");
            var objectResult = (OkObjectResult) result.Result;            
            var valueResult = (IEnumerable<Showtime>) objectResult.Value;
            
            var expectedResult = new List<Showtime>()
            {
                showtimeResult
            };

            // Assert
            CollectionAssert.AreEquivalent(expectedResult, valueResult.ToList());
        }
    }
}
