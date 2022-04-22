# LogisticsVR
Simple logistics VR game

v.1.0:
- city buildings;
- warehouse building;
- police car;
- big and small trucks;
- forklift;
- boxes.

v.1.1:
- VR setup, oculus integration;
- player rig;
- teleportation.

v.1.2:
- remodel warehouse building;
- assets cleaning.

v.1.3:
- Small truck moving AI;
- Small truck back moving;
- Small truck get stuck moving;
- Warehouse truck loading manager (trucks distribution by waiting area, calling for loading);
- Warehouse loading waypoints;
- Pallet prefab.

v.1.4 - 1.4.1:
- Big truck moving;
- Trailer moving with truck;
- Forklift lift moving by shifter;
- Player sit on moving objects;
- Player sit inside forklift;
- Forklift driving by player;
- Forklift moving back;
- Setting for meta quest 2.

v.1.5:
- forklift change lift shifter logic (3 steps: up, cancel, down);
- forklift change rudder logic (new grab interaction and new rotation for wheels);
- forklift remove smoke;
- setup project physics and time settings.

v.1.6:
- fix: shifter don't work when forklift turning;
- fix: grab parent objects with target (shifter, rudder);
- fix: size of forklift.

v.1.7:
- forklift movement make easier;
- remove forklift audio;
- fix: player turn forward when teleport to forklift;
- fix: player turning face while moving forklift.

v.1.8:
- forklift speed limit and fast start moving/stop moving;
- trailer wheels moving;
- truck moving to warehouse;
- truck parking;
- truck moving to exit;
- new city design;
- simplify small truck loading process;
- fix: player inertia after forklift stop.

v.1.9:
- truck calling button;
- truck finish loading triggers (closed doors and empty trailer);
- truck doors opening;
- interaction with the boxes;
- complects of cargo (4 different sizes);
- cargo inside the trailer.

v.2.0:
- small truck city delivery route;
- small truck parking to warehouse;
- small truck finish loading triggers (limit boxes in cargo slot);
- small truck controller script optmization;
- player moving inside trailer;
- forklift moving inside warehouse;
- fix: forklift body colliders;
- fix: forklift shaking;
- fix: floor level in warehouse;
- fix: player body interaction matrix with objects;
- fix: truck stucks on border;
- fix: trucks accelerates when parked.

v.2.1:
- small track return to loading manager after delivery;
- small truck unload boxes at local points;
- add 3 small trucks;
- each truck moving to waiting zone, loading zone, parking, delivery;
- fix: small truck loaded check for forklift.

v.2.2:
- police car moving;
- police car enter warehouse zone and arrest all trucks;
- police car leave warehouse zone.

v.2.3:
- police car lights and sounds;
- player give box to police to avoide warehouse arrest.

v.2.3.1:
- fix: increase boxes stability;
- fix: small truck moving without boxes when forklift take pallet with boxes;
- fix: small truck start parking while other truck in parking zone;
- improve city design.

v.2.3.2:
- update oculus integration sdk (not used);
- new moving controller for forklift (by axis button);
- each hand can control forklift;
- fix: press button by physical hand;
- fix: hands move player when touch the wall;
- fix: teleport ray hit the player;
- fix: boxes can move heavy objects;
- fix: pallet is very big.
