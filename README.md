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

v.2.4:
- fix: forklift rudder rotation;
- fix: forklift get rudder player rotation;
- fix: forklift decrease lift speed;
- fix: big truck doors correct grabbing;
- fix: cargo decrease size;
- fix: boxes weight and parametrs;
- fix: boxes rotates when grabbed.

v.2.4.1:
- fix: boxes can turn small truck;
- fix: check all trucks routes, a lot of stucks;
- fix: small truck get stuck behavior;
- fix: big truck checkpoint movement;
- fix: big truck call button call only 1 time;
- fix: increase pallet height.

v.2.4.2:
- police car random call;
- fix: police audio volume;
- fix: small truck stop engine smoke when dont move;
- fix: small truck audio volume;
- fix: big truck audio volume;
- fix: big truck doors improve movement;
- fix: player body fall when moving truck;
- fix: player teleport to warehouse outside stairs;
- fix: warehouse wall looks like window.

v.2.5:
- box delivery (player drops boxes from truck to delivery points);
- info desk on warehouse wall (boxes loaded, unloaded, delivered and money counter);
- parking helper system for trucks (more accurate connection to warehouse);
- loading boxes counter (player put boxes in the truck);
- loading boxes counter consider boxes that were in truck before loading;
- unloading boxes counter (player get boxes from big truck trailer);
- delivered boxes counter;
- boxes are fixed on pallet, unfixing when loading in truck;
- box drop audio (glass);
- fix: police signal audio with pauses and low volume;
- fix: forklift shifter longer and expanded attach collider;
- fix: expanded teleport to forklift.

v.2.5.1:
- fix: game slow down when big truck trailer is parking. Physics and truck controller get a lot of CPU;
- fix: big truck parking helper rotation;
- fix: big truck doors very hard to open.

v.2.5.2:
- update boxes on pallet (change joint on kinematic) to avoid CPU increasing;
- update unfreezing boxes when put on pallet in small truck;
- delete loading manager 2 for big truck and merge it to loading zone manager;
- update start moving process for big truck, there is no more need to go back before move forward;
- fix: big truck stuck when exit warehouse;
- fix: increase big truck floor level for the best forklift behavior.

v.2.5.3:
- fix: big truck rotation helper bug (need to change center coordinates of trailer);
- fix: big truck sometimes park not correct;
- fix: big truck sometimes can't start moving;
- fix: big truck smoke when parked;
- fix: big truck stop while delivery when police arest;
- fix: forklift can't get pallet (boxes kinematic conflict with pallet).

v.2.6 - 2.6.1 (release):
- prize desk, player can buy items by game money;
- hand disable when get prize;
- fix: forklift lift sometimes disconnecting;
- fix: forklift crash when moving between truck and warehouse;
- fix: forklift rudder turn slower;
- fix: can take box from kinematic pallet;
- fix: pallet destroy but boxes are still in air;
- fix: big truck get stuck repair;
- fix: update small truck moving (delete move back patch);
- fix: change unloading point positions;
- fix: loud box drop audio when player is far from source;
- fix: money counter.
