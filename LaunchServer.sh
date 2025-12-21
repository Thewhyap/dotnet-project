#!/bin/bash

# Script pour lancer le serveur d'échecs
# Kill le processus sur le port 8080 si nécessaire

echo "================================================"
echo "  FF Chess Server Launcher"
echo "================================================"

# Couleurs pour les messages
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Vérifier si un processus écoute sur le port 8080
PORT=8080
PID=$(lsof -ti :$PORT)

if [ ! -z "$PID" ]; then
    echo -e "${YELLOW}⚠️  Un processus ($PID) écoute déjà sur le port $PORT${NC}"
    echo -e "${YELLOW}   Arrêt du processus...${NC}"
    kill -9 $PID
    sleep 1
    
    # Vérifier si le processus est bien arrêté
    if lsof -ti :$PORT > /dev/null 2>&1; then
        echo -e "${RED}❌ Impossible d'arrêter le processus sur le port $PORT${NC}"
        exit 1
    else
        echo -e "${GREEN}✅ Processus arrêté avec succès${NC}"
    fi
fi

# Aller dans le répertoire du serveur
cd "$(dirname "$0")/ff-chess-server"

echo ""
echo -e "${GREEN}🚀 Démarrage du serveur...${NC}"
echo "   Port: $PORT"
echo "   Répertoire: $(pwd)"
echo ""

# Compiler et lancer le serveur
dotnet run --project FFChessServer.csproj

