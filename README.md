# GlueHome - Platform Technical Task

## Assumptions 

- Whenever we update a delivery we can only modify the state and nothing else for security, if anything else needs to be edited, a new delivery should be generated and the old one deleted.
- We will asume that we will keep the deliveries in the current database for a short amount of time (the ones that are about to be processed) and after we will move them to another db for logs or delete them,
 this is so the DeliveryExpirationService will not have to que for a lot of data which will make the expiration process slow.
- We will asume authorisation is given only to the right type of user (Partner / User) which will be generated from the Auth API

  Everything else is pretty much a match for the mentioned business requirements.

  ## Technical Requirements
  
- The requirement for this application is a local MongoDB database open on the default port: 27017 and .net 6.0
