L’outil Tiled est disponible ici: https://www.mapeditor.org/

Il permet d’éditer des tilemaps (fichiers tmx) à l’aide de Tilesets (fichiers tsx). Ce sont des fichiers exploitables au format XML.

Il permet également d’exporter les nouveaux aux formats image.

Le repertoire Samples contient 4 fichiers:
- Mario_level1.tmx et Terrain.tsx ont été crées avec Tiled. Vous pouvez les ouvrir avec cet outil.
- deux fichiers image back.png et Terrain.png à partir de ressources gratuites disponibles ici: https://pixel-frog.itch.io/pixel-adventure-1
Les fichiers png sont référencés par les fichiers tmx et tsx pour construire le niveau.

Vous pouvez ouvrir Mario_level1.tmx avec Tiled et le modifier si vous voulez.

Les deux fichiers C# construisent un programme qui prend un fichier tmx et en construit un niveau qui réarrange les colonnes de tiles au hasard et génère ainsi un niveau aléatoire (sans doute pas très jouable!).

Pour faire tourner le “mélangeur de colonnes” sur l’exemple:

- Pour construire et compiler, avec csc:
csc -nologo -debug:full *.cs

- Pour exécuter avec mono:
mono MarioLevelRandomizer.exe Samples/Mario_level1.tmx Samples/random_level.tmx

- Comparez, en les ouvrant avec Tiled, les fichiers Mario_level1.tmx et random_level.tmx

Le format tmx décrit ici: https://doc.mapeditor.org/en/stable/reference/tmx-map-format/

A vous de faire mieux.

