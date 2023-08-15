using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;
using Moq;
using AutoFixture;
using FluentAssertions;
using RepositoryContracts;
using System.Linq.Expressions;
using Serilog;
using ContactsManager.Core.Domain.Entities;

namespace CRUDTests
{
    public class PersonsServiceTests 
    {
        //Private field
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsSorterService _personsSorterService;


        private readonly Mock<IPersonsRepository> _personRepositoryMock;
        private readonly IPersonsRepository _personsRepository;
       

        private readonly ITestOutputHelper _testoutputHelper;

        private readonly IFixture _fixture;

        //construtor
        public PersonsServiceTests(ITestOutputHelper testOutputHelper)
        {

            _personRepositoryMock = new Mock<IPersonsRepository>();

            _personsRepository = _personRepositoryMock.Object;

            var diagnosticContextMock = new Mock<IDiagnosticContext>();

            var loggerGetter = new Mock<Microsoft.Extensions.Logging.ILogger<PersonsGetterService>>();
            var loggerAdder = new Mock<Microsoft.Extensions.Logging.ILogger<PersonsAdderService>>();
            var loggerDeleter = new Mock<Microsoft.Extensions.Logging.ILogger<PersonsDeleterService>>();
            var loggerUpdater = new Mock<Microsoft.Extensions.Logging.ILogger<PersonsUpdaterService>>();
            var loggerSorter = new Mock<Microsoft.Extensions.Logging.ILogger<PersonsSorterService>>();

            _personsGetterService = new PersonsGetterService(_personsRepository, loggerGetter.Object, diagnosticContextMock.Object);
            _personsAdderService = new PersonsAdderService(_personsRepository, loggerAdder.Object, diagnosticContextMock.Object);
            _personsUpdaterService = new PersonsUpdaterService(_personsRepository, loggerUpdater.Object, diagnosticContextMock.Object);
            _personsDeleterService = new PersonsDeleterService(_personsRepository, loggerDeleter.Object, diagnosticContextMock.Object);
            _personsSorterService = new PersonsSorterService(_personsRepository, loggerSorter.Object, diagnosticContextMock.Object);



            _testoutputHelper = testOutputHelper;
            _fixture = new Fixture();

            ////MockDbContext
            //List<Country> countriesinitialdata = new List<Country>() { };

            //List<Person> Personsinitialdata = new List<Person>() { };


            //DbContextMock<ApplicationDbContext> dbContextMock =
            //    new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);



            ////Mockbject
            //ApplicationDbContext dbContext = dbContextMock.Object;

            //dbContextMock.CreateDbSetMock(temp => temp.Persons, Personsinitialdata);
            //dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesinitialdata);


        }

        #region Add Person
        //when we supply null value as PersonAddRequest, it should return ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            //Arrange
            PersonAddRequest? request = null;


