using AutoFixture;
using Castle.Core.Logging;
using CRUDExample.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDTests
{
    public class PersonsControllerTests
    {
        private readonly ICountriesService _countriesService;
        private readonly Mock<ICountriesService> _countriesServiceMock;

        private readonly IPersonsGetterService _personsGetterService;
        private readonly Mock<IPersonsGetterService> _personsGetterMock;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly Mock<IPersonsAdderService> _personsAdderMock;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly Mock<IPersonsUpdaterService> _personsUpdaterMock;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly Mock<IPersonsDeleterService> _personsDeleterMock;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly Mock<IPersonsSorterService> _personsSorterMock;



        private readonly Fixture _autoFixture;

        public PersonsControllerTests()
        {
            _autoFixture = new Fixture();

            _countriesServiceMock = new Mock<ICountriesService>();

            _countriesService = _countriesServiceMock.Object;

            _personsGetterMock = new Mock<IPersonsGetterService>();
            _personsGetterService = _personsGetterMock.Object;
            _personsDeleterMock = new Mock<IPersonsDeleterService>();
            _personsDeleterService = _personsDeleterMock.Object;
            _personsUpdaterMock = new Mock<IPersonsUpdaterService>();
            _personsUpdaterService = _personsUpdaterMock.Object;
            _personsAdderMock = new Mock<IPersonsAdderService>();
            _personsAdderService = _personsAdderMock.Object;
            _personsSorterMock = new Mock<IPersonsSorterService>();
            _personsSorterService = _personsSorterMock.Object;

        }
        #region Index

        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            //Arrange

            List<PersonResponse> personslist = _autoFixture.Create<List<PersonResponse>>();

            var logger = new Mock<ILogger<PersonsController>>();
            PersonsController personsController = new PersonsController(_personsGetterService,_personsDeleterService,_personsAdderService,_personsSorterService,_personsUpdaterService,_countriesService, logger.Object);

            _personsGetterMock.Setup(x=>x.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(personslist);

            _personsSorterMock.Setup(x => x.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderoptions>())).ReturnsAsync(personslist);

            //Act
            IActionResult result = await personsController.Index(_autoFixture.Create<string>(), _autoFixture.Create<string>(), _autoFixture.Create<string>(), _autoFixture.Create<SortOrderoptions>());

            //Assert
           ViewResult viewresult =  Assert.IsType<ViewResult>(result);
            viewresult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();

            viewresult.ViewData.Model.Should().Be(personslist);
        }

        #endregion

        //#region Create
        //[Fact]
        //public async Task Create_IfModelValidationError_ToReturnCreateView()
        //{
        //    //Arrange
        //    PersonAddRequest personAddRequest = _autoFixture.Create<PersonAddRequest>();

        //    PersonResponse personslist = _autoFixture.Create<PersonResponse>();

        //    List<CountryResponse> countries = _autoFixture.Create<List<CountryResponse>>();

        //    var logger = new Mock<ILogger<PersonsController>>();

        //    PersonsController personsController = new PersonsController(_personsService, _countriesService, logger.Object);


        //    _countriesServiceMock.Setup(x => x.GetAllCountry()).ReturnsAsync(countries);

        //    _personsServiceMock.Setup(x=>x.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(personslist);

        //    //Act
        //    personsController.ModelState.AddModelError("PersonName", "PersonName can't be blank");

        //    IActionResult result = await personsController.Create(personAddRequest);

        //    //Assert
        //    ViewResult viewresult = Assert.IsType<ViewResult>(result);
        //    viewresult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();

        //    viewresult.ViewData.Model.Should().Be(personAddRequest);    


        //}

        [Fact]
        public async Task Create_IfNoModelValidationError_ToReturnIndexView()
        {
            //Arrange
            PersonAddRequest personAddRequest = _autoFixture.Create<PersonAddRequest>();

            PersonResponse personslist = _autoFixture.Create<PersonResponse>();

            List<CountryResponse> countries = _autoFixture.Create<List<CountryResponse>>();

            var logger = new Mock<ILogger<PersonsController>>();

            PersonsController personsController = new PersonsController(_personsGetterService,_personsDeleterService,_personsAdderService,_personsSorterService,_personsUpdaterService, _countriesService,logger.Object);


            _countriesServiceMock.Setup(x => x.GetAllCountry()).ReturnsAsync(countries);

            _personsAdderMock.Setup(x => x.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(personslist);

            //Act

            IActionResult result = await personsController.Create(personAddRequest);

            //Assert
            RedirectToActionResult redirectresult = Assert.IsType<RedirectToActionResult>(result);
            redirectresult.ActionName.Should().Be("Index");        

        }
        //#endregion


    }
}
