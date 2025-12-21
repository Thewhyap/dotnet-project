# FFChess

Team : PASSERON Adrien, AYIVI Credo, GRUAU Elyan

- Market : [![CI Build](https://github.com/Thewhyap/dotnet-project/actions/workflows/ci.yml/badge.svg)](https://github.com/Thewhyap/dotnet-project/actions/workflows/ci.yml)
- Server : [![CI Build](https://github.com/Thewhyap/dotnet-project/actions/workflows/ci.yml/badge.svg)](https://github.com/Thewhyap/dotnet-project/actions/workflows/ci.yml)
- Client : [![CI Build](https://github.com/Thewhyap/dotnet-project/actions/workflows/ci.yml/badge.svg)](https://github.com/Thewhyap/dotnet-project/actions/workflows/ci.yml)
- Lib : [![CI Build](https://github.com/Thewhyap/dotnet-project/actions/workflows/ci.yml/badge.svg)](https://github.com/Thewhyap/dotnet-project/actions/workflows/ci.yml)

## Sujet

- Un jeu d'echec
- Godot
- .Net
- 2 Joueurs reels
- 0-N viewers
- Jouable sur PC

Jeu (Godot, UNITY, Winform, Console, …)
Le jeu doit mettre en place les IHM permettant aux joueurs de jouer

### Rendu
#### Jeu 
Certains problèmes sont connus, notamment : 
- Impossible de manger des pions
- Impossible de quitter une partie (en clickant sur quitter)

### Marketplace (Steam)
- TODO


## Lancer une partie : 
Server :
```bash
cd FFChessServer
dotnet build FFChessServer.csproj
dotnet run --project FFChessServer.csproj
```
ou le script LaunchServer.sh qui permet de lancer directement le serveur tout en tuant le serveur déjà en cours d'exécution.

Client :
```bash
cd FFChessClient
dotnet build FFChessClient.csproj
dotnet run --project FFChessClient.csproj
```
ou le script LaunchClient.sh qui permet de lancer directement plusieurs clients.
