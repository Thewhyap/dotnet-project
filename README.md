# FFChess

## Team :
- PASSERON Adrien
- AYIVI Marie-diégo Crédo
- GRUAU Elyan

## Sujets choisi 

- Serveur ASP
- Moteur de Jeu
- Jeu

## Partie Web ASP

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


## Serveur de jeu et jeu

### Rendu

1. On lance le jeu.
2. On arrive sur le lobby.
3. On crée une partie, ou on en rejoint une existante.
4. On attend que quelqu'un d'autre se connecte si besoin.
5. La partie commence.
6. On joue. (Mouvement d'une pièce / Promotion d'un pion)
7. Les personnes suivantes qui se connectent sont des spectateurs. (Observer)
8. Quand les conditions sont rencontrées, la partie se termine.

Certains problèmes sont connus, notamment :
- Impossible de manger des pions
- Impossible de quitter une partie (en cliquant sur quitter)
