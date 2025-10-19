// Import des modules depuis un CDN moderne (ici esm.sh)
import * as THREE from "https://esm.sh/three@0.164.0";
import { OrbitControls } from "https://esm.sh/three@0.164.0/examples/jsm/controls/OrbitControls.js";
import { gsap } from "https://esm.sh/gsap@3.12.5";
import { GLTFLoader } from "https://esm.sh/three@0.164.0/examples/jsm/loaders/GLTFLoader.js";

// --- Création de la scène ---
const scene = new THREE.Scene();
scene.background = new THREE.Color(0x87ceeb);
const clock = new THREE.Clock(); // utile pour synchroniser les animations

// --- Caméra ---
const camera = new THREE.PerspectiveCamera(
  75, // champ de vision en degrés
  window.innerWidth / window.innerHeight, // ratio d'aspect
  0.1, // distance minimale visible
  100  // distance maximale visible
);
camera.position.set(6, 5, 8);

// --- Rendu ---
const renderer = new THREE.WebGLRenderer({ antialias: true });
renderer.shadowMap.enabled = true; // active le rendu des ombres
renderer.setSize(window.innerWidth, window.innerHeight);
document.body.appendChild(renderer.domElement);

// --- Contrôles de la caméra (souris) ---
const controls = new OrbitControls(camera, renderer.domElement);

// --- Resize dynamique ---
window.addEventListener("resize", () => {
  camera.aspect = window.innerWidth / window.innerHeight;
  camera.updateProjectionMatrix();
  renderer.setSize(window.innerWidth, window.innerHeight);
});

// ===============================
// Lumières
// ===============================

// Lumière ambiante (éclaire tout faiblement)
scene.add(new THREE.AmbientLight(0xffffff, 0.4));

// Lumière directionnelle (comme un soleil)
const dirLight = new THREE.DirectionalLight(0xffffff, 1);
dirLight.castShadow = true; // permet de projeter des ombres
dirLight.target.position.set(0, 0, 0);
scene.add(new THREE.DirectionalLightHelper(dirLight, 1)); // aide visuelle
scene.add(dirLight);

// Réglages des ombres du soleil
dirLight.shadow.mapSize.width = 2048;
dirLight.shadow.mapSize.height = 2048;
dirLight.shadow.camera.near = 0.5;
dirLight.shadow.camera.far = 50;
dirLight.shadow.camera.left = -10;
dirLight.shadow.camera.right = 10;
dirLight.shadow.camera.top = 10;
dirLight.shadow.camera.bottom = -10;

// ===============================
// Sol
// ===============================
const planeGeo = new THREE.PlaneGeometry(20, 20);
const planeMat = new THREE.MeshStandardMaterial({ color: 0x88cc88 });
const plane = new THREE.Mesh(planeGeo, planeMat);
plane.rotation.x = -Math.PI / 2; // pour être à plat
plane.receiveShadow = true; // reçoit les ombres
scene.add(plane);

// ===============================
// Maison
// ===============================
const houseGroup = new THREE.Group(); // groupe d’objets pour déplacer la maison entière

// Corps principal
const house = new THREE.Mesh(
  new THREE.BoxGeometry(2, 1.5, 2),
  new THREE.MeshStandardMaterial({ color: 0xbb7744 })
);
house.position.y = 0.75;
house.castShadow = true;
house.receiveShadow = true;
houseGroup.add(house);

// Toit
const roof = new THREE.Mesh(
  new THREE.ConeGeometry(1.6, 1, 4),
  new THREE.MeshStandardMaterial({ color: 0x884422 })
);
roof.position.y = 2;
roof.rotation.y = Math.PI / 4;
roof.castShadow = true;
houseGroup.add(roof);

// Porte
const doorGeo = new THREE.BoxGeometry(0.6, 1, 0.1)
doorGeo.translate(0.3, 0, 0); // décale la géométrie vers la droite
const door = new THREE.Mesh(
  doorGeo,
  new THREE.MeshStandardMaterial({ color: 0x553311 })
);
door.position.set(-0.3, 0.5, 1.01);
door.castShadow = true;
houseGroup.add(door);

// Ajout dans la scène
houseGroup.position.set(0, 0, -3);
scene.add(houseGroup);

// ===============================
// Arbre
// ===============================
const trunk = new THREE.Mesh(
  new THREE.CylinderGeometry(0.2, 0.2, 1.5, 8),
  new THREE.MeshStandardMaterial({ color: 0x8b4513 })
);
trunk.position.y = 0.75;
trunk.castShadow = true;

const foliage = new THREE.Mesh(
  new THREE.SphereGeometry(1, 16, 16),
  new THREE.MeshStandardMaterial({ color: 0x228833 })
);
foliage.position.y = 2;
foliage.castShadow = true;

const tree = new THREE.Group();
tree.add(trunk);
tree.add(foliage);
tree.position.set(3, 0, 2);
scene.add(tree);

// Duplication de l'arbre
const tree2 = tree.clone()
tree2.position.set(-3, 0, 2);
scene.add(tree2)

// ===============================
// Chargement du personnage GLTF
// ===============================
const mixers = []; // stocke les "mixers" (objets qui gèrent les animations)
let mixer;
let animations = [];
let currentAction;
let currentAnimIndex = 0;
let mainCharacter;

