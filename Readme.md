# Email Template Builder

This is a simple email template builder built with Gulp and .NET (tester). This is far from finished, it should be extensible and should accept parameters. For now it can be used by updating the hardcoded values in Gulpfile.js and Program.cs (for testing emails).

## How to use

### Building email templates

* Run `npm install` for the first time.
* Open Gulpfile.js and update the paths if needed.
* There is a Sample folder in the Templates folder. You can either change this or copy-paste and create your own email template. Don't forget to update the paths in the Gulpfile.
* Run `gulp` to execute the default Gulp task.
* The compiled template will be in the dist folder.

### Sending emails

* Run `dotnet build` for the first time.
* Update Program.cs (subject, email address etc.) should accept these values from config or parameters I am aware.
* By default it will send emails using localhost which means that you need a local SMTP server for development like Papercut or Smpt4dev.
* If you want to send real emails then set your credentials in user secrets for the EmailTemplateBuilder project and update `sendProductionEmail` variable to `true`.
* Execute `dotnet run` command.