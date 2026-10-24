# KindHome

KindHome est une application mobile qui relie des orphelinats de Douala (Cameroun) a des donateurs et des benevoles.

Il y a trois roles a l'inscription :
- Donateur : il cherche un orphelinat (liste ou carte), regarde ses besoins, propose un don en nature ou en argent, envoie un message a l'orphelinat, suit son historique et ses notifications.
- Benevole : il utilise les memes ecrans que le donateur (la barre d'onglets est la meme) et peut en plus envoyer une candidature de benevole (domaines, jours, creneau, motivation). Dans le code, la seule difference entre donateur et benevole est le texte du role affiche dans le profil.
- Organisation (orphelinat) : elle a son propre espace avec un tableau de bord, la liste de ses besoins, la publication d'un nouveau besoin, les demandes de benevolat (Accepter ou Refuser), les dons recus (Confirmer la remise), son profil et ses notifications.

Il existe aussi un mode invite ("Continuer sans compte") pour parcourir l'application sans compte.

## Technologies

- .NET MAUI, avec .NET 10 (cible net10.0-android dans KindHome.csproj).
- MVVM ecrit a la main : pas de bibliotheque MVVM, un ViewModel par page.
- SQLite avec le package sqlite-net-pcl (version 1.11.285) et SQLitePCLRaw.bundle_green (version 2.1.11).
- Cible : Android, emulateur Pixel 7 - API 36 (Android 16).

L'application n'a pas de serveur. La base SQLite est un fichier local a l'appareil (kindhome.db3, dans le dossier de donnees de l'application). Rien n'est envoye sur internet.

## Lancer le projet

1. Ouvrir la solution KindHome.slnx (ou KindHome.csproj) dans Visual Studio Community, avec la charge de travail ".NET MAUI" installee.
2. Choisir la configuration Debug et un emulateur Android dans la barre d'outils, par exemple "Pixel 7 - API 36.0". S'il n'existe pas, le creer avec l'Android Device Manager (menu Outils).
3. Appuyer sur F5 (ou sur le bouton vert). Visual Studio compile, installe l'application et l'ouvre. Le premier lancement est long.
4. L'application s'ouvre sur l'ecran de bienvenue. Les tables de la base sont creees automatiquement. Il n'y a rien a configurer.

Ctrl+F5 lance sans le debogueur. Il faut que "Deployer" soit coche pour le projet dans le Configuration Manager (menu Generer), sinon l'application n'est pas installee sur l'emulateur.

L'application est prevue pour le mode portrait. On ne la lance pas sous Windows.

## Comptes de demonstration

| Role | E-mail | Mot de passe |
|---|---|---|
| Donatrice (Christela Mba) | christela@mail.com | kind1234 |
| Organisation (Maison des Anges) | maison@mail.com | kind1234 |

Le mode invite existe aussi ("Continuer sans compte").

Ces deux comptes ne peuvent pas changer de mot de passe : leur mot de passe est ecrit dans le code (Data/MockData.cs). Un compte cree par l'inscription, lui, est enregistre dans la base et garde ses donnees apres la fermeture de l'application.

## Ce qui est enregistre dans la base

Chaque table correspond a une classe *Row du dossier Models. Elles sont creees au premier usage par les services.