const loader = new GLTFLoader();
loader.load(
  "assets/Characters/gltf/knight.glb",
  (gltf) => {
    mainCharacter = gltf.scene;
    mainCharacter.scale.set(0.4, 0.4, 0.4);
    mainCharacter.position.set(0, 0, 0);

    // Active les ombres sur tous les sous-objets du modèle
    mainCharacter.traverse((child) => {
      if (child.isMesh) {
        child.castShadow = true;
        child.receiveShadow = true;
      }
    });
    scene.add(mainCharacter);

    // Mixer = objet qui synchronise le temps et l'animation du modèle
    mixer = new THREE.AnimationMixer(mainCharacter);
    mixers.push(mixer);

    // Récupère la liste des animations intégrées au modèle
    animations = gltf.animations;
    console.log("Nbr d'animations :", animations.length);

    if (animations.length > 0) {
      playAnimation(0); // Lance la première animation
    }
  },
  undefined,
  (error) => console.error("Erreur GLTFLoader:", error)
);

// --- Fonction pour changer d’animation ---
function playAnimation(index) {
  if (!animations.length) return;

  // Arrête l’animation en cours
  if (currentAction) currentAction.stop();

  // Calcul d’un index valide (modulo = boucle circulaire)
  currentAnimIndex = (index + animations.length) % animations.length;
  const clip = animations[currentAnimIndex];

  // Joue la nouvelle animation
  currentAction = mixer.clipAction(clip);
  currentAction.reset().play();
  console.log("Animation jouée :", clip.name);
}

// ===============================
// Soleil + Nuage animé
// ===============================
const sun = new THREE.Mesh(
  new THREE.SphereGeometry(0.5, 32, 32),
  new THREE.MeshStandardMaterial({ emissive: 0xffdd33 }) // émet de la lumière
);
sun.position.set(5, 5, 0);
scene.add(sun);

// Nuage
const cloud = new THREE.Mesh(
  new THREE.BoxGeometry(2, 0.5, 1),
  new THREE.MeshStandardMaterial({ color: 0xffffff })
);
cloud.position.set(-8, 3, 0);
cloud.castShadow = true;
scene.add(cloud);

// GSAP : anime le nuage d’un côté à l’autre
// → GSAP = GreenSock Animation Platform (librairie d’animation fluide)
gsap.to(cloud.position, {
  x: 8,             // direction
  duration: 8,      // durée en secondes
  repeat: -1,       // infini
  yoyo: true,       // revient en arrière
  ease: "sine.inOut" // courbe d’animation douce
});

// ===============================
// Interaction avec la porte (Raycaster)
// ===============================
// Le "raycaster" simule un rayon depuis la caméra vers la souris
// pour détecter les objets cliqués
const raycaster = new THREE.Raycaster();
const mouse = new THREE.Vector2();
let doorOpen = false;

window.addEventListener("click", (event) => {
  // Conversion des coordonnées souris -> coordonnées 3D normalisées
  mouse.x = (event.clientX / window.innerWidth) * 2 - 1;
  mouse.y = -(event.clientY / window.innerHeight) * 2 + 1;

  // Tire un rayon depuis la caméra vers le point cliqué
  raycaster.setFromCamera(mouse, camera);
  const intersects = raycaster.intersectObjects([door]);

  if (intersects.length > 0) {
    // Ouvre ou ferme la porte
    gsap.to(door.rotation, { y: doorOpen ? 0 : -Math.PI / 2, duration: 1 });
    doorOpen = !doorOpen;
  }
});

// ===============================
// Changement d’animation clavier
// ===============================
window.addEventListener("keydown", (event) => {
  if (event.key === "ArrowRight") playAnimation(currentAnimIndex + 1);
  if (event.key === "ArrowLeft") playAnimation(currentAnimIndex - 1);
});

// ===============================
// TEXTE D’INSTRUCTIONS
// ===============================
const instructions = document.createElement("div");
instructions.innerHTML = `
  <p style="
    position: absolute;
    top: 20px;
    left: 50%;
    transform: translateX(-50%);
    color: white;
    background: rgba(0,0,0,0.5);
    padding: 10px 20px;
    border-radius: 12px;
    font-family: 'Arial', sans-serif;
    font-size: 16px;
    text-align: center;
  ">
    Cliquez sur la porte pour l’ouvrir ou la fermer<br>
    Utilisez les flèches gauche/droite pour changer d’animation
  </p>
`;
document.body.appendChild(instructions);

// ===============================
// Boucle d’animation
// ===============================
function animate() {
  requestAnimationFrame(animate); // rappelle la fonction à chaque frame (~60fps)

  const delta = clock.getDelta(); // temps écoulé entre 2 frames
  mixers.forEach((mixer) => mixer.update(delta)); // mise à jour fluide des anims

  // Mouvement du soleil (rotation autour de la scène)
  const time = Date.now() * 0.001;
  sun.position.x = Math.cos(time) * 6;
  sun.position.z = Math.sin(time) * 6;
  dirLight.position.copy(sun.position); // la lumière suit le soleil

  controls.update();
  renderer.render(scene, camera);
}

animate();
