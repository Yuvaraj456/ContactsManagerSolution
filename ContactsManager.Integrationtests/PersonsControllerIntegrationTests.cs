using Azure;
using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CRUDTests
{
    public class PersonsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;


        public PersonsControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        #region Index
        [Fact]   
        public async Task Index_ToReturnView()
        {
            //Arrange

            //Act

           HttpResponseMessage httpResponse = await _httpClient.GetAsync("Persons/Index");

            //Assert
            httpResponse.Should().BeSuccessful();//status code 2XX like 200 series  

            string bodyContent = await httpResponse.Content.ReadAsStringAsync();

            HtmlDocument html = new HtmlDocument();

            html.LoadHtml(bodyContent);

            var document = html.DocumentNode;

            document.QuerySelectorAll("table.persons").Should().NotBeNull();
        }
        #endregion
    }
}