- AccountRow : les comptes crees a l'inscription (e-mail en minuscules, nom, telephone, mot de passe hache, role).
- NeedRow : les besoins (titre, nom de l'orphelinat, quantite collectee et totale, unite, categorie, texte "pourquoi", noms des images). Au premier lancement, les 3 besoins d'exemple y sont ranges. Les besoins publies par une organisation y sont ajoutes.
- DonationRow : les dons (e-mail du donateur, numero et titre du besoin, orphelinat, don en nature ou en argent, quantite, montant, remise sur place ou non, date souhaitee, message, date d'envoi, statut).
- VolunteerRow : les candidatures de benevoles (e-mail et nom du candidat, besoin, orphelinat, domaines, jours, creneau, motivation, charte acceptee, date d'envoi, statut).
- ContactRow : les messages de contact (e-mail de l'expediteur, orphelinat, objet, message, date d'envoi).
- NotificationRow : les notifications (e-mail du destinataire, titre, texte, icone, date, lue ou non).

Ce qui reste dans le code et n'est pas en base :
- les 6 orphelinats (Data/MockData.cs), avec leurs photos, actualites, coordonnees et position sur la carte dessinee ;
- les donnees d'exemple de Christela (historique de dons et de missions, notifications, chiffres du profil) ;
- les donnees d'exemple de la Maison des Anges (tableau de bord, 3 demandes de benevolat, 3 dons recus, notifications).

## Comment les donnees circulent

Les pages (Views) sont liees a des ViewModels. Les ViewModels appellent des services (dossier Services), et les services lisent et ecrivent dans SQLite avec la connexion de Data/AppDb.cs. Les pages ne parlent jamais directement a la base.

Exemple avec un don en nature :
1. Le donateur remplit DonationPage. DonationViewModel appelle DonationService.SaveAsync.
2. Le service enregistre la ligne du don et fait monter le compteur du besoin (Collected, sans depasser le total) dans la meme transaction : si l'un des deux echoue, rien n'est garde. Un don en argent est enregistre mais ne change pas le compteur.
3. NotificationService previent l'organisation : elle recoit une notification "Nouveau don reçu". Le compte de l'organisation est retrouve par son nom, qui doit etre celui de l'orphelinat.
4. L'organisation ouvre "Dons recus" (OrgService.GetDonationsAsync). Le don est "À confirmer". Elle appuie sur "Confirmer la remise" : le statut du don passe a "Remis" en base.
5. Le donateur recoit une notification "Don remis". Quand il ouvre son historique, HistoryService relit la base et affiche le statut "Remis".

Pour une candidature de benevole, c'est le meme principe : l'organisation est prevenue, puis le benevole recoit "Candidature acceptée" ou "Candidature refusée".

## Securite des mots de passe

Les mots de passe sont haches avec PBKDF2 (SHA-256, 100000 iterations, sel aleatoire de 16 octets) avant d'etre ecrits dans la base. Le mot de passe en clair n'est pas garde dans le compte en memoire. Si une ligne de la base contient encore un ancien mot de passe en clair, il est converti en hash a la premiere connexion reussie. Un texte de hash abime en base est refuse sans erreur.

## Mot de passe oublie

La page Mot de passe oublie fonctionne en deux etapes :
1. La personne saisit son adresse e-mail et son numero de telephone. Ils doivent correspondre au meme compte. Le numero est compare sans espaces et sans le prefixe +237. Si l'e-mail est inconnu ou si le telephone ne correspond pas, le message est le meme.
2. Elle choisit un nouveau mot de passe (8 caracteres minimum, dont un chiffre) et le confirme. L'identite est verifiee une deuxieme fois avant d'ecrire le nouveau hash.

On utilise cette methode parce que l'application n'a pas de serveur : elle ne peut pas envoyer d'e-mail avec un lien. Apres 5 verifications ratees, la page se verrouille.

## Limites connues

- Les noms d'organisation ne sont pas uniques. Le lien entre un compte organisation, ses besoins, ses dons et ses candidatures se fait par le nom (celui de l'orphelinat), pas par un identifiant.
- Le mot de passe oublie repose sur l'e-mail et le telephone, qui ne sont pas des secrets. La limite de 5 essais est gardee dans la page : elle se remet a zero quand on rouvre la page.
- Les statuts des donnees d'exemple (les 3 demandes et les 3 dons d'exemple de la Maison des Anges) ne sont pas enregistres : ils repartent de leur etat d'origine a chaque lancement. Il en est de meme pour l'etat lu ou non lu des notifications d'exemple.
- Les dons, candidatures et messages d'un invite sont enregistres sans e-mail. Ils ne sont rattaches a aucun compte, et l'invite ne les retrouve que pendant la session (historique de session).
- La cloche des notifications n'a pas de badge : une nouvelle notification ne se voit qu'en ouvrant la page. L'interrupteur "Notifications" des reglages n'est pas enregistre (il repasse a "active" a chaque connexion).
- Seule la Maison des Anges a un compte de demonstration. Un don, une candidature ou un message envoye aux 5 autres orphelinats est enregistre, mais aucune organisation ne le voit et personne n'est prevenu.
- Les messages de contact sont enregistres, mais l'organisation n'a pas de page pour les lire. Elle voit seulement le debut du message (80 caracteres) dans la notification.
- L'annuaire affiche les 6 orphelinats ecrits dans le code. Une organisation creee a l'inscription n'y apparait pas, et ses besoins publies n'ont pas de photo.
- Les deux comptes de demonstration ont leur mot de passe ecrit dans le code, sans hash.
- La carte de l'annuaire est un dessin. Le bouton de position remet la carte dans son etat de depart (il n'y a pas de vraie localisation).
- Le build affiche des avertissements NU1903 : une vulnerabilite est signalee sur le paquet SQLitePCLRaw.lib.e_sqlite3 (version 2.1.11).
- Le depot ne contient pas de projet de tests automatiques.

