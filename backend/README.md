# SwipeFeast

## Setup
You need to have Visual Studio or Visual Studio Code with an [extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) installed in order to run the backend. Also, .NET version 8 needs to be installed on the machine.
When the backend is run it automatically opens a browser tab that runs Swagger. Using Swagger you can test out the backend with all the accessible API endpoints.

### User Secrets
In order to correctly use the application it is necessary to set the Google API Token as a user secret. This can be achieved as follows:

Move into the `backend` folder.

Then, initialize user secrets.
```
dotnet user-secrets init
```

Afterwards, set user secret for Google API Token.
```
dotnet user-secrets set "GoogleApiToken" "the_api_token_that_was_provided" --project SwipeFeast.API
```

After this, it should be possible to run the backend including functionality that is dependent on the Google API Token.

## Run through Visual Studio Code
In Visual Studio Code you must open the project in it's base folder, meaning you would have access to both the backend and the frontend folder. Then on the navigation bar click on run and launch. After this select the corresponding debugger (.NET 5+ and .NET Core).


## Start the docker container seperately
Altough it is not recommended, it is possible to start the backend seperately. For this you can use the following command:

```
*Change into the backend directory by using cd for example*
docker build -t swipefeast:backend .  
docker run swipefeast:backend -dit
```
