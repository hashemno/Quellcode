using UnityEngine;
using Player;

namespace Character
{
    public class CharacterEditor : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer hairRenderer;  
        [SerializeField] private SkinnedMeshRenderer topRenderer;  
        [SerializeField] private SkinnedMeshRenderer pantsRenderer; 
        [SerializeField] private SkinnedMeshRenderer shoesRenderer; 

        [SerializeField] private Mesh[] hairMeshes; 
        [SerializeField] private Material[] hairMaterials; 

        [SerializeField] private Mesh[] topMeshes; 
        [SerializeField] private Material[] topMaterials; 

        [SerializeField] private Mesh[] pantsMeshes; 
        [SerializeField] private Material[] pantsMaterials; 

        [SerializeField] private Mesh[] shoesMeshes; 
        [SerializeField] private Material[] shoesMaterials; 

        // Indices for the current clothing selection
        private int currentHairIndex = 0; 
        private int currentTopIndex = 0; 
        private int currentPantsIndex = 0;
        private int currentShoesIndex = 0;

        private Player.PlayerMovement _playerMovement;

        void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            _playerMovement = player.GetComponent<Player.PlayerMovement>();
        }

        void Update()
        {
            if(_playerMovement.getIsMovementEnabled())
            {
                getInputs();
            }
        }

        // Inputs to change clothing or take a screenshot
        private void getInputs()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                ChangeToNextTop();
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                ChangeToNextPants();
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                ChangeToNextHair();
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                ChangeToNextShoes();
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                ScreenCapture.CaptureScreenshot("screenshot.png");
                Debug.Log("A screenshot was taken!");
            }
        }

        public void ChangeToNextHair()
        {
            if (hairMeshes == null || hairMaterials == null || hairMeshes.Length == 0 || hairMaterials.Length == 0)
            {
                Debug.LogError("Hair meshes or materials not assigned.");
                return;
            }

            // Cycle through hair meshes
            currentHairIndex = (currentHairIndex + 1) % hairMeshes.Length;
            Debug.Log($"New HairIndex: {currentHairIndex}");

            // Apply the selected hair mesh and material
            ApplyHair(hairMeshes[currentHairIndex], hairMaterials[currentHairIndex]);
        }

        public void ChangeToNextTop()
        {
            Debug.Log("ChangeToNextTop called");
            Debug.Log($"Current TopIndex: {currentTopIndex}, TopMeshes Length: {topMeshes.Length}");

            currentTopIndex = (currentTopIndex + 1) % topMeshes.Length;
            Debug.Log($"New TopIndex: {currentTopIndex}");

            ApplyOutfit(topMeshes[currentTopIndex], topMaterials[currentTopIndex], topRenderer);
        }

        public void ChangeToNextPants()
        {
            Debug.Log("ChangeToNextPants called");
            Debug.Log($"Current PantsIndex: {currentPantsIndex}, PantsMeshes Length: {pantsMeshes.Length}");

            currentPantsIndex = (currentPantsIndex + 1) % pantsMeshes.Length;
            Debug.Log($"New PantsIndex: {currentPantsIndex}");

            ApplyOutfit(pantsMeshes[currentPantsIndex], pantsMaterials[currentPantsIndex], pantsRenderer);
        }

        public void ChangeToNextShoes()
        {
            Debug.Log("ChangeToNextShoes called");
            Debug.Log($"Current ShoesIndex: {currentShoesIndex}, ShoesMeshes Length: {shoesMeshes.Length}");

            currentShoesIndex = (currentShoesIndex + 1) % shoesMeshes.Length;
            Debug.Log($"New ShoesIndex: {currentShoesIndex}");

            ApplyOutfit(shoesMeshes[currentShoesIndex], shoesMaterials[currentShoesIndex], shoesRenderer);
        }

        private void ApplyOutfit(Mesh mesh, Material material, SkinnedMeshRenderer renderer)
        {
            // Check if the SkinnedMeshRenderer is assigned
            if (renderer == null)
            {
                Debug.LogError("SkinnedMeshRenderer not assigned.");
                return;
            }

            // Set the mesh and material for the SkinnedMeshRenderer
            renderer.sharedMesh = mesh;
            renderer.material = material;
        }

        private void ApplyHair(Mesh mesh, Material material)
        {
            if (hairRenderer == null)
            {
                Debug.LogError("No SkinnedMeshRenderer assigned for the hair.");
                return;
            }

            // Set both the mesh and the material for the hair
            hairRenderer.sharedMesh = mesh;
            hairRenderer.material = material;
        }

        public Mesh[] GetHairMeshes()
        {
            return hairMeshes;
        }

        public void SetHairMeshes(Mesh[] meshes)
        {
            hairMeshes = meshes;
        }

        public Mesh[] GetTopMeshes()
        {
            return topMeshes;
        }

        public void SetTopMeshes(Mesh[] meshes)
        {
            topMeshes = meshes;
        }

        public Mesh[] GetPantsMeshes()
        {
            return pantsMeshes;
        }

        public void SetPantsMeshes(Mesh[] meshes)
        {
            pantsMeshes = meshes;
        }

        public Mesh[] GetShoesMeshes()
        {
            return shoesMeshes;
        }

        public void SetShoesMeshes(Mesh[] meshes)
        {
            shoesMeshes = meshes;
        }

        public int GetCurrentHairIndex()
        {
            return currentHairIndex;
        }
    }
}
