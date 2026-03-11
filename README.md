# Restaurant reservation system
This project Features a full stack webpage for an imaginary restaurant LaMaison. The website is mainly focused on solving a few problems concerning the management of reservations for restaurants. The process of receiving the reservation, validating the reservation against the current state of the restaurant on a given day, and formating the state of the restaurant ready for humans to easily process and make decisons around are the key processes the system aims to handle.

The system however remains quite fluid, since reservations arive in the pending state needing the approval of a person, with the ability to cancel or approve a reservation with minimal effort.

## Prerequiesites
The whole app is containerized using docker which handles the app environment, including setting up the asp.net core dependencies, setting up the database with the required table and data for testing purposes, and setting up a unit test environment which is used as proof of the apps logical correctness.

Git is optional but allows for downloading the app source code from the command line.

### Before following the steps make sure you have:
1. Docker installed and running: [Docker desktop](https://www.docker.com/products/docker-desktop/)
   Download the latest version for your operating system and processor architecture
2. Docker compose version: v5.0.2, check it with:
```powershell
## Check docker compose version
docker compose version
```
4. Run Docker after it is installed
5. Install git: [Git](https://git-scm.com/install/) (Optional step)
### How to run
1. Start by opening a command line interface of your choice (both linux or windows work)
2. Once you are positioned into the directory in which you wish to download the source code enter the following command:
   (alternatively you can also download the source code manualy from this github repository)
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
5. To run tests, access the command line for the environment where the app is running or access the database open docker desktop and search for the restaurantreservationsystem containers
### Navigating the app
There are three main parts to the app when it is running.
#### UI
The UI can be accessed from any browser by going to http://localhost:8080/ or https://localhost:8081/

The UI consists of:
1. The index page (./) - the page for submiting new reservations
2. The successful reservation page (./SuccessfulReservation/{id:int}) - page the app redirects to when a reservation is successful once more showing all the details the the customer
3. The admin page (./admin) - the page to view existing reservations
4. The reservation details page (./reservationdetails?id=) - the page to view all the information about a reservation and to update the status of a reservation

#### Microsoft SQL Server database
Accessed through the docker desktop under restaurantreservationsystem-db. In the exec tab you are able to directly connect to the SQL server and run transact sql commands with the following command:
```bash
## Connect to database
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "ASDkjkfsjd@.DKfj23dk" -C
```
#### The tests container
You can run all the tests by just starting the built docker container reservationsys-lamaisonrestaurant.test-1 that wasn't run right after the build. In docker desktop it is colored orange.
#### The app container
You can run the command line for the environment where the app is running by going into the running reservationsys-lamaisonrestaurant-1 container 

## Assumptions when building the app and possible problems
1. When developing the UI I made the assumption that the person making a reservation will jump around the individual fields, and I believe I covered all cases for input which does result in a robust client side validation which can only be circumvented by the use of javascript disabling or manually editing the post request.
Because of the robustness of the client side validation I implemeneted the serverside validation in a way that doesn't send back any information on why it failed. This might be a drawback if there is a case in client side validation I overlooked, and could cause a failure to make a reservation without proper user feedback being sent.
2. There is an important assumption in the use of DayTime.Now and that is that the server is in the same time zone (more specifically if I am calculating this right the server.time - client.time has to be less than 14). The more I am thinking about this it doesn't seem to be a problem, but nonetheless I do think it is worth considering.
