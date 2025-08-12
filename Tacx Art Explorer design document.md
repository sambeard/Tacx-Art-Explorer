# Tacx Art Explorer design document





- Startup behavior
  - Check local db
- Orchestrator
  - Has Data retreival service, data caching service 
- Data retrieval service
  - API Integration
  - CRON job

- Data interaction service
  - Lightweight DB
  - Store data
  - Retreive data
- Data Presentation service
- Models
  - List of artworks
    - Artwork
      - thumbnail
      - Image
      - Author
      - Name
      - Description
      - Year
- Pages
  - List view
    - thumbnail
    - medium
    - title
    - artist title
  - Detail view
    - Main image
    - description
    - year
    - title
    - more metadat perhaps
  - Navigate back from detail view using back button in top left
  - navigate between different detail views of different artworks using swipe left right