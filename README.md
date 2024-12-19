#

## Pour commencer
Ces instructions vous permettront d'obtenir une copie du projet opérationnelle sur votre machine locale à des fins de développement et de test. Consultez la section Déploiement pour obtenir des notes sur la manière de déployer le projet sur un système en direct.

### Prérequis
Vous devez avoir une version de git et d'unity sur votre ordinateur.
- Lien git : https://git-scm.com/downloads
- Version d'Unity : 2022.3.47f1
- Suivre le guide d'installation Kinect [README-Installation.txt](https://github.com/FASanterre/PFE021/blob/main/README-Installation.txt)

### Installation

1. Clonez ce dépôt Git sur votre machine locale :
    ```
    git clone <URL_DU_DÉPÔT>
    ```
    Remplacez <URL_DU_DÉPÔT> par l'URL HTTPS ou SSH de votre projet GitHub. 

2. Ouvrez le projet dans Unity :
    1. Lancez Unity Hub.
    2. Cliquez sur "Open" et naviguez jusqu'au dossier où vous avez cloné le projet.
    3. Sélectionnez le dossier racine du projet et cliquez sur "Open".
    4. Unity configurera automatiquement le projet pour la version spécifiée (2022.3.47f1).

3. Vérifiez que le projet fonctionne :
    - Dans Unity, ouvrez la scène principale (par exemple, MainScene ou SampleScene).
    - Cliquez sur le bouton "Play" pour exécuter le projet.

### Déploiement
  Pour déployer le projet sur un système en direct :
  
  1. Naviguez vers File > Build Settings dans Unity.
  2. Configurez la plateforme cible (par exemple, PC, Mac & Linux Standalone, WebGL, Android, etc.).
  3. Cliquez sur "Build" et choisissez un emplacement pour enregistrer le fichier de build.
  4. Une fois le build terminé, exécutez les fichiers générés pour tester le déploiement.

## Modifications
### Ajouter un nouvel objet au projet
1. Importer l'objet dans le projet
    1. Glissez-déposez (Drag and Drop) l’objet avec son matériel (mat) et ses textures (text) dans le dossier Objet de votre projet Unity.
    2. Le dossier Objet se trouve dans l'onglet Project.

2. Charger la scène appropriée
    1. Ouvrez la scène dans laquelle vous souhaitez travailler.
        - Dans l'onglet Project, double-cliquez sur le fichier de scène (par exemple, MainScene.unity ou autre).
          
3. Ajouter l'objet à la scène
    1. Faites glisser l’objet du dossier Objet dans la Hiérarchie ou directement dans la vue de la scène.
    2. Placez-le où vous le souhaitez dans la scène.

4. Sélectionner le prefab de l’objet
    1. Si l'objet est déjà un prefab :
        - Cliquez sur l'objet dans la Hiérarchie ou la scène.
        - Vous verrez les propriétés du prefab dans l'Inspecteur.
    2. Si l'objet n'est pas encore un prefab, vous pouvez en créer un :
        - Faites glisser l'objet depuis la Hiérarchie dans le dossier Objets pour le transformer en prefab.
      
5. Ajouter des components
    1.  Sélectionnez le prefab ou l'objet dans la scène.
    2. Dans l’onglet Inspecteur (par défaut, à droite de l’écran), cliquez sur Add Component.
    3. Dans la barre de recherche qui s’affiche, tapez et sélectionnez les components suivants, un à un :
        - MeshCollider
        - MeshDataManager
        - MeshDeformer 
    4. Répétez cette étape trois fois, une pour chaque component.
6. Configurer le MeshCollider
    1. Une fois le MeshCollider ajouté, dans l’Inspecteur :
        - Cochez la case Provides Contact.


### Ajouter des modifications au projet 
Pour contribuer au projet ou ajouter vos propres modifications, suivez ces étapes :

  1. Assurez-vous d'être à jour avec la branche principale.
     - Avant de commencer, synchronisez votre copie locale avec la dernière version du dépôt :
         
       ```
       git pull origin main
       ```

   
  2. Créez une branche pour vos modifications
      - Travaillez toujours sur une branche pour éviter de perturber la branche principale.
      - Pour créer une nouvelle branche :
    
         ```
          git checkout -b <nom_de_votre_branche>
         ```
      - Pour vous rendre sur une branche locale déjà existante :
        
         ```
          git checkout <nom_de_votre_branche>
         ```
     - Remplacez <nom_de_votre_branche> par un nom descriptif (par exemple, ajout-feature, correction-bug).
       
  3. Effectuez vos modifications dans Unity
      - Ouvrez le projet dans Unity comme indiqué dans la section "Installation".
      - Apportez les modifications nécessaires (ajout de fonctionnalités, correction de bugs, etc.).
      - Assurez-vous que toutes les modifications sont testées dans l'éditeur Unity et qu'elles ne causent pas d'erreurs.
    
  4. Ajoutez les fichiers modifiés à Git
      - Une fois vos modifications terminées, ajoutez-les à votre commit
        ```
        git add .
        ```
  5. Rédigez un message de commit clair et descriptif
       - Faites un commit pour enregistrer vos modifications localement :
         
          ```
          git commit -m "Description de vos modifications"
          ```
  6. Poussez votre branche vers le dépôt distant
        - Envoyez vos modifications vers le dépôt GitHub :
        - S'il s'agit du premier push à partir de cette branche :
          
          ```
          git push --set-upstream origin <branch name>
          ```
          
        - S'il ne s'agit pas de la première fois que vous poussez des modifications sur cette branche :
          
           ```
           git push origin <nom_de_votre_branche>
           ```
  
