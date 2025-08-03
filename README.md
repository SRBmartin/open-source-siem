# Getting started

## Starting up the services
Open terminal of your choice and position yourself to `cd docker`.
After you are in `docker` folder, run `docker-compose up -d` to start the services. Keep in mind that seeders will fail until you configure services with default (unseedable) values.

## Setting up Keycloak for seeding process
Open the Keycloak on `http://localhost:8081`, and log in with development credentials: master and master00.
Navigate to `Manage realms`, press `Create realm` and enter `Realm name` **siem**, leave `Enabled` to **On** and click `Create`.
Check if you are positioned to siem (Current realm label on top right corner).
Navigate to `Clients` tab and press `Create client`:
* **General settings**:
`Client type` - OpenID Connect
`Client ID` - seeder
`Name` - Seeder client
`Description` - Client used for seeder job
* **Capability config**:
`Client authentication` - On
`Authorization` - On
`Authentication flow` - On are Standard flow, Direct access grants and Service account roles (should be on already)
* **Login settings**: 
`Root URL` - ${authBaseUrl}
`Valid redirect URIs` - http://localhost:4200/* (it's only a placeholder)
`Valid post logout redirect URIs` - http://localhost:4200/* (it's only a placeholder)
---
After you press `Save`, you will be reddirected to Client details page, where you will switch to tab `Credentials` and you will go to `Keycloak.Seeder` project's appsettings.json and will paste the `Client Secret` that you will copy from Keycloak's client details page.
Now, go back to the client details and switch to tab `Service accounts roles` where you will select `Assign role -> Client roles`. Here select the following roles: `manage-users`, `manage-realm`, `manage-clients`.
Now, you can run the seeder `docker-compose up -d --build --no-deps keycloak-seeder` and follow the logs to see if everything is created as it is supposed to be.