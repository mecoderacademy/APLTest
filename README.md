# APLTest

Steps 1:

-I Started with TDD defining my tests and coding against them to pass
-This was taking a while so for times sake I was satisfied with the tests I had and so began implementing the services for uploading the file
- I began with the azure service I used appropriate dependancy injection and defined interfaces for services.
- I defined a single interface to be re-used for both local storage and cloud
- I wrote the code for each method of interface trying not to make the method too big and considered smaller private methods but thought this wont be testable
- I finished implementing the cloud service ensuring appropriate error handling and memory management with using blocks
- I did the same for the local service and opted for storing the image as a local file -> this service would be useful for testing scenarios and if more time permitted I would have written tests to demonstrate this
- for times sake i didnt use any mocking library's but this would have been to go to choice
- I then implemented the second method in both services for storing and auditing information about the image. -> this used entity framework and code first approach.
- I used a sql lite in memory database because I did not have access to sql management studio as I was working between my work laptop and a macbook
- unfortunately I was not able to run any migration commands as I had issues with the .net core versionings on my macbook and was having issues on my work laptop due to policies and therefore the storing of the fileupload object was not possible
- I would have implemented the repository pattern for data access layer but time did not permit
- I tested the uploading of random image files a number of times using swagger and fixed any bugs along the way ( usually would write tests and mock each permutation)
- I wrote the front end in angular and implemented some front end tests with jest. Unfortunately some of these tests will not pass and are there for cosmetic reasons.
- I wrote components for file upload and submission of file
- I broke these into services
- I wrote the rest of the html and plugged in my services making use of bindings/observables  from service layer.
- I added a loader for useability. I tested this against the backend and ran into CORS issues which I solved but modifying the program.cs file in the api project
- In reality I would have mocked the front end first with tests before spinning up the backend
- I also had issues with conversion to JSON when returing the response back to client side and I had to add a "produce" attribute at the top of the API controller class
- I was able to successfully return the object back to the client and render the image
- I then figured out that I cant run EF migrations with a sqlite in memory database so but ensured the DB is created and deleted in which solves issue of saving


  
