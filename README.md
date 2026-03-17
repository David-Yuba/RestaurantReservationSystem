# Restaurant reservation system
This project Features a full stack webpage for an imaginary restaurant LaMaison. The website is mainly focused on solving a few problems concerning the management of reservations for restaurants. The process of receiving the reservation, validating the reservation against the current state of the restaurant on a given day, and formatting the state of the restaurant ready for humans to easily process and make decisions around are the key processes the system aims to handle and augment.

The system however remains quite fluid, since reservations arrive in the pending state needing the approval of a person, with the ability to cancel or approve a reservation with minimal effort.

## Prerequisites
The whole app is containerized using docker which handles the app environment, including setting up the asp.net core dependencies, setting up the database with the required tables and data for testing purposes, and setting up a unit test environment which is used as proof of the apps logical correctness.

Git is optional but allows for downloading the app source code from the command line.

Visual Studio 2026 is optional but provides the best experience when view source code.

### Before following the steps make sure you have:
1. Docker installed and running: [Download](https://www.docker.com/products/docker-desktop/)
   the latest version for your operating system and processor architecture
2. Docker compose version: v5.1.0, check it with:
```powershell
## Check docker compose version
docker compose version
```
4. Run Docker after it is installed and configure it so that the you are using the linux-builder (this should be by default)
5. Install git: [Git](https://git-scm.com/install/) (Optional step)
### How to run
1. Start by opening a command line interface of your choice (both linux or windows work)
2. Once you are positioned into the directory in which you wish to download the source code enter the following command:
   (alternatively you can also download the source code manually from this github repository)
```powershell
## Download the source code from this repository
git clone https://github.com/David-Yuba/RestaurantReservationSystem.git
```
3. Position yourself into the root of the source code repository. If you followed the previous commands this step you can complete with:
```powershell
## Position yourself into the newly downloaded directory
cd .\RestaurantReservationSystem\
```
4. Start the building of the project using this command: (Make sure you have Docker both installed and running when executing this step)
```powershell
## Build the docker container and run the app
docker compose up --build
```
5. The app UI can be accessed in a browser by going to http://localhost:8080/ or https://localhost:8081/
6. To run tests run the following command:
```powershell
## Runs the built docker container that contains the tests
docker container start reservationsys-lamaisonrestaurant-reservationsys-lamaisonrestaurant.test-1 -a
```
7. To access the database run:
```powershell
## Connects to the database
docker exec -it reservationsys-lamaisonrestaurant-db-1 sh -c '/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "ASDkjkfsjd@.DKfj23dk" -No -C'
```
8. To connect to the app terminal environment run:
```powershell
## Run the terminal environment where the dotnet app is running
docker exec -it reservationsys-lamaisonrestaurant-reservationsys-lamaisonrestaurant-1 /bin/sh
```
### Navigating the app
There are four main parts to the app when it is running.
#### UI

The UI consists of:
1. The index page (./) - the page for submitting new reservations
2. The successful reservation page (./SuccessfulReservation/{id:int}) - the page to which the app redirects the user to when a reservation is successful, informing the user once more about all the user inputs
3. The admin page (./admin) - the page to view existing reservations
4. The reservation details page (./reservationdetails?id=) - the page to view all the information about a reservation and to update the status of a reservation

#### Microsoft SQL Server database
Accessed through the docker desktop under restaurantreservationsystem-db or through the terminal.

#### The tests container
The test container is built but not run after docker compose. To run it use the terminal or in docker desktop start the reservationsys-lamaisonrestaurant.test-1 container.

#### The app container
You can access the app environment through docker desktop container reservationsys-lamaisonrestaurant-1 exec tab. Alternatively connect to the container through the terminal by using the above mentioned command.

## Possible problems
1. When building the app with docker, sometimes an extra container is built which doesn't do anything. This is quite hard to debug, because it doesn't happen always and I didn't want to waste time on it, since even when it happens the app works just fine. 

## Documentation
I have written a Latex file to serve as documentation for the codebase. The file is in the root directory, with its compiled pdf version.
However I haven't had the time to proof read it or to make it pretty, which is why it might not be very good but I do believe it does shine a light on my thought process while building the app as well as some conventions I decided to follow.
For that reason I also added annotations to the codebase in the form of comments, and grouped code together with regions where possible.

## Master vs AdminWithViewComponents
I have created the second branch AdminWithViewComponents because on a last read through of the specification I noticed I interpreted the specification the way I wanted to. The AdminWithViewComponents is mostly identical, although it doesn't implement the feature to show full time slots on the list view. I will not merge them because I do believe there is some utility in showing them side by side, and I don't have enough time to fully develop the AdminWithViewComponents branch.

Which ever one you wish to grade is fine by me, although I would prefer if the view components are graded more highly that you choose the AdminWithViewComponets.

To navigate the admin page click a reservation row to enter details view, and click the apply button to exit details view back to the reservation list.

I didn't document the view components, but I did follow this [microsoft tutorial](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/view-components?view=aspnetcore-8.0) faithfully, so my thought process should be easy to follow once the tutorial is looked over.
