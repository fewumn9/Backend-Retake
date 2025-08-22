using RevueCrafters.Models;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Text.Json;

namespace RevueCrafters
{
    public class Tests
    {
        private RestClient _client;
        private static string? _createdRevueId;
        private static string _baseUrl = "https://d2925tksfvgq8c.cloudfront.net/ ";

        [OneTimeSetUp]
        public void Setup()
        {
            string token = GetJwtToken("retake0822@softuni.com", "ElaboratePass2025");
            
            var options = new RestClientOptions(_baseUrl)
            {
                Authenticator = new JwtAuthenticator(token)
            };

           _client = new RestClient(options);
        }

    
            private string GetJwtToken(string email, string password)
        {
            var loginClient = new RestClient(_baseUrl);

            var request = new RestRequest("/api/User/Authentication", Method.Post);
            request.AddJsonBody(new { email, password });

            var response = loginClient.Execute(request);

            var json = JsonSerializer.Deserialize<JsonElement>(response.Content!);
            return json.GetProperty("accessToken").GetString()!;
        }

        [Test, Order(1)]
        public void CreateRevue_ShouldReturnOk()
        {
            var revue = new RevueDTO
            {
                Title = "New revue",
                Url = "",
                Description = "Test revue description"
            };

            var request = new RestRequest("/api/Revue/Create", Method.Post);
            request.AddJsonBody(revue);
            var response =_client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var json = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content!);

            Assert.That(json!.Msg, Is.EqualTo("Successfully created!"));

        }
        
        [Test, Order(2)]
        public void GetAllRevues_ShouldReturnList()
        {
            var request = new RestRequest("/api/Revue/All", Method.Get);
            var response =_client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var revues = JsonSerializer.Deserialize<List<ApiResponseDTO>>(response.Content!);
            Assert.That(revues, Is.Not.Empty);
            
            var lastRevue = revues.Last();
            
            _createdRevueId = lastRevue.RevueId;
        }

        [Test, Order(3)]
        public void EditRevue_ShouldReturnOk()
        {
            var edited = new RevueDTO ()
            {
                Title = "edited revue",
                Url = "",
                Description = "Test revue description"
            };

            var request = new RestRequest($"/api/Revue/Edit?revueId={_createdRevueId}", Method.Put);
            request.AddJsonBody(edited);

            var response =_client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var json = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content!);
            Assert.That(json!.Msg, Is.EqualTo("Edited successfully"));
        }
        

        [Test, Order(4)]
        public void DeleteRevue_ShouldReturnOk()
        {
            var request = new RestRequest($"/api/Revue/Delete?revueId={_createdRevueId}", Method.Delete);
            var response =_client.Execute(request);


            var json = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content!);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(json!.Msg, Is.EqualTo("The revue is deleted!"));

        }

        [Test, Order(5)]
        public void CreateRevue_WithoutRequiredFields_ShouldReturnBadRequest()
        {
            var revue = new RevueDTO()
            {};

            var request = new RestRequest("/api/Revue/Create", Method.Post);
            request.AddJsonBody(revue);

            var response =_client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test, Order(6)]
        public void EditNonExistingRevue_ShouldReturnBadRequest()
        {
            string fakeId = "123";
            var revue = new
            {
                Title = "edited revue",
                Url = "",
                Description = "Test revue description"
            };
            var request = new RestRequest($"/api/Revue/Edit?revueId={fakeId}", Method.Put);
            request.AddJsonBody(revue);

            var response =_client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var json = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content!);
            Assert.That(json!.Msg, Is.EqualTo("There is no such revue!"));

        }

        [Test, Order(7)]
        public void DeleteNonExistingRevue_ShouldReturnBadRequest()
        {
            string fakeId = "123";
            var request = new RestRequest($"/api/Revue/Delete?revueId={fakeId}", Method.Delete);
            var response =_client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var json = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content!);
            Assert.That(json!.Msg, Is.EqualTo("There is no such revue!"));
        }
        
        [OneTimeTearDown]
        public void Cleanup()
        {
           _client.Dispose();
        }

    }
}