            //Assert 

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                //Act
               await _personsAdderService.AddPerson(request);
            });
        }

        //when we supply null value as PersonName, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange
            PersonAddRequest? request = _fixture.Build<PersonAddRequest>()
                .With(x=>x.PersonName, null as string)
                .Create();

            Person person = request.ToPerson();

            //When PersonRepository.AddPerson is called, it has to return the same "person" object
            _personRepositoryMock.Setup(x=>x.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                await _personsAdderService.AddPerson(request);
            });
        }

        //when we supply Proper Person details, it should insert the person into the persons list; 
        //and it should return an object of PersonResponse, Which includes with the newly generated person id
        [Fact]
        public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
        {
            //Arrange
            //PersonAddRequest? request = new PersonAddRequest()
            //{
            //    PersonName = "Yuvaraj",
            //    Address = "No 1/114 RajivGandhi Nagar, Semmanchery",
            //    Email = "yuvarajryd1@gmail.com",
            //    DateOfBirth = Convert.ToDateTime("14-10-1999"),
            //    Gender = GenderOptions.Male,
            //    CountryId = Guid.NewGuid(),
            //    ReceiveNewsLetters = true,

            //};

            PersonAddRequest? request = _fixture.Build<PersonAddRequest>()
                .With(temp=>temp.Email, "Someone@gmail.com")
                .Create();

            Person person = request.ToPerson();

            PersonResponse Exp_Response = person.ToPersonResponse();

            //If we supply any argument value to the addPerson method, it should return same return value
            _personRepositoryMock.Setup(x => x.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);


            //Act
            PersonResponse personResponse = await _personsAdderService.AddPerson(request);

            Exp_Response.PersonId = personResponse.PersonId;

            //Assert
            personResponse.PersonId.Should().NotBe(Guid.Empty);

            personResponse.Should().Be(Exp_Response);


        }

        #endregion

        #region getPersonById
        [Fact]
        public async Task GetPersonById_NullPersonId_ToBeNull()
        {
            //Arrange
            Guid? personId = null;

            //Act
            PersonResponse? response = await _personsGetterService.GetPersonById(personId);

            //Assert
            Assert.Null(response);

        }

        [Fact]
        public async Task GetPersonById_WithPersonId_ToBeSuccessful()
        {
            

           //Arrange
            Person person = _fixture.Build<Person>()
                .With(x=>x.Email, "Yuvaraj@gmail.com")
                .With(x=>x.Country, null as Country)
                .Create();

            PersonResponse Exp_personResponse = person.ToPersonResponse();

            _personRepositoryMock.Setup(x=>x.GetPersonById(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            //Act
            PersonResponse? personResponse_from_add = await _personsGetterService.GetPersonById(person.PersonId);
                

            //Assert
            personResponse_from_add.Should().Be(Exp_personResponse);

        }

        #endregion

        #region getAllPersons

        //the GetAllpersons() should return empty list by default
        [Fact]
        public async Task GetAllPersons_ToBeEmptyList()
        {
            //Arrange
            List<Person> person = new List<Person>();
            _personRepositoryMock.Setup(x => x.GetAllPersons())
                .ReturnsAsync(person);

            //Act
            List<PersonResponse>? persons = await _personsGetterService.GetAllPersons();

            //Assert
            Assert.Empty(persons);
        }

        //First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
        [Fact]
        public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
        {
            //Arrange
            List<Person> peoples = new List<Person>() {

                _fixture.Build<Person>()
                .With(x => x.Email, "Yuvaraj@gmail.com")
                .With(x=>x.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(x => x.Email, "Mary@gmail.com")
                .With(x=>x.Country, null as Country)
                .Create(),

                _fixture.Build<Person>()
                .With(x => x.Email, "Moni@gmail.com")
                .With(x=>x.Country, null as Country)
                .Create()
            };

            List<PersonResponse> Exp_personResponses =  peoples.Select(x => x.ToPersonResponse()).ToList();
           
            //print personResponses_from_list 
            _testoutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in Exp_personResponses)
            {
                _testoutputHelper.WriteLine(person.ToString());
            }

            //Repository mocking
            _personRepositoryMock.Setup(x => x.GetAllPersons())
                .ReturnsAsync(peoples);

            //Act
            List<PersonResponse> getallpersons = await _personsGetterService.GetAllPersons();

            //print getallpersons 
            _testoutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person in getallpersons)
            {
                _testoutputHelper.WriteLine(person.ToString());
            }

            //Assert
            getallpersons.Should().BeEquivalentTo(Exp_personResponses);


        }

        #endregion

        #region GetFilteredPersons


        //If the search text is empty and search by it "PersonName", it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchTest_ToReturnAllPersons()
        {

            //Arrange          
            List<Person> peoples = new List<Person>() {

            _fixture.Build<Person>()
                .With(x => x.Email, "Yuvaraj@gmail.com")
                .With(x=>x.Country, null as Country)
                .Create(),

            _fixture.Build<Person>()
                .With(x => x.Email, "Mary@gmail.com")
                .With(x=>x.Country, null as Country)
                .Create(),

            _fixture.Build<Person>()
                .With(x => x.Email, "Moni@gmail.com")
                .With(x=>x.Country, null as Country)
                .Create()
            };

            List<PersonResponse> Exp_personResponses_from_list = peoples.Select(x => x.ToPersonResponse()).ToList();
            

            //print personResponses_from_list 
            _testoutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in Exp_personResponses_from_list)
            {
                _testoutputHelper.WriteLine(person.ToString());
            }

            _personRepositoryMock.Setup(x=>x.GetFilteredPersons(It.IsAny<Expression<Func<Person,bool>>>()))
                .ReturnsAsync(peoples);

            _personRepositoryMock.Setup(x => x.GetAllPersons()).ReturnsAsync(peoples);
            //Act
            List<PersonResponse> getallpersons_get = await _personsGetterService.GetFilteredPersons(nameof(Person.PersonName), "");

            //print getallpersons 
            _testoutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person in getallpersons_get)
            {
                _testoutputHelper.WriteLine(person.ToString());
            }

            //Assert
            foreach (PersonResponse person in getallpersons_get)
            {
                Assert.Contains(person, Exp_personResponses_from_list);
            }


        }

        //First we will add few persons; and then we will search based on person name with some search string. 
        //it should return the matching person.
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {

            //Arrange          
            List<Person> peoples = new List<Person>() {

            _fixture.Build<Person>()
                .With(x => x.Email, "Yuvaraj@gmail.com")
                .With(x=>x.Country, null as Country)
                .Create(),

            _fixture.Build<Person>()
                .With(x => x.Email, "Mary@gmail.com")
                .With(x=>x.Country, null as Country)
                .Create(),

            _fixture.Build<Person>()
                .With(x => x.Email, "Moni@gmail.com")
                .With(x=>x.Country, null as Country)
                .Create()
            };

            List<PersonResponse> Exp_personResponses_from_list = peoples.Select(x => x.ToPersonResponse()).ToList();


            //print personResponses_from_list 
            _testoutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in Exp_personResponses_from_list)
            {
                _testoutputHelper.WriteLine(person.ToString());
            }

            _personRepositoryMock.Setup(x => x.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(peoples);

            _personRepositoryMock.Setup(x => x.GetAllPersons()).ReturnsAsync(peoples);
            //Act
            List<PersonResponse> getallpersons_get = await _personsGetterService.GetFilteredPersons(nameof(Person.Email), "YU");

            //print getallpersons 
            _testoutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person in getallpersons_get)
            {
                _testoutputHelper.WriteLine(person.ToString());
            }

            //Assert
            foreach (PersonResponse person in getallpersons_get)
            {
                Assert.Contains(person, Exp_personResponses_from_list);
            }

        }

        #endregion

        #region GetSortedPersons
        //When we sort Personname in Desc order, it should return persons list in descending on PersonName
        [Fact]
        public async Task GetSortedPersons_ToBeSuccessful()
        {
            // Arrange
            List<Person> peoples = new List<Person>() {

            _fixture.Build<Person>()
                .With(x => x.Email, "Yuvaraj@gmail.com")
                .With(x=>x.Country, null as Country)
                .Create(),

            _fixture.Build<Person>()
                .With(x => x.Email, "Mary@gmail.com")
                .With(x=>x.Country, null as Country)
                .Create(),

            _fixture.Build<Person>()
                .With(x => x.Email, "Moni@gmail.com")
                .With(x=>x.Country, null as Country)
                .Create()
            };

            List<PersonResponse> Exp_personResponses_from_list = peoples.Select(x => x.ToPersonResponse()).ToList();
           
            //print personResponses_from_list 
            _testoutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person in Exp_personResponses_from_list)
            {
                _testoutputHelper.WriteLine(person.ToString());
            }

            _personRepositoryMock.Setup(x => x.GetAllPersons()).ReturnsAsync(peoples);

            List<PersonResponse> allpersons = await _personsGetterService.GetAllPersons();


            //Act
            List<PersonResponse> persons_list_from_sort = await _personsSorterService.GetSortedPersons(allpersons, nameof(Person.PersonName), SortOrderoptions.DESC);


            //print getallpersons 
            _testoutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person in persons_list_from_sort)
            {
                _testoutputHelper.WriteLine(person.ToString());
            }


            //Assert
            persons_list_from_sort.Should().BeInDescendingOrder(x => x.PersonName);
        }


        #endregion



        #region UpdatePerson
        //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
           {
               //Act
               await _personsUpdaterService.UpdatePerson(personUpdateRequest);
           });



        }

        //when we supply invalid Personid , should throws argumentexception.
        [Fact]
        public async Task UpdatePerson_InvalidPersonId_ToBeArgumentException()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = _fixture.Create<PersonUpdateRequest>();
               

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                await _personsUpdaterService.UpdatePerson(personUpdateRequest);
            });

        }

        //when PersonName is null, should throws argumentexception.
        [Fact]
        public async Task UpdatePerson_PersonNameNull_ToBeArgumentException()
        {
            //Arrange

            Person personAddRequest = _fixture.Build<Person>()
                .With(x => x.Gender, "Male")
                .With(x=>x.PersonName, null as string)
                .With(x => x.Country, null as Country)
                .Create();               

            PersonResponse personResponse = personAddRequest.ToPersonResponse();

            PersonUpdateRequest? personUpdateRequest = personResponse.ToPersonUpdateRequest();


            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                await _personsUpdaterService.UpdatePerson(personUpdateRequest);
            });

        }

        //First add a new person and try to update the personname and email.
        [Fact]
        public async Task UpdatePerson_PersonFullDetails_ToBeSuccessful()
        {
            //Arrange
            Person personAddRequest = _fixture.Build<Person>()
                .With(x=>x.Gender, "Male")
                .With(x => x.Email, "yuvarajryd1@gmail.com")
                .With(x => x.Country, null as Country)
                .Create();

            PersonResponse Exp_personResponse = personAddRequest.ToPersonResponse();

            PersonUpdateRequest? personUpdateRequest = Exp_personResponse.ToPersonUpdateRequest();
       
            _personRepositoryMock.Setup(x=>x.UpdatePerson(It.IsAny<Person>()))
                .ReturnsAsync(personAddRequest);

            _personRepositoryMock.Setup(x => x.GetPersonById(It.IsAny<Guid>()))
                .ReturnsAsync(personAddRequest);

            //Act
            PersonResponse person_response_from_update = await _personsUpdaterService.UpdatePerson(personUpdateRequest);

            //Assert

            Assert.Equal(Exp_personResponse, person_response_from_update);

        }
        #endregion



        #region DeletePerson

        //If we supply an valid personId, it should return true
        [Fact]
        public async Task DeletePerson_validPersonId_ToBeSuccessful()
        {
            //Arrange     


            Person personAddRequest = _fixture.Build<Person>()
                .With(x=>x.Email, "Yuvaraj@gmail.com")
                .With(x => x.Country, null as Country)
                .With(x => x.Gender, "Male")
                .Create();

            _personRepositoryMock.Setup(x => x.DeletePersonById(It.IsAny<Guid>())).ReturnsAsync(true);

            _personRepositoryMock.Setup(x => x.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(personAddRequest);

            //Act

            bool Isdeleted = await _personsDeleterService.DeletePerson(personAddRequest.PersonId);


            //Assert

            Assert.True(Isdeleted);

        }

        //If we supply an invalid personId, it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonId_ToBeFalse()
        {
                        
            //Act

            bool Isdeleted = await _personsDeleterService.DeletePerson(Guid.NewGuid());


            //Assert

            Assert.False(Isdeleted);
 
        }
        #endregion
    }

}



































































