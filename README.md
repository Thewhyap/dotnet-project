# FFChess

## Team :
- PASSERON Adrien
- AYIVI Marie-diégo Crédo
- GRUAU Elyan

## Repertoire de travail

[./ff-project/](ff-project)

## Sujets choisi 

- Serveur ASP
- Moteur de Jeu
- Jeu

> Details partie Web ASP

- > Utilisateurs enrégistré dans le système

  - **ADMIN**
      - **email** : admin@admin.com
      - **password** : 123456@
  - **CLIENT**
      - **email**: client@client.com
      - **password**: 123456_

- > Fonctionalités implémentées

  - administrateur :
      - Ajouter des jeux
      - Supprimer des jeux
      - Modifier un jeu
      - Ajouter de nouvelles catégories
      - Modifier une catégorie
      - Supprimer une catégorie
  - utilisateur doit pouvoir :
      - Consulter la liste des jeux possédés
      - Acheter un nouveau jeu
      - Voir les jeux possédés
      - Consulter la liste des autres joueurs inscrits et leurs statuts en temps réel
  - n'importe quel utilisateur
      - Consulter la liste de tous les jeux
      - Filtrer par nom / prix / catégorie / possédé / taille
      - Consulter la liste de toutes les catégories
  - Partie API 
    - l'ensemble des routes ont été intégrées

- > Fonctionalités options implémentées

    Parmi ces fonctionnalités
    - Filtres sur les différents affichages de liste 
      - Parmis les filtres par jeu possédé par défaut c'est désactivé et quand un utilisateur CLIENT se connecte il apparait
    - Mise en place du stockage optimal du fichier afin qu'il soit bien téléchargé depuis le swagger


> Details du Serveur de jeu et du jeu

### Rendu
#### FFChess
- Un serveur jeu (Ou l'on peut connecter plusieurs clients)
- Un client jeu (Ou l'on peut jouer / regarder une partie)
- 2+n clients peuvent se connecter au serveur 
- Un lobby pour choisir une partie ou en créer une

1. On lance le jeu
2. On arrive sur le lobby
3. On attend que quelqu'un d'autre se connecte
4. La partie commence
5. On joue
6. Les personnes suivantes sont des spectateurs (Observer)
7. La partie se termine
