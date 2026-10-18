# KindHome

KindHome est une application mobile qui relie des donateurs, des benevoles et des orphelinats a Douala (Cameroun).
On peut trouver un orphelinat pres de soi, voir ses besoins, proposer un don, devenir benevole ou envoyer un message.
Les orphelinats ont leur propre espace : tableau de bord, besoins publies, demandes de benevolat et dons recus.

Cette version est le front-end seul (.NET MAUI, MVVM ecrit a la main, aucun package NuGet ajoute) : toutes les donnees sont des exemples ecrits dans le code, rien n'est envoye nulle part.

## Lancer l'application sur un emulateur Android (Visual Studio)

1. Ouvrir KindHome.csproj (ou la solution) dans Visual Studio 2022 ou plus recent, avec la charge de travail ".NET MAUI" installee.
2. Dans la barre d'outils, choisir la configuration Debug et, comme cible, un emulateur Android (par exemple "Pixel 7 - API 34"). Si aucun emulateur n'existe, le creer avec l'Android Device Manager (menu Outils).
3. Appuyer sur F5 (ou sur le bouton vert) : Visual Studio compile, installe l'application et l'ouvre. Le premier lancement est long. Ctrl+F5 lance sans le debogueur ; il demande que "Deployer" soit coche pour le projet dans le Configuration Manager (menu Generer), sinon l'application n'est pas installee sur l'emulateur.
4. L'application s'ouvre sur l'ecran de bienvenue. Aucun package a ajouter, aucune cle d'API, aucune connexion a un service.

L'application est prevue pour le mode portrait. On ne la lance pas sous Windows.

## Les ecrans

Avant la connexion :
1. Bienvenue (OnboardingPage)
2. Connexion (LoginPage)
3. Inscription (RegisterPage)
4. Mot de passe oublie (ForgotPasswordPage)

Donateur et benevole (barre d'onglets Accueil, Carte, Besoins, Profil) :
5. Accueil (HomePage)
6. Orphelinats a proximite, liste et carte (DirectoryPage)
7. Fiche d'un orphelinat (OrphanageDetailPage), ouverte par-dessus les onglets
8. Liste des besoins (NeedsPage)
9. Detail d'un besoin (NeedDetailPage)
10. Proposer un don (DonationPage)
11. Merci pour votre don (DonationConfirmationPage)
12. Devenir benevole (VolunteerPage)
13. Candidature envoyee (VolunteerConfirmationPage)
14. Contacter l'orphelinat (ContactPage)
15. Profil (ProfilePage)

Organisation (barre d'onglets Accueil, Besoins, Benevoles, Dons, Profil) :
16. Espace organisation : tableau de bord (OrgDashboardPage)

Pages ajoutees apres la maquette (elles reprennent les styles et composants existants) :
- Mon historique (HistoryPage), Reglages (SettingsPage) et Notifications (NotificationsPage), ouvertes par-dessus les onglets, pour le donateur comme pour l'organisation.
- Espace organisation : Mes besoins (OrgNeedsPage), Demandes de benevolat (OrgVolunteersPage, avec Accepter et Refuser), Dons recus (OrgDonationsPage, avec Confirmer la remise) et Profil de la structure (OrgProfilePage). La page Publier un besoin (PublishNeedPage) s'ouvre par-dessus.

Les etats vides, de chargement et d'erreur (liste vide, aucun resultat de recherche, pas de connexion, localisation desactivee, envoi en cours, echec d'envoi, mot de passe incorrect, aucun besoin publie) apparaissent dans les pages quand la situation les provoque.

Dons, candidatures et besoins publies pendant la session s'ajoutent aux listes (historique, tableau de bord) et disparaissent a la deconnexion ou a la fermeture de l'application.

## Comptes d'exemple

| Role | E-mail | Mot de passe |
|---|---|---|
| Donatrice et benevole (Christela Mba) | christela@mail.com | kind1234 |
| Organisation (Maison des Anges) | maison@mail.com | kind1234 |

"Continuer sans compte" ouvre l'accueil en invite. Un compte cree par l'inscription (donateur, benevole ou orphelinat) fonctionne tant que l'application reste ouverte ; ses listes commencent vides.

## DebugFlags (fichier Data/DebugFlags.cs)

Ces valeurs servent a montrer les differents etats a l'enseignant. StartRoute vaut "OnboardingPage" par defaut et les quatre interrupteurs valent false. Il faut relancer l'application apres un changement.

| Valeur | Effet |
|---|---|
| StartRoute | Page affichee au demarrage (texte). Mettre le nom d'une page, par exemple "HomePage", "NeedsPage" ou "OrgDashboardPage". Les pages qui attendent des donnees (NeedDetailPage, DonationPage, VolunteerPage, ContactPage...) s'ouvrent alors sans donnees. Pour les voir remplies, y arriver par la navigation normale. |
| SimulateNetworkError | true : la page Besoins affiche l'etat "Pas de connexion" (avec "Reessayer" et "Voir les donnees enregistrees"). |
| SimulateLocationDenied | true : l'annuaire affiche l'etat "Localisation desactivee". |
| SimulateSendFailure | true : l'envoi d'un don, d'une candidature, d'un message de contact ou d'un besoin publie echoue apres une seconde. |
| SimulateNoOrgNeeds | true : les besoins de depart de la Maison des Anges sont masques ("Aucun besoin publie") ; les besoins publies pendant la session restent affiches. |

## Actions de l'appareil

Plus aucun bouton n'est en attente. Les telephones, e-mails et coordonnees des orphelinats sont des exemples.
- Partager (fiche d'un orphelinat) : ouvre la feuille de partage du systeme avec un court texte.
- Appeler, E-mail et Itineraire (page Contacter l'orphelinat) : ouvrent le composeur, la messagerie (avec l'objet choisi) et l'application de cartes. Si l'appareil n'a pas l'application, une petite fenetre l'indique.
- Bouton de position (viseur) de la carte de l'annuaire : retire la selection du repere et remet la carte dans son etat de depart (aucune vraie localisation).

## Ce qui reste a faire

- Le back-end : comptes, orphelinats, besoins, dons, candidatures, messages et notifications sont aujourd'hui des donnees d'exemple dans Data/MockData.cs et en memoire. Aucun envoi n'est reel et rien n'est conserve apres la fermeture.
- La vraie localisation et une vraie carte (le bouton de position ne fait que recentrer le dessin).