## Pages de l'application

Avant la connexion : Bienvenue (OnboardingPage), Connexion (LoginPage), Inscription (RegisterPage), Mot de passe oublie (ForgotPasswordPage).

Donateur et benevole (barre d'onglets Accueil, Carte, Besoins, Profil) :
- Accueil (HomePage), Orphelinats a proximite en liste et en carte (DirectoryPage), Liste des besoins (NeedsPage), Profil (ProfilePage).
- Ouvertes par-dessus les onglets : fiche d'un orphelinat (OrphanageDetailPage), detail d'un besoin (NeedDetailPage), Proposer un don (DonationPage) et sa confirmation (DonationConfirmationPage), Devenir benevole (VolunteerPage) et sa confirmation (VolunteerConfirmationPage), Contacter l'orphelinat (ContactPage), Mon historique (HistoryPage), Notifications (NotificationsPage), Reglages (SettingsPage).

Organisation (barre d'onglets Accueil, Besoins, Benevoles, Dons, Profil) :
- Tableau de bord (OrgDashboardPage), Mes besoins (OrgNeedsPage), Demandes de benevolat (OrgVolunteersPage), Dons recus (OrgDonationsPage), Profil de la structure (OrgProfilePage).
- Ouvertes par-dessus : Publier un besoin (PublishNeedPage), Notifications, Reglages.

La navigation utilise le Shell de MAUI (AppShell.xaml et AppShell.xaml.cs).

## Actions de l'appareil

Les telephones, e-mails et coordonnees des orphelinats sont des exemples.
- Partager (fiche d'un orphelinat) : ouvre la feuille de partage du systeme avec un court texte.
- Appeler, E-mail et Itineraire (page Contacter l'orphelinat) : ouvrent le composeur, la messagerie (avec l'objet choisi) et l'application de cartes. Si l'appareil n'a pas l'application, une petite fenetre l'indique.

## DebugFlags (fichier Data/DebugFlags.cs)

Ces valeurs servent a montrer les differents etats. StartRoute vaut "OnboardingPage" par defaut et les quatre interrupteurs valent false. Il faut relancer l'application apres un changement.

| Valeur | Effet |
|---|---|
| StartRoute | Page affichee au demarrage (texte). Mettre le nom d'une page, par exemple "HomePage", "NeedsPage" ou "OrgDashboardPage". Les pages qui attendent des donnees (NeedDetailPage, DonationPage, VolunteerPage, ContactPage...) s'ouvrent alors sans donnees. Pour les voir remplies, y arriver par la navigation normale. |
| SimulateNetworkError | true : la page Besoins affiche l'etat "Pas de connexion". |
| SimulateLocationDenied | true : l'annuaire affiche l'etat "Localisation desactivee". |
| SimulateSendFailure | true : l'envoi d'un don, d'une candidature, d'un message de contact ou d'un besoin publie echoue apres une seconde. |
| SimulateNoOrgNeeds | true : l'organisation connectee voit l'etat "Aucun besoin publie". |

## Structure du projet

- Models : les classes de donnees (Orphanage, Need, Donation, VolunteerRequest, UserAccount, UserSession...) et les classes *Row, une par table SQLite.
- ViewModels : un ViewModel par page, plus quelques classes pour les elements de liste (NotificationItem, OrgRequestItem, OrgDonationItem, ChipItem...). BaseViewModel gere les changements de valeur.
- Views : les pages XAML et leur code-behind. Le sous-dossier Components contient les composants reutilisables (champs de saisie, boutons, cartes, barres d'onglets). DeviceActions.cs regroupe appel, e-mail, itineraire et partage.
- Services : l'acces a la base. AccountService (comptes et mot de passe oublie), PasswordHasher (hachage), NeedService, DonationService, VolunteerService, ContactService, NotificationService, HistoryService (historique du donateur), OrgService (espace organisation).
- Data : AppDb.cs (connexion SQLite), MockData.cs (orphelinats et donnees d'exemple), DebugFlags.cs (options de demonstration).
- Resources : l'icone et l'ecran de demarrage de l'application (AppIcon, Splash), les polices (Fonts), les images et icones (Images), les couleurs et styles (Styles).
- Platforms : les fichiers propres a chaque systeme (Android, iOS, MacCatalyst, Windows).
- MauiProgram.cs, App.xaml, AppShell.xaml : demarrage de l'application, ressources globales et navigation.
