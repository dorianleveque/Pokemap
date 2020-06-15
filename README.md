<div align="center">
    <img src="assets/Pokemap-icon.png" alt="drawing" width="200px;" style="filter: opacity(0.6) drop-shadow(0 0 0 blue);"/>
    <h1>
        <b>
          Pokemap
        </b>
    </h1>
    <h3>Générateur de carte Pokemon</h3>
    </br>
    </br>
    <img src="assets/Pokemap.png" alt="drawing" width="600px;" style="filter: opacity(0.6) drop-shadow(0 0 0 blue);"/>
</div>

# Description du projet

Projet réalisé au sein du module IAS (Intelligence Artificiel) à l'ENIB.
Ce projet a pour but d'explorer la génération procédurale de contenu, en générant de manière procédurale des niveaux de jeux vidéo inspirés de l'univers de Pokémon.

Nous avons souhaité utiliser l'outil Tiled au sein du projet afin de pouvoir éditer des tilemaps (fichier tmx) à l’aide de Tilesets (fichiers tsx). Ce sont des fichiers exploitables au format XML. Il permet également d’exporter au format image.

Dans un premier temps, nous avons implémenté la généreration procédurale de notre carte en définissant des règles pour placer les différentes éléments. Puis dans un second temps, nous avons réutilisé la carte précédemment générée comme fichier d'entrée pour générer une nouvelle carte à l'aide de l'algorithme Wave Function Collapse (WFC).

Nous avons rencontré des soucis pour utiliser le WFC, avec les fichier .tmx, c'est pourquoi nous effectuons une conversion sous forme d'une image (wfc.png). Une fois que le WFC a terminé et généré l'image de sortie, nous re-convertissons la carte au format .tmx.

Nous avons utilisé les technologies suivantes pour la première partie:
- Noise Perlin: algo utilisé pour générer le relief
- A*: pour générer les chemins entre les maisons

Après exécution du programme, les fichiers suivants sont créés:
- /assets/pokemap.tmx: carte générée procéduralement
- /assets/pokemap-generated.tmx: carte générée via WFC
- /assets/wfc.png: image temporaire envoyée à l'entrée du WFC
- /assets/wfc-generated.png: image temporaire générée par WFC


# 📂 Contenu du répertoire

Le répertoire src contient 3 fichiers:
- Tiled.cs : afin de manipuler les fichiers de Tiled
- PokemonLevelRandomizer.cs: fichier principal
- MapGenerator: générateur 

Le répertoire assets contient les fichiers suivants:
- Terrain.tsx: tileset du projet
- Terrain.png: image du tileset

# 📦 Installation du projet


1. Installer l'outil d'édition de niveau 2D Tiled [tiled](https://www.mapeditor.org/)

2. Cloner le dépôt GitLab sur votre répertoire
```bash
git clone https://git.enib.fr/j5delabr/generationstarterkit.git
```

3. Se rendre dans le répertoire à l'aide d'un terminal
```bash
cd generationstarterkit
```

# Execution du projet

Pour executer le projet, il suffit d'executer la commande suivante au sein du projet

```bash
dotnet run <size> <HouseCount> <?Seed>
```

Exemple: La commande suivante générera une carte de 200 par 200 avec 50 maisons ayant pour seed 12353
```bash
dotnet run 200 50 12353
```

/!\ Attention:

Si rien ne se passe lors de l'exécution, il se pourrait que vous ayez renseigné trop de maisons ou qu'il n'y est que de l'océan. On peut résoudre le problème, en modifiant les paramètres d'entrée.
