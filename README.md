Projectoverzicht
N.E.O Runner is een op beweging gebaseerde, score-gedreven game ontwikkeld in Unity, met gebruik van Kinect v2 voor realtime gebaren- en lichaamsdetectie. Het project onderzoekt dynamische moeilijkheidsgraad, calibratie, power-up-mechanieken en locatiegebaseerde gameplay, met als doel fysieke activiteit leuk en toegankelijk te maken voor een breed publiek.


Benodigdheden:
Unity Editor 6000.0.33f1 of nieuwer

Windows 10/11

Kinect v2 sensor (met USB 3.0)

Minimaal: Intel i5 processor, 8GB RAM, dedicated GPU


Installatie:
Download of clone deze repository.

Open het project in Unity Hub (versie 6000.0.33f1 aanbevolen).

Sluit de Kinect v2 sensor aan op je PC (controleer met de Kinect Configuration Verifier).

Open de MainMenu scene en druk op Play om te starten.


Projectstructuur:
Assets/Scenes/ – Alle scenes (MainMenu, Game, GameOver, enz.)

Assets/Scripts/ – Kernscripts (beweging, score, obstakels, calibratie, enz.)

Assets/Prefabs/ – Gameobjecten (obstakels, power-ups, UI)

Assets/Assets/ – Diverse gebruikte assets

Assets/Font/ – Gebruikte fonts

Assets/Textures/ – Textures van 3D-modellen


Gebruik & Spelinstructies:
Zorg dat de Kinect v2 correct is aangesloten en voor het speelveld staat.

Start het spel vanuit de MainMenu scene.

Gebruik je hele lichaam om het personage te besturen, power-ups te verzamelen en obstakels te ontwijken.

De score stijgt automatisch over tijd en bij succesvolle acties.


Features / Functionaliteiten:
Real-time motion tracking via Kinect v2

Verschillende soorten obstakels en power-ups

Solo- en 1v1-modus

Visuele feedback en animaties



Onderhoud & Support:
Controleer de Unity Console bij fouten.

Controleer of alle Inspector-referenties correct zijn ingesteld.

Voor Kinect-problemen: gebruik de Kinect Configuration Verifier en een USB 3.0-poort.

Maak regelmatig back-ups en gebruik versiebeheer (Git).


Bekende Problemen:
De implementatie van de 1v1-modus veroorzaakte scriptconflicten en game-breaking bugs, waardoor een volledige refactor nodig was.

Calibratie-thresholds kunnen per gebruiker aanpassing vereisen.

Sommige obstakels en power-ups hadden spawn-inconsistenties, opgelost met individuele spawners.

Inspiratie & Vergelijkbare Projecten:
Geïnspireerd door games als Just Dance, Subway Surfers en Zombies, Run!

Locatiegebaseerde gameplay en motion tracking zijn gebaseerd op best practices uit Unity Learn en Microsoft Kinect Developer Resources.



--------------------------------------------------------------------------LINKS------------------------------------------------------------------------------ 

Bronnen & Referenties
Dit project is ontwikkeld met behulp van diverse artikelen, forms en technische documentatie, waaronder:

Installatie & Setup:
- https://www.youtube.com/watch?v=GehUgGG9Z-U&ab_channel=NXTWindows
- https://www.instructables.com/How-to-Connect-a-Kinect/
- https://www.reddit.com/r/kinect/comments/pgu04d/kinect_v2_to_windows_instructions_how_to_properly/
- https://www.youtube.com/watch?v=m3BAlJAGIkQ&ab_channel=CodetoCreate
- https://www.youtube.com/watch?v=aHGlLxh6a88&ab_channel=VRwithAndrew


Bewegingsdetectie & Interactie:
- https://www.youtube.com/watch?v=6jhoWsHwU7w
- https://www.youtube.com/watch?v=B7T0XTNk-Vg&ab_channel=VRwithAndrew
- https://www.youtube.com/watch?v=hKDaI_E7rDg&ab_channel=VRwithAndrew

Unity Best Practices:
- https://docs.unity3d.com/Manual/index.html
- https://discussions.unity.com/

Calibratie & Geavanceerde Setup:
- https://github.com/taochenshh/Kinect-v2-Tutorial
- https://discussions.unity.com/t/how-can-i-use-kinect-for-windows-v2-with-unity/119681
- https://github.com/opentrack/opentrack/wiki/Using-Microsoft-Kinect-V2/072a59103527cf398489915414b4df05739b6a98

Game related links

Spawners & Instanciation
  - https://docs.unity3d.com/Packages/com.unity.entities@1.1/manual/ecs-workflow-create-entities.html
  - https://www.youtube.com/watch?v=gIuJ10RlUXc

Power-Ups & Collectibles
  - https://www.sharpcoderblog.com/blog/creating-collectibles-and-power-ups-in-unity
  - https://www.youtube.com/watch?v=CLSiRf_OrBk&ab_channel=Brackeys


Survey:
https://docs.google.com/forms/d/e/1FAIpQLSeHaSHNHt8NjFMsqFEzMpEaUyZJQtc1SHsRxDFDtyqx79MMoQ/viewform?usp=header

Xd wire frame  en mockups:
https://xd.adobe.com/view/443e98c4-3d94-406f-8fb3-0b586e9c8c68-116d/

gebruikt AI's:
- https://www.perplexity.ai/
- https://chatgpt.com/


Credits
Ontwikkeld door Gauthier Lambeau en Noham Pace Alkhomaili, 2025.
Met dank aan alle testers en bijdragers.
