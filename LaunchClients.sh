#!/bin/bash

# Script pour lancer 3 clients Godot en parallèle

echo "================================================"
echo "  FF Chess Clients Launcher"
echo "================================================"

# Couleurs pour les messages
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Répertoire du projet Godot
GODOT_PROJECT_DIR="$(dirname "$0")/ff-chess"
GODOT_EXECUTABLE=""

# Liste des emplacements possibles de Godot
GODOT_PATHS=(
    "/Applications/Godot_mono.app/Contents/MacOS/Godot"
    "/Applications/Godot.app/Contents/MacOS/Godot"
    "$HOME/Applications/Godot_mono.app/Contents/MacOS/Godot"
    "$HOME/Applications/Godot.app/Contents/MacOS/Godot"
    "/opt/homebrew/Caskroom/godot-mono/*/Godot_mono.app/Contents/MacOS/Godot"
)

echo -e "${YELLOW}🔍 Recherche de Godot...${NC}"

# Chercher Godot dans les emplacements connus
for path in "${GODOT_PATHS[@]}"; do
    # Gestion du wildcard pour homebrew
    if [[ "$path" == *"*"* ]]; then
        for expanded_path in $path; do
            if [ -f "$expanded_path" ]; then
                GODOT_EXECUTABLE="$expanded_path"
                break 2
            fi
        done
    elif [ -f "$path" ]; then
        GODOT_EXECUTABLE="$path"
        break
    fi
done

# Si toujours pas trouvé, chercher via mdfind (Spotlight)
if [ -z "$GODOT_EXECUTABLE" ]; then
    echo -e "${YELLOW}   Recherche via Spotlight...${NC}"
    SPOTLIGHT_RESULT=$(mdfind "kMDItemKind == 'Application' && kMDItemFSName == 'Godot*.app'" 2>/dev/null | head -n 1)
    
    if [ -n "$SPOTLIGHT_RESULT" ]; then
        GODOT_EXECUTABLE="$SPOTLIGHT_RESULT/Contents/MacOS/Godot"
        if [ ! -f "$GODOT_EXECUTABLE" ]; then
            GODOT_EXECUTABLE=""
        fi
    fi
fi

# Si toujours pas trouvé, demander le chemin
if [ -z "$GODOT_EXECUTABLE" ]; then
    echo -e "${RED}❌ Godot introuvable automatiquement${NC}"
    echo ""
    echo "Veuillez glisser-déposer l'application Godot ici et appuyer sur Entrée:"
    echo "(ou entrer le chemin complet vers Godot.app)"
    read -r USER_PATH
    
    # Nettoyer le chemin (enlever les espaces, quotes, etc.)
    USER_PATH=$(echo "$USER_PATH" | sed "s/^[[:space:]]*//;s/[[:space:]]*$//;s/^'//;s/'$//")
    
    # Si c'est le .app, ajouter le chemin vers l'exécutable
    if [[ "$USER_PATH" == *.app ]]; then
        GODOT_EXECUTABLE="$USER_PATH/Contents/MacOS/Godot"
    else
        GODOT_EXECUTABLE="$USER_PATH"
    fi
    
    if [ ! -f "$GODOT_EXECUTABLE" ]; then
        echo -e "${RED}❌ Chemin invalide: $GODOT_EXECUTABLE${NC}"
        exit 1
    fi
fi

echo -e "${GREEN}✅ Godot trouvé: $GODOT_EXECUTABLE${NC}"

# Vérifier si le projet existe
if [ ! -f "$GODOT_PROJECT_DIR/project.godot" ]; then
    echo -e "${RED}❌ Projet Godot introuvable dans: $GODOT_PROJECT_DIR${NC}"
    exit 1
fi

echo ""
echo -e "${GREEN}🎮 Lancement de 3 clients...${NC}"
echo "   Projet: $GODOT_PROJECT_DIR"
echo ""

# Fonction pour lancer un client
launch_client() {
    local client_num=$1
    local color=$2

    echo -e "${color}🚀 Client $client_num : Démarrage...${NC}"

    # Délai entre les clients
    sleep $((client_num - 1))
    
    # Lancer Godot en arrière-plan
    "$GODOT_EXECUTABLE" --path "$GODOT_PROJECT_DIR" > "/tmp/godot_client_${client_num}.log" 2>&1 &

    local pid=$!
    echo -e "${color}   Client $client_num PID: $pid (log: /tmp/godot_client_${client_num}.log)${NC}"
}

# Lancer les 3 clients avec des couleurs différentes
launch_client 1 "${BLUE}"
launch_client 2 "${GREEN}"
launch_client 3 "${YELLOW}"

echo ""
echo -e "${GREEN}✅ Les 3 clients sont en cours de lancement${NC}"
echo ""
echo "📝 Logs sauvegardés dans:"
echo "   - /tmp/godot_client_1.log"
echo "   - /tmp/godot_client_2.log"
echo "   - /tmp/godot_client_3.log"
echo ""
echo "Pour arrêter tous les clients:"
echo "  pkill -f 'Godot.*ff-chess'"
echo ""

# Attendre un peu pour que les processus démarrent
sleep 3

# Afficher les processus Godot lancés
echo "Processus Godot actifs:"
ps aux | grep -i "[G]odot.*ff-chess" | awk '{print "  PID: "$2}'

echo ""
echo "================================================"
