Start Function App:
	- func start

Start Client:
	- dotnet run
	- press green play button

To connect api and client:
	- swa start http://localhost:5102 --api-location http://localhost:7071 --devserver-timeout 0

Azurite:
	- azurite --location C:\Azurite\ --silent

Tailwind:
	- npx @tailwindcss/cli -i .\wwwroot\css\app.css -o .\wwwroot\css\tailwind.css --watch