using ServiceContracts;
using Services;
using ServiceContracts.DTO;
using Moq;
using AutoFixture;
using RepositoryContracts;
using FluentAssertions;
using ContactsManager.Core.Domain.Entities;

namespace CRUDTests
{
    public class CountryServiceTest
    {
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;

        private readonly ICountriesRepository _countriesRepository;

        private readonly ICountriesService _countryService;



        private readonly IFixture _fixture;

        //constructor
        public CountryServiceTest()
        {
            _countriesRepositoryMock = new Mock<ICountriesRepository>();

            _countriesRepository = _countriesRepositoryMock.Object;

            _countryService = new CountriesService(_countriesRepository);


            _fixture = new Fixture();

            //MockDbContext
            //List<Country> countriesInitialData = new List<Country>() { };

            ////MockObject
            //DbContextMock<ApplicationDbContext> dbContextMock = 
            //    new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            //ApplicationDbContext dbContext = dbContextMock.Object;

            //dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);           


        }

        #region AddCountry
        //when CountryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public  async Task AddCountry_NullCountry_ToBeArgumentNullException() //MethodName_whattodo_whatweexpect
        {
            //Arrange
            CountryAddRequest? addRequest = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                //Act
               await _countryService.AddCountry(addRequest);

            });
        }

        //When the CountryName is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_IfCountryNameNull_ToBeArgumentException() //MethodName_whattodo
        {
            //Arrange
            CountryAddRequest? addRequest = _fixture.Build<CountryAddRequest>()
                .With(x=>x.CountryName, null as string)
                .Create();

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                await _countryService.AddCountry(addRequest);

            });
        }

        //When the CountryName is Duplicate, it should throw ArgumentException

        [Fact]
        public async Task AddCountry_IfCountryNameDuplicate_ToBeArgumentException() //MethodName_whattodo
        {
            //Arrange
            CountryAddRequest? firstrequest = _fixture.Build<CountryAddRequest>()
                .With(x => x.CountryName, "India")                
                .Create();
        
            CountryAddRequest? secondrequest = _fixture.Build<CountryAddRequest>()
                .With(x => x.CountryName, "India")
                .Create();

            Country firstcountry = firstrequest.ToCountry();

            Country secondcountry = secondrequest.ToCountry();

            _countriesRepositoryMock.Setup(x => x.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(firstcountry);

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                await _countryService.AddCountry(firstrequest);
                await _countryService.AddCountry(secondrequest);
            });
        }

        //When you supply proper CountryName, it should insert (add) the country to the existing list of countries

        [Fact]
        public async Task AddCountry_ifAddCountrySuccess() //MethodName_whattodo
        {
            //Arrange
            CountryAddRequest? addRequest1 = _fixture.Build<CountryAddRequest>()
                .With(x => x.CountryName, "japan")
                .Create(); ;


            //Act
            CountryResponse response = await _countryService.AddCountry(addRequest1);

            //Assert

            Assert.True(response.CountryId != Guid.Empty) ;
            Assert.True(response.CountryName != string.Empty) ;
            response.Should().Be(response);


        }
        #endregion
        #region GetAllCountries
        
        //The list of countries empty by default (before adding any countries)

        [Fact]
        public async Task GetAllCounries_CheckEmpty_ToBEEmpty()
        {
            List<Country> country = new List<Country>();
            _countriesRepositoryMock.Setup(x => x.GetAllCountries()).ReturnsAsync(country);
            
            //Act
            List<CountryResponse> response = await _countryService.GetAllCountry();

            //Assert
            Assert.Empty(response); //this object is empty the test is pass
           
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<CountryAddRequest> countryAddRequests = new List<CountryAddRequest>()
            {
                 _fixture.Build<CountryAddRequest>()
                .With(x => x.CountryName, "India")
                .Create(),

                _fixture.Build<CountryAddRequest>()
               .With(x => x.CountryName, "japan")
               .Create()
            };

            List<Country> country = countryAddRequests.Select(x => x.ToCountry()).ToList(); 

            List<CountryResponse> Exp_Response = country.Select(x=>x.ToCountryResponse()).ToList();

            _countriesRepositoryMock.Setup(x => x.GetAllCountries()).ReturnsAsync(country);


            //Act
          
            List<CountryResponse> actualcountryResponses = await _countryService.GetAllCountry();

            //read each element from countries list from add country
            foreach(CountryResponse expected_countryResponse in Exp_Response)
            {
                Assert.Contains(expected_countryResponse, actualcountryResponses); //internally calls equels method
            }

            //Assert
        }
        #endregion
        #region GetCountryById

        //if we supply null Countryid, it should return null as a countryresponse
        [Fact]
        public async Task GetCountryById_NullCheck_ToBeNull()
        {
            //Arrange
            Guid? id = null;

            //Act
            CountryResponse? response = await _countryService.GetCountryById(id);

            //Assert    
            Assert.Null(response);

        }

        [Fact]
        //if we supply a valid countryid it should return matching country details as CountryResponse object 
        public async Task GetCountryById_ValidId_ValidResponse()
        {

            //Arrange
            CountryAddRequest addrequest = _fixture.Build<CountryAddRequest>()
                .With(x => x.CountryName, "India")
                .Create();

            Country country = addrequest.ToCountry();

            CountryResponse Exp_response = country.ToCountryResponse();

            _countriesRepositoryMock.Setup(x => x.GetCountryById(It.IsAny<Guid>())).ReturnsAsync(country);
            
            //act
           CountryResponse? getbyid = await _countryService.GetCountryById(country.CountryId);


            //assert
            Assert.Equal(Exp_response, getbyid);
        }
        #endregion
    }
}