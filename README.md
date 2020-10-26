# pocketgraphs

This is a blog site project built initially for my college assignment. It uses ASP.NET MVC Web App framework and uses MSSQL database.

# logic

The project seeds an Admin and a Moderator user and lets other users to register/login as standard users. 
The Admin has access rights to all actions in the app while the moderator can view all posts and can only create and edit comments in a post.
All standard users can view posts and comment on the posts.

# project-setup

Requirements: ASP.NET environment, MSSQL Server (SQLExpress)
Instructions:

To setup the project in the local environment, make sure to have the required programs installed on the device.
Then clone the repository and run the following command to initialize and seed the database:

`update-database`

Be sure to adjust the connection string if the MSSQL Server is setup differently.

Then run by selecting `Debug` > `Start Without Debugging` or simply pressing `CTRL + F5`.

The users `admin@pocketgraphs.com` and `moderator@pocketgraphs.com` have been seeded in the database and their credentials can
be used by entering the email as the email and password.

Standard users can be created from the registration page.

`Note:` This project was originally created in March 21, 2019, but it has been updated, renamed and refactored recently.
