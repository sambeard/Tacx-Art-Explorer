# Tacx Art Explorer

This is the Tacx Art Explorer project, which is the assignment for my interview process at Tacx.

It can be used to browse artworks from the [Art Instituate of Chicago](https://www.artic.edu/). All the images and provided meta data is originally sourced from the [Artic API]((https://api.artic.edu/docs/#introduction) )

![Tacx Art Explorer Preview](./UsageRecording.gif)

More detailed information about the requirements of this assignment can be found in the [assignment description](.\Test assignment for Windows desktop developer.pdf)

## Setup

To get started all you need to do is to open the solution in visual studio (version 2022 used) and build and run the project. The database will automatically be created and initialized from the program, however if problems with the database persist the existing DB can be removed using the command:

```powershell
 ./DropDb.ps1
```

Performed in the root project directory.



## Design

Tacx Art Explorer has two main views, the art list view and art detail view. These views allow the user to browse artworks of the Art Institute of Chicago. The list view allows for a quick overview of all artworks by a specific artist. Clicking one of the list items opens up a detail view which displays more information as well as an image of the artwork itself. Using the back button one can navigate back to the main list view, or browse other artworks in detail using the previous and next buttons.

![Graphical wireframe style design tacx art explorer](./mockup.jpg)

## Architecture

The Tacx ArteXplorer project makes use of the MVVM framework in combination with a network of services that are provided to their respective viewmodels via dependency injection. Below is a list of the different services and their use:

**ArtApiService**

Responsible for querying the Artic API using custom HTTPclients and retrieving and parsing the resulting data into a format that can be used by the application. DTOs are used to separate the APIs format from the domain models, which are translated using the DTOMapper. 

**CacheService**

Responsible for caching the result from the API in a local database. This is not yet implemented but will make use of a lightweight sql database such as sqlite to store and retrieve artworks.

**ArtService**

Wrapper class that combines the logic of the ArtApiService and the CacheService to provide a fail safe way of retrieving Artworks, irrespective of the current data connectivity (internet, data)

**NavigationService**

The navigation service is used throughout the application to provide relevant viewmodels to the mainwindow. This way viewmodels can be context aware of the other viewmodels and effect the global application state whilst keeping them as DRY and loosely coupled as possible.

**ImageRepository**

The imagerepository is a tool used to store and convert raw binary image data to usable image sources. 

**BackgroundSyncer**

The background syncer service is responsible for polling the database for artists and refreshing all art pieces for these artists in the cache using the ArtAPIService

 

## Roadmap

- [x] Models
  - [x] Artist
  - [x] ArtPiece
  - [x] ArtPieceImage
  - [x] DTOs
  - [x] DTO mapper
- [ ] Services
  - [x] Art Api Service
    - [x] Artworks search by artist
    - [x] Image search by id
  - [x] Art Service
    - [x] api fallback logic
    - [x] timed data resync
  - [x] Cache service
    - [x] db initialization / setup
    - [x] store
    - [x] retreive
    - [x] invalidate
  - [x] ImageRepo
  - [ ] 
- [x] Views
  - [ ] splash screen
  - [x] list view
    - [x] List view item
    - [ ] Drag from top refresh
    - [x] Menu bar
    - [x] Progress indicator
  - [x] Detail view
    - [x] Back button
    - [x] Image view
      - [x] Focus on image
      - [x] Title overlay
    - [x] Detail meta data
    - [x] move buttons

- [x] Logic (ViewModels)
- [x] Testing
  - [ ] Service testing (partly done)
  - [ ] Viewmodel testing (partly done)
- [ ] User level logging
  - [ ] log from ILogger logs
  - [ ] use toasts / temporary banners




## Licenses

All data is provided by the Artic API, which is provided by the Art Institute of Chicago.

The `description` field in the metadata response is licensed under a Creative Commons Attribution 4.0 Generic License (CC-By) and the Terms and Conditions of artic.edu. All other meta is licensed under a Creative Commons Zero (CC0) 1.0 designation and the Terms and Conditions of artic.edu

More information:

[CC 1.0](https://creativecommons.org/publicdomain/zero/1.0/)

[Artic Terms](https://www.artic.edu/terms)	