module NSwag.PetStore.Client.Tests.Tests

open System
open Xunit
open NSwag.PetStore.Client
open JustEat.HttpClientInterception

[<Fact>]
let ``Given missing collection property, we deserialize to an empty collection. 🚀`` () =

    let builder =
        HttpRequestInterceptionBuilder()
            .Requests().ForAnyHost().ForPath("/pet/1234")
            .Responds().WithContent """{
  "id": 0,
  "name": "doggie",
  "photoUrls": [
    "string"
  ],
  "status": "available"
}"""

    let options =  HttpClientInterceptorOptions().ThrowsOnMissingRegistration()
    builder.RegisterWith options |> ignore
    use innerClient = options.CreateHttpClient()
    
    innerClient.BaseAddress <- "http://test-host" |> Uri
    
    let client = PetStoreClient(innerClient)
    let pet = client.GetPetByIdAsync(1234L) |> Async.AwaitTask |> Async.RunSynchronously
    
    Assert.NotNull pet.Tags
    Assert.Empty pet.Tags
